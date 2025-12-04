namespace FactoryPattern;

/// <summary>
/// Factory class for creating vehicles.
///
/// ⚠️ INCOMPLETE - YOU NEED TO IMPLEMENT THIS! ⚠️
/// </summary>
public static class VehicleFactory
{
    /// <summary>
    /// Creates a vehicle based on the specified type.
    /// </summary>
    public static IVehicle CreateVehicle(VehicleType type)
    {
        // TODO: Implement factory method using switch expression
        //
        // Hints:
        // return type switch
        // {
        //     VehicleType.Car => new Car(),
        //     VehicleType.Motorcycle => ???,
        //     VehicleType.Truck => ???,
        //     _ => throw new ArgumentException($"Unknown vehicle type: {type}")
        // };

        throw new NotImplementedException("TODO: Implement CreateVehicle method");
    }
}
