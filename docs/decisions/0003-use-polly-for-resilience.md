# 3. Use Polly for Resilience Patterns

**Status:** ✅ Accepted

**Date:** 2024-12-01

**Deciders:** Architecture Team, DevOps Team

**Technical Story:** Implementation in `samples/03-Advanced/ResiliencePatterns`

---

## Context and Problem Statement

Distributed systems face transient failures: network timeouts, temporary service unavailability, rate limiting, etc. Without resilience patterns, cascading failures can bring down the entire system. We need to implement retry logic, circuit breakers, timeouts, and fallbacks.

**Requirements:**
- Retry transient failures with exponential backoff
- Prevent cascading failures with circuit breakers
- Enforce request timeouts
- Provide fallback mechanisms
- Monitor and log resilience events

---

## Decision Drivers

* **Industry Standard** - Use battle-tested library, not custom implementation
* **Flexibility** - Support multiple patterns (retry, circuit breaker, timeout, fallback)
* **Observability** - Log when policies execute
* **Performance** - Minimal overhead
* **Maintainability** - Clear, readable policy definitions

---

## Considered Options

* **Option 1** - Polly library
* **Option 2** - Custom resilience implementation
* **Option 3** - Cloud-provider specific solutions (AWS App Mesh, Azure Service Fabric)

---

## Decision Outcome

**Chosen option:** "Polly", because it's the de-facto standard for resilience in .NET, used by Microsoft itself in HTTP client factory, and provides comprehensive patterns out-of-the-box.

### Positive Consequences

* **Proven** - Used in production by Microsoft, Stack Overflow, and thousands of companies
* **Comprehensive** - All resilience patterns in one library
* **Integrated** - Works seamlessly with HttpClientFactory
* **Observable** - Built-in hooks for logging and monitoring
* **Well-Documented** - Extensive documentation and community support
* **Polly v8** - New resilience pipelines API is cleaner and more performant

### Negative Consequences

* **External Dependency** - Adds NuGet package
* **Learning Curve** - Team needs to understand resilience patterns
* **Configuration Complexity** - Policies need careful tuning

---

## Pros and Cons of the Options

### Polly Library (Chosen)

**Pros:**
* Battle-tested (10+ years in production)
* Used by Microsoft in .NET Core HttpClientFactory
* Supports all major resilience patterns
* Excellent async/await support
* Policy composition (retry + circuit breaker + timeout)
* Extensive telemetry hooks
* v8 redesign with improved API

**Cons:**
* External dependency
* Policies need tuning for specific use cases
* Can add latency if misconfigured

**Example:**
```csharp
// Polly v8 - Resilience Pipeline
var pipeline = new ResiliencePipelineBuilder()
    .AddRetry(new RetryStrategyOptions
    {
        MaxRetryAttempts = 3,
        Delay = TimeSpan.FromSeconds(1),
        BackoffType = DelayBackoffType.Exponential
    })
    .AddCircuitBreaker(new CircuitBreakerStrategyOptions
    {
        FailureRatio = 0.5,
        MinimumThroughput = 10,
        BreakDuration = TimeSpan.FromSeconds(30)
    })
    .AddTimeout(TimeSpan.FromSeconds(10))
    .Build();

// Execute with resilience
var result = await pipeline.ExecuteAsync(async ct =>
{
    return await httpClient.GetStringAsync(url, ct);
});
```

### Custom Resilience Implementation

**Pros:**
* No external dependencies
* Full control over implementation
* Can optimize for specific scenarios

**Cons:**
* **Requires 1000+ lines of code** for retry + circuit breaker + timeout
* Difficult to test edge cases
* Need to handle concurrent access to circuit breaker state
* No telemetry hooks
* Maintenance burden

**Why Rejected:**
```csharp
// Custom retry (simplified - doesn't handle edge cases)
public async Task<T> RetryAsync<T>(Func<Task<T>> operation, int maxRetries)
{
    for (int i = 0; i < maxRetries; i++)
    {
        try
        {
            return await operation();
        }
        catch (Exception ex)
        {
            if (i == maxRetries - 1) throw;
            await Task.Delay(TimeSpan.FromSeconds(Math.Pow(2, i))); // Exponential backoff
        }
    }
    throw new InvalidOperationException("Should never reach here");
}
```

This is **200+ lines just for basic retry**. Circuit breaker adds **500+ lines**. Polly provides all of this in a well-tested package.

### Cloud-Provider Solutions

**Examples:**
- AWS App Mesh (service mesh)
- Azure Service Fabric (runtime)
- Istio (Kubernetes service mesh)

**Pros:**
* Infrastructure-level resilience
* Language-agnostic
* Centralized configuration

**Cons:**
* **Vendor lock-in** - ties you to specific cloud provider
* Infrastructure dependency - can't test locally without complex setup
* Operational complexity - need to manage service mesh
* Overkill for many applications

**Why Rejected:**
Service meshes are excellent for large-scale microservice deployments, but they add significant operational complexity. **Polly provides application-level resilience** that works anywhere (local, any cloud, any container orchestrator) without infrastructure changes.

---

## Implementation Examples

### Retry Policy
```csharp
var retryPipeline = new ResiliencePipelineBuilder()
    .AddRetry(new RetryStrategyOptions
    {
        MaxRetryAttempts = 3,
        Delay = TimeSpan.FromSeconds(1),
        BackoffType = DelayBackoffType.Exponential,
        OnRetry = args =>
        {
            _logger.LogWarning("Retry {Attempt} after {Delay}",
                args.AttemptNumber, args.RetryDelay);
            return ValueTask.CompletedTask;
        }
    })
    .Build();
```

### Circuit Breaker
```csharp
var circuitBreakerPipeline = new ResiliencePipelineBuilder()
    .AddCircuitBreaker(new CircuitBreakerStrategyOptions
    {
        FailureRatio = 0.5,              // Open if 50% of calls fail
        SamplingDuration = TimeSpan.FromSeconds(10),
        MinimumThroughput = 10,          // Need 10 calls before evaluating
        BreakDuration = TimeSpan.FromSeconds(30),
        OnOpened = args =>
        {
            _logger.LogError("Circuit breaker OPENED");
            return ValueTask.CompletedTask;
        },
        OnHalfOpened = args =>
        {
            _logger.LogInformation("Circuit breaker HALF-OPEN (testing)");
            return ValueTask.CompletedTask;
        },
        OnClosed = args =>
        {
            _logger.LogInformation("Circuit breaker CLOSED (healthy)");
            return ValueTask.CompletedTask;
        }
    })
    .Build();
```

### Combined Policies
```csharp
var pipeline = new ResiliencePipelineBuilder()
    .AddCircuitBreaker(circuitBreakerOptions)  // First line of defense
    .AddRetry(retryOptions)                    // Retry if circuit is closed
    .AddTimeout(TimeSpan.FromSeconds(30))      // Overall timeout
    .Build();
```

---

## Integration with .NET Aspire

**.NET Aspire automatically adds resilience** to all HttpClient instances:

```csharp
// In Aspire apps, this is automatic:
builder.Services.ConfigureHttpClientDefaults(http =>
{
    http.AddStandardResilienceHandler(); // Polly retry + circuit breaker + timeout
    http.AddServiceDiscovery();
});
```

---

## Links

* [Polly Official Website](https://www.thepollyproject.org/)
* [Polly GitHub](https://github.com/App-vNext/Polly)
* [Polly v8 Migration Guide](https://www.pollydocs.org/v8/migration.html)
* [Sample Implementation](../../samples/03-Advanced/ResiliencePatterns)

---

## Notes

**Policy Configuration Guidelines:**
- **Retry**: 3 attempts with exponential backoff (1s, 2s, 4s)
- **Circuit Breaker**: 50% failure ratio, 30s break duration, 10 minimum throughput
- **Timeout**: 10-30s depending on operation (database: 10s, external API: 30s)
- **Fallback**: Always have a fallback strategy (cached data, default value, error page)

**Anti-Patterns to Avoid:**
- ❌ Retrying non-transient errors (404, 401, validation errors)
- ❌ Infinite retries
- ❌ Same timeout and retry delay (causes thundering herd)
- ❌ No circuit breaker on external dependencies

**Monitoring:**
- Log every circuit breaker state change
- Track retry counts in metrics
- Alert on high retry rates
- Monitor circuit breaker open/close events

**Review Date:** 2025-12-01
