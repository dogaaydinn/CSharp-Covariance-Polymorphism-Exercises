# Enterprise Solution: Write-Behind + Refresh-Ahead + Cache Warming

## 🎯 Yaklaşım

**Write-Behind** (async writes) + **Refresh-Ahead** (proactive refresh) + **Cache Warming** ile maximum throughput ve %99.9 uptime.

## 🚀 Write-Behind (Write-Back) Pattern

### Nasıl Çalışır?

```csharp
// Write Flow:
public async Task UpdateProductAsync(Product product)
{
    // 1. Write to cache immediately (fast!)
    await _cache.SetAsync($"product:{product.Id}", product);

    // 2. Queue database write (async)
    await _queue.EnqueueAsync(new WriteProductJob
    {
        Product = product,
        Timestamp = DateTime.UtcNow
    });

    // Return immediately - eventual consistency!
    return;
}

// Background Worker:
public class WriteBackgroundWorker : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var jobs = await _queue.DequeueAsync(100); // Batch 100 writes

            // Batch write to database
            await _db.Products.BulkUpdateAsync(jobs.Select(j => j.Product));

            Console.WriteLine($"[Write-Behind] Flushed {jobs.Count} writes to DB");
        }
    }
}
```

### Flow Diagram

```
Write Request → Write to Cache → Queue DB Write → Return (fast!)
                                        ↓
                                Background Worker
                                        ↓
                                Batch Write to DB
```

### Advantages

- ⚡ **Ultra-fast writes** (< 5ms)
- 📦 **Batch processing** (reduces DB load by 10x)
- 🎯 **High throughput** (10,000+ writes/sec)

### Challenges

- ❌ **Data loss risk** (if cache crashes before flush)
- ❌ **Eventual consistency**
- ❌ **Complex error handling**

## 🔄 Refresh-Ahead Pattern

### Problem: Cache Miss Storm

```
Popular product expires at 10:00:00
First request at 10:00:00.001 experiences cache miss
Meanwhile, 1000 more requests queue up
User experience degraded!
```

### Solution: Proactive Refresh

```csharp
public class RefreshAheadCache
{
    private readonly IDistributedCache _cache;
    private readonly AppDbContext _db;
    private readonly Timer _refreshTimer;

    public RefreshAheadCache()
    {
        // Refresh cache every minute for hot items
        _refreshTimer = new Timer(
            async _ => await RefreshHotItemsAsync(),
            null,
            TimeSpan.FromMinutes(1),
            TimeSpan.FromMinutes(1)
        );
    }

    public async Task<Product?> GetProductAsync(int id)
    {
        var cacheKey = $"product:{id}";

        // Always read from cache
        var cachedJson = await _cache.GetStringAsync(cacheKey);
        if (cachedJson != null)
        {
            var product = JsonSerializer.Deserialize<Product>(cachedJson);

            // Check if nearing expiration
            var metadata = await GetCacheMetadataAsync(cacheKey);
            var timeToExpiration = metadata.ExpiresAt - DateTime.UtcNow;

            // Refresh in background if < 20% of TTL remaining
            if (timeToExpiration.TotalSeconds < 60) // Last 1 minute of 5 min TTL
            {
                _ = Task.Run(async () =>
                {
                    Console.WriteLine($"[Refresh-Ahead] Proactively refreshing {cacheKey}");
                    await RefreshCacheAsync(id);
                });
            }

            return product;
        }

        // Cache miss - load normally
        return await LoadAndCacheAsync(id);
    }

    private async Task RefreshHotItemsAsync()
    {
        // Get hot items (high traffic products)
        var hotProductIds = await GetHotProductIdsAsync();

        Console.WriteLine($"[Refresh-Ahead] Refreshing {hotProductIds.Count} hot items");

        // Refresh in parallel
        await Parallel.ForEachAsync(hotProductIds, async (productId, ct) =>
        {
            await RefreshCacheAsync(productId);
        });
    }

    private async Task RefreshCacheAsync(int productId)
    {
        var product = await _db.Products
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == productId);

        if (product != null)
        {
            await _cache.SetStringAsync(
                $"product:{productId}",
                JsonSerializer.Serialize(product),
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
                });
        }
    }
}
```

## 🔥 Cache Warming

### Strategy 1: Application Startup

```csharp
public class CacheWarmingService : IHostedService
{
    private readonly IDistributedCache _cache;
    private readonly AppDbContext _db;
    private readonly ILogger<CacheWarmingService> _logger;

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("[Cache Warming] Starting...");

        var stopwatch = Stopwatch.StartNew();

        // Warm popular products
        var popularProducts = await _db.Products
            .Where(p => p.Sales > 100) // Popular threshold
            .OrderByDescending(p => p.Sales)
            .Take(1000) // Top 1000 products
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        // Load into cache in parallel
        await Parallel.ForEachAsync(popularProducts, cancellationToken, async (product, ct) =>
        {
            await _cache.SetStringAsync(
                $"product:{product.Id}",
                JsonSerializer.Serialize(product),
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1)
                });
        });

        stopwatch.Stop();
        _logger.LogInformation(
            "[Cache Warming] Completed in {Elapsed}ms. Warmed {Count} products",
            stopwatch.ElapsedMilliseconds,
            popularProducts.Count);
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
```

### Strategy 2: Scheduled Refresh

```csharp
public class ScheduledCacheRefreshService : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var now = DateTime.UtcNow;

            // Refresh every hour at :00 minutes
            var nextRefresh = new DateTime(now.Year, now.Month, now.Day, now.Hour, 0, 0)
                .AddHours(1);
            var delay = nextRefresh - now;

            await Task.Delay(delay, stoppingToken);

            _logger.LogInformation("[Scheduled Refresh] Starting hourly refresh");

            // Refresh all cached products
            var cachedProductIds = await GetAllCachedProductIdsAsync();
            await RefreshProductsAsync(cachedProductIds);

            _logger.LogInformation("[Scheduled Refresh] Completed");
        }
    }
}
```

### Strategy 3: Event-Driven Warming

```csharp
public class EventDrivenCacheWarming
{
    public async Task Handle(ProductViewedEvent @event)
    {
        // Track product views
        await _analytics.IncrementViewCountAsync(@event.ProductId);

        var viewCount = await _analytics.GetViewCountAsync(@event.ProductId);

        // If product becomes hot (>100 views in last hour), warm cache
        if (viewCount > 100)
        {
            _logger.LogInformation(
                "[Event-Driven Warming] Product {ProductId} is hot ({ViewCount} views)",
                @event.ProductId,
                viewCount);

            await WarmProductCacheAsync(@event.ProductId);
        }
    }

    private async Task WarmProductCacheAsync(int productId)
    {
        var product = await _db.Products.FindAsync(productId);
        if (product != null)
        {
            await _cache.SetStringAsync(
                $"product:{productId}",
                JsonSerializer.Serialize(product),
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(2), // Longer TTL for hot items
                    Priority = CacheItemPriority.High
                });
        }
    }
}
```

## 📊 Advanced Cache Monitoring

```csharp
public class CacheMetricsService
{
    private long _cacheHits;
    private long _cacheMisses;
    private long _cacheWrites;
    private readonly ConcurrentDictionary<string, long> _latencyBuckets = new();

    public void RecordCacheHit(string key, TimeSpan latency)
    {
        Interlocked.Increment(ref _cacheHits);
        RecordLatency("hit", latency);
    }

    public void RecordCacheMiss(string key, TimeSpan latency)
    {
        Interlocked.Increment(ref _cacheMisses);
        RecordLatency("miss", latency);
    }

    public void RecordCacheWrite(string key, TimeSpan latency)
    {
        Interlocked.Increment(ref _cacheWrites);
        RecordLatency("write", latency);
    }

    public CacheStatistics GetStatistics()
    {
        var total = _cacheHits + _cacheMisses;
        var hitRate = total > 0 ? (_cacheHits * 100.0 / total) : 0;

        return new CacheStatistics
        {
            CacheHits = _cacheHits,
            CacheMisses = _cacheMisses,
            CacheWrites = _cacheWrites,
            HitRate = hitRate,
            AvgHitLatency = GetAvgLatency("hit"),
            AvgMissLatency = GetAvgLatency("miss"),
            AvgWriteLatency = GetAvgLatency("write")
        };
    }

    private void RecordLatency(string operation, TimeSpan latency)
    {
        var bucket = GetLatencyBucket(latency);
        _latencyBuckets.AddOrUpdate($"{operation}_{bucket}", 1, (k, v) => v + 1);
    }

    private string GetLatencyBucket(TimeSpan latency)
    {
        return latency.TotalMilliseconds switch
        {
            < 1 => "0-1ms",
            < 5 => "1-5ms",
            < 10 => "5-10ms",
            < 50 => "10-50ms",
            < 100 => "50-100ms",
            _ => "100ms+"
        };
    }
}
```

## 🎯 Complete Enterprise Service

```csharp
public class EnterpriseCachingService
{
    private readonly IMemoryCache _l1Cache;
    private readonly IDistributedCache _l2Cache;
    private readonly AppDbContext _db;
    private readonly IBackgroundTaskQueue _queue;
    private readonly CacheMetricsService _metrics;
    private readonly ILogger<EnterpriseCachingService> _logger;

    // GET: Multi-level cache with refresh-ahead
    public async Task<Product?> GetProductAsync(int id)
    {
        var sw = Stopwatch.StartNew();
        var cacheKey = $"product:{id}";

        // L1: Memory cache (0.1ms)
        if (_l1Cache.TryGetValue(cacheKey, out Product? l1Product))
        {
            _metrics.RecordCacheHit($"L1:{cacheKey}", sw.Elapsed);
            return l1Product;
        }

        // L2: Redis cache (2ms)
        var cachedJson = await _l2Cache.GetStringAsync(cacheKey);
        if (cachedJson != null)
        {
            var l2Product = JsonSerializer.Deserialize<Product>(cachedJson);

            // Populate L1
            _l1Cache.Set(cacheKey, l2Product, TimeSpan.FromMinutes(5));

            _metrics.RecordCacheHit($"L2:{cacheKey}", sw.Elapsed);

            // Refresh-ahead check
            await CheckAndRefreshAsync(id);

            return l2Product;
        }

        // L3: Database (50-200ms)
        _metrics.RecordCacheMiss(cacheKey, sw.Elapsed);

        var product = await _db.Products
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id);

        if (product != null)
        {
            // Populate both caches
            await PopulateAllCachesAsync(product);
        }

        return product;
    }

    // CREATE: Write-through
    public async Task<Product> CreateProductAsync(Product product)
    {
        // 1. Write to database
        _db.Products.Add(product);
        await _db.SaveChangesAsync();

        // 2. Write to cache
        await PopulateAllCachesAsync(product);

        _logger.LogInformation("[Create] Product {Id} created and cached", product.Id);
        return product;
    }

    // UPDATE: Write-behind (async)
    public async Task UpdateProductAsync(Product product)
    {
        // 1. Update cache immediately
        await PopulateAllCachesAsync(product);

        // 2. Queue database write (async)
        await _queue.QueueBackgroundWorkItemAsync(async token =>
        {
            await Task.Delay(1000, token); // Debounce 1 second

            _db.Products.Update(product);
            await _db.SaveChangesAsync(token);

            _logger.LogInformation("[Write-Behind] Product {Id} flushed to DB", product.Id);
        });

        _logger.LogInformation("[Update] Product {Id} updated in cache", product.Id);
    }

    // DELETE: Write-through
    public async Task DeleteProductAsync(int id)
    {
        // 1. Delete from database
        var product = await _db.Products.FindAsync(id);
        if (product != null)
        {
            _db.Products.Remove(product);
            await _db.SaveChangesAsync();
        }

        // 2. Invalidate caches
        _l1Cache.Remove($"product:{id}");
        await _l2Cache.RemoveAsync($"product:{id}");

        _logger.LogInformation("[Delete] Product {Id} deleted and cache invalidated", id);
    }

    private async Task PopulateAllCachesAsync(Product product)
    {
        var cacheKey = $"product:{product.Id}";

        // L1 cache
        _l1Cache.Set(cacheKey, product, TimeSpan.FromMinutes(5));

        // L2 cache
        await _l2Cache.SetStringAsync(
            cacheKey,
            JsonSerializer.Serialize(product),
            new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30)
            });
    }

    private async Task CheckAndRefreshAsync(int productId)
    {
        // Check if nearing expiration (simplified)
        var shouldRefresh = Random.Shared.NextDouble() < 0.1; // 10% chance

        if (shouldRefresh)
        {
            _ = Task.Run(async () =>
            {
                var product = await _db.Products.FindAsync(productId);
                if (product != null)
                {
                    await PopulateAllCachesAsync(product);
                    _logger.LogInformation("[Refresh-Ahead] Product {Id} refreshed", productId);
                }
            });
        }
    }
}
```

## 📊 Performance Metrics

### Write Performance

| Strategy | Write Latency | Throughput | DB Load |
|----------|---------------|------------|---------|
| No Cache | 50ms | 20 req/s | 100% |
| Write-Through | 55ms | 18 req/s | 100% |
| Write-Behind | 5ms | 200 req/s | 10% |
| **Improvement** | **90% faster** | **10x higher** | **90% lower** |

### Read Performance

| Strategy | Cold Read | Warm Read | Cache Hit Rate |
|----------|-----------|-----------|----------------|
| Cache-Aside | 200ms | 2ms | 90% |
| Refresh-Ahead | 2ms | 2ms | 99.9% |
| **Improvement** | **100x faster** | **Same** | **No cache miss!** |

## 📝 Summary

**Write-Behind + Refresh-Ahead + Cache Warming** = Maximum performance!

**Avantajlar**:
- ⚡ Ultra-fast writes (< 5ms)
- 🎯 No cache miss (99.9% hit rate)
- 📦 Batch processing (10x DB efficiency)
- 🔥 Proactive warming
- 📊 Advanced monitoring

**Dezavantajlar**:
- ❌ Very complex
- ❌ Eventual consistency
- ❌ Data loss risk (mitigated with persistence)
- ❌ High infrastructure cost

**Ne Zaman Kullanmalı**:
- ✅ Ultra-high traffic (>1000 req/s)
- ✅ Write-heavy workloads
- ✅ Performance-critical systems
- ✅ Enterprise budget

**Sonuç**: `COMPARISON.md` - Tüm stratejilerin karşılaştırması
