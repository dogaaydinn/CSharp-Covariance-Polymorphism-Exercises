using MicroVideoPlatform.Shared.Events;

namespace MicroVideoPlatform.Shared.Contracts;

/// <summary>
/// Contract for event bus operations (publish/subscribe pattern)
/// </summary>
public interface IEventBus
{
    /// <summary>
    /// Publishes an event to the event bus
    /// </summary>
    /// <typeparam name="TEvent">Type of event to publish</typeparam>
    /// <param name="event">Event instance to publish</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Task representing the async operation</returns>
    Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default)
        where TEvent : DomainEventBase;

    /// <summary>
    /// Subscribes to events of a specific type
    /// </summary>
    /// <typeparam name="TEvent">Type of event to subscribe to</typeparam>
    /// <param name="handler">Handler function to process the event</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Task representing the async operation</returns>
    Task SubscribeAsync<TEvent>(Func<TEvent, CancellationToken, Task> handler, CancellationToken cancellationToken = default)
        where TEvent : DomainEventBase;

    /// <summary>
    /// Unsubscribes from events of a specific type
    /// </summary>
    /// <typeparam name="TEvent">Type of event to unsubscribe from</typeparam>
    /// <returns>Task representing the async operation</returns>
    Task UnsubscribeAsync<TEvent>() where TEvent : DomainEventBase;
}
