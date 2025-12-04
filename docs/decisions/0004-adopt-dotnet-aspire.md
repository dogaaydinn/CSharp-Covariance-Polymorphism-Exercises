# 4. Adopt .NET Aspire for Cloud-Native Stack

**Status:** ✅ Accepted

**Date:** 2024-12-01

**Deciders:** Architecture Team, Platform Engineering Team

**Technical Story:** Implementation in `samples/06-CuttingEdge/AspireCloudStack`

---

## Context and Problem Statement

Building cloud-native distributed applications requires orchestrating multiple services (APIs, databases, caches, message queues) with proper observability (logs, traces, metrics), service discovery, resilience, and configuration management.

**Traditional approach problems:**
- Manual docker-compose files for local development
- Hard-coded connection strings in appsettings.json
- Manual OpenTelemetry configuration (50+ lines)
- Separate configuration for dev/test/prod
- No unified observability dashboard
- Complex service discovery setup

**We need a solution that:**
- Orchestrates containers automatically
- Injects configuration automatically
- Provides built-in observability
- Works identically local → cloud
- Reduces boilerplate significantly

---

## Decision Drivers

* **Developer Experience** - Eliminate manual docker-compose and connection string management
* **Observability** - Built-in distributed tracing, metrics, logs
* **Production Parity** - Same code works local/test/prod
* **Microsoft Support** - First-party Microsoft solution
* **Future-Proof** - Microsoft's official direction for cloud-native .NET

---

## Considered Options

* **Option 1** - .NET Aspire
* **Option 2** - docker-compose + manual configuration
* **Option 3** - Kubernetes + Helm locally (Minikube/K3s)
* **Option 4** - Service Mesh (Istio/Linkerd)

---

## Decision Outcome

**Chosen option:** ".NET Aspire", because it's Microsoft's official cloud-native stack that eliminates 80% of boilerplate while providing production-grade observability and service orchestration.

### Positive Consequences

* **10x Simpler** - No docker-compose, no connection strings, no manual OTel setup
* **Automatic Service Discovery** - Services find each other automatically
* **Built-in Observability** - Traces, metrics, logs with zero configuration
* **Aspire Dashboard** - Amazing local development experience (http://localhost:18888)
* **Production Parity** - Same AppHost code works everywhere
* **Future-Proof** - Microsoft's official direction (GA May 2024)
* **Azure Integration** - Deploy to Azure Container Apps with `azd up`

### Negative Consequences

* **Bleeding Edge** - Preview technology (GA in May 2024)
* **Learning Curve** - New concepts (AppHost, ServiceDefaults, Resources)
* **Limited Ecosystem** - Not all libraries have Aspire integrations yet
* **Tooling Requirement** - Requires .NET 8 SDK

---

## Pros and Cons of the Options

### .NET Aspire (Chosen)

**Architecture:**
```
AppHost (Orchestrator)
├── Defines Resources (Postgres, Redis, etc.)
├── Defines Projects (APIs, Web apps)
└── Manages Dependencies (who depends on what)

ServiceDefaults (Shared Config)
├── OpenTelemetry
├── Health Checks
├── Service Discovery
└── Resilience
```

**Pros:**
* **Eliminates docker-compose** - Containers defined in C# code
* **Automatic injection** - Connection strings, configuration injected automatically
* **Observability out-of-the-box** - Traces, metrics, logs with zero code
* **Aspire Dashboard** - Best-in-class local development dashboard
* **Works everywhere** - Local, Azure, Kubernetes, anywhere containers run
* **Type-safe** - Container configuration in C#, not YAML
* **Production-ready** - Used by Microsoft itself

**Cons:**
* **New technology** - GA in May 2024 (currently preview/RC)
* **Breaking changes possible** - Until GA
* **Requires .NET 8** - Can't use with older versions
* **Limited integrations** - Not all cloud services have Aspire integrations

**Example:**
```csharp
// AppHost/Program.cs - Replaces docker-compose
var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres")
    .WithPgAdmin();  // Adds PgAdmin automatically

var postgresdb = postgres.AddDatabase("postgresdb");

var redis = builder.AddRedis("redis")
    .WithRedisCommander();  // Adds Redis Commander

var api = builder.AddProject<Projects.ApiService>("api")
    .WithReference(postgresdb)  // Automatic connection string injection!
    .WithReference(redis)       // Automatic connection string injection!
    .WithReplicas(2);           // Run 2 instances

var app = builder.Build();
await app.RunAsync();

// That's it! No docker-compose, no connection strings, no manual OTel setup!
```

### docker-compose + Manual Configuration

**Traditional Approach:**

**Pros:**
* Industry standard (everyone knows docker-compose)
* Works with any language/framework
* Mature tooling

**Cons:**
* **Requires docker-compose.yml** (YAML configuration)
* **Manual connection strings** in appsettings.json
* **Manual OpenTelemetry** setup (50+ lines)
* **No automatic service discovery**
* **No unified dashboard** (need to use separate tools)
* **Dev/Prod differences** - Different configuration

**Example (what we avoid):**
```yaml
# docker-compose.yml
services:
  postgres:
    image: postgres:16
    environment:
      POSTGRES_PASSWORD: password
    ports:
      - "5432:5432"

  redis:
    image: redis:7
    ports:
      - "6379:6379"

  api:
    build: ./api
    environment:
      ConnectionStrings__Postgres: "Host=postgres;Database=mydb;Username=postgres;Password=password"
      ConnectionStrings__Redis: "redis:6379"
    depends_on:
      - postgres
      - redis
```

```json
// appsettings.json - Manual connection strings
{
  "ConnectionStrings": {
    "Postgres": "Host=localhost;Port=5432;Database=mydb;Username=postgres;Password=password",
    "Redis": "localhost:6379"
  }
}
```

**Why Rejected:**
This requires **3 files** (docker-compose.yml, appsettings.Development.json, appsettings.Production.json) and **manual synchronization**. Connection strings are duplicated. No automatic service discovery. No observability dashboard.

### Kubernetes + Helm Locally

**Pros:**
* Production-like environment locally
* Learn Kubernetes

**Cons:**
* **Extremely complex** for local development
* Requires Minikube/K3s/Docker Desktop with K8s
* **Slow startup** (minutes, not seconds)
* **Resource intensive** (4GB+ RAM)
* **YAML hell** (100+ lines for simple app)

**Why Rejected:**
Running Kubernetes locally is overkill. It's slow, resource-intensive, and adds unnecessary complexity for development. Aspire provides the benefits (service discovery, observability) without the complexity.

### Service Mesh (Istio/Linkerd)

**Pros:**
* Infrastructure-level resilience
* Advanced traffic management
* Security (mTLS)

**Cons:**
* **Operational complexity** - Requires running control plane
* **Vendor lock-in** to Kubernetes
* **Performance overhead** - Sidecar proxies
* **Overkill** for most applications

**Why Rejected:**
Service meshes solve different problems (service-to-service communication at scale). For our use case, Aspire's application-level features are sufficient and much simpler.

---

## Migration Path

**Phase 1 (Current):**
- Adopt Aspire for new projects (AspireCloudStack sample)
- Document patterns and best practices
- Train team on Aspire concepts

**Phase 2 (Q1 2025):**
- Migrate WebApiAdvanced to Aspire
- Migrate MicroserviceTemplate to Aspire
- Compare before/after

**Phase 3 (Q2 2025):**
- Production deployment to Azure Container Apps
- Evaluate Kubernetes deployment option
- Measure production metrics

---

## Links

* [.NET Aspire Official Docs](https://learn.microsoft.com/en-us/dotnet/aspire/)
* [.NET Aspire GitHub](https://github.com/dotnet/aspire)
* [Aspire Dashboard Demo](https://www.youtube.com/watch?v=z1M-7Bms1Jg)
* [Sample Implementation](../../samples/06-CuttingEdge/AspireCloudStack)

---

## Notes

**When to Use Aspire:**
- ✅ New cloud-native applications
- ✅ Microservices architectures
- ✅ Applications with multiple services (API + DB + Cache + Queue)
- ✅ Need for strong observability
- ✅ Deploying to Azure or Kubernetes

**When NOT to Use:**
- ❌ .NET Framework applications
- ❌ .NET 7 or older
- ❌ Simple monoliths with no external dependencies
- ❌ Applications that can't use preview/RC technology (until GA)

**Cost Analysis:**
- **Docker Compose**: 0 lines of YAML (we pay with complexity)
- **Aspire**: ~30 lines of C# in AppHost

**Observability Comparison:**
| Feature | Manual | Aspire |
|---------|--------|--------|
| Distributed Tracing | 50+ lines | ✅ Automatic |
| Metrics | 30+ lines | ✅ Automatic |
| Logs | 20+ lines | ✅ Automatic |
| Dashboard | Need external tools | ✅ Built-in |
| Setup Time | 2-3 hours | ✅ 5 minutes |

**Review Date:** 2025-06-01 (after GA release)
