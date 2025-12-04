using System;
using System.Collections.Generic;

namespace AdvancedConcepts.Samples.CovarianceContravariance.Examples;

/// <summary>
/// Demonstrates covariance (out T) with IEnumerable and delegates.
/// Covariance allows you to use a more derived type than originally specified.
/// </summary>
/// <remarks>
/// Key Concept: Covariance works when T appears only in OUTPUT positions (return values).
/// The 'out' keyword indicates that T is covariant.
///
/// Real-world analogy: A dog shelter produces dogs. Since dogs are animals,
/// you can treat a "dog producer" as an "animal producer" - what comes out is still valid!
/// </remarks>
public static class CovarianceExample
{
    /// <summary>
    /// Runs covariance demonstrations including IEnumerable, arrays, and delegates.
    /// </summary>
    public static void Run()
    {
        Console.WriteLine("COVARIANCE allows you to use a MORE DERIVED type than originally specified.");
        Console.WriteLine("Keyword: 'out' - used for OUTPUT/RETURN types only");
        Console.WriteLine();

        DemonstrateIEnumerableCovariance();
        Console.WriteLine();

        DemonstrateArrayCovariance();
        Console.WriteLine();

        DemonstrateDelegateCovariance();
    }

    /// <summary>
    /// Demonstrates covariance with IEnumerable&lt;out T&gt;.
    /// IEnumerable is covariant because it only returns items, never accepts them.
    /// </summary>
    private static void DemonstrateIEnumerableCovariance()
    {
        Console.WriteLine("1. IEnumerable<out T> Covariance:");
        Console.WriteLine("   " + "=".PadRight(50, '='));

        // Create a collection of Dogs
        List<Dog> dogs = new()
        {
            new Dog { Name = "Buddy", Breed = "Golden Retriever" },
            new Dog { Name = "Max", Breed = "Labrador" },
            new Dog { Name = "Charlie", Breed = "Beagle" }
        };

        // COVARIANCE: IEnumerable<Dog> can be assigned to IEnumerable<Animal>
        // This is safe because IEnumerable only RETURNS items (covariant)
        IEnumerable<Animal> animals = dogs;

        Console.WriteLine("   ✅ COVARIANCE SUCCESS:");
        Console.WriteLine("      IEnumerable<Dog> → IEnumerable<Animal> (allowed!)");
        Console.WriteLine();

        Console.WriteLine("   Animals in the collection:");
        foreach (var animal in animals)
        {
            // Polymorphism: even though we're iterating as Animal,
            // the actual type is Dog, and we can safely upcast
            Console.WriteLine($"      - {animal.Name} ({animal.GetType().Name})");
            animal.MakeSound();
        }
        Console.WriteLine();

        Console.WriteLine("   WHY IT WORKS:");
        Console.WriteLine("      • IEnumerable<out T> only RETURNS items (covariant)");
        Console.WriteLine("      • Reading Dogs as Animals is safe (upcasting)");
        Console.WriteLine("      • Dog IS-A Animal ✓");
        Console.WriteLine("      • No risk of adding wrong types (read-only)");
    }

    /// <summary>
    /// Demonstrates array covariance (legacy feature with safety concerns).
    /// Arrays are covariant in C#, but this can lead to runtime errors!
    /// </summary>
    /// <remarks>
    /// WARNING: Array covariance is a design flaw inherited from earlier versions.
    /// It allows code that should fail at compile-time to fail at runtime instead.
    /// Prefer IEnumerable for covariant collections.
    /// </remarks>
    private static void DemonstrateArrayCovariance()
    {
        Console.WriteLine("2. Array Covariance (Legacy Feature - Use with Caution!):");
        Console.WriteLine("   " + "=".PadRight(50, '='));

        // Arrays are covariant (but this is dangerous!)
        Dog[] dogs = new Dog[]
        {
            new Dog { Name = "Rex", Breed = "German Shepherd" }
        };

        // Array covariance allows this assignment
        Animal[] animals = dogs;

        Console.WriteLine("   ⚠️  Array covariance is allowed but DANGEROUS:");
        Console.WriteLine("      Dog[] → Animal[] (compiles, but risky!)");
        Console.WriteLine();

        Console.WriteLine("   Safe operation - reading:");
        foreach (var animal in animals)
        {
            Console.WriteLine($"      ✓ Reading: {animal.Name}");
        }
        Console.WriteLine();

        Console.WriteLine("   DANGEROUS operation - writing:");
        Console.WriteLine("      The following would COMPILE but throw ArrayTypeMismatchException:");
        Console.WriteLine("      animals[0] = new Cat { Name = \"Whiskers\" }; // RUNTIME ERROR! 💥");
        Console.WriteLine();

        Console.WriteLine("   WHY IT'S DANGEROUS:");
        Console.WriteLine("      • Arrays allow BOTH reading AND writing");
        Console.WriteLine("      • Type checking happens at RUNTIME, not compile-time");
        Console.WriteLine("      • Prefer IEnumerable<T> for safe covariance");
    }

    /// <summary>
    /// Demonstrates delegate covariance with Func&lt;out TResult&gt;.
    /// Delegates can be covariant when they only return values.
    /// </summary>
    private static void DemonstrateDelegateCovariance()
    {
        Console.WriteLine("3. Delegate Covariance (Func<out TResult>):");
        Console.WriteLine("   " + "=".PadRight(50, '='));

        // Define a function that returns a Dog
        Func<Dog> getDog = () => new Dog { Name = "Bella", Breed = "Poodle" };

        // COVARIANCE: Func<Dog> can be assigned to Func<Animal>
        // This works because Func<out TResult> is covariant
        Func<Animal> getAnimal = getDog;

        Console.WriteLine("   ✅ Func<Dog> → Func<Animal> (covariance)");
        Console.WriteLine();

        var animal = getAnimal();
        Console.WriteLine($"   Retrieved: {animal.Name} - {animal.GetType().Name}");
        Console.WriteLine();

        Console.WriteLine("   WHY IT WORKS:");
        Console.WriteLine("      • Func<out TResult> only RETURNS values");
        Console.WriteLine("      • A function returning Dog can be treated as returning Animal");
        Console.WriteLine("      • The returned Dog is still a valid Animal");
        Console.WriteLine();

        Console.WriteLine("   PRACTICAL USE CASE:");
        Console.WriteLine("      Factory methods, lazy initialization, LINQ queries");
    }

    #region Model Classes

    /// <summary>
    /// Base class representing an animal with common properties and behaviors.
    /// </summary>
    public class Animal
    {
        public string Name { get; set; } = string.Empty;

        public virtual void MakeSound()
        {
            Console.WriteLine($"         [{Name} makes a generic animal sound]");
        }
    }

    /// <summary>
    /// Derived class representing a dog - a specific type of animal.
    /// </summary>
    public class Dog : Animal
    {
        public string Breed { get; set; } = string.Empty;

        public override void MakeSound()
        {
            Console.WriteLine($"         [{Name} the {Breed} barks: Woof! Woof!]");
        }
    }

    /// <summary>
    /// Another derived class representing a cat - used to demonstrate type safety.
    /// </summary>
    public class Cat : Animal
    {
        public string Color { get; set; } = string.Empty;

        public override void MakeSound()
        {
            Console.WriteLine($"         [{Name} the cat meows: Meow!]");
        }
    }

    #endregion
}
