using StackExchange.Redis;

namespace RateLimiting.Enterprise;

/// <summary>
/// Enterprise-grade distributed rate limiter using Token Bucket algorithm.
/// Features: Burst handling, multi-tier, analytics, hybrid caching
/// Best for: High-traffic systems, mission-critical APIs, SaaS platforms
/// </summary>
public class DistributedRateLimiter
{
    private readonly IConnectionMultiplexer _redis;
    private readonly IMemoryCache _localCache;

    private const string TokenBucketLuaScript = @"
        local key = KEYS[1]
        local capacity = tonumber(ARGV[1])
        local refill_rate = tonumber(ARGV[2])
        local requested = tonumber(ARGV[3])
        local now = tonumber(ARGV[4])
        
        local bucket = redis.call('HMGET', key, 'tokens', 'last_refill')
        local tokens = tonumber(bucket[1])
        local last_refill = tonumber(bucket[2])
        
        if tokens == nil then
            tokens = capacity
            last_refill = now
        end
        
        local elapsed = now - last_refill
        local refill_tokens = elapsed * refill_rate
        tokens = math.min(capacity, tokens + refill_tokens)
        
        local allowed = 0
        if tokens >= requested then
            tokens = tokens - requested
            allowed = 1
        end
        
        redis.call('HMSET', key, 'tokens', tokens, 'last_refill', now)
        redis.call('EXPIRE', key, 3600)
        
        return {allowed, math.floor(tokens), capacity}
    ";

    public DistributedRateLimiter(IConnectionMultiplexer redis, IMemoryCache localCache)
    {
        _redis = redis;
        _localCache = localCache;
    }

    public async Task<RateLimitResult> CheckLimitAsync(
        string clientId,
        TierConfig tier,
        int requestTokens = 1)
    {
        // L1 Cache: Check local memory first (fast path)
        var localKey = $"local:{clientId}";
        if (_localCache.TryGetValue(localKey, out TokenBucketCache cache))
        {
            if (cache.Count < cache.Threshold)
            {
                cache.Count++;
                return new RateLimitResult
                {
                    IsAllowed = true,
                    Remaining = tier.BucketCapacity - cache.Count,
                    Limit = tier.BucketCapacity
                };
            }
        }

        // L2 Cache: Check Redis (distributed)
        var db = _redis.GetDatabase();
        var key = $"tokenbucket:{clientId}";
        var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

        var result = await db.ScriptEvaluateAsync(
            TokenBucketLuaScript,
            new RedisKey[] { key },
            new RedisValue[]
            {
                tier.BucketCapacity,
                tier.RefillRatePerSecond,
                requestTokens,
                now
            });

        var resultArray = (RedisValue[])result;
        var isAllowed = (int)resultArray[0] == 1;

        // Update local cache
        _localCache.Set(localKey, new TokenBucketCache
        {
            Count = 0,
            Threshold = 10 // Allow 10 requests before checking Redis again
        }, TimeSpan.FromMinutes(1));

        return new RateLimitResult
        {
            IsAllowed = isAllowed,
            Remaining = (int)resultArray[1],
            Limit = (int)resultArray[2],
            ResetAt = DateTime.UtcNow.AddHours(1)
        };
    }
}

public class TierConfig
{
    public string Name { get; set; }
    public int BucketCapacity { get; set; }
    public double RefillRatePerSecond { get; set; }
}

public class TokenBucketCache
{
    public int Count { get; set; }
    public int Threshold { get; set; }
}

public class RateLimitResult
{
    public bool IsAllowed { get; set; }
    public int Remaining { get; set; }
    public int Limit { get; set; }
    public DateTime ResetAt { get; set; }
}

// Example Tier Configuration
public static class TierConfigurations
{
    public static readonly Dictionary<string, TierConfig> Tiers = new()
    {
        ["Free"] = new()
        {
            Name = "Free",
            BucketCapacity = 100,
            RefillRatePerSecond = 0.028 // ~100 requests/hour
        },
        ["Premium"] = new()
        {
            Name = "Premium",
            BucketCapacity = 1000,
            RefillRatePerSecond = 2.78 // ~10,000 requests/hour
        },
        ["Enterprise"] = new()
        {
            Name = "Enterprise",
            BucketCapacity = 10000,
            RefillRatePerSecond = 27.8 // ~100,000 requests/hour
        }
    };
}
