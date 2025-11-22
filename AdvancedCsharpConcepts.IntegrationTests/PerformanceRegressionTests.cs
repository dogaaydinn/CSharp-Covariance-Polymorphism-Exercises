using System.Diagnostics;
using AdvancedCsharpConcepts.Advanced.HighPerformance;
using FluentAssertions;
using Xunit;

namespace AdvancedCsharpConcepts.IntegrationTests;

/// <summary>
/// Performance regression tests - Ensures performance doesn't degrade over time.
/// These tests establish performance baselines and fail if performance regresses significantly.
/// </summary>
public class PerformanceRegressionTests
{
    private const int WarmupIterations = 5;

    #region Helper Methods

    private static TimeSpan MeasurePerformance(Action action, int iterations = 1)
    {
        // Warm-up
        for (var i = 0; i < WarmupIterations; i++)
        {
            action();
        }

        // Force GC before measurement
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();

        // Measure
        var sw = Stopwatch.StartNew();
        for (var i = 0; i < iterations; i++)
        {
            action();
        }
        sw.Stop();

        return sw.Elapsed;
    }

    private static async Task<TimeSpan> MeasurePerformanceAsync(Func<Task> action, int iterations = 1)
    {
        // Warm-up
        for (var i = 0; i < WarmupIterations; i++)
        {
            await action();
        }

        // Force GC
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();

        // Measure
        var sw = Stopwatch.StartNew();
        for (var i = 0; i < iterations; i++)
        {
            await action();
        }
        sw.Stop();

        return sw.Elapsed;
    }

    #endregion

    #region Parallel Processing Performance

    [Fact]
    public void ParallelSum_Should_Complete_Within_Performance_Budget()
    {
        // Arrange
        const int dataSize = 10_000_000;
        const int maxMilliseconds = 500; // Performance budget: 500ms

        // Act
        var elapsed = MeasurePerformance(() =>
        {
            var result = ParallelProcessingExamples.ParallelForSum(dataSize);
            result.Should().BeGreaterThan(0);
        });

        // Assert
        elapsed.TotalMilliseconds.Should().BeLessThan(maxMilliseconds,
            $"Parallel sum should complete within {maxMilliseconds}ms (actual: {elapsed.TotalMilliseconds:F2}ms)");
    }

    [Fact]
    public void OptimizedParallelSum_Should_Be_Faster_Than_Sequential()
    {
        // Arrange
        const int dataSize = 10_000_000;

        // Act - Sequential
        var sequentialTime = MeasurePerformance(() =>
        {
            ParallelProcessingExamples.SequentialSum(dataSize);
        });

        // Act - Parallel
        var parallelTime = MeasurePerformance(() =>
        {
            ParallelProcessingExamples.OptimizedParallelSum(dataSize);
        });

        // Assert - Parallel should be at least 1.5x faster
        parallelTime.TotalMilliseconds.Should().BeLessThan(sequentialTime.TotalMilliseconds / 1.5,
            $"Parallel version should be at least 1.5x faster (Sequential: {sequentialTime.TotalMilliseconds:F2}ms, Parallel: {parallelTime.TotalMilliseconds:F2}ms)");
    }

    #endregion

    #region SIMD Performance

    [Fact]
    public void SIMD_VectorAdd_Should_Be_Faster_Than_Scalar()
    {
        // Arrange
        const int size = 1_000_000;
        var array1 = Enumerable.Range(0, size).Select(i => (float)i).ToArray();
        var array2 = Enumerable.Range(0, size).Select(i => (float)i * 0.5f).ToArray();

        // Act - Scalar
        var scalarTime = MeasurePerformance(() =>
        {
            SIMDExamples.ScalarAdd(array1, array2);
        }, iterations: 10);

        // Act - SIMD
        var simdTime = MeasurePerformance(() =>
        {
            SIMDExamples.VectorAdd(array1, array2);
        }, iterations: 10);

        // Assert - SIMD should be at least 2x faster
        simdTime.TotalMilliseconds.Should().BeLessThan(scalarTime.TotalMilliseconds / 2.0,
            $"SIMD version should be at least 2x faster (Scalar: {scalarTime.TotalMilliseconds:F2}ms, SIMD: {simdTime.TotalMilliseconds:F2}ms, Speedup: {scalarTime.TotalMilliseconds / simdTime.TotalMilliseconds:F2}x)");
    }

    [Fact]
    public void SIMD_DotProduct_Should_Complete_Within_Budget()
    {
        // Arrange
        const int size = 1_000_000;
        var array1 = Enumerable.Range(0, size).Select(i => (float)i).ToArray();
        var array2 = Enumerable.Range(0, size).Select(i => (float)i * 0.5f).ToArray();
        const int maxMilliseconds = 20;

        // Act
        var elapsed = MeasurePerformance(() =>
        {
            var result = SIMDExamples.DotProduct(array1, array2);
            result.Should().BeGreaterThan(0);
        }, iterations: 10);

        // Assert
        elapsed.TotalMilliseconds.Should().BeLessThan(maxMilliseconds,
            $"SIMD dot product should complete within {maxMilliseconds}ms (actual: {elapsed.TotalMilliseconds:F2}ms)");
    }

    #endregion

    #region ArrayPool Performance

    [Fact]
    public void ArrayPool_Should_Reduce_GC_Pressure()
    {
        // Arrange
        const int iterations = 1000;
        const int bufferSize = 1000;

        // Act - Traditional (measure GC collections)
        var gen0Before = GC.CollectionCount(0);
        var gen1Before = GC.CollectionCount(1);

        for (var i = 0; i < iterations; i++)
        {
            ArrayPoolExamples.ProcessDataTraditional(10, bufferSize);
        }

        var traditionalGen0 = GC.CollectionCount(0) - gen0Before;
        var traditionalGen1 = GC.CollectionCount(1) - gen1Before;

        // Force GC to reset
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();

        // Act - ArrayPool (measure GC collections)
        gen0Before = GC.CollectionCount(0);
        gen1Before = GC.CollectionCount(1);

        for (var i = 0; i < iterations; i++)
        {
            ArrayPoolExamples.ProcessDataOptimized(10, bufferSize);
        }

        var optimizedGen0 = GC.CollectionCount(0) - gen0Before;
        var optimizedGen1 = GC.CollectionCount(1) - gen1Before;

        // Assert - ArrayPool should cause significantly fewer GC collections
        optimizedGen0.Should().BeLessThan(traditionalGen0,
            $"ArrayPool should reduce Gen0 collections (Traditional: {traditionalGen0}, ArrayPool: {optimizedGen0})");
    }

    [Fact]
    public void ArrayPool_Should_Be_Faster_Than_Traditional()
    {
        // Arrange
        const int iterations = 1000;
        const int bufferSize = 1000;

        // Act - Traditional
        var traditionalTime = MeasurePerformance(() =>
        {
            ArrayPoolExamples.ProcessDataTraditional(iterations, bufferSize);
        });

        // Act - ArrayPool
        var optimizedTime = MeasurePerformance(() =>
        {
            ArrayPoolExamples.ProcessDataOptimized(iterations, bufferSize);
        });

        // Assert - ArrayPool should be faster
        optimizedTime.TotalMilliseconds.Should().BeLessThan(traditionalTime.TotalMilliseconds,
            $"ArrayPool should be faster (Traditional: {traditionalTime.TotalMilliseconds:F2}ms, ArrayPool: {optimizedTime.TotalMilliseconds:F2}ms, Speedup: {traditionalTime.TotalMilliseconds / optimizedTime.TotalMilliseconds:F2}x)");
    }

    #endregion

    #region Span<T> Performance

    [Fact]
    public void SpanMemory_ModernApproach_Should_Be_Faster()
    {
        // Arrange
        var input = string.Join(",", Enumerable.Range(1, 1000));

        // Act - Traditional
        var traditionalTime = MeasurePerformance(() =>
        {
            SpanMemoryExamples.ParseNumbersTraditional(input);
        }, iterations: 1000);

        // Act - Modern (Span)
        var modernTime = MeasurePerformance(() =>
        {
            SpanMemoryExamples.ParseNumbersModern(input);
        }, iterations: 1000);

        // Assert
        modernTime.TotalMilliseconds.Should().BeLessThan(traditionalTime.TotalMilliseconds,
            $"Span<T> approach should be faster (Traditional: {traditionalTime.TotalMilliseconds:F2}ms, Modern: {modernTime.TotalMilliseconds:F2}ms)");
    }

    #endregion

    #region Matrix Operations Performance

    [Fact]
    public void MatrixMultiplication_Should_Complete_Within_Budget()
    {
        // Arrange
        const int size = 100;
        var matrixA = new double[size, size];
        var matrixB = new double[size, size];

        for (var i = 0; i < size; i++)
        {
            for (var j = 0; j < size; j++)
            {
                matrixA[i, j] = i + j;
                matrixB[i, j] = i - j;
            }
        }

        const int maxMilliseconds = 100; // 100ms budget for 100x100 matrices

        // Act
        var elapsed = MeasurePerformance(() =>
        {
            var result = ParallelProcessingExamples.ParallelMatrixMultiply(matrixA, matrixB);
            result.Should().NotBeNull();
        });

        // Assert
        elapsed.TotalMilliseconds.Should().BeLessThan(maxMilliseconds,
            $"Matrix multiplication should complete within {maxMilliseconds}ms (actual: {elapsed.TotalMilliseconds:F2}ms)");
    }

    #endregion

    #region Memory Allocation Tests

    [Fact]
    public void PerformanceTest_Should_Not_Allocate_Excessively()
    {
        // Arrange
        const long maxBytesAllocated = 100 * 1024 * 1024; // 100 MB max

        // Act
        var memoryBefore = GC.GetTotalMemory(true);

        // Run some operations
        for (var i = 0; i < 1000; i++)
        {
            ArrayPoolExamples.ProcessDataOptimized(10, 100);
        }

        var memoryAfter = GC.GetTotalMemory(false);
        var allocated = memoryAfter - memoryBefore;

        // Assert
        allocated.Should().BeLessThan(maxBytesAllocated,
            $"Should not allocate more than {maxBytesAllocated / 1024 / 1024}MB (actual: {allocated / 1024 / 1024}MB)");
    }

    #endregion

    #region Throughput Tests

    [Fact]
    public void ParallelProcessing_Should_Maintain_Minimum_Throughput()
    {
        // Arrange
        const int dataSize = 100_000_000;
        const double minThroughputMOpsPerSec = 100; // Minimum 100 million operations per second

        // Act
        var elapsed = MeasurePerformance(() =>
        {
            ParallelProcessingExamples.OptimizedParallelSum(dataSize);
        });

        var throughput = dataSize / elapsed.TotalSeconds / 1_000_000; // MOps/sec

        // Assert
        throughput.Should().BeGreaterThan(minThroughputMOpsPerSec,
            $"Throughput should be at least {minThroughputMOpsPerSec} MOps/sec (actual: {throughput:F2} MOps/sec)");
    }

    [Fact]
    public async Task AsyncOperations_Should_Maintain_Throughput()
    {
        // Arrange
        const int packetCount = 1000;
        const int packetSize = 1500;
        const double minThroughputMBPerSec = 1.0; // At least 1 MB/sec

        // Act
        var elapsed = await MeasurePerformanceAsync(async () =>
        {
            await ArrayPoolExamples.ProcessPacketsAsync(packetCount, packetSize);
        });

        var totalBytes = packetCount * packetSize;
        var throughputMBPerSec = (totalBytes / elapsed.TotalSeconds) / 1024 / 1024;

        // Assert
        throughputMBPerSec.Should().BeGreaterThan(minThroughputMBPerSec,
            $"Async throughput should be at least {minThroughputMBPerSec} MB/sec (actual: {throughputMBPerSec:F2} MB/sec)");
    }

    #endregion

    #region Scalability Tests

    [Fact]
    public void ParallelProcessing_Should_Scale_With_Data_Size()
    {
        // Arrange - Test that doubling data size doesn't more than double execution time
        const int smallSize = 1_000_000;
        const int largeSize = 2_000_000;

        // Act
        var smallTime = MeasurePerformance(() =>
        {
            ParallelProcessingExamples.OptimizedParallelSum(smallSize);
        }, iterations: 10);

        var largeTime = MeasurePerformance(() =>
        {
            ParallelProcessingExamples.OptimizedParallelSum(largeSize);
        }, iterations: 10);

        var scalingFactor = largeTime.TotalMilliseconds / smallTime.TotalMilliseconds;

        // Assert - Should scale near-linearly (factor should be close to 2.0, allow up to 2.5)
        scalingFactor.Should().BeLessThan(2.5,
            $"Performance should scale linearly with data size (1M→2M: {scalingFactor:F2}x slowdown, expected <2.5x)");
    }

    #endregion
}
