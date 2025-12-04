var builder = DistributedApplication.CreateBuilder(args);

// Add infrastructure resources
var redis = builder.AddRedis("cache")
    .WithRedisCommander(); // Adds Redis Commander for easy inspection

var postgres = builder.AddPostgres("postgres")
    .WithPgAdmin() // Adds PgAdmin for database management
    .AddDatabase("videodb");

// Add processing service (simulated microservice for demonstration)
var processingService = builder.AddProject("videoprocessing", "../VideoService.API/VideoService.API.csproj")
    .WithReference(redis)
    .WithReference(postgres);

// Add main API service with dependencies
var apiService = builder.AddProject("api", "../VideoService.API/VideoService.API.csproj")
    .WithReference(redis)
    .WithReference(postgres)
    .WithReference(processingService) // Service discovery demo
    .WithExternalHttpEndpoints();

// Add Blazor Web frontend
builder.AddProject("web", "../VideoService.Web/VideoService.Web.csproj")
    .WithReference(apiService)
    .WithExternalHttpEndpoints();

builder.Build().Run();
