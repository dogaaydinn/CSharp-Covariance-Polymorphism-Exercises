using System;

namespace DesignPatterns.Creational;

/// <summary>
/// FACTORY PATTERN - Creates objects without specifying exact class type
///
/// Problem:
/// - Client code shouldn't depend on concrete classes
/// - Object creation logic is complex or varies
/// - Need centralized control over object creation
///
/// UML Structure:
/// ┌──────────────┐
/// │   Product    │ (interface)
/// └──────────────┘
///        △
///        │ implements
///        │
/// ┌──────┴──────┬──────────┬──────────┐
/// │ ConcreteA   │ConcreteB │ConcreteC │
/// └─────────────┴──────────┴──────────┘
///                  ↑
///                  │ creates
/// ┌────────────────┴───────┐
/// │      Factory           │
/// │ +CreateProduct(type)   │
/// └────────────────────────┘
///
/// When to Use:
/// - Object creation depends on runtime conditions
/// - Need to decouple client from concrete classes
/// - Want to centralize object creation logic
/// - Subclasses should specify objects to create
///
/// Benefits:
/// - Loose coupling
/// - Single Responsibility Principle
/// - Open/Closed Principle
/// </summary>

#region Vehicle Factory

/// <summary>
/// Product interface for vehicles
/// </summary>
public interface IVehicle
{
    void Drive();
    void Stop();
    string GetInfo();
    int MaxSpeed { get; }
}

/// <summary>
/// Concrete product: Car
/// </summary>
public class Car : IVehicle
{
    private readonly string _model;
    private bool _isMoving;

    public Car(string model = "Sedan")
    {
        _model = model;
        Console.WriteLine($"  [Factory] Creating car: {_model}");
    }

    public int MaxSpeed => 200;

    public void Drive()
    {
        _isMoving = true;
        Console.WriteLine($"  [Factory] Driving {_model} at max speed {MaxSpeed} km/h");
    }

    public void Stop()
    {
        _isMoving = false;
        Console.WriteLine($"  [Factory] {_model} stopped");
    }

    public string GetInfo() => $"Car ({_model}) - Max Speed: {MaxSpeed} km/h";
}

/// <summary>
/// Concrete product: Motorcycle
/// </summary>
public class Motorcycle : IVehicle
{
    private readonly string _type;

    public Motorcycle(string type = "Sport")
    {
        _type = type;
        Console.WriteLine($"  [Factory] Creating motorcycle: {_type}");
    }

    public int MaxSpeed => 250;

    public void Drive()
    {
        Console.WriteLine($"  [Factory] Riding {_type} motorcycle at max speed {MaxSpeed} km/h");
    }

    public void Stop()
    {
        Console.WriteLine($"  [Factory] {_type} motorcycle stopped");
    }

    public string GetInfo() => $"Motorcycle ({_type}) - Max Speed: {MaxSpeed} km/h";
}

/// <summary>
/// Concrete product: Truck
/// </summary>
public class Truck : IVehicle
{
    private readonly int _capacity;

    public Truck(int capacity = 5000)
    {
        _capacity = capacity;
        Console.WriteLine($"  [Factory] Creating truck with capacity: {_capacity} kg");
    }

    public int MaxSpeed => 120;

    public void Drive()
    {
        Console.WriteLine($"  [Factory] Driving truck (capacity: {_capacity}kg) at max speed {MaxSpeed} km/h");
    }

    public void Stop()
    {
        Console.WriteLine($"  [Factory] Truck stopped");
    }

    public string GetInfo() => $"Truck (Capacity: {_capacity}kg) - Max Speed: {MaxSpeed} km/h";
}

/// <summary>
/// Concrete product: Electric Car
/// </summary>
public class ElectricCar : IVehicle
{
    private readonly int _batteryCapacity;

    public ElectricCar(int batteryCapacity = 75)
    {
        _batteryCapacity = batteryCapacity;
        Console.WriteLine($"  [Factory] Creating electric car with battery: {_batteryCapacity} kWh");
    }

    public int MaxSpeed => 180;

    public void Drive()
    {
        Console.WriteLine($"  [Factory] Silently driving electric car (Battery: {_batteryCapacity}kWh) at {MaxSpeed} km/h");
    }

    public void Stop()
    {
        Console.WriteLine($"  [Factory] Electric car stopped (regenerative braking activated)");
    }

    public string GetInfo() => $"Electric Car (Battery: {_batteryCapacity}kWh) - Max Speed: {MaxSpeed} km/h";
}

/// <summary>
/// Simple Factory - Creates vehicles based on type
/// </summary>
public static class VehicleFactory
{
    public static IVehicle CreateVehicle(string type) => type.ToLower() switch
    {
        "car" => new Car("Family Sedan"),
        "sportcar" => new Car("Sports Car"),
        "motorcycle" => new Motorcycle("Sport"),
        "cruiser" => new Motorcycle("Cruiser"),
        "truck" => new Truck(5000),
        "heavytruck" => new Truck(10000),
        "electric" => new ElectricCar(75),
        _ => throw new ArgumentException($"Unknown vehicle type: {type}")
    };
}

#endregion

#region Abstract Factory Pattern

/// <summary>
/// Abstract Factory Pattern - Provides interface for creating families of related objects
///
/// UML Structure:
/// ┌─────────────────┐         ┌─────────────────┐
/// │ AbstractFactory │◄────────│     Client      │
/// └─────────────────┘         └─────────────────┘
///        △
///        │
/// ┌──────┴──────┬──────────────┐
/// │ ConcreteA   │  ConcreteB   │
/// │ Factory     │  Factory     │
/// └─────────────┴──────────────┘
/// </summary>

/// <summary>
/// Abstract product: Engine
/// </summary>
public interface IEngine
{
    void Start();
    string GetType();
}

/// <summary>
/// Abstract product: Transmission
/// </summary>
public interface ITransmission
{
    void ShiftGear(int gear);
    string GetType();
}

// Luxury vehicle parts
public class LuxuryEngine : IEngine
{
    public void Start() => Console.WriteLine("  [Abstract Factory] Luxury V8 engine starting smoothly...");
    public string GetType() => "Luxury V8";
}

public class LuxuryTransmission : ITransmission
{
    public void ShiftGear(int gear) => Console.WriteLine($"  [Abstract Factory] Automatic transmission shifting to gear {gear}");
    public string GetType() => "8-Speed Automatic";
}

// Economy vehicle parts
public class EconomyEngine : IEngine
{
    public void Start() => Console.WriteLine("  [Abstract Factory] Economy 4-cylinder engine starting...");
    public string GetType() => "Economy 4-Cylinder";
}

public class EconomyTransmission : ITransmission
{
    public void ShiftGear(int gear) => Console.WriteLine($"  [Abstract Factory] Manual transmission in gear {gear}");
    public string GetType() => "5-Speed Manual";
}

// Sport vehicle parts
public class SportEngine : IEngine
{
    public void Start() => Console.WriteLine("  [Abstract Factory] Sport V6 Turbo engine roaring to life!");
    public string GetType() => "V6 Turbo";
}

public class SportTransmission : ITransmission
{
    public void ShiftGear(int gear) => Console.WriteLine($"  [Abstract Factory] Sport dual-clutch transmission: gear {gear}");
    public string GetType() => "7-Speed Dual-Clutch";
}

/// <summary>
/// Abstract Factory interface
/// </summary>
public interface IVehicleFactory
{
    IEngine CreateEngine();
    ITransmission CreateTransmission();
}

/// <summary>
/// Concrete Factory: Luxury vehicles
/// </summary>
public class LuxuryVehicleFactory : IVehicleFactory
{
    public IEngine CreateEngine() => new LuxuryEngine();
    public ITransmission CreateTransmission() => new LuxuryTransmission();
}

/// <summary>
/// Concrete Factory: Economy vehicles
/// </summary>
public class EconomyVehicleFactory : IVehicleFactory
{
    public IEngine CreateEngine() => new EconomyEngine();
    public ITransmission CreateTransmission() => new EconomyTransmission();
}

/// <summary>
/// Concrete Factory: Sport vehicles
/// </summary>
public class SportVehicleFactory : IVehicleFactory
{
    public IEngine CreateEngine() => new SportEngine();
    public ITransmission CreateTransmission() => new SportTransmission();
}

/// <summary>
/// Client that uses abstract factory
/// </summary>
public class VehicleAssembler
{
    private readonly IVehicleFactory _factory;

    public VehicleAssembler(IVehicleFactory factory)
    {
        _factory = factory;
    }

    public void AssembleVehicle()
    {
        Console.WriteLine("  [Abstract Factory] Assembling vehicle...");

        var engine = _factory.CreateEngine();
        var transmission = _factory.CreateTransmission();

        Console.WriteLine($"  [Abstract Factory] Installed: {engine.GetType()} engine");
        Console.WriteLine($"  [Abstract Factory] Installed: {transmission.GetType()} transmission");

        engine.Start();
        transmission.ShiftGear(1);
        transmission.ShiftGear(2);
    }
}

#endregion

/// <summary>
/// Example demonstrating Factory patterns
/// </summary>
public static class FactoryExample
{
    public static void Run()
    {
        Console.WriteLine();
        Console.WriteLine("2. FACTORY PATTERN - Creates objects without specifying exact class");
        Console.WriteLine("-".PadRight(70, '-'));
        Console.WriteLine();

        // Example 1: Simple Factory
        Console.WriteLine("Example 1: Simple Factory Pattern");
        Console.WriteLine();

        var vehicles = new List<IVehicle>
        {
            VehicleFactory.CreateVehicle("car"),
            VehicleFactory.CreateVehicle("motorcycle"),
            VehicleFactory.CreateVehicle("truck"),
            VehicleFactory.CreateVehicle("electric")
        };

        Console.WriteLine();
        Console.WriteLine("  Testing all vehicles:");
        foreach (var vehicle in vehicles)
        {
            Console.WriteLine($"  - {vehicle.GetInfo()}");
            vehicle.Drive();
            vehicle.Stop();
            Console.WriteLine();
        }

        // Example 2: Abstract Factory
        Console.WriteLine("Example 2: Abstract Factory Pattern");
        Console.WriteLine();

        // Create luxury vehicle
        Console.WriteLine("  Creating Luxury Vehicle:");
        var luxuryAssembler = new VehicleAssembler(new LuxuryVehicleFactory());
        luxuryAssembler.AssembleVehicle();

        Console.WriteLine();

        // Create economy vehicle
        Console.WriteLine("  Creating Economy Vehicle:");
        var economyAssembler = new VehicleAssembler(new EconomyVehicleFactory());
        economyAssembler.AssembleVehicle();

        Console.WriteLine();

        // Create sport vehicle
        Console.WriteLine("  Creating Sport Vehicle:");
        var sportAssembler = new VehicleAssembler(new SportVehicleFactory());
        sportAssembler.AssembleVehicle();

        Console.WriteLine();
        Console.WriteLine("  Key Benefit: Client code doesn't depend on concrete classes!");
    }
}
