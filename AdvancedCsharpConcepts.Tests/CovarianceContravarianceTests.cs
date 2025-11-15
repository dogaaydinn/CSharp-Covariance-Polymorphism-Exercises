using AdvancedCsharpConcepts.Advanced.GenericCovarianceContravariance;
using AdvancedCsharpConcepts.Beginner.Polymorphism_AssignCompatibility;
using FluentAssertions;
using Xunit;

namespace AdvancedCsharpConcepts.Tests;

/// <summary>
/// Unit tests for covariance and contravariance concepts
/// </summary>
public class CovarianceContravarianceTests
{
    [Fact]
    public void IEnumerableCovariance_AllowsStringToObjectConversion()
    {
        // Arrange
        IEnumerable<string> stringList = new List<string> { "Alice", "Bob", "Charlie" };

        // Act
        IEnumerable<object> objectList = stringList;

        // Assert
        objectList.Should().NotBeNull();
        objectList.Should().HaveCount(3);
        objectList.Should().Contain("Alice");
    }

    [Fact]
    public void ArrayCovariance_AllowsCatArrayToAnimalArray()
    {
        // Arrange
        Cat[] cats = new Cat[2]
        {
            new Cat { Name = "Fluffy", Species = "Feline" },
            new Cat { Name = "Whiskers", Species = "Feline" }
        };

        // Act
        Animal[] animals = cats;

        // Assert
        animals.Should().NotBeNull();
        animals.Should().HaveCount(2);
        animals[0].Should().BeOfType<Cat>();
    }

    [Fact]
    public void IProducerCovariance_AllowsCatProducerAsAnimalProducer()
    {
        // Arrange
        var catProducer = new CatProducer();

        // Act
        IProducer<Animal> animalProducer = catProducer;
        var animal = animalProducer.Produce();

        // Assert
        animal.Should().NotBeNull();
        animal.Should().BeOfType<Cat>();
    }

    [Fact]
    public void IConsumerContravariance_AllowsAnimalConsumerAsCatConsumer()
    {
        // Arrange
        var animalConsumer = new AnimalConsumer();

        // Act
        IConsumer<Cat> catConsumer = animalConsumer;
        var cat = new Cat { Name = "Test Cat" };

        // Assert - Should not throw
        var act = () => catConsumer.Consume(cat);
        act.Should().NotThrow();
    }

    [Fact]
    public void FuncCovariance_AllowsDerivedTypeAsReturnType()
    {
        // Arrange
        Func<Cat> catProducer = () => new Cat { Name = "Mittens" };

        // Act
        Func<Animal> animalProducer = catProducer;
        var animal = animalProducer();

        // Assert
        animal.Should().NotBeNull();
        animal.Should().BeOfType<Cat>();
        animal.Name.Should().Be("Mittens");
    }

    [Fact]
    public void ActionContravariance_AllowsBaseTypeAsParameter()
    {
        // Arrange
        var capturedAnimal = new Animal();
        Action<Animal> animalAction = a => { capturedAnimal = a; };

        // Act
        Action<Cat> catAction = animalAction;
        var cat = new Cat { Name = "Shadow", Species = "Feline" };
        catAction(cat);

        // Assert
        capturedAnimal.Should().BeOfType<Cat>();
        capturedAnimal.Name.Should().Be("Shadow");
    }
}
