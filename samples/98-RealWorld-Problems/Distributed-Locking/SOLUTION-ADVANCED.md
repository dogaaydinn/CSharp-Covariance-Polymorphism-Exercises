# Advanced Solution: Optimistic Concurrency with Version/ETag

## Approach: Version Column + Retry Logic

```csharp
public async Task<bool> UpdateStockOptimisticAsync(int productId, int quantity)
{
    const int maxRetries = 3;
    
    for (int attempt = 0; attempt < maxRetries; attempt++)
    {
        var product = await _db.Products.FindAsync(productId);
        
        if (product.Stock < quantity)
            return false;
        
        var originalVersion = product.Version;
        product.Stock -= quantity;
        product.Version += 1;
        
        try
        {
            // This will fail if version changed (someone else updated)
            var rowsAffected = await _db.Database.ExecuteSqlRawAsync(
                "UPDATE Products SET Stock = {0}, Version = {1} WHERE Id = {2} AND Version = {3}",
                product.Stock, product.Version, productId, originalVersion);
            
            if (rowsAffected > 0)
                return true;  // Success!
            
            // Version mismatch, retry
            await Task.Delay(100 * (attempt + 1));  // Exponential backoff
        }
        catch (DbUpdateConcurrencyException)
        {
            // Concurrency conflict, retry
        }
    }
    
    return false;  // Max retries exceeded
}
```

**Pros**: Fast, no locks, scalable
**Cons**: Retries needed, not suitable for high contention

**When to Use**: Low to medium contention, multiple servers
