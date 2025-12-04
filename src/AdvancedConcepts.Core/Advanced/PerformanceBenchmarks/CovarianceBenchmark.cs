using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using AdvancedCsharpConcepts.Beginner.Polymorphism_AssignCompatibility;

namespace AdvancedCsharpConcepts.Advanced.PerformanceBenchmarks;

/// <summary>
/// Benchmark comparing different covariance/contravariance patterns.
/// Silicon Valley best practices: measure before optimizing.
/// </summary>
[MemoryDiagnoser]
[Orderer(SummaryOrderPolicy.FastestToSlowest)]
public class CovarianceBenchmark
{
    private readonly List<Cat> _cats = new();
    private readonly Cat[] _catArray = new Cat[1000];
    private const int IterationCount = 1000;

    [GlobalSetup]
    public void Setup()
    {
        for (var i = 0; i < IterationCount; i++)
        {
            var cat = new Cat();
            _cats.Add(cat);
            _catArray[i] = cat;
        }
    }

    /// <summary>
    /// Direct generic list iteration (no variance conversion).
    /// Expected: Baseline performance, no overhead.
    /// </summary>
    [Benchmark(Baseline = true)]
    public int DirectIteration()
    {
        var count = 0;
        foreach (var cat in _cats)
        {
            cat.Speak();
            count++;
        }
        return count;
    }

    /// <summary>
    /// Covariant IEnumerable conversion.
    /// Expected: Minimal overhead due to interface virtualization.
    /// </summary>
    [Benchmark]
    public int CovariantEnumerable()
    {
        IEnumerable<Animal> animals = _cats;
        var count = 0;
        foreach (var animal in animals)
        {
            animal.Speak();
            count++;
        }
        return count;
    }

    /// <summary>
    /// Array covariance (older pattern, has runtime type checking overhead).
    /// Expected: Slower due to runtime type safety checks.
    /// </summary>
    [Benchmark]
    public int ArrayCovariance()
    {
        Animal[] animals = _catArray;
        var count = 0;
        foreach (var animal in animals)
        {
            animal.Speak();
            count++;
        }
        return count;
    }

    /// <summary>
    /// Modern pattern using Span&lt;T&gt; with ref structs (zero-allocation).
    /// Expected: Fastest approach with no heap allocations.
    /// </summary>
    [Benchmark]
    public int SpanIteration()
    {
        var span = System.Runtime.InteropServices.CollectionsMarshal.AsSpan(_cats);
        var count = 0;
        foreach (var cat in span)
        {
            cat.Speak();
            count++;
        }
        return count;
    }

    /// <summary>
    /// Parallel processing for large datasets (NVIDIA-style parallelism).
    /// Expected: Best for CPU-bound operations on multi-core systems.
    /// </summary>
    [Benchmark]
    public int ParallelIteration()
    {
        var count = 0;
        Parallel.ForEach(_cats, cat =>
        {
            cat.Speak();
            Interlocked.Increment(ref count);
        });
        return count;
    }
}
