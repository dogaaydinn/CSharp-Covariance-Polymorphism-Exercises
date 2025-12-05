# QuickSort

## Problem
Implement quicksort algorithm to sort an array in O(n log n) average time.

## Examples
```csharp
QuickSort([3, 1, 4, 1, 5, 9, 2, 6]) => [1, 1, 2, 3, 4, 5, 6, 9]
QuickSort([5, 4, 3, 2, 1]) => [1, 2, 3, 4, 5]
```

## Algorithm
1. Choose pivot (usually last element)
2. Partition: Move elements < pivot to left, > pivot to right
3. Recursively sort left and right subarrays

## Time Complexity
- Average: O(n log n)
- Worst: O(n²) if always pick min/max as pivot
