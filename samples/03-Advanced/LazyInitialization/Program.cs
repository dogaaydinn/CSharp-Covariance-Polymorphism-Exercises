// Lazy Initialization: Deferred object creation

namespace LazyInitialization;

public class Program
{
    public static void Main()
    {
        Console.WriteLine("=== Lazy Initialization Demo ===\n");

        // ❌ BAD: Eager loading (expensive)
        Console.WriteLine("❌ BAD - Eager initialization:");
        var bad = new BadResourceManager();
        Console.WriteLine("Instance created (heavy resource loaded even if not used!)\n");

        // ✅ GOOD: Lazy<T>
        Console.WriteLine("✅ GOOD - Lazy<T>:");
        var good = new ResourceManager();
        Console.WriteLine("Instance created (no heavy resource yet)");
        Console.WriteLine("Accessing resource...");
        var data = good.GetData();
        Console.WriteLine($"Data: {data}");
        Console.WriteLine("Accessing again (cached)...");
        var data2 = good.GetData();

        // Advanced: Lazy properties
        Console.WriteLine("\n✅ ADVANCED - Lazy properties:");
        var config = new Configuration();
        Console.WriteLine("Configuration created");
        Console.WriteLine($"Database: {config.DatabaseConnection}");
        Console.WriteLine($"Cache: {config.CacheConnection}");

        Console.WriteLine("\n=== Lazy Initialization Applied ===");
    }
}

// ❌ BAD: Eager initialization
public class BadResourceManager
{
    private readonly HeavyResource _resource;

    public BadResourceManager()
    {
        _resource = new HeavyResource(); // Loaded immediately!
        Console.WriteLine("❌ Heavy resource loaded in constructor");
    }
}

// ✅ GOOD: Lazy<T>
public class ResourceManager
{
    private readonly Lazy<HeavyResource> _resource;

    public ResourceManager()
    {
        _resource = new Lazy<HeavyResource>(() =>
        {
            Console.WriteLine("✅ Loading heavy resource NOW (first access)");
            return new HeavyResource();
        });
    }

    public string GetData()
    {
        return _resource.Value.GetData(); // Loaded on first access
    }
}

public class HeavyResource
{
    public HeavyResource()
    {
        // Simulate expensive initialization
        Thread.Sleep(500);
    }

    public string GetData() => "Important data from heavy resource";
}

// Advanced: Lazy properties
public class Configuration
{
    private readonly Lazy<string> _databaseConnection;
    private readonly Lazy<string> _cacheConnection;
    private readonly Lazy<Dictionary<string, string>> _settings;

    public Configuration()
    {
        _databaseConnection = new Lazy<string>(() =>
        {
            Console.WriteLine("  ✅ Loading database config...");
            Thread.Sleep(200);
            return "Server=localhost;Database=MyDb";
        });

        _cacheConnection = new Lazy<string>(() =>
        {
            Console.WriteLine("  ✅ Loading cache config...");
            Thread.Sleep(150);
            return "localhost:6379";
        });

        _settings = new Lazy<Dictionary<string, string>>(() =>
        {
            Console.WriteLine("  ✅ Loading settings...");
            return new Dictionary<string, string>
            {
                ["AppName"] = "MyApp",
                ["Version"] = "1.0.0"
            };
        });
    }

    public string DatabaseConnection => _databaseConnection.Value;
    public string CacheConnection => _cacheConnection.Value;
    public Dictionary<string, string> Settings => _settings.Value;
}

// BENCHMARK
// Approach        | Startup Time | Memory  | Access Time
// ----------------|--------------|---------|------------
// Eager           | 500ms        | High    | 0ms
// Lazy            | 0ms          | Low     | 500ms (first), 0ms (after)
// Use Lazy when resource might not be used, or used infrequently
