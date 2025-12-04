# Microservice Template - Clean Architecture

> **Level:** Real-World  
> **Architecture:** Clean Architecture (Domain-Driven Design)  
> **Patterns:** CQRS, MediatR, Repository, Unit of Work

## 📚 Overview

Production-ready microservice template following Clean Architecture principles with Domain-Driven Design, CQRS pattern, and dependency injection.

## 🏗️ Architecture Layers

```
┌─────────────────────────────────────┐
│           API Layer                  │  ← Controllers, Middleware
│  (Presentation - Web API)            │
├─────────────────────────────────────┤
│        Application Layer             │  ← Use Cases, CQRS
│  (Business Logic - MediatR)          │
├─────────────────────────────────────┤
│          Domain Layer                │  ← Entities, Value Objects
│  (Core Business Rules)               │
├─────────────────────────────────────┤
│      Infrastructure Layer            │  ← EF Core, External Services
│  (Data Access, External)             │
└─────────────────────────────────────┘
```

## 🎯 Key Features

- ✅ **Clean Architecture** - Dependency inversion, testability
- ✅ **CQRS Pattern** - Separate read/write operations
- ✅ **MediatR** - Request/response pipeline
- ✅ **Repository Pattern** - Data access abstraction
- ✅ **Validation** - FluentValidation
- ✅ **API Versioning** - v1, v2 support
- ✅ **Health Checks** - Liveness/readiness probes
- ✅ **Swagger/OpenAPI** - Auto-generated documentation
- ✅ **Docker Support** - Containerization ready

## 🚀 Quick Start

```bash
cd samples/05-RealWorld/MicroserviceTemplate
dotnet build
dotnet run --project src/Api

# Navigate to https://localhost:5001/swagger
```

## 📊 Project Structure

```
MicroserviceTemplate/
├── src/
│   ├── Api/                    # Web API (Controllers, Middleware)
│   │   ├── Controllers/
│   │   ├── Middleware/
│   │   └── Program.cs
│   ├── Application/            # Use Cases (Commands, Queries)
│   │   ├── Commands/
│   │   ├── Queries/
│   │   └── Services/
│   ├── Domain/                 # Business Logic (Entities, Rules)
│   │   ├── Entities/
│   │   └── ValueObjects/
│   └── Infrastructure/         # External Concerns (DB, Services)
│       ├── Persistence/
│       └── Services/
├── tests/
│   ├── UnitTests/
│   └── IntegrationTests/
└── docker-compose.yml
```

## 🔑 Design Principles

### SOLID Principles
- **S**ingle Responsibility
- **O**pen/Closed  
- **L**iskov Substitution
- **I**nterface Segregation
- **D**ependency Inversion

### Clean Architecture Rules
1. Dependencies point inward
2. Domain has no dependencies
3. Application depends only on Domain
4. Infrastructure depends on Application

## 📈 Real-World Benefits

**Testability:**
- Domain logic: 100% unit testable
- Application logic: Mockable dependencies
- API: Integration testable

**Maintainability:**
- Clear separation of concerns
- Easy to find and modify code
- Minimal coupling

**Scalability:**
- CQRS allows independent read/write scaling
- Stateless design
- Container-ready

## 🔗 Further Reading

- [Clean Architecture](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html) by Uncle Bob
- [Domain-Driven Design](https://www.domainlanguage.com/ddd/) by Eric Evans
- [CQRS Pattern](https://martinfowler.com/bliki/CQRS.html) by Martin Fowler

---

**Production-Ready Microservice Template! 🚀**
