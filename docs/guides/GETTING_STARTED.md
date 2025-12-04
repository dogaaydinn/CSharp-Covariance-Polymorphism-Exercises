# Getting Started with Advanced C# Concepts

Welcome to Advanced C# Concepts! This guide will help you get up and running quickly.

## Table of Contents

- [Prerequisites](#prerequisites)
- [Installation](#installation)
- [Quick Start](#quick-start)
- [Project Structure](#project-structure)
- [Running Examples](#running-examples)
- [Running Tests](#running-tests)
- [Running Benchmarks](#running-benchmarks)
- [Using Docker](#using-docker)
- [Next Steps](#next-steps)

## Prerequisites

### Required
- **.NET 8 SDK** (8.0.201 or later)
  - Download: https://dotnet.microsoft.com/download/dotnet/8.0
  - Verify: `dotnet --version`

- **Git** for version control
  - Download: https://git-scm.com/downloads
  - Verify: `git --version`

### Recommended
- **IDE:** One of the following
  - [Visual Studio 2022](https://visualstudio.microsoft.com/) (Community, Professional, or Enterprise)
  - [JetBrains Rider](https://www.jetbrains.com/rider/)
  - [Visual Studio Code](https://code.visualstudio.com/) with C# extension

- **Docker Desktop** (optional, for observability stack)
  - Download: https://www.docker.com/products/docker-desktop
  - Verify: `docker --version`

## Installation

### 1. Clone the Repository

```bash
# HTTPS
git clone https://github.com/dogaaydinn/CSharp-Covariance-Polymorphism-Exercises.git

# Or SSH
git clone git@github.com:dogaaydinn/CSharp-Covariance-Polymorphism-Exercises.git

# Navigate to project directory
cd CSharp-Covariance-Polymorphism-Exercises
```

### 2. Restore Dependencies

```bash
# Restore NuGet packages
dotnet restore

# This downloads all required packages (may take a few minutes)
```

### 3. Build the Solution

```bash
# Build in Debug mode
dotnet build

# Or build in Release mode
dotnet build --configuration Release
```

Expected output:
```
Build succeeded.
    0 Warning(s)
    0 Error(s)
```

### 4. Verify Installation

```bash
# Run tests to verify everything works
dotnet test

# Expected: All tests should pass
```

## Quick Start

### Run Your First Example

```bash
# Run the main application
dotnet run --project src/AdvancedConcepts.Core
```

This will execute all example categories in sequence:
1. Beginner examples (Polymorphism basics)
2. Intermediate examples (Boxing, Covariance, Contravariance)
3. Advanced examples (Pattern matching, LINQ, async/await)
4. Enterprise patterns (SOLID, Polly, FluentValidation)

### Explore Specific Topics

The project is organized into progressive difficulty levels:

#### Beginner Level
```bash
# Location: src/AdvancedConcepts.Core/Beginner/
```
- **PolymorphismBasics.cs** - Virtual methods, method overriding
- **InheritanceExamples.cs** - Class hierarchies
- **InterfaceBasics.cs** - Interface implementation

#### Intermediate Level
```bash
# Location: src/AdvancedConcepts.Core/Intermediate/
```
- **BoxingUnboxingExamples.cs** - Value/reference type conversion
- **CovarianceExamples.cs** - Covariant generic interfaces
- **ContravarianceExamples.cs** - Contravariant generic interfaces
- **GenericVarianceExamples.cs** - Combined variance scenarios

#### Advanced Level
```bash
# Location: src/AdvancedConcepts.Core/Advanced/
```
- **PatternMatchingExamples.cs** - Modern C# pattern matching
- **LinqAdvancedExamples.cs** - Advanced LINQ operations
- **AsyncAwaitExamples.cs** - Asynchronous programming
- **PerformancePatterns.cs** - Span<T>, Memory<T>, ArrayPool<T>
- **DIExamples.cs** - Dependency injection patterns
- **PollyExamples.cs** - Resilience and fault handling
- **ValidationExamples.cs** - FluentValidation usage
- **ResultPattern.cs** - Railway-oriented programming
- **EnhancedSerilogExamples.cs** - Structured logging
- **OpenTelemetryExamples.cs** - Distributed tracing & metrics
- **HealthCheckExamples.cs** - Application health monitoring

## Project Structure

```
CSharp-Covariance-Polymorphism-Exercises/
├── src/
│   └── AdvancedConcepts.Core/          # Main application
│       ├── Beginner/                    # Beginner examples
│       ├── Intermediate/                # Intermediate examples
│       ├── Advanced/                    # Advanced examples
│       └── Program.cs                   # Entry point
├── tests/
│   ├── AdvancedConcepts.UnitTests/     # 119 unit tests
│   └── AdvancedConcepts.IntegrationTests/  # 9 integration tests
├── benchmarks/
│   └── AdvancedConcepts.Benchmarks/    # Performance benchmarks
├── docs/
│   ├── architecture/                    # Architecture documentation
│   ├── guides/                          # User guides
│   └── security/                        # Security documentation
├── .github/
│   └── workflows/                       # CI/CD pipelines
├── docker-compose.yml                   # Observability stack
├── Dockerfile                           # Container definition
└── README.md                            # Project overview
```

## Running Examples

### Run All Examples

```bash
dotnet run --project src/AdvancedConcepts.Core
```

### Run Specific Examples (modify Program.cs)

Edit `src/AdvancedConcepts.Core/Program.cs` to focus on specific examples:

```csharp
// Focus on just polymorphism
var polymorphism = new PolymorphismBasics();
polymorphism.Demonstrate();

// Focus on covariance
var covariance = new CovarianceExamples();
covariance.DemonstrateCovariance();
```

### With Logging

```bash
# Run with verbose logging
dotnet run --project src/AdvancedConcepts.Core --verbosity detailed
```

## Running Tests

### Run All Tests

```bash
# Run all tests (unit + integration)
dotnet test

# Run with verbose output
dotnet test --verbosity normal
```

### Run Specific Test Projects

```bash
# Unit tests only
dotnet test tests/AdvancedConcepts.UnitTests

# Integration tests only
dotnet test tests/AdvancedConcepts.IntegrationTests
```

### Run Tests with Coverage

```bash
# Generate code coverage report
dotnet test --collect:"XPlat Code Coverage"

# Coverage report will be in TestResults/ directory
```

### Run Tests in Watch Mode

```bash
# Auto-run tests on file changes
dotnet watch test --project tests/AdvancedConcepts.UnitTests
```

### Run Specific Test Classes

```bash
# Filter by test class name
dotnet test --filter "FullyQualifiedName~BoxingUnboxing"

# Filter by test method name
dotnet test --filter "Name~WhenGiven"
```

## Running Benchmarks

### Prerequisites for Benchmarks
- Build in **Release** mode (required for accurate measurements)
- Close other applications to minimize interference
- Disable power-saving mode
- Run multiple times to establish baseline

### Run All Benchmarks

```bash
dotnet run --project benchmarks/AdvancedConcepts.Benchmarks --configuration Release
```

### Run Specific Benchmark Categories

```bash
# Boxing benchmarks only
dotnet run --project benchmarks/AdvancedConcepts.Benchmarks -c Release -- --filter *Boxing*

# LINQ benchmarks only
dotnet run --project benchmarks/AdvancedConcepts.Benchmarks -c Release -- --filter *Linq*

# Span benchmarks only
dotnet run --project benchmarks/AdvancedConcepts.Benchmarks -c Release -- --filter *Span*
```

### View Benchmark Results

Results are saved in `BenchmarkDotNet.Artifacts/results/`:
- **HTML report:** `results-*.html` (open in browser)
- **Markdown report:** `*-report.md`
- **CSV data:** `*-report.csv`

## Using Docker

### Start Observability Stack

```bash
# Start Seq (logs), Prometheus (metrics), Grafana (dashboards)
docker-compose up -d
```

### Access Services

- **Seq (Logs):** http://localhost:5342
- **Prometheus (Metrics):** http://localhost:9090
- **Grafana (Dashboards):** http://localhost:3000
  - Default credentials: admin/admin

### View Application Logs in Seq

1. Start Docker Compose: `docker-compose up -d`
2. Run application: `dotnet run --project src/AdvancedConcepts.Core`
3. Open Seq: http://localhost:5342
4. See structured logs with queryable properties

### Query Metrics in Prometheus

1. Open Prometheus: http://localhost:9090
2. Try queries:
   - `http_requests_total` - Total HTTP requests
   - `request_duration_seconds` - Request duration histogram
   - `active_connections` - Current active connections

### View Dashboards in Grafana

1. Open Grafana: http://localhost:3000
2. Login (admin/admin)
3. Go to Dashboards
4. Import dashboards or create custom visualizations

### Stop Services

```bash
# Stop all containers
docker-compose down

# Stop and remove volumes
docker-compose down -v
```

## IDE Setup

### Visual Studio 2022

1. Open `AdvancedCsharpConcepts.sln`
2. Build → Build Solution (Ctrl+Shift+B)
3. Test → Run All Tests (Ctrl+R, A)
4. Right-click project → Set as Startup Project
5. Press F5 to run

### JetBrains Rider

1. Open `AdvancedCsharpConcepts.sln`
2. Build → Build Solution (Ctrl+Shift+F9)
3. Run → Run All Tests (Ctrl+T, L)
4. Right-click project → Run (Alt+Shift+F10)

### Visual Studio Code

1. Open project folder
2. Install C# extension (ms-dotnettools.csharp)
3. Press F5 to run
4. View → Command Palette → ".NET: Run All Tests"

## Troubleshooting

### Build Errors

**Error:** SDK not found
```bash
# Solution: Install .NET 8 SDK
# Download from https://dotnet.microsoft.com/download/dotnet/8.0
```

**Error:** Restore failed
```bash
# Solution: Clear NuGet cache and restore
dotnet nuget locals all --clear
dotnet restore
```

### Test Failures

**Flaky integration tests:**
```bash
# Solution: Re-run tests
dotnet test --no-build
```

**Missing dependencies:**
```bash
# Solution: Restore and rebuild
dotnet restore
dotnet build
```

### Docker Issues

**Port already in use:**
```bash
# Solution: Change ports in docker-compose.yml or stop conflicting services
# Check what's using the port:
# Windows: netstat -ano | findstr :5342
# Linux/Mac: lsof -i :5342
```

**Container won't start:**
```bash
# Solution: Pull latest images
docker-compose pull
docker-compose up -d
```

## Next Steps

### Learning Path

1. **Start with Beginner examples**
   - Understand polymorphism basics
   - Learn inheritance patterns
   - Master interface implementation

2. **Move to Intermediate topics**
   - Explore boxing/unboxing
   - Understand covariance
   - Master contravariance

3. **Advance to Complex patterns**
   - Modern pattern matching
   - LINQ optimization
   - Async/await best practices

4. **Study Enterprise patterns**
   - SOLID principles in practice
   - Resilience with Polly
   - Validation with FluentValidation
   - Result pattern (Railway-oriented programming)

5. **Master Performance**
   - Run benchmarks
   - Analyze memory allocation
   - Optimize hot paths
   - Use Span<T> and Memory<T>

### Explore Documentation

- **[Architecture Decision Records](../architecture/01-architecture-decision-records/)** - Understand design decisions
- **[C4 Diagrams](../architecture/02-c4-diagrams/)** - Visualize system architecture
- **[Security Best Practices](../security/BEST_PRACTICES.md)** - Learn security patterns

### Contribute

Want to contribute? See our [Contributing Guide](../../CONTRIBUTING.md) for:
- Development setup
- Coding standards
- Pull request process

### Get Help

- **[GitHub Discussions](https://github.com/dogaaydinn/CSharp-Covariance-Polymorphism-Exercises/discussions)** - Ask questions
- **[Issue Tracker](https://github.com/dogaaydinn/CSharp-Covariance-Polymorphism-Exercises/issues)** - Report bugs
- **[Support Guide](../../SUPPORT.md)** - Get support

## Additional Resources

### Microsoft Documentation
- [C# Programming Guide](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/)
- [.NET Performance Tips](https://docs.microsoft.com/en-us/dotnet/core/whats-new/performance-improvements)
- [Async Programming](https://docs.microsoft.com/en-us/dotnet/csharp/async)

### Books
- "C# in Depth" by Jon Skeet
- "CLR via C#" by Jeffrey Richter
- "Pro .NET Performance" by Sasha Goldshtein

### Online Courses
- [Microsoft Learn - C#](https://learn.microsoft.com/en-us/training/paths/get-started-c-sharp-part-1/)
- [Pluralsight - Advanced C#](https://www.pluralsight.com/paths/csharp)

---

**Ready to dive in? Start with:** `dotnet run --project src/AdvancedConcepts.Core`

**Have questions?** Check our [Support Guide](../../SUPPORT.md) or create a [Discussion](https://github.com/dogaaydinn/CSharp-Covariance-Polymorphism-Exercises/discussions)

Happy coding! 🚀

---

**Last Updated:** 2025-11-30
**Version:** 1.0
