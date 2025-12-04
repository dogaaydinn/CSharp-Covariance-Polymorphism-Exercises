namespace AdvancedCsharpConcepts.Beginner.Polymorphism_AssignCompatibility;

/// <summary>
/// Represents a dog, demonstrating polymorphism through method overriding.
/// Inherits from <see cref="Animal"/> and provides dog-specific behavior.
/// </summary>
public class Dog : Animal
{
    /// <summary>
    /// Gets or sets the breed of the dog.
    /// </summary>
    public string? Breed { get; set; }

    /// <summary>
    /// Overrides the base Speak method to provide dog-specific sound.
    /// Demonstrates runtime polymorphism and virtual method dispatch.
    /// </summary>
    public override void Speak()
    {
        Console.WriteLine("Dog barks");
    }
}