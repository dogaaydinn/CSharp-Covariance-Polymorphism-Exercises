namespace AdvancedConcepts.Benchmarks;

/// <summary>
/// Polymorphism Performance Benchmarks.
/// NVIDIA-style: Virtual method dispatch vs sealed class optimization.
/// </summary>
[MemoryDiagnoser]
[RankColumn]
public class PolymorphismBenchmarks
{
    private const int Iterations = 10000;
    private readonly BaseClass _baseInstance;
    private readonly SealedClass _sealedInstance;
    private readonly ICalculator _interfaceInstance;

    public PolymorphismBenchmarks()
    {
        _baseInstance = new DerivedClass();
        _sealedInstance = new SealedClass();
        _interfaceInstance = new ConcreteCalculator();
    }

    [Benchmark(Baseline = true)]
    public int VirtualMethodCall()
    {
        var sum = 0;
        for (var i = 0; i < Iterations; i++)
        {
            sum += _baseInstance.Calculate(i);
        }
        return sum;
    }

    [Benchmark]
    public int SealedMethodCall()
    {
        var sum = 0;
        for (var i = 0; i < Iterations; i++)
        {
            sum += _sealedInstance.Calculate(i);
        }
        return sum;
    }

    [Benchmark]
    public int InterfaceMethodCall()
    {
        var sum = 0;
        for (var i = 0; i < Iterations; i++)
        {
            sum += _interfaceInstance.Calculate(i);
        }
        return sum;
    }

    [Benchmark]
    public int DirectMethodCall()
    {
        var instance = new SealedClass();
        var sum = 0;
        for (var i = 0; i < Iterations; i++)
        {
            sum += instance.Calculate(i);
        }
        return sum;
    }

    // Test classes
    public abstract class BaseClass
    {
        public virtual int Calculate(int value) => value * 2;
    }

    public class DerivedClass : BaseClass
    {
        public override int Calculate(int value) => value * 2;
    }

    public sealed class SealedClass
    {
        public int Calculate(int value) => value * 2;
    }

    public interface ICalculator
    {
        int Calculate(int value);
    }

    public class ConcreteCalculator : ICalculator
    {
        public int Calculate(int value) => value * 2;
    }
}
