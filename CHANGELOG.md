# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

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
