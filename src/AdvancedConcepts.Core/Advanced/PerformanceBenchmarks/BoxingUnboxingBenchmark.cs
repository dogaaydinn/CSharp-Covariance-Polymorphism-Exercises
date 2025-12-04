using System.Collections;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;

namespace AdvancedCsharpConcepts.Advanced.PerformanceBenchmarks;

/// <summary>
/// High-performance benchmark comparing boxing/unboxing vs generic collections.
/// Demonstrates NVIDIA-style performance optimization thinking.
/// </summary>
[MemoryDiagnoser]
[Orderer(SummaryOrderPolicy.FastestToSlowest)]
[RankColumn]
public class BoxingUnboxingBenchmark
{
    private const int IterationCount = 10000;
    private readonly List<int> _genericList = new(IterationCount);
    private readonly ArrayList _arrayList = new(IterationCount);

    [GlobalSetup]
    public void Setup()
    {
        for (var i = 0; i < IterationCount; i++)
        {
            _genericList.Add(i);
            _arrayList.Add(i); // Boxing occurs here
        }
    }

    /// <summary>
    /// Baseline: Generic List with no boxing overhead.
    /// Expected: ~5-10x faster than ArrayList with minimal allocations.
    /// </summary>
    [Benchmark(Baseline = true)]
    public long GenericListSum()
    {
        long sum = 0;
        foreach (var value in _genericList)
        {
            sum += value; // No unboxing
        }
        return sum;
    }

    /// <summary>
    /// Boxing/Unboxing overhead demonstration.
    /// Expected: Slower due to heap allocations and type conversions.
    /// </summary>
    [Benchmark]
    public long ArrayListSum()
    {
        long sum = 0;
        foreach (var value in _arrayList)
        {
            sum += (int)value!; // Unboxing occurs here - heap allocation overhead
        }
        return sum;
    }

    /// <summary>
    /// Modern C# pattern using Span&lt;T&gt; for zero-allocation iteration.
    /// Expected: Fastest approach with stack-only allocations.
    /// </summary>
    [Benchmark]
    public long SpanSum()
    {
        long sum = 0;
        var span = System.Runtime.InteropServices.CollectionsMarshal.AsSpan(_genericList);
        foreach (var value in span)
        {
            sum += value;
        }
        return sum;
    }

    /// <summary>
    /// Manual span iteration for reference comparison.
    /// </summary>
    [Benchmark]
    public long ManualSpanSum()
    {
        var span = System.Runtime.InteropServices.CollectionsMarshal.AsSpan(_genericList);
        long sum = 0;
        for (var i = 0; i < span.Length; i++)
        {
            sum += span[i];
        }
        return sum;
    }
}
