using System;
using System.Numerics;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;
using System.Threading;

namespace AdvancedPerformance;

/// <summary>
/// Advanced Performance Tutorial - Expert-Level Optimization
/// SIMD, Hardware Intrinsics, Lock-Free Programming, CPU Cache Optimization
/// </summary>
class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("═══════════════════════════════════════════════════════════════════");
        Console.WriteLine("ADVANCED PERFORMANCE TUTORIAL");
        Console.WriteLine("Expert-Level Optimization Techniques");
        Console.WriteLine("═══════════════════════════════════════════════════════════════════\n");

        CheckHardwareSupport();
        SIMDBasics();
        ParallelOptimization();
        LockFreeProgramming();
        CacheOptimization();
        BestPractices();

        Console.WriteLine("\n\n═══════════════════════════════════════════════════════════════════");
        Console.WriteLine("Tutorial Complete!");
        Console.WriteLine("═══════════════════════════════════════════════════════════════════");
    }

    #region 1. Hardware Support Check

    static void CheckHardwareSupport()
    {
        Console.WriteLine("\n1. HARDWARE SUPPORT");
        Console.WriteLine("─────────────────────────────────────────────────────────────────\n");

        Console.WriteLine("Checking CPU capabilities...\n");

        // Vector<T> support (always available on 64-bit)
        Console.WriteLine($"Vector<T> Support:");
        Console.WriteLine($"  Vector.IsHardwareAccelerated: {Vector.IsHardwareAccelerated}");
        Console.WriteLine($"  Vector<float>.Count: {Vector<float>.Count} (SIMD lanes)");
        Console.WriteLine($"  Vector<int>.Count: {Vector<int>.Count}");

        // Hardware intrinsics
        Console.WriteLine($"\nHardware Intrinsics:");
        Console.WriteLine($"  SSE:    {Sse.IsSupported}");
        Console.WriteLine($"  SSE2:   {Sse2.IsSupported}");
        Console.WriteLine($"  SSE3:   {Sse3.IsSupported}");
        Console.WriteLine($"  AVX:    {Avx.IsSupported}");
        Console.WriteLine($"  AVX2:   {Avx2.IsSupported}");

        Console.WriteLine("\nKey Concepts:");
        Console.WriteLine("  SIMD: Single Instruction, Multiple Data");
        Console.WriteLine("  Processes multiple values in one CPU instruction");
        Console.WriteLine("  Example: Add 8 floats in one operation (8x faster!)");
    }

    #endregion

    #region 2. SIMD Basics

    static void SIMDBasics()
    {
        Console.WriteLine("\n\n2. SIMD VECTORIZATION");
        Console.WriteLine("─────────────────────────────────────────────────────────────────\n");

        const int size = 1000;
        float[] a = new float[size];
        float[] b = new float[size];
        float[] result = new float[size];

        // Initialize arrays
        for (int i = 0; i < size; i++)
        {
            a[i] = i;
            b[i] = i * 2;
        }

        // ❌ Scalar: Process one element at a time
        Console.WriteLine("❌ Scalar Addition (one at a time):");
        var sw = System.Diagnostics.Stopwatch.StartNew();
        for (int i = 0; i < size; i++)
        {
            result[i] = a[i] + b[i];  // 1 addition per iteration
        }
        sw.Stop();
        Console.WriteLine($"  Time: {sw.Elapsed.TotalMicroseconds:F2} μs");
        Console.WriteLine($"  Operations: {size} additions");

        // ✅ SIMD: Process multiple elements at once
        Console.WriteLine("\n✅ SIMD Addition (8 at a time with AVX):");
        sw.Restart();
        int vectorSize = Vector<float>.Count;  // Usually 4 or 8
        int i2 = 0;
        
        // Process in chunks
        for (; i2 <= size - vectorSize; i2 += vectorSize)
        {
            var va = new Vector<float>(a, i2);
            var vb = new Vector<float>(b, i2);
            var vr = va + vb;  // Adds 8 floats in ONE CPU instruction!
            vr.CopyTo(result, i2);
        }
        
        // Handle remainder
        for (; i2 < size; i2++)
        {
            result[i2] = a[i2] + b[i2];
        }
        
        sw.Stop();
        Console.WriteLine($"  Time: {sw.Elapsed.TotalMicroseconds:F2} μs");
        Console.WriteLine($"  Speedup: ~{1000.0 * sw.Elapsed.TotalMicroseconds / sw.Elapsed.TotalMicroseconds:F1}x faster");
        Console.WriteLine($"  Operations: {size / vectorSize} vector ops + {size % vectorSize} scalar ops");

        Console.WriteLine("\nKey Points:");
        Console.WriteLine("  ✓ SIMD processes multiple values simultaneously");
        Console.WriteLine($"  ✓ Vector<float>.Count = {vectorSize} (depends on CPU)");
        Console.WriteLine("  ✓ Best for: Math operations, image processing, physics");
        Console.WriteLine("  ✗ Not all operations can be vectorized");
    }

    #endregion

    #region 3. Parallel Optimization

    static void ParallelOptimization()
    {
        Console.WriteLine("\n\n3. PARALLEL OPTIMIZATION");
        Console.WriteLine("─────────────────────────────────────────────────────────────────\n");

        const int size = 10_000_000;
        int[] data = new int[size];
        for (int i = 0; i < size; i++) data[i] = i;

        // ❌ Sequential
        Console.WriteLine("❌ Sequential Processing:");
        var sw = System.Diagnostics.Stopwatch.StartNew();
        long sum1 = 0;
        for (int i = 0; i < size; i++)
        {
            sum1 += data[i];
        }
        sw.Stop();
        Console.WriteLine($"  Time: {sw.ElapsedMilliseconds}ms");
        Console.WriteLine($"  Sum: {sum1}");

        // ✅ Parallel.For (naive)
        Console.WriteLine("\n⚠️  Parallel.For (naive - slow!):");
        sw.Restart();
        long sum2 = 0;
        Parallel.For(0, size, i =>
        {
            Interlocked.Add(ref sum2, data[i]);  // Bottleneck: lock contention!
        });
        sw.Stop();
        Console.WriteLine($"  Time: {sw.ElapsedMilliseconds}ms (SLOWER!)");
        Console.WriteLine($"  Problem: Lock contention on every operation");

        // ✅ Parallel.For (optimized with thread-local)
        Console.WriteLine("\n✅ Parallel.For (optimized):");
        sw.Restart();
        long sum3 = 0;
        object lockObj = new object();
        Parallel.For(0, size,
            () => 0L,  // Thread-local initial value
            (i, loop, subtotal) =>
            {
                return subtotal + data[i];  // No locking!
            },
            (subtotal) =>
            {
                lock (lockObj)
                {
                    sum3 += subtotal;  // Lock only once per thread
                }
            });
        sw.Stop();
        Console.WriteLine($"  Time: {sw.ElapsedMilliseconds}ms");
        Console.WriteLine($"  Sum: {sum3}");

        Console.WriteLine("\nKey Points:");
        Console.WriteLine("  ✓ Use thread-local accumulation");
        Console.WriteLine("  ✓ Reduce lock contention");
        Console.WriteLine("  ✓ Batch operations per thread");
        Console.WriteLine("  ✗ Don't parallelize small work items");
    }

    #endregion

    #region 4. Lock-Free Programming

    static void LockFreeProgramming()
    {
        Console.WriteLine("\n\n4. LOCK-FREE PROGRAMMING");
        Console.WriteLine("─────────────────────────────────────────────────────────────────\n");

        Console.WriteLine("Lock-free data structures avoid traditional locks.\n");

        // Counter example
        int counter = 0;
        const int iterations = 1_000_000;

        // ❌ With lock (slow)
        Console.WriteLine("❌ With lock:");
        object lockObj = new object();
        var sw = System.Diagnostics.Stopwatch.StartNew();
        Parallel.For(0, iterations, i =>
        {
            lock (lockObj)
            {
                counter++;  // Safe but slow
            }
        });
        sw.Stop();
        Console.WriteLine($"  Time: {sw.ElapsedMilliseconds}ms");
        Console.WriteLine($"  Counter: {counter}");

        // ✅ Lock-free with Interlocked (fast)
        Console.WriteLine("\n✅ Lock-free with Interlocked:");
        counter = 0;
        sw.Restart();
        Parallel.For(0, iterations, i =>
        {
            Interlocked.Increment(ref counter);  // Atomic, no lock!
        });
        sw.Stop();
        Console.WriteLine($"  Time: {sw.ElapsedMilliseconds}ms");
        Console.WriteLine($"  Counter: {counter}");

        Console.WriteLine("\nInterlocked Operations:");
        Console.WriteLine("  Interlocked.Increment()     - Atomic ++");
        Console.WriteLine("  Interlocked.Add()           - Atomic +=");
        Console.WriteLine("  Interlocked.Exchange()      - Atomic assignment");
        Console.WriteLine("  Interlocked.CompareExchange() - CAS (Compare-And-Swap)");

        Console.WriteLine("\nKey Points:");
        Console.WriteLine("  ✓ Lock-free = no thread blocking");
        Console.WriteLine("  ✓ Uses CPU atomic operations");
        Console.WriteLine("  ✓ Better throughput under contention");
        Console.WriteLine("  ✗ More complex to implement correctly");
    }

    #endregion

    #region 5. Cache Optimization

    static void CacheOptimization()
    {
        Console.WriteLine("\n\n5. CPU CACHE OPTIMIZATION");
        Console.WriteLine("─────────────────────────────────────────────────────────────────\n");

        const int size = 10000;
        int[,] matrix = new int[size, size];

        Console.WriteLine("CPU Cache Hierarchy:");
        Console.WriteLine("  L1 Cache: ~32KB, ~4 cycles");
        Console.WriteLine("  L2 Cache: ~256KB, ~12 cycles");
        Console.WriteLine("  L3 Cache: ~8MB, ~40 cycles");
        Console.WriteLine("  RAM:      ~16GB, ~100+ cycles");
        Console.WriteLine("\nCache line: 64 bytes (16 ints)");

        // ❌ Cache-unfriendly (column-major)
        Console.WriteLine("\n❌ Cache-unfriendly (column-major access):");
        var sw = System.Diagnostics.Stopwatch.StartNew();
        for (int col = 0; col < 100; col++)
        {
            for (int row = 0; row < size; row++)
            {
                matrix[row, col] = row + col;  // Jumps memory, cache misses!
            }
        }
        sw.Stop();
        Console.WriteLine($"  Time: {sw.ElapsedMilliseconds}ms");
        Console.WriteLine("  Problem: Each access loads new cache line");

        // ✅ Cache-friendly (row-major)
        Console.WriteLine("\n✅ Cache-friendly (row-major access):");
        sw.Restart();
        for (int row = 0; row < size; row++)
        {
            for (int col = 0; col < 100; col++)
            {
                matrix[row, col] = row + col;  // Sequential, cache hits!
            }
        }
        sw.Stop();
        Console.WriteLine($"  Time: {sw.ElapsedMilliseconds}ms");
        Console.WriteLine("  Benefit: Sequential access = better cache usage");

        Console.WriteLine("\nCache Optimization Tips:");
        Console.WriteLine("  ✓ Access memory sequentially");
        Console.WriteLine("  ✓ Keep data structures small (<L1 cache if possible)");
        Console.WriteLine("  ✓ Avoid false sharing (align to cache line)");
        Console.WriteLine("  ✓ Use structs for small, frequently accessed data");
        Console.WriteLine("  ✗ Don't jump around memory randomly");
    }

    #endregion

    #region 6. Best Practices

    static void BestPractices()
    {
        Console.WriteLine("\n\n6. BEST PRACTICES");
        Console.WriteLine("─────────────────────────────────────────────────────────────────\n");

        Console.WriteLine("✅ DO:");
        Console.WriteLine("  • Profile first (dotnet-trace, PerfView)");
        Console.WriteLine("  • Measure with BenchmarkDotNet");
        Console.WriteLine("  • Optimize hot paths only (80/20 rule)");
        Console.WriteLine("  • Use SIMD for math-heavy operations");
        Console.WriteLine("  • Use Parallel.For for CPU-bound work");
        Console.WriteLine("  • Use Interlocked for simple atomic ops");
        Console.WriteLine("  • Access memory sequentially");

        Console.WriteLine("\n❌ DON'T:");
        Console.WriteLine("  • Don't optimize without measuring");
        Console.WriteLine("  • Don't parallelize small operations");
        Console.WriteLine("  • Don't use locks for simple counters");
        Console.WriteLine("  • Don't ignore CPU cache behavior");
        Console.WriteLine("  • Don't use SIMD everywhere (overhead!)");

        Console.WriteLine("\n🎯 When to Use Each:");
        Console.WriteLine("\nSIMD:");
        Console.WriteLine("  • Math operations (add, multiply, etc.)");
        Console.WriteLine("  • Image/video processing");
        Console.WriteLine("  • Physics simulations");
        Console.WriteLine("  • Large arrays (>1000 elements)");

        Console.WriteLine("\nParallel:");
        Console.WriteLine("  • CPU-bound independent operations");
        Console.WriteLine("  • Work items > 1ms each");
        Console.WriteLine("  • More work than CPU cores");

        Console.WriteLine("\nLock-Free:");
        Console.WriteLine("  • High-contention scenarios");
        Console.WriteLine("  • Simple atomic operations");
        Console.WriteLine("  • Performance-critical sections");

        Console.WriteLine("\n📊 Typical Performance Gains:");
        Console.WriteLine("  SIMD:       4-8x faster (vectorization)");
        Console.WriteLine("  Parallel:   2-4x faster (multi-core)");
        Console.WriteLine("  Lock-Free:  2-10x faster (no blocking)");
        Console.WriteLine("  Cache:      10-100x faster (L1 vs RAM)");

        Console.WriteLine("\n⚠️  Warnings:");
        Console.WriteLine("  • SIMD code is platform-specific");
        Console.WriteLine("  • Parallel has overhead (thread creation)");
        Console.WriteLine("  • Lock-free is hard to get right");
        Console.WriteLine("  • Always measure in production-like scenarios");
    }

    #endregion
}
