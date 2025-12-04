using System.Collections;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using System.Text.Json;

namespace BoxingPerformance.Examples;

/// <summary>
/// Real-world scenarios where boxing occurs and causes production performance issues.
/// Demonstrates common pitfalls and their solutions.
/// </summary>
/// <remarks>
/// Common Production Issues:
/// 1. Legacy code using non-generic collections
/// 2. LINQ queries with mixed types
/// 3. Logging frameworks boxing parameters
/// 4. Serialization/deserialization boxing
/// 5. Event handlers boxing event args
/// 6. Reflection-based scenarios
/// 7. String formatting in loops
/// 8. Database parameter handling
/// </remarks>
public static class RealWorldScenarios
{
    /// <summary>
    /// Scenario 1: Legacy code with non-generic collections in production.
    /// Shows migration path from ArrayList to List&lt;T&gt;.
    /// </summary>
    public static void LegacyCollectionMigration()
    {
        Console.WriteLine("=== Scenario 1: Legacy Collection Migration ===");
        Console.WriteLine();
        Console.WriteLine("Problem: Inherited codebase using ArrayList for order processing");
        Console.WriteLine();

        const int orderCount = 50_000;

        // BEFORE: Legacy code (boxing)
        Console.WriteLine("BEFORE - Legacy code with ArrayList:");
        var sw1 = Stopwatch.StartNew();
        ArrayList legacyOrders = new ArrayList();

        for (int i = 0; i < orderCount; i++)
        {
            var order = new Order
            {
                OrderId = i,
                Amount = i * 10.5m,
                CustomerId = i % 1000
            };
            legacyOrders.Add(order);  // Boxing if Order is struct!
        }

        // Process orders
        decimal totalAmount = 0;
        foreach (var obj in legacyOrders)
        {
            var order = (Order)obj;  // Cast required
            totalAmount += order.Amount;
        }
        sw1.Stop();

        Console.WriteLine($"  Time: {sw1.ElapsedMilliseconds} ms");
        Console.WriteLine($"  Total: ${totalAmount:N2}");
        Console.WriteLine($"  Issues: Type safety, casting overhead, unclear intent");
        Console.WriteLine();

        // AFTER: Modern code (no boxing)
        Console.WriteLine("AFTER - Modern code with List<Order>:");
        var sw2 = Stopwatch.StartNew();
        List<Order> modernOrders = new List<Order>(orderCount);

        for (int i = 0; i < orderCount; i++)
        {
            var order = new Order
            {
                OrderId = i,
                Amount = i * 10.5m,
                CustomerId = i % 1000
            };
            modernOrders.Add(order);  // No boxing, type-safe
        }

        // Process orders
        totalAmount = 0;
        foreach (var order in modernOrders)  // No cast needed
        {
            totalAmount += order.Amount;
        }
        sw2.Stop();

        Console.WriteLine($"  Time: {sw2.ElapsedMilliseconds} ms");
        Console.WriteLine($"  Total: ${totalAmount:N2}");
        Console.WriteLine($"  Benefits: Type safety, {sw1.ElapsedMilliseconds / Math.Max(sw2.ElapsedMilliseconds, 1)}x faster, cleaner code");
        Console.WriteLine();
    }

    /// <summary>
    /// Scenario 2: Logging framework boxing parameters.
    /// Common issue with structured logging.
    /// </summary>
    public static void LoggingFrameworkBoxing()
    {
        Console.WriteLine("=== Scenario 2: Logging Framework Boxing ===");
        Console.WriteLine();
        Console.WriteLine("Problem: High-frequency logging causing GC pressure");
        Console.WriteLine();

        const int logCount = 100_000;
        int userId = 12345;
        long timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

        // BAD: Boxing on every log call
        Console.WriteLine("BAD - Traditional logging (boxing):");
        var sw1 = Stopwatch.StartNew();
        for (int i = 0; i < logCount; i++)
        {
            LogWithBoxing("User {0} performed action at {1}", userId, timestamp);
        }
        sw1.Stop();
        Console.WriteLine($"  Time: {sw1.ElapsedMilliseconds} ms");
        Console.WriteLine($"  Issue: Every int/long parameter is boxed to object[]");
        Console.WriteLine();

        // GOOD: Structured logging without boxing
        Console.WriteLine("GOOD - Structured logging with interpolation:");
        var sw2 = Stopwatch.StartNew();
        for (int i = 0; i < logCount; i++)
        {
            LogWithoutBoxing(userId, timestamp);
        }
        sw2.Stop();
        Console.WriteLine($"  Time: {sw2.ElapsedMilliseconds} ms");
        Console.WriteLine($"  Benefit: No boxing, {sw1.ElapsedMilliseconds / Math.Max(sw2.ElapsedMilliseconds, 1)}x faster");
        Console.WriteLine();

        // BETTER: Log level check to avoid unnecessary work
        Console.WriteLine("BETTER - Guard with log level check:");
        Console.WriteLine("  if (logger.IsEnabled(LogLevel.Debug))");
        Console.WriteLine("      logger.LogDebug(\"User {UserId} at {Timestamp}\", userId, timestamp);");
        Console.WriteLine("  Prevents boxing when log level is disabled");
        Console.WriteLine();
    }

    /// <summary>
    /// Scenario 3: LINQ queries with value types.
    /// Shows boxing in Select/Where with lambda captures.
    /// </summary>
    public static void LinqBoxingPitfalls()
    {
        Console.WriteLine("=== Scenario 3: LINQ Boxing Pitfalls ===");
        Console.WriteLine();
        Console.WriteLine("Problem: LINQ operations on value types can cause boxing");
        Console.WriteLine();

        var numbers = Enumerable.Range(1, 100_000).ToArray();

        // BAD: Boxing with non-generic IEnumerable
        Console.WriteLine("BAD - Non-generic IEnumerable (boxing):");
        var sw1 = Stopwatch.StartNew();
        ArrayList boxedList = new ArrayList(numbers);
        var badResult = boxedList
            .Cast<int>()  // Unboxing
            .Where(n => n % 2 == 0)
            .Select(n => n * 2)
            .ToList();
        sw1.Stop();
        Console.WriteLine($"  Time: {sw1.ElapsedMilliseconds} ms");
        Console.WriteLine($"  Result count: {badResult.Count}");
        Console.WriteLine();

        // GOOD: Generic collections throughout
        Console.WriteLine("GOOD - Generic IEnumerable<int> (no boxing):");
        var sw2 = Stopwatch.StartNew();
        var goodResult = numbers
            .Where(n => n % 2 == 0)
            .Select(n => n * 2)
            .ToList();
        sw2.Stop();
        Console.WriteLine($"  Time: {sw2.ElapsedMilliseconds} ms");
        Console.WriteLine($"  Result count: {goodResult.Count}");
        Console.WriteLine($"  Benefit: {sw1.ElapsedMilliseconds / Math.Max(sw2.ElapsedMilliseconds, 1)}x faster, no boxing");
        Console.WriteLine();

        // PITFALL: Capturing variables
        Console.WriteLine("PITFALL - Be careful with captures:");
        Console.WriteLine("  var threshold = 50;  // Value type");
        Console.WriteLine("  var result = numbers.Where(n => n > threshold);  // No boxing");
        Console.WriteLine("  BUT if threshold is object: boxing occurs in lambda!");
        Console.WriteLine();
    }

    /// <summary>
    /// Scenario 4: String building in hot paths.
    /// Common performance killer in request processing.
    /// </summary>
    public static void StringBuildingInHotPath()
    {
        Console.WriteLine("=== Scenario 4: String Building in Hot Path ===");
        Console.WriteLine();
        Console.WriteLine("Problem: Building JSON response strings in API endpoints");
        Console.WriteLine();

        const int requestCount = 10_000;
        var data = new ResponseData { Id = 123, Name = "Product", Price = 99.99m, InStock = true };

        // BAD: String concatenation (boxing + allocations)
        Console.WriteLine("BAD - String concatenation (boxing):");
        var sw1 = Stopwatch.StartNew();
        for (int i = 0; i < requestCount; i++)
        {
            string json = "{" +
                "\"id\":" + data.Id + "," +
                "\"name\":\"" + data.Name + "\"," +
                "\"price\":" + data.Price + "," +
                "\"inStock\":" + data.InStock.ToString().ToLower() +
                "}";
        }
        sw1.Stop();
        Console.WriteLine($"  Time: {sw1.ElapsedMilliseconds} ms");
        Console.WriteLine($"  Issues: Multiple allocations, boxing, O(n²) complexity");
        Console.WriteLine();

        // BETTER: StringBuilder (minimal boxing)
        Console.WriteLine("BETTER - StringBuilder (minimal boxing):");
        var sw2 = Stopwatch.StartNew();
        for (int i = 0; i < requestCount; i++)
        {
            var sb = new StringBuilder(100);
            sb.Append("{\"id\":");
            sb.Append(data.Id);  // No boxing - Append(int) overload
            sb.Append(",\"name\":\"");
            sb.Append(data.Name);
            sb.Append("\",\"price\":");
            sb.Append(data.Price);  // No boxing - Append(decimal) overload
            sb.Append(",\"inStock\":");
            sb.Append(data.InStock ? "true" : "false");
            sb.Append("}");
            string json = sb.ToString();
        }
        sw2.Stop();
        Console.WriteLine($"  Time: {sw2.ElapsedMilliseconds} ms");
        Console.WriteLine($"  Improvement: {sw1.ElapsedMilliseconds / Math.Max(sw2.ElapsedMilliseconds, 1)}x faster");
        Console.WriteLine();

        // BEST: System.Text.Json (optimized)
        Console.WriteLine("BEST - System.Text.Json (fully optimized):");
        var sw3 = Stopwatch.StartNew();
        for (int i = 0; i < requestCount; i++)
        {
            string json = JsonSerializer.Serialize(data);
        }
        sw3.Stop();
        Console.WriteLine($"  Time: {sw3.ElapsedMilliseconds} ms");
        Console.WriteLine($"  Benefits: No manual boxing, optimized, type-safe");
        Console.WriteLine();
    }

    /// <summary>
    /// Scenario 5: Event handlers and delegates boxing.
    /// Common in UI frameworks and event-driven architectures.
    /// </summary>
    public static void EventHandlerBoxing()
    {
        Console.WriteLine("=== Scenario 5: Event Handler Boxing ===");
        Console.WriteLine();
        Console.WriteLine("Problem: EventArgs with value types in high-frequency events");
        Console.WriteLine();

        const int eventCount = 100_000;

        // BAD: Boxing in event args
        Console.WriteLine("BAD - Value type in EventArgs (boxing):");
        var publisher1 = new BadEventPublisher();
        var sw1 = Stopwatch.StartNew();
        publisher1.ValueChanged += (sender, args) =>
        {
            int value = (int)args.Value;  // Unboxing required
        };

        for (int i = 0; i < eventCount; i++)
        {
            publisher1.RaiseEvent(i);  // Boxing on every event
        }
        sw1.Stop();
        Console.WriteLine($"  Time: {sw1.ElapsedMilliseconds} ms");
        Console.WriteLine($"  Issue: Boxing on every event raise");
        Console.WriteLine();

        // GOOD: Generic event args
        Console.WriteLine("GOOD - Generic EventArgs<T> (no boxing):");
        var publisher2 = new GoodEventPublisher();
        var sw2 = Stopwatch.StartNew();
        publisher2.ValueChanged += (sender, args) =>
        {
            int value = args.Value;  // No unboxing needed
        };

        for (int i = 0; i < eventCount; i++)
        {
            publisher2.RaiseEvent(i);  // No boxing
        }
        sw2.Stop();
        Console.WriteLine($"  Time: {sw2.ElapsedMilliseconds} ms");
        Console.WriteLine($"  Improvement: {sw1.ElapsedMilliseconds / Math.Max(sw2.ElapsedMilliseconds, 1)}x faster, type-safe");
        Console.WriteLine();
    }

    /// <summary>
    /// Scenario 6: Reflection boxing.
    /// Common in serialization, DI containers, mapping libraries.
    /// </summary>
    public static void ReflectionBoxing()
    {
        Console.WriteLine("=== Scenario 6: Reflection Boxing ===");
        Console.WriteLine();
        Console.WriteLine("Problem: Property setters boxing value types via reflection");
        Console.WriteLine();

        const int iterations = 10_000;
        var data = new DataModel { Id = 0, Value = 0.0 };
        PropertyInfo idProp = typeof(DataModel).GetProperty(nameof(DataModel.Id))!;
        PropertyInfo valueProp = typeof(DataModel).GetProperty(nameof(DataModel.Value))!;

        // BAD: Reflection with boxing
        Console.WriteLine("BAD - Reflection SetValue (boxing):");
        var sw1 = Stopwatch.StartNew();
        for (int i = 0; i < iterations; i++)
        {
            idProp.SetValue(data, i);  // Boxing: int -> object
            valueProp.SetValue(data, (double)i);  // Boxing: double -> object
        }
        sw1.Stop();
        Console.WriteLine($"  Time: {sw1.ElapsedMilliseconds} ms");
        Console.WriteLine($"  Issue: Every SetValue boxes the value type");
        Console.WriteLine();

        // BETTER: Compiled expression (one-time boxing)
        Console.WriteLine("BETTER - Direct property access (no boxing):");
        var sw2 = Stopwatch.StartNew();
        for (int i = 0; i < iterations; i++)
        {
            data.Id = i;  // No boxing
            data.Value = i;  // No boxing
        }
        sw2.Stop();
        Console.WriteLine($"  Time: {sw2.ElapsedMilliseconds} ms");
        Console.WriteLine($"  Improvement: {sw1.ElapsedMilliseconds / Math.Max(sw2.ElapsedMilliseconds, 1)}x faster");
        Console.WriteLine();

        Console.WriteLine("SOLUTION for reflection scenarios:");
        Console.WriteLine("  1. Cache compiled expressions (Expression<T>)");
        Console.WriteLine("  2. Use source generators (compile-time code generation)");
        Console.WriteLine("  3. Consider FastMember or similar libraries");
        Console.WriteLine();
    }

    /// <summary>
    /// Scenario 7: Database parameter boxing.
    /// Common when adding parameters to ADO.NET commands.
    /// </summary>
    public static void DatabaseParameterBoxing()
    {
        Console.WriteLine("=== Scenario 7: Database Parameter Boxing ===");
        Console.WriteLine();
        Console.WriteLine("Problem: Adding value type parameters to SQL commands");
        Console.WriteLine();

        Console.WriteLine("BAD - Direct parameter value (boxing):");
        Console.WriteLine("  var cmd = new SqlCommand(\"SELECT * FROM Users WHERE Id = @Id\");");
        Console.WriteLine("  cmd.Parameters.AddWithValue(\"@Id\", userId);  // Boxing!");
        Console.WriteLine("  Issue: AddWithValue boxes value types to object");
        Console.WriteLine();

        Console.WriteLine("BETTER - Strongly-typed parameter:");
        Console.WriteLine("  var param = new SqlParameter(\"@Id\", SqlDbType.Int);");
        Console.WriteLine("  param.Value = userId;  // Still boxes, but clearer intent");
        Console.WriteLine();

        Console.WriteLine("BEST - Use micro-ORM (Dapper, EF Core):");
        Console.WriteLine("  var users = connection.Query<User>(");
        Console.WriteLine("      \"SELECT * FROM Users WHERE Id = @Id\",");
        Console.WriteLine("      new { Id = userId });");
        Console.WriteLine("  Benefits: Optimized parameter handling, less boxing");
        Console.WriteLine();

        Console.WriteLine("Measurement (simulated):");
        const int queryCount = 10_000;

        // Simulate boxing overhead
        var sw1 = Stopwatch.StartNew();
        for (int i = 0; i < queryCount; i++)
        {
            object boxed = i;  // Simulates AddWithValue boxing
            int unboxed = (int)boxed;
        }
        sw1.Stop();

        // Simulate no boxing
        var sw2 = Stopwatch.StartNew();
        for (int i = 0; i < queryCount; i++)
        {
            int value = i;  // No boxing
        }
        sw2.Stop();

        Console.WriteLine($"  With boxing:    {sw1.ElapsedMilliseconds} ms");
        Console.WriteLine($"  Without boxing: {sw2.ElapsedMilliseconds} ms");
        Console.WriteLine($"  Impact: {sw1.ElapsedMilliseconds / Math.Max(sw2.ElapsedMilliseconds, 1)}x slowdown from boxing alone");
        Console.WriteLine();
    }

    /// <summary>
    /// Scenario 8: Collection initialization boxing.
    /// Common mistake in collection builders.
    /// </summary>
    public static void CollectionInitializationBoxing()
    {
        Console.WriteLine("=== Scenario 8: Collection Initialization Boxing ===");
        Console.WriteLine();
        Console.WriteLine("Problem: Using object initializers with mixed types");
        Console.WriteLine();

        Console.WriteLine("BAD - Mixed type collection (boxing):");
        var sw1 = Stopwatch.StartNew();
        for (int i = 0; i < 10_000; i++)
        {
            var mixed = new ArrayList { 1, "two", 3.0, true };  // Boxing!
        }
        sw1.Stop();
        Console.WriteLine($"  Time: {sw1.ElapsedMilliseconds} ms");
        Console.WriteLine($"  Issue: All value types are boxed");
        Console.WriteLine();

        Console.WriteLine("GOOD - Type-safe initialization:");
        var sw2 = Stopwatch.StartNew();
        for (int i = 0; i < 10_000; i++)
        {
            var ints = new List<int> { 1, 2, 3 };
            var strings = new List<string> { "one", "two" };
            var bools = new List<bool> { true, false };
        }
        sw2.Stop();
        Console.WriteLine($"  Time: {sw2.ElapsedMilliseconds} ms");
        Console.WriteLine($"  Improvement: {sw1.ElapsedMilliseconds / Math.Max(sw2.ElapsedMilliseconds, 1)}x faster, type-safe");
        Console.WriteLine();

        Console.WriteLine("BEST - Collection expressions (C# 12+):");
        Console.WriteLine("  List<int> numbers = [1, 2, 3, 4, 5];");
        Console.WriteLine("  Benefits: Concise, type-safe, optimized by compiler");
        Console.WriteLine();
    }

    #region Helper Types and Methods

    private static void LogWithBoxing(string format, params object[] args)
    {
        // Simulates traditional logging - all args are boxed
    }

    private static void LogWithoutBoxing(int userId, long timestamp)
    {
        // Simulates structured logging with strongly-typed parameters
        string message = $"User {userId} performed action at {timestamp}";
    }

    #endregion

    /// <summary>
    /// Runs all real-world scenario demonstrations.
    /// </summary>
    public static void RunAll()
    {
        LegacyCollectionMigration();
        Console.WriteLine("=".PadRight(70, '='));
        Console.WriteLine();

        LoggingFrameworkBoxing();
        Console.WriteLine("=".PadRight(70, '='));
        Console.WriteLine();

        LinqBoxingPitfalls();
        Console.WriteLine("=".PadRight(70, '='));
        Console.WriteLine();

        StringBuildingInHotPath();
        Console.WriteLine("=".PadRight(70, '='));
        Console.WriteLine();

        EventHandlerBoxing();
        Console.WriteLine("=".PadRight(70, '='));
        Console.WriteLine();

        ReflectionBoxing();
        Console.WriteLine("=".PadRight(70, '='));
        Console.WriteLine();

        DatabaseParameterBoxing();
        Console.WriteLine("=".PadRight(70, '='));
        Console.WriteLine();

        CollectionInitializationBoxing();
    }
}

#region Supporting Types

public class Order
{
    public int OrderId { get; set; }
    public decimal Amount { get; set; }
    public int CustomerId { get; set; }
}

public class ResponseData
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public bool InStock { get; set; }
}

public class DataModel
{
    public int Id { get; set; }
    public double Value { get; set; }
}

// Bad event pattern - boxing
public class BadEventPublisher
{
    public event EventHandler<ValueEventArgs>? ValueChanged;

    public void RaiseEvent(int value)
    {
        ValueChanged?.Invoke(this, new ValueEventArgs { Value = value });
    }
}

public class ValueEventArgs : EventArgs
{
    public object Value { get; set; } = null!;  // Boxing occurs here
}

// Good event pattern - generic
public class GoodEventPublisher
{
    public event EventHandler<GenericEventArgs<int>>? ValueChanged;

    public void RaiseEvent(int value)
    {
        ValueChanged?.Invoke(this, new GenericEventArgs<int> { Value = value });
    }
}

public class GenericEventArgs<T> : EventArgs
{
    public T Value { get; set; } = default!;  // No boxing
}

#endregion
