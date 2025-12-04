using FluentAssertions;
using NUnit.Framework;

namespace Contravariance.Tests;

[TestFixture]
public class ContravarianceTests
{
    // ========== TODO 1 & 2: IContravariantComparer Tests ==========
    [Test]
    public void ContravariantComparer_ShouldAllowAnimalComparerForDogs()
    {
        // This test verifies that IContravariantComparer<Animal> can be used as IContravariantComparer<Dog>

        // Arrange
        var animalComparer = new Program.AnimalWeightComparer();

        // Act - Contravariance allows this assignment
        Program.IContravariantComparer<Dog> dogComparer = animalComparer;

        var dog1 = new Dog("Rex", 5, 25.5, "German Shepherd");
        var dog2 = new Dog("Max", 3, 30.0, "Golden Retriever");

        int result = dogComparer.Compare(dog1, dog2);

        // Assert
        result.Should().BeLessThan(0, "Rex (25.5kg) is lighter than Max (30kg)");
    }

    [Test]
    public void AnimalWeightComparer_Compare_ShouldCompareByWeight()
    {
        // Arrange
        var comparer = new Program.AnimalWeightComparer();
        var lighter = new Animal("Light", 2, 10.0);
        var heavier = new Animal("Heavy", 3, 20.0);

        // Act
        int result1 = comparer.Compare(lighter, heavier);
        int result2 = comparer.Compare(heavier, lighter);
        int result3 = comparer.Compare(lighter, lighter);

        // Assert
        result1.Should().BeLessThan(0, "lighter < heavier");
        result2.Should().BeGreaterThan(0, "heavier > lighter");
        result3.Should().Be(0, "lighter == lighter");
    }

    [Test]
    public void AnimalWeightComparer_Equals_ShouldCheckWeightEquality()
    {
        // Arrange
        var comparer = new Program.AnimalWeightComparer();
        var animal1 = new Animal("A1", 2, 15.0);
        var animal2 = new Animal("A2", 3, 15.0);
        var animal3 = new Animal("A3", 4, 20.0);

        // Act & Assert
        comparer.Equals(animal1, animal2).Should().BeTrue("same weight");
        comparer.Equals(animal1, animal3).Should().BeFalse("different weight");
    }

    // ========== TODO 3: Action Contravariance Tests ==========
    [Test]
    public void ProcessAnimals_ShouldProcessAllDogs()
    {
        // Arrange
        var processedAnimals = new List<string>();
        Action<Animal> processor = animal => processedAnimals.Add(animal.Name);

        var dogs = new List<Dog>
        {
            new Dog("Rex", 5, 25.5, "German Shepherd"),
            new Dog("Max", 3, 30.0, "Golden Retriever"),
            new Dog("Buddy", 4, 22.0, "Beagle")
        };

        // Act
        Program.ProcessAnimals(processor, dogs);

        // Assert
        processedAnimals.Should().HaveCount(3);
        processedAnimals.Should().Contain(new[] { "Rex", "Max", "Buddy" });
    }

    [Test]
    public void ProcessAnimals_ShouldAllowAnimalProcessor()
    {
        // Arrange
        int count = 0;
        Action<Animal> processor = animal => count++;

        var dogs = new List<Dog>
        {
            new Dog("D1", 1, 10.0, "Breed1"),
            new Dog("D2", 2, 20.0, "Breed2")
        };

        // Act
        Program.ProcessAnimals(processor, dogs);

        // Assert
        count.Should().Be(2, "processed 2 dogs");
    }

    // ========== TODO 4: Func Variance Tests ==========
    [Test]
    public void GetAnimalNameGetter_ShouldReturnNameGetter()
    {
        // Act
        var nameGetter = Program.GetAnimalNameGetter();
        var dog = new Dog("Rex", 5, 25.5, "German Shepherd");
        string name = nameGetter(dog);

        // Assert
        name.Should().Be("Rex");
    }

    [Test]
    public void GetAnimalNameGetter_ShouldWorkWithAnyDog()
    {
        // Arrange
        var nameGetter = Program.GetAnimalNameGetter();
        var dogs = new List<Dog>
        {
            new Dog("Max", 3, 30.0, "Golden Retriever"),
            new Dog("Buddy", 4, 22.0, "Beagle")
        };

        // Act
        var names = dogs.Select(nameGetter).ToList();

        // Assert
        names.Should().Equal("Max", "Buddy");
    }

    // ========== TODO 5: IComparer Contravariance Tests ==========
    [Test]
    public void SortDogsUsingAnimalComparer_ShouldSortByWeight()
    {
        // Arrange
        var dogs = new List<Dog>
        {
            new Dog("Heavy", 5, 35.0, "Breed1"),
            new Dog("Light", 3, 15.0, "Breed2"),
            new Dog("Medium", 4, 25.0, "Breed3")
        };

        // Act
        var sorted = Program.SortDogsUsingAnimalComparer(dogs);

        // Assert
        sorted[0].Name.Should().Be("Light");
        sorted[1].Name.Should().Be("Medium");
        sorted[2].Name.Should().Be("Heavy");
    }

    [Test]
    public void SortDogsUsingAnimalComparer_ShouldHandleEmptyList()
    {
        // Arrange
        var dogs = new List<Dog>();

        // Act
        var sorted = Program.SortDogsUsingAnimalComparer(dogs);

        // Assert
        sorted.Should().BeEmpty();
    }

    [Test]
    public void SortDogsUsingAnimalComparer_ShouldHandleSingleDog()
    {
        // Arrange
        var dogs = new List<Dog>
        {
            new Dog("Solo", 5, 25.0, "Breed1")
        };

        // Act
        var sorted = Program.SortDogsUsingAnimalComparer(dogs);

        // Assert
        sorted.Should().HaveCount(1);
        sorted[0].Name.Should().Be("Solo");
    }

    // ========== TODO 6: Event Handler Contravariance Tests ==========
    [Test]
    public void GetDogEventHandler_ShouldHandleAnimalEvents()
    {
        // Act
        var handler = Program.GetDogEventHandler();

        // Assert
        handler.Should().NotBeNull();

        // Verify it can handle events
        bool handled = false;
        handler = (sender, e) =>
        {
            handled = true;
            e.Animal.Should().NotBeNull();
        };

        var dog = new Dog("Rex", 5, 25.5, "German Shepherd");
        var eventArgs = new AnimalEventArgs(dog, "DogBark");

        handler.Invoke(this, eventArgs);
        handled.Should().BeTrue();
    }

    [Test]
    public void GetDogEventHandler_ShouldReceiveCorrectEventData()
    {
        // Arrange
        string? receivedName = null;
        string? receivedEventType = null;

        EventHandler<AnimalEventArgs> handler = (sender, e) =>
        {
            receivedName = e.Animal.Name;
            receivedEventType = e.EventType;
        };

        var dog = new Dog("Max", 3, 30.0, "Golden Retriever");
        var eventArgs = new AnimalEventArgs(dog, "DogArrival");

        // Act
        handler.Invoke(this, eventArgs);

        // Assert
        receivedName.Should().Be("Max");
        receivedEventType.Should().Be("DogArrival");
    }

    // ========== Integration Test ==========
    [Test]
    public void Contravariance_IntegrationTest_ShouldDemonstrateAllConcepts()
    {
        // Test 1: Action contravariance
        Action<Animal> animalAction = animal => { /* process */ };
        Action<Dog> dogAction = animalAction; // Contravariance!
        dogAction.Should().NotBeNull();

        // Test 2: Func contravariance (in input) + covariance (in output)
        Func<Animal, Animal> animalFunc = animal => animal;
        Func<Dog, Animal> dogFunc = animalFunc; // Contravariance in input!
        dogFunc.Should().NotBeNull();

        // Test 3: IComparer contravariance
        IComparer<Animal> animalComparer = Comparer<Animal>.Create((x, y) =>
            x.Age.CompareTo(y.Age));
        IComparer<Dog> dogComparer = animalComparer; // Contravariance!
        dogComparer.Should().NotBeNull();
    }
}
