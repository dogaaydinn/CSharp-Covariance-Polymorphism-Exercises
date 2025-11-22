namespace AdvancedCsharpConcepts.Advanced.DesignPatterns;

/// <summary>
/// Facade Pattern - Provides a simplified interface to a complex subsystem.
/// Silicon Valley best practice: Hide complexity, expose simple APIs.
/// </summary>
public static class FacadePattern
{
    // Complex Subsystem: Home Theater Components
    #region Home Theater Subsystem

    public class Amplifier
    {
        public void On() => Console.WriteLine("[Amplifier] Turning on");
        public void Off() => Console.WriteLine("[Amplifier] Turning off");
        public void SetVolume(int level) => Console.WriteLine($"[Amplifier] Setting volume to {level}");
        public void SetSurroundSound() => Console.WriteLine("[Amplifier] Setting surround sound mode");
    }

    public class DvdPlayer
    {
        public void On() => Console.WriteLine("[DvdPlayer] Turning on");
        public void Off() => Console.WriteLine("[DvdPlayer] Turning off");
        public void Play(string movie) => Console.WriteLine($"[DvdPlayer] Playing '{movie}'");
        public void Stop() => Console.WriteLine("[DvdPlayer] Stopped");
        public void Eject() => Console.WriteLine("[DvdPlayer] Ejecting disc");
    }

    public class Projector
    {
        public void On() => Console.WriteLine("[Projector] Turning on");
        public void Off() => Console.WriteLine("[Projector] Turning off");
        public void WideScreenMode() => Console.WriteLine("[Projector] Setting widescreen mode (16:9)");
    }

    public class TheaterLights
    {
        public void Dim(int level) => Console.WriteLine($"[Lights] Dimming to {level}%");
        public void On() => Console.WriteLine("[Lights] Turning lights on");
    }

    public class Screen
    {
        public void Down() => Console.WriteLine("[Screen] Lowering screen");
        public void Up() => Console.WriteLine("[Screen] Raising screen");
    }

    public class PopcornPopper
    {
        public void On() => Console.WriteLine("[Popcorn] Turning on");
        public void Off() => Console.WriteLine("[Popcorn] Turning off");
        public void Pop() => Console.WriteLine("[Popcorn] Popping popcorn!");
    }

    #endregion

    // Facade: Simplifies the complex subsystem
    public class HomeTheaterFacade
    {
        private readonly Amplifier _amp;
        private readonly DvdPlayer _dvd;
        private readonly Projector _projector;
        private readonly TheaterLights _lights;
        private readonly Screen _screen;
        private readonly PopcornPopper _popper;

        public HomeTheaterFacade(
            Amplifier amp,
            DvdPlayer dvd,
            Projector projector,
            TheaterLights lights,
            Screen screen,
            PopcornPopper popper)
        {
            _amp = amp ?? throw new ArgumentNullException(nameof(amp));
            _dvd = dvd ?? throw new ArgumentNullException(nameof(dvd));
            _projector = projector ?? throw new ArgumentNullException(nameof(projector));
            _lights = lights ?? throw new ArgumentNullException(nameof(lights));
            _screen = screen ?? throw new ArgumentNullException(nameof(screen));
            _popper = popper ?? throw new ArgumentNullException(nameof(popper));
        }

        public void WatchMovie(string movie)
        {
            Console.WriteLine($"\n🎬 Get ready to watch '{movie}'...\n");

            _popper.On();
            _popper.Pop();
            _lights.Dim(10);
            _screen.Down();
            _projector.On();
            _projector.WideScreenMode();
            _amp.On();
            _amp.SetSurroundSound();
            _amp.SetVolume(75);
            _dvd.On();
            _dvd.Play(movie);

            Console.WriteLine("\n✅ Movie is now playing! Enjoy!\n");
        }

        public void EndMovie()
        {
            Console.WriteLine("\n🛑 Shutting down home theater...\n");

            _popper.Off();
            _lights.On();
            _screen.Up();
            _projector.Off();
            _amp.Off();
            _dvd.Stop();
            _dvd.Eject();
            _dvd.Off();

            Console.WriteLine("\n✅ Home theater is off. Thanks for watching!\n");
        }
    }

    // Real-world example: E-Commerce Checkout Facade
    #region E-Commerce Subsystem

    public class InventoryService
    {
        public bool CheckAvailability(string productId, int quantity)
        {
            Console.WriteLine($"[Inventory] Checking availability for {productId} x{quantity}");
            return true;
        }

        public void ReserveStock(string productId, int quantity)
        {
            Console.WriteLine($"[Inventory] Reserved {quantity} units of {productId}");
        }

        public void ReleaseStock(string productId, int quantity)
        {
            Console.WriteLine($"[Inventory] Released {quantity} units of {productId}");
        }
    }

    public class PaymentService
    {
        public async Task<string> ProcessPaymentAsync(string cardNumber, decimal amount, CancellationToken cancellationToken = default)
        {
            await Task.Delay(100, cancellationToken);
            Console.WriteLine($"[Payment] Processing payment of ${amount:F2} with card ending in {cardNumber[^4..]}");
            return $"TXN-{Guid.NewGuid():N}";
        }

        public async Task RefundAsync(string transactionId, CancellationToken cancellationToken = default)
        {
            await Task.Delay(50, cancellationToken);
            Console.WriteLine($"[Payment] Refunded transaction {transactionId}");
        }
    }

    public class ShippingService
    {
        public async Task<string> CreateShipmentAsync(string address, string productId, CancellationToken cancellationToken = default)
        {
            await Task.Delay(100, cancellationToken);
            Console.WriteLine($"[Shipping] Creating shipment for {productId} to {address}");
            return $"SHIP-{Guid.NewGuid():N}";
        }

        public void CancelShipment(string shipmentId)
        {
            Console.WriteLine($"[Shipping] Cancelled shipment {shipmentId}");
        }
    }

    public class NotificationService
    {
        public async Task SendEmailAsync(string email, string subject, string body, CancellationToken cancellationToken = default)
        {
            await Task.Delay(50, cancellationToken);
            Console.WriteLine($"[Notification] Sent email to {email}: {subject}");
        }
    }

    public class OrderDatabase
    {
        public async Task<string> SaveOrderAsync(Order order, CancellationToken cancellationToken = default)
        {
            await Task.Delay(50, cancellationToken);
            var orderId = $"ORD-{Guid.NewGuid():N}";
            Console.WriteLine($"[Database] Saved order {orderId}");
            return orderId;
        }

        public async Task UpdateOrderStatusAsync(string orderId, string status, CancellationToken cancellationToken = default)
        {
            await Task.Delay(30, cancellationToken);
            Console.WriteLine($"[Database] Updated order {orderId} status to {status}");
        }
    }

    public record Order(string ProductId, int Quantity, string CustomerEmail, string ShippingAddress, string CardNumber, decimal Price);

    #endregion

    // Facade: Simplifies complex checkout process
    public class CheckoutFacade
    {
        private readonly InventoryService _inventory;
        private readonly PaymentService _payment;
        private readonly ShippingService _shipping;
        private readonly NotificationService _notification;
        private readonly OrderDatabase _database;

        public CheckoutFacade(
            InventoryService inventory,
            PaymentService payment,
            ShippingService shipping,
            NotificationService notification,
            OrderDatabase database)
        {
            _inventory = inventory ?? throw new ArgumentNullException(nameof(inventory));
            _payment = payment ?? throw new ArgumentNullException(nameof(payment));
            _shipping = shipping ?? throw new ArgumentNullException(nameof(shipping));
            _notification = notification ?? throw new ArgumentNullException(nameof(notification));
            _database = database ?? throw new ArgumentNullException(nameof(database));
        }

        public async Task<CheckoutResult> PlaceOrderAsync(Order order, CancellationToken cancellationToken = default)
        {
            Console.WriteLine("\n🛒 Starting checkout process...\n");

            try
            {
                // Step 1: Check inventory
                if (!_inventory.CheckAvailability(order.ProductId, order.Quantity))
                {
                    return new CheckoutResult(false, "Product not available");
                }

                // Step 2: Reserve stock
                _inventory.ReserveStock(order.ProductId, order.Quantity);

                // Step 3: Process payment
                var transactionId = await _payment.ProcessPaymentAsync(order.CardNumber, order.Price, cancellationToken);

                // Step 4: Save order
                var orderId = await _database.SaveOrderAsync(order, cancellationToken);

                // Step 5: Create shipment
                var shipmentId = await _shipping.CreateShipmentAsync(order.ShippingAddress, order.ProductId, cancellationToken);

                // Step 6: Update order status
                await _database.UpdateOrderStatusAsync(orderId, "Confirmed", cancellationToken);

                // Step 7: Send confirmation email
                await _notification.SendEmailAsync(
                    order.CustomerEmail,
                    $"Order Confirmed - {orderId}",
                    $"Your order has been confirmed and will ship soon!",
                    cancellationToken);

                Console.WriteLine("\n✅ Checkout completed successfully!\n");
                return new CheckoutResult(true, "Order placed successfully", orderId, transactionId, shipmentId);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n❌ Checkout failed: {ex.Message}\n");

                // Rollback (simplified)
                _inventory.ReleaseStock(order.ProductId, order.Quantity);

                return new CheckoutResult(false, $"Checkout failed: {ex.Message}");
            }
        }
    }

    public record CheckoutResult(
        bool Success,
        string Message,
        string? OrderId = null,
        string? TransactionId = null,
        string? ShipmentId = null);

    /// <summary>
    /// Demonstrates the Facade Pattern.
    /// </summary>
    public static async Task RunExample(CancellationToken cancellationToken = default)
    {
        Console.WriteLine("=== Facade Pattern Examples ===\n");

        // Example 1: Home Theater Facade
        Console.WriteLine("1. Home Theater Facade:\n");

        var amp = new Amplifier();
        var dvd = new DvdPlayer();
        var projector = new Projector();
        var lights = new TheaterLights();
        var screen = new Screen();
        var popper = new PopcornPopper();

        var homeTheater = new HomeTheaterFacade(amp, dvd, projector, lights, screen, popper);

        // Simple one-line method call instead of managing 6 components
        homeTheater.WatchMovie("The Matrix");

        await Task.Delay(1000, cancellationToken); // Simulate watching

        homeTheater.EndMovie();

        // Example 2: E-Commerce Checkout Facade
        Console.WriteLine("\n2. E-Commerce Checkout Facade:\n");

        var inventory = new InventoryService();
        var payment = new PaymentService();
        var shipping = new ShippingService();
        var notification = new NotificationService();
        var database = new OrderDatabase();

        var checkout = new CheckoutFacade(inventory, payment, shipping, notification, database);

        var order = new Order(
            ProductId: "LAPTOP-123",
            Quantity: 1,
            CustomerEmail: "customer@example.com",
            ShippingAddress: "123 Main St, City, Country",
            CardNumber: "1234567890123456",
            Price: 1299.99m
        );

        // Simple one-line method call instead of managing 5 services
        var result = await checkout.PlaceOrderAsync(order, cancellationToken);

        Console.WriteLine($"📋 Result: {result.Message}");
        if (result.Success)
        {
            Console.WriteLine($"   Order ID: {result.OrderId}");
            Console.WriteLine($"   Transaction ID: {result.TransactionId}");
            Console.WriteLine($"   Shipment ID: {result.ShipmentId}");
        }
    }
}
