# C4 Model: System Context Diagram

## Overview

The System Context diagram shows the Advanced C# Concepts system and how it fits into the world around it. This is a high-level view showing the system's users and external dependencies.

## Diagram

```mermaid
C4Context
    title System Context Diagram - Advanced C# Concepts

    Person(developer, "Developer", "Software engineer learning advanced C# concepts")
    Person(educator, "Educator", "Teacher using examples for training")
    Person(contributor, "Contributor", "Open source contributor")

    System(advancedConcepts, "Advanced C# Concepts", "Educational framework demonstrating enterprise-grade C# patterns, performance optimization, and best practices")

    System_Ext(nuget, "NuGet.org", "Package distribution platform")
    System_Ext(github, "GitHub", "Source control and CI/CD")
    System_Ext(dockerhub, "GitHub Container Registry", "Container image registry")
    System_Ext(seq, "Seq", "Structured log aggregation")
    System_Ext(prometheus, "Prometheus", "Metrics collection")
    System_Ext(grafana, "Grafana", "Metrics visualization")

    Rel(developer, advancedConcepts, "Uses", "Learns advanced C# concepts")
    Rel(educator, advancedConcepts, "Uses", "Teaches from examples")
    Rel(contributor, advancedConcepts, "Contributes to", "HTTPS/Git")

    Rel(advancedConcepts, nuget, "Publishes packages to", "HTTPS")
    Rel(advancedConcepts, github, "Hosted on", "HTTPS/Git")
    Rel(advancedConcepts, dockerhub, "Publishes images to", "HTTPS")
    Rel(advancedConcepts, seq, "Sends logs to", "HTTP")
    Rel(advancedConcepts, prometheus, "Exposes metrics to", "HTTP")
    Rel(grafana, prometheus, "Queries metrics from", "HTTP")

    UpdateLayoutConfig($c4ShapeInRow="3", $c4BoundaryInRow="2")
```

## System Users

### Primary Users

#### Developer
- **Role:** Individual learning advanced C# concepts
- **Goals:**
  - Learn covariance and contravariance
  - Understand polymorphism patterns
  - Master performance optimization
  - Study enterprise architecture patterns
- **Interactions:**
  - Reads documentation
  - Runs code examples
  - Executes benchmarks
  - Studies test patterns

#### Educator
- **Role:** Teacher or trainer using the system for education
- **Goals:**
  - Teach advanced C# concepts
  - Demonstrate best practices
  - Show real-world examples
  - Provide hands-on exercises
- **Interactions:**
  - Uses examples in courses
  - References documentation
  - Assigns exercises to students
  - Demonstrates patterns

#### Contributor
- **Role:** Open source contributor improving the system
- **Goals:**
  - Add new examples
  - Improve documentation
  - Fix bugs
  - Enhance performance
- **Interactions:**
  - Forks repository
  - Creates pull requests
  - Reviews code
  - Participates in discussions

## External Systems

### NuGet.org
- **Purpose:** Package distribution platform
- **Interaction:** System publishes NuGet packages
- **Protocol:** HTTPS
- **Frequency:** On new releases
- **Packages:**
  - AdvancedConcepts.Core
  - Future: Additional libraries

### GitHub
- **Purpose:** Source control, CI/CD, and collaboration
- **Interactions:**
  - Source code hosting
  - Issue tracking
  - Pull request reviews
  - CI/CD pipelines (GitHub Actions)
  - Container registry (GHCR)
  - Documentation hosting (GitHub Pages)
- **Protocol:** HTTPS/Git
- **Frequency:** Continuous

### GitHub Container Registry
- **Purpose:** Docker image hosting
- **Interaction:** System publishes Docker images
- **Protocol:** HTTPS
- **Frequency:** On new releases and commits to main
- **Images:**
  - `ghcr.io/dogaaydinn/advancedconcepts:latest`
  - `ghcr.io/dogaaydinn/advancedconcepts:v1.0.0`

### Seq
- **Purpose:** Structured log aggregation and analysis
- **Interaction:** System sends structured logs
- **Protocol:** HTTP
- **Frequency:** Real-time (async)
- **Port:** 5341
- **Environment:** Development and staging

### Prometheus
- **Purpose:** Metrics collection and storage
- **Interaction:** System exposes metrics endpoint
- **Protocol:** HTTP
- **Frequency:** Scraped every 15 seconds
- **Port:** 9090
- **Metrics:**
  - Request counters
  - Duration histograms
  - Active connections gauge

### Grafana
- **Purpose:** Metrics visualization and dashboards
- **Interaction:** Queries Prometheus for metrics
- **Protocol:** HTTP
- **Frequency:** Real-time
- **Port:** 3000
- **Dashboards:**
  - Application performance
  - Resource utilization
  - Error rates

## System Scope

### In Scope
- ✅ Advanced C# concept demonstrations
- ✅ Performance optimization examples
- ✅ Enterprise architecture patterns
- ✅ Testing strategies and examples
- ✅ CI/CD pipeline automation
- ✅ Documentation and tutorials
- ✅ Observability and monitoring

### Out of Scope
- ❌ Production application runtime
- ❌ User authentication/authorization
- ❌ Database persistence
- ❌ External API integrations
- ❌ Payment processing
- ❌ User management

## Technology Context

### Development Environment
- **.NET 8 LTS:** Runtime and SDK
- **C# 12:** Programming language
- **Visual Studio / Rider / VS Code:** IDEs

### Build & Deployment
- **GitHub Actions:** CI/CD platform
- **Docker:** Containerization
- **Kubernetes:** Container orchestration (optional)
- **Helm:** Package management for K8s

### Observability Stack
- **Serilog:** Structured logging
- **OpenTelemetry:** Distributed tracing and metrics
- **Seq:** Log aggregation (development)
- **Prometheus:** Metrics storage
- **Grafana:** Visualization

### Quality & Security
- **6 Analyzers:** Code quality
- **xUnit:** Testing framework
- **BenchmarkDotNet:** Performance testing
- **Stryker.NET:** Mutation testing
- **Snyk, OWASP, Gitleaks:** Security scanning

## Security Considerations

### Authentication
- **GitHub:** OAuth for contributors
- **NuGet.org:** API key for package publishing
- **Container Registry:** GitHub token authentication

### Data Protection
- **No PII collected:** Educational system with no user data
- **Secrets management:** GitHub Secrets for CI/CD
- **Container security:** Non-root user, minimal attack surface

### Network Security
- **HTTPS:** All external communications
- **TLS:** Encrypted connections
- **Firewall:** Kubernetes network policies (when deployed)

## Deployment Context

### Environments

#### Local Development
- Docker Compose with all services
- Console output for logs
- In-memory metrics

#### Staging
- Kubernetes cluster
- Seq for log aggregation
- Prometheus + Grafana for metrics

#### Production (Future)
- Kubernetes cluster with autoscaling
- Cloud-native logging (Application Insights / CloudWatch)
- High availability setup

## References

- [C4 Model](https://c4model.com/)
- [System Context Documentation](https://c4model.com/#SystemContextDiagram)
- [Mermaid C4 Diagrams](https://mermaid.js.org/syntax/c4.html)

---

**Abstraction Level:** Level 1 - System Context
**Target Audience:** Everyone (technical and non-technical)
**Last Updated:** 2025-11-30
