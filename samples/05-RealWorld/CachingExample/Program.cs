using Microsoft.Extensions.Caching.Memory;

var cache = new MemoryCache(new MemoryCacheOptions());

Console.WriteLine("=== Caching Example ===\n");

// Cache-aside pattern
var userId = 42;
var user = GetUserWithCache(cache, userId);
Console.WriteLine($"First call: {user.Name} (from database)");

user = GetUserWithCache(cache, userId);
Console.WriteLine($"Second call: {user.Name} (from cache!)");

// Invalidate cache
cache.Remove($"user_{userId}");
user = GetUserWithCache(cache, userId);
Console.WriteLine($"After invalidation: {user.Name} (from database again)");

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

static User GetUserFromDatabase(int userId)
{
    Thread.Sleep(100);  // Simulate database delay
    return new User(userId, $"User{userId}", $"user{userId}@example.com");
}

record User(int Id, string Name, string Email);
