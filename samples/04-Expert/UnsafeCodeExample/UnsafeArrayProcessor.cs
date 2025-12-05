using System.Runtime.CompilerServices;

namespace UnsafeCodeExample;

/// <summary>
/// High-performance array processing using unsafe code.
///
/// PERFORMANCE BENEFITS:
/// - No bounds checking (2-3x faster)
/// - Direct memory access
/// - Cache-friendly iteration
/// - SIMD-friendly code patterns
///
/// WHEN TO USE:
/// - Performance-critical loops
/// - Large data processing
/// - Image/video processing
/// - Scientific computing
/// - Game engines
/// </summary>
public static class UnsafeArrayProcessor
{
    /// <summary>
    /// Demonstrate high-performance array sum.
    /// </summary>
    public static unsafe void DemonstrateArrayProcessing()
    {
        Console.WriteLine("\n2. High-Performance Array Sum");

        int[] array = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };

        var sum = SumArrayUnsafe(array);
        Console.WriteLine($"   Sum: {sum}");

        var min = FindMinUnsafe(array);
        Console.WriteLine($"   Min: {min}");

        var max = FindMaxUnsafe(array);
        Console.WriteLine($"   Max: {max}");
    }

    /// <summary>
    /// Sum array elements using unsafe pointer iteration.
    /// 2-3x faster than safe version due to eliminated bounds checks.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe int SumArrayUnsafe(int[] array)
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

    /// <summary>
    /// Find minimum value using unsafe code.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe int FindMinUnsafe(int[] array)
    {
        if (array.Length == 0)
            throw new ArgumentException("Array cannot be empty");

        fixed (int* ptr = array)
        {
            int min = *ptr;
            int* current = ptr + 1;
            int* end = ptr + array.Length;

            while (current < end)
            {
                if (*current < min)
                    min = *current;
                current++;
            }

            return min;
        }
    }

    /// <summary>
    /// Find maximum value using unsafe code.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe int FindMaxUnsafe(int[] array)
    {
        if (array.Length == 0)
            throw new ArgumentException("Array cannot be empty");

        fixed (int* ptr = array)
        {
            int max = *ptr;
            int* current = ptr + 1;
            int* end = ptr + array.Length;

            while (current < end)
            {
                if (*current > max)
                    max = *current;
                current++;
            }

            return max;
        }
    }

    /// <summary>
    /// Multiply all array elements by a scalar (in-place).
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe void MultiplyByScalar(int[] array, int scalar)
    {
        fixed (int* ptr = array)
        {
            int* current = ptr;
            int* end = ptr + array.Length;

            while (current < end)
            {
                *current *= scalar;
                current++;
            }
        }
    }

    /// <summary>
    /// Reverse array in-place using pointers.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe void ReverseArray(int[] array)
    {
        fixed (int* ptr = array)
        {
            int* left = ptr;
            int* right = ptr + array.Length - 1;

            while (left < right)
            {
                // Swap using pointers
                int temp = *left;
                *left = *right;
                *right = temp;

                left++;
                right--;
            }
        }
    }

    /// <summary>
    /// Calculate dot product of two arrays (vectorizable).
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe int DotProduct(int[] a, int[] b)
    {
        if (a.Length != b.Length)
            throw new ArgumentException("Arrays must have same length");

        fixed (int* ptrA = a)
        fixed (int* ptrB = b)
        {
            int result = 0;
            int* currentA = ptrA;
            int* currentB = ptrB;
            int* end = ptrA + a.Length;

            while (currentA < end)
            {
                result += (*currentA) * (*currentB);
                currentA++;
                currentB++;
            }

            return result;
        }
    }

    /// <summary>
    /// Copy array using unsafe code (memcpy-style).
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe void CopyArray(int[] source, int[] destination)
    {
        if (source.Length != destination.Length)
            throw new ArgumentException("Arrays must have same length");

        fixed (int* srcPtr = source)
        fixed (int* dstPtr = destination)
        {
            int* src = srcPtr;
            int* dst = dstPtr;
            int* end = srcPtr + source.Length;

            while (src < end)
            {
                *dst++ = *src++;
            }
        }
    }
}
