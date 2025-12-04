# ⚠️ SPOILER WARNING ⚠️

**DO NOT READ THIS FILE UNTIL YOU'VE TRIED TO SOLVE THE EXERCISES YOURSELF!**

This file contains complete solutions to all TODO exercises. Try to complete them on your own first!

---

# Binary Search - Complete Solutions

## TODO 1: BinarySearchIterative

```csharp
public static int BinarySearchIterative(int[] array, int target)
{
    int left = 0;
    int right = array.Length - 1;

    while (left <= right)
    {
        int mid = left + (right - left) / 2;

        if (array[mid] == target)
            return mid;

        if (array[mid] < target)
            left = mid + 1;  // Search right half
        else
            right = mid - 1;  // Search left half
    }

    return -1;  // Not found
}
```

**Explanation:**
- **Initialization**: Start with `left = 0` and `right = array.Length - 1`
- **Loop condition**: Continue while search space exists (`left <= right`)
- **Calculate mid**: Use `left + (right - left) / 2` to avoid overflow
- **Three cases**:
  1. Found: `array[mid] == target` → return mid
  2. Too small: `array[mid] < target` → search right (left = mid + 1)
  3. Too large: `array[mid] > target` → search left (right = mid - 1)
- **Not found**: Loop exits when left > right → return -1

**Time Complexity**: O(log n) - halves search space each iteration
**Space Complexity**: O(1) - only uses a few variables

**Example Trace:**
```
Array: [1, 3, 5, 7, 9, 11, 13]  Target: 7

Iteration 1: left=0, right=6, mid=3, array[3]=7 → FOUND! Return 3
```

---

## TODO 2: BinarySearchRecursive

```csharp
public static int BinarySearchRecursive(int[] array, int target)
{
    return BinarySearchRecursiveHelper(array, target, 0, array.Length - 1);
}

private static int BinarySearchRecursiveHelper(int[] array, int target, int left, int right)
{
    // Base case: search space exhausted
    if (left > right)
        return -1;

    int mid = left + (right - left) / 2;

    // Found!
    if (array[mid] == target)
        return mid;

    // Recursive cases
    if (array[mid] < target)
        return BinarySearchRecursiveHelper(array, target, mid + 1, right);  // Search right
    else
        return BinarySearchRecursiveHelper(array, target, left, mid - 1);  // Search left
}
```

**Explanation:**
- **Base case**: When `left > right`, search space is empty → return -1
- **Found case**: When `array[mid] == target`, return mid immediately
- **Recursive cases**:
  - If target is larger, recurse on right half: `(mid + 1, right)`
  - If target is smaller, recurse on left half: `(left, mid - 1)`

**Time Complexity**: O(log n) - same as iterative
**Space Complexity**: O(log n) - recursive call stack depth

**Recursion Tree Example:**
```
Array: [1, 3, 5, 7, 9]  Target: 7

Call 1: left=0, right=4, mid=2, array[2]=5 → 5 < 7, recurse right
  Call 2: left=3, right=4, mid=3, array[3]=7 → FOUND! Return 3
Return 3
```

**Trade-offs:**
- ✅ More elegant and easier to read
- ✅ Easier to explain in interviews
- ❌ Uses O(log n) stack space
- ❌ Risk of stack overflow for very large arrays
- ❌ Slightly slower due to function call overhead

---

## TODO 3: FindFirstOccurrence

```csharp
public static int FindFirstOccurrence(int[] array, int target)
{
    int left = 0;
    int right = array.Length - 1;
    int result = -1;  // Store the leftmost found index

    while (left <= right)
    {
        int mid = left + (right - left) / 2;

        if (array[mid] == target)
        {
            result = mid;      // Found! But don't return yet
            right = mid - 1;   // Continue searching left for earlier occurrences
        }
        else if (array[mid] < target)
            left = mid + 1;
        else
            right = mid - 1;
    }

    return result;
}
```

**Explanation:**
- **Key modification**: When target is found, **don't return immediately**
- Store the index in `result` variable
- Continue searching **left half** (`right = mid - 1`) to find earlier occurrences
- Loop continues until entire search space is explored
- Final `result` contains the leftmost occurrence

**Why it works:**
```
Array: [1, 2, 2, 2, 3, 4]  Target: 2

Step 1: mid=2, array[2]=2 → Found! result=2, search left (right=1)
Step 2: mid=1, array[1]=2 → Found! result=1, search left (right=0)
Step 3: mid=0, array[0]=1 → 1 < 2, search right (left=1)
Step 4: left > right, exit loop
Return result=1 (first occurrence)
```

**Time Complexity**: O(log n) - still binary search
**Space Complexity**: O(1)

**Use cases:**
- Finding range of elements in sorted array
- Finding first insertion point for duplicates
- LeetCode problems: "Find First and Last Position of Element in Sorted Array"

---

## TODO 4: FindLastOccurrence

```csharp
public static int FindLastOccurrence(int[] array, int target)
{
    int left = 0;
    int right = array.Length - 1;
    int result = -1;  // Store the rightmost found index

    while (left <= right)
    {
        int mid = left + (right - left) / 2;

        if (array[mid] == target)
        {
            result = mid;      // Found! But don't return yet
            left = mid + 1;    // Continue searching right for later occurrences
        }
        else if (array[mid] < target)
            left = mid + 1;
        else
            right = mid - 1;
    }

    return result;
}
```

**Explanation:**
- **Opposite of FindFirstOccurrence**
- When target is found, store index and search **right half** (`left = mid + 1`)
- This ensures we find the rightmost occurrence

**Example:**
```
Array: [1, 2, 2, 2, 3, 4]  Target: 2

Step 1: mid=2, array[2]=2 → Found! result=2, search right (left=3)
Step 2: mid=4, array[4]=3 → 3 > 2, search left (right=3)
Step 3: mid=3, array[3]=2 → Found! result=3, search right (left=4)
Step 4: left > right, exit loop
Return result=3 (last occurrence)
```

**Counting occurrences:**
```csharp
int count = FindLastOccurrence(array, target) - FindFirstOccurrence(array, target) + 1;
// For [1, 2, 2, 2, 3]: last=3, first=1 → count = 3 - 1 + 1 = 3 occurrences
```

---

## TODO 5: FindClosestElement

```csharp
public static int FindClosestElement(int[] array, int target)
{
    if (array.Length == 0)
        throw new ArgumentException("Array cannot be empty");

    if (array.Length == 1)
        return 0;

    int left = 0;
    int right = array.Length - 1;

    // Standard binary search to narrow down
    while (left < right)
    {
        int mid = left + (right - left) / 2;

        if (array[mid] == target)
            return mid;  // Exact match - closest possible

        if (array[mid] < target)
            left = mid + 1;
        else
            right = mid;  // Note: mid, not mid - 1
    }

    // At this point, left == right or search narrowed to 1-2 elements
    // Compare distances to find closest

    // Edge cases
    if (left == 0)
        return 0;
    if (left >= array.Length)
        return array.Length - 1;

    // Compare left and left-1
    int distLeft = Math.Abs(array[left - 1] - target);
    int distRight = Math.Abs(array[left] - target);

    return distLeft <= distRight ? left - 1 : left;
}
```

**Explanation:**
- **Phase 1**: Standard binary search to narrow down to 1-2 candidates
- **Phase 2**: Compare distances of final candidates to target
- **Edge cases**:
  - Empty array → throw exception (cannot find closest)
  - Single element → return 0 (only option)
  - Target smaller than all → return 0
  - Target larger than all → return last index

**Example walkthrough:**
```
Array: [1, 3, 5, 7, 9]  Target: 6

Binary search phase:
  left=0, right=4, mid=2, array[2]=5 → 5 < 6, left=3
  left=3, right=4, mid=3, array[3]=7 → 7 > 6, right=3
  left == right, exit loop

Comparison phase:
  left=3: array[3]=7, distance = |7 - 6| = 1
  left-1=2: array[2]=5, distance = |5 - 6| = 1
  Equal distance, return smaller index: 2
```

**Tie-breaking rule**: When distances are equal, return smaller index (or you can choose larger - be consistent).

---

## Alternative Solutions

### TODO 1 - Iterative with Early Exit Optimization

```csharp
public static int BinarySearchIterative(int[] array, int target)
{
    if (array.Length == 0) return -1;  // Early exit

    int left = 0;
    int right = array.Length - 1;

    while (left <= right)
    {
        // Special cases for small search spaces
        if (left == right)
            return array[left] == target ? left : -1;

        int mid = left + (right - left) / 2;

        if (array[mid] == target) return mid;
        if (array[mid] < target)
            left = mid + 1;
        else
            right = mid - 1;
    }

    return -1;
}
```

### TODO 2 - Tail Recursive (C# doesn't optimize, but good to know)

```csharp
// Note: C# compiler doesn't do tail call optimization
// This is for educational purposes
private static int BinarySearchTailRecursive(int[] array, int target, int left, int right)
{
    if (left > right) return -1;

    int mid = left + (right - left) / 2;

    if (array[mid] == target) return mid;

    // Tail recursive calls
    return array[mid] < target
        ? BinarySearchTailRecursive(array, target, mid + 1, right)
        : BinarySearchTailRecursive(array, target, left, mid - 1);
}
```

### TODO 5 - Alternative Closest Element (Two-pointer approach)

```csharp
public static int FindClosestElement(int[] array, int target)
{
    if (array.Length == 0)
        throw new ArgumentException("Array cannot be empty");

    int closestIndex = 0;
    int minDistance = Math.Abs(array[0] - target);

    // Linear scan (O(n)) - simple but slower
    for (int i = 1; i < array.Length; i++)
    {
        int distance = Math.Abs(array[i] - target);
        if (distance < minDistance)
        {
            minDistance = distance;
            closestIndex = i;
        }
    }

    return closestIndex;
}
```

---

## Complexity Analysis Summary

| Method | Time | Space | Notes |
|--------|------|-------|-------|
| BinarySearchIterative | O(log n) | O(1) | **Best for production** |
| BinarySearchRecursive | O(log n) | O(log n) | Stack depth = log n |
| FindFirstOccurrence | O(log n) | O(1) | Modified binary search |
| FindLastOccurrence | O(log n) | O(1) | Modified binary search |
| FindClosestElement | O(log n) | O(1) | Binary search + comparison |

**Why log n steps?**
- Each iteration halves the search space
- For n = 1,024 elements: log₂(1024) = 10 steps maximum
- For n = 1,000,000: log₂(1,000,000) ≈ 20 steps maximum

---

## Common Interview Follow-ups

### Q1: "What if the array is not sorted?"
**Answer**: Binary search **requires** a sorted array. Options:
1. Sort first: O(n log n) + O(log n) = O(n log n)
2. Use linear search: O(n) - better if array is small or unsorted

### Q2: "How would you search in a rotated sorted array?"
**Example**: `[4, 5, 6, 7, 0, 1, 2]` target = 0

**Answer**: Modified binary search:
```csharp
public static int SearchRotated(int[] array, int target)
{
    int left = 0, right = array.Length - 1;

    while (left <= right)
    {
        int mid = left + (right - left) / 2;
        if (array[mid] == target) return mid;

        // Determine which half is sorted
        if (array[left] <= array[mid])  // Left half is sorted
        {
            if (target >= array[left] && target < array[mid])
                right = mid - 1;  // Target in sorted left half
            else
                left = mid + 1;   // Target in right half
        }
        else  // Right half is sorted
        {
            if (target > array[mid] && target <= array[right])
                left = mid + 1;   // Target in sorted right half
            else
                right = mid - 1;  // Target in left half
        }
    }

    return -1;
}
```

### Q3: "Find peak element in array"
**Definition**: Element greater than neighbors

**Answer**: Modified binary search:
```csharp
public static int FindPeakElement(int[] array)
{
    int left = 0, right = array.Length - 1;

    while (left < right)
    {
        int mid = left + (right - left) / 2;

        if (array[mid] > array[mid + 1])
            right = mid;  // Peak is on left (or mid itself)
        else
            left = mid + 1;  // Peak is on right
    }

    return left;
}
```

---

## Key Takeaways

1. **Binary search works only on sorted data** - Always verify this precondition
2. **O(log n) is extremely efficient** - Scales to millions of elements
3. **Iterative > Recursive for production** - O(1) space, no stack overflow
4. **Modified binary search is powerful** - First/last occurrence, closest element, rotated array
5. **Edge cases matter** - Empty array, single element, not found, duplicates
6. **Mid calculation matters** - Use `left + (right - left) / 2` to avoid overflow

---

**Congratulations on completing the Binary Search exercise! 🎉**

You now understand:
- ✅ Classic binary search (iterative & recursive)
- ✅ Finding first/last occurrences in duplicates
- ✅ Finding closest elements
- ✅ Time/space complexity analysis
- ✅ Common interview variations

*Next up: QuickSort - a divide-and-conquer sorting algorithm!*
