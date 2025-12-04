# 10. Implement OpenTelemetry for Observability

**Status:** ✅ Accepted

**Date:** 2024-12-01

**Deciders:** Architecture Team, SRE Team

**Technical Story:** Implementation in `src/AdvancedConcepts.Core/Advanced/Observability/OpenTelemetryExamples.cs`

---

## Context and Problem Statement

Modern distributed systems require comprehensive observability across the **three pillars**:
1. **Traces** - Request flow across services
2. **Metrics** - Performance counters, business metrics
3. **Logs** - Event records

**Problems with traditional monitoring:**
- No correlation between logs, traces, and metrics
- Vendor lock-in (Application Insights, Datadog formats)
- Manual instrumentation required
- Different libraries for each observability type

**We need:**
- Unified observability (traces + metrics + logs)
- Distributed tracing across microservices
- Vendor-neutral format (switch from Jaeger to Zipkin easily)
- Automatic instrumentation
- Cloud-native integration (Azure, AWS, GCP)

---

## Decision Drivers

* **Vendor Neutrality** - Not locked into specific APM tool
* **Three Pillars** - Unified traces, metrics, logs
* **Automatic Instrumentation** - Capture HTTP, database, messaging automatically
* **Standards-Based** - OpenTelemetry is CNCF standard
* **Cloud Integration** - Works with all major clouds
* **Performance** - Minimal overhead (< 5% CPU)

---

## Considered Options

* **Option 1** - OpenTelemetry (.NET)
* **Option 2** - Application Insights SDK (Azure-specific)
* **Option 3** - Datadog APM (vendor-specific)
* **Option 4** - Manual instrumentation

---

## Decision Outcome

**Chosen option:** "OpenTelemetry", because it's the vendor-neutral, CNCF-backed standard that provides automatic instrumentation for traces, metrics, and logs with support for all major backends (Jaeger, Zipkin, Prometheus, Application Insights, Datadog).

### Positive Consequences

* **Vendor Neutral** - Export to any backend (Jaeger, Zipkin, Application Insights)
* **Automatic Instrumentation** - HTTP, gRPC, database, messaging tracked automatically
* **Unified Observability** - Traces, metrics, logs in one library
* **Standards-Based** - OpenTelemetry is CNCF graduated project
* **W3C Trace Context** - Distributed tracing across any technology (.NET, Java, Python)
* **.NET Aspire Integration** - Automatic setup in Aspire apps
* **Performance** - Sampling, batching, async export

### Negative Consequences

* **Complexity** - More configuration than simple logging
* **Collector Required** - Need OpenTelemetry Collector in production
* **Learning Curve** - Team needs to understand distributed tracing concepts
* **Overhead** - Small performance impact (2-5% CPU typically)

---

## Pros and Cons of the Options

### OpenTelemetry (Chosen)

**What is OpenTelemetry?**

OpenTelemetry (OTel) is an open-source observability framework that provides APIs, SDKs, and tools to instrument, generate, collect, and export telemetry data (traces, metrics, logs).

**Architecture:**
```
Your Application
    ↓ (automatic instrumentation)
OpenTelemetry SDK
    ↓ (exports)
OpenTelemetry Collector (optional)
    ↓ (sends to backends)
Jaeger / Zipkin / Prometheus / Application Insights / Datadog
```

**Pros:**
* **Vendor neutral** - Switch backends without code changes
* **Auto-instrumentation** - HTTP, database, messaging captured automatically
* **W3C standard** - traceparent header works across languages
* **Unified** - Traces, metrics, logs in one framework
* **CNCF graduated** - Industry standard backed by major vendors
* **Sampling** - Control overhead with sampling strategies

**Cons:**
* **Setup complexity** - More configuration than vendor SDKs
* **Collector management** - Production needs OTel Collector
* **Incomplete** - Some features still in beta (logs especially)

**Installation:**
```bash
dotnet add package OpenTelemetry.Extensions.Hosting
dotnet add package OpenTelemetry.Instrumentation.AspNetCore
dotnet add package OpenTelemetry.Instrumentation.Http
dotnet add package OpenTelemetry.Instrumentation.SqlClient
dotnet add package OpenTelemetry.Exporter.Console
dotnet add package OpenTelemetry.Exporter.OpenTelemetryProtocol  # OTLP exporter
```

**Basic Setup:**
```csharp
// Program.cs
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using OpenTelemetry.Metrics;

var builder = WebApplication.CreateBuilder(args);

var serviceName = "MyApiService";
var serviceVersion = "1.0.0";

// Add OpenTelemetry
builder.Services.AddOpenTelemetry()
    .ConfigureResource(resource => resource
        .AddService(serviceName, serviceVersion: serviceVersion)
        .AddAttributes(new Dictionary<string, object>
        {
            ["deployment.environment"] = builder.Environment.EnvironmentName,
            ["host.name"] = Environment.MachineName
        }))
    .WithTracing(tracing => tracing
        // Automatic instrumentation
        .AddAspNetCoreInstrumentation(options =>
        {
            options.RecordException = true;
            options.Filter = httpContext =>
            {
                // Don't trace health checks
                return !httpContext.Request.Path.StartsWithSegments("/health");
            };
        })
        .AddHttpClientInstrumentation()
        .AddSqlClientInstrumentation(options =>
        {
            options.SetDbStatementForText = true;  // Capture SQL queries
            options.RecordException = true;
        })
        // Exporters
        .AddConsoleExporter()  // Development
        .AddOtlpExporter(otlpOptions =>
        {
            // Production - send to OpenTelemetry Collector
            otlpOptions.Endpoint = new Uri("http://otel-collector:4317");
        }))
    .WithMetrics(metrics => metrics
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation()
        .AddRuntimeInstrumentation()  // GC, thread pool, etc.
        .AddMeter("MyApiService")     // Custom metrics
        .AddConsoleExporter()
        .AddOtlpExporter());

var app = builder.Build();
app.Run();
```

**.NET Aspire (Even Simpler):**
```csharp
// Aspire automatically adds OpenTelemetry!
// In ServiceDefaults/Extensions.cs (auto-generated)

public static IHostApplicationBuilder AddServiceDefaults(this IHostApplicationBuilder builder)
{
    builder.Services.AddOpenTelemetry()
        .WithMetrics(metrics => metrics
            .AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation()
            .AddRuntimeInstrumentation())
        .WithTracing(tracing => tracing
            .AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation()
            .AddSqlClientInstrumentation());

    // Aspire Dashboard automatically receives telemetry
    // Navigate to: http://localhost:18888/traces
    return builder;
}

// That's it! No manual configuration needed!
```

**Custom Tracing:**
```csharp
using System.Diagnostics;

public class ProductService
{
    private static readonly ActivitySource ActivitySource = new("MyApiService");

    public async Task<Product> GetProductAsync(int productId)
    {
        // Create custom span
        using var activity = ActivitySource.StartActivity("GetProduct");
        activity?.SetTag("product.id", productId);

        try
        {
            var product = await _repository.GetByIdAsync(productId);

            if (product == null)
            {
                activity?.SetStatus(ActivityStatusCode.Error, "Product not found");
                activity?.SetTag("product.found", false);
                return null;
            }

            activity?.SetTag("product.name", product.Name);
            activity?.SetTag("product.price", product.Price);
            activity?.SetTag("product.found", true);

            return product;
        }
        catch (Exception ex)
        {
            activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
            activity?.RecordException(ex);
            throw;
        }
    }
}

// Trace example:
// TraceId: 4bf92f3577b34da6a3ce929d0e0e4736
// SpanId: 00f067aa0ba902b7
// ParentSpanId: null
// Name: GetProduct
// Tags:
//   - product.id: 123
//   - product.name: "Widget"
//   - product.price: 29.99
//   - product.found: true
// Duration: 45ms
```

**Custom Metrics:**
```csharp
using System.Diagnostics.Metrics;

public class OrderService
{
    private static readonly Meter Meter = new("MyApiService");
    private static readonly Counter<long> OrdersProcessed = Meter.CreateCounter<long>("orders.processed");
    private static readonly Histogram<double> OrderValue = Meter.CreateHistogram<double>("orders.value");

    public async Task ProcessOrderAsync(Order order)
    {
        await ProcessAsync(order);

        // Increment counter
        OrdersProcessed.Add(1, new KeyValuePair<string, object>("status", "success"));

        // Record histogram
        OrderValue.Record(order.TotalAmount, new KeyValuePair<string, object>("currency", "USD"));
    }
}

// Metrics exported:
// orders.processed{status="success"} = 1234
// orders.value.bucket{le="10"} = 45
// orders.value.bucket{le="100"} = 156
// orders.value.sum = 45678.90
// orders.value.count = 1234
```

**Distributed Tracing (Cross-Service):**
```csharp
// Service A calls Service B
// OpenTelemetry automatically propagates trace context via HTTP headers

// Service A (calling service)
var httpClient = _httpClientFactory.CreateClient();
var response = await httpClient.GetAsync("http://service-b/api/products/123");
// OpenTelemetry automatically adds traceparent header:
// traceparent: 00-4bf92f3577b34da6a3ce929d0e0e4736-00f067aa0ba902b7-01

// Service B (receiving service)
// OpenTelemetry automatically reads traceparent header
// Creates child span with same TraceId but different SpanId

// Result: Single distributed trace across both services!
// TraceId: 4bf92f3577b34da6a3ce929d0e0e4736
//   ├─ Service A: GET /api/orders/456 (200ms)
//   │   └─ HTTP GET http://service-b/api/products/123 (150ms)
//   │       └─ Service B: GET /api/products/123 (145ms)
//   │           └─ SQL SELECT * FROM products WHERE id = 123 (5ms)
```

### Application Insights SDK (Azure-Specific)

**Pros:**
* Deep Azure integration
* Automatic dependency tracking
* Live metrics stream
* Application Map (visual topology)
* Excellent portal experience

**Cons:**
* **Vendor lock-in** - Azure only
* **Proprietary format** - Can't export to Jaeger, Zipkin
* **Cost** - Pay per GB ingested (~$2.30/GB)
* **Migration complexity** - Hard to switch to other APM

**When to Use:**
- Azure-only deployment
- No multi-cloud requirements
- Team heavily invested in Azure

**Why Not Primary Choice:**
Application Insights is excellent for Azure, but we want **vendor neutrality**. OpenTelemetry can export to Application Insights while keeping options open.

### Datadog APM / New Relic / Dynatrace

**Pros:**
* Full-featured APM platforms
* Great UX
* AI-powered insights
* Alerting and dashboards

**Cons:**
* **Expensive** - $15-31/host/month
* **Vendor lock-in** - Proprietary agents
* **Can't self-host** - SaaS only

**Why Not Chosen:**
Commercial APM tools are excellent but expensive and lock you in. OpenTelemetry + open-source backends (Jaeger, Prometheus, Grafana) provide 80% of functionality at 10% of cost.

### Manual Instrumentation

**Pros:**
* Full control
* No dependencies

**Cons:**
* **Time-consuming** - Hundreds of hours to instrument app
* **Error-prone** - Easy to miss critical paths
* **Maintenance burden** - Must update when code changes
* **No standards** - Incompatible with other tools

**Why Rejected:**
Manual instrumentation would require 100+ hours for basic tracing. OpenTelemetry provides automatic instrumentation that captures 90% of important operations.

---

## OpenTelemetry Backends

**Jaeger (Distributed Tracing):**
```bash
# Docker
docker run -d --name jaeger \
  -p 16686:16686 \
  -p 4317:4317 \
  jaegertracing/all-in-one:latest

# Navigate to: http://localhost:16686
```

**Prometheus + Grafana (Metrics):**
```yaml
# docker-compose.yml
version: '3.8'
services:
  prometheus:
    image: prom/prometheus
    volumes:
      - ./prometheus.yml:/etc/prometheus/prometheus.yml
    ports:
      - "9090:9090"

  grafana:
    image: grafana/grafana
    ports:
      - "3000:3000"
    environment:
      - GF_SECURITY_ADMIN_PASSWORD=admin
```

**OpenTelemetry Collector:**
```yaml
# otel-collector-config.yaml
receivers:
  otlp:
    protocols:
      grpc:
        endpoint: 0.0.0.0:4317
      http:
        endpoint: 0.0.0.0:4318

processors:
  batch:
    timeout: 10s
    send_batch_size: 1024

exporters:
  jaeger:
    endpoint: jaeger:14250
    tls:
      insecure: true
  prometheus:
    endpoint: 0.0.0.0:8889
  logging:
    loglevel: debug

service:
  pipelines:
    traces:
      receivers: [otlp]
      processors: [batch]
      exporters: [jaeger, logging]
    metrics:
      receivers: [otlp]
      processors: [batch]
      exporters: [prometheus, logging]
```

---

## Sampling Strategies

**1. Always On (Development):**
```csharp
.AddOtlpExporter(options =>
{
    options.SamplingProbability = 1.0;  // 100% of traces
});
```

**2. Probabilistic (Production):**
```csharp
// Sample 10% of requests
builder.Services.Configure<AspNetCoreInstrumentationOptions>(options =>
{
    options.RecordException = true;
});

// Custom sampler
builder.Services.AddOpenTelemetry()
    .WithTracing(tracing => tracing
        .SetSampler(new TraceIdRatioBasedSampler(0.1)));  // 10% sampling
```

**3. Parent-Based (Respect upstream decisions):**
```csharp
.SetSampler(new ParentBasedSampler(new TraceIdRatioBasedSampler(0.1)));
```

---

## Best Practices

**1. Add Meaningful Tags:**
```csharp
activity?.SetTag("user.id", userId);
activity?.SetTag("product.category", "electronics");
activity?.SetTag("order.total", 99.99);
```

**2. Record Exceptions:**
```csharp
try
{
    // ...
}
catch (Exception ex)
{
    activity?.RecordException(ex);
    activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
    throw;
}
```

**3. Don't Trace Everything:**
```csharp
// ❌ Don't trace health checks
options.Filter = httpContext =>
    !httpContext.Request.Path.StartsWithSegments("/health");
```

---

## Links

* [OpenTelemetry .NET](https://github.com/open-telemetry/opentelemetry-dotnet)
* [OpenTelemetry Documentation](https://opentelemetry.io/docs/)
* [Jaeger](https://www.jaegertracing.io/)
* [Sample Implementation](../../src/AdvancedConcepts.Core/Advanced/Observability/OpenTelemetryExamples.cs)

---

## Notes

**Three Pillars of Observability:**
1. **Traces** - Where time went (distributed request flow)
2. **Metrics** - How many and how much (counters, gauges, histograms)
3. **Logs** - What happened (event records)

**Correlation:**
- Trace ID links all spans in a request
- Span ID identifies individual operation
- Logs can include trace ID for correlation

**Review Date:** 2025-12-01
