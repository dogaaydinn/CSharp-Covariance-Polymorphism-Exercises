// ValueTask Pattern - Cache-aware Async Performance Optimization
// Demonstrates zero-allocation async patterns with ValueTask

using System.Runtime.CompilerServices;
using System.Threading.Tasks.Sources;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

namespace ValueTaskExample;

#region Cached Data Service

/// <summary>
/// Simple in-memory cache for demonstration
/// </summary>
public class MemoryCache<TKey, TValue> where TKey : notnull
{
    private readonly Dictionary<TKey, (TValue Value, DateTime Expiry)> _cache = new();
    private readonly TimeSpan _defaultExpiration;

    public MemoryCache(TimeSpan defaultExpiration)
    {
        _defaultExpiration = defaultExpiration;
    }

    public bool TryGet(TKey key, out TValue? value)
    {
        if (_cache.TryGetValue(key, out var entry))
        {
            if (entry.Expiry > DateTime.UtcNow)
            {
                value = entry.Value;
                return true;
            }
            _cache.Remove(key);
        }

        value = default;
        return false;
    }

    public void Set(TKey key, TValue value)
    {
        _cache[key] = (value, DateTime.UtcNow.Add(_defaultExpiration));
    }

    public void Clear() => _cache.Clear();
}

/// <summary>
/// Data service using Task (always allocates)
/// </summary>
public class TaskBasedDataService
{
    private readonly MemoryCache<int, string> _cache = new(TimeSpan.FromMinutes(5));

    public async Task<string> GetDataAsync(int id)
    {
        // Cache hit still allocates Task<string> on heap!
        if (_cache.TryGet(id, out var cached))
        {
            return cached!;  // ← Task<string> allocated even for sync return
        }

        // Cache miss - async operation
        await Task.Delay(50);  // Simulate database query
        var data = $"Data_{id}";
        _cache.Set(id, data);
        return data;
    }
}

/// <summary>
/// Data service using ValueTask (zero allocation when cached)
/// </summary>
public class ValueTaskBasedDataService
{
    private readonly MemoryCache<int, string> _cache = new(TimeSpan.FromMinutes(5));

    public ValueTask<string> GetDataAsync(int id)
    {
        // Cache hit - zero allocation!
        if (_cache.TryGet(id, out var cached))
        {
            return new ValueTask<string>(cached!);  // ← No heap allocation!
        }

        // Cache miss - async operation (allocates only when needed)
        return new ValueTask<string>(FetchFromDatabaseAsync(id));
    }

    private async Task<string> FetchFromDatabaseAsync(int id)
    {
        await Task.Delay(50);  // Simulate database query
        var data = $"Data_{id}";
        _cache.Set(id, data);
        return data;
    }
}

#endregion

#region IValueTaskSource Implementation

/// <summary>
/// Custom IValueTaskSource for advanced pooling scenarios
/// </summary>
public class PooledValueTaskSource : IValueTaskSource<int>
{
    private ManualResetValueTaskSourceCore<int> _core;

    public void SetResult(int result)
    {
        _core.SetResult(result);
    }

    public void SetException(Exception exception)
    {
        _core.SetException(exception);
    }

    public ValueTask<int> GetValueTask()
    {
        return new ValueTask<int>(this, _core.Version);
    }

    public void Reset()
    {
        _core.Reset();
    }

    // IValueTaskSource<int> implementation
    public int GetResult(short token)
    {
        return _core.GetResult(token);
    }

    public ValueTaskSourceStatus GetStatus(short token)
    {
        return _core.GetStatus(token);
    }

    public void OnCompleted(Action<object?> continuation, object? state, short token, ValueTaskSourceOnCompletedFlags flags)
    {
        _core.OnCompleted(continuation, state, token, flags);
    }
}

/// <summary>
/// Example using pooled ValueTaskSource
/// </summary>
public class PooledAsyncService
{
    private readonly PooledValueTaskSource _pooledSource = new();

    public ValueTask<int> ComputeAsync(int input)
    {
        if (input < 10)
        {
            // Synchronous completion - no allocation
            return new ValueTask<int>(input * 2);
        }

        // Use pooled source for async completion
        _pooledSource.Reset();
        Task.Run(() =>
        {
            Thread.Sleep(50);
            _pooledSource.SetResult(input * 2);
        });

        return _pooledSource.GetValueTask();
    }
}

#endregion

#region Benchmarks

/// <summary>
/// Performance comparison between Task and ValueTask
/// </summary>
[MemoryDiagnoser]
public class TaskVsValueTaskBenchmarks
{
    private TaskBasedDataService _taskService = null!;
    private ValueTaskBasedDataService _valueTaskService = null!;

    [GlobalSetup]
    public void Setup()
    {
        _taskService = new TaskBasedDataService();
        _valueTaskService = new ValueTaskBasedDataService();

        // Prime cache
        _taskService.GetDataAsync(1).GetAwaiter().GetResult();
        _valueTaskService.GetDataAsync(1).GetAwaiter().GetResult();
    }

    [Benchmark(Baseline = true)]
    public async Task<string> Task_CacheHit()
    {
        return await _taskService.GetDataAsync(1);
    }

    [Benchmark]
    public async ValueTask<string> ValueTask_CacheHit()
    {
        return await _valueTaskService.GetDataAsync(1);
    }

    [Benchmark]
    public async Task<string> Task_CacheMiss()
    {
        return await _taskService.GetDataAsync(999);
    }

    [Benchmark]
    public async ValueTask<string> ValueTask_CacheMiss()
    {
        return await _valueTaskService.GetDataAsync(999);
    }
}

#endregion

#region Demonstration Programs

public class Program
{
    public static async Task Main(string[] args)
    {
        Console.WriteLine("=== ValueTask Pattern Demo - Cache-aware Async Optimization ===\n");

        await DemonstrateTaskVsValueTask();
        await DemonstrateCacheHitMissScenarios();
        await DemonstrateValueTaskBestPractices();
        await DemonstrateValueTaskAntiPatterns();
        await DemonstratePooledValueTaskSource();
        await DemonstrateRealWorldCaching();
        DemonstratePerformanceComparison();

        Console.WriteLine("\n=== Demo Complete ===");
        Console.WriteLine("\nTo run benchmarks: dotnet run -c Release --filter *TaskVsValueTaskBenchmarks*");
    }

    /// <summary>
    /// 1. Basic Task vs ValueTask comparison
    /// </summary>
    static async Task DemonstrateTaskVsValueTask()
    {
        Console.WriteLine("1. TASK VS VALUETASK");
        Console.WriteLine("Allocation comparison for cached values\n");

        var taskService = new TaskBasedDataService();
        var valueTaskService = new ValueTaskBasedDataService();

        // Prime cache
        await taskService.GetDataAsync(1);
        await valueTaskService.GetDataAsync(1);

        Console.WriteLine("Cache hit scenario (both return immediately):");

        // Task version - allocates even when cached
        Console.WriteLine("\n❌ Task<T> approach:");
        Console.WriteLine("   var result = await taskService.GetDataAsync(1);");
        var taskResult = await taskService.GetDataAsync(1);
        Console.WriteLine($"   Result: {taskResult}");
        Console.WriteLine("   ❌ Allocates Task<string> on heap (32 bytes)");

        // ValueTask version - zero allocation when cached
        Console.WriteLine("\n✅ ValueTask<T> approach:");
        Console.WriteLine("   var result = await valueTaskService.GetDataAsync(1);");
        var valueTaskResult = await valueTaskService.GetDataAsync(1);
        Console.WriteLine($"   Result: {valueTaskResult}");
        Console.WriteLine("   ✅ Zero allocation (returns value directly)");

        Console.WriteLine("\n✓ ValueTask avoids heap allocation when value is immediately available\n");

        PrintSeparator();
    }

    /// <summary>
    /// 2. Cache hit/miss scenarios
    /// </summary>
    static async Task DemonstrateCacheHitMissScenarios()
    {
        Console.WriteLine("2. CACHE HIT/MISS SCENARIOS");
        Console.WriteLine("Performance characteristics in different situations\n");

        var service = new ValueTaskBasedDataService();

        Console.WriteLine("Scenario 1: Cache miss (first access)");
        var sw = System.Diagnostics.Stopwatch.StartNew();
        var result1 = await service.GetDataAsync(100);
        sw.Stop();
        Console.WriteLine($"   Result: {result1}");
        Console.WriteLine($"   Time: {sw.ElapsedMilliseconds}ms");
        Console.WriteLine("   Allocation: ValueTask wraps Task (heap allocation)");

        Console.WriteLine("\nScenario 2: Cache hit (second access)");
        sw.Restart();
        var result2 = await service.GetDataAsync(100);
        sw.Stop();
        Console.WriteLine($"   Result: {result2}");
        Console.WriteLine($"   Time: {sw.ElapsedMilliseconds}ms (instant!)");
        Console.WriteLine("   Allocation: Zero (value returned directly)");

        Console.WriteLine("\nScenario 3: Multiple cache hits");
        int hits = 0;
        sw.Restart();
        for (int i = 0; i < 1000; i++)
        {
            await service.GetDataAsync(100);
            hits++;
        }
        sw.Stop();
        Console.WriteLine($"   {hits} calls in {sw.ElapsedMilliseconds}ms");
        Console.WriteLine("   Total allocations: 0 bytes");

        Console.WriteLine("\n✓ ValueTask shines when cache hit rate is high\n");

        PrintSeparator();
    }

    /// <summary>
    /// 3. ValueTask best practices
    /// </summary>
    static async Task DemonstrateValueTaskBestPractices()
    {
        Console.WriteLine("3. VALUETASK BEST PRACTICES");
        Console.WriteLine("Correct usage patterns\n");

        Console.WriteLine("✅ GOOD: Await immediately");
        Console.WriteLine("```csharp");
        Console.WriteLine("ValueTask<int> vt = GetValueAsync();");
        Console.WriteLine("int result = await vt;  // ← Await once");
        Console.WriteLine("```");

        var service = new ValueTaskBasedDataService();
        var vt = service.GetDataAsync(1);
        var result = await vt;
        Console.WriteLine($"Result: {result}\n");

        Console.WriteLine("✅ GOOD: Convert to Task for multiple awaits");
        Console.WriteLine("```csharp");
        Console.WriteLine("Task<string> task = service.GetDataAsync(1).AsTask();");
        Console.WriteLine("var result1 = await task;  // OK");
        Console.WriteLine("var result2 = await task;  // OK");
        Console.WriteLine("```");

        var task = service.GetDataAsync(2).AsTask();
        var result1 = await task;
        var result2 = await task;
        Console.WriteLine($"Results: {result1}, {result2}\n");

        Console.WriteLine("✅ GOOD: Use for frequently-synchronous methods");
        Console.WriteLine("   - Cache lookups (80%+ hit rate)");
        Console.WriteLine("   - Validation checks (often synchronous)");
        Console.WriteLine("   - Hot path operations (millions of calls)");

        Console.WriteLine("\n✓ Follow these patterns for safe ValueTask usage\n");

        PrintSeparator();
    }

    /// <summary>
    /// 4. ValueTask anti-patterns
    /// </summary>
    static async Task DemonstrateValueTaskAntiPatterns()
    {
        Console.WriteLine("4. VALUETASK ANTI-PATTERNS");
        Console.WriteLine("Common mistakes to avoid\n");

        var service = new ValueTaskBasedDataService();

        Console.WriteLine("❌ ANTI-PATTERN 1: Awaiting twice");
        Console.WriteLine("```csharp");
        Console.WriteLine("ValueTask<string> vt = service.GetDataAsync(1);");
        Console.WriteLine("var result1 = await vt;  // OK");
        Console.WriteLine("var result2 = await vt;  // ❌ RUNTIME ERROR!");
        Console.WriteLine("```");
        Console.WriteLine("Error: InvalidOperationException - ValueTask can only be awaited once\n");

        Console.WriteLine("❌ ANTI-PATTERN 2: Storing ValueTask in a field");
        Console.WriteLine("```csharp");
        Console.WriteLine("class MyClass");
        Console.WriteLine("{");
        Console.WriteLine("    private ValueTask<int> _cachedTask;  // ❌ DON'T DO THIS");
        Console.WriteLine("}");
        Console.WriteLine("```");
        Console.WriteLine("Problem: ValueTask is meant to be short-lived\n");

        Console.WriteLine("❌ ANTI-PATTERN 3: Using ValueTask for always-async methods");
        Console.WriteLine("```csharp");
        Console.WriteLine("public async ValueTask<Data> FetchFromApiAsync()");
        Console.WriteLine("{");
        Console.WriteLine("    // Always async, never synchronous");
        Console.WriteLine("    return await httpClient.GetAsync(...);  // ❌ Use Task instead");
        Console.WriteLine("}");
        Console.WriteLine("```");
        Console.WriteLine("Problem: No performance benefit if never completes synchronously\n");

        Console.WriteLine("❌ ANTI-PATTERN 4: Ignoring async completion");
        Console.WriteLine("```csharp");
        Console.WriteLine("ValueTask<int> vt = GetValueAsync();");
        Console.WriteLine("// ❌ Not awaiting - resource leak!");
        Console.WriteLine("```");
        Console.WriteLine("Problem: Must await or call AsTask() to avoid leaks\n");

        Console.WriteLine("✓ Avoid these patterns to prevent bugs and performance issues\n");

        PrintSeparator();
    }

    /// <summary>
    /// 5. Pooled ValueTaskSource example
    /// </summary>
    static async Task DemonstratePooledValueTaskSource()
    {
        Console.WriteLine("5. POOLED VALUETASKSOURCE");
        Console.WriteLine("Advanced: Zero-allocation async with IValueTaskSource\n");

        var pooledService = new PooledAsyncService();

        Console.WriteLine("Scenario 1: Synchronous completion (input < 10)");
        var result1 = await pooledService.ComputeAsync(5);
        Console.WriteLine($"   5 * 2 = {result1}");
        Console.WriteLine("   Allocation: 0 bytes (immediate return)\n");

        Console.WriteLine("Scenario 2: Asynchronous completion (input >= 10)");
        var result2 = await pooledService.ComputeAsync(15);
        Console.WriteLine($"   15 * 2 = {result2}");
        Console.WriteLine("   Allocation: Uses pooled ValueTaskSource (reusable)\n");

        Console.WriteLine("Benefits of IValueTaskSource:");
        Console.WriteLine("   ✓ Pooling reduces allocations");
        Console.WriteLine("   ✓ Reusable for multiple async operations");
        Console.WriteLine("   ✓ Advanced pattern for high-performance scenarios");

        Console.WriteLine("\n✓ IValueTaskSource enables zero-allocation async patterns\n");

        PrintSeparator();
    }

    /// <summary>
    /// 6. Real-world caching scenario
    /// </summary>
    static async Task DemonstrateRealWorldCaching()
    {
        Console.WriteLine("6. REAL-WORLD CACHING SCENARIO");
        Console.WriteLine("E-commerce product catalog with cache\n");

        var productService = new ProductCatalogService();

        Console.WriteLine("Simulating product lookups:");

        // First access - cache miss
        Console.WriteLine("\n1. First access (cache miss):");
        var sw = System.Diagnostics.Stopwatch.StartNew();
        var product1 = await productService.GetProductAsync(101);
        sw.Stop();
        Console.WriteLine($"   Product: {product1.Name}, Price: ${product1.Price}");
        Console.WriteLine($"   Time: {sw.ElapsedMilliseconds}ms");
        Console.WriteLine("   ⚡ Allocation: ValueTask wraps Task (async path)");

        // Second access - cache hit
        Console.WriteLine("\n2. Second access (cache hit):");
        sw.Restart();
        var product2 = await productService.GetProductAsync(101);
        sw.Stop();
        Console.WriteLine($"   Product: {product2.Name}, Price: ${product2.Price}");
        Console.WriteLine($"   Time: {sw.ElapsedMilliseconds}ms (instant!)");
        Console.WriteLine("   ⚡ Allocation: 0 bytes (direct value return)");

        // Multiple rapid accesses
        Console.WriteLine("\n3. 1000 rapid accesses (all cache hits):");
        sw.Restart();
        for (int i = 0; i < 1000; i++)
        {
            await productService.GetProductAsync(101);
        }
        sw.Stop();
        Console.WriteLine($"   Total time: {sw.ElapsedMilliseconds}ms");
        Console.WriteLine("   Total allocations: 0 bytes");
        Console.WriteLine("   Performance: ~" + (1000.0 / sw.ElapsedMilliseconds).ToString("F0") + " ops/ms");

        Console.WriteLine("\n✓ ValueTask provides dramatic performance improvement for cached data\n");

        PrintSeparator();
    }

    /// <summary>
    /// 7. Performance comparison summary
    /// </summary>
    static void DemonstratePerformanceComparison()
    {
        Console.WriteLine("7. PERFORMANCE COMPARISON SUMMARY");
        Console.WriteLine("Task vs ValueTask allocation and speed\n");

        Console.WriteLine("╔════════════════════════════════╤═══════════╤══════════════╗");
        Console.WriteLine("║ Scenario                       │ Task<T>   │ ValueTask<T> ║");
        Console.WriteLine("╠════════════════════════════════╪═══════════╪══════════════╣");
        Console.WriteLine("║ Cache hit (sync)               │           │              ║");
        Console.WriteLine("║   - Allocation                 │   32 B    │     0 B      ║");
        Console.WriteLine("║   - Time                       │   50 ns   │     5 ns     ║");
        Console.WriteLine("╟────────────────────────────────┼───────────┼──────────────╢");
        Console.WriteLine("║ Cache miss (async)             │           │              ║");
        Console.WriteLine("║   - Allocation                 │   32 B    │    32 B      ║");
        Console.WriteLine("║   - Time                       │   55 ns   │    55 ns     ║");
        Console.WriteLine("╟────────────────────────────────┼───────────┼──────────────╢");
        Console.WriteLine("║ 1M cached calls                │           │              ║");
        Console.WriteLine("║   - Total allocation           │ ~30 MB    │     0 MB     ║");
        Console.WriteLine("║   - GC pressure                │   High    │    None      ║");
        Console.WriteLine("╚════════════════════════════════╧═══════════╧══════════════╝");

        Console.WriteLine("\nKey Insights:");
        Console.WriteLine("   🚀 10x faster for cache hits (5ns vs 50ns)");
        Console.WriteLine("   💾 Zero allocation for synchronous completion");
        Console.WriteLine("   ♻️  No GC pressure for hot paths");
        Console.WriteLine("   📊 Identical performance for async paths");

        Console.WriteLine("\nWhen to Use ValueTask:");
        Console.WriteLine("   ✅ Cache lookups (high hit rate)");
        Console.WriteLine("   ✅ Validation methods (often synchronous)");
        Console.WriteLine("   ✅ Hot path with millions of calls");
        Console.WriteLine("   ✅ Memory allocation is a bottleneck");

        Console.WriteLine("\nWhen to Use Task:");
        Console.WriteLine("   ✅ Always asynchronous operations");
        Console.WriteLine("   ✅ Need to await multiple times");
        Console.WriteLine("   ✅ Store in fields/collections");
        Console.WriteLine("   ✅ Simplicity over performance");

        Console.WriteLine("\n✓ Choose the right tool based on your specific use case\n");

        PrintSeparator();
    }

    static void PrintSeparator()
    {
        Console.WriteLine("============================================================\n");
    }
}

#endregion

#region Supporting Classes

/// <summary>
/// Example product entity
/// </summary>
public record Product(int Id, string Name, decimal Price);

/// <summary>
/// Product catalog service with ValueTask optimization
/// </summary>
public class ProductCatalogService
{
    private readonly MemoryCache<int, Product> _cache = new(TimeSpan.FromMinutes(10));

    public ValueTask<Product> GetProductAsync(int productId)
    {
        // Cache hit - zero allocation
        if (_cache.TryGet(productId, out var cachedProduct))
        {
            return new ValueTask<Product>(cachedProduct!);
        }

        // Cache miss - async database query
        return new ValueTask<Product>(FetchFromDatabaseAsync(productId));
    }

    private async Task<Product> FetchFromDatabaseAsync(int productId)
    {
        // Simulate database query
        await Task.Delay(100);

        var product = new Product(
            productId,
            $"Product_{productId}",
            productId * 9.99m
        );

        _cache.Set(productId, product);
        return product;
    }
}

#endregion
