namespace AdvancedCsharpConcepts.Advanced.ExplicitImplicitConversion;

/// <summary>
/// Demonstrates explicit and implicit type conversion in C#.
/// Shows how to convert between different numeric types with potential data loss.
/// </summary>
/// <remarks>
/// Explicit conversions require a cast operator and may result in data loss.
/// Implicit conversions happen automatically when no data loss can occur.
///
/// Examples:
/// - double to int: Explicit (loses decimal portion)
/// - int to double: Implicit (no data loss)
/// - double to float: Explicit (may lose precision)
/// </remarks>
public class ExplicitImplicitConversion
{
    /// <summary>
    /// Demonstrates explicit type conversions between numeric types.
    /// Shows data loss when converting from double to int.
    /// </summary>
    /// <example>
    /// <code>
    /// var converter = new ExplicitImplicitConversion();
    /// converter.ConversionExample();
    /// // Output:
    /// // Original double: 10.5
    /// // Explicitly converted to int: 10      (decimal portion lost)
    /// // Explicitly converted to float: 10.5
    /// </code>
    /// </example>
    public void ConversionExample()
    {
        double x = 10.5;
        int y = (int)x; // Explicit conversion - loses decimal portion
        float z = (float)x; // Explicit conversion - may lose precision for very large/small values

        Console.WriteLine($"Original double: {x}");
        Console.WriteLine($"Explicitly converted to int: {y}"); // y = 10 (decimal truncated)
        Console.WriteLine($"Explicitly converted to float: {z}"); // z = 10.5
    }
}
