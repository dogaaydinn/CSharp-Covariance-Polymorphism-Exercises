namespace AdvancedCsharpConcepts.Advanced.ExplicitImplicitConversion;

public class ExplicitImplicitConversion
{
    public void ConversionExample()
    {
        double x = 10.5;
        int y = (int)x; // Explicit conversion
        float z = (float)x; // Explicit conversion

        Console.WriteLine($"Original double: {x}");
        Console.WriteLine($"Explicitly converted to int: {y}"); // y = 10
        Console.WriteLine($"Explicitly converted to float: {z}"); // z = 10.5
    }
}