# Basic Solution: Cache-Aside Pattern with In-Memory Cache

## 🎯 Yaklaşım

**Cache-Aside** (Lazy Loading) - En basit ve yaygın kullanılan cache stratejisi. Uygulama cache'i manuel yönetir.

## 🔧 Nasıl Çalışır?

```csharp
// 1. Check cache
var product = await _cache.GetAsync<Product>($"product:{id}");

// 2. If miss, load from database
if (product == null)
{
    product = await _db.Products.FindAsync(id);

    // 3. Store in cache
    await _cache.SetAsync($"product:{id}", product, TimeSpan.FromMinutes(5));
}

return product;
```

### Flow Diagram

```
Request → Check Cache → Cache Hit? → Return Data
                            ↓ No
                      Load from DB
                            ↓
                      Store in Cache
                            ↓
                      Return Data
```

## ✅ Avantajlar

1. **Basit**: Kolay anlaşılır ve implement edilir
2. **Lazy Loading**: Sadece gerekli data cache'lenir
3. **Resilient**: Cache fail olsa bile app çalışır
4. **Flexible**: TTL ve invalidation kontrolü kolay

## ❌ Dezavantajlar

1. **Cache Miss Penalty**: İlk request yavaş
2. **Stale Data**: TTL boyunca eski data dönebilir
3. **Cache Stampede**: Aynı anda çok request cache miss yapabilir

## 💾 IMemoryCache Kullanımı

### Basic Example

```csharp
public class ProductService
{
    private readonly IMemoryCache _cache;
    private readonly AppDbContext _db;

    public async Task<Product> GetProductAsync(int id)
    {
        var cacheKey = $"product:{id}";

        // Try get from cache
        if (_cache.TryGetValue(cacheKey, out Product cachedProduct))
        {
            Console.WriteLine("[Cache HIT]");
            return cachedProduct;
        }

        Console.WriteLine("[Cache MISS]");

        // Load from database
        var product = await _db.Products.FindAsync(id);

        // Store in cache with 5 minute expiration
        var cacheOptions = new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5),
            Priority = CacheItemPriority.Normal
        };

        _cache.Set(cacheKey, product, cacheOptions);

        return product;
    }
}
```

### With GetOrCreateAsync

```csharp
public async Task<Product> GetProductAsync(int id)
{
    return await _cache.GetOrCreateAsync($"product:{id}", async entry =>
    {
        entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);

        Console.WriteLine("[Cache MISS] Loading from database");
        return await _db.Products.FindAsync(id);
    });
}
```

## 🔄 Cache Invalidation

### Manual Invalidation

```csharp
public async Task UpdateProductAsync(Product product)
{
    // Update database
    _db.Products.Update(product);
    await _db.SaveChangesAsync();

    // Invalidate cache
    _cache.Remove($"product:{product.Id}");

    Console.WriteLine($"[Cache INVALIDATED] Product {product.Id}");
}
```

### Pattern-Based Invalidation

```csharp
public void InvalidateProductCache(int productId)
{
    // Invalidate single product
    _cache.Remove($"product:{productId}");

    // Invalidate related caches
    _cache.Remove($"product:{productId}:reviews");
    _cache.Remove($"product:{productId}:stock");
    _cache.Remove("products:list"); // Invalidate list cache
}
```

## ⏱️ TTL Strategies

### Fixed TTL

```csharp
// All products cached for 5 minutes
entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);
```

### Sliding Expiration

```csharp
// Reset expiration on each access
entry.SlidingExpiration = TimeSpan.FromMinutes(5);
// If accessed within 5 minutes, stays in cache
```

### Priority-Based

```csharp
// High priority items stay longer
if (product.IsPopular)
{
    entry.Priority = CacheItemPriority.High;
    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1);
}
else
{
    entry.Priority = CacheItemPriority.Low;
    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);
}
```

## 📊 Performance Comparison

### Before Caching

```
Request → Database → 200ms
1000 requests = 1000 DB queries = 200,000ms (3.3 minutes!)
Database CPU: 85%
```

### After Caching (90% hit rate)

```
900 requests → Cache → 0.5ms each = 450ms
100 requests → Database → 200ms each = 20,000ms
Total: 20,450ms (vs 200,000ms)
Improvement: 90% faster!
Database CPU: 15%
```

## 🧪 Complete Example

```csharp
public class ProductService
{
    private readonly IMemoryCache _cache;
    private readonly AppDbContext _db;
    private readonly ILogger<ProductService> _logger;

    public ProductService(
        IMemoryCache cache,
        AppDbContext db,
        ILogger<ProductService> logger)
    {
        _cache = cache;
        _db = db;
        _logger = logger;
    }

    public async Task<Product?> GetProductAsync(int id)
    {
        var cacheKey = $"product:{id}";

        return await _cache.GetOrCreateAsync(cacheKey, async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);
            entry.Priority = CacheItemPriority.Normal;

            // Post-eviction callback
            entry.RegisterPostEvictionCallback((key, value, reason, state) =>
            {
                _logger.LogInformation(
                    "Cache evicted: {Key}, Reason: {Reason}",
                    key, reason);
            });

            _logger.LogInformation("Cache MISS for product {Id}", id);

            var product = await _db.Products
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == id);

            return product;
        });
    }

    public async Task<List<Product>> GetProductListAsync(int categoryId)
    {
        var cacheKey = $"products:category:{categoryId}";

        return await _cache.GetOrCreateAsync(cacheKey, async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10);

            _logger.LogInformation(
                "Cache MISS for category {CategoryId}",
                categoryId);

            return await _db.Products
                .Where(p => p.CategoryId == categoryId)
                .AsNoTracking()
                .ToListAsync();
        });
    }

    public async Task UpdateProductAsync(Product product)
    {
        _db.Products.Update(product);
        await _db.SaveChangesAsync();

        // Invalidate caches
        _cache.Remove($"product:{product.Id}");
        _cache.Remove($"products:category:{product.CategoryId}");

        _logger.LogInformation("Product {Id} updated and cache invalidated", product.Id);
    }

    public async Task<int> DeleteProductAsync(int id)
    {
        var product = await _db.Products.FindAsync(id);
        if (product == null)
            return 0;

        _db.Products.Remove(product);
        var deleted = await _db.SaveChangesAsync();

        // Invalidate caches
        _cache.Remove($"product:{id}");
        _cache.Remove($"products:category:{product.CategoryId}");

        return deleted;
    }
}
```

## 🎯 Best Practices

### DO ✅

```csharp
// Use appropriate TTL
entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);

// Use AsNoTracking for cached data
var product = await _db.Products.AsNoTracking().FindAsync(id);

// Invalidate on writes
_cache.Remove($"product:{id}");

// Log cache hits/misses
_logger.LogInformation("[Cache {Status}]", isCached ? "HIT" : "MISS");

// Handle null values
if (product == null)
{
    // Cache null result with shorter TTL
    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(1);
    return null;
}
```

### DON'T ❌

```csharp
// Don't cache forever
entry.AbsoluteExpirationRelativeToNow = TimeSpan.MaxValue; // Bad!

// Don't cache large objects
var allProducts = await _db.Products.ToListAsync(); // 10GB in memory!

// Don't forget to invalidate
await _db.Products.UpdateAsync(product); // Cache now stale!

// Don't cache user-specific data in shared cache
_cache.Set("user-cart", cart); // Security issue!
```

## 📝 Summary

**Cache-Aside** en basit ve en yaygın stratejidir. Çoğu uygulama için yeterlidir.

**Ne Zaman Kullanmalı**:
- ✅ Read-heavy workloads (>80% reads)
- ✅ Stale data acceptable
- ✅ Simple caching needs
- ✅ Single-server applications

**Ne Zaman Kullanmamalı**:
- ❌ Strong consistency required
- ❌ Multi-server (needs distributed cache)
- ❌ Very high traffic (cache stampede risk)

**Sonraki Adım**: `SOLUTION-ADVANCED.md` - Write-Through + Distributed Caching
