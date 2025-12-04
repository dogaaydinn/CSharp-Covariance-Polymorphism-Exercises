using FluentAssertions;
using NUnit.Framework;

namespace Covariance.Tests;

[TestFixture]
public class CovarianceTests
{
    // ========== TODO 1 & 2: ICovariantRepository Tests ==========
    [Test]
    public void CovariantRepository_ShouldAllowDogRepositoryAsAnimalRepository()
    {
        // This test verifies that ICovariantRepository<Dog> can be used as ICovariantRepository<Animal>

        // Arrange
        var dogRepo = new DogRepository();

        // Act - Covariance allows this assignment
        Program.ICovariantRepository<Animal> animalRepo = dogRepo;

        // Assert
        animalRepo.Should().NotBeNull();
        animalRepo.GetAll().Should().HaveCount(2);
    }

    [Test]
    public void CovariantRepository_GetAll_ShouldReturnAllAnimals()
    {
        // Arrange
        var repo = new Program.AnimalRepository();

        // Act
        var animals = repo.GetAll().ToList();

        // Assert
        animals.Should().HaveCountGreaterThan(0);
        animals.Should().AllBeOfType<Animal>();
    }

    [Test]
    public void CovariantRepository_Get_ShouldReturnSpecificAnimal()
    {
        // Arrange
        var repo = new Program.AnimalRepository();

        // Act
        var animal = repo.Get(0);

        // Assert
        animal.Should().NotBeNull();
        animal.Should().BeOfType<Animal>();
    }

    // ========== TODO 3: IEnumerable Covariance Tests ==========
    [Test]
    public void ConvertToAnimals_ShouldAcceptDogCollection()
    {
        // Arrange
        var dogs = new List<Dog>
        {
            new Dog("Rex", 5, "German Shepherd"),
            new Dog("Max", 3, "Golden Retriever")
        };

        // Act
        var animals = Program.ConvertToAnimals(dogs).ToList();

        // Assert
        animals.Should().HaveCount(2);
        animals.Should().AllBeAssignableTo<Animal>();
        animals[0].Name.Should().Be("Rex");
        animals[1].Name.Should().Be("Max");
    }

    [Test]
    public void ConvertToAnimals_ShouldAcceptCatCollection()
    {
        // Arrange
        var cats = new List<Cat>
        {
            new Cat("Whiskers", 2, true),
            new Cat("Shadow", 4, false)
        };

        // Act
        var animals = Program.ConvertToAnimals(cats).ToList();

        // Assert
        animals.Should().HaveCount(2);
        animals.Should().AllBeAssignableTo<Animal>();
    }

    [Test]
    public void ConvertToAnimals_ShouldAcceptMixedMammals()
    {
        // Arrange
        var mammals = new List<Mammal>
        {
            new Dog("Buddy", 5, "Beagle"),
            new Cat("Fluffy", 3, true)
        };

        // Act
        var animals = Program.ConvertToAnimals(mammals).ToList();

        // Assert
        animals.Should().HaveCount(2);
        animals.Should().AllBeAssignableTo<Animal>();
    }

    // ========== TODO 4: Func Delegate Covariance Tests ==========
    [Test]
    public void GetAnimalFactory_Dog_ShouldReturnDogFactory()
    {
        // Act
        var factory = Program.GetAnimalFactory("Dog");
        var animal = factory();

        // Assert
        animal.Should().NotBeNull();
        animal.Should().BeOfType<Dog>();
        animal.MakeSound().Should().Be("Woof!");
    }

    [Test]
    public void GetAnimalFactory_Cat_ShouldReturnCatFactory()
    {
        // Act
        var factory = Program.GetAnimalFactory("Cat");
        var animal = factory();

        // Assert
        animal.Should().NotBeNull();
        animal.Should().BeOfType<Cat>();
        animal.MakeSound().Should().Be("Meow!");
    }

    [Test]
    public void GetAnimalFactory_Unknown_ShouldReturnBirdFactory()
    {
        // Act
        var factory = Program.GetAnimalFactory("Unknown");
        var animal = factory();

        // Assert
        animal.Should().NotBeNull();
        animal.Should().BeOfType<Bird>();
    }

    // ========== TODO 5: Array Covariance Tests ==========
    [Test]
    public void DemonstrateArrayCovariance_ShouldNotThrow()
    {
        // Act & Assert
        // This should execute without throwing (it handles exceptions internally)
        Action act = () => Program.DemonstrateArrayCovariance();
        act.Should().NotThrow();
    }

    // ========== TODO 6: Safe Covariant Collection Tests ==========
    [Test]
    public void GetSafeAnimalList_ShouldReturnReadOnlyList()
    {
        // Act
        var animals = Program.GetSafeAnimalList();

        // Assert
        animals.Should().NotBeNull();
        animals.Should().BeAssignableTo<IReadOnlyList<Animal>>();
        animals.Should().HaveCountGreaterThan(0);
    }

    [Test]
    public void GetSafeAnimalList_ShouldContainOnlyDogs()
    {
        // Act
        var animals = Program.GetSafeAnimalList();

        // Assert
        animals.Should().AllBeOfType<Dog>();
    }

    [Test]
    public void GetSafeAnimalList_ShouldBeSafe()
    {
        // Act
        var animals = Program.GetSafeAnimalList();

        // Assert - IReadOnlyList doesn't have Add/Remove methods
        animals.Should().NotBeAssignableTo<IList<Animal>>();
        // This ensures type safety at compile time
    }

    // ========== Integration Test ==========
    [Test]
    public void Covariance_IntegrationTest_ShouldDemonstrateAllConcepts()
    {
        // Test 1: IEnumerable covariance
        IEnumerable<Dog> dogs = new List<Dog> { new Dog("Rex", 5, "Lab") };
        IEnumerable<Animal> animals1 = dogs; // Covariance!
        animals1.Should().HaveCount(1);

        // Test 2: Func covariance
        Func<Dog> dogFactory = () => new Dog("Max", 3, "Beagle");
        Func<Animal> animalFactory = dogFactory; // Covariance!
        animalFactory().Should().BeOfType<Dog>();

        // Test 3: IReadOnlyList covariance
        IReadOnlyList<Cat> cats = new List<Cat> { new Cat("Whiskers", 2, true) };
        IReadOnlyList<Animal> animals2 = cats; // Covariance!
        animals2.Should().HaveCount(1);
    }
}

// Helper class for testing covariant repository
public class DogRepository : Program.ICovariantRepository<Dog>
{
    private readonly List<Dog> _dogs = new()
    {
        new Dog("Rex", 5, "German Shepherd"),
        new Dog("Max", 3, "Golden Retriever")
    };

    public Dog Get(int id) => _dogs[id];
    public IEnumerable<Dog> GetAll() => _dogs;
}
