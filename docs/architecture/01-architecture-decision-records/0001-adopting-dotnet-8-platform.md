# ADR-0001: Adopting .NET 8 Platform

**Status:** Accepted
**Date:** 2025-12-02
**Deciders:** Architecture Team
**Technical Story:** Foundation for cloud-native samples

## Context

The project needs a modern, performant platform for building cloud-native microservices with enterprise-grade features. The choice of runtime platform affects performance, tooling, ecosystem support, and long-term maintainability. We need to demonstrate current best practices and modern .NET development.

Key requirements:
- Long-term support (LTS) release
- Cloud-native features and performance
- Modern C# language features
- First-class container support
- Cross-platform deployment
- Strong observability tooling

## Decision

We will use **.NET 8** as the target framework for all projects in the AspireVideoService sample.

Specific choices:
- Target Framework: `net8.0`
- C# Language Version: 12 (implicit with .NET 8)
- Runtime: .NET 8.0 Runtime
- SDK: .NET 8.0 SDK or later

## Consequences

### Positive

- **LTS Support**: .NET 8 is a Long-Term Support release (supported until November 2026)
- **Performance**: Significant performance improvements over .NET 6/7 (20-40% faster in many scenarios)
- **Native AOT**: Support for Native Ahead-of-Time compilation (future optimization path)
- **Modern C#**: Access to C# 12 features (primary constructors, collection expressions, etc.)
- **Aspire Support**: .NET 8 is required for .NET Aspire (13.0+)
- **Container Optimization**: Built-in support for minimal container images and chiseled Ubuntu images
- **Observability**: Enhanced OpenTelemetry integration and metrics APIs
- **JSON Performance**: System.Text.Json improvements with source generators
- **Ecosystem**: All major libraries support .NET 8 (EF Core 8, ASP.NET Core 8, etc.)

### Negative

- **Adoption Barrier**: Requires developers to upgrade from .NET 6/7
- **Tooling Requirements**: Requires Visual Studio 2022 17.8+ or Rider 2023.3+
- **Breaking Changes**: Some APIs deprecated from .NET 6 (minimal migration impact)
- **Container Image Size**: .NET 8 images slightly larger than Alpine .NET 6 (mitigated with chiseled images)

### Neutral

- **Regular Updates**: Monthly security and bug fix patches
- **Migration Path**: Clear upgrade path from .NET 6/7 (standard upgrade process)
- **Learning Curve**: Developers familiar with .NET 6/7 adapt quickly

## Alternatives Considered

### Alternative 1: .NET 6 LTS

**Pros:**
- Mature ecosystem with 3+ years of production use
- Wider adoption in enterprise environments
- Supported until November 2024 (extended support available)
- More conservative choice

**Cons:**
- End of standard support in November 2024 (6 months from project start)
- Lacks .NET Aspire support (requires .NET 8)
- Missing C# 11/12 features
- Lower performance compared to .NET 8
- No Native AOT support

**Why rejected:** End of support too soon, lacks Aspire compatibility, missing modern features needed for cloud-native development.

### Alternative 2: .NET 9 (Preview/RC)

**Pros:**
- Latest features and performance improvements
- Cutting-edge cloud-native tooling
- Shows commitment to latest technology

**Cons:**
- Not LTS (18-month support only)
- Preview/RC status at project start (unstable)
- Limited production adoption
- Breaking changes expected
- Library ecosystem not fully updated

**Why rejected:** Stability concerns for educational samples. .NET 9 GA in November 2024 would be appropriate for future updates, but .NET 8 LTS provides better stability for learning.

### Alternative 3: .NET 7

**Pros:**
- Modern feature set (better than .NET 6)
- Stable and production-ready
- Good performance improvements

**Cons:**
- Not LTS (support ends May 2024 - already ended)
- Short support window makes it unsuitable for samples
- No significant advantage over .NET 8
- .NET Aspire targets .NET 8

**Why rejected:** Already out of support. No reason to choose .NET 7 when .NET 8 LTS is available.

## Related Decisions

- [ADR-0002](0002-using-dotnet-aspire.md): Using .NET Aspire (requires .NET 8)
- [ADR-0009](0009-minimal-apis-over-controllers.md): Minimal APIs leverage .NET 8 improvements
- [ADR-0012](0012-container-first-development.md): .NET 8 container optimizations

## Related Links

- [.NET 8 Announcement](https://devblogs.microsoft.com/dotnet/announcing-dotnet-8/)
- [.NET 8 Performance Improvements](https://devblogs.microsoft.com/dotnet/performance-improvements-in-net-8/)
- [.NET Support Policy](https://dotnet.microsoft.com/platform/support/policy/dotnet-core)
- [What's New in .NET 8](https://learn.microsoft.com/dotnet/core/whats-new/dotnet-8)

## Notes

- Consider upgrading to .NET 9 when it reaches LTS status (November 2025)
- Monitor .NET 8 monthly updates for security patches
- Evaluate Native AOT for API services in future iterations
- C# 12 features should be used where they improve code clarity (primary constructors, collection expressions)
