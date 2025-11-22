namespace AdvancedCsharpConcepts.Beginner.Polymorphism_AssignCompatibility;

/// <summary>
/// Represents a cat, demonstrating polymorphism through method overriding.
/// Inherits from <see cref="Animal"/> and provides cat-specific behavior.
/// </summary>
public class Cat : Animal
{
    /// <summary>
    /// Gets or sets the color of the cat.
    /// </summary>
    public string? Color { get; set; }

    /// <summary>
    /// Overrides the base Speak method to provide cat-specific sound.
    /// Demonstrates runtime polymorphism and virtual method dispatch.
    /// </summary>
    public override void Speak()
    {
        Console.WriteLine("Cat meows");
    }
}