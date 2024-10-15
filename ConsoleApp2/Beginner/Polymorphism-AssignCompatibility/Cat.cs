namespace ConsoleApp2.Beginner.Polymorphism;

public class Cat : Animal
{
    public string? Color { get; set; }

    public override void Speak()
    {
        Console.WriteLine("Cat meows");
    }
}