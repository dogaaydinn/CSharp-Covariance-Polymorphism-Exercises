using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace RealWorldProblems.CacheStrategy.Advanced;

// Write-Through + Redis Distributed Cache
public class WriteThroughProductService
{
    private readonly IDistributedCache _cache;
    private readonly AppDbContext _db;

    public WriteThroughProductService(IDistributedCache cache, AppDbContext db)
    {
        _cache = cache;
        _db = db;
    }

    // CREATE: Write-Through
    public async Task<Product> CreateProductAsync(Product product)
    {
        _db.Products.Add(product);
        await _db.SaveChangesAsync();
        
        await WriteToCacheAsync(product);
        Console.WriteLine("[Write-Through] Product created and cached");
        return product;
    }

    // UPDATE: Write-Through
    public async Task UpdateProductAsync(Product product)
    {
        _db.Products.Update(product);
        await _db.SaveChangesAsync();
        
        await WriteToCacheAsync(product);
        Console.WriteLine("[Write-Through] Product updated and cache synced");
    }

    // DELETE: Write-Through
    public async Task DeleteProductAsync(int id)
    {
        var product = await _db.Products.FindAsync(id);
        if (product != null)
        {
            _db.Products.Remove(product);
            await _db.SaveChangesAsync();
        }
        
        await _cache.RemoveAsync($"product:{id}");
        Console.WriteLine("[Write-Through] Product deleted and cache invalidated");
    }

    // READ: Always from cache
    public async Task<Product?> GetProductAsync(int id)
    {
        var cacheKey = $"product:{id}";
        var cachedJson = await _cache.GetStringAsync(cacheKey);
        
        if (cachedJson != null)
        {
            return JsonSerializer.Deserialize<Product>(cachedJson);
        }
        
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

// Multi-Level Cache (L1 + L2)
public class MultiLevelCacheService
{
    private readonly IMemoryCache _l1Cache;
    private readonly IDistributedCache _l2Cache;
    private readonly AppDbContext _db;

    public async Task<Product?> GetProductAsync(int id)
    {
        var cacheKey = $"product:{id}";
        
        // L1: Memory (0.1ms)
        if (_l1Cache.TryGetValue(cacheKey, out Product? l1Product))
        {
            Console.WriteLine("[L1 HIT]");
            return l1Product;
        }
        
        // L2: Redis (2ms)
        var cachedJson = await _l2Cache.GetStringAsync(cacheKey);
        if (cachedJson != null)
        {
            Console.WriteLine("[L2 HIT]");
            var l2Product = JsonSerializer.Deserialize<Product>(cachedJson);
            _l1Cache.Set(cacheKey, l2Product, TimeSpan.FromMinutes(5));
            return l2Product;
        }
        
        // L3: Database (50-200ms)
        Console.WriteLine("[Database]");
        var product = await _db.Products.FindAsync(id);
        
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
}

// Cache Stampede Prevention with Distributed Lock
public class StampedePreventionService
{
    private readonly IDistributedCache _cache;
    private readonly AppDbContext _db;

    public async Task<Product?> GetProductWithLockAsync(int id)
    {
        var cacheKey = $"product:{id}";
        var lockKey = $"lock:{cacheKey}";
        
        var cachedJson = await _cache.GetStringAsync(cacheKey);
        if (cachedJson != null)
        {
            return JsonSerializer.Deserialize<Product>(cachedJson);
        }
        
        // Try acquire lock
        var lockValue = Guid.NewGuid().ToString();
        var lockAcquired = await TryAcquireLockAsync(lockKey, lockValue, TimeSpan.FromSeconds(10));
        
        if (!lockAcquired)
        {
            await Task.Delay(50);
            return await GetProductWithLockAsync(id);
        }
        
        try
        {
            cachedJson = await _cache.GetStringAsync(cacheKey);
            if (cachedJson != null)
            {
                return JsonSerializer.Deserialize<Product>(cachedJson);
            }
            
            Console.WriteLine("[Stampede PREVENTED] Loading from DB");
            var product = await _db.Products.FindAsync(id);
            
            if (product != null)
            {
                await _cache.SetStringAsync(
                    cacheKey,
                    JsonSerializer.Serialize(product),
                    new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5) });
            }
            
            return product;
        }
        finally
        {
            await ReleaseLockAsync(lockKey, lockValue);
        }
    }

    private async Task<bool> TryAcquireLockAsync(string key, string value, TimeSpan expiration)
    {
        var options = new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = expiration };
        
        try
        {
            await _cache.SetAsync(key, Encoding.UTF8.GetBytes(value), options);
            return true;
        }
        catch
        {
            return false;
        }
    }

    private async Task ReleaseLockAsync(string key, string value)
    {
        await _cache.RemoveAsync(key);
    }
}

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int CategoryId { get; set; }
}

public class AppDbContext : DbContext
{
    public DbSet<Product> Products { get; set; }
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
}
