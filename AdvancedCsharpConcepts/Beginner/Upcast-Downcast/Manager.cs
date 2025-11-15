namespace AdvancedCsharpConcepts.Beginner.Upcast_Downcast;

/// <summary>
/// Represents a manager who is a specialized type of Employee.
/// Demonstrates downcasting from Employee back to Manager and safe type checking.
/// </summary>
public class Manager : Employee
{
    /// <summary>
    /// Gets or sets the bonus amount for the manager.
    /// This property is only available on Manager, not on the base Employee class.
    /// </summary>
    public int Bonus { get; set; }

    /// <summary>
    /// Demonstrates safe downcasting using pattern matching (is operator).
    /// Shows how to upcast a Manager to Employee and then safely downcast back to Manager.
    /// </summary>
    public static void DownCast()
    {
        // Create a Manager instance
        var manager = new Manager
        {
            Name = "Alice",
            Age = 35,
            Bonus = 5000
        };

        // Upcast Manager to Employee (implicit conversion)
        Employee myEmployee = manager;

        // Display properties accessible after upcasting
        Console.WriteLine("Accessing properties after upcasting:");
        myEmployee.DisplayInfo(); // Can access: Name and Age

        // Demonstrate safe downcasting with pattern matching
        if (myEmployee is Manager myCheckedManager)
        {
            Console.WriteLine("Downcasting successful:");
            Console.WriteLine($"Bonus: {myCheckedManager.Bonus}");
        }
        else
        {
            Console.WriteLine("Downcasting failed.");
        }
    }
}
