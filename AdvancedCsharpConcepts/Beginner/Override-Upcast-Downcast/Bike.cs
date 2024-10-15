namespace AdvancedCsharpConcepts.Beginner.Override_Upcast_Downcast;

public class Bike : Vehicle
{
    public override void Drive()
    {
        Console.WriteLine("Bike is driving");
    }

    public override void DisplayInfo()
    {
        Console.WriteLine("Bike info");
    }
}