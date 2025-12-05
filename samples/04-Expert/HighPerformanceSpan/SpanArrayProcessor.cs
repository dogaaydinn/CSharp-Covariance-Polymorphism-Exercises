namespace HighPerformanceSpan;

/// <summary>
/// High-performance array operations using Span&lt;T&gt;.
///
/// SPAN vs ARRAY:
/// - Span<T>: Stack-based view, zero allocation
/// - T[]: Heap-based storage, GC overhead
///
/// BENEFITS:
/// - Slice without copying (O(1))
/// - Stack allocation possible
/// - Unified API (array, stack, native memory)
/// - Type-safe and bounds-checked
/// </summary>
public static class SpanArrayProcessor
{
    /// <summary>
    /// Demonstrate array slicing without copying.
    /// </summary>
    public static void DemonstrateArraySlicing()
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

    /// <summary>
    /// Sum array elements using Span (zero allocation).
    /// </summary>
    public static int SumArray(ReadOnlySpan<int> numbers)
    {
        int sum = 0;
        for (int i = 0; i < numbers.Length; i++)
        {
            sum += numbers[i];
        }
        return sum;
    }

    /// <summary>
    /// Find maximum value in array using Span.
    /// </summary>
    public static int FindMax(ReadOnlySpan<int> numbers)
    {
        if (numbers.Length == 0)
            throw new ArgumentException("Array cannot be empty");

        int max = numbers[0];
        for (int i = 1; i < numbers.Length; i++)
        {
            if (numbers[i] > max)
                max = numbers[i];
        }

        return max;
    }

    /// <summary>
    /// Reverse array in-place using Span.
    /// </summary>
    public static void ReverseArray(Span<int> numbers)
    {
        for (int i = 0; i < numbers.Length / 2; i++)
        {
            int j = numbers.Length - 1 - i;
            (numbers[i], numbers[j]) = (numbers[j], numbers[i]);
        }
    }

    /// <summary>
    /// Copy array slice using Span (zero extra allocation).
    /// </summary>
    public static void CopySlice(ReadOnlySpan<int> source, Span<int> destination)
    {
        if (source.Length > destination.Length)
            throw new ArgumentException("Destination too small");

        source.CopyTo(destination);
    }

    /// <summary>
    /// Fill array with value using Span.
    /// </summary>
    public static void FillArray(Span<int> array, int value)
    {
        array.Fill(value);
    }

    /// <summary>
    /// Check if two arrays are equal using Span.
    /// </summary>
    public static bool ArraysEqual(ReadOnlySpan<int> a, ReadOnlySpan<int> b)
    {
        return a.SequenceEqual(b);
    }

    /// <summary>
    /// Find index of element using Span.
    /// </summary>
    public static int IndexOf(ReadOnlySpan<int> array, int value)
    {
        for (int i = 0; i < array.Length; i++)
        {
            if (array[i] == value)
                return i;
        }
        return -1;
    }

    /// <summary>
    /// Calculate moving average using Span (zero allocation).
    /// </summary>
    public static void CalculateMovingAverage(ReadOnlySpan<double> data, Span<double> result, int windowSize)
    {
        if (result.Length != data.Length - windowSize + 1)
            throw new ArgumentException("Invalid result array size");

        for (int i = 0; i <= data.Length - windowSize; i++)
        {
            double sum = 0;
            var window = data.Slice(i, windowSize);

            for (int j = 0; j < window.Length; j++)
            {
                sum += window[j];
            }

            result[i] = sum / windowSize;
        }
    }

    /// <summary>
    /// Process byte array as integers (reinterpret cast).
    /// Uses MemoryMarshal for safe type reinterpretation.
    /// </summary>
    public static void ReinterpretBytes(ReadOnlySpan<byte> bytes)
    {
        if (bytes.Length < 4)
            return;

        // Reinterpret bytes as integers (zero copy!)
        var integers = System.Runtime.InteropServices.MemoryMarshal.Cast<byte, int>(bytes);

        Console.WriteLine($"\n   {bytes.Length} bytes → {integers.Length} integers:");
        for (int i = 0; i < integers.Length; i++)
        {
            Console.WriteLine($"   [{i}] = {integers[i]}");
        }
    }

    /// <summary>
    /// Sort array slice in-place using Span.
    /// </summary>
    public static void SortSlice(Span<int> slice)
    {
        // Use built-in Sort (optimized)
        slice.Sort();
    }
}
