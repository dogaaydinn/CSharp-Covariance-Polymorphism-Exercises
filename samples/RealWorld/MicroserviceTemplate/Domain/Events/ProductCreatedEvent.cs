namespace MicroserviceTemplate.Domain.Events;

/// <summary>
/// Domain event raised when a product is created
/// </summary>
public record ProductCreatedEvent : IDomainEvent
{
    public Guid ProductId { get; init; }
    public string ProductName { get; init; }
    public decimal Price { get; init; }
    public DateTime OccurredOn { get; init; }

    public ProductCreatedEvent(Guid productId, string productName, decimal price)
    {
        ProductId = productId;
        ProductName = productName;
        Price = price;
        OccurredOn = DateTime.UtcNow;
    }
}
