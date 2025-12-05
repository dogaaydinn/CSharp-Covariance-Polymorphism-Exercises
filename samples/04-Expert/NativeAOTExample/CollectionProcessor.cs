namespace NativeAOTExample;

/// <summary>
/// AOT-friendly collection processor using Span&lt;T&gt;.
/// Zero allocations, high performance.
/// </summary>
public static class CollectionProcessor
{
    /// <summary>
    /// Process items using Span for zero-allocation iteration.
    /// Native AOT can fully optimize this code.
    /// </summary>
    public static void ProcessItemsWithSpan(string[] items)
    {
        ReadOnlySpan<string> span = items;

        Console.WriteLine($"   Processing {span.Length} items:");
        for (var i = 0; i < span.Length; i++)
        {
            Console.WriteLine($"   - {span[i]} ({span[i].Length} chars)");
        }
    }

    /// <summary>
    /// Filter items by length using Span.
    /// Returns count (not list) to avoid allocation.
    /// </summary>
    public static int CountItemsLongerThan(ReadOnlySpan<string> items, int minLength)
    {
        var count = 0;
        for (var i = 0; i < items.Length; i++)
        {
            if (items[i].Length > minLength)
                count++;
        }
        return count;
    }

    /// <summary>
    /// Process integers with Span for maximum performance.
    /// </summary>
    public static void ProcessNumbers(ReadOnlySpan<int> numbers)
    {
        Console.WriteLine($"\n   Processing {numbers.Length} numbers:");

        var sum = 0;
        var max = numbers[0];
        var min = numbers[0];

        for (var i = 0; i < numbers.Length; i++)
        {
            sum += numbers[i];
            if (numbers[i] > max) max = numbers[i];
            if (numbers[i] < min) min = numbers[i];
        }

        Console.WriteLine($"   Sum: {sum}");
        Console.WriteLine($"   Max: {max}");
        Console.WriteLine($"   Min: {min}");
        Console.WriteLine($"   Average: {(double)sum / numbers.Length:F2}");
    }

    /// <summary>
    /// Transform strings in place (zero allocation).
    /// </summary>
    public static void TransformToUpperCase(Span<string> items)
    {
        for (var i = 0; i < items.Length; i++)
        {
            items[i] = items[i].ToUpperInvariant();
        }
    }
}
