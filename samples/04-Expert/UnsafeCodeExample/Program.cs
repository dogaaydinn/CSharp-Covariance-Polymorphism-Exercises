using System;
using System.Runtime.CompilerServices;

namespace UnsafeCodeExample;

/// <summary>
/// Demonstrates unsafe code and pointer arithmetic for high-performance scenarios.
/// </summary>
class Program
{
    static void Main()
    {
        Console.WriteLine("=== Unsafe Code Example ===\n");

        // 1. Pointer arithmetic
        DemonstratePointerArithmetic();

        // 2. High-performance array processing
        DemonstrateArrayProcessing();

        // 3. stackalloc for stack-based arrays
        DemonstrateStackAlloc();

        Console.WriteLine("\n✅ Unsafe code demonstration complete!");
    }

    static unsafe void DemonstratePointerArithmetic()
    {
        Console.WriteLine("1. Pointer Arithmetic");

        int value = 42;
        int* ptr = &value;  // Get pointer to value

        Console.WriteLine($"   Value: {value}");
        Console.WriteLine($"   Pointer address: 0x{((long)ptr):X}");
        Console.WriteLine($"   Dereferenced: {*ptr}");

        *ptr = 100;  // Modify through pointer
        Console.WriteLine($"   After modification: {value}");
    }

    static unsafe void DemonstrateArrayProcessing()
    {
        Console.WriteLine("\n2. High-Performance Array Sum");

        int[] array = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };

        var sum = SumArrayUnsafe(array);
        Console.WriteLine($"   Sum: {sum}");
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static unsafe int SumArrayUnsafe(int[] array)
    {
        fixed (int* ptr = array)  // Pin array in memory
        {
            int sum = 0;
            int* current = ptr;
            int* end = ptr + array.Length;

            while (current < end)
            {
                sum += *current;
                current++;  // Pointer arithmetic
            }

            return sum;
        }
    }

    static unsafe void DemonstrateStackAlloc()
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
}
