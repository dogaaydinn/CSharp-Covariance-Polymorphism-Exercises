using System;
using System.Collections.Generic;
using System.Linq;

namespace AdvancedConcepts.Samples.CovarianceContravariance.Examples;

/// <summary>
/// Demonstrates real-world applications of variance using the Repository pattern.
/// Shows how variance enables flexible and reusable code in practical scenarios.
/// </summary>
/// <remarks>
/// This example demonstrates:
/// - Covariant read-only repositories (IReadOnlyRepository&lt;out T&gt;)
/// - Contravariant validators (IValidator&lt;in T&gt;)
/// - Contravariant specifications (ISpecification&lt;in T&gt;)
/// - How variance reduces code duplication and increases flexibility
/// </remarks>
public static class RealWorldExample
{
    /// <summary>
    /// Runs comprehensive real-world variance examples including repositories,
    /// validators, and query specifications.
    /// </summary>
    public static void Run()
    {
        Console.WriteLine("Real-world example: Repository pattern with variance");
        Console.WriteLine();

        DemonstrateReadOnlyRepository();
        Console.WriteLine();

        DemonstrateContravariantValidator();
        Console.WriteLine();

        DemonstrateSpecificationPattern();
        Console.WriteLine();

        DemonstrateQueryBuilder();
    }

    /// <summary>
    /// Demonstrates covariant read-only repository pattern.
    /// Read-only repositories can be covariant because they only return data.
    /// </summary>
    private static void DemonstrateReadOnlyRepository()
    {
        Console.WriteLine("1. Read-Only Repository (Covariant):");
        Console.WriteLine("   " + "=".PadRight(50, '='));

        // Create a repository for Dogs
        IReadOnlyRepository<Dog> dogRepo = new DogRepository();

        // COVARIANCE: Treat Dog repository as Animal repository
        IReadOnlyRepository<Animal> animalRepo = dogRepo;

        Console.WriteLine("   ✅ IReadOnlyRepository<Dog> → IReadOnlyRepository<Animal>");
        Console.WriteLine();

        Console.WriteLine("   Retrieving all animals from dog repository:");
        var animals = animalRepo.GetAll();
        foreach (var animal in animals)
        {
            Console.WriteLine($"      - {animal.Name} ({animal.GetType().Name})");
        }
        Console.WriteLine();

        Console.WriteLine("   Retrieving by ID:");
        var firstAnimal = animalRepo.GetById(1);
        Console.WriteLine($"      Found: {firstAnimal.Name}");
        Console.WriteLine();

        Console.WriteLine("   WHY THIS IS USEFUL:");
        Console.WriteLine("      • Write ONE query method accepting IReadOnlyRepository<Animal>");
        Console.WriteLine("      • Works with Dog, Cat, or any animal repository");
        Console.WriteLine("      • Read-only ensures no type safety issues");
        Console.WriteLine("      • Perfect for CQRS pattern (Query side)");
    }

    /// <summary>
    /// Demonstrates contravariant validator pattern.
    /// Validators can be contravariant because they only accept data.
    /// </summary>
    private static void DemonstrateContravariantValidator()
    {
        Console.WriteLine("2. Validator Pattern (Contravariant):");
        Console.WriteLine("   " + "=".PadRight(50, '='));

        // Create a validator that works for any Animal
        IValidator<Animal> animalValidator = new AnimalValidator();

        // CONTRAVARIANCE: Use Animal validator for Dogs
        IValidator<Dog> dogValidator = animalValidator;
        IValidator<Cat> catValidator = animalValidator;

        Console.WriteLine("   ✅ One Animal validator works for all derived types!");
        Console.WriteLine();

        // Validate different animal types
        var validDog = new Dog { Name = "Buddy", Breed = "Labrador", Age = 3 };
        var invalidDog = new Dog { Name = "", Breed = "Poodle", Age = -1 };

        Console.WriteLine("   Validating dogs:");
        ValidateAndPrint(dogValidator, validDog);
        ValidateAndPrint(dogValidator, invalidDog);
        Console.WriteLine();

        var validCat = new Cat { Name = "Whiskers", Color = "Orange", Age = 2 };
        var invalidCat = new Cat { Name = "Shadow", Color = "", Age = 15 };

        Console.WriteLine("   Validating cats:");
        ValidateAndPrint(catValidator, validCat);
        ValidateAndPrint(catValidator, invalidCat);
        Console.WriteLine();

        Console.WriteLine("   BENEFIT:");
        Console.WriteLine("      • Write validation logic ONCE for base type");
        Console.WriteLine("      • Reuse for ALL derived types");
        Console.WriteLine("      • Add new animal types without new validators");
        Console.WriteLine("      • Follows Open/Closed Principle");
    }

    /// <summary>
    /// Demonstrates contravariant specification pattern for complex queries.
    /// Specifications can be contravariant because they only evaluate objects.
    /// </summary>
    private static void DemonstrateSpecificationPattern()
    {
        Console.WriteLine("3. Specification Pattern (Contravariant):");
        Console.WriteLine("   " + "=".PadRight(50, '='));

        // Create specifications for Animals
        ISpecification<Animal> adultAnimalSpec = new AgeSpecification(2); // Adults are 2+ years
        ISpecification<Animal> youngAnimalSpec = new AgeSpecification(0, 1); // Young are 0-1 years

        // CONTRAVARIANCE: Use Animal specifications for Dogs
        ISpecification<Dog> adultDogSpec = adultAnimalSpec;

        var dogs = new List<Dog>
        {
            new Dog { Name = "Max", Breed = "Retriever", Age = 5 },
            new Dog { Name = "Puppy", Breed = "Beagle", Age = 1 },
            new Dog { Name = "Buddy", Breed = "Labrador", Age = 3 }
        };

        Console.WriteLine("   Finding adult dogs (age >= 2):");
        var adultDogs = dogs.Where(d => adultDogSpec.IsSatisfiedBy(d));
        foreach (var dog in adultDogs)
        {
            Console.WriteLine($"      - {dog.Name}, {dog.Age} years old");
        }
        Console.WriteLine();

        Console.WriteLine("   WHY SPECIFICATIONS ARE POWERFUL:");
        Console.WriteLine("      • Encapsulate business rules");
        Console.WriteLine("      • Reusable across entity types");
        Console.WriteLine("      • Composable (can combine with AND/OR)");
        Console.WriteLine("      • Testable in isolation");
    }

    /// <summary>
    /// Demonstrates combining covariance and contravariance in a query builder.
    /// Shows how variance enables fluent, flexible APIs.
    /// </summary>
    private static void DemonstrateQueryBuilder()
    {
        Console.WriteLine("4. Query Builder (Combining Both Variances):");
        Console.WriteLine("   " + "=".PadRight(50, '='));

        // Repository (covariant - produces results)
        IReadOnlyRepository<Dog> dogRepo = new DogRepository();

        // Specification (contravariant - filters results)
        ISpecification<Animal> nameSpec = new NameSpecification("B"); // Names starting with B

        Console.WriteLine("   Querying dogs with names starting with 'B':");
        Console.WriteLine();

        // Combine covariant and contravariant types
        var results = QueryAnimals(dogRepo, nameSpec);

        foreach (var animal in results)
        {
            Console.WriteLine($"      - {animal.Name}");
        }
        Console.WriteLine();

        Console.WriteLine("   WHAT JUST HAPPENED:");
        Console.WriteLine("      1. IReadOnlyRepository<Dog> used as IReadOnlyRepository<Animal> (covariance)");
        Console.WriteLine("      2. ISpecification<Animal> used to filter Dogs (contravariance)");
        Console.WriteLine("      3. Single method works with ANY animal type!");
        Console.WriteLine();

        Console.WriteLine("   REAL-WORLD APPLICATIONS:");
        Console.WriteLine("      • LINQ-style query builders");
        Console.WriteLine("      • Entity Framework Core");
        Console.WriteLine("      • Specification pattern with repositories");
        Console.WriteLine("      • Filter/sorting pipelines");
    }

    /// <summary>
    /// Generic query method leveraging both covariance and contravariance.
    /// This method can query any animal repository with any animal specification.
    /// </summary>
    /// <typeparam name="T">The animal type.</typeparam>
    /// <param name="repository">Covariant repository producing results.</param>
    /// <param name="specification">Contravariant specification filtering results.</param>
    /// <returns>Filtered results.</returns>
    private static IEnumerable<T> QueryAnimals<T>(
        IReadOnlyRepository<T> repository,
        ISpecification<T> specification) where T : Animal
    {
        return repository.GetAll().Where(animal => specification.IsSatisfiedBy(animal));
    }

    /// <summary>
    /// Helper method to validate and print results.
    /// </summary>
    private static void ValidateAndPrint<T>(IValidator<T> validator, T item) where T : Animal
    {
        var result = validator.Validate(item);
        var status = result ? "✅ Valid" : "❌ Invalid";
        Console.WriteLine($"      {status}: {item.Name} ({item.GetType().Name})");
    }

    #region Covariant Interfaces (Producers)

    /// <summary>
    /// Covariant read-only repository interface.
    /// Uses 'out' because T only appears in return positions.
    /// </summary>
    public interface IReadOnlyRepository<out T>
    {
        IEnumerable<T> GetAll();
        T GetById(int id);
        // Note: Can't have Add(T) here - would require 'in', creating invariance
    }

    #endregion

    #region Contravariant Interfaces (Consumers)

    /// <summary>
    /// Contravariant validator interface.
    /// Uses 'in' because T only appears in parameter positions.
    /// </summary>
    public interface IValidator<in T>
    {
        bool Validate(T item);
        // Note: Can't return T here - would require 'out', creating invariance
    }

    /// <summary>
    /// Contravariant specification interface for query filtering.
    /// Uses 'in' because T only appears in parameter positions.
    /// </summary>
    public interface ISpecification<in T>
    {
        bool IsSatisfiedBy(T candidate);
    }

    #endregion

    #region Implementations

    /// <summary>
    /// Concrete implementation of dog repository.
    /// </summary>
    private class DogRepository : IReadOnlyRepository<Dog>
    {
        private readonly List<Dog> _dogs = new()
        {
            new Dog { Name = "Rover", Breed = "Dalmatian", Age = 4 },
            new Dog { Name = "Bella", Breed = "Beagle", Age = 2 },
            new Dog { Name = "Spot", Breed = "Bulldog", Age = 6 }
        };

        public IEnumerable<Dog> GetAll() => _dogs;

        public Dog GetById(int id) => _dogs.FirstOrDefault() ?? new Dog();
    }

    /// <summary>
    /// Validator that validates common animal properties.
    /// Can be used contravariantly for any derived animal type.
    /// </summary>
    private class AnimalValidator : IValidator<Animal>
    {
        public bool Validate(Animal item)
        {
            if (string.IsNullOrWhiteSpace(item.Name))
                return false;

            if (item.Age < 0 || item.Age > 30)
                return false;

            // Additional validation specific to subclasses
            return item switch
            {
                Dog dog => !string.IsNullOrWhiteSpace(dog.Breed),
                Cat cat => !string.IsNullOrWhiteSpace(cat.Color),
                _ => true
            };
        }
    }

    /// <summary>
    /// Specification that filters animals by age range.
    /// </summary>
    private class AgeSpecification : ISpecification<Animal>
    {
        private readonly int _minAge;
        private readonly int _maxAge;

        public AgeSpecification(int minAge, int maxAge = int.MaxValue)
        {
            _minAge = minAge;
            _maxAge = maxAge;
        }

        public bool IsSatisfiedBy(Animal candidate)
        {
            return candidate.Age >= _minAge && candidate.Age <= _maxAge;
        }
    }

    /// <summary>
    /// Specification that filters animals by name pattern.
    /// </summary>
    private class NameSpecification : ISpecification<Animal>
    {
        private readonly string _pattern;

        public NameSpecification(string pattern)
        {
            _pattern = pattern;
        }

        public bool IsSatisfiedBy(Animal candidate)
        {
            return candidate.Name.StartsWith(_pattern, StringComparison.OrdinalIgnoreCase);
        }
    }

    #endregion

    #region Model Classes

    /// <summary>
    /// Base animal class with common properties.
    /// </summary>
    public class Animal
    {
        public string Name { get; set; } = string.Empty;
        public int Age { get; set; }
    }

    /// <summary>
    /// Dog entity with breed-specific properties.
    /// </summary>
    public class Dog : Animal
    {
        public string Breed { get; set; } = string.Empty;
    }

    /// <summary>
    /// Cat entity with color-specific properties.
    /// </summary>
    public class Cat : Animal
    {
        public string Color { get; set; } = string.Empty;
    }

    #endregion
}
