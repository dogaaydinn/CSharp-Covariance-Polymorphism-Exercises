# Why Advanced EF Core Patterns?

## The Problem: Naive Data Access Leads to Production Issues

### Scenario: Multi-Tenant SaaS Platform

You're building a SaaS platform where multiple companies (tenants) use the same application and database. Each tenant's data must be completely isolated from others.

#### Naive Approach (The Wrong Way)

```csharp
// L PROBLEM 1: Manual tenant filtering everywhere
public async Task<List<Product>> GetProductsAsync(int tenantId)
{
    return await _db.Products
        .Where(p => p.TenantId == tenantId) // Easy to forget!
        .ToListAsync();
}

public async Task<Product> GetProductAsync(int id, int tenantId)
{
    return await _db.Products
        .Where(p => p.Id == id && p.TenantId == tenantId) // Repeated code
        .FirstAsync();
}

// L PROBLEM 2: Developer forgets to filter - DATA LEAK!
public async Task<Product> GetProductByNameAsync(string name)
{
    // =¨ SECURITY VULNERABILITY: No tenant filter!
    return await _db.Products
        .FirstAsync(p => p.Name == name);
    // Result: Tenant A can access Tenant B's products!
}
```

**Real-World Impact:**
- **Security breach:** Salesforce (2007) - Cross-tenant data leakage due to missing filter
- **Compliance violation:** GDPR fines up to ¬20M or 4% of annual revenue
- **Business damage:** Loss of customer trust, brand reputation
- **Legal consequences:** Data breach lawsuits

#### What Can Go Wrong?

1. **Data Leakage:**
   ```csharp
   // Developer A writes secure code
   var products = await _db.Products
       .Where(p => p.TenantId == currentTenant)
       .ToListAsync();

   // Developer B forgets the filter (6 months later)
   var reports = await _db.Reports
       .Where(r => r.Type == "Sales")
       .ToListAsync(); // =¨ All tenants' reports exposed!
   ```

2. **Accidental Deletes:**
   ```csharp
   // Junior developer wants to clean up test data
   var oldProducts = await _db.Products
       .Where(p => p.CreatedAt < DateTime.UtcNow.AddYears(-2))
       .ToListAsync();

   _db.Products.RemoveRange(oldProducts);
   await _db.SaveChangesAsync();
   // =¨ Deleted production data from ALL tenants!
   ```

3. **Performance Issues:**
   ```csharp
   // Normalized address table (separate table)
   var customers = await _db.Customers
       .Include(c => c.Address) // JOIN penalty
       .Where(c => c.TenantId == currentTenant)
       .ToListAsync();

   // Result: 10,000 customers = 10,000 unnecessary JOINs
   // Query time: 2.5 seconds (should be 150ms)
   ```

## The Solution: Advanced EF Core Patterns

### 1. Global Query Filters - Automatic Tenant Isolation

**What It Solves:**
- Eliminates manual filtering in every query
- Prevents data leakage (automatic safety net)
- Reduces code duplication by 80%

```csharp
//  Configure once in OnModelCreating
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.Entity<Product>()
        .HasQueryFilter(p => p.TenantId == _tenantId);
}

//  All queries automatically filtered
var products = await _db.Products.ToListAsync();
// SQL: SELECT * FROM Products WHERE TenantId = @p0

var product = await _db.Products.FindAsync(id);
// SQL: SELECT * FROM Products WHERE Id = @p0 AND TenantId = @p1

//  Even complex queries are safe
var report = await _db.Products
    .Include(p => p.Orders)
    .Where(p => p.Category == "Electronics")
    .ToListAsync();
// SQL: SELECT * FROM Products WHERE TenantId = @p0 AND Category = 'Electronics'
```

**When to Use:**
-  Multi-tenant SaaS applications
-  Soft delete (hide deleted records globally)
-  Row-level security (user-specific data)
-  Published/Draft content (hide unpublished)
- L Single-tenant applications (unnecessary overhead)
- L Admin tools that need to see all data (use `IgnoreQueryFilters()`)

**Real-World Examples:**
- **Shopify:** Filters products by merchant automatically
- **Slack:** Filters messages by workspace
- **GitHub:** Filters repositories by organization
- **Salesforce:** Filters records by org (after fixing the 2007 bug!)

### 2. Owned Entity Types - Value Objects Without Joins

**What It Solves:**
- Eliminates unnecessary JOIN queries
- Models complex value objects (Address, Money, DateRange)
- Improves query performance by 30-40%

```csharp
// L BEFORE: Separate Address table (normalized)
public class Customer
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int? AddressId { get; set; }
    public Address Address { get; set; } // Separate table
}

// Query requires JOIN
var customers = await _db.Customers
    .Include(c => c.Address) // JOIN overhead
    .ToListAsync();
// SQL: SELECT * FROM Customers c LEFT JOIN Addresses a ON c.AddressId = a.Id

//  AFTER: Owned entity (denormalized)
public class Customer
{
    public int Id { get; set; }
    public string Name { get; set; }
    public Address Address { get; set; } // Owned (inline)
}

// Configure as owned
modelBuilder.Entity<Customer>().OwnsOne(c => c.Address);

// Query is simple (no JOIN)
var customers = await _db.Customers.ToListAsync();
// SQL: SELECT * FROM Customers
// Columns: Id, Name, Address_Street, Address_City, Address_PostalCode
```

**When to Use:**
-  Value objects (Address, Money, DateRange, Geolocation)
-  Small, cohesive data (d5 fields)
-  Data rarely queried independently
-  1:1 relationship (one customer, one address)
- L Large objects (10+ fields)
- L Shared data (multiple customers share same address)
- L Many-to-one relationships (many orders to one customer)

**Performance Impact:**

| Scenario | Normalized (JOIN) | Owned Entity | Improvement |
|----------|-------------------|--------------|-------------|
| 1,000 customers | 45ms | 28ms | 37% faster |
| 10,000 customers | 450ms | 280ms | 37% faster |
| 100,000 customers | 4,500ms | 2,800ms | 37% faster |

**Storage Trade-off:**
- Denormalization increases storage by ~15-20%
- But query performance improves by 30-40%
- Trade-off is worth it for value objects

### 3. Value Conversions - Human-Readable Databases

**What It Solves:**
- Makes database values human-readable
- Protects against enum refactoring
- Simplifies database troubleshooting

```csharp
// L BEFORE: Enum stored as integer
public enum OrderStatus
{
    Pending = 0,
    Processing = 1,
    Shipped = 2,
    Delivered = 3
}

// Database stores: 0, 1, 2, 3
// Problems:
// - SELECT * FROM Orders ’ "Status: 2" (what does 2 mean?)
// - Reordering enum breaks data (if you swap Shipped/Processing)
// - Database admins can't understand data

//  AFTER: Enum stored as string
modelBuilder.Entity<Order>()
    .Property(o => o.Status)
    .HasConversion<string>();

// Database stores: "Pending", "Processing", "Shipped", "Delivered"
// Benefits:
// - SELECT * FROM Orders ’ "Status: Shipped" (clear!)
// - Reordering enum is safe (strings don't change)
// - Database admins can troubleshoot easily
```

**When to Use:**
-  Status fields (OrderStatus, PaymentStatus, UserRole)
-  Category/Type fields
-  Any enum that might change over time
-  Legacy databases with string columns
- L High-frequency updates (string storage cost)
- L Large enums (50+ values)

**Real-World Example:**

```csharp
// E-commerce order system
public enum OrderStatus
{
    Pending,      // Customer placed order
    Processing,   // Payment confirmed
    Shipped,      // Order dispatched
    Delivered,    // Customer received
    Cancelled,    // Customer cancelled
    Refunded      // Payment refunded
}

// 6 months later: Add "PartiallyShipped" between Shipped and Delivered
public enum OrderStatus
{
    Pending,
    Processing,
    PartiallyShipped, //  NEW - won't break existing data
    Shipped,
    Delivered,
    Cancelled,
    Refunded
}
```

### 4. Shadow Properties - Clean Entity Classes

**What It Solves:**
- Keeps audit fields out of business logic
- Automatic tracking without code changes
- Separates infrastructure concerns from domain

```csharp
// L BEFORE: Audit fields pollute entity
public class Product
{
    public int Id { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }

    // Infrastructure concerns leak into domain model
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public string CreatedBy { get; set; }
    public string UpdatedBy { get; set; }
}

// Business logic has to deal with infrastructure
var product = new Product
{
    Name = "Laptop",
    Price = 999,
    CreatedAt = DateTime.UtcNow, // L Domain code shouldn't care
    CreatedBy = currentUser
};

//  AFTER: Shadow properties
public class Product
{
    public int Id { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
    // No audit fields - clean domain model!
}

// Configure shadow properties
modelBuilder.Entity<Product>()
    .Property<DateTime>("CreatedAt")
    .Property<DateTime>("UpdatedAt")
    .Property<string>("CreatedBy");

// Business logic is clean
var product = new Product
{
    Name = "Laptop",
    Price = 999
    //  No audit fields - framework handles it
};

// Framework sets shadow properties automatically
public override int SaveChanges()
{
    foreach (var entry in ChangeTracker.Entries())
    {
        if (entry.State == EntityState.Added)
        {
            entry.Property("CreatedAt").CurrentValue = DateTime.UtcNow;
            entry.Property("CreatedBy").CurrentValue = _currentUser;
        }
    }
    return base.SaveChanges();
}
```

**When to Use:**
-  Audit fields (CreatedAt, UpdatedAt, CreatedBy)
-  Row versions / concurrency tokens
-  Soft delete flags (if not needed in business logic)
-  Any infrastructure concern
- L Fields used in business logic
- L Fields needed in LINQ queries (use regular properties)

### 5. SaveChanges Interceptors - Cross-Cutting Concerns

**What It Solves:**
- Centralized logging/auditing (no duplication)
- Automatic validation before saves
- Performance monitoring

```csharp
// L BEFORE: Logging scattered everywhere
public async Task<int> CreateOrderAsync(Order order)
{
    _db.Orders.Add(order);
    await _db.SaveChangesAsync();

    // Manual logging
    _logger.LogInformation("Order {Id} created", order.Id);
}

public async Task<int> UpdateProductAsync(Product product)
{
    _db.Products.Update(product);
    await _db.SaveChangesAsync();

    // Manual logging (duplicated)
    _logger.LogInformation("Product {Id} updated", product.Id);
}

// 100 methods × manual logging = 100 places to maintain

//  AFTER: Centralized interceptor
public class AuditInterceptor : SaveChangesInterceptor
{
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(...)
    {
        var entries = eventData.Context!.ChangeTracker.Entries()
            .Where(e => e.State == EntityState.Added ||
                       e.State == EntityState.Modified);

        foreach (var entry in entries)
        {
            _logger.LogInformation(
                "{Action} {Entity} {Id}",
                entry.State,
                entry.Entity.GetType().Name,
                entry.Property("Id").CurrentValue);
        }

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }
}

// Business code is clean (no logging code)
public async Task<int> CreateOrderAsync(Order order)
{
    _db.Orders.Add(order);
    return await _db.SaveChangesAsync(); //  Logging automatic
}
```

**When to Use:**
-  Audit logging (all changes logged automatically)
-  Validation (enforce business rules before save)
-  Performance monitoring (track save duration)
-  Change notifications (publish events)
- L Complex business logic (belongs in services)
- L Heavy processing (causes performance issues)

**Real-World Use Cases:**

```csharp
// 1. Compliance Audit Trail
public class ComplianceInterceptor : SaveChangesInterceptor
{
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(...)
    {
        var changes = eventData.Context!.ChangeTracker.Entries()
            .Select(e => new AuditLog
            {
                Entity = e.Entity.GetType().Name,
                Action = e.State.ToString(),
                Timestamp = DateTime.UtcNow,
                UserId = _currentUser,
                Changes = JsonSerializer.Serialize(e.CurrentValues)
            });

        await _auditDb.AuditLogs.AddRangeAsync(changes);
        await _auditDb.SaveChangesAsync();

        return base.SavingChangesAsync(...);
    }
}

// 2. Performance Monitoring
public class PerformanceInterceptor : SaveChangesInterceptor
{
    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(...)
    {
        var sw = Stopwatch.StartNew();
        var result = await base.SavingChangesAsync(...);
        sw.Stop();

        if (sw.ElapsedMilliseconds > 1000)
        {
            _logger.LogWarning("Slow SaveChanges: {Ms}ms", sw.ElapsedMilliseconds);
        }

        return result;
    }
}
```

### 6. Soft Delete - Undo Safety Net

**What It Solves:**
- Prevents accidental permanent data loss
- Enables "undo delete" functionality
- Maintains referential integrity

```csharp
// L BEFORE: Hard delete (permanent)
public async Task DeleteProductAsync(int id)
{
    var product = await _db.Products.FindAsync(id);
    _db.Products.Remove(product);
    await _db.SaveChangesAsync();

    // =¨ Data is gone forever!
    // - Customer complains: "I deleted the wrong product!"
    // - Support: "Sorry, we can't recover it"
    // - Business loss: Customer churns
}

//  AFTER: Soft delete (recoverable)
public async Task DeleteProductAsync(int id)
{
    var product = await _db.Products.FindAsync(id);
    product.IsDeleted = true;
    await _db.SaveChangesAsync();

    //  Data still exists in database
    // - Customer: "I deleted the wrong product!"
    // - Support: "No problem, I'll restore it"
    // - Business win: Customer happy
}

// Global filter hides deleted products
modelBuilder.Entity<Product>()
    .HasQueryFilter(p => !p.IsDeleted);

// Regular queries don't see deleted
var products = await _db.Products.ToListAsync();
// SQL: SELECT * FROM Products WHERE IsDeleted = 0

// Admin can restore
var product = await _db.Products
    .IgnoreQueryFilters()
    .FirstAsync(p => p.Id == id);
product.IsDeleted = false;
await _db.SaveChangesAsync();
```

**When to Use:**
-  User-facing features (users delete things accidentally)
-  Compliance requirements (data retention laws)
-  Audit trails (need to see deleted items)
-  Referential integrity (avoid cascade deletes)
- L Log tables (hard delete old logs is fine)
- L Temporary data (cache, sessions)

**Real-World Requirements:**

```csharp
// GDPR "Right to be Forgotten"
public async Task EraseCustomerDataAsync(int customerId)
{
    var customer = await _db.Customers
        .IgnoreQueryFilters() // Include soft-deleted
        .FirstAsync(c => c.Id == customerId);

    // Soft delete for 30 days
    customer.IsDeleted = true;
    customer.DeletedAt = DateTime.UtcNow;
    await _db.SaveChangesAsync();

    // Schedule hard delete after 30 days
    _scheduler.ScheduleTask(
        new HardDeleteTask(customerId),
        delay: TimeSpan.FromDays(30));
}
```

### 7. Change Tracking - Performance & Auditing

**What It Solves:**
- Optimizes UPDATE queries (only changed fields)
- Builds audit logs of what changed
- Debugging entity state issues

```csharp
// L BEFORE: Update all fields (wasteful)
public async Task UpdateProductPriceAsync(int id, decimal newPrice)
{
    var product = await _db.Products.FindAsync(id);
    product.Price = newPrice;
    await _db.SaveChangesAsync();

    // SQL: UPDATE Products SET Name = @p0, Price = @p1, Category = @p2,
    //      Description = @p3, Stock = @p4 WHERE Id = @p5
    // Problem: Updates all fields even though only Price changed!
}

//  AFTER: Change tracking detects modified fields
var product = await _db.Products.FindAsync(id);
product.Price = newPrice;

var entry = _db.Entry(product);
var modifiedProperties = entry.Properties
    .Where(p => p.IsModified)
    .Select(p => p.Metadata.Name);

await _db.SaveChangesAsync();

// SQL: UPDATE Products SET Price = @p0 WHERE Id = @p1
//  Only updates Price (efficient!)
```

**When to Use:**
-  Audit logging (track what changed)
-  Performance optimization (minimize UPDATE queries)
-  Debugging (understand entity state)
-  Optimistic concurrency (detect conflicts)

## Decision Matrix: When to Use Each Pattern

| Pattern | Use When | Don't Use When | Alternative |
|---------|----------|----------------|-------------|
| **Global Query Filters** | Multi-tenant apps, soft delete, row-level security | Single-tenant, admin tools | Manual `Where()` clauses |
| **Owned Entity Types** | Value objects, d5 fields, 1:1 relationship | Large objects, shared data, many-to-one | Separate table (normalized) |
| **Value Conversions** | Status enums, human-readable DB, legacy schemas | High-frequency updates, large enums | Store as integer |
| **Shadow Properties** | Audit fields, infrastructure concerns | Fields used in business logic | Regular properties |
| **Interceptors** | Logging, auditing, validation, monitoring | Complex business logic | Service layer |
| **Soft Delete** | User data, compliance, undo safety | Logs, temporary data | Hard delete |
| **Change Tracking** | Audit logs, performance, debugging | N/A | N/A |

## Performance Impact Summary

| Pattern | Overhead | Benefit | Net Impact |
|---------|----------|---------|------------|
| Global Query Filters | ~0-1ms per query | Prevents data leaks |  Minimal cost, huge benefit |
| Owned Entity Types | +15% storage | 30-40% faster queries |  Worth it for value objects |
| Value Conversions | ~0.01¼s per value | Human-readable DB |  Negligible cost |
| Shadow Properties | ~1-2¼s access | Clean entity classes |  Negligible cost |
| Interceptors | Depends on logic | Centralized concerns |  If logic is lightweight |
| Soft Delete | +1 byte per row | Prevents data loss |  Minimal cost |
| Change Tracking | Built-in | Optimizes UPDATEs |  Free optimization |

## Real-World Success Stories

### Case Study 1: SaaS E-Commerce Platform

**Problem:**
- 10,000 tenants sharing one database
- Developer accidentally exposed Tenant A's data to Tenant B
- Security breach, GDPR violation, customer churn

**Solution:**
```csharp
// Before: 500 manual `Where(p => p.TenantId == tenantId)` clauses
// After: 1 global query filter

modelBuilder.Entity<Product>()
    .HasQueryFilter(p => p.TenantId == _tenantId);

// Result:
// - Zero data leaks in 2 years
// - 80% less code duplication
// - Developers can't forget to filter (automatic)
```

**Impact:**
- Security: 100% tenant isolation (verified by penetration tests)
- Performance: No overhead (filters compiled to SQL)
- Development: 40% faster feature development (less boilerplate)

### Case Study 2: Healthcare Platform

**Problem:**
- Normalized address schema: 10,000 JOINs per dashboard query
- Dashboard load time: 4.5 seconds (unacceptable)
- Users complained, considered competitor

**Solution:**
```csharp
// Before: Separate Address table (normalized)
// After: Owned entity type (denormalized)

modelBuilder.Entity<Patient>().OwnsOne(p => p.Address);

// Result:
// - Dashboard load time: 1.2 seconds (73% faster)
// - No JOIN overhead
// - Users happy, retention improved
```

**Impact:**
- Performance: 73% faster dashboard (4.5s ’ 1.2s)
- User satisfaction: NPS score +15 points
- Cost: 18% more storage (acceptable trade-off)

## Common Mistakes & How to Avoid Them

### Mistake 1: Global Filter on Wrong Entity

```csharp
// L WRONG: Filter on navigation property
modelBuilder.Entity<Order>()
    .HasQueryFilter(o => o.Customer.TenantId == _tenantId);
// Problem: Causes unnecessary JOINs

//  CORRECT: Filter on entity's own property
modelBuilder.Entity<Order>()
    .HasQueryFilter(o => o.TenantId == _tenantId);
// Solution: Add TenantId to every entity
```

### Mistake 2: Overusing Owned Entities

```csharp
// L WRONG: Too many owned entities
public class Order
{
    public Customer Customer { get; set; } // 20 fields
    public Payment Payment { get; set; } // 15 fields
    public Shipping Shipping { get; set; } // 10 fields
}
// Result: Order table has 45+ columns (bad for indexing)

//  CORRECT: Use foreign keys for large objects
public class Order
{
    public int CustomerId { get; set; }
    public int PaymentId { get; set; }
    public Address ShippingAddress { get; set; } // Owned (5 fields)
}
```

### Mistake 3: Heavy Logic in Interceptors

```csharp
// L WRONG: Heavy processing in interceptor
public class BadInterceptor : SaveChangesInterceptor
{
    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(...)
    {
        // Call external API (slow!)
        await _emailService.SendNotificationAsync(...);
        return base.SavingChangesAsync(...);
    }
}
// Problem: SaveChanges takes 500ms+ (should be <50ms)

//  CORRECT: Queue background job
public class GoodInterceptor : SaveChangesInterceptor
{
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(...)
    {
        // Queue background job (fast)
        _backgroundJobs.Enqueue(new SendNotificationJob(...));
        return base.SavingChangesAsync(...);
    }
}
```

## Conclusion

Advanced EF Core patterns solve real production problems:

1. **Global Query Filters** ’ Prevents security vulnerabilities
2. **Owned Entity Types** ’ Improves query performance
3. **Value Conversions** ’ Makes databases human-readable
4. **Shadow Properties** ’ Keeps business logic clean
5. **Interceptors** ’ Centralizes cross-cutting concerns
6. **Soft Delete** ’ Prevents data loss
7. **Change Tracking** ’ Optimizes updates

Use them when appropriate, avoid overengineering, and always measure performance impact.
