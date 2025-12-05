using Microsoft.Extensions.Caching.Memory;
using Microsoft.EntityFrameworkCore;

namespace RealWorldProblems.CacheStrategy.Basic;

/// <summary>
/// Basic Solution: Cache-Aside Pattern with IMemoryCache
/// Simple lazy loading pattern for single-server applications
/// </summary>
public class CacheAsideProductService
{
    private readonly IMemoryCache _cache;
    private readonly AppDbContext _db;
    private readonly ILogger<CacheAsideProductService> _logger;

    public CacheAsideProductService(
        IMemoryCache cache,
        AppDbContext db,
        ILogger<CacheAsideProductService> logger)
    {
        _cache = cache;
        _db = db;
        _logger = logger;
    }

    /// <summary>
    /// GET with Cache-Aside pattern
    /// </summary>
    public async Task<Product?> GetProductAsync(int id)
    {
        var cacheKey = $"product:{id}";

        // Try get from cache
        if (_cache.TryGetValue(cacheKey, out Product? cachedProduct))
        {
            _logger.LogInformation("[Cache HIT] Product {Id}", id);
            return cachedProduct;
        }

        _logger.LogInformation("[Cache MISS] Product {Id} - Loading from database", id);

        // Load from database
        var product = await _db.Products
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id);

        if (product != null)
        {
            // Store in cache with 5 minute expiration
            var cacheOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5),
                Priority = CacheItemPriority.Normal
            };

            _cache.Set(cacheKey, product, cacheOptions);
            _logger.LogInformation("[Cache SET] Product {Id} cached for 5 minutes", id);
        }

        return product;
    }

    /// <summary>
    /// GET using GetOrCreateAsync helper
    /// </summary>
    public async Task<Product?> GetProductSimpleAsync(int id)
    {
        return await _cache.GetOrCreateAsync($"product:{id}", async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);
            entry.Priority = CacheItemPriority.Normal;

            // Log on eviction
            entry.RegisterPostEvictionCallback((key, value, reason, state) =>
            {
                _logger.LogInformation(
                    "Cache evicted: {Key}, Reason: {Reason}",
                    key, reason);
            });

            _logger.LogInformation("[Cache MISS] Loading product {Id} from database", id);

            return await _db.Products
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == id);
        });
    }

    /// <summary>
    /// GET list with caching
    /// </summary>
    public async Task<List<Product>> GetProductsByCategoryAsync(int categoryId)
    {
        var cacheKey = $"products:category:{categoryId}";

        return await _cache.GetOrCreateAsync(cacheKey, async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10);

            _logger.LogInformation(
                "[Cache MISS] Loading products for category {CategoryId}",
                categoryId);

            return await _db.Products
                .Where(p => p.CategoryId == categoryId)
                .AsNoTracking()
                .ToListAsync();
        }) ?? new List<Product>();
    }

    /// <summary>
    /// UPDATE with cache invalidation
    /// </summary>
    public async Task UpdateProductAsync(Product product)
    {
        // Update database
        _db.Products.Update(product);
        await _db.SaveChangesAsync();

        // Invalidate cache
        _cache.Remove($"product:{product.Id}");
        _cache.Remove($"products:category:{product.CategoryId}");

        _logger.LogInformation("[Cache INVALIDATED] Product {Id}", product.Id);
    }

    /// <summary>
    /// DELETE with cache invalidation
    /// </summary>
    public async Task DeleteProductAsync(int id)
    {
        var product = await _db.Products.FindAsync(id);
        if (product == null)
            return;

        _db.Products.Remove(product);
        await _db.SaveChangesAsync();

        // Invalidate caches
        _cache.Remove($"product:{id}");
        _cache.Remove($"products:category:{product.CategoryId}");

        _logger.LogInformation("[Cache INVALIDATED] Product {Id} deleted", id);
    }
}

/// <summary>
/// Advanced Cache-Aside with sliding expiration and priorities
/// </summary>
public class AdvancedCacheAsideService
{
    private readonly IMemoryCache _cache;
    private readonly AppDbContext _db;

    public AdvancedCacheAsideService(IMemoryCache cache, AppDbContext db)
    {
        _cache = cache;
        _db = db;
    }

    /// <summary>
    /// GET with sliding expiration (resets on each access)
    /// </summary>
    public async Task<Product?> GetProductWithSlidingExpirationAsync(int id)
    {
        return await _cache.GetOrCreateAsync($"product:{id}", async entry =>
        {
            // Sliding expiration - resets every time it's accessed
            entry.SlidingExpiration = TimeSpan.FromMinutes(5);

            return await _db.Products.FindAsync(id);
        });
    }

    /// <summary>
    /// GET with priority-based caching
    /// Popular products stay longer
    /// </summary>
    public async Task<Product?> GetProductWithPriorityAsync(int id)
    {
        return await _cache.GetOrCreateAsync($"product:{id}", async entry =>
        {
            var product = await _db.Products.FindAsync(id);

            if (product != null)
            {
                // Hot items stay longer with high priority
                if (product.Sales > 1000)
                {
                    entry.Priority = CacheItemPriority.High;
                    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1);
                }
                else
                {
                    entry.Priority = CacheItemPriority.Low;
                    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);
                }
            }

            return product;
        });
    }

    /// <summary>
    /// GET with size limit
    /// </summary>
    public async Task<Product?> GetProductWithSizeLimitAsync(int id)
    {
        return await _cache.GetOrCreateAsync($"product:{id}", async entry =>
        {
            entry.Size = 1; // Each product = 1 unit
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);

            return await _db.Products.FindAsync(id);
        });
    }
}

/// <summary>
/// Cache-Aside with null value caching
/// Prevents cache penetration
/// </summary>
public class NullSafeCacheService
{
    private readonly IMemoryCache _cache;
    private readonly AppDbContext _db;

    public NullSafeCacheService(IMemoryCache cache, AppDbContext db)
    {
        _cache = cache;
        _db = db;
    }

    /// <summary>
    /// Cache null results to prevent repeated database hits
    /// </summary>
    public async Task<Product?> GetProductSafeAsync(int id)
    {
        var cacheKey = $"product:{id}";

        return await _cache.GetOrCreateAsync(cacheKey, async entry =>
        {
            var product = await _db.Products.FindAsync(id);

            if (product == null)
            {
                // Cache null result with shorter TTL
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(1);
                Console.WriteLine($"[Null Cached] Product {id} not found, cached for 1 minute");
            }
            else
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);
            }

            return product;
        });
    }
}

/// <summary>
/// Performance monitoring and metrics
/// </summary>
public class CacheMonitoringService
{
    private long _cacheHits;
    private long _cacheMisses;

    public void RecordCacheHit()
    {
        Interlocked.Increment(ref _cacheHits);
    }

    public void RecordCacheMiss()
    {
        Interlocked.Increment(ref _cacheMisses);
    }

    public CacheStatistics GetStatistics()
    {
        var total = _cacheHits + _cacheMisses;
        var hitRate = total > 0 ? (_cacheHits * 100.0 / total) : 0;

        return new CacheStatistics
        {
            Hits = _cacheHits,
            Misses = _cacheMisses,
            Total = total,
            HitRate = hitRate
        };
    }

    public void Reset()
    {
        Interlocked.Exchange(ref _cacheHits, 0);
        Interlocked.Exchange(ref _cacheMisses, 0);
    }
}

// Entity Models
public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public int CategoryId { get; set; }
    public int Sales { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class AppDbContext : DbContext
{
    public DbSet<Product> Products { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }
}

// DTOs
public class CacheStatistics
{
    public long Hits { get; set; }
    public long Misses { get; set; }
    public long Total { get; set; }
    public double HitRate { get; set; }

    public override string ToString()
    {
        return $"Hits: {Hits}, Misses: {Misses}, Hit Rate: {HitRate:F2}%";
    }
}

/// <summary>
/// Demo: Performance comparison
/// </summary>
public class CachePerformanceDemo
{
    private readonly IMemoryCache _cache;
    private readonly AppDbContext _db;

    public CachePerformanceDemo(IMemoryCache cache, AppDbContext db)
    {
        _cache = cache;
        _db = db;
    }

    public async Task RunBenchmarkAsync()
    {
        Console.WriteLine("=== Cache Performance Benchmark ===\n");

        // Test 1: Without cache
        var sw1 = System.Diagnostics.Stopwatch.StartNew();
        for (int i = 0; i < 100; i++)
        {
            var product = await _db.Products.FindAsync(1);
        }
        sw1.Stop();

        // Test 2: With cache
        var sw2 = System.Diagnostics.Stopwatch.StartNew();
        for (int i = 0; i < 100; i++)
        {
            var product = await _cache.GetOrCreateAsync("product:1", async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);
                return await _db.Products.FindAsync(1);
            });
        }
        sw2.Stop();

        Console.WriteLine($"Without cache (100 requests): {sw1.ElapsedMilliseconds}ms");
        Console.WriteLine($"With cache (100 requests):    {sw2.ElapsedMilliseconds}ms");
        Console.WriteLine($"Improvement: {(sw1.ElapsedMilliseconds - sw2.ElapsedMilliseconds) * 100.0 / sw1.ElapsedMilliseconds:F1}%");
    }
}
