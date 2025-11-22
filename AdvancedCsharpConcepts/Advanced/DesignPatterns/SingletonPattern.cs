namespace AdvancedCsharpConcepts.Advanced.DesignPatterns;

/// <summary>
/// Singleton Pattern - Ensures a class has only one instance and provides global access.
/// Silicon Valley best practice: Use for shared resources like configuration, logging, caching.
/// </summary>
public static class SingletonPattern
{
    /// <summary>
    /// Thread-safe Singleton using Lazy&lt;T&gt; (recommended approach).
    /// </summary>
    public sealed class ConfigurationManager
    {
        private static readonly Lazy<ConfigurationManager> _instance =
            new(() => new ConfigurationManager());

        private readonly Dictionary<string, string> _settings = new();

        /// <summary>
        /// Gets the singleton instance.
        /// </summary>
        public static ConfigurationManager Instance => _instance.Value;

        /// <summary>
        /// Private constructor prevents external instantiation.
        /// </summary>
        private ConfigurationManager()
        {
            // Load default configuration
            _settings["AppName"] = "AdvancedCsharpConcepts";
            _settings["Version"] = "3.1.0";
            _settings["Environment"] = "Production";
        }

        public string GetSetting(string key)
        {
            return _settings.TryGetValue(key, out var value) ? value : string.Empty;
        }

        public void SetSetting(string key, string value)
        {
            _settings[key] = value;
        }

        public IReadOnlyDictionary<string, string> GetAllSettings()
        {
            return _settings;
        }
    }

    /// <summary>
    /// Classic Double-Check Locking Singleton (alternative approach).
    /// </summary>
    public sealed class Logger
    {
        private static Logger? _instance;
        private static readonly object _lock = new();
        private readonly List<string> _logs = new();

        /// <summary>
        /// Gets the singleton instance with double-check locking.
        /// </summary>
        public static Logger Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = new Logger();
                        }
                    }
                }
                return _instance;
            }
        }

        private Logger()
        {
            Console.WriteLine("[Logger] Singleton instance created");
        }

        public void Log(string message)
        {
            var timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
            var logEntry = $"[{timestamp}] {message}";
            _logs.Add(logEntry);
            Console.WriteLine(logEntry);
        }

        public IReadOnlyList<string> GetLogs() => _logs;

        public void ClearLogs() => _logs.Clear();
    }

    /// <summary>
    /// Singleton using Static Constructor (thread-safe by CLR).
    /// </summary>
    public sealed class CacheManager
    {
        private static readonly CacheManager _instance = new();
        private readonly Dictionary<string, object> _cache = new();

        /// <summary>
        /// Gets the singleton instance.
        /// </summary>
        public static CacheManager Instance => _instance;

        /// <summary>
        /// Static constructor ensures thread-safe initialization.
        /// </summary>
        static CacheManager()
        {
            Console.WriteLine("[CacheManager] Static constructor called");
        }

        private CacheManager()
        {
            Console.WriteLine("[CacheManager] Instance constructor called");
        }

        public void Set<T>(string key, T value) where T : notnull
        {
            _cache[key] = value;
        }

        public T? Get<T>(string key)
        {
            return _cache.TryGetValue(key, out var value) && value is T typedValue
                ? typedValue
                : default;
        }

        public bool Contains(string key) => _cache.ContainsKey(key);

        public void Remove(string key) => _cache.Remove(key);

        public void Clear() => _cache.Clear();

        public int Count => _cache.Count;
    }

    /// <summary>
    /// Connection Pool Singleton (realistic production example).
    /// </summary>
    public sealed class DatabaseConnectionPool
    {
        private static readonly Lazy<DatabaseConnectionPool> _instance =
            new(() => new DatabaseConnectionPool());

        private readonly Queue<Connection> _availableConnections = new();
        private readonly List<Connection> _allConnections = new();
        private readonly int _maxPoolSize;
        private readonly object _lock = new();

        public static DatabaseConnectionPool Instance => _instance.Value;

        private DatabaseConnectionPool(int maxPoolSize = 10)
        {
            _maxPoolSize = maxPoolSize;
            Console.WriteLine($"[ConnectionPool] Initialized with max size: {_maxPoolSize}");
        }

        public Connection GetConnection()
        {
            lock (_lock)
            {
                if (_availableConnections.Count > 0)
                {
                    return _availableConnections.Dequeue();
                }

                if (_allConnections.Count < _maxPoolSize)
                {
                    var connection = new Connection(_allConnections.Count + 1);
                    _allConnections.Add(connection);
                    return connection;
                }

                throw new InvalidOperationException("Connection pool exhausted");
            }
        }

        public void ReturnConnection(Connection connection)
        {
            if (connection == null)
                throw new ArgumentNullException(nameof(connection));

            lock (_lock)
            {
                if (_allConnections.Contains(connection) && !_availableConnections.Contains(connection))
                {
                    _availableConnections.Enqueue(connection);
                }
            }
        }

        public PoolStats GetStats()
        {
            lock (_lock)
            {
                return new PoolStats
                {
                    TotalConnections = _allConnections.Count,
                    AvailableConnections = _availableConnections.Count,
                    InUseConnections = _allConnections.Count - _availableConnections.Count,
                    MaxPoolSize = _maxPoolSize
                };
            }
        }
    }

    /// <summary>
    /// Represents a database connection (mock).
    /// </summary>
    public class Connection
    {
        public int Id { get; }

        public Connection(int id)
        {
            Id = id;
        }

        public override string ToString() => $"Connection #{Id}";
    }

    /// <summary>
    /// Connection pool statistics.
    /// </summary>
    public record PoolStats
    {
        public int TotalConnections { get; init; }
        public int AvailableConnections { get; init; }
        public int InUseConnections { get; init; }
        public int MaxPoolSize { get; init; }
    }

    /// <summary>
    /// Demonstrates the Singleton Pattern.
    /// </summary>
    public static void RunExample()
    {
        Console.WriteLine("=== Singleton Pattern Examples ===\n");

        // Example 1: Configuration Manager
        Console.WriteLine("1. Configuration Manager (Lazy<T> Singleton):");
        var config1 = ConfigurationManager.Instance;
        var config2 = ConfigurationManager.Instance;

        Console.WriteLine($"  Same instance? {ReferenceEquals(config1, config2)}");
        Console.WriteLine($"  AppName: {config1.GetSetting("AppName")}");
        Console.WriteLine($"  Version: {config1.GetSetting("Version")}");

        config1.SetSetting("Database", "PostgreSQL");
        Console.WriteLine($"  Database (from config2): {config2.GetSetting("Database")}");

        // Example 2: Logger
        Console.WriteLine("\n2. Logger (Double-Check Locking Singleton):");
        var logger1 = Logger.Instance;
        var logger2 = Logger.Instance;

        Console.WriteLine($"  Same instance? {ReferenceEquals(logger1, logger2)}");
        logger1.Log("Application started");
        logger2.Log("Processing request");
        Console.WriteLine($"  Total logs: {logger1.GetLogs().Count}");

        // Example 3: Cache Manager
        Console.WriteLine("\n3. Cache Manager (Static Constructor Singleton):");
        var cache1 = CacheManager.Instance;
        var cache2 = CacheManager.Instance;

        Console.WriteLine($"  Same instance? {ReferenceEquals(cache1, cache2)}");
        cache1.Set("user:123", new { Name = "Alice", Email = "alice@example.com" });
        cache1.Set("product:456", "Laptop");

        var user = cache2.Get<object>("user:123");
        Console.WriteLine($"  Retrieved user from cache2: {user}");
        Console.WriteLine($"  Cache count: {cache2.Count}");

        // Example 4: Connection Pool
        Console.WriteLine("\n4. Database Connection Pool:");
        var pool = DatabaseConnectionPool.Instance;

        var conn1 = pool.GetConnection();
        var conn2 = pool.GetConnection();
        var conn3 = pool.GetConnection();

        Console.WriteLine($"  Acquired connections: {conn1}, {conn2}, {conn3}");

        var stats = pool.GetStats();
        Console.WriteLine($"  Pool stats: {stats.InUseConnections}/{stats.TotalConnections} in use");

        pool.ReturnConnection(conn2);
        stats = pool.GetStats();
        Console.WriteLine($"  After returning conn2: {stats.InUseConnections}/{stats.TotalConnections} in use");

        // Demonstrate thread safety
        Console.WriteLine("\n5. Thread Safety Test:");
        var tasks = new List<Task>();
        for (var i = 0; i < 5; i++)
        {
            var taskId = i;
            tasks.Add(Task.Run(() =>
            {
                var cfg = ConfigurationManager.Instance;
                Console.WriteLine($"  Thread {taskId}: Got config instance (HashCode: {cfg.GetHashCode()})");
            }));
        }
        Task.WaitAll(tasks.ToArray());
    }
}
