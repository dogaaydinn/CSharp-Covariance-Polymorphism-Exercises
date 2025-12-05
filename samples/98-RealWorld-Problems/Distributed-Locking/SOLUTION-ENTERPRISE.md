# Enterprise Solution: Distributed Lock with Redis (Redlock)

## Approach: Redis-based Distributed Locking

```csharp
public class RedisDistributedLock
{
    private readonly IConnectionMultiplexer _redis;
    
    public async Task<bool> ExecuteWithLockAsync(
        string lockKey,
        TimeSpan lockTimeout,
        Func<Task> action)
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
            return false;
        
        try
        {
            await action();
            return true;
        }
        finally
        {
            // Release lock (only if we still own it)
            var luaScript = @"
                if redis.call('get', KEYS[1]) == ARGV[1] then
                    return redis.call('del', KEYS[1])
                else
                    return 0
                end";
            
            await db.ScriptEvaluateAsync(luaScript, 
                new RedisKey[] { lockKey }, 
                new RedisValue[] { lockValue });
        }
    }
}

// Usage
var success = await _lock.ExecuteWithLockAsync(
    "product:lock:1",
    TimeSpan.FromSeconds(10),
    async () =>
    {
        var product = await _db.Products.FindAsync(1);
        product.Stock -= 1;
        await _db.SaveChangesAsync();
    });
```

**Pros**: Works across multiple servers, fast, reliable
**Cons**: Requires Redis, complex implementation

**When to Use**: Multi-server, high concurrency, distributed systems
