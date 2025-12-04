namespace CastingExamples.Examples;

/// <summary>
/// Demonstrates downcasting - explicit conversion from base to derived type
/// </summary>
public static class DowncastingExample
{
    public static void Run()
    {
        Console.WriteLine("╔════════════════════════════════════════════════════════════════╗");
        Console.WriteLine("║                 DOWNCASTING (EXPLICIT CASTING)                 ║");
        Console.WriteLine("╚════════════════════════════════════════════════════════════════╝\n");

        Console.WriteLine("Downcasting requires explicit cast operator and can FAIL at runtime.\n");

        // Example 1: Successful downcast
        PrintSection("Example 1: Successful Downcast");

        Animal animal = new Dog { Name = "Buddy", Breed = "Golden Retriever" };
        Console.WriteLine($"animal variable type: Animal");
        Console.WriteLine($"animal runtime type: {animal.GetType().Name}");
        Console.WriteLine();

        // Downcast Animal → Dog (requires explicit cast)
        Dog dog = (Dog)animal;  // Downcast with cast operator
        Console.WriteLine($"✅ Successful downcast: Animal → Dog");
        Console.WriteLine($"dog.Name: {dog.Name}");
        Console.WriteLine($"dog.Breed: {dog.Breed} (now accessible!)");
        Console.WriteLine();

        // Example 2: Failed downcast (InvalidCastException)
        PrintSection("Example 2: Failed Downcast (Exception)");

        Animal catAnimal = new Cat { Name = "Whiskers", Lives = 9 };
        Console.WriteLine($"catAnimal variable type: Animal");
        Console.WriteLine($"catAnimal runtime type: {catAnimal.GetType().Name}");
        Console.WriteLine();

        try
        {
            // This will throw InvalidCastException!
            Dog invalidDog = (Dog)catAnimal;  // Cat cannot be cast to Dog
            Console.WriteLine($"This line never executes");
        }
        catch (InvalidCastException ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"❌ InvalidCastException: {ex.Message}");
            Console.ResetColor();
            Console.WriteLine($"   Cannot cast Cat to Dog - they're incompatible types!");
        }
        Console.WriteLine();

        // Example 3: Downcast in array iteration
        PrintSection("Example 3: Downcast in Collection");

        Animal[] animals = new Animal[]
        {
            new Dog { Name = "Max", Breed = "Husky" },
            new Cat { Name = "Luna", Lives = 9 },
            new Dog { Name = "Charlie", Breed = "Poodle" }
        };

        Console.WriteLine("Processing animals (with type checking):\n");
        foreach (var a in animals)
        {
            Console.Write($"  {a.Name} ({a.GetType().Name}): ");

            if (a is Dog)
            {
                Dog d = (Dog)a;  // Safe downcast after 'is' check
                Console.WriteLine($"Breed: {d.Breed}");
            }
            else if (a is Cat)
            {
                Cat c = (Cat)a;  // Safe downcast after 'is' check
                Console.WriteLine($"Lives: {c.Lives}");
            }
        }
        Console.WriteLine();

        // Key Points
        PrintSection("Key Points");
        Console.WriteLine("⚠️  Downcasting is EXPLICIT (requires cast operator)");
        Console.WriteLine("⚠️  Can FAIL at runtime with InvalidCastException");
        Console.WriteLine("✅ Gives access to derived type members");
        Console.WriteLine("✅ Only succeeds if runtime type is compatible");
        Console.WriteLine("💡 Use 'is' or 'as' operators for safe downcasting");
        Console.WriteLine();
    }

    static void PrintSection(string title)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"▶ {title}");
        Console.WriteLine(new string('─', 64));
        Console.ResetColor();
    }
}
