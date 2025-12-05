namespace Content.API.Infrastructure;

public interface IEventBus
{
    Task PublishAsync<TEvent>(TEvent @event) where TEvent : class;
}

// In-memory event bus for demo (replace with RabbitMQ in production)
public class InMemoryEventBus : IEventBus
{
    private readonly ILogger<InMemoryEventBus> _logger;

    public InMemoryEventBus(ILogger<InMemoryEventBus> logger)
    {
        _logger = logger;
    }

    public Task PublishAsync<TEvent>(TEvent @event) where TEvent : class
    {
        _logger.LogInformation("Event published: {EventType} - {Event}",
            typeof(TEvent).Name,
            System.Text.Json.JsonSerializer.Serialize(@event));

        // In production: publish to RabbitMQ, Azure Service Bus, etc.
        return Task.CompletedTask;
    }
}
