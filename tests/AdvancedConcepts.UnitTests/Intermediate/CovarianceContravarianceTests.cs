using FluentAssertions;

namespace AdvancedCsharpConcepts.Tests.Intermediate;

/// <summary>
/// Tests for covariance and contravariance concepts.
/// </summary>
public class CovarianceContravarianceTests
{
    // Test classes for covariance/contravariance
    private class Animal { public string Name { get; set; } = string.Empty; }
    private class Dog : Animal { public string Breed { get; set; } = string.Empty; }
    private class Cat : Animal { public string Color { get; set; } = string.Empty; }

    [Fact]
    public void ArrayCovariance_DogArrayToAnimalArray_ShouldSucceed()
    {
        // Arrange
        Dog[] dogs = new[]
        {
            new Dog { Name = "Max", Breed = "Golden Retriever" },
            new Dog { Name = "Buddy", Breed = "Labrador" }
        };

        // Act
        Animal[] animals = dogs; // Array covariance

        // Assert
        animals.Should().HaveCount(2);
        animals[0].Should().BeOfType<Dog>();
    }

    [Fact]
    public void ArrayCovariance_WritingWrongType_ShouldThrowAtRuntime()
    {
        // Arrange
        Dog[] dogs = new[] { new Dog { Name = "Max" } };
        Animal[] animals = dogs; // Array covariance

        // Act - Try to add a Cat to Dog array
        Action act = () => animals[0] = new Cat { Name = "Whiskers" };

        // Assert
        act.Should().Throw<ArrayTypeMismatchException>();
    }

    [Fact]
    public void IEnumerableCovariance_DogListToAnimalEnumerable_ShouldSucceed()
    {
        // Arrange
        List<Dog> dogs = new()
        {
            new Dog { Name = "Rex", Breed = "German Shepherd" }
        };

        // Act
        IEnumerable<Animal> animals = dogs; // Covariance

        // Assert
        animals.Should().HaveCount(1);
        animals.First().Should().BeOfType<Dog>();
    }

    [Fact]
    public void IEnumerableCovariance_WithStringsToObjects_ShouldWork()
    {
        // Arrange
        List<string> strings = new() { "Hello", "World" };

        // Act
        IEnumerable<object> objects = strings; // Covariance

        // Assert
        objects.Should().HaveCount(2);
        objects.Should().AllBeOfType<string>();
    }

    [Fact]
    public void FuncCovariance_ReturningDog_AssignableToReturningAnimal()
    {
        // Arrange
        Func<Dog> getDog = () => new Dog { Name = "Spot", Breed = "Beagle" };

        // Act
        Func<Animal> getAnimal = getDog; // Covariance

        // Assert
        var result = getAnimal();
        result.Should().BeOfType<Dog>();
        result.Name.Should().Be("Spot");
    }

    [Fact]
    public void ActionContravariance_AcceptingAnimal_AssignableToAcceptingDog()
    {
        // Arrange
        var processedAnimals = new List<string>();
        Action<Animal> processAnimal = (animal) => processedAnimals.Add(animal.Name);

        // Act
        Action<Dog> processDog = processAnimal; // Contravariance
        processDog(new Dog { Name = "Fido", Breed = "Poodle" });

        // Assert
        processedAnimals.Should().Contain("Fido");
    }

    [Fact]
    public void IComparer Contravariance_AnimalComparer_UsableForDogs()
    {
        // Arrange
        var animalComparer = Comparer<Animal>.Create((a, b) =>
            string.Compare(a.Name, b.Name, StringComparison.Ordinal));

        // Act
        IComparer<Dog> dogComparer = animalComparer; // Contravariance
        var dogs = new List<Dog>
        {
            new Dog { Name = "Zeke" },
            new Dog { Name = "Ace" }
        };
        dogs.Sort(dogComparer);

        // Assert
        dogs[0].Name.Should().Be("Ace");
        dogs[1].Name.Should().Be("Zeke");
    }

    [Fact]
    public void GenericInterfaceCovariance_IEnumerableOfDerived()
    {
        // Arrange
        IEnumerable<Dog> dogs = new List<Dog>
        {
            new Dog { Name = "Max" }
        };

        // Act
        IEnumerable<Animal> animals = dogs;
        var firstAnimal = animals.First();

        // Assert
        firstAnimal.Should().BeOfType<Dog>();
    }

    [Fact]
    public void DelegateCovariance_FuncReturningDerivedType()
    {
        // Arrange
        Func<Dog> createDog = () => new Dog { Name = "Lucky", Breed = "Husky" };

        // Act
        Func<object> createObject = createDog; // Covariance
        var result = createObject();

        // Assert
        result.Should().BeOfType<Dog>();
        ((Dog)result).Breed.Should().Be("Husky");
    }

    [Fact]
    public void DelegateContravariance_ActionAcceptingBaseType()
    {
        // Arrange
        var log = new List<string>();
        Action<object> logObject = (obj) => log.Add(obj?.ToString() ?? "null");

        // Act
        Action<string> logString = logObject; // Contravariance
        logString("Hello");
        logString("World");

        // Assert
        log.Should().Equal("Hello", "World");
    }

    [Theory]
    [InlineData(1)]
    [InlineData(5)]
    [InlineData(10)]
    public void Covariance_ShouldWorkWithDifferentCollectionSizes(int count)
    {
        // Arrange
        var dogs = Enumerable.Range(0, count)
            .Select(i => new Dog { Name = $"Dog{i}", Breed = "Mixed" })
            .ToList();

        // Act
        IEnumerable<Animal> animals = dogs;

        // Assert
        animals.Should().HaveCount(count);
        animals.Should().AllBeOfType<Dog>();
    }

    [Fact]
    public void MultipleLevelsOfCovariance_ShouldWork()
    {
        // Arrange
        IEnumerable<Dog> dogs = new List<Dog> { new Dog { Name = "Rover" } };

        // Act - Multiple levels: Dog -> Animal -> object
        IEnumerable<Animal> animals = dogs;
        IEnumerable<object> objects = animals;

        // Assert
        objects.Should().HaveCount(1);
        objects.First().Should().BeOfType<Dog>();
    }
}
