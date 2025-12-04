using System;
using System.Collections.Generic;

namespace AdvancedConcepts.Samples.CovarianceContravariance.Examples;

/// <summary>
/// Demonstrates contravariance (in T) with IComparer, Action delegates, and custom interfaces.
/// Contravariance allows you to use a more generic type than originally specified.
/// </summary>
/// <remarks>
/// Key Concept: Contravariance works when T appears only in INPUT positions (parameters).
/// The 'in' keyword indicates that T is contravariant.
///
/// Real-world analogy: An animal feeder can feed any animal. Since dogs are animals,
/// you can use an "animal feeder" to feed dogs - what goes in is acceptable!
/// </remarks>
public static class ContravarianceExample
{
    /// <summary>
    /// Runs contravariance demonstrations including IComparer, Action, and event handlers.
    /// </summary>
    public static void Run()
    {
        Console.WriteLine("CONTRAVARIANCE allows you to use a MORE GENERIC type than originally specified.");
        Console.WriteLine("Keyword: 'in' - used for INPUT/PARAMETER types only");
        Console.WriteLine();

        DemonstrateIComparerContravariance();
        Console.WriteLine();

        DemonstrateActionContravariance();
        Console.WriteLine();

        DemonstrateEventHandlerContravariance();
    }

    /// <summary>
    /// Demonstrates contravariance with IComparer&lt;in T&gt;.
    /// IComparer is contravariant because it only accepts items, never returns them.
    /// </summary>
    private static void DemonstrateIComparerContravariance()
    {
        Console.WriteLine("1. IComparer<in T> Contravariance:");
        Console.WriteLine("   " + "=".PadRight(50, '='));

        // Create a comparer that can compare any Animal by name
        IComparer<Animal> animalComparer = new AnimalNameComparer();

        // CONTRAVARIANCE: IComparer<Animal> can be used as IComparer<Dog>
        // This works because IComparer<in T> is contravariant
        IComparer<Dog> dogComparer = animalComparer;

        Console.WriteLine("   ✅ CONTRAVARIANCE SUCCESS:");
        Console.WriteLine("      IComparer<Animal> → IComparer<Dog> (allowed!)");
        Console.WriteLine();

        var dogs = new List<Dog>
        {
            new Dog { Name = "Zeus", Breed = "German Shepherd" },
            new Dog { Name = "Apollo", Breed = "Husky" },
            new Dog { Name = "Bella", Breed = "Beagle" }
        };

        Console.WriteLine("   Unsorted dogs:");
        foreach (var dog in dogs)
            Console.WriteLine($"      - {dog.Name} ({dog.Breed})");

        // Sort using the contravariant comparer
        dogs.Sort(dogComparer);

        Console.WriteLine();
        Console.WriteLine("   Sorted dogs (using Animal comparer):");
        foreach (var dog in dogs)
            Console.WriteLine($"      - {dog.Name} ({dog.Breed})");
        Console.WriteLine();

        Console.WriteLine("   WHY IT WORKS:");
        Console.WriteLine("      • IComparer<in T> only RECEIVES items (contravariant)");
        Console.WriteLine("      • AnimalComparer can compare Dogs (Dogs are Animals)");
        Console.WriteLine("      • More generic type works for more specific type ✓");
        Console.WriteLine("      • If it can compare Animals, it can compare Dogs!");
    }

    /// <summary>
    /// Demonstrates contravariance with Action&lt;in T&gt; delegate.
    /// Action delegates are contravariant because they only accept parameters.
    /// </summary>
    private static void DemonstrateActionContravariance()
    {
        Console.WriteLine("2. Action<in T> Contravariance:");
        Console.WriteLine("   " + "=".PadRight(50, '='));

        // Create an action that handles any Animal
        Action<Animal> handleAnimal = (animal) =>
        {
            Console.WriteLine($"      Processing animal: {animal.Name}");
        };

        // CONTRAVARIANCE: Action<Animal> can be used as Action<Dog>
        Action<Dog> handleDog = handleAnimal;

        Console.WriteLine("   ✅ Action<Animal> → Action<Dog> (contravariance)");
        Console.WriteLine();

        var testDog = new Dog { Name = "Rex", Breed = "Rottweiler" };
        Console.WriteLine("   Calling Action<Dog> with Dog instance:");
        handleDog(testDog);
        Console.WriteLine();

        Console.WriteLine("   WHY IT WORKS:");
        Console.WriteLine("      • Action<in T> only ACCEPTS parameters");
        Console.WriteLine("      • A handler for Animals can handle Dogs");
        Console.WriteLine("      • The Dog parameter is a valid Animal");
        Console.WriteLine();

        Console.WriteLine("   PRACTICAL USE CASE:");
        Console.WriteLine("      Event handlers, callbacks, command patterns");
    }

    /// <summary>
    /// Demonstrates contravariance in event handler scenarios.
    /// Shows how a generic event handler can be used for specific types.
    /// </summary>
    private static void DemonstrateEventHandlerContravariance()
    {
        Console.WriteLine("3. Event Handler Contravariance:");
        Console.WriteLine("   " + "=".PadRight(50, '='));

        // Create a generic animal event handler
        Action<Animal, string> animalEventHandler = (animal, eventType) =>
        {
            Console.WriteLine($"      [Event: {eventType}] {animal.Name} ({animal.GetType().Name})");
        };

        // Use it as a Dog-specific handler (contravariance)
        Action<Dog, string> dogEventHandler = animalEventHandler;

        Console.WriteLine("   Generic Animal event handler used for Dogs:");
        Console.WriteLine();

        var dog = new Dog { Name = "Max", Breed = "Labrador" };
        dogEventHandler(dog, "Bark");
        dogEventHandler(dog, "Eat");
        dogEventHandler(dog, "Sleep");

        Console.WriteLine();
        Console.WriteLine("   BENEFIT:");
        Console.WriteLine("      • Write ONE handler for base type (Animal)");
        Console.WriteLine("      • Reuse for ALL derived types (Dog, Cat, etc.)");
        Console.WriteLine("      • Reduces code duplication");
        Console.WriteLine("      • Follows DRY principle (Don't Repeat Yourself)");
    }

    #region Helper Classes

    /// <summary>
    /// Comparer that sorts animals alphabetically by name.
    /// Can be used contravariantly for any derived animal type.
    /// </summary>
    private class AnimalNameComparer : IComparer<Animal>
    {
        public int Compare(Animal? x, Animal? y)
        {
            if (x == null || y == null) return 0;
            return string.Compare(x.Name, y.Name, StringComparison.Ordinal);
        }
    }

    #endregion

    #region Model Classes

    /// <summary>
    /// Base class representing an animal with common properties.
    /// </summary>
    public class Animal
    {
        public string Name { get; set; } = string.Empty;
    }

    /// <summary>
    /// Derived class representing a dog - a specific type of animal.
    /// </summary>
    public class Dog : Animal
    {
        public string Breed { get; set; } = string.Empty;
    }

    #endregion
}
