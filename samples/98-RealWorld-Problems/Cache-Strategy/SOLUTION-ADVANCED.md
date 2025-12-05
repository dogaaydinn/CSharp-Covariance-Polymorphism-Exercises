# Advanced Solution: Write-Through + Distributed Cache (Redis)

## 🎯 Yaklaşım

**Write-Through** + **Redis Distributed Cache** ile multi-server desteği ve strong consistency.

## 🔧 Write-Through Pattern

### Nasıl Çalışır?

```csharp
// Write Flow:
public async Task UpdateProductAsync(Product product)
{
    // 1. Write to cache first
    await _cache.SetAsync($"product:{product.Id}", product);

    // 2. Write to database (synchronous)
    _db.Products.Update(product);
    await _db.SaveChangesAsync();

    // Data is always in sync!
}

// Read Flow:
public async Task<Product> GetProductAsync(int id)
{
    // 1. Always read from cache
    var product = await _cache.GetAsync<Product>($"product:{id}");

    // 2. If miss, load from DB and cache it
    if (product == null)
    {
        product = await _db.Products.FindAsync(id);
        await _cache.SetAsync($"product:{id}", product);
    }

    return product;
}
```

### Flow Diagram

```
Write Request → Write to Cache → Write to DB → Return Success
                      ↓
                Always in Sync!

Read Request → Check Cache → Cache Hit? → Return Data
                                  ↓ No
                            Load from DB → Write to Cache → Return Data
```

## 🌐 Redis Distributed Cache

### Why Redis?

1. **Multi-Server Support**: Shared cache across all servers
2. **High Performance**: In-memory storage
3. **Persistence**: Optional disk snapshots
4. **Advanced Features**: Pub/Sub, transactions, Lua scripts

### Setup

```csharp
// Program.cs
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = "localhost:6379";
    options.InstanceName = "MyApp_";
});
```

### Basic Usage

```csharp
public class ProductService
{
    private readonly IDistributedCache _cache;
    private readonly AppDbContext _db;

    public async Task<Product?> GetProductAsync(int id)
    {
        var cacheKey = $"product:{id}";

        // Try get from Redis
        var cachedJson = await _cache.GetStringAsync(cacheKey);
        if (cachedJson != null)
        {
            Console.WriteLine("[Redis Cache HIT]");
            return JsonSerializer.Deserialize<Product>(cachedJson);
        }

        Console.WriteLine("[Redis Cache MISS]");

        // Load from database
        var product = await _db.Products.FindAsync(id);
        if (product == null)
            return null;

        // Store in Redis with 5 minute expiration
        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
        };

        await _cache.SetStringAsync(
            cacheKey,
            JsonSerializer.Serialize(product),
            options);

        return product;
    }
}
```

## 🔄 Write-Through Implementation

```csharp
public class WriteThrough ProductService
{
    private readonly IDistributedCache _cache;
    private readonly AppDbContext _db;

    /// <summary>
    /// Create product (Write-Through)
    /// </summary>
    public async Task<Product> CreateProductAsync(Product product)
    {
        // 1. Add to database
        _db.Products.Add(product);
        await _db.SaveChangesAsync();

        // 2. Write to cache immediately
        await WriteToCacheAsync(product);

        Console.WriteLine("[Write-Through] Product created and cached");
        return product;
    }

    /// <summary>
    /// Update product (Write-Through)
    /// </summary>
    public async Task UpdateProductAsync(Product product)
    {
        // 1. Update database
        _db.Products.Update(product);
        await _db.SaveChangesAsync();

        // 2. Update cache
        await WriteToCacheAsync(product);

        Console.WriteLine("[Write-Through] Product updated and cache synced");
    }

    /// <summary>
    /// Delete product (Write-Through)
    /// </summary>
    public async Task DeleteProductAsync(int id)
    {
        // 1. Delete from database
        var product = await _db.Products.FindAsync(id);
        if (product != null)
        {
            _db.Products.Remove(product);
            await _db.SaveChangesAsync();
        }

        // 2. Remove from cache
        await _cache.RemoveAsync($"product:{id}");

        Console.WriteLine("[Write-Through] Product deleted and cache invalidated");
    }

    /// <summary>
    /// Read product (always from cache)
    /// </summary>
    public async Task<Product?> GetProductAsync(int id)
    {
        var cacheKey = $"product:{id}";

        // Check cache
        var cachedJson = await _cache.GetStringAsync(cacheKey);
        if (cachedJson != null)
        {
            return JsonSerializer.Deserialize<Product>(cachedJson);
        }

        // Cache miss - load from DB
        var product = await _db.Products.FindAsync(id);
        if (product != null)
        {
            await WriteToCacheAsync(product);
        }

        return product;
    }

    private async Task WriteToCacheAsync(Product product)
    {
        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30)
        };

        await _cache.SetStringAsync(
            $"product:{product.Id}",
            JsonSerializer.Serialize(product),
            options);
    }
}
```

## 📊 Multi-Level Cache (L1 + L2)

Combine **IMemoryCache** (L1) + **Redis** (L2) for maximum performance!

```csharp
public class MultiLevelCacheService
{
    private readonly IMemoryCache _l1Cache;        // Fast, local
    private readonly IDistributedCache _l2Cache;   // Shared, durable
    private readonly AppDbContext _db;

    public async Task<Product?> GetProductAsync(int id)
    {
        var cacheKey = $"product:{id}";

        // Level 1: Check memory cache (fastest - 0.1ms)
        if (_l1Cache.TryGetValue(cacheKey, out Product? l1Product))
        {
            Console.WriteLine("[L1 Cache HIT] Memory");
            return l1Product;
        }

        // Level 2: Check Redis (fast - 2ms)
        var cachedJson = await _l2Cache.GetStringAsync(cacheKey);
        if (cachedJson != null)
        {
            Console.WriteLine("[L2 Cache HIT] Redis");
            var l2Product = JsonSerializer.Deserialize<Product>(cachedJson);

            // Populate L1 cache
            _l1Cache.Set(cacheKey, l2Product, TimeSpan.FromMinutes(5));

            return l2Product;
        }

        // Level 3: Database (slow - 50-200ms)
        Console.WriteLine("[Cache MISS] Database");
        var product = await _db.Products
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id);

        if (product != null)
        {
            // Populate both caches
            _l1Cache.Set(cacheKey, product, TimeSpan.FromMinutes(5));

            await _l2Cache.SetStringAsync(
                cacheKey,
                JsonSerializer.Serialize(product),
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30)
                });
        }

        return product;
    }

    public async Task UpdateProductAsync(Product product)
    {
        // Update database
        _db.Products.Update(product);
        await _db.SaveChangesAsync();

        var cacheKey = $"product:{product.Id}";

        // Invalidate L1 cache
        _l1Cache.Remove(cacheKey);

        // Update L2 cache
        await _l2Cache.SetStringAsync(
            cacheKey,
            JsonSerializer.Serialize(product),
            new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30)
            });

        Console.WriteLine("[Multi-Level Cache] Updated both L1 and L2");
    }
}
```

## 🎯 Cache Stampede Prevention

### Problem

```
Popular product cache expires at 10:00:00
10,000 concurrent requests at 10:00:00.001
All 10,000 requests miss cache → 10,000 DB queries!
Database overloaded!
```

### Solution 1: Distributed Lock with Redis

```csharp
public async Task<Product?> GetProductWithLockAsync(int id)
{
    var cacheKey = $"product:{id}";
    var lockKey = $"lock:{cacheKey}";

    // Try get from cache
    var cachedJson = await _cache.GetStringAsync(cacheKey);
    if (cachedJson != null)
    {
        return JsonSerializer.Deserialize<Product>(cachedJson);
    }

    // Try acquire lock (only first request succeeds)
    var lockAcquired = await _cache.SetAsync(
        lockKey,
        Encoding.UTF8.GetBytes("locked"),
        new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(10)
        });

    if (!lockAcquired)
    {
        // Lock held by another request, wait and retry
        await Task.Delay(50);
        return await GetProductWithLockAsync(id); // Retry
    }

    try
    {
        // Double-check cache (another request might have loaded it)
        cachedJson = await _cache.GetStringAsync(cacheKey);
        if (cachedJson != null)
        {
            return JsonSerializer.Deserialize<Product>(cachedJson);
        }

        // Load from database (only first request does this)
        Console.WriteLine("[Cache Stampede PREVENTED] Loading from DB");
        var product = await _db.Products.FindAsync(id);

        if (product != null)
        {
            await _cache.SetStringAsync(
                cacheKey,
                JsonSerializer.Serialize(product),
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
                });
        }

        return product;
    }
    finally
    {
        // Release lock
        await _cache.RemoveAsync(lockKey);
    }
}
```

### Solution 2: Probabilistic Early Expiration

```csharp
public async Task<Product?> GetProductWithProbabilisticRefreshAsync(int id)
{
    var cacheKey = $"product:{id}";
    var entry = await GetCacheEntryWithMetadataAsync(cacheKey);

    if (entry != null)
    {
        var timeToExpiration = entry.ExpiresAt - DateTime.UtcNow;
        var totalTTL = TimeSpan.FromMinutes(5);

        // Probabilistic early refresh
        var beta = 1.0;
        var randomValue = Random.Shared.NextDouble();
        var threshold = beta * Math.Log(totalTTL.TotalSeconds / timeToExpiration.TotalSeconds);

        if (randomValue < threshold)
        {
            // Refresh cache in background (don't block)
            _ = Task.Run(async () =>
            {
                Console.WriteLine("[Probabilistic Refresh] Refreshing cache");
                var product = await _db.Products.FindAsync(id);
                if (product != null)
                {
                    await WriteToCacheAsync(product);
                }
            });
        }

        return entry.Value;
    }

    // Cache miss - load normally
    var dbProduct = await _db.Products.FindAsync(id);
    if (dbProduct != null)
    {
        await WriteToCacheAsync(dbProduct);
    }

    return dbProduct;
}
```

## 📊 Performance Metrics

### Cache Hit Rates

| Cache Level | Hit Rate | Latency | Use Case |
|-------------|----------|---------|----------|
| L1 (Memory) | 80-90% | 0.1ms | Hot data (last 5 min) |
| L2 (Redis) | 90-95% | 2ms | Warm data (last 30 min) |
| Database | 5-10% | 50-200ms | Cold data / Cache miss |

### Multi-Server Comparison

| Setup | Response Time | Database Load | Notes |
|-------|---------------|---------------|-------|
| No Cache | 200ms | 100% | Baseline |
| IMemoryCache only | 2ms | 10% | Not shared across servers! |
| Redis only | 5ms | 5% | Shared, but slightly slower |
| L1 + L2 (both) | 0.5ms (L1) / 5ms (L2) | 2% | Best of both worlds! |

## 📝 Summary

**Write-Through + Redis** enterprise-grade performans ve consistency sağlar.

**Avantajlar**:
- ✅ Multi-server support (distributed cache)
- ✅ Strong consistency (write-through)
- ✅ High performance (L1 + L2)
- ✅ Stampede prevention (distributed lock)

**Dezavantajlar**:
- ❌ More complex setup (Redis)
- ❌ Higher latency for writes
- ❌ Infrastructure cost

**Ne Zaman Kullanmalı**:
- ✅ Multi-server applications
- ✅ Strong consistency needed
- ✅ High traffic (>100 req/s)
- ✅ Budget for Redis

**Sonraki Adım**: `SOLUTION-ENTERPRISE.md` - Write-Behind + Cache Warming + Advanced Patterns
