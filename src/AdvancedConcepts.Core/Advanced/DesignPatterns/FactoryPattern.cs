namespace AdvancedConcepts.Core.Advanced.DesignPatterns;

/// <summary>
/// Factory Pattern - Creational design pattern for object creation.
/// Silicon Valley best practice: Use factories for complex object creation.
/// </summary>
public static class FactoryPattern
{
    /// <summary>
    /// Abstract product interface.
    /// </summary>
    public interface IVehicle
    {
        string GetDescription();
        int GetWheelCount();
        void Start();
    }

    /// <summary>
    /// Concrete product: Car.
    /// </summary>
    public class Car : IVehicle
    {
        public string Model { get; }

        public Car(string model)
        {
            Model = model;
        }

        public string GetDescription() => $"Car: {Model}";
        public int GetWheelCount() => 4;
        public void Start() => Console.WriteLine($"{Model} engine started!");
    }

    /// <summary>
    /// Concrete product: Motorcycle.
    /// </summary>
    public class Motorcycle : IVehicle
    {
        public string Brand { get; }

        public Motorcycle(string brand)
        {
            Brand = brand;
        }

        public string GetDescription() => $"Motorcycle: {Brand}";
        public int GetWheelCount() => 2;
        public void Start() => Console.WriteLine($"{Brand} bike started!");
    }

    /// <summary>
    /// Concrete product: Truck.
    /// </summary>
    public class Truck : IVehicle
    {
        public int Capacity { get; }

        public Truck(int capacity)
        {
            Capacity = capacity;
        }

        public string GetDescription() => $"Truck ({Capacity} tons)";
        public int GetWheelCount() => 6;
        public void Start() => Console.WriteLine($"Truck started (capacity: {Capacity}t)!");
    }

    /// <summary>
    /// Vehicle type enumeration.
    /// </summary>
    public enum VehicleType
    {
        Car,
        Motorcycle,
        Truck
    }

    /// <summary>
    /// Simple Factory - Creates vehicles based on type.
    /// </summary>
    public static class VehicleFactory
    {
        public static IVehicle CreateVehicle(VehicleType type, string parameter = "Default")
        {
            return type switch
            {
                VehicleType.Car => new Car(parameter),
                VehicleType.Motorcycle => new Motorcycle(parameter),
                VehicleType.Truck => new Truck(int.Parse(parameter)),
                _ => throw new ArgumentException($"Unknown vehicle type: {type}")
            };
        }
    }

    /// <summary>
    /// Generic Factory - Type-safe creation with generics.
    /// </summary>
    public static class GenericVehicleFactory
    {
        public static T CreateVehicle<T>(params object[] args) where T : IVehicle
        {
            return (T)Activator.CreateInstance(typeof(T), args)!;
        }
    }

    /// <summary>
    /// Factory Method Pattern - Abstract creator with concrete factories.
    /// </summary>
    public abstract class VehicleCreator
    {
        public abstract IVehicle CreateVehicle();

        public void ProcessVehicle()
        {
            var vehicle = CreateVehicle();
            Console.WriteLine($"Processing: {vehicle.GetDescription()}");
            vehicle.Start();
        }
    }

    public class CarCreator : VehicleCreator
    {
        private readonly string _model;

        public CarCreator(string model) => _model = model;

        public override IVehicle CreateVehicle() => new Car(_model);
    }

    public class MotorcycleCreator : VehicleCreator
    {
        private readonly string _brand;

        public MotorcycleCreator(string brand) => _brand = brand;

        public override IVehicle CreateVehicle() => new Motorcycle(_brand);
    }

    /// <summary>
    /// Demonstrates the Factory Pattern.
    /// </summary>
    public static void RunExample()
    {
        Console.WriteLine("=== Factory Pattern Examples ===\n");

        // Simple Factory
        Console.WriteLine("1. Simple Factory:");
        var car = VehicleFactory.CreateVehicle(VehicleType.Car, "Tesla Model 3");
        var motorcycle = VehicleFactory.CreateVehicle(VehicleType.Motorcycle, "Harley Davidson");
        var truck = VehicleFactory.CreateVehicle(VehicleType.Truck, "15");

        car.Start();
        motorcycle.Start();
        truck.Start();

        // Generic Factory
        Console.WriteLine("\n2. Generic Factory:");
        var genericCar = GenericVehicleFactory.CreateVehicle<Car>("BMW M3");
        var genericBike = GenericVehicleFactory.CreateVehicle<Motorcycle>("Yamaha");

        Console.WriteLine(genericCar.GetDescription());
        Console.WriteLine(genericBike.GetDescription());

        // Factory Method Pattern
        Console.WriteLine("\n3. Factory Method Pattern:");
        VehicleCreator carCreator = new CarCreator("Audi A4");
        VehicleCreator bikeCreator = new MotorcycleCreator("Ducati");

        carCreator.ProcessVehicle();
        bikeCreator.ProcessVehicle();
    }
}
