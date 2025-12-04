# 🚀 Full Application Samples

This directory contains complete, production-ready applications that demonstrate advanced C# concepts in real-world scenarios. Unlike snippets, these are fully functional applications you can deploy and use.

## 📂 Applications

### RealWorld
Production-grade applications solving real business problems:

- **MicroserviceTemplate** - Complete microservice template with:
  - Clean Architecture
  - CQRS with MediatR
  - Entity Framework Core
  - API documentation (Swagger)
  - Health checks
  - Docker support

- **WebApiAdvanced** - Advanced Web API featuring:
  - JWT Authentication
  - Rate limiting
  - Caching strategies
  - Pagination
  - Filtering and sorting
  - API versioning
  - OpenAPI/Swagger

- **MLNetIntegration** - Machine Learning integration:
  - ML.NET model training
  - Prediction API
  - Model evaluation
  - Real-time inference

### CloudNative
Cloud-native applications using modern patterns:

- **AspireVideoService** - .NET Aspire video platform:
  - Distributed architecture
  - Service discovery
  - Observability (OpenTelemetry)
  - Redis caching
  - PostgreSQL database
  - Container orchestration

### CuttingEdge
Latest .NET features and technologies:

- **AspireCloudStack** - Full cloud-native stack:
  - .NET Aspire orchestration
  - Microservices architecture
  - Modern observability
  - Cloud deployment ready

### Capstone
Comprehensive enterprise applications:

- **MicroVideoPlatform** - Video streaming platform:
  - Video processing
  - Analytics service
  - User management
  - Content delivery
  - Scalable architecture

### RealWorldProblems
Solutions to common production challenges:

- **API-Rate-Limiting** - Implementing rate limiting
- **Cache-Strategy** - Effective caching patterns
- **Database-Migration** - Zero-downtime migrations
- **N-Plus-One-Problem** - Query optimization
- **Microservice-Communication** - Service-to-service patterns
- **Microservice-Error-Handling** - Fault tolerance
- **Legacy-Code-Refactoring** - Modernization strategies
- **Production-Incident-Response** - Incident handling

## 🎯 How to Run

Each application includes:
- `README.md` - Application-specific documentation
- `docker-compose.yml` - Container orchestration (if applicable)
- `.env.example` - Environment variables template

### Quick Start

```bash
# Navigate to an application
cd samples/RealWorld/MicroserviceTemplate

# Restore dependencies
dotnet restore

# Build the application
dotnet build

# Run the application
dotnet run
```

### Docker Deployment

```bash
# For applications with Docker support
docker-compose up -d
```

## 📝 Application Standards

All applications follow these standards:

✅ **Production-Ready**
- Error handling
- Logging and monitoring
- Health checks
- Configuration management

✅ **Security**
- Authentication/Authorization
- Input validation
- SQL injection prevention
- CORS configuration

✅ **Performance**
- Caching strategies
- Database optimization
- Async/await patterns
- Resource management

✅ **Testing**
- Unit tests
- Integration tests
- API tests
- Load tests (where applicable)

✅ **Documentation**
- API documentation
- Deployment guides
- Architecture diagrams
- Troubleshooting guides

## 🔗 See Also

- [Code Snippets (snippets/)](../snippets/README.md) - Focused learning examples
- [Main Documentation](../README.md) - Project overview
- [Architecture Decisions](../docs/architecture/) - ADRs
- [Deployment Guides](../docs/guides/) - Production deployment

## 🛠️ Development

### Prerequisites

- .NET 8.0 SDK
- Docker Desktop (for containerized apps)
- PostgreSQL (for database apps)
- Redis (for caching apps)

### CI/CD

All samples are validated in CI:
- Build verification
- Test execution
- Docker image builds
- Deployment readiness checks

See [`.github/workflows/validate-samples.yml`](../.github/workflows/validate-samples.yml) for details.
