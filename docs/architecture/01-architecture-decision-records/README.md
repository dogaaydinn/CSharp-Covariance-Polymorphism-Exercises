# Architecture Decision Records (ADRs)

This directory contains Architecture Decision Records (ADRs) documenting significant architectural and technology decisions made in the AspireVideoService project.

## What is an ADR?

An Architecture Decision Record (ADR) is a document that captures an important architectural decision made along with its context and consequences. ADRs help teams:
- Understand **why** decisions were made
- Evaluate **alternatives** that were considered
- Document **trade-offs** and **consequences**
- Provide **context** for future decisions
- Onboard new team members quickly

## ADR Format

Each ADR follows a consistent structure:
- **Status**: Proposed | Accepted | Deprecated | Superseded
- **Context**: The problem or situation requiring a decision
- **Decision**: The chosen solution
- **Consequences**: Positive, negative, and neutral impacts
- **Alternatives Considered**: Other options and why they were rejected
- **Related Decisions**: Links to related ADRs
- **Notes**: Implementation details and best practices

## Index of Decisions

### Platform & Framework

- **[ADR-0001](0001-adopting-dotnet-8-platform.md)**: Adopting .NET 8 Platform
  - Why .NET 8 over .NET 6/7/9
  - LTS support, performance, modern features

- **[ADR-0002](0002-using-dotnet-aspire.md)**: Using .NET Aspire for Cloud-Native Development
  - Why Aspire over Docker Compose, Kubernetes, manual setup
  - Service orchestration, discovery, observability

### Data Layer

- **[ADR-0003](0003-entity-framework-core-data-access.md)**: Entity Framework Core for Data Access
  - Why EF Core over Dapper, ADO.NET, NHibernate
  - ORM benefits vs performance trade-offs

- **[ADR-0004](0004-postgresql-primary-database.md)**: PostgreSQL as Primary Database
  - Why PostgreSQL over SQL Server, MySQL, SQLite, NoSQL
  - Open source, JSON support, cloud-native

- **[ADR-0010](0010-direct-dbcontext-usage.md)**: Direct DbContext Usage (No Repository Pattern)
  - Why no Repository pattern over EF Core
  - Simplicity vs abstraction debate

### Caching & Performance

- **[ADR-0005](0005-redis-distributed-caching.md)**: Redis for Distributed Caching
  - Why Redis over in-memory cache, Memcached, SQL Server
  - Distributed caching for horizontal scaling

- **[ADR-0006](0006-stackexchange-redis-client.md)**: StackExchange.Redis Client Library
  - Why StackExchange.Redis over ServiceStack, CSRedis
  - Performance, Aspire integration, community support

- **[ADR-0016](0016-cache-aside-pattern.md)**: Cache-Aside Pattern for Data Access
  - Caching strategy and invalidation
  - Performance vs consistency trade-offs

### Observability & Monitoring

- **[ADR-0007](0007-opentelemetry-for-observability.md)**: OpenTelemetry for Observability
  - Why OpenTelemetry over Application Insights, Serilog, custom
  - Vendor-neutral, distributed tracing, metrics, logging

- **[ADR-0014](0014-health-checks-kubernetes.md)**: Health Checks for Kubernetes Readiness
  - Liveness and readiness probes
  - Kubernetes-compatible health endpoints

### Frontend & API

- **[ADR-0008](0008-blazor-server-frontend.md)**: Blazor Server for Web Frontend
  - Why Blazor Server over WebAssembly, React, Angular, Vue
  - C# full-stack, real-time updates, type safety

- **[ADR-0009](0009-minimal-apis-over-controllers.md)**: Minimal APIs over MVC Controllers
  - Why Minimal APIs over traditional Controllers
  - Performance, simplicity, modern .NET direction

- **[ADR-0017](0017-async-first-api-design.md)**: Async-First API Design
  - Why all endpoints must be async
  - Scalability and performance benefits

- **[ADR-0018](0018-swagger-openapi-documentation.md)**: Swagger/OpenAPI for API Documentation
  - Interactive API documentation
  - Code-first, always up-to-date

### Architecture Patterns

- **[ADR-0011](0011-service-discovery-pattern.md)**: Service Discovery Pattern
  - Why service discovery over hardcoded URLs
  - Aspire-based discovery for development

- **[ADR-0013](0013-servicedefaults-pattern.md)**: ServiceDefaults Pattern for Cross-Cutting Concerns
  - Consistent configuration across services
  - DRY principle for observability, resilience, health checks

- **[ADR-0019](0019-apphost-orchestration.md)**: AppHost for Service Orchestration
  - Development environment orchestration
  - Declarative service composition

### Development & Deployment

- **[ADR-0012](0012-container-first-development.md)**: Container-First Development
  - Why containers for infrastructure
  - Dev/prod parity, clean machine

- **[ADR-0015](0015-stylecop-code-quality.md)**: StyleCop and Analyzers for Code Quality
  - Automated code quality enforcement
  - Consistent style, best practices

- **[ADR-0020](0020-zero-configuration-infrastructure.md)**: Zero-Configuration Infrastructure
  - 5-minute onboarding with no setup
  - Aspire-managed infrastructure

## Decision Categories

### Technology Stack
- Platform: .NET 8
- Framework: ASP.NET Core, Aspire
- Database: PostgreSQL + EF Core
- Cache: Redis + StackExchange.Redis
- Frontend: Blazor Server
- Observability: OpenTelemetry

### Architectural Patterns
- Service Discovery
- Cache-Aside
- ServiceDefaults (Cross-Cutting Concerns)
- Health Checks (Kubernetes-compatible)
- Minimal APIs (vs Controllers)
- Direct DbContext (No Repository)

### Development Practices
- Container-First Development
- Zero-Configuration Infrastructure
- Async-First API Design
- Code Quality (StyleCop, Analyzers)
- API Documentation (Swagger/OpenAPI)

## Creating New ADRs

When making a significant architectural decision:

1. **Copy the template**: Use `adr-template.md` as starting point
2. **Number sequentially**: Next ADR is 0021
3. **Fill in all sections**: Don't skip Context, Alternatives, or Consequences
4. **Link related ADRs**: Reference decisions that influenced or relate to this one
5. **Update this README**: Add entry to index and relevant category
6. **Get review**: Discuss with team before marking "Accepted"

## ADR Lifecycle

- **Proposed**: Decision under discussion
- **Accepted**: Decision made and implemented
- **Deprecated**: No longer follows this decision
- **Superseded**: Replaced by newer ADR (link to replacement)

## Best Practices

1. **Write when you decide**, not months later
2. **Include alternatives** - show you considered multiple options
3. **Be honest about trade-offs** - every decision has pros and cons
4. **Link extensively** - to related ADRs, PRs, issues, docs
5. **Keep concise** - ADRs are documentation, not novels
6. **Use present tense** - "We will use X" not "We used X"

## Further Reading

- [Michael Nygard's ADR concept](https://cognitect.com/blog/2011/11/15/documenting-architecture-decisions)
- [ADR GitHub Organization](https://adr.github.io/)
- [Markdown Architectural Decision Records](https://github.com/joelparkerhenderson/architecture-decision-record)
- [ThoughtWorks on ADRs](https://www.thoughtworks.com/radar/techniques/lightweight-architecture-decision-records)

---

**Total ADRs**: 20
**Last Updated**: 2025-12-02
**Status**: All Accepted ✅
