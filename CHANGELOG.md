# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

## [2.1.0] - 2025-11-22

### Fixed
- ⚠️ **CRITICAL**: Fixed .NET version mismatch between main project (8.0) and test project (6.0→8.0)
- ⚠️ **CRITICAL**: Fixed CI/CD pipeline to use .NET 8.0 instead of 6.0 (all 3 workflow occurrences)
- ⚠️ **CRITICAL**: Resolved language version conflict (removed C# 10.0 override, now inherits C# 12.0)
- Enhanced XML documentation for base polymorphism classes (Mammal, Animal, Cat, Dog)

### Changed
- Updated test dependencies to latest stable versions:
  - Microsoft.NET.Test.Sdk: 17.1.0 → 17.11.1
  - xUnit: 2.4.1 → 2.9.2
  - xUnit.runner.visualstudio: 2.4.3 → 2.8.2
- Added `IsTestProject` property to test project configuration

### Added
- ✅ **CODE_REVIEW_REPORT.md** - Comprehensive production-readiness assessment
  - 87/100 overall score (B+)
  - Detailed analysis of all code patterns
  - Performance metrics and benchmarks
  - Security assessment
  - Production readiness sign-off

### Security
- Verified CodeQL security scanning configuration
- Confirmed Docker runs as non-root user
- Validated no hardcoded secrets in codebase

## [2.0.0] - 2025-01-16

### Added
- ✅ **Unit Tests** (42 comprehensive tests with FluentAssertions)
  - `PrimaryConstructorsTests` - Record types and modern C# testing
  - `PatternMatchingTests` - Advanced pattern matching validation
  - `SpanMemoryTests` - Zero-allocation pattern verification
  - `ParallelProcessingTests` - Multi-threading correctness tests
- ✅ **CI/CD Pipeline** (GitHub Actions)
  - Automated build and test on push/PR
  - Code coverage reporting with Codecov
  - Performance benchmarks on master branch
  - Code quality checks with dotnet format
- ✅ **GitHub Templates**
  - Bug report template
  - Feature request template
  - Pull request template
- ✅ **Documentation**
  - CHANGELOG.md (this file)
  - SECURITY.md with vulnerability reporting process
  - CODE_OF_CONDUCT.md for community guidelines
  - Enhanced CONTRIBUTING.md
- ✅ **Advanced C# Features**
  - Modern C# 10/11 patterns (Primary Constructors, Records)
  - Collection expressions and modern syntax
  - Advanced pattern matching (type, property, relational)
- ✅ **High-Performance Computing**
  - Span&lt;T&gt; & Memory&lt;T&gt; zero-allocation patterns
  - Parallel processing (PLINQ, Parallel.For)
  - ArrayPool&lt;T&gt; memory pooling
  - Stack allocation examples
  - Parallel matrix multiplication
  - Custom ref structs for parsing
- ✅ **Performance Benchmarks**
  - BenchmarkDotNet integration
  - Boxing/Unboxing benchmarks
  - Covariance/Contravariance benchmarks
  - Memory allocation profiling
- ✅ **Command-Line Interface**
  - `--help` - Usage information
  - `--basics` - Run basic examples only
  - `--advanced` - Run advanced examples only
  - `--benchmark` - Performance benchmarks

### Changed
- Upgraded from .NET 6.0 to support modern C# 10 features
- Enhanced README with badges, performance metrics, and comprehensive documentation
- Improved code organization (Beginner → Intermediate → Advanced)
- Added XML documentation to all public APIs

### Performance Improvements
- **10x faster** - Generic collections vs ArrayList
- **5x faster** - Span&lt;T&gt; parsing vs traditional String.Split
- **4-8x speedup** - Parallel processing on multi-core CPUs
- **Zero allocations** - Stack-based Span&lt;T&gt; patterns

## [1.0.0] - 2024-XX-XX

### Added
- Initial project structure
- Basic polymorphism examples
- Upcasting and downcasting demonstrations
- Covariance and contravariance basics
- Boxing and unboxing examples
- Type conversion examples

---

## Legend
- **Added** - New features
- **Changed** - Changes in existing functionality
- **Deprecated** - Soon-to-be removed features
- **Removed** - Removed features
- **Fixed** - Bug fixes
- **Security** - Vulnerability fixes

[Unreleased]: https://github.com/dogaaydinn/CSharp-Covariance-Polymorphism-Exercises/compare/v2.0.0...HEAD
[2.0.0]: https://github.com/dogaaydinn/CSharp-Covariance-Polymorphism-Exercises/compare/v1.0.0...v2.0.0
[1.0.0]: https://github.com/dogaaydinn/CSharp-Covariance-Polymorphism-Exercises/releases/tag/v1.0.0
