using FsCheck;
using FsCheck.Xunit;
using Xunit;
using AdvancedCsharpConcepts.Beginner.Override_Upcast_Downcast;
using FluentAssertions;

namespace AdvancedCsharpConcepts.Tests.PropertyBasedTests;

/// <summary>
/// Property-based tests for polymorphism and type conversion.
/// Uses FsCheck to generate random test data and verify properties hold for all inputs.
/// </summary>
public class PolymorphismPropertyTests
{
    /// <summary>
    /// Property: Upcasting should always succeed for derived types.
    /// For all derived objects, upcasting to base type should preserve the object.
    /// </summary>
    [Property]
    public Property UpcastingShouldAlwaysSucceed()
    {
        return Prop.ForAll<int>(
            speed =>
            {
                // Arrange: Create a Car (derived type)
                var car = new Car { Speed = Math.Abs(speed) };

                // Act: Upcast to Vehicle (base type)
                Vehicle vehicle = car;

                // Assert: Should succeed and preserve object
                vehicle.Should().NotBeNull();
                vehicle.Should().BeSameAs(car);
                return true;
            });
    }

    /// <summary>
    /// Property: Downcasting should only succeed for objects of the correct type.
    /// </summary>
    [Property]
    public Property DowncastingShouldSucceedForCorrectType()
    {
        return Prop.ForAll<int>(
            speed =>
            {
                // Arrange
                Vehicle vehicle = new Car { Speed = Math.Abs(speed) };

                // Act: Downcast to Car
                var car = vehicle as Car;

                // Assert: Should succeed
                car.Should().NotBeNull();
                car.Should().BeSameAs(vehicle);
                return true;
            });
    }

    /// <summary>
    /// Property: Downcasting should fail gracefully for incorrect types.
    /// </summary>
    [Property]
    public Property DowncastingShouldFailGracefullyForIncorrectType()
    {
        return Prop.ForAll<int>(
            speed =>
            {
                // Arrange: Create a Bike (not a Car)
                Vehicle vehicle = new Bike { Speed = Math.Abs(speed) };

                // Act: Try to downcast to Car (should fail)
                var car = vehicle as Car;

                // Assert: Should return null (safe downcast)
                car.Should().BeNull();
                return true;
            });
    }

    /// <summary>
    /// Property: Type checking with 'is' operator should be consistent.
    /// </summary>
    [Property]
    public Property TypeCheckingShouldBeConsistent()
    {
        return Prop.ForAll<int>(
            speed =>
            {
                // Arrange
                var car = new Car { Speed = Math.Abs(speed) };
                Vehicle vehicle = car;

                // Assert: Type checking should be consistent
                (vehicle is Car).Should().BeTrue();
                (vehicle is Bike).Should().BeFalse();
                (vehicle is Vehicle).Should().BeTrue();
                return true;
            });
    }

    /// <summary>
    /// Property: Speed should be preserved through type conversions.
    /// </summary>
    [Property]
    public Property SpeedShouldBePreservedThroughConversions()
    {
        return Prop.ForAll<int>(
            speed =>
            {
                var positiveSpeed = Math.Abs(speed);

                // Arrange
                var car = new Car { Speed = positiveSpeed };

                // Act: Upcast and downcast
                Vehicle vehicle = car;
                var downcastCar = vehicle as Car;

                // Assert: Speed should be preserved
                downcastCar.Should().NotBeNull();
                downcastCar!.Speed.Should().Be(positiveSpeed);
                return true;
            });
    }

    /// <summary>
    /// Property: Pattern matching should be equivalent to 'is' operator.
    /// </summary>
    [Property]
    public Property PatternMatchingShouldBeEquivalentToIsOperator()
    {
        return Prop.ForAll<int>(
            speed =>
            {
                // Arrange
                Vehicle vehicle = new Car { Speed = Math.Abs(speed) };

                // Act & Assert: Pattern matching should match 'is' operator
                var isOperatorResult = vehicle is Car;
                var patternMatchingResult = vehicle is Car c && c != null;

                isOperatorResult.Should().Be(patternMatchingResult);
                return true;
            });
    }
}
