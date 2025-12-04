# ÇÖZÜM 2: DISTRIBUTED CACHING (Redis)

## 🎯 ÇÖZÜM ÖZETİ

Redis kullanarak distributed, consistent caching.

**Kimler için:** Multi-server production systems

---

## 💻 IMPLEMENTATION

```csharp
public class DistributedProductService
{
    private readonly IDistributedCache _cache;
    private readonly AppDbContext _context;
    private readonly ILogger<DistributedProductService> _logger;

    public async Task<List<Product>> GetProductsByCategoryAsync(int categoryId)
    {
        var cacheKey = $"products:category:{categoryId}";
        
        // Try to get from cache
        var cachedData = await _cache.GetStringAsync(cacheKey);
        if (cachedData != null)
        {
            _logger.LogInformation("Cache HIT for {CacheKey}", cacheKey);
            return JsonSerializer.Deserialize<List<Product>>(cachedData);
        }

        // Cache MISS - get from database
        _logger.LogWarning("Cache MISS for {CacheKey}", cacheKey);
        var products = await _context.Products
            .Where(p => p.CategoryId == categoryId && p.IsActive)
            .OrderByDescending(p => p.SalesRank)
            .ToListAsync();

        // Store in cache
        var serialized = JsonSerializer.Serialize(products);
        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5),
            SlidingExpiration = TimeSpan.FromMinutes(2)
        };
        await _cache.SetStringAsync(cacheKey, serialized, options);

        return products;
    }

    // Cache invalidation with pub/sub
    public async Task InvalidateProductCacheAsync(int categoryId)
    {
        var cacheKey = $"products:category:{categoryId}";
        await _cache.RemoveAsync(cacheKey);
        
        // Publish invalidation event to all instances
        await _redis.GetSubscriber()
            .PublishAsync("cache:invalidate", cacheKey);
    }
}
```

**Avantajlar:**
- ✅ Distributed across instances
- ✅ Survives restarts
- ✅ Consistent cache

**Dezavantajlar:**
- ❌ Redis dependency
- ❌ Network latency (1-2ms)
- ❌ Serialization overhead
