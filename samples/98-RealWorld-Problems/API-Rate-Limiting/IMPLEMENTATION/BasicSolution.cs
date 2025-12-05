using System.Collections.Concurrent;

namespace RealWorldProblems.RateLimiting.Basic;

/// <summary>
/// Fixed Window Rate Limiter
/// Simple but has burst problem at window boundaries
/// </summary>
public class FixedWindowRateLimiter
{
    private readonly int _limit;
    private readonly int _windowSeconds;
    private readonly ConcurrentDictionary<string, (int count, DateTime windowStart)> _cache;
    private readonly Timer _cleanupTimer;

    public FixedWindowRateLimiter(int limit, int windowSeconds = 60)
    {
        _limit = limit;
        _windowSeconds = windowSeconds;
        _cache = new ConcurrentDictionary<string, (int, DateTime)>();

        // Cleanup old windows every minute
        _cleanupTimer = new Timer(
            _ => CleanupOldWindows(),
            null,
            TimeSpan.FromMinutes(1),
            TimeSpan.FromMinutes(1)
        );
    }

    public bool AllowRequest(string userId)
    {
        var key = GetWindowKey(userId);

        var data = _cache.AddOrUpdate(
            key,
            k => (count: 1, windowStart: DateTime.UtcNow),
            (k, existing) =>
            {
                // Check if still in same window
                var elapsed = (DateTime.UtcNow - existing.windowStart).TotalSeconds;
                if (elapsed >= _windowSeconds)
                {
                    // New window
                    return (count: 1, windowStart: DateTime.UtcNow);
                }
                else
                {
                    // Same window, increment
                    return (count: existing.count + 1, existing.windowStart);
                }
            }
        );

        return data.count <= _limit;
    }

    public RateLimitInfo GetInfo(string userId)
    {
        var key = GetWindowKey(userId);

        if (_cache.TryGetValue(key, out var data))
        {
            var elapsed = (DateTime.UtcNow - data.windowStart).TotalSeconds;
            var remaining = Math.Max(0, _limit - data.count);
            var resetIn = Math.Max(0, _windowSeconds - (int)elapsed);

            return new RateLimitInfo
            {
                Limit = _limit,
                Remaining = remaining,
                ResetInSeconds = resetIn,
                IsAllowed = data.count <= _limit
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

    private string GetWindowKey(string userId)
    {
        var now = DateTime.UtcNow;
        var windowId = new DateTime(
            now.Year, now.Month, now.Day,
            now.Hour, now.Minute, 0
        ).ToString("yyyyMMddHHmmss");

        return $"{userId}:{windowId}";
    }

    private void CleanupOldWindows()
    {
        var cutoff = DateTime.UtcNow.AddSeconds(-_windowSeconds * 2);
        var keysToRemove = _cache
            .Where(kvp => kvp.Value.windowStart < cutoff)
            .Select(kvp => kvp.Key)
            .ToList();

        foreach (var key in keysToRemove)
        {
            _cache.TryRemove(key, out _);
        }

        Console.WriteLine($"[Cleanup] Removed {keysToRemove.Count} old windows. Active windows: {_cache.Count}");
    }

    public void Dispose()
    {
        _cleanupTimer?.Dispose();
    }
}

/// <summary>
/// ASP.NET Core Middleware for Fixed Window Rate Limiting
/// </summary>
public class FixedWindowRateLimitMiddleware
{
    private readonly RequestDelegate _next;
    private readonly FixedWindowRateLimiter _limiter;

    public FixedWindowRateLimitMiddleware(
        RequestDelegate next,
        FixedWindowRateLimiter limiter)
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
                resetInSeconds = info.ResetInSeconds
            });

            return;
        }

        await _next(context);
    }

    private string GetUserId(HttpContext context)
    {
        // Try to get from Authorization header
        var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();
        if (!string.IsNullOrEmpty(authHeader))
        {
            return authHeader;
        }

        // Fallback to IP address
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
public static class FixedWindowRateLimiterExtensions
{
    public static IServiceCollection AddFixedWindowRateLimit(
        this IServiceCollection services,
        int limit = 10,
        int windowSeconds = 60)
    {
        services.AddSingleton(new FixedWindowRateLimiter(limit, windowSeconds));
        return services;
    }

    public static IApplicationBuilder UseFixedWindowRateLimit(
        this IApplicationBuilder app)
    {
        return app.UseMiddleware<FixedWindowRateLimitMiddleware>();
    }
}

// Example usage in Program.cs:
// builder.Services.AddFixedWindowRateLimit(limit: 10, windowSeconds: 60);
// app.UseFixedWindowRateLimit();
