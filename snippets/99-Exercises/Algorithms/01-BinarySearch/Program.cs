namespace BinarySearch;

public class Program
{
    public static void Main(string[] args)
    {
        Console.WriteLine("Binary Search Algorithm Exercise");
        Console.WriteLine("Run 'dotnet test' to check your solutions\n");

        int[] sortedArray = { 1, 3, 5, 7, 9, 11, 13, 15, 17, 19, 21, 23, 25 };
        int target = 13;

        Console.WriteLine($"Array: [{string.Join(", ", sortedArray)}]");
        Console.WriteLine($"Target: {target}");

        // Uncomment these when you implement the methods
        /*
        Console.WriteLine("\n=== Binary Search (Iterative) ===");
        int resultIterative = BinarySearchIterative(sortedArray, target);
        Console.WriteLine($"Index: {resultIterative}");

        Console.WriteLine("\n=== Binary Search (Recursive) ===");
        int resultRecursive = BinarySearchRecursive(sortedArray, target);
        Console.WriteLine($"Index: {resultRecursive}");

        int[] duplicates = { 1, 2, 2, 2, 3, 4, 5, 5, 5, 6, 7 };
        Console.WriteLine($"\nArray with duplicates: [{string.Join(", ", duplicates)}]");
        Console.WriteLine($"First occurrence of 5: {FindFirstOccurrence(duplicates, 5)}");
        Console.WriteLine($"Last occurrence of 5: {FindLastOccurrence(duplicates, 5)}");
        */
    }

    // TODO 1: Implement iterative binary search
    // HINT: Use two pointers (left, right) and iterate while left <= right
    // TIME COMPLEXITY: O(log n) - halves the search space each iteration
    // SPACE COMPLEXITY: O(1) - only uses constant extra space
    // RETURN: Index of target if found, -1 otherwise
    public static int BinarySearchIterative(int[] array, int target)
    {
        // TODO: Implement iterative binary search
        // Algorithm:
        // 1. Initialize left = 0, right = array.Length - 1
        // 2. While left <= right:
        //    a. Calculate mid = left + (right - left) / 2  (avoids overflow)
        //    b. If array[mid] == target, return mid
        //    c. If array[mid] < target, search right half: left = mid + 1
        //    d. If array[mid] > target, search left half: right = mid - 1
        // 3. Return -1 if not found
        throw new NotImplementedException();
    }

    // TODO 2: Implement recursive binary search
    // HINT: Use helper method with left and right parameters
    // TIME COMPLEXITY: O(log n) - same as iterative
    // SPACE COMPLEXITY: O(log n) - recursive call stack
    // RETURN: Index of target if found, -1 otherwise
    public static int BinarySearchRecursive(int[] array, int target)
    {
        // TODO: Implement recursive binary search
        // You'll need a helper method:
        // private static int BinarySearchRecursiveHelper(int[] array, int target, int left, int right)
        // Base case: if left > right, return -1
        // Recursive case: calculate mid, compare, recurse on half
        throw new NotImplementedException();
    }

    // TODO 3: Find the first (leftmost) occurrence of target in sorted array with duplicates
    // HINT: When found, continue searching left to find earlier occurrences
    // TIME COMPLEXITY: O(log n)
    // SPACE COMPLEXITY: O(1)
    // RETURN: Index of first occurrence, -1 if not found
    public static int FindFirstOccurrence(int[] array, int target)
    {
        // TODO: Modified binary search
        // When array[mid] == target, don't return immediately
        // Instead, store the index and search left half (right = mid - 1)
        // This finds the leftmost occurrence
        //
        // Example: [1, 2, 2, 2, 3] target=2
        // Regular binary search might return index 1 or 2
        // This should return 1 (first occurrence)
        throw new NotImplementedException();
    }

    // TODO 4: Find the last (rightmost) occurrence of target in sorted array with duplicates
    // HINT: When found, continue searching right to find later occurrences
    // TIME COMPLEXITY: O(log n)
    // SPACE COMPLEXITY: O(1)
    // RETURN: Index of last occurrence, -1 if not found
    public static int FindLastOccurrence(int[] array, int target)
    {
        // TODO: Modified binary search
        // When array[mid] == target, store the index and search right half (left = mid + 1)
        // This finds the rightmost occurrence
        //
        // Example: [1, 2, 2, 2, 3] target=2
        // This should return 3 (last occurrence)
        throw new NotImplementedException();
    }

    // TODO 5: Find the element closest to target (may not exist in array)
    // HINT: Track the closest element seen so far while searching
    // TIME COMPLEXITY: O(log n)
    // SPACE COMPLEXITY: O(1)
    // RETURN: Index of element closest to target
    public static int FindClosestElement(int[] array, int target)
    {
        // TODO: Modified binary search
        // Keep track of the closest element during search
        // After binary search completes, compare final left and right positions
        //
        // Example: [1, 3, 5, 7, 9] target=6
        // Should return index of 5 or 7 (both distance 1 from 6)
        // If tie, return smaller index
        //
        // Edge case: Empty array → throw ArgumentException
        // Edge case: Single element → return 0
        throw new NotImplementedException();
    }

    // Helper method for recursive binary search (you'll implement this)
    private static int BinarySearchRecursiveHelper(int[] array, int target, int left, int right)
    {
        // TODO: Implement recursive helper
        // Base case: if (left > right) return -1;
        // Calculate mid
        // If found, return mid
        // If array[mid] < target, recurse on right half
        // If array[mid] > target, recurse on left half
        throw new NotImplementedException();
    }
}
