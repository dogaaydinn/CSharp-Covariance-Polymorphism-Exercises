# Architecture Decision Records (ADRs)

## What are ADRs?

**Architecture Decision Records (ADRs)** are documents that capture important architectural decisions made along with their context and consequences. They serve as a historical record of the "why" behind the "what" in our codebase.

## Why We Use ADRs

In professional software engineering, documenting architectural decisions is crucial for:

✅ **Knowledge Transfer** - New team members understand why things are the way they are
✅ **Historical Context** - Future developers know what was considered and why certain paths were chosen
✅ **Avoiding Repeated Discussions** - Decisions are documented once, referenced many times
✅ **Critical Thinking** - Forces us to articulate trade-offs and alternatives
✅ **Reversibility** - Provides context for when decisions need to be revisited

## ADR Index

| ADR | Title | Status | Date |
|-----|-------|--------|------|
| [0001](0001-use-mediatr-for-cqrs.md) | Use MediatR for CQRS Implementation | ✅ Accepted | 2024-12 |
| [0002](0002-use-clean-architecture-layers.md) | Adopt Clean Architecture Layering | ✅ Accepted | 2024-12 |
| [0003](0003-use-polly-for-resilience.md) | Use Polly for Resilience Patterns | ✅ Accepted | 2024-12 |
| [0004](0004-adopt-dotnet-aspire.md) | Adopt .NET Aspire for Cloud-Native Stack | ✅ Accepted | 2024-12 |
| [0005](0005-implement-source-generators.md) | Implement Custom Source Generators | ✅ Accepted | 2024-12 |
| [0006](0006-build-custom-roslyn-analyzers.md) | Build Custom Roslyn Analyzers | ✅ Accepted | 2024-12 |
| [0007](0007-choose-postgresql-over-sqlserver.md) | Choose PostgreSQL over SQL Server | ✅ Accepted | 2024-12 |
| [0008](0008-use-redis-for-distributed-caching.md) | Use Redis for Distributed Caching | ✅ Accepted | 2024-12 |
| [0009](0009-adopt-serilog-structured-logging.md) | Adopt Serilog for Structured Logging | ✅ Accepted | 2024-12 |
| [0010](0010-implement-opentelemetry.md) | Implement OpenTelemetry for Observability | ✅ Accepted | 2024-12 |
| [0011](0011-use-jwt-for-authentication.md) | Use JWT Bearer Tokens for Authentication | ✅ Accepted | 2024-12 |
| [0012](0012-use-ef-core-for-data-access.md) | Use Entity Framework Core for Data Access | ✅ Accepted | 2024-12 |
| [0013](0013-use-benchmarkdotnet.md) | Use BenchmarkDotNet for Performance Measurement | ✅ Accepted | 2024-12 |
| [0014](0014-containerize-with-docker.md) | Containerize Applications with Docker | ✅ Accepted | 2024-12 |
| [0015](0015-deploy-to-kubernetes.md) | Deploy to Kubernetes for Production | ✅ Accepted | 2024-12 |

## ADR Template

When creating new ADRs, use the [template](TEMPLATE.md) provided.

## ADR Statuses

- **✅ Accepted** - Decision has been made and is currently in use
- **🟡 Proposed** - Decision is under consideration
- **🔴 Deprecated** - Decision is no longer valid but kept for historical reference
- **🔄 Superseded** - Decision has been replaced by a newer ADR (link to new ADR)

## How to Contribute

When making a significant architectural decision:

1. Copy the [TEMPLATE.md](TEMPLATE.md) file
2. Rename it with the next sequential number (e.g., `0016-your-decision.md`)
3. Fill in all sections thoroughly
4. Submit a pull request
5. After review and approval, update this index

## Questions?

If you have questions about any decision, please:
1. Read the ADR thoroughly
2. Check the "Consequences" and "Alternatives" sections
3. If still unclear, open an issue referencing the ADR number

---

**Remember:** ADRs are living documents. They can be superseded but should never be deleted. Historical context is valuable.
