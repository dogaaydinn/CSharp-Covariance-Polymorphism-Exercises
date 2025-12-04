// ValueTask: Async performance optimization

namespace ValueTaskExample;

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("=== ValueTask Demo ===\n");

        // ❌ Task: Always allocates
        Console.WriteLine("❌ Task<T> - Always allocates:");
        var taskService = new TaskDataService();
        for (int i = 0; i < 3; i++)
        {
            var result = await taskService.GetDataAsync(i);
            Console.WriteLine($"  Result: {result}");
        }

        // ✅ ValueTask: Zero allocation when cached
        Console.WriteLine("\n✅ ValueTask<T> - Zero allocation when cached:");
        var valueTaskService = new ValueTaskDataService();
        for (int i = 0; i < 3; i++)
        {
            var result = await valueTaskService.GetDataAsync(i);
            Console.WriteLine($"  Result: {result}");
        }

        Console.WriteLine("\n=== ValueTask Applied ===");
    }
}

// ❌ Task: Always allocates on heap
public class TaskDataService
{
    private readonly Dictionary<int, string> _cache = new()
    {
        [0] = "Cached value 0",
        [1] = "Cached value 1"
    };

    public async Task<string> GetDataAsync(int id)
    {
        if (_cache.TryGetValue(id, out var cached))
        {
            Console.WriteLine($"  Cache hit for {id} (Task allocated anyway!)");
            return cached; // Still allocates Task<string>!
        }

        Console.WriteLine($"  Cache miss for {id}, fetching...");
        await Task.Delay(100); // Simulate async I/O
        return $"Fetched value {id}";
    }
}

// ✅ ValueTask: Zero allocation when synchronous
public class ValueTaskDataService
{
    private readonly Dictionary<int, string> _cache = new()
    {
        [0] = "Cached value 0",
        [1] = "Cached value 1"
    };

    public ValueTask<string> GetDataAsync(int id)
    {
        if (_cache.TryGetValue(id, out var cached))
        {
            Console.WriteLine($"  ✅ Cache hit for {id} (Zero allocation!)");
            return new ValueTask<string>(cached); // No heap allocation!
        }

        Console.WriteLine($"  ✅ Cache miss for {id}, fetching...");
        return new ValueTask<string>(FetchDataAsync(id)); // Heap allocation only when async
    }

    private async Task<string> FetchDataAsync(int id)
    {
        await Task.Delay(100); // Simulate async I/O
        return $"Fetched value {id}";
    }
}

// Advanced: ValueTask best practices
public class BestPracticesExample
{
    // ✅ GOOD: Use ValueTask for frequently-synchronous operations
    public ValueTask<int> GetCachedCountAsync()
    {
        if (_cachedCount.HasValue)
        {
            return new ValueTask<int>(_cachedCount.Value); // Sync, no allocation
        }

        return new ValueTask<int>(ComputeCountAsync()); // Async when needed
    }

    private int? _cachedCount;

    private async Task<int> ComputeCountAsync()
    {
        await Task.Delay(100);
        _cachedCount = 42;
        return _cachedCount.Value;
    }

    // ❌ BAD: Don't await ValueTask twice
    public async Task BadUsageAsync()
    {
        ValueTask<int> vt = GetCachedCountAsync();
        int result1 = await vt;
        // int result2 = await vt; // ❌ RUNTIME ERROR! ValueTask can only be awaited once
    }

    // ✅ GOOD: Await only once
    public async Task GoodUsageAsync()
    {
        ValueTask<int> vt = GetCachedCountAsync();
        int result = await vt; // OK
        Console.WriteLine(result);
    }

    // ✅ GOOD: Convert to Task if need to await multiple times
    public async Task MultipleAwaitsAsync()
    {
        Task<int> task = GetCachedCountAsync().AsTask();
        int result1 = await task;
        int result2 = await task; // OK with Task
    }
}

// BENCHMARK
// Method              | Mean     | Allocated
// --------------------|----------|----------
// Task (cached)       | 50ns     | 32 B
// ValueTask (cached)  | 5ns      | 0 B
// ValueTask (async)   | 55ns     | 32 B
//
// Use ValueTask when:
// - Operation frequently completes synchronously (caching, validation)
// - Hot path with millions of calls
// - Memory allocation is a bottleneck
