using Xunit;
using FluentAssertions;

namespace AdvancedConcepts.UnitTests.Intermediate;

/// <summary>
/// Advanced tests for boxing and unboxing operations
/// </summary>
public class BoxingUnboxingAdvancedTests
{
    [Fact]
    public void Boxing_ValueTypeToObject_CreatesBoxedCopy()
    {
        // Arrange
        int value = 42;

        // Act
        object boxed = value;  // Boxing occurs

        // Assert
        boxed.Should().Be(42);
        boxed.Should().BeOfType<int>();
    }

    [Fact]
    public void Unboxing_ObjectToValueType_ExtractsValue()
    {
        // Arrange
        object boxed = 42;

        // Act
        int unboxed = (int)boxed;  // Unboxing occurs

        // Assert
        unboxed.Should().Be(42);
    }

    [Fact]
    public void Unboxing_WrongType_ThrowsInvalidCastException()
    {
        // Arrange
        object boxed = 42;

        // Act
        Action act = () => { long wrong = (long)boxed; };

        // Assert
        act.Should().Throw<InvalidCastException>();
    }

    [Fact]
    public void Boxing_ModifyingOriginal_DoesNotAffectBoxed()
    {
        // Arrange
        int original = 10;
        object boxed = original;

        // Act
        original = 20;

        // Assert
        boxed.Should().Be(10);  // Boxed copy remains unchanged
        original.Should().Be(20);
    }

    [Fact]
    public void Boxing_NullableWithValue_BoxesTheValue()
    {
        // Arrange
        int? nullable = 42;

        // Act
        object boxed = nullable;  // Boxes the underlying value

        // Assert
        boxed.Should().NotBeNull();
        boxed.Should().Be(42);
    }

    [Fact]
    public void Boxing_NullableWithoutValue_BoxesAsNull()
    {
        // Arrange
        int? nullable = null;

        // Act
        object boxed = nullable;  // Boxes as null

        // Assert
        boxed.Should().BeNull();
    }

    [Fact]
    public void Boxing_InArrays_CreatesMultipleBoxes()
    {
        // Arrange
        int[] values = { 1, 2, 3 };

        // Act
        object[] boxed = new object[values.Length];
        for (int i = 0; i < values.Length; i++)
        {
            boxed[i] = values[i];  // Each assignment boxes
        }

        // Assert
        boxed.Should().HaveCount(3);
        boxed[0].Should().Be(1);
        boxed[1].Should().Be(2);
        boxed[2].Should().Be(3);
    }

    [Fact]
    public void Boxing_WithInterfaces_CausesBoxing()
    {
        // Arrange
        int value = 42;

        // Act
        IComparable comparable = value;  // Boxing occurs

        // Assert
        comparable.Should().NotBeNull();
        comparable.CompareTo(40).Should().BeGreaterThan(0);
    }

    [Fact]
    public void Unboxing_NullValue_ThrowsNullReferenceException()
    {
        // Arrange
        object? boxed = null;

        // Act
        Action act = () => { int value = (int)boxed!; };

        // Assert
        act.Should().Throw<NullReferenceException>();
    }

    [Fact]
    public void Boxing_Struct_BoxesEntireStruct()
    {
        // Arrange
        var point = new Point { X = 10, Y = 20 };

        // Act
        object boxed = point;  // Boxing

        // Assert
        boxed.Should().BeOfType<Point>();
        ((Point)boxed).X.Should().Be(10);
        ((Point)boxed).Y.Should().Be(20);
    }

    [Fact]
    public void Boxing_Enum_BoxesAsUnderlyingType()
    {
        // Arrange
        DayOfWeek day = DayOfWeek.Monday;

        // Act
        object boxed = day;  // Boxing

        // Assert
        boxed.Should().BeOfType<DayOfWeek>();
        boxed.Should().Be(DayOfWeek.Monday);
    }

    [Fact]
    public void Unboxing_ToNullable_Succeeds()
    {
        // Arrange
        object boxed = 42;

        // Act
        int? nullable = (int?)boxed;  // Unboxing to nullable

        // Assert
        nullable.Should().HaveValue();
        nullable.Value.Should().Be(42);
    }

    [Fact]
    public void Boxing_InCollections_GenericAvoidBoxing()
    {
        // Arrange & Act
        var genericList = new List<int> { 1, 2, 3 };  // No boxing
        var nonGenericList = new System.Collections.ArrayList { 1, 2, 3 };  // Boxing occurs

        // Assert
        genericList.Should().HaveCount(3);
        nonGenericList.Count.Should().Be(3);
    }

    [Fact]
    public void Boxing_WithStringFormatting_CausesBoxing()
    {
        // Arrange
        int value = 42;

        // Act
        string result = string.Format("Value: {0}", value);  // Boxing occurs

        // Assert
        result.Should().Be("Value: 42");
    }

    [Fact]
    public void Boxing_GetType_CausesBoxing()
    {
        // Arrange
        int value = 42;

        // Act
        Type type = value.GetType();  // Boxing occurs

        // Assert
        type.Should().Be(typeof(int));
    }

    // Helper struct
    private struct Point
    {
        public int X { get; set; }
        public int Y { get; set; }
    }
}
