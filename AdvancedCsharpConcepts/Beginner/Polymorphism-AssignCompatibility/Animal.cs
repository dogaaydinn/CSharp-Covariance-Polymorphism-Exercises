namespace AdvancedCsharpConcepts.Beginner.Polymorphism_AssignCompatibility;

/// <summary>
/// Represents an animal that extends the Mammal class.
/// Demonstrates inheritance, polymorphism, and virtual method dispatch.
/// </summary>
public class Animal : Mammal
{
    /// <summary>
    /// Gets or sets the name of the animal.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Makes the animal speak. This method can be overridden by derived classes
    /// to provide specific animal sounds, demonstrating polymorphic behavior.
    /// </summary>
    public virtual void Speak()
    {
        Console.WriteLine("Animal speaks");
    }
}