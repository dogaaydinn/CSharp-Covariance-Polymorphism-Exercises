namespace AdvancedCsharpConcepts.Beginner.Override_Upcast_Downcast;

/// <summary>
/// Represents a bike, which is a specific type of vehicle.
/// Demonstrates method overriding and polymorphic behavior.
/// </summary>
public class Bike : Vehicle
{
    /// <summary>
    /// Overrides the base Drive method to provide bike-specific driving behavior.
    /// </summary>
    public override void Drive()
    {
        Console.WriteLine("Bike is driving");
    }

    /// <summary>
    /// Overrides the base DisplayInfo method to show bike-specific information.
    /// </summary>
    public override void DisplayInfo()
    {
        Console.WriteLine("Bike info");
    }
}