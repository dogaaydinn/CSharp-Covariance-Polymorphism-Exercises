# Resilience Patterns with Polly

Comprehensive demonstration of resilience patterns using Polly 8.x for building robust, production-ready distributed systems.

## Overview

This sample demonstrates essential resilience patterns that every production system should implement:

- **Retry Pattern**: Automatically retry transient failures with exponential backoff
- **Circuit Breaker Pattern**: Prevent cascading failures by failing fast when services are down
- **Timeout Pattern**: Prevent operations from hanging indefinitely
- **Fallback Pattern**: Provide graceful degradation when primary operations fail
- **Combined Patterns**: Layer multiple patterns for defense in depth

## Running the Sample

```bash
# Run all patterns
dotnet run

# Run specific pattern
dotnet run retry
dotnet run circuit-breaker
dotnet run timeout
dotnet run fallback
dotnet run combined

# Show help
dotnet run --help
```

## Pattern Overview

### 1. Retry Pattern

**Purpose**: Automatically retry operations that fail due to transient errors.

**When to Use**:
- Network calls that may fail temporarily
- Database operations during brief unavailability
- External API calls with transient failures
- Message queue operations

**Key Features**:
- Exponential backoff with jitter
- Selective retry (only specific exceptions)
- Result-based retry (HTTP status codes)
- Custom delay strategies (Retry-After headers)
- Maximum retry limits

**Configuration Example**:
```csharp
var retryPipeline = new ResiliencePipelineBuilder()
    .AddRetry(new RetryStrategyOptions
    {
        MaxRetryAttempts = 3,
        Delay = TimeSpan.FromSeconds(1),
        BackoffType = DelayBackoffType.Exponential,
        UseJitter = true, // Prevent thundering herd
        ShouldHandle = new PredicateBuilder()
            .Handle<HttpRequestException>()
            .Handle<TimeoutException>()
    })
    .Build();
```

**Best Practices**:
- Always use jitter to prevent thundering herd
- Set maximum delay cap (e.g., 30 seconds)
- Only retry transient, recoverable errors
- Log retry attempts for monitoring
- Consider circuit breaker for repeated failures

**Anti-Patterns to Avoid**:
- Retrying non-transient errors (404, 401)
- No maximum delay cap (could wait hours)
- Too many retries for user-facing operations
- Not logging retry attempts

### 2. Circuit Breaker Pattern

**Purpose**: Prevent cascading failures by failing fast when a service is consistently unavailable.

**When to Use**:
- Protecting calls to external services
- Preventing system overload during outages
- Microservice-to-microservice communication
- Database connection management

**State Machine**:
```
CLOSED (Normal) ──[Too many failures]──> OPEN (Failing fast)
    ↑                                         │
    │                                         │ [Wait break duration]
    │                                         ↓
    └─────[Success]────── HALF-OPEN (Testing recovery)
```

**Configuration Example**:
```csharp
var circuitBreakerPipeline = new ResiliencePipelineBuilder()
    .AddCircuitBreaker(new CircuitBreakerStrategyOptions
    {
        FailureRatio = 0.5,           // Open after 50% failures
        MinimumThroughput = 3,        // Need at least 3 requests
        SamplingDuration = TimeSpan.FromSeconds(30),
        BreakDuration = TimeSpan.FromSeconds(60),
        OnOpened = args => {
            // Alert on-call team
            // Switch to fallback mode
        }
    })
    .Build();
```

**Configuration Guidance**:

| Traffic Level | Minimum Throughput | Failure Ratio | Break Duration |
|---------------|-------------------|---------------|----------------|
| Low           | 3-5 requests      | 0.5-0.6       | 30-60s         |
| Medium        | 10-20 requests    | 0.5           | 60s            |
| High          | 50+ requests      | 0.4-0.5       | 60-120s        |

**Best Practices**:
- One circuit breaker per dependency
- Monitor circuit state with metrics
- Provide fallback behavior when circuit opens
- Alert on-call when circuit opens
- Log state transitions with context
- Test circuit breaker behavior in load tests

### 3. Timeout Pattern

**Purpose**: Prevent operations from hanging indefinitely and blocking resources.

**When to Use**:
- All I/O operations (HTTP, database, file)
- External service calls
- User-facing operations
- Background job processing

**Configuration Examples**:

| Operation Type      | Recommended Timeout | Rationale                          |
|--------------------|---------------------|-------------------------------------|
| Cache Lookup       | 100-500ms           | In-memory, should be very fast      |
| Database Query     | 1-5 seconds         | Network + query execution           |
| HTTP API Call      | 5-30 seconds        | Network latency + processing        |
| File Upload        | 30-60 seconds       | Based on expected file size         |
| Report Generation  | 1-5 minutes         | Complex computation allowed         |
| Batch Processing   | 5-30 minutes        | Large data set processing           |

**Configuration Example**:
```csharp
var timeoutPipeline = new ResiliencePipelineBuilder()
    .AddTimeout(new TimeoutStrategyOptions
    {
        Timeout = TimeSpan.FromSeconds(5),
        OnTimeout = args => {
            // Log timeout event
            // Clean up resources
            // Increment timeout metric
        }
    })
    .Build();
```

**Dynamic Timeout Example**:
```csharp
var dynamicTimeoutPipeline = new ResiliencePipelineBuilder()
    .AddTimeout(new TimeoutStrategyOptions
    {
        TimeoutGenerator = args => {
            var operationType = args.Context.Properties
                .GetValue(new ResiliencePropertyKey<string>("OperationType"), "default");

            return ValueTask.FromResult(operationType switch {
                "quick" => TimeSpan.FromSeconds(1),
                "standard" => TimeSpan.FromSeconds(5),
                "long-running" => TimeSpan.FromSeconds(30),
                _ => TimeSpan.FromSeconds(10)
            });
        }
    })
    .Build();
```

**Best Practices**:
- Always set timeouts (never infinite waits)
- Pass and respect CancellationToken throughout call chain
- Set timeout shorter than upstream timeout (leave buffer)
- Log timeout events for monitoring and tuning
- Test timeout behavior under load
- Consider retry for transient timeout scenarios
- Clean up resources in timeout handlers

**Anti-Patterns to Avoid**:
- No timeout set (can hang forever)
- Ignoring cancellation tokens
- Timeout too long for user-facing operations
- Timeout too short (causes false timeouts under load)
- Not cleaning up on timeout
- All services in chain have same timeout (cascading)

### 4. Fallback Pattern

**Purpose**: Provide graceful degradation when primary operations fail.

**When to Use**:
- Cache fallback when live data unavailable
- Backup service when primary service fails
- Default values for non-critical data
- Degraded functionality mode

**Configuration Example**:
```csharp
var fallbackPipeline = new ResiliencePipelineBuilder<string>()
    .AddFallback(new FallbackStrategyOptions<string>
    {
        ShouldHandle = new PredicateBuilder<string>()
            .Handle<HttpRequestException>()
            .Handle<TimeoutException>(),
        FallbackAction = args => {
            // Return cached data
            // Use backup service
            // Return default value
            return Outcome.FromResultAsValueTask("Cached data");
        }
    })
    .Build();
```

**Multi-Layer Fallback**:
```
Primary API ──[Fail]──> Cache ──[Miss]──> Backup API ──[Fail]──> Default Value
```

**Fallback Strategies by Scenario**:

| Scenario              | Primary                  | Fallback                    | Considerations                           |
|-----------------------|--------------------------|-----------------------------|------------------------------------------|
| API Unavailable       | Live API call            | Cached response             | Cache TTL, stale data acceptable?        |
| Real-time Pricing     | Dynamic pricing engine   | Static base prices          | Notify user about missing promotions     |
| Recommendations       | ML-based recommendations | Popular items               | Degraded experience acceptable           |
| User Preferences      | User settings DB         | System defaults             | Log fallback usage for investigation     |
| Payment Processing    | Primary payment gateway  | Backup payment gateway      | Ensure transaction idempotency           |

**Best Practices**:
- Always notify users when in fallback mode (if user-facing)
- Log fallback activations for monitoring and alerting
- Test fallback paths regularly (chaos engineering)
- Ensure fallback is fast (don't fallback to slow operation)
- Consider multiple fallback layers
- Document degraded functionality clearly
- Monitor fallback usage rate
- Alert when fallback rate exceeds threshold (e.g., 5%)

**When NOT to Use Fallback**:

| Scenario                     | Why NOT                                      | Correct Approach              |
|------------------------------|----------------------------------------------|-------------------------------|
| Financial Transactions       | Can't approximate payment amounts            | Fail transaction, retry       |
| Authentication/Authorization | Can't allow access when auth service down    | Deny access, fail secure      |
| Critical Data Accuracy       | Can't use stale medical records              | Indicate data unavailable     |
| Compliance-Required Checks   | Can't skip fraud detection                   | Reject transaction            |

### 5. Combined Patterns

**Purpose**: Layer multiple resilience patterns for comprehensive defense in depth.

**Recommended Combinations**:

#### HTTP API Calls
```
Order: Fallback -> Circuit Breaker -> Retry -> Timeout
```

```csharp
var apiPipeline = new ResiliencePipelineBuilder<ApiResponse>()
    .AddFallback(...)       // Outermost: Return cached data
    .AddCircuitBreaker(...) // Prevent overwhelming failed service
    .AddRetry(...)          // Retry transient failures
    .AddTimeout(...)        // Innermost: Timeout individual attempts
    .Build();
```

**Rationale**:
- Timeout individual attempts
- Retry transient failures
- Circuit break on persistent failures
- Fallback to cache as last resort

#### Database Queries
```
Order: Bulkhead -> Circuit Breaker -> Retry -> Timeout
```

**Rationale**:
- Timeout queries
- Retry transient connection failures
- Circuit break to prevent connection pool exhaustion
- Bulkhead isolates database from other resources

#### Message Processing
```
Order: Circuit Breaker -> Retry -> Timeout
```

**Rationale**:
- Timeout message processing
- Retry failed messages
- Circuit break to dead letter queue

#### Low-Latency Requirements
```
Order: Fallback -> Hedging
```

```csharp
var hedgingPipeline = new ResiliencePipelineBuilder<string>()
    .AddFallback(...)  // Final fallback if all hedged requests fail
    .AddHedging(new HedgingStrategyOptions<string> {
        MaxHedgedAttempts = 2,
        Delay = TimeSpan.FromMilliseconds(500)
    })
    .Build();
```

**Rationale**:
- Send parallel requests
- Use fastest response
- Fallback if all fail

### Pattern Order Guidelines

**Correct Order** (Inner to Outer):
```
1. Timeout       - Innermost (individual operation)
2. Retry         - After timeout (retry timed-out operations)
3. Circuit Breaker - After retry (prevent retry storms)
4. Fallback      - Outermost (last resort)
5. Bulkhead      - Separate concern, wraps entire pipeline
```

**Why Order Matters**:

```csharp
// CORRECT: Timeout inside Retry
Retry -> Timeout
// Result: Each retry attempt gets fresh timeout
// Attempt 1: Times out after 2s
// Attempt 2: Fresh 2s timeout
// Total: Up to 6s (3 attempts × 2s)

// INCORRECT: Retry inside Timeout
Timeout -> Retry
// Result: All retries share one timeout
// Attempt 1: Times out after 2s
// No more attempts (timeout already expired)
```

## Real-World Examples

### Example 1: E-Commerce Order Processing

```csharp
public class OrderService
{
    private readonly ResiliencePipeline<PaymentResult> _paymentPipeline;

    public OrderService()
    {
        _paymentPipeline = new ResiliencePipelineBuilder<PaymentResult>()
            // Layer 4: Fallback to backup payment gateway
            .AddFallback(new FallbackStrategyOptions<PaymentResult> {
                FallbackAction = async args => {
                    return await _backupPaymentGateway.ProcessAsync();
                }
            })
            // Layer 3: Circuit breaker protects payment gateway
            .AddCircuitBreaker(new CircuitBreakerStrategyOptions<PaymentResult> {
                FailureRatio = 0.5,
                MinimumThroughput = 3,
                BreakDuration = TimeSpan.FromSeconds(30)
            })
            // Layer 2: Retry transient payment failures
            .AddRetry(new RetryStrategyOptions<PaymentResult> {
                MaxRetryAttempts = 2,
                Delay = TimeSpan.FromSeconds(1)
            })
            // Layer 1: Timeout payment requests
            .AddTimeout(TimeSpan.FromSeconds(10))
            .Build();
    }

    public async Task<OrderResult> ProcessOrderAsync(Order order)
    {
        var payment = await _paymentPipeline.ExecuteAsync(async token => {
            return await _primaryPaymentGateway.ProcessAsync(order, token);
        });

        // Continue order processing...
    }
}
```

### Example 2: Microservice Communication

```csharp
public class ProductCatalogClient
{
    private readonly ResiliencePipeline<Product> _pipeline;
    private readonly ICache _cache;

    public ProductCatalogClient(ICache cache)
    {
        _cache = cache;

        _pipeline = new ResiliencePipelineBuilder<Product>()
            .AddFallback(new FallbackStrategyOptions<Product> {
                FallbackAction = async args => {
                    // Return cached product data
                    var productId = args.Context.Properties
                        .GetValue(new ResiliencePropertyKey<string>("ProductId"), "");
                    return await _cache.GetAsync<Product>(productId);
                }
            })
            .AddCircuitBreaker(new CircuitBreakerStrategyOptions<Product> {
                FailureRatio = 0.6,
                MinimumThroughput = 5,
                BreakDuration = TimeSpan.FromSeconds(30),
                OnOpened = args => {
                    _logger.LogWarning("Product catalog circuit opened");
                    _metrics.IncrementCircuitBreakerOpened("product-catalog");
                }
            })
            .AddRetry(new RetryStrategyOptions<Product> {
                MaxRetryAttempts = 2,
                Delay = TimeSpan.FromMilliseconds(500),
                BackoffType = DelayBackoffType.Exponential
            })
            .AddTimeout(TimeSpan.FromSeconds(5))
            .Build();
    }

    public async Task<Product> GetProductAsync(string productId)
    {
        var context = ResilienceContextPool.Shared.Get();
        context.Properties.Set(new ResiliencePropertyKey<string>("ProductId"), productId);

        try
        {
            return await _pipeline.ExecuteAsync(async token => {
                return await _httpClient.GetAsync<Product>($"/api/products/{productId}", token);
            }, context);
        }
        finally
        {
            ResilienceContextPool.Shared.Return(context);
        }
    }
}
```

## Production Checklist

### Configuration
- [ ] All timeouts configured (no infinite waits)
- [ ] Retry counts appropriate for operation type
- [ ] Circuit breaker thresholds tuned from load tests
- [ ] Fallback behavior defined for all critical paths
- [ ] Bulkhead limits sized based on capacity planning

### Observability
- [ ] All patterns log events (retry, timeout, fallback, circuit state)
- [ ] Metrics tracked (retry rate, circuit state, fallback rate)
- [ ] Alerts configured (high retry rate, circuit opened)
- [ ] Dashboards show resilience health
- [ ] Distributed tracing includes resilience context

### Testing
- [ ] Unit tests for each pattern
- [ ] Integration tests with failure injection
- [ ] Load tests verify circuit breaker behavior
- [ ] Chaos engineering validates fallback paths
- [ ] Timeout tuning under realistic load

### Operations
- [ ] Runbook for circuit breaker opened scenario
- [ ] Procedure to manually isolate circuit
- [ ] Process to validate service recovery
- [ ] Regular review of resilience metrics
- [ ] Post-incident analysis includes resilience effectiveness

## Monitoring and Metrics

### Key Metrics to Track

**Retry Metrics**:
- Retry rate (retries per request)
- Retry attempts per operation
- Success rate after retries
- Time spent in retries

**Circuit Breaker Metrics**:
- Circuit state (closed/open/half-open)
- Time in open state
- Failure rate that triggered opening
- Recovery success rate

**Timeout Metrics**:
- Timeout rate
- P50, P95, P99 operation duration
- Operations timing out by type
- Timeout threshold tuning recommendations

**Fallback Metrics**:
- Fallback activation rate
- Fallback success rate
- Time in fallback mode
- Cache hit rate (for cache fallback)

### Sample Prometheus Metrics

```csharp
// Retry metrics
_retryCounter.WithLabels(operation, "attempt").Inc();
_retryDuration.Observe(duration);

// Circuit breaker metrics
_circuitBreakerState.WithLabels(dependency).Set(state);
_circuitBreakerOpened.WithLabels(dependency).Inc();

// Timeout metrics
_timeoutCounter.WithLabels(operation).Inc();
_operationDuration.Observe(duration);

// Fallback metrics
_fallbackCounter.WithLabels(operation, source).Inc();
_fallbackActivation.WithLabels(operation).Set(isActive ? 1 : 0);
```

## Common Patterns by Technology

### HTTP/REST APIs
```csharp
Timeout (5-30s) -> Retry (3 attempts) -> Circuit Breaker (50% @ 10 req) -> Fallback (cache)
```

### Database Queries
```csharp
Timeout (2-5s) -> Retry (5 attempts) -> Circuit Breaker (60% @ 5 req) -> Bulkhead (10 concurrent)
```

### Message Queue Processing
```csharp
Timeout (30s) -> Retry (10 attempts, exponential) -> Circuit Breaker -> Dead Letter Queue
```

### External Service Integration
```csharp
Timeout (10s) -> Hedging (2 parallel) -> Retry (2 attempts) -> Circuit Breaker -> Fallback
```

## Resources

- [Polly Documentation](https://www.pollydocs.org/)
- [Polly GitHub Repository](https://github.com/App-vNext/Polly)
- [Cloud Design Patterns](https://docs.microsoft.com/en-us/azure/architecture/patterns/)
- [Release It! (Book)](https://pragprog.com/titles/mnee2/release-it-second-edition/)

## License

This sample code is provided for educational purposes.

## Contributing

Feel free to submit issues or pull requests for improvements.
