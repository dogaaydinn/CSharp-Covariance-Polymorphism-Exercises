# Microservice Template

> Clean architecture microservice with CQRS pattern using MediatR.

## Architecture
- **CQRS** - Command Query Responsibility Segregation
- **MediatR** - Mediator pattern for decoupling
- **Minimal API** - Modern .NET 8 endpoints

## Layers
```
Application/     # Use cases (queries, commands)
Domain/          # Business logic, entities
Infrastructure/  # Data access, external services
API/             # HTTP endpoints
```

## Run
```bash
dotnet run
curl http://localhost:5000/orders/1
```
