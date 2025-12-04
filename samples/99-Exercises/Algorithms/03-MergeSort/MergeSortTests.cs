using FluentAssertions;
using NUnit.Framework;

namespace MergeSort.Tests;

[TestFixture]
public class MergeSortTests
{
    // ========== TODO 1: Merge Tests ==========
    [Test]
    public void Merge_ShouldMergeTwoSortedArrays()
    {
        // Arrange
        int[] left = { 1, 3, 5 };
        int[] right = { 2, 4, 6 };

        // Act
        int[] result = Program.Merge(left, right);

        // Assert
        result.Should().Equal(1, 2, 3, 4, 5, 6);
    }

    [Test]
    public void Merge_ShouldHandleEmptyLeftArray()
    {
        // Arrange
        int[] left = Array.Empty<int>();
        int[] right = { 1, 2, 3 };

        // Act
        int[] result = Program.Merge(left, right);

        // Assert
        result.Should().Equal(1, 2, 3);
    }

    [Test]
    public void Merge_ShouldHandleEmptyRightArray()
    {
        // Arrange
        int[] left = { 1, 2, 3 };
        int[] right = Array.Empty<int>();

        // Act
        int[] result = Program.Merge(left, right);

        // Assert
        result.Should().Equal(1, 2, 3);
    }

    [Test]
    public void Merge_ShouldHandleDifferentSizedArrays()
    {
        // Arrange
        int[] left = { 1, 5, 9 };
        int[] right = { 2, 3, 4, 6, 7, 8 };

        // Act
        int[] result = Program.Merge(left, right);

        // Assert
        result.Should().Equal(1, 2, 3, 4, 5, 6, 7, 8, 9);
    }

    // ========== TODO 2: MergeSortRecursive Tests ==========
    [Test]
    public void MergeSortRecursive_ShouldSortRandomArray()
    {
        // Arrange
        int[] array = { 64, 34, 25, 12, 22, 11, 90, 88 };

        // Act
        int[] result = Program.MergeSortRecursive(array);

        // Assert
        result.Should().Equal(11, 12, 22, 25, 34, 64, 88, 90);
    }

    [Test]
    public void MergeSortRecursive_ShouldHandleEmptyArray()
    {
        // Arrange
        int[] array = Array.Empty<int>();

        // Act
        int[] result = Program.MergeSortRecursive(array);

        // Assert
        result.Should().BeEmpty();
    }

    [Test]
    public void MergeSortRecursive_ShouldHandleSingleElement()
    {
        // Arrange
        int[] array = { 42 };

        // Act
        int[] result = Program.MergeSortRecursive(array);

        // Assert
        result.Should().Equal(42);
    }

    [Test]
    public void MergeSortRecursive_ShouldHandleAlreadySorted()
    {
        // Arrange
        int[] array = { 1, 2, 3, 4, 5 };

        // Act
        int[] result = Program.MergeSortRecursive(array);

        // Assert
        result.Should().Equal(1, 2, 3, 4, 5);
    }

    [Test]
    public void MergeSortRecursive_ShouldBeStable()
    {
        // Arrange - array with duplicates
        int[] array = { 5, 2, 3, 2, 1 };

        // Act
        int[] result = Program.MergeSortRecursive(array);

        // Assert
        result.Should().Equal(1, 2, 2, 3, 5);
    }

    // ========== TODO 3: MergeSortIterative Tests ==========
    [Test]
    public void MergeSortIterative_ShouldSortRandomArray()
    {
        // Arrange
        int[] array = { 64, 34, 25, 12, 22, 11, 90, 88 };

        // Act
        Program.MergeSortIterative(array);

        // Assert
        array.Should().Equal(11, 12, 22, 25, 34, 64, 88, 90);
    }

    [Test]
    public void MergeSortIterative_ShouldHandleEmptyArray()
    {
        // Arrange
        int[] array = Array.Empty<int>();

        // Act
        Program.MergeSortIterative(array);

        // Assert
        array.Should().BeEmpty();
    }

    [Test]
    public void MergeSortIterative_ShouldHandlePowerOfTwo()
    {
        // Arrange
        int[] array = { 8, 7, 6, 5, 4, 3, 2, 1 };

        // Act
        Program.MergeSortIterative(array);

        // Assert
        array.Should().Equal(1, 2, 3, 4, 5, 6, 7, 8);
    }

    [Test]
    public void MergeSortIterative_ShouldMatchRecursiveResult()
    {
        // Arrange
        int[] array1 = { 5, 2, 9, 1, 7, 6, 3 };
        int[] array2 = (int[])array1.Clone();

        // Act
        var recursive = Program.MergeSortRecursive(array1);
        Program.MergeSortIterative(array2);

        // Assert
        array2.Should().Equal(recursive);
    }

    // ========== TODO 4: CountInversions Tests ==========
    [Test]
    public void CountInversions_ShouldCountZeroForSortedArray()
    {
        // Arrange
        int[] array = { 1, 2, 3, 4, 5 };

        // Act
        long count = Program.CountInversions(array);

        // Assert
        count.Should().Be(0, "sorted array has no inversions");
    }

    [Test]
    public void CountInversions_ShouldCountCorrectly()
    {
        // Arrange
        int[] array = { 5, 3, 2, 4, 1 };

        // Act
        long count = Program.CountInversions(array);

        // Assert
        // Inversions: (5,3), (5,2), (5,4), (5,1), (3,2), (3,1), (2,1), (4,1)
        count.Should().Be(8);
    }

    [Test]
    public void CountInversions_ShouldCountMaxForReverseSorted()
    {
        // Arrange
        int[] array = { 5, 4, 3, 2, 1 };

        // Act
        long count = Program.CountInversions(array);

        // Assert
        // Max inversions = n(n-1)/2 = 5*4/2 = 10
        count.Should().Be(10);
    }

    [Test]
    public void CountInversions_ShouldHandleDuplicates()
    {
        // Arrange
        int[] array = { 2, 1, 2, 1 };

        // Act
        long count = Program.CountInversions(array);

        // Assert
        // Inversions: (2,1), (2,1), (2,1)
        count.Should().Be(3);
    }

    // ========== TODO 5: MergeSortLinkedList Tests ==========
    [Test]
    public void MergeSortLinkedList_ShouldSortList()
    {
        // Arrange
        var head = CreateLinkedList(new[] { 4, 2, 1, 3 });

        // Act
        var sorted = Program.MergeSortLinkedList(head);

        // Assert
        LinkedListToArray(sorted).Should().Equal(1, 2, 3, 4);
    }

    [Test]
    public void MergeSortLinkedList_ShouldHandleEmptyList()
    {
        // Act
        var sorted = Program.MergeSortLinkedList(null);

        // Assert
        sorted.Should().BeNull();
    }

    [Test]
    public void MergeSortLinkedList_ShouldHandleSingleNode()
    {
        // Arrange
        var head = new LinkedListNode(42);

        // Act
        var sorted = Program.MergeSortLinkedList(head);

        // Assert
        sorted.Should().NotBeNull();
        sorted!.Value.Should().Be(42);
        sorted.Next.Should().BeNull();
    }

    [Test]
    public void MergeSortLinkedList_ShouldHandleAlreadySorted()
    {
        // Arrange
        var head = CreateLinkedList(new[] { 1, 2, 3, 4, 5 });

        // Act
        var sorted = Program.MergeSortLinkedList(head);

        // Assert
        LinkedListToArray(sorted).Should().Equal(1, 2, 3, 4, 5);
    }

    // ========== Integration Tests ==========
    [Test]
    public void AllSorts_ShouldProduceSameResult()
    {
        // Arrange
        int[] original = { 64, 34, 25, 12, 22, 11, 90, 88, 45 };
        int[] array1 = (int[])original.Clone();
        int[] array2 = (int[])original.Clone();

        // Act
        var recursive = Program.MergeSortRecursive(array1);
        Program.MergeSortIterative(array2);

        // Assert
        recursive.Should().Equal(array2);
        recursive.Should().Equal(11, 12, 22, 25, 34, 45, 64, 88, 90);
    }

    // Helper methods
    private LinkedListNode? CreateLinkedList(int[] values)
    {
        if (values.Length == 0) return null;

        var head = new LinkedListNode(values[0]);
        var current = head;

        for (int i = 1; i < values.Length; i++)
        {
            current.Next = new LinkedListNode(values[i]);
            current = current.Next;
        }

        return head;
    }

    private int[] LinkedListToArray(LinkedListNode? head)
    {
        var result = new List<int>();
        var current = head;

        while (current != null)
        {
            result.Add(current.Value);
            current = current.Next;
        }

        return result.ToArray();
    }
}
