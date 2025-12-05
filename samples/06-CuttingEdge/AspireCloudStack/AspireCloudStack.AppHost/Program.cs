var builder = DistributedApplication.CreateBuilder(args);

// Add Redis cache
var cache = builder.AddRedis("cache");

// Add API service
var apiService = builder.AddProject<Projects.AspireCloudStack_ApiService>("apiservice")
    .WithReference(cache);

// Add Web frontend
builder.AddProject<Projects.AspireCloudStack_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithReference(cache)
    .WithReference(apiService);

builder.Build().Run();
