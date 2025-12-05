using StackExchange.Redis;
using System.Text.Json;

namespace RealWorldProblems.RateLimiting.Enterprise;

/// <summary>
/// Token Bucket Rate Limiter with Redis
/// Enterprise-grade solution for distributed systems
/// </summary>
public class TokenBucketRateLimiter
{
    private readonly IConnectionMultiplexer _redis;
    private readonly int _capacity;
    private readonly double _refillRate; // tokens per second
    private readonly string _luaScript;

    public TokenBucketRateLimiter(
        IConnectionMultiplexer redis,
        int capacity,
        double refillRate)
    {
        _redis = redis;
        _capacity = capacity;
        _refillRate = refillRate;
        _luaScript = LoadLuaScript();
    }

    public async Task<RateLimitResult> AllowRequestAsync(
        string userId,
        int tokensRequested = 1)
    {
        var db = _redis.GetDatabase();
        var key = $"rate_limit:token_bucket:{userId}";
        var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

        try
        {
            var result = await db.ScriptEvaluateAsync(
                _luaScript,
                new RedisKey[] { key },
                new RedisValue[]
                {
                    _capacity,
                    _refillRate,
                    tokensRequested,
                    now
                }
            );

            var values = (RedisValue[])result;
            var allowed = (int)values[0] == 1;
            var tokens = (double)values[1];
            var retryAfter = (int)values[2];

            return new RateLimitResult
            {
                IsAllowed = allowed,
                Limit = _capacity,
                Remaining = (int)Math.Floor(tokens),
                RetryAfterSeconds = retryAfter
            };
        }
        catch (RedisException ex)
        {
            // Fail-open: Allow request when Redis is unavailable
            Console.WriteLine($"Redis error: {ex.Message}. Allowing request (fail-open).");
            return new RateLimitResult
            {
                IsAllowed = true,
                Limit = _capacity,
                Remaining = _capacity,
                RetryAfterSeconds = 0
            };
        }
    }

    private string LoadLuaScript()
    {
        return @"
local key = KEYS[1]
local capacity = tonumber(ARGV[1])
local refill_rate = tonumber(ARGV[2])
local requested = tonumber(ARGV[3])
local now = tonumber(ARGV[4])

-- Get current bucket state
local bucket = redis.call('HMGET', key, 'tokens', 'last_refill')
local tokens = tonumber(bucket[1])
local last_refill = tonumber(bucket[2])

-- Initialize if first request
if tokens == nil then
    tokens = capacity
    last_refill = now
end

-- Calculate tokens to add since last refill
local elapsed = math.max(0, now - last_refill)
local tokens_to_add = elapsed * refill_rate
tokens = math.min(capacity, tokens + tokens_to_add)

-- Try to consume tokens
local allowed = 0
local retry_after = 0

if tokens >= requested then
    tokens = tokens - requested
    allowed = 1
else
    -- Calculate how long to wait for enough tokens
    local tokens_needed = requested - tokens
    retry_after = math.ceil(tokens_needed / refill_rate)
end

-- Update bucket state
redis.call('HSET', key, 'tokens', tokens, 'last_refill', now)
redis.call('EXPIRE', key, 3600)  -- Expire after 1 hour of inactivity

return {allowed, tokens, retry_after}
";
    }
}

/// <summary>
/// In-Memory Token Bucket (for local development/testing)
/// </summary>
public class InMemoryTokenBucketRateLimiter
{
    private class TokenBucket
    {
        public double Tokens { get; set; }
        public DateTime LastRefill { get; set; }
    }

    private readonly int _capacity;
    private readonly double _refillRate;
    private readonly Dictionary<string, TokenBucket> _buckets;
    private readonly object _lock = new();

    public InMemoryTokenBucketRateLimiter(int capacity, double refillRate)
    {
        _capacity = capacity;
        _refillRate = refillRate;
        _buckets = new Dictionary<string, TokenBucket>();
    }

    public RateLimitResult AllowRequest(string userId, int tokensRequested = 1)
    {
        lock (_lock)
        {
            var now = DateTime.UtcNow;

            if (!_buckets.TryGetValue(userId, out var bucket))
            {
                bucket = new TokenBucket
                {
                    Tokens = _capacity,
                    LastRefill = now
                };
                _buckets[userId] = bucket;
            }

            // Refill tokens
            var elapsed = (now - bucket.LastRefill).TotalSeconds;
            var tokensToAdd = elapsed * _refillRate;
            bucket.Tokens = Math.Min(_capacity, bucket.Tokens + tokensToAdd);
            bucket.LastRefill = now;

            // Try to consume tokens
            if (bucket.Tokens >= tokensRequested)
            {
                bucket.Tokens -= tokensRequested;
                return new RateLimitResult
                {
                    IsAllowed = true,
                    Limit = _capacity,
                    Remaining = (int)Math.Floor(bucket.Tokens),
                    RetryAfterSeconds = 0
                };
            }
            else
            {
                var tokensNeeded = tokensRequested - bucket.Tokens;
                var retryAfter = (int)Math.Ceiling(tokensNeeded / _refillRate);

                return new RateLimitResult
                {
                    IsAllowed = false,
                    Limit = _capacity,
                    Remaining = (int)Math.Floor(bucket.Tokens),
                    RetryAfterSeconds = retryAfter
                };
            }
        }
    }
}

/// <summary>
/// Cost-Based Rate Limiting
/// Different endpoints consume different number of tokens
/// </summary>
public class CostBasedRateLimiter
{
    private readonly TokenBucketRateLimiter _limiter;
    private readonly Dictionary<string, int> _endpointCosts;

    public CostBasedRateLimiter(
        TokenBucketRateLimiter limiter,
        Dictionary<string, int> endpointCosts)
    {
        _limiter = limiter;
        _endpointCosts = endpointCosts;
    }

    public async Task<RateLimitResult> AllowRequestAsync(
        string userId,
        string endpoint)
    {
        var cost = _endpointCosts.GetValueOrDefault(endpoint, 1);
        return await _limiter.AllowRequestAsync(userId, cost);
    }
}

/// <summary>
/// Multi-Tier Rate Limiting
/// Different limits for different user tiers
/// </summary>
public class TieredRateLimiter
{
    private readonly Dictionary<string, TokenBucketRateLimiter> _limiters;

    public TieredRateLimiter(IConnectionMultiplexer redis)
    {
        _limiters = new Dictionary<string, TokenBucketRateLimiter>
        {
            ["free"] = new TokenBucketRateLimiter(redis, capacity: 10, refillRate: 10.0 / 60),
            ["basic"] = new TokenBucketRateLimiter(redis, capacity: 100, refillRate: 100.0 / 60),
            ["premium"] = new TokenBucketRateLimiter(redis, capacity: 1000, refillRate: 1000.0 / 60),
            ["enterprise"] = new TokenBucketRateLimiter(redis, capacity: 10000, refillRate: 10000.0 / 60)
        };
    }

    public async Task<RateLimitResult> AllowRequestAsync(
        string userId,
        string tier = "free")
    {
        if (!_limiters.TryGetValue(tier, out var limiter))
        {
            limiter = _limiters["free"];
        }

        return await limiter.AllowRequestAsync(userId);
    }
}

public class RateLimitResult
{
    public bool IsAllowed { get; set; }
    public int Limit { get; set; }
    public int Remaining { get; set; }
    public int RetryAfterSeconds { get; set; }
}

/// <summary>
/// ASP.NET Core Middleware for Token Bucket Rate Limiting
/// </summary>
public class TokenBucketRateLimitMiddleware
{
    private readonly RequestDelegate _next;
    private readonly TokenBucketRateLimiter _limiter;

    public TokenBucketRateLimitMiddleware(
        RequestDelegate next,
        TokenBucketRateLimiter limiter)
    {
        _next = next;
        _limiter = limiter;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var userId = GetUserId(context);
        var result = await _limiter.AllowRequestAsync(userId);

        // Add rate limit headers
        context.Response.Headers["X-RateLimit-Limit"] = result.Limit.ToString();
        context.Response.Headers["X-RateLimit-Remaining"] = result.Remaining.ToString();
        context.Response.Headers["X-RateLimit-Algorithm"] = "token-bucket";

        if (!result.IsAllowed)
        {
            context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
            context.Response.Headers["Retry-After"] = result.RetryAfterSeconds.ToString();

            await context.Response.WriteAsJsonAsync(new
            {
                error = "Rate limit exceeded",
                message = $"You have exceeded your rate limit. Please try again in {result.RetryAfterSeconds} seconds.",
                limit = result.Limit,
                remaining = result.Remaining,
                retryAfterSeconds = result.RetryAfterSeconds,
                algorithm = "token-bucket"
            });

            return;
        }

        await _next(context);
    }

    private string GetUserId(HttpContext context)
    {
        var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();
        if (!string.IsNullOrEmpty(authHeader))
        {
            return authHeader;
        }

        var apiKey = context.Request.Headers["X-API-Key"].FirstOrDefault();
        if (!string.IsNullOrEmpty(apiKey))
        {
            return apiKey;
        }

        var ipAddress = context.Connection.RemoteIpAddress?.ToString();
        return ipAddress ?? "anonymous";
    }
}

/// <summary>
/// Extension methods for easy setup
/// </summary>
public static class TokenBucketRateLimiterExtensions
{
    public static IServiceCollection AddTokenBucketRateLimit(
        this IServiceCollection services,
        string redisConnectionString,
        int capacity = 100,
        double refillRate = 100.0 / 60.0)
    {
        var redis = ConnectionMultiplexer.Connect(redisConnectionString);
        services.AddSingleton<IConnectionMultiplexer>(redis);
        services.AddSingleton(new TokenBucketRateLimiter(redis, capacity, refillRate));
        return services;
    }

    public static IApplicationBuilder UseTokenBucketRateLimit(
        this IApplicationBuilder app)
    {
        return app.UseMiddleware<TokenBucketRateLimitMiddleware>();
    }
}

// Example usage in Program.cs:
// builder.Services.AddTokenBucketRateLimit(
//     redisConnectionString: "localhost:6379",
//     capacity: 100,
//     refillRate: 100.0 / 60.0  // 100 tokens per minute
// );
// app.UseTokenBucketRateLimit();
