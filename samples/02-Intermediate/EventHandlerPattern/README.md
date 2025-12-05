# Event Handler Pattern 🎯

[![.NET Version](https://img.shields.io/badge/.NET-8.0-512BD4)](https://dotnet.microsoft.com/)
[![C# Version](https://img.shields.io/badge/C%23-12.0-239120)](https://docs.microsoft.com/en-us/dotnet/csharp/)
[![License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)
[![Build Status](https://img.shields.io/badge/build-passing-brightgreen.svg)]()

A comprehensive demonstration of the Event Handler Pattern in C# using a real-world **Order Tracking System**. This example showcases event-driven architecture, custom EventArgs, multiple subscribers, and best practices for memory management.

## 📊 Project Statistics

| Metric | Value |
|--------|-------|
| **Total Lines of Code** | ~757 lines |
| **Number of Events** | 4 (OrderCreated, StatusChanged, OrderCancelled, ItemAdded) |
| **Event Subscribers** | 6 services (Notification, Logging, Inventory, Email, Temp, Weak) |
| **Demonstration Scenarios** | 6 complete workflows |
| **Build Status** | ✅ 0 Errors |
| **.NET Version** | 8.0 LTS |
| **C# Language Version** | 12.0 |

## 🎯 Key Features

### 1. Event Declarations with EventHandler&lt;T&gt;
- **Strongly-typed events** using `EventHandler<TEventArgs>`
- **Custom EventArgs** for each event type
- **Nullable event handlers** with modern C# syntax

### 2. Event Raising Best Practices
- **Thread-safe** event raising using null-conditional operator
- **Protected virtual methods** for event raising
- **Exception handling** in event handlers
- **Local copy** of event delegate before invocation

### 3. Multiple Event Subscribers
- **Decoupled architecture** with multiple listeners
- **NotificationService** - User notifications
- **LoggingService** - Audit trail
- **InventoryService** - Stock management
- **EmailService** - Customer communications

### 4. Memory Leak Prevention
- **Proper unsubscription** patterns
- **Weak event pattern** demonstration
- **IDisposable** implementation for cleanup

### 5. Real-World Order Workflow
- Complete order lifecycle: Pending → Processing → Shipped → Delivered
- Order cancellation with reason tracking
- Item addition with inventory reservation

### 6. Exception Handling
- Demonstrates exception propagation in event handlers
- Shows how one failing handler affects others

## 🚀 Quick Start

### Prerequisites
```bash
# .NET 8.0 SDK or later
dotnet --version  # Should show 8.0.x or higher
```

### Build and Run
```bash
# Navigate to the project directory
cd samples/02-Intermediate/EventHandlerPattern

# Build the project
dotnet build

# Run the application
dotnet run
```

### Expected Output
```
=== Event Handler Pattern Demo ===

1. BASIC EVENT USAGE
✓ Event: Order created - ID: ORD-001, Customer: John Doe
✓ Event: Status changed from Pending to Processing
✓ Event: Status changed from Processing to Shipped
   Final Order Total: $1250.00

2. MULTIPLE EVENT SUBSCRIBERS
   [Notification] Item added to ORD-002: Keyboard
   [Inventory] Reserved stock: Keyboard x1
   [Log] 12:34:56 - Order created: ORD-002
   Total notifications sent: 4
   Total logs written: 2

3. EVENT UNSUBSCRIPTION (Memory Leak Prevention)
✓ Temp-Service-1 subscribed to order events
✓ Temp-Service-1 unsubscribed from order events
   Proper unsubscription prevents memory leaks!
```

## 📚 Core Concepts

### 1. Event Declaration

Events in C# are declared using the `event` keyword with a delegate type:

```csharp
public class Order
{
    // Standard event pattern using EventHandler<T>
    public event EventHandler<OrderCreatedEventArgs>? OrderCreated;
    public event EventHandler<OrderStatusChangedEventArgs>? StatusChanged;
    public event EventHandler<OrderCancelledEventArgs>? OrderCancelled;
    public event EventHandler<OrderItemAddedEventArgs>? ItemAdded;
}
```

**Why EventHandler&lt;T&gt;?**
- Type-safe event data
- Consistent signature across all events
- IntelliSense support for event args
- Better refactoring support

### 2. Custom EventArgs Classes

All event data classes inherit from `EventArgs`:

```csharp
public class OrderCreatedEventArgs : EventArgs
{
    public string OrderId { get; }
    public string CustomerName { get; }
    public DateTime CreatedAt { get; }

    public OrderCreatedEventArgs(string orderId, string customerName, DateTime createdAt)
    {
        OrderId = orderId;
        CustomerName = customerName;
        CreatedAt = createdAt;
    }
}
```

**Best Practices:**
- ✅ Make properties **read-only** (get-only)
- ✅ Initialize in **constructor**
- ✅ Include **timestamp** for audit trail
- ✅ Inherit from **EventArgs**

### 3. Event Raising (The Right Way)

```csharp
protected virtual void OnOrderCreated(OrderCreatedEventArgs e)
{
    // Best Practice 1: Copy event reference to local variable
    var handler = OrderCreated;

    // Best Practice 2: Null-conditional operator for thread safety
    handler?.Invoke(this, e);
}
```

**Why this pattern?**
1. **Thread Safety**: Local copy prevents race conditions
2. **Null Safety**: `?.` prevents NullReferenceException
3. **Virtual**: Allows derived classes to override
4. **Protected**: Encapsulates event raising logic

### 4. Event Subscription

```csharp
// Subscribe to an event
order.OrderCreated += OnOrderCreated;
order.StatusChanged += OnStatusChanged;

// Event handler method
private void OnOrderCreated(object? sender, OrderCreatedEventArgs e)
{
    Console.WriteLine($"Order {e.OrderId} created for {e.CustomerName}");
}
```

### 5. Event Unsubscription (Critical!)

```csharp
public class TemporarySubscriber : IDisposable
{
    private Order? _order;

    public void SubscribeTo(Order order)
    {
        _order = order;
        _order.ItemAdded += OnItemAdded;
    }

    public void Dispose()
    {
        if (_order != null)
        {
            _order.ItemAdded -= OnItemAdded;  // ⚠️ Always unsubscribe!
            _order = null;
        }
    }

    private void OnItemAdded(object? sender, OrderItemAddedEventArgs e)
    {
        // Handle event
    }
}
```

**Why unsubscribe?**
- Prevents **memory leaks**
- Publisher holds reference to subscriber
- Subscriber can't be garbage collected
- Use `IDisposable` for cleanup

## 💡 Usage Examples

### Example 1: Basic Event Usage

```csharp
// Create order (fires OrderCreated event)
var order = new Order("ORD-001", "John Doe");

// Subscribe to status changes
order.StatusChanged += (sender, e) =>
{
    Console.WriteLine($"Status: {e.OldStatus} → {e.NewStatus}");
};

// Add items (fires ItemAdded event)
order.AddItem(new OrderItem("Laptop", 1200m, 1));

// Change status (fires StatusChanged event)
order.ChangeStatus(OrderStatus.Processing);
```

**Output:**
```
Status: Pending → Processing
```

### Example 2: Multiple Subscribers

```csharp
var order = new Order("ORD-002", "Jane Smith");

// Multiple services listening to the same event
var notification = new NotificationService();
var logging = new LoggingService();
var inventory = new InventoryService();

notification.SubscribeTo(order);
logging.SubscribeTo(order);
inventory.SubscribeTo(order);

// One event triggers all three services
order.AddItem(new OrderItem("Mouse", 25m, 1));

// Output:
// [Notification] Item added to ORD-002: Mouse
// [Inventory] Reserved stock: Mouse x1
// (No log because LoggingService doesn't subscribe to ItemAdded)
```

### Example 3: Complete Order Workflow

```csharp
var processor = new OrderProcessor();
var order = processor.CreateOrder("ORD-003", "Bob Johnson");

// Add items
processor.AddItemToOrder(order, new OrderItem("Phone", 800m, 1));
processor.AddItemToOrder(order, new OrderItem("Case", 15m, 1));

// Process through workflow
processor.ProcessOrder(order);   // Pending → Processing
processor.ShipOrder(order);      // Processing → Shipped
processor.CompleteOrder(order);  // Shipped → Delivered

Console.WriteLine($"Order complete! Total: ${order.CalculateTotal():F2}");
// Output: Order complete! Total: $815.00
```

### Example 4: Event Cancellation

```csharp
var order = new Order("ORD-004", "Alice Brown");

// Subscribe to cancellation event
order.OrderCancelled += (sender, e) =>
{
    Console.WriteLine($"Order {e.OrderId} cancelled: {e.Reason}");
    // Send refund, release inventory, notify customer, etc.
};

order.AddItem(new OrderItem("Tablet", 400m, 1));
order.Cancel("Customer requested cancellation");

// Output:
// Order ORD-004 cancelled: Customer requested cancellation
```

### Example 5: Exception Handling in Events

```csharp
var order = new Order("ORD-005", "Charlie Davis");

// Handler 1 (works fine)
order.StatusChanged += (sender, e) =>
{
    Console.WriteLine($"Handler 1: {e.NewStatus}");
};

// Handler 2 (throws exception)
order.StatusChanged += (sender, e) =>
{
    Console.WriteLine($"Handler 2: {e.NewStatus}");
    throw new InvalidOperationException("Handler 2 failed!");
};

// Handler 3 (never executes if Handler 2 throws)
order.StatusChanged += (sender, e) =>
{
    Console.WriteLine($"Handler 3: {e.NewStatus}");
};

try
{
    order.ChangeStatus(OrderStatus.Processing);
}
catch (Exception ex)
{
    Console.WriteLine($"Exception: {ex.Message}");
}

// Output:
// Handler 1: Processing
// Handler 2: Processing
// Exception: Handler 2 failed!
// Note: Handler 3 never executes!
```

**Lesson**: One failing handler can break the entire event chain.

### Example 6: Weak Event Pattern (Advanced)

```csharp
var order = new Order("ORD-006", "David Wilson");
var subscriber = new WeakEventSubscriber("Service-1");

// Subscribe using weak reference
WeakEventManager.Subscribe(order, nameof(order.StatusChanged), subscriber.OnStatusChanged);

order.ChangeStatus(OrderStatus.Processing);

// subscriber can be garbage collected even if order is still alive
subscriber = null;
GC.Collect();

order.ChangeStatus(OrderStatus.Shipped);
// subscriber no longer receives events
```

## 🏗️ Architecture

### Event Flow Diagram

```
┌─────────────┐
│   Order     │ (Event Publisher)
│             │
│ • OrderCreated
│ • StatusChanged
│ • OrderCancelled
│ • ItemAdded
└──────┬──────┘
       │ fires events
       ├───────────────┬─────────────┬─────────────┐
       │               │             │             │
       ▼               ▼             ▼             ▼
┌──────────────┐ ┌───────────┐ ┌──────────┐ ┌──────────┐
│Notification  │ │ Logging   │ │Inventory │ │  Email   │
│  Service     │ │ Service   │ │ Service  │ │ Service  │
└──────────────┘ └───────────┘ └──────────┘ └──────────┘
(Event Subscribers - loosely coupled)
```

### Order State Machine

```
┌─────────┐
│ Pending │ (Initial state)
└────┬────┘
     │ ChangeStatus(Processing)
     ▼
┌────────────┐
│ Processing │
└─────┬──────┘
      │ ChangeStatus(Shipped)
      ▼
┌──────────┐
│ Shipped  │
└─────┬────┘
      │ ChangeStatus(Delivered)
      ▼
┌───────────┐
│ Delivered │ (Final state)
└───────────┘

Note: Can transition to Cancelled from any state except Delivered
```

## 🔍 Best Practices

### DO ✅

1. **Use EventHandler&lt;T&gt; for type safety**
   ```csharp
   public event EventHandler<OrderCreatedEventArgs>? OrderCreated;
   ```

2. **Make EventArgs properties read-only**
   ```csharp
   public class OrderCreatedEventArgs : EventArgs
   {
       public string OrderId { get; }  // Read-only
       public OrderCreatedEventArgs(string orderId) => OrderId = orderId;
   }
   ```

3. **Raise events with protected virtual methods**
   ```csharp
   protected virtual void OnOrderCreated(OrderCreatedEventArgs e)
   {
       var handler = OrderCreated;
       handler?.Invoke(this, e);
   }
   ```

4. **Always unsubscribe from events**
   ```csharp
   public void Dispose()
   {
       order.OrderCreated -= OnOrderCreated;
   }
   ```

5. **Use meaningful EventArgs names**
   ```csharp
   OrderCreatedEventArgs    // ✅ Clear purpose
   OrderStatusChangedEventArgs  // ✅ Descriptive
   ```

### DON'T ❌

1. **Don't expose event raising publicly**
   ```csharp
   public void RaiseOrderCreated() { ... }  // ❌ Bad
   protected virtual void OnOrderCreated() { ... }  // ✅ Good
   ```

2. **Don't forget null checks**
   ```csharp
   OrderCreated.Invoke(this, e);  // ❌ May throw NullReferenceException
   OrderCreated?.Invoke(this, e); // ✅ Safe
   ```

3. **Don't use EventArgs.Empty for custom data**
   ```csharp
   OrderCreated?.Invoke(this, EventArgs.Empty);  // ❌ Lost data
   OrderCreated?.Invoke(this, new OrderCreatedEventArgs(...));  // ✅ Proper
   ```

4. **Don't raise events in constructors (usually)**
   ```csharp
   public Order(string id)
   {
       OrderId = id;
       OnOrderCreated(new OrderCreatedEventArgs(id));  // ⚠️ Risky
       // Object not fully constructed yet!
   }
   ```

5. **Don't forget to handle exceptions in subscribers**
   ```csharp
   order.StatusChanged += (s, e) =>
   {
       // ❌ No exception handling - can break other subscribers
       SendEmail(e.OrderId);
   };

   order.StatusChanged += (s, e) =>
   {
       try
       {
           SendEmail(e.OrderId);  // ✅ Protected
       }
       catch (Exception ex)
       {
           _logger.LogError(ex, "Failed to send email");
       }
   };
   ```

## 🎭 Common Patterns

### 1. Publisher-Subscriber Pattern

**Purpose**: Decouples event source from event consumers.

```csharp
// Publisher
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

// Subscriber 1
public class NotificationService
{
    public void SubscribeTo(Order order)
    {
        order.StatusChanged += (s, e) => NotifyCustomer(e);
    }
}

// Subscriber 2
public class LoggingService
{
    public void SubscribeTo(Order order)
    {
        order.StatusChanged += (s, e) => LogStatusChange(e);
    }
}
```

### 2. Event Aggregator Pattern

**Purpose**: Centralizes event handling across multiple publishers.

```csharp
public class EventAggregator
{
    private readonly Dictionary<Type, List<Delegate>> _subscribers = new();

    public void Subscribe<T>(Action<T> handler) where T : EventArgs
    {
        var type = typeof(T);
        if (!_subscribers.ContainsKey(type))
            _subscribers[type] = new List<Delegate>();

        _subscribers[type].Add(handler);
    }

    public void Publish<T>(T eventArgs) where T : EventArgs
    {
        var type = typeof(T);
        if (_subscribers.ContainsKey(type))
        {
            foreach (var handler in _subscribers[type].Cast<Action<T>>())
            {
                handler(eventArgs);
            }
        }
    }
}

// Usage
var aggregator = new EventAggregator();
aggregator.Subscribe<OrderCreatedEventArgs>(e => Console.WriteLine($"Order created: {e.OrderId}"));
aggregator.Publish(new OrderCreatedEventArgs("ORD-001", "John", DateTime.Now));
```

### 3. Weak Event Pattern

**Purpose**: Prevents memory leaks by allowing subscribers to be garbage collected.

```csharp
public class WeakEventManager<TEventArgs> where TEventArgs : EventArgs
{
    private readonly List<WeakReference<EventHandler<TEventArgs>>> _handlers = new();

    public void AddHandler(EventHandler<TEventArgs> handler)
    {
        _handlers.Add(new WeakReference<EventHandler<TEventArgs>>(handler));
    }

    public void Raise(object sender, TEventArgs e)
    {
        var handlersToRemove = new List<WeakReference<EventHandler<TEventArgs>>>();

        foreach (var weakHandler in _handlers)
        {
            if (weakHandler.TryGetTarget(out var handler))
            {
                handler(sender, e);
            }
            else
            {
                handlersToRemove.Add(weakHandler);
            }
        }

        foreach (var handler in handlersToRemove)
        {
            _handlers.Remove(handler);
        }
    }
}
```

### 4. Cancellable Events Pattern

**Purpose**: Allows subscribers to cancel an operation.

```csharp
public class OrderCancellingEventArgs : EventArgs
{
    public string OrderId { get; }
    public bool Cancel { get; set; }  // Mutable for cancellation

    public OrderCancellingEventArgs(string orderId)
    {
        OrderId = orderId;
        Cancel = false;
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
            Console.WriteLine("Cancellation prevented by subscriber");
            return false;
        }

        // Proceed with cancellation
        _status = OrderStatus.Cancelled;
        return true;
    }
}

// Usage
order.OrderCancelling += (s, e) =>
{
    if (IsPaymentProcessed())
    {
        e.Cancel = true;  // Prevent cancellation
    }
};
```

## 🎯 Use Cases

### 1. UI Updates (WPF, WinForms)
```csharp
public class OrderViewModel
{
    private Order _order;

    public OrderViewModel(Order order)
    {
        _order = order;
        _order.StatusChanged += OnOrderStatusChanged;
    }

    private void OnOrderStatusChanged(object? sender, OrderStatusChangedEventArgs e)
    {
        // Update UI
        StatusLabel.Text = e.NewStatus.ToString();
        RaisePropertyChanged(nameof(Status));
    }
}
```

### 2. Audit Logging
```csharp
public class AuditLogger
{
    public void SubscribeTo(Order order)
    {
        order.OrderCreated += LogOrderCreated;
        order.StatusChanged += LogStatusChanged;
        order.OrderCancelled += LogOrderCancelled;
    }

    private void LogOrderCreated(object? sender, OrderCreatedEventArgs e)
    {
        _db.AuditLogs.Add(new AuditLog
        {
            Timestamp = e.CreatedAt,
            Action = "OrderCreated",
            OrderId = e.OrderId,
            Details = $"Customer: {e.CustomerName}"
        });
    }
}
```

### 3. Real-time Notifications
```csharp
public class RealTimeNotificationService
{
    private readonly IHubContext<NotificationHub> _hubContext;

    public void SubscribeTo(Order order)
    {
        order.StatusChanged += async (s, e) =>
        {
            await _hubContext.Clients.User(order.CustomerId)
                .SendAsync("OrderStatusChanged", e);
        };
    }
}
```

### 4. Workflow Automation
```csharp
public class WorkflowEngine
{
    public void SubscribeTo(Order order)
    {
        order.StatusChanged += async (s, e) =>
        {
            switch (e.NewStatus)
            {
                case OrderStatus.Processing:
                    await ReserveInventoryAsync(order);
                    await ChargePaymentAsync(order);
                    break;

                case OrderStatus.Shipped:
                    await SendShippingNotificationAsync(order);
                    await UpdateTrackingInfoAsync(order);
                    break;

                case OrderStatus.Delivered:
                    await RequestReviewAsync(order);
                    await CompleteTransactionAsync(order);
                    break;
            }
        };
    }
}
```

## 🚨 Common Pitfalls

### Pitfall 1: Memory Leaks from Forgotten Unsubscriptions

**Problem:**
```csharp
public class OrderMonitor
{
    public void StartMonitoring(Order order)
    {
        order.StatusChanged += OnStatusChanged;
        // ❌ Never unsubscribes!
    }

    private void OnStatusChanged(object? sender, OrderStatusChangedEventArgs e)
    {
        Console.WriteLine($"Status changed: {e.NewStatus}");
    }
}

// Usage
var order = new Order("ORD-001", "John");
for (int i = 0; i < 1000; i++)
{
    var monitor = new OrderMonitor();
    monitor.StartMonitoring(order);  // ❌ Creates 1000 subscriptions!
}
```

**Solution:**
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
            _order.StatusChanged -= OnStatusChanged;  // ✅ Clean up
            _order = null;
        }
    }
}
```

### Pitfall 2: Raising Events Before Object is Fully Constructed

**Problem:**
```csharp
public class Order
{
    public event EventHandler<OrderCreatedEventArgs>? OrderCreated;

    public Order(string orderId)
    {
        OnOrderCreated(new OrderCreatedEventArgs(orderId, "", DateTime.Now));
        // ❌ Raised before CustomerName is set!
        CustomerName = GetCustomerName(orderId);
    }
}
```

**Solution:**
```csharp
public class Order
{
    public Order(string orderId, string customerName)
    {
        OrderId = orderId;
        CustomerName = customerName;
        // ✅ All properties set before raising event
        OnOrderCreated(new OrderCreatedEventArgs(orderId, customerName, DateTime.Now));
    }
}
```

### Pitfall 3: Exception in One Handler Stops Others

**Problem:**
```csharp
order.StatusChanged += (s, e) => { Console.WriteLine("Handler 1"); };
order.StatusChanged += (s, e) => { throw new Exception(); };  // ❌ Breaks chain
order.StatusChanged += (s, e) => { Console.WriteLine("Handler 3"); };  // Never executes

order.ChangeStatus(OrderStatus.Processing);
// Only "Handler 1" is printed!
```

**Solution:**
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
                subscriber(this, e);  // ✅ Each handler isolated
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Event handler failed");
            }
        }
    }
}
```

## 📈 Performance Considerations

### Event Raising Overhead

```
Operation                          | Time     | Notes
-----------------------------------|----------|------------------
Direct method call                 | 1.0x     | Baseline
Event with 1 subscriber            | 1.5x     | Delegate overhead
Event with 10 subscribers          | 2.0x     | Invocation list
Event with weak reference          | 3.0x     | GC tracking
```

### Memory Impact

- Each event subscription: ~24 bytes (delegate object)
- EventArgs object: Variable (depends on data)
- Weak reference: +8 bytes per subscription

### Optimization Tips

1. **Avoid frequent event raising in tight loops**
   ```csharp
   // ❌ Bad
   for (int i = 0; i < 1000000; i++)
   {
       OnValueChanged(new ValueChangedEventArgs(i));
   }

   // ✅ Better
   for (int i = 0; i < 1000000; i++)
   {
       // Process
   }
   OnBatchCompleted(new BatchCompletedEventArgs(1000000));
   ```

2. **Reuse EventArgs when possible**
   ```csharp
   private static readonly EventArgs Empty = EventArgs.Empty;

   protected virtual void OnCompleted()
   {
       Completed?.Invoke(this, Empty);  // ✅ No allocation
   }
   ```

3. **Use weak events for long-lived publishers**
   ```csharp
   // Singleton event publisher with many short-lived subscribers
   public class ApplicationEvents
   {
       private static readonly WeakEventManager<OrderCreatedEventArgs> _orderCreated = new();

       public static event EventHandler<OrderCreatedEventArgs> OrderCreated
       {
           add => _orderCreated.AddHandler(value);
           remove => _orderCreated.RemoveHandler(value);
       }
   }
   ```

## 🔗 Related Patterns

### Observer Pattern
The Event Handler Pattern is a C# implementation of the Observer pattern:

| Aspect | Observer (GoF) | Event Handler (C#) |
|--------|---------------|-------------------|
| Subject | Observable class | Class with events |
| Observer | Observer interface | Event subscriber |
| Attach | `Attach(observer)` | `event += handler` |
| Detach | `Detach(observer)` | `event -= handler` |
| Notify | `NotifyObservers()` | `event?.Invoke()` |

### Command Pattern
Events can trigger commands:

```csharp
public class OrderCommandInvoker
{
    public void SubscribeTo(Order order)
    {
        order.StatusChanged += (s, e) =>
        {
            var command = CreateCommand(e.NewStatus);
            command.Execute();
        };
    }
}
```

### Mediator Pattern
Event aggregator acts as mediator:

```csharp
public class OrderMediator
{
    public event EventHandler<OrderEventArgs>? OrderEvent;

    public void PublishOrderCreated(Order order)
    {
        OrderEvent?.Invoke(this, new OrderEventArgs(order, "Created"));
    }
}
```

## 📖 Additional Resources

- [C# Events Documentation](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/events/)
- [Event Pattern Guidelines](https://docs.microsoft.com/en-us/dotnet/csharp/event-pattern)
- [Weak Event Pattern](https://docs.microsoft.com/en-us/dotnet/desktop/wpf/advanced/weak-event-patterns)
- [Observer Pattern (GoF)](https://refactoring.guru/design-patterns/observer)

## 🎓 Learning Path

1. **Start with basic events** - Understand syntax and mechanics
2. **Create custom EventArgs** - Learn event data patterns
3. **Practice unsubscription** - Prevent memory leaks
4. **Implement multiple subscribers** - See decoupling benefits
5. **Handle exceptions** - Build robust event handlers
6. **Explore weak events** - Advanced memory management

## 📝 Summary

| Feature | Description | Benefit |
|---------|-------------|---------|
| **EventHandler&lt;T&gt;** | Strongly-typed events | Type safety, IntelliSense |
| **Custom EventArgs** | Event-specific data | Clear contracts |
| **Protected On*() methods** | Encapsulated raising | Inheritance support |
| **Null-conditional ?.** | Safe invocation | No NullReferenceException |
| **Unsubscription** | -= operator | Memory leak prevention |
| **Weak events** | Weak references | GC-friendly subscriptions |

## 🎯 Key Takeaways

1. ✅ **Events decouple** publishers from subscribers
2. ✅ **Always unsubscribe** to prevent memory leaks
3. ✅ **Use EventHandler&lt;T&gt;** for consistency
4. ✅ **Make EventArgs immutable** (read-only properties)
5. ✅ **Raise events safely** with null-conditional operator
6. ✅ **Handle exceptions** in subscribers
7. ✅ **Consider weak events** for long-lived publishers
8. ✅ **Follow naming conventions**: On*MethodName*
9. ✅ **Protect event raising** with virtual methods
10. ✅ **Document event contracts** clearly

---

**Built with ❤️ using C# 12 and .NET 8**
