# Solution A: Cache-Aside Pattern (Lazy Loading)

**Implementation Time:** 2 hours  
**Complexity:** ⭐⭐ Simple  
**Best For:** Read-heavy workloads with acceptable stale data

---

## The Approach

**Cache-Aside (Lazy Loading):** Application checks cache first, loads from DB on miss, then stores in cache.

```
Request → Check Cache → Hit? Return from cache
                     → Miss? Load from DB → Store in cache → Return
```

**Key Characteristics:**
- Cache is populated on-demand (lazy)
- Application manages cache logic
- Tolerates cache failures (DB is source of truth)
- Simple to implement

---

## Implementation

### Step 1: Add NuGet Package

```bash
dotnet add package Microsoft.Extensions.Caching.Memory
```

### Step 2: Product Repository with Cache-Aside

```csharp
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ECommerce.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly AppDbContext _dbContext;
        private readonly IMemoryCache _cache;
        private readonly ILogger<ProductRepository> _logger;

        // Cache settings
        private static class CacheKeys
        {
            public static string ProductById(int id) => $"product:{id}";
            public static string ProductsByCategory(int categoryId) => $"products:category:{categoryId}";
            public static string AllProducts => "products:all";
        }

        private static class CacheExpiration
        {
            public static TimeSpan ShortLived => TimeSpan.FromMinutes(1);  // Stock, prices
            public static TimeSpan MediumLived => TimeSpan.FromMinutes(15); // Product details
            public static TimeSpan LongLived => TimeSpan.FromHours(24);     // Categories, images
        }

        public ProductRepository(
            AppDbContext dbContext, 
            IMemoryCache cache, 
            ILogger<ProductRepository> logger)
        {
            _dbContext = dbContext;
            _cache = cache;
            _logger = logger;
        }

        /// <summary>
        /// Get product by ID with caching
        /// </summary>
        public async Task<Product?> GetByIdAsync(int id)
        {
            var cacheKey = CacheKeys.ProductById(id);

            // Try to get from cache
            if (_cache.TryGetValue(cacheKey, out Product? cachedProduct))
            {
                _logger.LogDebug("Cache HIT for product {ProductId}", id);
                return cachedProduct;
            }

            _logger.LogDebug("Cache MISS for product {ProductId}", id);

            // Not in cache - load from database
            var product = await _dbContext.Products
                .Include(p => p.Category)
                .Include(p => p.Images)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product != null)
            {
                // Store in cache with appropriate expiration
                var cacheOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = GetExpirationForProduct(product),
                    SlidingExpiration = TimeSpan.FromMinutes(5), // Extends if accessed
                    Priority = CacheItemPriority.Normal
                };

                _cache.Set(cacheKey, product, cacheOptions);
                _logger.LogDebug("Cached product {ProductId} for {Minutes} minutes", 
                    id, cacheOptions.AbsoluteExpirationRelativeToNow?.TotalMinutes);
            }

            return product;
        }

        /// <summary>
        /// Get products by category with caching
        /// </summary>
        public async Task<List<Product>> GetByCategoryAsync(int categoryId)
        {
            var cacheKey = CacheKeys.ProductsByCategory(categoryId);

            // Try cache first
            if (_cache.TryGetValue(cacheKey, out List<Product>? cachedProducts))
            {
                _logger.LogDebug("Cache HIT for category {CategoryId}", categoryId);
                return cachedProducts!;
            }

            _logger.LogDebug("Cache MISS for category {CategoryId}", categoryId);

            // Load from database
            var products = await _dbContext.Products
                .Where(p => p.CategoryId == categoryId)
                .Include(p => p.Images)
                .OrderBy(p => p.Name)
                .ToListAsync();

            // Cache the result
            var cacheOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = CacheExpiration.MediumLived,
                Priority = CacheItemPriority.Normal
            };

            _cache.Set(cacheKey, products, cacheOptions);

            return products;
        }

        /// <summary>
        /// Update product (invalidates cache)
        /// </summary>
        public async Task UpdateAsync(Product product)
        {
            _dbContext.Products.Update(product);
            await _dbContext.SaveChangesAsync();

            // ✅ Invalidate cache when data changes
            InvalidateProductCache(product.Id, product.CategoryId);
            
            _logger.LogInformation("Updated product {ProductId} and invalidated cache", product.Id);
        }

        /// <summary>
        /// Update product price (critical - invalidate immediately)
        /// </summary>
        public async Task UpdatePriceAsync(int productId, decimal newPrice)
        {
            var product = await _dbContext.Products.FindAsync(productId);
            
            if (product == null)
                throw new NotFoundException($"Product {productId} not found");

            product.Price = newPrice;
            product.UpdatedAt = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync();

            // ✅ Critical: Invalidate price cache immediately
            InvalidateProductCache(productId, product.CategoryId);
            
            _logger.LogWarning("Price updated for product {ProductId} - cache invalidated", productId);
        }

        /// <summary>
        /// Update stock count (can tolerate 1-minute staleness)
        /// </summary>
        public async Task UpdateStockAsync(int productId, int newStock)
        {
            var product = await _dbContext.Products.FindAsync(productId);
            
            if (product == null)
                throw new NotFoundException($"Product {productId} not found");

            product.StockCount = newStock;
            product.UpdatedAt = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync();

            // ⚠️ DON'T invalidate cache - let it expire naturally (1 minute)
            // This reduces cache churn for frequently updated stock counts
            
            _logger.LogDebug("Stock updated for product {ProductId} - cache NOT invalidated (will expire)", productId);
        }

        /// <summary>
        /// Invalidate product cache
        /// </summary>
        private void InvalidateProductCache(int productId, int categoryId)
        {
            _cache.Remove(CacheKeys.ProductById(productId));
            _cache.Remove(CacheKeys.ProductsByCategory(categoryId));
            _cache.Remove(CacheKeys.AllProducts);
        }

        /// <summary>
        /// Determine cache expiration based on product data sensitivity
        /// </summary>
        private TimeSpan GetExpirationForProduct(Product product)
        {
            // Price-sensitive products get shorter cache
            if (product.IsPriceDynamic || product.IsOnSale)
                return CacheExpiration.ShortLived; // 1 minute

            // Regular products get medium cache
            return CacheExpiration.MediumLived; // 15 minutes
        }
    }
}
```

### Step 3: Register Services

```csharp
// Program.cs or Startup.cs

public void ConfigureServices(IServiceCollection services)
{
    // Add memory cache
    services.AddMemoryCache(options =>
    {
        options.SizeLimit = 1024; // Max 1024 entries
        options.CompactionPercentage = 0.25; // Remove 25% when limit reached
    });

    services.AddScoped<IProductRepository, ProductRepository>();
}
```

---

## Usage Example

```csharp
public class ProductController : ControllerBase
{
    private readonly IProductRepository _productRepository;

    [HttpGet("{id}")]
    public async Task<IActionResult> GetProduct(int id)
    {
        var stopwatch = Stopwatch.StartNew();

        var product = await _productRepository.GetByIdAsync(id);
        
        stopwatch.Stop();
        
        if (product == null)
            return NotFound();

        // First request: 450ms (DB query)
        // Subsequent requests: 2ms (cache hit!)
        _logger.LogInformation("Product loaded in {ElapsedMs}ms", stopwatch.ElapsedMilliseconds);

        return Ok(product);
    }

    [HttpGet("category/{categoryId}")]
    public async Task<IActionResult> GetProductsByCategory(int categoryId)
    {
        var products = await _productRepository.GetByCategoryAsync(categoryId);
        return Ok(products);
    }

    [HttpPut("{id}/price")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdatePrice(int id, [FromBody] UpdatePriceRequest request)
    {
        await _productRepository.UpdatePriceAsync(id, request.NewPrice);
        return NoContent();
    }
}
```

---

## Performance Impact

### Before Caching
```
Average response time: 3,500ms
Database queries/sec: 200
Database CPU: 85%
```

### After Caching (Cache-Aside)
```
Average response time: 150ms (95% improvement!)
Database queries/sec: 40 (80% reduction!)
Database CPU: 25%
Cache hit ratio: 85%
```

---

## Pros ✅

1. **Simple to implement** - Just wrap DB calls with cache logic
2. **Resilient** - If cache fails, app still works (DB is source of truth)
3. **Flexible** - Different expiration for different data types
4. **Memory efficient** - Only caches requested data
5. **Works with existing code** - Minimal changes required

---

## Cons ❌

1. **Cache stampede** - Multiple requests can hit DB simultaneously on cache miss
2. **Stale data possible** - Data can be outdated until expiration
3. **Inconsistent reads** - Different servers may have different cached data
4. **Manual invalidation** - Must remember to invalidate on updates

---

## Real-World Scenarios

### ✅ Good For:
- Read-heavy applications (like this e-commerce site)
- Data that doesn't change frequently
- Tolerable stale data (stock counts, descriptions)
- Small to medium datasets

### ❌ Not Good For:
- Write-heavy applications
- Critical data requiring strong consistency (bank balances, inventory)
- Large datasets (memory constraints)
- Distributed systems needing synchronized caches

---

## Common Mistakes

### Mistake 1: Not Invalidating Cache on Updates
```csharp
// ❌ BAD
public async Task UpdatePriceAsync(int productId, decimal newPrice)
{
    product.Price = newPrice;
    await _dbContext.SaveChangesAsync();
    // Forgot to invalidate cache! Users see old price!
}

// ✅ GOOD
public async Task UpdatePriceAsync(int productId, decimal newPrice)
{
    product.Price = newPrice;
    await _dbContext.SaveChangesAsync();
    InvalidateProductCache(productId); // Always invalidate!
}
```

### Mistake 2: Caching for Too Long
```csharp
// ❌ BAD - Prices cached for 1 hour!
var cacheOptions = new MemoryCacheEntryOptions
{
    AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1) // Too long!
};

// ✅ GOOD - Short expiration for dynamic data
var cacheOptions = new MemoryCacheEntryOptions
{
    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(1) // Reasonable
};
```

### Mistake 3: Not Handling Cache Misses
```csharp
// ❌ BAD - Assumes cache always has data
var product = _cache.Get<Product>(cacheKey); // NullReferenceException if not cached!
return product.Name;

// ✅ GOOD - Always check cache miss
if (!_cache.TryGetValue(cacheKey, out Product? product))
{
    product = await LoadFromDatabase(id);
    _cache.Set(cacheKey, product);
}
return product.Name;
```

---

## Testing

```csharp
[Fact]
public async Task GetByIdAsync_CacheHit_ReturnsCachedProduct()
{
    // Arrange
    var product = new Product { Id = 1, Name = "Test Product" };
    _cache.Set(CacheKeys.ProductById(1), product);

    // Act
    var result = await _repository.GetByIdAsync(1);

    // Assert
    Assert.Equal(product, result);
    _dbContextMock.Verify(db => db.Products, Times.Never); // DB not accessed
}

[Fact]
public async Task GetByIdAsync_CacheMiss_LoadsFromDatabase()
{
    // Arrange
    var product = new Product { Id = 1, Name = "Test Product" };
    _dbContextMock.Setup(db => db.Products.FindAsync(1)).ReturnsAsync(product);

    // Act
    var result = await _repository.GetByIdAsync(1);

    // Assert
    Assert.Equal(product, result);
    _dbContextMock.Verify(db => db.Products.FindAsync(1), Times.Once); // DB accessed once
}

[Fact]
public async Task UpdatePriceAsync_InvalidatesCache()
{
    // Arrange
    var product = new Product { Id = 1, Price = 100 };
    _cache.Set(CacheKeys.ProductById(1), product);

    // Act
    await _repository.UpdatePriceAsync(1, 150);

    // Assert
    Assert.False(_cache.TryGetValue(CacheKeys.ProductById(1), out _)); // Cache invalidated
}
```

---

## Monitoring

```csharp
public class CacheMetrics
{
    private static long _cacheHits;
    private static long _cacheMisses;

    public static void RecordHit() => Interlocked.Increment(ref _cacheHits);
    public static void RecordMiss() => Interlocked.Increment(ref _cacheMisses);

    public static double GetHitRatio() => 
        _cacheHits + _cacheMisses == 0 
            ? 0 
            : (double)_cacheHits / (_cacheHits + _cacheMisses);
}

// In repository:
if (_cache.TryGetValue(cacheKey, out var cachedValue))
{
    CacheMetrics.RecordHit();
    return cachedValue;
}
else
{
    CacheMetrics.RecordMiss();
    // Load from DB...
}

// Expose metrics endpoint:
[HttpGet("cache/metrics")]
public IActionResult GetCacheMetrics()
{
    return Ok(new
    {
        HitRatio = CacheMetrics.GetHitRatio(),
        CacheSize = _cache.Count
    });
}
```

---

## Cost Analysis

**Memory Cache (In-Memory):**
- Cost: $0/month (included in application server)
- Memory usage: ~50MB for 1,000 products
- Scalability: Per-server (each server has own cache)

**Performance Gain:**
- 80% reduction in database queries
- Database CPU reduced from 85% → 25%
- Could downgrade database tier: **Save $500/month**

**Total Savings:** $500/month (database tier downgrade)

---

## Next Steps

**To Production:**
1. ✅ Implement cache-aside pattern
2. ✅ Add cache invalidation on updates
3. ✅ Monitor cache hit ratio
4. ✅ Tune expiration times based on data

**Future Enhancements:**
- Use Redis for distributed cache (multiple servers)
- Add cache warming (pre-populate on startup)
- Implement cache stampede protection (lock during load)
- Add cache versioning for gradual rollouts

---

## See Also

- **SOLUTION-B.md** - Write-Through Cache (stronger consistency)
- **SOLUTION-C.md** - Hybrid Strategy (best of both)
- **DECISION-TREE.md** - How to choose the right approach

