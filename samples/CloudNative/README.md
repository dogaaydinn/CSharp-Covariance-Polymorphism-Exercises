# Cloud-Native Samples - .NET Aspire

This directory contains advanced cloud-native application samples built with **.NET Aspire**, Microsoft's modern distributed application development stack.

## What is .NET Aspire?

.NET Aspire is an **opinionated, cloud-ready stack** for building observable, production-ready distributed applications. It provides:

- **🔍 Service Discovery**: Automatic resolution without hardcoded URLs
- **📊 Observability**: Built-in OpenTelemetry (tracing, metrics, logging)
- **🛡️ Resilience**: Automatic retry, circuit breaker, timeout patterns
- **🐳 Infrastructure as Code**: Declarative Redis, PostgreSQL, RabbitMQ, etc.
- **🏥 Health Checks**: Kubernetes-ready liveness/readiness probes
- **🌐 Cloud-Agnostic**: Deploy anywhere (Azure, AWS, GCP, on-prem)

## Samples

### 1. AspireVideoService - Complete Microservices Demo

**Path**: `AspireVideoService/`

A production-ready video service demonstrating:
- Service orchestration with AppHost
- Redis caching with automatic invalidation
- PostgreSQL with Entity Framework Core
- Service discovery between microservices
- Distributed tracing with OpenTelemetry
- Blazor frontend consuming API

**Key Features**:
- ✅ Zero-config infrastructure (Redis, PostgreSQL)
- ✅ Real-time observability dashboard
- ✅ Automatic resilience patterns
- ✅ Cross-service distributed tracing
- ✅ Production-ready health checks

**Start in 5 minutes**:
```bash
cd AspireVideoService
dotnet run --project VideoService.AppHost
```

See `AspireVideoService/QUICKSTART.md` for guided tour.

## Why Cloud-Native?

Traditional monoliths can't scale to modern demands. Cloud-native architecture provides:

1. **Independent Scaling**: Scale services individually
2. **Fault Isolation**: One service failure doesn't crash everything
3. **Technology Diversity**: Use the best tool for each job
4. **Continuous Deployment**: Deploy services independently
5. **Observability**: Debug production issues with distributed tracing

## Aspire vs. Docker Compose / Kubernetes

### Docker Compose
- ❌ No service discovery
- ❌ No built-in observability
- ❌ Manual health checks
- ✅ Simple for basic multi-container apps

### Kubernetes
- ✅ Production orchestration
- ❌ Complex local development
- ❌ Steep learning curve
- ❌ Overkill for dev environment

### .NET Aspire
- ✅ **Best of both worlds**
- ✅ Simple local development (like Docker Compose)
- ✅ Production patterns (like Kubernetes)
- ✅ Built-in observability
- ✅ Service discovery out of the box
- ✅ Deploy to any cloud

## Learning Path

### Beginner
1. **Run AspireVideoService** - See Aspire in action
2. **Explore Dashboard** - Understand traces, logs, metrics
3. **Modify an endpoint** - Experience hot reload

### Intermediate
4. **Add a new service** - Practice service orchestration
5. **Implement caching** - Redis patterns
6. **Add health checks** - Kubernetes readiness

### Advanced
7. **Custom observability** - OpenTelemetry customization
8. **Deploy to Azure** - Container Apps deployment
9. **Production patterns** - Circuit breakers, retries

## Project Structure

```
07-CloudNative/
├── AspireVideoService/           # Complete microservices demo
│   ├── VideoService.AppHost/     # Orchestration layer
│   ├── VideoService.ServiceDefaults/  # Shared config
│   ├── VideoService.API/         # REST API service
│   ├── VideoService.Web/         # Blazor frontend
│   ├── README.md                 # Detailed documentation
│   └── QUICKSTART.md             # 5-minute getting started
└── README.md                     # This file
```

## Prerequisites

- **.NET 8.0 SDK** or later
- **Docker Desktop** (for Redis, PostgreSQL containers)
- **Optional**: .NET Aspire workload (`dotnet workload install aspire`)

## Common Commands

```bash
# Run an Aspire app
dotnet run --project <AppHost.csproj>

# Build solution
dotnet build

# Clean and rebuild
dotnet clean && dotnet build

# View running containers
docker ps

# Stop all containers
docker stop $(docker ps -q)
```

## Key Concepts

### AppHost
The orchestrator that defines all services and dependencies:
```csharp
var builder = DistributedApplication.CreateBuilder(args);
var redis = builder.AddRedis("cache");
var api = builder.AddProject("api", "../API/API.csproj")
    .WithReference(redis);
```

### Service Discovery
No more hardcoded URLs:
```csharp
// Instead of: http://localhost:5001
// Use service name: http://api
builder.Services.AddHttpClient("api", client =>
{
    client.BaseAddress = new Uri("http://api");
});
```

### ServiceDefaults
Shared configuration for all services:
```csharp
builder.AddServiceDefaults();  // OpenTelemetry, health checks, resilience
```

### Observability
Built-in distributed tracing:
```csharp
builder.Services.AddOpenTelemetry()
    .WithTracing(tracing =>
        tracing.AddAspNetCoreInstrumentation()
               .AddHttpClientInstrumentation());
```

## Real-World Use Cases

### E-commerce Platform
- Order Service → Inventory Service → Payment Service
- Redis for cart caching
- PostgreSQL for order data
- Service discovery for inter-service communication

### Video Streaming (Our Sample!)
- API Service → Processing Service → Storage Service
- Redis for video metadata caching
- PostgreSQL for user data
- Distributed tracing for debugging playback issues

### IoT Dashboard
- Ingestion Service → Processing Service → Dashboard Service
- Time-series database integration
- Real-time metrics via SignalR
- Circuit breakers for flaky sensors

## Further Reading

- [.NET Aspire Documentation](https://learn.microsoft.com/dotnet/aspire/)
- [OpenTelemetry .NET](https://opentelemetry.io/docs/languages/net/)
- [Microservices Patterns](https://microservices.io/patterns/)
- [Redis Best Practices](https://redis.io/docs/manual/patterns/)
- [Entity Framework Core](https://learn.microsoft.com/ef/core/)

## Deployment

### Azure Container Apps
```bash
azd init
azd up  # One command deployment!
```

### Kubernetes
Generate manifests:
```bash
dotnet publish /t:GenerateKubernetesManifests
kubectl apply -f bin/Release/net8.0/manifests/
```

### Docker Containers
Build and push:
```bash
dotnet publish --os linux --arch x64 -p:PublishProfile=DefaultContainer
docker push <your-registry>/<image-name>
```

## Troubleshooting

### Port Conflicts
```bash
# Check what's using ports
lsof -i :5000
docker ps
```

### Container Issues
```bash
# View container logs
docker logs <container-id>

# Restart Docker Desktop
# macOS: Docker Desktop → Restart
# Windows: Docker Desktop → Settings → Reset
```

### Aspire Dashboard Not Opening
Check terminal output for actual URL (port may vary)

## Contributing

To add a new sample:
1. Create directory under `07-CloudNative/`
2. Follow Aspire project structure
3. Add comprehensive README.md
4. Include QUICKSTART.md
5. Update this README.md

## License

Educational samples provided as-is.

---

**Built with .NET 8 + Aspire 13.0** | **Production-Ready Patterns** | **Cloud-Agnostic Architecture**
