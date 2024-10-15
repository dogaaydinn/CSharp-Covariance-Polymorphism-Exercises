namespace AdvancedCsharpConcepts.Beginner.Polymorphism_AssignCompatibility;

public class Animal : Mammal
{
    public string? Name { get; set; }

    public virtual void Speak()
    {
        Console.WriteLine("Animal speaks");
    }
}