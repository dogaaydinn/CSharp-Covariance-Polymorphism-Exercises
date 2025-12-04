// Dependency Injection: Inversion of Control

using Microsoft.Extensions.DependencyInjection;

namespace DependencyInjection;

public class Program
{
    public static void Main()
    {
        Console.WriteLine("=== Dependency Injection Demo ===\n");

        // ❌ BAD: new keyword everywhere
        Console.WriteLine("❌ BAD - Tight coupling:");
        var badService = new BadOrderService();
        badService.PlaceOrder("ORD123");

        // ✅ GOOD: Constructor Injection
        Console.WriteLine("\n✅ GOOD - Constructor Injection:");
        ILogger logger = new ConsoleLogger();
        IEmailService email = new EmailService(logger);
        var goodService = new OrderService(logger, email);
        goodService.PlaceOrder("ORD456");

        // ✅ BEST: DI Container
        Console.WriteLine("\n✅ BEST - DI Container:");
        var serviceProvider = ConfigureServices();

        var containerService = serviceProvider.GetRequiredService<OrderService>();
        containerService.PlaceOrder("ORD789");

        // Lifetime demos
        Console.WriteLine("\n✅ Service Lifetimes:");
        DemoLifetimes(serviceProvider);

        Console.WriteLine("\n=== DI Applied ===");
    }

    static ServiceProvider ConfigureServices()
    {
        var services = new ServiceCollection();

        services.AddSingleton<ILogger, ConsoleLogger>();
        services.AddScoped<IEmailService, EmailService>();
        services.AddTransient<OrderService>();

        return services.BuildServiceProvider();
    }

    static void DemoLifetimes(ServiceProvider provider)
    {
        using var scope1 = provider.CreateScope();
        using var scope2 = provider.CreateScope();

        var singleton1 = scope1.ServiceProvider.GetRequiredService<ILogger>();
        var singleton2 = scope2.ServiceProvider.GetRequiredService<ILogger>();
        Console.WriteLine($"Singleton same instance: {ReferenceEquals(singleton1, singleton2)}");

        var scoped1 = scope1.ServiceProvider.GetRequiredService<IEmailService>();
        var scoped2 = scope2.ServiceProvider.GetRequiredService<IEmailService>();
        Console.WriteLine($"Scoped different instance: {!ReferenceEquals(scoped1, scoped2)}");
    }
}

// ❌ BAD
public class BadOrderService
{
    public void PlaceOrder(string orderId)
    {
        var logger = new ConsoleLogger(); // Hard-coded dependency
        logger.Log($"Order {orderId} placed");
    }
}

// ✅ GOOD
public interface ILogger
{
    void Log(string message);
}

public interface IEmailService
{
    void SendEmail(string to, string message);
}

public class ConsoleLogger : ILogger
{
    public void Log(string message)
    {
        Console.WriteLine($"[LOG] {message}");
    }
}

public class EmailService : IEmailService
{
    private readonly ILogger _logger;

    public EmailService(ILogger logger) // Constructor Injection
    {
        _logger = logger;
    }

    public void SendEmail(string to, string message)
    {
        _logger.Log($"Sending email to {to}: {message}");
        Console.WriteLine($"[EMAIL] To: {to}");
    }
}

public class OrderService
{
    private readonly ILogger _logger;
    private readonly IEmailService _emailService;

    public OrderService(ILogger logger, IEmailService emailService)
    {
        _logger = logger;
        _emailService = emailService;
    }

    public void PlaceOrder(string orderId)
    {
        _logger.Log($"✅ Processing order {orderId}");
        _emailService.SendEmail("customer@example.com", $"Order {orderId} confirmed");
        _logger.Log($"✅ Order {orderId} completed");
    }
}

// Service Lifetimes:
// - Singleton: One instance for app lifetime
// - Scoped: One instance per request/scope
// - Transient: New instance every time
