using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using VideoService.API.Data;
using VideoService.API.Models;
using VideoService.API.Services;
using VideoService.ServiceDefaults;

var builder = WebApplication.CreateBuilder(args);

// Add Aspire service defaults (OpenTelemetry, health checks, service discovery)
builder.AddServiceDefaults();

// Add PostgreSQL with Entity Framework Core
builder.AddNpgsqlDbContext<VideoDbContext>("videodb");

// Add Redis for caching
builder.AddRedisClient("cache");

// Add VideoProcessingClient with service discovery
// The "http://videoprocessing" URL will be resolved via service discovery
builder.Services.AddHttpClient<VideoProcessingClient>(client =>
{
    client.BaseAddress = new Uri("http://videoprocessing");
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Apply migrations automatically
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<VideoDbContext>();
    await db.Database.EnsureCreatedAsync();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Map health checks
app.MapDefaultEndpoints();

// Video API Endpoints
app.MapGet("/api/videos", async (VideoDbContext db, IDatabase cache) =>
{
    // Try to get from cache first
    var cachedVideos = await cache.StringGetAsync("videos:all");
    if (!cachedVideos.IsNullOrEmpty)
    {
        return Results.Ok(System.Text.Json.JsonSerializer.Deserialize<List<Video>>(cachedVideos!));
    }

    var videos = await db.Videos.ToListAsync();

    // Cache for 5 minutes
    await cache.StringSetAsync("videos:all",
        System.Text.Json.JsonSerializer.Serialize(videos),
        TimeSpan.FromMinutes(5));

    return Results.Ok(videos);
})
.WithName("GetVideos")
.WithOpenApi();

app.MapGet("/api/videos/{id}", async (int id, VideoDbContext db, IDatabase cache) =>
{
    var cacheKey = $"video:{id}";
    var cached = await cache.StringGetAsync(cacheKey);

    if (!cached.IsNullOrEmpty)
    {
        return Results.Ok(System.Text.Json.JsonSerializer.Deserialize<Video>(cached!));
    }

    var video = await db.Videos.FindAsync(id);
    if (video == null) return Results.NotFound();

    await cache.StringSetAsync(cacheKey,
        System.Text.Json.JsonSerializer.Serialize(video),
        TimeSpan.FromMinutes(10));

    return Results.Ok(video);
})
.WithName("GetVideo")
.WithOpenApi();

app.MapPost("/api/videos", async (Video video, VideoDbContext db, IDatabase cache, VideoProcessingClient processingClient) =>
{
    db.Videos.Add(video);
    await db.SaveChangesAsync();

    // Invalidate cache
    await cache.KeyDeleteAsync("videos:all");

    // Trigger async processing via service discovery
    _ = Task.Run(async () =>
    {
        await Task.Delay(1000); // Simulate delay
        await processingClient.ProcessVideoAsync(video.Id);
    });

    return Results.Created($"/api/videos/{video.Id}", video);
})
.WithName("CreateVideo")
.WithOpenApi();

app.MapPost("/api/videos/{id}/view", async (int id, VideoDbContext db, IDatabase cache) =>
{
    var video = await db.Videos.FindAsync(id);
    if (video == null) return Results.NotFound();

    video.ViewCount++;
    await db.SaveChangesAsync();

    // Invalidate cache
    await cache.KeyDeleteAsync($"video:{id}");
    await cache.KeyDeleteAsync("videos:all");

    return Results.Ok(video);
})
.WithName("IncrementViewCount")
.WithOpenApi();

// Processing service endpoints (simulated)
app.MapPost("/api/process/{id}", async (int id, VideoDbContext db) =>
{
    var video = await db.Videos.FindAsync(id);
    if (video == null) return Results.NotFound();

    video.Status = VideoStatus.Processing;
    await db.SaveChangesAsync();

    // Simulate processing
    _ = Task.Run(async () =>
    {
        await Task.Delay(5000);
        video.Status = VideoStatus.Ready;
        await db.SaveChangesAsync();
    });

    return Results.Accepted();
})
.WithName("ProcessVideo")
.WithOpenApi();

app.MapGet("/api/status/{id}", async (int id, VideoDbContext db) =>
{
    var video = await db.Videos.FindAsync(id);
    return video == null ? Results.NotFound() : Results.Ok(video.Status);
})
.WithName("GetProcessingStatus")
.WithOpenApi();

app.Run();
