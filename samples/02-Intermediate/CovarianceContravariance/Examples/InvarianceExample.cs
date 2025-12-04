using System;
using System.Collections.Generic;

namespace AdvancedConcepts.Samples.CovarianceContravariance.Examples;

/// <summary>
/// Demonstrates invariance - when no variance is allowed.
/// Invariance occurs when a type parameter appears in both input and output positions.
/// </summary>
/// <remarks>
/// Key Concept: Invariance means you must use the EXACT type - no conversion allowed.
/// This prevents type safety violations when a type is used for both reading and writing.
///
/// Real-world analogy: A dog kennel stores and returns dogs. If you could treat it as
/// an animal kennel, you might put cats in, then try to get them out as dogs - disaster!
/// </remarks>
public static class InvarianceExample
{
    /// <summary>
    /// Runs invariance demonstrations showing why certain collections cannot be variant.
    /// </summary>
    public static void Run()
    {
        Console.WriteLine("INVARIANCE means NO variance - you must use the exact type.");
        Console.WriteLine("No 'in' or 'out' keyword - occurs when type is in BOTH positions");
        Console.WriteLine();

        DemonstrateIListInvariance();
        Console.WriteLine();

        DemonstrateWhyInvarianceIsNecessary();
        Console.WriteLine();

        DemonstrateSafeAlternatives();
    }

    /// <summary>
    /// Demonstrates that IList&lt;T&gt; is invariant and why this prevents compilation.
    /// </summary>
    private static void DemonstrateIListInvariance()
    {
        Console.WriteLine("1. IList<T> Invariance:");
        Console.WriteLine("   " + "=".PadRight(50, '='));

        List<Dog> dogs = new()
        {
            new Dog { Name = "Charlie", Breed = "Poodle" },
            new Dog { Name = "Daisy", Breed = "Corgi" }
        };

        Console.WriteLine("   ❌ INVARIANCE - The following would NOT compile:");
        Console.WriteLine("      IList<Dog> dogList = new List<Dog>();");
        Console.WriteLine("      IList<Animal> animalList = dogList;  // COMPILE ERROR!");
        Console.WriteLine();

        Console.WriteLine("   ERROR MESSAGE:");
        Console.WriteLine("      Cannot implicitly convert type 'IList<Dog>' to 'IList<Animal>'");
        Console.WriteLine();

        Console.WriteLine("   WHY IT FAILS:");
        Console.WriteLine("      IList<T> has methods with T in BOTH positions:");
        Console.WriteLine("         • T this[int index] { get; set; }  // GET (output) and SET (input)");
        Console.WriteLine("         • void Add(T item)                 // Accepts T (input)");
        Console.WriteLine("         • T RemoveAt(int index)            // Returns T (output)");
        Console.WriteLine();
        Console.WriteLine("      • Can't be covariant (out) - has Add(T) method");
        Console.WriteLine("      • Can't be contravariant (in) - has indexer get");
        Console.WriteLine("      • Therefore: INVARIANT (no variance)");
    }

    /// <summary>
    /// Demonstrates the type safety problems that would occur if IList were variant.
    /// This shows WHY the compiler enforces invariance.
    /// </summary>
    private static void DemonstrateWhyInvarianceIsNecessary()
    {
        Console.WriteLine("2. Why Invariance is Necessary (Type Safety):");
        Console.WriteLine("   " + "=".PadRight(50, '='));

        Console.WriteLine("   IMAGINE if IList<T> were covariant (it's not!):");
        Console.WriteLine();
        Console.WriteLine("   // Step 1: Create a list of Dogs");
        Console.WriteLine("   IList<Dog> dogs = new List<Dog>();");
        Console.WriteLine("   dogs.Add(new Dog { Name = \"Buddy\" });");
        Console.WriteLine();
        Console.WriteLine("   // Step 2: Treat it as IList<Animal> (imaginary covariance)");
        Console.WriteLine("   IList<Animal> animals = dogs;  // Pretend this worked");
        Console.WriteLine();
        Console.WriteLine("   // Step 3: Add a Cat to the \"Animal\" list");
        Console.WriteLine("   animals.Add(new Cat { Name = \"Whiskers\" });  // Would add Cat!");
        Console.WriteLine();
        Console.WriteLine("   // Step 4: Try to retrieve as Dog");
        Console.WriteLine("   Dog myDog = dogs[0];  // Expected Dog, got Cat - BOOM! 💥");
        Console.WriteLine();

        Console.WriteLine("   THE PROBLEM:");
        Console.WriteLine("      1. Started with IList<Dog>");
        Console.WriteLine("      2. Converted to IList<Animal> (imaginary)");
        Console.WriteLine("      3. Added Cat through Animal interface");
        Console.WriteLine("      4. Retrieved from Dog list → Type safety violation!");
        Console.WriteLine();

        Console.WriteLine("   CONCLUSION:");
        Console.WriteLine("      • Allowing variance would break type safety");
        Console.WriteLine("      • Compiler prevents this by making IList<T> INVARIANT");
        Console.WriteLine("      • This is a GOOD thing - catches errors at compile-time!");
    }

    /// <summary>
    /// Demonstrates safe alternatives when you need variance-like behavior.
    /// Shows how to use covariant interfaces for read-only access.
    /// </summary>
    private static void DemonstrateSafeAlternatives()
    {
        Console.WriteLine("3. Safe Alternatives to Invariant Collections:");
        Console.WriteLine("   " + "=".PadRight(50, '='));

        List<Dog> dogs = new()
        {
            new Dog { Name = "Max", Breed = "Golden Retriever" },
            new Dog { Name = "Luna", Breed = "Husky" }
        };

        Console.WriteLine("   SOLUTION 1: Use IEnumerable<T> for read-only access");
        Console.WriteLine();

        // IEnumerable is covariant, so this works!
        IEnumerable<Animal> readOnlyAnimals = dogs;

        Console.WriteLine("   ✅ IEnumerable<Dog> → IEnumerable<Animal> (works!)");
        Console.WriteLine("   Reading animals:");
        foreach (var animal in readOnlyAnimals)
        {
            Console.WriteLine($"      - {animal.Name}");
        }
        Console.WriteLine();

        Console.WriteLine("   WHY IT WORKS:");
        Console.WriteLine("      • IEnumerable<out T> is covariant (read-only)");
        Console.WriteLine("      • Can't add items, so no type safety risk");
        Console.WriteLine("      • Perfect for querying collections");
        Console.WriteLine();

        Console.WriteLine("   SOLUTION 2: Use IReadOnlyCollection<T> or IReadOnlyList<T>");
        Console.WriteLine();

        // IReadOnlyList is also covariant
        IReadOnlyList<Animal> readOnlyList = dogs;

        Console.WriteLine("   ✅ IReadOnlyList<Dog> → IReadOnlyList<Animal> (works!)");
        Console.WriteLine($"   Count: {readOnlyList.Count}");
        Console.WriteLine($"   First animal: {readOnlyList[0].Name}");
        Console.WriteLine();

        Console.WriteLine("   WHY IT WORKS:");
        Console.WriteLine("      • IReadOnlyList<out T> is covariant");
        Console.WriteLine("      • Provides indexer access without modification");
        Console.WriteLine("      • Safe for reading but prevents mutation");
        Console.WriteLine();

        Console.WriteLine("   SOLUTION 3: Create a new list with the desired type");
        Console.WriteLine();

        // Explicitly create a new list
        List<Animal> animalList = new List<Animal>(dogs);

        Console.WriteLine("   ✅ Create new List<Animal> from List<Dog>");
        Console.WriteLine($"   Can now add any Animal type safely");
        animalList.Add(new Cat { Name = "Felix", Color = "Black" });
        Console.WriteLine($"   Total animals: {animalList.Count} (2 dogs + 1 cat)");
        Console.WriteLine();

        Console.WriteLine("   BENEFIT:");
        Console.WriteLine("      • Type-safe: List<Animal> can hold any animal");
        Console.WriteLine("      • Explicit: Clear that we're creating a new collection");
        Console.WriteLine("      • No aliasing: Changes don't affect original list");
    }

    #region Custom Invariant Interface Example

    /// <summary>
    /// Example of a custom invariant interface.
    /// Repository has both Get (output) and Add (input), so it must be invariant.
    /// </summary>
    /// <remarks>
    /// This interface cannot have variance because:
    /// - Get() returns T (would need 'out')
    /// - Add() accepts T (would need 'in')
    /// - Can't have both, so no variance keyword = INVARIANT
    /// </remarks>
    public interface IRepository<T>
    {
        T Get(int id);          // Output position (would want 'out')
        void Add(T item);        // Input position (would want 'in')
        // Therefore: INVARIANT - no variance allowed
    }

    #endregion

    #region Model Classes

    /// <summary>
    /// Base class representing an animal.
    /// </summary>
    public class Animal
    {
        public string Name { get; set; } = string.Empty;
    }

    /// <summary>
    /// Derived class representing a dog.
    /// </summary>
    public class Dog : Animal
    {
        public string Breed { get; set; } = string.Empty;
    }

    /// <summary>
    /// Derived class representing a cat.
    /// </summary>
    public class Cat : Animal
    {
        public string Color { get; set; } = string.Empty;
    }

    #endregion
}
