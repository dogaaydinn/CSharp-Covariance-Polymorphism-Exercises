# Path 2 - Months 3-4 Capstone: E-Commerce Domain Layer with CQRS

**Difficulty**: ⭐⭐⭐⭐☆ (Advanced)
**Estimated Time**: 50-60 hours
**Prerequisites**: Months 1-2 of Path 2 completed

---

## 🎯 Project Overview

Implement a complete e-commerce domain layer using Domain-Driven Design (DDD), CQRS pattern, Event Sourcing, MediatR, and Clean Architecture.

### Learning Objectives

- ✅ Domain-Driven Design principles
- ✅ CQRS (Command Query Responsibility Segregation)
- ✅ Event Sourcing basics
- ✅ MediatR for command/query handling
- ✅ Repository and Unit of Work patterns
- ✅ Clean Architecture layers

---

## 📋 Requirements

### Domain Model

**Aggregates**:
1. **Order** (root)
   - OrderId, CustomerId, OrderDate, Status, TotalAmount
   - OrderItems (value objects)
   - Domain events: OrderPlaced, OrderShipped, OrderCancelled

2. **Customer** (root)
   - CustomerId, Name, Email, LoyaltyPoints
   - ShippingAddresses (value objects)
   - Domain events: CustomerRegistered, AddressAdded

3. **Product** (root)
   - ProductId, Name, Price, StockQuantity
   - Category (value object)
   - Domain events: ProductCreated, StockUpdated

**Value Objects**:
- Money (Amount, Currency)
- Address (Street, City, PostalCode, Country)
- OrderItem (ProductId, Quantity, UnitPrice)

### CQRS Implementation

**Commands** (write operations):
```csharp
// Place order
public record PlaceOrderCommand(
    Guid CustomerId,
    List<OrderItemDto> Items,
    AddressDto ShippingAddress
) : IRequest<Guid>;

// Update stock
public record UpdateStockCommand(
    Guid ProductId,
    int Quantity
) : IRequest<bool>;
```

**Queries** (read operations):
```csharp
// Get order details
public record GetOrderByIdQuery(Guid OrderId) : IRequest<OrderDto>;

// Get customer orders
public record GetCustomerOrdersQuery(
    Guid CustomerId,
    int Page,
    int PageSize
) : IRequest<PaginatedList<OrderSummaryDto>>;
```

**Handlers**:
```csharp
public class PlaceOrderCommandHandler : IRequestHandler<PlaceOrderCommand, Guid>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IProductRepository _productRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDomainEventDispatcher _eventDispatcher;

    public async Task<Guid> Handle(PlaceOrderCommand request, CancellationToken ct)
    {
        // 1. Validate customer exists
        // 2. Validate products and stock
        // 3. Create order aggregate
        // 4. Deduct stock
        // 5. Raise domain events
        // 6. Save to repository
        // 7. Commit unit of work

        var order = Order.Create(request.CustomerId, request.Items);
        await _orderRepository.AddAsync(order);

        order.Place(); // Raises OrderPlaced event

        await _unitOfWork.SaveChangesAsync(ct);
        await _eventDispatcher.DispatchAsync(order.DomainEvents, ct);

        return order.Id;
    }
}
```

### Event Sourcing

```csharp
public abstract class EventSourcedAggregate
{
    private readonly List<IDomainEvent> _domainEvents = new();
    public IReadOnlyList<IDomainEvent> DomainEvents => _domainEvents;

    protected void RaiseEvent(IDomainEvent @event)
    {
        _domainEvents.Add(@event);
        Apply(@event);
    }

    protected abstract void Apply(IDomainEvent @event);

    public void ClearEvents() => _domainEvents.Clear();
}

public class Order : EventSourcedAggregate
{
    public Guid Id { get; private set; }
    public OrderStatus Status { get; private set; }
    private readonly List<OrderItem> _items = new();

    protected override void Apply(IDomainEvent @event)
    {
        switch (@event)
        {
            case OrderPlacedEvent e:
                Id = e.OrderId;
                Status = OrderStatus.Placed;
                break;
            case OrderShippedEvent e:
                Status = OrderStatus.Shipped;
                break;
            // ... more events
        }
    }

    public void Place()
    {
        if (Status != OrderStatus.Draft)
            throw new InvalidOperationException("Order already placed");

        RaiseEvent(new OrderPlacedEvent(Id, DateTime.UtcNow));
    }
}
```

---

## 🏗️ Clean Architecture Structure

```
ECommerce.Domain/            # Core domain logic
├── Aggregates/
│   ├── Order.cs
│   ├── Customer.cs
│   └── Product.cs
├── ValueObjects/
│   ├── Money.cs
│   ├── Address.cs
│   └── OrderItem.cs
├── Events/
│   ├── IDomainEvent.cs
│   ├── OrderPlacedEvent.cs
│   └── StockUpdatedEvent.cs
├── Repositories/            # Interfaces only
│   ├── IOrderRepository.cs
│   └── IProductRepository.cs
└── Exceptions/
    └── DomainException.cs

ECommerce.Application/       # Use cases (CQRS)
├── Commands/
│   ├── PlaceOrderCommand.cs
│   └── UpdateStockCommand.cs
├── Queries/
│   ├── GetOrderByIdQuery.cs
│   └── GetCustomerOrdersQuery.cs
├── Handlers/
│   ├── PlaceOrderCommandHandler.cs
│   └── GetOrderByIdQueryHandler.cs
├── DTOs/
├── Validators/
└── Services/
    └── INotificationService.cs

ECommerce.Infrastructure/    # Implementation
├── Persistence/
│   ├── ECommerceDbContext.cs
│   ├── Repositories/
│   │   ├── OrderRepository.cs
│   │   └── ProductRepository.cs
│   └── UnitOfWork.cs
├── EventStore/
│   └── InMemoryEventStore.cs
└── Services/
    └── EmailNotificationService.cs

ECommerce.Api/               # Web API (thin layer)
└── Controllers/
    ├── OrdersController.cs
    └── ProductsController.cs
```

---

## 🎯 Milestones

1. **Week 1-2**: Domain model with aggregates and value objects
2. **Week 3-4**: CQRS commands and queries with MediatR
3. **Week 5-6**: Event sourcing implementation
4. **Week 7-8**: Clean architecture integration, testing

---

## ✅ Evaluation

| Criteria | Weight |
|----------|--------|
| Domain Model (DDD) | 25% |
| CQRS Implementation | 25% |
| Event Sourcing | 20% |
| Clean Architecture | 15% |
| Tests | 15% |

**Pass**: 75%

---

## 📚 Resources

- DDD: "Domain-Driven Design" by Eric Evans
- CQRS: https://martinfowler.com/bliki/CQRS.html
- MediatR: https://github.com/jbogard/MediatR
- Clean Architecture: https://github.com/jasontaylordev/CleanArchitecture

---

*Template Version: 1.0*
