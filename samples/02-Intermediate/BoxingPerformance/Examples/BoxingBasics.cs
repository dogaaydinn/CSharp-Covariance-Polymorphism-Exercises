using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace BoxingPerformance.Examples;

/// <summary>
/// Demonstrates fundamental boxing and unboxing operations in C#.
/// Boxing converts value types to reference types (heap allocation).
/// Unboxing converts reference types back to value types (requires type checking).
/// </summary>
/// <remarks>
/// Boxing Process:
/// 1. Allocates memory on the managed heap
/// 2. Copies the value type data to the newly allocated heap memory
/// 3. Returns a reference to the boxed object
///
/// Unboxing Process:
/// 1. Checks if the object reference is not null
/// 2. Verifies that the object is a boxed value of the correct type
/// 3. Copies the value from the heap to the stack
///
/// Performance Impact:
/// - Boxing: ~10-20ns per operation + heap allocation
/// - Unboxing: ~10-15ns per operation + type checking
/// - Increased GC pressure from heap allocations
/// </remarks>
public static class BoxingBasics
{
    /// <summary>
    /// Demonstrates basic boxing operation with memory allocation tracking.
    /// Shows how a simple assignment causes heap allocation.
    /// </summary>
    public static void DemonstrateBasicBoxing()
    {
        Console.WriteLine("=== Basic Boxing Demonstration ===");
        Console.WriteLine();

        // Track memory before boxing
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();
        long memoryBefore = GC.GetTotalMemory(true);

        // Value type on stack
        int valueType = 42;
        Console.WriteLine($"1. Value Type (stack): {valueType}");
        Console.WriteLine($"   Type: {valueType.GetType()}");
        Console.WriteLine($"   Size: {sizeof(int)} bytes");
        Console.WriteLine();

        // Boxing: int -> object (heap allocation)
        object boxed = valueType;
        Console.WriteLine($"2. Boxed to Object (heap): {boxed}");
        Console.WriteLine($"   Type: {boxed.GetType()}");
        Console.WriteLine($"   Reference Type: object");

        // Track memory after boxing
        long memoryAfter = GC.GetTotalMemory(false);
        long allocated = memoryAfter - memoryBefore;

        Console.WriteLine();
        Console.WriteLine($"MEMORY IMPACT:");
        Console.WriteLine($"  Memory Before: {memoryBefore:N0} bytes");
        Console.WriteLine($"  Memory After:  {memoryAfter:N0} bytes");
        Console.WriteLine($"  Allocated:     {allocated:N0} bytes (includes object header + data)");
        Console.WriteLine();
        Console.WriteLine($"NOTE: Object header (~8-12 bytes) + int (4 bytes) + alignment padding");
        Console.WriteLine();
    }

    /// <summary>
    /// Demonstrates basic unboxing with type checking and potential exceptions.
    /// </summary>
    public static void DemonstrateBasicUnboxing()
    {
        Console.WriteLine("=== Basic Unboxing Demonstration ===");
        Console.WriteLine();

        int original = 123;
        object boxed = original;  // Boxing

        Console.WriteLine($"Boxed value: {boxed}");
        Console.WriteLine();

        // Correct unboxing
        Console.WriteLine("1. CORRECT unboxing:");
        int unboxed = (int)boxed;
        Console.WriteLine($"   Unboxed value: {unboxed}");
        Console.WriteLine($"   Success! Type matched.");
        Console.WriteLine();

        // Incorrect unboxing - demonstrates type checking
        Console.WriteLine("2. INCORRECT unboxing (wrong type):");
        try
        {
            long wrongType = (long)boxed;  // Will throw InvalidCastException
            Console.WriteLine($"   This won't execute: {wrongType}");
        }
        catch (InvalidCastException ex)
        {
            Console.WriteLine($"   Exception: {ex.Message}");
            Console.WriteLine($"   Runtime performs strict type checking!");
        }
        Console.WriteLine();

        // Nullable unboxing
        Console.WriteLine("3. NULLABLE unboxing:");
        object? nullBox = null;
        int? nullableResult = nullBox as int?;
        Console.WriteLine($"   Null boxed value: {nullableResult?.ToString() ?? "null"}");
        Console.WriteLine($"   Safe unboxing with 'as' operator");
        Console.WriteLine();
    }

    /// <summary>
    /// Demonstrates when boxing occurs implicitly in common scenarios.
    /// </summary>
    public static void DemonstrateImplicitBoxing()
    {
        Console.WriteLine("=== Implicit Boxing Scenarios ===");
        Console.WriteLine();

        int number = 42;

        // Scenario 1: String concatenation with value types
        Console.WriteLine("1. String Concatenation:");
        Console.WriteLine($"   string result = \"Value: \" + number;");
        string result = "Value: " + number;  // Boxing occurs!
        Console.WriteLine($"   Result: {result}");
        Console.WriteLine($"   Boxing occurred when converting int to string");
        Console.WriteLine();

        // Scenario 2: String.Format (before interpolation)
        Console.WriteLine("2. String.Format:");
        Console.WriteLine($"   string formatted = String.Format(\"{{0}}\", number);");
        string formatted = string.Format("{0}", number);  // Boxing occurs!
        Console.WriteLine($"   Result: {formatted}");
        Console.WriteLine($"   Boxing occurred - value type passed as object");
        Console.WriteLine();

        // Scenario 3: String interpolation (optimized, less boxing)
        Console.WriteLine("3. String Interpolation (Modern C#):");
        Console.WriteLine($"   string interpolated = $\"Value: {{number}}\";");
        string interpolated = $"Value: {number}";  // Optimized by compiler
        Console.WriteLine($"   Result: {interpolated}");
        Console.WriteLine($"   Compiler optimizes this - minimal/no boxing");
        Console.WriteLine();

        // Scenario 4: Interface implementation
        Console.WriteLine("4. Interface Cast:");
        IComparable comparable = number;  // Boxing occurs!
        Console.WriteLine($"   IComparable comparable = number;");
        Console.WriteLine($"   Boxing occurred - struct implements interface");
        Console.WriteLine();

        // Scenario 5: Object parameters
        Console.WriteLine("5. Method with Object Parameter:");
        Console.WriteLine($"   ProcessObject(number);");
        ProcessObject(number);  // Boxing occurs!
        Console.WriteLine($"   Boxing occurred when passing to object parameter");
        Console.WriteLine();
    }

    /// <summary>
    /// Measures the performance cost of boxing vs no boxing operations.
    /// </summary>
    public static void MeasureBoxingPerformance()
    {
        Console.WriteLine("=== Boxing Performance Measurement ===");
        Console.WriteLine();

        const int iterations = 1_000_000;
        int value = 42;

        // Warmup
        for (int i = 0; i < 1000; i++)
        {
            object _ = value;
        }

        Console.WriteLine($"Running {iterations:N0} iterations...");
        Console.WriteLine();

        // Test 1: With Boxing
        GC.Collect();
        GC.WaitForPendingFinalizers();
        var sw1 = Stopwatch.StartNew();
        for (int i = 0; i < iterations; i++)
        {
            object boxed = value;  // Boxing
            int unboxed = (int)boxed;  // Unboxing
        }
        sw1.Stop();

        // Test 2: Without Boxing
        GC.Collect();
        GC.WaitForPendingFinalizers();
        var sw2 = Stopwatch.StartNew();
        for (int i = 0; i < iterations; i++)
        {
            int temp = value;  // Just copy
            int result = temp;  // Just copy
        }
        sw2.Stop();

        Console.WriteLine($"WITH Boxing/Unboxing:    {sw1.ElapsedMilliseconds,6} ms ({sw1.ElapsedTicks,10} ticks)");
        Console.WriteLine($"WITHOUT Boxing:          {sw2.ElapsedMilliseconds,6} ms ({sw2.ElapsedTicks,10} ticks)");
        Console.WriteLine();

        if (sw2.ElapsedMilliseconds > 0)
        {
            double slowdown = (double)sw1.ElapsedMilliseconds / sw2.ElapsedMilliseconds;
            Console.WriteLine($"Boxing is {slowdown:F1}x SLOWER");
        }
        else
        {
            Console.WriteLine($"Boxing overhead: {sw1.ElapsedMilliseconds} ms");
        }

        Console.WriteLine();
        Console.WriteLine($"Per-operation cost:");
        Console.WriteLine($"  Boxing:   ~{(double)sw1.ElapsedTicks / iterations:F2} ticks");
        Console.WriteLine($"  No-boxing: ~{(double)sw2.ElapsedTicks / iterations:F2} ticks");
        Console.WriteLine();
    }

    /// <summary>
    /// Demonstrates stack vs heap allocation for value types.
    /// </summary>
    public static void DemonstrateStackVsHeap()
    {
        Console.WriteLine("=== Stack vs Heap Allocation ===");
        Console.WriteLine();

        Console.WriteLine("STACK ALLOCATION (Value Type):");
        Console.WriteLine("  - Fast allocation (just move stack pointer)");
        Console.WriteLine("  - Automatic deallocation when scope ends");
        Console.WriteLine("  - No GC pressure");
        Console.WriteLine("  - Limited space (~1MB default)");
        Console.WriteLine();

        // Value type on stack
        int stackValue = 100;
        Console.WriteLine($"  int stackValue = {stackValue}");
        Console.WriteLine($"  Location: Stack");
        Console.WriteLine($"  Lifetime: Current scope");
        Console.WriteLine();

        Console.WriteLine("HEAP ALLOCATION (Boxed Value Type):");
        Console.WriteLine("  - Slower allocation (heap management overhead)");
        Console.WriteLine("  - Managed by Garbage Collector");
        Console.WriteLine("  - Increases GC pressure");
        Console.WriteLine("  - Larger available space (~2GB per process)");
        Console.WriteLine();

        // Boxing to heap
        object heapValue = stackValue;
        Console.WriteLine($"  object heapValue = stackValue (boxed)");
        Console.WriteLine($"  Location: Managed Heap");
        Console.WriteLine($"  Lifetime: Until GC collects");
        Console.WriteLine($"  Object Header: ~8-12 bytes overhead");
        Console.WriteLine();

        Console.WriteLine("MEMORY LAYOUT:");
        Console.WriteLine();
        Console.WriteLine("Stack (value type):");
        Console.WriteLine("  [int: 4 bytes]");
        Console.WriteLine();
        Console.WriteLine("Heap (boxed value):");
        Console.WriteLine("  [Object Header: 8-12 bytes]");
        Console.WriteLine("  [Type Handle: 4-8 bytes]");
        Console.WriteLine("  [int value: 4 bytes]");
        Console.WriteLine("  [Padding: 0-4 bytes for alignment]");
        Console.WriteLine("  Total: ~24 bytes (6x overhead!)");
        Console.WriteLine();
    }

    /// <summary>
    /// Helper method demonstrating boxing when passing to object parameters.
    /// </summary>
    [MethodImpl(MethodImplOptions.NoInlining)]
    private static void ProcessObject(object obj)
    {
        // Just to demonstrate boxing occurs
    }

    /// <summary>
    /// Runs all boxing basics demonstrations.
    /// </summary>
    public static void RunAll()
    {
        DemonstrateBasicBoxing();
        Console.WriteLine();
        Console.WriteLine("=".PadRight(70, '='));
        Console.WriteLine();

        DemonstrateBasicUnboxing();
        Console.WriteLine("=".PadRight(70, '='));
        Console.WriteLine();

        DemonstrateImplicitBoxing();
        Console.WriteLine("=".PadRight(70, '='));
        Console.WriteLine();

        DemonstrateStackVsHeap();
        Console.WriteLine("=".PadRight(70, '='));
        Console.WriteLine();

        MeasureBoxingPerformance();
    }
}
