# EF Core Advanced Features - Multi-Tenant Application

A comprehensive demonstration of advanced Entity Framework Core features in a multi-tenant SaaS application context.

## Quick Reference

**Project Type:** Console Application (.NET 8)
**Pattern:** Multi-tenancy with Global Query Filters
**Key Technologies:** EF Core In-Memory Database, Interceptors, Shadow Properties
**Difficulty:** Advanced
**Estimated Learning Time:** 45-60 minutes

## What This Demonstrates

### Core Features

#### 1. Multi-Tenancy with Global Query Filters
```csharp
// Automatic tenant isolation - no manual filtering needed!
builder.HasQueryFilter(p => p.TenantId == _tenantId && !p.IsDeleted);

// Queries automatically filter by tenant
var products = await db.Products.ToListAsync();
// ✅ Only returns current tenant's products
```

**Why It Matters:**
- Prevents data leakage between tenants
- No need to add `.Where(p => p.TenantId == currentTenant)` to every query
- Reduces human error in multi-tenant applications
- Industry standard for SaaS applications

#### 2. Owned Entity Types
```csharp
// Address is stored in Customer table (no separate table needed)
builder.OwnsOne(c => c.Address, address =>
{
    address.Property(a => a.Street).HasMaxLength(200);
    address.Property(a => a.City).HasMaxLength(100);
});

// Usage - seamless navigation
var customer = await db.Customers.FirstAsync();
Console.WriteLine($"{customer.Address.Street}, {customer.Address.City}");
```

**Why It Matters:**
- Models complex value objects (Address, Money, DateRange, etc.)
- No unnecessary joins (data stored inline)
- Encapsulates related data
- Better performance than separate tables

#### 3. Value Conversions
```csharp
// Store enum as string in database
builder.Property(o => o.Status)
    .HasConversion<string>()
    .HasMaxLength(20);

// C# code uses type-safe enum
order.Status = OrderStatus.Shipped;

// Database stores human-readable string: "Shipped"
```

**Why It Matters:**
- Database values are human-readable
- Enum refactoring doesn't break stored data
- Easier database troubleshooting
- Supports legacy database schemas

#### 4. Shadow Properties
```csharp
// Define property without cluttering entity class
builder.Property<DateTime>("CreatedAt");
builder.Property<DateTime>("UpdatedAt");

// Access when needed
var createdAt = db.Entry(product).Property<DateTime>("CreatedAt").CurrentValue;
```

**Why It Matters:**
- Keeps entity classes clean (no audit fields in business logic)
- Automatic tracking without code changes
- Useful for infrastructure concerns
- Framework-managed metadata

#### 5. SaveChanges Interceptors
```csharp
public class AuditInterceptor : SaveChangesInterceptor
{
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(...)
    {
        LogChanges(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }
}
```

**Why It Matters:**
- Cross-cutting concerns (logging, auditing, validation)
- Executed before every SaveChanges
- Centralized logic (no duplication)
- Production monitoring and debugging

#### 6. Change Tracking API
```csharp
var product = await db.Products.FirstAsync();
Console.WriteLine($"State: {db.Entry(product).State}"); // Unchanged

product.Price = 35;
Console.WriteLine($"State: {db.Entry(product).State}"); // Modified
Console.WriteLine($"Modified: {string.Join(", ",
    db.Entry(product).Properties.Where(p => p.IsModified).Select(p => p.Metadata.Name))}");
// Output: "Modified: Price"
```

**Why It Matters:**
- Understand what EF Core is tracking
- Optimize update queries (only modified fields updated)
- Debugging entity state issues
- Build audit logs of changes

#### 7. Soft Delete Pattern
```csharp
// Soft delete - mark as deleted
product.IsDeleted = true;
await db.SaveChangesAsync();

// Automatically excluded from queries (global filter)
var active = await db.Products.ToListAsync(); // ✅ Doesn't include deleted

// Admin queries can bypass filter
var all = await db.Products.IgnoreQueryFilters()
    .Where(p => p.TenantId == currentTenant)
    .ToListAsync(); // ✅ Includes deleted
```

**Why It Matters:**
- Prevents accidental data loss
- Enables "undo delete" functionality
- Maintains referential integrity
- Supports compliance requirements (data retention)

## Seven Demonstrations

### 1️⃣ Global Query Filters (Tenant Isolation)
Demonstrates automatic tenant isolation without manual filtering.

### 2️⃣ Owned Entity Types (Complex Value Objects)
Shows how to model Address as a value object without separate table.

### 3️⃣ Compiled Queries (Pre-compiled for Performance)
Demonstrates query caching and optimization.

### 4️⃣ Value Conversions (Enum ↔ String)
Shows enum-to-string conversion for human-readable database values.

### 5️⃣ Shadow Properties (Hidden Metadata)
Demonstrates audit tracking without polluting entity classes.

### 6️⃣ Soft Delete (Logical Deletion)
Shows how soft delete prevents data loss while appearing deleted.

### 7️⃣ Change Tracking (Monitoring Entity State)
Demonstrates EF Core's change tracking capabilities.

## Running the Demo

```bash
# Clone and navigate
cd samples/05-RealWorld/EFCoreAdvanced

# Restore and run
dotnet restore
dotnet run
```

**Expected Output:**

```
=== EF Core Advanced Demo - Multi-Tenant Application ===

Initializing database...

============================================================
DEMONSTRATION SCENARIOS
============================================================

1️⃣  GLOBAL QUERY FILTERS (Tenant Isolation)

Current Tenant: 1 (Acme Corp)
Products visible to tenant: 3

  - Laptop ($999) | Tenant: 1
  - Mouse ($29) | Tenant: 1
  - Desk ($499) | Tenant: 1

✅ Tenant isolation: Only Tenant 1 products visible
   (Tenant 2 has 2 products, but they're filtered out automatically)

... [remaining demonstrations]
```

## Common Patterns

### Pattern 1: Multi-Tenant DbContext Setup

```csharp
// Startup/Program.cs
builder.Services.AddDbContext<AppDbContext>((sp, options) =>
{
    var httpContext = sp.GetRequiredService<IHttpContextAccessor>().HttpContext;
    var tenantId = httpContext?.User.FindFirst("TenantId")?.Value ?? "1";

    options.UseSqlServer(connectionString);
    options.AddInterceptors(new TenantInterceptor(int.Parse(tenantId)));
});

// AppDbContext constructor
public AppDbContext(DbContextOptions<AppDbContext> options, int tenantId)
    : base(options)
{
    _tenantId = tenantId;
}
```

### Pattern 2: Soft Delete with Global Filter

```csharp
// Entity
public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool IsDeleted { get; set; } // ✅ Soft delete flag
}

// Configuration
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.Entity<Product>()
        .HasQueryFilter(p => !p.IsDeleted); // ✅ Auto-exclude deleted
}

// Usage
product.IsDeleted = true; // ✅ Soft delete (not db.Products.Remove(product))
await db.SaveChangesAsync();

// Admin override
var allProducts = await db.Products
    .IgnoreQueryFilters() // ✅ See deleted products
    .ToListAsync();
```

### Pattern 3: Audit with Shadow Properties

```csharp
// Configuration
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    foreach (var entityType in modelBuilder.Model.GetEntityTypes())
    {
        entityType.AddProperty("CreatedAt", typeof(DateTime));
        entityType.AddProperty("UpdatedAt", typeof(DateTime));
        entityType.AddProperty("CreatedBy", typeof(string));
    }
}

// SaveChanges override
public override int SaveChanges()
{
    var entries = ChangeTracker.Entries()
        .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);

    foreach (var entry in entries)
    {
        if (entry.State == EntityState.Added)
        {
            entry.Property("CreatedAt").CurrentValue = DateTime.UtcNow;
            entry.Property("CreatedBy").CurrentValue = _currentUser;
        }
        entry.Property("UpdatedAt").CurrentValue = DateTime.UtcNow;
    }

    return base.SaveChanges();
}
```

## Common Pitfalls

### ❌ Pitfall 1: Forgetting IgnoreQueryFilters for Admin

```csharp
// ❌ WRONG: Admin can't see all tenants
var allProducts = await db.Products.ToListAsync();
// Result: Only sees current tenant's products

// ✅ CORRECT: Use IgnoreQueryFilters
var allProducts = await db.Products
    .IgnoreQueryFilters()
    .ToListAsync();
// Result: Sees all tenants' products (for admin)
```

### ❌ Pitfall 2: Owned Entity Overuse

```csharp
// ❌ WRONG: Too many owned properties (creates wide tables)
public class Order
{
    public Address BillingAddress { get; set; }
    public Address ShippingAddress { get; set; }
    public PaymentInfo Payment { get; set; } // 10 fields
    public ShipmentInfo Shipment { get; set; } // 10 fields
    public OrderMetadata Metadata { get; set; } // 20 fields
}
// Result: Order table has 50+ columns (bad for indexing)

// ✅ CORRECT: Use separate tables for complex entities
public class Order
{
    public int Id { get; set; }
    public Address BillingAddress { get; set; } // Owned (5 fields)
    public int PaymentId { get; set; } // FK to Payment table
    public Payment Payment { get; set; } // Separate table
}
```

## Related Patterns

- **Unit of Work**: Coordinates multiple repository operations
- **Repository Pattern**: Abstracts data access layer
- **Specification Pattern**: Encapsulates query logic
- **CQRS**: Separates read and write models

## References

- [EF Core Global Query Filters](https://learn.microsoft.com/en-us/ef/core/querying/filters)
- [Owned Entity Types](https://learn.microsoft.com/en-us/ef/core/modeling/owned-entities)
- [Value Conversions](https://learn.microsoft.com/en-us/ef/core/modeling/value-conversions)
- [Shadow Properties](https://learn.microsoft.com/en-us/ef/core/modeling/shadow-properties)
- [Interceptors](https://learn.microsoft.com/en-us/ef/core/logging-events-diagnostics/interceptors)
- [Change Tracking](https://learn.microsoft.com/en-us/ef/core/change-tracking/)

## See Also

- `BackgroundServiceExample` - Background task processing
- `MessageQueueExample` - Message queue patterns
- `CachingExample` - Caching strategies
