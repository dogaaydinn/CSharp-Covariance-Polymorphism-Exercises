using System;
using System.Buffers;

namespace PerformanceOptimization.Examples;

/// <summary>
/// Demonstrates the performance advantages of Span&lt;T&gt; over arrays
/// </summary>
public static class SpanVsArray
{
    /// <summary>
    /// Example 1: Basic Span usage - no heap allocation
    /// </summary>
    public static void BasicSpanUsage()
    {
        Console.WriteLine("\n=== Basic Span Usage ===");
        
        // Traditional array approach - heap allocation
        int[] array = new int[] { 1, 2, 3, 4, 5 };
        int sum1 = SumArray(array);
        Console.WriteLine($"Array sum: {sum1}");
        
        // Span approach - can work with stack, heap, or native memory
        Span<int> span = stackalloc int[] { 1, 2, 3, 4, 5 };
        int sum2 = SumSpan(span);
        Console.WriteLine($"Span sum: {sum2}");
        
        Console.WriteLine("\nKey insight: Span can be allocated on stack (no GC pressure!)");
    }
    
    private static int SumArray(int[] array)
    {
        int sum = 0;
        for (int i = 0; i < array.Length; i++)
            sum += array[i];
        return sum;
    }
    
    private static int SumSpan(Span<int> span)
    {
        int sum = 0;
        for (int i = 0; i < span.Length; i++)
            sum += span[i];
        return sum;
    }

    /// <summary>
    /// Example 2: Slicing without copying - massive performance gain
    /// </summary>
    public static void SlicingWithoutCopying()
    {
        Console.WriteLine("\n=== Slicing Without Copying ===");
        
        int[] numbers = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
        
        // Traditional approach - creates new array (allocates memory)
        int[] middleArray = new int[4];
        Array.Copy(numbers, 3, middleArray, 0, 4);
        Console.WriteLine($"Array slice: [{string.Join(", ", middleArray)}]");
        
        // Span approach - just a view, no allocation
        Span<int> span = numbers;
        Span<int> middleSpan = span.Slice(3, 4);
        Console.WriteLine($"Span slice: [{string.Join(", ", middleSpan.ToArray())}]");
        
        // Modify span - affects original array
        middleSpan[0] = 999;
        Console.WriteLine($"Original array after span modification: [{string.Join(", ", numbers)}]");
        Console.WriteLine("\nKey insight: Span is a view over memory, slicing is free!");
    }

    /// <summary>
    /// Example 3: String manipulation with Span - avoid allocations
    /// </summary>
    public static void StringSpan()
    {
        Console.WriteLine("\n=== String Manipulation with Span ===");
        
        string text = "Hello, World!";
        
        // Traditional substring - allocates new string
        string sub1 = text.Substring(0, 5);
        Console.WriteLine($"Substring: '{sub1}'");
        
        // Span approach - no allocation
        ReadOnlySpan<char> span = text.AsSpan();
        ReadOnlySpan<char> sub2 = span.Slice(0, 5);
        Console.WriteLine($"Span slice: '{sub2.ToString()}'");
        
        // Parse directly from span
        ReadOnlySpan<char> numberSpan = "12345".AsSpan();
        int number = int.Parse(numberSpan);
        Console.WriteLine($"Parsed number: {number}");
        
        Console.WriteLine("\nKey insight: ReadOnlySpan<char> can parse without string allocation!");
    }

    /// <summary>
    /// Example 4: Span with stackalloc - ultimate performance
    /// </summary>
    public static void StackallocWithSpan()
    {
        Console.WriteLine("\n=== Stackalloc with Span ===");
        
        // Allocate on stack (safe with Span, dangerous with pointers)
        Span<byte> buffer = stackalloc byte[256];
        
        // Fill with data
        for (int i = 0; i < buffer.Length; i++)
            buffer[i] = (byte)(i % 256);
        
        // Process data
        int sum = 0;
        foreach (byte b in buffer)
            sum += b;
        
        Console.WriteLine($"Processed {buffer.Length} bytes on stack");
        Console.WriteLine($"Sum: {sum}");
        Console.WriteLine("\nKey insight: stackalloc + Span = zero heap allocations!");
        Console.WriteLine("⚠️ Warning: Only use stackalloc for small buffers (< 1KB recommended)");
    }

    /// <summary>
    /// Example 5: Memory&lt;T&gt; for async scenarios
    /// </summary>
    public static void MemoryVsSpan()
    {
        Console.WriteLine("\n=== Memory<T> vs Span<T> ===");
        
        int[] data = { 1, 2, 3, 4, 5 };
        
        // Span - cannot be used in async methods or stored in fields
        Span<int> span = data;
        Console.WriteLine($"Span length: {span.Length}");
        
        // Memory - can be used in async methods and stored
        Memory<int> memory = data;
        Console.WriteLine($"Memory length: {memory.Length}");
        
        // Convert Memory to Span when needed
        Span<int> spanFromMemory = memory.Span;
        spanFromMemory[0] = 999;
        Console.WriteLine($"Modified through Memory: [{string.Join(", ", data)}]");
        
        Console.WriteLine("\nKey differences:");
        Console.WriteLine("  Span<T>   - ref struct, stack only, fastest, sync only");
        Console.WriteLine("  Memory<T> - struct, can be stored, async-friendly");
    }

    /// <summary>
    /// Example 6: ArrayPool for reusable buffers
    /// </summary>
    public static void ArrayPoolExample()
    {
        Console.WriteLine("\n=== ArrayPool for Buffer Reuse ===");
        
        // Rent array from pool (much faster than new[])
        int[] buffer = ArrayPool<int>.Shared.Rent(100);
        
        try
        {
            // Use only the portion you need
            Span<int> usableBuffer = buffer.AsSpan(0, 50);
            
            // Fill with data
            for (int i = 0; i < usableBuffer.Length; i++)
                usableBuffer[i] = i * 2;
            
            int sum = 0;
            foreach (int value in usableBuffer)
                sum += value;
            
            Console.WriteLine($"Processed {usableBuffer.Length} elements");
            Console.WriteLine($"Sum: {sum}");
            Console.WriteLine("\nKey insight: Rent() returns existing array, Return() makes it reusable");
        }
        finally
        {
            // Always return to pool
            ArrayPool<int>.Shared.Return(buffer, clearArray: true);
            Console.WriteLine("✓ Buffer returned to pool");
        }
    }

    /// <summary>
    /// Example 7: Real-world scenario - CSV parsing
    /// </summary>
    public static void CsvParsingExample()
    {
        Console.WriteLine("\n=== CSV Parsing with Span ===");
        
        string csvLine = "John,Doe,30,Engineer,75000";
        
        // Traditional approach - multiple string allocations
        string[] parts1 = csvLine.Split(',');
        Console.WriteLine($"Traditional: {parts1.Length} parts, multiple allocations");
        
        // Span approach - zero allocations
        ReadOnlySpan<char> line = csvLine.AsSpan();
        int partCount = 0;
        int start = 0;
        
        for (int i = 0; i <= line.Length; i++)
        {
            if (i == line.Length || line[i] == ',')
            {
                ReadOnlySpan<char> part = line.Slice(start, i - start);
                partCount++;
                
                if (partCount == 1) Console.Write($"Span: {part.ToString()}");
                else if (partCount == 2) Console.Write($", {part.ToString()}");
                else if (partCount == 3)
                {
                    int age = int.Parse(part);
                    Console.Write($", {age} years old");
                }
                
                start = i + 1;
            }
        }
        
        Console.WriteLine($"\nSpan: {partCount} parts, ZERO allocations!");
    }

    /// <summary>
    /// Example 8: When NOT to use Span
    /// </summary>
    public static void SpanLimitations()
    {
        Console.WriteLine("\n=== Span Limitations ===");
        
        Console.WriteLine("\n❌ Cannot use Span<T> in:");
        Console.WriteLine("  1. Async methods (use Memory<T>)");
        Console.WriteLine("  2. Class fields (use Memory<T>)");
        Console.WriteLine("  3. LINQ operations (use array/list)");
        Console.WriteLine("  4. Across await boundaries");
        Console.WriteLine("  5. Boxing scenarios (Span is ref struct)");
        
        Console.WriteLine("\n✅ Use Span<T> when:");
        Console.WriteLine("  1. Processing data in hot path");
        Console.WriteLine("  2. Slicing arrays without copying");
        Console.WriteLine("  3. Parsing strings efficiently");
        Console.WriteLine("  4. Working with stackalloc buffers");
        Console.WriteLine("  5. Interop with unmanaged code");
    }
}
