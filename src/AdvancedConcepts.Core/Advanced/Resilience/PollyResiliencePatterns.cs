using Polly;
using Polly.CircuitBreaker;
using Polly.Retry;
using Polly.Timeout;

namespace AdvancedCsharpConcepts.Advanced.Resilience;

/// <summary>
/// Polly Resilience Patterns - Enterprise-grade error handling.
/// NVIDIA/Silicon Valley best practice: Fault tolerance and graceful degradation.
/// </summary>
public static class PollyResiliencePatterns
{
    /// <summary>
    /// Retry Policy - Retry failed operations with exponential backoff.
    /// </summary>
    public static async Task<string> RetryPolicyExample()
    {
        var retryPipeline = new ResiliencePipelineBuilder()
            .AddRetry(new RetryStrategyOptions
            {
                MaxRetryAttempts = 3,
                Delay = TimeSpan.FromSeconds(1),
                BackoffType = DelayBackoffType.Exponential,
                OnRetry = args =>
                {
                    Console.WriteLine($"Retry #{args.AttemptNumber} after {args.RetryDelay}");
                    return ValueTask.CompletedTask;
                }
            })
            .Build();

        var result = await retryPipeline.ExecuteAsync(async token =>
        {
            // Simulate unreliable operation
            if (Random.Shared.Next(0, 2) == 0)
            {
                throw new HttpRequestException("Simulated network error");
            }

            await Task.Delay(100, token);
            return "Success!";
        });

        return result;
    }

    /// <summary>
    /// Circuit Breaker - Prevent cascading failures.
    /// Opens circuit after consecutive failures, then allows test requests.
    /// </summary>
    public static class CircuitBreakerExample
    {
        private static readonly ResiliencePipeline Pipeline = new ResiliencePipelineBuilder()
            .AddCircuitBreaker(new CircuitBreakerStrategyOptions
            {
                FailureRatio = 0.5, // Break if 50% fail
                SamplingDuration = TimeSpan.FromSeconds(10),
                MinimumThroughput = 3,
                BreakDuration = TimeSpan.FromSeconds(30),
                OnOpened = args =>
                {
                    Console.WriteLine($"Circuit opened at {DateTime.Now}");
                    return ValueTask.CompletedTask;
                },
                OnClosed = args =>
                {
                    Console.WriteLine($"Circuit closed at {DateTime.Now}");
                    return ValueTask.CompletedTask;
                },
                OnHalfOpened = args =>
                {
                    Console.WriteLine($"Circuit half-opened at {DateTime.Now}");
                    return ValueTask.CompletedTask;
                }
            })
            .Build();

        public static async Task<string> ExecuteWithCircuitBreaker(Func<Task<string>> operation)
        {
            try
            {
                return await Pipeline.ExecuteAsync(async token => await operation());
            }
            catch (BrokenCircuitException)
            {
                return "Circuit is open - service unavailable";
            }
        }
    }

    /// <summary>
    /// Timeout Policy - Prevent operations from hanging indefinitely.
    /// </summary>
    public static async Task<string> TimeoutPolicyExample()
    {
        var timeoutPipeline = new ResiliencePipelineBuilder()
            .AddTimeout(new TimeoutStrategyOptions
            {
                Timeout = TimeSpan.FromSeconds(2),
                OnTimeout = args =>
                {
                    Console.WriteLine($"Operation timed out after {args.Timeout}");
                    return ValueTask.CompletedTask;
                }
            })
            .Build();

        try
        {
            return await timeoutPipeline.ExecuteAsync(async token =>
            {
                // Simulate slow operation
                await Task.Delay(TimeSpan.FromSeconds(5), token);
                return "Completed";
            });
        }
        catch (TimeoutRejectedException)
        {
            return "Operation timed out";
        }
    }

    /// <summary>
    /// Combined Policies - Retry + Circuit Breaker + Timeout.
    /// NVIDIA-style: Layered resilience for production systems.
    /// </summary>
    public static class CombinedPoliciesExample
    {
        private static readonly ResiliencePipeline<string> Pipeline = new ResiliencePipelineBuilder<string>()
            // Outermost: Timeout (2 seconds max)
            .AddTimeout(TimeSpan.FromSeconds(2))
            // Middle: Circuit Breaker
            .AddCircuitBreaker(new CircuitBreakerStrategyOptions<string>
            {
                FailureRatio = 0.5,
                SamplingDuration = TimeSpan.FromSeconds(10),
                MinimumThroughput = 2,
                BreakDuration = TimeSpan.FromSeconds(5)
            })
            // Innermost: Retry (3 attempts with exponential backoff)
            .AddRetry(new RetryStrategyOptions<string>
            {
                MaxRetryAttempts = 3,
                Delay = TimeSpan.FromMilliseconds(500),
                BackoffType = DelayBackoffType.Exponential
            })
            .Build();

        public static async Task<string> ExecuteResilientOperation(Func<CancellationToken, Task<string>> operation)
        {
            try
            {
                return await Pipeline.ExecuteAsync(async token =>
                {
                    var result = await operation(token);
                    return result;
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"All resilience strategies exhausted: {ex.Message}");
                return "Fallback: Service temporarily unavailable";
            }
        }
    }

    /// <summary>
    /// Fallback Pattern - Manual fallback when operation fails.
    /// </summary>
    public static async Task<string> FallbackPatternExample()
    {
        try
        {
            // Try primary operation
            await Task.Delay(100);
            if (Random.Shared.Next(0, 2) == 0)
                throw new Exception("Primary data source unavailable");

            return "Data from primary source";
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Primary failed: {ex.Message}, using fallback");
            return "Fallback data from cache";
        }
    }

    /// <summary>
    /// Demonstrates all Polly resilience patterns.
    /// </summary>
    public static async Task RunExamples()
    {
        Console.WriteLine("=== Polly Resilience Patterns ===\n");

        // 1. Retry Policy
        Console.WriteLine("1. Retry Policy:");
        try
        {
            var retryResult = await RetryPolicyExample();
            Console.WriteLine($"Result: {retryResult}\n");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed after retries: {ex.Message}\n");
        }

        // 2. Timeout Policy
        Console.WriteLine("2. Timeout Policy:");
        var timeoutResult = await TimeoutPolicyExample();
        Console.WriteLine($"Result: {timeoutResult}\n");

        // 3. Fallback Pattern
        Console.WriteLine("3. Fallback Pattern:");
        var fallbackResult = await FallbackPatternExample();
        Console.WriteLine($"Result: {fallbackResult}\n");

        // 4. Combined Policies
        Console.WriteLine("4. Combined Policies (Retry + Circuit Breaker + Timeout):");
        var combinedResult = await CombinedPoliciesExample.ExecuteResilientOperation(async token =>
        {
            await Task.Delay(100, token);
            return "Operation successful";
        });
        Console.WriteLine($"Result: {combinedResult}\n");

        Console.WriteLine("All Polly patterns demonstrated successfully!");
    }
}
