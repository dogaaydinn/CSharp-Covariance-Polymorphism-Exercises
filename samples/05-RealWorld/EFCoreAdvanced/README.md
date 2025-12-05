# EF Core Advanced Patterns

> Advanced Entity Framework Core patterns: global query filters, owned types, performance.

## Features
- **Global Query Filters** - Auto-apply soft delete filtering
- **In-Memory Database** - Testing without real database
- **Change Tracking** - Optimized queries

## Patterns
```csharp
// Soft Delete Filter
modelBuilder.Entity<Product>()
    .HasQueryFilter(p => !p.IsDeleted);

// Now all queries auto-filter deleted items!
var active = db.Products.ToList();  // Only active products
```

## Run
```bash
dotnet run
```
