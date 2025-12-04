namespace CastingExamples.Examples;

/// <summary>
/// Demonstrates common casting mistakes and pitfalls
/// </summary>
public static class CastingPitfallsExample
{
    public static void Run()
    {
        Console.WriteLine("╔════════════════════════════════════════════════════════════════╗");
        Console.WriteLine("║                  CASTING PITFALLS & MISTAKES                   ║");
        Console.WriteLine("╚════════════════════════════════════════════════════════════════╝\n");

        // Pitfall 1: Casting incompatible types
        PrintSection("Pitfall 1: Casting Incompatible Types");

        Animal cat = new Cat { Name = "Whiskers", Lives = 9 };

        Console.WriteLine("❌ BAD: Unchecked downcast");
        Console.WriteLine("   Dog dog = (Dog)cat;  // InvalidCastException!\n");

        try
        {
            Dog dog = (Dog)cat;
        }
        catch (InvalidCastException ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"   Exception: {ex.Message}\n");
            Console.ResetColor();
        }

        Console.WriteLine("✅ GOOD: Check before casting");
        Console.WriteLine("   if (cat is Dog dog) { ... }");
        Console.WriteLine("   Or: Dog? dog = cat as Dog;");
        Console.WriteLine();

        // Pitfall 2: Null reference after 'as'
        PrintSection("Pitfall 2: Forgetting Null Check After 'as'");

        Animal animal = new Cat { Name = "Luna", Lives = 9 };

        Console.WriteLine("❌ BAD: Using result without null check");
        Console.WriteLine("   Dog dog = animal as Dog;");
        Console.WriteLine("   Console.WriteLine(dog.Breed);  // NullReferenceException!\n");

        try
        {
            Dog? dog = animal as Dog;
            Console.WriteLine(dog!.Breed);  // Null reference!
        }
        catch (NullReferenceException ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"   Exception: {ex.Message}\n");
            Console.ResetColor();
        }

        Console.WriteLine("✅ GOOD: Always check for null");
        Console.WriteLine("   Dog? dog = animal as Dog;");
        Console.WriteLine("   if (dog != null) { ... }");
        Console.WriteLine("   Or: string breed = (animal as Dog)?.Breed ?? \"N/A\";");
        Console.WriteLine();

        // Pitfall 3: Casting value types
        PrintSection("Pitfall 3: 'as' Doesn't Work with Value Types");

        object obj = 42;

        Console.WriteLine("❌ BAD: Using 'as' with value types");
        Console.WriteLine("   int? number = obj as int;  // Compilation error!");
        Console.WriteLine();

        Console.WriteLine("✅ GOOD: Use cast operator or 'is' with value types");
        if (obj is int number)
        {
            Console.WriteLine($"   int number = (int)obj;  // {number}");
            Console.WriteLine($"   Or: if (obj is int n) {{ ... }}  // {number}");
        }
        Console.WriteLine();

        // Pitfall 4: Losing derived type info
        PrintSection("Pitfall 4: Losing Derived Type Information");

        Dog originalDog = new Dog { Name = "Buddy", Breed = "Golden Retriever" };
        Animal upcastedAnimal = originalDog;  // Upcast

        Console.WriteLine($"Original dog.Breed: {originalDog.Breed}");
        Console.WriteLine($"After upcast, can't access Breed:");
        Console.WriteLine($"   // upcastedAnimal.Breed  // Compilation error!");
        Console.WriteLine();

        Console.WriteLine("✅ Solution: Downcast back to access derived members");
        if (upcastedAnimal is Dog recoveredDog)
        {
            Console.WriteLine($"   Recovered dog.Breed: {recoveredDog.Breed}");
        }
        Console.WriteLine();

        // Pitfall 5: Multiple unnecessary casts
        PrintSection("Pitfall 5: Multiple Unnecessary Casts");

        Animal[] animals = new Animal[]
        {
            new Dog { Name = "Max", Breed = "Husky" },
            new Cat { Name = "Shadow", Lives = 7 }
        };

        Console.WriteLine("❌ BAD: Multiple casts");
        Console.WriteLine("   foreach (var animal in animals)");
        Console.WriteLine("   {");
        Console.WriteLine("       if (animal is Dog)");
        Console.WriteLine("           Console.WriteLine(((Dog)animal).Breed);  // Cast twice!");
        Console.WriteLine("   }\n");

        Console.WriteLine("✅ GOOD: Pattern matching (cast once)");
        Console.WriteLine("   foreach (var animal in animals)");
        Console.WriteLine("   {");
        Console.WriteLine("       if (animal is Dog dog)");
        Console.WriteLine("           Console.WriteLine(dog.Breed);  // Cast once!");
        Console.WriteLine("   }\n");

        foreach (var item in animals)
        {
            if (item is Dog dog)
                Console.WriteLine($"   Dog: {dog.Breed}");
        }
        Console.WriteLine();

        // Summary
        PrintSection("Best Practices Summary");
        Console.WriteLine("✅ Use 'is' or 'as' before downcasting with cast operator");
        Console.WriteLine("✅ Always null-check after 'as' operator");
        Console.WriteLine("✅ Use pattern matching to avoid double casting");
        Console.WriteLine("✅ Remember: 'as' only works with reference types");
        Console.WriteLine("⚠️  Prefer safe casting ('is', 'as') over direct cast operator");
        Console.WriteLine("⚠️  InvalidCastException means you're casting incompatible types");
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
