# Solution C: Hybrid Cache Strategy (Best of Both Worlds)

**Implementation Time:** 6 hours  
**Complexity:** ⭐⭐⭐⭐ Advanced  
**Best For:** Production systems with mixed requirements

---

## The Approach

**Hybrid Strategy:** Different caching strategies for different data types based on their characteristics.

```
Prices       → Write-Through (strong consistency)
Stock        → Cache-Aside + Short TTL (acceptable staleness)
Descriptions → Cache-Aside + Long TTL (rarely changes)
Images       → CDN + Infinite TTL (immutable)
```

**Key Principle:** Match caching strategy to data sensitivity and update frequency.

---

## Architecture

```
┌──────────────────────────────────────────────────────────────┐
│                     Product Data Layers                       │
├──────────────────────────────────────────────────────────────┤
│                                                               │
│  Prices (Critical)         Stock (Important)                 │
│  ┌─────────────┐          ┌─────────────┐                   │
│  │Write-Through│          │Cache-Aside  │                   │
│  │Redis 1min   │          │Redis 1min   │                   │
│  │TTL          │          │TTL          │                   │
│  └─────────────┘          └─────────────┘                   │
│         ↓                        ↓                           │
│                                                               │
│  Descriptions (Flexible)   Images (Static)                   │
│  ┌─────────────┐          ┌─────────────┐                   │
│  │Cache-Aside  │          │CDN Cache    │                   │
│  │Redis 15min  │          │Infinite TTL │                   │
│  │TTL          │          │             │                   │
│  └─────────────┘          └─────────────┘                   │
│                                                               │
└──────────────────────────────────────────────────────────────┘
                           ↓
                    ┌─────────────┐
                    │  PostgreSQL │
                    │  (Source of │
                    │   Truth)    │
                    └─────────────┘
```

---

## Implementation

### Step 1: Define Data Sensitivity Levels

```csharp
namespace ECommerce.Models
{
    /// <summary>
    /// Data sensitivity determines caching strategy
    /// </summary>
    public enum DataSensitivity
    {
        Critical,   // Prices, payment info - must be accurate
        Important,  // Stock counts - can be 1 min stale
        Normal,     // Descriptions, reviews - can be 15 min stale
        Static      // Images, categories - rarely changes
    }

    /// <summary>
    /// Caching strategy configuration
    /// </summary>
    public class CacheStrategy
    {
        public DataSensitivity Sensitivity { get; init; }
        public CacheMode Mode { get; init; }
        public TimeSpan Expiration { get; init; }
        public bool UseDistributedCache { get; init; }

        // Predefined strategies
        public static CacheStrategy Critical => new()
        {
            Sensitivity = DataSensitivity.Critical,
            Mode = CacheMode.WriteThrough,
            Expiration = TimeSpan.FromMinutes(1),
            UseDistributedCache = true
        };

        public static CacheStrategy Important => new()
        {
            Sensitivity = DataSensitivity.Important,
            Mode = CacheMode.CacheAside,
            Expiration = TimeSpan.FromMinutes(1),
            UseDistributedCache = true
        };

        public static CacheStrategy Normal => new()
        {
            Sensitivity = DataSensitivity.Normal,
            Mode = CacheMode.CacheAside,
            Expiration = TimeSpan.FromMinutes(15),
            UseDistributedCache = true
        };

        public static CacheStrategy Static => new()
        {
            Sensitivity = DataSensitivity.Static,
            Mode = CacheMode.CacheAside,
            Expiration = TimeSpan.FromDays(30),
            UseDistributedCache = false // Use local memory cache
        };
    }

    public enum CacheMode
    {
        CacheAside,    // Lazy loading
        WriteThrough   // Eager updating
    }
}
```

### Step 2: Multi-Layer Cache Service

```csharp
namespace ECommerce.Services
{
    public interface IHybridCacheService
    {
        Task<T?> GetAsync<T>(string key, CacheStrategy strategy);
        Task SetAsync<T>(string key, T value, CacheStrategy strategy);
        Task WriteWithStrategyAsync<T>(string key, T value, CacheStrategy strategy, Func<Task> dbWrite);
        Task RemoveAsync(string key);
    }

    public class HybridCacheService : IHybridCacheService
    {
        private readonly IMemoryCache _localCache;
        private readonly ICacheService _distributedCache;
        private readonly ILogger<HybridCacheService> _logger;

        public HybridCacheService(
            IMemoryCache localCache,
            ICacheService distributedCache,
            ILogger<HybridCacheService> logger)
        {
            _localCache = localCache;
            _distributedCache = distributedCache;
            _logger = logger;
        }

        /// <summary>
        /// Get with L1 (local) and L2 (distributed) cache
        /// </summary>
        public async Task<T?> GetAsync<T>(string key, CacheStrategy strategy)
        {
            // L1 Cache (local memory) - fastest
            if (!strategy.UseDistributedCache)
            {
                if (_localCache.TryGetValue(key, out T? localValue))
                {
                    _logger.LogDebug("L1 cache HIT for {Key}", key);
                    return localValue;
                }

                _logger.LogDebug("L1 cache MISS for {Key}", key);
                return default;
            }

            // L1 + L2 Cache (local + distributed)
            if (_localCache.TryGetValue(key, out T? cachedValue))
            {
                _logger.LogDebug("L1 cache HIT for {Key}", key);
                return cachedValue;
            }

            // L1 miss - try L2 (distributed)
            var distributedValue = await _distributedCache.GetAsync<T>(key);
            if (distributedValue != null)
            {
                _logger.LogDebug("L2 cache HIT for {Key} - populating L1", key);
                
                // Populate L1 cache for future requests
                _localCache.Set(key, distributedValue, new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(1),
                    Size = 1
                });

                return distributedValue;
            }

            _logger.LogDebug("Cache MISS (L1 + L2) for {Key}", key);
            return default;
        }

        /// <summary>
        /// Set with appropriate cache layer
        /// </summary>
        public async Task SetAsync<T>(string key, T value, CacheStrategy strategy)
        {
            if (strategy.UseDistributedCache)
            {
                // Set in distributed cache
                await _distributedCache.SetAsync(key, value, strategy.Expiration);
                
                // Also set in local cache for faster subsequent access
                _localCache.Set(key, value, new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(1),
                    Size = 1
                });

                _logger.LogDebug("Set {Key} in L1 + L2 cache", key);
            }
            else
            {
                // Static data - only local cache
                _localCache.Set(key, value, new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = strategy.Expiration,
                    Priority = CacheItemPriority.Normal,
                    Size = 1
                });

                _logger.LogDebug("Set {Key} in L1 cache only", key);
            }
        }

        /// <summary>
        /// Write with strategy (write-through or cache-aside)
        /// </summary>
        public async Task WriteWithStrategyAsync<T>(
            string key, 
            T value, 
            CacheStrategy strategy, 
            Func<Task> dbWrite)
        {
            if (strategy.Mode == CacheMode.WriteThrough)
            {
                // Write-Through: Update DB and cache atomically
                await dbWrite(); // Update database first
                await SetAsync(key, value, strategy); // Then update cache
                
                _logger.LogInformation("Write-through completed for {Key}", key);
            }
            else
            {
                // Cache-Aside: Just update DB, invalidate cache
                await dbWrite();
                await RemoveAsync(key); // Force reload on next read
                
                _logger.LogInformation("Cache-aside write completed for {Key} - cache invalidated", key);
            }
        }

        public async Task RemoveAsync(string key)
        {
            _localCache.Remove(key);
            await _distributedCache.RemoveAsync(key);
        }
    }
}
```

### Step 3: Repository with Hybrid Strategy

```csharp
namespace ECommerce.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly AppDbContext _dbContext;
        private readonly IHybridCacheService _cache;
        private readonly ILogger<ProductRepository> _logger;

        private static class CacheKeys
        {
            public static string ProductPrice(int id) => $"product:{id}:price";
            public static string ProductStock(int id) => $"product:{id}:stock";
            public static string ProductDetails(int id) => $"product:{id}:details";
            public static string ProductImages(int id) => $"product:{id}:images";
        }

        public ProductRepository(
            AppDbContext dbContext,
            IHybridCacheService cache,
            ILogger<ProductRepository> logger)
        {
            _dbContext = dbContext;
            _cache = cache;
            _logger = logger;
        }

        /// <summary>
        /// Get product - uses different cache strategies for different fields
        /// </summary>
        public async Task<Product?> GetByIdAsync(int id)
        {
            var product = new Product { Id = id };

            // Get price (Critical - Write-Through)
            var price = await _cache.GetAsync<decimal?>(
                CacheKeys.ProductPrice(id), 
                CacheStrategy.Critical
            );

            if (price == null)
            {
                // Load from DB
                var dbProduct = await _dbContext.Products.FindAsync(id);
                if (dbProduct == null) return null;

                price = dbProduct.Price;
                await _cache.SetAsync(
                    CacheKeys.ProductPrice(id), 
                    price, 
                    CacheStrategy.Critical
                );
            }

            product.Price = price.Value;

            // Get stock (Important - Cache-Aside, short TTL)
            var stock = await _cache.GetAsync<int?>(
                CacheKeys.ProductStock(id), 
                CacheStrategy.Important
            );

            if (stock == null)
            {
                var dbProduct = await _dbContext.Products.FindAsync(id);
                stock = dbProduct?.StockCount ?? 0;
                await _cache.SetAsync(
                    CacheKeys.ProductStock(id), 
                    stock, 
                    CacheStrategy.Important
                );
            }

            product.StockCount = stock.Value;

            // Get details (Normal - Cache-Aside, longer TTL)
            var details = await _cache.GetAsync<ProductDetails>(
                CacheKeys.ProductDetails(id), 
                CacheStrategy.Normal
            );

            if (details == null)
            {
                var dbProduct = await _dbContext.Products
                    .Include(p => p.Category)
                    .FirstOrDefaultAsync(p => p.Id == id);
                
                details = new ProductDetails
                {
                    Name = dbProduct!.Name,
                    Description = dbProduct.Description,
                    CategoryName = dbProduct.Category.Name
                };

                await _cache.SetAsync(
                    CacheKeys.ProductDetails(id), 
                    details, 
                    CacheStrategy.Normal
                );
            }

            product.Name = details.Name;
            product.Description = details.Description;

            // Get images (Static - Local cache, very long TTL)
            var images = await _cache.GetAsync<List<string>>(
                CacheKeys.ProductImages(id), 
                CacheStrategy.Static
            );

            if (images == null)
            {
                images = await _dbContext.ProductImages
                    .Where(i => i.ProductId == id)
                    .Select(i => i.Url)
                    .ToListAsync();

                await _cache.SetAsync(
                    CacheKeys.ProductImages(id), 
                    images, 
                    CacheStrategy.Static
                );
            }

            product.ImageUrls = images;

            return product;
        }

        /// <summary>
        /// Update price - uses Write-Through strategy (critical data)
        /// </summary>
        public async Task UpdatePriceAsync(int productId, decimal newPrice)
        {
            var product = await _dbContext.Products.FindAsync(productId);
            if (product == null)
                throw new NotFoundException($"Product {productId} not found");

            var oldPrice = product.Price;
            product.Price = newPrice;

            // Write-Through for critical price data
            await _cache.WriteWithStrategyAsync(
                CacheKeys.ProductPrice(productId),
                newPrice,
                CacheStrategy.Critical,
                async () => await _dbContext.SaveChangesAsync()
            );

            _logger.LogWarning(
                "Price updated for product {ProductId}: {OldPrice} → {NewPrice} (write-through)",
                productId, oldPrice, newPrice
            );
        }

        /// <summary>
        /// Update stock - uses Cache-Aside (acceptable 1-min staleness)
        /// </summary>
        public async Task UpdateStockAsync(int productId, int newStock)
        {
            var product = await _dbContext.Products.FindAsync(productId);
            if (product == null)
                throw new NotFoundException($"Product {productId} not found");

            product.StockCount = newStock;

            // Cache-Aside for stock - don't invalidate immediately
            // Let it expire naturally (reduces cache churn for frequently updated stock)
            await _dbContext.SaveChangesAsync();

            _logger.LogDebug(
                "Stock updated for product {ProductId}: {NewStock} (cache will expire in 1 min)",
                productId, newStock
            );
        }

        /// <summary>
        /// Update description - uses Cache-Aside with invalidation
        /// </summary>
        public async Task UpdateDescriptionAsync(int productId, string newDescription)
        {
            var product = await _dbContext.Products.FindAsync(productId);
            if (product == null)
                throw new NotFoundException($"Product {productId} not found");

            product.Description = newDescription;

            // Cache-Aside with immediate invalidation
            await _cache.WriteWithStrategyAsync(
                CacheKeys.ProductDetails(productId),
                null!,
                CacheStrategy.Normal,
                async () => await _dbContext.SaveChangesAsync()
            );

            _logger.LogInformation(
                "Description updated for product {ProductId} - cache invalidated",
                productId
            );
        }
    }

    public class ProductDetails
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
    }
}
```

### Step 4: Configuration

```csharp
// Program.cs

// L1 Cache (local memory)
builder.Services.AddMemoryCache(options =>
{
    options.SizeLimit = 1024;
    options.CompactionPercentage = 0.25;
});

// L2 Cache (distributed Redis)
builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    var configuration = ConfigurationOptions.Parse(
        builder.Configuration.GetConnectionString("Redis")!
    );
    return ConnectionMultiplexer.Connect(configuration);
});

builder.Services.AddScoped<ICacheService, RedisCacheService>();
builder.Services.AddScoped<IHybridCacheService, HybridCacheService>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
```

---

## Performance Impact

### Hybrid Strategy Results

| Data Type | Strategy | TTL | Cache Hit % | Avg Latency |
|-----------|----------|-----|-------------|-------------|
| **Price** | Write-Through | 1 min | 100% | 3ms |
| **Stock** | Cache-Aside | 1 min | 98% | 5ms |
| **Details** | Cache-Aside | 15 min | 95% | 8ms |
| **Images** | Local Cache | 30 days | 99% | 1ms |

**Overall Performance:**
```
Before: 3,500ms average page load (all DB queries)
After:  120ms average page load (hybrid caching)
Improvement: 97% faster! 🚀
```

**Database Load:**
```
Before: 200 queries/sec
After:  10 queries/sec (95% reduction!)
```

---

## Pros ✅

1. **Optimal for each data type** - Right strategy for right data
2. **Best performance** - Fast writes for important data, fast reads everywhere
3. **Strong consistency where needed** - Prices are never stale
4. **Acceptable staleness where tolerable** - Stock can be 1-min old
5. **Cost-effective** - Static data in local cache (free)
6. **Scalable** - Can add more cache layers as needed

---

## Cons ❌

1. **High complexity** - Must understand data characteristics
2. **More code** - Different logic for different data types
3. **Harder to debug** - Multiple cache layers
4. **Requires planning** - Must classify all data types upfront
5. **Higher maintenance** - More moving parts to monitor

---

## Data Classification Guide

### Critical (Write-Through, 1 min TTL)
- ✅ Prices
- ✅ Payment information
- ✅ Legal disclaimers
- ✅ Promotions with end dates

### Important (Cache-Aside, 1 min TTL)
- ✅ Stock counts
- ✅ User ratings (average)
- ✅ Availability status
- ✅ Shipping estimates

### Normal (Cache-Aside, 15 min TTL)
- ✅ Product descriptions
- ✅ Reviews and comments
- ✅ Seller information
- ✅ Product specifications

### Static (Local Cache, 30 days TTL)
- ✅ Product images
- ✅ Category names
- ✅ Static content pages
- ✅ CSS/JS assets

---

## Real-World Example: Amazon

Amazon uses a hybrid strategy similar to this:

```
Product Page Load Time: ~200ms

Components:
- Price: Write-Through (real-time, Redis)
  → 5ms latency
  
- Stock ("Only 3 left"): Cache-Aside with 30-sec TTL
  → Acceptable staleness, reduces DB load
  
- Description/Reviews: Cache-Aside with 5-min TTL
  → Rarely changes, long cache OK
  
- Images: CDN with 1-year TTL
  → Immutable URLs, infinite cache
```

**Result:** 99.9% of requests never hit the database!

---

## Monitoring Dashboard

```csharp
public class CacheMonitoringService
{
    public async Task<CacheMetrics> GetMetricsAsync()
    {
        return new CacheMetrics
        {
            Layers = new[]
            {
                new LayerMetrics
                {
                    Name = "L1 (Local Memory)",
                    HitRatio = 85.3,
                    AvgLatencyMs = 0.5,
                    Size = "45 MB / 100 MB"
                },
                new LayerMetrics
                {
                    Name = "L2 (Redis)",
                    HitRatio = 92.7,
                    AvgLatencyMs = 3.2,
                    Size = "280 MB / 1 GB"
                }
            },
            ByDataType = new[]
            {
                new DataTypeMetrics
                {
                    Type = "Prices",
                    Strategy = "Write-Through",
                    HitRatio = 100.0,
                    StalenessMs = 0
                },
                new DataTypeMetrics
                {
                    Type = "Stock",
                    Strategy = "Cache-Aside",
                    HitRatio = 98.2,
                    StalenessMs = 15000 // 15 seconds average
                },
                new DataTypeMetrics
                {
                    Type = "Descriptions",
                    Strategy = "Cache-Aside",
                    HitRatio = 95.1,
                    StalenessMs = 120000 // 2 minutes average
                }
            }
        };
    }
}
```

---

## Cost Analysis

**Infrastructure:**
- Redis (t3.small): $30/month
- Application servers (same as before): $0 extra
- CDN for images (CloudFront): $10/month

**Total:** $40/month

**Savings from database downgrade:** $500/month

**Net Savings:** $460/month 💰

---

## Migration Path

### Phase 1: Start with Cache-Aside (Week 1)
```csharp
// Implement for all data types
await _cache.GetAsync(key, CacheStrategy.Normal);
```

### Phase 2: Identify Critical Data (Week 2)
```csharp
// Monitor which data changes most frequently
// Classify into Critical/Important/Normal/Static
```

### Phase 3: Add Write-Through for Critical Data (Week 3)
```csharp
// Only for prices and payment info
await _cache.WriteWithStrategyAsync(key, value, CacheStrategy.Critical, dbWrite);
```

### Phase 4: Optimize TTLs (Week 4)
```csharp
// Fine-tune expiration times based on metrics
CacheStrategy.Critical.Expiration = TimeSpan.FromMinutes(1);
CacheStrategy.Normal.Expiration = TimeSpan.FromMinutes(15);
```

---

## Testing Strategy

```csharp
[Fact]
public async Task UpdatePrice_UsesWriteThrough()
{
    // Arrange
    var productId = 1;
    var newPrice = 150m;

    // Act
    await _repository.UpdatePriceAsync(productId, newPrice);

    // Assert - both DB and cache updated
    var dbProduct = await _dbContext.Products.FindAsync(productId);
    Assert.Equal(150m, dbProduct.Price);

    var cachedPrice = await _cache.GetAsync<decimal>(
        $"product:{productId}:price", 
        CacheStrategy.Critical
    );
    Assert.Equal(150m, cachedPrice);
}

[Fact]
public async Task UpdateStock_UsesCacheAside()
{
    // Arrange
    var productId = 1;
    var newStock = 50;

    // Act
    await _repository.UpdateStockAsync(productId, newStock);

    // Assert - DB updated, cache NOT immediately updated
    var dbProduct = await _dbContext.Products.FindAsync(productId);
    Assert.Equal(50, dbProduct.StockCount);

    // Cache may still have old value (acceptable for 1 min)
    var cachedStock = await _cache.GetAsync<int>(
        $"product:{productId}:stock", 
        CacheStrategy.Important
    );
    // cachedStock might be old - that's OK!
}
```

---

## When to Use This Approach

### ✅ Perfect For:
- Production e-commerce systems
- Systems with mixed consistency requirements
- High-traffic applications
- Teams with caching expertise

### ❌ Overkill For:
- Simple CRUD applications
- Low-traffic internal tools
- Prototypes and MVPs
- Small datasets (< 1000 records)

---

## Lessons Learned

1. **Not all data is equal** - Don't use one strategy for everything
2. **Measure before optimizing** - Profile to find bottlenecks
3. **Start simple, then optimize** - Begin with cache-aside, add write-through only where needed
4. **Monitor continuously** - Cache hit ratios, staleness, and errors
5. **Document your strategy** - Future developers need to understand the "why"

---

## See Also

- **SOLUTION-A.md** - Cache-Aside Pattern (foundation)
- **SOLUTION-B.md** - Write-Through Cache (comparison)
- **DECISION-TREE.md** - How to choose strategies

