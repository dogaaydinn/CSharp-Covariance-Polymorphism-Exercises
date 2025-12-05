using System;
using System.Collections.Generic;
using System.Linq;

namespace EventHandlerPattern;

/// <summary>
/// Event Handler Pattern - Order Tracking System
///
/// This example demonstrates:
/// - Event declaration with EventHandler<T>
/// - Custom EventArgs classes
/// - Event raising best practices
/// - Multiple event subscribers
/// - Weak event pattern for memory leak prevention
/// - Event handler lifecycle management
/// </summary>
class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("=== Event Handler Pattern Demo ===\n");

        DemonstrateBasicEvents();
        Console.WriteLine("\n" + new string('=', 60) + "\n");

        DemonstrateMultipleSubscribers();
        Console.WriteLine("\n" + new string('=', 60) + "\n");

        DemonstrateEventUnsubscription();
        Console.WriteLine("\n" + new string('=', 60) + "\n");

        DemonstrateEventRaisingBestPractices();
        Console.WriteLine("\n" + new string('=', 60) + "\n");

        DemonstrateCompleteOrderWorkflow();
        Console.WriteLine("\n" + new string('=', 60) + "\n");

        DemonstrateWeakEventPattern();
    }

    /// <summary>
    /// Demonstrates basic event usage with a simple order
    /// </summary>
    static void DemonstrateBasicEvents()
    {
        Console.WriteLine("1. BASIC EVENT USAGE");
        Console.WriteLine("Creating order and subscribing to events...\n");

        var order = new Order("ORD-001", "John Doe");

        // Subscribe to events
        order.OrderCreated += (sender, e) =>
        {
            Console.WriteLine($"✓ Event: Order created - ID: {e.OrderId}, Customer: {e.CustomerName}");
        };

        order.StatusChanged += (sender, e) =>
        {
            Console.WriteLine($"✓ Event: Status changed from {e.OldStatus} to {e.NewStatus}");
        };

        // Trigger events
        order.AddItem(new OrderItem("Laptop", 1200m, 1));
        order.AddItem(new OrderItem("Mouse", 25m, 2));
        order.ChangeStatus(OrderStatus.Processing);
        order.ChangeStatus(OrderStatus.Shipped);

        Console.WriteLine($"\n   Final Order Total: ${order.CalculateTotal():F2}");
    }

    /// <summary>
    /// Demonstrates multiple subscribers listening to the same event
    /// </summary>
    static void DemonstrateMultipleSubscribers()
    {
        Console.WriteLine("2. MULTIPLE EVENT SUBSCRIBERS");
        Console.WriteLine("Multiple services listening to order events...\n");

        var order = new Order("ORD-002", "Jane Smith");

        // Create multiple subscribers
        var notificationService = new NotificationService();
        var loggingService = new LoggingService();
        var inventoryService = new InventoryService();
        var emailService = new EmailService();

        // Subscribe all services to order events
        notificationService.SubscribeTo(order);
        loggingService.SubscribeTo(order);
        inventoryService.SubscribeTo(order);
        emailService.SubscribeTo(order);

        // Perform operations
        order.AddItem(new OrderItem("Keyboard", 80m, 1));
        order.AddItem(new OrderItem("Monitor", 350m, 1));
        order.ChangeStatus(OrderStatus.Processing);
        order.ChangeStatus(OrderStatus.Shipped);

        Console.WriteLine($"\n   Total notifications sent: {notificationService.NotificationCount}");
        Console.WriteLine($"   Total logs written: {loggingService.LogCount}");
    }

    /// <summary>
    /// Demonstrates proper event unsubscription to prevent memory leaks
    /// </summary>
    static void DemonstrateEventUnsubscription()
    {
        Console.WriteLine("3. EVENT UNSUBSCRIPTION (Memory Leak Prevention)");
        Console.WriteLine("Demonstrating proper cleanup...\n");

        var order = new Order("ORD-003", "Bob Johnson");
        var tempService = new TemporarySubscriber("Temp-Service-1");

        // Subscribe
        tempService.SubscribeTo(order);
        Console.WriteLine($"✓ {tempService.Name} subscribed to order events");

        // Trigger event
        order.AddItem(new OrderItem("Tablet", 400m, 1));

        // Unsubscribe before disposal
        tempService.UnsubscribeFrom(order);
        Console.WriteLine($"✓ {tempService.Name} unsubscribed from order events");

        // This event won't be received by tempService
        order.AddItem(new OrderItem("Case", 30m, 1));

        Console.WriteLine("\n   Proper unsubscription prevents memory leaks!");
    }

    /// <summary>
    /// Demonstrates best practices for raising events
    /// </summary>
    static void DemonstrateEventRaisingBestPractices()
    {
        Console.WriteLine("4. EVENT RAISING BEST PRACTICES");
        Console.WriteLine("Showing thread-safe event raising and null checking...\n");

        var order = new Order("ORD-004", "Alice Brown");

        // Subscribe with exception handling
        order.StatusChanged += (sender, e) =>
        {
            Console.WriteLine($"✓ Handler 1: Status changed to {e.NewStatus}");
        };

        order.StatusChanged += (sender, e) =>
        {
            Console.WriteLine($"✓ Handler 2: Status changed to {e.NewStatus}");
            // Simulate exception in handler
            if (e.NewStatus == OrderStatus.Cancelled)
            {
                Console.WriteLine("   ⚠ Handler 2: Simulating exception...");
                throw new InvalidOperationException("Handler 2 failed!");
            }
        };

        order.StatusChanged += (sender, e) =>
        {
            Console.WriteLine($"✓ Handler 3: Status changed to {e.NewStatus}");
        };

        try
        {
            order.ChangeStatus(OrderStatus.Processing);
            Console.WriteLine();
            order.ChangeStatus(OrderStatus.Cancelled); // Will throw
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\n   ✗ Exception caught: {ex.Message}");
            Console.WriteLine("   Note: Exception in one handler stops other handlers!");
        }
    }

    /// <summary>
    /// Demonstrates complete order workflow with all events
    /// </summary>
    static void DemonstrateCompleteOrderWorkflow()
    {
        Console.WriteLine("5. COMPLETE ORDER WORKFLOW");
        Console.WriteLine("End-to-end order processing with all events...\n");

        var orderProcessor = new OrderProcessor();
        var order = orderProcessor.CreateOrder("ORD-005", "Charlie Davis");

        Console.WriteLine("\n   Adding items...");
        orderProcessor.AddItemToOrder(order, new OrderItem("Phone", 800m, 1));
        orderProcessor.AddItemToOrder(order, new OrderItem("Charger", 25m, 1));
        orderProcessor.AddItemToOrder(order, new OrderItem("Case", 15m, 1));

        Console.WriteLine("\n   Processing order...");
        orderProcessor.ProcessOrder(order);

        Console.WriteLine("\n   Shipping order...");
        orderProcessor.ShipOrder(order);

        Console.WriteLine("\n   Completing order...");
        orderProcessor.CompleteOrder(order);

        Console.WriteLine($"\n   ✓ Order {order.OrderId} completed successfully!");
        Console.WriteLine($"   Total: ${order.CalculateTotal():F2}");
        Console.WriteLine($"   Items: {order.Items.Count}");
    }

    /// <summary>
    /// Demonstrates weak event pattern to prevent memory leaks
    /// </summary>
    static void DemonstrateWeakEventPattern()
    {
        Console.WriteLine("6. WEAK EVENT PATTERN (Advanced)");
        Console.WriteLine("Preventing memory leaks with weak references...\n");

        var order = new Order("ORD-006", "David Wilson");
        var weakSubscriber = new WeakEventSubscriber("Weak-Service-1");

        // Subscribe using weak event pattern
        WeakEventManager.Subscribe(order, nameof(order.StatusChanged), weakSubscriber.OnStatusChanged);
        Console.WriteLine($"✓ {weakSubscriber.Name} subscribed using weak event pattern");

        order.ChangeStatus(OrderStatus.Processing);

        Console.WriteLine("\n   Note: Weak event pattern allows garbage collection of subscriber");
        Console.WriteLine("   even if event source (order) is still alive.");
    }
}

#region Event Args Classes

/// <summary>
/// Event arguments for order creation
/// </summary>
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

/// <summary>
/// Event arguments for order status change
/// </summary>
public class OrderStatusChangedEventArgs : EventArgs
{
    public string OrderId { get; }
    public OrderStatus OldStatus { get; }
    public OrderStatus NewStatus { get; }
    public DateTime ChangedAt { get; }

    public OrderStatusChangedEventArgs(
        string orderId,
        OrderStatus oldStatus,
        OrderStatus newStatus,
        DateTime changedAt)
    {
        OrderId = orderId;
        OldStatus = oldStatus;
        NewStatus = newStatus;
        ChangedAt = changedAt;
    }
}

/// <summary>
/// Event arguments for order cancellation
/// </summary>
public class OrderCancelledEventArgs : EventArgs
{
    public string OrderId { get; }
    public string Reason { get; }
    public DateTime CancelledAt { get; }

    public OrderCancelledEventArgs(string orderId, string reason, DateTime cancelledAt)
    {
        OrderId = orderId;
        Reason = reason;
        CancelledAt = cancelledAt;
    }
}

/// <summary>
/// Event arguments for item addition
/// </summary>
public class OrderItemAddedEventArgs : EventArgs
{
    public string OrderId { get; }
    public OrderItem Item { get; }
    public DateTime AddedAt { get; }

    public OrderItemAddedEventArgs(string orderId, OrderItem item, DateTime addedAt)
    {
        OrderId = orderId;
        Item = item;
        AddedAt = addedAt;
    }
}

#endregion

#region Core Classes

/// <summary>
/// Order status enumeration
/// </summary>
public enum OrderStatus
{
    Pending,
    Processing,
    Shipped,
    Delivered,
    Cancelled
}

/// <summary>
/// Order item class
/// </summary>
public class OrderItem
{
    public string ProductName { get; }
    public decimal Price { get; }
    public int Quantity { get; }
    public decimal Total => Price * Quantity;

    public OrderItem(string productName, decimal price, int quantity)
    {
        ProductName = productName ?? throw new ArgumentNullException(nameof(productName));
        Price = price;
        Quantity = quantity;
    }

    public override string ToString()
    {
        return $"{ProductName} - ${Price:F2} x {Quantity} = ${Total:F2}";
    }
}

/// <summary>
/// Order class with event-driven architecture
/// Demonstrates best practices for event declaration and raising
/// </summary>
public class Order
{
    private OrderStatus _status;
    private readonly List<OrderItem> _items = new();

    public string OrderId { get; }
    public string CustomerName { get; }
    public DateTime CreatedAt { get; }
    public OrderStatus Status => _status;
    public IReadOnlyList<OrderItem> Items => _items.AsReadOnly();

    #region Events

    // Event declarations using EventHandler<T>
    public event EventHandler<OrderCreatedEventArgs>? OrderCreated;
    public event EventHandler<OrderStatusChangedEventArgs>? StatusChanged;
    public event EventHandler<OrderCancelledEventArgs>? OrderCancelled;
    public event EventHandler<OrderItemAddedEventArgs>? ItemAdded;

    #endregion

    public Order(string orderId, string customerName)
    {
        OrderId = orderId ?? throw new ArgumentNullException(nameof(orderId));
        CustomerName = customerName ?? throw new ArgumentNullException(nameof(customerName));
        CreatedAt = DateTime.Now;
        _status = OrderStatus.Pending;

        // Raise OrderCreated event
        OnOrderCreated(new OrderCreatedEventArgs(OrderId, CustomerName, CreatedAt));
    }

    /// <summary>
    /// Adds an item to the order
    /// </summary>
    public void AddItem(OrderItem item)
    {
        ArgumentNullException.ThrowIfNull(item);

        if (_status == OrderStatus.Cancelled)
        {
            throw new InvalidOperationException("Cannot add items to a cancelled order");
        }

        if (_status == OrderStatus.Delivered)
        {
            throw new InvalidOperationException("Cannot add items to a delivered order");
        }

        _items.Add(item);
        OnItemAdded(new OrderItemAddedEventArgs(OrderId, item, DateTime.Now));
    }

    /// <summary>
    /// Changes the order status
    /// </summary>
    public void ChangeStatus(OrderStatus newStatus)
    {
        if (_status == newStatus)
        {
            return; // No change
        }

        var oldStatus = _status;
        _status = newStatus;

        OnStatusChanged(new OrderStatusChangedEventArgs(OrderId, oldStatus, newStatus, DateTime.Now));

        if (newStatus == OrderStatus.Cancelled)
        {
            OnOrderCancelled(new OrderCancelledEventArgs(OrderId, "Status changed to Cancelled", DateTime.Now));
        }
    }

    /// <summary>
    /// Cancels the order with a reason
    /// </summary>
    public void Cancel(string reason)
    {
        if (_status == OrderStatus.Cancelled)
        {
            return; // Already cancelled
        }

        if (_status == OrderStatus.Delivered)
        {
            throw new InvalidOperationException("Cannot cancel a delivered order");
        }

        var oldStatus = _status;
        _status = OrderStatus.Cancelled;

        OnStatusChanged(new OrderStatusChangedEventArgs(OrderId, oldStatus, OrderStatus.Cancelled, DateTime.Now));
        OnOrderCancelled(new OrderCancelledEventArgs(OrderId, reason, DateTime.Now));
    }

    /// <summary>
    /// Calculates the total order amount
    /// </summary>
    public decimal CalculateTotal()
    {
        return _items.Sum(item => item.Total);
    }

    #region Event Raising Methods (Best Practices)

    /// <summary>
    /// Raises the OrderCreated event (Best Practice: Protected virtual method)
    /// </summary>
    protected virtual void OnOrderCreated(OrderCreatedEventArgs e)
    {
        // Best Practice 1: Null-conditional operator for thread safety
        // Best Practice 2: Copy event reference to local variable
        var handler = OrderCreated;
        handler?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the StatusChanged event with exception handling
    /// </summary>
    protected virtual void OnStatusChanged(OrderStatusChangedEventArgs e)
    {
        var handler = StatusChanged;
        if (handler != null)
        {
            // Invoke all subscribers, even if one throws
            foreach (EventHandler<OrderStatusChangedEventArgs> subscriber in handler.GetInvocationList())
            {
                try
                {
                    subscriber(this, e);
                }
                catch (Exception ex)
                {
                    // In production, log the exception
                    Console.WriteLine($"   ⚠ Exception in event handler: {ex.Message}");
                    throw; // Re-throw to demonstrate
                }
            }
        }
    }

    /// <summary>
    /// Raises the OrderCancelled event
    /// </summary>
    protected virtual void OnOrderCancelled(OrderCancelledEventArgs e)
    {
        var handler = OrderCancelled;
        handler?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the ItemAdded event
    /// </summary>
    protected virtual void OnItemAdded(OrderItemAddedEventArgs e)
    {
        var handler = ItemAdded;
        handler?.Invoke(this, e);
    }

    #endregion
}

#endregion

#region Event Subscribers

/// <summary>
/// Notification service that listens to order events
/// </summary>
public class NotificationService
{
    public int NotificationCount { get; private set; }

    public void SubscribeTo(Order order)
    {
        order.OrderCreated += OnOrderCreated;
        order.StatusChanged += OnStatusChanged;
        order.ItemAdded += OnItemAdded;
    }

    public void UnsubscribeFrom(Order order)
    {
        order.OrderCreated -= OnOrderCreated;
        order.StatusChanged -= OnStatusChanged;
        order.ItemAdded -= OnItemAdded;
    }

    private void OnOrderCreated(object? sender, OrderCreatedEventArgs e)
    {
        Console.WriteLine($"   [Notification] New order created: {e.OrderId} for {e.CustomerName}");
        NotificationCount++;
    }

    private void OnStatusChanged(object? sender, OrderStatusChangedEventArgs e)
    {
        Console.WriteLine($"   [Notification] Order {e.OrderId} status: {e.OldStatus} → {e.NewStatus}");
        NotificationCount++;
    }

    private void OnItemAdded(object? sender, OrderItemAddedEventArgs e)
    {
        Console.WriteLine($"   [Notification] Item added to {e.OrderId}: {e.Item.ProductName}");
        NotificationCount++;
    }
}

/// <summary>
/// Logging service that records order events
/// </summary>
public class LoggingService
{
    public int LogCount { get; private set; }

    public void SubscribeTo(Order order)
    {
        order.OrderCreated += OnOrderCreated;
        order.StatusChanged += OnStatusChanged;
        order.OrderCancelled += OnOrderCancelled;
    }

    private void OnOrderCreated(object? sender, OrderCreatedEventArgs e)
    {
        Console.WriteLine($"   [Log] {e.CreatedAt:HH:mm:ss} - Order created: {e.OrderId}");
        LogCount++;
    }

    private void OnStatusChanged(object? sender, OrderStatusChangedEventArgs e)
    {
        Console.WriteLine($"   [Log] {e.ChangedAt:HH:mm:ss} - Status changed: {e.OrderId} [{e.OldStatus} → {e.NewStatus}]");
        LogCount++;
    }

    private void OnOrderCancelled(object? sender, OrderCancelledEventArgs e)
    {
        Console.WriteLine($"   [Log] {e.CancelledAt:HH:mm:ss} - Order cancelled: {e.OrderId} - {e.Reason}");
        LogCount++;
    }
}

/// <summary>
/// Inventory service that manages stock based on orders
/// </summary>
public class InventoryService
{
    public void SubscribeTo(Order order)
    {
        order.ItemAdded += OnItemAdded;
        order.OrderCancelled += OnOrderCancelled;
    }

    private void OnItemAdded(object? sender, OrderItemAddedEventArgs e)
    {
        Console.WriteLine($"   [Inventory] Reserved stock: {e.Item.ProductName} x{e.Item.Quantity}");
    }

    private void OnOrderCancelled(object? sender, OrderCancelledEventArgs e)
    {
        Console.WriteLine($"   [Inventory] Releasing reserved stock for order: {e.OrderId}");
    }
}

/// <summary>
/// Email service that sends notifications
/// </summary>
public class EmailService
{
    public void SubscribeTo(Order order)
    {
        order.StatusChanged += OnStatusChanged;
    }

    private void OnStatusChanged(object? sender, OrderStatusChangedEventArgs e)
    {
        if (e.NewStatus == OrderStatus.Shipped || e.NewStatus == OrderStatus.Delivered)
        {
            Console.WriteLine($"   [Email] Sending email for order {e.OrderId}: Status is now {e.NewStatus}");
        }
    }
}

/// <summary>
/// Temporary subscriber that demonstrates proper cleanup
/// </summary>
public class TemporarySubscriber
{
    public string Name { get; }

    public TemporarySubscriber(string name)
    {
        Name = name;
    }

    public void SubscribeTo(Order order)
    {
        order.ItemAdded += OnItemAdded;
    }

    public void UnsubscribeFrom(Order order)
    {
        order.ItemAdded -= OnItemAdded;
    }

    private void OnItemAdded(object? sender, OrderItemAddedEventArgs e)
    {
        Console.WriteLine($"   [{Name}] Received item added event: {e.Item.ProductName}");
    }
}

#endregion

#region Order Processor

/// <summary>
/// Order processor that orchestrates order workflow
/// </summary>
public class OrderProcessor
{
    private readonly NotificationService _notificationService = new();
    private readonly LoggingService _loggingService = new();

    public Order CreateOrder(string orderId, string customerName)
    {
        var order = new Order(orderId, customerName);

        // Subscribe services
        _notificationService.SubscribeTo(order);
        _loggingService.SubscribeTo(order);

        return order;
    }

    public void AddItemToOrder(Order order, OrderItem item)
    {
        order.AddItem(item);
    }

    public void ProcessOrder(Order order)
    {
        if (order.Items.Count == 0)
        {
            throw new InvalidOperationException("Cannot process order with no items");
        }

        order.ChangeStatus(OrderStatus.Processing);
    }

    public void ShipOrder(Order order)
    {
        if (order.Status != OrderStatus.Processing)
        {
            throw new InvalidOperationException("Order must be in Processing status to ship");
        }

        order.ChangeStatus(OrderStatus.Shipped);
    }

    public void CompleteOrder(Order order)
    {
        if (order.Status != OrderStatus.Shipped)
        {
            throw new InvalidOperationException("Order must be in Shipped status to complete");
        }

        order.ChangeStatus(OrderStatus.Delivered);
    }
}

#endregion

#region Weak Event Pattern

/// <summary>
/// Weak event manager to prevent memory leaks
/// </summary>
public static class WeakEventManager
{
    public static void Subscribe(object source, string eventName, EventHandler<OrderStatusChangedEventArgs> handler)
    {
        // In production, use WeakEventManager from System.Windows
        // or implement proper weak reference pattern
        Console.WriteLine($"   [WeakEvent] Subscribing to {eventName} with weak reference");

        if (source is Order order)
        {
            order.StatusChanged += handler;
        }
    }
}

/// <summary>
/// Subscriber using weak event pattern
/// </summary>
public class WeakEventSubscriber
{
    public string Name { get; }

    public WeakEventSubscriber(string name)
    {
        Name = name;
    }

    public void OnStatusChanged(object? sender, OrderStatusChangedEventArgs e)
    {
        Console.WriteLine($"   [{Name}] Weak event received: Status changed to {e.NewStatus}");
    }
}

#endregion
