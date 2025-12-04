# MergeSort Algorithm Exercise

## 📚 Learning Objectives
- **MergeSort**: Guaranteed O(n log n) divide-and-conquer algorithm
- **Stable Sorting**: Maintains relative order of equal elements
- **Recursion vs Iteration**: Top-down vs bottom-up approach
- **Counting Inversions**: Classic problem using modified MergeSort
- **Linked List Sorting**: Why MergeSort is ideal for linked lists

## 🎯 Exercise Tasks

Complete **5 TODO methods**:
1. ✅ **Merge()** - Merge two sorted arrays
2. ✅ **MergeSortRecursive()** - Top-down approach
3. ✅ **MergeSortIterative()** - Bottom-up approach
4. ✅ **CountInversions()** - Modified MergeSort
5. ✅ **MergeSortLinkedList()** - Sort linked list

## 🚀 Getting Started

```bash
cd samples/99-Exercises/Algorithms/03-MergeSort
dotnet test  # Should see ~25 FAILED tests
```

## 💡 Quick Hints

### TODO 1: Merge
```csharp
public static int[] Merge(int[] left, int[] right)
{
    int[] result = new int[left.Length + right.Length];
    int i = 0, j = 0, k = 0;

    while (i < left.Length && j < right.Length)
    {
        if (left[i] <= right[j])
            result[k++] = left[i++];
        else
            result[k++] = right[j++];
    }

    while (i < left.Length) result[k++] = left[i++];
    while (j < right.Length) result[k++] = right[j++];

    return result;
}
```

### TODO 2: MergeSortRecursive
```csharp
public static int[] MergeSortRecursive(int[] array)
{
    if (array.Length <= 1) return array;

    int mid = array.Length / 2;
    int[] left = array[..mid];
    int[] right = array[mid..];

    left = MergeSortRecursive(left);
    right = MergeSortRecursive(right);

    return Merge(left, right);
}
```

### TODO 3: MergeSortIterative
```csharp
public static void MergeSortIterative(int[] array)
{
    int n = array.Length;

    for (int size = 1; size < n; size *= 2)
    {
        for (int start = 0; start < n - size; start += 2 * size)
        {
            int mid = start + size - 1;
            int end = Math.Min(start + 2 * size - 1, n - 1);

            // Merge subarrays [start..mid] and [mid+1..end]
            int[] left = array[start..(mid + 1)];
            int[] right = array[(mid + 1)..(end + 1)];
            int[] merged = Merge(left, right);

            Array.Copy(merged, 0, array, start, merged.Length);
        }
    }
}
```

### TODO 4: CountInversions
```csharp
public static long CountInversions(int[] array)
{
    int[] temp = new int[array.Length];
    return CountInversionsHelper(array, temp, 0, array.Length - 1);
}

private static long CountInversionsHelper(int[] array, int[] temp, int left, int right)
{
    if (left >= right) return 0;

    int mid = left + (right - left) / 2;
    long count = 0;

    count += CountInversionsHelper(array, temp, left, mid);
    count += CountInversionsHelper(array, temp, mid + 1, right);
    count += MergeAndCount(array, temp, left, mid, right);

    return count;
}

private static long MergeAndCount(int[] array, int[] temp, int left, int mid, int right)
{
    int i = left, j = mid + 1, k = left;
    long invCount = 0;

    while (i <= mid && j <= right)
    {
        if (array[i] <= array[j])
            temp[k++] = array[i++];
        else
        {
            temp[k++] = array[j++];
            invCount += (mid - i + 1);  // All remaining in left array are inversions
        }
    }

    while (i <= mid) temp[k++] = array[i++];
    while (j <= right) temp[k++] = array[j++];

    for (i = left; i <= right; i++)
        array[i] = temp[i];

    return invCount;
}
```

### TODO 5: MergeSortLinkedList
```csharp
public static LinkedListNode? MergeSortLinkedList(LinkedListNode? head)
{
    if (head == null || head.Next == null) return head;

    // Find middle using slow/fast pointers
    LinkedListNode slow = head, fast = head, prev = head;
    while (fast != null && fast.Next != null)
    {
        prev = slow;
        slow = slow.Next;
        fast = fast.Next.Next;
    }

    prev.Next = null;  // Split list

    var left = MergeSortLinkedList(head);
    var right = MergeSortLinkedList(slow);

    return MergeSortedLists(left, right);
}

private static LinkedListNode? MergeSortedLists(LinkedListNode? left, LinkedListNode? right)
{
    if (left == null) return right;
    if (right == null) return left;

    LinkedListNode? result;
    if (left.Value <= right.Value)
    {
        result = left;
        result.Next = MergeSortedLists(left.Next, right);
    }
    else
    {
        result = right;
        result.Next = MergeSortedLists(left, right.Next);
    }

    return result;
}
```

## 📊 Complexity Analysis

| Operation | Time | Space | Notes |
|-----------|------|-------|-------|
| MergeSort | O(n log n) | O(n) | **Guaranteed** (no worst case!) |
| CountInversions | O(n log n) | O(n) | Better than O(n²) brute force |
| LinkedList Sort | O(n log n) | O(log n) | O(1) for merging, O(log n) stack |

**Key advantages:**
- ✅ **Stable**: Equal elements keep relative order
- ✅ **Predictable**: Always O(n log n), no worst case
- ✅ **Great for linked lists**: No random access needed

## 🎓 Interview Tips

**MergeSort vs QuickSort:**
- MergeSort: Stable, guaranteed O(n log n), more space
- QuickSort: Faster in practice, in-place, unstable

**When to use MergeSort:**
1. Need **stability** (equal elements maintain order)
2. Sorting **linked lists** (QuickSort needs random access)
3. **External sorting** (sorting large files on disk)
4. **Predictable performance** required (no O(n²) worst case)

**Classic interview problems:**
- Count inversions (shows array "sortedness")
- Merge k sorted lists
- Sort linked list

## 📚 Resources
- [Visualgo MergeSort](https://visualgo.net/en/sorting)
- [LeetCode #148 - Sort List](https://leetcode.com/problems/sort-list/)

---

**Good luck! 🎉** Check `SOLUTION.md` after trying.
