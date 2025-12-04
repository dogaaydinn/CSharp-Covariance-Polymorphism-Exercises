using FluentAssertions;
using NUnit.Framework;

namespace BinarySearch.Tests;

[TestFixture]
public class BinarySearchTests
{
    // ========== TODO 1: BinarySearchIterative Tests ==========
    [Test]
    public void BinarySearchIterative_ShouldFindElementInMiddle()
    {
        // Arrange
        int[] array = { 1, 3, 5, 7, 9, 11, 13 };
        int target = 7;

        // Act
        int result = Program.BinarySearchIterative(array, target);

        // Assert
        result.Should().Be(3, "7 is at index 3");
    }

    [Test]
    public void BinarySearchIterative_ShouldFindFirstElement()
    {
        // Arrange
        int[] array = { 1, 3, 5, 7, 9 };
        int target = 1;

        // Act
        int result = Program.BinarySearchIterative(array, target);

        // Assert
        result.Should().Be(0, "1 is at index 0");
    }

    [Test]
    public void BinarySearchIterative_ShouldFindLastElement()
    {
        // Arrange
        int[] array = { 1, 3, 5, 7, 9 };
        int target = 9;

        // Act
        int result = Program.BinarySearchIterative(array, target);

        // Assert
        result.Should().Be(4, "9 is at index 4");
    }

    [Test]
    public void BinarySearchIterative_ShouldReturnMinusOneWhenNotFound()
    {
        // Arrange
        int[] array = { 1, 3, 5, 7, 9 };
        int target = 6;

        // Act
        int result = Program.BinarySearchIterative(array, target);

        // Assert
        result.Should().Be(-1, "6 is not in the array");
    }

    [Test]
    public void BinarySearchIterative_ShouldHandleSingleElement()
    {
        // Arrange
        int[] array = { 5 };

        // Act & Assert
        Program.BinarySearchIterative(array, 5).Should().Be(0);
        Program.BinarySearchIterative(array, 3).Should().Be(-1);
    }

    [Test]
    public void BinarySearchIterative_ShouldHandleEmptyArray()
    {
        // Arrange
        int[] array = Array.Empty<int>();
        int target = 5;

        // Act
        int result = Program.BinarySearchIterative(array, target);

        // Assert
        result.Should().Be(-1, "empty array contains no elements");
    }

    // ========== TODO 2: BinarySearchRecursive Tests ==========
    [Test]
    public void BinarySearchRecursive_ShouldFindElementInMiddle()
    {
        // Arrange
        int[] array = { 1, 3, 5, 7, 9, 11, 13 };
        int target = 7;

        // Act
        int result = Program.BinarySearchRecursive(array, target);

        // Assert
        result.Should().Be(3, "7 is at index 3");
    }

    [Test]
    public void BinarySearchRecursive_ShouldFindFirstElement()
    {
        // Arrange
        int[] array = { 2, 4, 6, 8, 10 };
        int target = 2;

        // Act
        int result = Program.BinarySearchRecursive(array, target);

        // Assert
        result.Should().Be(0);
    }

    [Test]
    public void BinarySearchRecursive_ShouldReturnMinusOneWhenNotFound()
    {
        // Arrange
        int[] array = { 1, 3, 5, 7, 9 };
        int target = 4;

        // Act
        int result = Program.BinarySearchRecursive(array, target);

        // Assert
        result.Should().Be(-1, "4 is not in the array");
    }

    [Test]
    public void BinarySearchRecursive_ShouldHandleEmptyArray()
    {
        // Arrange
        int[] array = Array.Empty<int>();
        int target = 5;

        // Act
        int result = Program.BinarySearchRecursive(array, target);

        // Assert
        result.Should().Be(-1);
    }

    // ========== TODO 3: FindFirstOccurrence Tests ==========
    [Test]
    public void FindFirstOccurrence_ShouldFindLeftmostInDuplicates()
    {
        // Arrange
        int[] array = { 1, 2, 2, 2, 3, 4, 5 };
        int target = 2;

        // Act
        int result = Program.FindFirstOccurrence(array, target);

        // Assert
        result.Should().Be(1, "first occurrence of 2 is at index 1");
    }

    [Test]
    public void FindFirstOccurrence_ShouldHandleAllSameElements()
    {
        // Arrange
        int[] array = { 5, 5, 5, 5, 5 };
        int target = 5;

        // Act
        int result = Program.FindFirstOccurrence(array, target);

        // Assert
        result.Should().Be(0, "first occurrence is at index 0");
    }

    [Test]
    public void FindFirstOccurrence_ShouldHandleSingleOccurrence()
    {
        // Arrange
        int[] array = { 1, 3, 5, 7, 9 };
        int target = 5;

        // Act
        int result = Program.FindFirstOccurrence(array, target);

        // Assert
        result.Should().Be(2, "single occurrence at index 2");
    }

    [Test]
    public void FindFirstOccurrence_ShouldReturnMinusOneWhenNotFound()
    {
        // Arrange
        int[] array = { 1, 2, 2, 2, 3 };
        int target = 5;

        // Act
        int result = Program.FindFirstOccurrence(array, target);

        // Assert
        result.Should().Be(-1);
    }

    // ========== TODO 4: FindLastOccurrence Tests ==========
    [Test]
    public void FindLastOccurrence_ShouldFindRightmostInDuplicates()
    {
        // Arrange
        int[] array = { 1, 2, 2, 2, 3, 4, 5 };
        int target = 2;

        // Act
        int result = Program.FindLastOccurrence(array, target);

        // Assert
        result.Should().Be(3, "last occurrence of 2 is at index 3");
    }

    [Test]
    public void FindLastOccurrence_ShouldHandleAllSameElements()
    {
        // Arrange
        int[] array = { 5, 5, 5, 5, 5 };
        int target = 5;

        // Act
        int result = Program.FindLastOccurrence(array, target);

        // Assert
        result.Should().Be(4, "last occurrence is at index 4");
    }

    [Test]
    public void FindLastOccurrence_ShouldHandleLastElementDuplicated()
    {
        // Arrange
        int[] array = { 1, 3, 5, 7, 9, 9, 9 };
        int target = 9;

        // Act
        int result = Program.FindLastOccurrence(array, target);

        // Assert
        result.Should().Be(6, "last occurrence of 9 is at index 6");
    }

    [Test]
    public void FindLastOccurrence_ShouldReturnMinusOneWhenNotFound()
    {
        // Arrange
        int[] array = { 1, 2, 2, 2, 3 };
        int target = 5;

        // Act
        int result = Program.FindLastOccurrence(array, target);

        // Assert
        result.Should().Be(-1);
    }

    // ========== TODO 5: FindClosestElement Tests ==========
    [Test]
    public void FindClosestElement_ShouldFindExactMatch()
    {
        // Arrange
        int[] array = { 1, 3, 5, 7, 9 };
        int target = 5;

        // Act
        int result = Program.FindClosestElement(array, target);

        // Assert
        result.Should().Be(2, "exact match at index 2");
    }

    [Test]
    public void FindClosestElement_ShouldFindClosestWhenBetweenElements()
    {
        // Arrange
        int[] array = { 1, 3, 5, 7, 9 };
        int target = 6;

        // Act
        int result = Program.FindClosestElement(array, target);

        // Assert
        // Both 5 (index 2) and 7 (index 3) are distance 1 from 6
        // Should return the smaller index (2) or either is acceptable
        result.Should().BeOneOf(2, 3);
    }

    [Test]
    public void FindClosestElement_ShouldHandleTargetSmallerThanAll()
    {
        // Arrange
        int[] array = { 10, 20, 30, 40 };
        int target = 5;

        // Act
        int result = Program.FindClosestElement(array, target);

        // Assert
        result.Should().Be(0, "10 is closest to 5");
    }

    [Test]
    public void FindClosestElement_ShouldHandleTargetLargerThanAll()
    {
        // Arrange
        int[] array = { 10, 20, 30, 40 };
        int target = 50;

        // Act
        int result = Program.FindClosestElement(array, target);

        // Assert
        result.Should().Be(3, "40 is closest to 50");
    }

    [Test]
    public void FindClosestElement_ShouldHandleSingleElement()
    {
        // Arrange
        int[] array = { 10 };
        int target = 100;

        // Act
        int result = Program.FindClosestElement(array, target);

        // Assert
        result.Should().Be(0, "only one element available");
    }

    [Test]
    public void FindClosestElement_ShouldThrowOnEmptyArray()
    {
        // Arrange
        int[] array = Array.Empty<int>();
        int target = 5;

        // Act & Assert
        Assert.Throws<ArgumentException>(() => Program.FindClosestElement(array, target),
            "cannot find closest element in empty array");
    }

    // ========== Integration Tests ==========
    [Test]
    public void BothImplementations_ShouldReturnSameResults()
    {
        // Arrange
        int[] array = { 1, 5, 10, 15, 20, 25, 30, 35, 40 };
        int[] targets = { 1, 10, 40, 15, 7, 100 };

        // Act & Assert
        foreach (int target in targets)
        {
            int iterative = Program.BinarySearchIterative(array, target);
            int recursive = Program.BinarySearchRecursive(array, target);

            iterative.Should().Be(recursive,
                $"both implementations should return same result for target {target}");
        }
    }

    [Test]
    public void FirstAndLastOccurrence_ShouldFormValidRange()
    {
        // Arrange
        int[] array = { 1, 2, 2, 2, 2, 3, 4, 5, 5, 5, 6 };
        int target = 2;

        // Act
        int first = Program.FindFirstOccurrence(array, target);
        int last = Program.FindLastOccurrence(array, target);

        // Assert
        first.Should().BeLessThanOrEqualTo(last, "first occurrence should be before or equal to last");
        first.Should().Be(1);
        last.Should().Be(4);

        // Verify all elements in range are equal to target
        for (int i = first; i <= last; i++)
        {
            array[i].Should().Be(target);
        }
    }
}
