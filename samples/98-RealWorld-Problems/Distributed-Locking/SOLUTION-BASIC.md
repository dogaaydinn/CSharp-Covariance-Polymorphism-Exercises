# Basic Solution: Database Pessimistic Locking

## Approach: Row-Level Locking with SQL FOR UPDATE

```csharp
using var transaction = await _db.Database.BeginTransactionAsync();
try
{
    // Lock row for update
    var product = await _db.Products
        .FromSqlRaw("SELECT * FROM Products WHERE Id = {0} FOR UPDATE", id)
        .FirstOrDefaultAsync();
    
    if (product.Stock > 0)
    {
        product.Stock -= 1;
        await _db.SaveChangesAsync();
        await transaction.CommitAsync();
        return true;
    }
    
    await transaction.RollbackAsync();
    return false;
}
catch
{
    await transaction.RollbackAsync();
    throw;
}
```

**Pros**: Simple, built-in database feature
**Cons**: Slow, limited to single database, deadlock risk

**When to Use**: Single-server applications, low concurrency
