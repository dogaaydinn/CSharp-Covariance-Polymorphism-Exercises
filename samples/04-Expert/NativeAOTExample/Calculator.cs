namespace NativeAOTExample;

/// <summary>
/// AOT-friendly calculator with zero allocations.
/// All methods are statically compiled and inlined.
/// </summary>
public static class Calculator
{
    /// <summary>
    /// Calculate sum of array elements.
    /// Zero allocations, AOT-optimized.
    /// </summary>
    public static int CalculateSum(int[] numbers)
    {
        var total = 0;
        foreach (var n in numbers)
            total += n;
        return total;
    }

    /// <summary>
    /// Calculate product of array elements.
    /// Zero allocations, AOT-optimized.
    /// </summary>
    public static int CalculateProduct(int[] numbers)
    {
        var product = 1;
        foreach (var n in numbers)
            product *= n;
        return product;
    }

    /// <summary>
    /// Calculate average (with Span for zero allocation).
    /// </summary>
    public static double CalculateAverage(ReadOnlySpan<int> numbers)
    {
        if (numbers.Length == 0)
            return 0;

        var sum = 0;
        foreach (var n in numbers)
            sum += n;

        return (double)sum / numbers.Length;
    }

    /// <summary>
    /// Find maximum value in array.
    /// </summary>
    public static int FindMax(ReadOnlySpan<int> numbers)
    {
        if (numbers.Length == 0)
            throw new ArgumentException("Array cannot be empty");

        var max = numbers[0];
        for (var i = 1; i < numbers.Length; i++)
        {
            if (numbers[i] > max)
                max = numbers[i];
        }

        return max;
    }
}
