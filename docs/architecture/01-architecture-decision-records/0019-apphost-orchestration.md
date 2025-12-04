# ADR-0019: AppHost for Service Orchestration

**Status:** Accepted
**Date:** 2025-12-02
**Deciders:** Architecture Team
**Technical Story:** Development environment orchestration

## Context

Microservices require orchestration in development:
- Start multiple services in correct order
- Manage dependencies (Redis, PostgreSQL)
- Configure service discovery
- Setup observability

Traditional: Docker Compose, manual process management
Modern: .NET Aspire AppHost

## Decision

Use **Aspire AppHost** project for service orchestration in development.

Structure:
```
VideoService.AppHost/
└── Program.cs          # Orchestration definition
```

## Consequences

### Positive

- **Declarative**: Services and dependencies defined in code
- **Type-Safe**: Compile-time checks for service references
- **Integrated**: OpenTelemetry, service discovery, health checks automatic
- **Dashboard**: Aspire Dashboard included
- **Single Command**: `dotnet run` starts everything

### Negative

- **Development Only**: Not used in production
- **Aspire Dependency**: Requires Aspire SDK
- **Learning Curve**: New concept for developers

## Implementation

```csharp
var builder = DistributedApplication.CreateBuilder(args);

// Infrastructure
var redis = builder.AddRedis("cache")
    .WithRedisCommander();

var postgres = builder.AddPostgres("postgres")
    .WithPgAdmin()
    .AddDatabase("videodb");

// Services
var api = builder.AddProject("api", "../VideoService.API/VideoService.API.csproj")
    .WithReference(redis)
    .WithReference(postgres)
    .WithExternalHttpEndpoints();

builder.AddProject("web", "../VideoService.Web/VideoService.Web.csproj")
    .WithReference(api)
    .WithExternalHttpEndpoints();

builder.Build().Run();
```

## Related Decisions

- [ADR-0002](0002-using-dotnet-aspire.md): Aspire framework choice
- [ADR-0011](0011-service-discovery-pattern.md): Service references

## Notes

- AppHost automatically generates Aspire Dashboard at `http://localhost:18888`
- Production deployment: Use `azd` or generate Kubernetes manifests
- Service references create service discovery bindings
- Infrastructure resources (Redis, PostgreSQL) automatically started as containers
