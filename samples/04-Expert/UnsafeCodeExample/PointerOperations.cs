using System.Runtime.CompilerServices;

namespace UnsafeCodeExample;

/// <summary>
/// Demonstrates pointer operations and unsafe code.
///
/// WHY UNSAFE CODE:
/// - Direct memory access (bypass bounds checking)
/// - Interop with native libraries
/// - Extreme performance scenarios
/// - Low-level system programming
///
/// RISKS:
/// - Memory corruption
/// - Buffer overflows
/// - Dangling pointers
/// - Security vulnerabilities
///
/// USE WITH CAUTION! ⚠️
/// </summary>
public static class PointerOperations
{
    /// <summary>
    /// Basic pointer arithmetic demonstration.
    /// Shows getting address, dereferencing, and modification through pointers.
    /// </summary>
    public static unsafe void DemonstratePointerArithmetic()
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

    /// <summary>
    /// Demonstrate pointer arithmetic with arrays.
    /// </summary>
    public static unsafe void DemonstratePointerArrayIteration()
    {
        Console.WriteLine("\n   Pointer Array Iteration:");

        int[] array = { 1, 2, 3, 4, 5 };

        fixed (int* basePtr = array)
        {
            for (int i = 0; i < array.Length; i++)
            {
                // Pointer arithmetic: basePtr + i
                int* currentPtr = basePtr + i;
                Console.WriteLine($"   array[{i}] = {*currentPtr} (address: 0x{((long)currentPtr):X})");
            }
        }
    }

    /// <summary>
    /// Demonstrate sizeof operator with pointers.
    /// </summary>
    public static unsafe void DemonstrateSizeOf()
    {
        Console.WriteLine("\n   Size of types:");
        Console.WriteLine($"   sizeof(byte):   {sizeof(byte)} bytes");
        Console.WriteLine($"   sizeof(int):    {sizeof(int)} bytes");
        Console.WriteLine($"   sizeof(long):   {sizeof(long)} bytes");
        Console.WriteLine($"   sizeof(double): {sizeof(double)} bytes");
        Console.WriteLine($"   sizeof(IntPtr): {sizeof(IntPtr)} bytes");
    }

    /// <summary>
    /// Swap two values using pointers (zero allocation).
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe void Swap(int* a, int* b)
    {
        int temp = *a;
        *a = *b;
        *b = temp;
    }

    /// <summary>
    /// Demonstrate swap with pointers.
    /// </summary>
    public static unsafe void DemonstrateSwap()
    {
        Console.WriteLine("\n   Swap using pointers:");

        int x = 10;
        int y = 20;

        Console.WriteLine($"   Before: x = {x}, y = {y}");

        Swap(&x, &y);

        Console.WriteLine($"   After:  x = {x}, y = {y}");
    }

    /// <summary>
    /// Find minimum value in array using pointer (fastest).
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe int FindMin(int* ptr, int length)
    {
        if (length == 0)
            throw new ArgumentException("Array cannot be empty");

        int min = *ptr;
        int* end = ptr + length;

        // Pointer iteration (faster than index access)
        while (++ptr < end)
        {
            if (*ptr < min)
                min = *ptr;
        }

        return min;
    }

    /// <summary>
    /// Copy memory from source to destination (like memcpy).
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe void MemoryCopy(int* source, int* destination, int count)
    {
        int* src = source;
        int* dst = destination;
        int* end = source + count;

        while (src < end)
        {
            *dst++ = *src++;  // Copy and increment pointers
        }
    }
}
