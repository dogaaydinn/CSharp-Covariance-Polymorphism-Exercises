using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Exporters.Csv;

namespace AdvancedConcepts.Benchmarks;

/// <summary>
/// Benchmark Runner for Advanced C# Concepts.
/// NVIDIA-style performance benchmarking with detailed diagnostics.
/// </summary>
public class Program
{
    public static void Main(string[] args)
    {
        var config = DefaultConfig.Instance
            .AddDiagnoser(MemoryDiagnoser.Default)
            .AddExporter(HtmlExporter.Default)
            .AddExporter(MarkdownExporter.GitHub)
            .AddExporter(CsvExporter.Default);

        // Run all benchmarks or specific ones
        if (args.Length > 0 && args[0] == "all")
        {
            BenchmarkRunner.Run<BoxingBenchmarks>(config);
            BenchmarkRunner.Run<PolymorphismBenchmarks>(config);
            BenchmarkRunner.Run<LinqBenchmarks>(config);
            BenchmarkRunner.Run<SpanBenchmarks>(config);
            BenchmarkRunner.Run<TypeConversionBenchmarks>(config);
        }
        else
        {
            // Use BenchmarkSwitcher for interactive selection
            var switcher = BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly);
            switcher.Run(args, config);
        }
    }
}
