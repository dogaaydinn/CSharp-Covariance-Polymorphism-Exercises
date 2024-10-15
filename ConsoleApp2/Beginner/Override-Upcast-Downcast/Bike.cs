namespace ConsoleApp2.Beginner;

public class Bike : Vehicle
{
    public override void Drive()
    {
        Console.WriteLine("Bike is driving");
    }

    public static void DisplayBikeInfo()
    {
        Console.WriteLine("Bike info");
    }
}