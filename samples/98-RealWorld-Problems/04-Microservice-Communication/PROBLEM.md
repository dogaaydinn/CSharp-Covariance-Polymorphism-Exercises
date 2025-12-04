# GERÇEK DÜNYA PROBLEMİ: Microservice Communication

## 🚨 PROBLEM SENARYOSU

**Şirket:** E-commerce platform, microservice architecture
**Services:** Orders, Payment, Inventory, Shipping (4 services)
**Problem:** Services birbirine sıkı coupled, cascade failures oluyor

**Olay - Cuma 10:00:**
Payment service'te bug var, timeout veriyor. Sonuç:
- Orders service de timeout veriyor (Payment'i bekliyor)
- Tüm sistem DOWN (cascade failure)
- 2 saat downtime
- Revenue loss: $200,000

**Mevcut Durum (KÖTÜ):**
```csharp
// OrderService
public async Task<Order> CreateOrderAsync(CreateOrderRequest request)
{
    var order = new Order(request);
    await _orderRepository.SaveAsync(order);
    
    // ❌ SENKRON CALL - BLOCKING!
    var payment = await _paymentService.ProcessPaymentAsync(order.Id);
    if (!payment.IsSuccess)
        throw new PaymentFailedException();
    
    // ❌ Eğer Payment service down ise, Order service de down!
    await _inventoryService.ReserveItemsAsync(order.Items);
    await _shippingService.CreateShipmentAsync(order.Id);
    
    return order;
}
```

## 🎯 PROBLEM STATEMENT

> "Nasıl microservice communication tasarlayabiliriz ki:
> - Loose coupling olsun (bir service down olsa diğerleri çalışsın)
> - Eventual consistency (asenkron işler)
> - Fault tolerance (retry, circuit breaker)
> - Observability (distributed tracing)"

## 🔗 ÇÖZÜMLER

1. **BASIC:** REST APIs (tight coupling, synchronous)
2. **ADVANCED:** Message Queue (RabbitMQ/Azure Service Bus)
3. **ENTERPRISE:** Event-Driven Architecture (Saga Pattern)

Devam → `SOLUTION-ADVANCED.md` (Message Queue öner!)
