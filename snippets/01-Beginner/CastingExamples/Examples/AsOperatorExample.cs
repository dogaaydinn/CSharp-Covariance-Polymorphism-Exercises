namespace CastingExamples.Examples;

/// <summary>
/// Demonstrates the 'as' operator for safe casting
/// </summary>
public static class AsOperatorExample
{
    public static void Run()
    {
        Console.WriteLine("╔════════════════════════════════════════════════════════════════╗");
        Console.WriteLine("║                  'as' OPERATOR - SAFE CASTING                  ║");
        Console.WriteLine("╚════════════════════════════════════════════════════════════════╝\n");

        Console.WriteLine("The 'as' operator attempts to cast, returns null on failure (never throws).\n");

        // Example 1: Successful cast
        PrintSection("Example 1: Successful Cast");

        Animal animal = new Dog { Name = "Buddy", Breed = "Golden Retriever" };

        Dog? dog = animal as Dog;  // Cast succeeds

        if (dog != null)
        {
            Console.WriteLine($"✅ Cast succeeded!");
            Console.WriteLine($"   dog.Name: {dog.Name}");
            Console.WriteLine($"   dog.Breed: {dog.Breed}");
        }
        else
        {
            Console.WriteLine($"❌ Cast failed (null returned)");
        }
        Console.WriteLine();

        // Example 2: Failed cast (returns null)
        PrintSection("Example 2: Failed Cast (No Exception!)");

        Animal catAnimal = new Cat { Name = "Whiskers", Lives = 9 };

        Dog? invalidDog = catAnimal as Dog;  // Cast fails, returns null

        if (invalidDog != null)
        {
            Console.WriteLine($"✅ Cast succeeded!");
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"⚠️  Cast failed - 'as' returned null (no exception!)");
            Console.ResetColor();
            Console.WriteLine($"   catAnimal is {catAnimal.GetType().Name}, cannot cast to Dog");
        }
        Console.WriteLine();

        // Example 3: Comparison with cast operator
        PrintSection("Example 3: 'as' vs Cast Operator");

        Animal cat = new Cat { Name = "Luna", Lives = 9 };

        Console.WriteLine("Using cast operator (Dog)cat:");
        try
        {
            Dog? d1 = (Dog)cat;  // Throws InvalidCastException!
            Console.WriteLine($"  Result: {d1?.Name}");
        }
        catch (InvalidCastException)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"  ❌ InvalidCastException thrown!");
            Console.ResetColor();
        }
        Console.WriteLine();

        Console.WriteLine("Using 'as' operator (cat as Dog):");
        Dog? d2 = cat as Dog;  // Returns null (no exception)
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"  ✅ Returns null (no exception)");
        Console.ResetColor();
        Console.WriteLine($"  Result: {d2?.Name ?? "null"}");
        Console.WriteLine();

        // Example 4: Processing collection with 'as'
        PrintSection("Example 4: Processing Collection with 'as'");

        Animal[] animals = new Animal[]
        {
            new Dog { Name = "Max", Breed = "Husky" },
            new Cat { Name = "Mittens", Lives = 9 },
            new Dog { Name = "Charlie", Breed = "Poodle" }
        };

        Console.WriteLine("Processing dogs only:\n");
        foreach (var a in animals)
        {
            Dog? d = a as Dog;  // Safe cast
            if (d != null)
            {
                Console.WriteLine($"  🐕 {d.Name} (Breed: {d.Breed})");
            }
        }
        Console.WriteLine();

        // Example 5: Null-coalescing with 'as'
        PrintSection("Example 5: Null-Coalescing Pattern");

        Animal animal2 = new Cat { Name = "Shadow", Lives = 7 };

        // Use 'as' with null-coalescing operator
        string breed = (animal2 as Dog)?.Breed ?? "Not a dog";
        Console.WriteLine($"Breed: {breed}");
        Console.WriteLine();

        // Key Points
        PrintSection("Key Points");
        Console.WriteLine("✅ 'as' operator returns null on failure (NEVER throws)");
        Console.WriteLine("✅ Always safe - no InvalidCastException");
        Console.WriteLine("✅ Only works with reference types (not value types)");
        Console.WriteLine("✅ Combines well with null-coalescing: (obj as Dog)?.Breed");
        Console.WriteLine("⚠️  Returns nullable type (Dog?), must check for null");
        Console.WriteLine("💡 Prefer 'as' when cast failure is expected/common");
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
