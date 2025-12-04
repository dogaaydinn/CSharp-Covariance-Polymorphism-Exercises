# 12. Use Entity Framework Core for Data Access

**Status:** ✅ Accepted

**Date:** 2024-12-01

**Deciders:** Architecture Team, Database Team

**Technical Story:** Implementation in `samples/05-RealWorld/MicroserviceTemplate/Infrastructure`

---

## Context and Problem Statement

Applications need to interact with databases to:
- Read and write business entities
- Execute complex queries
- Manage database schema migrations
- Handle transactions

**Traditional ADO.NET problems:**
- Verbose code (100+ lines for simple CRUD)
- Manual object mapping
- SQL injection risks
- No schema migration support
- Tedious connection management

**Requirements:**
- Object-Relational Mapping (ORM)
- LINQ query support
- Database migrations
- Multiple database support (PostgreSQL, SQL Server, SQLite)
- Change tracking
- Transaction management

---

## Decision Drivers

* **Productivity** - Reduce boilerplate code
* **Type Safety** - Compile-time query validation
* **LINQ Support** - Write queries in C#, not SQL
* **Migrations** - Automatic schema versioning
* **Microsoft Support** - First-party solution
* **Performance** - Efficient SQL generation

---

## Considered Options

* **Option 1** - Entity Framework Core (EF Core)
* **Option 2** - Dapper (micro-ORM)
* **Option 3** - ADO.NET (raw SQL)
* **Option 4** - NHibernate

---

## Decision Outcome

**Chosen option:** "Entity Framework Core", because it provides full-featured ORM with LINQ support, automatic migrations, change tracking, and excellent developer productivity while maintaining good performance for most scenarios.

### Positive Consequences

* **Productivity** - 10x less code than ADO.NET
* **Type-Safe Queries** - LINQ instead of string SQL
* **Migrations** - Automatic schema management
* **Change Tracking** - Automatic UPDATE statement generation
* **Database Agnostic** - Swap providers easily
* **Eager/Lazy Loading** - Control data loading strategy
* **Microsoft Support** - First-party, well-documented

### Negative Consequences

* **Performance Overhead** - 10-30% slower than Dapper for simple queries
* **Complexity** - N+1 query problem if not careful
* **Learning Curve** - Team needs to understand EF Core patterns
* **Black Box** - Generated SQL can be surprising

---

## Pros and Cons of the Options

### Entity Framework Core (Chosen)

**What is EF Core?**

Entity Framework Core is a modern object-relational mapper (ORM) that enables .NET developers to work with databases using .NET objects, eliminating most data-access code.

**Pros:**
* **LINQ Queries** - Write queries in C#
* **Change Tracking** - Automatically detect modifications
* **Migrations** - Schema versioning
* **Database Providers** - PostgreSQL, SQL Server, SQLite, MySQL, Cosmos DB, etc.
* **Lazy/Eager Loading** - Control when related data loads
* **Global Query Filters** - Soft delete, multi-tenancy
* **Split Queries** - Avoid cartesian explosion
* **Compiled Queries** - Cache query execution plans

**Cons:**
* **Performance** - Overhead compared to micro-ORMs
* **N+1 Problem** - Easy to create inefficient queries
* **Complex SQL** - Some queries better as raw SQL
* **Memory Usage** - Change tracking uses RAM

**Installation:**
```bash
dotnet add package Microsoft.EntityFrameworkCore
dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL
dotnet add package Microsoft.EntityFrameworkCore.Design
```

**DbContext Setup:**
```csharp
public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Product> Products { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure entities
        modelBuilder.Entity<Product>(entity =>
        {
            entity.ToTable("products");

            entity.HasKey(p => p.Id);

            entity.Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(p => p.Price)
                .HasColumnType("decimal(18,2)");

            entity.HasIndex(p => p.Name);

            // Relationship
            entity.HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryId);

            // Global query filter (soft delete)
            entity.HasQueryFilter(p => !p.IsDeleted);
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.ToTable("orders");

            // Owned type (value object)
            entity.OwnsOne(o => o.ShippingAddress, address =>
            {
                address.Property(a => a.Street).HasColumnName("shipping_street");
                address.Property(a => a.City).HasColumnName("shipping_city");
            });

            // One-to-many
            entity.HasMany(o => o.Items)
                .WithOne(i => i.Order)
                .HasForeignKey(i => i.OrderId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Seed data
        modelBuilder.Entity<Category>().HasData(
            new Category { Id = 1, Name = "Electronics" },
            new Category { Id = 2, Name = "Books" }
        );
    }
}
```

**Program.cs Registration:**
```csharp
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        npgsqlOptions =>
        {
            npgsqlOptions.EnableRetryOnFailure(
                maxRetryCount: 3,
                maxRetryDelay: TimeSpan.FromSeconds(5),
                errorCodesToAdd: null);

            npgsqlOptions.MigrationsAssembly("Infrastructure");
            npgsqlOptions.CommandTimeout(30);
        })
    .LogTo(Console.WriteLine, LogLevel.Information)  // Log SQL (dev only)
    .EnableSensitiveDataLogging()  // Log parameter values (dev only)
    .EnableDetailedErrors());      // Detailed error messages (dev only)
```

**Repository Pattern:**
```csharp
public interface IProductRepository
{
    Task<Product?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<List<Product>> GetAllAsync(CancellationToken ct = default);
    Task<Product> AddAsync(Product product, CancellationToken ct = default);
    Task UpdateAsync(Product product, CancellationToken ct = default);
    Task DeleteAsync(int id, CancellationToken ct = default);
}

public class ProductRepository : IProductRepository
{
    private readonly ApplicationDbContext _context;

    public ProductRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Product?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        return await _context.Products
            .Include(p => p.Category)  // Eager load
            .AsNoTracking()            // Read-only query (faster)
            .FirstOrDefaultAsync(p => p.Id == id, ct);
    }

    public async Task<List<Product>> GetAllAsync(CancellationToken ct = default)
    {
        return await _context.Products
            .Include(p => p.Category)
            .AsNoTracking()
            .ToListAsync(ct);
    }

    public async Task<Product> AddAsync(Product product, CancellationToken ct = default)
    {
        _context.Products.Add(product);
        await _context.SaveChangesAsync(ct);
        return product;
    }

    public async Task UpdateAsync(Product product, CancellationToken ct = default)
    {
        _context.Products.Update(product);
        await _context.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(int id, CancellationToken ct = default)
    {
        var product = await _context.Products.FindAsync(new object[] { id }, ct);
        if (product != null)
        {
            _context.Products.Remove(product);
            await _context.SaveChangesAsync(ct);
        }
    }
}
```

**Advanced Queries:**

**1. Complex LINQ:**
```csharp
// Get products with price between $10-$100, in Electronics category, ordered by name
var products = await _context.Products
    .Where(p => p.Price >= 10 && p.Price <= 100)
    .Where(p => p.Category.Name == "Electronics")
    .OrderBy(p => p.Name)
    .Select(p => new ProductDto
    {
        Id = p.Id,
        Name = p.Name,
        Price = p.Price,
        CategoryName = p.Category.Name
    })
    .ToListAsync();

// Generated SQL:
// SELECT p.id, p.name, p.price, c.name
// FROM products p
// INNER JOIN categories c ON p.category_id = c.id
// WHERE p.price >= 10 AND p.price <= 100 AND c.name = 'Electronics'
// ORDER BY p.name
```

**2. Avoid N+1 Problem:**
```csharp
// ❌ BAD: N+1 queries
var orders = await _context.Orders.ToListAsync();
foreach (var order in orders)  // 1 query
{
    var items = await _context.OrderItems
        .Where(i => i.OrderId == order.Id)
        .ToListAsync();  // N queries (1 per order)
}

// ✅ GOOD: Single query with Include
var orders = await _context.Orders
    .Include(o => o.Items)
        .ThenInclude(i => i.Product)
    .ToListAsync();  // 1 query
```

**3. Split Query (Avoid Cartesian Explosion):**
```csharp
// ❌ BAD: Cartesian explosion when including multiple collections
var orders = await _context.Orders
    .Include(o => o.Items)      // 10 items per order
    .Include(o => o.Payments)   // 3 payments per order
    .ToListAsync();
// Returns: 1 order × 10 items × 3 payments = 30 rows (duplicates!)

// ✅ GOOD: Split into separate queries
var orders = await _context.Orders
    .Include(o => o.Items)
    .Include(o => o.Payments)
    .AsSplitQuery()  // Executes 3 queries instead of 1
    .ToListAsync();
// Query 1: SELECT * FROM orders
// Query 2: SELECT * FROM order_items WHERE order_id IN (...)
// Query 3: SELECT * FROM payments WHERE order_id IN (...)
```

**4. Raw SQL (When Needed):**
```csharp
// Execute raw SQL query
var products = await _context.Products
    .FromSqlRaw("SELECT * FROM products WHERE price > {0}", 100)
    .ToListAsync();

// Execute stored procedure
var result = await _context.Database
    .ExecuteSqlRawAsync("EXEC GenerateMonthlyReport @Month={0}", month);
```

**5. Transactions:**
```csharp
using var transaction = await _context.Database.BeginTransactionAsync();
try
{
    var order = new Order { /* ... */ };
    _context.Orders.Add(order);
    await _context.SaveChangesAsync();

    foreach (var item in orderItems)
    {
        item.OrderId = order.Id;
        _context.OrderItems.Add(item);
    }
    await _context.SaveChangesAsync();

    await transaction.CommitAsync();
}
catch
{
    await transaction.RollbackAsync();
    throw;
}
```

**Migrations:**
```bash
# Create migration
dotnet ef migrations add InitialCreate --project Infrastructure --startup-project API

# Apply migration
dotnet ef database update --project Infrastructure --startup-project API

# Generate SQL script
dotnet ef migrations script --project Infrastructure --startup-project API --output migration.sql
```

### Dapper (Micro-ORM)

**Pros:**
* **Fast** - Minimal overhead, 2-3x faster than EF Core
* **Simple** - Straightforward API
* **Control** - Write exact SQL you want
* **No change tracking** - Lower memory usage

**Cons:**
* **No migrations** - Manage schema manually
* **Manual mapping** - Write SQL and map to objects
* **No LINQ** - String-based queries
* **More code** - Repository boilerplate required

**Example:**
```csharp
// Dapper
public async Task<Product?> GetByIdAsync(int id)
{
    using var connection = new NpgsqlConnection(_connectionString);
    return await connection.QueryFirstOrDefaultAsync<Product>(
        "SELECT * FROM products WHERE id = @Id",
        new { Id = id });
}

// vs EF Core
public async Task<Product?> GetByIdAsync(int id)
{
    return await _context.Products.FindAsync(id);
}
```

**When to Use Dapper:**
- Performance-critical queries
- Complex SQL that's hard in LINQ
- Read-heavy scenarios
- Complement to EF Core (use both)

**Decision:**
Use **EF Core for general data access**, Dapper for **performance-critical** read queries.

### ADO.NET (Raw SQL)

**Pros:**
* Maximum performance
* Full control
* No dependencies

**Cons:**
* **Extremely verbose** - 100+ lines for simple CRUD
* **SQL injection risk** - If not using parameters
* **Manual mapping** - Code for every property
* **No migrations** - Manage schema manually

**Example:**
```csharp
// ADO.NET (verbose!)
public async Task<Product?> GetByIdAsync(int id)
{
    using var connection = new NpgsqlConnection(_connectionString);
    await connection.OpenAsync();

    using var command = connection.CreateCommand();
    command.CommandText = "SELECT id, name, price, category_id FROM products WHERE id = @Id";
    command.Parameters.AddWithValue("@Id", id);

    using var reader = await command.ExecuteReaderAsync();
    if (await reader.ReadAsync())
    {
        return new Product
        {
            Id = reader.GetInt32(0),
            Name = reader.GetString(1),
            Price = reader.GetDecimal(2),
            CategoryId = reader.GetInt32(3)
        };
    }
    return null;
}
```

**Why Rejected:**
ADO.NET requires 10x more code than EF Core for the same functionality. Use only when maximum performance is critical.

### NHibernate

**Pros:**
* Very mature (ported from Java Hibernate)
* Powerful
* Large feature set

**Cons:**
* **XML configuration** - Less C#-friendly than EF Core
* **Legacy feel** - Not modern .NET
* **Smaller ecosystem** - Fewer contributors than EF Core
* **Microsoft doesn't support** - Third-party

**Why Not Chosen:**
EF Core is Microsoft's official ORM with better .NET integration and larger community.

---

## Performance Optimization

**1. Use AsNoTracking for Read-Only:**
```csharp
// ✅ 30% faster for read-only
var products = await _context.Products.AsNoTracking().ToListAsync();
```

**2. Select Only Needed Columns:**
```csharp
// ❌ Loads all columns
var products = await _context.Products.ToListAsync();

// ✅ Only needed columns
var products = await _context.Products
    .Select(p => new { p.Id, p.Name, p.Price })
    .ToListAsync();
```

**3. Use Compiled Queries:**
```csharp
private static readonly Func<ApplicationDbContext, int, Task<Product?>> GetProductById =
    EF.CompileAsyncQuery((ApplicationDbContext context, int id) =>
        context.Products.FirstOrDefault(p => p.Id == id));

// Usage (cached execution plan)
var product = await GetProductById(_context, 123);
```

**4. Batch Operations:**
```csharp
// ❌ Slow: One query per item
foreach (var product in products)
{
    _context.Products.Add(product);
    await _context.SaveChangesAsync();  // Don't do this!
}

// ✅ Fast: Single batch
_context.Products.AddRange(products);
await _context.SaveChangesAsync();  // One query
```

---

## Links

* [EF Core Documentation](https://learn.microsoft.com/en-us/ef/core/)
* [EF Core Performance](https://learn.microsoft.com/en-us/ef/core/performance/)
* [Sample Implementation](../../samples/05-RealWorld/MicroserviceTemplate/Infrastructure)

---

## Notes

**When to Use EF Core:**
- ✅ Standard CRUD operations
- ✅ Complex object graphs
- ✅ Need migrations
- ✅ Rapid development

**When to Use Dapper:**
- ✅ Performance-critical queries
- ✅ Complex reporting
- ✅ Read-heavy scenarios

**Common Pitfalls:**
- ❌ Not using AsNoTracking for read-only queries
- ❌ N+1 queries (forgetting Include)
- ❌ Loading entire tables (use pagination)
- ❌ Using EF Core for bulk operations (use SqlBulkCopy)

**Review Date:** 2025-12-01
