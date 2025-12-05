// Repository Pattern - Generic Data Access Layer with EF Core
// Demonstrates abstraction over data access, Unit of Work, and Specification patterns

using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace RepositoryPattern;

public class Program
{
    public static void Main()
    {
        Console.WriteLine("=== Repository Pattern Demo - Generic Data Access Layer ===\n");

        DemonstrateBasicRepositoryPattern();
        Console.WriteLine("\n" + new string('=', 60) + "\n");

        DemonstrateUnitOfWorkPattern();
        Console.WriteLine("\n" + new string('=', 60) + "\n");

        DemonstrateSpecificationPattern();
        Console.WriteLine("\n" + new string('=', 60) + "\n");

        DemonstrateGenericRepositoryBenefits();
        Console.WriteLine("\n" + new string('=', 60) + "\n");

        DemonstrateRepositoryVsDirectDbContext();
        Console.WriteLine("\n" + new string('=', 60) + "\n");

        DemonstrateComplexQueriesWithSpecifications();
        Console.WriteLine("\n" + new string('=', 60) + "\n");

        DemonstrateProblemWithoutRepository();

        Console.WriteLine("\n=== Demo Complete ===");
    }

    // 1. Basic Repository Pattern
    static void DemonstrateBasicRepositoryPattern()
    {
        Console.WriteLine("1. BASIC REPOSITORY PATTERN");
        Console.WriteLine("Abstraction over data access with generic interface\n");

        var options = CreateInMemoryDatabaseOptions("BasicRepoDb");
        using var context = new AppDbContext(options);

        IRepository<Product> productRepo = new EfRepository<Product>(context);

        // Add products
        var laptop = new Product { Name = "Laptop", Price = 999.99m, Stock = 10 };
        var mouse = new Product { Name = "Mouse", Price = 29.99m, Stock = 50 };

        productRepo.Add(laptop);
        productRepo.Add(mouse);
        context.SaveChanges();

        Console.WriteLine($"Added: {laptop.Name} (ID: {laptop.Id})");
        Console.WriteLine($"Added: {mouse.Name} (ID: {mouse.Id})\n");

        // Retrieve
        var allProducts = productRepo.GetAll();
        Console.WriteLine($"Total products: {allProducts.Count()}");

        var foundProduct = productRepo.GetById(laptop.Id);
        Console.WriteLine($"Found by ID: {foundProduct?.Name}\n");

        // Update
        laptop.Price = 899.99m;
        productRepo.Update(laptop);
        context.SaveChanges();
        Console.WriteLine($"Updated price: ${productRepo.GetById(laptop.Id)?.Price:F2}\n");

        // Delete
        productRepo.Delete(mouse.Id);
        context.SaveChanges();
        Console.WriteLine($"After delete: {productRepo.GetAll().Count()} products");

        Console.WriteLine("\n✓ Repository provides clean abstraction over data access");
    }

    // 2. Unit of Work Pattern
    static void DemonstrateUnitOfWorkPattern()
    {
        Console.WriteLine("2. UNIT OF WORK PATTERN");
        Console.WriteLine("Manage transactions across multiple repositories\n");

        var options = CreateInMemoryDatabaseOptions("UnitOfWorkDb");
        using var unitOfWork = new UnitOfWork(options);

        // Create entities in multiple repositories
        var category = new Category { Name = "Electronics" };
        unitOfWork.Categories.Add(category);
        unitOfWork.SaveChanges();

        var product1 = new Product
        {
            Name = "Laptop",
            Price = 999.99m,
            Stock = 10,
            CategoryId = category.Id
        };

        var product2 = new Product
        {
            Name = "Monitor",
            Price = 299.99m,
            Stock = 15,
            CategoryId = category.Id
        };

        unitOfWork.Products.Add(product1);
        unitOfWork.Products.Add(product2);

        // Single SaveChanges for all operations (transaction)
        unitOfWork.SaveChanges();

        Console.WriteLine($"Category: {category.Name} (ID: {category.Id})");
        Console.WriteLine($"Product 1: {product1.Name} (ID: {product1.Id})");
        Console.WriteLine($"Product 2: {product2.Name} (ID: {product2.Id})\n");

        // Verify
        var productsInCategory = unitOfWork.Products
            .Find(p => p.CategoryId == category.Id)
            .ToList();

        Console.WriteLine($"Products in '{category.Name}': {productsInCategory.Count}");
        foreach (var p in productsInCategory)
        {
            Console.WriteLine($"   - {p.Name}: ${p.Price:F2}");
        }

        Console.WriteLine("\n✓ Unit of Work ensures all changes are committed together");
    }

    // 3. Specification Pattern
    static void DemonstrateSpecificationPattern()
    {
        Console.WriteLine("3. SPECIFICATION PATTERN");
        Console.WriteLine("Encapsulate query logic in reusable specifications\n");

        var options = CreateInMemoryDatabaseOptions("SpecificationDb");
        using var context = new AppDbContext(options);

        // Seed data
        var products = new[]
        {
            new Product { Name = "Laptop", Price = 999.99m, Stock = 10 },
            new Product { Name = "Mouse", Price = 29.99m, Stock = 50 },
            new Product { Name = "Keyboard", Price = 79.99m, Stock = 30 },
            new Product { Name = "Monitor", Price = 299.99m, Stock = 5 },
            new Product { Name = "Webcam", Price = 89.99m, Stock = 0 }
        };

        context.Products.AddRange(products);
        context.SaveChanges();

        var repo = new EfRepository<Product>(context);

        // Use specifications
        Console.WriteLine("1. Expensive Products (> $100):");
        var expensiveSpec = new ExpensiveProductsSpecification();
        var expensiveProducts = repo.Find(expensiveSpec).ToList();
        foreach (var p in expensiveProducts)
        {
            Console.WriteLine($"   {p.Name}: ${p.Price:F2}");
        }

        Console.WriteLine("\n2. Out of Stock Products:");
        var outOfStockSpec = new OutOfStockSpecification();
        var outOfStock = repo.Find(outOfStockSpec).ToList();
        foreach (var p in outOfStock)
        {
            Console.WriteLine($"   {p.Name} (Stock: {p.Stock})");
        }

        Console.WriteLine("\n3. Low Stock Products (< 10):");
        var lowStockSpec = new LowStockSpecification(10);
        var lowStock = repo.Find(lowStockSpec).ToList();
        foreach (var p in lowStock)
        {
            Console.WriteLine($"   {p.Name} (Stock: {p.Stock})");
        }

        // Combine specifications with AND
        Console.WriteLine("\n4. Expensive AND Out of Stock:");
        var combinedSpec = new AndSpecification<Product>(expensiveSpec, outOfStockSpec);
        var combined = repo.Find(combinedSpec).ToList();
        if (combined.Any())
        {
            foreach (var p in combined)
            {
                Console.WriteLine($"   {p.Name}: ${p.Price:F2}, Stock: {p.Stock}");
            }
        }
        else
        {
            Console.WriteLine("   (No products match both criteria)");
        }

        Console.WriteLine("\n✓ Specifications encapsulate query logic and are reusable");
    }

    // 4. Generic Repository Benefits
    static void DemonstrateGenericRepositoryBenefits()
    {
        Console.WriteLine("4. GENERIC REPOSITORY BENEFITS");
        Console.WriteLine("Single implementation works for all entities\n");

        var options = CreateInMemoryDatabaseOptions("GenericRepoDb");
        using var context = new AppDbContext(options);

        // Same repository implementation for different entities
        var productRepo = new EfRepository<Product>(context);
        var categoryRepo = new EfRepository<Category>(context);
        var orderRepo = new EfRepository<Order>(context);

        // Add different entity types
        productRepo.Add(new Product { Name = "Laptop", Price = 999.99m, Stock = 10 });
        categoryRepo.Add(new Category { Name = "Electronics" });
        orderRepo.Add(new Order { CustomerName = "John Doe", TotalAmount = 1299.99m });

        context.SaveChanges();

        Console.WriteLine($"Products: {productRepo.GetAll().Count()}");
        Console.WriteLine($"Categories: {categoryRepo.GetAll().Count()}");
        Console.WriteLine($"Orders: {orderRepo.GetAll().Count()}\n");

        Console.WriteLine("✓ Benefits:");
        Console.WriteLine("   1. Code reuse - Write once, use for all entities");
        Console.WriteLine("   2. Consistency - Same behavior across all repositories");
        Console.WriteLine("   3. Testability - Easy to mock IRepository<T>");
        Console.WriteLine("   4. Maintainability - Changes in one place affect all");
    }

    // 5. Repository vs Direct DbContext
    static void DemonstrateRepositoryVsDirectDbContext()
    {
        Console.WriteLine("5. REPOSITORY VS DIRECT DBCONTEXT");
        Console.WriteLine("Comparing abstraction levels\n");

        var options = CreateInMemoryDatabaseOptions("ComparisonDb");

        Console.WriteLine("❌ Direct DbContext (tight coupling):");
        Console.WriteLine("```csharp");
        Console.WriteLine("public class ProductService");
        Console.WriteLine("{");
        Console.WriteLine("    private readonly AppDbContext _context;");
        Console.WriteLine("");
        Console.WriteLine("    public Product GetProduct(int id)");
        Console.WriteLine("    {");
        Console.WriteLine("        return _context.Products.Find(id);  // ← Direct EF dependency");
        Console.WriteLine("    }");
        Console.WriteLine("}");
        Console.WriteLine("```");

        Console.WriteLine("\nProblems:");
        Console.WriteLine("   - Hard to test (requires real database or complex mocking)");
        Console.WriteLine("   - Tight coupling to EF Core");
        Console.WriteLine("   - Business logic mixed with data access");
        Console.WriteLine("   - Difficult to switch data providers\n");

        Console.WriteLine("✅ Repository Pattern (loose coupling):");
        Console.WriteLine("```csharp");
        Console.WriteLine("public class ProductService");
        Console.WriteLine("{");
        Console.WriteLine("    private readonly IRepository<Product> _productRepo;");
        Console.WriteLine("");
        Console.WriteLine("    public Product GetProduct(int id)");
        Console.WriteLine("    {");
        Console.WriteLine("        return _productRepo.GetById(id);  // ← Abstract interface");
        Console.WriteLine("    }");
        Console.WriteLine("}");
        Console.WriteLine("```");

        Console.WriteLine("\nBenefits:");
        Console.WriteLine("   ✓ Easy to test (mock IRepository<Product>)");
        Console.WriteLine("   ✓ Loose coupling to data access technology");
        Console.WriteLine("   ✓ Clear separation of concerns");
        Console.WriteLine("   ✓ Can switch from EF to Dapper/ADO.NET without changing service");
    }

    // 6. Complex Queries with Specifications
    static void DemonstrateComplexQueriesWithSpecifications()
    {
        Console.WriteLine("6. COMPLEX QUERIES WITH SPECIFICATIONS");
        Console.WriteLine("Build complex filters using specification composition\n");

        var options = CreateInMemoryDatabaseOptions("ComplexQueryDb");
        using var context = new AppDbContext(options);

        // Seed data
        var products = new[]
        {
            new Product { Name = "Gaming Laptop", Price = 1499.99m, Stock = 5 },
            new Product { Name = "Office Laptop", Price = 799.99m, Stock = 15 },
            new Product { Name = "Mechanical Keyboard", Price = 149.99m, Stock = 25 },
            new Product { Name = "Gaming Mouse", Price = 79.99m, Stock = 30 },
            new Product { Name = "4K Monitor", Price = 599.99m, Stock = 3 },
            new Product { Name = "Webcam HD", Price = 89.99m, Stock = 0 }
        };

        context.Products.AddRange(products);
        context.SaveChanges();

        var repo = new EfRepository<Product>(context);

        // Complex query: Expensive products (> $100) that are low on stock (< 10)
        var expensiveSpec = new ExpensiveProductsSpecification(100m);
        var lowStockSpec = new LowStockSpecification(10);
        var expensiveAndLowStock = new AndSpecification<Product>(expensiveSpec, lowStockSpec);

        Console.WriteLine("Products that are expensive (> $100) AND low stock (< 10):");
        var results = repo.Find(expensiveAndLowStock).ToList();
        foreach (var p in results)
        {
            Console.WriteLine($"   {p.Name}: ${p.Price:F2}, Stock: {p.Stock}");
        }

        // Another complex query: Cheap OR out of stock
        var cheapSpec = new Specification<Product>(p => p.Price < 100m);
        var outOfStockSpec = new OutOfStockSpecification();
        var cheapOrOutOfStock = new OrSpecification<Product>(cheapSpec, outOfStockSpec);

        Console.WriteLine("\nProducts that are cheap (< $100) OR out of stock:");
        var results2 = repo.Find(cheapOrOutOfStock).ToList();
        foreach (var p in results2)
        {
            Console.WriteLine($"   {p.Name}: ${p.Price:F2}, Stock: {p.Stock}");
        }

        Console.WriteLine("\n✓ Specifications can be combined to build complex queries");
    }

    // 7. Problem Without Repository Pattern
    static void DemonstrateProblemWithoutRepository()
    {
        Console.WriteLine("7. PROBLEM WITHOUT REPOSITORY PATTERN");
        Console.WriteLine("Direct database access everywhere\n");

        Console.WriteLine("❌ Legacy approach (direct DbContext in every service):");
        Console.WriteLine("```csharp");
        Console.WriteLine("public class ProductService");
        Console.WriteLine("{");
        Console.WriteLine("    private readonly AppDbContext _context;");
        Console.WriteLine("");
        Console.WriteLine("    public void AddProduct(Product product)");
        Console.WriteLine("    {");
        Console.WriteLine("        _context.Products.Add(product);");
        Console.WriteLine("        _context.SaveChanges();");
        Console.WriteLine("    }");
        Console.WriteLine("}");
        Console.WriteLine("");
        Console.WriteLine("public class OrderService");
        Console.WriteLine("{");
        Console.WriteLine("    private readonly AppDbContext _context;");
        Console.WriteLine("");
        Console.WriteLine("    public void CreateOrder(Order order)");
        Console.WriteLine("    {");
        Console.WriteLine("        _context.Orders.Add(order);");
        Console.WriteLine("        _context.SaveChanges();");
        Console.WriteLine("    }");
        Console.WriteLine("}");
        Console.WriteLine("// ... duplicate code in every service!");
        Console.WriteLine("```\n");

        Console.WriteLine("Problems:");
        Console.WriteLine("   1. Code duplication: CRUD logic repeated everywhere");
        Console.WriteLine("   2. Hard to test: Must mock entire DbContext");
        Console.WriteLine("   3. Tight coupling: Services depend on EF Core");
        Console.WriteLine("   4. Difficult to change: Switching ORM affects all services");
        Console.WriteLine("   5. Mixed concerns: Business logic + data access in same class\n");

        Console.WriteLine("✅ Repository pattern solves all these issues:");
        Console.WriteLine("   1. DRY: CRUD logic in one place (generic repository)");
        Console.WriteLine("   2. Testable: Mock IRepository<T> easily");
        Console.WriteLine("   3. Loose coupling: Services depend on interface");
        Console.WriteLine("   4. Flexible: Swap implementations without changing services");
        Console.WriteLine("   5. Separation of concerns: Clear layering");
    }

    // Helper method
    static DbContextOptions<AppDbContext> CreateInMemoryDatabaseOptions(string dbName)
    {
        return new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: dbName)
            .Options;
    }
}

// ============================================================================
// ENTITY CLASSES
// ============================================================================

/// <summary>
/// Product entity
/// </summary>
public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public int? CategoryId { get; set; }
    public Category? Category { get; set; }
}

/// <summary>
/// Category entity
/// </summary>
public class Category
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public ICollection<Product> Products { get; set; } = new List<Product>();
}

/// <summary>
/// Order entity
/// </summary>
public class Order
{
    public int Id { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public DateTime OrderDate { get; set; } = DateTime.Now;
}

// ============================================================================
// DATABASE CONTEXT
// ============================================================================

/// <summary>
/// Application database context using EF Core
/// </summary>
public class AppDbContext : DbContext
{
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Order> Orders => Set<Order>();

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configure Product
        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(p => p.Id);
            entity.Property(p => p.Name).IsRequired().HasMaxLength(200);
            entity.Property(p => p.Price).HasPrecision(18, 2);

            entity.HasOne(p => p.Category)
                  .WithMany(c => c.Products)
                  .HasForeignKey(p => p.CategoryId);
        });

        // Configure Category
        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(c => c.Id);
            entity.Property(c => c.Name).IsRequired().HasMaxLength(100);
        });

        // Configure Order
        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(o => o.Id);
            entity.Property(o => o.CustomerName).IsRequired().HasMaxLength(200);
            entity.Property(o => o.TotalAmount).HasPrecision(18, 2);
        });
    }
}

// ============================================================================
// REPOSITORY PATTERN
// ============================================================================

/// <summary>
/// Generic repository interface
/// Provides abstraction over data access
/// </summary>
public interface IRepository<T> where T : class
{
    T? GetById(int id);
    IEnumerable<T> GetAll();
    IEnumerable<T> Find(Expression<Func<T, bool>> predicate);
    IEnumerable<T> Find(ISpecification<T> specification);
    void Add(T entity);
    void AddRange(IEnumerable<T> entities);
    void Update(T entity);
    void Delete(int id);
    void DeleteRange(IEnumerable<T> entities);
}

/// <summary>
/// EF Core implementation of generic repository
/// </summary>
public class EfRepository<T> : IRepository<T> where T : class
{
    protected readonly AppDbContext _context;
    protected readonly DbSet<T> _dbSet;

    public EfRepository(AppDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public T? GetById(int id)
    {
        return _dbSet.Find(id);
    }

    public IEnumerable<T> GetAll()
    {
        return _dbSet.ToList();
    }

    public IEnumerable<T> Find(Expression<Func<T, bool>> predicate)
    {
        return _dbSet.Where(predicate).ToList();
    }

    public IEnumerable<T> Find(ISpecification<T> specification)
    {
        return _dbSet.Where(specification.ToExpression()).ToList();
    }

    public void Add(T entity)
    {
        _dbSet.Add(entity);
    }

    public void AddRange(IEnumerable<T> entities)
    {
        _dbSet.AddRange(entities);
    }

    public void Update(T entity)
    {
        _dbSet.Update(entity);
    }

    public void Delete(int id)
    {
        var entity = GetById(id);
        if (entity != null)
        {
            _dbSet.Remove(entity);
        }
    }

    public void DeleteRange(IEnumerable<T> entities)
    {
        _dbSet.RemoveRange(entities);
    }
}

// ============================================================================
// UNIT OF WORK PATTERN
// ============================================================================

/// <summary>
/// Unit of Work interface
/// Manages transactions across multiple repositories
/// </summary>
public interface IUnitOfWork : IDisposable
{
    IRepository<Product> Products { get; }
    IRepository<Category> Categories { get; }
    IRepository<Order> Orders { get; }
    int SaveChanges();
}

/// <summary>
/// Unit of Work implementation
/// Coordinates work of multiple repositories and ensures atomic commits
/// </summary>
public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;
    private IRepository<Product>? _products;
    private IRepository<Category>? _categories;
    private IRepository<Order>? _orders;

    public UnitOfWork(DbContextOptions<AppDbContext> options)
    {
        _context = new AppDbContext(options);
    }

    public IRepository<Product> Products =>
        _products ??= new EfRepository<Product>(_context);

    public IRepository<Category> Categories =>
        _categories ??= new EfRepository<Category>(_context);

    public IRepository<Order> Orders =>
        _orders ??= new EfRepository<Order>(_context);

    public int SaveChanges()
    {
        return _context.SaveChanges();
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}

// ============================================================================
// SPECIFICATION PATTERN
// ============================================================================

/// <summary>
/// Specification interface
/// Encapsulates query logic in reusable objects
/// </summary>
public interface ISpecification<T>
{
    Expression<Func<T, bool>> ToExpression();
    bool IsSatisfiedBy(T entity);
}

/// <summary>
/// Base specification class
/// </summary>
public class Specification<T> : ISpecification<T>
{
    private readonly Expression<Func<T, bool>> _expression;

    public Specification(Expression<Func<T, bool>> expression)
    {
        _expression = expression;
    }

    public Expression<Func<T, bool>> ToExpression()
    {
        return _expression;
    }

    public bool IsSatisfiedBy(T entity)
    {
        return _expression.Compile()(entity);
    }
}

/// <summary>
/// AND specification - combines two specifications with AND logic
/// </summary>
public class AndSpecification<T> : ISpecification<T>
{
    private readonly ISpecification<T> _left;
    private readonly ISpecification<T> _right;

    public AndSpecification(ISpecification<T> left, ISpecification<T> right)
    {
        _left = left;
        _right = right;
    }

    public Expression<Func<T, bool>> ToExpression()
    {
        var leftExpr = _left.ToExpression();
        var rightExpr = _right.ToExpression();

        var parameter = Expression.Parameter(typeof(T));

        var combined = Expression.AndAlso(
            Expression.Invoke(leftExpr, parameter),
            Expression.Invoke(rightExpr, parameter)
        );

        return Expression.Lambda<Func<T, bool>>(combined, parameter);
    }

    public bool IsSatisfiedBy(T entity)
    {
        return _left.IsSatisfiedBy(entity) && _right.IsSatisfiedBy(entity);
    }
}

/// <summary>
/// OR specification - combines two specifications with OR logic
/// </summary>
public class OrSpecification<T> : ISpecification<T>
{
    private readonly ISpecification<T> _left;
    private readonly ISpecification<T> _right;

    public OrSpecification(ISpecification<T> left, ISpecification<T> right)
    {
        _left = left;
        _right = right;
    }

    public Expression<Func<T, bool>> ToExpression()
    {
        var leftExpr = _left.ToExpression();
        var rightExpr = _right.ToExpression();

        var parameter = Expression.Parameter(typeof(T));

        var combined = Expression.OrElse(
            Expression.Invoke(leftExpr, parameter),
            Expression.Invoke(rightExpr, parameter)
        );

        return Expression.Lambda<Func<T, bool>>(combined, parameter);
    }

    public bool IsSatisfiedBy(T entity)
    {
        return _left.IsSatisfiedBy(entity) || _right.IsSatisfiedBy(entity);
    }
}

// ============================================================================
// CONCRETE SPECIFICATIONS
// ============================================================================

/// <summary>
/// Specification for expensive products
/// </summary>
public class ExpensiveProductsSpecification : Specification<Product>
{
    public ExpensiveProductsSpecification(decimal threshold = 100m)
        : base(p => p.Price > threshold)
    {
    }
}

/// <summary>
/// Specification for out of stock products
/// </summary>
public class OutOfStockSpecification : Specification<Product>
{
    public OutOfStockSpecification()
        : base(p => p.Stock == 0)
    {
    }
}

/// <summary>
/// Specification for low stock products
/// </summary>
public class LowStockSpecification : Specification<Product>
{
    public LowStockSpecification(int threshold)
        : base(p => p.Stock < threshold && p.Stock > 0)
    {
    }
}
