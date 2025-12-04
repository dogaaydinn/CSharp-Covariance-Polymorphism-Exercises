# 1. Use MediatR for CQRS Implementation

**Status:** ✅ Accepted

**Date:** 2024-12-01

**Deciders:** Architecture Team, Senior Engineers

**Technical Story:** Implementation in `samples/05-RealWorld/MicroserviceTemplate`

---

## Context and Problem Statement

The MicroserviceTemplate project requires a clean separation between command operations (writes) and query operations (reads) following the CQRS (Command Query Responsibility Segregation) pattern. We need a mechanism to handle commands and queries without creating tight coupling between request handlers and the controllers.

**Key Requirements:**
- Clear separation of concerns
- Testable request handlers
- Minimal boilerplate code
- Industry-standard pattern
- Support for cross-cutting concerns (validation, logging, transactions)

---

## Decision Drivers

* **Maintainability** - Code should be easy to understand and modify
* **Testability** - Handlers should be independently testable
* **Industry Standards** - Pattern should be widely recognized
* **Developer Experience** - Should reduce boilerplate code
* **Extensibility** - Should support behaviors (pipeline pattern)

---

## Considered Options

* **Option 1** - MediatR library
* **Option 2** - Manual CQRS implementation (custom mediator)
* **Option 3** - Direct controller-to-service pattern (no mediator)

---

## Decision Outcome

**Chosen option:** "MediatR library", because it provides a battle-tested implementation of the mediator pattern with excellent support for CQRS, minimal boilerplate, and extensive community adoption.

### Positive Consequences

* **Clean Code** - Controllers become thin, handlers are focused on single responsibility
* **Testability** - Each handler can be unit tested in isolation
* **Pipeline Behaviors** - Built-in support for cross-cutting concerns (validation, logging, etc.)
* **Industry Recognition** - Widely used pattern that any .NET developer can understand
* **Documentation** - Extensive official and community documentation available
* **Integration** - Works seamlessly with dependency injection

### Negative Consequences

* **External Dependency** - Adds NuGet package dependency to the project
* **Learning Curve** - Team members unfamiliar with CQRS need to learn the pattern
* **Over-Engineering Risk** - Can be overkill for simple CRUD operations
* **Reflection Overhead** - Minimal performance cost due to handler resolution via reflection

---

## Pros and Cons of the Options

### MediatR Library

[Chosen Option]

**Pros:**
* Battle-tested with millions of downloads
* Excellent documentation and community support
* Built-in pipeline behaviors for cross-cutting concerns
* Reduces boilerplate code significantly
* Supports both synchronous and asynchronous handlers
* Works with ASP.NET Core dependency injection out-of-the-box

**Cons:**
* External dependency (NuGet package)
* Adds small runtime overhead due to reflection
* Requires understanding of the mediator pattern
* Can be seen as over-engineering for very simple apps

**Example Usage:**
```csharp
// Command
public record CreateProductCommand(string Name, decimal Price) : IRequest<ProductDto>;

// Handler
public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, ProductDto>
{
    public async Task<ProductDto> Handle(CreateProductCommand request, CancellationToken ct)
    {
        // Business logic here
    }
}

// Controller (thin)
[HttpPost]
public async Task<ActionResult<ProductDto>> Create([FromBody] CreateProductCommand command)
{
    var result = await _mediator.Send(command);
    return Ok(result);
}
```

### Manual CQRS Implementation

**Pros:**
* No external dependencies
* Full control over implementation
* Can be optimized for specific use cases
* No reflection overhead

**Cons:**
* Requires significant boilerplate code
* Need to implement handler registration manually
* Need to implement pipeline behaviors from scratch
* Requires ongoing maintenance
* Not industry-standard (harder for new developers)
* Estimated 500+ lines of infrastructure code

**Why Rejected:**
While this gives full control, it requires implementing everything that MediatR provides out-of-the-box. The maintenance burden and lack of standardization outweigh the benefits.

### Direct Controller-to-Service Pattern

**Pros:**
* Simple and straightforward
* No additional abstractions
* Easy for beginners to understand
* No external dependencies

**Cons:**
* Controllers become fat and handle multiple responsibilities
* Difficult to test (need to mock services in controllers)
* No clear separation between commands and queries
* Hard to add cross-cutting concerns
* Violates Single Responsibility Principle
* Difficult to scale as complexity grows

**Example (problematic):**
```csharp
public class ProductsController
{
    private readonly IProductService _productService;
    private readonly IValidator<Product> _validator;
    private readonly ILogger _logger;
    private readonly IDbContext _context;

    [HttpPost]
    public async Task<ActionResult> Create([FromBody] Product product)
    {
        // Validation logic
        var validationResult = await _validator.ValidateAsync(product);
        if (!validationResult.IsValid) return BadRequest();

        // Logging
        _logger.LogInformation("Creating product {Name}", product.Name);

        // Transaction handling
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            // Business logic
            var created = await _productService.CreateAsync(product);
            await transaction.CommitAsync();
            return Ok(created);
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}
```

**Why Rejected:**
This approach doesn't scale. Controllers become cluttered with validation, logging, transaction handling, and business logic. Testing requires mocking multiple services. Adding new features requires modifying controllers.

---

## Implementation Details

### Registration
```csharp
// Program.cs
services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssemblyContaining<Program>();
});

// Optional: Add behaviors
services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
```

### Usage Pattern
```csharp
// Commands modify state
public record CreateProductCommand : IRequest<ProductDto> { }
public record UpdateProductCommand : IRequest<ProductDto> { }
public record DeleteProductCommand : IRequest<Unit> { }

// Queries return data (no side effects)
public record GetProductQuery : IRequest<ProductDto> { }
public record GetAllProductsQuery : IRequest<List<ProductDto>> { }
```

---

## Links

* [MediatR GitHub Repository](https://github.com/jbogard/MediatR)
* [CQRS Pattern by Martin Fowler](https://martinfowler.com/bliki/CQRS.html)
* [MediatR Wiki](https://github.com/jbogard/MediatR/wiki)
* [Sample Implementation](../../samples/05-RealWorld/MicroserviceTemplate)

---

## Notes

**Performance Consideration:**
The reflection overhead of MediatR is negligible in most applications (< 1ms per request). If you're building a ultra-high-performance API where every microsecond matters, consider manual implementation. For 99% of applications, the benefits far outweigh the minimal performance cost.

**When NOT to Use MediatR:**
- Very simple CRUD applications with no business logic
- Prototypes or proof-of-concepts
- Applications with fewer than 5 endpoints
- Teams unfamiliar with CQRS and unwilling to learn

**Future Considerations:**
- Consider adding validation behaviors using FluentValidation
- Consider adding logging behaviors for audit trails
- Consider adding transaction behaviors for multi-entity operations
- Monitor performance in production; optimize if MediatR becomes a bottleneck (unlikely)

**Review Date:** 2025-12-01 (annual review)
