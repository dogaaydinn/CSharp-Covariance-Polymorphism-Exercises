# Binary Search

## Problem
Implement binary search to find an element in a sorted array in O(log n) time.

## Examples
```csharp
BinarySearch([1, 3, 5, 7, 9], 5) => 2  // Index of 5
BinarySearch([1, 3, 5, 7, 9], 6) => -1 // Not found
```

## Starter Code
```csharp
public class BinarySearch
{
    public int Search(int[] arr, int target)
    {
        // TODO: Implement binary search
        return -1;
    }
}
```

## Solution Approach
1. Set left = 0, right = arr.Length - 1
2. While left <= right:
   - Calculate mid = (left + right) / 2
   - If arr[mid] == target, return mid
   - If arr[mid] < target, left = mid + 1
   - If arr[mid] > target, right = mid - 1
3. Return -1 if not found

## Time Complexity: O(log n)
## Space Complexity: O(1)
