using AdvancedCsharpConcepts.Advanced.ModernCSharp;
using FluentAssertions;

namespace AdvancedCsharpConcepts.Tests.ModernCSharp;

public class PrimaryConstructorsTests
{
    [Fact]
    public void VehicleModern_ShouldInitializeWithBrandAndYear()
    {
        // Arrange & Act
        var vehicle = new PrimaryConstructorsExample.VehicleModern("Tesla", 2024);

        // Assert
        vehicle.Brand.Should().Be("Tesla");
        vehicle.Year.Should().Be(2024);
    }

    [Fact]
    public void ElectricVehicle_ShouldCalculateRangeCorrectly()
    {
        // Arrange
        const int batteryCapacity = 100;
        const double expectedRange = 500.0; // 100 kWh * 5.0 km/kWh

        // Act
        var ev = new PrimaryConstructorsExample.ElectricVehicle("Tesla Model S", 2024, batteryCapacity);

        // Assert
        ev.Range.Should().Be(expectedRange);
    }

    [Fact]
    public void ElectricVehicle_ShouldThrowException_WhenBatteryCapacityIsNegative()
    {
        // Arrange & Act
        Action act = () => new PrimaryConstructorsExample.ElectricVehicle("Tesla", 2024, -10);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("Battery capacity must be positive*");
    }

    [Fact]
    public void RecordEquality_ShouldWorkCorrectly()
    {
        // Arrange
        var ev1 = new PrimaryConstructorsExample.ElectricVehicle("Tesla Model S", 2024, 100);
        var ev2 = new PrimaryConstructorsExample.ElectricVehicle("Tesla Model S", 2024, 100);
        var ev3 = new PrimaryConstructorsExample.ElectricVehicle("Tesla Model 3", 2024, 75);

        // Assert
        ev1.Should().Be(ev2); // Value equality
        ev1.Should().NotBe(ev3);
    }
}
