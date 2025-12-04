namespace MicroVideoPlatform.Shared.Events;

/// <summary>
/// Base class for all domain events in the system
/// </summary>
public abstract record DomainEventBase
{
    /// <summary>
    /// Unique identifier for this event instance
    /// </summary>
    public Guid EventId { get; init; } = Guid.NewGuid();

    /// <summary>
    /// Timestamp when the event occurred
    /// </summary>
    public DateTime OccurredAt { get; init; } = DateTime.UtcNow;

    /// <summary>
    /// Event type name (derived from class name)
    /// </summary>
    public string EventType { get; init; }

    /// <summary>
    /// Version of the event schema
    /// </summary>
    public string EventVersion { get; init; } = "1.0";

    /// <summary>
    /// Correlation ID for tracking related events
    /// </summary>
    public Guid? CorrelationId { get; init; }

    /// <summary>
    /// User or service that caused this event
    /// </summary>
    public string? CausedBy { get; init; }

    /// <summary>
    /// Additional metadata for the event
    /// </summary>
    public Dictionary<string, string>? Metadata { get; init; }

    protected DomainEventBase()
    {
        EventType = GetType().Name;
    }
}
