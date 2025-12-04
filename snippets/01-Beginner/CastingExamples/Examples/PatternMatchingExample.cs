namespace CastingExamples.Examples;

/// <summary>
/// Demonstrates modern C# pattern matching with type patterns
/// </summary>
public static class PatternMatchingExample
{
    public static void Run()
    {
        Console.WriteLine("╔════════════════════════════════════════════════════════════════╗");
        Console.WriteLine("║              PATTERN MATCHING - MODERN C# CASTING              ║");
        Console.WriteLine("╚════════════════════════════════════════════════════════════════╝\n");

        Console.WriteLine("Pattern matching combines type checking and casting in elegant syntax.\n");

        // Example 1: Basic pattern matching (C# 7+)
        PrintSection("Example 1: Basic Pattern Matching");

        Animal animal1 = new Dog { Name = "Buddy", Breed = "Golden Retriever" };
        Animal animal2 = new Cat { Name = "Whiskers", Lives = 9 };

        // Old way (check then cast)
        Console.WriteLine("Old way:");
        if (animal1 is Dog)
        {
            Dog d = (Dog)animal1;
            Console.WriteLine($"  Dog: {d.Name}, Breed: {d.Breed}");
        }
        Console.WriteLine();

        // New way (pattern matching - check and cast in one!)
        Console.WriteLine("Pattern matching:");
        if (animal1 is Dog dog)  // Check + cast in one line!
        {
            Console.WriteLine($"  Dog: {dog.Name}, Breed: {dog.Breed}");
        }
        Console.WriteLine();

        // Example 2: Switch expressions (C# 8+)
        PrintSection("Example 2: Switch Expressions with Pattern Matching");

        Animal[] animals = new Animal[]
        {
            new Dog { Name = "Max", Breed = "Husky" },
            new Cat { Name = "Luna", Lives = 9 },
            new Dog { Name = "Charlie", Breed = "Poodle" }
        };

        Console.WriteLine("Using switch expressions:\n");
        foreach (var animal in animals)
        {
            string description = animal switch
            {
                Dog d => $"🐕 Dog: {d.Name}, Breed: {d.Breed}",
                Cat c => $"🐈 Cat: {c.Name}, Lives: {c.Lives}",
                _ => "Unknown animal"
            };
            Console.WriteLine($"  {description}");
        }
        Console.WriteLine();

        // Example 3: Property patterns (C# 8+)
        PrintSection("Example 3: Property Pattern Matching");

        Console.WriteLine("Matching on properties:\n");
        foreach (var animal in animals)
        {
            string message = animal switch
            {
                Dog { Breed: "Husky" } => "Found a Husky!",
                Dog { Breed: "Poodle" } => "Found a Poodle!",
                Dog d => $"Found a {d.Breed}",
                Cat { Lives: > 7 } c => $"Cat {c.Name} has many lives!",
                Cat c => $"Cat {c.Name}",
                _ => "Unknown"
            };
            Console.WriteLine($"  {message}");
        }
        Console.WriteLine();

        // Example 4: Relational and logical patterns (C# 9+)
        PrintSection("Example 4: Advanced Pattern Matching");

        Console.WriteLine("Complex patterns:\n");

        ProcessAnimal(new Dog { Name = "Tiny", Breed = "Chihuahua" });
        ProcessAnimal(new Dog { Name = "Big", Breed = "Great Dane" });
        ProcessAnimal(new Cat { Name = "Lucky", Lives = 9 });
        ProcessAnimal(new Cat { Name = "Unlucky", Lives = 3 });
        Console.WriteLine();

        // Example 5: Not pattern (C# 9+)
        PrintSection("Example 5: Not Pattern");

        Console.WriteLine("Using 'not' pattern:\n");
        foreach (var animal in animals)
        {
            if (animal is not Cat)  // Not a cat = must be dog
            {
                Console.WriteLine($"  Not a cat: {animal.Name}");
            }
        }
        Console.WriteLine();

        // Key Points
        PrintSection("Key Points");
        Console.WriteLine("✅ Pattern matching: if (obj is Dog d) - check + cast together");
        Console.WriteLine("✅ Switch expressions: more concise than switch statements");
        Console.WriteLine("✅ Property patterns: match on object properties");
        Console.WriteLine("✅ Relational patterns: >, <, >=, <=, and, or, not");
        Console.WriteLine("💡 Modern C# makes type checking and casting more elegant");
        Console.WriteLine();
    }

    static void ProcessAnimal(Animal animal)
    {
        string size = animal switch
        {
            Dog { Breed: "Chihuahua" or "Poodle" } => "Small dog",
            Dog { Breed: "Great Dane" or "Husky" } => "Large dog",
            Dog => "Medium dog",
            Cat { Lives: >= 7 } => "Lucky cat",
            Cat => "Regular cat",
            _ => "Unknown"
        };
        Console.WriteLine($"  {animal.Name}: {size}");
    }

    static void PrintSection(string title)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"▶ {title}");
        Console.WriteLine(new string('─', 64));
        Console.ResetColor();
    }
}
