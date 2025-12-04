namespace MergeSort;

public class Program
{
    public static void Main(string[] args)
    {
        Console.WriteLine("MergeSort Algorithm Exercise");
        Console.WriteLine("Run 'dotnet test' to check your solutions\n");

        int[] array = { 64, 34, 25, 12, 22, 11, 90, 88 };
        Console.WriteLine($"Original: [{string.Join(", ", array)}]");

        // Uncomment when implemented
        /*
        int[] sorted = MergeSortRecursive((int[])array.Clone());
        Console.WriteLine($"Sorted:   [{string.Join(", ", sorted)}]");

        int[] inversions = { 5, 3, 2, 4, 1 };
        int count = CountInversions(inversions);
        Console.WriteLine($"\nInversions in [{string.Join(", ", inversions)}]: {count}");
        */
    }

    // TODO 1: Merge two sorted arrays into one sorted array
    // HINT: Use two pointers to compare and merge
    // TIME: O(n + m), SPACE: O(n + m)
    public static int[] Merge(int[] left, int[] right)
    {
        // TODO: Merge two sorted arrays
        // Algorithm:
        // 1. Create result array of size (left.Length + right.Length)
        // 2. Use two pointers i=0, j=0
        // 3. While both arrays have elements:
        //    - Compare left[i] vs right[j]
        //    - Add smaller to result, increment its pointer
        // 4. Copy remaining elements from whichever array has leftovers
        //
        // Example: Merge([1, 3, 5], [2, 4, 6])
        // Result: [1, 2, 3, 4, 5, 6]
        throw new NotImplementedException();
    }

    // TODO 2: Recursive MergeSort
    // HINT: Divide in half, sort recursively, merge
    // TIME: O(n log n), SPACE: O(n)
    public static int[] MergeSortRecursive(int[] array)
    {
        // TODO: Implement recursive MergeSort
        // Algorithm:
        // 1. Base case: if array.Length <= 1, return array (already sorted)
        // 2. Find middle: mid = array.Length / 2
        // 3. Divide: left = array[0..mid], right = array[mid..end]
        // 4. Conquer: recursively sort left and right
        // 5. Combine: merge sorted left and right
        //
        // Note: Returns new array (not in-place)
        throw new NotImplementedException();
    }

    // TODO 3: Iterative (Bottom-Up) MergeSort
    // HINT: Start with size 1, merge pairs, double size each round
    // TIME: O(n log n), SPACE: O(n)
    public static void MergeSortIterative(int[] array)
    {
        // TODO: Implement bottom-up MergeSort
        // Algorithm:
        // 1. Start with subarray size = 1
        // 2. For each pair of subarrays of current size:
        //    - Merge them
        // 3. Double the size
        // 4. Repeat until size >= array.Length
        //
        // Example progression (array of 8 elements):
        // Size 1: [3][1] [4][2] [7][5] [8][6] → [1,3] [2,4] [5,7] [6,8]
        // Size 2: [1,3][2,4] [5,7][6,8] → [1,2,3,4] [5,6,7,8]
        // Size 4: [1,2,3,4][5,6,7,8] → [1,2,3,4,5,6,7,8]
        throw new NotImplementedException();
    }

    // TODO 4: Count inversions in array
    // HINT: Modified MergeSort that counts swaps
    // TIME: O(n log n), SPACE: O(n)
    public static long CountInversions(int[] array)
    {
        // TODO: Count inversions using modified MergeSort
        // Inversion: pair (i, j) where i < j but array[i] > array[j]
        //
        // Example: [5, 3, 2, 4, 1]
        // Inversions: (5,3), (5,2), (5,4), (5,1), (3,2), (3,1), (2,1), (4,1)
        // Count: 8
        //
        // Algorithm: During merge, when taking from right array,
        // all remaining elements in left array form inversions
        throw new NotImplementedException();
    }

    // TODO 5: Sort linked list using MergeSort
    // HINT: Find middle with slow/fast pointers, merge recursively
    // TIME: O(n log n), SPACE: O(log n) for recursion
    public static LinkedListNode? MergeSortLinkedList(LinkedListNode? head)
    {
        // TODO: Sort linked list using MergeSort
        // Algorithm:
        // 1. Base case: if head == null or head.Next == null, return head
        // 2. Find middle using slow/fast pointers
        // 3. Split list into two halves
        // 4. Recursively sort both halves
        // 5. Merge sorted halves
        //
        // Why MergeSort for linked lists?
        // - No random access needed (unlike QuickSort)
        // - O(1) space for merging (just pointer manipulation)
        throw new NotImplementedException();
    }

    // Helper for CountInversions
    private static long CountInversionsHelper(int[] array, int[] temp, int left, int right)
    {
        // TODO: Recursive helper for counting inversions
        throw new NotImplementedException();
    }

    private static long MergeAndCount(int[] array, int[] temp, int left, int mid, int right)
    {
        // TODO: Merge and count inversions
        throw new NotImplementedException();
    }

    // Helper for linked list
    private static LinkedListNode? MergeSortedLists(LinkedListNode? left, LinkedListNode? right)
    {
        // TODO: Merge two sorted linked lists
        throw new NotImplementedException();
    }
}

// Linked list node definition
public class LinkedListNode
{
    public int Value { get; set; }
    public LinkedListNode? Next { get; set; }

    public LinkedListNode(int value)
    {
        Value = value;
    }
}
