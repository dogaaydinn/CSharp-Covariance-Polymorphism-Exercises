using FluentAssertions;
using Xunit;

namespace FactoryPattern;

public class VehicleFactoryTests
{
    [Fact]
    public void CreateVehicle_Car_ReturnsCarInstance()
    {
        // Act
        var vehicle = VehicleFactory.CreateVehicle(VehicleType.Car);

        // Assert
        vehicle.Should().BeOfType<Car>();
        vehicle.GetWheelCount().Should().Be(4);
        vehicle.GetDescription().Should().Contain("Car");
    }

    [Fact]
    public void CreateVehicle_Motorcycle_ReturnsMotorcycleInstance()
    {
        // Act
        var vehicle = VehicleFactory.CreateVehicle(VehicleType.Motorcycle);

        // Assert
        vehicle.Should().BeOfType<Motorcycle>();
        vehicle.GetWheelCount().Should().Be(2);
        vehicle.GetDescription().Should().Contain("Motorcycle");
    }

    [Fact]
    public void CreateVehicle_Truck_ReturnsTruckInstance()
    {
        // Act
        var vehicle = VehicleFactory.CreateVehicle(VehicleType.Truck);

        // Assert
        vehicle.Should().BeOfType<Truck>();
        vehicle.GetWheelCount().Should().Be(18);
        vehicle.GetDescription().Should().Contain("Truck");
    }

    [Fact]
    public void CreateVehicle_InvalidType_ThrowsException()
    {
        // Act & Assert
        var act = () => VehicleFactory.CreateVehicle((VehicleType)999);
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void CreateVehicle_ReturnsIVehicleInterface()
    {
        // Act
        IVehicle car = VehicleFactory.CreateVehicle(VehicleType.Car);
        IVehicle motorcycle = VehicleFactory.CreateVehicle(VehicleType.Motorcycle);
        IVehicle truck = VehicleFactory.CreateVehicle(VehicleType.Truck);

        // Assert - All should implement IVehicle
        car.Should().BeAssignableTo<IVehicle>();
        motorcycle.Should().BeAssignableTo<IVehicle>();
        truck.Should().BeAssignableTo<IVehicle>();
    }
}
