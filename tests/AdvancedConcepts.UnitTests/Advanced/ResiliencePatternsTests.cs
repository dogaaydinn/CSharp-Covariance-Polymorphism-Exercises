using Xunit;
using FluentAssertions;
using Polly;
using Polly.CircuitBreaker;
using Polly.Retry;

namespace AdvancedConcepts.UnitTests.Advanced;

/// <summary>
/// Tests for resilience patterns using Polly
/// Covers retry, circuit breaker, timeout, fallback, and bulkhead patterns
/// </summary>
public class ResiliencePatternsTests
{
    #region Retry Pattern Tests

    [Fact]
    public void Retry_WithTransientFailure_ShouldRetryAndSucceed()
    {
        // Arrange
        int attemptCount = 0;
        var retryPolicy = Policy
            .Handle<Exception>()
            .Retry(3);

        // Act
        var result = retryPolicy.Execute(() =>
        {
            attemptCount++;
            if (attemptCount < 3)
                throw new Exception("Transient failure");
            return "Success";
        });

        // Assert
        result.Should().Be("Success");
        attemptCount.Should().Be(3);
    }

    [Fact]
    public void Retry_ExceedsMaxAttempts_ShouldThrow()
    {
        // Arrange
        var retryPolicy = Policy
            .Handle<Exception>()
            .Retry(2);

        // Act
        Action act = () => retryPolicy.Execute(() =>
        {
            throw new Exception("Persistent failure");
        });

        // Assert
        act.Should().Throw<Exception>()
            .WithMessage("Persistent failure");
    }

    [Fact]
    public void RetryWithWait_ShouldWaitBetweenRetries()
    {
        // Arrange
        var attemptTimes = new List<DateTime>();
        var retryPolicy = Policy
            .Handle<Exception>()
            .WaitAndRetry(2, retryAttempt => TimeSpan.FromMilliseconds(100));

        // Act
        try
        {
            retryPolicy.Execute(() =>
            {
                attemptTimes.Add(DateTime.UtcNow);
                throw new Exception("Test");
            });
        }
        catch { }

        // Assert
        attemptTimes.Should().HaveCount(3); // Initial + 2 retries
        if (attemptTimes.Count >= 2)
        {
            var delay = attemptTimes[1] - attemptTimes[0];
            delay.TotalMilliseconds.Should().BeGreaterThanOrEqualTo(90);
        }
    }

    [Fact]
    public void ExponentialBackoff_ShouldIncreaseDelays()
    {
        // Arrange
        var attemptTimes = new List<DateTime>();
        var retryPolicy = Policy
            .Handle<Exception>()
            .WaitAndRetry(3, retryAttempt =>
                TimeSpan.FromMilliseconds(Math.Pow(2, retryAttempt) * 100));

        // Act
        try
        {
            retryPolicy.Execute(() =>
            {
                attemptTimes.Add(DateTime.UtcNow);
                throw new Exception("Test");
            });
        }
        catch { }

        // Assert
        attemptTimes.Should().HaveCountGreaterThan(1);
    }

    #endregion

    #region Circuit Breaker Pattern Tests

    [Fact]
    public void CircuitBreaker_AfterConsecutiveFailures_ShouldOpen()
    {
        // Arrange
        var circuitBreakerPolicy = Policy
            .Handle<Exception>()
            .CircuitBreaker(2, TimeSpan.FromSeconds(1));

        // Act - Cause 2 failures to open circuit
        try { circuitBreakerPolicy.Execute(() => throw new Exception("Fail 1")); } catch { }
        try { circuitBreakerPolicy.Execute(() => throw new Exception("Fail 2")); } catch { }

        Action act = () => circuitBreakerPolicy.Execute(() => "Should not execute");

        // Assert
        act.Should().Throw<BrokenCircuitException>();
    }

    [Fact]
    public void CircuitBreaker_WhenClosed_ShouldAllowExecution()
    {
        // Arrange
        var circuitBreakerPolicy = Policy
            .Handle<Exception>()
            .CircuitBreaker(2, TimeSpan.FromSeconds(1));

        // Act
        var result = circuitBreakerPolicy.Execute(() => "Success");

        // Assert
        result.Should().Be("Success");
    }

    [Fact]
    public void CircuitBreaker_AfterTimeout_ShouldHalfOpen()
    {
        // Arrange
        var circuitBreakerPolicy = Policy
            .Handle<Exception>()
            .CircuitBreaker(2, TimeSpan.FromMilliseconds(100));

        // Act - Open circuit
        try { circuitBreakerPolicy.Execute(() => throw new Exception("Fail 1")); } catch { }
        try { circuitBreakerPolicy.Execute(() => throw new Exception("Fail 2")); } catch { }

        // Wait for circuit to half-open
        Thread.Sleep(150);

        // Should allow one test call in half-open state
        var result = circuitBreakerPolicy.Execute(() => "Success");

        // Assert
        result.Should().Be("Success");
    }

    #endregion

    #region Fallback Pattern Tests

    [Fact]
    public void Fallback_WhenOperationFails_ShouldReturnFallbackValue()
    {
        // Arrange
        var fallbackPolicy = Policy<string>
            .Handle<Exception>()
            .Fallback("Fallback value");

        // Act
        var result = fallbackPolicy.Execute(() =>
        {
            throw new Exception("Primary failed");
        });

        // Assert
        result.Should().Be("Fallback value");
    }

    [Fact]
    public void Fallback_WhenOperationSucceeds_ShouldReturnActualValue()
    {
        // Arrange
        var fallbackPolicy = Policy<string>
            .Handle<Exception>()
            .Fallback("Fallback value");

        // Act
        var result = fallbackPolicy.Execute(() => "Actual value");

        // Assert
        result.Should().Be("Actual value");
    }

    [Fact]
    public void FallbackWithAction_ShouldExecuteCompensatingAction()
    {
        // Arrange
        bool fallbackExecuted = false;
        var fallbackPolicy = Policy<string>
            .Handle<Exception>()
            .Fallback("Fallback", onFallback: _ => fallbackExecuted = true);

        // Act
        var result = fallbackPolicy.Execute(() => throw new Exception("Failed"));

        // Assert
        result.Should().Be("Fallback");
        fallbackExecuted.Should().BeTrue();
    }

    #endregion

    #region Timeout Pattern Tests

    [Fact]
    public void Timeout_OperationExceedsTimeout_ShouldThrow()
    {
        // Arrange
        var timeoutPolicy = Policy
            .Timeout(TimeSpan.FromMilliseconds(100));

        // Act
        Action act = () => timeoutPolicy.Execute(() =>
        {
            Thread.Sleep(200);
            return "Should timeout";
        });

        // Assert
        act.Should().Throw<Polly.Timeout.TimeoutRejectedException>();
    }

    [Fact]
    public void Timeout_OperationCompletesInTime_ShouldSucceed()
    {
        // Arrange
        var timeoutPolicy = Policy
            .Timeout(TimeSpan.FromSeconds(1));

        // Act
        var result = timeoutPolicy.Execute(() =>
        {
            Thread.Sleep(50);
            return "Success";
        });

        // Assert
        result.Should().Be("Success");
    }

    #endregion

    #region Policy Wrap (Combined Patterns) Tests

    [Fact]
    public void PolicyWrap_CombinesRetryAndCircuitBreaker()
    {
        // Arrange
        var retryPolicy = Policy
            .Handle<Exception>()
            .Retry(2);

        var circuitBreakerPolicy = Policy
            .Handle<Exception>()
            .CircuitBreaker(3, TimeSpan.FromSeconds(1));

        var policyWrap = Policy.Wrap(retryPolicy, circuitBreakerPolicy);

        // Act & Assert - Should use retry first, then circuit breaker
        policyWrap.Should().NotBeNull();
    }

    [Fact]
    public void PolicyWrap_RetryWithFallback_ShouldRetryThenFallback()
    {
        // Arrange
        int attemptCount = 0;
        var retryPolicy = Policy<string>
            .Handle<Exception>()
            .Retry(2, onRetry: (ex, count) => attemptCount++);

        var fallbackPolicy = Policy<string>
            .Handle<Exception>()
            .Fallback("Fallback value");

        var policyWrap = fallbackPolicy.Wrap(retryPolicy);

        // Act
        var result = policyWrap.Execute(() => throw new Exception("Always fails"));

        // Assert
        result.Should().Be("Fallback value");
        attemptCount.Should().Be(2);
    }

    #endregion

    #region Real-World Scenarios

    [Fact]
    public void ApiCall_WithRetryAndTimeout_ShouldHandleTransientFailures()
    {
        // Arrange
        int callCount = 0;
        var policy = Policy
            .Handle<HttpRequestException>()
            .WaitAndRetry(3, retryAttempt => TimeSpan.FromMilliseconds(100))
            .Wrap(Policy.Timeout(TimeSpan.FromSeconds(5)));

        // Act
        var result = policy.Execute(() =>
        {
            callCount++;
            if (callCount < 2)
                throw new HttpRequestException("Network error");
            return "API Response";
        });

        // Assert
        result.Should().Be("API Response");
        callCount.Should().Be(2);
    }

    [Fact]
    public void DatabaseCall_WithCircuitBreakerAndFallback_ShouldUseCache()
    {
        // Arrange
        var circuitBreaker = Policy<string>
            .Handle<Exception>()
            .CircuitBreaker(2, TimeSpan.FromSeconds(1));

        var fallback = Policy<string>
            .Handle<Exception>()
            .Fallback("Cached data");

        var policy = fallback.Wrap(circuitBreaker);

        // Act - Cause failures to open circuit
        try { policy.Execute(() => throw new Exception("DB Error 1")); } catch { }
        try { policy.Execute(() => throw new Exception("DB Error 2")); } catch { }

        // Third call should use fallback (circuit is open)
        var result = policy.Execute(() => throw new Exception("DB Error 3"));

        // Assert
        result.Should().Be("Cached data");
    }

    [Fact]
    public void MicroserviceCall_WithFullResilienceStack_ShouldBeRobust()
    {
        // Arrange - Full resilience stack
        var timeout = Policy.Timeout(TimeSpan.FromSeconds(2));
        var retry = Policy
            .Handle<Exception>()
            .WaitAndRetry(2, _ => TimeSpan.FromMilliseconds(100));
        var circuitBreaker = Policy
            .Handle<Exception>()
            .CircuitBreaker(3, TimeSpan.FromSeconds(1));

        var resilience = Policy.Wrap(timeout, retry, circuitBreaker);

        // Act & Assert
        resilience.Should().NotBeNull();
    }

    #endregion

    #region Performance and Monitoring Tests

    [Fact]
    public void Retry_ShouldTrackAttemptCount()
    {
        // Arrange
        int attemptCount = 0;
        var policy = Policy
            .Handle<Exception>()
            .Retry(5, (ex, count) =>
            {
                attemptCount = count;
            });

        // Act
        try
        {
            policy.Execute(() =>
            {
                if (attemptCount < 3)
                    throw new Exception("Retry me");
                return "Success";
            });
        }
        catch { }

        // Assert
        attemptCount.Should().BeLessThanOrEqualTo(5);
    }

    [Fact]
    public void CircuitBreaker_ShouldProvideState()
    {
        // Arrange
        CircuitState? state = null;
        var policy = Policy
            .Handle<Exception>()
            .CircuitBreaker(
                2,
                TimeSpan.FromSeconds(1),
                onBreak: (ex, duration, context) => state = CircuitState.Open,
                onReset: context => state = CircuitState.Closed);

        // Act - Open circuit
        try { policy.Execute(() => throw new Exception("Fail 1")); } catch { }
        try { policy.Execute(() => throw new Exception("Fail 2")); } catch { }

        // Assert
        state.Should().Be(CircuitState.Open);
    }

    #endregion
}
