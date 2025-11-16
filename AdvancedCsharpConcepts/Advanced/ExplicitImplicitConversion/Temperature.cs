namespace AdvancedCsharpConcepts.Advanced.ExplicitImplicitConversion;

/// <summary>
/// Represents a temperature in Celsius with conversion operators.
/// Demonstrates implicit and explicit type conversion using user-defined conversion operators.
/// </summary>
public class Temperature
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Temperature"/> class.
    /// </summary>
    /// <param name="celsius">The temperature value in Celsius.</param>
    public Temperature(double celsius)
    {
        Celsius = celsius;
    }

    private double Celsius { get; }

    /// <summary>
    /// Implicitly converts a Temperature (Celsius) to TemperatureFahrenheit.
    /// </summary>
    /// <param name="celsius">The temperature in Celsius to convert.</param>
    /// <returns>A new TemperatureFahrenheit instance.</returns>
    public static implicit operator TemperatureFahrenheit(Temperature celsius)
    {
        return new TemperatureFahrenheit((celsius.Celsius * 9 / 5) + 32);
    }

    /// <summary>
    /// Explicitly converts a Temperature to its underlying double value in Celsius.
    /// </summary>
    /// <param name="celsius">The Temperature instance to convert.</param>
    /// <returns>The temperature value in Celsius.</returns>
    public static explicit operator double(Temperature celsius)
    {
        return celsius.Celsius;
    }

    /// <summary>
    /// Explicitly converts a double value to a Temperature instance.
    /// </summary>
    /// <param name="celsius">The Celsius value to convert.</param>
    /// <returns>A new Temperature instance.</returns>
    public static explicit operator Temperature(double celsius)
    {
        return new Temperature(celsius);
    }

    /// <summary>
    /// Returns a string representation of the temperature in Celsius.
    /// </summary>
    /// <returns>A string showing the temperature value with the °C symbol.</returns>
    public override string ToString()
    {
        return $"{Celsius} °C";
    }
}

/// <summary>
/// Represents a temperature in Fahrenheit.
/// </summary>
public class TemperatureFahrenheit
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TemperatureFahrenheit"/> class.
    /// </summary>
    /// <param name="fahrenheit">The temperature value in Fahrenheit.</param>
    public TemperatureFahrenheit(double fahrenheit)
    {
        Value = fahrenheit;
    }

    /// <summary>
    /// Gets the temperature value in Fahrenheit.
    /// </summary>
    public double Value { get; }

    /// <summary>
    /// Returns a string representation of the temperature in Fahrenheit.
    /// </summary>
    /// <returns>A string showing the temperature value with the °F symbol.</returns>
    public override string ToString()
    {
        return $"{Value} °F";
    }
}

/// <summary>
/// Represents a temperature in Celsius for conversion purposes.
/// </summary>
public class TemperatureCelsius
{
    private TemperatureCelsius(double celsius)
    {
        Value = celsius;
    }

    private double Value { get; }

    /// <summary>
    /// Implicitly converts a TemperatureFahrenheit to TemperatureCelsius.
    /// </summary>
    /// <param name="fahrenheit">The temperature in Fahrenheit to convert.</param>
    /// <returns>A new TemperatureCelsius instance.</returns>
    public static implicit operator TemperatureCelsius(TemperatureFahrenheit fahrenheit)
    {
        return new TemperatureCelsius((fahrenheit.Value - 32) * 5 / 9);
    }

    /// <summary>
    /// Returns a string representation of the temperature in Celsius.
    /// </summary>
    /// <returns>A string showing the temperature value with the °C symbol.</returns>
    public override string ToString()
    {
        return $"{Value} °C";
    }
}
