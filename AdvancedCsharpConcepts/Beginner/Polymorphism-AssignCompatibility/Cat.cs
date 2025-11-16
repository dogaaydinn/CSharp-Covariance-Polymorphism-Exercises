namespace AdvancedCsharpConcepts.Beginner.Polymorphism_AssignCompatibility;

/// <summary>
/// Represents a cat, demonstrating polymorphism by overriding the Speak method.
/// Used in covariance and contravariance examples.
/// </summary>
public class Cat : Animal
{
    /// <summary>
    /// Gets or sets the color of the cat.
    /// </summary>
    public string? Color { get; set; }

    /// <summary>
    /// Makes the cat meow, overriding the base Animal.Speak() method.
    /// Demonstrates runtime polymorphism and method overriding.
    /// </summary>
    public override void Speak()
    {
        Console.WriteLine("Cat meows");
    }
}