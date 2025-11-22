namespace AdvancedCsharpConcepts.Advanced.DesignPatterns;

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
    /// Production-ready with input validation and error handling.
    /// </summary>
    public static class VehicleFactory
    {
        /// <summary>
        /// Creates a vehicle based on the specified type and parameters.
        /// </summary>
        /// <param name="type">The type of vehicle to create.</param>
        /// <param name="parameter">Vehicle-specific parameter (model/brand for Car/Motorcycle, capacity for Truck).</param>
        /// <returns>An instance of the requested vehicle type.</returns>
        /// <exception cref="ArgumentException">Thrown when vehicle type is unknown or parameters are invalid.</exception>
        /// <exception cref="ArgumentNullException">Thrown when required parameter is null or empty.</exception>
        public static IVehicle CreateVehicle(VehicleType type, string parameter = "Default")
        {
            return type switch
            {
                VehicleType.Car => CreateCar(parameter),
                VehicleType.Motorcycle => CreateMotorcycle(parameter),
                VehicleType.Truck => CreateTruck(parameter),
                _ => throw new ArgumentException($"Unknown vehicle type: {type}", nameof(type))
            };
        }

        private static IVehicle CreateCar(string model)
        {
            if (string.IsNullOrWhiteSpace(model))
                throw new ArgumentNullException(nameof(model), "Car model cannot be null or empty");

            return new Car(model);
        }

        private static IVehicle CreateMotorcycle(string brand)
        {
            if (string.IsNullOrWhiteSpace(brand))
                throw new ArgumentNullException(nameof(brand), "Motorcycle brand cannot be null or empty");

            return new Motorcycle(brand);
        }

        private static IVehicle CreateTruck(string capacityStr)
        {
            if (string.IsNullOrWhiteSpace(capacityStr))
                throw new ArgumentNullException(nameof(capacityStr), "Truck capacity cannot be null or empty");

            if (!int.TryParse(capacityStr, out var capacity))
                throw new ArgumentException($"Invalid truck capacity: '{capacityStr}'. Must be a valid integer.", nameof(capacityStr));

            if (capacity <= 0)
                throw new ArgumentException($"Invalid truck capacity: {capacity}. Must be greater than zero.", nameof(capacityStr));

            return new Truck(capacity);
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
