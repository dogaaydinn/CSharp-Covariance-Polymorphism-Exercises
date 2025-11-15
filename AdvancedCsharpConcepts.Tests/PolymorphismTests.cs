using AdvancedCsharpConcepts.Beginner.Override_Upcast_Downcast;
using AdvancedCsharpConcepts.Beginner.Polymorphism_AssignCompatibility;
using FluentAssertions;
using Xunit;

namespace AdvancedCsharpConcepts.Tests;

/// <summary>
/// Unit tests for polymorphism and assignment compatibility concepts
/// </summary>
public class PolymorphismTests
{
    [Fact]
    public void Animal_CanBeAssignedToMammal_WhenUpcasting()
    {
        // Arrange & Act
        Mammal mammal = new Animal { Species = "Generic Animal" };

        // Assert
        mammal.Should().NotBeNull();
        mammal.Should().BeOfType<Animal>();
        mammal.Species.Should().Be("Generic Animal");
    }

    [Fact]
    public void Cat_InheritsFromAnimal()
    {
        // Arrange
        var cat = new Cat { Species = "Feline", Name = "Whiskers", Color = "Orange" };

        // Act
        Animal animal = cat;

        // Assert
        animal.Should().NotBeNull();
        animal.Should().BeOfType<Cat>();
        animal.Name.Should().Be("Whiskers");
    }

    [Fact]
    public void Dog_InheritsFromAnimal()
    {
        // Arrange
        var dog = new Dog { Species = "Canine", Name = "Buddy", Breed = "Golden Retriever" };

        // Act
        Animal animal = dog;

        // Assert
        animal.Should().NotBeNull();
        animal.Should().BeOfType<Dog>();
        animal.Name.Should().Be("Buddy");
    }

    [Fact]
    public void Vehicle_CanBeOverridden_ByCarAndBike()
    {
        // Arrange
        var vehicles = new List<Vehicle>
        {
            new Car(),
            new Bike()
        };

        // Act & Assert
        vehicles.Should().HaveCount(2);
        vehicles[0].Should().BeOfType<Car>();
        vehicles[1].Should().BeOfType<Bike>();
    }

    [Fact]
    public void IsOperator_CorrectlyIdentifiesType()
    {
        // Arrange
        Mammal mammal = new Dog { Species = "Dog", Breed = "Labrador", Name = "Max" };

        // Act & Assert
        (mammal is Dog).Should().BeTrue();
        (mammal is Mammal).Should().BeTrue();
        (mammal is Animal).Should().BeTrue();
        (mammal is Cat).Should().BeFalse();
    }

    [Fact]
    public void AsOperator_ReturnsNull_WhenCastFails()
    {
        // Arrange
        Mammal mammal = new Animal();

        // Act
        var cat = mammal as Cat;

        // Assert
        cat.Should().BeNull();
    }

    [Fact]
    public void AsOperator_ReturnsObject_WhenCastSucceeds()
    {
        // Arrange
        Animal animal = new Cat { Name = "Fluffy" };

        // Act
        var cat = animal as Cat;

        // Assert
        cat.Should().NotBeNull();
        cat!.Name.Should().Be("Fluffy");
    }
}
