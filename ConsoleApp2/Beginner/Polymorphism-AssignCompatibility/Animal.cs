namespace ConsoleApp2.Beginner.Polymorphism;

public class Animal : Mammal
{
    public string? Name { get; set; }

    public virtual void Speak()
    {
        Console.WriteLine("Animal speaks");
    }
}