using AdvancedCsharpConcepts.Beginner.Polymorphism_AssignCompatibility;
using FluentAssertions;

namespace AdvancedCsharpConcepts.Tests.Beginner;

/// <summary>
/// Comprehensive tests for polymorphism and inheritance concepts.
/// </summary>
public class PolymorphismTests
{
    [Fact]
    public void Mammal_ShouldSetSpecies()
    {
        // Arrange & Act
        var mammal = new Mammal { Species = "Feline" };

        // Assert
        mammal.Species.Should().Be("Feline");
    }

    [Fact]
    public void Animal_ShouldInheritFromMammal()
    {
        // Arrange & Act
        var animal = new Animal();

        // Assert
        animal.Should().BeAssignableTo<Mammal>();
    }

    [Fact]
    public void Animal_ShouldHaveNameProperty()
    {
        // Arrange & Act
        var animal = new Animal { Name = "Generic Animal" };

        // Assert
        animal.Name.Should().Be("Generic Animal");
    }

    [Fact]
    public void Cat_ShouldInheritFromAnimal()
    {
        // Arrange & Act
        var cat = new Cat();

        // Assert
        cat.Should().BeAssignableTo<Animal>();
        cat.Should().BeAssignableTo<Mammal>();
    }

    [Fact]
    public void Cat_ShouldHaveColorProperty()
    {
        // Arrange & Act
        var cat = new Cat { Color = "Orange", Name = "Garfield" };

        // Assert
        cat.Color.Should().Be("Orange");
        cat.Name.Should().Be("Garfield");
    }

    [Fact]
    public void Dog_ShouldInheritFromAnimal()
    {
        // Arrange & Act
        var dog = new Dog();

        // Assert
        dog.Should().BeAssignableTo<Animal>();
        dog.Should().BeAssignableTo<Mammal>();
    }

    [Fact]
    public void Dog_ShouldHaveBreedProperty()
    {
        // Arrange & Act
        var dog = new Dog { Breed = "Golden Retriever", Name = "Max" };

        // Assert
        dog.Breed.Should().Be("Golden Retriever");
        dog.Name.Should().Be("Max");
    }

    [Fact]
    public void Polymorphism_AnimalListShouldAcceptDerivedTypes()
    {
        // Arrange
        var animals = new List<Animal>
        {
            new Cat { Name = "Whiskers", Color = "Gray" },
            new Dog { Name = "Buddy", Breed = "Labrador" },
            new Animal { Name = "Generic" }
        };

        // Assert
        animals.Should().HaveCount(3);
        animals[0].Should().BeOfType<Cat>();
        animals[1].Should().BeOfType<Dog>();
        animals[2].Should().BeOfType<Animal>();
    }

    [Fact]
    public void Upcasting_DogToAnimal_ShouldSucceed()
    {
        // Arrange
        var dog = new Dog { Name = "Rex", Breed = "German Shepherd" };

        // Act
        Animal animal = dog; // Upcasting (implicit)

        // Assert
        animal.Should().BeSameAs(dog);
        animal.Should().BeOfType<Dog>();
    }

    [Fact]
    public void Downcasting_AnimalToCat_WithCastOperator_ShouldSucceedWhenValid()
    {
        // Arrange
        Animal animal = new Cat { Name = "Mittens", Color = "White" };

        // Act
        var cat = (Cat)animal; // Downcasting (explicit)

        // Assert
        cat.Should().NotBeNull();
        cat.Name.Should().Be("Mittens");
        cat.Color.Should().Be("White");
    }

    [Fact]
    public void Downcasting_AnimalToDog_WithCastOperator_ShouldThrowWhenInvalid()
    {
        // Arrange
        Animal animal = new Cat { Name = "Mittens" };

        // Act
        Action act = () => { var dog = (Dog)animal; };

        // Assert
        act.Should().Throw<InvalidCastException>();
    }

    [Fact]
    public void Downcasting_AnimalToCat_WithAsOperator_ShouldReturnNullWhenInvalid()
    {
        // Arrange
        Animal animal = new Dog { Name = "Buddy" };

        // Act
        var cat = animal as Cat;

        // Assert
        cat.Should().BeNull();
    }

    [Fact]
    public void Downcasting_AnimalToCat_WithAsOperator_ShouldSucceedWhenValid()
    {
        // Arrange
        Animal animal = new Cat { Name = "Whiskers", Color = "Orange" };

        // Act
        var cat = animal as Cat;

        // Assert
        cat.Should().NotBeNull();
        cat!.Name.Should().Be("Whiskers");
        cat.Color.Should().Be("Orange");
    }

    [Fact]
    public void IsOperator_ShouldReturnTrueForCorrectType()
    {
        // Arrange
        Animal animal = new Dog { Name = "Max" };

        // Act & Assert
        (animal is Dog).Should().BeTrue();
        (animal is Cat).Should().BeFalse();
        (animal is Animal).Should().BeTrue();
        (animal is Mammal).Should().BeTrue();
    }

    [Fact]
    public void PatternMatching_WithIsOperator_ShouldExtractVariable()
    {
        // Arrange
        Animal animal = new Cat { Name = "Fluffy", Color = "Black" };

        // Act
        if (animal is Cat cat)
        {
            // Assert
            cat.Should().NotBeNull();
            cat.Color.Should().Be("Black");
        }
        else
        {
            Assert.Fail("Pattern matching failed");
        }
    }

    [Theory]
    [InlineData("Whiskers", "Gray")]
    [InlineData("Garfield", "Orange")]
    [InlineData("Tom", "Blue")]
    public void Cat_ShouldAcceptVariousColors(string name, string color)
    {
        // Arrange & Act
        var cat = new Cat { Name = name, Color = color };

        // Assert
        cat.Name.Should().Be(name);
        cat.Color.Should().Be(color);
    }

    [Theory]
    [InlineData("Max", "Golden Retriever")]
    [InlineData("Buddy", "Labrador")]
    [InlineData("Rex", "German Shepherd")]
    public void Dog_ShouldAcceptVariousBreeds(string name, string breed)
    {
        // Arrange & Act
        var dog = new Dog { Name = name, Breed = breed };

        // Assert
        dog.Name.Should().Be(name);
        dog.Breed.Should().Be(breed);
    }

    [Fact]
    public void MultipleInheritanceLevels_ShouldMaintainHierarchy()
    {
        // Arrange
        var dog = new Dog
        {
            Species = "Canis familiaris",
            Name = "Max",
            Breed = "Beagle"
        };

        // Assert
        dog.Species.Should().Be("Canis familiaris");
        dog.Name.Should().Be("Max");
        dog.Breed.Should().Be("Beagle");
    }

    [Fact]
    public void NullableProperties_ShouldAcceptNull()
    {
        // Arrange & Act
        var animal = new Animal { Name = null };
        var cat = new Cat { Color = null };
        var dog = new Dog { Breed = null };

        // Assert
        animal.Name.Should().BeNull();
        cat.Color.Should().BeNull();
        dog.Breed.Should().BeNull();
    }
}
