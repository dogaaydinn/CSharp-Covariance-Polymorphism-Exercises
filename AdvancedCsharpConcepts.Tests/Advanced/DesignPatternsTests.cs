using AdvancedCsharpConcepts.Advanced.DesignPatterns;
using FluentAssertions;
using Xunit;

namespace AdvancedCsharpConcepts.Tests.Advanced;

/// <summary>
/// Comprehensive tests for Design Patterns including exception scenarios.
/// Tests Factory and Builder patterns for both happy paths and edge cases.
/// </summary>
public class DesignPatternsTests
{
    #region Factory Pattern Tests

    [Fact]
    public void SimpleFactory_CreateCar_ShouldSucceed()
    {
        // Arrange & Act
        var car = FactoryPattern.VehicleFactory.CreateVehicle(
            FactoryPattern.VehicleType.Car,
            "Tesla Model S");

        // Assert
        car.Should().NotBeNull();
        car.Should().BeOfType<FactoryPattern.Car>();
        car.GetDescription().Should().Contain("Tesla Model S");
        car.GetWheelCount().Should().Be(4);
    }

    [Fact]
    public void SimpleFactory_CreateMotorcycle_ShouldSucceed()
    {
        // Arrange & Act
        var motorcycle = FactoryPattern.VehicleFactory.CreateVehicle(
            FactoryPattern.VehicleType.Motorcycle,
            "Harley Davidson");

        // Assert
        motorcycle.Should().NotBeNull();
        motorcycle.Should().BeOfType<FactoryPattern.Motorcycle>();
        motorcycle.GetDescription().Should().Contain("Harley Davidson");
        motorcycle.GetWheelCount().Should().Be(2);
    }

    [Fact]
    public void SimpleFactory_CreateTruck_ShouldSucceed()
    {
        // Arrange & Act
        var truck = FactoryPattern.VehicleFactory.CreateVehicle(
            FactoryPattern.VehicleType.Truck,
            "20");

        // Assert
        truck.Should().NotBeNull();
        truck.Should().BeOfType<FactoryPattern.Truck>();
        truck.GetDescription().Should().Contain("20 tons");
        truck.GetWheelCount().Should().Be(6);
    }

    [Fact]
    public void SimpleFactory_CreateTruck_WithInvalidCapacity_ShouldThrow()
    {
        // Arrange & Act
        Action act = () => FactoryPattern.VehicleFactory.CreateVehicle(
            FactoryPattern.VehicleType.Truck,
            "invalid");

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("Invalid truck capacity: 'invalid'. Must be a valid integer.*");
    }

    [Fact]
    public void SimpleFactory_CreateTruck_WithNegativeCapacity_ShouldThrow()
    {
        // Arrange & Act
        Action act = () => FactoryPattern.VehicleFactory.CreateVehicle(
            FactoryPattern.VehicleType.Truck,
            "-10");

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("Invalid truck capacity: -10. Must be greater than zero.*");
    }

    [Fact]
    public void SimpleFactory_CreateTruck_WithZeroCapacity_ShouldThrow()
    {
        // Arrange & Act
        Action act = () => FactoryPattern.VehicleFactory.CreateVehicle(
            FactoryPattern.VehicleType.Truck,
            "0");

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("Invalid truck capacity: 0. Must be greater than zero.*");
    }

    [Fact]
    public void SimpleFactory_CreateCar_WithEmptyModel_ShouldThrow()
    {
        // Arrange & Act
        Action act = () => FactoryPattern.VehicleFactory.CreateVehicle(
            FactoryPattern.VehicleType.Car,
            "");

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithMessage("*Car model cannot be null or empty*");
    }

    [Fact]
    public void SimpleFactory_CreateMotorcycle_WithEmptyBrand_ShouldThrow()
    {
        // Arrange & Act
        Action act = () => FactoryPattern.VehicleFactory.CreateVehicle(
            FactoryPattern.VehicleType.Motorcycle,
            "");

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithMessage("*Motorcycle brand cannot be null or empty*");
    }

    [Theory]
    [InlineData("Tesla Model 3")]
    [InlineData("BMW M3")]
    [InlineData("Audi RS6")]
    public void SimpleFactory_CreateMultipleCars_ShouldSucceed(string model)
    {
        // Arrange & Act
        var car = FactoryPattern.VehicleFactory.CreateVehicle(
            FactoryPattern.VehicleType.Car,
            model);

        // Assert
        car.Should().BeOfType<FactoryPattern.Car>();
        car.GetDescription().Should().Contain(model);
    }

    [Fact]
    public void GenericFactory_CreateCar_ShouldSucceed()
    {
        // Arrange & Act
        var car = FactoryPattern.GenericVehicleFactory.CreateVehicle<FactoryPattern.Car>("Porsche 911");

        // Assert
        car.Should().NotBeNull();
        car.Should().BeOfType<FactoryPattern.Car>();
        car.Model.Should().Be("Porsche 911");
    }

    [Fact]
    public void GenericFactory_CreateMotorcycle_ShouldSucceed()
    {
        // Arrange & Act
        var motorcycle = FactoryPattern.GenericVehicleFactory.CreateVehicle<FactoryPattern.Motorcycle>("Ducati");

        // Assert
        motorcycle.Should().NotBeNull();
        motorcycle.Brand.Should().Be("Ducati");
    }

    [Fact]
    public void GenericFactory_CreateTruck_ShouldSucceed()
    {
        // Arrange & Act
        var truck = FactoryPattern.GenericVehicleFactory.CreateVehicle<FactoryPattern.Truck>(25);

        // Assert
        truck.Should().NotBeNull();
        truck.Capacity.Should().Be(25);
    }

    [Fact]
    public void GenericFactory_CreateWithWrongArguments_ShouldThrow()
    {
        // Arrange & Act
        Action act = () => FactoryPattern.GenericVehicleFactory.CreateVehicle<FactoryPattern.Car>();

        // Assert
        act.Should().Throw<MissingMethodException>();
    }

    [Fact]
    public void FactoryMethod_CarCreator_ShouldCreateCar()
    {
        // Arrange
        var creator = new FactoryPattern.CarCreator("Mercedes-Benz");

        // Act
        var vehicle = creator.CreateVehicle();

        // Assert
        vehicle.Should().NotBeNull();
        vehicle.Should().BeOfType<FactoryPattern.Car>();
        vehicle.GetDescription().Should().Contain("Mercedes-Benz");
    }

    [Fact]
    public void FactoryMethod_MotorcycleCreator_ShouldCreateMotorcycle()
    {
        // Arrange
        var creator = new FactoryPattern.MotorcycleCreator("Kawasaki");

        // Act
        var vehicle = creator.CreateVehicle();

        // Assert
        vehicle.Should().NotBeNull();
        vehicle.Should().BeOfType<FactoryPattern.Motorcycle>();
        vehicle.GetDescription().Should().Contain("Kawasaki");
    }

    #endregion

    #region Builder Pattern Tests

    [Fact]
    public void Builder_BuildCompletePCsharputer_ShouldSucceed()
    {
        // Arrange & Act
        var computer = new BuilderPattern.ComputerBuilder()
            .WithCPU("Intel Core i9-13900K")
            .WithMotherboard("ASUS ROG")
            .WithRAM(32)
            .WithGPU("NVIDIA RTX 4090")
            .WithStorage(2000, ssd: true)
            .WithWifi()
            .WithCooling("Liquid Cooling")
            .WithPowerSupply(1000)
            .Build();

        // Assert
        computer.Should().NotBeNull();
        computer.CPU.Should().Be("Intel Core i9-13900K");
        computer.Motherboard.Should().Be("ASUS ROG");
        computer.RAM.Should().Be(32);
        computer.GPU.Should().Be("NVIDIA RTX 4090");
        computer.Storage.Should().Be(2000);
        computer.HasSSD.Should().BeTrue();
        computer.HasWifi.Should().BeTrue();
        computer.CoolingSystem.Should().Be("Liquid Cooling");
        computer.PowerSupply.Should().Be(1000);
    }

    [Fact]
    public void Builder_BuildMinimalComputer_ShouldSucceed()
    {
        // Arrange & Act
        var computer = new BuilderPattern.ComputerBuilder()
            .WithCPU("Intel Core i5")
            .WithMotherboard("MSI B660")
            .WithRAM(16)
            .Build();

        // Assert
        computer.Should().NotBeNull();
        computer.CPU.Should().Be("Intel Core i5");
        computer.Motherboard.Should().Be("MSI B660");
        computer.RAM.Should().Be(16);
        computer.GPU.Should().BeNull();
        computer.Storage.Should().BeNull();
        computer.HasWifi.Should().BeFalse();
        computer.PowerSupply.Should().Be(500); // Default value
    }

    [Fact]
    public void Builder_BuildWithoutCPU_ShouldThrow()
    {
        // Arrange
        var builder = new BuilderPattern.ComputerBuilder()
            .WithMotherboard("ASUS")
            .WithRAM(16);

        // Act
        Action act = () => builder.Build();

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("CPU is required");
    }

    [Fact]
    public void Builder_BuildWithoutMotherboard_ShouldThrow()
    {
        // Arrange
        var builder = new BuilderPattern.ComputerBuilder()
            .WithCPU("Intel Core i7")
            .WithRAM(16);

        // Act
        Action act = () => builder.Build();

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("Motherboard is required");
    }

    [Fact]
    public void Builder_BuildWithoutRAM_ShouldThrow()
    {
        // Arrange
        var builder = new BuilderPattern.ComputerBuilder()
            .WithCPU("Intel Core i7")
            .WithMotherboard("ASUS");

        // Act
        Action act = () => builder.Build();

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("RAM is required");
    }

    [Fact]
    public void Builder_WithNegativeRAM_ShouldThrow()
    {
        // Arrange
        var builder = new BuilderPattern.ComputerBuilder();

        // Act
        Action act = () => builder.WithRAM(-8);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("RAM must be positive*")
            .And.ParamName.Should().Be("ram");
    }

    [Fact]
    public void Builder_WithZeroRAM_ShouldThrow()
    {
        // Arrange
        var builder = new BuilderPattern.ComputerBuilder();

        // Act
        Action act = () => builder.WithRAM(0);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("RAM must be positive*");
    }

    [Fact]
    public void Builder_WithNegativeStorage_ShouldThrow()
    {
        // Arrange
        var builder = new BuilderPattern.ComputerBuilder();

        // Act
        Action act = () => builder.WithStorage(-500);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("Storage must be positive*")
            .And.ParamName.Should().Be("storage");
    }

    [Fact]
    public void Builder_WithLowPowerSupply_ShouldThrow()
    {
        // Arrange
        var builder = new BuilderPattern.ComputerBuilder();

        // Act
        Action act = () => builder.WithPowerSupply(250);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("Power supply must be at least 300W*")
            .And.ParamName.Should().Be("watts");
    }

    [Theory]
    [InlineData(300)]
    [InlineData(500)]
    [InlineData(750)]
    [InlineData(1000)]
    public void Builder_WithValidPowerSupply_ShouldSucceed(int watts)
    {
        // Arrange & Act
        var computer = new BuilderPattern.ComputerBuilder()
            .WithCPU("AMD Ryzen 9")
            .WithMotherboard("MSI X670")
            .WithRAM(32)
            .WithPowerSupply(watts)
            .Build();

        // Assert
        computer.PowerSupply.Should().Be(watts);
    }

    [Fact]
    public void Builder_FluentAPI_ShouldChain()
    {
        // Arrange & Act
        var result = new BuilderPattern.ComputerBuilder()
            .WithCPU("Test")
            .WithMotherboard("Test")
            .WithRAM(8)
            .WithGPU("Test GPU")
            .WithWifi()
            .WithCooling("Air");

        // Assert
        result.Should().BeOfType<BuilderPattern.ComputerBuilder>();
        result.Should().NotBeNull();
    }

    #endregion

    #region Modern Builder Pattern Tests

    [Fact]
    public void ModernBuilder_BuildCompleteServerConfig_ShouldSucceed()
    {
        // Arrange & Act
        var config = BuilderPattern.ServerConfig.Builder
            .WithServerName("WebAPI-Prod")
            .WithPort(8080)
            .WithHost("api.example.com")
            .WithSSL()
            .WithMaxConnections(500)
            .WithTimeout(60)
            .WithLogging("/var/log/api.log")
            .Build();

        // Assert
        config.Should().NotBeNull();
        config.ServerName.Should().Be("WebAPI-Prod");
        config.Port.Should().Be(8080);
        config.Host.Should().Be("api.example.com");
        config.UseSSL.Should().BeTrue();
        config.MaxConnections.Should().Be(500);
        config.TimeoutSeconds.Should().Be(60);
        config.LogPath.Should().Be("/var/log/api.log");
    }

    [Fact]
    public void ModernBuilder_BuildMinimalServerConfig_ShouldSucceed()
    {
        // Arrange & Act
        var config = BuilderPattern.ServerConfig.Builder
            .WithServerName("TestServer")
            .WithPort(3000)
            .Build();

        // Assert
        config.Should().NotBeNull();
        config.ServerName.Should().Be("TestServer");
        config.Port.Should().Be(3000);
        config.Host.Should().Be("localhost"); // Default
        config.UseSSL.Should().BeFalse();
        config.MaxConnections.Should().Be(100); // Default
        config.TimeoutSeconds.Should().Be(30); // Default
        config.LogPath.Should().BeNull();
    }

    [Fact]
    public void ModernBuilder_BuildWithoutServerName_ShouldThrow()
    {
        // Arrange
        var builder = BuilderPattern.ServerConfig.Builder
            .WithPort(8080);

        // Act
        Action act = () => builder.Build();

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("Server name is required");
    }

    [Fact]
    public void ModernBuilder_BuildWithoutPort_ShouldThrow()
    {
        // Arrange
        var builder = BuilderPattern.ServerConfig.Builder
            .WithServerName("TestServer");

        // Act
        Action act = () => builder.Build();

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("Invalid port number");
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(65536)]
    [InlineData(70000)]
    public void ModernBuilder_BuildWithInvalidPort_ShouldThrow(int port)
    {
        // Arrange
        var builder = BuilderPattern.ServerConfig.Builder
            .WithServerName("TestServer")
            .WithPort(port);

        // Act
        Action act = () => builder.Build();

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("Invalid port number");
    }

    [Theory]
    [InlineData(1)]
    [InlineData(80)]
    [InlineData(443)]
    [InlineData(8080)]
    [InlineData(65535)]
    public void ModernBuilder_BuildWithValidPort_ShouldSucceed(int port)
    {
        // Arrange & Act
        var config = BuilderPattern.ServerConfig.Builder
            .WithServerName("TestServer")
            .WithPort(port)
            .Build();

        // Assert
        config.Port.Should().Be(port);
    }

    [Fact]
    public void ModernBuilder_FluentAPI_ShouldChain()
    {
        // Arrange & Act
        var result = BuilderPattern.ServerConfig.Builder
            .WithServerName("Test")
            .WithPort(8080)
            .WithHost("localhost")
            .WithSSL()
            .WithMaxConnections(200)
            .WithTimeout(45);

        // Assert
        result.Should().BeOfType<BuilderPattern.ServerConfig.ServerConfigBuilder>();
        result.Should().NotBeNull();
    }

    #endregion
}
