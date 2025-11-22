namespace AdvancedCsharpConcepts.Beginner.Polymorphism_AssignCompatibility;

/// <summary>
/// Represents an animal in the inheritance hierarchy.
/// Demonstrates polymorphism and method overriding concepts.
/// </summary>
public class Animal : Mammal
{
    /// <summary>
    /// Gets or sets the name of the animal.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Makes the animal speak. Can be overridden by derived classes.
    /// Demonstrates virtual method dispatch and runtime polymorphism.
    /// </summary>
    public virtual void Speak()
    {
        Console.WriteLine("Animal speaks");
    }
}