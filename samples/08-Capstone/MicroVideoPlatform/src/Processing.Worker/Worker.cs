namespace Processing.Worker;

public class VideoProcessingWorker : BackgroundService
{
    private readonly ILogger<VideoProcessingWorker> _logger;

    public VideoProcessingWorker(ILogger<VideoProcessingWorker> logger)
    {
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Video Processing Worker started at: {time}", DateTimeOffset.Now);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                // Simulate checking for new videos to process
                await ProcessVideosAsync(stoppingToken);

                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
            }
            catch (OperationCanceledException)
            {
                // Graceful shutdown
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing videos");
                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
        }

        _logger.LogInformation("Video Processing Worker stopped at: {time}", DateTimeOffset.Now);
    }

    private async Task ProcessVideosAsync(CancellationToken cancellationToken)
    {
        // In production:
        // 1. Consume VideoUploadedEvent from RabbitMQ
        // 2. Download video from blob storage
        // 3. Transcode to multiple resolutions (1080p, 720p, 480p)
        // 4. Generate thumbnails
        // 5. Upload processed video to CDN
        // 6. Publish VideoProcessedEvent

        _logger.LogInformation("Checking for videos to process...");

        // Simulate processing
        var videoId = Guid.NewGuid();
        _logger.LogInformation("Processing video {VideoId}", videoId);
        await Task.Delay(TimeSpan.FromSeconds(2), cancellationToken);
        _logger.LogInformation("Video {VideoId} processed successfully", videoId);

        // Publish VideoProcessedEvent
        var processedEvent = new
        {
            VideoId = videoId,
            ProcessedUrl = $"https://cdn.example.com/videos/{videoId}/1080p.mp4",
            Thumbnails = new[]
            {
                $"https://cdn.example.com/videos/{videoId}/thumb-1.jpg",
                $"https://cdn.example.com/videos/{videoId}/thumb-2.jpg"
            },
            ProcessedAt = DateTime.UtcNow
        };

        _logger.LogInformation("VideoProcessedEvent: {Event}",
            System.Text.Json.JsonSerializer.Serialize(processedEvent));
    }
}
