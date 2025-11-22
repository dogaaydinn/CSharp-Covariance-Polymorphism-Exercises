using System.Buffers;
using System.Diagnostics;

namespace AdvancedCsharpConcepts.Advanced.HighPerformance;

/// <summary>
/// ArrayPool&lt;T&gt; Examples - Zero-allocation array reuse for hot paths.
/// NVIDIA GPU Developer best practice: Minimize GC pressure through object pooling.
/// </summary>
/// <remarks>
/// ArrayPool provides a high-performance pool of reusable arrays.
/// Benefits:
/// - Reduces GC pressure (fewer allocations)
/// - Faster than new T[] in hot paths (no allocation overhead)
/// - Thread-safe and optimized for multi-core
/// - Typical performance: 10-100x faster than allocation in loops
/// </remarks>
public static class ArrayPoolExamples
{
    /// <summary>
    /// Traditional approach: Allocates new arrays (causes GC pressure).
    /// </summary>
    public static long ProcessDataTraditional(int iterations, int bufferSize)
    {
        long totalSum = 0;

        for (var i = 0; i < iterations; i++)
        {
            var buffer = new int[bufferSize]; // ❌ Allocation every iteration

            // Simulate work
            for (var j = 0; j < buffer.Length; j++)
            {
                buffer[j] = j * 2;
            }

            totalSum += buffer.Sum();
            // Buffer becomes garbage (GC pressure)
        }

        return totalSum;
    }

    /// <summary>
    /// Optimized approach: Uses ArrayPool to reuse arrays (zero GC pressure).
    /// </summary>
    public static long ProcessDataOptimized(int iterations, int bufferSize)
    {
        long totalSum = 0;
        var pool = ArrayPool<int>.Shared;

        for (var i = 0; i < iterations; i++)
        {
            var buffer = pool.Rent(bufferSize); // ✅ Rent from pool (no allocation)

            try
            {
                // Simulate work
                for (var j = 0; j < bufferSize; j++)
                {
                    buffer[j] = j * 2;
                }

                // Important: Only use up to bufferSize (rented array may be larger)
                totalSum += buffer.AsSpan(0, bufferSize).ToArray().Sum();
            }
            finally
            {
                pool.Return(buffer, clearArray: true); // Return to pool for reuse
            }
        }

        return totalSum;
    }

    /// <summary>
    /// Real-world example: String processing with ArrayPool.
    /// </summary>
    public static string[] ParseCsvOptimized(string csv, char delimiter = ',')
    {
        if (string.IsNullOrEmpty(csv))
            return Array.Empty<string>();

        var pool = ArrayPool<string>.Shared;
        var tempBuffer = pool.Rent(1000); // Rent buffer for temporary storage

        try
        {
            var count = 0;
            var span = csv.AsSpan();
            var start = 0;

            for (var i = 0; i < span.Length; i++)
            {
                if (span[i] == delimiter || i == span.Length - 1)
                {
                    var end = (i == span.Length - 1 && span[i] != delimiter) ? i + 1 : i;
                    var segment = span.Slice(start, end - start).Trim();

                    if (count >= tempBuffer.Length)
                    {
                        // Grow buffer if needed
                        var newBuffer = pool.Rent(tempBuffer.Length * 2);
                        Array.Copy(tempBuffer, newBuffer, count);
                        pool.Return(tempBuffer);
                        tempBuffer = newBuffer;
                    }

                    tempBuffer[count++] = segment.ToString();
                    start = i + 1;
                }
            }

            // Copy to final array of exact size
            var result = new string[count];
            Array.Copy(tempBuffer, result, count);
            return result;
        }
        finally
        {
            pool.Return(tempBuffer, clearArray: true);
        }
    }

    /// <summary>
    /// Image processing example using ArrayPool.
    /// </summary>
    public static byte[] ApplyGrayscaleFilter(byte[] rgbPixels)
    {
        if (rgbPixels == null || rgbPixels.Length % 3 != 0)
            throw new ArgumentException("Invalid RGB array");

        var pixelCount = rgbPixels.Length / 3;
        var pool = ArrayPool<byte>.Shared;
        var grayscale = new byte[pixelCount];

        // Rent temporary buffer for intermediate calculations
        var tempBuffer = pool.Rent(pixelCount);

        try
        {
            // Process pixels in chunks
            for (var i = 0; i < pixelCount; i++)
            {
                var r = rgbPixels[i * 3];
                var g = rgbPixels[i * 3 + 1];
                var b = rgbPixels[i * 3 + 2];

                // Grayscale formula: 0.299*R + 0.587*G + 0.114*B
                tempBuffer[i] = (byte)(0.299 * r + 0.587 * g + 0.114 * b);
            }

            // Copy results
            Array.Copy(tempBuffer, grayscale, pixelCount);
        }
        finally
        {
            pool.Return(tempBuffer);
        }

        return grayscale;
    }

    /// <summary>
    /// Network packet processing example with ArrayPool.
    /// </summary>
    public static async Task<int> ProcessPacketsAsync(int packetCount, int packetSize, CancellationToken cancellationToken = default)
    {
        var pool = ArrayPool<byte>.Shared;
        var totalBytesProcessed = 0;

        for (var i = 0; i < packetCount; i++)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var packet = pool.Rent(packetSize);

            try
            {
                // Simulate receiving packet
                await Task.Delay(1, cancellationToken);

                // Fill packet with data
                for (var j = 0; j < packetSize; j++)
                {
                    packet[j] = (byte)(j % 256);
                }

                // Process packet (example: checksum calculation)
                var checksum = CalculateChecksum(packet.AsSpan(0, packetSize));
                totalBytesProcessed += packetSize;

                // Simulate sending response
                if (checksum > 0)
                {
                    await Task.Delay(1, cancellationToken);
                }
            }
            finally
            {
                pool.Return(packet);
            }
        }

        return totalBytesProcessed;
    }

    private static int CalculateChecksum(Span<byte> data)
    {
        var sum = 0;
        foreach (var b in data)
        {
            sum += b;
        }
        return sum;
    }

    /// <summary>
    /// Matrix operations with ArrayPool.
    /// </summary>
    public static double[,] MultiplyMatricesOptimized(double[,] a, double[,] b)
    {
        var rowsA = a.GetLength(0);
        var colsA = a.GetLength(1);
        var rowsB = b.GetLength(0);
        var colsB = b.GetLength(1);

        if (colsA != rowsB)
            throw new ArgumentException("Matrix dimensions incompatible for multiplication");

        var result = new double[rowsA, colsB];
        var pool = ArrayPool<double>.Shared;

        // Rent temporary buffer for row/column data
        var tempRow = pool.Rent(colsA);
        var tempCol = pool.Rent(rowsB);

        try
        {
            for (var i = 0; i < rowsA; i++)
            {
                for (var j = 0; j < colsB; j++)
                {
                    // Extract column from B into temp buffer
                    for (var k = 0; k < rowsB; k++)
                    {
                        tempCol[k] = b[k, j];
                    }

                    // Calculate dot product
                    double sum = 0;
                    for (var k = 0; k < colsA; k++)
                    {
                        sum += a[i, k] * tempCol[k];
                    }

                    result[i, j] = sum;
                }
            }
        }
        finally
        {
            pool.Return(tempRow);
            pool.Return(tempCol);
        }

        return result;
    }

    /// <summary>
    /// Demonstrates ArrayPool performance benefits.
    /// </summary>
    public static void RunExample()
    {
        Console.WriteLine("=== ArrayPool<T> Optimization Examples ===\n");

        // Example 1: Performance comparison
        Console.WriteLine("1. Performance Comparison (Traditional vs ArrayPool):");
        const int iterations = 10_000;
        const int bufferSize = 1000;

        // Warm-up
        ProcessDataTraditional(100, bufferSize);
        ProcessDataOptimized(100, bufferSize);

        // Traditional approach
        var sw = Stopwatch.StartNew();
        var gen0Before = GC.CollectionCount(0);
        var result1 = ProcessDataTraditional(iterations, bufferSize);
        var traditionalTime = sw.Elapsed;
        var gen0After = GC.CollectionCount(0);
        var traditionalGC = gen0After - gen0Before;

        // ArrayPool approach
        sw.Restart();
        gen0Before = GC.CollectionCount(0);
        var result2 = ProcessDataOptimized(iterations, bufferSize);
        var optimizedTime = sw.Elapsed;
        gen0After = GC.CollectionCount(0);
        var optimizedGC = gen0After - gen0Before;

        Console.WriteLine($"  Traditional: {traditionalTime.TotalMilliseconds:F2} ms, GC Gen0: {traditionalGC}");
        Console.WriteLine($"  ArrayPool:   {optimizedTime.TotalMilliseconds:F2} ms, GC Gen0: {optimizedGC}");
        Console.WriteLine($"  Speedup:     {traditionalTime.TotalMilliseconds / optimizedTime.TotalMilliseconds:F2}x");
        Console.WriteLine($"  GC Reduction: {(traditionalGC - optimizedGC) / (double)traditionalGC * 100:F1}%");
        Console.WriteLine($"  Results match: {result1 == result2}");

        // Example 2: CSV parsing
        Console.WriteLine("\n2. CSV Parsing with ArrayPool:");
        var csv = string.Join(",", Enumerable.Range(1, 100).Select(i => $"Item{i}"));
        sw.Restart();
        var items = ParseCsvOptimized(csv);
        Console.WriteLine($"  Parsed {items.Length} items in {sw.Elapsed.TotalMicroseconds:F0} μs");
        Console.WriteLine($"  First 5 items: {string.Join(", ", items.Take(5))}");

        // Example 3: Image processing
        Console.WriteLine("\n3. Image Processing (RGB to Grayscale):");
        var rgbData = new byte[1920 * 1080 * 3]; // Full HD image
        new Random(42).NextBytes(rgbData);

        sw.Restart();
        var grayscale = ApplyGrayscaleFilter(rgbData);
        Console.WriteLine($"  Processed {grayscale.Length:N0} pixels in {sw.Elapsed.TotalMilliseconds:F2} ms");
        Console.WriteLine($"  Throughput: {grayscale.Length / sw.Elapsed.TotalSeconds / 1_000_000:F2} MPixels/sec");

        // Example 4: Network packet processing
        Console.WriteLine("\n4. Network Packet Processing:");
        sw.Restart();
        var bytesProcessed = ProcessPacketsAsync(1000, 1500).Result;
        Console.WriteLine($"  Processed {bytesProcessed / 1024:N0} KB in {sw.Elapsed.TotalMilliseconds:F2} ms");
        Console.WriteLine($"  Throughput: {bytesProcessed / sw.Elapsed.TotalSeconds / 1024 / 1024:F2} MB/sec");

        // Example 5: Matrix multiplication
        Console.WriteLine("\n5. Matrix Multiplication with ArrayPool:");
        var matrixA = new double[100, 100];
        var matrixB = new double[100, 100];

        // Initialize matrices
        for (var i = 0; i < 100; i++)
        {
            for (var j = 0; j < 100; j++)
            {
                matrixA[i, j] = i + j;
                matrixB[i, j] = i - j;
            }
        }

        sw.Restart();
        var resultMatrix = MultiplyMatricesOptimized(matrixA, matrixB);
        Console.WriteLine($"  Multiplied 100x100 matrices in {sw.Elapsed.TotalMilliseconds:F2} ms");
        Console.WriteLine($"  Result[50,50]: {resultMatrix[50, 50]:F2}");
    }
}
