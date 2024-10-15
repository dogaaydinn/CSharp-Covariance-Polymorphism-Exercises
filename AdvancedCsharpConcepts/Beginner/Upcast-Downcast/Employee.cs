namespace AdvancedCsharpConcepts.Beginner.Upcast_Downcast;

public class Employee
{
    protected int Age;
    protected string? Name;

    public void DisplayInfo()
    {
        Console.WriteLine($"Name: {Name}, Age: {Age}");
    }
}