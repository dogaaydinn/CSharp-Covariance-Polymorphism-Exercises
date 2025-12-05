namespace UnsafeCodeExample;

/// <summary>
/// Demonstrates stack allocation using stackalloc.
///
/// STACK vs HEAP:
/// - Stack: Fast, automatic cleanup, limited size (~1MB)
/// - Heap: Slower, GC overhead, unlimited size
///
/// BENEFITS of stackalloc:
/// - Zero heap allocations (no GC pressure)
/// - Faster allocation (pointer bump)
/// - Automatic cleanup (stack unwind)
/// - Cache-friendly (local memory)
///
/// LIMITATIONS:
/// - Only for small arrays (< 1KB recommended)
/// - Stack overflow risk with large allocations
/// - Cannot escape method scope
///
/// PERFECT FOR:
/// - Temporary buffers
/// - Small work arrays
/// - Performance-critical paths
/// </summary>
public static class StackAllocator
{
    /// <summary>
    /// Basic stackalloc demonstration.
    /// </summary>
    public static unsafe void DemonstrateStackAlloc()
    {
        Console.WriteLine("\n3. Stack Allocation (stackalloc)");

        // Allocate on stack (no heap allocation!)
        Span<int> numbers = stackalloc int[5] { 10, 20, 30, 40, 50 };

        Console.WriteLine($"   Stack-allocated array:");
        for (int i = 0; i < numbers.Length; i++)
        {
            Console.WriteLine($"   [{i}] = {numbers[i]}");
        }

        Console.WriteLine($"   ✅ Zero heap allocations!");
    }

    /// <summary>
    /// Process data using stackalloc buffer (zero allocation).
    /// </summary>
    public static void ProcessWithStackBuffer()
    {
        Console.WriteLine("\n   Processing with stack buffer:");

        // Allocate temporary buffer on stack
        Span<int> buffer = stackalloc int[10];

        // Fill buffer with data
        for (int i = 0; i < buffer.Length; i++)
        {
            buffer[i] = i * i;  // Square numbers
        }

        // Process buffer
        int sum = 0;
        foreach (var value in buffer)
        {
            sum += value;
        }

        Console.WriteLine($"   Sum of squares (0-9): {sum}");
        Console.WriteLine($"   ✅ No heap allocation, no GC!");
    }

    /// <summary>
    /// String manipulation using stackalloc.
    /// </summary>
    public static unsafe void StringManipulationWithStack()
    {
        Console.WriteLine("\n   String manipulation on stack:");

        // Allocate char buffer on stack
        Span<char> buffer = stackalloc char[50];

        // Build string without allocation
        "Hello".AsSpan().CopyTo(buffer);
        ", ".AsSpan().CopyTo(buffer.Slice(5));
        "World!".AsSpan().CopyTo(buffer.Slice(7));

        var result = new string(buffer.Slice(0, 13));
        Console.WriteLine($"   Result: {result}");
        Console.WriteLine($"   ✅ Minimal allocations!");
    }

    /// <summary>
    /// Compute statistics using stack-allocated workspace.
    /// </summary>
    public static void ComputeStatisticsWithStack(ReadOnlySpan<int> data)
    {
        Console.WriteLine("\n   Computing statistics with stack buffer:");

        // Stack-allocate workspace for sorted data
        Span<int> sorted = stackalloc int[data.Length];
        data.CopyTo(sorted);

        // Sort in-place
        sorted.Sort();

        // Calculate statistics
        int min = sorted[0];
        int max = sorted[^1];
        int median = sorted[sorted.Length / 2];

        int sum = 0;
        foreach (var value in sorted)
            sum += value;
        double average = (double)sum / sorted.Length;

        Console.WriteLine($"   Min: {min}");
        Console.WriteLine($"   Max: {max}");
        Console.WriteLine($"   Median: {median}");
        Console.WriteLine($"   Average: {average:F2}");
        Console.WriteLine($"   ✅ All computation on stack!");
    }

    /// <summary>
    /// Matrix multiplication using stack allocation (small matrices).
    /// </summary>
    public static void MultiplySmallMatrices()
    {
        Console.WriteLine("\n   2x2 Matrix multiplication on stack:");

        // Stack-allocate matrices
        Span<int> matrixA = stackalloc int[4] { 1, 2, 3, 4 };
        Span<int> matrixB = stackalloc int[4] { 5, 6, 7, 8 };
        Span<int> result = stackalloc int[4];

        // Multiply (2x2 matrices)
        result[0] = matrixA[0] * matrixB[0] + matrixA[1] * matrixB[2];
        result[1] = matrixA[0] * matrixB[1] + matrixA[1] * matrixB[3];
        result[2] = matrixA[2] * matrixB[0] + matrixA[3] * matrixB[2];
        result[3] = matrixA[2] * matrixB[1] + matrixA[3] * matrixB[3];

        Console.WriteLine($"   Result matrix:");
        Console.WriteLine($"   [{result[0]}, {result[1]}]");
        Console.WriteLine($"   [{result[2]}, {result[3]}]");
        Console.WriteLine($"   ✅ Zero allocations for entire operation!");
    }

    /// <summary>
    /// Parse numbers from string using stack buffer.
    /// </summary>
    public static void ParseNumbersWithStack(string input)
    {
        Console.WriteLine($"\n   Parsing '{input}' with stack buffer:");

        // Stack-allocate result buffer
        Span<int> numbers = stackalloc int[10];
        int count = 0;

        // Parse comma-separated numbers
        var parts = input.Split(',');
        foreach (var part in parts)
        {
            if (int.TryParse(part.Trim(), out int value))
            {
                numbers[count++] = value;
            }
        }

        // Display parsed numbers
        Console.Write("   Parsed: ");
        for (int i = 0; i < count; i++)
        {
            Console.Write($"{numbers[i]} ");
        }
        Console.WriteLine();
        Console.WriteLine($"   ✅ Minimal heap allocations!");
    }
}
