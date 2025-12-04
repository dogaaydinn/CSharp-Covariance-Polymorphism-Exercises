using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using MicroVideoPlatform.Shared.Events;
using MicroVideoPlatform.Shared.DTOs;
using MicroVideoPlatform.Shared.Enums;

namespace MicroVideoPlatform.Processing.Worker;

public class VideoProcessingWorker : BackgroundService
{
    private readonly ILogger<VideoProcessingWorker> _logger;
    private readonly IConfiguration _configuration;
    private IConnection? _connection;
    private IModel? _channel;
    private const string QueueName = "video.uploaded";

    public VideoProcessingWorker(ILogger<VideoProcessingWorker> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
        InitializeRabbitMQ();
    }

    private void InitializeRabbitMQ()
    {
        var factory = new ConnectionFactory
        {
            HostName = _configuration["RabbitMQ:Host"] ?? "localhost",
            Port = int.Parse(_configuration["RabbitMQ:Port"] ?? "5672"),
            UserName = _configuration["RabbitMQ:Username"] ?? "guest",
            Password = _configuration["RabbitMQ:Password"] ?? "guest"
        };

        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();
        _channel.QueueDeclare(QueueName, durable: true, exclusive: false, autoDelete: false);
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);

            try
            {
                var @event = JsonSerializer.Deserialize<VideoUploadedEvent>(message);
                if (@event != null)
                {
                    _logger.LogInformation("Processing video {VideoId}", @event.VideoId);
                    await ProcessVideo(@event, stoppingToken);
                    _channel?.BasicAck(ea.DeliveryTag, false);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing message");
                _channel?.BasicNack(ea.DeliveryTag, false, true);
            }
        };

        _channel?.BasicConsume(QueueName, autoAck: false, consumer);
        return Task.CompletedTask;
    }

    private async Task ProcessVideo(VideoUploadedEvent @event, CancellationToken cancellationToken)
    {
        var startTime = DateTime.UtcNow;

        // Simulate video processing
        await Task.Delay(5000, cancellationToken);

        // Publish completion event
        var completedEvent = new VideoProcessingCompletedEvent
        {
            VideoId = @event.VideoId,
            Metadata = new VideoMetadataDto
            {
                DurationSeconds = 120,
                Resolution = "1920x1080",
                Width = 1920,
                Height = 1080,
                Format = "mp4"
            },
            ProcessingStatus = new VideoProcessingStatusDto
            {
                VideoId = @event.VideoId,
                Status = VideoStatus.Completed,
                StartedAt = startTime,
                CompletedAt = DateTime.UtcNow,
                ProcessingTimeMs = (long)(DateTime.UtcNow - startTime).TotalMilliseconds,
                WorkerId = Environment.MachineName
            },
            ProcessingTimeMs = (long)(DateTime.UtcNow - startTime).TotalMilliseconds,
            WorkerId = Environment.MachineName
        };

        PublishEvent(completedEvent, "video.processing.completed");
        _logger.LogInformation("Video {VideoId} processed successfully", @event.VideoId);
    }

    private void PublishEvent<TEvent>(TEvent @event, string routingKey) where TEvent : DomainEventBase
    {
        var json = JsonSerializer.Serialize(@event);
        var body = Encoding.UTF8.GetBytes(json);
        var properties = _channel!.CreateBasicProperties();
        properties.Persistent = true;
        _channel.BasicPublish("video.events", routingKey, properties, body);
    }

    public override void Dispose()
    {
        _channel?.Close();
        _connection?.Close();
        base.Dispose();
    }
}
