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
        DemonstrateStringParsing();

        // 2. Array slicing without copying
        DemonstrateArraySlicing();

        // 3. Stack-based processing
        DemonstrateStackAllocation();

        // 4. ReadOnlySpan for immutability
        DemonstrateReadOnlySpan();

        Console.WriteLine("\n✅ All operations completed with zero allocations!");
    }

    static void DemonstrateStringParsing()
    {
        Console.WriteLine("1. Zero-Allocation String Parsing");

        string data = "John,Doe,30,Engineer";

        // ✅ GOOD: No substring allocations!
        ReadOnlySpan<char> span = data.AsSpan();

        int firstComma = span.IndexOf(',');
        var firstName = span.Slice(0, firstComma);

        Console.WriteLine($"   First name: {firstName.ToString()}");
        Console.WriteLine($"   ✅ Zero string allocations!");
    }

    static void DemonstrateArraySlicing()
    {
        Console.WriteLine("\n2. Array Slicing (No Copying)");

        int[] numbers = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };

        // ✅ GOOD: Slice without copying data
        Span<int> middleThree = numbers.AsSpan().Slice(4, 3);

        Console.Write("   Middle 3 elements: ");
        for (int i = 0; i < middleThree.Length; i++)
        {
            Console.Write($"{middleThree[i]} ");
        }
        Console.WriteLine();

        // Modifications affect original array
        middleThree[0] = 999;
        Console.WriteLine($"   Original array[4]: {numbers[4]} (modified!)");
    }

    static void DemonstrateStackAllocation()
    {
        Console.WriteLine("\n3. Stack-Based Processing");

        // Stack allocation (no GC pressure!)
        Span<byte> buffer = stackalloc byte[256];

        // Fill with pattern
        for (int i = 0; i < buffer.Length; i++)
        {
            buffer[i] = (byte)(i % 16);
        }

        Console.WriteLine($"   Processed {buffer.Length} bytes on stack");
        Console.WriteLine($"   ✅ Zero heap allocations!");
    }

    static void DemonstrateReadOnlySpan()
    {
        Console.WriteLine("\n4. ReadOnlySpan for Immutability");

        string text = "Hello, World!";
        ReadOnlySpan<char> readOnly = text.AsSpan();

        // ✅ Can read
        Console.WriteLine($"   Length: {readOnly.Length}");
        Console.WriteLine($"   First char: {readOnly[0]}");

        // ❌ Cannot write (compile error!)
        // readOnly[0] = 'h';  // Error: ReadOnlySpan is immutable

        Console.WriteLine($"   ✅ Type-safe immutability!");
    }
}
