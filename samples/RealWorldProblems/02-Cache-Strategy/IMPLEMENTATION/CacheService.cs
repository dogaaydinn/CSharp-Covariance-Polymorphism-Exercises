using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Caching.Distributed;

namespace CacheStrategy;

/// <summary>
/// Multi-level cache service implementing L1 (Memory) + L2 (Redis) caching
/// </summary>
public class MultiLevelCacheService
{
    private readonly IMemoryCache _l1Cache;
    private readonly IDistributedCache _l2Cache;
    private readonly ILogger<MultiLevelCacheService> _logger;

    public async Task<T> GetOrCreateAsync<T>(
        string key,
        Func<Task<T>> factory,
        CacheOptions options = null)
    {
        options ??= CacheOptions.Default;

        // L1: Memory Cache (fastest)
        if (_l1Cache.TryGetValue(key, out T l1Value))
        {
            _logger.LogDebug("L1 Cache HIT: {Key}", key);
            return l1Value;
        }

        // L2: Distributed Cache (Redis)
        var l2Data = await _l2Cache.GetStringAsync(key);
        if (l2Data != null)
        {
            _logger.LogDebug("L2 Cache HIT: {Key}", key);
            var l2Value = JsonSerializer.Deserialize<T>(l2Data);
            
            // Populate L1
            _l1Cache.Set(key, l2Value, options.L1Duration);
            return l2Value;
        }

        // Cache MISS - Execute factory
        _logger.LogWarning("Cache MISS: {Key}", key);
        var value = await factory();

        // Store in both caches
        await SetAsync(key, value, options);

        return value;
    }

    private async Task SetAsync<T>(string key, T value, CacheOptions options)
    {
        // L1: Memory
        _l1Cache.Set(key, value, options.L1Duration);

        // L2: Redis
        var serialized = JsonSerializer.Serialize(value);
        await _l2Cache.SetStringAsync(key, serialized, new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = options.L2Duration
        });
    }

    public async Task InvalidateAsync(string key)
    {
        _l1Cache.Remove(key);
        await _l2Cache.RemoveAsync(key);
        _logger.LogInformation("Cache invalidated: {Key}", key);
    }
}

public class CacheOptions
{
    public TimeSpan L1Duration { get; set; } = TimeSpan.FromMinutes(1);
    public TimeSpan L2Duration { get; set; } = TimeSpan.FromMinutes(5);

    public static CacheOptions Default => new();
    public static CacheOptions ShortLived => new() 
    { 
        L1Duration = TimeSpan.FromSeconds(30),
        L2Duration = TimeSpan.FromMinutes(2)
    };
    public static CacheOptions LongLived => new() 
    { 
        L1Duration = TimeSpan.FromMinutes(5),
        L2Duration = TimeSpan.FromHours(1)
    };
}
