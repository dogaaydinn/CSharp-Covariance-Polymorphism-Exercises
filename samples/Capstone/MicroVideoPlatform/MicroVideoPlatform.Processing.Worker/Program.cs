using Serilog;
using MicroVideoPlatform.Processing.Worker;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.Seq("http://seq:5341")
    .CreateLogger();

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<VideoProcessingWorker>();
builder.Services.AddSerilog();

var host = builder.Build();
Log.Information("Processing.Worker starting");
host.Run();
