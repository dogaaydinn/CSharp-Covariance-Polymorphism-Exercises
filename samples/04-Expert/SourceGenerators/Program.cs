using System;
using SourceGenerators.Examples;

namespace SourceGenerators;

/// <summary>
/// Source Generators Demo - Compile-Time Code Generation
///
/// This sample demonstrates Roslyn Source Generators - a powerful .NET feature that
/// generates code at compile-time, eliminating runtime overhead while maintaining
/// clean, declarative syntax.
///
/// What You'll Learn:
/// 1. AutoMapGenerator - Zero-overhead DTO mapping
/// 2. LoggerMessageGenerator - High-performance structured logging
/// 3. ValidationGenerator - Compile-time validation code generation
///
/// Key Benefits:
/// - Zero runtime reflection overhead
/// - Compile-time type safety
/// - Performance equivalent to hand-written code
/// - Clean, maintainable attribute-based API
/// </summary>
class Program
{
    static void Main(string[] args)
    {
        PrintHeader();

        bool runAll = args.Length == 0 || args[0] == "all";

        if (runAll || args.Contains("intro"))
        {
            PrintIntroduction();
        }

        if (runAll || args.Contains("automap"))
        {
            RunSection("AutoMap Generator - Automatic DTO Mapping", AutoMapExample.Run);
        }

        if (runAll || args.Contains("logger"))
        {
            RunSection("LoggerMessage Generator - High-Performance Logging", LoggerExample.Run);
        }

        if (runAll)
        {
            PrintSummary();
            PrintGeneratedFilesLocation();
        }

        PrintFooter();
    }

    private static void PrintHeader()
    {
        Console.Clear();
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("╔" + "═".PadRight(78, '═') + "╗");
        Console.WriteLine("║" + CenterText("SOURCE GENERATORS DEMO", 78) + "║");
        Console.WriteLine("║" + CenterText("Compile-Time Code Generation with Roslyn", 78) + "║");
        Console.WriteLine("╚" + "═".PadRight(78, '═') + "╝");
        Console.ResetColor();
        Console.WriteLine();
    }

    private static void PrintIntroduction()
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("WHAT ARE SOURCE GENERATORS?");
        Console.ResetColor();
        Console.WriteLine("─".PadRight(80, '─'));
        Console.WriteLine();

        Console.WriteLine("Source generators are programs that run during compilation and generate");
        Console.WriteLine("additional C# source code that becomes part of your assembly.");
        Console.WriteLine();

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("Traditional Approach (Reflection):");
        Console.ResetColor();
        Console.WriteLine("  var dto = mapper.Map<UserDto>(user);  // Runtime overhead, slow");
        Console.WriteLine();

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("Source Generator Approach:");
        Console.ResetColor();
        Console.WriteLine("  var dto = user.ToUserDto();  // Generated method, FAST, zero overhead");
        Console.WriteLine();

        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("HOW IT WORKS:");
        Console.ResetColor();
        Console.WriteLine();
        Console.WriteLine("  ┌──────────────┐");
        Console.WriteLine("  │  Your Code   │  1. You write code with attributes");
        Console.WriteLine("  │  + Attributes│     [AutoMap(typeof(UserDto))]");
        Console.WriteLine("  └──────┬───────┘");
        Console.WriteLine("         │");
        Console.WriteLine("         ▼");
        Console.WriteLine("  ┌──────────────────┐");
        Console.WriteLine("  │ Roslyn Compiler  │  2. Compiler analyzes your code");
        Console.WriteLine("  └──────┬───────────┘");
        Console.WriteLine("         │");
        Console.WriteLine("         ▼");
        Console.WriteLine("  ┌─────────────────────────┐");
        Console.WriteLine("  │ Source Generators Run   │  3. Generators create C# code");
        Console.WriteLine("  │ - AutoMapGenerator      │");
        Console.WriteLine("  │ - LoggerMessageGenerator│");
        Console.WriteLine("  │ - ValidationGenerator   │");
        Console.WriteLine("  └──────┬──────────────────┘");
        Console.WriteLine("         │");
        Console.WriteLine("         ▼");
        Console.WriteLine("  ┌──────────────────┐");
        Console.WriteLine("  │ Generated Code   │  4. Generated code is compiled");
        Console.WriteLine("  │ - *.g.cs files   │     with your code");
        Console.WriteLine("  └──────┬───────────┘");
        Console.WriteLine("         │");
        Console.WriteLine("         ▼");
        Console.WriteLine("  ┌──────────────────┐");
        Console.WriteLine("  │ Final Assembly   │  5. Single optimized assembly");
        Console.WriteLine("  │ (Your App.dll)   │");
        Console.WriteLine("  └──────────────────┘");
        Console.WriteLine();

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("KEY BENEFITS:");
        Console.ResetColor();
        Console.WriteLine("  ✓ Zero runtime overhead (no reflection)");
        Console.WriteLine("  ✓ Compile-time type safety");
        Console.WriteLine("  ✓ IntelliSense support for generated code");
        Console.WriteLine("  ✓ Performance equal to hand-written code");
        Console.WriteLine("  ✓ Clean, declarative attribute-based API");
        Console.WriteLine();

        Console.WriteLine("Press any key to continue...");
        Console.ReadKey(true);
        Console.Clear();
        Console.WriteLine();
    }

    private static void RunSection(string title, Action action)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("╔" + "═".PadRight(78, '═') + "╗");
        Console.WriteLine("║" + CenterText(title, 78) + "║");
        Console.WriteLine("╚" + "═".PadRight(78, '═') + "╝");
        Console.ResetColor();
        Console.WriteLine();

        try
        {
            action();
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Error: {ex.Message}");
            Console.ResetColor();
        }

        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine("─".PadRight(80, '─'));
        Console.ResetColor();
        Console.WriteLine();
    }

    private static void PrintSummary()
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("╔" + "═".PadRight(78, '═') + "╗");
        Console.WriteLine("║" + CenterText("SUMMARY - Source Generators Benefits", 78) + "║");
        Console.WriteLine("╚" + "═".PadRight(78, '═') + "╝");
        Console.ResetColor();
        Console.WriteLine();

        PrintSummaryItem(
            "AutoMap Generator",
            "Zero-allocation DTO mapping",
            "As fast as manual mapping, no reflection overhead");

        PrintSummaryItem(
            "LoggerMessage Generator",
            "High-performance logging",
            "6x faster than string interpolation, zero allocations");

        Console.WriteLine();

        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("WHEN TO USE SOURCE GENERATORS:");
        Console.ResetColor();
        Console.WriteLine();
        Console.WriteLine("  ✓ Performance-critical paths");
        Console.WriteLine("  ✓ Eliminating boilerplate code");
        Console.WriteLine("  ✓ Compile-time code generation");
        Console.WriteLine("  ✓ Reducing runtime reflection");
        Console.WriteLine("  ✓ Domain-specific languages (DSLs)");
        Console.WriteLine();

        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("PERFORMANCE RESULTS:");
        Console.ResetColor();
        Console.WriteLine();
        Console.WriteLine("  AutoMap:        Equivalent to manual mapping (0-5% difference)");
        Console.WriteLine("  LoggerMessage:  6x faster than string interpolation");
        Console.WriteLine();
        Console.WriteLine("  All with ZERO runtime reflection overhead!");
        Console.WriteLine();
    }

    private static void PrintSummaryItem(string name, string description, string benefit)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.Write("  ✓ ");
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine(name);
        Console.ResetColor();
        Console.WriteLine($"    Description: {description}");
        Console.WriteLine($"    Benefit:     {benefit}");
        Console.WriteLine();
    }

    private static void PrintGeneratedFilesLocation()
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("WHERE TO FIND GENERATED CODE:");
        Console.ResetColor();
        Console.WriteLine("─".PadRight(80, '─'));
        Console.WriteLine();

        var basePath = "obj/Debug/net8.0/generated/AdvancedConcepts.SourceGenerators";

        Console.WriteLine("Generated files are located in:");
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"  {basePath}/");
        Console.ResetColor();
        Console.WriteLine();

        Console.WriteLine("Files you can inspect:");
        Console.WriteLine($"  • AutoMapAttribute.g.cs          - AutoMap attribute definitions");
        Console.WriteLine($"  • User_AutoMap.g.cs              - User mapping extensions");
        Console.WriteLine($"  • Order_AutoMap.g.cs             - Order mapping extensions");
        Console.WriteLine($"  • Product_AutoMap.g.cs           - Product mapping extensions");
        Console.WriteLine($"  • AppLogs.g.cs                   - Generated logging methods (MS generator)");
        Console.WriteLine();

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("TIP: Open these files to see exactly what code was generated!");
        Console.ResetColor();
        Console.WriteLine();

        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("HOW TO VIEW GENERATED CODE:");
        Console.ResetColor();
        Console.WriteLine();
        Console.WriteLine("Option 1 - Enable output to source folder:");
        Console.WriteLine("  Add to .csproj:");
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine("  <PropertyGroup>");
        Console.WriteLine("    <EmitCompilerGeneratedFiles>true</EmitCompilerGeneratedFiles>");
        Console.WriteLine("    <CompilerGeneratedFilesOutputPath>$(BaseIntermediateOutputPath)Generated</CompilerGeneratedFilesOutputPath>");
        Console.WriteLine("  </PropertyGroup>");
        Console.ResetColor();
        Console.WriteLine();

        Console.WriteLine("Option 2 - View in obj folder:");
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine("  cat obj/Debug/net8.0/generated/.../User_AutoMap.g.cs");
        Console.ResetColor();
        Console.WriteLine();

        Console.WriteLine("Option 3 - Debug the generator:");
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine("  export DOTNET_CLI_DEBUG_GENERATOR=1");
        Console.WriteLine("  dotnet build");
        Console.ResetColor();
        Console.WriteLine();
    }

    private static void PrintFooter()
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("╔" + "═".PadRight(78, '═') + "╗");
        Console.WriteLine("║" + CenterText("Demo Complete!", 78) + "║");
        Console.WriteLine("╚" + "═".PadRight(78, '═') + "╝");
        Console.ResetColor();
        Console.WriteLine();

        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("NEXT STEPS:");
        Console.ResetColor();
        Console.WriteLine();
        Console.WriteLine("  1. Inspect the generated code files (see locations above)");
        Console.WriteLine("  2. Examine the source generator implementations:");
        Console.WriteLine("     • src/AdvancedConcepts.SourceGenerators/AutoMapGenerator.cs");
        Console.WriteLine("     • src/AdvancedConcepts.SourceGenerators/LoggerMessageGenerator.cs");
        Console.WriteLine("     • src/AdvancedConcepts.SourceGenerators/ValidationGenerator.cs");
        Console.WriteLine();
        Console.WriteLine("  3. Try creating your own source generator");
        Console.WriteLine("  4. Read the README.md for more details");
        Console.WriteLine();

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("USEFUL COMMANDS:");
        Console.ResetColor();
        Console.WriteLine();
        Console.WriteLine("  Run specific examples:");
        Console.WriteLine("    dotnet run -- automap       # Only AutoMap examples");
        Console.WriteLine("    dotnet run -- logger        # Only Logger examples");
        Console.WriteLine();
        Console.WriteLine("  Build and inspect:");
        Console.WriteLine("    dotnet clean");
        Console.WriteLine("    dotnet build");
        Console.WriteLine("    cat obj/Debug/net8.0/generated/.../User_AutoMap.g.cs");
        Console.WriteLine();

        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("FURTHER READING:");
        Console.ResetColor();
        Console.WriteLine();
        Console.WriteLine("  • Source Generators Cookbook:");
        Console.WriteLine("    https://github.com/dotnet/roslyn/blob/main/docs/features/source-generators.cookbook.md");
        Console.WriteLine();
        Console.WriteLine("  • Incremental Generators:");
        Console.WriteLine("    https://github.com/dotnet/roslyn/blob/main/docs/features/incremental-generators.md");
        Console.WriteLine();
        Console.WriteLine("  • LoggerMessage Attribute:");
        Console.WriteLine("    https://learn.microsoft.com/en-us/dotnet/core/extensions/logger-message-generator");
        Console.WriteLine();
    }

    private static string CenterText(string text, int width)
    {
        if (text.Length >= width)
            return text;

        int padding = (width - text.Length) / 2;
        return text.PadLeft(text.Length + padding).PadRight(width);
    }

}

// Extension methods must be in a non-generic static class
internal static class ArrayExtensions
{
    public static bool Contains(this string[] array, string value)
    {
        return Array.IndexOf(array, value) >= 0;
    }
}
