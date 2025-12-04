using FluentAssertions;
using NUnit.Framework;

namespace QuickSort.Tests;

[TestFixture]
public class QuickSortTests
{
    // ========== TODO 1: PartitionLomuto Tests ==========
    [Test]
    public void PartitionLomuto_ShouldPlacePivotInCorrectPosition()
    {
        // Arrange
        int[] array = { 5, 2, 9, 1, 7, 6, 3 };
        int expectedPivot = 3;  // Value 3 should end up at correct position

        // Act
        int pivotIndex = Program.PartitionLomuto(array, 0, array.Length - 1);

        // Assert
        array[pivotIndex].Should().Be(expectedPivot, "pivot should be at correct position");

        // All elements left of pivot should be <= pivot
        for (int i = 0; i < pivotIndex; i++)
        {
            array[i].Should().BeLessOrEqualTo(expectedPivot);
        }

        // All elements right of pivot should be > pivot
        for (int i = pivotIndex + 1; i < array.Length; i++)
        {
            array[i].Should().BeGreaterThan(expectedPivot);
        }
    }

    [Test]
    public void PartitionLomuto_ShouldHandleSingleElement()
    {
        // Arrange
        int[] array = { 5 };

        // Act
        int pivotIndex = Program.PartitionLomuto(array, 0, 0);

        // Assert
        pivotIndex.Should().Be(0);
        array[0].Should().Be(5);
    }

    [Test]
    public void PartitionLomuto_ShouldHandleAlreadySorted()
    {
        // Arrange
        int[] array = { 1, 2, 3, 4, 5 };

        // Act
        int pivotIndex = Program.PartitionLomuto(array, 0, array.Length - 1);

        // Assert
        pivotIndex.Should().Be(4, "pivot (5) is already largest");
    }

    [Test]
    public void PartitionLomuto_ShouldHandleReverseSorted()
    {
        // Arrange
        int[] array = { 5, 4, 3, 2, 1 };

        // Act
        int pivotIndex = Program.PartitionLomuto(array, 0, array.Length - 1);

        // Assert
        pivotIndex.Should().Be(0, "pivot (1) should move to start");
    }

    // ========== TODO 2: QuickSort Tests ==========
    [Test]
    public void QuickSort_ShouldSortRandomArray()
    {
        // Arrange
        int[] array = { 64, 34, 25, 12, 22, 11, 90, 88, 45, 50 };
        int[] expected = { 11, 12, 22, 25, 34, 45, 50, 64, 88, 90 };

        // Act
        Program.QuickSort(array, 0, array.Length - 1);

        // Assert
        array.Should().Equal(expected);
    }

    [Test]
    public void QuickSort_ShouldHandleEmptyArray()
    {
        // Arrange
        int[] array = Array.Empty<int>();

        // Act
        Program.QuickSort(array, 0, -1);

        // Assert
        array.Should().BeEmpty();
    }

    [Test]
    public void QuickSort_ShouldHandleSingleElement()
    {
        // Arrange
        int[] array = { 42 };

        // Act
        Program.QuickSort(array, 0, 0);

        // Assert
        array.Should().Equal(42);
    }

    [Test]
    public void QuickSort_ShouldHandleTwoElements()
    {
        // Arrange
        int[] array = { 5, 3 };

        // Act
        Program.QuickSort(array, 0, 1);

        // Assert
        array.Should().Equal(3, 5);
    }

    [Test]
    public void QuickSort_ShouldHandleAllSameElements()
    {
        // Arrange
        int[] array = { 5, 5, 5, 5, 5 };

        // Act
        Program.QuickSort(array, 0, array.Length - 1);

        // Assert
        array.Should().Equal(5, 5, 5, 5, 5);
    }

    [Test]
    public void QuickSort_ShouldHandleAlreadySorted()
    {
        // Arrange
        int[] array = { 1, 2, 3, 4, 5 };

        // Act
        Program.QuickSort(array, 0, array.Length - 1);

        // Assert
        array.Should().Equal(1, 2, 3, 4, 5);
    }

    [Test]
    public void QuickSort_ShouldHandleNegativeNumbers()
    {
        // Arrange
        int[] array = { -5, 3, -1, 0, 8, -3 };

        // Act
        Program.QuickSort(array, 0, array.Length - 1);

        // Assert
        array.Should().Equal(-5, -3, -1, 0, 3, 8);
    }

    // ========== TODO 3: QuickSortIterative Tests ==========
    [Test]
    public void QuickSortIterative_ShouldSortRandomArray()
    {
        // Arrange
        int[] array = { 64, 34, 25, 12, 22, 11, 90 };
        int[] expected = { 11, 12, 22, 25, 34, 64, 90 };

        // Act
        Program.QuickSortIterative(array);

        // Assert
        array.Should().Equal(expected);
    }

    [Test]
    public void QuickSortIterative_ShouldHandleEmptyArray()
    {
        // Arrange
        int[] array = Array.Empty<int>();

        // Act
        Program.QuickSortIterative(array);

        // Assert
        array.Should().BeEmpty();
    }

    [Test]
    public void QuickSortIterative_ShouldHandleSingleElement()
    {
        // Arrange
        int[] array = { 42 };

        // Act
        Program.QuickSortIterative(array);

        // Assert
        array.Should().Equal(42);
    }

    [Test]
    public void QuickSortIterative_ShouldMatchRecursiveResult()
    {
        // Arrange
        int[] array1 = { 5, 2, 9, 1, 7, 6, 3 };
        int[] array2 = (int[])array1.Clone();

        // Act
        Program.QuickSort(array1, 0, array1.Length - 1);
        Program.QuickSortIterative(array2);

        // Assert
        array1.Should().Equal(array2, "both implementations should produce same result");
    }

    // ========== TODO 4: FindKthLargest Tests ==========
    [Test]
    public void FindKthLargest_ShouldFindLargestElement()
    {
        // Arrange
        int[] array = { 3, 2, 1, 5, 6, 4 };

        // Act
        int result = Program.FindKthLargest(array, 1);

        // Assert
        result.Should().Be(6, "largest element is 6");
    }

    [Test]
    public void FindKthLargest_ShouldFind2ndLargest()
    {
        // Arrange
        int[] array = { 3, 2, 1, 5, 6, 4 };

        // Act
        int result = Program.FindKthLargest(array, 2);

        // Assert
        result.Should().Be(5, "2nd largest element is 5");
    }

    [Test]
    public void FindKthLargest_ShouldFindSmallestWhenKEqualsLength()
    {
        // Arrange
        int[] array = { 3, 2, 1, 5, 6, 4 };

        // Act
        int result = Program.FindKthLargest(array, 6);

        // Assert
        result.Should().Be(1, "6th largest (smallest) element is 1");
    }

    [Test]
    public void FindKthLargest_ShouldHandleDuplicates()
    {
        // Arrange
        int[] array = { 3, 2, 3, 1, 2, 4, 5, 5, 6 };

        // Act
        int result = Program.FindKthLargest(array, 4);

        // Assert
        result.Should().Be(4, "4th largest is 4");
    }

    [Test]
    public void FindKthLargest_ShouldHandleSingleElement()
    {
        // Arrange
        int[] array = { 42 };

        // Act
        int result = Program.FindKthLargest(array, 1);

        // Assert
        result.Should().Be(42);
    }

    // ========== TODO 5: SortColors Tests ==========
    [Test]
    public void SortColors_ShouldSortRandomColors()
    {
        // Arrange
        int[] array = { 2, 0, 2, 1, 1, 0 };
        int[] expected = { 0, 0, 1, 1, 2, 2 };

        // Act
        Program.SortColors(array);

        // Assert
        array.Should().Equal(expected);
    }

    [Test]
    public void SortColors_ShouldHandleAllZeros()
    {
        // Arrange
        int[] array = { 0, 0, 0, 0 };

        // Act
        Program.SortColors(array);

        // Assert
        array.Should().Equal(0, 0, 0, 0);
    }

    [Test]
    public void SortColors_ShouldHandleAllOnes()
    {
        // Arrange
        int[] array = { 1, 1, 1, 1 };

        // Act
        Program.SortColors(array);

        // Assert
        array.Should().Equal(1, 1, 1, 1);
    }

    [Test]
    public void SortColors_ShouldHandleAllTwos()
    {
        // Arrange
        int[] array = { 2, 2, 2, 2 };

        // Act
        Program.SortColors(array);

        // Assert
        array.Should().Equal(2, 2, 2, 2);
    }

    [Test]
    public void SortColors_ShouldHandleAlreadySorted()
    {
        // Arrange
        int[] array = { 0, 0, 1, 1, 2, 2 };

        // Act
        Program.SortColors(array);

        // Assert
        array.Should().Equal(0, 0, 1, 1, 2, 2);
    }

    [Test]
    public void SortColors_ShouldHandleReverseSorted()
    {
        // Arrange
        int[] array = { 2, 2, 1, 1, 0, 0 };

        // Act
        Program.SortColors(array);

        // Assert
        array.Should().Equal(0, 0, 1, 1, 2, 2);
    }

    [Test]
    public void SortColors_ShouldHandleSingleElement()
    {
        // Arrange
        int[] array = { 1 };

        // Act
        Program.SortColors(array);

        // Assert
        array.Should().Equal(1);
    }

    [Test]
    public void SortColors_ShouldHandleEmptyArray()
    {
        // Arrange
        int[] array = Array.Empty<int>();

        // Act
        Program.SortColors(array);

        // Assert
        array.Should().BeEmpty();
    }

    // ========== Integration Tests ==========
    [Test]
    public void QuickSort_ShouldSortLargeArray()
    {
        // Arrange
        var random = new Random(42);  // Fixed seed for reproducibility
        int[] array = Enumerable.Range(0, 1000).Select(_ => random.Next(0, 1000)).ToArray();
        int[] expected = (int[])array.Clone();
        Array.Sort(expected);

        // Act
        Program.QuickSort(array, 0, array.Length - 1);

        // Assert
        array.Should().Equal(expected);
    }

    [Test]
    public void RecursiveAndIterative_ShouldProduceSameResult()
    {
        // Arrange
        int[] array1 = { 64, 34, 25, 12, 22, 11, 90, 88, 45, 50, 77, 33, 55 };
        int[] array2 = (int[])array1.Clone();

        // Act
        Program.QuickSort(array1, 0, array1.Length - 1);
        Program.QuickSortIterative(array2);

        // Assert
        array1.Should().Equal(array2);
    }
}
