namespace CastingExamples.Examples;

/// <summary>
/// Demonstrates upcasting - implicit conversion from derived to base type
/// </summary>
public static class UpcastingExample
{
    public static void Run()
    {
        Console.WriteLine("╔════════════════════════════════════════════════════════════════╗");
        Console.WriteLine("║                  UPCASTING (IMPLICIT CASTING)                  ║");
        Console.WriteLine("╚════════════════════════════════════════════════════════════════╝\n");

        Console.WriteLine("Upcasting is ALWAYS safe because derived types contain all base type members.\n");

        // Example 1: Animal hierarchy
        PrintSection("Example 1: Animal Hierarchy");

        Dog dog = new Dog { Name = "Buddy", Breed = "Golden Retriever" };
        Cat cat = new Cat { Name = "Whiskers", Lives = 9 };

        // Implicit upcast to Animal (no cast operator needed)
        Animal animal1 = dog;  // Dog → Animal (upcast)
        Animal animal2 = cat;  // Cat → Animal (upcast)

        Console.WriteLine($"dog is Dog: {dog.GetType().Name}");
        Console.WriteLine($"animal1 is Animal: {animal1.GetType().Name} (but still a Dog at runtime!)");
        Console.WriteLine($"animal1.MakeSound(): {animal1.MakeSound()}");
        Console.WriteLine();

        Console.WriteLine($"cat is Cat: {cat.GetType().Name}");
        Console.WriteLine($"animal2 is Animal: {animal2.GetType().Name} (but still a Cat at runtime!)");
        Console.WriteLine($"animal2.MakeSound(): {animal2.MakeSound()}");
        Console.WriteLine();

        // Example 2: Array of base type
        PrintSection("Example 2: Array of Base Type (Polymorphism)");

        Animal[] animals = new Animal[]
        {
            new Dog { Name = "Max", Breed = "Husky" },      // Upcast
            new Cat { Name = "Luna", Lives = 9 },           // Upcast
            new Dog { Name = "Charlie", Breed = "Poodle" }  // Upcast
        };

        Console.WriteLine("Iterating through Animal[] (upcasted objects):\n");
        foreach (var animal in animals)
        {
            Console.WriteLine($"  {animal.Name} ({animal.GetType().Name}): {animal.MakeSound()}");
        }
        Console.WriteLine();

        // Example 3: Method parameter
        PrintSection("Example 3: Method Parameter (Upcast as Argument)");

        ProcessAnimal(dog);  // Implicit upcast Dog → Animal
        ProcessAnimal(cat);  // Implicit upcast Cat → Animal
        Console.WriteLine();

        // Key Points
        PrintSection("Key Points");
        Console.WriteLine("✅ Upcasting is IMPLICIT (no cast operator needed)");
        Console.WriteLine("✅ Always safe - can never fail");
        Console.WriteLine("✅ Enables polymorphism - treat different types uniformly");
        Console.WriteLine("✅ Runtime type is preserved (dog is still Dog internally)");
        Console.WriteLine("⚠️  You lose access to derived type members (e.g., dog.Breed)");
        Console.WriteLine();
    }

    static void ProcessAnimal(Animal animal)
    {
        Console.WriteLine($"Processing {animal.Name}: {animal.MakeSound()}");
    }

    static void PrintSection(string title)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"▶ {title}");
        Console.WriteLine(new string('─', 64));
        Console.ResetColor();
    }
}

// Animal hierarchy
public abstract class Animal
{
    public string Name { get; set; } = "";
    public abstract string MakeSound();
}

public class Dog : Animal
{
    public string Breed { get; set; } = "";
    public override string MakeSound() => "Woof!";
}

public class Cat : Animal
{
    public int Lives { get; set; }
    public override string MakeSound() => "Meow!";
}
