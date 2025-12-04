# ÇÖZÜM: STRANGLER FIG PATTERN (Incremental Refactoring)

## 🎯 ÇÖZÜM ÖZETİ

Strangler Fig Pattern: Eski kodu yavaş yavaş yeni kod ile değiştir.

## 📊 STRANGLER FIG PATTERN

```
OLD SYSTEM              NEW SYSTEM
┌─────────────┐        ┌─────────────┐
│ Legacy      │        │ Modern      │
│ Monolith    │        │ Services    │
│             │        │             │
│ ┌─────────┐ │  ┌────►│ ┌─────────┐ │
│ │Order    │ │  │     │ │Order    │ │
│ │Service  │◄├──┘     │ │Service  │ │
│ └─────────┘ │        │ └─────────┘ │
│             │        │             │
│ ┌─────────┐ │        │ ┌─────────┐ │
│ │Payment◄─┼─┼────────┼►│Payment  │ │
│ │Service  │ │        │ │Service  │ │
│ └─────────┘ │        │ └─────────┘ │
└─────────────┘        └─────────────┘
     OLD                    NEW
   (Legacy)            (Refactored)
```

## 💻 IMPLEMENTATION STEPS

### Step 1: Add Characterization Tests
```csharp
// Önce mevcut davranışı test et (regression prevention)
[Fact]
public void ProcessOrder_ValidOrder_ShouldSucceed()
{
    // Arrange
    var processor = new OrderProcessor();
    var orderId = CreateTestOrder();
    
    // Act
    processor.ProcessOrder(orderId);
    
    // Assert
    // Verify current behavior (even if it's bad!)
    var order = GetOrderFromDatabase(orderId);
    Assert.Equal(OrderStatus.Processed, order.Status);
}
```

### Step 2: Extract Interface
```csharp
// Old code
public class LegacyOrderProcessor
{
    public void ProcessOrder(int orderId) { /* 800 lines */ }
}

// New interface
public interface IOrderProcessor
{
    void ProcessOrder(int orderId);
}

// Legacy implements interface
public class LegacyOrderProcessor : IOrderProcessor
{
    public void ProcessOrder(int orderId) { /* 800 lines - don't touch yet! */ }
}
```

### Step 3: Create New Implementation
```csharp
// New, clean implementation
public class ModernOrderProcessor : IOrderProcessor
{
    private readonly IOrderRepository _repository;
    private readonly IPaymentService _paymentService;
    private readonly IEmailService _emailService;
    private readonly ILogger<ModernOrderProcessor> _logger;

    public ModernOrderProcessor(
        IOrderRepository repository,
        IPaymentService paymentService,
        IEmailService emailService,
        ILogger<ModernOrderProcessor> logger)
    {
        _repository = repository;
        _paymentService = paymentService;
        _emailService = emailService;
        _logger = logger;
    }

    public async Task ProcessOrderAsync(int orderId)
    {
        _logger.LogInformation("Processing order {OrderId}", orderId);
        
        var order = await _repository.GetByIdAsync(orderId);
        if (order == null)
        {
            throw new OrderNotFoundException(orderId);
        }

        try
        {
            await _paymentService.ProcessPaymentAsync(order);
            await _emailService.SendOrderConfirmationAsync(order);
            
            order.Status = OrderStatus.Processed;
            await _repository.UpdateAsync(order);
            
            _logger.LogInformation("Order {OrderId} processed successfully", orderId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to process order {OrderId}", orderId);
            throw;
        }
    }
}
```

### Step 4: Feature Toggle (Gradual Rollout)
```csharp
public class OrderProcessorProxy : IOrderProcessor
{
    private readonly LegacyOrderProcessor _legacyProcessor;
    private readonly ModernOrderProcessor _modernProcessor;
    private readonly IFeatureToggle _featureToggle;

    public async Task ProcessOrder(int orderId)
    {
        // Feature toggle: gradually move users to new implementation
        if (await _featureToggle.IsEnabledAsync("UseModernOrderProcessor", orderId))
        {
            await _modernProcessor.ProcessOrderAsync(orderId);
        }
        else
        {
            _legacyProcessor.ProcessOrder(orderId);
        }
    }
}
```

### Step 5: Monitor & Validate
```csharp
public class MonitoredOrderProcessor : IOrderProcessor
{
    public async Task ProcessOrder(int orderId)
    {
        var legacyResult = await RunLegacyAsync(orderId);
        var modernResult = await RunModernAsync(orderId);

        // Compare results
        if (legacyResult != modernResult)
        {
            _logger.LogWarning(
                "Mismatch detected! OrderId: {OrderId}, Legacy: {Legacy}, Modern: {Modern}",
                orderId, legacyResult, modernResult);
        }

        // Use legacy result for now (safe)
        return legacyResult;
    }
}
```

## 📊 TIMELINE

**Week 1-2:** Characterization tests (100% coverage of critical paths)
**Week 3-4:** Extract interfaces, create proxies
**Week 5-8:** Implement modern version
**Week 9:** Deploy with feature toggle (0% traffic to new)
**Week 10:** Gradual rollout (10% → 25% → 50% → 100%)
**Week 11:** Monitor, validate, compare results
**Week 12:** Remove legacy code (celebrate! 🎉)

## ✅ AVANTAJLAR
- ✅ Zero downtime
- ✅ Incremental progress
- ✅ Rollback anytime
- ✅ Safe (feature toggle)

## ⚠️ TRADE-OFFS
- ⚠️ Takes longer (3 months vs 1 month big bang)
- ⚠️ Temporary duplication (old + new code)
- ⚠️ Requires discipline

**Result:**
- Legacy 800-line method → Clean 50-line SOLID code
- Test coverage: 0% → 90%
- Production incidents: 0
- Team confidence: 📈

**Seviye:** Senior Developer - This is how pros refactor!
