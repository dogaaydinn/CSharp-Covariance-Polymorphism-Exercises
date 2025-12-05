using Microsoft.Extensions.Hosting;
using System.Collections.Concurrent;
using System.Text.Json;

namespace RealWorldProblems.CacheStrategy.Enterprise;

// Write-Behind Pattern
public class WriteBehindProductService
{
    private readonly IDistributedCache _cache;
    private readonly IBackgroundTaskQueue _queue;

    public async Task UpdateProductAsync(Product product)
    {
        // 1. Update cache immediately (fast!)
        await _cache.SetStringAsync(
            $"product:{product.Id}",
            JsonSerializer.Serialize(product),
            new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30) });
        
        // 2. Queue database write (async)
        await _queue.QueueBackgroundWorkItemAsync(async token =>
        {
            await Task.Delay(1000, token); // Debounce
            await _db.Products.UpdateAsync(product, token);
            Console.WriteLine($"[Write-Behind] Product {product.Id} flushed to DB");
        });
        
        Console.WriteLine($"[Write-Behind] Product {product.Id} updated in cache");
    }
}

// Refresh-Ahead Pattern
public class RefreshAheadCacheService : BackgroundService
{
    private readonly IDistributedCache _cache;
    private readonly AppDbContext _db;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            
            var hotProductIds = await GetHotProductIdsAsync();
            Console.WriteLine($"[Refresh-Ahead] Refreshing {hotProductIds.Count} hot items");
            
            await Parallel.ForEachAsync(hotProductIds, stoppingToken, async (productId, ct) =>
            {
                var product = await _db.Products.FindAsync(productId, ct);
                if (product != null)
                {
                    await _cache.SetStringAsync(
                        $"product:{productId}",
                        JsonSerializer.Serialize(product),
                        new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5) });
                }
            });
        }
    }

    private async Task<List<int>> GetHotProductIdsAsync()
    {
        return await _db.Products
            .Where(p => p.Sales > 100)
            .OrderByDescending(p => p.Sales)
            .Take(100)
            .Select(p => p.Id)
            .ToListAsync();
    }
}

// Cache Warming Service
public class CacheWarmingService : IHostedService
{
    private readonly IDistributedCache _cache;
    private readonly AppDbContext _db;
    private readonly ILogger<CacheWarmingService> _logger;

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("[Cache Warming] Starting...");
        var stopwatch = Stopwatch.StartNew();
        
        var popularProducts = await _db.Products
            .Where(p => p.Sales > 100)
            .OrderByDescending(p => p.Sales)
            .Take(1000)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
        
        await Parallel.ForEachAsync(popularProducts, cancellationToken, async (product, ct) =>
        {
            await _cache.SetStringAsync(
                $"product:{product.Id}",
                JsonSerializer.Serialize(product),
                new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1) });
        });
        
        stopwatch.Stop();
        _logger.LogInformation(
            "[Cache Warming] Completed in {Elapsed}ms. Warmed {Count} products",
            stopwatch.ElapsedMilliseconds,
            popularProducts.Count);
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}

// Complete Enterprise Service
public class EnterpriseCachingService
{
    private readonly IMemoryCache _l1Cache;
    private readonly IDistributedCache _l2Cache;
    private readonly AppDbContext _db;
    private readonly IBackgroundTaskQueue _queue;
    private readonly CacheMetricsService _metrics;

    public async Task<Product?> GetProductAsync(int id)
    {
        var sw = Stopwatch.StartNew();
        var cacheKey = $"product:{id}";
        
        // L1: Memory
        if (_l1Cache.TryGetValue(cacheKey, out Product? l1Product))
        {
            _metrics.RecordCacheHit($"L1:{cacheKey}", sw.Elapsed);
            return l1Product;
        }
        
        // L2: Redis
        var cachedJson = await _l2Cache.GetStringAsync(cacheKey);
        if (cachedJson != null)
        {
            var l2Product = JsonSerializer.Deserialize<Product>(cachedJson);
            _l1Cache.Set(cacheKey, l2Product, TimeSpan.FromMinutes(5));
            _metrics.RecordCacheHit($"L2:{cacheKey}", sw.Elapsed);
            return l2Product;
        }
        
        // L3: Database
        _metrics.RecordCacheMiss(cacheKey, sw.Elapsed);
        var product = await _db.Products.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
        
        if (product != null)
        {
            _l1Cache.Set(cacheKey, product, TimeSpan.FromMinutes(5));
            await _l2Cache.SetStringAsync(
                cacheKey,
                JsonSerializer.Serialize(product),
                new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30) });
        }
        
        return product;
    }

    // Write-behind update
    public async Task UpdateProductAsync(Product product)
    {
        await PopulateAllCachesAsync(product);
        
        await _queue.QueueBackgroundWorkItemAsync(async token =>
        {
            await Task.Delay(1000, token);
            _db.Products.Update(product);
            await _db.SaveChangesAsync(token);
        });
    }

    private async Task PopulateAllCachesAsync(Product product)
    {
        var cacheKey = $"product:{product.Id}";
        _l1Cache.Set(cacheKey, product, TimeSpan.FromMinutes(5));
        await _l2Cache.SetStringAsync(
            cacheKey,
            JsonSerializer.Serialize(product),
            new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30) });
    }
}

// Cache Metrics
public class CacheMetricsService
{
    private long _cacheHits;
    private long _cacheMisses;

    public void RecordCacheHit(string key, TimeSpan latency) => Interlocked.Increment(ref _cacheHits);
    public void RecordCacheMiss(string key, TimeSpan latency) => Interlocked.Increment(ref _cacheMisses);

    public CacheStatistics GetStatistics()
    {
        var total = _cacheHits + _cacheMisses;
        var hitRate = total > 0 ? (_cacheHits * 100.0 / total) : 0;
        return new CacheStatistics { Hits = _cacheHits, Misses = _cacheMisses, HitRate = hitRate };
    }
}

public interface IBackgroundTaskQueue
{
    ValueTask QueueBackgroundWorkItemAsync(Func<CancellationToken, ValueTask> workItem);
    ValueTask<Func<CancellationToken, ValueTask>> DequeueAsync(CancellationToken cancellationToken);
}

public class CacheStatistics
{
    public long Hits { get; set; }
    public long Misses { get; set; }
    public double HitRate { get; set; }
}

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Sales { get; set; }
}

public class AppDbContext : DbContext
{
    public DbSet<Product> Products { get; set; }
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
}
