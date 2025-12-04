namespace AdvancedConcepts.Benchmarks;

/// <summary>
/// Span&lt;T&gt; Performance Benchmarks.
/// NVIDIA-style: Zero-allocation slicing and memory efficiency.
/// </summary>
[MemoryDiagnoser]
[RankColumn]
public class SpanBenchmarks
{
    private readonly int[] _data;
    private const int Size = 10000;

    public SpanBenchmarks()
    {
        _data = Enumerable.Range(1, Size).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int ArraySlicingWithCopy()
    {
        var slice = new int[_data.Length / 2];
        Array.Copy(_data, 0, slice, 0, slice.Length);

        var sum = 0;
        for (var i = 0; i < slice.Length; i++)
        {
            sum += slice[i];
        }
        return sum;
    }

    [Benchmark]
    public int SpanSlicingZeroAlloc()
    {
        var span = _data.AsSpan(0, _data.Length / 2);

        var sum = 0;
        for (var i = 0; i < span.Length; i++)
        {
            sum += span[i];
        }
        return sum;
    }

    [Benchmark]
    public int ArrayReversal()
    {
        var copy = new int[_data.Length];
        Array.Copy(_data, copy, _data.Length);
        Array.Reverse(copy);

        var sum = 0;
        for (var i = 0; i < copy.Length; i++)
        {
            sum += copy[i];
        }
        return sum;
    }

    [Benchmark]
    public int SpanReversal()
    {
        Span<int> span = stackalloc int[_data.Length];
        _data.AsSpan().CopyTo(span);
        span.Reverse();

        var sum = 0;
        for (var i = 0; i < span.Length; i++)
        {
            sum += span[i];
        }
        return sum;
    }

    [Benchmark]
    public int ArraySearch()
    {
        var count = 0;
        for (var i = 0; i < 100; i++)
        {
            var index = Array.IndexOf(_data, 5000);
            if (index >= 0)
                count++;
        }
        return count;
    }

    [Benchmark]
    public int SpanSearch()
    {
        var count = 0;
        for (var i = 0; i < 100; i++)
        {
            var span = _data.AsSpan();
            var index = span.IndexOf(5000);
            if (index >= 0)
                count++;
        }
        return count;
    }

    [Benchmark]
    public bool ArrayEquals()
    {
        var copy = new int[_data.Length];
        Array.Copy(_data, copy, _data.Length);
        return _data.SequenceEqual(copy);
    }

    [Benchmark]
    public bool SpanEquals()
    {
        Span<int> copy = stackalloc int[_data.Length];
        _data.AsSpan().CopyTo(copy);
        return _data.AsSpan().SequenceEqual(copy);
    }
}
