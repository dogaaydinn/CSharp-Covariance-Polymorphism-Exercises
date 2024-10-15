namespace ConsoleApp2.Beginner.Polymorphism;

public class Dog : Animal
{
    public string? Breed { get; set; }

    public override void Speak()
    {
        Console.WriteLine("Dog barks");
    }
}