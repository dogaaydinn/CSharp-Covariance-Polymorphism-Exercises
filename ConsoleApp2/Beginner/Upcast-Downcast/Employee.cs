namespace ConsoleApp2.Beginner;

public class Employee
{
    protected int Age;
    protected string? Name;

    public void DisplayInfo()
    {
        Console.WriteLine($"Name: {Name}, Age: {Age}");
    }
}