using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MicroVideoPlatform.Analytics.Function.Services;
using Serilog;

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateLogger();

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureServices(services =>
    {
        // Application Insights
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();

        // Register ML.NET services as singletons (models are expensive to create)
        services.AddSingleton<VideoRecommendationService>();
        services.AddSingleton<VideoCommentAnalyzer>();

        // Logging
        services.AddLogging(builder =>
        {
            builder.AddSerilog(dispose: true);
        });
    })
    .Build();

Log.Information("Analytics Function starting...");

try
{
    await host.RunAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Analytics Function terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
