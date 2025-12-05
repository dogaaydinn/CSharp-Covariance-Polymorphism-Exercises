using Content.API.Application.Commands;
using Content.API.Application.Queries;
using Content.API.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));
builder.Services.AddDbContext<VideoDbContext>(options =>
    options.UseInMemoryDatabase("VideosDb"));
builder.Services.AddSingleton<IEventBus, InMemoryEventBus>();

var app = builder.Build();

app.MapGet("/", () => new
{
    service = "Content.API",
    version = "1.0.0",
    architecture = "Clean Architecture + CQRS",
    endpoints = new[]
    {
        "GET /api/videos - List all videos",
        "GET /api/videos/{id} - Get video by ID",
        "POST /api/videos - Upload new video",
        "GET /health - Health check"
    }
});

// Commands (Write operations)
app.MapPost("/api/videos", async (UploadVideoRequest request, IMediator mediator) =>
{
    var command = new UploadVideoCommand(request.Title, request.Url);
    var videoId = await mediator.Send(command);
    return Results.Created($"/api/videos/{videoId}", new { id = videoId });
});

// Queries (Read operations)
app.MapGet("/api/videos", async (IMediator mediator) =>
{
    var query = new GetAllVideosQuery();
    var videos = await mediator.Send(query);
    return Results.Ok(videos);
});

app.MapGet("/api/videos/{id:guid}", async (Guid id, IMediator mediator) =>
{
    var query = new GetVideoByIdQuery(id);
    var video = await mediator.Send(query);
    return video != null ? Results.Ok(video) : Results.NotFound();
});

app.MapGet("/health", () => Results.Ok(new { status = "healthy", service = "content-api" }));

app.Run();

public record UploadVideoRequest(string Title, string Url);
