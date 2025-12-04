namespace FactoryPattern;

public class Car : IVehicle
{
    public string GetDescription() => "Car (Sedan)";
    public int GetWheelCount() => 4;
}

public class Motorcycle : IVehicle
{
    public string GetDescription() => "Motorcycle";
    public int GetWheelCount() => 2;
}

public class Truck : IVehicle
{
    public string GetDescription() => "Truck (18-Wheeler)";
    public int GetWheelCount() => 18;
}

public enum VehicleType
{
    Car,
    Motorcycle,
    Truck
}
