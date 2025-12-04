# C4 Model: Container Diagram

## Overview

The Container diagram zooms into the Advanced C# Concepts system and shows the high-level shape of the software architecture and how responsibilities are distributed across it. It shows the major technology choices and how the containers communicate with one another.

## Diagram

```mermaid
C4Container
    title Container Diagram - Advanced C# Concepts

    Person(developer, "Developer", "Learning advanced C# concepts")

    System_Boundary(advancedConcepts, "Advanced C# Concepts System") {
        Container(coreApp, "Core Application", ".NET 8 Console", "Main demonstration application with examples")
        Container(unitTests, "Unit Test Suite", "xUnit", "119 unit tests with comprehensive coverage")
        Container(integrationTests, "Integration Test Suite", "xUnit", "9 integration tests")
        Container(benchmarks, "Performance Benchmarks", "BenchmarkDotNet", "30+ benchmarks across 5 categories")
        Container(docs, "Documentation Site", "DocFX / GitHub Pages", "API documentation and guides")
    }

    System_Boundary(observability, "Observability Stack") {
        ContainerDb(seq, "Seq Server", "Seq", "Structured log aggregation")
        ContainerDb(prometheus, "Prometheus", "Time-Series DB", "Metrics storage")
        Container(grafana, "Grafana", "Visualization", "Dashboards and alerts")
    }

    System_Boundary(cicd, "CI/CD Pipeline") {
        Container(githubActions, "GitHub Actions", "Workflow Engine", "5 automated workflows")
        Container(codeql, "CodeQL", "Security Scanner", "Semantic code analysis")
        Container(stryker, "Stryker.NET", "Mutation Tester", "Test quality validation")
    }

    System_Ext(ghcr, "GitHub Container Registry", "Docker image hosting")
    System_Ext(nuget, "NuGet.org", "Package repository")
    System_Ext(githubPages, "GitHub Pages", "Static site hosting")

    Rel(developer, coreApp, "Runs examples", "CLI")
    Rel(developer, benchmarks, "Executes benchmarks", "CLI")
    Rel(developer, docs, "Reads documentation", "HTTPS")

    Rel(coreApp, seq, "Sends logs", "HTTP/JSON")
    Rel(coreApp, prometheus, "Exposes metrics", "HTTP")
    Rel(grafana, prometheus, "Queries", "HTTP")

    Rel(githubActions, unitTests, "Runs", "dotnet test")
    Rel(githubActions, integrationTests, "Runs", "dotnet test")
    Rel(githubActions, benchmarks, "Executes", "dotnet run")
    Rel(githubActions, codeql, "Triggers", "Workflow")
    Rel(githubActions, stryker, "Executes", "dotnet stryker")

    Rel(githubActions, ghcr, "Publishes images", "HTTPS")
    Rel(githubActions, nuget, "Publishes packages", "HTTPS")
    Rel(githubActions, githubPages, "Deploys docs", "Git")

    UpdateLayoutConfig($c4ShapeInRow="3", $c4BoundaryInRow="1")
```

## Containers

### Core System Containers

#### Core Application
- **Technology:** .NET 8 Console Application
- **Purpose:** Main demonstration application
- **Responsibilities:**
  - Advanced C# concept demonstrations
  - Polymorphism examples
  - Covariance/contravariance patterns
  - Performance optimization examples
  - Enterprise architecture patterns
- **Key Components:**
  - Beginner examples
  - Intermediate examples
  - Advanced examples
  - SOLID principles
  - Design patterns
- **External Dependencies:**
  - Serilog for logging
  - OpenTelemetry for tracing
  - Polly for resilience
  - FluentValidation

#### Unit Test Suite
- **Technology:** xUnit with multiple frameworks
- **Purpose:** Comprehensive unit testing
- **Test Count:** 119 unit tests
- **Frameworks:**
  - xUnit (test framework)
  - FluentAssertions (assertions)
  - Moq + NSubstitute (mocking)
  - AutoFixture + Bogus (test data)
  - FsCheck (property-based testing)
- **Coverage:** 6.57% (baseline for educational codebase)
- **Execution Time:** <10 seconds

#### Integration Test Suite
- **Technology:** xUnit
- **Purpose:** System-level testing
- **Test Count:** 9 integration tests
- **Focus Areas:**
  - Performance integration tests
  - End-to-end scenarios
  - Cross-component interactions
- **Execution Time:** ~5 seconds

#### Performance Benchmarks
- **Technology:** BenchmarkDotNet
- **Purpose:** Performance measurement and regression detection
- **Benchmark Categories:**
  1. Boxing/Unboxing (4 benchmarks)
  2. Polymorphism (4 benchmarks)
  3. LINQ Performance (8 benchmarks)
  4. Span<T> Operations (8 benchmarks)
  5. Type Conversion (6 benchmarks)
- **Output Formats:** HTML, Markdown, CSV, JSON
- **Diagnoser:** MemoryDiagnoser for allocation tracking

#### Documentation Site
- **Technology:** DocFX + GitHub Pages
- **Purpose:** Comprehensive documentation
- **Content:**
  - API reference (auto-generated)
  - Architecture documentation
  - User guides and tutorials
  - Advanced topics
  - Performance guides
- **Hosting:** GitHub Pages (static site)
- **URL:** https://dogaaydinn.github.io/advancedconcepts

### Observability Containers

#### Seq Server
- **Technology:** Seq (Datalust)
- **Purpose:** Structured log aggregation and analysis
- **Port:** 5341 (ingestion), 5342 (UI)
- **Features:**
  - Full-text search
  - Structured querying
  - Real-time tail
  - Alerting
- **Environment:** Development, Staging
- **Data Retention:** 7 days

#### Prometheus
- **Technology:** Prometheus Time-Series Database
- **Purpose:** Metrics collection and storage
- **Port:** 9090
- **Scrape Interval:** 15 seconds
- **Metrics:**
  - Request counters
  - Duration histograms
  - Active connections
  - Error rates
  - Custom business metrics
- **Retention:** 15 days

#### Grafana
- **Technology:** Grafana
- **Purpose:** Metrics visualization and dashboards
- **Port:** 3000
- **Dashboards:**
  - Application Performance
  - Resource Utilization
  - Error Tracking
  - Business Metrics
- **Data Sources:** Prometheus

### CI/CD Containers

#### GitHub Actions
- **Technology:** GitHub Actions Workflow Engine
- **Purpose:** Automated CI/CD pipeline
- **Workflows:**
  1. **ci.yml** - Build, test, coverage (multi-platform)
  2. **cd.yml** - Deploy to staging/production
  3. **release.yml** - Version, package, release
  4. **performance.yml** - Benchmark regression detection
  5. **docs.yml** - Documentation generation
  6. **security.yml** - Security scanning (7 tools)
  7. **codeql.yml** - Semantic code analysis
- **Runners:** ubuntu-latest, windows-latest, macos-latest

#### CodeQL
- **Technology:** GitHub CodeQL
- **Purpose:** Semantic code analysis and security scanning
- **Languages:** C#
- **Schedule:** Weekly
- **Queries:** Default + security-extended
- **Output:** SARIF to GitHub Security tab

#### Stryker.NET
- **Technology:** Mutation Testing Framework
- **Purpose:** Test quality validation
- **Metrics:**
  - Mutation score: 20.07% (baseline)
  - Mutants created: 399
  - Mutants tested: 85
  - Mutants killed: 56
- **Reports:** HTML + JSON + Console

## Container Communication

### Internal Communication

#### Application → Observability
```
Core App → Seq (HTTP/JSON)
├── Structured logs (Serilog)
├── Log levels (Debug, Info, Warning, Error)
└── Context enrichment (Machine, Process, Thread)

Core App → Prometheus (HTTP)
├── Metrics endpoint (/metrics)
├── Counters (requests, errors)
├── Histograms (duration, size)
└── Gauges (active connections)
```

#### CI/CD → Tests
```
GitHub Actions → Unit Tests
├── dotnet test
├── Coverage collection
└── Result publishing

GitHub Actions → Integration Tests
├── dotnet test
├── Environment setup
└── Result publishing

GitHub Actions → Benchmarks
├── dotnet run --project Benchmarks
├── Baseline comparison
└── Regression detection
```

### External Communication

#### GitHub Actions → Registries
```
GitHub Actions → GHCR
├── Docker build
├── Multi-platform images
├── Image push
└── SBOM generation

GitHub Actions → NuGet.org
├── dotnet pack
├── Package validation
├── Package push
└── Symbol packages

GitHub Actions → GitHub Pages
├── DocFX build
├── Static site generation
└── Git push to gh-pages branch
```

## Technology Decisions

### Why .NET 8?
- LTS support until November 2026
- 15-25% performance improvement over .NET 6
- C# 12 language features
- Native AOT support

### Why Console Application?
- **Educational focus:** Demonstrations don't need web UI
- **Simplicity:** Easy to understand and modify
- **Performance:** No HTTP overhead for benchmarks
- **Portability:** Runs anywhere .NET 8 runs

### Why xUnit?
- **Modern:** Parallel test execution by default
- **Extensible:** Rich ecosystem
- **Theory tests:** Data-driven testing support
- **Community:** Industry standard

### Why BenchmarkDotNet?
- **Industry standard:** Used by .NET team
- **Accurate:** Proper warm-up, statistical analysis
- **Rich output:** Multiple export formats
- **Memory profiling:** MemoryDiagnoser

### Why Seq for Development?
- **Structured logging:** Query by properties
- **Easy setup:** Docker container
- **Free:** For development use
- **Rich UI:** Excellent developer experience

### Why Prometheus + Grafana?
- **Industry standard:** Cloud-native metrics stack
- **Pull model:** Application exposes metrics
- **PromQL:** Powerful query language
- **Grafana:** Best-in-class visualization

## Deployment Architecture

### Local Development
```
docker-compose.yml
├── advancedconcepts (application)
├── seq (logs)
├── prometheus (metrics)
└── grafana (visualization)
```

### Staging/Production
```
Kubernetes Cluster
├── Deployment (3 replicas)
├── Service (LoadBalancer)
├── ConfigMap (configuration)
├── Secret (sensitive data)
└── HPA (autoscaling 2-10 pods)
```

## Scaling Considerations

### Application Scaling
- **Horizontal:** Kubernetes HPA (2-10 pods)
- **Vertical:** Resource limits (CPU: 1000m, Memory: 1Gi)
- **Autoscaling triggers:**
  - CPU > 70%
  - Memory > 80%

### Observability Scaling
- **Seq:** Single instance (development only)
- **Prometheus:** Federated setup for production
- **Grafana:** High availability mode (3 replicas)

## Security

### Container Security
- **Non-root user:** Application runs as user 1000
- **Minimal image:** Alpine Linux (~100MB)
- **No secrets in images:** Environment variables only
- **Image scanning:** Trivy in CI/CD

### Network Security
- **TLS:** All external communications
- **Network policies:** Kubernetes isolation
- **Firewalls:** Cloud provider firewalls

## Performance Characteristics

### Container Performance
| Container | Startup | Memory | CPU |
|-----------|---------|--------|-----|
| Core App | <2s | ~50MB | <100m |
| Seq | <5s | ~200MB | <200m |
| Prometheus | <3s | ~300MB | <300m |
| Grafana | <10s | ~150MB | <100m |

### Test Execution
| Test Suite | Duration | Parallelization |
|------------|----------|-----------------|
| Unit Tests | ~8s | Yes (per-class) |
| Integration | ~5s | Yes (per-class) |
| Benchmarks | ~5min | No (sequential) |
| Mutation | ~2min | Yes (parallel) |

## References

- [C4 Container Diagram](https://c4model.com/#ContainerDiagram)
- [.NET Architecture](https://learn.microsoft.com/en-us/dotnet/architecture/)
- [Docker Best Practices](https://docs.docker.com/develop/dev-best-practices/)
- [Kubernetes Patterns](https://kubernetes.io/docs/concepts/)

---

**Abstraction Level:** Level 2 - Container
**Target Audience:** Technical team (developers, architects)
**Last Updated:** 2025-11-30
