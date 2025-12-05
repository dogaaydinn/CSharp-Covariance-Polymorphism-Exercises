# Why Event Handler Pattern? 🎯

## Table of Contents
1. [What is the Event Handler Pattern?](#what-is-the-event-handler-pattern)
2. [Historical Context](#historical-context)
3. [Why Use Event Handler Pattern?](#why-use-event-handler-pattern)
4. [When to Use Events](#when-to-use-events)
5. [Real-World Scenarios](#real-world-scenarios)
6. [Performance Considerations](#performance-considerations)
7. [Common Mistakes and Fixes](#common-mistakes-and-fixes)
8. [Migration Strategies](#migration-strategies)
9. [Comparison with Alternatives](#comparison-with-alternatives)
10. [Advanced Techniques](#advanced-techniques)
11. [Summary](#summary)

---

## What is the Event Handler Pattern?

The Event Handler Pattern is a behavioral design pattern that establishes a **one-to-many dependency** between objects so that when one object (the publisher) changes state, all its dependents (subscribers) are notified and updated automatically.

### Core Syntax

```csharp
// ❌ Without events - tight coupling
public class Order
{
    private NotificationService _notificationService;
    private LoggingService _loggingService;

    public void ChangeStatus(OrderStatus newStatus)
    {
        _status = newStatus;
        _notificationService.NotifyCustomer(this);  // Direct dependency
        _loggingService.LogStatusChange(this);      // Direct dependency
    }
}

// ✅ With events - loose coupling
public class Order
{
    public event EventHandler<OrderStatusChangedEventArgs>? StatusChanged;

    public void ChangeStatus(OrderStatus newStatus)
    {
        var oldStatus = _status;
        _status = newStatus;
        OnStatusChanged(new OrderStatusChangedEventArgs(OrderId, oldStatus, newStatus));
        // Publisher doesn't know who's listening!
    }

    protected virtual void OnStatusChanged(OrderStatusChangedEventArgs e)
    {
        StatusChanged?.Invoke(this, e);
    }
}
```

### Three Components of Event Pattern

1. **Event Declaration**: `public event EventHandler<T>? EventName;`
2. **Event Raising**: Protected method that invokes the event
3. **Event Subscription**: Subscribers attach/detach handlers

```csharp
// Component 1: Declaration
public event EventHandler<OrderCreatedEventArgs>? OrderCreated;

// Component 2: Raising
protected virtual void OnOrderCreated(OrderCreatedEventArgs e)
{
    OrderCreated?.Invoke(this, e);
}

// Component 3: Subscription
order.OrderCreated += (sender, e) => Console.WriteLine($"Order {e.OrderId} created");
```

---

## Historical Context

### Pre-.NET 1.0 (Before 2002) - No Built-in Events

```csharp
// Callbacks in C/C++ style
public delegate void StatusChangedCallback(Order order);

public class Order
{
    private List<StatusChangedCallback> callbacks = new();

    public void RegisterCallback(StatusChangedCallback callback)
    {
        callbacks.Add(callback);
    }

    public void UnregisterCallback(StatusChangedCallback callback)
    {
        callbacks.Remove(callback);
    }

    public void ChangeStatus(OrderStatus newStatus)
    {
        _status = newStatus;
        foreach (var callback in callbacks)
        {
            callback(this);  // Manual invocation
        }
    }
}

// Usage - verbose and error-prone
void OnStatusChanged(Order order)
{
    Console.WriteLine($"Status changed for {order.OrderId}");
}

order.RegisterCallback(OnStatusChanged);  // Must remember to unregister!
```

**Problems:**
- No language support
- Manual management of callback lists
- No type safety for event data
- Easy to forget unregistering
- No protection against null references

### .NET 1.0 (2002) - Events Introduced

```csharp
// Events become first-class language feature
public class Order
{
    public event EventHandler StatusChanged;  // Built-in support!

    public void ChangeStatus(OrderStatus newStatus)
    {
        _status = newStatus;
        StatusChanged?.Invoke(this, EventArgs.Empty);
    }
}

// Usage - clean and safe
order.StatusChanged += OnStatusChanged;

void OnStatusChanged(object sender, EventArgs e)
{
    Console.WriteLine("Status changed");
}
```

**Improvements:**
- `event` keyword for encapsulation
- += and -= operators for subscription
- Compiler-enforced += usage (can't directly invoke)
- Thread-safe add/remove operations

### .NET 2.0 (2005) - Generic EventHandler&lt;T&gt;

```csharp
// Before: Custom delegate for each event
public delegate void OrderStatusChangedEventHandler(
    object sender,
    OrderStatusChangedEventArgs e);

public event OrderStatusChangedEventHandler StatusChanged;

// After: Generic EventHandler<T>
public event EventHandler<OrderStatusChangedEventArgs>? StatusChanged;
```

**Benefits:**
- No need to declare custom delegates
- Consistent pattern across all events
- Better tooling support
- Less boilerplate code

### .NET 4.0 (2010) - Weak Events (WPF)

```csharp
// Problem: Long-lived publisher + short-lived subscribers = memory leaks
// Solution: Weak Event Pattern
WeakEventManager<Order, OrderStatusChangedEventArgs>
    .AddHandler(order, nameof(order.StatusChanged), OnStatusChanged);
```

### Why Microsoft Designed Events This Way

**Goal 1: Encapsulation**
```csharp
public event EventHandler<OrderCreatedEventArgs>? OrderCreated;

// ❌ Can't do this from outside the class:
order.OrderCreated?.Invoke(this, e);  // Compile error!

// ✅ Only the declaring class can raise events
```

**Goal 2: Multicast Support**
```csharp
// Multiple subscribers automatically supported
order.OrderCreated += Subscriber1;
order.OrderCreated += Subscriber2;
order.OrderCreated += Subscriber3;

// One raise, all three get notified
```

**Goal 3: Memory Management**
```csharp
// -= operator for cleanup
order.OrderCreated -= Subscriber1;  // Prevents memory leaks
```

---

## Why Use Event Handler Pattern?

### 1. Loose Coupling 🔗

**Problem**: Direct dependencies make code rigid and hard to change.

```csharp
// ❌ Tightly coupled - Order knows about all services
public class Order
{
    private readonly NotificationService _notification;
    private readonly LoggingService _logging;
    private readonly InventoryService _inventory;
    private readonly EmailService _email;
    private readonly AnalyticsService _analytics;
    // Add new service? Must modify Order class!

    public Order(
        NotificationService notification,
        LoggingService logging,
        InventoryService inventory,
        EmailService email,
        AnalyticsService analytics)
    {
        _notification = notification;
        _logging = logging;
        _inventory = inventory;
        _email = email;
        _analytics = analytics;
    }

    public void ChangeStatus(OrderStatus newStatus)
    {
        _status = newStatus;
        _notification.Notify(this);
        _logging.Log(this);
        _inventory.Update(this);
        _email.Send(this);
        _analytics.Track(this);
        // All services must be called explicitly
    }
}
```

```csharp
// ✅ Loosely coupled - Order doesn't know about services
public class Order
{
    public event EventHandler<OrderStatusChangedEventArgs>? StatusChanged;

    public void ChangeStatus(OrderStatus newStatus)
    {
        var oldStatus = _status;
        _status = newStatus;
        OnStatusChanged(new OrderStatusChangedEventArgs(OrderId, oldStatus, newStatus));
        // Done! Don't care who's listening
    }

    protected virtual void OnStatusChanged(OrderStatusChangedEventArgs e)
    {
        StatusChanged?.Invoke(this, e);
    }
}

// Services subscribe independently
var order = new Order("ORD-001", "John");
notificationService.SubscribeTo(order);  // Optional
loggingService.SubscribeTo(order);       // Optional
// Add new service? No changes to Order!
```

**Benefit**: **Open/Closed Principle** - Open for extension, closed for modification.

### 2. One-to-Many Relationships 📢

**Problem**: Notifying multiple objects requires maintaining a list and iterating.

```csharp
// ❌ Manual observer list management
public class Order
{
    private List<IOrderObserver> _observers = new();

    public void Attach(IOrderObserver observer)
    {
        if (!_observers.Contains(observer))
            _observers.Add(observer);
    }

    public void Detach(IOrderObserver observer)
    {
        _observers.Remove(observer);
    }

    public void Notify()
    {
        foreach (var observer in _observers)
        {
            observer.Update(this);
        }
    }

    public void ChangeStatus(OrderStatus newStatus)
    {
        _status = newStatus;
        Notify();  // Manual notification
    }
}
```

```csharp
// ✅ Events handle multicast automatically
public class Order
{
    public event EventHandler<OrderStatusChangedEventArgs>? StatusChanged;

    public void ChangeStatus(OrderStatus newStatus)
    {
        _status = newStatus;
        OnStatusChanged(new OrderStatusChangedEventArgs(...));
        // Automatically notifies all subscribers!
    }
}

// Multiple subscribers - no manual list management
order.StatusChanged += Handler1;
order.StatusChanged += Handler2;
order.StatusChanged += Handler3;
```

**Benefit**: Compiler manages the invocation list, thread-safe add/remove.

### 3. Asynchronous Notifications 🚀

Events integrate seamlessly with async/await:

```csharp
public class Order
{
    public event EventHandler<OrderStatusChangedEventArgs>? StatusChanged;

    // Subscribers can be async
}

// Async event handler
order.StatusChanged += async (sender, e) =>
{
    await SendEmailAsync(e.OrderId);
    await UpdateDatabaseAsync(e);
    await NotifyExternalSystemAsync(e);
};

order.ChangeStatus(OrderStatus.Shipped);
// All async operations start concurrently!
```

### 4. Testability 🧪

**Problem**: Tightly coupled code is hard to test in isolation.

```csharp
// ❌ Hard to test - requires all dependencies
public class Order
{
    private NotificationService _notification;

    public Order(NotificationService notification)
    {
        _notification = notification;  // Required dependency
    }

    public void ChangeStatus(OrderStatus newStatus)
    {
        _status = newStatus;
        _notification.Notify(this);  // Can't test Order without NotificationService
    }
}

// Test requires real or mocked NotificationService
[Fact]
public void ChangeStatus_UpdatesStatus()
{
    var mockNotification = new Mock<NotificationService>();  // Must mock!
    var order = new Order(mockNotification.Object);

    order.ChangeStatus(OrderStatus.Processing);

    Assert.Equal(OrderStatus.Processing, order.Status);
}
```

```csharp
// ✅ Easy to test - no required dependencies
public class Order
{
    public event EventHandler<OrderStatusChangedEventArgs>? StatusChanged;

    public void ChangeStatus(OrderStatus newStatus)
    {
        _status = newStatus;
        OnStatusChanged(new OrderStatusChangedEventArgs(...));
    }
}

// Test is simple and focused
[Fact]
public void ChangeStatus_UpdatesStatus()
{
    var order = new Order("ORD-001", "John");  // No dependencies!

    order.ChangeStatus(OrderStatus.Processing);

    Assert.Equal(OrderStatus.Processing, order.Status);
}

// Test event raising separately
[Fact]
public void ChangeStatus_RaisesStatusChangedEvent()
{
    var order = new Order("ORD-001", "John");
    OrderStatusChangedEventArgs? raisedArgs = null;

    order.StatusChanged += (s, e) => raisedArgs = e;

    order.ChangeStatus(OrderStatus.Processing);

    Assert.NotNull(raisedArgs);
    Assert.Equal(OrderStatus.Processing, raisedArgs.NewStatus);
}
```

### 5. Dynamic Behavior at Runtime 🔄

Events allow behavior to be modified at runtime without changing code:

```csharp
public class Order
{
    public event EventHandler<OrderStatusChangedEventArgs>? StatusChanged;
}

// Configuration-based subscription
if (config.EnableEmailNotifications)
{
    order.StatusChanged += emailService.OnStatusChanged;
}

if (config.EnableSmsNotifications)
{
    order.StatusChanged += smsService.OnStatusChanged;
}

if (config.EnablePushNotifications)
{
    order.StatusChanged += pushService.OnStatusChanged;
}

// Feature flags
if (featureFlags.IsEnabled("advanced-analytics"))
{
    order.StatusChanged += analyticsService.TrackAdvancedMetrics;
}

// A/B testing
if (user.IsInExperimentGroup("notification-v2"))
{
    order.StatusChanged += notificationV2Service.Notify;
}
else
{
    order.StatusChanged += notificationV1Service.Notify;
}
```

---

## When to Use Events

### ✅ Use Events When:

#### 1. **Publisher Doesn't Know Subscribers** (Loose Coupling)

```csharp
// Publisher: Order
public class Order
{
    public event EventHandler<OrderStatusChangedEventArgs>? StatusChanged;
    // Order doesn't know or care who's listening
}

// Subscriber 1: NotificationService
public class NotificationService
{
    public void SubscribeTo(Order order)
    {
        order.StatusChanged += OnStatusChanged;
    }
}

// Subscriber 2: LoggingService (added later)
public class LoggingService
{
    public void SubscribeTo(Order order)
    {
        order.StatusChanged += OnStatusChanged;
    }
}

// No changes to Order class needed!
```

#### 2. **Multiple Subscribers** (One-to-Many)

```csharp
// One event, many subscribers
order.StatusChanged += notificationService.Notify;
order.StatusChanged += loggingService.Log;
order.StatusChanged += inventoryService.Update;
order.StatusChanged += emailService.Send;
order.StatusChanged += analyticsService.Track;

// All receive the same notification
order.ChangeStatus(OrderStatus.Shipped);
```

#### 3. **Asynchronous Notifications**

```csharp
// Subscribers process independently
order.StatusChanged += async (s, e) =>
{
    await SendEmailAsync(e);  // Don't wait for this
};

order.StatusChanged += async (s, e) =>
{
    await UpdateCacheAsync(e);  // Or this
};

order.ChangeStatus(OrderStatus.Processing);
// Returns immediately, subscribers run in background
```

#### 4. **Optional Behavior** (Plug-in Architecture)

```csharp
// Core functionality without events
var order = new Order("ORD-001", "John");
order.AddItem(new OrderItem("Laptop", 1200m, 1));
order.ChangeStatus(OrderStatus.Processing);
// Works fine without any subscribers

// Optional: Add logging if needed
loggingService.SubscribeTo(order);

// Optional: Add notifications if needed
notificationService.SubscribeTo(order);
```

#### 5. **Separation of Concerns**

```csharp
// Order handles business logic
public class Order
{
    public void ChangeStatus(OrderStatus newStatus)
    {
        // Business rules
        if (_status == OrderStatus.Delivered)
            throw new InvalidOperationException("Cannot change status of delivered order");

        _status = newStatus;
        OnStatusChanged(new OrderStatusChangedEventArgs(...));
    }
}

// Services handle cross-cutting concerns
public class AuditService  // Logging concern
{
    public void SubscribeTo(Order order)
    {
        order.StatusChanged += LogChange;
    }
}

public class CacheService  // Caching concern
{
    public void SubscribeTo(Order order)
    {
        order.StatusChanged += InvalidateCache;
    }
}
```

### ❌ DON'T Use Events When:

#### 1. **Direct Communication Needed** (Request-Response)

```csharp
// ❌ Bad use of events
public class OrderValidator
{
    public event EventHandler<ValidationResultEventArgs>? ValidationCompleted;

    public void Validate(Order order)
    {
        var result = PerformValidation(order);
        OnValidationCompleted(new ValidationResultEventArgs(result));
        // Caller needs to wait for result - don't use events!
    }
}

// ✅ Good - direct return
public class OrderValidator
{
    public ValidationResult Validate(Order order)
    {
        return PerformValidation(order);  // Direct return
    }
}
```

#### 2. **Guaranteed Processing Required**

```csharp
// ❌ Bad - events can have zero subscribers
public void ProcessPayment(Order order)
{
    OnPaymentProcessing(new PaymentEventArgs(order));
    // What if no one is listening? Payment not processed!
}

// ✅ Good - explicit dependency
public void ProcessPayment(Order order, IPaymentProcessor processor)
{
    processor.ProcessPayment(order);  // Guaranteed to execute
}
```

#### 3. **Single Subscriber** (One-to-One)

```csharp
// ❌ Overkill for single subscriber
public class Order
{
    public event EventHandler<OrderCreatedEventArgs>? OrderCreated;
    // Only OrderRepository ever subscribes - don't need event
}

// ✅ Better - direct dependency
public class Order
{
    private readonly IOrderRepository _repository;

    public Order(IOrderRepository repository)
    {
        _repository = repository;
    }

    public void Save()
    {
        _repository.Save(this);
    }
}
```

#### 4. **Tight Coupling Acceptable**

```csharp
// ❌ Unnecessary abstraction
public class Button
{
    public event EventHandler<ClickEventArgs>? Clicked;
    // Button IS the UI - tight coupling to UI framework is fine
}

// ✅ Better - direct method
public class Button
{
    protected virtual void OnClick()
    {
        // Default behavior
        UpdateUI();
    }
}
```

#### 5. **Performance Critical Paths**

```csharp
// ❌ Events add overhead in hot paths
public void ProcessPixel(int x, int y, Color color)
{
    OnPixelProcessed(new PixelEventArgs(x, y, color));
    // Called millions of times per frame - event overhead significant
}

// ✅ Better - direct processing
public void ProcessPixel(int x, int y, Color color)
{
    _buffer[y * width + x] = color;  // Direct, fast
}
```

---

## Real-World Scenarios

### Scenario 1: E-Commerce Order Processing 🛒

**Context**: Online store processing customer orders with multiple backend services.

**Problem Without Events**:

```csharp
public class OrderService
{
    private readonly INotificationService _notification;
    private readonly IInventoryService _inventory;
    private readonly IPaymentService _payment;
    private readonly IShippingService _shipping;
    private readonly IEmailService _email;
    private readonly IAnalyticsService _analytics;
    private readonly ILoyaltyService _loyalty;

    public OrderService(/* 7 dependencies! */)
    {
        // Constructor hell
    }

    public async Task ProcessOrder(Order order)
    {
        // Change status
        order.Status = OrderStatus.Processing;

        // Manually call each service
        await _notification.NotifyCustomer(order);
        await _inventory.ReserveStock(order);
        await _payment.ProcessPayment(order);
        await _shipping.CalculateShipping(order);
        await _email.SendConfirmation(order);
        await _analytics.TrackOrder(order);
        await _loyalty.AwardPoints(order);

        // If any service fails, rollback is complex
        // Adding new service requires changing this code
    }
}
```

**Solution With Events**:

```csharp
// Order domain class
public class Order
{
    public event EventHandler<OrderCreatedEventArgs>? OrderCreated;
    public event EventHandler<OrderStatusChangedEventArgs>? StatusChanged;

    public Order(string orderId, string customerName)
    {
        OrderId = orderId;
        CustomerName = customerName;
        Status = OrderStatus.Pending;
        OnOrderCreated(new OrderCreatedEventArgs(orderId, customerName, DateTime.Now));
    }

    public void ChangeStatus(OrderStatus newStatus)
    {
        var oldStatus = Status;
        Status = newStatus;
        OnStatusChanged(new OrderStatusChangedEventArgs(OrderId, oldStatus, newStatus, DateTime.Now));
    }

    protected virtual void OnOrderCreated(OrderCreatedEventArgs e)
    {
        OrderCreated?.Invoke(this, e);
    }

    protected virtual void OnStatusChanged(OrderStatusChangedEventArgs e)
    {
        StatusChanged?.Invoke(this, e);
    }
}

// Each service subscribes independently
public class NotificationService
{
    public void SubscribeTo(Order order)
    {
        order.OrderCreated += OnOrderCreated;
        order.StatusChanged += OnStatusChanged;
    }

    private async void OnOrderCreated(object? sender, OrderCreatedEventArgs e)
    {
        await NotifyCustomerAsync($"Order {e.OrderId} created");
    }

    private async void OnStatusChanged(object? sender, OrderStatusChangedEventArgs e)
    {
        await NotifyCustomerAsync($"Order {e.OrderId}: {e.OldStatus} → {e.NewStatus}");
    }
}

public class InventoryService
{
    public void SubscribeTo(Order order)
    {
        order.StatusChanged += OnStatusChanged;
    }

    private async void OnStatusChanged(object? sender, OrderStatusChangedEventArgs e)
    {
        if (e.NewStatus == OrderStatus.Processing)
        {
            await ReserveStockAsync(e.OrderId);
        }
        else if (e.NewStatus == OrderStatus.Cancelled)
        {
            await ReleaseStockAsync(e.OrderId);
        }
    }
}

// Simple orchestrator
public class OrderProcessor
{
    public Order CreateOrder(string orderId, string customerName)
    {
        var order = new Order(orderId, customerName);

        // Wire up services
        _notificationService.SubscribeTo(order);
        _inventoryService.SubscribeTo(order);
        _paymentService.SubscribeTo(order);
        _emailService.SubscribeTo(order);
        _analyticsService.SubscribeTo(order);
        // Adding new service? Just add one line!

        return order;
    }

    public void ProcessOrder(Order order)
    {
        order.ChangeStatus(OrderStatus.Processing);
        // All services notified automatically
    }
}
```

**Benefits**:
- Order class has **zero dependencies**
- Each service is **independent**
- **Easy to add/remove** services
- Services can **fail independently** without affecting others
- **Clear separation of concerns**

---

### Scenario 2: Stock Trading Application 📈

**Context**: Real-time stock trading platform with multiple data consumers.

**Problem Without Events**:

```csharp
public class StockPriceService
{
    private readonly IChartUpdater _chartUpdater;
    private readonly IAlertService _alertService;
    private readonly IPortfolioManager _portfolioManager;
    private readonly INotificationService _notificationService;

    public void UpdatePrice(string symbol, decimal price)
    {
        // Manually update all consumers
        _chartUpdater.UpdateChart(symbol, price);
        _alertService.CheckAlerts(symbol, price);
        _portfolioManager.RecalculateValue(symbol, price);
        _notificationService.NotifyUsers(symbol, price);

        // If chart update is slow, everything is blocked!
        // Can't add new consumer without changing this code
    }
}
```

**Solution With Events**:

```csharp
public class StockPriceService
{
    public event EventHandler<PriceChangedEventArgs>? PriceChanged;

    public void UpdatePrice(string symbol, decimal price, DateTime timestamp)
    {
        OnPriceChanged(new PriceChangedEventArgs(symbol, price, timestamp));
        // Done! Don't care how many consumers or what they do
    }

    protected virtual void OnPriceChanged(PriceChangedEventArgs e)
    {
        PriceChanged?.Invoke(this, e);
    }
}

// Multiple independent consumers
public class ChartUpdater
{
    public void SubscribeTo(StockPriceService service)
    {
        service.PriceChanged += OnPriceChanged;
    }

    private void OnPriceChanged(object? sender, PriceChangedEventArgs e)
    {
        UpdateChart(e.Symbol, e.Price);
    }
}

public class AlertService
{
    public void SubscribeTo(StockPriceService service)
    {
        service.PriceChanged += OnPriceChanged;
    }

    private void OnPriceChanged(object? sender, PriceChangedEventArgs e)
    {
        if (ShouldAlert(e.Symbol, e.Price))
        {
            SendAlert(e.Symbol, e.Price);
        }
    }
}

public class PortfolioManager
{
    public void SubscribeTo(StockPriceService service)
    {
        service.PriceChanged += OnPriceChanged;
    }

    private void OnPriceChanged(object? sender, PriceChangedEventArgs e)
    {
        RecalculatePortfolioValue(e.Symbol, e.Price);
    }
}
```

**Benefits**:
- **Concurrent processing**: All consumers notified simultaneously
- **Non-blocking**: Slow consumer doesn't block others
- **Scalable**: Add unlimited consumers without changing publisher
- **Real-time**: Price updates propagate instantly

---

### Scenario 3: Game Development 🎮

**Context**: Game engine with multiple systems responding to game events.

**Solution With Events**:

```csharp
public class Player
{
    public event EventHandler<PlayerDamagedEventArgs>? Damaged;
    public event EventHandler<PlayerHealedEventArgs>? Healed;
    public event EventHandler<PlayerLeveledUpEventArgs>? LeveledUp;
    public event EventHandler<PlayerDiedEventArgs>? Died;

    public void TakeDamage(float amount)
    {
        _health -= amount;
        OnDamaged(new PlayerDamagedEventArgs(_health, amount));

        if (_health <= 0)
        {
            OnDied(new PlayerDiedEventArgs());
        }
    }

    public void Heal(float amount)
    {
        _health += amount;
        OnHealed(new PlayerHealedEventArgs(_health, amount));
    }

    public void AddExperience(int xp)
    {
        _experience += xp;
        if (_experience >= _experienceToNextLevel)
        {
            _level++;
            OnLeveledUp(new PlayerLeveledUpEventArgs(_level));
        }
    }
}

// UI System
public class UISystem
{
    public void SubscribeTo(Player player)
    {
        player.Damaged += OnPlayerDamaged;
        player.Healed += OnPlayerHealed;
        player.LeveledUp += OnPlayerLeveledUp;
    }

    private void OnPlayerDamaged(object? sender, PlayerDamagedEventArgs e)
    {
        UpdateHealthBar(e.CurrentHealth);
        ShowDamageNumber(e.DamageAmount);
    }

    private void OnPlayerHealed(object? sender, PlayerHealedEventArgs e)
    {
        UpdateHealthBar(e.CurrentHealth);
        ShowHealEffect(e.HealAmount);
    }

    private void OnPlayerLeveledUp(object? sender, PlayerLeveledUpEventArgs e)
    {
        ShowLevelUpAnimation(e.NewLevel);
        PlayLevelUpSound();
    }
}

// Sound System
public class AudioSystem
{
    public void SubscribeTo(Player player)
    {
        player.Damaged += (s, e) => PlaySound("player_hurt.wav");
        player.Healed += (s, e) => PlaySound("heal.wav");
        player.LeveledUp += (s, e) => PlaySound("level_up.wav");
        player.Died += (s, e) => PlaySound("death.wav");
    }
}

// Achievement System
public class AchievementSystem
{
    public void SubscribeTo(Player player)
    {
        player.LeveledUp += OnPlayerLeveledUp;
        player.Died += OnPlayerDied;
    }

    private void OnPlayerLeveledUp(object? sender, PlayerLeveledUpEventArgs e)
    {
        if (e.NewLevel == 10)
            UnlockAchievement("Reach Level 10");

        if (e.NewLevel == 50)
            UnlockAchievement("Reach Level 50");
    }

    private void OnPlayerDied(object? sender, PlayerDiedEventArgs e)
    {
        if (HasNeverDiedBefore())
            UnlockAchievement("First Death");
    }
}

// Analytics System
public class AnalyticsSystem
{
    public void SubscribeTo(Player player)
    {
        player.Damaged += (s, e) => TrackEvent("player_damaged", e.DamageAmount);
        player.LeveledUp += (s, e) => TrackEvent("player_leveled_up", e.NewLevel);
        player.Died += (s, e) => TrackEvent("player_died");
    }
}
```

**Benefits**:
- **Modular systems**: Each system is independent
- **Easy to add/remove**: Enable/disable systems without changing Player
- **Performance**: Systems process events concurrently
- **Maintainability**: Clear separation between game logic and presentation

---

## Performance Considerations

### Event Invocation Overhead

```
Operation                          | Time      | Allocations
-----------------------------------|-----------|-------------
Direct method call                 | 1.0 ns    | 0 B
Event with 0 subscribers           | 1.2 ns    | 0 B
Event with 1 subscriber            | 4.5 ns    | 0 B
Event with 5 subscribers           | 12.0 ns   | 0 B
Event with 10 subscribers          | 22.0 ns   | 0 B
Async event handler (void)         | 80.0 ns   | 88 B (Task)
Weak event subscription            | 45.0 ns   | 40 B (WeakReference)
```

### Memory Impact Per Subscription

```
Component                | Size
------------------------|-------
Delegate object         | 24 bytes
EventArgs (empty)       | 16 bytes
EventArgs (3 properties)| 40 bytes
Weak reference          | 32 bytes
```

### Optimization Strategies

#### 1. Reuse EventArgs When Possible

```csharp
// ❌ Bad - creates new object every time
private static readonly EventArgs EmptyArgs = new();

protected virtual void OnCompleted()
{
    Completed?.Invoke(this, new EventArgs());  // New allocation!
}

// ✅ Good - reuse static instance
protected virtual void OnCompleted()
{
    Completed?.Invoke(this, EventArgs.Empty);  // No allocation
}
```

#### 2. Avoid Raising Events in Tight Loops

```csharp
// ❌ Bad - raises event 1 million times
for (int i = 0; i < 1_000_000; i++)
{
    OnValueChanged(new ValueChangedEventArgs(i));
}

// ✅ Better - batch updates
var changes = new List<int>();
for (int i = 0; i < 1_000_000; i++)
{
    changes.Add(i);
}
OnBatchChanged(new BatchChangedEventArgs(changes));
```

#### 3. Use Weak Events for Long-Lived Publishers

```csharp
// Singleton service (lives forever)
public class ApplicationEventBus
{
    // ❌ Bad - holds strong references to all subscribers
    public event EventHandler<EventArgs>? GlobalEvent;

    // ✅ Good - weak references allow GC
    private readonly WeakEventManager<EventArgs> _weakManager = new();

    public event EventHandler<EventArgs> GlobalEvent
    {
        add => _weakManager.AddHandler(value);
        remove => _weakManager.RemoveHandler(value);
    }
}
```

#### 4. Unsubscribe in Dispose

```csharp
public class OrderMonitor : IDisposable
{
    private Order? _order;

    public void SubscribeTo(Order order)
    {
        _order = order;
        _order.StatusChanged += OnStatusChanged;
    }

    public void Dispose()
    {
        if (_order != null)
        {
            _order.StatusChanged -= OnStatusChanged;  // Critical!
            _order = null;
        }
    }
}
```

#### 5. Consider Sync vs Async Event Handlers

```csharp
// Synchronous - blocks publisher
order.StatusChanged += (s, e) =>
{
    Thread.Sleep(5000);  // Blocks for 5 seconds!
};

order.ChangeStatus(OrderStatus.Processing);  // Waits 5 seconds

// Asynchronous - doesn't block publisher
order.StatusChanged += async (s, e) =>
{
    await Task.Delay(5000);  // Doesn't block!
};

order.ChangeStatus(OrderStatus.Processing);  // Returns immediately
```

---

## Common Mistakes and Fixes

### Mistake 1: Forgetting to Unsubscribe (Memory Leaks) 💧

**Problem**:

```csharp
public class OrderMonitor
{
    public void StartMonitoring(Order order)
    {
        order.StatusChanged += OnStatusChanged;
        // ❌ Never unsubscribes - memory leak!
    }

    private void OnStatusChanged(object? sender, OrderStatusChangedEventArgs e)
    {
        Console.WriteLine($"Status changed: {e.NewStatus}");
    }
}

// Memory leak scenario
var order = new Order("ORD-001", "John");
for (int i = 0; i < 10000; i++)
{
    var monitor = new OrderMonitor();
    monitor.StartMonitoring(order);  // 10,000 subscriptions!
    // monitors go out of scope but can't be GC'd
}

// order holds references to all 10,000 monitors!
```

**Fix**:

```csharp
public class OrderMonitor : IDisposable
{
    private Order? _order;

    public void StartMonitoring(Order order)
    {
        _order = order;
        _order.StatusChanged += OnStatusChanged;
    }

    public void Dispose()
    {
        if (_order != null)
        {
            _order.StatusChanged -= OnStatusChanged;  // ✅ Unsubscribe
            _order = null;
        }
    }
}

// Proper usage
var order = new Order("ORD-001", "John");
using (var monitor = new OrderMonitor())
{
    monitor.StartMonitoring(order);
    // monitor.Dispose() called automatically
}
```

### Mistake 2: Raising Events Before Object Fully Constructed

**Problem**:

```csharp
public class Order
{
    public event EventHandler<OrderCreatedEventArgs>? OrderCreated;

    public string OrderId { get; }
    public string CustomerName { get; set; }  // Mutable

    public Order(string orderId)
    {
        OrderId = orderId;
        OnOrderCreated(new OrderCreatedEventArgs(orderId, CustomerName));
        // ❌ CustomerName is null! Object not fully initialized
        CustomerName = GetCustomerName(orderId);
    }
}

// Subscriber receives partially initialized object
order.OrderCreated += (s, e) =>
{
    Console.WriteLine($"Customer: {e.CustomerName}");  // null!
};
```

**Fix**:

```csharp
public class Order
{
    public Order(string orderId, string customerName)
    {
        OrderId = orderId;
        CustomerName = customerName;
        // ✅ All properties set before raising event
        OnOrderCreated(new OrderCreatedEventArgs(orderId, customerName));
    }
}
```

### Mistake 3: Not Handling Null Events

**Problem**:

```csharp
protected virtual void OnStatusChanged(OrderStatusChangedEventArgs e)
{
    StatusChanged.Invoke(this, e);  // ❌ NullReferenceException if no subscribers!
}
```

**Fix**:

```csharp
protected virtual void OnStatusChanged(OrderStatusChangedEventArgs e)
{
    StatusChanged?.Invoke(this, e);  // ✅ Null-conditional operator
}

// Or
protected virtual void OnStatusChanged(OrderStatusChangedEventArgs e)
{
    var handler = StatusChanged;
    if (handler != null)
    {
        handler(this, e);
    }
}
```

### Mistake 4: Exception in One Handler Stops Others

**Problem**:

```csharp
order.StatusChanged += (s, e) => Console.WriteLine("Handler 1");
order.StatusChanged += (s, e) => throw new Exception("Fail!");  // ❌ Breaks chain
order.StatusChanged += (s, e) => Console.WriteLine("Handler 3");  // Never executes

order.ChangeStatus(OrderStatus.Processing);
// Only "Handler 1" prints!
```

**Fix**:

```csharp
protected virtual void OnStatusChanged(OrderStatusChangedEventArgs e)
{
    var handler = StatusChanged;
    if (handler != null)
    {
        foreach (EventHandler<OrderStatusChangedEventArgs> subscriber in handler.GetInvocationList())
        {
            try
            {
                subscriber(this, e);  // ✅ Isolated execution
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Event handler failed");
                // Continue to next handler
            }
        }
    }
}
```

### Mistake 5: Using Mutable EventArgs

**Problem**:

```csharp
public class OrderStatusChangedEventArgs : EventArgs
{
    public OrderStatus NewStatus { get; set; }  // ❌ Mutable!
}

// Subscriber 1
order.StatusChanged += (s, e) =>
{
    if (e.NewStatus == OrderStatus.Cancelled)
    {
        e.NewStatus = OrderStatus.Pending;  // ❌ Modifies event args!
    }
};

// Subscriber 2
order.StatusChanged += (s, e) =>
{
    Console.WriteLine(e.NewStatus);  // Sees modified value!
};
```

**Fix**:

```csharp
public class OrderStatusChangedEventArgs : EventArgs
{
    public OrderStatus NewStatus { get; }  // ✅ Read-only!

    public OrderStatusChangedEventArgs(OrderStatus newStatus)
    {
        NewStatus = newStatus;
    }
}
```

---

## Migration Strategies

### Strategy 1: Gradual Migration (Low Risk)

**Step 1**: Add events alongside existing dependencies

```csharp
public class Order
{
    private readonly INotificationService _notification;  // Keep old dependency

    public event EventHandler<OrderStatusChangedEventArgs>? StatusChanged;  // Add event

    public void ChangeStatus(OrderStatus newStatus)
    {
        _status = newStatus;
        _notification.Notify(this);  // Old way (still works)
        OnStatusChanged(new OrderStatusChangedEventArgs(...));  // New way
    }
}
```

**Step 2**: Subscribe new services via events

```csharp
// Old service still uses constructor injection
var order = new Order("ORD-001", "John", notificationService);

// New services use events
loggingService.SubscribeTo(order);
analyticsService.SubscribeTo(order);
```

**Step 3**: Migrate old services to events

```csharp
public class NotificationService
{
    public void SubscribeTo(Order order)
    {
        order.StatusChanged += OnStatusChanged;
    }
}

// Remove constructor parameter
public class Order
{
    // private readonly INotificationService _notification;  // Remove

    public event EventHandler<OrderStatusChangedEventArgs>? StatusChanged;

    public void ChangeStatus(OrderStatus newStatus)
    {
        _status = newStatus;
        // _notification.Notify(this);  // Remove
        OnStatusChanged(new OrderStatusChangedEventArgs(...));
    }
}
```

### Strategy 2: Wrapper Pattern (Zero Risk)

```csharp
// Keep old class unchanged
public class LegacyOrder
{
    private readonly INotificationService _notification;

    public void ChangeStatus(OrderStatus newStatus)
    {
        _status = newStatus;
        _notification.Notify(this);
    }
}

// Create event-based wrapper
public class EventEnabledOrder : LegacyOrder
{
    public event EventHandler<OrderStatusChangedEventArgs>? StatusChanged;

    public new void ChangeStatus(OrderStatus newStatus)
    {
        base.ChangeStatus(newStatus);  // Call legacy code
        OnStatusChanged(new OrderStatusChangedEventArgs(...));  // Raise event
    }

    protected virtual void OnStatusChanged(OrderStatusChangedEventArgs e)
    {
        StatusChanged?.Invoke(this, e);
    }
}
```

---

## Comparison with Alternatives

### Alternative 1: Observer Pattern (Interface-Based)

**Interface-Based Observer**:

```csharp
public interface IOrderObserver
{
    void OnStatusChanged(Order order, OrderStatus oldStatus, OrderStatus newStatus);
}

public class Order
{
    private List<IOrderObserver> _observers = new();

    public void Attach(IOrderObserver observer) => _observers.Add(observer);
    public void Detach(IOrderObserver observer) => _observers.Remove(observer);

    public void ChangeStatus(OrderStatus newStatus)
    {
        var oldStatus = _status;
        _status = newStatus;

        foreach (var observer in _observers)
        {
            observer.OnStatusChanged(this, oldStatus, newStatus);
        }
    }
}

// Usage
public class NotificationService : IOrderObserver
{
    public void OnStatusChanged(Order order, OrderStatus oldStatus, OrderStatus newStatus)
    {
        // Handle notification
    }
}

order.Attach(new NotificationService());
```

**Event-Based**:

```csharp
public class Order
{
    public event EventHandler<OrderStatusChangedEventArgs>? StatusChanged;

    public void ChangeStatus(OrderStatus newStatus)
    {
        var oldStatus = _status;
        _status = newStatus;
        OnStatusChanged(new OrderStatusChangedEventArgs(OrderId, oldStatus, newStatus));
    }
}

// Usage
order.StatusChanged += (s, e) => NotifyCustomer(e);
```

| Aspect | Interface Observer | Events |
|--------|-------------------|--------|
| **Syntax** | Verbose | Concise |
| **Type Safety** | ✅ Compile-time | ✅ Compile-time |
| **Multicast** | Manual list management | Automatic |
| **Unsubscribe** | `Detach(observer)` | `event -= handler` |
| **Boilerplate** | High (interface + impl) | Low |
| **Anonymous handlers** | ❌ No | ✅ Yes (lambdas) |

**Verdict**: Events win for C# development. Interface-based observer is useful for cross-language scenarios.

### Alternative 2: Reactive Extensions (Rx)

**Rx (IObservable)**:

```csharp
public class Order
{
    private Subject<OrderStatusChangedEventArgs> _statusChangedSubject = new();

    public IObservable<OrderStatusChangedEventArgs> StatusChanged => _statusChangedSubject.AsObservable();

    public void ChangeStatus(OrderStatus newStatus)
    {
        var oldStatus = _status;
        _status = newStatus;
        _statusChangedSubject.OnNext(new OrderStatusChangedEventArgs(OrderId, oldStatus, newStatus));
    }
}

// Usage - powerful LINQ operators
order.StatusChanged
    .Where(e => e.NewStatus == OrderStatus.Shipped)
    .Throttle(TimeSpan.FromSeconds(1))
    .Subscribe(e => NotifyCustomer(e));
```

| Aspect | Events | Reactive Extensions |
|--------|--------|---------------------|
| **Learning curve** | Low | High |
| **LINQ operators** | ❌ No | ✅ Yes (powerful!) |
| **Async/await** | ✅ Native | ✅ Good integration |
| **Memory overhead** | Low | Medium (operators) |
| **Dependencies** | None | System.Reactive NuGet |

**Verdict**: Use Rx when you need advanced operators (throttle, buffer, merge, etc.). Use events for simple pub-sub.

### Alternative 3: Message Bus / Mediator

**MediatR / Event Bus**:

```csharp
public class OrderStatusChangedEvent : INotification
{
    public string OrderId { get; }
    public OrderStatus OldStatus { get; }
    public OrderStatus NewStatus { get; }
}

public class Order
{
    private readonly IMediator _mediator;

    public void ChangeStatus(OrderStatus newStatus)
    {
        var oldStatus = _status;
        _status = newStatus;
        _mediator.Publish(new OrderStatusChangedEvent(OrderId, oldStatus, newStatus));
    }
}

// Handlers registered separately
public class NotificationHandler : INotificationHandler<OrderStatusChangedEvent>
{
    public async Task Handle(OrderStatusChangedEvent notification, CancellationToken cancellationToken)
    {
        await NotifyCustomer(notification);
    }
}
```

| Aspect | Events | Message Bus |
|--------|--------|-------------|
| **Decoupling** | High | Very High |
| **Testability** | Good | Excellent |
| **Setup complexity** | Low | Medium (DI registration) |
| **Dependencies** | None | MediatR NuGet |
| **Async support** | Manual | Built-in |

**Verdict**: Use message bus for large applications with many handlers. Use events for simpler scenarios.

---

## Advanced Techniques

### 1. Weak Event Pattern (WPF)

```csharp
public class WeakEventManager<TEventSource, TEventArgs>
    where TEventSource : class
    where TEventArgs : EventArgs
{
    private readonly List<WeakReference<EventHandler<TEventArgs>>> _handlers = new();

    public void AddHandler(EventHandler<TEventArgs> handler)
    {
        _handlers.Add(new WeakReference<EventHandler<TEventArgs>>(handler));
    }

    public void RemoveHandler(EventHandler<TEventArgs> handler)
    {
        _handlers.RemoveAll(wr =>
        {
            if (wr.TryGetTarget(out var target))
                return target == handler;
            return true;  // Remove dead references
        });
    }

    public void RaiseEvent(TEventSource source, TEventArgs args)
    {
        var deadReferences = new List<WeakReference<EventHandler<TEventArgs>>>();

        foreach (var weakHandler in _handlers)
        {
            if (weakHandler.TryGetTarget(out var handler))
            {
                handler(source, args);
            }
            else
            {
                deadReferences.Add(weakHandler);
            }
        }

        // Clean up dead references
        foreach (var dead in deadReferences)
        {
            _handlers.Remove(dead);
        }
    }
}

// Usage
public class Order
{
    private readonly WeakEventManager<Order, OrderStatusChangedEventArgs> _weakEventManager = new();

    public event EventHandler<OrderStatusChangedEventArgs> StatusChanged
    {
        add => _weakEventManager.AddHandler(value);
        remove => _weakEventManager.RemoveHandler(value);
    }

    protected void OnStatusChanged(OrderStatusChangedEventArgs e)
    {
        _weakEventManager.RaiseEvent(this, e);
    }
}
```

### 2. Cancellable Events

```csharp
public class OrderCancellingEventArgs : EventArgs
{
    public string OrderId { get; }
    public bool Cancel { get; set; }  // Mutable for cancellation
    public string CancellationReason { get; set; } = "";

    public OrderCancellingEventArgs(string orderId)
    {
        OrderId = orderId;
    }
}

public class Order
{
    public event EventHandler<OrderCancellingEventArgs>? OrderCancelling;

    public bool TryCancel(string reason)
    {
        var args = new OrderCancellingEventArgs(OrderId);
        OnOrderCancelling(args);

        if (args.Cancel)
        {
            Console.WriteLine($"Cancellation prevented: {args.CancellationReason}");
            return false;
        }

        _status = OrderStatus.Cancelled;
        OnOrderCancelled(new OrderCancelledEventArgs(OrderId, reason));
        return true;
    }
}

// Usage - prevent cancellation
order.OrderCancelling += (s, e) =>
{
    if (IsPaymentProcessed())
    {
        e.Cancel = true;
        e.CancellationReason = "Payment already processed";
    }
};
```

### 3. Event Aggregator

```csharp
public class EventAggregator
{
    private readonly Dictionary<Type, List<Delegate>> _subscribers = new();

    public void Subscribe<TEventArgs>(Action<TEventArgs> handler)
        where TEventArgs : EventArgs
    {
        var type = typeof(TEventArgs);

        if (!_subscribers.ContainsKey(type))
            _subscribers[type] = new List<Delegate>();

        _subscribers[type].Add(handler);
    }

    public void Unsubscribe<TEventArgs>(Action<TEventArgs> handler)
        where TEventArgs : EventArgs
    {
        var type = typeof(TEventArgs);

        if (_subscribers.ContainsKey(type))
        {
            _subscribers[type].Remove(handler);
        }
    }

    public void Publish<TEventArgs>(TEventArgs eventArgs)
        where TEventArgs : EventArgs
    {
        var type = typeof(TEventArgs);

        if (_subscribers.ContainsKey(type))
        {
            foreach (var handler in _subscribers[type].Cast<Action<TEventArgs>>())
            {
                handler(eventArgs);
            }
        }
    }
}

// Usage
var aggregator = new EventAggregator();

// Subscribe from anywhere
aggregator.Subscribe<OrderCreatedEventArgs>(e =>
    Console.WriteLine($"Order created: {e.OrderId}"));

// Publish from anywhere
aggregator.Publish(new OrderCreatedEventArgs("ORD-001", "John", DateTime.Now));
```

---

## Summary

### Key Takeaways 🎯

1. **Events decouple** publishers from subscribers
2. **One-to-many** relationship built into the language
3. **EventHandler&lt;T&gt;** provides type safety
4. **Always unsubscribe** to prevent memory leaks
5. **Protected On*() methods** for raising events
6. **Null-conditional ?.** prevents NullReferenceException
7. **Events are synchronous** by default
8. **Weak events** prevent memory leaks for long-lived publishers
9. **IDisposable** pattern for cleanup
10. **Test events** separately from business logic

### When to Use Events ✅

- Publisher doesn't know subscribers
- Multiple subscribers (one-to-many)
- Asynchronous notifications
- Optional behavior (plug-ins)
- Separation of concerns
- UI updates (WPF, WinForms, Blazor)
- Game engines
- Stock trading / real-time data
- Audit logging

### When NOT to Use Events ❌

- Direct communication (request-response)
- Guaranteed processing required
- Single subscriber (one-to-one)
- Performance-critical hot paths
- Tight coupling is acceptable

### The Event Pattern 📋

```csharp
// Template for implementing events
public class Publisher
{
    // 1. Event declaration
    public event EventHandler<CustomEventArgs>? CustomEvent;

    // 2. Event raising method (protected virtual)
    protected virtual void OnCustomEvent(CustomEventArgs e)
    {
        var handler = CustomEvent;
        handler?.Invoke(this, e);
    }

    // 3. Trigger event when appropriate
    public void DoSomething()
    {
        // Business logic
        OnCustomEvent(new CustomEventArgs(...));
    }
}

// EventArgs class
public class CustomEventArgs : EventArgs
{
    public string Data { get; }

    public CustomEventArgs(string data)
    {
        Data = data;
    }
}

// Subscriber
public class Subscriber
{
    public void SubscribeTo(Publisher publisher)
    {
        publisher.CustomEvent += OnCustomEvent;
    }

    private void OnCustomEvent(object? sender, CustomEventArgs e)
    {
        // Handle event
    }
}
```

---

Events are a fundamental feature of C# that enable **loose coupling**, **scalability**, and **maintainability**. Use them to build flexible, extensible applications that can evolve without breaking existing code. Just remember: **with great power comes great responsibility** - always unsubscribe to prevent memory leaks!
