# QuickSort Algorithm Exercise

## 📚 Learning Objectives
- **QuickSort**: Divide-and-conquer sorting algorithm
- **Partition Schemes**: Lomuto vs Hoare partitioning
- **Recursion vs Iteration**: Two implementation approaches
- **QuickSelect**: Find kth largest in O(n) average time
- **Dutch National Flag**: Three-way partitioning for special cases
- **Complexity Analysis**: Best/Average/Worst case scenarios

## 🎯 Exercise Tasks

Complete **5 TODO methods**:
1. ✅ **PartitionLomuto()** - Lomuto partition scheme
2. ✅ **QuickSort()** - Recursive sorting
3. ✅ **QuickSortIterative()** - Iterative with stack
4. ✅ **FindKthLargest()** - QuickSelect algorithm
5. ✅ **SortColors()** - Dutch National Flag problem

## 🚀 Getting Started

```bash
cd samples/99-Exercises/Algorithms/02-QuickSort
dotnet restore
dotnet test  # Should see 30 FAILED tests
# Complete TODOs in Program.cs
dotnet test  # Goal: 30 PASSED tests
```

## 💡 Quick Hints

### TODO 1: PartitionLomuto
```csharp
public static int PartitionLomuto(int[] array, int low, int high)
{
    int pivot = array[high];  // Choose last element
    int i = low - 1;          // Boundary for elements <= pivot

    for (int j = low; j < high; j++)
    {
        if (array[j] <= pivot)
        {
            i++;
            Swap(ref array[i], ref array[j]);
        }
    }

    Swap(ref array[i + 1], ref array[high]);  // Place pivot
    return i + 1;
}
```

### TODO 2: QuickSort (Recursive)
```csharp
public static void QuickSort(int[] array, int low, int high)
{
    if (low < high)
    {
        int pivotIndex = PartitionLomuto(array, low, high);
        QuickSort(array, low, pivotIndex - 1);   // Sort left
        QuickSort(array, pivotIndex + 1, high);  // Sort right
    }
}
```

### TODO 3: QuickSortIterative
```csharp
public static void QuickSortIterative(int[] array)
{
    if (array.Length <= 1) return;

    var stack = new Stack<(int low, int high)>();
    stack.Push((0, array.Length - 1));

    while (stack.Count > 0)
    {
        var (low, high) = stack.Pop();

        if (low < high)
        {
            int pivotIndex = PartitionLomuto(array, low, high);
            stack.Push((low, pivotIndex - 1));
            stack.Push((pivotIndex + 1, high));
        }
    }
}
```

### TODO 4: FindKthLargest
```csharp
public static int FindKthLargest(int[] array, int k)
{
    int targetIndex = array.Length - k;  // Convert kth largest to index
    return QuickSelectHelper(array, 0, array.Length - 1, targetIndex);
}

private static int QuickSelectHelper(int[] array, int low, int high, int targetIndex)
{
    int pivotIndex = PartitionLomuto(array, low, high);

    if (pivotIndex == targetIndex)
        return array[pivotIndex];
    else if (pivotIndex < targetIndex)
        return QuickSelectHelper(array, pivotIndex + 1, high, targetIndex);
    else
        return QuickSelectHelper(array, low, pivotIndex - 1, targetIndex);
}
```

### TODO 5: SortColors
```csharp
public static void SortColors(int[] array)
{
    int low = 0, mid = 0, high = array.Length - 1;

    while (mid <= high)
    {
        if (array[mid] == 0)
        {
            Swap(ref array[low], ref array[mid]);
            low++;
            mid++;
        }
        else if (array[mid] == 1)
        {
            mid++;
        }
        else  // array[mid] == 2
        {
            Swap(ref array[mid], ref array[high]);
            high--;
            // Don't increment mid - need to check swapped element
        }
    }
}
```

## 📊 Complexity Analysis

| Algorithm | Time (Best) | Time (Avg) | Time (Worst) | Space |
|-----------|-------------|------------|--------------|-------|
| QuickSort | O(n log n) | O(n log n) | O(n²) | O(log n) |
| QuickSelect | O(n) | O(n) | O(n²) | O(log n) |
| SortColors | O(n) | O(n) | O(n) | O(1) |

**Worst case** happens when pivot is always smallest/largest (already sorted array).
**Average case** with random pivots: O(n log n).

## 🎓 Interview Tips

**Common Questions:**
1. "What's the worst case for QuickSort?" → O(n²) when array is sorted
2. "How to avoid worst case?" → Randomized pivot selection
3. "QuickSort vs MergeSort?" → QuickSort faster in practice (cache locality), MergeSort stable
4. "Is QuickSort stable?" → No (can be modified but loses efficiency)
5. "Space complexity?" → O(log n) for recursion stack

**Dutch National Flag** is asked at Google, Facebook interviews!

## 📚 Resources
- [Visualgo QuickSort](https://visualgo.net/en/sorting)
- [LeetCode #215 - Kth Largest Element](https://leetcode.com/problems/kth-largest-element-in-an-array/)
- [LeetCode #75 - Sort Colors](https://leetcode.com/problems/sort-colors/)

---

**Good luck! 🎉** Check `SOLUTION.md` after trying yourself.
