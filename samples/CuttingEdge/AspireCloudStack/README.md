# .NET Aspire Cloud-Native Stack

> **Level:** Cutting-Edge
> **Framework:** .NET Aspire 8.0
> **Topics:** Cloud-Native, Service Discovery, Observability, Container Orchestration

## 🌟 What is .NET Aspire?

**.NET Aspire** is Microsoft's **opinionated, cloud-ready stack** for building **observable, production-ready, distributed applications**. It was announced in November 2023 and represents the future of cloud-native .NET development.

### The Problem .NET Aspire Solves

Traditional microservice development requires manually setting up:
- ❌ Service discovery (How does Service A find Service B?)
- ❌ Configuration management (How do services get connection strings?)
- ❌ Container orchestration (How do I run PostgreSQL, Redis locally?)
- ❌ Observability (How do I see logs, traces, metrics across services?)
- ❌ Resilience (How do I add retry/circuit breaker to HTTP calls?)
- ❌ Health checks (Is my service healthy?)

**.NET Aspire provides ALL of this out-of-the-box** ✅

---

## 🎯 What This Sample Demonstrates

This sample shows a **production-ready cloud-native application** with:

### ✅ Infrastructure Resources (Automatic Container Orchestration)
- **PostgreSQL** (database) - Auto-started in Docker container
- **Redis** (distributed cache) - Auto-started in Docker container
- **PgAdmin** (database UI) - http://localhost:5050
- **Redis Commander** (cache UI) - http://localhost:8081

### ✅ Application Services
- **API Service** (ASP.NET Core Web API)
  - JWT Authentication
  - Rate Limiting (100 req/min)
  - Serilog Structured Logging
  - EF Core with PostgreSQL
  - Redis Distributed Caching
  - Swagger/OpenAPI docs

### ✅ Built-in Observability (Automatic)
- **Aspire Dashboard** - http://localhost:18888
  - Real-time logs from all services
  - Distributed tracing (OpenTelemetry)
  - Metrics (CPU, memory, requests)
  - Service health status
  - Container status

### ✅ Developer Experience Features
- **Service Discovery** - Services find each other automatically
- **Configuration** - Connection strings injected automatically
- **Resilience** - HTTP calls have retry/circuit breaker by default
- **Health Checks** - `/health` and `/alive` endpoints automatic

---

## 🚀 Quick Start

### Prerequisites

```bash
# Required
- .NET 8 SDK (8.0.100 or later)
- Docker Desktop (for containers)

# Optional but recommended
- Visual Studio 2022 17.9+ or Rider 2024.1+
- .NET Aspire workload (optional, sample works without it)
```

### Install .NET Aspire Workload (Recommended)

```bash
# This adds Aspire templates and tooling
dotnet workload update
dotnet workload install aspire
```

### Run the Application

```bash
# Navigate to the AppHost project
cd samples/06-CuttingEdge/AspireCloudStack

# Run the orchestrator (this starts EVERYTHING)
dotnet run --project AspireCloudStack.AppHost
```

**That's it!** The AppHost will:
1. Start PostgreSQL container
2. Start Redis container
3. Start PgAdmin container
4. Start Redis Commander container
5. Build and run API Service (2 replicas)
6. Open Aspire Dashboard at http://localhost:18888

---

## 📊 Aspire Dashboard (http://localhost:18888)

The **Aspire Dashboard** is your **mission control** for the entire application.

### Dashboard Features:

#### 1. **Resources Tab**
```
┌─────────────────────────────────────────────────────┐
│ Resource Name    │ Type      │ State   │ Endpoints  │
├─────────────────────────────────────────────────────┤
│ postgres         │ Container │ Running │ 5432       │
│ postgresdb       │ Database  │ Running │ -          │
│ redis            │ Container │ Running │ 6379       │
│ apiservice       │ Project   │ Running │ https://.. │
│ apiservice-2     │ Project   │ Running │ https://.. │
└─────────────────────────────────────────────────────┘
```

#### 2. **Logs Tab**
- Real-time logs from ALL services
- Filter by resource, level, timestamp
- Search logs with keywords
- Example: See PostgreSQL startup logs, API request logs

#### 3. **Traces Tab** (Distributed Tracing)
```
HTTP Request → API Service → PostgreSQL Query → Redis Cache
     │              │              │                 │
   100ms          20ms           15ms              5ms

Total: 140ms (trace shows exactly where time was spent)
```

#### 4. **Metrics Tab**
- CPU usage per service
- Memory consumption
- HTTP request rates
- Database connection pool stats
- Redis cache hit/miss ratio

#### 5. **Health Tab**
- Service health status (Healthy/Degraded/Unhealthy)
- `/health` endpoint results
- Last health check timestamp

---

## 🔗 Access Points

After running `dotnet run --project AspireCloudStack.AppHost`:

| Service | URL | Credentials |
|---------|-----|-------------|
| **Aspire Dashboard** | http://localhost:18888 | No auth |
| **API Swagger** | http://localhost:5000/swagger | No auth for docs |
| **API Auth** | http://localhost:5000/api/auth/login | admin/admin123 |
| **PgAdmin** | http://localhost:5050 | admin@admin.com / admin |
| **Redis Commander** | http://localhost:8081 | No auth |

---

## 🧪 Test the API

### 1. Get Weather Forecast (No Auth Required)

```bash
curl http://localhost:5000/api/weather?days=5
```

### 2. Get All Products (Cached with Redis)

```bash
curl http://localhost:5000/api/products
```

### 3. Login to Get JWT Token

```bash
curl -X POST http://localhost:5000/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"username": "admin", "password": "admin123"}'

# Response:
{
  "token": "eyJhbGciOiJIUzI1NiIs...",
  "username": "admin",
  "expiresIn": 3600
}
```

### 4. Create Product (Requires Auth)

```bash
curl -X POST http://localhost:5000/api/products \
  -H "Authorization: Bearer YOUR_TOKEN_HERE" \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Gaming Mouse",
    "description": "RGB mouse with 16000 DPI",
    "price": 79.99,
    "stock": 15
  }'
```

### 5. View Trace in Dashboard

1. Go to http://localhost:18888
2. Click **Traces** tab
3. Find your request (e.g., `POST /api/products`)
4. See the complete trace:
   ```
   HTTP Request
   ├─ JWT Validation (5ms)
   ├─ Rate Limit Check (1ms)
   ├─ ProductService.CreateAsync
   │  ├─ PostgreSQL INSERT (12ms)
   │  └─ Redis Cache Invalidate (3ms)
   └─ Response (200 OK)

   Total: 45ms
   ```

---

## 🏗️ Project Structure

```
AspireCloudStack/
│
├── AspireCloudStack.AppHost/               ← Orchestration Layer
│   ├── Program.cs                          ← Defines all resources
│   └── AspireCloudStack.AppHost.csproj
│
├── AspireCloudStack.ApiService/            ← Backend API
│   ├── Controllers/
│   │   ├── ProductsController.cs           ← CRUD with caching
│   │   ├── WeatherController.cs            ← Sample endpoint
│   │   └── AuthController.cs               ← JWT authentication
│   ├── Services/
│   │   ├── ProductService.cs               ← Business logic + Redis
│   │   └── WeatherService.cs
│   ├── Data/
│   │   └── ApplicationDbContext.cs         ← EF Core DbContext
│   ├── Models/
│   │   ├── Product.cs                      ← Entity models
│   │   └── WeatherForecast.cs
│   ├── Program.cs                          ← API startup
│   └── appsettings.json
│
└── AspireCloudStack.ServiceDefaults/       ← Shared Extensions
    ├── Extensions.cs                       ← OpenTelemetry, Health Checks
    └── AspireCloudStack.ServiceDefaults.csproj
```

---

## 🔬 How .NET Aspire Works (Under the Hood)

### 1. Service Discovery (Automatic)

**Without Aspire:**
```csharp
// Hard-coded connection string
services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql("Host=localhost;Port=5432;Database=mydb;Username=user;Password=pass"));
```

**With Aspire:**
```csharp
// Just reference the resource - connection string injected automatically!
builder.AddNpgsqlDbContext<ApplicationDbContext>("postgresdb");

// AppHost provides:
// builder.AddPostgres("postgres").AddDatabase("postgresdb");
```

Aspire **automatically**:
- Starts PostgreSQL container
- Generates connection string
- Injects it into ApiService
- Updates connection string if port changes

### 2. OpenTelemetry (Automatic)

**Without Aspire:**
```csharp
// Manual OpenTelemetry setup (50+ lines of code)
services.AddOpenTelemetry()
    .WithTracing(builder => builder
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation()
        .AddSource("MyApp")
        .AddOtlpExporter(options => {
            options.Endpoint = new Uri("http://jaeger:4317");
        }))
    .WithMetrics(builder => builder
        .AddAspNetCoreInstrumentation()
        .AddRuntimeInstrumentation()
        .AddPrometheusExporter());
```

**With Aspire:**
```csharp
// Single line!
builder.AddServiceDefaults();

// Automatically configures:
// - Traces (distributed tracing)
// - Metrics (performance counters)
// - Logs (structured logging)
// - Exporters (OTLP to Aspire Dashboard)
```

### 3. Resilience (Automatic)

**Without Aspire:**
```csharp
// Manual Polly configuration
services.AddHttpClient("MyClient")
    .AddPolicyHandler(Policy<HttpResponseMessage>
        .Handle<HttpRequestException>()
        .WaitAndRetryAsync(3, attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt))))
    .AddPolicyHandler(Policy.TimeoutAsync<HttpResponseMessage>(TimeSpan.FromSeconds(10)));
```

**With Aspire:**
```csharp
// Automatic! All HttpClients get:
// - Retry (3 attempts with exponential backoff)
// - Circuit breaker (opens after 5 failures)
// - Timeout (10 seconds)
services.ConfigureHttpClientDefaults(http =>
{
    http.AddStandardResilienceHandler(); // Aspire adds this automatically
});
```

---

## 🆚 Comparison: Traditional vs Aspire

### Scenario: Add PostgreSQL to Your App

#### Traditional Approach (❌ Complex):

```bash
# 1. Manually create docker-compose.yml
# 2. Add connection string to appsettings.json
# 3. Install EF Core packages
# 4. Configure DbContext in Program.cs
# 5. Run docker-compose up
# 6. Run migrations
# 7. Hope connection string matches everywhere

docker-compose up -d postgres
dotnet ef migrations add Initial
dotnet ef database update
dotnet run
```

**Problems:**
- Connection string mismatches (localhost vs container name)
- Forgot to start Docker? App crashes
- No observability into PostgreSQL
- Different setup for Dev vs Production
- Developers need to know Docker commands

#### Aspire Approach (✅ Simple):

```csharp
// In AppHost/Program.cs:
var postgres = builder.AddPostgres("postgres");
var postgresdb = postgres.AddDatabase("postgresdb");

// In ApiService/Program.cs:
builder.AddNpgsqlDbContext<ApplicationDbContext>("postgresdb");
```

**That's it!** Aspire handles:
- ✅ Starting PostgreSQL container
- ✅ Generating connection string
- ✅ Injecting connection string
- ✅ Running migrations
- ✅ Observability (logs, traces, health)
- ✅ Same code works in Dev, Test, Prod

---

## 📈 Why .NET Aspire is a Game-Changer

### 1. **Eliminates "Works on My Machine" Syndrome**

Before:
```
Developer 1: "My app works!"
Developer 2: "Mine doesn't, PostgreSQL port conflict"
Developer 3: "What's the Redis password?"
```

With Aspire:
```bash
# Everyone runs the same command:
dotnet run --project AppHost

# Everyone gets identical environment:
# - PostgreSQL on auto-assigned port
# - Redis on auto-assigned port
# - All connection strings injected automatically
```

### 2. **Production Parity**

The **same AppHost code** works in:
- ✅ Local development (containers)
- ✅ CI/CD pipelines (containers)
- ✅ Kubernetes (manifests generated from AppHost)
- ✅ Azure Container Apps (native support)

```csharp
// This code runs EVERYWHERE:
var postgres = builder.AddPostgres("postgres");
var redis = builder.AddRedis("redis");
var api = builder.AddProject<Projects.ApiService>("api")
    .WithReference(postgres)
    .WithReference(redis);
```

### 3. **Built-in Best Practices**

Aspire **enforces** production best practices:
- ✅ OpenTelemetry (observability)
- ✅ Health checks (liveness/readiness)
- ✅ Resilience (retry/circuit breaker)
- ✅ Service discovery
- ✅ Configuration management
- ✅ Structured logging

You **can't** build an Aspire app without these. They're automatic.

### 4. **Incredible Developer Experience**

```
Traditional Stack         →  .NET Aspire Stack
────────────────────────────────────────────────────────
docker-compose.yml       →  AppHost/Program.cs
.env files               →  Automatic injection
kubectl apply -f *.yaml  →  Aspire generates manifests
Manual service discovery →  Automatic
50+ lines of OTel setup  →  1 line: AddServiceDefaults()
Separate logging setup   →  Built-in
Manual health checks     →  Automatic
Polly boilerplate        →  Automatic resilience
```

---

## 🎓 Key Aspire Concepts

### 1. **Resources**

Resources are **things your app depends on**:
- Containers (PostgreSQL, Redis, RabbitMQ)
- Projects (your APIs, web apps)
- Cloud services (Azure Storage, Azure Service Bus)

```csharp
// Define resources in AppHost:
var postgres = builder.AddPostgres("postgres");  // Container resource
var redis = builder.AddRedis("redis");           // Container resource
var api = builder.AddProject<Projects.Api>("api"); // Project resource
```

### 2. **References**

References create **dependencies between resources**:

```csharp
var api = builder.AddProject<Projects.Api>("api")
    .WithReference(postgres)  // API depends on PostgreSQL
    .WithReference(redis);    // API depends on Redis

// Aspire automatically:
// 1. Starts postgres and redis BEFORE api
// 2. Injects connection strings into api
// 3. Waits for postgres/redis to be healthy
```

### 3. **Service Defaults**

Service Defaults are **shared configurations** for all services:
- OpenTelemetry
- Health checks
- Service discovery
- Resilience

```csharp
// In ServiceDefaults/Extensions.cs:
builder.AddServiceDefaults(); // Adds all of the above

// Every service gets:
// - Distributed tracing
// - Metrics collection
// - Structured logging
// - Retry/circuit breaker
// - Health endpoints
```

---

## 🚀 Deployment to Production

### Azure Container Apps (Recommended)

Aspire has **first-class support** for Azure Container Apps:

```bash
# Install Azure Developer CLI
winget install microsoft.azd

# Deploy to Azure (one command!)
azd up

# Aspire automatically:
# - Creates Azure Container Apps
# - Creates Azure PostgreSQL
# - Creates Azure Redis
# - Configures networking
# - Sets up OpenTelemetry → Application Insights
# - Deploys all services
```

### Kubernetes

Generate Kubernetes manifests from AppHost:

```bash
# Install Aspire deployment tool
dotnet tool install -g aspirate

# Generate manifests
aspirate generate

# Output:
# manifests/
#   ├── postgres-deployment.yaml
#   ├── postgres-service.yaml
#   ├── redis-deployment.yaml
#   ├── redis-service.yaml
#   ├── api-deployment.yaml
#   └── api-service.yaml

# Deploy to Kubernetes
kubectl apply -f manifests/
```

---

## 📚 Additional Features Demonstrated

### 1. **Redis Distributed Caching**

```csharp
// ProductService.cs
public async Task<Product?> GetByIdAsync(int id)
{
    // Try cache first
    var cached = await _cache.GetStringAsync($"product:{id}");
    if (cached != null)
        return JsonSerializer.Deserialize<Product>(cached);

    // Cache miss - fetch from database
    var product = await _context.Products.FindAsync(id);

    // Store in cache for 5 minutes
    await _cache.SetStringAsync($"product:{id}",
        JsonSerializer.Serialize(product),
        new DistributedCacheEntryOptions {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
        });

    return product;
}
```

### 2. **JWT Authentication**

```csharp
// Login
POST /api/auth/login
{
  "username": "admin",
  "password": "admin123"
}

// Response
{
  "token": "eyJhbGc...",
  "username": "admin",
  "expiresIn": 3600
}

// Use token
GET /api/products
Authorization: Bearer eyJhbGc...
```

### 3. **Rate Limiting**

```csharp
// Program.cs
builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("fixed", opt =>
    {
        opt.PermitLimit = 100;  // 100 requests
        opt.Window = TimeSpan.FromMinutes(1); // per minute
    });
});

// ProductsController.cs
[EnableRateLimiting("fixed")]
public class ProductsController : ControllerBase
{
    // All endpoints limited to 100 req/min
}
```

### 4. **Entity Framework Core with PostgreSQL**

```csharp
// Automatic migrations on startup
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await dbContext.Database.MigrateAsync();
}

// Seeded data available immediately
var products = await context.Products.ToListAsync();
// Returns: Laptop, Mouse, Keyboard (from seed data)
```

---

## 🎯 Learning Outcomes

After completing this sample, you will understand:

✅ **What .NET Aspire is** and why it's revolutionary
✅ **How service discovery works** (automatic connection strings)
✅ **How to orchestrate containers** (PostgreSQL, Redis) without docker-compose
✅ **How OpenTelemetry provides observability** (traces, metrics, logs)
✅ **How to use the Aspire Dashboard** (mission control for your app)
✅ **How to build cloud-native APIs** with Aspire patterns
✅ **How to deploy Aspire apps** to Azure and Kubernetes
✅ **Why this is the future** of cloud-native .NET development

---

## 🔗 Related Samples

- **[WebApiAdvanced](../../05-RealWorld/WebApiAdvanced/)** - Similar API patterns without Aspire
- **[MicroserviceTemplate](../../05-RealWorld/MicroserviceTemplate/)** - Clean Architecture microservice
- **[ResiliencePatterns](../../03-Advanced/ResiliencePatterns/)** - Manual Polly configuration
- **[ObservabilityPatterns](../../03-Advanced/ObservabilityPatterns/)** - Manual OpenTelemetry setup

Compare this sample with **WebApiAdvanced** to see the difference:
- WebApiAdvanced: Manual setup (docker-compose, connection strings, OTel config)
- AspireCloudStack: Automatic setup (AppHost handles everything)

---

## 📖 Further Learning

### Official Resources
- [.NET Aspire Documentation](https://learn.microsoft.com/en-us/dotnet/aspire/)
- [.NET Aspire GitHub](https://github.com/dotnet/aspire)
- [Aspire Samples](https://github.com/dotnet/aspire-samples)

### Community Resources
- [.NET Blog: Introducing .NET Aspire](https://devblogs.microsoft.com/dotnet/introducing-dotnet-aspire/)
- [Aspire Workshop](https://github.com/dotnet/aspire-workshop)
- [Awesome .NET Aspire](https://github.com/timheuer/awesome-dotnet-aspire)

### Video Tutorials
- [.NET Conf 2023: Announcing .NET Aspire](https://www.youtube.com/watch?v=z1M-7Bms1Jg)
- [Azure Developers: Build Cloud-Native Apps with .NET Aspire](https://www.youtube.com/playlist?list=PLlrxD0HtieHi-2nGdpXL4m5KVZ2u3wDVL)

---

## 💡 Pro Tips

1. **Use Aspire Dashboard for debugging** - It's better than Application Insights for local dev
2. **Reference resources, don't hard-code** - Let Aspire inject connection strings
3. **Use .WithReplicas(n)** - Test load balancing locally
4. **Add .WithPgAdmin()** - Inspect PostgreSQL data visually
5. **Add .WithRedisCommander()** - Inspect Redis cache visually
6. **Check health checks** - Dashboard shows if services are degraded
7. **Export traces** - Use "Export" button in Dashboard to share traces
8. **Deploy to Azure early** - Test production deployment with `azd up`

---

## 🏆 Why This Matters for Your Career

.NET Aspire represents **Microsoft's official cloud-native direction**. Learning it now means:

✅ **Future-proof skills** - This is where .NET is heading
✅ **Competitive advantage** - Few developers know Aspire yet
✅ **Interview differentiator** - "I build cloud-native apps with Aspire"
✅ **Production-ready knowledge** - Deploy to Azure/K8s with confidence
✅ **Modern development practices** - Observability, resilience, service discovery

**This sample shows you're not just a .NET developer - you're a cloud-native engineer.**

---

**Ready to build cloud-native applications?** Run the sample and explore the Aspire Dashboard!

```bash
cd AspireCloudStack
dotnet run --project AspireCloudStack.AppHost

# Then open: http://localhost:18888
```

---

**Last Updated:** December 2024
**Aspire Version:** 8.0.0
**.NET Version:** 8.0 LTS
**Author:** Advanced C# Learning Platform
