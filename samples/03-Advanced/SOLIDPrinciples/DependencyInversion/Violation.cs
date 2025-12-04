namespace SOLIDPrinciples.DependencyInversion;

/// <summary>
/// VIOLATION: High-level modules depend on low-level modules.
/// Problem: Direct dependencies on concrete implementations.
/// Consequences:
/// - Tight coupling to specific implementations
/// - Hard to test (can't mock dependencies)
/// - Hard to swap implementations
/// - Changes to low-level modules affect high-level modules
/// </summary>

#region Notification System Violation

/// <summary>
/// Low-level module: Concrete email service
/// </summary>
public class EmailService
{
    public void SendEmail(string to, string subject, string body)
    {
        Console.WriteLine($"[LOW-LEVEL] EmailService: Sending email to {to}");
        Console.WriteLine($"  Subject: {subject}");
        Console.WriteLine($"  Connecting to SMTP server...");
        Console.WriteLine($"  Email sent successfully!");
    }
}

/// <summary>
/// Low-level module: Concrete SMS service
/// </summary>
public class SmsService
{
    public void SendSms(string phoneNumber, string message)
    {
        Console.WriteLine($"[LOW-LEVEL] SmsService: Sending SMS to {phoneNumber}");
        Console.WriteLine($"  Message: {message}");
        Console.WriteLine($"  Connecting to SMS gateway...");
        Console.WriteLine($"  SMS sent successfully!");
    }
}

/// <summary>
/// VIOLATION: High-level module depends on low-level concrete classes
/// </summary>
public class OrderServiceViolation
{
    // Direct dependencies on concrete classes
    private readonly EmailService _emailService = new();
    private readonly SmsService _smsService = new();

    public void PlaceOrder(string customerEmail, string phoneNumber, decimal amount)
    {
        Console.WriteLine($"\n[VIOLATION] Placing order for ${amount}");

        // Business logic...
        Console.WriteLine("  Processing payment...");
        Console.WriteLine("  Updating inventory...");

        // VIOLATION: Tightly coupled to specific notification implementations
        _emailService.SendEmail(
            customerEmail,
            "Order Confirmation",
            $"Your order of ${amount} has been placed");

        _smsService.SendSms(
            phoneNumber,
            $"Order confirmed: ${amount}");

        Console.WriteLine("Order placed successfully!");
    }
}

/// <summary>
/// Problems with this approach:
/// 1. Can't easily test OrderService (must use real email/SMS services)
/// 2. Can't swap email providers without modifying OrderService
/// 3. Can't add new notification types (Push, Slack) without modifying OrderService
/// 4. Changes to EmailService or SmsService might break OrderService
/// </summary>
public class NotificationViolationDemo
{
    public static void DemonstrateProblems()
    {
        Console.WriteLine("\n=== NOTIFICATION SYSTEM PROBLEMS ===");

        Console.WriteLine("\nProblem 1: Hard to test");
        Console.WriteLine("  OrderService creates its own dependencies");
        Console.WriteLine("  Can't mock EmailService or SmsService");
        Console.WriteLine("  Tests will send real emails and SMS!");

        Console.WriteLine("\nProblem 2: Hard to change implementation");
        Console.WriteLine("  Want to switch from SMTP to SendGrid?");
        Console.WriteLine("  Must modify OrderService code!");

        Console.WriteLine("\nProblem 3: Can't extend easily");
        Console.WriteLine("  Want to add push notifications?");
        Console.WriteLine("  Must modify OrderService class!");

        var orderService = new OrderServiceViolation();
        orderService.PlaceOrder("customer@example.com", "+1234567890", 99.99m);
    }
}

#endregion

#region Data Access Violation

/// <summary>
/// Low-level module: Concrete database implementation
/// </summary>
public class SqlDatabase
{
    public void SaveUser(string name, string email)
    {
        Console.WriteLine($"[LOW-LEVEL] SqlDatabase: Saving user to SQL Server");
        Console.WriteLine($"  Name: {name}, Email: {email}");
        Console.WriteLine($"  INSERT INTO Users...");
        Console.WriteLine($"  User saved!");
    }

    public void GetUser(int id)
    {
        Console.WriteLine($"[LOW-LEVEL] SqlDatabase: Getting user {id} from SQL Server");
        Console.WriteLine($"  SELECT * FROM Users WHERE Id = {id}");
    }
}

/// <summary>
/// VIOLATION: High-level business logic depends on concrete database
/// </summary>
public class UserManagerViolation
{
    // Direct dependency on concrete class
    private readonly SqlDatabase _database = new();

    public void RegisterUser(string name, string email)
    {
        Console.WriteLine($"\n[VIOLATION] Registering user: {name}");

        // Validation logic...
        Console.WriteLine("  Validating user data...");

        // VIOLATION: Tightly coupled to SQL Server
        _database.SaveUser(name, email);

        Console.WriteLine("User registered successfully!");
    }

    public void GetUserInfo(int userId)
    {
        Console.WriteLine($"\n[VIOLATION] Getting user info: {userId}");
        _database.GetUser(userId);
    }
}

/// <summary>
/// Problems:
/// 1. Can't switch to MongoDB without modifying UserManager
/// 2. Can't test without real database
/// 3. UserManager knows about SQL Server specifics
/// </summary>
public class DataAccessViolationDemo
{
    public static void DemonstrateProblems()
    {
        Console.WriteLine("\n=== DATA ACCESS PROBLEMS ===");

        Console.WriteLine("\nProblem 1: Tied to specific database");
        Console.WriteLine("  UserManager creates its own SqlDatabase");
        Console.WriteLine("  Want to use MongoDB? Must modify UserManager!");

        Console.WriteLine("\nProblem 2: Can't test in isolation");
        Console.WriteLine("  Tests require real SQL Server connection");
        Console.WriteLine("  Slow and brittle tests");

        Console.WriteLine("\nProblem 3: Business logic mixed with infrastructure");
        Console.WriteLine("  UserManager shouldn't know about SQL Server");
        Console.WriteLine("  High-level policy depends on low-level detail");

        var userManager = new UserManagerViolation();
        userManager.RegisterUser("John Doe", "john@example.com");
        userManager.GetUserInfo(1);
    }
}

#endregion

#region Logging Violation

/// <summary>
/// Low-level module: Concrete file logger
/// </summary>
public class FileLogger
{
    public void Log(string message)
    {
        Console.WriteLine($"[LOW-LEVEL] FileLogger: Writing to log file");
        Console.WriteLine($"  Message: {message}");
        Console.WriteLine($"  File: app.log");
    }
}

/// <summary>
/// VIOLATION: Payment processor depends on concrete logger
/// </summary>
public class PaymentProcessorViolation
{
    // Direct dependency on concrete class
    private readonly FileLogger _logger = new();

    public void ProcessPayment(decimal amount)
    {
        Console.WriteLine($"\n[VIOLATION] Processing payment: ${amount}");

        _logger.Log($"Payment started: ${amount}");

        // Payment logic...
        Console.WriteLine("  Contacting payment gateway...");
        Console.WriteLine("  Payment approved!");

        _logger.Log($"Payment completed: ${amount}");
    }
}

/// <summary>
/// Problems:
/// 1. Can't switch to database logging or cloud logging
/// 2. Can't aggregate logs from multiple sources
/// 3. Can't test without creating log files
/// </summary>
public class LoggingViolationDemo
{
    public static void DemonstrateProblems()
    {
        Console.WriteLine("\n=== LOGGING PROBLEMS ===");

        Console.WriteLine("\nProblem 1: Tied to file logging");
        Console.WriteLine("  Want to use structured logging (Serilog)?");
        Console.WriteLine("  Must modify PaymentProcessor!");

        Console.WriteLine("\nProblem 2: Can't aggregate logs");
        Console.WriteLine("  Want to send logs to Elasticsearch?");
        Console.WriteLine("  Must modify every class that logs!");

        Console.WriteLine("\nProblem 3: Testing creates files");
        Console.WriteLine("  Tests write real log files");
        Console.WriteLine("  Must clean up after tests");

        var processor = new PaymentProcessorViolation();
        processor.ProcessPayment(149.99m);
    }
}

#endregion

#region Cache Violation

/// <summary>
/// Low-level module: Concrete Redis cache
/// </summary>
public class RedisCache
{
    public void Set(string key, string value)
    {
        Console.WriteLine($"[LOW-LEVEL] RedisCache: Setting {key} = {value}");
        Console.WriteLine($"  Connecting to Redis server...");
        Console.WriteLine($"  Value cached!");
    }

    public string? Get(string key)
    {
        Console.WriteLine($"[LOW-LEVEL] RedisCache: Getting {key}");
        Console.WriteLine($"  Connecting to Redis server...");
        return "cached_value";
    }
}

/// <summary>
/// VIOLATION: Product service depends on concrete cache
/// </summary>
public class ProductServiceViolation
{
    // Direct dependency on concrete class
    private readonly RedisCache _cache = new();

    public void GetProduct(int productId)
    {
        Console.WriteLine($"\n[VIOLATION] Getting product: {productId}");

        var cacheKey = $"product:{productId}";

        // VIOLATION: Tightly coupled to Redis
        var cached = _cache.Get(cacheKey);

        if (cached != null)
        {
            Console.WriteLine($"  Found in cache: {cached}");
        }
        else
        {
            Console.WriteLine($"  Cache miss, loading from database...");
            _cache.Set(cacheKey, "product_data");
        }
    }
}

/// <summary>
/// Problems:
/// 1. Can't switch to Memcached or in-memory cache
/// 2. Can't test without Redis server
/// 3. Can't use different caching strategies
/// </summary>
public class CacheViolationDemo
{
    public static void DemonstrateProblems()
    {
        Console.WriteLine("\n=== CACHE PROBLEMS ===");

        Console.WriteLine("\nProblem 1: Tied to Redis");
        Console.WriteLine("  Want to use in-memory cache for testing?");
        Console.WriteLine("  Must modify ProductService!");

        Console.WriteLine("\nProblem 2: Can't test easily");
        Console.WriteLine("  Tests require Redis server running");
        Console.WriteLine("  Setup and teardown complexity");

        Console.WriteLine("\nProblem 3: Can't change caching strategy");
        Console.WriteLine("  Want to add cache invalidation?");
        Console.WriteLine("  Must modify multiple services!");

        var productService = new ProductServiceViolation();
        productService.GetProduct(123);
    }
}

#endregion

/// <summary>
/// Demonstrates all Dependency Inversion Principle violations
/// </summary>
public class DependencyInversionViolationDemo
{
    public static void DemonstrateAllProblems()
    {
        Console.WriteLine("\n=== PROBLEMS WITH VIOLATING DEPENDENCY INVERSION PRINCIPLE ===");

        Console.WriteLine("\nCore Problem: High-level modules depend on low-level modules");
        Console.WriteLine("  Business logic (high-level) knows about infrastructure (low-level)");
        Console.WriteLine("  Changes to infrastructure require changes to business logic");
        Console.WriteLine("  Can't easily swap implementations");

        Console.WriteLine("\nConsequences:");
        Console.WriteLine("  1. Tight coupling - hard to change");
        Console.WriteLine("  2. Hard to test - must use real dependencies");
        Console.WriteLine("  3. Hard to extend - must modify existing code");
        Console.WriteLine("  4. Business logic mixed with technical details");
        Console.WriteLine("  5. Can't compose systems at runtime");

        NotificationViolationDemo.DemonstrateProblems();
        DataAccessViolationDemo.DemonstrateProblems();
        LoggingViolationDemo.DemonstrateProblems();
        CacheViolationDemo.DemonstrateProblems();

        Console.WriteLine("\n=== REMEMBER ===");
        Console.WriteLine("High-level modules should NOT depend on low-level modules!");
        Console.WriteLine("Both should depend on ABSTRACTIONS!");
    }
}
