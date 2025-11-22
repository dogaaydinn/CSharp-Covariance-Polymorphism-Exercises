namespace AdvancedCsharpConcepts.Beginner.Upcast_Downcast;

/// <summary>
/// Represents an employee in the organization.
/// Demonstrates upcasting and downcasting concepts in object-oriented programming.
/// </summary>
public class Employee
{
    /// <summary>
    /// Gets or sets the age of the employee.
    /// </summary>
    protected int Age { get; set; }

    /// <summary>
    /// Gets or sets the name of the employee.
    /// </summary>
    protected string? Name { get; set; }

    /// <summary>
    /// Displays employee information to the console.
    /// </summary>
    public void DisplayInfo()
    {
        Console.WriteLine($"Name: {Name}, Age: {Age}");
    }
}
