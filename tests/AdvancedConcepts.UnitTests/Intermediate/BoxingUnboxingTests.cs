using FluentAssertions;

namespace AdvancedCsharpConcepts.Tests.Intermediate;

/// <summary>
/// Tests for boxing and unboxing concepts.
/// </summary>
public class BoxingUnboxingTests
{
    [Fact]
    public void Boxing_IntToObject_ShouldSucceed()
    {
        // Arrange
        int value = 42;

        // Act
        object boxed = value; // Boxing

        // Assert
        boxed.Should().BeOfType<int>();
        boxed.Should().Be(42);
    }

    [Fact]
    public void Unboxing_ObjectToInt_ShouldSucceed()
    {
        // Arrange
        object boxed = 42;

        // Act
        int unboxed = (int)boxed; // Unboxing

        // Assert
        unboxed.Should().Be(42);
    }

    [Fact]
    public void Unboxing_WrongType_ShouldThrowInvalidCastException()
    {
        // Arrange
        object boxed = 42;

        // Act
        Action act = () => { long value = (long)boxed; };

        // Assert
        act.Should().Throw<InvalidCastException>();
    }

    [Fact]
    public void ArrayList_ShouldBoxValueTypes()
    {
        // Arrange
        var list = new System.Collections.ArrayList();

        // Act
        list.Add(1); // Boxing occurs
        list.Add(2);
        list.Add(3);

        // Assert
        list.Count.Should().Be(3);
        list[0].Should().BeOfType<int>();
    }

    [Fact]
    public void GenericList_ShouldAvoidBoxing()
    {
        // Arrange & Act
        var list = new List<int> { 1, 2, 3 };

        // Assert
        list.Should().HaveCount(3);
        list[0].Should().Be(1);
    }

    [Fact]
    public void Boxing_DifferentValueTypes_ShouldWork()
    {
        // Arrange & Act
        object intBoxed = 42;
        object doubleBoxed = 3.14;
        object boolBoxed = true;
        object charBoxed = 'A';

        // Assert
        intBoxed.Should().BeOfType<int>();
        doubleBoxed.Should().BeOfType<double>();
        boolBoxed.Should().BeOfType<bool>();
        charBoxed.Should().BeOfType<char>();
    }

    [Fact]
    public void Unboxing_ThenModifying_ShouldNotAffectOriginalBoxedValue()
    {
        // Arrange
        int original = 42;
        object boxed = original;

        // Act
        int unboxed = (int)boxed;
        unboxed = 100;

        // Assert
        boxed.Should().Be(42); // Original boxed value unchanged
        original.Should().Be(42); // Original value unchanged
    }

    [Theory]
    [InlineData(1)]
    [InlineData(100)]
    [InlineData(1000)]
    [InlineData(10000)]
    public void GenericList_Performance_ShouldBeBetterThanArrayList(int count)
    {
        // This is a conceptual test - actual benchmarking should use BenchmarkDotNet
        // Arrange
        var arrayList = new System.Collections.ArrayList(count);
        var genericList = new List<int>(count);

        // Act - ArrayList requires boxing
        for (int i = 0; i < count; i++)
        {
            arrayList.Add(i); // Boxing
        }

        // Act - Generic list avoids boxing
        for (int i = 0; i < count; i++)
        {
            genericList.Add(i); // No boxing
        }

        // Assert
        arrayList.Count.Should().Be(count);
        genericList.Should().HaveCount(count);
        genericList.Should().Equal(Enumerable.Range(0, count));
    }

    [Fact]
    public void NullableValueType_ShouldBoxToNull()
    {
        // Arrange
        int? nullableInt = null;

        // Act
        object boxed = nullableInt;

        // Assert
        boxed.Should().BeNull();
    }

    [Fact]
    public void NullableValueType_WithValue_ShouldBoxToValue()
    {
        // Arrange
        int? nullableInt = 42;

        // Act
        object boxed = nullableInt;

        // Assert
        boxed.Should().NotBeNull();
        boxed.Should().Be(42);
        boxed.Should().BeOfType<int>(); // Not Nullable<int>
    }
}
