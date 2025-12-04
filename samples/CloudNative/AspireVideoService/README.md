# .NET Aspire Video Service - Cloud-Native Microservices Example

A production-ready cloud-native video service built with .NET Aspire, demonstrating modern distributed system patterns including service orchestration, service discovery, observability, caching, and resilience.

## What is .NET Aspire?

.NET Aspire is Microsoft's **orchestrator-agnostic cloud-native development stack** that provides:
- **Service Discovery**: Automatic resolution of service endpoints without hardcoding URLs
- **Observability**: Built-in OpenTelemetry for distributed tracing, metrics, and logging
- **Dependency Management**: Declarative infrastructure (Redis, PostgreSQL, etc.) with zero configuration
- **Health Checks**: Kubernetes-ready liveness and readiness probes
- **Resilience**: Automatic retry, circuit breaker, and timeout patterns

Unlike deploying to Azure or AWS, Aspire is a **local development experience** that works with any cloud platform or on-premises deployment.

## Architecture Overview

This sample demonstrates a microservices architecture with:

```
┌─────────────────────────────────────────────────────────────┐
│                     .NET Aspire AppHost                     │
│                    (Orchestration Layer)                    │
└────────────┬───────────────────────────────┬────────────────┘
             │                               │
     ┌───────▼────────┐              ┌──────▼──────┐
     │   Blazor Web   │              │   API       │
     │   Frontend     │─────────────▶│  Service    │
     └────────────────┘              └──────┬──────┘
                                            │
                              ┌─────────────┼─────────────┐
                              │             │             │
                     ┌────────▼─────┐  ┌───▼───┐  ┌──────▼──────┐
                     │ PostgreSQL   │  │ Redis │  │ Processing  │
                     │  Database    │  │ Cache │  │  Service    │
                     └──────────────┘  └───────┘  └─────────────┘
```

### Projects

1. **VideoService.AppHost**: Orchestration project that defines all services and dependencies
2. **VideoService.ServiceDefaults**: Shared library with default configurations (OpenTelemetry, health checks, resilience)
3. **VideoService.API**: RESTful Web API with PostgreSQL and Redis integration
4. **VideoService.Web**: Blazor Server frontend that consumes the API

## Key Features Demonstrated

### 1. Service Discovery
No hardcoded URLs! Services reference each other by name:
```csharp
builder.Services.AddHttpClient<VideoProcessingClient>(client =>
{
    client.BaseAddress = new Uri("http://videoprocessing"); // Resolved automatically!
});
```

### 2. Infrastructure as Code
Declare infrastructure dependencies in `AppHost`:
```csharp
var redis = builder.AddRedis("cache").WithRedisCommander();
var postgres = builder.AddPostgres("postgres").WithPgAdmin().AddDatabase("videodb");
```

### 3. Observability
Built-in OpenTelemetry integration:
- **Distributed Tracing**: Track requests across services
- **Metrics**: Runtime, HTTP, and ASP.NET Core metrics
- **Logging**: Structured logging with correlation IDs

### 4. Resilience Patterns
Automatic retry and circuit breaker via Polly:
```csharp
builder.Services.ConfigureHttpClientDefaults(http =>
{
    http.AddStandardResilienceHandler(); // Retry + Circuit Breaker + Timeout
});
```

### 5. Health Checks
Kubernetes-compatible endpoints:
- `/health` - Overall health
- `/alive` - Liveness probe

### 6. Caching Strategy
Redis caching with automatic invalidation:
```csharp
// Cache videos for 5 minutes
await cache.StringSetAsync("videos:all",
    JsonSerializer.Serialize(videos),
    TimeSpan.FromMinutes(5));
```

## Prerequisites

- .NET 8.0 SDK or later
- Docker Desktop (for Redis and PostgreSQL containers)
- **Optional**: .NET Aspire workload (`dotnet workload install aspire`)

## Getting Started

### 1. Clone and Navigate
```bash
cd samples/07-CloudNative/AspireVideoService
```

### 2. Restore Dependencies
```bash
dotnet restore
```

### 3. Run the Application
```bash
dotnet run --project VideoService.AppHost
```

This single command will:
- Start the AppHost orchestrator
- Spin up Redis and PostgreSQL containers
- Launch the API and Web services
- Open the **Aspire Dashboard** at `http://localhost:18888`

### 4. Access the Services

| Service | URL | Description |
|---------|-----|-------------|
| **Aspire Dashboard** | http://localhost:18888 | Observability dashboard |
| **Blazor Web** | http://localhost:5xxx | Frontend application |
| **API** | http://localhost:5xxx | REST API with Swagger |
| **Redis Commander** | http://localhost:8081 | Redis inspection tool |
| **PgAdmin** | http://localhost:5050 | PostgreSQL management |

> Note: Actual ports are assigned dynamically. Check the Aspire Dashboard for exact URLs.

## Aspire Dashboard Features

The Aspire Dashboard (`localhost:18888`) provides:

### Resources Tab
- View all running services and containers
- See service status, URLs, and logs in real-time
- Restart or stop individual services

### Console Logs
- Live streaming logs from all services
- Filter by service or log level
- Search across all logs

### Structured Logs
- OpenTelemetry-formatted logs with correlation
- Filter by trace ID to see related logs

### Traces
- Distributed tracing across all services
- Visualize request flow through the system
- Identify performance bottlenecks

### Metrics
- Real-time metrics dashboards
- HTTP request rates and durations
- .NET runtime metrics (GC, exceptions, etc.)

## How It Works

### Service Discovery in Action

1. **AppHost Registration**:
```csharp
var processingService = builder.AddProject<Projects.VideoService_API>("videoprocessing");
var apiService = builder.AddProject<Projects.VideoService_API>("api")
    .WithReference(processingService); // Creates service discovery binding
```

2. **Client Configuration**:
```csharp
builder.Services.AddHttpClient<VideoProcessingClient>(client =>
{
    client.BaseAddress = new Uri("http://videoprocessing"); // Service name, not URL!
});
```

3. **Automatic Resolution**: Aspire injects the actual endpoint at runtime, handling:
   - Dynamic port assignment
   - Container networking
   - Load balancing (in production scenarios)

### Redis Caching Pattern

```csharp
app.MapGet("/api/videos", async (VideoDbContext db, IDatabase cache) =>
{
    // Try cache first
    var cached = await cache.StringGetAsync("videos:all");
    if (!cached.IsNullOrEmpty)
        return Results.Ok(JsonSerializer.Deserialize<List<Video>>(cached!));

    // Cache miss - query database
    var videos = await db.Videos.ToListAsync();

    // Update cache
    await cache.StringSetAsync("videos:all",
        JsonSerializer.Serialize(videos),
        TimeSpan.FromMinutes(5));

    return Results.Ok(videos);
});
```

### PostgreSQL with EF Core

Aspire automatically configures the connection string:
```csharp
builder.AddNpgsqlDbContext<VideoDbContext>("videodb");
```

No need to manage connection strings manually!

## API Endpoints

### Videos
- `GET /api/videos` - List all videos (cached)
- `GET /api/videos/{id}` - Get video by ID (cached)
- `POST /api/videos` - Create new video
- `POST /api/videos/{id}/view` - Increment view count

### Processing (Internal)
- `POST /api/process/{id}` - Trigger video processing
- `GET /api/status/{id}` - Get processing status

### Health
- `GET /health` - Overall health check
- `GET /alive` - Liveness probe

## Project Structure

```
AspireVideoService/
├── VideoService.AppHost/
│   └── Program.cs              # Service orchestration
├── VideoService.ServiceDefaults/
│   └── Extensions.cs           # Shared configuration
├── VideoService.API/
│   ├── Models/
│   │   └── Video.cs            # Domain model
│   ├── Data/
│   │   └── VideoDbContext.cs   # EF Core context
│   ├── Services/
│   │   └── VideoProcessingClient.cs  # Service discovery client
│   └── Program.cs              # API endpoints
├── VideoService.Web/
│   ├── Components/
│   │   └── Pages/
│   │       └── Videos.razor    # Video management UI
│   └── Program.cs              # Blazor configuration
└── README.md                   # This file
```

## Testing the Application

### 1. Add a Video
1. Navigate to http://localhost:5xxx/videos
2. Fill in Title, Description, and URL
3. Click "Add Video"
4. Watch the status change from "Pending" → "Processing" → "Ready"

### 2. Verify Caching
1. Add multiple videos
2. Open the Aspire Dashboard → Traces
3. Observe the first `/api/videos` request hits the database
4. Subsequent requests (within 5 minutes) return from cache instantly

### 3. Service Discovery Demo
1. Create a video (triggers processing service call)
2. Open Aspire Dashboard → Traces
3. See the distributed trace from API → Processing Service
4. Notice no hardcoded URLs in the code!

### 4. Observability
1. Open Aspire Dashboard → Structured Logs
2. Filter by trace ID from a request
3. See all related logs across services
4. Examine the distributed trace timeline

## Production Deployment

While Aspire is primarily a **local development tool**, you can deploy to production by:

1. **Container Registry**: Build Docker images from each project
2. **Kubernetes**: Use Aspire-generated manifests or Helm charts
3. **Azure Container Apps**: Direct deployment via `azd`
4. **Any Cloud**: Aspire is orchestrator-agnostic

Example deployment command:
```bash
azd init
azd up  # Deploys to Azure Container Apps
```

## Key Takeaways

### Why .NET Aspire?
- **No Configuration Hell**: Zero-config Redis, PostgreSQL, and service discovery
- **Local-First Development**: Test entire distributed system on your laptop
- **Production-Ready Patterns**: Observability, resilience, and health checks built-in
- **Cloud-Agnostic**: Works with any cloud provider or on-premises

### What You Learned
1. How to orchestrate multiple services with `AppHost`
2. Service discovery without hardcoded URLs
3. Redis caching integration
4. PostgreSQL with Entity Framework Core
5. Distributed tracing with OpenTelemetry
6. Automatic health checks and resilience patterns

### Beyond "Hello World"
This isn't a toy example. The patterns here are used in production by:
- Microsoft (Azure services)
- Fortune 500 companies
- Startups building cloud-native SaaS

## Troubleshooting

### Port Conflicts
If ports are already in use:
```bash
docker ps  # Check running containers
docker stop <container_id>
```

### Redis/PostgreSQL Not Starting
Ensure Docker Desktop is running:
```bash
docker info  # Should show server information
```

### Service Discovery Not Working
Check the Aspire Dashboard → Resources tab:
- All services should be "Running"
- Click on a service to see its logs

### Clear Cache
```bash
docker exec -it <redis-container> redis-cli FLUSHALL
```

## Further Reading

- [.NET Aspire Documentation](https://learn.microsoft.com/dotnet/aspire/)
- [OpenTelemetry in .NET](https://opentelemetry.io/docs/languages/net/)
- [Polly Resilience Patterns](https://www.pollydocs.org/)
- [Redis Best Practices](https://redis.io/docs/manual/patterns/)

## License

This sample is provided as-is for educational purposes.

---

**Built with .NET 8 + Aspire 13.0** | **Cloud-Native Ready** | **Production Patterns**
