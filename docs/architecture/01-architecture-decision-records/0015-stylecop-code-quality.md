# ADR-0015: StyleCop and Analyzers for Code Quality

**Status:** Accepted
**Date:** 2025-12-02
**Deciders:** Architecture Team
**Technical Story:** Code quality and consistency enforcement

## Context

C# projects benefit from automated code quality checks for:
- Consistent code style across team
- Early detection of potential bugs
- Best practice enforcement
- Reduced code review time

Without analyzers:
- Inconsistent naming conventions
- Style debates in code reviews
- Bugs slip through
- Lower code quality

## Decision

Enable **StyleCop.Analyzers** and **.NET analyzers** in all projects for automated code quality enforcement.

Configuration:
- StyleCop.Analyzers 1.2.0-beta
- .NET analyzers enabled by default (.NET 8)
- Warnings as informational (not blocking)
- Team can adjust rules via `.editorconfig`

## Consequences

### Positive

- **Consistent Style**: Enforced naming conventions (PascalCase, camelCase)
- **Documentation**: Encourages XML documentation comments
- **Best Practices**: Detects common mistakes (async/await, null checks)
- **Automatic**: IDE shows warnings in real-time
- **Teachable**: New developers learn best practices

### Negative

- **Noise**: 100+ warnings in new projects
- **Opinionated**: Some rules may not fit team preferences
- **Refactoring**: Old code generates many warnings

### Neutral

- **Configuration**: Requires `.editorconfig` for customization

## Alternatives Considered

### Alternative 1: No Analyzers

**Pros:**
- Zero setup
- No warnings

**Cons:**
- Inconsistent code style
- More bugs
- Lower quality

**Why rejected:** Code quality matters.

### Alternative 2: ReSharper/Rider Only

**Pros:**
- Powerful refactoring
- Rich inspections

**Cons:**
- IDE-specific (doesn't work in VS Code)
- Not in CI/CD
- Commercial license ($150/year)

**Why rejected:** Analyzers work everywhere (VS, Rider, VS Code, CI/CD).

## Related Links

- [StyleCop.Analyzers](https://github.com/DotNetAnalyzers/StyleCopAnalyzers)
- [.NET Code Analysis](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/overview)

## Notes

Common warnings:
- SA1633: Missing file header (can disable)
- SA1200: Using directives placement
- MA0004: ConfigureAwait(false) usage

Disable specific rules in `.editorconfig`:
```ini
[*.cs]
dotnet_diagnostic.SA1633.severity = none
```
