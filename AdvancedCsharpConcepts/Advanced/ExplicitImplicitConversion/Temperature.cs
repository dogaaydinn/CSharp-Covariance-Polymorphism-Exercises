namespace AdvancedCsharpConcepts.Advanced.ExplicitImplicitConversion;

/// <summary>
/// Represents a temperature value in Celsius.
/// Demonstrates implicit and explicit conversion operators.
/// </summary>
public class Temperature
{
    /// <summary>
    /// Conversion factor for Celsius to Fahrenheit (9/5 = 1.8).
    /// </summary>
    private const double CelsiusToFahrenheitMultiplier = 9.0 / 5.0;

    /// <summary>
    /// Offset for Fahrenheit conversion (+32).
    /// </summary>
    private const double FahrenheitOffset = 32.0;

    /// <summary>
    /// Conversion factor for Fahrenheit to Celsius (5/9).
    /// </summary>
    private const double FahrenheitToCelsiusMultiplier = 5.0 / 9.0;

    /// <summary>
    /// Initializes a new instance of the <see cref="Temperature"/> class.
    /// </summary>
    /// <param name="celsius">The temperature value in Celsius.</param>
    public Temperature(double celsius)
    {
        Celsius = celsius;
    }

    /// <summary>
    /// Gets the temperature value in Celsius.
    /// </summary>
    private double Celsius { get; }

    /// <summary>
    /// Implicitly converts Temperature (Celsius) to Fahrenheit.
    /// Implicit conversion is safe as no data loss occurs.
    /// </summary>
    /// <param name="celsius">The temperature in Celsius.</param>
    /// <returns>The temperature in Fahrenheit.</returns>
    public static implicit operator TemperatureFahrenheit(Temperature celsius)
    {
        return new TemperatureFahrenheit(celsius.Celsius * CelsiusToFahrenheitMultiplier + FahrenheitOffset);
    }

    /// <summary>
    /// Explicitly converts Temperature to double (Celsius value).
    /// Explicit because it loses the Temperature type context.
    /// </summary>
    /// <param name="celsius">The temperature object.</param>
    /// <returns>The raw Celsius value.</returns>
    public static explicit operator double(Temperature celsius)
    {
        return celsius.Celsius;
    }

    /// <summary>
    /// Explicitly converts double to Temperature (Celsius).
    /// Explicit to make it clear that we're treating the double as Celsius.
    /// </summary>
    /// <param name="celsius">The Celsius value.</param>
    /// <returns>A Temperature object.</returns>
    public static explicit operator Temperature(double celsius)
    {
        return new Temperature(celsius);
    }

    /// <summary>
    /// Returns a string representation of the temperature.
    /// </summary>
    /// <returns>The temperature in Celsius with unit.</returns>
    public override string ToString()
    {
        return $"{Celsius} °C";
    }
}

/// <summary>
/// Represents a temperature value in Fahrenheit.
/// </summary>
public class TemperatureFahrenheit
{
    /// <summary>
    /// Conversion factor for Fahrenheit to Celsius (5/9).
    /// </summary>
    private const double FahrenheitToCelsiusMultiplier = 5.0 / 9.0;

    /// <summary>
    /// Offset for Fahrenheit conversion (-32).
    /// </summary>
    private const double FahrenheitOffset = 32.0;

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
    /// Returns a string representation of the temperature.
    /// </summary>
    /// <returns>The temperature in Fahrenheit with unit.</returns>
    public override string ToString()
    {
        return $"{Value} °F";
    }
}

/// <summary>
/// Represents a temperature value in Celsius (alternative implementation).
/// Demonstrates implicit conversion from Fahrenheit.
/// </summary>
public class TemperatureCelsius
{
    /// <summary>
    /// Conversion factor for Fahrenheit to Celsius (5/9).
    /// </summary>
    private const double FahrenheitToCelsiusMultiplier = 5.0 / 9.0;

    /// <summary>
    /// Offset for Fahrenheit conversion (-32).
    /// </summary>
    private const double FahrenheitOffset = 32.0;

    /// <summary>
    /// Initializes a new instance of the <see cref="TemperatureCelsius"/> class.
    /// Private to enforce creation only through implicit operator.
    /// </summary>
    /// <param name="celsius">The temperature value in Celsius.</param>
    private TemperatureCelsius(double celsius)
    {
        Value = celsius;
    }

    /// <summary>
    /// Gets the temperature value in Celsius.
    /// </summary>
    private double Value { get; }

    /// <summary>
    /// Implicitly converts Fahrenheit to Celsius.
    /// Implicit conversion is appropriate as it's a common, safe operation.
    /// </summary>
    /// <param name="fahrenheit">The temperature in Fahrenheit.</param>
    /// <returns>The temperature in Celsius.</returns>
    public static implicit operator TemperatureCelsius(TemperatureFahrenheit fahrenheit)
    {
        return new TemperatureCelsius((fahrenheit.Value - FahrenheitOffset) * FahrenheitToCelsiusMultiplier);
    }

    /// <summary>
    /// Returns a string representation of the temperature.
    /// </summary>
    /// <returns>The temperature in Celsius with unit.</returns>
    public override string ToString()
    {
        return $"{Value} °C";
    }
}
