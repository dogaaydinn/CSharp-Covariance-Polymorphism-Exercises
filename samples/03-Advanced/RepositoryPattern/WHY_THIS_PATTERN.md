# Why Use the Repository Pattern?

A deep dive into understanding when, why, and how to use the Repository Pattern effectively in .NET applications.

## The Problem: Tight Coupling to Data Access

Imagine you're building an e-commerce application. Without the Repository pattern, your code might look like this:

```csharp
public class ProductService
{
    private readonly AppDbContext _context;

    public ProductService(AppDbContext context)
    {
        _context = context;
    }

    public Product GetProduct(int id)
    {
        return _context.Products.Find(id);  // ← Direct EF Core dependency
    }

    public List<Product> GetExpensiveProducts()
    {
        return _context.Products
                      .Where(p => p.Price > 100)
                      .ToList();  // ← EF Core LINQ
    }

    public void AddProduct(Product product)
    {
        _context.Products.Add(product);
        _context.SaveChanges();  // ← Direct SaveChanges
    }
}

public class OrderService
{
    private readonly AppDbContext _context;

    public void CreateOrder(Order order)
    {
        _context.Orders.Add(order);
        _context.SaveChanges();  // ← Duplicate code
    }
}
```

**Problems:**
1. **Tight coupling to EF Core**: Services directly depend on `DbContext`
2. **Hard to test**: Must mock entire `DbContext` or use real database
3. **Code duplication**: CRUD logic repeated in every service
4. **Mixed concerns**: Business logic and data access in same class
5. **Difficult to change**: Switching from EF to Dapper affects all services

## The Solution: Repository Pattern

```csharp
// 1. Define abstraction
public interface IRepository<T> where T : class
{
    T? GetById(int id);
    IEnumerable<T> GetAll();
    IEnumerable<T> Find(Expression<Func<T, bool>> predicate);
    void Add(T entity);
    void Update(T entity);
    void Delete(int id);
}

// 2. EF Core implementation
public class EfRepository<T> : IRepository<T> where T : class
{
    private readonly AppDbContext _context;
    private readonly DbSet<T> _dbSet;

    public EfRepository(AppDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public T? GetById(int id) => _dbSet.Find(id);
    public IEnumerable<T> GetAll() => _dbSet.ToList();
    public IEnumerable<T> Find(Expression<Func<T, bool>> predicate)
        => _dbSet.Where(predicate).ToList();
    // ... other methods
}

// 3. Service depends on abstraction
public class ProductService
{
    private readonly IRepository<Product> _repository;

    public ProductService(IRepository<Product> repository)
    {
        _repository = repository;  // ← Depends on interface
    }

    public Product GetProduct(int id)
    {
        return _repository.GetById(id);  // ← Clean, testable
    }

    public List<Product> GetExpensiveProducts()
    {
        return _repository.Find(p => p.Price > 100).ToList();
    }
}
```

**Benefits:**
1. **Loose coupling**: Services depend on `IRepository<T>`, not EF Core
2. **Easy to test**: Mock `IRepository<T>` with simple setup
3. **DRY**: CRUD logic in one place (generic repository)
4. **Separation of concerns**: Clear boundaries between layers
5. **Flexible**: Swap implementations (EF → Dapper, SQL → NoSQL)

## When to Use Repository Pattern

### ✅ Use Repository When:

1. **You need testable code**
   ```csharp
   // Easy to mock
   var mockRepo = new Mock<IRepository<Product>>();
   mockRepo.Setup(r => r.GetById(1))
           .Returns(new Product { Id = 1, Name = "Test" });

   var service = new ProductService(mockRepo.Object);
   var product = service.GetProduct(1);
   // Test passes without database!
   ```

2. **You might switch data providers**
   - EF Core → Dapper
   - SQL Server → PostgreSQL
   - Relational → NoSQL (MongoDB, CosmosDB)

3. **You want centralized data access logic**
   ```csharp
   // Add logging, caching, validation in repository
   public class LoggingRepository<T> : IRepository<T> where T : class
   {
       private readonly IRepository<T> _inner;
       private readonly ILogger _logger;

       public T? GetById(int id)
       {
           _logger.LogInformation($"Getting {typeof(T).Name} with ID {id}");
           return _inner.GetById(id);
       }
   }
   ```

4. **You have complex querying needs**
   - Use Specification pattern for reusable, composable queries
   - Encapsulate business rules in specifications

### ❌ Don't Use Repository When:

1. **Simple CRUD application with no tests**
   - Overhead not justified
   - EF Core's `DbContext` is already a repository

2. **You need EF Core-specific features**
   - Change tracking
   - Lazy loading
   - Include/ThenInclude for eager loading
   - Repository abstracts these away

3. **Performance is critical**
   - Every abstraction adds overhead (~5-20%)
   - Direct `DbContext` is faster

4. **Team is small and dedicated to one ORM**
   - No plans to switch providers
   - Everyone knows EF Core well

## Unit of Work Pattern

### The Problem

Without Unit of Work, you have multiple SaveChanges calls:

```csharp
// ❌ BAD: Not atomic
productRepo.Add(product);
context.SaveChanges();  // ← First commit

categoryRepo.Add(category);
context.SaveChanges();  // ← Second commit

// If second commit fails, first is already saved!
// Inconsistent state!
```

### The Solution

```csharp
public interface IUnitOfWork : IDisposable
{
    IRepository<Product> Products { get; }
    IRepository<Category> Categories { get; }
    IRepository<Order> Orders { get; }
    int SaveChanges();  // ← Single commit point
}

// Usage
using var unitOfWork = new UnitOfWork(options);

unitOfWork.Products.Add(product);
unitOfWork.Categories.Add(category);
unitOfWork.Orders.Add(order);

// ✅ GOOD: Atomic commit - all or nothing
unitOfWork.SaveChanges();
```

**Benefits:**
- **Atomic transactions**: All changes commit together or none
- **Consistency**: No partial updates
- **Coordination**: Manages multiple repositories

## Specification Pattern

### The Problem

Business logic leaks into services:

```csharp
// ❌ BAD: Query logic in service
public class ProductService
{
    public List<Product> GetExpensiveProducts()
    {
        return _repository.Find(p => p.Price > 100).ToList();
        // What if "expensive" definition changes?
        // Must update every service that uses this logic!
    }

    public List<Product> GetLowStockProducts()
    {
        return _repository.Find(p => p.Stock < 10 && p.Stock > 0).ToList();
        // Duplicate logic if used elsewhere
    }
}
```

### The Solution

```csharp
// ✅ GOOD: Encapsulate query logic in specifications
public class ExpensiveProductsSpecification : Specification<Product>
{
    public ExpensiveProductsSpecification(decimal threshold = 100m)
        : base(p => p.Price > threshold)
    {
    }
}

public class LowStockSpecification : Specification<Product>
{
    public LowStockSpecification(int threshold)
        : base(p => p.Stock < threshold && p.Stock > 0)
    {
    }
}

// Usage
var expensiveProducts = _repository.Find(new ExpensiveProductsSpecification());
var lowStock = _repository.Find(new LowStockSpecification(10));

// Combine specifications
var expensiveAndLowStock = new AndSpecification<Product>(
    new ExpensiveProductsSpecification(),
    new LowStockSpecification(10)
);
var results = _repository.Find(expensiveAndLowStock);
```

**Benefits:**
- **Reusable**: Use same specification in multiple services
- **Testable**: Test specifications independently
- **Composable**: Combine with AND/OR logic
- **Maintainable**: Business rules in one place

## Real-World Scenario: E-commerce System

### Without Repository (Tightly Coupled)

```csharp
public class OrderService
{
    private readonly AppDbContext _context;

    public void CreateOrder(int productId, int quantity)
    {
        // Directly accessing DbContext everywhere
        var product = _context.Products.Find(productId);
        if (product == null) throw new Exception("Product not found");

        if (product.Stock < quantity)
            throw new Exception("Insufficient stock");

        product.Stock -= quantity;
        _context.Products.Update(product);

        var order = new Order
        {
            ProductId = productId,
            Quantity = quantity,
            Total = product.Price * quantity
        };

        _context.Orders.Add(order);
        _context.SaveChanges();

        // Hard to test - need real database!
        // Tight coupling to EF Core
        // Mixed concerns - business logic + data access
    }
}
```

### With Repository (Loosely Coupled)

```csharp
public class OrderService
{
    private readonly IUnitOfWork _unitOfWork;

    public OrderService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public void CreateOrder(int productId, int quantity)
    {
        // Use repositories
        var product = _unitOfWork.Products.GetById(productId);
        if (product == null) throw new ProductNotFoundException();

        if (product.Stock < quantity)
            throw new InsufficientStockException();

        product.Stock -= quantity;
        _unitOfWork.Products.Update(product);

        var order = new Order
        {
            ProductId = productId,
            Quantity = quantity,
            Total = product.Price * quantity
        };

        _unitOfWork.Orders.Add(order);

        // Atomic commit
        _unitOfWork.SaveChanges();

        // Easy to test - mock IUnitOfWork!
        // Loose coupling to data access
        // Clear separation of concerns
    }
}
```

## Testing Strategies

### Unit Testing with Repository Pattern

```csharp
[Fact]
public void CreateOrder_ReducesStock_WhenOrderCreated()
{
    // Arrange
    var mockUnitOfWork = new Mock<IUnitOfWork>();
    var mockProductRepo = new Mock<IRepository<Product>>();
    var mockOrderRepo = new Mock<IRepository<Order>>();

    var product = new Product { Id = 1, Name = "Laptop", Stock = 10, Price = 999m };

    mockProductRepo.Setup(r => r.GetById(1)).Returns(product);
    mockUnitOfWork.Setup(u => u.Products).Returns(mockProductRepo.Object);
    mockUnitOfWork.Setup(u => u.Orders).Returns(mockOrderRepo.Object);

    var service = new OrderService(mockUnitOfWork.Object);

    // Act
    service.CreateOrder(productId: 1, quantity: 2);

    // Assert
    Assert.Equal(8, product.Stock);  // 10 - 2 = 8
    mockProductRepo.Verify(r => r.Update(product), Times.Once);
    mockOrderRepo.Verify(r => r.Add(It.IsAny<Order>()), Times.Once);
    mockUnitOfWork.Verify(u => u.SaveChanges(), Times.Once);
}
```

### Integration Testing

```csharp
[Fact]
public void Repository_RoundTrip_WorksCorrectly()
{
    // Arrange
    var options = new DbContextOptionsBuilder<AppDbContext>()
        .UseInMemoryDatabase("TestDb")
        .Options;

    using var context = new AppDbContext(options);
    var repo = new EfRepository<Product>(context);

    // Act
    var product = new Product { Name = "Test Product", Price = 99.99m, Stock = 10 };
    repo.Add(product);
    context.SaveChanges();

    var retrieved = repo.GetById(product.Id);

    // Assert
    Assert.NotNull(retrieved);
    Assert.Equal("Test Product", retrieved.Name);
    Assert.Equal(99.99m, retrieved.Price);
}
```

## Common Mistakes

### ❌ Mistake 1: Returning IQueryable

```csharp
// BAD: Leaks EF Core abstraction
public IQueryable<Product> GetAll()
{
    return _dbSet;  // ← Caller can call .Include(), .AsNoTracking(), etc.
}
```

**Why it's bad:** Breaks abstraction - caller depends on EF Core

**Solution:**
```csharp
// GOOD: Return materialized results
public IEnumerable<Product> GetAll()
{
    return _dbSet.ToList();  // ← Materialized, no EF Core leakage
}
```

### ❌ Mistake 2: Over-abstracting

```csharp
// BAD: Too many specialized repositories
public interface IProductRepository : IRepository<Product>
{
    List<Product> GetExpensiveProducts();
    List<Product> GetLowStockProducts();
    List<Product> GetExpensiveAndLowStockProducts();
    List<Product> GetCheapOrOutOfStockProducts();
    // ... 50 more query methods!
}
```

**Why it's bad:** Violates Single Responsibility, explosion of methods

**Solution:** Use Specification pattern
```csharp
// GOOD: Generic repository + specifications
var expensive = repository.Find(new ExpensiveProductsSpecification());
var lowStock = repository.Find(new LowStockSpecification(10));
```

### ❌ Mistake 3: Not Using Unit of Work

```csharp
// BAD: Multiple SaveChanges
void TransferStock(int fromProductId, int toProductId, int quantity)
{
    var from = productRepo.GetById(fromProductId);
    from.Stock -= quantity;
    productRepo.Update(from);
    context.SaveChanges();  // ← First commit

    var to = productRepo.GetById(toProductId);
    to.Stock += quantity;
    productRepo.Update(to);
    context.SaveChanges();  // ← Second commit - might fail!

    // If second fails, first is already saved → inconsistent state!
}
```

**Solution:** Use Unit of Work
```csharp
// GOOD: Atomic commit
void TransferStock(int fromProductId, int toProductId, int quantity)
{
    var from = unitOfWork.Products.GetById(fromProductId);
    from.Stock -= quantity;
    unitOfWork.Products.Update(from);

    var to = unitOfWork.Products.GetById(toProductId);
    to.Stock += quantity;
    unitOfWork.Products.Update(to);

    unitOfWork.SaveChanges();  // ← Atomic - both or neither
}
```

## The Debate: Repository Over EF Core?

### Arguments AGAINST Repository Pattern

> "EF Core's `DbContext` is already a repository! Why add another abstraction?"

**Valid points:**
- `DbContext` implements Unit of Work pattern
- `DbSet<T>` acts like a repository
- Extra layer adds complexity
- May prevent using EF Core features (Include, AsNoTracking)

**Counter-arguments:**
- Testing: Mocking `DbContext` is complex, mocking `IRepository<T>` is trivial
- Flexibility: Can swap EF → Dapper without changing services
- Centralization: Data access logic in one place
- Specification pattern: Reusable, composable queries

### When DbContext is Enough

For simple applications:
- No unit tests
- No plans to switch ORMs
- Small team, everyone knows EF Core
- Performance-critical (no abstraction overhead)

### When Repository Adds Value

For enterprise applications:
- Extensive unit testing required
- Might switch data providers
- Complex domain logic (Specification pattern)
- Multiple data sources (SQL + NoSQL)

## Conclusion

### Key Takeaways

1. **Repository Pattern abstracts data access** behind a clean interface
2. **Unit of Work coordinates multiple repositories** for atomic transactions
3. **Specification Pattern encapsulates query logic** in reusable objects
4. **Testability is the #1 benefit** - easy to mock repositories
5. **Trade-off**: Added complexity vs flexibility and testability

### When to Choose Repository

✅ **Choose Repository when:**
- Testability is important
- You might switch ORMs or databases
- You have complex querying needs
- You need centralized data access logic

❌ **Avoid Repository when:**
- Simple CRUD app with no tests
- Dedicated to one ORM (EF Core)
- Performance is critical
- Team is small and specialized

### Final Recommendation

**For most .NET applications:**
- Start with `DbContext` directly (YAGNI principle)
- Add Repository when testing becomes painful
- Use Unit of Work for multi-repository transactions
- Use Specification pattern for complex queries

**Don't over-engineer**, but be prepared to refactor when needed!
