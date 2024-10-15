namespace ConsoleApp2.Beginner;

public class Vehicle
{
    public virtual void Drive()
    {
        Console.WriteLine("Vehicle is driving");
    }

    public void DisplayInfo()
    {
        Console.WriteLine("Vehicle info");
    }
}