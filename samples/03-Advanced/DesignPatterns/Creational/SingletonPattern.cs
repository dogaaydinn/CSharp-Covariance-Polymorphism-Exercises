using System;
using System.Threading;

namespace DesignPatterns.Creational;

/// <summary>
/// SINGLETON PATTERN - Ensures a class has only one instance and provides global access point
///
/// Problem:
/// - Need exactly one instance of a class (database connection, configuration manager)
/// - Multiple instances could cause issues (resource conflicts, inconsistent state)
/// - Need global access without using static classes
///
/// UML Structure:
/// ┌─────────────────────┐
/// │    Singleton        │
/// ├─────────────────────┤
/// │ -instance: Singleton│ (static)
/// │ -Singleton()        │ (private constructor)
/// ├─────────────────────┤
/// │ +Instance: Singleton│ (static property)
/// │ +Operation()        │
/// └─────────────────────┘
///
/// When to Use:
/// - Configuration managers
/// - Database connections
/// - Thread pools
/// - Logging services
/// - Caching mechanisms
///
/// Thread Safety:
/// This implementation uses Lazy<T> which is thread-safe and lazy-initialized
/// </summary>
public sealed class DatabaseConnection
{
    // Lazy<T> provides thread-safe lazy initialization
    private static readonly Lazy<DatabaseConnection> _instance =
        new Lazy<DatabaseConnection>(() => new DatabaseConnection());

    /// <summary>
    /// Gets the singleton instance
    /// </summary>
    public static DatabaseConnection Instance => _instance.Value;

    private bool _isConnected;
    private readonly string _connectionString;
    private int _queryCount;

    /// <summary>
    /// Private constructor prevents external instantiation
    /// </summary>
    private DatabaseConnection()
    {
        _connectionString = "Server=localhost;Database=MyDb;";
        Console.WriteLine("  [Singleton] Database connection initialized");
        Console.WriteLine($"  [Singleton] Connection string: {_connectionString}");
    }

    /// <summary>
    /// Establishes database connection
    /// </summary>
    public void Connect()
    {
        if (!_isConnected)
        {
            Console.WriteLine("  [Singleton] Connecting to database...");
            Thread.Sleep(100); // Simulate connection delay
            _isConnected = true;
            Console.WriteLine("  [Singleton] Connected successfully");
        }
        else
        {
            Console.WriteLine("  [Singleton] Already connected");
        }
    }

    /// <summary>
    /// Executes a database query
    /// </summary>
    public void Query(string sql)
    {
        if (!_isConnected)
        {
            throw new InvalidOperationException("Not connected to database");
        }

        _queryCount++;
        Console.WriteLine($"  [Singleton] Query #{_queryCount}: {sql}");
    }

    /// <summary>
    /// Disconnects from database
    /// </summary>
    public void Disconnect()
    {
        if (_isConnected)
        {
            Console.WriteLine("  [Singleton] Disconnecting...");
            _isConnected = false;
        }
    }

    /// <summary>
    /// Gets current connection status
    /// </summary>
    public bool IsConnected => _isConnected;

    /// <summary>
    /// Gets total query count
    /// </summary>
    public int QueryCount => _queryCount;
}

/// <summary>
/// Alternative Singleton implementation using static constructor
/// This approach is also thread-safe and eager-initialized
/// </summary>
public sealed class ConfigurationManager
{
    // Static field initialized when class is first accessed
    private static readonly ConfigurationManager _instance = new ConfigurationManager();

    /// <summary>
    /// Static constructor ensures thread safety
    /// </summary>
    static ConfigurationManager()
    {
        // Static constructor is called only once per AppDomain
        Console.WriteLine("  [Singleton] ConfigurationManager static constructor called");
    }

    /// <summary>
    /// Private constructor prevents external instantiation
    /// </summary>
    private ConfigurationManager()
    {
        LoadConfiguration();
    }

    /// <summary>
    /// Gets the singleton instance
    /// </summary>
    public static ConfigurationManager Instance => _instance;

    private readonly Dictionary<string, string> _settings = new();

    private void LoadConfiguration()
    {
        Console.WriteLine("  [Singleton] Loading configuration...");
        _settings["AppName"] = "DesignPatterns Demo";
        _settings["Version"] = "1.0.0";
        _settings["Environment"] = "Development";
        Console.WriteLine($"  [Singleton] Loaded {_settings.Count} configuration settings");
    }

    /// <summary>
    /// Gets a configuration value
    /// </summary>
    public string GetSetting(string key)
    {
        return _settings.TryGetValue(key, out var value) ? value : "Not found";
    }

    /// <summary>
    /// Updates a configuration value
    /// </summary>
    public void SetSetting(string key, string value)
    {
        _settings[key] = value;
        Console.WriteLine($"  [Singleton] Updated setting: {key} = {value}");
    }

    /// <summary>
    /// Gets all configuration settings
    /// </summary>
    public IReadOnlyDictionary<string, string> GetAllSettings() => _settings;
}

/// <summary>
/// Example demonstrating Singleton pattern usage
/// </summary>
public static class SingletonExample
{
    public static void Run()
    {
        Console.WriteLine();
        Console.WriteLine("1. SINGLETON PATTERN - Ensures only one instance exists");
        Console.WriteLine("-".PadRight(70, '-'));
        Console.WriteLine();

        // Example 1: Database Connection Singleton
        Console.WriteLine("Example 1: Database Connection (Lazy<T> implementation)");
        Console.WriteLine();

        var db1 = DatabaseConnection.Instance;
        var db2 = DatabaseConnection.Instance;

        Console.WriteLine($"  Same instance? {ReferenceEquals(db1, db2)}");
        Console.WriteLine($"  Instance hash: {db1.GetHashCode()}");
        Console.WriteLine();

        db1.Connect();
        db1.Query("SELECT * FROM users");
        db2.Query("SELECT * FROM products"); // Same instance!

        Console.WriteLine($"  Total queries executed: {db1.QueryCount}");
        Console.WriteLine();

        // Example 2: Configuration Manager Singleton
        Console.WriteLine("Example 2: Configuration Manager (Static constructor)");
        Console.WriteLine();

        var config1 = ConfigurationManager.Instance;
        var config2 = ConfigurationManager.Instance;

        Console.WriteLine($"  Same instance? {ReferenceEquals(config1, config2)}");
        Console.WriteLine();

        Console.WriteLine("  Current settings:");
        foreach (var setting in config1.GetAllSettings())
        {
            Console.WriteLine($"    {setting.Key}: {setting.Value}");
        }

        Console.WriteLine();
        config1.SetSetting("LogLevel", "Debug");
        Console.WriteLine($"  LogLevel from config2: {config2.GetSetting("LogLevel")}");
        Console.WriteLine();

        // Demonstrate thread safety
        Console.WriteLine("Example 3: Thread Safety Test");
        Console.WriteLine();

        var threads = new List<Thread>();
        for (int i = 0; i < 5; i++)
        {
            int threadNum = i;
            threads.Add(new Thread(() =>
            {
                var instance = DatabaseConnection.Instance;
                Console.WriteLine($"  Thread {threadNum}: Hash = {instance.GetHashCode()}");
            }));
        }

        foreach (var thread in threads) thread.Start();
        foreach (var thread in threads) thread.Join();

        Console.WriteLine();
        Console.WriteLine("  All threads received the same instance!");
    }
}
