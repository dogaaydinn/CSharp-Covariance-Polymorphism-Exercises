# Path 2 - Months 7-8 Capstone: Production-Ready API with Observability

**Difficulty**: ⭐⭐⭐⭐☆ (Advanced)
**Estimated Time**: 50-60 hours
**Prerequisites**: Months 1-6 of Path 2 completed

---

## 🎯 Project Overview

Take any previous API and make it production-ready with complete observability stack: structured logging, distributed tracing, health checks, circuit breaker, retry policies, and monitoring.

### Learning Objectives

- ✅ Structured logging with Serilog
- ✅ OpenTelemetry and distributed tracing
- ✅ Health checks and readiness probes
- ✅ Circuit breaker pattern (Polly)
- ✅ Retry and timeout policies
- ✅ Monitoring dashboards
- ✅ Load testing

---

## 📋 Requirements

### 1. Structured Logging with Serilog

```csharp
// Program.cs
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .Enrich.WithMachineName()
    .Enrich.WithEnvironmentName()
    .Enrich.WithProperty("Application", "MyApi")
    .WriteTo.Console(new JsonFormatter())
    .WriteTo.Seq("http://seq:5341")
    .CreateLogger();

// Usage
_logger.LogInformation("Processing order {OrderId} for customer {CustomerId}",
    orderId, customerId);

_logger.LogWarning("Payment failed for order {OrderId}. Reason: {Reason}",
    orderId, reason);
```

### 2. OpenTelemetry Tracing

```csharp
// Program.cs
builder.Services.AddOpenTelemetry()
    .WithTracing(tracing =>
    {
        tracing
            .AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation()
            .AddEntityFrameworkCoreInstrumentation()
            .AddSource("MyApi")
            .AddJaegerExporter(options =>
            {
                options.AgentHost = "jaeger";
                options.AgentPort = 6831;
            });
    });

// Usage with custom spans
private readonly ActivitySource _activitySource = new("MyApi");

public async Task<Order> ProcessOrder(Guid orderId)
{
    using var activity = _activitySource.StartActivity("ProcessOrder");
    activity?.SetTag("order.id", orderId);

    try
    {
        var order = await _orderRepository.GetByIdAsync(orderId);
        activity?.SetTag("order.total", order.TotalAmount);

        // Process order...

        activity?.SetStatus(ActivityStatusCode.Ok);
        return order;
    }
    catch (Exception ex)
    {
        activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
        throw;
    }
}
```

### 3. Health Checks

```csharp
// Program.cs
builder.Services.AddHealthChecks()
    .AddDbContextCheck<ApplicationDbContext>("database")
    .AddRabbitMQ(rabbitConnectionString, name: "rabbitmq")
    .AddRedis(redisConnectionString, name: "redis")
    .AddUrlGroup(new Uri("https://external-api.com/health"), name: "external-api");

app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.MapHealthChecks("/health/ready", new HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("ready")
});

app.MapHealthChecks("/health/live", new HealthCheckOptions
{
    Predicate = _ => false // Always healthy if app is running
});
```

### 4. Resilience with Polly

```csharp
// Retry policy
var retryPolicy = Policy
    .Handle<HttpRequestException>()
    .Or<TimeoutException>()
    .WaitAndRetryAsync(
        retryCount: 3,
        sleepDurationProvider: attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt)),
        onRetry: (exception, timeSpan, retryCount, context) =>
        {
            _logger.LogWarning(
                "Retry {RetryCount} after {Delay}s due to: {Exception}",
                retryCount, timeSpan.TotalSeconds, exception.Message);
        });

// Circuit breaker
var circuitBreakerPolicy = Policy
    .Handle<HttpRequestException>()
    .CircuitBreakerAsync(
        handledEventsAllowedBeforeBreaking: 5,
        durationOfBreak: TimeSpan.FromSeconds(30),
        onBreak: (exception, duration) =>
        {
            _logger.LogError("Circuit breaker opened for {Duration}s", duration.TotalSeconds);
        },
        onReset: () =>
        {
            _logger.LogInformation("Circuit breaker reset");
        });

// Combine policies
var policyWrap = Policy.WrapAsync(retryPolicy, circuitBreakerPolicy);

// Usage
await policyWrap.ExecuteAsync(async () =>
{
    return await _httpClient.GetAsync("https://external-api.com/data");
});
```

### 5. Metrics and Monitoring

```csharp
using System.Diagnostics.Metrics;

public class OrderMetrics
{
    private static readonly Meter _meter = new("MyApi.Orders", "1.0.0");
    private static readonly Counter<long> _ordersCreated = _meter.CreateCounter<long>("orders.created");
    private static readonly Histogram<double> _orderProcessingDuration = _meter.CreateHistogram<double>("orders.processing.duration", "ms");
    private static readonly ObservableGauge<int> _pendingOrders = _meter.CreateObservableGauge("orders.pending", () => GetPendingOrdersCount());

    public void RecordOrderCreated(string status)
    {
        _ordersCreated.Add(1, new KeyValuePair<string, object?>("status", status));
    }

    public void RecordProcessingDuration(double durationMs, string status)
    {
        _orderProcessingDuration.Record(durationMs,
            new KeyValuePair<string, object?>("status", status));
    }

    private static int GetPendingOrdersCount()
    {
        // Query database for pending orders
        return 42;
    }
}
```

---

## 🎯 Observability Stack

### Docker Compose Setup

```yaml
version: '3.8'

services:
  api:
    build: .
    ports:
      - "5000:80"
    environment:
      - SeqUrl=http://seq:5341
      - JaegerHost=jaeger
    depends_on:
      - seq
      - jaeger
      - prometheus

  seq:
    image: datalust/seq:latest
    ports:
      - "5341:80"
    environment:
      - ACCEPT_EULA=Y

  jaeger:
    image: jaegertracing/all-in-one:latest
    ports:
      - "6831:6831/udp"
      - "16686:16686"

  prometheus:
    image: prom/prometheus:latest
    ports:
      - "9090:9090"
    volumes:
      - ./prometheus.yml:/etc/prometheus/prometheus.yml

  grafana:
    image: grafana/grafana:latest
    ports:
      - "3000:3000"
    environment:
      - GF_SECURITY_ADMIN_PASSWORD=admin
    depends_on:
      - prometheus
```

---

## 🎯 Milestones

1. **Week 1-2**: Add Serilog structured logging
2. **Week 3-4**: Implement OpenTelemetry tracing
3. **Week 5-6**: Add health checks and Polly resilience
4. **Week 7**: Set up monitoring stack (Prometheus, Grafana)
5. **Week 8**: Load testing and optimization

---

## ✅ Evaluation

| Criteria | Weight |
|----------|--------|
| Structured Logging | 20% |
| Distributed Tracing | 20% |
| Health Checks | 15% |
| Resilience Patterns | 20% |
| Monitoring Dashboard | 15% |
| Load Testing | 10% |

**Pass**: 75%

---

## 📚 Resources

- `samples/03-Advanced/Observability/`
- Serilog: https://serilog.net/
- OpenTelemetry: https://opentelemetry.io/
- Polly: https://github.com/App-vNext/Polly
- Health Checks: https://learn.microsoft.com/en-us/aspnet/core/host-and-deploy/health-checks

---

*Template Version: 1.0*
