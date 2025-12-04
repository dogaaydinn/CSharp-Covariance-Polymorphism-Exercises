# ADR-0002: Using .NET Aspire for Cloud-Native Development

**Status:** Accepted
**Date:** 2025-12-02
**Deciders:** Architecture Team
**Technical Story:** Cloud-native orchestration and observability requirements

## Context

Building cloud-native microservices requires solving common distributed system challenges:
- Service discovery and communication
- Observability (tracing, metrics, logging)
- Infrastructure dependencies (databases, caches, message queues)
- Local development environment complexity
- Configuration management across services
- Health checks and resilience patterns

Traditional approaches require:
- Manual Docker Compose setup
- Custom service discovery implementation
- Manual OpenTelemetry configuration
- Complex connection string management
- Separate development and production tooling

We need a solution that provides production-ready patterns while maintaining developer productivity.

## Decision

We will use **.NET Aspire** as the orchestration and cloud-native development framework for the AspireVideoService sample.

Specific implementation:
- AppHost project for service orchestration
- ServiceDefaults library for shared configuration
- Aspire hosting packages for infrastructure (Redis, PostgreSQL)
- Built-in service discovery
- Automatic OpenTelemetry configuration

## Consequences

### Positive

- **Zero-Config Infrastructure**: `builder.AddRedis("cache")` provisions Redis with zero configuration
- **Automatic Service Discovery**: Services reference each other by name (`http://api`) without URLs
- **Built-in Observability**: OpenTelemetry tracing, metrics, and logging configured automatically
- **Developer Experience**: Single command (`dotnet run`) starts entire distributed system
- **Aspire Dashboard**: Real-time observability dashboard at `localhost:18888`
- **Production Patterns**: Circuit breakers, retries, and health checks included by default
- **Type-Safe Configuration**: Compile-time checks for service references
- **Cloud-Agnostic**: Deploy to any cloud (Azure, AWS, GCP) or on-premises
- **Consistent Environment**: Dev environment matches production topology
- **Rapid Prototyping**: Add new services in minutes, not hours

### Negative

- **Learning Curve**: Developers must learn Aspire-specific APIs and patterns
- **Abstraction Layer**: Less control over low-level infrastructure configuration
- **Tooling Requirement**: Requires .NET 8 SDK (not compatible with older versions)
- **Early Adoption Risk**: Aspire 13.0 is relatively new (GA: February 2024)
- **Deployment Complexity**: Production deployment requires understanding Aspire manifests
- **Docker Dependency**: Requires Docker Desktop for local development
- **Limited Customization**: Some infrastructure options limited by Aspire abstractions

### Neutral

- **Package Updates**: Aspire evolves rapidly; frequent updates expected
- **Community Size**: Growing but smaller than established frameworks (Kubernetes, Docker Compose)
- **Documentation**: Comprehensive Microsoft docs, but community resources still developing

## Alternatives Considered

### Alternative 1: Docker Compose

**Pros:**
- Industry standard, widely understood
- Simple YAML configuration
- No framework lock-in
- Large community and ecosystem
- Works with any language/platform

**Cons:**
- No service discovery (must use static ports or DNS)
- Manual observability setup (must configure OpenTelemetry separately)
- No built-in resilience patterns
- Connection strings must be managed manually
- No integrated dashboard
- Dev environment differs from production
- Limited type safety

**Why rejected:** Requires too much manual configuration. Aspire provides same capabilities with better developer experience and production-ready patterns.

### Alternative 2: Kubernetes with Helm

**Pros:**
- Production-grade orchestration
- Battle-tested in large-scale deployments
- Comprehensive ecosystem (Istio, Prometheus, Grafana)
- Industry standard for cloud-native apps

**Cons:**
- Extremely complex for local development
- Steep learning curve (weeks to months)
- Overkill for development environment
- Requires Minikube/Kind/Docker Desktop Kubernetes
- YAML configuration hell (100s of lines)
- Slow iteration cycle
- Not beginner-friendly for educational samples

**Why rejected:** Too complex for local development and educational purposes. Kubernetes is appropriate for production, but Aspire provides similar patterns with 10x better developer experience.

### Alternative 3: Manual Configuration (No Orchestrator)

**Pros:**
- Complete control over every aspect
- No abstraction layers
- No additional dependencies
- Simple mental model

**Cons:**
- Must manually implement service discovery
- Must manually configure OpenTelemetry
- Must write custom health checks
- Must implement resilience patterns from scratch
- No dashboard out of the box
- High maintenance burden
- Error-prone (easy to misconfigure)
- Not scalable beyond 2-3 services

**Why rejected:** Defeats the purpose of demonstrating cloud-native best practices. Would require 1000s of lines of boilerplate code that Aspire provides for free.

### Alternative 4: Tye (Microsoft Project Tye)

**Pros:**
- Microsoft's previous microservices tool
- Similar goals to Aspire
- YAML-based configuration

**Cons:**
- Deprecated in favor of Aspire
- No active development
- Limited observability features
- No built-in dashboard
- Less type-safe than Aspire

**Why rejected:** Microsoft officially deprecated Tye in favor of Aspire. Aspire is the spiritual successor with significantly better features.

## Related Decisions

- [ADR-0001](0001-adopting-dotnet-8-platform.md): .NET 8 required for Aspire
- [ADR-0007](0007-opentelemetry-for-observability.md): Aspire includes OpenTelemetry
- [ADR-0011](0011-service-discovery-pattern.md): Service discovery provided by Aspire
- [ADR-0019](0019-apphost-orchestration.md): AppHost pattern from Aspire
- [ADR-0020](0020-zero-configuration-infrastructure.md): Aspire's infrastructure model

## Related Links

- [.NET Aspire Documentation](https://learn.microsoft.com/dotnet/aspire/)
- [Aspire GitHub Repository](https://github.com/dotnet/aspire)
- [Aspire 13.0 Announcement](https://devblogs.microsoft.com/dotnet/aspire-13-0-ga/)
- [Aspire vs Docker Compose](https://learn.microsoft.com/dotnet/aspire/get-started/aspire-overview#aspire-vs-docker-compose)

## Notes

- Aspire is designed for local development; production deployment uses standard containers/Kubernetes
- Aspire Dashboard is development-only; production uses Prometheus/Grafana/Jaeger
- Consider `azd` (Azure Developer CLI) for Azure Container Apps deployment
- Watch for Aspire 14.0 (expected Q2 2025) with enhanced features
- ServiceDefaults pattern is key to consistent configuration across services
