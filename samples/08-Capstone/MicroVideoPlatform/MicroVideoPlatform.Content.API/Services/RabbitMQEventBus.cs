using System.Text;
using System.Text.Json;
using MicroVideoPlatform.Shared.Contracts;
using MicroVideoPlatform.Shared.Events;
using RabbitMQ.Client;

namespace MicroVideoPlatform.Content.API.Services;

public sealed class RabbitMQEventBus : IEventBus, IDisposable
{
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private readonly ILogger<RabbitMQEventBus> _logger;
    private const string ExchangeName = "video.events";

    public RabbitMQEventBus(IConfiguration configuration, ILogger<RabbitMQEventBus> logger)
    {
        _logger = logger;
        var factory = new ConnectionFactory
        {
            HostName = configuration["RabbitMQ:Host"] ?? "localhost",
            Port = int.Parse(configuration["RabbitMQ:Port"] ?? "5672"),
            UserName = configuration["RabbitMQ:Username"] ?? "guest",
            Password = configuration["RabbitMQ:Password"] ?? "guest",
            VirtualHost = configuration["RabbitMQ:VirtualHost"] ?? "/"
        };

        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();
        _channel.ExchangeDeclare(ExchangeName, ExchangeType.Topic, durable: true);
    }

    public Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default) where TEvent : DomainEventBase
    {
        var routingKey = @event.EventType switch
        {
            nameof(VideoUploadedEvent) => "video.uploaded",
            nameof(VideoProcessingCompletedEvent) => "video.processing.completed",
            nameof(VideoProcessingFailedEvent) => "video.processing.failed",
            nameof(VideoAnalyticsCompletedEvent) => "video.analytics.completed",
            _ => @event.EventType.ToLower()
        };

        var json = JsonSerializer.Serialize(@event, new JsonSerializerOptions { WriteIndented = false });
        var body = Encoding.UTF8.GetBytes(json);

        var properties = _channel.CreateBasicProperties();
        properties.Persistent = true;
        properties.ContentType = "application/json";
        properties.Type = @event.EventType;
        properties.MessageId = @event.EventId.ToString();
        properties.Timestamp = new AmqpTimestamp(DateTimeOffset.UtcNow.ToUnixTimeSeconds());

        _channel.BasicPublish(ExchangeName, routingKey, properties, body);
        _logger.LogInformation("Published event {EventType} with ID {EventId} to routing key {RoutingKey}",
            @event.EventType, @event.EventId, routingKey);

        return Task.CompletedTask;
    }

    public Task SubscribeAsync<TEvent>(Func<TEvent, CancellationToken, Task> handler, CancellationToken cancellationToken = default) where TEvent : DomainEventBase
    {
        throw new NotImplementedException("Content.API does not subscribe to events");
    }

    public Task UnsubscribeAsync<TEvent>() where TEvent : DomainEventBase
    {
        throw new NotImplementedException("Content.API does not subscribe to events");
    }

    public void Dispose()
    {
        _channel?.Close();
        _channel?.Dispose();
        _connection?.Close();
        _connection?.Dispose();
    }
}
