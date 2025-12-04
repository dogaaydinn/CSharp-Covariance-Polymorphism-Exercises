# 2. Adopt Clean Architecture Layering

**Status:** ✅ Accepted

**Date:** 2024-12-01

**Deciders:** Architecture Team, Tech Lead

**Technical Story:** Implementation in `samples/05-RealWorld/MicroserviceTemplate`

---

## Context and Problem Statement

As our microservices grow in complexity, we need a standardized architecture that ensures:
- Clear separation of concerns
- Testable business logic
- Independence from frameworks and databases
- Ability to change UI or database without affecting business rules

Traditional layered architecture (Controller → Service → Repository) works for simple apps but doesn't scale well. We need an architecture that puts **business logic at the center** and makes external concerns (UI, database, external services) **dependent on business logic**, not the other way around.

---

## Decision Drivers

* **Testability** - Business logic must be testable without database or UI
* **Maintainability** - Changes should be isolated to specific layers
* **Framework Independence** - Business logic shouldn't depend on ASP.NET or EF Core
* **Database Independence** - Should be able to swap PostgreSQL for SQL Server easily
* **Team Onboarding** - New developers should quickly understand structure
* **Industry Standards** - Architecture should be widely recognized

---

## Considered Options

* **Option 1** - Clean Architecture (Onion Architecture / Hexagonal Architecture)
* **Option 2** - Traditional N-Tier Layered Architecture
* **Option 3** - Vertical Slice Architecture
* **Option 4** - Domain-Driven Design (DDD) Layers

---

## Decision Outcome

**Chosen option:** "Clean Architecture", because it enforces the Dependency Inversion Principle, makes business logic the core of the application, and has proven track record in enterprise microservices.

### Positive Consequences

* **Testable Business Logic** - Domain layer has zero external dependencies
* **Framework Agnostic** - Can change from ASP.NET to gRPC without touching business logic
* **Database Agnostic** - Can swap EF Core for Dapper or ADO.NET
* **Clear Boundaries** - Each layer has well-defined responsibilities
* **Maintainability** - Changes are isolated; changing UI doesn't break business logic
* **Screaming Architecture** - Project structure screams what the app does (Products, Orders, etc.)

### Negative Consequences

* **More Files** - More layers means more files and folders
* **Learning Curve** - Developers need to understand dependency flow
* **Boilerplate** - DTOs, interfaces, and mappers required
* **Overkill for Simple Apps** - Unnecessary complexity for CRUD apps

---

## Pros and Cons of the Options

### Clean Architecture (Chosen)

**Layer Structure:**
```
┌─────────────────────────────────────┐
│   Presentation Layer (API/Web)      │  ← Depends on Application
├─────────────────────────────────────┤
│   Application Layer (Use Cases)     │  ← Depends on Domain
├─────────────────────────────────────┤
│   Domain Layer (Business Logic)     │  ← NO dependencies!
├─────────────────────────────────────┤
│   Infrastructure Layer (Data/IO)    │  ← Depends on Domain (interfaces)
└─────────────────────────────────────┘
```

**Pros:**
* Business logic is **completely independent** of frameworks
* Easy to test (mock interfaces, not concrete implementations)
* Changes to database don't affect business rules
* **Dependency Inversion** - outer layers depend on inner layers
* Widely documented and understood
* Used by Microsoft, Netflix, and other large companies

**Cons:**
* More folders and files than traditional architecture
* Requires discipline to maintain layer boundaries
* DTOs needed to translate between layers
* Can feel like over-engineering for simple apps

**Example Implementation:**
```
MicroserviceTemplate/
├── Domain/                           ← Core business logic (NO dependencies)
│   ├── Entities/Product.cs
│   ├── ValueObjects/Money.cs
│   ├── Events/ProductCreatedEvent.cs
│   └── Repositories/IProductRepository.cs  ← Interface only!
│
├── Application/                      ← Use cases (depends on Domain)
│   ├── Commands/CreateProductCommand.cs
│   ├── Queries/GetProductQuery.cs
│   ├── DTOs/ProductDto.cs
│   └── Validators/CreateProductValidator.cs
│
├── Infrastructure/                   ← External concerns (implements Domain interfaces)
│   ├── Repositories/ProductRepository.cs   ← Implements IProductRepository
│   ├── Data/ApplicationDbContext.cs
│   └── DependencyInjection.cs
│
└── API/                             ← Presentation layer
    ├── Controllers/ProductsController.cs
    └── Program.cs
```

### Traditional N-Tier Architecture

**Layer Structure:**
```
Controller → Service → Repository → Database
```

**Pros:**
* Simple and straightforward
* Familiar to most developers
* Less files and folders
* Quick to set up for small apps

**Cons:**
* **Business logic mixed with infrastructure** (Services depend on EF Core)
* Hard to test (services tightly coupled to database)
* Controllers depend on concrete services (not interfaces)
* Database changes affect business logic
* No dependency inversion

**Why Rejected:**
```csharp
// ProductService.cs (tightly coupled to EF Core)
public class ProductService
{
    private readonly ApplicationDbContext _context; // ❌ Depends on EF Core!

    public async Task<Product> CreateAsync(Product product)
    {
        _context.Products.Add(product);  // ❌ Business logic mixed with data access
        await _context.SaveChangesAsync();
        return product;
    }
}
```

This approach makes it **impossible to test business logic** without a database. You can't swap EF Core for another ORM without rewriting business logic.

### Vertical Slice Architecture

**Structure:**
```
Features/
├── CreateProduct/
│   ├── CreateProductCommand.cs
│   ├── CreateProductHandler.cs
│   └── CreateProductValidator.cs
├── GetProduct/
│   ├── GetProductQuery.cs
│   └── GetProductHandler.cs
```

**Pros:**
* Features are co-located
* Easy to find everything related to a feature
* No cross-layer navigation needed
* Good for feature teams

**Cons:**
* Shared business logic is difficult to manage
* Domain entities scattered across features
* Hard to enforce consistency
* Less suitable for complex domains

**Why Rejected:**
While this works well for simple CRUD apps, our microservices have **complex business rules** that need to be centralized in a Domain layer. Vertical slices work better for applications with independent features and minimal shared logic.

### DDD Tactical Patterns

**Structure:**
```
Domain/
├── Aggregates/
├── Entities/
├── ValueObjects/
├── DomainServices/
├── DomainEvents/
└── Repositories/
```

**Pros:**
* Extremely expressive and rich domain models
* Handles complex business logic well
* Aggregates enforce consistency boundaries

**Cons:**
* Steep learning curve
* Requires deep domain expertise
* Can be over-engineering for simple domains
* More complex than Clean Architecture

**Why Not Chosen (Yet):**
DDD tactical patterns are **compatible with Clean Architecture** and can be adopted incrementally. We start with Clean Architecture and introduce DDD patterns (Aggregates, Value Objects, Domain Events) as domain complexity grows.

---

## Implementation Guidelines

### Dependency Flow (Critical!)

```csharp
// ✅ CORRECT: Infrastructure depends on Domain
public class ProductRepository : IProductRepository  // Interface from Domain
{
    private readonly ApplicationDbContext _context;
    // Implementation details...
}

// ✅ CORRECT: Application depends on Domain
public class CreateProductHandler : IRequestHandler<CreateProductCommand, ProductDto>
{
    private readonly IProductRepository _repository; // Interface from Domain
    // Use case logic...
}

// ❌ WRONG: Domain depends on Infrastructure
public class Product  // Domain entity
{
    public ApplicationDbContext Context { get; set; } // ❌ Never do this!
}
```

### Layer Responsibilities

**Domain Layer (Core - No Dependencies):**
```csharp
// ✅ Business logic and rules
public class Product
{
    public void ChangePrice(Money newPrice)
    {
        if (newPrice.Amount <= 0)
            throw new DomainException("Price must be positive");

        Price = newPrice;
        UpdatedAt = DateTime.UtcNow;
    }
}

// ✅ Domain interfaces (implemented by Infrastructure)
public interface IProductRepository
{
    Task<Product> GetByIdAsync(int id);
    Task AddAsync(Product product);
}
```

**Application Layer (Use Cases):**
```csharp
// ✅ Orchestrates domain logic
public class CreateProductHandler
{
    private readonly IProductRepository _repository;

    public async Task<ProductDto> Handle(CreateProductCommand command)
    {
        var product = new Product(command.Name, new Money(command.Price));
        await _repository.AddAsync(product);
        return ProductDto.FromEntity(product);
    }
}
```

**Infrastructure Layer (External Concerns):**
```csharp
// ✅ Implements domain interfaces
public class ProductRepository : IProductRepository
{
    private readonly ApplicationDbContext _context;

    public async Task AddAsync(Product product)
    {
        _context.Products.Add(product);
        await _context.SaveChangesAsync();
    }
}
```

**Presentation Layer (API/Web):**
```csharp
// ✅ Thin controllers
[ApiController]
public class ProductsController
{
    private readonly IMediator _mediator;

    [HttpPost]
    public async Task<ActionResult<ProductDto>> Create(CreateProductCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }
}
```

---

## Links

* [Clean Architecture by Uncle Bob](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)
* [Jason Taylor's Clean Architecture Template](https://github.com/jasontaylordev/CleanArchitecture)
* [Clean Architecture Sample](../../samples/05-RealWorld/MicroserviceTemplate)
* [Onion Architecture by Jeffrey Palermo](https://jeffreypalermo.com/2008/07/the-onion-architecture-part-1/)

---

## Notes

**When to Use Clean Architecture:**
- ✅ Microservices with complex business logic
- ✅ Long-lived applications (5+ years)
- ✅ Applications with changing requirements
- ✅ Applications that need to be testable
- ✅ Applications with multiple UIs (Web + Mobile + Desktop)

**When NOT to Use:**
- ❌ Simple CRUD applications
- ❌ Prototypes or proof-of-concepts
- ❌ Throw-away scripts or tools
- ❌ Applications with no business logic

**Common Mistakes to Avoid:**
1. **Leaking Infrastructure into Domain** - Don't reference EF Core in domain entities
2. **Anemic Domain Models** - Don't make entities just data bags; put logic in them
3. **Too Many Layers** - Don't add layers just for the sake of it
4. **Over-Abstracting** - Not everything needs an interface

**Evolution Path:**
1. **Start:** Clean Architecture with simple entities
2. **Grow:** Add Value Objects as domain complexity increases
3. **Mature:** Introduce Aggregates and Domain Events when needed
4. **Advanced:** Adopt full DDD tactical patterns for complex domains

**Review Date:** 2025-12-01
