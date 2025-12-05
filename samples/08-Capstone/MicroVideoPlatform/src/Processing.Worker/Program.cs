using Processing.Worker;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<VideoProcessingWorker>();

var host = builder.Build();
host.Run();
