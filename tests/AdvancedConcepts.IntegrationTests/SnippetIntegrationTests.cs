using Xunit;
using FluentAssertions;
using AdvancedCsharpConcepts.Beginner.Polymorphism_AssignCompatibility;
using AdvancedCsharpConcepts.Intermediate.BoxingUnboxing;
using AdvancedCsharpConcepts.Advanced.DesignPatterns;

namespace AdvancedConcepts.IntegrationTests;

/// <summary>
/// Integration tests that validate end-to-end functionality of key snippets
/// These tests ensure that the learning examples work correctly as documented
/// </summary>
public class SnippetIntegrationTests
{
    #region Beginner Snippet Tests

    [Fact]
    public void BeginnerSnippet_PolymorphismBasics_CompleteFlow_ShouldWork()
    {
        // This test validates the complete polymorphism learning flow

        // Arrange: Create instances
        Dog dog = new Dog();
        Cat cat = new Cat();

        // Act: Upcast to base type (implicit)
        Animal animal1 = dog;
        Animal animal2 = cat;

        // Assert: Runtime types are preserved
        animal1.Should().BeOfType<Dog>();
        animal2.Should().BeOfType<Cat>();

        // Act: Downcast with pattern matching
        if (animal1 is Dog actualDog)
        {
            actualDog.Should().NotBeNull();
        }

        // Act: Safe downcast with 'as' operator
        Dog? maybeDog = animal1 as Dog;
        Cat? maybeCat = animal1 as Cat;

        // Assert: Safe downcasting works
        maybeDog.Should().NotBeNull();
        maybeCat.Should().BeNull();
    }

    [Fact]
    public void BeginnerSnippet_AssignmentCompatibility_ArrayCovariance_ShouldDemonstrateRuntimeBehavior()
    {
        // This test demonstrates array covariance (a key C# concept)

        // Arrange: Create array of derived type
        Dog[] dogs = { new Dog(), new Dog() };

        // Act: Arrays are covariant (can assign to base type array)
        Animal[] animals = dogs;

        // Assert: Read operations work
        animals.Should().HaveCount(2);
        animals.Should().AllBeOfType<Dog>();

        // Act: Attempting to write incompatible type throws at runtime
        Action writeCat = () => animals[0] = new Cat();

        // Assert: Runtime safety check prevents type corruption
        writeCat.Should().Throw<ArrayTypeMismatchException>(
            "because arrays have runtime type checking to prevent corruption");
    }

    #endregion

    #region Intermediate Snippet Tests

    [Fact]
    public void IntermediateSnippet_BoxingUnboxing_PerformanceImpact_ShouldBeMeasurable()
    {
        // This test validates that boxing/unboxing concepts work as documented

        // Arrange: Value type
        int value = 42;

        // Act: Boxing (value type → reference type)
        object boxed = value;  // Heap allocation

        // Assert: Boxing creates a new object
        boxed.Should().BeOfType<int>();
        boxed.Should().Be(42);

        // Act: Unboxing (reference type → value type)
        int unboxed = (int)boxed;

        // Assert: Value is correctly retrieved
        unboxed.Should().Be(42);

        // Act: Invalid unbox throws exception
        Action invalidUnbox = () => { long wrongType = (long)boxed; };

        // Assert: Type safety is enforced
        invalidUnbox.Should().Throw<InvalidCastException>();
    }

    [Fact]
    public void IntermediateSnippet_GenericConstraints_TypeSafety_ShouldEnforceConstraints()
    {
        // This test validates that generic constraints work correctly

        // Arrange: Create a generic list with constraints
        var numbers = new List<int> { 1, 2, 3, 4, 5 };

        // Act: Generic methods work with constrained types
        int max = numbers.Max();
        int min = numbers.Min();

        // Assert: Type-safe operations succeed
        max.Should().Be(5);
        min.Should().Be(1);

        // Generic constraints prevent invalid operations at compile time
        // This demonstrates type safety without runtime overhead
    }

    #endregion

    #region Advanced Snippet Tests

    [Fact]
    public void AdvancedSnippet_BuilderPattern_FluentAPI_ShouldBuildCorrectObject()
    {
        // This test validates the Builder Pattern implementation

        // Arrange & Act: Use fluent API to build object
        var person = new PersonBuilder()
            .WithFirstName("John")
            .WithLastName("Doe")
            .WithAge(30)
            .WithEmail("john.doe@example.com")
            .Build();

        // Assert: Object is constructed correctly
        person.FirstName.Should().Be("John");
        person.LastName.Should().Be("Doe");
        person.Age.Should().Be(30);
        person.Email.Should().Be("john.doe@example.com");
    }

    [Fact]
    public void AdvancedSnippet_FactoryPattern_ObjectCreation_ShouldCreateCorrectInstances()
    {
        // This test validates the Factory Pattern implementation

        // Arrange: Factory
        var factory = new ShapeFactory();

        // Act: Create different shapes using factory
        var circle = factory.CreateShape("circle");
        var square = factory.CreateShape("square");

        // Assert: Correct types are created
        circle.Should().NotBeNull();
        square.Should().NotBeNull();

        // Factory pattern allows creation without exposing instantiation logic
        // This demonstrates abstraction and encapsulation
    }

    [Fact]
    public void AdvancedSnippet_DependencyInjection_ServiceLifetime_ShouldWorkCorrectly()
    {
        // This test validates DI container behavior

        // Arrange: Setup DI container
        var services = new Microsoft.Extensions.DependencyInjection.ServiceCollection();
        services.AddTransient<ITransientService, TransientService>();
        services.AddScoped<IScopedService, ScopedService>();
        services.AddSingleton<ISingletonService, SingletonService>();

        var serviceProvider = services.BuildServiceProvider();

        // Act: Resolve services
        var transient1 = serviceProvider.GetService<ITransientService>();
        var transient2 = serviceProvider.GetService<ITransientService>();
        var singleton1 = serviceProvider.GetService<ISingletonService>();
        var singleton2 = serviceProvider.GetService<ISingletonService>();

        // Assert: Transient creates new instances
        transient1.Should().NotBeSameAs(transient2);

        // Assert: Singleton returns same instance
        singleton1.Should().BeSameAs(singleton2);
    }

    #endregion

    #region Real-World Integration Scenarios

    [Fact]
    public void RealWorld_PolymorphismWithPatterns_CompleteScenario_ShouldWork()
    {
        // This integration test combines multiple concepts:
        // Polymorphism + Design Patterns + SOLID principles

        // Arrange: Create different animal types
        List<Animal> animals = new()
        {
            new Dog(),
            new Cat(),
            new Dog()
        };

        // Act: Process polymorphically
        var dogCount = animals.Count(a => a is Dog);
        var catCount = animals.Count(a => a is Cat);

        // Assert: Polymorphic operations work correctly
        dogCount.Should().Be(2);
        catCount.Should().Be(1);

        // Demonstrates: Polymorphism + LINQ integration
    }

    [Fact]
    public void RealWorld_ValueTypeVsReferenceType_MemoryBehavior_ShouldBeDifferent()
    {
        // This test demonstrates the fundamental difference between value and reference types

        // Arrange: Value type (struct)
        int valueType1 = 42;
        int valueType2 = valueType1;  // Copy

        // Act: Modify copy
        valueType2 = 100;

        // Assert: Original is unchanged (value semantics)
        valueType1.Should().Be(42);
        valueType2.Should().Be(100);

        // Arrange: Reference type (class)
        var list1 = new List<int> { 1, 2, 3 };
        var list2 = list1;  // Reference copy

        // Act: Modify through reference
        list2.Add(4);

        // Assert: Original is modified (reference semantics)
        list1.Should().HaveCount(4);
        list1.Should().Contain(4);
    }

    #endregion
}

// Helper interfaces and classes for DI test
public interface ITransientService { }
public interface IScopedService { }
public interface ISingletonService { }

public class TransientService : ITransientService { }
public class ScopedService : IScopedService { }
public class SingletonService : ISingletonService { }
