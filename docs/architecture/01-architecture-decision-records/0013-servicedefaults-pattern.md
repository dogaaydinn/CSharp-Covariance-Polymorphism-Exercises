# ADR-0013: ServiceDefaults Pattern for Cross-Cutting Concerns

**Status:** Accepted
**Date:** 2025-12-02
**Deciders:** Architecture Team
**Technical Story:** Consistent configuration across microservices

## Context

Microservices require consistent configuration for:
- OpenTelemetry (tracing, metrics, logging)
- Service discovery
- Health checks
- HTTP client resilience (retry, circuit breaker, timeout)
- Logging configuration
- Error handling

Without standardization, each service implements these differently, leading to:
- Inconsistent observability
- Repeated configuration code
- Configuration drift
- Higher maintenance burden

## Decision

We will use the **ServiceDefaults** pattern: a shared library that provides standard configuration for all cross-cutting concerns.

Implementation:
- `VideoService.ServiceDefaults` project
- Single `AddServiceDefaults()` extension method
- All services call `builder.AddServiceDefaults()`
- OpenTelemetry, service discovery, resilience configured automatically

## Consequences

### Positive

- **DRY Principle**: Configuration defined once, reused everywhere
- **Consistency**: All services configured identically
- **Easy Updates**: Change once, affects all services
- **Onboarding**: New services get best practices automatically
- **Standards Enforcement**: Impossible to forget health checks or observability

### Negative

- **Shared Dependency**: All services depend on ServiceDefaults
- **Less Flexibility**: Harder to customize per-service
- **Breaking Changes**: Changes affect all services

### Neutral

- **Convention Over Configuration**: Opinionated but consistent

## Related Decisions

- [ADR-0002](0002-using-dotnet-aspire.md): Aspire recommends ServiceDefaults pattern
- [ADR-0007](0007-opentelemetry-for-observability.md): OpenTelemetry configured in ServiceDefaults

## Related Links

- [Aspire ServiceDefaults](https://learn.microsoft.com/dotnet/aspire/fundamentals/service-defaults)

## Notes

ServiceDefaults configuration:
```csharp
public static IHostApplicationBuilder AddServiceDefaults(this IHostApplicationBuilder builder)
{
    builder.ConfigureOpenTelemetry();
    builder.AddDefaultHealthChecks();
    builder.Services.AddServiceDiscovery();
    builder.Services.ConfigureHttpClientDefaults(http =>
    {
        http.AddServiceDiscovery();
        http.AddStandardResilienceHandler();
    });
    return builder;
}
```

Usage in services:
```csharp
var builder = WebApplication.CreateBuilder(args);
builder.AddServiceDefaults(); // ← One line for everything!
```
