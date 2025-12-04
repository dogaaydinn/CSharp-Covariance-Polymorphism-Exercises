// Advanced Pattern Matching: List patterns, slice patterns (C# 11+)

namespace PatternMatchingAdvanced;

public class Program
{
    public static void Main()
    {
        Console.WriteLine("=== Advanced Pattern Matching Demo ===\n");

        // List patterns (C# 11)
        Console.WriteLine("✅ List Patterns:");
        TestListPatterns();

        // Slice patterns
        Console.WriteLine("\n✅ Slice Patterns:");
        TestSlicePatterns();

        // Property patterns
        Console.WriteLine("\n✅ Property Patterns:");
        TestPropertyPatterns();

        // Relational and logical patterns
        Console.WriteLine("\n✅ Relational Patterns:");
        TestRelationalPatterns();

        Console.WriteLine("\n=== Pattern Matching Applied ===");
    }

    static void TestListPatterns()
    {
        int[] empty = [];
        int[] single = [1];
        int[] pair = [1, 2];
        int[] triple = [1, 2, 3];
        int[] many = [1, 2, 3, 4, 5];

        Console.WriteLine($"Empty: {ClassifyArray(empty)}");
        Console.WriteLine($"Single: {ClassifyArray(single)}");
        Console.WriteLine($"Pair: {ClassifyArray(pair)}");
        Console.WriteLine($"Triple: {ClassifyArray(triple)}");
        Console.WriteLine($"Many: {ClassifyArray(many)}");
    }

    static string ClassifyArray(int[] arr) => arr switch
    {
        [] => "Empty array",
        [var x] => $"Single element: {x}",
        [var x, var y] => $"Pair: {x}, {y}",
        [1, 2, 3] => "Exactly [1, 2, 3]",
        [1, .., 5] => "Starts with 1, ends with 5",
        [.., var last] => $"Ends with: {last}",
        _ => "Other"
    };

    static void TestSlicePatterns()
    {
        string[] words1 = ["Hello"];
        string[] words2 = ["Hello", "World"];
        string[] words3 = ["Hello", "Beautiful", "World"];

        Console.WriteLine($"words1: {DescribeWords(words1)}");
        Console.WriteLine($"words2: {DescribeWords(words2)}");
        Console.WriteLine($"words3: {DescribeWords(words3)}");
    }

    static string DescribeWords(string[] words) => words switch
    {
        [var first] => $"Just '{first}'",
        [var first, var last] => $"'{first}' and '{last}'",
        [var first, .., var last] => $"'{first}' ... '{last}' ({words.Length} words)",
        _ => "Empty"
    };

    static void TestPropertyPatterns()
    {
        var person1 = new Person("Alice", 25, "New York");
        var person2 = new Person("Bob", 17, "London");
        var person3 = new Person("Charlie", 30, "New York");

        Console.WriteLine($"{person1.Name}: {ClassifyPerson(person1)}");
        Console.WriteLine($"{person2.Name}: {ClassifyPerson(person2)}");
        Console.WriteLine($"{person3.Name}: {ClassifyPerson(person3)}");
    }

    static string ClassifyPerson(Person person) => person switch
    {
        { Age: < 18 } => "Minor",
        { Age: >= 18 and < 65, City: "New York" } => "Adult New Yorker",
        { Age: >= 65 } => "Senior",
        _ => "Adult"
    };

    static void TestRelationalPatterns()
    {
        Console.WriteLine($"15°C: {ClassifyTemperature(15)}");
        Console.WriteLine($"25°C: {ClassifyTemperature(25)}");
        Console.WriteLine($"35°C: {ClassifyTemperature(35)}");
    }

    static string ClassifyTemperature(int temp) => temp switch
    {
        < 0 => "Freezing",
        >= 0 and < 10 => "Cold",
        >= 10 and < 20 => "Cool",
        >= 20 and < 30 => "Warm",
        >= 30 => "Hot",
        _ => "Unknown"
    };
}

public record Person(string Name, int Age, string City);

// Advanced: Recursive patterns
public abstract record Shape;
public record Circle(double Radius) : Shape;
public record Rectangle(double Width, double Height) : Shape;
public record Group(Shape[] Shapes) : Shape;

public class ShapeCalculator
{
    public static double CalculateArea(Shape shape) => shape switch
    {
        Circle { Radius: var r } => Math.PI * r * r,
        Rectangle { Width: var w, Height: var h } => w * h,
        Group { Shapes: var shapes } => shapes.Sum(CalculateArea), // Recursive!
        _ => 0
    };
}

// Practical example: Validation
public class Validator
{
    public static string ValidateInput(string? input) => input switch
    {
        null or "" => "Input cannot be empty",
        { Length: < 3 } => "Input too short (min 3 characters)",
        { Length: > 50 } => "Input too long (max 50 characters)",
        var s when s.All(char.IsDigit) => "Input cannot be all digits",
        var s when !s.Any(char.IsLetter) => "Input must contain letters",
        _ => "Valid"
    };
}

// COMPARISON
// Pattern Type          | Example                    | C# Version
// ----------------------|----------------------------|------------
// Type pattern          | obj is string s            | C# 7
// Property pattern      | { Age: > 18 }              | C# 8
// Tuple pattern         | (x, y) is (0, 0)           | C# 8
// Positional pattern    | point is (0, 0)            | C# 8
// Relational pattern    | temp is >= 0 and < 10      | C# 9
// Logical pattern       | value is not null          | C# 9
// List pattern          | arr is [1, 2, 3]           | C# 11
// Slice pattern         | arr is [var first, ..]     | C# 11
