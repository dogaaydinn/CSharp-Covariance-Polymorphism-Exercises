namespace AdvancedCsharpConcepts.Advanced.ModernCSharp;

/// <summary>
/// Modern C# Collection Patterns - Concise, performant syntax.
/// NVIDIA developer mindset: write clean, performant code.
/// Note: C# 12 collection expressions require .NET 8+
/// This example shows C# 10/11 modern patterns.
/// </summary>
public class CollectionExpressionsExample
{
    /// <summary>
    /// Traditional collection initialization.
    /// </summary>
    public static void TraditionalCollections()
    {
        // Traditional array
        int[] numbersArray = new int[] { 1, 2, 3, 4, 5 };

        // Traditional list
        List<string> namesList = new List<string> { "Alice", "Bob", "Charlie" };

        // Traditional concatenation - verbose
        var combined = new List<int>(numbersArray);
        combined.AddRange(new[] { 6, 7, 8 });

        Console.WriteLine("Traditional: " + string.Join(", ", combined));
    }

    /// <summary>
    /// Modern C# 10/11 collection initialization - cleaner syntax.
    /// Note: Using target-typed new and collection initializers.
    /// </summary>
    public static void ModernCollections()
    {
        // Modern array with target-typed new (C# 9+)
        int[] numbers = new[] { 1, 2, 3, 4, 5 };

        // Modern list with target-typed new
        List<string> names = new() { "Alice", "Bob", "Charlie" };

        // Modern concatenation with LINQ
        int[] combined = numbers.Concat(new[] { 6, 7, 8 }).ToArray();

        Console.WriteLine("Modern: " + string.Join(", ", combined));
    }

    /// <summary>
    /// Advanced pattern: combining collections with LINQ (C# 10/11).
    /// </summary>
    public static void AdvancedCombining()
    {
        int[] first = new[] { 1, 2, 3 };
        int[] second = new[] { 4, 5, 6 };
        int[] third = new[] { 7, 8, 9 };

        // LINQ for combining multiple collections
        int[] all = first.Concat(second).Concat(third).ToArray();

        // Mix literal values with collections
        int[] withLiterals = new[] { 0 }
            .Concat(first)
            .Concat(new[] { 10 })
            .Concat(second)
            .Concat(new[] { 20 })
            .Concat(third)
            .Concat(new[] { 30 })
            .ToArray();

        Console.WriteLine("Combined: " + string.Join(", ", all));
        Console.WriteLine("With literals: " + string.Join(", ", withLiterals));
    }

    /// <summary>
    /// Performance comparison: Different initialization patterns.
    /// Target-typed new reduces verbosity while maintaining performance.
    /// </summary>
    public static void PerformanceOptimized()
    {
        // Traditional - multiple allocations
        var traditionalList = new List<int>();
        traditionalList.Add(1);
        traditionalList.Add(2);
        traditionalList.Add(3);
        var traditionalArray = traditionalList.ToArray();

        // Modern - single allocation with collection initializer
        int[] modernArray = new[] { 1, 2, 3 };

        Console.WriteLine($"Traditional result: {string.Join(", ", traditionalArray)}");
        Console.WriteLine($"Modern result: {string.Join(", ", modernArray)}");
    }

    /// <summary>
    /// Real-world example: filtering and combining with LINQ.
    /// </summary>
    public static void RealWorldExample()
    {
        // Collection initialization
        var numbers = new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };

        // Filter evens and odds using LINQ
        var evens = numbers.Where(n => n % 2 == 0).ToArray();
        var odds = numbers.Where(n => n % 2 != 0).ToArray();

        // Combine back with separator using LINQ
        int[] separated = evens.Concat(new[] { 0 }).Concat(odds).ToArray();

        Console.WriteLine($"Evens: {string.Join(", ", evens)}");
        Console.WriteLine($"Odds: {string.Join(", ", odds)}");
        Console.WriteLine($"Separated by 0: {string.Join(", ", separated)}");
    }

    /// <summary>
    /// Demonstrates modern collection patterns (C# 10/11).
    /// </summary>
    public static void RunExample()
    {
        Console.WriteLine("\n=== C# 10/11 Modern Collection Patterns ===\n");

        Console.WriteLine("1. Traditional vs Modern:");
        TraditionalCollections();
        ModernCollections();

        Console.WriteLine("\n2. Advanced Combining:");
        AdvancedCombining();

        Console.WriteLine("\n3. Performance Optimized:");
        PerformanceOptimized();

        Console.WriteLine("\n4. Real-World Example:");
        RealWorldExample();
    }
}
