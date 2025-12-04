# 8. Use Redis for Distributed Caching

**Status:** ✅ Accepted

**Date:** 2024-12-01

**Deciders:** Architecture Team, Performance Team

**Technical Story:** Implementation in `samples/06-CuttingEdge/AspireCloudStack`

---

## Context and Problem Statement

Modern microservices require caching to:
- Reduce database load
- Improve API response times
- Store session data across multiple instances
- Implement rate limiting
- Cache expensive computations

**Requirements:**
- Distributed cache (shared across multiple app instances)
- Support for TTL (Time To Live)
- High performance (sub-millisecond reads)
- Pub/Sub support for real-time features
- Data structure support (lists, sets, sorted sets, hashes)

**Traditional in-memory caching problems:**
- `MemoryCache` is per-instance (not shared)
- No cache invalidation across instances
- Lost when app restarts
- Can't use in load-balanced scenarios

---

## Decision Drivers

* **Performance** - Sub-millisecond read/write latency
* **Distributed** - Shared cache across all instances
* **Persistence Options** - Can survive restarts if needed
* **Data Structures** - Beyond key-value (lists, sets, sorted sets)
* **Pub/Sub** - Real-time messaging capabilities
* **Cloud Native** - First-class support in all clouds

---

## Considered Options

* **Option 1** - Redis
* **Option 2** - In-Memory Cache (MemoryCache)
* **Option 3** - SQL Server/PostgreSQL as cache
* **Option 4** - Memcached

---

## Decision Outcome

**Chosen option:** "Redis", because it provides high-performance distributed caching with rich data structures, pub/sub support, persistence options, and excellent .NET integration via StackExchange.Redis.

### Positive Consequences

* **Blazing Fast** - Sub-millisecond latency (avg 0.1-0.3ms)
* **Distributed** - All app instances share same cache
* **Rich Data Types** - Strings, lists, sets, sorted sets, hashes, bitmaps, hyperloglogs
* **Pub/Sub** - Built-in messaging for real-time features
* **Persistence** - Optional RDB/AOF for cache survival
* **TTL Support** - Automatic expiration of keys
* **Atomic Operations** - INCR, DECR, GETSET, etc.
* **Cloud Support** - Azure Cache for Redis, AWS ElastiCache, GCP Memorystore

### Negative Consequences

* **External Dependency** - Requires Redis server
* **Memory Limit** - Data must fit in RAM
* **Single Point of Failure** - Without clustering/replication
* **Eviction Policies** - Can lose data when memory full

---

## Pros and Cons of the Options

### Redis (Chosen)

**What is Redis?**

Redis (Remote Dictionary Server) is an in-memory data structure store used as a database, cache, and message broker. Known for exceptional performance and versatile data structures.

**Pros:**
* **Extremely Fast** - 100K+ ops/sec on single instance
* **Data Structures** - Lists, sets, sorted sets, hashes, bitmaps, streams
* **Pub/Sub** - Built-in messaging
* **Atomic Operations** - Thread-safe increments, decrements
* **Lua Scripting** - Server-side logic execution
* **Transactions** - MULTI/EXEC for atomicity
* **Persistence** - RDB snapshots + AOF logs
* **Replication** - Master-slave for high availability
* **Clustering** - Horizontal scaling

**Cons:**
* **Memory-Only** - Dataset limited by RAM (can use Redis on Flash)
* **Eviction** - Data lost when memory full (configurable policies)
* **Single-Threaded** - One command at a time per instance (but pipelined)
* **Complexity** - Clustering setup requires expertise

**Docker Compose Setup:**
```yaml
version: '3.8'

services:
  redis:
    image: redis:7-alpine
    container_name: myapp-redis
    ports:
      - "6379:6379"
    volumes:
      - redis-data:/data
    command: redis-server --appendonly yes  # Enable persistence
    healthcheck:
      test: ["CMD", "redis-cli", "ping"]
      interval: 10s
      timeout: 3s
      retries: 5

  redis-commander:  # Web UI for Redis
    image: rediscommander/redis-commander:latest
    container_name: myapp-redis-ui
    environment:
      - REDIS_HOSTS=local:redis:6379
    ports:
      - "8081:8081"
    depends_on:
      - redis

volumes:
  redis-data:
```

**.NET Aspire (Even Simpler):**
```csharp
// AppHost/Program.cs
var builder = DistributedApplication.CreateBuilder(args);

var redis = builder.AddRedis("redis")
    .WithRedisCommander()       // Automatically adds Redis Commander UI
    .WithDataVolume();          // Persistent storage

var api = builder.AddProject<Projects.ApiService>("api")
    .WithReference(redis);      // Connection string injected automatically!

await builder.Build().RunAsync();

// No docker-compose needed!
// Redis Commander: http://localhost:18888 (Aspire Dashboard shows link)
```

**.NET Integration:**
```csharp
// Install: StackExchange.Redis
// Install: Microsoft.Extensions.Caching.StackExchangeRedis

// Program.cs
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis");
    options.InstanceName = "MyApp:";  // Prefix for all keys
});

// Or use IConnectionMultiplexer for advanced scenarios
builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    var configuration = ConfigurationOptions.Parse(
        builder.Configuration.GetConnectionString("Redis"),
        ignoreUnknown: true);

    configuration.AbortOnConnectFail = false;
    configuration.ConnectRetry = 3;
    configuration.ConnectTimeout = 5000;

    return ConnectionMultiplexer.Connect(configuration);
});

// Usage - Simple Caching
public class ProductService
{
    private readonly IDistributedCache _cache;
    private readonly IProductRepository _repository;

    public async Task<Product> GetProductAsync(int id)
    {
        var cacheKey = $"product:{id}";

        // Try cache first
        var cachedJson = await _cache.GetStringAsync(cacheKey);
        if (cachedJson != null)
        {
            return JsonSerializer.Deserialize<Product>(cachedJson);
        }

        // Cache miss - fetch from database
        var product = await _repository.GetByIdAsync(id);

        // Store in cache (30 minute expiration)
        await _cache.SetStringAsync(
            cacheKey,
            JsonSerializer.Serialize(product),
            new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30)
            });

        return product;
    }
}
```

**Advanced Redis Features:**

**1. Atomic Counters (Rate Limiting):**
```csharp
public class RateLimiter
{
    private readonly IConnectionMultiplexer _redis;

    public async Task<bool> IsAllowedAsync(string userId, int maxRequests, TimeSpan window)
    {
        var db = _redis.GetDatabase();
        var key = $"rate:{userId}";

        // Atomic increment
        var current = await db.StringIncrementAsync(key);

        if (current == 1)
        {
            // First request - set expiration
            await db.KeyExpireAsync(key, window);
        }

        return current <= maxRequests;
    }
}

// Usage
if (!await _rateLimiter.IsAllowedAsync(userId, maxRequests: 100, window: TimeSpan.FromMinutes(1)))
{
    return StatusCode(429, "Too many requests");
}
```

**2. Distributed Locks:**
```csharp
public class DistributedLockService
{
    private readonly IConnectionMultiplexer _redis;

    public async Task<bool> AcquireLockAsync(string resource, string lockId, TimeSpan expiry)
    {
        var db = _redis.GetDatabase();
        var key = $"lock:{resource}";

        // SET NX EX (Set if Not eXists with EXpiry)
        return await db.StringSetAsync(key, lockId, expiry, When.NotExists);
    }

    public async Task ReleaseLockAsync(string resource, string lockId)
    {
        var db = _redis.GetDatabase();
        var key = $"lock:{resource}";

        // Lua script ensures we only delete our own lock
        var script = @"
            if redis.call('get', KEYS[1]) == ARGV[1] then
                return redis.call('del', KEYS[1])
            else
                return 0
            end";

        await db.ScriptEvaluateAsync(script, new RedisKey[] { key }, new RedisValue[] { lockId });
    }
}

// Usage - Prevent duplicate processing
var lockId = Guid.NewGuid().ToString();
if (await _lockService.AcquireLockAsync($"process-order:{orderId}", lockId, TimeSpan.FromSeconds(30)))
{
    try
    {
        await ProcessOrderAsync(orderId);
    }
    finally
    {
        await _lockService.ReleaseLockAsync($"process-order:{orderId}", lockId);
    }
}
```

**3. Pub/Sub (Real-Time Messaging):**
```csharp
// Publisher
public class NotificationPublisher
{
    private readonly IConnectionMultiplexer _redis;

    public async Task PublishAsync(string channel, string message)
    {
        var subscriber = _redis.GetSubscriber();
        await subscriber.PublishAsync(channel, message);
    }
}

// Subscriber (Background Service)
public class NotificationSubscriber : BackgroundService
{
    private readonly IConnectionMultiplexer _redis;
    private readonly ILogger<NotificationSubscriber> _logger;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var subscriber = _redis.GetSubscriber();

        await subscriber.SubscribeAsync("notifications", (channel, message) =>
        {
            _logger.LogInformation($"Received: {message}");
            // Handle notification (send email, push notification, etc.)
        });

        await Task.Delay(Timeout.Infinite, stoppingToken);
    }
}

// Usage
await _publisher.PublishAsync("notifications", JsonSerializer.Serialize(new
{
    Type = "OrderShipped",
    OrderId = 12345,
    UserId = "user-123"
}));
```

**4. Sorted Sets (Leaderboards):**
```csharp
public class LeaderboardService
{
    private readonly IConnectionMultiplexer _redis;

    public async Task AddScoreAsync(string userId, double score)
    {
        var db = _redis.GetDatabase();
        await db.SortedSetAddAsync("leaderboard", userId, score);
    }

    public async Task<(string UserId, double Score)[]> GetTopAsync(int count)
    {
        var db = _redis.GetDatabase();
        var top = await db.SortedSetRangeByRankWithScoresAsync(
            "leaderboard",
            start: 0,
            stop: count - 1,
            order: Order.Descending);

        return top.Select(x => (x.Element.ToString(), x.Score)).ToArray();
    }

    public async Task<long> GetRankAsync(string userId)
    {
        var db = _redis.GetDatabase();
        var rank = await db.SortedSetRankAsync("leaderboard", userId, Order.Descending);
        return rank.HasValue ? rank.Value + 1 : -1;  // 1-based ranking
    }
}

// Usage
await _leaderboard.AddScoreAsync("user123", 9500);
var top10 = await _leaderboard.GetTopAsync(10);
var userRank = await _leaderboard.GetRankAsync("user123");  // e.g., 42
```

**5. Sets (Session Management):**
```csharp
public class SessionService
{
    private readonly IConnectionMultiplexer _redis;

    public async Task AddActiveUserAsync(string userId)
    {
        var db = _redis.GetDatabase();
        await db.SetAddAsync("active-users", userId);
    }

    public async Task RemoveActiveUserAsync(string userId)
    {
        var db = _redis.GetDatabase();
        await db.SetRemoveAsync("active-users", userId);
    }

    public async Task<long> GetActiveUserCountAsync()
    {
        var db = _redis.GetDatabase();
        return await db.SetLengthAsync("active-users");
    }

    public async Task<bool> IsUserActiveAsync(string userId)
    {
        var db = _redis.GetDatabase();
        return await db.SetContainsAsync("active-users", userId);
    }
}
```

### In-Memory Cache (MemoryCache)

**Pros:**
* Built into .NET (no external dependencies)
* Very fast (no network latency)
* Simple to use

**Cons:**
* **Per-Instance** - Each app instance has separate cache
* **Lost on Restart** - Cache cleared when app restarts
* **No Distribution** - Can't share across load-balanced instances
* **Memory Pressure** - Competes with app for RAM

**Example:**
```csharp
// Works for single instance only!
public class ProductService
{
    private readonly IMemoryCache _cache;

    public async Task<Product> GetProductAsync(int id)
    {
        return await _cache.GetOrCreateAsync($"product:{id}", async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30);
            return await _repository.GetByIdAsync(id);
        });
    }
}
```

**When to Use:**
- Single instance applications
- Desktop applications
- Development/testing only

**Why Rejected for Microservices:**
Microservices run in multiple instances behind load balancer. MemoryCache would result in cache inconsistency (each instance has different data) and wasted database queries.

### SQL Server/PostgreSQL as Cache

**Pros:**
* Already have the infrastructure
* Persistent by default
* ACID transactions

**Cons:**
* **Slow** - 10-100x slower than Redis (disk I/O)
* **Not Designed for Caching** - No automatic eviction
* **Schema Required** - Need to define tables
* **Overhead** - Full RDBMS for simple key-value

**Why Rejected:**
Using a database for caching defeats the purpose of caching (reducing database load). Redis is 100x faster because it's RAM-only.

### Memcached

**Pros:**
* Simple and fast
* Widely adopted
* Lower memory overhead than Redis

**Cons:**
* **Only Key-Value** - No lists, sets, sorted sets
* **No Persistence** - Always volatile
* **No Pub/Sub** - Can't use for messaging
* **Limited Operations** - No atomic increments (in multi-threaded way)

**Why Not Chosen:**
Memcached is excellent for simple key-value caching, but Redis provides **all of Memcached's benefits plus** rich data structures, pub/sub, Lua scripting, and persistence. The extra features justify the slightly higher memory usage.

---

## Feature Comparison

| Feature | Redis | Memcached | MemoryCache | Database |
|---------|-------|-----------|-------------|----------|
| **Performance** | ✅ < 1ms | ✅ < 1ms | ✅ < 0.1ms | ❌ 10-100ms |
| **Distributed** | ✅ | ✅ | ❌ | ✅ |
| **Persistence** | ✅ Optional | ❌ | ❌ | ✅ Always |
| **Data Structures** | ✅ Many | ❌ Key-Value only | ❌ Key-Value | ✅ Tables |
| **TTL** | ✅ | ✅ | ✅ | ⚠️ Manual |
| **Pub/Sub** | ✅ | ❌ | ❌ | ⚠️ Complex |
| **Atomic Ops** | ✅ | ⚠️ Limited | ❌ | ✅ |
| **Clustering** | ✅ | ⚠️ Client-side | ❌ | ✅ |

---

## Production Deployment

**Azure Cache for Redis:**
```bash
az redis create \
  --name myapp-redis \
  --resource-group myapp-rg \
  --location eastus \
  --sku Standard \
  --vm-size C1 \
  --enable-non-ssl-port false

# Connection string
myapp-redis.redis.cache.windows.net:6380,password=xxx,ssl=True,abortConnect=False
```

**AWS ElastiCache:**
```bash
aws elasticache create-cache-cluster \
  --cache-cluster-id myapp-redis \
  --engine redis \
  --cache-node-type cache.t3.medium \
  --num-cache-nodes 1
```

**Kubernetes Helm:**
```bash
helm install redis bitnami/redis \
  --set auth.password=password \
  --set master.persistence.size=8Gi
```

---

## Best Practices

**1. Key Naming Convention:**
```csharp
// Good - Hierarchical, readable
"myapp:user:123:profile"
"myapp:product:456:details"
"myapp:session:abc-def-ghi"

// Bad - Hard to understand
"u123"
"p456"
```

**2. Set Appropriate TTL:**
```csharp
// Frequently changing data - short TTL
await _cache.SetAsync("trending-products", data, TimeSpan.FromMinutes(5));

// Rarely changing data - longer TTL
await _cache.SetAsync("categories", data, TimeSpan.FromHours(24));

// Static data - very long TTL
await _cache.SetAsync("country-codes", data, TimeSpan.FromDays(30));
```

**3. Cache Invalidation:**
```csharp
// When data changes, invalidate cache
public async Task UpdateProductAsync(Product product)
{
    await _repository.UpdateAsync(product);

    // Invalidate cache
    await _cache.RemoveAsync($"product:{product.Id}");
}
```

**4. Handle Cache Failures Gracefully:**
```csharp
public async Task<Product> GetProductAsync(int id)
{
    try
    {
        var cached = await _cache.GetStringAsync($"product:{id}");
        if (cached != null)
            return JsonSerializer.Deserialize<Product>(cached);
    }
    catch (RedisException ex)
    {
        _logger.LogWarning(ex, "Redis cache failure, falling back to database");
        // Continue to database - don't fail the request
    }

    return await _repository.GetByIdAsync(id);
}
```

---

## Links

* [Redis Official Documentation](https://redis.io/documentation)
* [StackExchange.Redis](https://stackexchange.github.io/StackExchange.Redis/)
* [Redis Data Types](https://redis.io/docs/data-types/)
* [Sample Implementation](../../samples/06-CuttingEdge/AspireCloudStack)

---

## Notes

**Common Use Cases:**
- ✅ Session storage
- ✅ API response caching
- ✅ Rate limiting
- ✅ Leaderboards
- ✅ Real-time analytics
- ✅ Job queues
- ✅ Pub/Sub messaging

**When NOT to Use Redis:**
- ❌ Primary data storage (use database)
- ❌ Data larger than RAM
- ❌ Complex queries and joins (use database)

**Eviction Policies:**
- `noeviction` - Return errors when memory full (default)
- `allkeys-lru` - Evict least recently used keys
- `volatile-lru` - Evict LRU keys with TTL
- `allkeys-lfu` - Evict least frequently used

**Review Date:** 2025-12-01
