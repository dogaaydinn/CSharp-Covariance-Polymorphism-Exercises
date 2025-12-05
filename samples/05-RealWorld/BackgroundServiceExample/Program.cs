using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<EmailWorker>();

var host = builder.Build();
host.Run();

public class EmailWorker : BackgroundService
{
    private readonly ILogger<EmailWorker> _logger;

    public EmailWorker(ILogger<EmailWorker> logger)
    {
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Email Worker starting...");

        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("Processing email queue... {Time}", DateTimeOffset.Now);

            // Simulate processing
            await Task.Delay(5000, stoppingToken);

            _logger.LogInformation("Sent 10 emails");
        }

        _logger.LogInformation("Email Worker stopping...");
    }
}
