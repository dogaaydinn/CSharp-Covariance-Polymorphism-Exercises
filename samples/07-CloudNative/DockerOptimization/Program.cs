var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

app.MapGet("/", () => new
{
    message = "Docker Optimization Demo",
    imageSize = "Compare standard vs optimized Dockerfile",
    optimizations = new[]
    {
        "Multi-stage builds (separates build from runtime)",
        "Alpine base image (minimal Linux, 5MB vs 200MB)",
        "Layer caching (reorder commands for better cache hits)",
        "Trimmed publish (removes unused assemblies)",
        "Single-file publish (self-contained executable)"
    }
});

app.MapGet("/health", () => Results.Ok(new { status = "healthy" }));

app.Run();
