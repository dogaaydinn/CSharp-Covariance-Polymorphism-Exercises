using Xunit;
using FluentAssertions;
using AdvancedCsharpConcepts.Beginner.Polymorphism_AssignCompatibility;

namespace AdvancedConcepts.UnitTests.Beginner;

/// <summary>
/// Tests for assignment compatibility and polymorphic assignments
/// </summary>
public class AssignmentCompatibilityTests
{
    #region Upcasting Tests

    [Fact]
    public void Upcast_DogToAnimal_ShouldSucceed()
    {
        // Arrange
        Dog dog = new();

        // Act
        Animal animal = dog;  // Implicit upcast

        // Assert
        animal.Should().NotBeNull();
        animal.Should().BeOfType<Dog>();
    }

    [Fact]
    public void Upcast_CatToAnimal_ShouldSucceed()
    {
        // Arrange
        Cat cat = new();

        // Act
        Animal animal = cat;  // Implicit upcast

        // Assert
        animal.Should().NotBeNull();
        animal.Should().BeOfType<Cat>();
    }

    [Fact]
    public void Upcast_DogToMammal_ShouldSucceed()
    {
        // Arrange
        Dog dog = new();

        // Act
        Mammal mammal = dog;  // Implicit upcast to intermediate class

        // Assert
        mammal.Should().NotBeNull();
        mammal.Should().BeOfType<Dog>();
    }

    [Fact]
    public void Upcast_CatToMammal_ShouldSucceed()
    {
        // Arrange
        Cat cat = new();

        // Act
        Mammal mammal = cat;  // Implicit upcast to intermediate class

        // Assert
        mammal.Should().NotBeNull();
        mammal.Should().BeOfType<Cat>();
    }

    [Fact]
    public void Upcast_AnimalToMammal_ShouldSucceed()
    {
        // Arrange
        Animal animal = new Dog();

        // Act
        Mammal mammal = animal;  // Implicit upcast

        // Assert
        mammal.Should().NotBeNull();
        mammal.Should().BeOfType<Dog>();
    }

    #endregion

    #region Downcasting Tests (as operator)

    [Fact]
    public void Downcast_AnimalToDog_WithValidType_ShouldSucceed()
    {
        // Arrange
        Animal animal = new Dog();

        // Act
        Dog? dog = animal as Dog;

        // Assert
        dog.Should().NotBeNull();
        dog.Should().BeOfType<Dog>();
    }

    [Fact]
    public void Downcast_AnimalToDog_WithInvalidType_ShouldReturnNull()
    {
        // Arrange
        Animal animal = new Cat();

        // Act
        Dog? dog = animal as Dog;

        // Assert
        dog.Should().BeNull();
    }

    [Fact]
    public void Downcast_AnimalToCat_WithValidType_ShouldSucceed()
    {
        // Arrange
        Animal animal = new Cat();

        // Act
        Cat? cat = animal as Cat;

        // Assert
        cat.Should().NotBeNull();
        cat.Should().BeOfType<Cat>();
    }

    [Fact]
    public void Downcast_AnimalToCat_WithInvalidType_ShouldReturnNull()
    {
        // Arrange
        Animal animal = new Dog();

        // Act
        Cat? cat = animal as Cat;

        // Assert
        cat.Should().BeNull();
    }

    [Fact]
    public void Downcast_MammalToDog_WithValidType_ShouldSucceed()
    {
        // Arrange
        Mammal mammal = new Dog();

        // Act
        Dog? dog = mammal as Dog;

        // Assert
        dog.Should().NotBeNull();
        dog.Should().BeOfType<Dog>();
    }

    #endregion

    #region Downcasting Tests (explicit cast)

    [Fact]
    public void ExplicitCast_AnimalToDog_WithValidType_ShouldSucceed()
    {
        // Arrange
        Animal animal = new Dog();

        // Act
        Dog dog = (Dog)animal;

        // Assert
        dog.Should().NotBeNull();
        dog.Should().BeOfType<Dog>();
    }

    [Fact]
    public void ExplicitCast_AnimalToDog_WithInvalidType_ShouldThrow()
    {
        // Arrange
        Animal animal = new Cat();

        // Act
        Action act = () => { Dog dog = (Dog)animal; };

        // Assert
        act.Should().Throw<InvalidCastException>();
    }

    [Fact]
    public void ExplicitCast_AnimalToCat_WithValidType_ShouldSucceed()
    {
        // Arrange
        Animal animal = new Cat();

        // Act
        Cat cat = (Cat)animal;

        // Assert
        cat.Should().NotBeNull();
        cat.Should().BeOfType<Cat>();
    }

    [Fact]
    public void ExplicitCast_MammalToDog_WithValidType_ShouldSucceed()
    {
        // Arrange
        Mammal mammal = new Dog();

        // Act
        Dog dog = (Dog)mammal;

        // Assert
        dog.Should().NotBeNull();
        dog.Should().BeOfType<Dog>();
    }

    #endregion

    #region Is Operator Tests

    [Fact]
    public void IsOperator_AnimalIsDog_WithDog_ShouldReturnTrue()
    {
        // Arrange
        Animal animal = new Dog();

        // Act
        bool isDog = animal is Dog;

        // Assert
        isDog.Should().BeTrue();
    }

    [Fact]
    public void IsOperator_AnimalIsDog_WithCat_ShouldReturnFalse()
    {
        // Arrange
        Animal animal = new Cat();

        // Act
        bool isDog = animal is Dog;

        // Assert
        isDog.Should().BeFalse();
    }

    [Fact]
    public void IsOperator_AnimalIsMammal_WithDog_ShouldReturnTrue()
    {
        // Arrange
        Animal animal = new Dog();

        // Act
        bool isMammal = animal is Mammal;

        // Assert
        isMammal.Should().BeTrue();
    }

    [Fact]
    public void IsOperator_NullAnimal_ShouldReturnFalse()
    {
        // Arrange
        Animal? animal = null;

        // Act
        bool isDog = animal is Dog;

        // Assert
        isDog.Should().BeFalse();
    }

    #endregion

    #region Pattern Matching Tests

    [Fact]
    public void PatternMatching_WithDog_ShouldMatchDogType()
    {
        // Arrange
        Animal animal = new Dog();
        string result = string.Empty;

        // Act
        if (animal is Dog dog)
        {
            result = "Dog";
        }

        // Assert
        result.Should().Be("Dog");
    }

    [Fact]
    public void PatternMatching_WithCat_ShouldNotMatchDogType()
    {
        // Arrange
        Animal animal = new Cat();
        bool matchedDog = false;

        // Act
        if (animal is Dog dog)
        {
            matchedDog = true;
        }

        // Assert
        matchedDog.Should().BeFalse();
    }

    [Fact]
    public void SwitchPattern_WithDifferentAnimals_ShouldMatchCorrectly()
    {
        // Arrange
        Animal[] animals = { new Dog(), new Cat(), new Dog() };
        int dogCount = 0;
        int catCount = 0;

        // Act
        foreach (var animal in animals)
        {
            switch (animal)
            {
                case Dog:
                    dogCount++;
                    break;
                case Cat:
                    catCount++;
                    break;
            }
        }

        // Assert
        dogCount.Should().Be(2);
        catCount.Should().Be(1);
    }

    #endregion

    #region Array Covariance Tests

    [Fact]
    public void ArrayCovariance_DogArrayToAnimalArray_ShouldSucceed()
    {
        // Arrange
        Dog[] dogs = { new Dog(), new Dog() };

        // Act
        Animal[] animals = dogs;  // Array covariance

        // Assert
        animals.Should().NotBeNull();
        animals.Should().HaveCount(2);
        animals.Should().AllBeOfType<Dog>();
    }

    [Fact]
    public void ArrayCovariance_MixedTypes_WriteOperation_ShouldThrow()
    {
        // Arrange
        Dog[] dogs = { new Dog(), new Dog() };
        Animal[] animals = dogs;

        // Act
        Action act = () => animals[0] = new Cat();  // Runtime error!

        // Assert
        act.Should().Throw<ArrayTypeMismatchException>();
    }

    #endregion

    #region Null Safety Tests

    [Fact]
    public void AsOperator_WithNull_ShouldReturnNull()
    {
        // Arrange
        Animal? animal = null;

        // Act
        Dog? dog = animal as Dog;

        // Assert
        dog.Should().BeNull();
    }

    [Fact]
    public void ExplicitCast_WithNull_ShouldThrow()
    {
        // Arrange
        Animal? animal = null;

        // Act
        Action act = () => { Dog dog = (Dog)animal!; };

        // Assert
        act.Should().Throw<NullReferenceException>();
    }

    #endregion
}
