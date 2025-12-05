# MicroVideoPlatform

> Production-ready microservices platform for video streaming with event-driven architecture.

## Architecture

```
┌─────────────┐
│  Web UI     │ (Blazor WebAssembly)
│  (Port 5000)│
└──────┬──────┘
       │
       ▼
┌─────────────────────┐
│   API Gateway       │ (YARP)
│   (Port 8080)       │
└──────┬──────────────┘
       │
       ├─────────────────┬──────────────────┐
       ▼                 ▼                  ▼
┌─────────────┐   ┌──────────────┐   ┌────────────────┐
│ Content.API │   │ Processing   │   │  Analytics     │
│ (Port 5001) │   │  .Worker     │   │  .Function     │
└─────────────┘   └──────────────┘   └────────────────┘
       │                 │                   │
       └─────────────────┴───────────────────┘
                         │
                         ▼
                  ┌─────────────┐
                  │  RabbitMQ   │ (Event Bus)
                  │  (Port 5672)│
                  └─────────────┘
```

## Services

### 1. ApiGateway (YARP)
- Routes requests to microservices
- Load balancing and retry policies
- Authentication/Authorization gateway

### 2. Content.API
- Video upload and metadata management
- CQRS with MediatR
- Clean Architecture (Domain, Application, Infrastructure)

### 3. Processing.Worker
- Background video processing
- Transcoding and thumbnail generation
- Event-driven with RabbitMQ

### 4. Analytics.Function
- Serverless analytics processing
- View count aggregation
- Azure Functions runtime

### 5. Web.UI
- Blazor WebAssembly frontend
- Video player and catalog
- Real-time updates with SignalR

## Technologies

- **.NET 8** - Platform
- **YARP** - API Gateway (reverse proxy)
- **MediatR** - CQRS pattern
- **RabbitMQ** - Event bus
- **SignalR** - Real-time updates
- **Docker** - Containerization
- **Kubernetes** - Orchestration

## Quick Start

### Run with Docker Compose
```bash
cd samples/08-Capstone/MicroVideoPlatform

# Start all services
docker-compose up -d

# Check services
docker-compose ps

# View logs
docker-compose logs -f
```

### Access Services
- **Web UI**: http://localhost:5000
- **API Gateway**: http://localhost:8080
- **Content API**: http://localhost:5001
- **RabbitMQ Management**: http://localhost:15672 (guest/guest)

### Stop Services
```bash
docker-compose down
```

## Run Locally (Development)

### Prerequisites
```bash
# Install RabbitMQ
docker run -d --name rabbitmq -p 5672:5672 -p 15672:15672 rabbitmq:3-management
```

### Run Services
```bash
# Terminal 1: Content API
cd src/Content.API
dotnet run

# Terminal 2: Processing Worker
cd src/Processing.Worker
dotnet run

# Terminal 3: Analytics Function
cd src/Analytics.Function
func start

# Terminal 4: API Gateway
cd src/ApiGateway
dotnet run

# Terminal 5: Web UI
cd src/Web.UI
dotnet run
```

## Deploy to Kubernetes

### 1. Build Docker Images
```bash
cd samples/08-Capstone/MicroVideoPlatform

# Build all images
docker build -t microvideo/content-api:latest -f src/Content.API/Dockerfile .
docker build -t microvideo/processing-worker:latest -f src/Processing.Worker/Dockerfile .
docker build -t microvideo/api-gateway:latest -f src/ApiGateway/Dockerfile .
docker build -t microvideo/web-ui:latest -f src/Web.UI/Dockerfile .
```

### 2. Deploy to Kubernetes
```bash
# Apply all manifests
kubectl apply -f k8s/

# Check deployments
kubectl get deployments
kubectl get services
kubectl get pods

# Access via LoadBalancer
kubectl get service api-gateway
# Visit: http://<EXTERNAL-IP>:8080
```

### 3. Scale Services
```bash
# Scale Content API to 3 replicas
kubectl scale deployment content-api --replicas=3

# Auto-scale based on CPU
kubectl autoscale deployment content-api --min=2 --max=10 --cpu-percent=70
```

## Event-Driven Architecture

### Events
```csharp
public record VideoUploadedEvent(Guid VideoId, string Title, string Url);
public record VideoProcessedEvent(Guid VideoId, string ProcessedUrl);
public record VideoViewedEvent(Guid VideoId, DateTime ViewedAt);
```

### Flow
1. **Upload Video** → Content.API publishes `VideoUploadedEvent`
2. **Process Video** → Processing.Worker consumes event, processes video
3. **Video Processed** → Worker publishes `VideoProcessedEvent`
4. **Track Analytics** → Analytics.Function consumes `VideoViewedEvent`

## CQRS Pattern

### Commands (Write Operations)
```csharp
public record UploadVideoCommand(string Title, string Url) : IRequest<Guid>;

public class UploadVideoCommandHandler : IRequestHandler<UploadVideoCommand, Guid>
{
    public async Task<Guid> Handle(UploadVideoCommand request, CancellationToken ct)
    {
        var video = new Video(request.Title, request.Url);
        await _repository.AddAsync(video);
        await _eventBus.PublishAsync(new VideoUploadedEvent(video.Id, video.Title, video.Url));
        return video.Id;
    }
}
```

### Queries (Read Operations)
```csharp
public record GetVideoByIdQuery(Guid Id) : IRequest<VideoDto>;

public class GetVideoByIdQueryHandler : IRequestHandler<GetVideoByIdQuery, VideoDto>
{
    public async Task<VideoDto> Handle(GetVideoByIdQuery request, CancellationToken ct)
    {
        var video = await _repository.GetByIdAsync(request.Id);
        return new VideoDto(video.Id, video.Title, video.Url);
    }
}
```

## Clean Architecture

```
Content.API/
├── src/
│   ├── Domain/              (Entities, Value Objects)
│   │   ├── Entities/
│   │   │   └── Video.cs
│   │   └── Events/
│   │       └── VideoUploadedEvent.cs
│   ├── Application/         (Use Cases, CQRS)
│   │   ├── Commands/
│   │   │   └── UploadVideoCommand.cs
│   │   └── Queries/
│   │       └── GetVideoByIdQuery.cs
│   ├── Infrastructure/      (Data Access, External Services)
│   │   ├── Repositories/
│   │   └── EventBus/
│   └── API/                 (Controllers, Startup)
│       └── Program.cs
└── tests/
    ├── Domain.Tests/
    ├── Application.Tests/
    └── API.Tests/
```

## Testing

### Unit Tests
```bash
cd tests/Content.API.Tests
dotnet test
```

### Integration Tests
```bash
# Start dependencies
docker-compose -f docker-compose.test.yml up -d

# Run integration tests
cd tests/Integration.Tests
dotnet test

# Cleanup
docker-compose -f docker-compose.test.yml down
```

## Monitoring

### Health Checks
- **Content API**: http://localhost:5001/health
- **API Gateway**: http://localhost:8080/health
- **Processing Worker**: http://localhost:5002/health

### Metrics (Prometheus)
```bash
# Expose metrics endpoint
curl http://localhost:5001/metrics
```

### Distributed Tracing (OpenTelemetry)
```bash
# View traces in Jaeger
docker run -d -p 16686:16686 jaegertracing/all-in-one
# Visit: http://localhost:16686
```

## Project Structure

```
MicroVideoPlatform/
├── src/
│   ├── ApiGateway/          (YARP reverse proxy)
│   ├── Content.API/         (Video metadata, CQRS)
│   ├── Processing.Worker/   (Background jobs)
│   ├── Analytics.Function/  (Serverless analytics)
│   └── Web.UI/              (Blazor WebAssembly)
├── k8s/                     (Kubernetes manifests)
│   ├── content-api.yaml
│   ├── api-gateway.yaml
│   └── rabbitmq.yaml
├── tests/
│   ├── Content.API.Tests/
│   └── Integration.Tests/
├── docker-compose.yml
└── README.md
```

## Key Features

### API Gateway (YARP)
- Dynamic routing
- Load balancing
- Circuit breaker
- Rate limiting
- Authentication

### Event-Driven
- Asynchronous processing
- Loose coupling
- Scalable architecture
- Fault tolerance

### CQRS
- Separate read/write models
- Optimized queries
- Event sourcing ready
- Clean separation of concerns

### Microservices
- Independent deployment
- Technology diversity
- Fault isolation
- Horizontal scaling

## Production Considerations

### Security
- API Gateway authentication
- Service-to-service mTLS
- Secret management (Key Vault)
- RBAC in Kubernetes

### Scalability
- Horizontal pod autoscaling
- Message queue buffering
- Database read replicas
- CDN for static assets

### Resilience
- Circuit breakers
- Retry policies
- Health checks
- Graceful degradation

### Observability
- Centralized logging (Seq, ELK)
- Distributed tracing (Jaeger)
- Metrics (Prometheus + Grafana)
- Alerting (AlertManager)

**Use Cases:** Video streaming platforms, e-learning systems, enterprise microservices, cloud-native applications.
