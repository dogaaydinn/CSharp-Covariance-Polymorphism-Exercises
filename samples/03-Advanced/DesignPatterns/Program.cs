using DesignPatterns.Creational;
using DesignPatterns.Structural;
using DesignPatterns.Behavioral;

namespace DesignPatterns;

/// <summary>
/// Design Patterns Tutorial - Comprehensive guide to Gang of Four patterns in C#
///
/// This tutorial demonstrates 9 essential design patterns across three categories:
/// - Creational Patterns: Object creation mechanisms
/// - Structural Patterns: Object composition and relationships
/// - Behavioral Patterns: Object interaction and responsibility distribution
///
/// Each pattern includes:
/// - Problem statement and motivation
/// - UML structure diagram
/// - Complete implementation
/// - Real-world examples
/// - Best practices and trade-offs
/// </summary>
class Program
{
    static void Main(string[] args)
    {
        PrintHeader("DESIGN PATTERNS TUTORIAL", '=');
        Console.WriteLine("  A comprehensive guide to Gang of Four design patterns");
        Console.WriteLine("  Demonstrating 9 essential patterns with practical examples");
        Console.WriteLine();

        // Creational Patterns - Focus on object creation
        PrintHeader("PART 1: CREATIONAL PATTERNS", '=');
        Console.WriteLine("  Patterns that deal with object creation mechanisms");
        Console.WriteLine("  Goal: Increase flexibility and reuse of existing code");
        Console.WriteLine();

        RunPattern(1, SingletonExample.Run);
        RunPattern(2, FactoryExample.Run);
        RunPattern(3, BuilderExample.Run);

        // Structural Patterns - Focus on object composition
        Console.WriteLine();
        PrintHeader("PART 2: STRUCTURAL PATTERNS", '=');
        Console.WriteLine("  Patterns that deal with object composition and relationships");
        Console.WriteLine("  Goal: Build flexible and efficient class structures");
        Console.WriteLine();

        RunPattern(4, DecoratorExample.Run);
        RunPattern(5, AdapterExample.Run);
        RunPattern(6, ProxyExample.Run);

        // Behavioral Patterns - Focus on object interaction
        Console.WriteLine();
        PrintHeader("PART 3: BEHAVIORAL PATTERNS", '=');
        Console.WriteLine("  Patterns that deal with object interaction and responsibility");
        Console.WriteLine("  Goal: Define clear communication between objects");
        Console.WriteLine();

        RunPattern(7, StrategyExample.Run);
        RunPattern(8, ObserverExample.Run);
        RunPattern(9, ChainOfResponsibilityExample.Run);

        // Summary
        Console.WriteLine();
        PrintHeader("TUTORIAL COMPLETE!", '=');
        Console.WriteLine();
        Console.WriteLine("  Summary of Patterns Demonstrated:");
        Console.WriteLine();

        PrintSummarySection("Creational Patterns (3)", new[]
        {
            "1. Singleton - Ensures only one instance exists",
            "2. Factory - Creates objects without specifying exact class",
            "3. Builder - Constructs complex objects step by step"
        });

        PrintSummarySection("Structural Patterns (3)", new[]
        {
            "4. Decorator - Adds behavior to objects dynamically",
            "5. Adapter - Makes incompatible interfaces work together",
            "6. Proxy - Controls access to objects"
        });

        PrintSummarySection("Behavioral Patterns (3)", new[]
        {
            "7. Strategy - Defines family of interchangeable algorithms",
            "8. Observer - Notifies multiple objects of state changes",
            "9. Chain of Responsibility - Passes requests through handler chain"
        });

        Console.WriteLine();
        PrintFooter();
    }

    /// <summary>
    /// Runs a pattern example with error handling and timing
    /// </summary>
    private static void RunPattern(int number, Action patternExample)
    {
        try
        {
            var startTime = DateTime.Now;
            patternExample();
            var elapsed = DateTime.Now - startTime;
            Console.WriteLine($"  [Completed in {elapsed.TotalMilliseconds:F0}ms]");
            Console.WriteLine();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"  [Error] Pattern {number} failed: {ex.Message}");
            Console.WriteLine();
        }
    }

    /// <summary>
    /// Prints a header with the specified title and border character
    /// </summary>
    private static void PrintHeader(string title, char borderChar)
    {
        const int width = 75;
        Console.WriteLine(new string(borderChar, width));
        Console.WriteLine($"  {title}");
        Console.WriteLine(new string(borderChar, width));
    }

    /// <summary>
    /// Prints a summary section with title and items
    /// </summary>
    private static void PrintSummarySection(string title, string[] items)
    {
        Console.WriteLine($"  {title}:");
        foreach (var item in items)
        {
            Console.WriteLine($"    {item}");
        }
        Console.WriteLine();
    }

    /// <summary>
    /// Prints the footer with learning resources
    /// </summary>
    private static void PrintFooter()
    {
        Console.WriteLine("  Next Steps:");
        Console.WriteLine("    - Review the pattern implementations in the source code");
        Console.WriteLine("    - Explore when to use each pattern (see README.md)");
        Console.WriteLine("    - Try implementing patterns in your own projects");
        Console.WriteLine("    - Study trade-offs and alternatives for each pattern");
        Console.WriteLine();
        Console.WriteLine("  Remember:");
        Console.WriteLine("    - Patterns are tools, not rules");
        Console.WriteLine("    - Use patterns to solve real problems, not for pattern's sake");
        Console.WriteLine("    - Prefer simple solutions over complex patterns when possible");
        Console.WriteLine("    - Understand the problem before applying a pattern");
        Console.WriteLine();
        Console.WriteLine(new string('=', 75));
        Console.WriteLine("  Thank you for completing the Design Patterns Tutorial!");
        Console.WriteLine(new string('=', 75));
    }
}
