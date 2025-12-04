namespace AdvancedCsharpConcepts.Advanced.ExplicitImplicitConversion;

public class Temperature
{
    public Temperature(double celsius)
    {
        Celsius = celsius;
    }
    private double Celsius { get; }

    public static implicit operator TemperatureFahrenheit(Temperature celsius)
    {
        return new TemperatureFahrenheit(celsius.Celsius * 9 / 5 + 32);
    }

    public static explicit operator double(Temperature celsius)
    {
        return celsius.Celsius;
    }

    public static explicit operator Temperature(double celsius)
    {
        return new Temperature(celsius);
    }
}

public class TemperatureFahrenheit
{
    public TemperatureFahrenheit(double fahrenheit)
    {
        Value = fahrenheit;
    }

    public double Value { get; }

    public override string ToString()
    {
        return $"{Value} °F";
    }
}

public class TemperatureCelsius
{
    private TemperatureCelsius(double celsius)
    {
        Value = celsius;
    }

    private double Value { get; }

    public static implicit operator TemperatureCelsius(TemperatureFahrenheit fahrenheit)
    {
        return new TemperatureCelsius((fahrenheit.Value - 32) * 5 / 9);
    }

    public override string ToString()
    {
        return $"{Value} °C";
    }
}
