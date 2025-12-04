namespace MicroVideoPlatform.Content.API.Middleware;

/// <summary>
/// Middleware that adds security headers to every response to protect against common web vulnerabilities.
/// This implements defense-in-depth strategy by adding multiple layers of security.
/// </summary>
public class SecurityHeadersMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<SecurityHeadersMiddleware> _logger;

    public SecurityHeadersMiddleware(RequestDelegate next, ILogger<SecurityHeadersMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Content Security Policy (CSP)
        // Prevents XSS attacks by controlling which resources can be loaded
        // 'self' = only resources from same origin
        // 'unsafe-inline' = allows inline scripts (needed for Swagger in dev)
        context.Response.Headers["Content-Security-Policy"] =
            "default-src 'self'; " +
            "script-src 'self' 'unsafe-inline' 'unsafe-eval'; " +
            "style-src 'self' 'unsafe-inline'; " +
            "img-src 'self' data: https:; " +
            "font-src 'self' data:; " +
            "connect-src 'self'; " +
            "frame-ancestors 'none'";

        // X-Content-Type-Options
        // Prevents MIME type sniffing attacks
        // Forces browser to respect declared Content-Type
        context.Response.Headers["X-Content-Type-Options"] = "nosniff";

        // X-Frame-Options
        // Prevents clickjacking attacks by not allowing page to be framed
        context.Response.Headers["X-Frame-Options"] = "DENY";

        // X-XSS-Protection
        // Enables browser's built-in XSS filter
        // Mode=block stops page rendering if XSS detected
        context.Response.Headers["X-XSS-Protection"] = "1; mode=block";

        // Referrer-Policy
        // Controls how much referrer information is included with requests
        // 'no-referrer-when-downgrade' = full URL on HTTPS, no referrer on HTTP
        context.Response.Headers["Referrer-Policy"] = "no-referrer-when-downgrade";

        // Permissions-Policy (formerly Feature-Policy)
        // Restricts which browser features can be used
        // Prevents malicious code from accessing camera, microphone, etc.
        context.Response.Headers["Permissions-Policy"] =
            "camera=(), " +
            "microphone=(), " +
            "geolocation=(), " +
            "payment=()";

        // X-Permitted-Cross-Domain-Policies
        // Prevents Adobe Flash and PDF from loading data from this domain
        context.Response.Headers["X-Permitted-Cross-Domain-Policies"] = "none";

        // Remove Server header (security through obscurity)
        // Hides ASP.NET Core version information
        context.Response.Headers.Remove("Server");
        context.Response.Headers.Remove("X-Powered-By");

        _logger.LogDebug("Security headers applied to {Path}", context.Request.Path);

        await _next(context);
    }
}

/// <summary>
/// Extension method to easily add SecurityHeadersMiddleware to the pipeline
/// </summary>
public static class SecurityHeadersMiddlewareExtensions
{
    public static IApplicationBuilder UseSecurityHeaders(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<SecurityHeadersMiddleware>();
    }
}
