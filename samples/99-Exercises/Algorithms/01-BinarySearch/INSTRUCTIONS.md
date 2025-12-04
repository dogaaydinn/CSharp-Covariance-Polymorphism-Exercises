# Binary Search Algorithm Exercise

## 📚 Learning Objectives

By completing this exercise, you will learn:
- **Binary Search Algorithm**: The fundamental O(log n) search algorithm
- **Iterative vs Recursive**: Two implementation approaches and their trade-offs
- **Modified Binary Search**: Finding first/last occurrences and closest elements
- **Time & Space Complexity**: Understanding Big O notation
- **Edge Cases**: Empty arrays, single elements, duplicates
- **Interview Skills**: One of the most common algorithm interview topics

## 🎯 Exercise Tasks

You need to complete **5 TODO methods** in `Program.cs`:

1. ✅ **BinarySearchIterative()** - Standard binary search (iterative)
2. ✅ **BinarySearchRecursive()** - Standard binary search (recursive)
3. ✅ **FindFirstOccurrence()** - Leftmost binary search for duplicates
4. ✅ **FindLastOccurrence()** - Rightmost binary search for duplicates
5. ✅ **FindClosestElement()** - Find element closest to target

## 🚀 Getting Started

### Step 1: Navigate to the Project
```bash
cd samples/99-Exercises/Algorithms/01-BinarySearch
```

### Step 2: Restore Dependencies
```bash
dotnet restore
```

### Step 3: Run the Tests (They Should FAIL)
```bash
dotnet test
```

You should see **FAILED** tests initially. This is expected!

### Step 4: Complete the TODOs
Open `Program.cs` and look for `// TODO` comments. Complete each method.

### Step 5: Re-run Tests
```bash
dotnet test
```

Keep working until **ALL TESTS PASS**! ✅

## 💡 Binary Search Fundamentals

### What is Binary Search?

Binary search is a **divide and conquer** algorithm that finds a target value in a **sorted array** by repeatedly halving the search space.

**Visualization:**
```
Array: [1, 3, 5, 7, 9, 11, 13, 15, 17]  Target: 13
                     ↑
                    mid

Step 1: mid = 9 (index 4)
        9 < 13, search right half → [11, 13, 15, 17]

Step 2: mid = 13 (index 6)
        13 == 13, FOUND at index 6!
```

### Time & Space Complexity

| Implementation | Time | Space | Why? |
|----------------|------|-------|------|
| **Iterative** | O(log n) | O(1) | Halves search space each step, no recursion |
| **Recursive** | O(log n) | O(log n) | Halves search space, but uses call stack |

**Why O(log n)?**
- Array size: 1,000 → Max steps: ~10
- Array size: 1,000,000 → Max steps: ~20
- Each step halves the remaining elements

## 🔍 Detailed Hints

### TODO 1: BinarySearchIterative

**Goal**: Find target using iterative approach (while loop).

**Algorithm:**
```csharp
public static int BinarySearchIterative(int[] array, int target)
{
    int left = 0;
    int right = array.Length - 1;

    while (left <= right)
    {
        int mid = left + (right - left) / 2;  // Avoids overflow

        if (array[mid] == target)
            return mid;  // Found!

        if (array[mid] < target)
            left = mid + 1;  // Search right half
        else
            right = mid - 1;  // Search left half
    }

    return -1;  // Not found
}
```

**Why `left + (right - left) / 2` instead of `(left + right) / 2`?**
- Avoids integer overflow when left + right > Int32.MaxValue
- Both give same result for normal cases

**Edge Cases to Handle:**
- Empty array → return -1
- Single element → check if it matches
- Target not in array → return -1

### TODO 2: BinarySearchRecursive

**Goal**: Find target using recursive approach.

**Algorithm:**
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

    if (array[mid] == target)
        return mid;

    // Recursive cases
    if (array[mid] < target)
        return BinarySearchRecursiveHelper(array, target, mid + 1, right);
    else
        return BinarySearchRecursiveHelper(array, target, left, mid - 1);
}
```

**Base Case**: When `left > right`, search space is empty → return -1
**Recursive Case**: Call helper on left or right half

**Trade-offs:**
- ✅ More elegant, easier to understand
- ❌ Uses O(log n) call stack space
- ❌ Slightly slower due to function call overhead

### TODO 3: FindFirstOccurrence

**Goal**: Find the **leftmost** (first) occurrence of target in array with duplicates.

**Example:**
```
Array: [1, 2, 2, 2, 3, 4]  Target: 2
            ↑
       First occurrence at index 1 (not 2 or 3)
```

**Modified Algorithm:**
```csharp
public static int FindFirstOccurrence(int[] array, int target)
{
    int left = 0;
    int right = array.Length - 1;
    int result = -1;  // Store the first found index

    while (left <= right)
    {
        int mid = left + (right - left) / 2;

        if (array[mid] == target)
        {
            result = mid;        // Found! But keep searching left
            right = mid - 1;     // Continue in left half
        }
        else if (array[mid] < target)
            left = mid + 1;
        else
            right = mid - 1;
    }

    return result;
}
```

**Key Difference**: When target is found, **don't return immediately**. Store the index and continue searching left to find earlier occurrences.

### TODO 4: FindLastOccurrence

**Goal**: Find the **rightmost** (last) occurrence of target.

**Example:**
```
Array: [1, 2, 2, 2, 3, 4]  Target: 2
                  ↑
       Last occurrence at index 3 (not 1 or 2)
```

**Algorithm:**
```csharp
public static int FindLastOccurrence(int[] array, int target)
{
    int left = 0;
    int right = array.Length - 1;
    int result = -1;

    while (left <= right)
    {
        int mid = left + (right - left) / 2;

        if (array[mid] == target)
        {
            result = mid;        // Found! But keep searching right
            left = mid + 1;      // Continue in right half
        }
        else if (array[mid] < target)
            left = mid + 1;
        else
            right = mid - 1;
    }

    return result;
}
```

**Key Difference**: When found, continue searching **right** to find later occurrences.

### TODO 5: FindClosestElement

**Goal**: Find the element **closest to target** (target may not exist in array).

**Example:**
```
Array: [1, 3, 5, 7, 9]  Target: 6
           ↑     ↑
          5(2)  7(3)
       Distance from 6: 1   1

Either index 2 or 3 is acceptable (both distance 1)
```

**Algorithm:**
```csharp
public static int FindClosestElement(int[] array, int target)
{
    if (array.Length == 0)
        throw new ArgumentException("Array cannot be empty");

    if (array.Length == 1)
        return 0;

    int left = 0;
    int right = array.Length - 1;

    // Binary search to narrow down to two candidates
    while (left < right)
    {
        int mid = left + (right - left) / 2;

        if (array[mid] == target)
            return mid;  // Exact match

        if (array[mid] < target)
            left = mid + 1;
        else
            right = mid;
    }

    // Now left == right or left == right + 1
    // Compare distances to find closest
    if (left == 0)
        return 0;
    if (left >= array.Length)
        return array.Length - 1;

    int distLeft = Math.Abs(array[left - 1] - target);
    int distRight = Math.Abs(array[left] - target);

    return distLeft <= distRight ? left - 1 : left;
}
```

**Edge Cases:**
- Empty array → throw exception
- Single element → return 0
- Target smaller than all → return 0
- Target larger than all → return last index

## ✅ Success Criteria

All tests should pass:
- [ ] `BinarySearchIterative_ShouldFindElementInMiddle` ✅
- [ ] `BinarySearchIterative_ShouldFindFirstElement` ✅
- [ ] `BinarySearchIterative_ShouldFindLastElement` ✅
- [ ] `BinarySearchIterative_ShouldReturnMinusOneWhenNotFound` ✅
- [ ] `BinarySearchIterative_ShouldHandleSingleElement` ✅
- [ ] `BinarySearchIterative_ShouldHandleEmptyArray` ✅
- [ ] `BinarySearchRecursive_ShouldFindElementInMiddle` ✅
- [ ] `BinarySearchRecursive_ShouldFindFirstElement` ✅
- [ ] `BinarySearchRecursive_ShouldReturnMinusOneWhenNotFound` ✅
- [ ] `BinarySearchRecursive_ShouldHandleEmptyArray` ✅
- [ ] `FindFirstOccurrence_ShouldFindLeftmostInDuplicates` ✅
- [ ] `FindFirstOccurrence_ShouldHandleAllSameElements` ✅
- [ ] `FindFirstOccurrence_ShouldHandleSingleOccurrence` ✅
- [ ] `FindFirstOccurrence_ShouldReturnMinusOneWhenNotFound` ✅
- [ ] `FindLastOccurrence_ShouldFindRightmostInDuplicates` ✅
- [ ] `FindLastOccurrence_ShouldHandleAllSameElements` ✅
- [ ] `FindLastOccurrence_ShouldHandleLastElementDuplicated` ✅
- [ ] `FindLastOccurrence_ShouldReturnMinusOneWhenNotFound` ✅
- [ ] `FindClosestElement_ShouldFindExactMatch` ✅
- [ ] `FindClosestElement_ShouldFindClosestWhenBetweenElements` ✅
- [ ] `FindClosestElement_ShouldHandleTargetSmallerThanAll` ✅
- [ ] `FindClosestElement_ShouldHandleTargetLargerThanAll` ✅
- [ ] `FindClosestElement_ShouldHandleSingleElement` ✅
- [ ] `FindClosestElement_ShouldThrowOnEmptyArray` ✅
- [ ] `BothImplementations_ShouldReturnSameResults` ✅
- [ ] `FirstAndLastOccurrence_ShouldFormValidRange` ✅

**Total: 26 tests must pass!**

## 🐛 Common Mistakes to Avoid

1. **Off-by-one errors**:
   ```csharp
   // ❌ Wrong - misses last element
   while (left < right)

   // ✅ Correct
   while (left <= right)
   ```

2. **Integer overflow**:
   ```csharp
   // ❌ Wrong - can overflow
   int mid = (left + right) / 2;

   // ✅ Correct
   int mid = left + (right - left) / 2;
   ```

3. **Forgetting to update pointers**:
   ```csharp
   // ❌ Wrong - infinite loop!
   if (array[mid] < target)
       left = mid;  // Should be mid + 1

   // ✅ Correct
   if (array[mid] < target)
       left = mid + 1;
   ```

4. **Not handling empty arrays**:
   ```csharp
   // ❌ Wrong - crashes on empty array
   int mid = array.Length / 2;
   if (array[mid] == target) ...

   // ✅ Correct
   if (array.Length == 0) return -1;
   ```

## 🎓 Interview Tips

### Common Binary Search Interview Questions

1. **Find target in sorted array** → TODO 1 or 2
2. **Find first/last occurrence** → TODO 3 & 4
3. **Search in rotated sorted array** → Modified binary search
4. **Find peak element** → Modified binary search
5. **Find minimum in rotated sorted array** → Modified binary search
6. **Search in 2D matrix** → Binary search on rows + columns

### What Interviewers Look For

✅ **Correct implementation** - algorithm works for all cases
✅ **Edge case handling** - empty, single element, not found
✅ **Complexity analysis** - can explain O(log n) time
✅ **Code clarity** - readable variable names, clear logic
✅ **Testing mindset** - mentions edge cases without prompting

### Common Follow-up Questions

Q: "What if the array is not sorted?"
A: Binary search requires sorted input. Would need to sort first O(n log n) or use linear search O(n).

Q: "Iterative vs Recursive - which is better?"
A: Iterative is better for production (O(1) space, no stack overflow risk). Recursive is more elegant for interviews.

Q: "How would you search a very large file that doesn't fit in memory?"
A: Use external binary search with disk I/O, reading only the middle block each iteration.

## 🚀 Challenge: Bonus Tasks

Once you complete all TODOs, try these variants:

1. **Search in Rotated Array**:
   ```
   [4, 5, 6, 7, 0, 1, 2] target=0 → index 4
   ```

2. **Find Peak Element**:
   ```
   [1, 2, 3, 1] → peak at index 2 (value 3)
   ```

3. **Search Insert Position**:
   ```
   [1, 3, 5, 6] target=2 → return 1 (where 2 should be inserted)
   ```

4. **Count Occurrences**:
   ```
   [1, 2, 2, 2, 3] target=2 → return 3 (using first + last occurrence)
   ```

## 📚 Additional Resources

- [Binary Search Visualization](https://www.cs.usfca.edu/~galles/visualization/Search.html)
- [LeetCode Binary Search Problems](https://leetcode.com/tag/binary-search/)
- [Big O Cheat Sheet](https://www.bigocheatsheet.com/)

---

**Good luck! 🎉**

*Once you complete all TODOs, check `SOLUTION.md` for complete solutions (but try yourself first!).*
