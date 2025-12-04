using System;
using PerformanceOptimization.Examples;

namespace PerformanceOptimization;

/// <summary>
/// Performance Optimization Tutorial - Advanced C# Techniques
/// Demonstrates high-performance patterns and anti-patterns
/// </summary>
class Program
{
    static async System.Threading.Tasks.Task Main(string[] args)
    {
        Console.WriteLine("═══════════════════════════════════════════════════════════════════");
        Console.WriteLine("PERFORMANCE OPTIMIZATION TUTORIAL");
        Console.WriteLine("Advanced C# Techniques for High-Performance Code");
        Console.WriteLine("═══════════════════════════════════════════════════════════════════");

        while (true)
        {
            Console.WriteLine("\n\n╔═══════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║                     MAIN MENU                                  ║");
            Console.WriteLine("╠═══════════════════════════════════════════════════════════════╣");
            Console.WriteLine("║  1. Span<T> vs Array                  (Zero-allocation)       ║");
            Console.WriteLine("║  2. String Optimization               (StringBuilder+)        ║");
            Console.WriteLine("║  3. LINQ Optimization                 (Pitfalls & Fixes)      ║");
            Console.WriteLine("║  4. Async Optimization                (ValueTask+)            ║");
            Console.WriteLine("║  5. All Examples                      (Run everything)        ║");
            Console.WriteLine("║  0. Exit                                                       ║");
            Console.WriteLine("╚═══════════════════════════════════════════════════════════════╝");
            Console.Write("\nSelect an option: ");

            string? choice = Console.ReadLine();

            try
            {
                switch (choice)
                {
                    case "1":
                        RunSpanExamples();
                        break;
                    case "2":
                        RunStringExamples();
                        break;
                    case "3":
                        RunLinqExamples();
                        break;
                    case "4":
                        await RunAsyncExamples();
                        break;
                    case "5":
                        await RunAllExamples();
                        break;
                    case "0":
                        Console.WriteLine("\nThank you for using the Performance Optimization Tutorial!");
                        return;
                    default:
                        Console.WriteLine("\n❌ Invalid option. Please try again.");
                        break;
                }

                Console.WriteLine("\n\nPress any key to continue...");
                Console.ReadKey();
                Console.Clear();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n❌ Error: {ex.Message}");
                Console.WriteLine("\nPress any key to continue...");
                Console.ReadKey();
                Console.Clear();
            }
        }
    }

    static void RunSpanExamples()
    {
        Console.WriteLine("\n\n╔═══════════════════════════════════════════════════════════════╗");
        Console.WriteLine("║              SPAN<T> VS ARRAY EXAMPLES                         ║");
        Console.WriteLine("╚═══════════════════════════════════════════════════════════════╝");

        SpanVsArray.BasicSpanUsage();
        SpanVsArray.SlicingWithoutCopying();
        SpanVsArray.StringSpan();
        SpanVsArray.StackallocWithSpan();
        SpanVsArray.MemoryVsSpan();
        SpanVsArray.ArrayPoolExample();
        SpanVsArray.CsvParsingExample();
        SpanVsArray.SpanLimitations();

        Console.WriteLine("\n\n══════════════════════════════════════════════════════════════");
        Console.WriteLine("KEY TAKEAWAYS:");
        Console.WriteLine("══════════════════════════════════════════════════════════════");
        Console.WriteLine("✓ Span<T>: Zero-allocation slicing and views");
        Console.WriteLine("✓ stackalloc: Stack allocation for small buffers");
        Console.WriteLine("✓ ArrayPool: Reusable buffers for reduced GC pressure");
        Console.WriteLine("✓ Memory<T>: Async-friendly alternative to Span<T>");
        Console.WriteLine("✓ ReadOnlySpan<char>: Parse strings without allocations");
        Console.WriteLine("══════════════════════════════════════════════════════════════");
    }

    static void RunStringExamples()
    {
        Console.WriteLine("\n\n╔═══════════════════════════════════════════════════════════════╗");
        Console.WriteLine("║              STRING OPTIMIZATION EXAMPLES                      ║");
        Console.WriteLine("╚═══════════════════════════════════════════════════════════════╝");

        StringOptimization.StringBuilderVsConcatenation();
        StringOptimization.StringInterning();
        StringOptimization.StringCreate();
        StringOptimization.StringAsSpan();
        StringOptimization.StringPooling();
        StringOptimization.StringFormatting();
        StringOptimization.StringComparisonOptimization();
        StringOptimization.BuildJsonEfficiently();
        StringOptimization.CommonPitfalls();

        Console.WriteLine("\n\n══════════════════════════════════════════════════════════════");
        Console.WriteLine("KEY TAKEAWAYS:");
        Console.WriteLine("══════════════════════════════════════════════════════════════");
        Console.WriteLine("✓ StringBuilder: Use for >5 concatenations or loops");
        Console.WriteLine("✓ string.AsSpan(): Zero-allocation string slicing");
        Console.WriteLine("✓ StringComparison: Always specify comparison type");
        Console.WriteLine("✓ String.Create: Build strings without intermediates");
        Console.WriteLine("✓ Avoid: ToLower() for comparisons, + in loops");
        Console.WriteLine("══════════════════════════════════════════════════════════════");
    }

    static void RunLinqExamples()
    {
        Console.WriteLine("\n\n╔═══════════════════════════════════════════════════════════════╗");
        Console.WriteLine("║              LINQ OPTIMIZATION EXAMPLES                        ║");
        Console.WriteLine("╚═══════════════════════════════════════════════════════════════╝");

        LinqOptimization.LinqVsForLoop();
        LinqOptimization.MultipleEnumeration();
        LinqOptimization.AnyVsCount();
        LinqOptimization.FirstVsSingle();
        LinqOptimization.QueryOrdering();
        LinqOptimization.ToListVsToArray();
        LinqOptimization.AvoidLinqInLoops();
        LinqOptimization.DeferredExecution();
        LinqOptimization.HashSetForContains();
        LinqOptimization.RealWorldOptimization();

        Console.WriteLine("\n\n══════════════════════════════════════════════════════════════");
        Console.WriteLine("KEY TAKEAWAYS:");
        Console.WriteLine("══════════════════════════════════════════════════════════════");
        Console.WriteLine("✓ Any() vs Count(): Use Any() for existence checks");
        Console.WriteLine("✓ FirstOrDefault(): Faster than SingleOrDefault()");
        Console.WriteLine("✓ Where before Select: Filter early, transform late");
        Console.WriteLine("✓ .ToList(): Materialize if using multiple times");
        Console.WriteLine("✓ HashSet: O(1) lookups vs List's O(n)");
        Console.WriteLine("✓ Avoid: LINQ in loops, multiple enumerations");
        Console.WriteLine("══════════════════════════════════════════════════════════════");
    }

    static async System.Threading.Tasks.Task RunAsyncExamples()
    {
        Console.WriteLine("\n\n╔═══════════════════════════════════════════════════════════════╗");
        Console.WriteLine("║              ASYNC OPTIMIZATION EXAMPLES                       ║");
        Console.WriteLine("╚═══════════════════════════════════════════════════════════════╝");

        await AsyncOptimization.ValueTaskVsTask();
        await AsyncOptimization.ConfigureAwaitExample();
        await AsyncOptimization.ParallelOperations();
        await AsyncOptimization.AsyncVoidProblem();
        await AsyncOptimization.AvoidUnnecessaryAsync();
        await AsyncOptimization.TaskYieldExample();
        await AsyncOptimization.AsyncLazyExample();
        await AsyncOptimization.AvoidAsyncOverSync();
        await AsyncOptimization.CancellationExample();
        await AsyncOptimization.AsyncRepositoryPattern();

        Console.WriteLine("\n\n══════════════════════════════════════════════════════════════");
        Console.WriteLine("KEY TAKEAWAYS:");
        Console.WriteLine("══════════════════════════════════════════════════════════════");
        Console.WriteLine("✓ ValueTask: Use for high-frequency, often-sync calls");
        Console.WriteLine("✓ ConfigureAwait(false): Use in library code");
        Console.WriteLine("✓ Task.WhenAll: Run multiple async operations in parallel");
        Console.WriteLine("✓ Never: Use async void (except event handlers)");
        Console.WriteLine("✓ CancellationToken: Always support cancellation");
        Console.WriteLine("✓ Avoid: .Result, .Wait() (causes deadlocks)");
        Console.WriteLine("══════════════════════════════════════════════════════════════");
    }

    static async System.Threading.Tasks.Task RunAllExamples()
    {
        Console.WriteLine("\n\n╔═══════════════════════════════════════════════════════════════╗");
        Console.WriteLine("║              RUNNING ALL EXAMPLES                              ║");
        Console.WriteLine("╚═══════════════════════════════════════════════════════════════╝");

        RunSpanExamples();
        Console.WriteLine("\n\n[Press any key to continue to String examples...]");
        Console.ReadKey();

        RunStringExamples();
        Console.WriteLine("\n\n[Press any key to continue to LINQ examples...]");
        Console.ReadKey();

        RunLinqExamples();
        Console.WriteLine("\n\n[Press any key to continue to Async examples...]");
        Console.ReadKey();

        await RunAsyncExamples();

        Console.WriteLine("\n\n╔═══════════════════════════════════════════════════════════════╗");
        Console.WriteLine("║              ALL EXAMPLES COMPLETED!                           ║");
        Console.WriteLine("╚═══════════════════════════════════════════════════════════════╝");

        Console.WriteLine("\n\n══════════════════════════════════════════════════════════════");
        Console.WriteLine("PERFORMANCE OPTIMIZATION - SUMMARY");
        Console.WriteLine("══════════════════════════════════════════════════════════════");
        Console.WriteLine("\n1. MEMORY OPTIMIZATION:");
        Console.WriteLine("   • Use Span<T> for zero-allocation operations");
        Console.WriteLine("   • Use ArrayPool<T> for buffer reuse");
        Console.WriteLine("   • Use stackalloc for small buffers (<1KB)");
        Console.WriteLine("\n2. STRING OPTIMIZATION:");
        Console.WriteLine("   • StringBuilder for multiple concatenations");
        Console.WriteLine("   • string.AsSpan() for slicing");
        Console.WriteLine("   • StringComparison for culture-aware comparisons");
        Console.WriteLine("\n3. LINQ OPTIMIZATION:");
        Console.WriteLine("   • Use Any() instead of Count() > 0");
        Console.WriteLine("   • Materialize with .ToList() if using multiple times");
        Console.WriteLine("   • HashSet for frequent Contains() checks");
        Console.WriteLine("   • Avoid LINQ in tight loops");
        Console.WriteLine("\n4. ASYNC OPTIMIZATION:");
        Console.WriteLine("   • ValueTask for frequently-called, often-sync methods");
        Console.WriteLine("   • ConfigureAwait(false) in library code");
        Console.WriteLine("   • Task.WhenAll for parallel async operations");
        Console.WriteLine("   • Never use async void (except event handlers)");
        Console.WriteLine("\n5. GENERAL PRINCIPLES:");
        Console.WriteLine("   • Measure first (use BenchmarkDotNet)");
        Console.WriteLine("   • Optimize hot paths only");
        Console.WriteLine("   • Prefer clarity over premature optimization");
        Console.WriteLine("   • Profile to find real bottlenecks");
        Console.WriteLine("══════════════════════════════════════════════════════════════");
    }
}
