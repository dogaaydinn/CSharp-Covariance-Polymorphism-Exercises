using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using HealthChecks.UI.Client;

var builder = WebApplication.CreateBuilder(args);

// Configure health checks
builder.Services.AddHealthChecks()
    .AddCheck("self", () => Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Healthy())
    .AddCheck("database", () =>
    {
        // Simulate database check
        var isHealthy = DateTime.UtcNow.Second % 10 != 0; // 90% healthy
        return isHealthy
            ? Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Healthy("Database is responsive")
            : Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Unhealthy("Database connection failed");
    })
    .AddCheck("readiness", () =>
    {
        // Readiness check - is the app ready to serve traffic?
        var isReady = DateTime.UtcNow > DateTime.UtcNow.Date.AddSeconds(5); // Ready after 5 seconds
        return isReady
            ? Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Healthy("Ready to accept traffic")
            : Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Degraded("Still initializing");
    });

var app = builder.Build();

// Liveness probe - is the app alive?
app.MapHealthChecks("/health/live", new HealthCheckOptions
{
    Predicate = check => check.Name == "self"
});

// Readiness probe - is the app ready to serve traffic?
app.MapHealthChecks("/health/ready", new HealthCheckOptions
{
    Predicate = check => check.Name == "readiness" || check.Name == "database"
});

// Detailed health check with JSON response
app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.MapGet("/", () => new
{
    service = "KubernetesReadyApi",
    status = "Running",
    endpoints = new[]
    {
        "/health/live - Liveness probe (for Kubernetes)",
        "/health/ready - Readiness probe (for Kubernetes)",
        "/health - Detailed health status"
    }
});

app.Run();
