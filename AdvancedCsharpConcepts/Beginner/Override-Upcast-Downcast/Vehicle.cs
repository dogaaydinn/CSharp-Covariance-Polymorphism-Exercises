namespace AdvancedCsharpConcepts.Beginner.Override_Upcast_Downcast;

public class Vehicle
{
    public virtual void Drive()
    {
        Console.WriteLine("Vehicle is driving");
    }

    public virtual void DisplayInfo()
    {
        Console.WriteLine("Vehicle info");
    }
}