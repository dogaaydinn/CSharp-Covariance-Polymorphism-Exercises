# Observability Patterns in C# - Production Monitoring

> **Level:** Advanced  
> **Prerequisites:** C# fundamentals, understanding of distributed systems  
> **Estimated Time:** 2-3 hours

## 📚 Overview

This tutorial teaches production observability patterns used in enterprise systems. Learn how to instrument your applications with logging, tracing, metrics, and health checks - the foundation of Site Reliability Engineering (SRE).

## 🎯 Learning Objectives

After completing this tutorial, you will be able to:

- ✅ Implement structured logging with Serilog
- ✅ Create distributed traces with .NET Activity API
- ✅ Collect metrics for Prometheus/Grafana
- ✅ Use correlation IDs to track requests across services
- ✅ Implement health check endpoints
- ✅ Design observable systems following SRE best practices

## 🚀 Quick Start

```bash
cd samples/03-Advanced/ObservabilityPatterns
dotnet run
```

## 📊 The Three Pillars of Observability

### 1. **Logs** - What happened?
Events that occurred in your system with context.
- Who: User ID, session ID
- What: Action performed
- When: Timestamp
- Where: Service, component
- Why: Request ID, correlation ID

### 2. **Metrics** - How much / how many?
Numerical measurements aggregated over time.
- Request rate (requests/second)
- Error rate (errors/total requests)
- Duration (P50, P95, P99 latency)
- Saturation (CPU%, memory%, queue depth)

### 3. **Traces** - Where did it go?
End-to-end request flow across services.
- Entry point → Service A → Database
- Service A → Service B → External API
- Timing for each step
- Parent-child relationships

---

## 📖 Detailed Topics

### 1. Structured Logging with Serilog

#### Why Structured Logging?

Traditional logging:
```csharp
// ❌ Bad: String interpolation
logger.LogInformation($"User {userId} logged in from {ipAddress}");
// Output: "User 12345 logged in from 192.168.1.1"
// Problem: Can't query by userId or ipAddress!
```

Structured logging:
```csharp
// ✅ Good: Named parameters
logger.LogInformation("User {UserId} logged in from {IpAddress}", userId, ipAddress);
// Output: {"UserId": 12345, "IpAddress": "192.168.1.1", "Message": "User..."}
// Benefit: Queryable! Can search all logs where UserId=12345
```

#### Serilog Features

**1. Object Destructuring**
```csharp
var user = new User { Id = 123, Name = "Alice", Email = "alice@example.com" };

// ❌ ToString(): "User { Id: 123, Name: Alice }"
Log.Information("User logged in: {User}", user);

// ✅ Destructure with @: Full object serialized
Log.Information("User logged in: {@User}", user);
// Output: {"User": {"Id": 123, "Name": "Alice", "Email": "alice@example.com"}}
```

**2. Log Context (Ambient Properties)**
```csharp
using (LogContext.PushProperty("CorrelationId", correlationId))
using (LogContext.PushProperty("UserId", userId))
{
    Log.Information("Processing request");  // Has CorrelationId & UserId
    CallServiceA();                          // Has CorrelationId & UserId
    CallServiceB();                          // Has CorrelationId & UserId
}
```

**3. Log Levels**
| Level | When to Use | Example |
|-------|-------------|---------|
| Verbose | Very noisy, dev only | "Loop iteration 42 of 1000" |
| Debug | Internal events | "Cache hit for key 'user:123'" |
| **Information** | General flow | "Request started", "Order created" |
| Warning | Unexpected but handled | "Retry attempt 2/3", "Fallback used" |
| Error | Something failed | "Payment gateway timeout" |
| Fatal | App can't continue | "Database unreachable" |

**Rule of thumb:** Production should only emit Information+ (not Debug/Verbose).

#### Configuration Example
```csharp
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .Enrich.FromLogContext()                // Enable LogContext
    .Enrich.WithMachineName()               // Add machine name
    .Enrich.WithEnvironmentName()           // Add environment (dev/prod)
    .WriteTo.Console()                      // Console sink
    .WriteTo.File("logs/app-.log", 
        rollingInterval: RollingInterval.Day)  // File sink
    .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(...))  // ELK stack
    .CreateLogger();
```

---

### 2. Distributed Tracing

#### What is Distributed Tracing?

In microservices, a single user request might touch 5+ services:
```
User Request
  → API Gateway (50ms)
    → Auth Service (20ms)
    → Order Service (100ms)
      → Inventory Service (30ms)
      → Payment Service (200ms)  ← SLOW!
    → Notification Service (10ms)
```

**Tracing tells you:**
- Total request time: 410ms
- Slowest service: Payment (200ms)
- Which service called which
- Full request path

#### .NET Activity API

.NET's built-in tracing uses `Activity` (OpenTelemetry compatible):

```csharp
// Create ActivitySource once per service
private static readonly ActivitySource Source = new("OrderService", "1.0.0");

// Start trace for incoming request
using var activity = Source.StartActivity("ProcessOrder", ActivityKind.Server);
activity?.SetTag("order.id", orderId);
activity?.SetTag("customer.id", customerId);

// Trace ID: Unique per request, same across all services
Console.WriteLine(Activity.Current?.TraceId);  // e.g., "4bf92f3577b34da6a3ce929d0e0e4736"

// Child span - database call
using (var dbSpan = Source.StartActivity("Database.Query", ActivityKind.Client))
{
    dbSpan?.SetTag("db.operation", "SELECT");
    // Database work here
    dbSpan?.SetStatus(ActivityStatusCode.Ok);
}

// Child span - API call
using (var apiSpan = Source.StartActivity("Payment.API", ActivityKind.Client))
{
    apiSpan?.SetTag("http.url", "https://payment.api/charge");
    // API call here
}
```

**Key Concepts:**
- **Trace ID**: Identifies entire request (stays same across all services)
- **Span ID**: Identifies single operation (unique per span)
- **Parent Span ID**: Links child spans to parent
- **Activity Kind**: Server (incoming), Client (outgoing), Internal, Producer, Consumer

#### Integration with Jaeger/Zipkin
```csharp
using OpenTelemetry.Trace;

var tracerProvider = Sdk.CreateTracerProviderBuilder()
    .AddSource("OrderService")
    .AddJaegerExporter(options =>
    {
        options.AgentHost = "localhost";
        options.AgentPort = 6831;
    })
    .Build();
```

---

### 3. Metrics Collection

#### Metric Types

**Counter: Monotonically Increasing**
```csharp
// Total requests (only goes up)
var requestCounter = meter.CreateCounter<long>("requests_total");
requestCounter.Add(1, new KeyValuePair<string, object?>("endpoint", "/api/orders"));

// Prometheus query: rate(requests_total[1m])  // Requests per second
```

**Histogram: Distribution of Values**
```csharp
// Request latency distribution
var requestDuration = meter.CreateHistogram<double>("request_duration_ms");
requestDuration.Record(45.2, new KeyValuePair<string, object?>("endpoint", "/api/orders"));

// Prometheus query: histogram_quantile(0.95, request_duration_ms)  // P95 latency
```

**Gauge: Current Value**
```csharp
// Current memory usage
var memoryGauge = meter.CreateObservableGauge("memory_usage_bytes", () =>
{
    return GC.GetTotalMemory(false);
});

// Prometheus query: memory_usage_bytes  // Current value
```

#### The Four Golden Signals (SRE)

1. **Latency**: How long did it take?
   ```csharp
   requestDuration.Record(stopwatch.ElapsedMilliseconds);
   ```

2. **Traffic**: How many requests?
   ```csharp
   requestCounter.Add(1);
   ```

3. **Errors**: How many failed?
   ```csharp
   errorCounter.Add(1, new KeyValuePair<string, object?>("error_type", "timeout"));
   ```

4. **Saturation**: How full is it?
   ```csharp
   queueDepthGauge.Record(currentQueueSize);
   ```

#### RED Method (Alternative)

- **Rate**: Requests per second
- **Errors**: Failed requests per second
- **Duration**: Latency (P50, P95, P99)

---

### 4. Correlation IDs

#### What is a Correlation ID?

A unique ID that follows a request through all services, logs, and traces.

**Without Correlation ID:**
```
Service A: "Processing request"
Service B: "Database query executed"
Service A: "Request completed"
Service B: "Processing request"
```
❌ Which Service B log belongs to which Service A request?

**With Correlation ID:**
```
Service A: [Correlation: abc123] "Processing request"
Service B: [Correlation: abc123] "Database query executed"
Service A: [Correlation: abc123] "Request completed"
Service B: [Correlation: def456] "Processing request"
```
✅ Clear! abc123 logs form one request, def456 is another.

#### Implementation Pattern
```csharp
// 1. Generate at entry point (API Gateway, Controller)
var correlationId = HttpContext.Request.Headers["X-Correlation-ID"].FirstOrDefault() 
                    ?? Guid.NewGuid().ToString();

// 2. Add to log context
using (LogContext.PushProperty("CorrelationId", correlationId))
{
    // All logs in this scope automatically have CorrelationId
    
    // 3. Pass to downstream services
    httpClient.DefaultRequestHeaders.Add("X-Correlation-ID", correlationId);
    
    // 4. Add to Activity for traces
    Activity.Current?.SetTag("correlation.id", correlationId);
}
```

---

### 5. Health Checks

#### Types of Health Checks

**Liveness Probe: "Is the app running?"**
- Simple check: Can the process respond?
- Fast (< 100ms)
- Failure = restart container
```csharp
// /health/live
return Ok("Alive");
```

**Readiness Probe: "Can the app serve traffic?"**
- Checks dependencies: database, cache, queues
- Moderate speed (< 1s)
- Failure = remove from load balancer (don't restart)
```csharp
// /health/ready
if (!database.CanConnect()) return ServiceUnavailable();
if (!cache.IsAvailable()) return ServiceUnavailable();
return Ok("Ready");
```

**Startup Probe: "Has the app finished starting?"**
- For slow-starting applications
- Only checked during startup
- Failure = restart container

#### Implementation
```csharp
var healthStatus = new
{
    Status = "Healthy",
    Checks = new Dictionary<string, object>
    {
        ["Database"] = CheckDatabase(),      // SELECT 1
        ["Cache"] = CheckCache(),            // PING
        ["MessageQueue"] = CheckQueue(),     // Connection check
        ["DiskSpace"] = CheckDiskSpace()     // > 10% free
    },
    Timestamp = DateTime.UtcNow
};
```

#### Kubernetes Integration
```yaml
livenessProbe:
  httpGet:
    path: /health/live
    port: 8080
  initialDelaySeconds: 30
  periodSeconds: 10
  
readinessProbe:
  httpGet:
    path: /health/ready
    port: 8080
  initialDelaySeconds: 5
  periodSeconds: 5
```

---

## 🏗️ Real-World Architecture

### Observability Stack

```
┌─────────────────────────────────────────────────────┐
│                 Your Application                     │
│  ┌──────────┐  ┌─────────┐  ┌────────────────────┐ │
│  │  Serilog │  │ Metrics │  │  ActivitySource    │ │
│  └─────┬────┘  └────┬────┘  └──────┬─────────────┘ │
└────────┼────────────┼───────────────┼───────────────┘
         │            │               │
         ▼            ▼               ▼
   ┌─────────┐  ┌──────────┐   ┌──────────┐
   │ ELK/    │  │Prometheus│   │  Jaeger/ │
   │ Splunk  │  │          │   │  Zipkin  │
   └────┬────┘  └─────┬────┘   └─────┬────┘
        │             │                │
        ▼             ▼                ▼
   ┌─────────────────────────────────────┐
   │          Grafana Dashboards         │
   └─────────────────────────────────────┘
```

**Logs:** ELK Stack (Elasticsearch, Logstash, Kibana) or Splunk  
**Metrics:** Prometheus + Grafana  
**Traces:** Jaeger or Zipkin  
**Unified:** Grafana (visualizes logs, metrics, traces together)

---

## 📊 Example Dashboard Queries

### Grafana + Prometheus

**Request Rate (RED Method)**
```promql
rate(requests_total[5m])
```

**Error Rate**
```promql
rate(requests_total{status="error"}[5m]) / rate(requests_total[5m])
```

**P95 Latency**
```promql
histogram_quantile(0.95, rate(request_duration_ms_bucket[5m]))
```

**Memory Usage**
```promql
memory_usage_bytes / 1024 / 1024  # Convert to MB
```

### ELK Stack

**Find all errors for a correlation ID:**
```json
{
  "query": {
    "bool": {
      "must": [
        { "term": { "CorrelationId": "abc-123" } },
        { "term": { "Level": "Error" } }
      ]
    }
  }
}
```

---

## ⚠️ Common Pitfalls

### Pitfall 1: Over-Logging
```csharp
// ❌ Bad: Logs in tight loop
for (int i = 0; i < 1000000; i++)
{
    Log.Debug("Processing item {Index}", i);  // 1M log entries!
}

// ✅ Good: Log summary
Log.Information("Processing {Count} items", items.Count);
ProcessItems(items);
Log.Information("Processed {Count} items", items.Count);
```

### Pitfall 2: Sensitive Data in Logs
```csharp
// ❌ Dangerous: Logging PII
Log.Information("User credentials: {@Credentials}", credentials);

// ✅ Safe: Redact sensitive fields
Log.Information("Login attempt for user {UserId}", userId);  // No password!
```

### Pitfall 3: Correlation ID Not Propagated
```csharp
// ❌ Bad: Lost correlation
var response = await httpClient.GetAsync("https://api.example.com");

// ✅ Good: Propagate correlation
httpClient.DefaultRequestHeaders.Add("X-Correlation-ID", correlationId);
var response = await httpClient.GetAsync("https://api.example.com");
```

### Pitfall 4: Slow Health Checks
```csharp
// ❌ Bad: Full database scan (slow!)
public async Task<bool> CheckDatabase()
{
    return await db.Users.CountAsync() > 0;  // Scans entire table!
}

// ✅ Good: Simple ping
public async Task<bool> CheckDatabase()
{
    return await db.Database.CanConnectAsync();  // Fast!
}
```

---

## 🎯 Best Practices Summary

### Logging
1. ✅ Use structured logging with named parameters
2. ✅ Use @ to destructure objects
3. ✅ Use LogContext for correlation IDs
4. ✅ Log at appropriate levels (Info+ in production)
5. ❌ Don't log sensitive data (passwords, tokens, PII)
6. ❌ Don't use string interpolation in logs

### Tracing
1. ✅ Create root activity for incoming requests
2. ✅ Create child activities for downstream calls
3. ✅ Add meaningful tags (user ID, order ID, etc.)
4. ✅ Set activity status (Ok, Error)
5. ✅ Use consistent trace/span naming conventions

### Metrics
1. ✅ Implement The Four Golden Signals (or RED method)
2. ✅ Add dimensions/labels for filtering
3. ✅ Use counters for totals, histograms for distributions
4. ❌ Don't create metrics for high-cardinality labels (user ID)
5. ❌ Don't track too many metrics (100s is too many)

### Health Checks
1. ✅ Separate liveness and readiness endpoints
2. ✅ Keep checks fast (< 1 second)
3. ✅ Check critical dependencies only
4. ✅ Return meaningful status information
5. ❌ Don't make health checks too complex

---

## 🔗 Related Topics

- **Intermediate:** [Covariance/Contravariance](../02-Intermediate/CovarianceContravariance/)
- **Advanced:** [Performance Optimization](../03-Advanced/PerformanceOptimization/)
- **Expert:** [Advanced Performance](../04-Expert/AdvancedPerformance/)

---

## 📚 Further Reading

### Official Documentation
- [.NET Activity API](https://docs.microsoft.com/en-us/dotnet/api/system.diagnostics.activity)
- [.NET Metrics](https://docs.microsoft.com/en-us/dotnet/core/diagnostics/metrics)
- [Serilog Documentation](https://serilog.net/)
- [OpenTelemetry .NET](https://opentelemetry.io/docs/instrumentation/net/)

### Books & Resources
- "Site Reliability Engineering" (Google SRE Book)
- "Observability Engineering" by Charity Majors
- "Distributed Tracing in Practice" by Austin Parker
- Prometheus Documentation: https://prometheus.io/docs/

### Tools
- **Logs:** ELK Stack, Splunk, Seq, Loki
- **Metrics:** Prometheus, Grafana, Datadog
- **Traces:** Jaeger, Zipkin, Honeycomb
- **All-in-One:** Grafana Cloud, New Relic, Datadog

---

## ✅ Learning Checklist

- [ ] Understand structured logging and why it's better than string interpolation
- [ ] Implement Serilog with enrichers and LogContext
- [ ] Create distributed traces with Activity API
- [ ] Understand trace/span relationships and propagation
- [ ] Implement counters, histograms, and gauges
- [ ] Apply The Four Golden Signals to your services
- [ ] Generate and propagate correlation IDs
- [ ] Implement liveness and readiness health checks
- [ ] Design an observability dashboard
- [ ] Query logs, metrics, and traces effectively

---

**Happy Observing! 🔍**

*"If you can't measure it, you can't improve it." - Peter Drucker*
