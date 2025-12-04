using Polly;
using Polly.CircuitBreaker;
using Polly.Retry;
using Polly.Timeout;

namespace ResiliencePatterns;

/// <summary>
/// Working demonstration of core Polly resilience patterns.
/// Demonstrates the 5 essential patterns with working examples.
/// </summary>
class FixedProgram
{
    static async Task Main()
    {
        Console.WriteLine("╔════════════════════════════════════════════════════════════════════╗");
        Console.WriteLine("║         RESILIENCE PATTERNS WITH POLLY - WORKING DEMO             ║");
        Console.WriteLine("╚════════════════════════════════════════════════════════════════════╝\n");

        await RunRetryPattern();
        await RunCircuitBreakerPattern();
        await RunTimeoutPattern();
        await RunFallbackPattern();
        await RunCombinedPattern();

        Console.WriteLine("\n╔════════════════════════════════════════════════════════════════════╗");
        Console.WriteLine("║                    DEMO COMPLETED                                  ║");
        Console.WriteLine("╚════════════════════════════════════════════════════════════════════╝");
    }

    static async Task RunRetryPattern()
    {
        Console.WriteLine("\n=== 1. RETRY PATTERN ===\n");

        var retryPipeline = new ResiliencePipelineBuilder()
            .AddRetry(new RetryStrategyOptions
            {
                MaxRetryAttempts = 3,
                Delay = TimeSpan.FromSeconds(1),
                BackoffType = DelayBackoffType.Exponential,
                UseJitter = true,
                OnRetry = args =>
                {
                    Console.WriteLine($"  [Retry #{args.AttemptNumber}] Delay: {args.RetryDelay.TotalMilliseconds:F0}ms");
                    return ValueTask.CompletedTask;
                }
            })
            .Build();

        int attempts = 0;
        try
        {
            await retryPipeline.ExecuteAsync(async token =>
            {
                attempts++;
                Console.WriteLine($"  [Attempt {attempts}] Calling service...");
                if (attempts < 3) throw new HttpRequestException("Network error");
                Console.WriteLine("  [Success] Service call completed");
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"  [Failed] {ex.Message}");
        }
    }

    static async Task RunCircuitBreakerPattern()
    {
        Console.WriteLine("\n=== 2. CIRCUIT BREAKER PATTERN ===\n");

        var circuitBreaker = new ResiliencePipelineBuilder()
            .AddCircuitBreaker(new CircuitBreakerStrategyOptions
            {
                FailureRatio = 0.5,
                MinimumThroughput = 3,
                BreakDuration = TimeSpan.FromSeconds(5),
                OnOpened = args =>
                {
                    Console.WriteLine("  [CIRCUIT OPENED] Too many failures");
                    return ValueTask.CompletedTask;
                },
                OnClosed = args =>
                {
                    Console.WriteLine("  [CIRCUIT CLOSED] Service recovered");
                    return ValueTask.CompletedTask;
                }
            })
            .Build();

        for (int i = 1; i <= 8; i++)
        {
            Console.WriteLine($"\nCall #{i}:");
            try
            {
                await circuitBreaker.ExecuteAsync(async token =>
                {
                    if (i <= 4) throw new Exception("Service down");
                    Console.WriteLine("  [Success] Call completed");
                });
            }
            catch (BrokenCircuitException)
            {
                Console.WriteLine("  [REJECTED] Circuit is open");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  [Failed] {ex.Message}");
            }
            await Task.Delay(i <= 5 ? 500 : 6000);
        }
    }

    static async Task RunTimeoutPattern()
    {
        Console.WriteLine("\n=== 3. TIMEOUT PATTERN ===\n");

        var timeoutPipeline = new ResiliencePipelineBuilder()
            .AddTimeout(TimeSpan.FromSeconds(2))
            .Build();

        // Fast operation
        Console.WriteLine("Test 1: Fast operation (1s)");
        try
        {
            await timeoutPipeline.ExecuteAsync(async token =>
            {
                await Task.Delay(1000, token);
                Console.WriteLine("  [Success] Completed in time");
            });
        }
        catch (TimeoutRejectedException)
        {
            Console.WriteLine("  [Timeout] Operation cancelled");
        }

        // Slow operation
        Console.WriteLine("\nTest 2: Slow operation (5s, timeout 2s)");
        try
        {
            await timeoutPipeline.ExecuteAsync(async token =>
            {
                await Task.Delay(5000, token);
                Console.WriteLine("  [Success] This won't print");
            });
        }
        catch (TimeoutRejectedException)
        {
            Console.WriteLine("  [Timeout] Operation exceeded limit");
        }
    }

    static async Task RunFallbackPattern()
    {
        Console.WriteLine("\n=== 4. FALLBACK PATTERN ===\n");

        var fallbackPipeline = new ResiliencePipelineBuilder<string>()
            .AddFallback(new Polly.Fallback.FallbackStrategyOptions<string>
            {
                ShouldHandle = new PredicateBuilder<string>()
                    .Handle<HttpRequestException>(),
                FallbackAction = args =>
                {
                    Console.WriteLine("  [FALLBACK] Using cached data");
                    return Outcome.FromResultAsValueTask("Cached response");
                }
            })
            .Build();

        // Success
        Console.WriteLine("Test 1: Primary service available");
        var result1 = await fallbackPipeline.ExecuteAsync(async token =>
        {
            await Task.Delay(100);
            return "Fresh data";
        });
        Console.WriteLine($"  [Result] {result1}\n");

        // Fallback
        Console.WriteLine("Test 2: Primary service unavailable");
        var result2 = await fallbackPipeline.ExecuteAsync<string>(async token =>
        {
            throw new HttpRequestException("Service down");
        });
        Console.WriteLine($"  [Result] {result2}");
    }

    static async Task RunCombinedPattern()
    {
        Console.WriteLine("\n=== 5. COMBINED PATTERNS ===\n");
        Console.WriteLine("Stack: Timeout -> Retry -> Circuit Breaker\n");

        var combinedPipeline = new ResiliencePipelineBuilder()
            .AddCircuitBreaker(new CircuitBreakerStrategyOptions
            {
                FailureRatio = 0.5,
                MinimumThroughput = 2,
                BreakDuration = TimeSpan.FromSeconds(3)
            })
            .AddRetry(new RetryStrategyOptions
            {
                MaxRetryAttempts = 2,
                Delay = TimeSpan.FromMilliseconds(500),
                OnRetry = args =>
                {
                    Console.WriteLine($"  [Retry {args.AttemptNumber}]");
                    return ValueTask.CompletedTask;
                }
            })
            .AddTimeout(TimeSpan.FromSeconds(5))
            .Build();

        int callCount = 0;
        for (int i = 1; i <= 5; i++)
        {
            Console.WriteLine($"Request #{i}:");
            try
            {
                await combinedPipeline.ExecuteAsync(async token =>
                {
                    callCount++;
                    if (callCount <= 3) throw new Exception("Transient error");
                    Console.WriteLine("  [Success] Request completed");
                });
            }
            catch (BrokenCircuitException)
            {
                Console.WriteLine("  [REJECTED] Circuit breaker open");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  [Failed] {ex.Message}");
            }
            await Task.Delay(300);
        }

        Console.WriteLine("\n═══ Key Takeaways ═══");
        Console.WriteLine("1. Retry handles transient failures automatically");
        Console.WriteLine("2. Circuit Breaker prevents cascading failures");
        Console.WriteLine("3. Timeout prevents hanging operations");
        Console.WriteLine("4. Fallback provides graceful degradation");
        Console.WriteLine("5. Combining patterns creates defense in depth");
    }
}
