using System.Collections.Concurrent;

namespace RealWorldProblems.RateLimiting.Advanced;

/// <summary>
/// Sliding Window Rate Limiter
/// More accurate than Fixed Window, prevents burst at boundaries
/// </summary>
public class SlidingWindowRateLimiter
{
    private readonly int _limit;
    private readonly int _windowSeconds;
    private readonly ConcurrentDictionary<string, List<DateTime>> _requests;
    private readonly Timer _cleanupTimer;
    private readonly object _lock = new();

    public SlidingWindowRateLimiter(int limit, int windowSeconds = 60)
    {
        _limit = limit;
        _windowSeconds = windowSeconds;
        _requests = new ConcurrentDictionary<string, List<DateTime>>();

        // Cleanup inactive users every 5 minutes
        _cleanupTimer = new Timer(
            _ => CleanupInactiveUsers(),
            null,
            TimeSpan.FromMinutes(5),
            TimeSpan.FromMinutes(5)
        );
    }

    public bool AllowRequest(string userId)
    {
        var now = DateTime.UtcNow;
        var windowStart = now.AddSeconds(-_windowSeconds);

        var timestamps = _requests.AddOrUpdate(
            userId,
            new List<DateTime> { now },
            (key, existing) =>
            {
                lock (existing)
                {
                    // Remove old requests outside window
                    existing.RemoveAll(ts => ts < windowStart);

                    // Check if we can add new request
                    if (existing.Count < _limit)
                    {
                        existing.Add(now);
                    }

                    return existing;
                }
            }
        );

        lock (timestamps)
        {
            return timestamps.Count <= _limit;
        }
    }

    public RateLimitInfo GetInfo(string userId)
    {
        var now = DateTime.UtcNow;
        var windowStart = now.AddSeconds(-_windowSeconds);

        if (_requests.TryGetValue(userId, out var timestamps))
        {
            lock (timestamps)
            {
                // Count requests in current window
                var recentRequests = timestamps.Count(ts => ts >= windowStart);
                var remaining = Math.Max(0, _limit - recentRequests);

                // Calculate when oldest request will expire
                var oldestInWindow = timestamps.FirstOrDefault(ts => ts >= windowStart);
                var resetInSeconds = oldestInWindow != DateTime.MinValue
                    ? (int)(oldestInWindow.AddSeconds(_windowSeconds) - now).TotalSeconds
                    : _windowSeconds;

                return new RateLimitInfo
                {
                    Limit = _limit,
                    Remaining = remaining,
                    ResetInSeconds = Math.Max(0, resetInSeconds),
                    IsAllowed = recentRequests < _limit
                };
            }
        }

        return new RateLimitInfo
        {
            Limit = _limit,
            Remaining = _limit,
            ResetInSeconds = 0,
            IsAllowed = true
        };
    }

    private void CleanupInactiveUsers()
    {
        var cutoff = DateTime.UtcNow.AddMinutes(-5);
        var usersToRemove = new List<string>();

        foreach (var kvp in _requests)
        {
            lock (kvp.Value)
            {
                // If user has no recent requests, remove
                if (kvp.Value.All(ts => ts < cutoff))
                {
                    usersToRemove.Add(kvp.Key);
                }
            }
        }

        foreach (var userId in usersToRemove)
        {
            _requests.TryRemove(userId, out _);
        }

        Console.WriteLine($"[Cleanup] Removed {usersToRemove.Count} inactive users. Active users: {_requests.Count}");
    }

    public void Dispose()
    {
        _cleanupTimer?.Dispose();
    }
}

/// <summary>
/// Optimized Sliding Window with Weighted Calculation
/// Uses previous + current window for better performance
/// </summary>
public class WeightedSlidingWindowRateLimiter
{
    private readonly int _limit;
    private readonly int _windowSeconds;
    private readonly ConcurrentDictionary<string, WindowData> _cache;

    private class WindowData
    {
        public int PreviousCount { get; set; }
        public int CurrentCount { get; set; }
        public DateTime WindowStart { get; set; }
    }

    public WeightedSlidingWindowRateLimiter(int limit, int windowSeconds = 60)
    {
        _limit = limit;
        _windowSeconds = windowSeconds;
        _cache = new ConcurrentDictionary<string, WindowData>();
    }

    public bool AllowRequest(string userId)
    {
        var now = DateTime.UtcNow;
        var windowStart = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, 0);

        var data = _cache.AddOrUpdate(
            userId,
            k => new WindowData
            {
                PreviousCount = 0,
                CurrentCount = 1,
                WindowStart = windowStart
            },
            (k, existing) =>
            {
                if (windowStart > existing.WindowStart)
                {
                    // New window started
                    return new WindowData
                    {
                        PreviousCount = existing.CurrentCount,
                        CurrentCount = 1,
                        WindowStart = windowStart
                    };
                }
                else
                {
                    // Same window
                    existing.CurrentCount++;
                    return existing;
                }
            }
        );

        // Weighted calculation
        var elapsed = (now - data.WindowStart).TotalSeconds;
        var weight = 1.0 - (elapsed / _windowSeconds);
        var estimatedCount = data.PreviousCount * weight + data.CurrentCount;

        return estimatedCount <= _limit;
    }

    public RateLimitInfo GetInfo(string userId)
    {
        var now = DateTime.UtcNow;
        var windowStart = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, 0);

        if (_cache.TryGetValue(userId, out var data))
        {
            var elapsed = (now - data.WindowStart).TotalSeconds;
            var weight = 1.0 - (elapsed / _windowSeconds);
            var estimatedCount = data.PreviousCount * weight + data.CurrentCount;
            var remaining = Math.Max(0, (int)(_limit - estimatedCount));

            return new RateLimitInfo
            {
                Limit = _limit,
                Remaining = remaining,
                ResetInSeconds = (int)(_windowSeconds - elapsed),
                IsAllowed = estimatedCount < _limit
            };
        }

        return new RateLimitInfo
        {
            Limit = _limit,
            Remaining = _limit,
            ResetInSeconds = _windowSeconds,
            IsAllowed = true
        };
    }
}

/// <summary>
/// ASP.NET Core Middleware for Sliding Window Rate Limiting
/// </summary>
public class SlidingWindowRateLimitMiddleware
{
    private readonly RequestDelegate _next;
    private readonly SlidingWindowRateLimiter _limiter;

    public SlidingWindowRateLimitMiddleware(
        RequestDelegate next,
        SlidingWindowRateLimiter limiter)
    {
        _next = next;
        _limiter = limiter;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var userId = GetUserId(context);
        var info = _limiter.GetInfo(userId);

        // Add rate limit headers
        context.Response.Headers["X-RateLimit-Limit"] = info.Limit.ToString();
        context.Response.Headers["X-RateLimit-Remaining"] = info.Remaining.ToString();
        context.Response.Headers["X-RateLimit-Reset"] = info.ResetInSeconds.ToString();

        if (!_limiter.AllowRequest(userId))
        {
            context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
            context.Response.Headers["Retry-After"] = info.ResetInSeconds.ToString();

            await context.Response.WriteAsJsonAsync(new
            {
                error = "Rate limit exceeded",
                message = $"You have exceeded your rate limit. Please try again in {info.ResetInSeconds} seconds.",
                limit = info.Limit,
                remaining = 0,
                resetInSeconds = info.ResetInSeconds,
                algorithm = "sliding-window"
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

        var ipAddress = context.Connection.RemoteIpAddress?.ToString();
        return ipAddress ?? "anonymous";
    }
}

public class RateLimitInfo
{
    public int Limit { get; set; }
    public int Remaining { get; set; }
    public int ResetInSeconds { get; set; }
    public bool IsAllowed { get; set; }
}

/// <summary>
/// Extension methods for easy setup
/// </summary>
public static class SlidingWindowRateLimiterExtensions
{
    public static IServiceCollection AddSlidingWindowRateLimit(
        this IServiceCollection services,
        int limit = 10,
        int windowSeconds = 60)
    {
        services.AddSingleton(new SlidingWindowRateLimiter(limit, windowSeconds));
        return services;
    }

    public static IApplicationBuilder UseSlidingWindowRateLimit(
        this IApplicationBuilder app)
    {
        return app.UseMiddleware<SlidingWindowRateLimitMiddleware>();
    }
}

// Example usage in Program.cs:
// builder.Services.AddSlidingWindowRateLimit(limit: 10, windowSeconds: 60);
// app.UseSlidingWindowRateLimit();
