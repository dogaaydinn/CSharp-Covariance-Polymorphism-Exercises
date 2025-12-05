var builder = WebApplication.CreateBuilder(args);

// Configure YARP (Yet Another Reverse Proxy)
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

var app = builder.Build();

app.MapGet("/", () => new
{
    service = "MicroVideoPlatform API Gateway",
    version = "1.0.0",
    routes = new[]
    {
        "/api/videos/* → Content.API",
        "/api/analytics/* → Analytics.Function",
        "/health → Health check"
    }
});

app.MapGet("/health", () => Results.Ok(new { status = "healthy", service = "api-gateway" }));

// YARP routes defined in appsettings.json
app.MapReverseProxy();

app.Run();
