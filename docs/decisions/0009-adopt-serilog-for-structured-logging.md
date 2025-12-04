# 9. Adopt Serilog for Structured Logging

**Status:** ✅ Accepted

**Date:** 2024-12-01

**Deciders:** Architecture Team, DevOps Team

**Technical Story:** Implementation in `src/AdvancedConcepts.Core/Advanced/Observability`

---

## Context and Problem Statement

Effective logging is critical for:
- Debugging production issues
- Performance monitoring
- Security auditing
- Compliance requirements
- Understanding user behavior

**Traditional logging problems:**
- String interpolation loses structure (`$"User {userId} logged in"`)
- Hard to query logs
- No correlation IDs across microservices
- Console logs don't scale to production
- Missing context (request ID, user ID, etc.)

**We need:**
- Structured logging (JSON format)
- Multiple sinks (Console, File, Elasticsearch, Application Insights)
- Automatic enrichment (timestamp, correlation ID, environment)
- Performance (minimal overhead)
- Easy integration with .NET logging abstraction

---

## Decision Drivers

* **Structured Logging** - Log as data, not text
* **Multiple Sinks** - Console (dev), File (prod), Cloud (monitoring)
* **Enrichment** - Automatic context injection
* **Performance** - Async logging, buffering
* **Integration** - Works with ILogger<T>
* **Ecosystem** - Wide adoption, many sinks available

---

## Considered Options

* **Option 1** - Serilog
* **Option 2** - Microsoft.Extensions.Logging (default)
* **Option 3** - NLog
* **Option 4** - log4net

---

## Decision Outcome

**Chosen option:** "Serilog", because it provides true structured logging with extensive sinks, automatic enrichment, excellent performance, and seamless integration with Microsoft.Extensions.Logging.

### Positive Consequences

* **Structured** - Logs as JSON with queryable properties
* **50+ Sinks** - Console, File, Elasticsearch, Seq, Application Insights, Datadog, etc.
* **Enrichment** - Automatic context (timestamp, thread, machine, correlation ID)
* **Performance** - Async, buffered, minimal allocations
* **ILogger Integration** - Works with existing ILogger<T> code
* **Filtering** - Fine-grained control over what gets logged
* **Configuration** - appsettings.json or fluent API

### Negative Consequences

* **External Dependency** - NuGet package required
* **Learning Curve** - Team needs to learn semantic logging
* **Configuration** - More complex than Console.WriteLine

---

## Pros and Cons of the Options

### Serilog (Chosen)

**What is Serilog?**

Serilog is a structured logging library for .NET that treats logs as first-class data structures rather than text strings.

**Pros:**
* **Structured Logging** - Properties, not interpolated strings
* **Extensive Sinks** - 50+ available (Elasticsearch, Seq, Application Insights, etc.)
* **Enrichers** - Automatic context injection
* **Filtering** - Log level per namespace
* **Performance** - Async, batched, low allocation
* **ILogger Bridge** - Drop-in replacement for Microsoft.Extensions.Logging

**Cons:**
* **Configuration Complexity** - More options than simple logging
* **Sink Dependencies** - Each sink is separate NuGet package

**Setup:**
```csharp
// Install:
// Serilog.AspNetCore
// Serilog.Sinks.Console
// Serilog.Sinks.File
// Serilog.Enrichers.Environment
// Serilog.Enrichers.Thread

// Program.cs
using Serilog;

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .MinimumLevel.Override("System", LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .Enrich.WithMachineName()
    .Enrich.WithThreadId()
    .Enrich.WithEnvironmentName()
    .WriteTo.Console(
        outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}")
    .WriteTo.File(
        path: "logs/log-.txt",
        rollingInterval: RollingInterval.Day,
        retainedFileCountLimit: 30,
        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}")
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

// Use Serilog
builder.Host.UseSerilog();

var app = builder.Build();

// Log all HTTP requests
app.UseSerilogRequestLogging();

try
{
    Log.Information("Starting web application");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
```

**Configuration via appsettings.json:**
```json
{
  "Serilog": {
    "Using": ["Serilog.Sinks.Console", "Serilog.Sinks.File"],
    "MinimumLevel": {
      "Default": "Information",
      "Override": {
        "Microsoft": "Warning",
        "System": "Warning"
      }
    },
    "WriteTo": [
      {
        "Name": "Console",
        "Args": {
          "outputTemplate": "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}"
        }
      },
      {
        "Name": "File",
        "Args": {
          "path": "logs/log-.txt",
          "rollingInterval": "Day",
          "retainedFileCountLimit": 30
        }
      }
    ],
    "Enrich": ["FromLogContext", "WithMachineName", "WithThreadId"]
  }
}
```

**Structured Logging Example:**
```csharp
public class ProductService
{
    private readonly ILogger<ProductService> _logger;

    public async Task<Product> GetProductAsync(int productId, string userId)
    {
        // ❌ BAD: String interpolation loses structure
        _logger.LogInformation($"User {userId} retrieving product {productId}");
        // Logs as: "User user123 retrieving product 42"
        // Can't query by userId or productId!

        // ✅ GOOD: Semantic/Structured logging
        _logger.LogInformation(
            "User {UserId} retrieving product {ProductId}",
            userId,
            productId);
        // Logs as JSON:
        // {
        //   "Timestamp": "2024-12-01T10:30:00",
        //   "Level": "Information",
        //   "Message": "User user123 retrieving product 42",
        //   "Properties": {
        //     "UserId": "user123",
        //     "ProductId": 42
        //   }
        // }
        // Can query: WHERE UserId = 'user123'

        try
        {
            var product = await _repository.GetByIdAsync(productId);

            if (product == null)
            {
                _logger.LogWarning(
                    "Product {ProductId} not found for user {UserId}",
                    productId,
                    userId);
                return null;
            }

            _logger.LogInformation(
                "Successfully retrieved product {ProductName} (ID: {ProductId}) for user {UserId}",
                product.Name,
                productId,
                userId);

            return product;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Failed to retrieve product {ProductId} for user {UserId}",
                productId,
                userId);
            throw;
        }
    }
}
```

**Advanced Features:**

**1. Log Context Enrichment:**
```csharp
public class RequestLoggingMiddleware
{
    public async Task InvokeAsync(HttpContext context)
    {
        // Add correlation ID to all logs in this request
        using (LogContext.PushProperty("CorrelationId", context.TraceIdentifier))
        using (LogContext.PushProperty("UserId", context.User.FindFirst("sub")?.Value))
        using (LogContext.PushProperty("RequestPath", context.Request.Path))
        {
            await _next(context);
        }
        // All logs within this scope automatically include these properties
    }
}

// Any log in this request will include CorrelationId, UserId, RequestPath
_logger.LogInformation("Processing order"); // Automatically enriched!
```

**2. Multiple Sinks (Different Destinations):**
```csharp
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    // Console - for local development
    .WriteTo.Console()
    // File - for production debugging
    .WriteTo.File(
        path: "logs/log-.txt",
        rollingInterval: RollingInterval.Day)
    // Elasticsearch - for centralized logging
    .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri("http://localhost:9200"))
    {
        AutoRegisterTemplate = true,
        IndexFormat = "myapp-logs-{0:yyyy.MM}"
    })
    // Application Insights - for Azure monitoring
    .WriteTo.ApplicationInsights(
        instrumentationKey,
        TelemetryConverter.Traces)
    // Seq - for structured log viewing
    .WriteTo.Seq("http://localhost:5341")
    .CreateLogger();
```

**3. Conditional Logging:**
```csharp
Log.Logger = new LoggerConfiguration()
    .WriteTo.Logger(lc => lc
        .Filter.ByIncludingOnly(e => e.Level >= LogEventLevel.Error)
        .WriteTo.File("logs/errors-.txt", rollingInterval: RollingInterval.Day))
    .WriteTo.Logger(lc => lc
        .Filter.ByIncludingOnly(e => e.Properties.ContainsKey("Performance"))
        .WriteTo.File("logs/performance-.txt", rollingInterval: RollingInterval.Day))
    .WriteTo.Console()
    .CreateLogger();

// Usage
_logger.LogInformation("Processing {OrderId} - {Performance}", orderId, new { Duration = 123 });
// Goes to both console and performance-.txt
```

**4. Performance Logging:**
```csharp
public class PerformanceLoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
{
    private readonly ILogger<PerformanceLoggingBehavior<TRequest, TResponse>> _logger;

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken ct)
    {
        var sw = Stopwatch.StartNew();
        var requestName = typeof(TRequest).Name;

        _logger.LogInformation("Executing {RequestName}", requestName);

        try
        {
            var response = await next();
            sw.Stop();

            if (sw.ElapsedMilliseconds > 500)
            {
                _logger.LogWarning(
                    "Long running request: {RequestName} took {ElapsedMs}ms",
                    requestName,
                    sw.ElapsedMilliseconds);
            }
            else
            {
                _logger.LogInformation(
                    "Completed {RequestName} in {ElapsedMs}ms",
                    requestName,
                    sw.ElapsedMilliseconds);
            }

            return response;
        }
        catch (Exception ex)
        {
            sw.Stop();
            _logger.LogError(
                ex,
                "Request {RequestName} failed after {ElapsedMs}ms",
                requestName,
                sw.ElapsedMilliseconds);
            throw;
        }
    }
}
```

### Microsoft.Extensions.Logging (Default)

**Pros:**
* Built into .NET (no external dependencies)
* Simple API
* Abstraction (can swap providers)

**Cons:**
* **Not Truly Structured** - Limited property support
* **Console Sink Only** - Need providers for file, Elasticsearch, etc.
* **No Enrichment** - Manual context management
* **Limited Filtering** - Can't filter by namespace in appsettings.json

**Why Insufficient:**
```csharp
// Microsoft.Extensions.Logging
_logger.LogInformation("User {UserId} logged in", userId);
// Console output: "User user123 logged in"
// ❌ Properties not preserved to all sinks

// Serilog
_logger.LogInformation("User {UserId} logged in", userId);
// JSON output: { "UserId": "user123", "Message": "..." }
// ✅ Properties preserved and queryable
```

**Decision:** Use Serilog as provider for Microsoft.Extensions.Logging via `UseSerilog()`.

### NLog

**Pros:**
* Mature (20+ years)
* Many targets (sinks)
* Good performance
* Configuration via XML or JSON

**Cons:**
* **XML-heavy configuration** - Not as clean as Serilog
* **Less structured** - Properties support added later
* **Smaller ecosystem** - Fewer community contributions
* **Heavier** - More memory footprint than Serilog

**Why Not Chosen:**
NLog is excellent, but Serilog's structured-first approach and cleaner configuration make it a better fit for modern cloud-native apps.

### log4net

**Pros:**
* Very mature
* Well-known (from Java)
* Stable

**Cons:**
* **Legacy** - Minimal active development
* **XML configuration** - No fluent API
* **Not structured** - Designed for text logs
* **No async** - Synchronous logging only

**Why Rejected:**
log4net is legacy technology. Serilog and NLog are both superior modern alternatives.

---

## Production Logging Stack

**Recommended Stack:**
```
Application (Serilog)
    ↓
Local: Console + File
    ↓
Production: Elasticsearch + Application Insights + Seq
    ↓
Visualization: Kibana (Elasticsearch) / Azure Portal / Seq UI
```

**Elasticsearch Setup:**
```csharp
// Install: Serilog.Sinks.Elasticsearch

Log.Logger = new LoggerConfiguration()
    .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri("https://elasticsearch:9200"))
    {
        AutoRegisterTemplate = true,
        IndexFormat = "myapp-{0:yyyy.MM}",
        NumberOfShards = 2,
        NumberOfReplicas = 1,
        ModifyConnectionSettings = conn => conn.BasicAuthentication("user", "pass")
    })
    .CreateLogger();
```

**Application Insights (Azure):**
```csharp
// Install: Serilog.Sinks.ApplicationInsights

Log.Logger = new LoggerConfiguration()
    .WriteTo.ApplicationInsights(
        new TelemetryConfiguration { ConnectionString = "InstrumentationKey=..." },
        TelemetryConverter.Traces)
    .CreateLogger();
```

**Seq (Local Development):**
```bash
# Docker
docker run --name seq -d --restart unless-stopped -e ACCEPT_EULA=Y -p 5341:80 datalust/seq:latest

# Navigate to http://localhost:5341
```

```csharp
// Install: Serilog.Sinks.Seq

Log.Logger = new LoggerConfiguration()
    .WriteTo.Seq("http://localhost:5341")
    .CreateLogger();
```

---

## Best Practices

**1. Use Semantic Logging:**
```csharp
// ❌ BAD: String interpolation
_logger.LogInformation($"Processing order {orderId} for user {userId}");

// ✅ GOOD: Structured properties
_logger.LogInformation("Processing order {OrderId} for user {UserId}", orderId, userId);
```

**2. Use Log Levels Correctly:**
```csharp
// Verbose/Debug - Detailed flow, development only
_logger.LogDebug("Calculating tax for amount {Amount}", amount);

// Information - General flow, important business events
_logger.LogInformation("Order {OrderId} created", orderId);

// Warning - Unexpected but handled
_logger.LogWarning("Product {ProductId} stock low: {Stock} remaining", productId, stock);

// Error - Operation failed, exception handled
_logger.LogError(ex, "Failed to process payment for order {OrderId}", orderId);

// Critical/Fatal - Application crash
_logger.LogCritical(ex, "Database connection failed");
```

**3. Don't Log Sensitive Data:**
```csharp
// ❌ BAD: Logging passwords, tokens, credit cards
_logger.LogInformation("User {UserId} logged in with password {Password}", userId, password);

// ✅ GOOD: Log only necessary data
_logger.LogInformation("User {UserId} logged in successfully", userId);
```

**4. Use Scopes for Context:**
```csharp
using (_logger.BeginScope(new Dictionary<string, object>
{
    ["OrderId"] = orderId,
    ["UserId"] = userId
}))
{
    _logger.LogInformation("Starting order processing");
    // ... 20 lines of code ...
    _logger.LogInformation("Order validation complete");
    // All logs in this scope include OrderId and UserId automatically
}
```

---

## Links

* [Serilog Official Site](https://serilog.net/)
* [Serilog GitHub](https://github.com/serilog/serilog)
* [Available Sinks](https://github.com/serilog/serilog/wiki/Provided-Sinks)
* [Sample Implementation](../../src/AdvancedConcepts.Core/Advanced/Observability)

---

## Notes

**Log Levels (Order):**
- Verbose (5) - Noisiest
- Debug (4) - Development
- Information (3) - Production default
- Warning (2) - Unexpected but handled
- Error (1) - Operation failed
- Fatal (0) - Application crash

**Performance:**
- Use async sinks for production
- Batch logs before sending to remote sinks
- Set appropriate minimum levels (Information in prod, Debug in dev)

**Common Pitfalls:**
- ❌ Logging inside loops (causes performance issues)
- ❌ Using string interpolation instead of structured logging
- ❌ Logging sensitive data (passwords, tokens, PII)
- ❌ Not setting minimum levels (too much logging in production)

**Review Date:** 2025-12-01
