# Aspire Cloud Stack

> **Cutting-Edge .NET 8 Pattern** - Multi-service distributed application with .NET Aspire orchestration.

## 🎯 What is .NET Aspire?

.NET Aspire is an opinionated, cloud-ready stack for building observable, production-ready distributed applications with .NET 8+.

**Key Features:**
- **Service Orchestration** - Coordinate multiple services
- **Service Discovery** - Automatic service-to-service communication
- **Observability** - Built-in telemetry (OpenTelemetry)
- **Resilience** - Automatic retries, circuit breakers
- **Health Checks** - Readiness and liveness probes

## 🏗️ Project Structure

```
AspireCloudStack/
├── AspireCloudStack.AppHost/          # Orchestration (starts all services)
│   ├── Program.cs                     # Service configuration
│   └── AspireCloudStack.AppHost.csproj
│
├── AspireCloudStack.ServiceDefaults/  # Shared configuration
│   ├── Extensions.cs                  # OpenTelemetry, health checks
│   └── AspireCloudStack.ServiceDefaults.csproj
│
├── AspireCloudStack.ApiService/       # Backend API
│   ├── Program.cs                     # Weather API
│   └── AspireCloudStack.ApiService.csproj
│
└── AspireCloudStack.Web/             # Frontend
    ├── Program.cs                     # Web UI
    └── AspireCloudStack.Web.csproj
```

## 🚀 Quick Start

### Prerequisites
- .NET 8 SDK
- Docker Desktop (for Redis)

### Run the Application

```bash
cd samples/06-CuttingEdge/AspireCloudStack

# Run the AppHost (orchestrates all services)
cd AspireCloudStack.AppHost
dotnet run

# Aspire Dashboard will open automatically at:
# http://localhost:15888
```

**What Happens:**
1. AppHost starts all services
2. Redis container launches automatically
3. ApiService starts on random port
4. Web frontend starts and connects to ApiService
5. Aspire Dashboard shows all services, metrics, traces, logs

## 📊 Aspire Dashboard

Navigate to `http://localhost:15888` to see:

- **Resources** - All running services
- **Console Logs** - Centralized logging
- **Structured Logs** - Filtered log viewing
- **Traces** - Distributed tracing (OpenTelemetry)
- **Metrics** - Performance metrics

## 🔍 How It Works

### 1. AppHost Configuration

```csharp
var builder = DistributedApplication.CreateBuilder(args);

// Add Redis
var cache = builder.AddRedis("cache");

// Add API service with Redis reference
var apiService = builder.AddProject<Projects.AspireCloudStack_ApiService>("apiservice")
    .WithReference(cache);

// Add Web with service discovery
builder.AddProject<Projects.AspireCloudStack_Web>("webfrontend")
    .WithReference(apiService);  // ✅ Automatic service discovery!
```

### 2. Service Defaults

Every service uses shared defaults:

```csharp
builder.AddServiceDefaults();  // Adds:
// - OpenTelemetry (traces, metrics, logs)
// - Health checks (/health, /alive)
// - Service discovery
// - Resilient HTTP clients
```

### 3. Service Discovery

Web calls API **by name**, not URL:

```csharp
builder.Services.AddHttpClient<WeatherApiClient>(client =>
{
    client.BaseAddress = new("https+http://apiservice");  // ✅ Resolved automatically!
});
```

**Magic:** Aspire resolves "apiservice" to actual URL (e.g., `http://localhost:5123`)

### 4. Resilience

All HTTP clients get automatic:
- ✅ **Retries** - 3 attempts with exponential backoff
- ✅ **Circuit Breaker** - Stops calling failing services
- ✅ **Timeout** - 10 second default

```csharp
builder.Services.ConfigureHttpClientDefaults(http =>
{
    http.AddStandardResilienceHandler();  // ✅ All this built-in!
});
```

## 🎓 Key Concepts

### Service Defaults Pattern

**Before Aspire (Manual Setup):**
```csharp
// In EVERY service, manually configure:
builder.Services.AddOpenTelemetry();
builder.Services.AddHealthChecks();
builder.Services.AddHttpClient(...).AddPolicyHandler(...);
// Repeated in 10 services = maintenance nightmare!
```

**With Aspire (One Line):**
```csharp
builder.AddServiceDefaults();  // ✅ Done!
```

### Service References

```csharp
// In AppHost:
var api = builder.AddProject<Projects.ApiService>("api");
var web = builder.AddProject<Projects.Web>("web")
    .WithReference(api);  // ✅ Web can now call "api"

// In Web service:
client.BaseAddress = new("https+http://api");  // ✅ Resolved!
```

### Resource Management

Aspire manages:
- **Containers** - Redis, PostgreSQL, RabbitMQ
- **Projects** - Your .NET services
- **Executables** - Node.js, Python scripts
- **Cloud Resources** - Azure, AWS services

## 📈 Observability

### Distributed Tracing

**Scenario:** Web → API → Redis

1. User requests `/weather`
2. Trace shows:
   ```
   [Web] GET /weather (200ms)
     ↳ [ApiService] GET /weatherforecast (150ms)
       ↳ [Redis] GET cache:weather (5ms)
   ```

**Benefit:** See exactly where time is spent!

### Metrics

Built-in metrics for:
- HTTP requests (count, duration, errors)
- .NET runtime (GC, threads, exceptions)
- Custom metrics (add your own)

### Structured Logs

All logs in one place:
```
[Web] INFO: User requested weather
[ApiService] INFO: Fetching weather data
[Redis] DEBUG: Cache hit for weather:forecast
```

## 🌐 Production Deployment

### Generate Manifests

```bash
dotnet run --project AspireCloudStack.AppHost -- --publisher manifest --output-path manifest.json
```

**Generates:**
- `manifest.json` - Service topology
- Can be deployed to:
  - Azure Container Apps
  - Kubernetes
  - Docker Compose

### Azure Container Apps

```bash
azd init
azd up  # Deploys entire stack to Azure!
```

**Result:**
- All services deployed
- Service discovery configured
- Application Insights enabled
- Auto-scaling configured

## 🔗 Related Technologies

| Technology | Purpose | Aspire Integration |
|------------|---------|-------------------|
| **OpenTelemetry** | Observability standard | Built-in |
| **Service Discovery** | Find services by name | Automatic |
| **Polly** | Resilience policies | Pre-configured |
| **Health Checks** | K8s probes | `/health` endpoint |
| **Docker** | Container management | Automatic |

## 💡 When to Use Aspire

**✅ Perfect For:**
- Microservices (3+ services)
- Cloud-native applications
- Apps needing observability
- Teams wanting consistency

**❌ Not Ideal For:**
- Single monolithic app
- Simple CRUD applications
- Non-.NET services (limited support)

## 🎯 Best Practices

1. **Use ServiceDefaults** - Don't repeat configuration
2. **Name services consistently** - `apiservice`, not `ApiService123`
3. **Reference resources** - Use `WithReference()` for dependencies
4. **Check Aspire Dashboard** - Debug issues visually
5. **Test locally** - All services run in development

## 📚 Learn More

### Official Docs
- [.NET Aspire Overview](https://learn.microsoft.com/en-us/dotnet/aspire/get-started/aspire-overview)
- [Aspire Samples](https://github.com/dotnet/aspire-samples)

### Endpoints

When running:
- **Aspire Dashboard:** http://localhost:15888
- **Web Frontend:** http://localhost:5000 (or check dashboard)
- **API Service:** http://localhost:5001 (or check dashboard)

## 🎉 Summary

.NET Aspire provides:
- ✅ **Orchestration** - Start all services with one command
- ✅ **Service Discovery** - Services find each other automatically
- ✅ **Observability** - Traces, metrics, logs out-of-the-box
- ✅ **Resilience** - Retries, circuit breakers built-in
- ✅ **Developer Experience** - Visual dashboard, fast F5

**Perfect for building modern cloud-native applications with .NET 8!**

---

**Next Steps:**
1. Run `dotnet run` in AppHost
2. Open Aspire Dashboard (http://localhost:15888)
3. Explore traces, logs, metrics
4. Try adding more services!
