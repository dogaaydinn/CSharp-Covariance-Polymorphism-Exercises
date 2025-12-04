# ADR-0007: OpenTelemetry for Observability

**Status:** Accepted
**Date:** 2025-12-02
**Deciders:** Architecture Team
**Technical Story:** Distributed tracing, metrics, and logging for microservices

## Context

Cloud-native microservices require comprehensive observability to:
- Debug issues across service boundaries
- Monitor performance and latency
- Track error rates and failures
- Understand system behavior in production
- Identify performance bottlenecks
- Correlate logs across services

Traditional logging is insufficient for distributed systems because:
- Logs are scattered across services
- No visibility into cross-service requests
- Cannot trace request flow through system
- Missing context for debugging
- No performance metrics

We need vendor-neutral, standards-based observability that works with any backend (Jaeger, Prometheus, Azure Monitor, AWS X-Ray, etc.).

## Decision

We will use **OpenTelemetry** for all observability needs (tracing, metrics, and logging).

Implementation:
- Automatic instrumentation via Aspire ServiceDefaults
- OpenTelemetry SDK 1.14.0
- Exporters: OTLP (OpenTelemetry Protocol)
- Dashboard: Aspire Dashboard (development), vendor-specific (production)
- Instrumentation: ASP.NET Core, HttpClient, EF Core, Runtime

## Consequences

### Positive

- **Vendor-Neutral**: Works with any observability backend
- **Standard**: OpenTelemetry is CNCF standard (like Kubernetes)
- **Automatic**: Aspire configures OpenTelemetry automatically
- **Distributed Tracing**: Track requests across all services
- **Correlation**: All logs/traces/metrics correlated by TraceId
- **Performance Metrics**: Built-in HTTP, runtime, and custom metrics
- **Production-Ready**: Used by Google, Microsoft, Uber, etc.
- **Aspire Dashboard**: Beautiful real-time dashboard in development
- **Future-Proof**: Industry is converging on OpenTelemetry
- **No Vendor Lock-in**: Switch backends (Jaeger → Prometheus) without code changes

### Negative

- **Overhead**: 5-10% CPU/memory overhead for telemetry collection
- **Complexity**: Understanding traces/spans requires learning curve
- **Storage**: Telemetry data can be massive (GB/day for busy services)
- **Cost**: Backend storage (Jaeger, Tempo) has infrastructure costs
- **Configuration**: Many knobs to tune (sampling, batching, exporters)

### Neutral

- **Sampling**: Production requires sampling (not all traces) to reduce cost
- **Cardinality**: High-cardinality metrics (user IDs, etc.) can overwhelm systems
- **Privacy**: Telemetry may contain PII (must sanitize)

## Alternatives Considered

### Alternative 1: Application Insights (Azure)

**Pros:**
- **Integrated**: Deep Azure integration
- **Powerful**: Advanced analytics and AI-powered insights
- **Automatic**: Auto-instrumentation for Azure services
- **Dashboards**: Rich out-of-box dashboards

**Cons:**
- **Azure Lock-in**: Only works with Azure
- **Cost**: Expensive at scale ($2-3/GB ingested)
- **Proprietary**: Not standards-based
- **Migration**: Hard to migrate to other clouds

**Why rejected:** Vendor lock-in unacceptable. OpenTelemetry works with Application Insights via OTLP.

### Alternative 2: Serilog + Seq

**Pros:**
- **Simple**: Structured logging with minimal setup
- **Seq**: Beautiful log exploration UI
- **Mature**: 10+ years of production use
- **.NET Native**: Built for .NET ecosystem

**Cons:**
- **Logging Only**: No distributed tracing or metrics
- **Not Standard**: Proprietary format
- **Correlation**: Manual correlation across services
- **Limited Metrics**: Cannot track performance metrics

**Why rejected:** Logging alone insufficient for microservices. OpenTelemetry provides tracing + metrics + logging.

### Alternative 3: Jaeger + Prometheus + Grafana (Manual Setup)

**Pros:**
- **Open Source**: No licensing costs
- **Powerful**: Industry-standard tools
- **Flexible**: Complete control

**Cons:**
- **Complex Setup**: Must configure everything manually
- **Fragmented**: Separate tools for traces, metrics, logs
- **No Standards**: Each tool uses different formats
- **High Maintenance**: Must manage 3+ separate systems

**Why rejected:** OpenTelemetry provides unified API. Can still export to Jaeger/Prometheus but with standard protocol.

### Alternative 4: AWS X-Ray

**Pros:**
- **AWS Integration**: Deep AWS service integration
- **Automatic**: Auto-instruments Lambda, ECS, etc.
- **Cost-Effective**: $5/million traces

**Cons:**
- **AWS Lock-in**: Only works on AWS
- **Limited Metrics**: Focused on tracing, weak metrics
- **.NET Support**: Weaker .NET support than OpenTelemetry

**Why rejected:** Same reasons as Application Insights - vendor lock-in.

### Alternative 5: Custom Logging + Manual Correlation

**Pros:**
- **Control**: Complete control over implementation
- **No Dependencies**: No external libraries

**Cons:**
- **Massive Effort**: 1000s of lines of boilerplate code
- **Reinventing Wheel**: Solving solved problems
- **Maintenance**: Must maintain forever
- **Not Standard**: Cannot use existing tools

**Why rejected:** Unacceptable engineering cost. OpenTelemetry is free and standardized.

## Related Decisions

- [ADR-0002](0002-using-dotnet-aspire.md): Aspire configures OpenTelemetry automatically
- [ADR-0013](0013-servicedefaults-pattern.md): ServiceDefaults includes OpenTelemetry setup
- [ADR-0011](0011-service-discovery-pattern.md): Tracing critical for debugging service-to-service calls

## Related Links

- [OpenTelemetry Documentation](https://opentelemetry.io/docs/)
- [OpenTelemetry .NET](https://github.com/open-telemetry/opentelemetry-dotnet)
- [Aspire Observability](https://learn.microsoft.com/dotnet/aspire/fundamentals/telemetry)
- [Aspire Dashboard](https://learn.microsoft.com/dotnet/aspire/fundamentals/dashboard)
- [CNCF OpenTelemetry](https://www.cncf.io/projects/opentelemetry/)

## Notes

- **Trace Context Propagation**: W3C Trace Context standard ensures traces cross service boundaries
- **Sampling Strategy**:
  - Development: 100% sampling (all traces)
  - Production: 1-10% sampling (or adaptive sampling)
- **Custom Metrics**: Add custom metrics for business KPIs
  ```csharp
  var videoViewCounter = meterProvider.GetMeter("VideoService")
      .CreateCounter<int>("video.views");
  videoViewCounter.Add(1, new KeyValuePair<string, object>("video.id", videoId));
  ```

- **Structured Logging**: Use structured logging for better searchability
  ```csharp
  logger.LogInformation("Video {VideoId} viewed by {UserId}", videoId, userId);
  ```

- **Aspire Dashboard Features**:
  - Real-time trace visualization
  - Metrics graphs
  - Structured logs with filtering
  - Service dependencies graph
  - Resource monitoring

- **Production Backends**:
  - Tracing: Jaeger, Tempo, Azure Monitor, AWS X-Ray
  - Metrics: Prometheus, Azure Monitor, AWS CloudWatch
  - Logs: Loki, Azure Monitor, AWS CloudWatch Logs

- **Performance**:
  - Use `ActivitySource` for custom spans
  - Batch exports (reduce network calls)
  - Sample in production (reduce overhead and cost)

- **Future**: OpenTelemetry Logs (currently experimental) will unify logging with traces/metrics
