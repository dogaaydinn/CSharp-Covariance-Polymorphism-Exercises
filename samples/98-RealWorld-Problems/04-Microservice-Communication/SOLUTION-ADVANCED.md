# ÇÖZÜM: MESSAGE QUEUE (Asynchronous Communication)

## 🎯 ÇÖZÜM ÖZETİ

Message Queue kullanarak services'leri decouple et. RabbitMQ veya Azure Service Bus.

## 💻 IMPLEMENTATION

### OrderService (Publisher)
```csharp
public class OrderService
{
    private readonly IMessagePublisher _publisher;

    public async Task<Order> CreateOrderAsync(CreateOrderRequest request)
    {
        // 1. Order'ı kaydet
        var order = new Order(request) { Status = OrderStatus.Pending };
        await _orderRepository.SaveAsync(order);

        // 2. Event publish et (asenkron!)
        await _publisher.PublishAsync(new OrderCreatedEvent
        {
            OrderId = order.Id,
            CustomerId = order.CustomerId,
            Items = order.Items,
            TotalAmount = order.TotalAmount
        });

        // 3. Hemen dön (Payment bekleme!)
        return order;
    }
}
```

### PaymentService (Consumer)
```csharp
public class PaymentEventConsumer : IEventConsumer<OrderCreatedEvent>
{
    public async Task HandleAsync(OrderCreatedEvent @event)
    {
        try
        {
            var payment = await _paymentGateway.ChargeAsync(
                @event.CustomerId,
                @event.TotalAmount);

            if (payment.IsSuccess)
            {
                await _publisher.PublishAsync(new PaymentCompletedEvent
                {
                    OrderId = @event.OrderId,
                    PaymentId = payment.Id
                });
            }
            else
            {
                await _publisher.PublishAsync(new PaymentFailedEvent
                {
                    OrderId = @event.OrderId,
                    Reason = payment.FailureReason
                });
            }
        }
        catch (Exception ex)
        {
            // Retry logic (dead letter queue)
            _logger.LogError(ex, "Payment processing failed");
            throw; // Message will be retried
        }
    }
}
```

### Message Queue Configuration (RabbitMQ)
```csharp
public static class MessageQueueExtensions
{
    public static IServiceCollection AddMessageQueue(this IServiceCollection services)
    {
        services.AddMassTransit(x =>
        {
            x.AddConsumer<PaymentEventConsumer>();
            x.AddConsumer<InventoryEventConsumer>();

            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host("rabbitmq://localhost", h =>
                {
                    h.Username("guest");
                    h.Password("guest");
                });

                // Configure retry policy
                cfg.UseMessageRetry(r => r.Exponential(
                    retryLimit: 5,
                    minInterval: TimeSpan.FromSeconds(1),
                    maxInterval: TimeSpan.FromMinutes(5),
                    intervalDelta: TimeSpan.FromSeconds(2)
                ));

                cfg.ConfigureEndpoints(context);
            });
        });

        return services;
    }
}
```

## ✅ AVANTAJLAR
- ✅ Loose coupling (Payment down olsa Order hala çalışır)
- ✅ Asynchronous (fast response to user)
- ✅ Scalable (consumers independently scale)
- ✅ Fault tolerant (retry, dead letter queue)

## ⚠️ TRADE-OFFS
- ⚠️ Eventual consistency (order immediately "Pending")
- ⚠️ Complexity (message queue infrastructure)
- ⚠️ Debugging harder (distributed)

**Result:**
- Orders service: 99.9% uptime (independent of Payment)
- Payment failure doesn't bring down entire system
- Better user experience (fast order confirmation)

**Seviye:** Mid-Level → Senior Developer
