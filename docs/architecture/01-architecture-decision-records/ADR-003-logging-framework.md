# ADR-003: Serilog as Structured Logging Framework

**Status:** ✅ Accepted
**Date:** 2025-11-30
**Deciders:** Development Team
**Technical Story:** Phase 5 - Observability & Monitoring

## Context

Enterprise applications require robust logging for:
1. Debugging production issues
2. Performance monitoring
3. Security auditing
4. Compliance requirements
5. Business analytics

We needed to choose a logging framework that provides:
- Structured logging (not just text)
- Multiple sink support (Console, File, external services)
- Performance (minimal overhead)
- Rich context enrichment
- Production-grade reliability

## Decision

We will use **Serilog** as our structured logging framework with:
- **Serilog.Sinks.Console** for development
- **Serilog.Sinks.File** for persistent logs
- **Serilog.Sinks.Seq** for centralized log aggregation
- **Enrichers** (Environment, Process, Thread) for context
- **OpenTelemetry** integration for distributed tracing

## Rationale

### Structured Logging
```csharp
// Traditional logging (unstructured)
logger.LogInformation($"User {userId} placed order {orderId}");

// Serilog (structured)
logger.Information("User {UserId} placed order {OrderId}",
    userId, orderId);
// Output: { "UserId": 123, "OrderId": 456, "Message": "User 123..." }
```

**Benefits:**
- **Queryable logs:** Search by UserId=123
- **Aggregation:** Count orders per user
- **Alerting:** Trigger on specific values
- **Analytics:** Business insights from logs

### Multiple Sinks
```csharp
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()           // Development
    .WriteTo.File("logs/app.log") // Persistent
    .WriteTo.Seq("http://localhost:5341") // Centralized
    .CreateLogger();
```

### Enrichers for Context
```csharp
.Enrich.WithMachineName()      // Which server?
.Enrich.WithEnvironmentName()  // Dev/Staging/Prod?
.Enrich.WithThreadId()         // Concurrency tracking
.Enrich.WithProcessId()        // Process isolation
.Enrich.FromLogContext()       // Request-scoped data
```

### Performance Characteristics
- **Minimal allocation:** Optimized for high-throughput
- **Async sinks:** Non-blocking log writes
- **Filtering:** Only log what matters
- **Sampling:** Reduce volume in production

### Enterprise Features
- **Minimum level per sink:** Debug to Console, Info to File
- **Log event filtering:** Exclude noisy sources
- **Destructuring:** Complex object logging
- **Message templates:** Consistent log format

## Comparison with Alternatives

### Serilog vs Microsoft.Extensions.Logging

| Feature | Serilog | Microsoft.Extensions.Logging |
|---------|---------|------------------------------|
| Structured Logging | ✅ Native | ⚠️ Limited |
| Sinks | 100+ available | Fewer providers |
| Performance | High | Good |
| Configuration | Fluent API | Configuration-based |
| Context Enrichment | Rich | Basic |
| Community | Large | Growing |

### Serilog vs NLog

| Feature | Serilog | NLog |
|---------|---------|------|
| Structured | ✅ First-class | ⚠️ Added later |
| Modern .NET | ✅ Excellent | ✅ Good |
| Performance | ✅ Faster | Good |
| Configuration | Code-first | XML/Config |
| Learning Curve | Easy | Moderate |

## Consequences

### Positive
- ✅ **Structured logging** enables advanced querying
- ✅ **Multiple sinks** for different environments
- ✅ **Rich enrichment** provides valuable context
- ✅ **High performance** with minimal overhead
- ✅ **Seq integration** for centralized logging
- ✅ **OpenTelemetry** compatibility for tracing
- ✅ **Production-ready** with proven reliability

### Negative
- ⚠️ **Additional dependency:** NuGet packages to manage
- ⚠️ **Learning curve:** Developers need to learn message templates
- ⚠️ **Configuration complexity:** More options to configure
- ⚠️ **Seq licensing:** Commercial license for Seq (optional)

### Operational Impact
- **Log Storage:** Structured logs are larger than plain text
- **Network Traffic:** Centralized logging increases bandwidth
- **Latency:** Async sinks minimize impact
- **Cost:** Seq server or cloud logging service

## Implementation

### Configuration Example
```csharp
public static void ConfigureSerilog()
{
    Log.Logger = new LoggerConfiguration()
        // Minimum levels
        .MinimumLevel.Debug()
        .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
        .MinimumLevel.Override("System", LogEventLevel.Warning)

        // Enrichers
        .Enrich.FromLogContext()
        .Enrich.WithMachineName()
        .Enrich.WithEnvironmentName()
        .Enrich.WithProcessId()
        .Enrich.WithThreadId()

        // Sinks
        .WriteTo.Console(
            outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
        .WriteTo.File(
            path: "logs/app-.log",
            rollingInterval: RollingInterval.Day,
            retainedFileCountLimit: 30,
            outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
        .WriteTo.Seq("http://localhost:5341")

        .CreateLogger();
}
```

### Usage Patterns

#### Basic Logging
```csharp
logger.Information("Application started at {StartTime}", DateTime.UtcNow);
logger.Warning("Retry attempt {RetryCount} for operation {OperationId}",
    retryCount, operationId);
logger.Error(exception, "Failed to process order {OrderId}", orderId);
```

#### Context Enrichment
```csharp
using (LogContext.PushProperty("CorrelationId", correlationId))
using (LogContext.PushProperty("UserId", userId))
{
    logger.Information("Processing payment");
    // All logs within this scope include CorrelationId and UserId
}
```

#### Performance Logging
```csharp
using (logger.BeginTimedOperation("Database Query"))
{
    await repository.GetAllAsync();
    // Automatically logs duration
}
```

#### Security Events
```csharp
logger.Warning("Failed login attempt for user {Username} from IP {IpAddress}",
    username, ipAddress);
logger.Information("User {UserId} accessed sensitive resource {ResourceId}",
    userId, resourceId);
```

## Integration with OpenTelemetry

```csharp
// Serilog enriched with TraceId and SpanId
.Enrich.FromLogContext()
.Enrich.WithProperty("TraceId", Activity.Current?.TraceId)
.Enrich.WithProperty("SpanId", Activity.Current?.SpanId)
```

This enables correlation between:
- **Logs:** What happened
- **Traces:** Request flow across services
- **Metrics:** Performance measurements

## Monitoring Stack

```
Developer → Application (Serilog)
              ↓
         ┌────┴────┐
         ↓         ↓
     Console    File Logs
         ↓         ↓
    Development  Archive

Production → Application (Serilog)
              ↓
         ┌────┴────┬────────┐
         ↓         ↓        ↓
      Seq      File    Elasticsearch
         ↓         ↓        ↓
    Dashboard  Archive  Kibana
```

## Verification

- ✅ Serilog configured with enrichers
- ✅ Console, File, and Seq sinks operational
- ✅ Structured logging examples implemented
- ✅ Performance logging patterns documented
- ✅ Security event logging in place
- ✅ Docker Compose with Seq container
- ✅ Log correlation with OpenTelemetry

## Packages Installed

```xml
<!-- Core Serilog -->
<PackageReference Include="Serilog" Version="4.1.0" />
<PackageReference Include="Serilog.AspNetCore" Version="8.0.3" />

<!-- Sinks -->
<PackageReference Include="Serilog.Sinks.Console" Version="6.0.0" />
<PackageReference Include="Serilog.Sinks.File" Version="6.0.0" />
<PackageReference Include="Serilog.Sinks.Seq" Version="8.0.0" />

<!-- Enrichers -->
<PackageReference Include="Serilog.Enrichers.Environment" Version="3.0.1" />
<PackageReference Include="Serilog.Enrichers.Process" Version="3.0.0" />
<PackageReference Include="Serilog.Enrichers.Thread" Version="4.0.0" />
```

## References

- [Serilog Documentation](https://serilog.net/)
- [Structured Logging Best Practices](https://github.com/serilog/serilog/wiki/Structured-Data)
- [Seq Documentation](https://docs.datalust.co/docs)
- [OpenTelemetry Integration](https://opentelemetry.io/docs/languages/net/)

## Related ADRs

- ADR-001: .NET 8 Upgrade (enables modern logging features)
- ADR-004: CI/CD Platform (logging in deployment pipeline)

## Future Considerations

- [ ] **Application Insights** for Azure deployments
- [ ] **CloudWatch Logs** for AWS deployments
- [ ] **ELK Stack** (Elasticsearch, Logstash, Kibana) for large scale
- [ ] **Grafana Loki** as lightweight alternative to Elasticsearch

---

**Last Updated:** 2025-11-30
**Next Review:** 2026-03-01
