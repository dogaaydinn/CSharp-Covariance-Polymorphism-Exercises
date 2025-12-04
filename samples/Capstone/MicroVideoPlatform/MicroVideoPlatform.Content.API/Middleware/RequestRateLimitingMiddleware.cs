using System.Collections.Concurrent;

namespace MicroVideoPlatform.Content.API.Middleware;

/// <summary>
/// Simple in-memory rate limiting middleware to protect against brute force and DDoS attacks.
/// In production, use Redis-based distributed rate limiting.
/// </summary>
public class RequestRateLimitingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestRateLimitingMiddleware> _logger;
    private readonly int _requestLimit;
    private readonly TimeSpan _timeWindow;

    // In-memory storage: IP -> (RequestCount, WindowStartTime)
    private static readonly ConcurrentDictionary<string, (int Count, DateTime WindowStart)> _requestCounts = new();

    public RequestRateLimitingMiddleware(
        RequestDelegate next,
        ILogger<RequestRateLimitingMiddleware> logger,
        IConfiguration configuration)
    {
        _next = next;
        _logger = logger;
        _requestLimit = configuration.GetValue<int>("RateLimiting:RequestLimit", 100);
        _timeWindow = TimeSpan.FromSeconds(configuration.GetValue<int>("RateLimiting:TimeWindowSeconds", 60));
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var clientIp = GetClientIpAddress(context);

        // Get or create request count for this IP
        var now = DateTime.UtcNow;
        var (count, windowStart) = _requestCounts.GetOrAdd(clientIp, _ => (0, now));

        // Reset counter if time window has passed
        if (now - windowStart > _timeWindow)
        {
            _requestCounts[clientIp] = (1, now);
            await _next(context);
            return;
        }

        // Check if limit exceeded
        if (count >= _requestLimit)
        {
            _logger.LogWarning("Rate limit exceeded for IP {ClientIp}. Requests: {Count}/{Limit}",
                clientIp, count, _requestLimit);

            context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
            context.Response.Headers["Retry-After"] = ((int)(_timeWindow - (now - windowStart)).TotalSeconds).ToString();

            await context.Response.WriteAsJsonAsync(new
            {
                error = "Too many requests",
                message = $"Rate limit of {_requestLimit} requests per {_timeWindow.TotalSeconds} seconds exceeded",
                retryAfter = (int)(_timeWindow - (now - windowStart)).TotalSeconds
            });

            return;
        }

        // Increment counter
        _requestCounts[clientIp] = (count + 1, windowStart);

        await _next(context);
    }

    private static string GetClientIpAddress(HttpContext context)
    {
        // Check for X-Forwarded-For header (proxy/load balancer)
        var forwardedFor = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
        if (!string.IsNullOrEmpty(forwardedFor))
        {
            return forwardedFor.Split(',')[0].Trim();
        }

        // Check for X-Real-IP header
        var realIp = context.Request.Headers["X-Real-IP"].FirstOrDefault();
        if (!string.IsNullOrEmpty(realIp))
        {
            return realIp;
        }

        // Fall back to RemoteIpAddress
        return context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
    }

    /// <summary>
    /// Background task to clean up old entries (prevent memory leak)
    /// </summary>
    public static Task CleanupOldEntries(TimeSpan maxAge)
    {
        var cutoff = DateTime.UtcNow - maxAge;
        var keysToRemove = _requestCounts
            .Where(kvp => kvp.Value.WindowStart < cutoff)
            .Select(kvp => kvp.Key)
            .ToList();

        foreach (var key in keysToRemove)
        {
            _requestCounts.TryRemove(key, out _);
        }

        return Task.CompletedTask;
    }
}

public static class RequestRateLimitingMiddlewareExtensions
{
    public static IApplicationBuilder UseRequestRateLimiting(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<RequestRateLimitingMiddleware>();
    }
}
