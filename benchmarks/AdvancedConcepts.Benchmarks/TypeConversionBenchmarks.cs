namespace AdvancedConcepts.Benchmarks;

/// <summary>
/// Type Conversion Performance Benchmarks.
/// NVIDIA-style: Cast vs As vs Is vs Pattern Matching.
/// </summary>
[MemoryDiagnoser]
[RankColumn]
public class TypeConversionBenchmarks
{
    private readonly object[] _objects;
    private const int Size = 1000;

    public TypeConversionBenchmarks()
    {
        _objects = new object[Size];
        for (var i = 0; i < Size; i++)
        {
            _objects[i] = i % 2 == 0 ? (object)i : (object)i.ToString();
        }
    }

    [Benchmark(Baseline = true)]
    public int DirectCastWithTryCatch()
    {
        var sum = 0;
        foreach (var obj in _objects)
        {
            try
            {
                sum += (int)obj;
            }
            catch
            {
                // Ignore
            }
        }
        return sum;
    }

    [Benchmark]
    public int AsOperator()
    {
        var sum = 0;
        foreach (var obj in _objects)
        {
            var value = obj as int?;
            if (value.HasValue)
            {
                sum += value.Value;
            }
        }
        return sum;
    }

    [Benchmark]
    public int IsOperatorWithCast()
    {
        var sum = 0;
        foreach (var obj in _objects)
        {
            if (obj is int)
            {
                sum += (int)obj;
            }
        }
        return sum;
    }

    [Benchmark]
    public int PatternMatchingWithDeclaration()
    {
        var sum = 0;
        foreach (var obj in _objects)
        {
            if (obj is int value)
            {
                sum += value;
            }
        }
        return sum;
    }

    [Benchmark]
    public int SwitchExpression()
    {
        var sum = 0;
        foreach (var obj in _objects)
        {
            sum += obj switch
            {
                int i => i,
                _ => 0
            };
        }
        return sum;
    }

    [Benchmark]
    public int TypeCheckWithGetType()
    {
        var sum = 0;
        foreach (var obj in _objects)
        {
            if (obj.GetType() == typeof(int))
            {
                sum += (int)obj;
            }
        }
        return sum;
    }
}
