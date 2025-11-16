namespace AdvancedCsharpConcepts.Beginner.Polymorphism_AssignCompatibility;

/// <summary>
/// Represents a dog, demonstrating polymorphism and inheritance.
/// Used in type casting and variance examples throughout the project.
/// </summary>
public class Dog : Animal
{
    /// <summary>
    /// Gets or sets the breed of the dog.
    /// </summary>
    public string? Breed { get; set; }

    /// <summary>
    /// Makes the dog bark, overriding the base Animal.Speak() method.
    /// Demonstrates runtime polymorphism and virtual method dispatch.
    /// </summary>
    public override void Speak()
    {
        Console.WriteLine("Dog barks");
    }
}