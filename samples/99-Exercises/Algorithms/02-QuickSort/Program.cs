namespace QuickSort;

public class Program
{
    public static void Main(string[] args)
    {
        Console.WriteLine("QuickSort Algorithm Exercise");
        Console.WriteLine("Run 'dotnet test' to check your solutions\n");

        int[] array = { 64, 34, 25, 12, 22, 11, 90, 88, 45, 50 };
        Console.WriteLine($"Original array: [{string.Join(", ", array)}]");

        // Uncomment these when you implement the methods
        /*
        int[] sorted = (int[])array.Clone();
        QuickSort(sorted, 0, sorted.Length - 1);
        Console.WriteLine($"Sorted array:   [{string.Join(", ", sorted)}]");

        int[] kthTest = { 3, 2, 1, 5, 6, 4 };
        int k = 2;
        int kthLargest = FindKthLargest(kthTest, k);
        Console.WriteLine($"\n{k}nd largest element in [{string.Join(", ", kthTest)}]: {kthLargest}");
        */
    }

    // TODO 1: Implement Lomuto partition scheme
    // HINT: Choose last element as pivot, partition array around it
    // TIME COMPLEXITY: O(n) - single pass through array
    // SPACE COMPLEXITY: O(1) - in-place partitioning
    // RETURN: Final index of pivot after partitioning
    public static int PartitionLomuto(int[] array, int low, int high)
    {
        // TODO: Implement Lomuto partition
        // Algorithm:
        // 1. Choose pivot = array[high] (last element)
        // 2. Maintain boundary 'i' for elements <= pivot
        // 3. Scan array[low..high-1]:
        //    - If array[j] <= pivot, swap array[i] with array[j], increment i
        // 4. Swap array[i] with array[high] (place pivot in correct position)
        // 5. Return i (final pivot index)
        //
        // Example: [5, 2, 9, 1, 7, 6, 3] pivot=3 (last)
        // After partition: [2, 1, 3, 9, 7, 6, 5] return 2
        //                        ↑
        //                    pivot at correct position
        throw new NotImplementedException();
    }

    // TODO 2: Implement recursive QuickSort
    // HINT: Partition, then recursively sort left and right subarrays
    // TIME COMPLEXITY: O(n log n) average, O(n²) worst case
    // SPACE COMPLEXITY: O(log n) average due to recursion stack
    // RETURN: void (sorts in-place)
    public static void QuickSort(int[] array, int low, int high)
    {
        // TODO: Implement recursive QuickSort
        // Algorithm:
        // 1. Base case: if (low >= high) return (0 or 1 element)
        // 2. Partition array, get pivot index
        // 3. Recursively sort left subarray: QuickSort(array, low, pivotIndex - 1)
        // 4. Recursively sort right subarray: QuickSort(array, pivotIndex + 1, high)
        //
        // Note: This sorts the array in-place
        throw new NotImplementedException();
    }

    // TODO 3: Implement iterative QuickSort using a stack
    // HINT: Use explicit stack to simulate recursion
    // TIME COMPLEXITY: O(n log n) average, O(n²) worst case
    // SPACE COMPLEXITY: O(log n) for stack (same as recursive)
    // RETURN: void (sorts in-place)
    public static void QuickSortIterative(int[] array)
    {
        // TODO: Implement iterative QuickSort
        // Algorithm:
        // 1. Create a stack to store (low, high) pairs
        // 2. Push initial range (0, array.Length - 1)
        // 3. While stack is not empty:
        //    a. Pop (low, high)
        //    b. If low < high:
        //       - Partition and get pivot index
        //       - Push left subarray (low, pivot - 1)
        //       - Push right subarray (pivot + 1, high)
        //
        // Note: You can use Stack<(int, int)> in C#
        throw new NotImplementedException();
    }

    // TODO 4: Find kth largest element using QuickSelect algorithm
    // HINT: Modified QuickSort that only recurses on one side
    // TIME COMPLEXITY: O(n) average, O(n²) worst case
    // SPACE COMPLEXITY: O(log n) due to recursion
    // RETURN: kth largest element (k=1 means largest, k=2 means 2nd largest)
    public static int FindKthLargest(int[] array, int k)
    {
        // TODO: Implement QuickSelect algorithm
        // Algorithm (for kth largest):
        // 1. Convert to finding (n-k)th smallest (0-indexed)
        // 2. Use modified QuickSort:
        //    - Partition array
        //    - If pivot index == target index, found!
        //    - If pivot index < target index, search right
        //    - If pivot index > target index, search left
        //
        // Example: [3, 2, 1, 5, 6, 4] k=2
        // 2nd largest is 5 (array sorted: [1, 2, 3, 4, 5, 6])
        //
        // Note: This modifies the original array
        throw new NotImplementedException();
    }

    // TODO 5: Sort Colors (Dutch National Flag Problem)
    // HINT: Three-way partitioning with low, mid, high pointers
    // TIME COMPLEXITY: O(n) - single pass
    // SPACE COMPLEXITY: O(1) - in-place
    // RETURN: void (sorts in-place)
    public static void SortColors(int[] array)
    {
        // TODO: Implement Dutch National Flag algorithm
        // Problem: Sort array containing only 0, 1, and 2 in one pass
        //
        // Algorithm:
        // 1. Use three pointers:
        //    - low: boundary for 0s
        //    - mid: current element
        //    - high: boundary for 2s
        // 2. While mid <= high:
        //    - If array[mid] == 0: swap with array[low], increment both low and mid
        //    - If array[mid] == 1: just increment mid
        //    - If array[mid] == 2: swap with array[high], decrement high (don't increment mid!)
        //
        // Example: [2, 0, 2, 1, 1, 0]
        // After: [0, 0, 1, 1, 2, 2]
        //
        // This is a classic interview problem!
        throw new NotImplementedException();
    }

    // Helper method for QuickSelect
    private static int QuickSelectHelper(int[] array, int low, int high, int targetIndex)
    {
        // TODO: Implement QuickSelect helper (used by FindKthLargest)
        // Similar to QuickSort but only recurse on one side
        throw new NotImplementedException();
    }

    // Helper method to swap two elements
    private static void Swap(ref int a, ref int b)
    {
        int temp = a;
        a = b;
        b = temp;
    }
}
