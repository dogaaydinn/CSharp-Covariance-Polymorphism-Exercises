namespace AdvancedCsharpConcepts.Beginner.Polymorphism_AssignCompatibility;

public class Cat : Animal
{
    public string? Color { get; set; }

    public override void Speak()
    {
        Console.WriteLine("Cat meows");
    }
}