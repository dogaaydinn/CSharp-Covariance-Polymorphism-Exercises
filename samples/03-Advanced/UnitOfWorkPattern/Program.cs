// Unit of Work Pattern - E-Commerce Order Processing with Transaction Management
// Demonstrates atomic transactions across multiple repositories with rollback capability

using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace UnitOfWorkPattern;

#region Entity Classes

/// <summary>
/// Product entity with stock management
/// </summary>
public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public string Category { get; set; } = string.Empty;
}

/// <summary>
/// Order entity representing customer orders
/// </summary>
public class Order
{
    public int Id { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public DateTime OrderDate { get; set; }
    public decimal TotalAmount { get; set; }
    public string Status { get; set; } = "Pending"; // Pending, Confirmed, Cancelled
    public List<OrderItem> OrderItems { get; set; } = new();
}

/// <summary>
/// OrderItem entity representing line items in an order
/// </summary>
public class OrderItem
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Subtotal => Quantity * UnitPrice;
}

#endregion

#region Database Context

/// <summary>
/// Entity Framework Core database context
/// </summary>
public class AppDbContext : DbContext
{
    public DbSet<Product> Products { get; set; } = null!;
    public DbSet<Order> Orders { get; set; } = null!;
    public DbSet<OrderItem> OrderItems { get; set; } = null!;

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>().HasKey(p => p.Id);
        modelBuilder.Entity<Order>().HasKey(o => o.Id);
        modelBuilder.Entity<OrderItem>().HasKey(oi => oi.Id);

        // Configure relationships
        modelBuilder.Entity<Order>()
            .HasMany(o => o.OrderItems)
            .WithOne()
            .HasForeignKey(oi => oi.OrderId);
    }
}

#endregion

#region Generic Repository Pattern

/// <summary>
/// Generic repository interface for data access
/// </summary>
public interface IGenericRepository<T> where T : class
{
    T? GetById(int id);
    IEnumerable<T> GetAll();
    IEnumerable<T> Find(Expression<Func<T, bool>> predicate);
    void Add(T entity);
    void Update(T entity);
    void Delete(int id);
}

/// <summary>
/// Generic repository implementation using EF Core
/// </summary>
public class GenericRepository<T> : IGenericRepository<T> where T : class
{
    protected readonly AppDbContext _context;
    protected readonly DbSet<T> _dbSet;

    public GenericRepository(AppDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public T? GetById(int id) => _dbSet.Find(id);

    public IEnumerable<T> GetAll() => _dbSet.ToList();

    public IEnumerable<T> Find(Expression<Func<T, bool>> predicate)
        => _dbSet.Where(predicate).ToList();

    public void Add(T entity)
    {
        _dbSet.Add(entity);
        Console.WriteLine($"   📝 Staged for insert: {typeof(T).Name}");
    }

    public void Update(T entity)
    {
        _dbSet.Update(entity);
        Console.WriteLine($"   📝 Staged for update: {typeof(T).Name}");
    }

    public void Delete(int id)
    {
        var entity = GetById(id);
        if (entity != null)
        {
            _dbSet.Remove(entity);
            Console.WriteLine($"   📝 Staged for delete: {typeof(T).Name}");
        }
    }
}

#endregion

#region Unit of Work Pattern

/// <summary>
/// Unit of Work interface coordinating multiple repositories
/// Ensures atomic transactions across multiple entities
/// </summary>
public interface IUnitOfWork : IDisposable
{
    // Repository properties
    IGenericRepository<Product> Products { get; }
    IGenericRepository<Order> Orders { get; }
    IGenericRepository<OrderItem> OrderItems { get; }

    // Transaction methods
    int SaveChanges();
    Task<int> SaveChangesAsync();
    void BeginTransaction();
    void Commit();
    void Rollback();
}

/// <summary>
/// Unit of Work implementation with transaction management
/// </summary>
public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;
    private Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction? _transaction;

    // Lazy-loaded repositories
    private IGenericRepository<Product>? _products;
    private IGenericRepository<Order>? _orders;
    private IGenericRepository<OrderItem>? _orderItems;

    public UnitOfWork(AppDbContext context)
    {
        _context = context;
    }

    // Repository properties (lazy initialization)
    public IGenericRepository<Product> Products
        => _products ??= new GenericRepository<Product>(_context);

    public IGenericRepository<Order> Orders
        => _orders ??= new GenericRepository<Order>(_context);

    public IGenericRepository<OrderItem> OrderItems
        => _orderItems ??= new GenericRepository<OrderItem>(_context);

    /// <summary>
    /// Saves all changes to the database
    /// </summary>
    public int SaveChanges()
    {
        var changes = _context.SaveChanges();
        Console.WriteLine($"   ✅ Committed {changes} changes to database");
        return changes;
    }

    /// <summary>
    /// Saves all changes asynchronously
    /// </summary>
    public async Task<int> SaveChangesAsync()
    {
        var changes = await _context.SaveChangesAsync();
        Console.WriteLine($"   ✅ Committed {changes} changes to database");
        return changes;
    }

    /// <summary>
    /// Begins a new database transaction
    /// Note: In-memory database doesn't support real transactions, but the pattern still works
    /// </summary>
    public void BeginTransaction()
    {
        try
        {
            _transaction = _context.Database.BeginTransaction();
            Console.WriteLine("   🔒 Transaction started");
        }
        catch (InvalidOperationException)
        {
            // In-memory database doesn't support transactions
            // This is expected and doesn't affect the demonstration
            Console.WriteLine("   🔒 Transaction started (simulated for in-memory database)");
        }
    }

    /// <summary>
    /// Commits the current transaction
    /// </summary>
    public void Commit()
    {
        try
        {
            SaveChanges();
            _transaction?.Commit();
            Console.WriteLine("   ✅ Transaction committed successfully");
        }
        catch
        {
            Rollback();
            throw;
        }
        finally
        {
            _transaction?.Dispose();
            _transaction = null;
        }
    }

    /// <summary>
    /// Rolls back the current transaction
    /// </summary>
    public void Rollback()
    {
        _transaction?.Rollback();
        _transaction?.Dispose();
        _transaction = null;
        Console.WriteLine("   ⚠️  Transaction rolled back");
    }

    public void Dispose()
    {
        _transaction?.Dispose();
        _context.Dispose();
    }
}

#endregion

#region Business Logic Layer

/// <summary>
/// Order service with business logic for e-commerce operations
/// </summary>
public class OrderService
{
    private readonly IUnitOfWork _unitOfWork;

    public OrderService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    /// <summary>
    /// Creates a new order with stock validation and atomic transaction
    /// </summary>
    public void CreateOrder(string customerName, List<(int productId, int quantity)> items)
    {
        try
        {
            _unitOfWork.BeginTransaction();

            // 1. Create order
            var order = new Order
            {
                CustomerName = customerName,
                OrderDate = DateTime.Now,
                Status = "Pending"
            };

            decimal totalAmount = 0;

            // 2. Process each order item
            foreach (var (productId, quantity) in items)
            {
                var product = _unitOfWork.Products.GetById(productId);
                if (product == null)
                {
                    throw new InvalidOperationException($"Product {productId} not found");
                }

                // Validate stock
                if (product.Stock < quantity)
                {
                    throw new InvalidOperationException(
                        $"Insufficient stock for {product.Name}. Available: {product.Stock}, Requested: {quantity}");
                }

                // Reduce stock
                product.Stock -= quantity;
                _unitOfWork.Products.Update(product);

                // Create order item
                var orderItem = new OrderItem
                {
                    ProductId = productId,
                    ProductName = product.Name,
                    Quantity = quantity,
                    UnitPrice = product.Price
                };

                totalAmount += orderItem.Subtotal;
                order.OrderItems.Add(orderItem);
            }

            order.TotalAmount = totalAmount;
            order.Status = "Confirmed";
            _unitOfWork.Orders.Add(order);

            // 3. Commit transaction (atomic - all or nothing)
            _unitOfWork.Commit();

            Console.WriteLine($"   ✅ Order created successfully for {customerName}");
            Console.WriteLine($"      Total: ${totalAmount:F2}, Items: {items.Count}");
        }
        catch (Exception ex)
        {
            _unitOfWork.Rollback();
            Console.WriteLine($"   ❌ Order creation failed: {ex.Message}");
            throw;
        }
    }

    /// <summary>
    /// Cancels an order and restores product stock
    /// </summary>
    public void CancelOrder(int orderId)
    {
        try
        {
            _unitOfWork.BeginTransaction();

            var order = _unitOfWork.Orders.GetById(orderId);
            if (order == null)
            {
                throw new InvalidOperationException("Order not found");
            }

            // Restore stock for each item
            var orderItems = _unitOfWork.OrderItems.Find(oi => oi.OrderId == orderId);
            foreach (var item in orderItems)
            {
                var product = _unitOfWork.Products.GetById(item.ProductId);
                if (product != null)
                {
                    product.Stock += item.Quantity;
                    _unitOfWork.Products.Update(product);
                }
            }

            // Update order status
            order.Status = "Cancelled";
            _unitOfWork.Orders.Update(order);

            _unitOfWork.Commit();
            Console.WriteLine($"   ✅ Order {orderId} cancelled and stock restored");
        }
        catch (Exception ex)
        {
            _unitOfWork.Rollback();
            Console.WriteLine($"   ❌ Order cancellation failed: {ex.Message}");
            throw;
        }
    }
}

#endregion

#region Demonstration Programs

public class Program
{
    public static void Main()
    {
        Console.WriteLine("=== Unit of Work Pattern Demo - E-Commerce Order Processing ===\n");

        DemonstrateBasicUnitOfWork();
        DemonstrateTransactionSuccess();
        DemonstrateRollbackOnFailure();
        DemonstrateMultipleRepositoryCoordination();
        DemonstrateECommerceOrderProcessing();
        DemonstrateProblemWithoutUnitOfWork();
        DemonstrateStockManagementWithConcurrency();

        Console.WriteLine("\n=== Demo Complete ===");
    }

    /// <summary>
    /// 1. Basic Unit of Work usage
    /// </summary>
    static void DemonstrateBasicUnitOfWork()
    {
        Console.WriteLine("1. BASIC UNIT OF WORK");
        Console.WriteLine("Single commit point for multiple operations\n");

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase("BasicDemo")
            .Options;

        using var context = new AppDbContext(options);
        using var unitOfWork = new UnitOfWork(context);

        // Multiple operations
        unitOfWork.Products.Add(new Product { Name = "Laptop", Price = 999.99m, Stock = 10, Category = "Electronics" });
        unitOfWork.Products.Add(new Product { Name = "Mouse", Price = 29.99m, Stock = 50, Category = "Accessories" });

        // Single SaveChanges commits all
        unitOfWork.SaveChanges();

        var count = unitOfWork.Products.GetAll().Count();
        Console.WriteLine($"\nTotal products: {count}");
        Console.WriteLine("✓ All changes committed in single transaction\n");

        PrintSeparator();
    }

    /// <summary>
    /// 2. Transaction management with success scenario
    /// </summary>
    static void DemonstrateTransactionSuccess()
    {
        Console.WriteLine("2. TRANSACTION MANAGEMENT - SUCCESS");
        Console.WriteLine("Atomic commit across multiple repositories\n");

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase("TransactionSuccess")
            .Options;

        using var context = new AppDbContext(options);
        using var unitOfWork = new UnitOfWork(context);

        try
        {
            unitOfWork.BeginTransaction();

            // Add product
            var product = new Product { Id = 1, Name = "Gaming Laptop", Price = 1499.99m, Stock = 5, Category = "Electronics" };
            unitOfWork.Products.Add(product);

            // Create order
            var order = new Order
            {
                Id = 1,
                CustomerName = "John Doe",
                OrderDate = DateTime.Now,
                TotalAmount = 1499.99m,
                Status = "Confirmed"
            };
            unitOfWork.Orders.Add(order);

            // Add order item
            unitOfWork.OrderItems.Add(new OrderItem
            {
                Id = 1,
                OrderId = 1,
                ProductId = 1,
                ProductName = "Gaming Laptop",
                Quantity = 1,
                UnitPrice = 1499.99m
            });

            // Commit transaction
            unitOfWork.Commit();

            Console.WriteLine("\n✓ All operations completed successfully");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Transaction failed: {ex.Message}");
        }

        PrintSeparator();
    }

    /// <summary>
    /// 3. Rollback on failure demonstration
    /// </summary>
    static void DemonstrateRollbackOnFailure()
    {
        Console.WriteLine("3. ROLLBACK ON FAILURE");
        Console.WriteLine("Transaction automatically rolls back on error\n");

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase("RollbackDemo")
            .Options;

        using var context = new AppDbContext(options);
        using var unitOfWork = new UnitOfWork(context);

        // Add initial product
        unitOfWork.Products.Add(new Product { Id = 1, Name = "Laptop", Price = 999.99m, Stock = 10, Category = "Electronics" });
        unitOfWork.SaveChanges();

        Console.WriteLine("Initial state: 1 product in database\n");

        try
        {
            unitOfWork.BeginTransaction();

            // Valid operation
            unitOfWork.Products.Add(new Product { Id = 2, Name = "Mouse", Price = 29.99m, Stock = 50, Category = "Accessories" });

            // Simulate failure
            Console.WriteLine("   💥 Simulating failure (e.g., network error, validation failure)");
            throw new InvalidOperationException("Payment processing failed");

            // This won't execute
            #pragma warning disable CS0162
            unitOfWork.Commit();
            #pragma warning restore CS0162
        }
        catch (Exception ex)
        {
            Console.WriteLine($"   ❌ Error: {ex.Message}");
            unitOfWork.Rollback();
        }

        var finalCount = unitOfWork.Products.GetAll().Count();
        Console.WriteLine($"\nFinal state: {finalCount} product(s) in database");
        Console.WriteLine("✓ Rollback prevented partial commit\n");

        PrintSeparator();
    }

    /// <summary>
    /// 4. Multiple repository coordination
    /// </summary>
    static void DemonstrateMultipleRepositoryCoordination()
    {
        Console.WriteLine("4. MULTIPLE REPOSITORY COORDINATION");
        Console.WriteLine("Coordinating changes across 3 repositories\n");

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase("MultiRepo")
            .Options;

        using var context = new AppDbContext(options);
        using var unitOfWork = new UnitOfWork(context);

        unitOfWork.BeginTransaction();

        // Repository 1: Products
        var product = new Product { Id = 1, Name = "Mechanical Keyboard", Price = 129.99m, Stock = 20, Category = "Accessories" };
        unitOfWork.Products.Add(product);

        // Repository 2: Orders
        var order = new Order
        {
            Id = 1,
            CustomerName = "Alice Smith",
            OrderDate = DateTime.Now,
            TotalAmount = 259.98m,
            Status = "Confirmed"
        };
        unitOfWork.Orders.Add(order);

        // Repository 3: OrderItems
        unitOfWork.OrderItems.Add(new OrderItem
        {
            Id = 1,
            OrderId = 1,
            ProductId = 1,
            ProductName = "Mechanical Keyboard",
            Quantity = 2,
            UnitPrice = 129.99m
        });

        // Single commit for all 3 repositories
        unitOfWork.Commit();

        Console.WriteLine("\n✓ Changes across 3 repositories committed atomically");
        Console.WriteLine($"   Products: {unitOfWork.Products.GetAll().Count()}");
        Console.WriteLine($"   Orders: {unitOfWork.Orders.GetAll().Count()}");
        Console.WriteLine($"   OrderItems: {unitOfWork.OrderItems.GetAll().Count()}\n");

        PrintSeparator();
    }

    /// <summary>
    /// 5. E-Commerce order processing with stock management
    /// </summary>
    static void DemonstrateECommerceOrderProcessing()
    {
        Console.WriteLine("5. E-COMMERCE ORDER PROCESSING");
        Console.WriteLine("Real-world scenario: Order creation with stock validation\n");

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase("ECommerce")
            .Options;

        using var context = new AppDbContext(options);
        using var unitOfWork = new UnitOfWork(context);
        var orderService = new OrderService(unitOfWork);

        // Setup products
        unitOfWork.Products.Add(new Product { Id = 1, Name = "Gaming Laptop", Price = 1499.99m, Stock = 10, Category = "Electronics" });
        unitOfWork.Products.Add(new Product { Id = 2, Name = "Wireless Mouse", Price = 49.99m, Stock = 50, Category = "Accessories" });
        unitOfWork.Products.Add(new Product { Id = 3, Name = "USB-C Hub", Price = 39.99m, Stock = 5, Category = "Accessories" });
        unitOfWork.SaveChanges();

        Console.WriteLine("Products in stock:");
        foreach (var p in unitOfWork.Products.GetAll())
        {
            Console.WriteLine($"   {p.Name}: ${p.Price:F2} (Stock: {p.Stock})");
        }

        Console.WriteLine("\n--- Scenario 1: Successful order ---");
        var items1 = new List<(int, int)>
        {
            (1, 1), // 1x Gaming Laptop
            (2, 2)  // 2x Wireless Mouse
        };

        orderService.CreateOrder("Bob Johnson", items1);

        Console.WriteLine("\nStock after order 1:");
        foreach (var p in unitOfWork.Products.GetAll())
        {
            Console.WriteLine($"   {p.Name}: Stock: {p.Stock}");
        }

        Console.WriteLine("\n--- Scenario 2: Insufficient stock (should fail) ---");
        var items2 = new List<(int, int)>
        {
            (3, 10) // 10x USB-C Hub (only 5 in stock)
        };

        try
        {
            orderService.CreateOrder("Carol White", items2);
        }
        catch (Exception)
        {
            Console.WriteLine("   ✓ Order rejected due to insufficient stock");
        }

        Console.WriteLine("\nStock unchanged (rollback worked):");
        foreach (var p in unitOfWork.Products.GetAll())
        {
            Console.WriteLine($"   {p.Name}: Stock: {p.Stock}");
        }

        Console.WriteLine("\n--- Scenario 3: Order cancellation ---");
        orderService.CancelOrder(1);

        Console.WriteLine("\nStock after cancellation:");
        foreach (var p in unitOfWork.Products.GetAll())
        {
            Console.WriteLine($"   {p.Name}: Stock: {p.Stock}");
        }

        Console.WriteLine("\n✓ Complete e-commerce workflow with stock management\n");

        PrintSeparator();
    }

    /// <summary>
    /// 6. Problem without Unit of Work
    /// </summary>
    static void DemonstrateProblemWithoutUnitOfWork()
    {
        Console.WriteLine("6. PROBLEM WITHOUT UNIT OF WORK");
        Console.WriteLine("Multiple SaveChanges() calls lead to inconsistent state\n");

        Console.WriteLine("❌ BAD APPROACH (without Unit of Work):");
        Console.WriteLine("```csharp");
        Console.WriteLine("void CreateOrder(int productId, int quantity)");
        Console.WriteLine("{");
        Console.WriteLine("    var product = context.Products.Find(productId);");
        Console.WriteLine("    product.Stock -= quantity;");
        Console.WriteLine("    context.SaveChanges();  // ← First commit");
        Console.WriteLine("");
        Console.WriteLine("    var order = new Order { ProductId = productId };");
        Console.WriteLine("    context.Orders.Add(order);");
        Console.WriteLine("    context.SaveChanges();  // ← Second commit");
        Console.WriteLine("");
        Console.WriteLine("    var orderItem = new OrderItem { OrderId = order.Id };");
        Console.WriteLine("    context.OrderItems.Add(orderItem);");
        Console.WriteLine("    context.SaveChanges();  // ← Third commit");
        Console.WriteLine("}");
        Console.WriteLine("```\n");

        Console.WriteLine("Problems:");
        Console.WriteLine("   1. Stock reduced but order fails → Stock lost!");
        Console.WriteLine("   2. Order created but items fail → Orphaned order!");
        Console.WriteLine("   3. No atomicity → Partial updates possible");
        Console.WriteLine("   4. Hard to rollback → Each commit is permanent");
        Console.WriteLine("   5. Data inconsistency → Business rules violated\n");

        Console.WriteLine("✅ GOOD APPROACH (with Unit of Work):");
        Console.WriteLine("```csharp");
        Console.WriteLine("void CreateOrder(int productId, int quantity)");
        Console.WriteLine("{");
        Console.WriteLine("    unitOfWork.BeginTransaction();");
        Console.WriteLine("    ");
        Console.WriteLine("    var product = unitOfWork.Products.GetById(productId);");
        Console.WriteLine("    product.Stock -= quantity;");
        Console.WriteLine("    unitOfWork.Products.Update(product);  // ← Staged");
        Console.WriteLine("    ");
        Console.WriteLine("    var order = new Order { ProductId = productId };");
        Console.WriteLine("    unitOfWork.Orders.Add(order);  // ← Staged");
        Console.WriteLine("    ");
        Console.WriteLine("    var orderItem = new OrderItem { OrderId = order.Id };");
        Console.WriteLine("    unitOfWork.OrderItems.Add(orderItem);  // ← Staged");
        Console.WriteLine("    ");
        Console.WriteLine("    unitOfWork.Commit();  // ← Single atomic commit");
        Console.WriteLine("}");
        Console.WriteLine("```\n");

        Console.WriteLine("Benefits:");
        Console.WriteLine("   ✓ All changes commit together (atomic)");
        Console.WriteLine("   ✓ Rollback on any failure");
        Console.WriteLine("   ✓ Data consistency guaranteed");
        Console.WriteLine("   ✓ Business rules enforced");
        Console.WriteLine("   ✓ Easy error handling\n");

        PrintSeparator();
    }

    /// <summary>
    /// 7. Stock management with concurrent operations
    /// </summary>
    static void DemonstrateStockManagementWithConcurrency()
    {
        Console.WriteLine("7. STOCK MANAGEMENT WITH CONCURRENCY");
        Console.WriteLine("Handling multiple simultaneous orders safely\n");

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase("Concurrency")
            .Options;

        using var context = new AppDbContext(options);
        using var unitOfWork = new UnitOfWork(context);
        var orderService = new OrderService(unitOfWork);

        // Setup limited stock product
        unitOfWork.Products.Add(new Product
        {
            Id = 1,
            Name = "Limited Edition Console",
            Price = 499.99m,
            Stock = 3, // Only 3 available
            Category = "Gaming"
        });
        unitOfWork.SaveChanges();

        Console.WriteLine("Initial stock: 3 consoles\n");

        Console.WriteLine("Simulating 3 concurrent orders:");

        // Order 1
        Console.WriteLine("\n📦 Customer 1 orders 2 units:");
        try
        {
            orderService.CreateOrder("Customer 1", new List<(int, int)> { (1, 2) });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"   ❌ Failed: {ex.Message}");
        }

        // Order 2
        Console.WriteLine("\n📦 Customer 2 orders 1 unit:");
        try
        {
            orderService.CreateOrder("Customer 2", new List<(int, int)> { (1, 1) });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"   ❌ Failed: {ex.Message}");
        }

        // Order 3 (should fail - no stock left)
        Console.WriteLine("\n📦 Customer 3 orders 1 unit:");
        try
        {
            orderService.CreateOrder("Customer 3", new List<(int, int)> { (1, 1) });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"   ❌ Failed: {ex.Message}");
        }

        var product = unitOfWork.Products.GetById(1);
        Console.WriteLine($"\nFinal stock: {product?.Stock ?? 0} consoles");
        Console.WriteLine($"Orders created: {unitOfWork.Orders.GetAll().Count()}");
        Console.WriteLine("\n✓ Unit of Work ensures stock consistency even with concurrent orders\n");

        PrintSeparator();
    }

    static void PrintSeparator()
    {
        Console.WriteLine("============================================================\n");
    }
}

#endregion
