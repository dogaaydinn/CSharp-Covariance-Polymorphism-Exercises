using AdvancedCsharpConcepts.Advanced.ExplicitImplicitConversion;
using FluentAssertions;
using Xunit;

namespace AdvancedCsharpConcepts.Tests;

/// <summary>
/// Unit tests for temperature conversion using implicit and explicit operators
/// </summary>
public class TemperatureConversionTests
{
    [Fact]
    public void ImplicitConversion_CelsiusToFahrenheit_ConvertsCorrectly()
    {
        // Arrange
        var tempC = new Temperature(25); // 25°C

        // Act
        TemperatureFahrenheit tempF = tempC; // Implicit conversion

        // Assert
        tempF.Should().NotBeNull();
        tempF.Value.Should().BeApproximately(77, 0.01); // 25°C = 77°F
    }

    [Fact]
    public void ImplicitConversion_FahrenheitToCelsius_ConvertsCorrectly()
    {
        // Arrange
        var tempF = new TemperatureFahrenheit(77); // 77°F

        // Act
        TemperatureCelsius tempC = tempF; // Implicit conversion

        // Assert
        tempC.Should().NotBeNull();
        tempC.ToString().Should().Contain("25"); // 77°F = 25°C
    }

    [Fact]
    public void ExplicitConversion_TemperatureToDouble_ReturnsValue()
    {
        // Arrange
        var temp = new Temperature(100);

        // Act
        var value = (double)temp;

        // Assert
        value.Should().Be(100);
    }

    [Fact]
    public void ExplicitConversion_DoubleToTemperature_CreatesInstance()
    {
        // Arrange
        double value = 50.0;

        // Act
        var temp = (Temperature)value;

        // Assert
        temp.Should().NotBeNull();
        ((double)temp).Should().Be(50.0);
    }

    [Theory]
    [InlineData(0, 32)]
    [InlineData(100, 212)]
    [InlineData(-40, -40)]
    [InlineData(37, 98.6)]
    public void ImplicitConversion_CelsiusToFahrenheit_VariousTemperatures(double celsius, double expectedFahrenheit)
    {
        // Arrange
        var tempC = new Temperature(celsius);

        // Act
        TemperatureFahrenheit tempF = tempC;

        // Assert
        tempF.Value.Should().BeApproximately(expectedFahrenheit, 0.1);
    }

    [Fact]
    public void TemperatureFahrenheit_ToString_FormatsCorrectly()
    {
        // Arrange
        var tempF = new TemperatureFahrenheit(98.6);

        // Act
        var result = tempF.ToString();

        // Assert
        result.Should().Contain("98.6");
        result.Should().Contain("°F");
    }
}
