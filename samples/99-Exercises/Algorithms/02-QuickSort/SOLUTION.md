# ⚠️ SPOILER WARNING ⚠️

**DO NOT READ UNTIL YOU'VE TRIED TO SOLVE IT YOURSELF!**

---

# QuickSort - Complete Solutions

## TODO 1: PartitionLomuto

```csharp
public static int PartitionLomuto(int[] array, int low, int high)
{
    int pivot = array[high];
    int i = low - 1;

    for (int j = low; j < high; j++)
    {
        if (array[j] <= pivot)
        {
            i++;
            Swap(ref array[i], ref array[j]);
        }
    }

    Swap(ref array[i + 1], ref array[high]);
    return i + 1;
}
```

**Time**: O(n), **Space**: O(1)

**How it works:**
- Pivot = last element
- `i` tracks boundary: elements[0..i] ≤ pivot
- Scan left to right, swap smaller elements to left
- Finally, place pivot in correct position

---

## TODO 2: QuickSort (Recursive)

```csharp
public static void QuickSort(int[] array, int low, int high)
{
    if (low < high)
    {
        int pivotIndex = PartitionLomuto(array, low, high);
        QuickSort(array, low, pivotIndex - 1);
        QuickSort(array, pivotIndex + 1, high);
    }
}
```

**Time**: O(n log n) average, O(n²) worst
**Space**: O(log n) stack depth

---

## TODO 3: QuickSortIterative

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

**Trade-offs vs Recursive:**
- ✅ No stack overflow risk
- ✅ More control over stack usage
- ❌ More verbose code

---

## TODO 4: FindKthLargest

```csharp
public static int FindKthLargest(int[] array, int k)
{
    int targetIndex = array.Length - k;
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

**Time**: O(n) average, O(n²) worst
**Key insight**: Only recurse on ONE side (not both like QuickSort)

**Example:**
```
[3, 2, 1, 5, 6, 4] k=2 (find 2nd largest)
Sorted: [1, 2, 3, 4, 5, 6]
        targetIndex = 6 - 2 = 4 (0-indexed)
        2nd largest = array[4] = 5 ✓
```

---

## TODO 5: SortColors

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
            // DON'T increment mid - need to check swapped element
        }
    }
}
```

**Time**: O(n) single pass
**Space**: O(1)

**Visualization:**
```
[2, 0, 2, 1, 1, 0]
 ↑     ↑        ↑
low   mid      high

Step 1: array[mid]=2, swap with array[high], high--
[2, 0, 0, 1, 1, 2]
 ↑     ↑     ↑
low   mid   high

Step 2: array[mid]=0, swap with array[low], low++, mid++
[0, 2, 0, 1, 1, 2]
    ↑     ↑  ↑
   low   mid high

... continues until mid > high
Final: [0, 0, 1, 1, 2, 2]
```

---

## Key Takeaways

1. **Partition is key** - O(n) partition enables O(n log n) sort
2. **QuickSelect genius** - Find kth largest in O(n) average!
3. **Dutch Flag** - Classic 3-way partition problem
4. **Worst case O(n²)** - Happens with sorted input, use random pivot to avoid
5. **Not stable** - Equal elements may swap order

---

**Interview Favorites:**
- ✅ LeetCode #75 (Sort Colors) - Dutch National Flag
- ✅ LeetCode #215 (Kth Largest) - QuickSelect
- ✅ "Implement QuickSort" - Classic question

**Congratulations! 🎉** Next: MergeSort (stable, guaranteed O(n log n))
