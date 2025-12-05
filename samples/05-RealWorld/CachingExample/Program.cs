using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System.Diagnostics;
using System.Text.Json;

/// <summary>
/// Distributed Caching with Redis Demo
///
/// Demonstrates:
/// - MemoryCache vs DistributedCache comparison
/// - Cache-aside pattern implementation
/// - Redis connection multiplexing
/// - Cache invalidation strategies
/// - Performance benchmarks
/// - Serialization strategies
/// </summary>

Console.WriteLine("=== Distributed Caching with Redis Demo ===\n");

// Check if Redis is available
var redisConnectionString = Environment.GetEnvironmentVariable("REDIS_CONNECTION") ?? "localhost:6379";
var useRedis = await IsRedisAvailableAsync(redisConnectionString);

if (useRedis)
{
    Console.WriteLine($"✅ Redis available at {redisConnectionString}\n");
    await RunRedisExamplesAsync(redisConnectionString);
}
else
{
    Console.WriteLine($"⚠️  Redis not available at {redisConnectionString}");
    Console.WriteLine("Running MemoryCache-only examples...\n");
    await RunMemoryCacheExamplesAsync();
}

Console.WriteLine("\n=== Demo Complete ===");

// ============================================================================
// Redis Examples (Full Distributed Cache)
// ============================================================================

static async Task RunRedisExamplesAsync(string connectionString)
{
    var services = new ServiceCollection();

    // Configure logging
    services.AddLogging(builder => builder.AddConsole().SetMinimumLevel(LogLevel.Warning));

    // Configure MemoryCache
    services.AddMemoryCache();

    // Configure Redis DistributedCache
    services.AddStackExchangeRedisCache(options =>
    {
        options.Configuration = connectionString;
        options.InstanceName = "CachingDemo:";
    });

    var serviceProvider = services.BuildServiceProvider();
    var memoryCache = serviceProvider.GetRequiredService<IMemoryCache>();
    var distributedCache = serviceProvider.GetRequiredService<IDistributedCache>();

    Console.WriteLine(new string('=', 60));
    Console.WriteLine("REDIS DISTRIBUTED CACHE DEMONSTRATIONS");
    Console.WriteLine(new string('=', 60) + "\n");

    // 1. MemoryCache vs DistributedCache
    await Demo1_MemoryVsDistributedAsync(memoryCache, distributedCache);

    // 2. Cache-Aside Pattern
    await Demo2_CacheAsidePatternAsync(distributedCache);

    // 3. Cache Invalidation Strategies
    await Demo3_InvalidationStrategiesAsync(distributedCache);

    // 4. Connection Multiplexing
    await Demo4_ConnectionMultiplexingAsync(connectionString);

    // 5. Performance Benchmarks
    await Demo5_PerformanceBenchmarksAsync(memoryCache, distributedCache);

    // 6. Serialization Strategies
    await Demo6_SerializationStrategiesAsync(distributedCache);

    // 7. TTL and Expiration
    await Demo7_TTLAndExpirationAsync(distributedCache);
}

// ============================================================================
// MemoryCache-Only Examples (Fallback)
// ============================================================================

static async Task RunMemoryCacheExamplesAsync()
{
    var cache = new MemoryCache(new MemoryCacheOptions());

    Console.WriteLine("1️⃣  MEMORY CACHE BASICS\n");

    // Cache-aside pattern with MemoryCache
    var userId = 42;
    var user = GetUserWithCache(cache, userId);
    Console.WriteLine($"First call: {user.Name} (from database)");

    user = GetUserWithCache(cache, userId);
    Console.WriteLine($"Second call: {user.Name} (from cache!)");

    // Invalidation
    cache.Remove($"user_{userId}");
    user = GetUserWithCache(cache, userId);
    Console.WriteLine($"After invalidation: {user.Name} (from database again)\n");

    // Expiration demo
    Console.WriteLine("2️⃣  CACHE EXPIRATION\n");
    cache.Set("shortLived", "I expire quickly", TimeSpan.FromSeconds(2));
    Console.WriteLine($"Immediately: {cache.Get<string>("shortLived")}");
    await Task.Delay(2500);
    Console.WriteLine($"After 2.5s: {cache.Get<string>("shortLived") ?? "(expired)"}\n");
}

static User GetUserWithCache(IMemoryCache cache, int userId)
{
    var key = $"user_{userId}";

    if (!cache.TryGetValue(key, out User? user))
    {
        Console.WriteLine("  → Cache MISS, querying database...");
        user = GetUserFromDatabase(userId);
        cache.Set(key, user, TimeSpan.FromMinutes(5));
    }
    else
    {
        Console.WriteLine("  → Cache HIT!");
    }

    return user!;
}

// ============================================================================
// Demonstrations
// ============================================================================

static async Task Demo1_MemoryVsDistributedAsync(IMemoryCache memoryCache, IDistributedCache distributedCache)
{
    Console.WriteLine("1️⃣  MEMORY CACHE vs DISTRIBUTED CACHE\n");

    var key = "demo1_user";
    var user = new User(1, "Alice", "alice@example.com");

    // MemoryCache: In-process, fast
    Console.WriteLine("MemoryCache (In-Process):");
    memoryCache.Set(key, user, TimeSpan.FromMinutes(5));
    var memoryCached = memoryCache.Get<User>(key);
    Console.WriteLine($"  ✓ Stored and retrieved: {memoryCached?.Name}");
    Console.WriteLine("  ✓ Scope: Single process only");
    Console.WriteLine("  ✓ Speed: ~0.001ms (in-memory)");
    Console.WriteLine("  ✓ Persistence: Lost on app restart\n");

    // DistributedCache: Out-of-process, shared
    Console.WriteLine("DistributedCache (Redis):");
    await distributedCache.SetStringAsync(key, JsonSerializer.Serialize(user),
        new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5) });
    var distributedCached = JsonSerializer.Deserialize<User>(await distributedCache.GetStringAsync(key) ?? "");
    Console.WriteLine($"  ✓ Stored and retrieved: {distributedCached?.Name}");
    Console.WriteLine("  ✓ Scope: Shared across all app instances");
    Console.WriteLine("  ✓ Speed: ~1-2ms (network roundtrip)");
    Console.WriteLine("  ✓ Persistence: Survives app restarts\n");

    Console.WriteLine("Key Differences:");
    Console.WriteLine("  • MemoryCache: Fast but isolated per process");
    Console.WriteLine("  • DistributedCache: Slower but shared across processes");
    Console.WriteLine("  • Use MemoryCache for: Single-server apps, frequently accessed data");
    Console.WriteLine("  • Use DistributedCache for: Multi-server apps, session state, shared data\n");
}

static async Task Demo2_CacheAsidePatternAsync(IDistributedCache cache)
{
    Console.WriteLine("2️⃣  CACHE-ASIDE PATTERN (Lazy Loading)\n");

    var productId = 123;

    // First call - cache miss, load from database
    var product1 = await GetProductWithCacheAsideAsync(cache, productId);
    Console.WriteLine($"First call: {product1.Name} | Price: ${product1.Price}");

    // Second call - cache hit
    var product2 = await GetProductWithCacheAsideAsync(cache, productId);
    Console.WriteLine($"Second call: {product2.Name} (from cache!)");

    // Update product - invalidate cache
    Console.WriteLine("\nUpdating product price...");
    await cache.RemoveAsync($"product_{productId}");

    // Third call - cache miss again (reloads from database)
    var product3 = await GetProductWithCacheAsideAsync(cache, productId);
    Console.WriteLine($"After update: {product3.Name} | New Price: ${product3.Price}\n");

    Console.WriteLine("Cache-Aside Pattern Steps:");
    Console.WriteLine("  1. App requests data");
    Console.WriteLine("  2. Check cache first");
    Console.WriteLine("  3. If MISS: Load from database, store in cache");
    Console.WriteLine("  4. If HIT: Return cached data");
    Console.WriteLine("  5. On update: Invalidate cache entry\n");
}

static async Task<Product> GetProductWithCacheAsideAsync(IDistributedCache cache, int productId)
{
    var key = $"product_{productId}";

    // Check cache
    var cachedData = await cache.GetStringAsync(key);
    if (cachedData != null)
    {
        Console.WriteLine("  → Cache HIT!");
        return JsonSerializer.Deserialize<Product>(cachedData)!;
    }

    // Cache miss - load from database
    Console.WriteLine("  → Cache MISS, querying database...");
    var product = await GetProductFromDatabaseAsync(productId);

    // Store in cache
    var options = new DistributedCacheEntryOptions
    {
        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
    };
    await cache.SetStringAsync(key, JsonSerializer.Serialize(product), options);

    return product;
}

static async Task Demo3_InvalidationStrategiesAsync(IDistributedCache cache)
{
    Console.WriteLine("3️⃣  CACHE INVALIDATION STRATEGIES\n");

    Console.WriteLine("Strategy 1: Time-Based Expiration (TTL)");
    await cache.SetStringAsync("ttl_demo", "Expires in 3 seconds",
        new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(3) });
    Console.WriteLine("  ✓ Set with 3-second TTL");
    await Task.Delay(1000);
    Console.WriteLine($"  ✓ After 1s: {await cache.GetStringAsync("ttl_demo") ?? "(null)"}");
    await Task.Delay(2500);
    Console.WriteLine($"  ✓ After 3.5s: {await cache.GetStringAsync("ttl_demo") ?? "(expired)"}\n");

    Console.WriteLine("Strategy 2: Manual Invalidation (on update)");
    await cache.SetStringAsync("manual_demo", "Original value");
    Console.WriteLine("  ✓ Stored: Original value");
    Console.WriteLine("  ✓ Simulating data update...");
    await cache.RemoveAsync("manual_demo");
    Console.WriteLine($"  ✓ After removal: {await cache.GetStringAsync("manual_demo") ?? "(removed)"}\n");

    Console.WriteLine("Strategy 3: Tag-Based Invalidation (invalidate multiple)");
    var userKeys = new[] { "user_1", "user_2", "user_3" };
    foreach (var key in userKeys)
    {
        await cache.SetStringAsync(key, $"Data for {key}");
    }
    Console.WriteLine("  ✓ Stored 3 user cache entries");
    Console.WriteLine("  ✓ Invalidating all user_* entries...");
    foreach (var key in userKeys)
    {
        await cache.RemoveAsync(key);
    }
    Console.WriteLine("  ✓ All user cache entries removed\n");

    Console.WriteLine("Best Practices:");
    Console.WriteLine("  • Short TTL for frequently changing data (30s-5m)");
    Console.WriteLine("  • Long TTL for static data (1h-24h)");
    Console.WriteLine("  • Manual invalidation on updates");
    Console.WriteLine("  • Tag-based invalidation for related data\n");
}

static async Task Demo4_ConnectionMultiplexingAsync(string connectionString)
{
    Console.WriteLine("4️⃣  REDIS CONNECTION MULTIPLEXING\n");

    // Single connection multiplexed across multiple operations
    var connection = await ConnectionMultiplexer.ConnectAsync(connectionString);
    var db = connection.GetDatabase();

    Console.WriteLine("Connection Multiplexing Benefits:");
    Console.WriteLine("  • Single TCP connection for all operations");
    Console.WriteLine("  • Reduces connection overhead");
    Console.WriteLine("  • Thread-safe, concurrent operations\n");

    // Perform multiple concurrent operations
    Console.WriteLine("Executing 5 concurrent SET operations...");
    var tasks = Enumerable.Range(1, 5).Select(i =>
        db.StringSetAsync($"multiplex_{i}", $"Value {i}")
    ).ToArray();
    await Task.WhenAll(tasks);
    Console.WriteLine("  ✓ All operations completed using single connection\n");

    // Verify
    Console.WriteLine("Verifying stored values:");
    for (int i = 1; i <= 5; i++)
    {
        var value = await db.StringGetAsync($"multiplex_{i}");
        Console.WriteLine($"  multiplex_{i}: {value}");
    }

    Console.WriteLine("\nConnection Info:");
    Console.WriteLine($"  • Endpoints: {string.Join(", ", connection.GetEndPoints().Select(ep => ep.ToString()))}");
    Console.WriteLine($"  • Connected: {connection.IsConnected}");
    Console.WriteLine($"  • Multiplexing: All operations share this connection\n");

    connection.Dispose();
}

static async Task Demo5_PerformanceBenchmarksAsync(IMemoryCache memoryCache, IDistributedCache distributedCache)
{
    Console.WriteLine("5️⃣  PERFORMANCE BENCHMARKS\n");

    var iterations = 1000;
    var testData = new User(999, "Benchmark User", "bench@example.com");

    // Benchmark MemoryCache
    var sw = Stopwatch.StartNew();
    for (int i = 0; i < iterations; i++)
    {
        memoryCache.Set($"bench_memory_{i}", testData);
        _ = memoryCache.Get<User>($"bench_memory_{i}");
    }
    sw.Stop();
    var memoryTime = sw.ElapsedMilliseconds;

    // Benchmark DistributedCache
    sw.Restart();
    for (int i = 0; i < iterations; i++)
    {
        await distributedCache.SetStringAsync($"bench_redis_{i}", JsonSerializer.Serialize(testData));
        _ = await distributedCache.GetStringAsync($"bench_redis_{i}");
    }
    sw.Stop();
    var redisTime = sw.ElapsedMilliseconds;

    Console.WriteLine($"MemoryCache ({iterations} operations):");
    Console.WriteLine($"  Total: {memoryTime}ms");
    Console.WriteLine($"  Avg: {(double)memoryTime / iterations:F3}ms per operation");
    var memoryThroughput = memoryTime > 0 ? iterations * 1000 / memoryTime : iterations * 1000;
    Console.WriteLine($"  Throughput: ~{memoryThroughput:F0} ops/sec\n");

    Console.WriteLine($"DistributedCache/Redis ({iterations} operations):");
    Console.WriteLine($"  Total: {redisTime}ms");
    Console.WriteLine($"  Avg: {(double)redisTime / iterations:F3}ms per operation");
    var redisThroughput = redisTime > 0 ? iterations * 1000 / redisTime : iterations * 1000;
    Console.WriteLine($"  Throughput: ~{redisThroughput:F0} ops/sec\n");

    Console.WriteLine("Performance Summary:");
    var speedRatio = memoryTime > 0 && redisTime > 0 ? (double)redisTime / memoryTime : 0;
    if (speedRatio > 1)
    {
        Console.WriteLine($"  • MemoryCache is ~{speedRatio:F1}x faster (in-process)");
    }
    else
    {
        Console.WriteLine("  • MemoryCache is significantly faster (in-process)");
    }
    Console.WriteLine("  • Redis adds network latency (~1-2ms per operation)");
    Console.WriteLine("  • Trade-off: Speed vs Shared state across processes\n");
}

static async Task Demo6_SerializationStrategiesAsync(IDistributedCache cache)
{
    Console.WriteLine("6️⃣  SERIALIZATION STRATEGIES\n");

    var order = new Order(1001, 250.50m, new[] { "Item1", "Item2", "Item3" }, DateTime.UtcNow);

    // Strategy 1: JSON Serialization (human-readable)
    Console.WriteLine("Strategy 1: JSON Serialization");
    var jsonData = JsonSerializer.Serialize(order);
    await cache.SetStringAsync("order_json", jsonData);
    Console.WriteLine($"  ✓ Size: {jsonData.Length} bytes");
    Console.WriteLine($"  ✓ Format: {jsonData[..50]}...");
    Console.WriteLine("  ✓ Pros: Human-readable, cross-platform");
    Console.WriteLine("  ✓ Cons: Larger size, slower\n");

    // Strategy 2: Compact JSON (no formatting)
    Console.WriteLine("Strategy 2: Compact JSON (production)");
    var compactJson = JsonSerializer.Serialize(order, new JsonSerializerOptions { WriteIndented = false });
    await cache.SetStringAsync("order_compact", compactJson);
    Console.WriteLine($"  ✓ Size: {compactJson.Length} bytes (same as above - already compact)");
    Console.WriteLine("  ✓ Pros: Smaller than formatted JSON");
    Console.WriteLine("  ✓ Cons: Still verbose for large objects\n");

    // Retrieve and verify
    var retrieved = JsonSerializer.Deserialize<Order>(await cache.GetStringAsync("order_json") ?? "");
    Console.WriteLine($"Retrieved Order #{retrieved?.Id}: ${retrieved?.TotalAmount}\n");
}

static async Task Demo7_TTLAndExpirationAsync(IDistributedCache cache)
{
    Console.WriteLine("7️⃣  TTL AND EXPIRATION POLICIES\n");

    Console.WriteLine("Expiration Type 1: Absolute Expiration");
    await cache.SetStringAsync("absolute_exp", "Expires at exact time",
        new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(5)
        });
    Console.WriteLine("  ✓ Set with 5-second absolute TTL");
    Console.WriteLine("  ✓ Will expire exactly 5 seconds from now\n");

    Console.WriteLine("Expiration Type 2: Sliding Expiration");
    await cache.SetStringAsync("sliding_exp", "Resets on each access",
        new DistributedCacheEntryOptions
        {
            SlidingExpiration = TimeSpan.FromSeconds(3)
        });
    Console.WriteLine("  ✓ Set with 3-second sliding TTL");
    Console.WriteLine("  ✓ Resets TTL on each access");

    await Task.Delay(2000);
    _ = await cache.GetStringAsync("sliding_exp"); // Access resets TTL
    Console.WriteLine("  ✓ Accessed after 2s (TTL reset)");

    await Task.Delay(2000);
    var stillExists = await cache.GetStringAsync("sliding_exp");
    Console.WriteLine($"  ✓ Still exists after 4s total: {stillExists != null}\n");

    Console.WriteLine("Best Practices:");
    Console.WriteLine("  • Absolute: Fixed expiration (session timeout, temporary tokens)");
    Console.WriteLine("  • Sliding: Frequently accessed data (user preferences)");
    Console.WriteLine("  • Combine both: Max lifetime + sliding window\n");
}

// ============================================================================
// Helper Methods
// ============================================================================

static async Task<bool> IsRedisAvailableAsync(string connectionString)
{
    try
    {
        var connection = await ConnectionMultiplexer.ConnectAsync(connectionString + ",connectTimeout=2000,abortConnect=false");
        var isConnected = connection.IsConnected;
        connection.Dispose();
        return isConnected;
    }
    catch
    {
        return false;
    }
}

static User GetUserFromDatabase(int userId)
{
    Thread.Sleep(100); // Simulate database delay
    return new User(userId, $"User{userId}", $"user{userId}@example.com");
}

static async Task<Product> GetProductFromDatabaseAsync(int productId)
{
    await Task.Delay(100); // Simulate database delay
    return new Product(productId, $"Product {productId}", 99.99m * productId);
}

// ============================================================================
// Models
// ============================================================================

record User(int Id, string Name, string Email);

record Product(int Id, string Name, decimal Price);

record Order(int Id, decimal TotalAmount, string[] Items, DateTime CreatedAt);
