namespace AdvancedCsharpConcepts.Beginner.Polymorphism_AssignCompatibility;

public class Dog : Animal
{
    public string? Breed { get; set; }

    public override void Speak()
    {
        Console.WriteLine("Dog barks");
    }
}