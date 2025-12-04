using Microsoft.Extensions.Caching.Memory;

namespace RateLimiting.Basic;

/// <summary>
/// Basic in-memory rate limiter using fixed window algorithm.
/// Best for: Single-server deployments, prototypes, low-traffic APIs
/// </summary>
public class BasicRateLimiter
{
    private readonly IMemoryCache _cache;
    private readonly int _requestLimit;
    private readonly TimeSpan _windowDuration;

    public BasicRateLimiter(IMemoryCache cache, int requestLimit = 100, TimeSpan? windowDuration = null)
    {
        _cache = cache;
        _requestLimit = requestLimit;
        _windowDuration = windowDuration ?? TimeSpan.FromHours(1);
    }

    public RateLimitResult CheckLimit(string clientId, string endpoint)
    {
        var key = $"ratelimit:{clientId}:{endpoint}";
        
        var counter = _cache.GetOrCreate(key, entry =>
        {
            entry.AbsoluteExpiration = DateTimeOffset.UtcNow.Add(_windowDuration);
            return new RateLimitCounter
            {
                Count = 0,
                WindowStart = DateTime.UtcNow,
                WindowEnd = DateTime.UtcNow.Add(_windowDuration)
            };
        });

        if (DateTime.UtcNow >= counter.WindowEnd)
        {
            counter.Count = 0;
            counter.WindowStart = DateTime.UtcNow;
            counter.WindowEnd = DateTime.UtcNow.Add(_windowDuration);
        }

        if (counter.Count >= _requestLimit)
        {
            return new RateLimitResult
            {
                IsAllowed = false,
                Remaining = 0,
                Limit = _requestLimit,
                ResetAt = counter.WindowEnd
            };
        }

        counter.Count++;
        _cache.Set(key, counter, counter.WindowEnd);

        return new RateLimitResult
        {
            IsAllowed = true,
            Remaining = _requestLimit - counter.Count,
            Limit = _requestLimit,
            ResetAt = counter.WindowEnd
        };
    }
}

public class RateLimitCounter
{
    public int Count { get; set; }
    public DateTime WindowStart { get; set; }
    public DateTime WindowEnd { get; set; }
}

public class RateLimitResult
{
    public bool IsAllowed { get; set; }
    public int Remaining { get; set; }
    public int Limit { get; set; }
    public DateTime ResetAt { get; set; }
}
