namespace AdvancedConcepts.Benchmarks;

/// <summary>
/// Boxing vs Generics Performance Benchmarks.
/// NVIDIA-style: Measure heap allocation and GC pressure.
/// </summary>
[MemoryDiagnoser]
[RankColumn]
public class BoxingBenchmarks
{
    private const int Iterations = 1000;
    private readonly int[] _data;

    public BoxingBenchmarks()
    {
        _data = Enumerable.Range(1, Iterations).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int BoxingWithObject()
    {
        var sum = 0;
        foreach (var item in _data)
        {
            object boxed = item; // Boxing allocation
            sum += (int)boxed;    // Unboxing
        }
        return sum;
    }

    [Benchmark]
    public int GenericWithoutBoxing()
    {
        var sum = 0;
        foreach (var item in _data)
        {
            sum += AddGeneric(item); // No boxing
        }
        return sum;
    }

    [Benchmark]
    public int ArrayListWithBoxing()
    {
        var list = new System.Collections.ArrayList();
        foreach (var item in _data)
        {
            list.Add(item); // Boxing on every add
        }

        var sum = 0;
        foreach (int item in list) // Unboxing on every read
        {
            sum += item;
        }
        return sum;
    }

    [Benchmark]
    public int GenericListWithoutBoxing()
    {
        var list = new List<int>();
        foreach (var item in _data)
        {
            list.Add(item); // No boxing
        }

        var sum = 0;
        foreach (var item in list) // No unboxing
        {
            sum += item;
        }
        return sum;
    }

    private T AddGeneric<T>(T value) => value;
}
