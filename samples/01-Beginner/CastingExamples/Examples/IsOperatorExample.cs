namespace CastingExamples.Examples;

/// <summary>
/// Demonstrates the 'is' operator for type checking
/// </summary>
public static class IsOperatorExample
{
    public static void Run()
    {
        Console.WriteLine("╔════════════════════════════════════════════════════════════════╗");
        Console.WriteLine("║                 'is' OPERATOR - TYPE CHECKING                  ║");
        Console.WriteLine("╚════════════════════════════════════════════════════════════════╝\n");

        Console.WriteLine("The 'is' operator checks if an object is of a specific type.\n");

        // Example 1: Basic type checking
        PrintSection("Example 1: Basic Type Checking");

        Animal dog = new Dog { Name = "Buddy", Breed = "Golden Retriever" };
        Animal cat = new Cat { Name = "Whiskers", Lives = 9 };

        Console.WriteLine($"dog is Dog: {dog is Dog}");
        Console.WriteLine($"dog is Cat: {dog is Cat}");
        Console.WriteLine($"dog is Animal: {dog is Animal}");
        Console.WriteLine();

        Console.WriteLine($"cat is Cat: {cat is Cat}");
        Console.WriteLine($"cat is Dog: {cat is Dog}");
        Console.WriteLine($"cat is Animal: {cat is Animal}");
        Console.WriteLine();

        // Example 2: Safe downcasting with 'is'
        PrintSection("Example 2: Safe Downcasting Pattern");

        Animal[] animals = new Animal[]
        {
            new Dog { Name = "Max", Breed = "Husky" },
            new Cat { Name = "Luna", Lives = 9 },
            new Dog { Name = "Charlie", Breed = "Poodle" }
        };

        Console.WriteLine("Processing animals with 'is' check before downcast:\n");
        foreach (var animal in animals)
        {
            Console.Write($"  {animal.Name}: ");

            if (animal is Dog)
            {
                Dog d = (Dog)animal;  // Safe downcast
                Console.WriteLine($"Dog, Breed: {d.Breed}");
            }
            else if (animal is Cat)
            {
                Cat c = (Cat)animal;  // Safe downcast
                Console.WriteLine($"Cat, Lives: {c.Lives}");
            }
        }
        Console.WriteLine();

        // Example 3: Pattern matching with 'is' (C# 7+)
        PrintSection("Example 3: Pattern Matching with 'is' (Modern C#)");

        Console.WriteLine("Using 'is' with pattern matching (combines check + cast):\n");
        foreach (var animal in animals)
        {
            Console.Write($"  {animal.Name}: ");

            // Modern pattern matching - check and cast in one line!
            if (animal is Dog d)
            {
                Console.WriteLine($"Dog, Breed: {d.Breed}");
            }
            else if (animal is Cat c)
            {
                Console.WriteLine($"Cat, Lives: {c.Lives}");
            }
        }
        Console.WriteLine();

        // Example 4: Null checking with 'is'
        PrintSection("Example 4: Null Checking");

        Animal? nullAnimal = null;
        Console.WriteLine($"nullAnimal is Dog: {nullAnimal is Dog}");
        Console.WriteLine($"nullAnimal is null: {nullAnimal is null}");
        Console.WriteLine($"nullAnimal is not null: {nullAnimal is not null}");
        Console.WriteLine();

        // Key Points
        PrintSection("Key Points");
        Console.WriteLine("✅ 'is' operator returns true/false (no exception)");
        Console.WriteLine("✅ Always safe - never throws exception");
        Console.WriteLine("✅ Works with null (returns false for type checks)");
        Console.WriteLine("✅ Modern C# allows pattern matching: if (obj is Dog d)");
        Console.WriteLine("💡 Use before downcasting to avoid InvalidCastException");
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
