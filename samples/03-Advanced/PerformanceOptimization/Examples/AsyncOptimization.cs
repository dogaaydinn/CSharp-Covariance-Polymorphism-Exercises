using System;
using System.Threading.Tasks;

namespace PerformanceOptimization.Examples;

/// <summary>
/// Demonstrates async/await optimization techniques
/// </summary>
public static class AsyncOptimization
{
    /// <summary>
    /// Example 1: ValueTask vs Task - when to use each
    /// </summary>
    public static async Task ValueTaskVsTask()
    {
        Console.WriteLine("\n=== ValueTask vs Task ===");
        
        // Task - always allocates on heap
        Task<int> task1 = GetValueWithTask();
        int result1 = await task1;
        Console.WriteLine($"Task result: {result1} (heap allocated)");
        
        // ValueTask - can avoid allocation if result is synchronous
        ValueTask<int> valueTask1 = GetValueWithValueTask(fromCache: true);
        int result2 = await valueTask1;
        Console.WriteLine($"ValueTask (cached): {result2} (no allocation)");
        
        ValueTask<int> valueTask2 = GetValueWithValueTask(fromCache: false);
        int result3 = await valueTask2;
        Console.WriteLine($"ValueTask (async): {result3} (allocated)");
        
        Console.WriteLine("\nWhen to use:");
        Console.WriteLine("  Task: Default choice, most scenarios");
        Console.WriteLine("  ValueTask: High-frequency calls with often-sync results");
        Console.WriteLine("  ValueTask: Cached/pooled results");
    }

    private static Task<int> GetValueWithTask()
    {
        return Task.FromResult(42);  // Still allocates Task object
    }

    private static ValueTask<int> GetValueWithValueTask(bool fromCache)
    {
        if (fromCache)
        {
            // Synchronous path - no allocation
            return new ValueTask<int>(42);
        }
        else
        {
            // Async path - allocates
            return new ValueTask<int>(Task.Delay(1).ContinueWith(_ => 42));
        }
    }

    /// <summary>
    /// Example 2: ConfigureAwait(false) - avoid context capture
    /// </summary>
    public static async Task ConfigureAwaitExample()
    {
        Console.WriteLine("\n=== ConfigureAwait(false) ===");
        
        Console.WriteLine("Without ConfigureAwait:");
        await OperationWithContextCapture();
        
        Console.WriteLine("\nWith ConfigureAwait(false):");
        await OperationWithoutContextCapture();
        
        Console.WriteLine("\nUse ConfigureAwait(false) when:");
        Console.WriteLine("  ✓ Library code (don't need UI/ASP.NET context)");
        Console.WriteLine("  ✓ Performance-critical paths");
        Console.WriteLine("  ✗ UI code (need to update UI)");
        Console.WriteLine("  ✗ ASP.NET (usually not needed in .NET Core+)");
    }

    private static async Task OperationWithContextCapture()
    {
        await Task.Delay(1);  // Captures synchronization context
        Console.WriteLine("  Resumed on original context (slower)");
    }

    private static async Task OperationWithoutContextCapture()
    {
        await Task.Delay(1).ConfigureAwait(false);  // No context capture
        Console.WriteLine("  Resumed on thread pool (faster)");
    }

    /// <summary>
    /// Example 3: Task.WhenAll for parallel operations
    /// </summary>
    public static async Task ParallelOperations()
    {
        Console.WriteLine("\n=== Parallel Async Operations ===");
        
        // ❌ Bad: Sequential execution
        Console.WriteLine("❌ Sequential:");
        var sw = System.Diagnostics.Stopwatch.StartNew();
        int result1 = await SlowOperation(1, 100);
        int result2 = await SlowOperation(2, 100);
        int result3 = await SlowOperation(3, 100);
        Console.WriteLine($"  Total time: {sw.ElapsedMilliseconds}ms (3 * 100ms = ~300ms)");
        
        // ✅ Good: Parallel execution
        Console.WriteLine("\n✅ Parallel:");
        sw.Restart();
        var task1 = SlowOperation(1, 100);
        var task2 = SlowOperation(2, 100);
        var task3 = SlowOperation(3, 100);
        int[] results = await Task.WhenAll(task1, task2, task3);
        Console.WriteLine($"  Total time: {sw.ElapsedMilliseconds}ms (~100ms)");
        Console.WriteLine($"  Results: [{string.Join(", ", results)}]");
    }

    private static async Task<int> SlowOperation(int id, int delayMs)
    {
        await Task.Delay(delayMs);
        return id * 10;
    }

    /// <summary>
    /// Example 4: Async void - the danger
    /// </summary>
    public static async Task AsyncVoidProblem()
    {
        Console.WriteLine("\n=== Async Void (Dangerous!) ===");
        
        Console.WriteLine("❌ Async void - cannot await, exceptions crash app:");
        Console.WriteLine("  async void BadMethod() { ... }");
        Console.WriteLine("  BadMethod();  // Fire and forget, no way to catch errors");
        
        Console.WriteLine("\n✅ Async Task - can await, exceptions catchable:");
        try
        {
            await GoodAsyncMethod();
            Console.WriteLine("  Completed successfully");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"  Caught exception: {ex.Message}");
        }
        
        Console.WriteLine("\nRule: NEVER use async void except for event handlers");
    }

    private static async Task GoodAsyncMethod()
    {
        await Task.Delay(1);
        // Exceptions can be caught by caller
    }

    /// <summary>
    /// Example 5: Avoid async/await for purely synchronous code
    /// </summary>
    public static async Task AvoidUnnecessaryAsync()
    {
        Console.WriteLine("\n=== Avoid Unnecessary Async ===");
        
        // ❌ Bad: Async for synchronous work
        int result1 = await UnnecessaryAsyncMethod();
        Console.WriteLine($"❌ Unnecessary async: {result1} (state machine overhead)");
        
        // ✅ Good: Synchronous method
        int result2 = SynchronousMethod();
        Console.WriteLine($"✅ Synchronous: {result2} (no overhead)");
        
        Console.WriteLine("\nRule: Don't make methods async if they do synchronous work only");
    }

    private static async Task<int> UnnecessaryAsyncMethod()
    {
        // No actual async work - wasteful
        return 42;
    }

    private static int SynchronousMethod()
    {
        return 42;
    }

    /// <summary>
    /// Example 6: Task.Yield for responsive UI
    /// </summary>
    public static async Task TaskYieldExample()
    {
        Console.WriteLine("\n=== Task.Yield() for Responsiveness ===");
        
        Console.WriteLine("Heavy computation with Task.Yield:");
        int total = 0;
        for (int i = 0; i < 10; i++)
        {
            // Simulate heavy work
            for (int j = 0; j < 1000; j++)
                total += j;
            
            // Yield to allow other work
            if (i % 3 == 0)
            {
                await Task.Yield();
                Console.WriteLine($"  Yielded after iteration {i} (allows UI to update)");
            }
        }
        
        Console.WriteLine("\nUse Task.Yield() to:");
        Console.WriteLine("  ✓ Keep UI responsive during long operations");
        Console.WriteLine("  ✓ Allow other async work to proceed");
        Console.WriteLine("  ✗ Don't overuse - has overhead");
    }

    /// <summary>
    /// Example 7: Async lazy initialization
    /// </summary>
    public static async Task AsyncLazyExample()
    {
        Console.WriteLine("\n=== Async Lazy Initialization ===");
        
        var lazy = new AsyncLazy<string>(async () =>
        {
            Console.WriteLine("  Initializing expensive resource...");
            await Task.Delay(100);
            return "Expensive Resource";
        });
        
        Console.WriteLine("Lazy created (not initialized yet)");
        
        // First access - initializes
        string value1 = await lazy.GetValueAsync();
        Console.WriteLine($"First access: {value1}");
        
        // Second access - uses cached value
        string value2 = await lazy.GetValueAsync();
        Console.WriteLine($"Second access: {value2} (cached, no initialization)");
    }

    /// <summary>
    /// Example 8: Avoiding async over sync
    /// </summary>
    public static async Task AvoidAsyncOverSync()
    {
        Console.WriteLine("\n=== Avoid Async-over-Sync (Deadlock Risk) ===");
        
        Console.WriteLine("❌ Bad pattern (can deadlock):");
        Console.WriteLine("  var result = AsyncMethod().Result;  // Blocking on async");
        Console.WriteLine("  var result = AsyncMethod().GetAwaiter().GetResult();");
        
        Console.WriteLine("\n✅ Good patterns:");
        Console.WriteLine("  var result = await AsyncMethod();  // Proper async");
        Console.WriteLine("  var result = SyncMethod();  // Proper sync");
        
        await Task.CompletedTask;
    }

    /// <summary>
    /// Example 9: Cancellation tokens
    /// </summary>
    public static async Task CancellationExample()
    {
        Console.WriteLine("\n=== Cancellation Tokens ===");
        
        using var cts = new System.Threading.CancellationTokenSource();
        cts.CancelAfter(TimeSpan.FromMilliseconds(50));
        
        try
        {
            await LongRunningOperation(cts.Token);
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine("  Operation was cancelled");
        }
        
        Console.WriteLine("\nAlways accept CancellationToken in async methods:");
        Console.WriteLine("  async Task MyMethod(CancellationToken ct = default)");
    }

    private static async Task LongRunningOperation(System.Threading.CancellationToken ct)
    {
        for (int i = 0; i < 10; i++)
        {
            ct.ThrowIfCancellationRequested();
            await Task.Delay(20, ct);
            Console.WriteLine($"  Iteration {i}");
        }
    }

    /// <summary>
    /// Example 10: Real-world pattern - async repository
    /// </summary>
    public static async Task AsyncRepositoryPattern()
    {
        Console.WriteLine("\n=== Real-World: Async Repository ===");
        
        var repo = new AsyncRepository();
        
        // Parallel queries
        var getUserTask = repo.GetUserByIdAsync(1);
        var getOrdersTask = repo.GetOrdersByUserIdAsync(1);
        
        await Task.WhenAll(getUserTask, getOrdersTask);
        
        Console.WriteLine($"User: {getUserTask.Result}");
        Console.WriteLine($"Orders: {getOrdersTask.Result}");
        Console.WriteLine("\n✓ Both queries executed in parallel");
    }

    private class AsyncRepository
    {
        public async Task<string> GetUserByIdAsync(int id)
        {
            await Task.Delay(50);  // Simulate DB query
            return $"User {id}";
        }

        public async Task<string> GetOrdersByUserIdAsync(int userId)
        {
            await Task.Delay(50);  // Simulate DB query
            return $"Orders for User {userId}";
        }
    }

    // Helper class for async lazy initialization
    private class AsyncLazy<T>
    {
        private readonly Lazy<Task<T>> _lazy;

        public AsyncLazy(Func<Task<T>> factory)
        {
            _lazy = new Lazy<Task<T>>(() => factory());
        }

        public Task<T> GetValueAsync() => _lazy.Value;
    }
}
