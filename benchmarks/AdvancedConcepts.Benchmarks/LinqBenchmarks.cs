namespace AdvancedConcepts.Benchmarks;

/// <summary>
/// LINQ Performance Benchmarks.
/// NVIDIA-style: LINQ vs For Loop vs Optimized implementations.
/// </summary>
[MemoryDiagnoser]
[RankColumn]
public class LinqBenchmarks
{
    private readonly int[] _data;
    private const int Size = 10000;

    public LinqBenchmarks()
    {
        _data = Enumerable.Range(1, Size).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int LinqWhere()
    {
        return _data.Where(x => x % 2 == 0).Sum();
    }

    [Benchmark]
    public int ForLoopEquivalent()
    {
        var sum = 0;
        for (var i = 0; i < _data.Length; i++)
        {
            if (_data[i] % 2 == 0)
            {
                sum += _data[i];
            }
        }
        return sum;
    }

    [Benchmark]
    public int ForeachEquivalent()
    {
        var sum = 0;
        foreach (var item in _data)
        {
            if (item % 2 == 0)
            {
                sum += item;
            }
        }
        return sum;
    }

    [Benchmark]
    public List<int> LinqSelect()
    {
        return _data.Select(x => x * 2).ToList();
    }

    [Benchmark]
    public List<int> ForLoopSelect()
    {
        var result = new List<int>(_data.Length);
        for (var i = 0; i < _data.Length; i++)
        {
            result.Add(_data[i] * 2);
        }
        return result;
    }

    [Benchmark]
    public int LinqAny()
    {
        var count = 0;
        for (var i = 0; i < 100; i++)
        {
            if (_data.Any(x => x > 5000))
                count++;
        }
        return count;
    }

    [Benchmark]
    public int ForLoopAny()
    {
        var count = 0;
        for (var i = 0; i < 100; i++)
        {
            var found = false;
            for (var j = 0; j < _data.Length; j++)
            {
                if (_data[j] > 5000)
                {
                    found = true;
                    break;
                }
            }
            if (found)
                count++;
        }
        return count;
    }

    [Benchmark]
    public int LinqOrderBy()
    {
        return _data.OrderBy(x => x).First();
    }

    [Benchmark]
    public int ManualMin()
    {
        var min = int.MaxValue;
        for (var i = 0; i < _data.Length; i++)
        {
            if (_data[i] < min)
                min = _data[i];
        }
        return min;
    }
}
