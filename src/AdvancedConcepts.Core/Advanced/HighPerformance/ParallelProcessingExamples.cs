using System.Collections.Concurrent;
using System.Diagnostics;

namespace AdvancedCsharpConcepts.Advanced.HighPerformance;

/// <summary>
/// High-Performance Parallel Processing Examples.
/// NVIDIA developer mindset: leverage multi-core CPUs for maximum throughput.
/// Best practices from Silicon Valley: measure, optimize, scale.
/// </summary>
public class ParallelProcessingExamples
{
    /// <summary>
    /// Sequential processing baseline for comparison.
    /// </summary>
    public static long SequentialSum(int count)
    {
        long sum = 0;
        for (var i = 0; i < count; i++)
        {
            sum += i;
        }
        return sum;
    }

    /// <summary>
    /// Parallel.For - distributes work across CPU cores.
    /// Expected: ~4-8x speedup on modern CPUs (depending on core count).
    /// </summary>
    public static long ParallelForSum(int count)
    {
        long sum = 0;
        Parallel.For(0, count, () => 0L,
            (i, state, localSum) => localSum + i,
            localSum => Interlocked.Add(ref sum, localSum));
        return sum;
    }

    /// <summary>
    /// PLINQ (Parallel LINQ) - declarative parallelism.
    /// Clean, expressive, and automatically parallelized.
    /// </summary>
    public static long PlinqSum(int count)
    {
        return Enumerable.Range(0, count)
            .AsParallel()
            .Sum(i => (long)i);
    }

    /// <summary>
    /// Advanced: Parallel aggregation with custom partitioning.
    /// NVIDIA-style: optimize data locality and reduce synchronization overhead.
    /// </summary>
    public static long OptimizedParallelSum(int count)
    {
        var partitioner = Partitioner.Create(0, count, count / Environment.ProcessorCount);
        long sum = 0;

        Parallel.ForEach(partitioner, range =>
        {
            long localSum = 0;
            for (var i = range.Item1; i < range.Item2; i++)
            {
                localSum += i;
            }
            Interlocked.Add(ref sum, localSum);
        });

        return sum;
    }

    /// <summary>
    /// Real-world example: Parallel matrix multiplication.
    /// GPU-like parallelism on CPU using multi-threading.
    /// </summary>
    public static double[,] ParallelMatrixMultiply(double[,] a, double[,] b)
    {
        var rowsA = a.GetLength(0);
        var colsA = a.GetLength(1);
        var colsB = b.GetLength(1);

        if (colsA != b.GetLength(0))
            throw new ArgumentException("Matrix dimensions incompatible for multiplication");

        var result = new double[rowsA, colsB];

        // Parallelize outer loop (each row can be computed independently)
        Parallel.For(0, rowsA, i =>
        {
            for (var j = 0; j < colsB; j++)
            {
                double sum = 0;
                for (var k = 0; k < colsA; k++)
                {
                    sum += a[i, k] * b[k, j];
                }
                result[i, j] = sum;
            }
        });

        return result;
    }

    /// <summary>
    /// Advanced: Parallel pipeline with PLINQ.
    /// High-throughput pipeline processing (similar to CUDA streams).
    /// </summary>
    public static List<int> ParallelPipeline(IEnumerable<int> input)
    {
        // Use PLINQ for parallel pipeline processing
        var results = input
            .AsParallel()
            .WithDegreeOfParallelism(Environment.ProcessorCount)
            .Select(x => x * x)          // Transform: square each number
            .Where(x => x % 2 == 0)      // Filter: keep only evens
            .ToList();

        return results;
    }

    /// <summary>
    /// Performance benchmark: Sequential vs Parallel approaches.
    /// </summary>
    public static void BenchmarkParallelism()
    {
        const int count = 100_000_000;
        var sw = Stopwatch.StartNew();

        // Sequential
        sw.Restart();
        var seqResult = SequentialSum(count);
        var seqTime = sw.Elapsed;

        // Parallel.For
        sw.Restart();
        var parResult = ParallelForSum(count);
        var parTime = sw.Elapsed;

        // PLINQ
        sw.Restart();
        var plinqResult = PlinqSum(count);
        var plinqTime = sw.Elapsed;

        // Optimized Parallel
        sw.Restart();
        var optResult = OptimizedParallelSum(count);
        var optTime = sw.Elapsed;

        Console.WriteLine($"Sequential:        {seqTime.TotalMilliseconds,8:F2} ms (Result: {seqResult})");
        Console.WriteLine($"Parallel.For:      {parTime.TotalMilliseconds,8:F2} ms (Result: {parResult}) - {seqTime.TotalMilliseconds / parTime.TotalMilliseconds:F2}x speedup");
        Console.WriteLine($"PLINQ:             {plinqTime.TotalMilliseconds,8:F2} ms (Result: {plinqResult}) - {seqTime.TotalMilliseconds / plinqTime.TotalMilliseconds:F2}x speedup");
        Console.WriteLine($"Optimized Parallel:{optTime.TotalMilliseconds,8:F2} ms (Result: {optResult}) - {seqTime.TotalMilliseconds / optTime.TotalMilliseconds:F2}x speedup");
        Console.WriteLine($"\nCPU Cores: {Environment.ProcessorCount}");
    }

    /// <summary>
    /// Demonstrates all parallel processing features.
    /// </summary>
    public static void RunExample()
    {
        Console.WriteLine("\n=== High-Performance Parallel Processing ===\n");

        Console.WriteLine("1. Summation Benchmark:");
        BenchmarkParallelism();

        Console.WriteLine("\n2. Matrix Multiplication:");
        var matrixSize = 100;
        var a = new double[matrixSize, matrixSize];
        var b = new double[matrixSize, matrixSize];

        // Initialize matrices
        for (var i = 0; i < matrixSize; i++)
        {
            for (var j = 0; j < matrixSize; j++)
            {
                a[i, j] = i + j;
                b[i, j] = i - j;
            }
        }

        var sw = Stopwatch.StartNew();
        var result = ParallelMatrixMultiply(a, b);
        sw.Stop();
        Console.WriteLine($"Matrix {matrixSize}x{matrixSize} multiplication: {sw.Elapsed.TotalMilliseconds:F2} ms");
        Console.WriteLine($"Result[0,0] = {result[0, 0]:F2}");

        Console.WriteLine("\n3. Parallel Pipeline (PLINQ):");
        var input = Enumerable.Range(1, 20);
        var pipelineResult = ParallelPipeline(input);
        Console.WriteLine($"Input: {string.Join(", ", input)}");
        Console.WriteLine($"Output (squared, evens only): {string.Join(", ", pipelineResult)}");
    }
}
