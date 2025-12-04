using StackExchange.Redis;

namespace RateLimiting.Advanced;

/// <summary>
/// Redis-based distributed rate limiter using sliding window log algorithm.
/// Best for: Multi-server deployments, production systems, accurate rate limiting
/// </summary>
public class AdvancedRateLimiter
{
    private readonly IConnectionMultiplexer _redis;
    private readonly Dictionary<string, TierConfig> _tierConfigs;

    private const string LuaScript = @"
        local key = KEYS[1]
        local limit = tonumber(ARGV[1])
        local window = tonumber(ARGV[2])
        local now = tonumber(ARGV[3])
        
        redis.call('ZREMRANGEBYSCORE', key, 0, now - window)
        local current = redis.call('ZCARD', key)
        
        if current < limit then
            redis.call('ZADD', key, now, now)
            redis.call('EXPIRE', key, window)
            return {1, limit - current - 1, limit}
        else
            return {0, 0, limit}
        end
    ";

    public AdvancedRateLimiter(IConnectionMultiplexer redis, Dictionary<string, TierConfig> tierConfigs)
    {
        _redis = redis;
        _tierConfigs = tierConfigs;
    }

    public async Task<RateLimitResult> CheckLimitAsync(string clientId, string tier, string endpoint)
    {
        try
        {
            var db = _redis.GetDatabase();
            var config = _tierConfigs.GetValueOrDefault(tier) ?? _tierConfigs["Free"];
            
            var key = $"ratelimit:{tier}:{clientId}:{endpoint}";
            var limit = config.RequestsPerHour;
            var window = 3600; // 1 hour
            var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

            var result = await db.ScriptEvaluateAsync(
                LuaScript,
                new RedisKey[] { key },
                new RedisValue[] { limit, window, now }
            );

            var resultArray = (RedisValue[])result;
            return new RateLimitResult
            {
                IsAllowed = (int)resultArray[0] == 1,
                Remaining = (int)resultArray[1],
                Limit = (int)resultArray[2],
                ResetAt = DateTime.UtcNow.AddSeconds(window)
            };
        }
        catch (RedisException)
        {
            // Fail-open: Allow request if Redis is down
            return new RateLimitResult
            {
                IsAllowed = true,
                Remaining = 999,
                Limit = 1000,
                ResetAt = DateTime.UtcNow.AddHours(1)
            };
        }
    }
}

public class TierConfig
{
    public int RequestsPerHour { get; set; }
}

public class RateLimitResult
{
    public bool IsAllowed { get; set; }
    public int Remaining { get; set; }
    public int Limit { get; set; }
    public DateTime ResetAt { get; set; }
}
