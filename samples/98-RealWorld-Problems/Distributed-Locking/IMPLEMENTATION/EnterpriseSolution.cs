using StackExchange.Redis;

namespace RealWorldProblems.DistributedLocking.Enterprise;

// Redis Distributed Lock (Redlock Pattern)
public class RedisDistributedLock
{
    private readonly IConnectionMultiplexer _redis;

    public RedisDistributedLock(IConnectionMultiplexer redis)
    {
        _redis = redis;
    }

    public async Task<bool> ExecuteWithLockAsync<T>(
        string lockKey,
        TimeSpan lockTimeout,
        Func<Task<T>> action,
        out T result)
    {
        var db = _redis.GetDatabase();
        var lockValue = Guid.NewGuid().ToString();
        
        // Try acquire lock with SET NX EX
        var acquired = await db.StringSetAsync(
            lockKey,
            lockValue,
            lockTimeout,
            When.NotExists);
        
        if (!acquired)
        {
            Console.WriteLine($"[Redis Lock] Failed to acquire lock: {lockKey}");
            result = default!;
            return false;
        }
        
        Console.WriteLine($"[Redis Lock] Acquired: {lockKey}");
        
        try
        {
            result = await action();
            return true;
        }
        finally
        {
            // Release lock atomically (Lua script)
            var luaScript = @"
                if redis.call('get', KEYS[1]) == ARGV[1] then
                    return redis.call('del', KEYS[1])
                else
                    return 0
                end";
            
            await db.ScriptEvaluateAsync(
                luaScript,
                new RedisKey[] { lockKey },
                new RedisValue[] { lockValue });
            
            Console.WriteLine($"[Redis Lock] Released: {lockKey}");
        }
    }
}

// Product Service with Distributed Locking
public class DistributedProductService
{
    private readonly RedisDistributedLock _lock;
    private readonly AppDbContext _db;

    public async Task<bool> UpdateStockAsync(int productId, int quantity)
    {
        var lockKey = $"product:lock:{productId}";
        
        var success = await _lock.ExecuteWithLockAsync(
            lockKey,
            TimeSpan.FromSeconds(10),
            async () =>
            {
                var product = await _db.Products.FindAsync(productId);
                
                if (product == null || product.Stock < quantity)
                    return false;
                
                product.Stock -= quantity;
                await _db.SaveChangesAsync();
                
                Console.WriteLine($"[Distributed Lock] Stock updated: {product.Stock} remaining");
                return true;
            },
            out bool result);
        
        return success && result;
    }
}

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Stock { get; set; }
}

public class AppDbContext : DbContext
{
    public DbSet<Product> Products { get; set; }
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
}
