using BoxingPerformance.Examples;

namespace BoxingPerformance;

/// <summary>
/// Boxing and Unboxing Performance - Comprehensive Tutorial
/// Demonstrates the performance impact of boxing/unboxing and strategies to avoid it.
/// </summary>
/// <remarks>
/// This sample covers:
/// 1. Boxing Basics - What boxing is and when it happens
/// 2. Performance Comparison - ArrayList vs List&lt;T&gt; benchmarks
/// 3. Avoiding Boxing - Strategies to eliminate boxing
/// 4. Real World Scenarios - Common production pitfalls
///
/// Key Takeaways:
/// - Boxing allocates on heap (slow, causes GC pressure)
/// - Unboxing requires type checking (slow, can throw exceptions)
/// - Generic collections avoid boxing entirely
/// - Boxing in hot paths can degrade performance by 5-10x
/// - Modern C# provides many tools to avoid boxing
/// </remarks>
class Program
{
    static void Main(string[] args)
    {
        PrintHeader();

        if (args.Length > 0)
        {
            RunSpecificExample(args[0]);
        }
        else
        {
            RunAllExamples();
        }

        PrintFooter();
    }

    private static void RunAllExamples()
    {
        Console.WriteLine("Running all examples...");
        Console.WriteLine();
        Console.WriteLine("=".PadRight(80, '='));
        Console.WriteLine();

        // 1. Boxing Basics
        RunSection("1. BOXING BASICS", "Understanding boxing and unboxing mechanics", () =>
        {
            BoxingBasics.RunAll();
        });

        // 2. Performance Comparison
        RunSection("2. PERFORMANCE COMPARISON", "Measuring the real performance impact", () =>
        {
            PerformanceComparison.RunAll();
        });

        // 3. Avoiding Boxing
        RunSection("3. AVOIDING BOXING", "Strategies and best practices", () =>
        {
            AvoidingBoxing.RunAll();
        });

        // 4. Real World Scenarios
        RunSection("4. REAL WORLD SCENARIOS", "Common production pitfalls and solutions", () =>
        {
            RealWorldScenarios.RunAll();
        });
    }

    private static void RunSpecificExample(string exampleName)
    {
        Console.WriteLine($"Running example: {exampleName}");
        Console.WriteLine();
        Console.WriteLine("=".PadRight(80, '='));
        Console.WriteLine();

        switch (exampleName.ToLowerInvariant())
        {
            case "basics":
            case "1":
                RunSection("BOXING BASICS", "Understanding boxing and unboxing", () =>
                {
                    BoxingBasics.RunAll();
                });
                break;

            case "performance":
            case "2":
                RunSection("PERFORMANCE COMPARISON", "Benchmarks and measurements", () =>
                {
                    PerformanceComparison.RunAll();
                });
                break;

            case "avoiding":
            case "3":
                RunSection("AVOIDING BOXING", "Prevention strategies", () =>
                {
                    AvoidingBoxing.RunAll();
                });
                break;

            case "realworld":
            case "scenarios":
            case "4":
                RunSection("REAL WORLD SCENARIOS", "Production examples", () =>
                {
                    RealWorldScenarios.RunAll();
                });
                break;

            // Individual demos
            case "basic-boxing":
                BoxingBasics.DemonstrateBasicBoxing();
                break;

            case "basic-unboxing":
                BoxingBasics.DemonstrateBasicUnboxing();
                break;

            case "implicit":
                BoxingBasics.DemonstrateImplicitBoxing();
                break;

            case "stack-heap":
                BoxingBasics.DemonstrateStackVsHeap();
                break;

            case "measure":
                BoxingBasics.MeasureBoxingPerformance();
                break;

            case "arraylist-list":
                PerformanceComparison.CompareArrayListVsListPerformance();
                break;

            case "hashtable-dictionary":
                PerformanceComparison.CompareHashtableVsDictionaryPerformance();
                break;

            case "hotpath":
                PerformanceComparison.DemonstrateHotPathBoxing();
                break;

            case "struct-boxing":
                PerformanceComparison.CompareStructBoxingScenarios();
                break;

            case "generic-collections":
                AvoidingBoxing.UseGenericCollections();
                break;

            case "generic-methods":
                AvoidingBoxing.UseGenericMethods();
                break;

            case "stringbuilder":
                AvoidingBoxing.UseStringBuilderCorrectly();
                break;

            case "tostring":
                AvoidingBoxing.OverrideToStringInStructs();
                break;

            case "readonly-struct":
                AvoidingBoxing.UseReadonlyStructs();
                break;

            case "interfaces":
                AvoidingBoxing.AvoidInterfaceCasts();
                break;

            case "constraints":
                AvoidingBoxing.UseGenericConstraints();
                break;

            case "detect":
                AvoidingBoxing.DetectBoxingAtCompileTime();
                break;

            case "legacy":
                RealWorldScenarios.LegacyCollectionMigration();
                break;

            case "logging":
                RealWorldScenarios.LoggingFrameworkBoxing();
                break;

            case "linq":
                RealWorldScenarios.LinqBoxingPitfalls();
                break;

            case "strings":
                RealWorldScenarios.StringBuildingInHotPath();
                break;

            case "events":
                RealWorldScenarios.EventHandlerBoxing();
                break;

            case "reflection":
                RealWorldScenarios.ReflectionBoxing();
                break;

            case "database":
                RealWorldScenarios.DatabaseParameterBoxing();
                break;

            case "initialization":
                RealWorldScenarios.CollectionInitializationBoxing();
                break;

            default:
                Console.WriteLine($"Unknown example: {exampleName}");
                Console.WriteLine();
                PrintUsage();
                break;
        }
    }

    private static void RunSection(string title, string description, Action action)
    {
        Console.WriteLine($"### {title} ###");
        Console.WriteLine($"    {description}");
        Console.WriteLine();
        Console.WriteLine("-".PadRight(80, '-'));
        Console.WriteLine();

        try
        {
            action();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"ERROR: {ex.Message}");
            Console.WriteLine(ex.StackTrace);
        }

        Console.WriteLine();
        Console.WriteLine("=".PadRight(80, '='));
        Console.WriteLine();
    }

    private static void PrintHeader()
    {
        Console.WriteLine("=".PadRight(80, '='));
        Console.WriteLine("BOXING AND UNBOXING PERFORMANCE - COMPREHENSIVE TUTORIAL");
        Console.WriteLine("=".PadRight(80, '='));
        Console.WriteLine();
        Console.WriteLine("Understanding and Eliminating Boxing Overhead in C#");
        Console.WriteLine();
        Console.WriteLine("Topics Covered:");
        Console.WriteLine("  1. Boxing Basics - Fundamentals and mechanics");
        Console.WriteLine("  2. Performance Comparison - Real benchmarks and measurements");
        Console.WriteLine("  3. Avoiding Boxing - Strategies and best practices");
        Console.WriteLine("  4. Real World Scenarios - Production pitfalls and solutions");
        Console.WriteLine();
    }

    private static void PrintFooter()
    {
        Console.WriteLine();
        Console.WriteLine("=".PadRight(80, '='));
        Console.WriteLine("Tutorial Complete!");
        Console.WriteLine("=".PadRight(80, '='));
        Console.WriteLine();
        Console.WriteLine("KEY TAKEAWAYS:");
        Console.WriteLine("  - Always use generic collections (List<T>, Dictionary<K,V>)");
        Console.WriteLine("  - Avoid boxing in hot paths (tight loops)");
        Console.WriteLine("  - Override ToString() in public structs");
        Console.WriteLine("  - Use generic methods and constraints");
        Console.WriteLine("  - Profile with tools like BenchmarkDotNet and PerfView");
        Console.WriteLine("  - Boxing can cause 5-10x performance degradation");
        Console.WriteLine();
        Console.WriteLine("For more information, see:");
        Console.WriteLine("  - README.md in this directory");
        Console.WriteLine("  - Microsoft Docs: Value Types and Boxing");
        Console.WriteLine("  - .NET Performance Best Practices");
        Console.WriteLine();
    }

    private static void PrintUsage()
    {
        Console.WriteLine("USAGE:");
        Console.WriteLine("  dotnet run                    Run all examples");
        Console.WriteLine("  dotnet run <example-name>     Run specific example");
        Console.WriteLine();
        Console.WriteLine("SECTION EXAMPLES:");
        Console.WriteLine("  basics, 1                     Boxing basics");
        Console.WriteLine("  performance, 2                Performance comparisons");
        Console.WriteLine("  avoiding, 3                   Avoiding boxing strategies");
        Console.WriteLine("  realworld, scenarios, 4       Real-world scenarios");
        Console.WriteLine();
        Console.WriteLine("INDIVIDUAL DEMOS:");
        Console.WriteLine();
        Console.WriteLine("Boxing Basics:");
        Console.WriteLine("  basic-boxing                  Basic boxing demonstration");
        Console.WriteLine("  basic-unboxing                Basic unboxing demonstration");
        Console.WriteLine("  implicit                      Implicit boxing scenarios");
        Console.WriteLine("  stack-heap                    Stack vs heap allocation");
        Console.WriteLine("  measure                       Performance measurement");
        Console.WriteLine();
        Console.WriteLine("Performance Comparison:");
        Console.WriteLine("  arraylist-list                ArrayList vs List<int>");
        Console.WriteLine("  hashtable-dictionary          Hashtable vs Dictionary<K,V>");
        Console.WriteLine("  hotpath                       Hot path boxing impact");
        Console.WriteLine("  struct-boxing                 Struct boxing scenarios");
        Console.WriteLine();
        Console.WriteLine("Avoiding Boxing:");
        Console.WriteLine("  generic-collections           Use generic collections");
        Console.WriteLine("  generic-methods               Use generic methods");
        Console.WriteLine("  stringbuilder                 StringBuilder techniques");
        Console.WriteLine("  tostring                      Override ToString()");
        Console.WriteLine("  readonly-struct               Use readonly struct");
        Console.WriteLine("  interfaces                    Avoid interface casts");
        Console.WriteLine("  constraints                   Generic constraints");
        Console.WriteLine("  detect                        Detect boxing at compile-time");
        Console.WriteLine();
        Console.WriteLine("Real World Scenarios:");
        Console.WriteLine("  legacy                        Legacy collection migration");
        Console.WriteLine("  logging                       Logging framework boxing");
        Console.WriteLine("  linq                          LINQ boxing pitfalls");
        Console.WriteLine("  strings                       String building in hot paths");
        Console.WriteLine("  events                        Event handler boxing");
        Console.WriteLine("  reflection                    Reflection boxing");
        Console.WriteLine("  database                      Database parameter boxing");
        Console.WriteLine("  initialization                Collection initialization");
        Console.WriteLine();
    }
}
