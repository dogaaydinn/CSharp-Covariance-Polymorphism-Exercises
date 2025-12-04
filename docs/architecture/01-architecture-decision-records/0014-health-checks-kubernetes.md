# ADR-0014: Health Checks for Kubernetes Readiness

**Status:** Accepted
**Date:** 2025-12-02
**Deciders:** Architecture Team
**Technical Story:** Production readiness and orchestration support

## Context

Cloud platforms (Kubernetes, Azure Container Apps) need to know:
- Is service alive? (liveness)
- Is service ready to accept traffic? (readiness)
- What dependencies are healthy?

Without health checks:
- Orchestrator routes traffic to failed instances
- No automatic restart on crash
- Cannot detect degraded state

## Decision

Implement Kubernetes-compatible **health check endpoints** in all services.

Endpoints:
- `/health` - Overall health (liveness + readiness)
- `/alive` - Liveness probe (is process running?)
- `/ready` - Readiness probe (can handle requests?)

## Consequences

### Positive

- **Automatic Recovery**: Kubernetes restarts failed pods
- **Zero-Downtime Deploys**: No traffic to unready pods
- **Dependency Monitoring**: Track database/cache health
- **Cloud-Native**: Works with any orchestrator
- **Observability**: Aspire Dashboard shows health status

### Negative

- **Endpoint Overhead**: Additional HTTP endpoints
- **Configuration**: Must configure Kubernetes probes correctly

## Related Decisions

- [ADR-0013](0013-servicedefaults-pattern.md): ServiceDefaults adds health checks
- [ADR-0002](0002-using-dotnet-aspire.md): Aspire includes health check support

## Notes

Implementation:
```csharp
builder.Services.AddHealthChecks()
    .AddCheck("self", () => HealthCheckResult.Healthy(), ["live"])
    .AddNpgsql(connectionString, tags: ["ready"])
    .AddRedis(connectionString, tags: ["ready"]);

app.MapHealthChecks("/health");
app.MapHealthChecks("/alive", new() { Predicate = r => r.Tags.Contains("live") });
app.MapHealthChecks("/ready", new() { Predicate = r => r.Tags.Contains("ready") });
```

Kubernetes configuration:
```yaml
livenessProbe:
  httpGet:
    path: /alive
    port: 8080
  initialDelaySeconds: 5
  periodSeconds: 10

readinessProbe:
  httpGet:
    path: /ready
    port: 8080
  initialDelaySeconds: 10
  periodSeconds: 5
```
