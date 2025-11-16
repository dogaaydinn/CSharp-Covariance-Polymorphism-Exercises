namespace AdvancedCsharpConcepts.Beginner.Override_Upcast_Downcast;

/// <summary>
/// Represents a base vehicle class that demonstrates polymorphism through method overriding.
/// This class serves as a parent class for specific vehicle types like Car and Bike.
/// </summary>
public class Vehicle
{
    /// <summary>
    /// Simulates the vehicle driving behavior.
    /// This virtual method can be overridden by derived classes to provide specific implementations.
    /// </summary>
    public virtual void Drive()
    {
        Console.WriteLine("Vehicle is driving");
    }

    /// <summary>
    /// Displays basic information about the vehicle.
    /// This virtual method can be overridden by derived classes to show type-specific details.
    /// </summary>
    public virtual void DisplayInfo()
    {
        Console.WriteLine("Vehicle info");
    }
}