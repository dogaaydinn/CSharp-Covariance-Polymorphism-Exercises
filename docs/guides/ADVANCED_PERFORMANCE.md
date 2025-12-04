# Advanced Performance Optimization Guide

## Overview

This guide covers advanced performance optimization techniques in C#, including SIMD (Single Instruction Multiple Data) operations, parallel processing, async streams, and memory optimization.

## Table of Contents

- [SIMD Operations](#simd-operations)
- [Parallel Processing](#parallel-processing)
- [Async Streams](#async-streams)
- [Memory Optimization](#memory-optimization)
- [GPU Acceleration](#gpu-acceleration)
- [Benchmarking](#benchmarking)
- [Best Practices](#best-practices)

## SIMD Operations

SIMD allows processing multiple data points with a single CPU instruction, dramatically improving performance for data-parallel operations.

### Vector<T> - Hardware Agnostic

```csharp
using System.Numerics;

public class VectorOperations
{
    // Add two arrays using SIMD
    public static void AddArrays(float[] a, float[] b, float[] result)
    {
        int vectorSize = Vector<float>.Count; // Typically 4, 8, or 16
        int i = 0;

        // Process in vector-sized chunks
        for (; i <= a.Length - vectorSize; i += vectorSize)
        {
            var va = new Vector<float>(a, i);
            var vb = new Vector<float>(b, i);
            var vr = va + vb;
            vr.CopyTo(result, i);
        }

        // Handle remaining elements
        for (; i < a.Length; i++)
        {
            result[i] = a[i] + b[i];
        }
    }

    // Multiply array by scalar
    public static void MultiplyScalar(float[] array, float scalar, float[] result)
    {
        var scalarVector = new Vector<float>(scalar);
        int vectorSize = Vector<float>.Count;
        int i = 0;

        for (; i <= array.Length - vectorSize; i += vectorSize)
        {
            var v = new Vector<float>(array, i);
            var r = v * scalarVector;
            r.CopyTo(result, i);
        }

        for (; i < array.Length; i++)
        {
            result[i] = array[i] * scalar;
        }
    }

    // Dot product using SIMD
    public static float DotProduct(float[] a, float[] b)
    {
        if (a.Length != b.Length)
            throw new ArgumentException("Arrays must have same length");

        var sum = Vector<float>.Zero;
        int vectorSize = Vector<float>.Count;
        int i = 0;

        for (; i <= a.Length - vectorSize; i += vectorSize)
        {
            var va = new Vector<float>(a, i);
            var vb = new Vector<float>(b, i);
            sum += va * vb;
        }

        float result = Vector.Dot(sum, Vector<float>.One);

        // Handle remaining elements
        for (; i < a.Length; i++)
        {
            result += a[i] * b[i];
        }

        return result;
    }

    // Find minimum value in array
    public static float FindMin(float[] array)
    {
        var min = new Vector<float>(float.MaxValue);
        int vectorSize = Vector<float>.Count;
        int i = 0;

        for (; i <= array.Length - vectorSize; i += vectorSize)
        {
            var v = new Vector<float>(array, i);
            min = Vector.Min(min, v);
        }

        float result = float.MaxValue;
        for (int j = 0; j < vectorSize; j++)
        {
            result = Math.Min(result, min[j]);
        }

        for (; i < array.Length; i++)
        {
            result = Math.Min(result, array[i]);
        }

        return result;
    }
}
```

### Vector128/256/512 - Explicit SIMD

```csharp
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;

public class ExplicitSimd
{
    // SSE2 - 128-bit vectors (always available on x64)
    public static void AddArraysSse2(float[] a, float[] b, float[] result)
    {
        if (!Sse.IsSupported)
        {
            throw new NotSupportedException("SSE not supported");
        }

        int i = 0;
        for (; i <= a.Length - 4; i += 4)
        {
            var va = Sse.LoadVector128(&a[i]);
            var vb = Sse.LoadVector128(&b[i]);
            var vr = Sse.Add(va, vb);
            Sse.Store(&result[i], vr);
        }

        for (; i < a.Length; i++)
        {
            result[i] = a[i] + b[i];
        }
    }

    // AVX2 - 256-bit vectors (8 floats at once)
    public static unsafe void AddArraysAvx2(float[] a, float[] b, float[] result)
    {
        if (!Avx.IsSupported)
        {
            // Fallback to SSE2 or scalar
            AddArraysSse2(a, b, result);
            return;
        }

        int i = 0;
        fixed (float* pA = a, pB = b, pResult = result)
        {
            for (; i <= a.Length - 8; i += 8)
            {
                var va = Avx.LoadVector256(pA + i);
                var vb = Avx.LoadVector256(pB + i);
                var vr = Avx.Add(va, vb);
                Avx.Store(pResult + i, vr);
            }
        }

        for (; i < a.Length; i++)
        {
            result[i] = a[i] + b[i];
        }
    }

    // AVX-512 - 512-bit vectors (16 floats at once)
    public static unsafe void AddArraysAvx512(float[] a, float[] b, float[] result)
    {
        if (!Avx512F.IsSupported)
        {
            AddArraysAvx2(a, b, result);
            return;
        }

        int i = 0;
        fixed (float* pA = a, pB = b, pResult = result)
        {
            for (; i <= a.Length - 16; i += 16)
            {
                var va = Avx512F.LoadVector512(pA + i);
                var vb = Avx512F.LoadVector512(pB + i);
                var vr = Avx512F.Add(va, vb);
                Avx512F.Store(pResult + i, vr);
            }
        }

        for (; i < a.Length; i++)
        {
            result[i] = a[i] + b[i];
        }
    }

    // FMA (Fused Multiply-Add) - a * b + c in one instruction
    public static unsafe void FusedMultiplyAdd(float[] a, float[] b, float[] c, float[] result)
    {
        if (!Fma.IsSupported)
        {
            for (int i = 0; i < a.Length; i++)
            {
                result[i] = a[i] * b[i] + c[i];
            }
            return;
        }

        int i = 0;
        fixed (float* pA = a, pB = b, pC = c, pResult = result)
        {
            for (; i <= a.Length - 8; i += 8)
            {
                var va = Avx.LoadVector256(pA + i);
                var vb = Avx.LoadVector256(pB + i);
                var vc = Avx.LoadVector256(pC + i);
                var vr = Fma.MultiplyAdd(va, vb, vc);
                Avx.Store(pResult + i, vr);
            }
        }

        for (; i < a.Length; i++)
        {
            result[i] = a[i] * b[i] + c[i];
        }
    }
}
```

### Practical SIMD Examples

#### Image Processing - Brightness Adjustment

```csharp
public class ImageProcessing
{
    // Adjust brightness of RGB image (3 bytes per pixel)
    public static void AdjustBrightness(byte[] pixels, float factor)
    {
        var factorVector = new Vector<float>(factor);
        int vectorSize = Vector<byte>.Count;
        int i = 0;

        // Process in chunks
        for (; i <= pixels.Length - vectorSize; i += vectorSize)
        {
            var v = new Vector<byte>(pixels, i);

            // Convert to float for multiplication
            Vector.Widen(v, out var v1, out var v2);
            Vector.Widen(v1, out var v1a, out var v1b);
            Vector.Widen(v2, out var v2a, out var v2b);

            // Apply brightness
            v1a = Vector.ConvertToInt32(Vector.Multiply(Vector.ConvertToSingle(v1a), factorVector));
            v1b = Vector.ConvertToInt32(Vector.Multiply(Vector.ConvertToSingle(v1b), factorVector));
            v2a = Vector.ConvertToInt32(Vector.Multiply(Vector.ConvertToSingle(v2a), factorVector));
            v2b = Vector.ConvertToInt32(Vector.Multiply(Vector.ConvertToSingle(v2b), factorVector));

            // Narrow back to bytes
            var result = Vector.Narrow(
                Vector.Narrow(v1a, v1b),
                Vector.Narrow(v2a, v2b));

            result.CopyTo(pixels, i);
        }

        // Handle remaining pixels
        for (; i < pixels.Length; i++)
        {
            pixels[i] = (byte)Math.Clamp(pixels[i] * factor, 0, 255);
        }
    }

    // Gaussian blur (simplified)
    public static void GaussianBlur(float[] input, float[] output, int width, int height)
    {
        // 3x3 Gaussian kernel
        float[] kernel = { 0.0625f, 0.125f, 0.0625f,
                          0.125f,  0.25f,  0.125f,
                          0.0625f, 0.125f, 0.0625f };

        for (int y = 1; y < height - 1; y++)
        {
            int i = y * width + 1;
            for (; i < (y + 1) * width - 1 - Vector<float>.Count; i += Vector<float>.Count)
            {
                var sum = Vector<float>.Zero;

                // Apply kernel using SIMD
                for (int ky = -1; ky <= 1; ky++)
                {
                    for (int kx = -1; kx <= 1; kx++)
                    {
                        int idx = (y + ky) * width + (i + kx);
                        var pixel = new Vector<float>(input, idx);
                        var weight = new Vector<float>(kernel[(ky + 1) * 3 + (kx + 1)]);
                        sum += pixel * weight;
                    }
                }

                sum.CopyTo(output, i);
            }
        }
    }
}
```

#### Matrix Operations

```csharp
public class MatrixOperations
{
    // Matrix multiplication with SIMD
    public static void Multiply(float[] a, float[] b, float[] result, int size)
    {
        // A: size x size, B: size x size, Result: size x size
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                var sum = Vector<float>.Zero;
                int k = 0;

                // Vectorized inner loop
                for (; k <= size - Vector<float>.Count; k += Vector<float>.Count)
                {
                    var va = new Vector<float>(a, i * size + k);
                    var vb = new Vector<float>(b, k * size + j);
                    sum += va * vb;
                }

                float scalar = 0;
                for (int v = 0; v < Vector<float>.Count; v++)
                {
                    scalar += sum[v];
                }

                // Remaining elements
                for (; k < size; k++)
                {
                    scalar += a[i * size + k] * b[k * size + j];
                }

                result[i * size + j] = scalar;
            }
        }
    }

    // Transpose matrix with SIMD (out-of-place)
    public static void Transpose(float[] input, float[] output, int rows, int cols)
    {
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                output[j * rows + i] = input[i * cols + j];
            }
        }
    }
}
```

## Parallel Processing

### Parallel.For and Parallel.ForEach

```csharp
using System.Threading.Tasks;

public class ParallelProcessing
{
    // Process large array in parallel
    public static void ProcessArrayParallel(int[] data, Func<int, int> process)
    {
        Parallel.For(0, data.Length, i =>
        {
            data[i] = process(data[i]);
        });
    }

    // Parallel ForEach with options
    public static void ProcessItemsParallel<T>(IEnumerable<T> items, Action<T> process)
    {
        var options = new ParallelOptions
        {
            MaxDegreeOfParallelism = Environment.ProcessorCount,
            CancellationToken = CancellationToken.None
        };

        Parallel.ForEach(items, options, item =>
        {
            process(item);
        });
    }

    // Parallel aggregation (sum)
    public static long ParallelSum(int[] data)
    {
        long sum = 0;
        var lockObject = new object();

        Parallel.For(0, data.Length, () => 0L, // thread-local init
            (i, state, localSum) =>
            {
                localSum += data[i];
                return localSum;
            },
            localSum =>
            {
                lock (lockObject)
                {
                    sum += localSum;
                }
            });

        return sum;
    }

    // Better: Use Partitioner for better work distribution
    public static long ParallelSumPartitioned(int[] data)
    {
        return Partitioner.Create(0, data.Length)
            .AsParallel()
            .Sum(range =>
            {
                long localSum = 0;
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    localSum += data[i];
                }
                return localSum;
            });
    }
}
```

### PLINQ (Parallel LINQ)

```csharp
public class PlinqExamples
{
    // Simple parallel query
    public static List<int> FilterAndTransform(int[] data)
    {
        return data
            .AsParallel()
            .Where(x => x % 2 == 0)
            .Select(x => x * x)
            .ToList();
    }

    // Parallel query with custom partitioning
    public static double AverageExpensiveComputation(int[] data)
    {
        return data
            .AsParallel()
            .WithDegreeOfParallelism(Environment.ProcessorCount)
            .Select(x => ExpensiveComputation(x))
            .Average();
    }

    // Order preservation
    public static List<int> OrderedParallelProcessing(int[] data)
    {
        return data
            .AsParallel()
            .AsOrdered() // Preserve input order
            .Select(x => x * 2)
            .ToList();
    }

    // Aggregation with PLINQ
    public static Dictionary<int, int> ParallelGrouping(int[] data)
    {
        return data
            .AsParallel()
            .GroupBy(x => x % 10)
            .ToDictionary(g => g.Key, g => g.Count());
    }

    private static double ExpensiveComputation(int x)
    {
        // Simulate expensive operation
        Thread.Sleep(1);
        return Math.Sqrt(x);
    }
}
```

### Channels for Producer-Consumer

```csharp
using System.Threading.Channels;

public class ChannelExamples
{
    // Producer-consumer with channels
    public static async Task ProcessWithChannels(IEnumerable<int> input)
    {
        var channel = Channel.CreateBounded<int>(new BoundedChannelOptions(100)
        {
            FullMode = BoundedChannelFullMode.Wait
        });

        // Producer task
        var producer = Task.Run(async () =>
        {
            foreach (var item in input)
            {
                await channel.Writer.WriteAsync(item);
            }
            channel.Writer.Complete();
        });

        // Multiple consumer tasks
        var consumers = Enumerable.Range(0, 4).Select(i =>
            Task.Run(async () =>
            {
                await foreach (var item in channel.Reader.ReadAllAsync())
                {
                    // Process item
                    await ProcessItemAsync(item);
                }
            })).ToArray();

        await Task.WhenAll(producer);
        await Task.WhenAll(consumers);
    }

    // Pipeline pattern with channels
    public static async Task<List<string>> ProcessPipeline(IEnumerable<int> input)
    {
        var stage1Channel = Channel.CreateUnbounded<int>();
        var stage2Channel = Channel.CreateUnbounded<double>();
        var stage3Channel = Channel.CreateUnbounded<string>();

        // Stage 1: Transform to double
        var stage1 = Task.Run(async () =>
        {
            foreach (var item in input)
            {
                await stage1Channel.Writer.WriteAsync(item);
            }
            stage1Channel.Writer.Complete();
        });

        // Stage 2: Process doubles
        var stage2 = Task.Run(async () =>
        {
            await foreach (var item in stage1Channel.Reader.ReadAllAsync())
            {
                await stage2Channel.Writer.WriteAsync(Math.Sqrt(item));
            }
            stage2Channel.Writer.Complete();
        });

        // Stage 3: Format strings
        var stage3 = Task.Run(async () =>
        {
            await foreach (var item in stage2Channel.Reader.ReadAllAsync())
            {
                await stage3Channel.Writer.WriteAsync($"Result: {item:F2}");
            }
            stage3Channel.Writer.Complete();
        });

        // Collect results
        var results = new List<string>();
        await foreach (var result in stage3Channel.Reader.ReadAllAsync())
        {
            results.Add(result);
        }

        await Task.WhenAll(stage1, stage2, stage3);
        return results;
    }

    private static async Task ProcessItemAsync(int item)
    {
        await Task.Delay(10); // Simulate async work
    }
}
```

## Async Streams

### IAsyncEnumerable<T>

```csharp
public class AsyncStreams
{
    // Async stream producer
    public static async IAsyncEnumerable<int> GenerateNumbersAsync(
        int count,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        for (int i = 0; i < count; i++)
        {
            cancellationToken.ThrowIfCancellationRequested();

            // Simulate async data source
            await Task.Delay(100, cancellationToken);

            yield return i;
        }
    }

    // Async stream transformation
    public static async IAsyncEnumerable<TResult> SelectAsync<T, TResult>(
        this IAsyncEnumerable<T> source,
        Func<T, Task<TResult>> selector)
    {
        await foreach (var item in source)
        {
            yield return await selector(item);
        }
    }

    // Async stream filtering
    public static async IAsyncEnumerable<T> WhereAsync<T>(
        this IAsyncEnumerable<T> source,
        Func<T, Task<bool>> predicate)
    {
        await foreach (var item in source)
        {
            if (await predicate(item))
            {
                yield return item;
            }
        }
    }

    // Consume async stream
    public static async Task ConsumeAsyncStream()
    {
        await foreach (var number in GenerateNumbersAsync(10))
        {
            Console.WriteLine($"Received: {number}");
        }
    }

    // Real-world example: Process large file
    public static async IAsyncEnumerable<string> ReadLargeFileAsync(
        string filePath,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        using var reader = new StreamReader(filePath);

        while (!reader.EndOfStream)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var line = await reader.ReadLineAsync();
            if (line != null)
            {
                yield return line;
            }
        }
    }

    // Process file with async stream
    public static async Task ProcessLargeFileAsync(string filePath)
    {
        var validLines = ReadLargeFileAsync(filePath)
            .WhereAsync(async line => await IsValidLineAsync(line))
            .SelectAsync(async line => await ProcessLineAsync(line));

        await foreach (var result in validLines)
        {
            Console.WriteLine(result);
        }
    }

    private static async Task<bool> IsValidLineAsync(string line)
    {
        await Task.Delay(1); // Simulate async validation
        return !string.IsNullOrWhiteSpace(line);
    }

    private static async Task<string> ProcessLineAsync(string line)
    {
        await Task.Delay(10); // Simulate async processing
        return line.ToUpper();
    }
}
```

## Memory Optimization

### Span<T> and Memory<T>

```csharp
public class MemoryOptimization
{
    // Parse integers from string without allocation
    public static int SumNumbersFromString(string input)
    {
        ReadOnlySpan<char> span = input.AsSpan();
        int sum = 0;

        while (span.Length > 0)
        {
            // Find next delimiter
            int delimiterIndex = span.IndexOf(',');
            ReadOnlySpan<char> numberSpan = delimiterIndex >= 0
                ? span.Slice(0, delimiterIndex)
                : span;

            // Parse without allocating substring
            if (int.TryParse(numberSpan, out int number))
            {
                sum += number;
            }

            // Move to next number
            span = delimiterIndex >= 0
                ? span.Slice(delimiterIndex + 1)
                : ReadOnlySpan<char>.Empty;
        }

        return sum;
    }

    // Stack allocation for small buffers
    public static string ProcessSmallBuffer()
    {
        Span<char> buffer = stackalloc char[256];

        // Use buffer without heap allocation
        int length = FormatData(buffer);

        return new string(buffer.Slice(0, length));
    }

    private static int FormatData(Span<char> buffer)
    {
        return "Hello, World!".AsSpan().TryCopyTo(buffer) ? 13 : 0;
    }

    // Memory pooling
    public static async Task ProcessLargeDataWithPool()
    {
        var pool = MemoryPool<byte>.Shared;

        using (var owner = pool.Rent(8192))
        {
            Memory<byte> memory = owner.Memory;

            // Use memory
            await ProcessDataAsync(memory);
        } // Returns to pool automatically
    }

    private static async Task ProcessDataAsync(Memory<byte> memory)
    {
        await Task.Delay(100);
    }

    // ArrayPool for reusable arrays
    public static void ProcessWithArrayPool()
    {
        var pool = ArrayPool<int>.Shared;
        int[] array = pool.Rent(1024);

        try
        {
            // Use array
            Array.Clear(array, 0, array.Length);
            ProcessArray(array);
        }
        finally
        {
            pool.Return(array);
        }
    }

    private static void ProcessArray(int[] array)
    {
        for (int i = 0; i < array.Length; i++)
        {
            array[i] = i * 2;
        }
    }
}
```

## GPU Acceleration

### CUDA.NET Reference

**Note:** GPU acceleration requires additional libraries and is platform-specific.

```csharp
// Reference implementation (requires CUDA.NET NuGet package)
// This is a conceptual example - actual implementation varies by library

/*
using ILGPU;
using ILGPU.Runtime;

public class GpuAcceleration
{
    // GPU kernel for array addition
    public static void AddArraysKernel(
        Index1D index,
        ArrayView<float> a,
        ArrayView<float> b,
        ArrayView<float> result)
    {
        result[index] = a[index] + b[index];
    }

    // Execute on GPU
    public static void AddArraysOnGpu(float[] a, float[] b, float[] result)
    {
        using var context = Context.CreateDefault();
        using var accelerator = context.GetPreferredDevice(false)
            .CreateAccelerator(context);

        using var aBuffer = accelerator.Allocate1D(a);
        using var bBuffer = accelerator.Allocate1D(b);
        using var resultBuffer = accelerator.Allocate1D<float>(result.Length);

        var kernel = accelerator.LoadAutoGroupedStreamKernel<
            Index1D, ArrayView<float>, ArrayView<float>, ArrayView<float>>(
            AddArraysKernel);

        kernel((int)aBuffer.Length, aBuffer.View, bBuffer.View, resultBuffer.View);

        accelerator.Synchronize();

        resultBuffer.CopyToCPU(result);
    }
}
*/
```

### Compute Shaders (DirectX/Vulkan)

For more advanced scenarios, consider:
- **DirectX Compute Shaders** (Windows)
- **Vulkan Compute** (Cross-platform)
- **Metal** (macOS/iOS)
- **OpenCL** (Cross-platform)

## Benchmarking

### BenchmarkDotNet

```csharp
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

[MemoryDiagnoser]
[SimpleJob(BenchmarkDotNet.Jobs.RuntimeMoniker.Net80)]
public class PerformanceBenchmarks
{
    private float[] _arrayA;
    private float[] _arrayB;
    private float[] _result;

    [Params(100, 1000, 10000)]
    public int Size { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        _arrayA = Enumerable.Range(0, Size).Select(x => (float)x).ToArray();
        _arrayB = Enumerable.Range(0, Size).Select(x => (float)x * 2).ToArray();
        _result = new float[Size];
    }

    [Benchmark(Baseline = true)]
    public void Scalar()
    {
        for (int i = 0; i < _arrayA.Length; i++)
        {
            _result[i] = _arrayA[i] + _arrayB[i];
        }
    }

    [Benchmark]
    public void Simd()
    {
        VectorOperations.AddArrays(_arrayA, _arrayB, _result);
    }

    [Benchmark]
    public void Parallel()
    {
        System.Threading.Tasks.Parallel.For(0, _arrayA.Length, i =>
        {
            _result[i] = _arrayA[i] + _arrayB[i];
        });
    }
}

// Run benchmarks
// BenchmarkRunner.Run<PerformanceBenchmarks>();
```

## Best Practices

### SIMD

1. **Use Vector<T> First**: Hardware-agnostic, easier to use
2. **Handle Remainders**: Always process leftover elements
3. **Alignment**: Consider memory alignment for best performance
4. **Benchmarking**: Always measure - SIMD isn't always faster for small data
5. **Fallback**: Provide scalar fallback for unsupported hardware

### Parallel Processing

1. **Measure First**: Parallelization has overhead
2. **Right Granularity**: Too fine-grained = overhead, too coarse = poor distribution
3. **Avoid Locks**: Use thread-local storage and aggregate
4. **Cancellation**: Support CancellationToken
5. **Exception Handling**: Handle exceptions in parallel code

### Memory Optimization

1. **Use Span<T>**: Avoid substring/slice allocations
2. **ArrayPool**: Reuse arrays for temporary buffers
3. **MemoryPool**: For async scenarios
4. **Stack Allocation**: Small, fixed-size buffers
5. **Measure Allocations**: Use memory profiler or [MemoryDiagnoser]

## Performance Checklist

- [ ] Profile before optimizing
- [ ] Use SIMD for data-parallel operations
- [ ] Parallelize CPU-bound work
- [ ] Use async streams for I/O-bound streaming
- [ ] Minimize allocations with Span<T>
- [ ] Pool temporary buffers
- [ ] Benchmark with realistic data sizes
- [ ] Consider Native AOT for startup/memory
- [ ] Monitor GC pressure
- [ ] Use value types where appropriate

## Resources

- [SIMD in .NET](https://learn.microsoft.com/en-us/dotnet/standard/simd)
- [Parallel Programming](https://learn.microsoft.com/en-us/dotnet/standard/parallel-programming/)
- [Async Streams](https://learn.microsoft.com/en-us/dotnet/csharp/whats-new/tutorials/generate-consume-asynchronous-stream)
- [Memory and Span](https://learn.microsoft.com/en-us/dotnet/standard/memory-and-spans/)
- [BenchmarkDotNet](https://benchmarkdotnet.org/)

---

**Last Updated:** 2025-12-01
**Version:** 1.0
