using System;

namespace HighPerformanceSpan;

/// <summary>
/// Demonstrates zero-allocation string and array processing with Span&lt;T&gt; and Memory&lt;T&gt;.
/// </summary>
class Program
{
    static void Main()
    {
        Console.WriteLine("=== High-Performance Span<T> Example ===\n");

        // 1. Zero-allocation string parsing
        StringParser.DemonstrateStringParsing();

        // 2. Array slicing without copying
        SpanArrayProcessor.DemonstrateArraySlicing();

        // 3. Stack-based processing
        StackProcessor.DemonstrateStackAllocation();

        // 4. ReadOnlySpan for immutability
        SpanHelper.DemonstrateReadOnlySpan();

        Console.WriteLine("\n✅ All operations completed with zero allocations!");
    }
}
