using FluentAssertions;
using Xunit;

namespace AdvancedCsharpConcepts.Tests;

/// <summary>
/// Unit tests for boxing and unboxing operations
/// </summary>
public class BoxingUnboxingTests
{
    [Fact]
    public void Boxing_ConvertsValueTypeToReferenceType()
    {
        // Arrange
        var myInt = 123;

        // Act
        object myObject = myInt; // Boxing

        // Assert
        myObject.Should().NotBeNull();
        myObject.Should().BeOfType<int>();
        myObject.Should().Be(123);
    }

    [Fact]
    public void Unboxing_ConvertsReferenceTypeToValueType()
    {
        // Arrange
        var myInt = 456;
        object myObject = myInt; // Boxing

        // Act
        var unboxedInt = (int)myObject; // Unboxing

        // Assert
        unboxedInt.Should().Be(456);
        unboxedInt.Should().BeOfType<int>();
    }

    [Fact]
    public void Unboxing_ThrowsException_WhenTypeDoesNotMatch()
    {
        // Arrange
        object myObject = 123.456; // Boxing a double

        // Act
        Action act = () => { var myInt = (int)myObject; }; // Attempt to unbox as int

        // Assert
        act.Should().Throw<InvalidCastException>();
    }

    [Fact]
    public void Boxing_WithDouble_PreservesValue()
    {
        // Arrange
        var myDouble = 123.456;

        // Act
        object myObject = myDouble; // Boxing

        // Assert
        myObject.Should().BeOfType<double>();
        myObject.Should().Be(123.456);
    }

    [Fact]
    public void Unboxing_DoubleToInt_CausesDataLoss()
    {
        // Arrange
        var myDouble = 123.789;
        object myObject = myDouble; // Boxing

        // Act
        var unboxedInt = (int)(double)myObject; // Unboxing and conversion

        // Assert
        unboxedInt.Should().Be(123); // Decimal part is lost
    }

    [Theory]
    [InlineData(10)]
    [InlineData(100)]
    [InlineData(1000)]
    public void Boxing_PreservesValue_ForMultipleIntegers(int value)
    {
        // Act
        object boxed = value;
        var unboxed = (int)boxed;

        // Assert
        unboxed.Should().Be(value);
    }
}
