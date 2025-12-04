namespace AdvancedCsharpConcepts.Advanced.ModernCSharp;

/// <summary>
/// Advanced Pattern Matching in C# 12.
/// Demonstrates type patterns, property patterns, list patterns, and more.
/// Silicon Valley best practice: expressive code that's also performant.
/// </summary>
public class AdvancedPatternMatching
{
    // Base classes for demonstration
    public abstract record Shape;
    public record Circle(double Radius) : Shape;
    public record Rectangle(double Width, double Height) : Shape;
    public record Triangle(double Base, double Height) : Shape;

    /// <summary>
    /// Type pattern matching with switch expressions (C# 8+).
    /// </summary>
    public static double CalculateArea(Shape shape) => shape switch
    {
        Circle { Radius: var r } => Math.PI * r * r,
        Rectangle { Width: var w, Height: var h } => w * h,
        Triangle { Base: var b, Height: var h } => 0.5 * b * h,
        _ => throw new ArgumentException("Unknown shape", nameof(shape))
    };

    /// <summary>
    /// Property pattern matching with when clauses.
    /// Advanced: combining patterns for complex business logic.
    /// </summary>
    public static string ClassifyShape(Shape shape) => shape switch
    {
        Circle { Radius: > 10 } => "Large Circle",
        Circle { Radius: > 5 } => "Medium Circle",
        Circle => "Small Circle",
        Rectangle { Width: var w, Height: var h } when w == h => "Square",
        Rectangle { Width: > 10, Height: > 10 } => "Large Rectangle",
        Rectangle => "Small Rectangle",
        Triangle { Base: var b, Height: var h } when b == h => "Isosceles-like Triangle",
        Triangle => "Triangle",
        _ => "Unknown"
    };

    /// <summary>
    /// Array length-based patterns (C# 10 compatible).
    /// Note: C# 11 list patterns are more concise but require .NET 7+
    /// </summary>
    public static string AnalyzeSequence(int[] numbers) => numbers.Length switch
    {
        0 => "Empty sequence",
        1 => $"Single element: {numbers[0]}",
        2 => $"Two elements: {numbers[0]}, {numbers[1]}",
        _ => $"Multiple elements, first: {numbers[0]}, last: {numbers[^1]}"
    };

    /// <summary>
    /// Sequence validation using property patterns (C# 10).
    /// </summary>
    public static bool IsValidSequence(int[] numbers)
    {
        if (numbers.Length >= 3 && numbers[0] == 1 && numbers[1] == 2 && numbers[2] == 3)
            return true;
        if (numbers.Length >= 3 && numbers[^3] == 8 && numbers[^2] == 9 && numbers[^1] == 10)
            return true;
        if (numbers.Length >= 2 && numbers[0] == 1 && numbers[^1] == 10)
            return true;
        return false;
    }

    /// <summary>
    /// Relational and logical patterns (C# 9+).
    /// Clean, expressive code for range checking.
    /// </summary>
    public static string ClassifyAge(int age) => age switch
    {
        < 0 => "Invalid age",
        >= 0 and < 13 => "Child",
        >= 13 and < 20 => "Teenager",
        >= 20 and < 65 => "Adult",
        _ => "Senior" // >= 65
    };

    /// <summary>
    /// Relational patterns for validation.
    /// </summary>
    public static bool IsValidScore(int score) => score >= 0 && score <= 100;

    public static string EvaluateScore(int score)
    {
        if (score < 0 || score > 100)
            return "Invalid score";

        return score switch
        {
            >= 90 => "A",
            >= 80 => "B",
            >= 70 => "C",
            >= 60 => "D",
            _ => "F"
        };
    }

    /// <summary>
    /// Recursive pattern matching with nested properties.
    /// </summary>
    public record Person(string Name, int Age, Address? Address);
    public record Address(string City, string Country);

    public static bool IsFromSiliconValley(Person person) => person switch
    {
        { Address: { City: "San Jose" or "Palo Alto" or "Mountain View", Country: "USA" } } => true,
        { Address: { City: "Cupertino" or "Sunnyvale" or "Santa Clara", Country: "USA" } } => true,
        _ => false
    };

    /// <summary>
    /// Performance optimization: pattern matching vs if-else chains.
    /// Pattern matching compiles to efficient jump tables for better performance.
    /// </summary>
    public static string OptimizedClassification(int value)
    {
        // Compiler optimizes this to a jump table for O(1) lookup
        return value switch
        {
            1 => "One",
            2 => "Two",
            3 => "Three",
            4 => "Four",
            5 => "Five",
            >= 6 and <= 10 => "Six to Ten",
            > 10 and <= 100 => "Eleven to Hundred",
            _ => "Other"
        };
    }

    /// <summary>
    /// Demonstrates all pattern matching features.
    /// </summary>
    public static void RunExample()
    {
        Console.WriteLine("\n=== Advanced Pattern Matching ===\n");

        // Type and property patterns
        Shape[] shapes = new Shape[]
        {
            new Circle(15),
            new Rectangle(5, 5),
            new Rectangle(12, 8),
            new Triangle(10, 10)
        };

        Console.WriteLine("1. Shape Classification:");
        foreach (var shape in shapes)
        {
            var area = CalculateArea(shape);
            var classification = ClassifyShape(shape);
            Console.WriteLine($"{shape} -> Area: {area:F2}, Class: {classification}");
        }

        // Array patterns (C# 10 compatible)
        Console.WriteLine("\n2. Array Length Patterns:");
        int[][] sequences = new[]
        {
            Array.Empty<int>(),
            new[] { 42 },
            new[] { 1, 2 },
            new[] { 1, 2, 3, 4, 5 },
            new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 }
        };

        foreach (var seq in sequences)
        {
            var analysis = AnalyzeSequence(seq);
            var valid = IsValidSequence(seq);
            Console.WriteLine($"[{string.Join(", ", seq)}] -> {analysis}, Valid: {valid}");
        }

        // Relational patterns
        Console.WriteLine("\n3. Relational Patterns:");
        int[] ages = new[] { 5, 15, 25, 70, -1 };
        foreach (var age in ages)
        {
            Console.WriteLine($"Age {age}: {ClassifyAge(age)}");
        }

        // Recursive patterns
        Console.WriteLine("\n4. Recursive Patterns:");
        Person[] people = new[]
        {
            new Person("Alice", 30, new Address("San Jose", "USA")),
            new Person("Bob", 25, new Address("New York", "USA")),
            new Person("Charlie", 35, new Address("Palo Alto", "USA"))
        };

        foreach (var person in people)
        {
            var isSV = IsFromSiliconValley(person);
            Console.WriteLine($"{person.Name} is from Silicon Valley: {isSV}");
        }

        // Performance optimized
        Console.WriteLine("\n5. Optimized Classification:");
        int[] values = new[] { 1, 5, 15, 150 };
        foreach (var val in values)
        {
            Console.WriteLine($"{val} -> {OptimizedClassification(val)}");
        }
    }
}
