using BenchmarkDotNet.Running;

namespace AdvancedCsharpConcepts.Advanced.PerformanceBenchmarks;

/// <summary>
/// Benchmark runner for performance analysis.
/// Silicon Valley best practice: always measure before optimizing.
///
/// Usage: Run with --benchmark flag to execute all benchmarks.
/// Example: dotnet run --configuration Release --benchmark
/// </summary>
public static class BenchmarkRunner
{
    /// <summary>
    /// Runs all benchmarks and generates detailed performance reports.
    /// </summary>
    public static void RunAllBenchmarks()
    {
        Console.WriteLine("╔════════════════════════════════════════════════════════════════╗");
        Console.WriteLine("║         High-Performance C# Benchmarks                        ║");
        Console.WriteLine("║         NVIDIA Developer × Silicon Valley Best Practices      ║");
        Console.WriteLine("╚════════════════════════════════════════════════════════════════╝\n");

        Console.WriteLine("Running benchmarks...");
        Console.WriteLine("This may take several minutes. Results will be saved to BenchmarkDotNet.Artifacts/\n");

        // Run Boxing/Unboxing benchmark
        Console.WriteLine("1. Boxing/Unboxing Performance Analysis...");
        BenchmarkDotNet.Running.BenchmarkRunner.Run<BoxingUnboxingBenchmark>();

        // Run Covariance benchmark
        Console.WriteLine("\n2. Covariance/Contravariance Performance Analysis...");
        BenchmarkDotNet.Running.BenchmarkRunner.Run<CovarianceBenchmark>();

        Console.WriteLine("\n✅ All benchmarks completed!");
        Console.WriteLine("📊 Check BenchmarkDotNet.Artifacts/results for detailed reports.");
    }

    /// <summary>
    /// Displays a quick performance summary without full benchmarks.
    /// </summary>
    public static void ShowQuickSummary()
    {
        Console.WriteLine("\n╔════════════════════════════════════════════════════════════════╗");
        Console.WriteLine("║         Performance Quick Summary                             ║");
        Console.WriteLine("╚════════════════════════════════════════════════════════════════╝\n");

        Console.WriteLine("Key Insights:");
        Console.WriteLine();
        Console.WriteLine("📦 Boxing/Unboxing:");
        Console.WriteLine("   • Generic List<T>:     ~10x faster than ArrayList");
        Console.WriteLine("   • Span<T>:             ~1.2x faster than List<T>");
        Console.WriteLine("   • Memory Impact:       ArrayList creates heap objects for each value type");
        Console.WriteLine();
        Console.WriteLine("🔄 Covariance:");
        Console.WriteLine("   • Direct Iteration:    Baseline (fastest)");
        Console.WriteLine("   • IEnumerable<T>:      ~1.1x overhead (interface virtualization)");
        Console.WriteLine("   • Array Covariance:    ~1.3x overhead (runtime type checking)");
        Console.WriteLine("   • Span<T>:             ~1.2x faster than direct (zero allocations)");
        Console.WriteLine("   • Parallel.ForEach:    ~4-8x faster for CPU-bound work (multi-core)");
        Console.WriteLine();
        Console.WriteLine("💡 Recommendations:");
        Console.WriteLine("   1. Always use List<T> over ArrayList for value types");
        Console.WriteLine("   2. Use Span<T> for hot paths with temporary data");
        Console.WriteLine("   3. Leverage Parallel.ForEach for CPU-intensive operations");
        Console.WriteLine("   4. Avoid array covariance for write operations (runtime exceptions)");
        Console.WriteLine("   5. Profile before optimizing - measure with BenchmarkDotNet");
        Console.WriteLine();
        Console.WriteLine("🚀 To run full benchmarks: dotnet run --configuration Release --benchmark");
        Console.WriteLine();
    }
}
