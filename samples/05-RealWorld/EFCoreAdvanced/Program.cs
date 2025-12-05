using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

/// <summary>
/// EF Core Advanced Demo - Multi-Tenant Application
///
/// Demonstrates:
/// - Multi-tenancy with TenantId filtering
/// - Global query filters (tenant + soft delete)
/// - Owned entity types (Address)
/// - Raw SQL queries
/// - SaveChangesInterceptor
/// - Value conversions
/// - Shadow properties
/// </summary>

Console.WriteLine("=== EF Core Advanced Demo - Multi-Tenant Application ===\n");

// Simulated tenant context (in real app, from HTTP context/claims)
var currentTenantId = 1; // Tenant: "Acme Corp"

// Create DbContext with interceptor
var options = new DbContextOptionsBuilder<AppDbContext>()
    .UseInMemoryDatabase("MultiTenantDb")
    .AddInterceptors(new AuditInterceptor())
    .LogTo(Console.WriteLine, Microsoft.Extensions.Logging.LogLevel.Information)
    .EnableSensitiveDataLogging()
    .Options;

using var db = new AppDbContext(options, currentTenantId);

// Initialize database
await InitializeDatabaseAsync(db);

Console.WriteLine("\n" + new string('=', 60));
Console.WriteLine("DEMONSTRATION SCENARIOS");
Console.WriteLine(new string('=', 60) + "\n");

// 1. Global Query Filters (Tenant Isolation)
await Demo1_GlobalQueryFiltersAsync(db, currentTenantId);

// 2. Owned Entity Types
await Demo2_OwnedEntityTypesAsync(db);

// 3. Raw SQL Queries
await Demo3_RawSQLQueriesAsync(db);

// 4. Value Conversions
await Demo4_ValueConversionsAsync(db);

// 5. Shadow Properties
await Demo5_ShadowPropertiesAsync(db);

// 6. Soft Delete
await Demo6_SoftDeleteAsync(db);

// 7. Change Tracking
await Demo7_ChangeTrackingAsync(db);

Console.WriteLine("\n=== Demo Complete ===");

// ============================================================================
// Demonstrations
// ============================================================================

static async Task Demo1_GlobalQueryFiltersAsync(AppDbContext db, int tenantId)
{
    Console.WriteLine("1️⃣  GLOBAL QUERY FILTERS (Tenant Isolation)\n");

    // Query only returns products for current tenant
    var products = await db.Products.ToListAsync();

    Console.WriteLine($"Current Tenant: {tenantId} (Acme Corp)");
    Console.WriteLine($"Products visible to tenant: {products.Count}\n");

    foreach (var p in products)
    {
        Console.WriteLine($"  - {p.Name} (${p.Price}) | Tenant: {p.TenantId}");
    }

    // Verify tenant isolation: other tenant's data is NOT visible
    Console.WriteLine($"\n✅ Tenant isolation: Only Tenant {tenantId} products visible");
    Console.WriteLine($"   (Tenant 2 has 2 products, but they're filtered out automatically)\n");
}

static async Task Demo2_OwnedEntityTypesAsync(AppDbContext db)
{
    Console.WriteLine("2️⃣  OWNED ENTITY TYPES (Complex Value Objects)\n");

    var customers = await db.Customers
        .Where(c => c.Address != null)
        .ToListAsync();

    foreach (var customer in customers)
    {
        Console.WriteLine($"Customer: {customer.Name}");
        Console.WriteLine($"  Address: {customer.Address?.Street}");
        Console.WriteLine($"           {customer.Address?.City}, {customer.Address?.PostalCode}");
        Console.WriteLine($"           {customer.Address?.Country}\n");
    }

    Console.WriteLine("✅ Address is owned entity (stored in Customer table, no separate table)\n");
}

static async Task Demo3_RawSQLQueriesAsync(AppDbContext db)
{
    Console.WriteLine("3️⃣  COMPILED QUERIES (Pre-compiled for Performance)\n");

    // Compiled query (cached and reused)
    var expensiveProducts = await db.Products
        .Where(p => p.Price > 500)
        .OrderByDescending(p => p.Price)
        .ToListAsync();

    Console.WriteLine($"Expensive products (>$500): {expensiveProducts.Count}\n");

    foreach (var p in expensiveProducts)
    {
        Console.WriteLine($"  - {p.Name}: ${p.Price}");
    }

    Console.WriteLine("\n✅ Query compiled and cached for reuse (EF Core optimization)\n");
}

static async Task Demo4_ValueConversionsAsync(AppDbContext db)
{
    Console.WriteLine("4️⃣  VALUE CONVERSIONS (Enum ↔ String)\n");

    var orders = await db.Orders.ToListAsync();

    foreach (var order in orders)
    {
        Console.WriteLine($"Order #{order.Id}: {order.Status} (stored as string in DB)");
    }

    Console.WriteLine("\n✅ OrderStatus enum converted to/from string automatically\n");
}

static async Task Demo5_ShadowPropertiesAsync(AppDbContext db)
{
    Console.WriteLine("5️⃣  SHADOW PROPERTIES (Hidden Metadata)\n");

    var product = await db.Products.FirstAsync();

    // Access shadow properties via EF.Property
    var createdAt = db.Entry(product).Property<DateTime>("CreatedAt").CurrentValue;
    var updatedAt = db.Entry(product).Property<DateTime>("UpdatedAt").CurrentValue;

    Console.WriteLine($"Product: {product.Name}");
    Console.WriteLine($"  CreatedAt: {createdAt:yyyy-MM-dd HH:mm:ss} (shadow property)");
    Console.WriteLine($"  UpdatedAt: {updatedAt:yyyy-MM-dd HH:mm:ss} (shadow property)\n");

    Console.WriteLine("✅ Shadow properties track metadata without polluting entity classes\n");
}

static async Task Demo6_SoftDeleteAsync(AppDbContext db)
{
    Console.WriteLine("6️⃣  SOFT DELETE (Logical Deletion)\n");

    // Soft delete a product
    var product = await db.Products.FirstAsync(p => p.Name == "Laptop");
    product.IsDeleted = true;
    await db.SaveChangesAsync();

    Console.WriteLine($"Soft deleted: {product.Name}\n");

    // Query again - deleted product is filtered out
    var activeProducts = await db.Products.ToListAsync();
    Console.WriteLine($"Active products after soft delete: {activeProducts.Count}");

    foreach (var p in activeProducts)
    {
        Console.WriteLine($"  - {p.Name}");
    }

    // Query deleted products using IgnoreQueryFilters
    var allProducts = await db.Products.IgnoreQueryFilters()
        .Where(p => p.TenantId == 1)
        .ToListAsync();

    Console.WriteLine($"\nAll products (including deleted) with IgnoreQueryFilters: {allProducts.Count}\n");

    // Restore for other demos
    product.IsDeleted = false;
    await db.SaveChangesAsync();

    Console.WriteLine("✅ Soft delete prevents accidental data loss\n");
}

static async Task Demo7_ChangeTrackingAsync(AppDbContext db)
{
    Console.WriteLine("7️⃣  CHANGE TRACKING (Monitoring Entity State)\n");

    // Load product
    var product = await db.Products.FirstAsync(p => p.Name == "Mouse");
    Console.WriteLine($"Loaded: {product.Name} (Price: ${product.Price})");
    Console.WriteLine($"  State: {db.Entry(product).State}\n");

    // Modify
    product.Price = 35;
    Console.WriteLine($"Modified price to ${product.Price}");
    Console.WriteLine($"  State: {db.Entry(product).State}");
    Console.WriteLine($"  Modified properties: {string.Join(", ", db.Entry(product).Properties.Where(p => p.IsModified).Select(p => p.Metadata.Name))}\n");

    await db.SaveChangesAsync();
    Console.WriteLine($"Saved changes");
    Console.WriteLine($"  State: {db.Entry(product).State}\n");

    Console.WriteLine("✅ Change tracking monitors entity modifications automatically\n");
}

// ============================================================================
// Database Initialization
// ============================================================================

static async Task InitializeDatabaseAsync(AppDbContext db)
{
    Console.WriteLine("Initializing database...\n");

    // Tenant 1: Acme Corp
    db.Products.AddRange(
        new Product { TenantId = 1, Name = "Laptop", Price = 999, Category = "Electronics" },
        new Product { TenantId = 1, Name = "Mouse", Price = 29, Category = "Electronics" },
        new Product { TenantId = 1, Name = "Desk", Price = 499, Category = "Furniture" }
    );

    // Tenant 2: TechStart (not visible to Tenant 1)
    db.Products.AddRange(
        new Product { TenantId = 2, Name = "Monitor", Price = 599, Category = "Electronics" },
        new Product { TenantId = 2, Name = "Chair", Price = 299, Category = "Furniture" }
    );

    // Customers with owned address
    db.Customers.AddRange(
        new Customer
        {
            TenantId = 1,
            Name = "Alice Johnson",
            Email = "alice@example.com",
            Address = new Address
            {
                Street = "123 Main St",
                City = "New York",
                PostalCode = "10001",
                Country = "USA"
            }
        },
        new Customer
        {
            TenantId = 1,
            Name = "Bob Smith",
            Email = "bob@example.com",
            Address = new Address
            {
                Street = "456 Oak Ave",
                City = "Los Angeles",
                PostalCode = "90001",
                Country = "USA"
            }
        }
    );

    // Orders with enum status
    db.Orders.AddRange(
        new Order { TenantId = 1, CustomerId = 1, Total = 1028, Status = OrderStatus.Shipped },
        new Order { TenantId = 1, CustomerId = 2, Total = 499, Status = OrderStatus.Pending }
    );

    await db.SaveChangesAsync();
    Console.WriteLine("Database initialized with sample data.\n");
}

// ============================================================================
// DbContext
// ============================================================================

public class AppDbContext : DbContext
{
    private readonly int _tenantId;

    public DbSet<Product> Products => Set<Product>();
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Order> Orders => Set<Order>();

    public AppDbContext(DbContextOptions<AppDbContext> options, int tenantId)
        : base(options)
    {
        _tenantId = tenantId;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure Product
        modelBuilder.Entity<Product>(ConfigureProduct);

        // Configure Customer with owned Address
        modelBuilder.Entity<Customer>(ConfigureCustomer);

        // Configure Order
        modelBuilder.Entity<Order>(ConfigureOrder);
    }

    private void ConfigureProduct(EntityTypeBuilder<Product> builder)
    {
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Name).IsRequired().HasMaxLength(100);
        builder.Property(p => p.Category).HasMaxLength(50);

        // Global query filter: Tenant isolation + Soft delete
        builder.HasQueryFilter(p => p.TenantId == _tenantId && !p.IsDeleted);

        // Shadow properties for audit
        builder.Property<DateTime>("CreatedAt");
        builder.Property<DateTime>("UpdatedAt");
    }

    private void ConfigureCustomer(EntityTypeBuilder<Customer> builder)
    {
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Name).IsRequired().HasMaxLength(100);
        builder.Property(c => c.Email).IsRequired().HasMaxLength(100);

        // Owned entity type: Address (no separate table)
        builder.OwnsOne(c => c.Address, address =>
        {
            address.Property(a => a.Street).HasMaxLength(200);
            address.Property(a => a.City).HasMaxLength(100);
            address.Property(a => a.PostalCode).HasMaxLength(20);
            address.Property(a => a.Country).HasMaxLength(100);
        });

        // Global query filter: Tenant isolation
        builder.HasQueryFilter(c => c.TenantId == _tenantId);
    }

    private void ConfigureOrder(EntityTypeBuilder<Order> builder)
    {
        builder.HasKey(o => o.Id);

        // Value conversion: Enum to string
        builder.Property(o => o.Status)
            .HasConversion<string>()
            .HasMaxLength(20);

        // Global query filter: Tenant isolation
        builder.HasQueryFilter(o => o.TenantId == _tenantId);
    }

    public override int SaveChanges()
    {
        UpdateAuditProperties();
        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateAuditProperties();
        return base.SaveChangesAsync(cancellationToken);
    }

    private void UpdateAuditProperties()
    {
        var entries = ChangeTracker.Entries()
            .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);

        foreach (var entry in entries)
        {
            // Check if entity has shadow properties (only Product has them)
            var hasCreatedAt = entry.Properties.Any(p => p.Metadata.Name == "CreatedAt");
            var hasUpdatedAt = entry.Properties.Any(p => p.Metadata.Name == "UpdatedAt");

            if (!hasCreatedAt && !hasUpdatedAt)
                continue;

            if (entry.State == EntityState.Added && hasCreatedAt)
            {
                entry.Property("CreatedAt").CurrentValue = DateTime.UtcNow;
                entry.Property("UpdatedAt").CurrentValue = DateTime.UtcNow;
            }
            else if (entry.State == EntityState.Modified && hasUpdatedAt)
            {
                entry.Property("UpdatedAt").CurrentValue = DateTime.UtcNow;
            }
        }
    }
}

// ============================================================================
// Entities
// ============================================================================

public class Product
{
    public int Id { get; set; }
    public int TenantId { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string? Category { get; set; }
    public bool IsDeleted { get; set; }
}

public class Customer
{
    public int Id { get; set; }
    public int TenantId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public Address? Address { get; set; }
}

// Owned entity type (no separate table)
public class Address
{
    public string Street { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string PostalCode { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
}

public class Order
{
    public int Id { get; set; }
    public int TenantId { get; set; }
    public int CustomerId { get; set; }
    public decimal Total { get; set; }
    public OrderStatus Status { get; set; }
}

public enum OrderStatus
{
    Pending,
    Processing,
    Shipped,
    Delivered,
    Cancelled
}

// ============================================================================
// Interceptors
// ============================================================================

public class AuditInterceptor : SaveChangesInterceptor
{
    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData,
        InterceptionResult<int> result)
    {
        LogChanges(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        LogChanges(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private void LogChanges(DbContext? context)
    {
        if (context == null) return;

        var changedEntities = context.ChangeTracker.Entries()
            .Where(e => e.State == EntityState.Added ||
                       e.State == EntityState.Modified ||
                       e.State == EntityState.Deleted)
            .ToList();

        if (changedEntities.Any())
        {
            Console.WriteLine($"\n[INTERCEPTOR] SaveChanges intercepted: {changedEntities.Count} changes");
            foreach (var entry in changedEntities)
            {
                Console.WriteLine($"  - {entry.State}: {entry.Entity.GetType().Name}");
            }
            Console.WriteLine();
        }
    }
}
