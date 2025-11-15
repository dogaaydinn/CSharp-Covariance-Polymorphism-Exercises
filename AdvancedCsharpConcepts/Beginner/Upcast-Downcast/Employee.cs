namespace AdvancedCsharpConcepts.Beginner.Upcast_Downcast;

/// <summary>
/// Represents an employee, demonstrating upcasting and downcasting concepts.
/// Serves as the base class for Manager to show type conversion between parent and child types.
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
    /// Displays the employee's information to the console.
    /// </summary>
    public void DisplayInfo()
    {
        Console.WriteLine($"Name: {Name}, Age: {Age}");
    }
}
