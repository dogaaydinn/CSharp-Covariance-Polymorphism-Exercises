using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;

namespace BoxingPerformance.Examples;

/// <summary>
/// Comprehensive strategies and techniques to avoid boxing in C# applications.
/// Demonstrates best practices and patterns that eliminate unnecessary boxing operations.
/// </summary>
/// <remarks>
/// Key Strategies:
/// 1. Use generic collections (List&lt;T&gt;, Dictionary&lt;K,V&gt;)
/// 2. Use generic methods and constraints
/// 3. Avoid object parameters when possible
/// 4. Use StringBuilder instead of string concatenation
/// 5. Override ToString() in structs
/// 6. Use ValueTask&lt;T&gt; for async value type returns
/// 7. Leverage Span&lt;T&gt; and Memory&lt;T&gt;
/// 8. Use readonly struct for immutable values
/// </remarks>
public static class AvoidingBoxing
{
    /// <summary>
    /// Demonstrates using generic collections to avoid boxing.
    /// </summary>
    public static void UseGenericCollections()
    {
        Console.WriteLine("=== Strategy 1: Use Generic Collections ===");
        Console.WriteLine();

        const int count = 100_000;

        // BAD: Non-generic collection (causes boxing)
        Console.WriteLine("BAD - Non-generic ArrayList (boxing):");
        var sw1 = Stopwatch.StartNew();
        System.Collections.ArrayList badList = new System.Collections.ArrayList();
        for (int i = 0; i < count; i++)
        {
            badList.Add(i);  // Boxing!
        }
        long sum1 = 0;
        foreach (object obj in badList)
        {
            sum1 += (int)obj;  // Unboxing!
        }
        sw1.Stop();
        Console.WriteLine($"  Time: {sw1.ElapsedMilliseconds} ms, Sum: {sum1}");
        Console.WriteLine($"  Problem: Every Add() and access causes boxing/unboxing");
        Console.WriteLine();

        // GOOD: Generic collection (no boxing)
        Console.WriteLine("GOOD - Generic List<int> (no boxing):");
        var sw2 = Stopwatch.StartNew();
        List<int> goodList = new List<int>();
        for (int i = 0; i < count; i++)
        {
            goodList.Add(i);  // No boxing!
        }
        long sum2 = 0;
        foreach (int value in goodList)
        {
            sum2 += value;  // No unboxing!
        }
        sw2.Stop();
        Console.WriteLine($"  Time: {sw2.ElapsedMilliseconds} ms, Sum: {sum2}");
        Console.WriteLine($"  Benefit: Type-safe, no allocations, {sw1.ElapsedMilliseconds / Math.Max(sw2.ElapsedMilliseconds, 1)}x faster");
        Console.WriteLine();

        // MORE EXAMPLES
        Console.WriteLine("Other generic collection replacements:");
        Console.WriteLine("  ArrayList      -> List<T>");
        Console.WriteLine("  Hashtable      -> Dictionary<TKey, TValue>");
        Console.WriteLine("  Queue          -> Queue<T>");
        Console.WriteLine("  Stack          -> Stack<T>");
        Console.WriteLine("  SortedList     -> SortedList<TKey, TValue>");
        Console.WriteLine();
    }

    /// <summary>
    /// Demonstrates using generic methods with constraints to avoid boxing.
    /// </summary>
    public static void UseGenericMethods()
    {
        Console.WriteLine("=== Strategy 2: Use Generic Methods ===");
        Console.WriteLine();

        int intValue = 42;
        double doubleValue = 3.14;
        Point customStruct = new Point(10, 20);

        // BAD: Object parameter (causes boxing)
        Console.WriteLine("BAD - Object parameter:");
        Console.WriteLine($"  PrintValue(intValue)    -> Boxing occurs");
        Console.WriteLine($"  PrintValue(doubleValue) -> Boxing occurs");
        Console.WriteLine($"  PrintValue(customStruct) -> Boxing occurs");
        PrintValueBoxing(intValue);
        PrintValueBoxing(doubleValue);
        PrintValueBoxing(customStruct);
        Console.WriteLine();

        // GOOD: Generic method (no boxing)
        Console.WriteLine("GOOD - Generic method:");
        Console.WriteLine($"  PrintValueGeneric<int>(intValue)       -> No boxing");
        Console.WriteLine($"  PrintValueGeneric<double>(doubleValue) -> No boxing");
        Console.WriteLine($"  PrintValueGeneric<Point>(customStruct) -> No boxing");
        PrintValueGeneric(intValue);
        PrintValueGeneric(doubleValue);
        PrintValueGeneric(customStruct);
        Console.WriteLine();

        // BEST: Generic method with constraints
        Console.WriteLine("BEST - Generic method with constraints:");
        int result = CompareValues(10, 20);  // No boxing, type-safe
        Console.WriteLine($"  CompareValues(10, 20) = {result}");
        Console.WriteLine($"  Benefit: Type safety + No boxing + Compile-time checks");
        Console.WriteLine();
    }

    /// <summary>
    /// Demonstrates proper string building to avoid boxing in string operations.
    /// </summary>
    public static void UseStringBuilderCorrectly()
    {
        Console.WriteLine("=== Strategy 3: Use StringBuilder Properly ===");
        Console.WriteLine();

        const int iterations = 10_000;

        // BAD: String concatenation with value types
        Console.WriteLine("BAD - String concatenation (boxing + allocations):");
        var sw1 = Stopwatch.StartNew();
        string result1 = "";
        for (int i = 0; i < Math.Min(iterations, 1000); i++)  // Limited to avoid OutOfMemoryException
        {
            result1 += i.ToString();  // Multiple allocations + boxing
        }
        sw1.Stop();
        Console.WriteLine($"  Time: {sw1.ElapsedMilliseconds} ms");
        Console.WriteLine($"  Problems: String immutability + boxing + O(n²) complexity");
        Console.WriteLine();

        // BETTER: String.Format or interpolation
        Console.WriteLine("BETTER - String interpolation (less boxing):");
        var sw2 = Stopwatch.StartNew();
        string result2 = "";
        for (int i = 0; i < Math.Min(iterations, 1000); i++)
        {
            result2 += $"{i}";  // Compiler optimized, but still creates strings
        }
        sw2.Stop();
        Console.WriteLine($"  Time: {sw2.ElapsedMilliseconds} ms");
        Console.WriteLine($"  Benefit: Compiler optimization, cleaner syntax");
        Console.WriteLine();

        // BEST: StringBuilder with proper capacity
        Console.WriteLine("BEST - StringBuilder with capacity (minimal boxing):");
        var sw3 = Stopwatch.StartNew();
        StringBuilder sb = new StringBuilder(iterations * 5);  // Pre-allocate
        for (int i = 0; i < iterations; i++)
        {
            sb.Append(i);  // No boxing with Append(int)
        }
        string result3 = sb.ToString();
        sw3.Stop();
        Console.WriteLine($"  Time: {sw3.ElapsedMilliseconds} ms");
        Console.WriteLine($"  Benefit: No boxing, O(n) complexity, single allocation");
        Console.WriteLine($"  Speedup: ~{sw1.ElapsedMilliseconds / Math.Max(sw3.ElapsedMilliseconds, 1)}x faster");
        Console.WriteLine();

        // Show StringBuilder's no-boxing Append overloads
        Console.WriteLine("StringBuilder has no-boxing overloads:");
        sb.Clear();
        sb.Append(42);              // Append(int) - no boxing
        sb.Append(3.14);            // Append(double) - no boxing
        sb.Append(true);            // Append(bool) - no boxing
        sb.Append('c');             // Append(char) - no boxing
        Console.WriteLine($"  Result: {sb}");
        Console.WriteLine($"  All value types appended without boxing!");
        Console.WriteLine();
    }

    /// <summary>
    /// Demonstrates overriding ToString() in structs to avoid boxing.
    /// </summary>
    public static void OverrideToStringInStructs()
    {
        Console.WriteLine("=== Strategy 4: Override ToString() in Structs ===");
        Console.WriteLine();

        // Test with struct that doesn't override ToString()
        var badPoint = new BadPoint(10, 20);
        Console.WriteLine("Struct WITHOUT ToString() override:");
        var sw1 = Stopwatch.StartNew();
        for (int i = 0; i < 100_000; i++)
        {
            string str = badPoint.ToString();  // Boxing occurs!
        }
        sw1.Stop();
        Console.WriteLine($"  Time: {sw1.ElapsedMilliseconds} ms");
        Console.WriteLine($"  Problem: Calls ValueType.ToString() -> boxing");
        Console.WriteLine();

        // Test with struct that overrides ToString()
        var goodPoint = new Point(10, 20);
        Console.WriteLine("Struct WITH ToString() override:");
        var sw2 = Stopwatch.StartNew();
        for (int i = 0; i < 100_000; i++)
        {
            string str = goodPoint.ToString();  // No boxing!
        }
        sw2.Stop();
        Console.WriteLine($"  Time: {sw2.ElapsedMilliseconds} ms");
        Console.WriteLine($"  Benefit: Direct ToString() call, no boxing");
        Console.WriteLine($"  Speedup: ~{sw1.ElapsedMilliseconds / Math.Max(sw2.ElapsedMilliseconds, 1)}x faster");
        Console.WriteLine();

        Console.WriteLine("RULE: Always override ToString() in public structs!");
        Console.WriteLine();
    }

    /// <summary>
    /// Demonstrates using readonly struct for better performance.
    /// </summary>
    public static void UseReadonlyStructs()
    {
        Console.WriteLine("=== Strategy 5: Use Readonly Struct ===");
        Console.WriteLine();

        Console.WriteLine("Benefits of readonly struct:");
        Console.WriteLine("  1. Prevents defensive copies");
        Console.WriteLine("  2. Compiler optimization opportunities");
        Console.WriteLine("  3. Clearer intent (immutability)");
        Console.WriteLine("  4. No hidden boxing from defensive copies");
        Console.WriteLine();

        // Regular struct (may cause defensive copies)
        var mutablePoint = new MutablePoint(10, 20);
        Console.WriteLine("Regular struct:");
        Console.WriteLine($"  {mutablePoint}");
        Console.WriteLine($"  Risk: Defensive copies in readonly contexts");
        Console.WriteLine();

        // Readonly struct (no defensive copies)
        var immutablePoint = new ImmutablePoint(10, 20);
        Console.WriteLine("Readonly struct:");
        Console.WriteLine($"  {immutablePoint}");
        Console.WriteLine($"  Benefit: Guaranteed no defensive copies");
        Console.WriteLine();

        // Demonstrate defensive copy issue
        DemonstrateDefensiveCopies();
    }

    /// <summary>
    /// Demonstrates avoiding interface casts with structs.
    /// </summary>
    public static void AvoidInterfaceCasts()
    {
        Console.WriteLine("=== Strategy 6: Avoid Interface Casts with Structs ===");
        Console.WriteLine();

        var point = new Point(10, 20);

        // BAD: Casting struct to interface (boxing)
        Console.WriteLine("BAD - Cast struct to interface:");
        var sw1 = Stopwatch.StartNew();
        for (int i = 0; i < 100_000; i++)
        {
            IFormattable formattable = point;  // Boxing!
            string str = formattable.ToString("", null);
        }
        sw1.Stop();
        Console.WriteLine($"  Time: {sw1.ElapsedMilliseconds} ms");
        Console.WriteLine($"  Problem: Interface cast causes boxing");
        Console.WriteLine();

        // GOOD: Call method directly on struct
        Console.WriteLine("GOOD - Call method directly on struct:");
        var sw2 = Stopwatch.StartNew();
        for (int i = 0; i < 100_000; i++)
        {
            string str = point.ToString();  // No boxing!
        }
        sw2.Stop();
        Console.WriteLine($"  Time: {sw2.ElapsedMilliseconds} ms");
        Console.WriteLine($"  Benefit: Direct call, no boxing");
        Console.WriteLine($"  Speedup: ~{sw1.ElapsedMilliseconds / Math.Max(sw2.ElapsedMilliseconds, 1)}x faster");
        Console.WriteLine();
    }

    /// <summary>
    /// Demonstrates using generic constraints to avoid boxing.
    /// </summary>
    public static void UseGenericConstraints()
    {
        Console.WriteLine("=== Strategy 7: Use Generic Constraints ===");
        Console.WriteLine();

        Console.WriteLine("Generic constraint examples:");
        Console.WriteLine();

        // Example 1: struct constraint
        Console.WriteLine("1. 'where T : struct' - Ensures value type:");
        int intResult = ProcessValueType(42);
        double doubleResult = ProcessValueType(3.14);
        Console.WriteLine($"   ProcessValueType(42) = {intResult}");
        Console.WriteLine($"   ProcessValueType(3.14) = {doubleResult:F2}");
        Console.WriteLine($"   No boxing - T is constrained to value types");
        Console.WriteLine();

        // Example 2: class constraint
        Console.WriteLine("2. 'where T : class' - Ensures reference type:");
        string strResult = ProcessReferenceType("Hello");
        Console.WriteLine($"   ProcessReferenceType(\"Hello\") = {strResult}");
        Console.WriteLine($"   No need for boxing - already reference type");
        Console.WriteLine();

        // Example 3: IComparable<T> constraint
        Console.WriteLine("3. 'where T : IComparable<T>' - Type-safe comparison:");
        int compareResult = CompareValues(10, 20);
        Console.WriteLine($"   CompareValues(10, 20) = {compareResult}");
        Console.WriteLine($"   No boxing - uses IComparable<T> not IComparable");
        Console.WriteLine();
    }

    /// <summary>
    /// Demonstrates compilation techniques to detect boxing.
    /// </summary>
    public static void DetectBoxingAtCompileTime()
    {
        Console.WriteLine("=== Strategy 8: Detect Boxing at Compile Time ===");
        Console.WriteLine();

        Console.WriteLine("Tools to detect boxing:");
        Console.WriteLine("  1. Visual Studio Code Analysis");
        Console.WriteLine("  2. ReSharper / Rider warnings");
        Console.WriteLine("  3. ILSpy / dnSpy to view IL code");
        Console.WriteLine("  4. BenchmarkDotNet for measurements");
        Console.WriteLine("  5. PerfView for allocation profiling");
        Console.WriteLine();

        Console.WriteLine("IL Instructions that indicate boxing:");
        Console.WriteLine("  box        - Boxing value type to object");
        Console.WriteLine("  unbox.any  - Unboxing object to value type");
        Console.WriteLine("  castclass  - Type casting (may involve boxing)");
        Console.WriteLine();

        Console.WriteLine("Example - look for 'box' in IL:");
        Console.WriteLine("  BAD CODE:");
        Console.WriteLine("    object obj = 42;");
        Console.WriteLine("  IL:");
        Console.WriteLine("    ldc.i4.s   42");
        Console.WriteLine("    box        [System.Runtime]System.Int32  <- BOXING!");
        Console.WriteLine();

        Console.WriteLine("  GOOD CODE:");
        Console.WriteLine("    int value = 42;");
        Console.WriteLine("  IL:");
        Console.WriteLine("    ldc.i4.s   42");
        Console.WriteLine("    (no box instruction - stays on stack)");
        Console.WriteLine();
    }

    #region Helper Methods

    // Bad: Object parameter causes boxing
    [MethodImpl(MethodImplOptions.NoInlining)]
    private static void PrintValueBoxing(object value)
    {
        // Boxing occurred when called with value type
    }

    // Good: Generic method avoids boxing
    [MethodImpl(MethodImplOptions.NoInlining)]
    private static void PrintValueGeneric<T>(T value)
    {
        // No boxing - T is preserved
    }

    // Generic method with constraint
    private static int CompareValues<T>(T a, T b) where T : IComparable<T>
    {
        return a.CompareTo(b);  // No boxing!
    }

    // Process value type without boxing
    private static T ProcessValueType<T>(T value) where T : struct
    {
        return value;  // No boxing
    }

    // Process reference type
    private static T ProcessReferenceType<T>(T value) where T : class
    {
        return value;  // Already reference type
    }

    private static void DemonstrateDefensiveCopies()
    {
        Console.WriteLine("Defensive Copy Example:");
        Console.WriteLine();

        // This method shows where defensive copies occur
        var container = new PointContainer
        {
            MutablePoint = new MutablePoint(5, 10),
            ImmutablePoint = new ImmutablePoint(5, 10)
        };

        // Accessing readonly field with mutable struct -> defensive copy
        Console.WriteLine($"  Mutable point: {container.MutablePoint}");
        Console.WriteLine($"    ^ Defensive copy may occur");

        // Accessing readonly field with readonly struct -> no defensive copy
        Console.WriteLine($"  Immutable point: {container.ImmutablePoint}");
        Console.WriteLine($"    ^ No defensive copy needed");
        Console.WriteLine();
    }

    #endregion

    /// <summary>
    /// Runs all boxing avoidance demonstrations.
    /// </summary>
    public static void RunAll()
    {
        UseGenericCollections();
        Console.WriteLine("=".PadRight(70, '='));
        Console.WriteLine();

        UseGenericMethods();
        Console.WriteLine("=".PadRight(70, '='));
        Console.WriteLine();

        UseStringBuilderCorrectly();
        Console.WriteLine("=".PadRight(70, '='));
        Console.WriteLine();

        OverrideToStringInStructs();
        Console.WriteLine("=".PadRight(70, '='));
        Console.WriteLine();

        UseReadonlyStructs();
        Console.WriteLine("=".PadRight(70, '='));
        Console.WriteLine();

        AvoidInterfaceCasts();
        Console.WriteLine("=".PadRight(70, '='));
        Console.WriteLine();

        UseGenericConstraints();
        Console.WriteLine("=".PadRight(70, '='));
        Console.WriteLine();

        DetectBoxingAtCompileTime();
    }
}

#region Helper Types

public struct Point : IFormattable
{
    public int X { get; }
    public int Y { get; }

    public Point(int x, int y)
    {
        X = x;
        Y = y;
    }

    public override string ToString() => $"({X}, {Y})";

    public string ToString(string? format, IFormatProvider? formatProvider) => ToString();
}

public struct BadPoint
{
    public int X { get; }
    public int Y { get; }

    public BadPoint(int x, int y)
    {
        X = x;
        Y = y;
    }
    // No ToString() override - will box when ToString() is called
}

public struct MutablePoint
{
    public int X { get; set; }
    public int Y { get; set; }

    public MutablePoint(int x, int y)
    {
        X = x;
        Y = y;
    }

    public override string ToString() => $"({X}, {Y})";
}

public readonly struct ImmutablePoint
{
    public int X { get; }
    public int Y { get; }

    public ImmutablePoint(int x, int y)
    {
        X = x;
        Y = y;
    }

    public override string ToString() => $"({X}, {Y})";
}

public class PointContainer
{
    public MutablePoint MutablePoint { get; init; }
    public ImmutablePoint ImmutablePoint { get; init; }
}

#endregion
