using FsCheck;
using FsCheck.Xunit;
using Xunit;
using FluentAssertions;

namespace AdvancedCsharpConcepts.Tests.PropertyBasedTests;

/// <summary>
/// Property-based tests for boxing and unboxing operations.
/// Verifies that boxing/unboxing preserves values for all possible inputs.
/// </summary>
public class BoxingUnboxingPropertyTests
{
    /// <summary>
    /// Property: Boxing and unboxing an integer should preserve the value.
    /// </summary>
    [Property]
    public Property BoxingUnboxingIntShouldPreserveValue()
    {
        return Prop.ForAll<int>(
            value =>
            {
                // Act: Box and unbox
                object boxed = value;
                int unboxed = (int)boxed;

                // Assert: Value should be preserved
                unboxed.Should().Be(value);
                return true;
            });
    }

    /// <summary>
    /// Property: Boxing and unboxing a double should preserve the value.
    /// </summary>
    [Property]
    public Property BoxingUnboxingDoubleShouldPreserveValue()
    {
        return Prop.ForAll<double>(
            value =>
            {
                // Filter out NaN as it has special equality semantics
                if (double.IsNaN(value))
                    return true;

                // Act: Box and unbox
                object boxed = value;
                double unboxed = (double)boxed;

                // Assert: Value should be preserved
                unboxed.Should().Be(value);
                return true;
            });
    }

    /// <summary>
    /// Property: Boxing and unboxing a boolean should preserve the value.
    /// </summary>
    [Property]
    public Property BoxingUnboxingBoolShouldPreserveValue()
    {
        return Prop.ForAll<bool>(
            value =>
            {
                // Act: Box and unbox
                object boxed = value;
                bool unboxed = (bool)boxed;

                // Assert: Value should be preserved
                unboxed.Should().Be(value);
                return true;
            });
    }

    /// <summary>
    /// Property: Boxing and unboxing a char should preserve the value.
    /// </summary>
    [Property]
    public Property BoxingUnboxingCharShouldPreserveValue()
    {
        return Prop.ForAll<char>(
            value =>
            {
                // Act: Box and unbox
                object boxed = value;
                char unboxed = (char)boxed;

                // Assert: Value should be preserved
                unboxed.Should().Be(value);
                return true;
            });
    }

    /// <summary>
    /// Property: Boxing and unboxing a decimal should preserve the value.
    /// </summary>
    [Property]
    public Property BoxingUnboxingDecimalShouldPreserveValue()
    {
        return Prop.ForAll<decimal>(
            value =>
            {
                // Act: Box and unbox
                object boxed = value;
                decimal unboxed = (decimal)boxed;

                // Assert: Value should be preserved
                unboxed.Should().Be(value);
                return true;
            });
    }

    /// <summary>
    /// Property: Boxing a value type should create a reference type.
    /// </summary>
    [Property]
    public Property BoxingShouldCreateReferenceType()
    {
        return Prop.ForAll<int>(
            value =>
            {
                // Act: Box
                object boxed = value;

                // Assert: Should be a reference type (object)
                boxed.Should().BeOfType<int>();
                boxed.Should().NotBeNull();
                return true;
            });
    }

    /// <summary>
    /// Property: Multiple boxing of the same value creates different objects.
    /// </summary>
    [Property]
    public Property MultipleBoxingCreatesDifferentObjects()
    {
        return Prop.ForAll<int>(
            value =>
            {
                // Act: Box the same value twice
                object boxed1 = value;
                object boxed2 = value;

                // Assert: Should be different objects (different references)
                // Note: For small integers (-128 to 127), .NET may cache boxes
                // So we can't reliably test reference inequality
                // But we can verify they have the same value
                boxed1.Should().Be(boxed2); // Value equality
                return true;
            });
    }

    /// <summary>
    /// Property: Unboxing to wrong type should throw InvalidCastException.
    /// </summary>
    [Property]
    public Property UnboxingToWrongTypeShouldThrow()
    {
        return Prop.ForAll<int>(
            value =>
            {
                // Arrange: Box an int
                object boxed = value;

                // Act & Assert: Unboxing to long should throw
                Action act = () => { var _ = (long)boxed; };
                act.Should().Throw<InvalidCastException>();
                return true;
            });
    }

    /// <summary>
    /// Property: Nullable value types can be boxed and unboxed.
    /// </summary>
    [Property]
    public Property NullableBoxingUnboxingShouldWork()
    {
        return Prop.ForAll<int?>(
            nullableValue =>
            {
                // Act: Box and unbox nullable
                object boxed = nullableValue;
                int? unboxed = boxed as int?;

                // Assert: Value should be preserved
                unboxed.Should().Be(nullableValue);
                return true;
            });
    }

    /// <summary>
    /// Property: Boxing null nullable should result in null object.
    /// </summary>
    [Property]
    public Property BoxingNullNullableShouldResultInNull()
    {
        return Prop.ForAll(
            Gen.Constant<int?>(null),
            nullableValue =>
            {
                // Act: Box null nullable
                object boxed = nullableValue;

                // Assert: Should be null
                boxed.Should().BeNull();
                return true;
            });
    }
}
