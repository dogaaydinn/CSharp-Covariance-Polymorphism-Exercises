namespace SOLIDPrinciples.DependencyInversion;

/// <summary>
/// CORRECT: Both high-level and low-level modules depend on abstractions.
/// Benefits:
/// - Loose coupling
/// - Easy to test (can mock dependencies)
/// - Easy to swap implementations
/// - Business logic independent of infrastructure
/// </summary>

#region Notification System (Correct)

/// <summary>
/// ABSTRACTION: High-level policy for sending notifications
/// Both high-level and low-level modules depend on this
/// </summary>
public interface INotificationService
{
    void SendNotification(string recipient, string message);
    string GetNotificationType();
}

/// <summary>
/// Low-level module: Concrete email implementation
/// Depends on INotificationService abstraction
/// </summary>
public class EmailNotificationService : INotificationService
{
    public string GetNotificationType() => "Email";

    public void SendNotification(string recipient, string message)
    {
        Console.WriteLine($"[CORRECT] EmailService: Sending to {recipient}");
        Console.WriteLine($"  Connecting to SMTP server...");
        Console.WriteLine($"  Subject: Notification");
        Console.WriteLine($"  Body: {message}");
        Console.WriteLine($"  Email sent successfully!");
    }
}

/// <summary>
/// Low-level module: Concrete SMS implementation
/// Depends on INotificationService abstraction
/// </summary>
public class SmsNotificationService : INotificationService
{
    public string GetNotificationType() => "SMS";

    public void SendNotification(string recipient, string message)
    {
        Console.WriteLine($"[CORRECT] SmsService: Sending to {recipient}");
        Console.WriteLine($"  Connecting to SMS gateway...");
        Console.WriteLine($"  Message: {message}");
        Console.WriteLine($"  SMS sent successfully!");
    }
}

/// <summary>
/// Low-level module: Push notification implementation
/// Easy to add without modifying existing code!
/// </summary>
public class PushNotificationService : INotificationService
{
    public string GetNotificationType() => "Push";

    public void SendNotification(string recipient, string message)
    {
        Console.WriteLine($"[CORRECT] PushService: Sending to device {recipient}");
        Console.WriteLine($"  Connecting to push notification service...");
        Console.WriteLine($"  Notification: {message}");
        Console.WriteLine($"  Push notification sent!");
    }
}

/// <summary>
/// Low-level module: Slack notification
/// Easy to add without modifying existing code!
/// </summary>
public class SlackNotificationService : INotificationService
{
    public string GetNotificationType() => "Slack";

    public void SendNotification(string recipient, string message)
    {
        Console.WriteLine($"[CORRECT] SlackService: Posting to channel {recipient}");
        Console.WriteLine($"  Connecting to Slack API...");
        Console.WriteLine($"  Message: {message}");
        Console.WriteLine($"  Slack message posted!");
    }
}

/// <summary>
/// High-level module: Depends on abstraction, not concretions
/// </summary>
public class OrderService
{
    private readonly IEnumerable<INotificationService> _notificationServices;

    // Dependency injection: Dependencies provided from outside
    public OrderService(IEnumerable<INotificationService> notificationServices)
    {
        _notificationServices = notificationServices;
    }

    public void PlaceOrder(string recipient, decimal amount)
    {
        Console.WriteLine($"\n[CORRECT] Placing order for ${amount}");

        // Business logic...
        Console.WriteLine("  Processing payment...");
        Console.WriteLine("  Updating inventory...");

        // Send notifications via all configured services
        var message = $"Your order of ${amount} has been placed successfully!";

        foreach (var service in _notificationServices)
        {
            Console.WriteLine($"\n  Sending {service.GetNotificationType()} notification:");
            service.SendNotification(recipient, message);
        }

        Console.WriteLine("\nOrder placed successfully!");
    }
}

#endregion

#region Data Access (Correct)

/// <summary>
/// ABSTRACTION: High-level policy for data access
/// </summary>
public interface IUserRepository
{
    void Save(User user);
    User? GetById(int id);
    List<User> GetAll();
}

/// <summary>
/// Domain model - pure business logic
/// </summary>
public class User
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}

/// <summary>
/// Low-level module: SQL Server implementation
/// </summary>
public class SqlUserRepository : IUserRepository
{
    public void Save(User user)
    {
        Console.WriteLine($"[CORRECT] SqlRepository: Saving user to SQL Server");
        Console.WriteLine($"  INSERT INTO Users (Name, Email) VALUES ('{user.Name}', '{user.Email}')");
        Console.WriteLine($"  User saved!");
    }

    public User? GetById(int id)
    {
        Console.WriteLine($"[CORRECT] SqlRepository: Getting user {id} from SQL Server");
        Console.WriteLine($"  SELECT * FROM Users WHERE Id = {id}");
        return new User { Id = id, Name = "John Doe", Email = "john@example.com" };
    }

    public List<User> GetAll()
    {
        Console.WriteLine($"[CORRECT] SqlRepository: Getting all users from SQL Server");
        return new List<User>();
    }
}

/// <summary>
/// Low-level module: MongoDB implementation
/// Easy to add without modifying business logic!
/// </summary>
public class MongoUserRepository : IUserRepository
{
    public void Save(User user)
    {
        Console.WriteLine($"[CORRECT] MongoRepository: Saving user to MongoDB");
        Console.WriteLine($"  db.users.insertOne({{ name: '{user.Name}', email: '{user.Email}' }})");
        Console.WriteLine($"  User saved!");
    }

    public User? GetById(int id)
    {
        Console.WriteLine($"[CORRECT] MongoRepository: Getting user {id} from MongoDB");
        Console.WriteLine($"  db.users.findOne({{ _id: {id} }})");
        return new User { Id = id, Name = "Jane Smith", Email = "jane@example.com" };
    }

    public List<User> GetAll()
    {
        Console.WriteLine($"[CORRECT] MongoRepository: Getting all users from MongoDB");
        return new List<User>();
    }
}

/// <summary>
/// Low-level module: In-memory implementation for testing
/// </summary>
public class InMemoryUserRepository : IUserRepository
{
    private readonly List<User> _users = new();
    private int _nextId = 1;

    public void Save(User user)
    {
        Console.WriteLine($"[CORRECT] InMemoryRepository: Saving user to memory");
        user.Id = _nextId++;
        _users.Add(user);
        Console.WriteLine($"  User saved with Id: {user.Id}");
    }

    public User? GetById(int id)
    {
        Console.WriteLine($"[CORRECT] InMemoryRepository: Getting user {id} from memory");
        return _users.FirstOrDefault(u => u.Id == id);
    }

    public List<User> GetAll()
    {
        Console.WriteLine($"[CORRECT] InMemoryRepository: Getting all users from memory");
        return _users;
    }
}

/// <summary>
/// High-level module: Business logic depends on abstraction
/// </summary>
public class UserManager
{
    private readonly IUserRepository _repository;

    // Dependency injection
    public UserManager(IUserRepository repository)
    {
        _repository = repository;
    }

    public void RegisterUser(string name, string email)
    {
        Console.WriteLine($"\n[CORRECT] Registering user: {name}");

        // Validation logic...
        Console.WriteLine("  Validating user data...");

        var user = new User { Name = name, Email = email };

        // Works with ANY repository implementation!
        _repository.Save(user);

        Console.WriteLine("User registered successfully!");
    }

    public void GetUserInfo(int userId)
    {
        Console.WriteLine($"\n[CORRECT] Getting user info: {userId}");
        var user = _repository.GetById(userId);

        if (user != null)
        {
            Console.WriteLine($"  Name: {user.Name}");
            Console.WriteLine($"  Email: {user.Email}");
        }
    }
}

#endregion

#region Logging (Correct)

/// <summary>
/// ABSTRACTION: Logging policy
/// </summary>
public interface ILogger
{
    void Log(string message);
    void LogError(string message);
    void LogWarning(string message);
}

/// <summary>
/// Low-level module: Console logger
/// </summary>
public class ConsoleLogger : ILogger
{
    public void Log(string message)
    {
        Console.WriteLine($"[CORRECT] ConsoleLogger: {message}");
    }

    public void LogError(string message)
    {
        Console.WriteLine($"[CORRECT] ConsoleLogger ERROR: {message}");
    }

    public void LogWarning(string message)
    {
        Console.WriteLine($"[CORRECT] ConsoleLogger WARNING: {message}");
    }
}

/// <summary>
/// Low-level module: File logger
/// </summary>
public class FileLoggerCorrect : ILogger
{
    private readonly string _fileName;

    public FileLoggerCorrect(string fileName)
    {
        _fileName = fileName;
    }

    public void Log(string message)
    {
        Console.WriteLine($"[CORRECT] FileLogger: Writing to {_fileName}");
        Console.WriteLine($"  {message}");
    }

    public void LogError(string message)
    {
        Console.WriteLine($"[CORRECT] FileLogger ERROR to {_fileName}: {message}");
    }

    public void LogWarning(string message)
    {
        Console.WriteLine($"[CORRECT] FileLogger WARNING to {_fileName}: {message}");
    }
}

/// <summary>
/// Low-level module: Composite logger (logs to multiple destinations)
/// </summary>
public class CompositeLogger : ILogger
{
    private readonly IEnumerable<ILogger> _loggers;

    public CompositeLogger(IEnumerable<ILogger> loggers)
    {
        _loggers = loggers;
    }

    public void Log(string message)
    {
        foreach (var logger in _loggers)
        {
            logger.Log(message);
        }
    }

    public void LogError(string message)
    {
        foreach (var logger in _loggers)
        {
            logger.LogError(message);
        }
    }

    public void LogWarning(string message)
    {
        foreach (var logger in _loggers)
        {
            logger.LogWarning(message);
        }
    }
}

/// <summary>
/// High-level module: Payment processor depends on abstraction
/// </summary>
public class PaymentProcessor
{
    private readonly ILogger _logger;

    // Dependency injection
    public PaymentProcessor(ILogger logger)
    {
        _logger = logger;
    }

    public void ProcessPayment(decimal amount)
    {
        Console.WriteLine($"\n[CORRECT] Processing payment: ${amount}");

        _logger.Log($"Payment started: ${amount}");

        // Payment logic...
        Console.WriteLine("  Contacting payment gateway...");

        if (amount > 0)
        {
            Console.WriteLine("  Payment approved!");
            _logger.Log($"Payment completed: ${amount}");
        }
        else
        {
            _logger.LogError($"Invalid payment amount: ${amount}");
        }
    }
}

#endregion

#region Cache (Correct)

/// <summary>
/// ABSTRACTION: Caching policy
/// </summary>
public interface ICache
{
    void Set(string key, string value);
    string? Get(string key);
    void Remove(string key);
    bool Exists(string key);
}

/// <summary>
/// Low-level module: Redis implementation
/// </summary>
public class RedisCacheCorrect : ICache
{
    public void Set(string key, string value)
    {
        Console.WriteLine($"[CORRECT] RedisCache: Setting {key}");
        Console.WriteLine($"  Connecting to Redis server...");
        Console.WriteLine($"  Value cached!");
    }

    public string? Get(string key)
    {
        Console.WriteLine($"[CORRECT] RedisCache: Getting {key}");
        Console.WriteLine($"  Connecting to Redis server...");
        return "cached_value_from_redis";
    }

    public void Remove(string key)
    {
        Console.WriteLine($"[CORRECT] RedisCache: Removing {key}");
    }

    public bool Exists(string key)
    {
        return true;
    }
}

/// <summary>
/// Low-level module: In-memory cache for testing/development
/// </summary>
public class InMemoryCache : ICache
{
    private readonly Dictionary<string, string> _cache = new();

    public void Set(string key, string value)
    {
        Console.WriteLine($"[CORRECT] InMemoryCache: Setting {key} = {value}");
        _cache[key] = value;
    }

    public string? Get(string key)
    {
        Console.WriteLine($"[CORRECT] InMemoryCache: Getting {key}");
        return _cache.TryGetValue(key, out var value) ? value : null;
    }

    public void Remove(string key)
    {
        Console.WriteLine($"[CORRECT] InMemoryCache: Removing {key}");
        _cache.Remove(key);
    }

    public bool Exists(string key)
    {
        return _cache.ContainsKey(key);
    }
}

/// <summary>
/// Low-level module: No-op cache (for disabling caching)
/// </summary>
public class NullCache : ICache
{
    public void Set(string key, string value)
    {
        Console.WriteLine($"[CORRECT] NullCache: Cache disabled, not storing {key}");
    }

    public string? Get(string key)
    {
        Console.WriteLine($"[CORRECT] NullCache: Cache disabled, returning null");
        return null;
    }

    public void Remove(string key)
    {
        // No-op
    }

    public bool Exists(string key)
    {
        return false;
    }
}

/// <summary>
/// High-level module: Product service depends on abstraction
/// </summary>
public class ProductService
{
    private readonly ICache _cache;

    // Dependency injection
    public ProductService(ICache cache)
    {
        _cache = cache;
    }

    public void GetProduct(int productId)
    {
        Console.WriteLine($"\n[CORRECT] Getting product: {productId}");

        var cacheKey = $"product:{productId}";

        // Works with ANY cache implementation!
        var cached = _cache.Get(cacheKey);

        if (cached != null)
        {
            Console.WriteLine($"  Found in cache: {cached}");
        }
        else
        {
            Console.WriteLine($"  Cache miss, loading from database...");
            var productData = $"Product_{productId}_Data";
            _cache.Set(cacheKey, productData);
            Console.WriteLine($"  Loaded and cached: {productData}");
        }
    }
}

#endregion

/// <summary>
/// Demonstrates the benefits of Dependency Inversion Principle
/// </summary>
public class DependencyInversionCorrectDemo
{
    public static void DemonstrateBenefits()
    {
        Console.WriteLine("\n=== BENEFITS OF DEPENDENCY INVERSION PRINCIPLE ===");

        Console.WriteLine("\nBenefit 1: Loose coupling");
        Console.WriteLine("  High-level modules don't depend on low-level modules");
        Console.WriteLine("  Both depend on abstractions");

        Console.WriteLine("\nBenefit 2: Easy to test");
        Console.WriteLine("  Can inject mock/stub implementations");
        Console.WriteLine("  No need for real infrastructure in tests");

        Console.WriteLine("\nBenefit 3: Easy to swap implementations");
        Console.WriteLine("  Change from SQL to MongoDB? Just swap the implementation!");
        Console.WriteLine("  No changes to business logic required");

        Console.WriteLine("\nBenefit 4: Runtime composition");
        Console.WriteLine("  Configure different implementations for different environments");
        Console.WriteLine("  Development, Testing, Production can use different implementations");

        DemonstrateNotificationSystem();
        DemonstrateDataAccess();
        DemonstrateLogging();
        DemonstrateCaching();
    }

    private static void DemonstrateNotificationSystem()
    {
        Console.WriteLine("\n--- Notification System with DI ---");

        // Production: Use email and SMS
        var productionNotifications = new List<INotificationService>
        {
            new EmailNotificationService(),
            new SmsNotificationService()
        };
        var productionOrder = new OrderService(productionNotifications);
        productionOrder.PlaceOrder("customer@example.com", 99.99m);

        // Development: Use only console logging (via custom implementation)
        Console.WriteLine("\n--- Switching to different notifications ---");
        var devNotifications = new List<INotificationService>
        {
            new PushNotificationService(),
            new SlackNotificationService()
        };
        var devOrder = new OrderService(devNotifications);
        devOrder.PlaceOrder("@dev-channel", 149.99m);

        Console.WriteLine("\n[CORRECT] Swapped implementations without changing OrderService!");
    }

    private static void DemonstrateDataAccess()
    {
        Console.WriteLine("\n--- Data Access with DI ---");

        // Production: Use SQL Server
        var sqlRepository = new SqlUserRepository();
        var productionUserManager = new UserManager(sqlRepository);
        productionUserManager.RegisterUser("Alice", "alice@example.com");

        // Development: Use MongoDB
        Console.WriteLine("\n--- Switching to MongoDB ---");
        var mongoRepository = new MongoUserRepository();
        var mongoUserManager = new UserManager(mongoRepository);
        mongoUserManager.RegisterUser("Bob", "bob@example.com");

        // Testing: Use in-memory
        Console.WriteLine("\n--- Switching to in-memory for tests ---");
        var memoryRepository = new InMemoryUserRepository();
        var testUserManager = new UserManager(memoryRepository);
        testUserManager.RegisterUser("Charlie", "charlie@example.com");
        testUserManager.GetUserInfo(1);

        Console.WriteLine("\n[CORRECT] Swapped databases without changing UserManager!");
    }

    private static void DemonstrateLogging()
    {
        Console.WriteLine("\n--- Logging with DI ---");

        // Console logging for development
        var consoleLogger = new ConsoleLogger();
        var devProcessor = new PaymentProcessor(consoleLogger);
        devProcessor.ProcessPayment(99.99m);

        // File logging for production
        Console.WriteLine("\n--- Switching to file logging ---");
        var fileLogger = new FileLoggerCorrect("app.log");
        var productionProcessor = new PaymentProcessor(fileLogger);
        productionProcessor.ProcessPayment(149.99m);

        // Composite logging - log to multiple destinations
        Console.WriteLine("\n--- Using composite logger ---");
        var compositeLogger = new CompositeLogger(new ILogger[]
        {
            new ConsoleLogger(),
            new FileLoggerCorrect("audit.log")
        });
        var auditProcessor = new PaymentProcessor(compositeLogger);
        auditProcessor.ProcessPayment(199.99m);

        Console.WriteLine("\n[CORRECT] Swapped loggers without changing PaymentProcessor!");
    }

    private static void DemonstrateCaching()
    {
        Console.WriteLine("\n--- Caching with DI ---");

        // Production: Use Redis
        var redisCache = new RedisCacheCorrect();
        var productionService = new ProductService(redisCache);
        productionService.GetProduct(123);

        // Development: Use in-memory cache
        Console.WriteLine("\n--- Switching to in-memory cache ---");
        var memoryCache = new InMemoryCache();
        var devService = new ProductService(memoryCache);
        devService.GetProduct(456);

        // Testing: Disable cache
        Console.WriteLine("\n--- Disabling cache for tests ---");
        var nullCache = new NullCache();
        var testService = new ProductService(nullCache);
        testService.GetProduct(789);

        Console.WriteLine("\n[CORRECT] Swapped cache implementations without changing ProductService!");
    }
}
