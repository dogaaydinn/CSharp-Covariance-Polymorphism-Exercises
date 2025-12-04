using System;
using AdvancedConcepts.Samples.CovarianceContravariance.Examples;

namespace AdvancedConcepts.Samples.CovarianceContravariance;

/// <summary>
/// Main program demonstrating covariance and contravariance in C#.
/// </summary>
/// <remarks>
/// This tutorial covers:
/// - Covariance (out T): Using more derived types in output positions
/// - Contravariance (in T): Using more generic types in input positions
/// - Invariance: When no variance is allowed (both input and output)
/// - Real-world patterns: Repository, Validator, Specification patterns
///
/// KEY CONCEPTS:
/// - Covariance: "I produce Dogs, treat me as producing Animals" (safe upcast)
/// - Contravariance: "I consume Animals, I can consume Dogs" (safe parameter widening)
/// - Invariance: "I both produce and consume, must use exact type" (type safety)
/// </remarks>
class Program
{
    static void Main(string[] args)
    {
        PrintHeader();

        // Example 1: Covariance (out T)
        RunExample(
            number: 1,
            title: "Covariance - IEnumerable<out T>, Func<out T>",
            action: CovarianceExample.Run
        );

        // Example 2: Contravariance (in T)
        RunExample(
            number: 2,
            title: "Contravariance - IComparer<in T>, Action<in T>",
            action: ContravarianceExample.Run
        );

        // Example 3: Invariance (no variance)
        RunExample(
            number: 3,
            title: "Invariance - IList<T> Limitations",
            action: InvarianceExample.Run
        );

        // Example 4: Real-World Patterns
        RunExample(
            number: 4,
            title: "Real-World Repository Pattern with Variance",
            action: RealWorldExample.Run
        );

        PrintFooter();
    }

    /// <summary>
    /// Prints the tutorial header.
    /// </summary>
    private static void PrintHeader()
    {
        if (IsInteractive())
        {
            Console.Clear();
        }

        Console.WriteLine("╔" + "═".PadRight(68, '═') + "╗");
        Console.WriteLine("║" + "  COVARIANCE & CONTRAVARIANCE TUTORIAL".PadRight(68) + "║");
        Console.WriteLine("║" + "  Understanding Generic Variance in C#".PadRight(68) + "║");
        Console.WriteLine("╚" + "═".PadRight(68, '═') + "╝");
        Console.WriteLine();
        Console.WriteLine("What you'll learn:");
        Console.WriteLine("  • Covariance (out T) - Using more derived types in outputs");
        Console.WriteLine("  • Contravariance (in T) - Using more generic types in inputs");
        Console.WriteLine("  • Invariance - When and why variance isn't allowed");
        Console.WriteLine("  • Real-world patterns leveraging variance");
        Console.WriteLine();

        if (IsInteractive())
        {
            Console.WriteLine("Press any key to start...");
            Console.ReadKey();
            Console.Clear();
        }
        else
        {
            Console.WriteLine("Running in non-interactive mode...");
            Console.WriteLine();
        }
    }

    /// <summary>
    /// Prints the tutorial footer.
    /// </summary>
    private static void PrintFooter()
    {
        Console.WriteLine();
        Console.WriteLine("╔" + "═".PadRight(68, '═') + "╗");
        Console.WriteLine("║" + "  Tutorial Complete!".PadRight(68) + "║");
        Console.WriteLine("╚" + "═".PadRight(68, '═') + "╝");
        Console.WriteLine();
        Console.WriteLine("🎯 Key Takeaways:");
        Console.WriteLine();
        Console.WriteLine("  COVARIANCE (out T):");
        Console.WriteLine("    ✓ Use when T appears only in OUTPUT positions (return values)");
        Console.WriteLine("    ✓ Allows IEnumerable<Dog> → IEnumerable<Animal>");
        Console.WriteLine("    ✓ Examples: IEnumerable<out T>, Func<out TResult>, Task<out T>");
        Console.WriteLine();
        Console.WriteLine("  CONTRAVARIANCE (in T):");
        Console.WriteLine("    ✓ Use when T appears only in INPUT positions (parameters)");
        Console.WriteLine("    ✓ Allows IComparer<Animal> → IComparer<Dog>");
        Console.WriteLine("    ✓ Examples: IComparer<in T>, Action<in T>, IValidator<in T>");
        Console.WriteLine();
        Console.WriteLine("  INVARIANCE (no variance):");
        Console.WriteLine("    ✓ Required when T appears in BOTH input and output");
        Console.WriteLine("    ✓ Prevents type safety violations");
        Console.WriteLine("    ✓ Examples: IList<T>, ICollection<T>, IDictionary<K,V>");
        Console.WriteLine();
        Console.WriteLine("📚 Memory Aid - PECS Principle:");
        Console.WriteLine("    Producer  → out → Covariant");
        Console.WriteLine("    Consumer  → in  → Contravariant");
        Console.WriteLine();
        Console.WriteLine("🚀 Next Steps:");
        Console.WriteLine("    • Explore src/AdvancedConcepts.Core/Advanced/GenericCovarianceContravariance/");
        Console.WriteLine("    • Read more: https://learn.microsoft.com/en-us/dotnet/csharp/programming-guide/concepts/covariance-contravariance/");
        Console.WriteLine("    • Try the BoxingPerformance sample next!");
        Console.WriteLine();

        if (IsInteractive())
        {
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }

    /// <summary>
    /// Runs a single example with formatted output.
    /// </summary>
    /// <param name="number">Example number.</param>
    /// <param name="title">Example title.</param>
    /// <param name="action">Action to execute.</param>
    private static void RunExample(int number, string title, Action action)
    {
        Console.WriteLine("╔" + "═".PadRight(68, '═') + "╗");
        Console.WriteLine($"║  Example {number}: {title}".PadRight(69) + "║");
        Console.WriteLine("╚" + "═".PadRight(68, '═') + "╝");
        Console.WriteLine();

        try
        {
            action();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error: {ex.Message}");
        }

        Console.WriteLine();
        Console.WriteLine("─".PadRight(70, '─'));
        Console.WriteLine();

        if (IsInteractive())
        {
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
            Console.Clear();
        }
        else
        {
            Console.WriteLine();
        }
    }

    /// <summary>
    /// Determines if the console is interactive (not redirected).
    /// </summary>
    /// <returns>True if interactive, false otherwise.</returns>
    private static bool IsInteractive()
    {
        try
        {
            return !Console.IsInputRedirected && !Console.IsOutputRedirected;
        }
        catch
        {
            return false;
        }
    }
}
