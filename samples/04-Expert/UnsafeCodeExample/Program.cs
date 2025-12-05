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
        PointerOperations.DemonstratePointerArithmetic();

        // 2. High-performance array processing
        UnsafeArrayProcessor.DemonstrateArrayProcessing();

        // 3. stackalloc for stack-based arrays
        StackAllocator.DemonstrateStackAlloc();

        Console.WriteLine("\n✅ Unsafe code demonstration complete!");
    }
}
