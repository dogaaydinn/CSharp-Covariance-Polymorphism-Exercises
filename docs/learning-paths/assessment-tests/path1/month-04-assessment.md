# Month 4 Comprehensive Assessment - Algorithms & Data Structures

**Month**: 4 (Weeks 13-16) | **Duration**: 90 min | **Pass**: 80% (24/30) | **Points**: 30

## Section 1: Multiple Choice (15 questions, 0.5 pts each = 7.5 pts)

1. Binary search time complexity:
   - a) O(n) | b) O(log n) | c) O(n log n) | d) O(1)

2. Binary search requirement:
   - a) Any array | b) Sorted array | c) Linked list | d) Hash table

3. Binary search space complexity:
   - a) O(1) iterative, O(log n) recursive | b) Always O(n) | c) O(log n) always | d) O(n²)

4. QuickSort average case:
   - a) O(n) | b) O(n log n) | c) O(n²) | d) O(log n)

5. QuickSort worst case:
   - a) O(n) | b) O(n log n) | c) O(n²) | d) O(log n)

6. What causes QuickSort worst case?
   - a) Random data | b) Already sorted with bad pivot | c) Duplicates | d) Large array

7. MergeSort time complexity:
   - a) O(n) | b) O(n log n) always | c) O(n²) | d) Varies

8. MergeSort space complexity:
   - a) O(1) | b) O(log n) | c) O(n) | d) O(n²)

9. What is stable sorting?
   - a) No crashes | b) Preserves relative order of equal elements | c) Always O(n log n) | d) In-place

10. Which is stable?
    - a) QuickSort | b) HeapSort | c) MergeSort | d) None

11. Stack follows:
    - a) FIFO | b) LIFO | c) Random access | d) Priority

12. Queue follows:
    - a) FIFO | b) LIFO | c) Random access | d) Priority

13. Array access time:
    - a) O(1) | b) O(n) | c) O(log n) | d) O(n²)

14. Linked List insertion at head:
    - a) O(1) | b) O(n) | c) O(log n) | d) O(n²)

15. Array vs Linked List for random access:
    - a) Both O(1) | b) Array O(1), List O(n) | c) Array O(n), List O(1) | d) Both O(n)

## Section 2: Short Answer (7 questions, 2 pts each = 14 pts)

16. Explain how binary search works. Why must the array be sorted?

17. Explain the partition step in QuickSort. What does it accomplish?

18. Compare QuickSort vs MergeSort:
    - Time complexity (best, average, worst)
    - Space complexity
    - Stability
    - When to use each

19. Explain what "divide and conquer" means. How do QuickSort and MergeSort use it differently?

20. Design a Stack<T> implementation using an array. What are the key operations and their time complexities?

21. Explain Big O notation. What's the difference between O(n), O(log n), and O(n²) with real examples?

22. When would you choose a Linked List over an Array? Give specific scenarios.

## Section 3: Code Implementation (4 questions, 2 pts each = 8 pts)

23. Implement iterative binary search:
```csharp
public static int BinarySearch(int[] arr, int target)
{
    // Return index of target, or -1 if not found
    // arr is sorted in ascending order
}
```

24. Implement the partition function for QuickSort (Lomuto partition):
```csharp
public static int Partition(int[] arr, int low, int high)
{
    // Choose last element as pivot
    // Rearrange so elements < pivot are on left
    // Return pivot's final position
}
```

25. Implement Stack<T> using array:
```csharp
public class Stack<T>
{
    // Requirements:
    // - Push(T item)
    // - T Pop()
    // - T Peek()
    // - bool IsEmpty
    // - Dynamic resize when full
}
```

26. Implement Queue<T> using linked list:
```csharp
public class Queue<T>
{
    // Requirements:
    // - Enqueue(T item) - add to rear
    // - T Dequeue() - remove from front
    // - T Peek()
    // - bool IsEmpty
}
```

## Answer Key

**MC**: 1.b | 2.b | 3.a | 4.b | 5.c | 6.b | 7.b | 8.c | 9.b | 10.c | 11.b | 12.a | 13.a | 14.a | 15.b

### Short Answer

**16. Binary Search** (2 pts):
- **How it works**:
  1. Compare target with middle element
  2. If target = middle, found!
  3. If target < middle, search left half
  4. If target > middle, search right half
  5. Repeat until found or range empty
- **Why sorted?**: Algorithm assumes ordering to eliminate half of remaining elements each step
- Without sorting, can't determine which half to search (would need to check both)
- Example: Finding 7 in [1,3,5,7,9,11,13]
  - Middle = 7 ✓ Found in 1 step!
  - If searching for 3: 3 < 7, search [1,3,5], middle = 3 ✓ Found

**17. Partition in QuickSort** (2 pts):
- **Purpose**: Rearrange array so pivot is in its final sorted position
- **Process** (Lomuto scheme):
  1. Choose pivot (usually last element)
  2. Maintain index `i` for smaller elements
  3. Scan left to right
  4. If element ≤ pivot, swap with position `i` and increment `i`
  5. Finally, swap pivot with position `i`
- **Result**: Elements before pivot are ≤ pivot, elements after are > pivot
- **Example**: `[3, 7, 1, 5, 9]` with pivot=9
  - After partition: `[3, 7, 1, 5, 9]` → `[3, 1, 5, 7, 9]` (pivot at index 4)

**18. QuickSort vs MergeSort** (2 pts):

| Aspect | QuickSort | MergeSort |
|--------|-----------|-----------|
| **Best case** | O(n log n) | O(n log n) |
| **Average** | O(n log n) | O(n log n) |
| **Worst case** | O(n²) | O(n log n) |
| **Space** | O(log n) stack | O(n) auxiliary |
| **Stability** | Unstable | Stable |
| **In-place** | Yes | No |

**When to use**:
- **QuickSort**: General purpose, memory constrained, average case performance critical
- **MergeSort**: Stability required, worst-case guarantee needed, linked lists

**19. Divide and Conquer** (2 pts):
- **Definition**: Break problem into smaller subproblems, solve recursively, combine results
- **QuickSort approach**:
  - Divide: Partition around pivot (O(n) work)
  - Conquer: Recursively sort left and right
  - Combine: Nothing! (already in place)
  - Work done BEFORE recursion
- **MergeSort approach**:
  - Divide: Split in half (O(1) work)
  - Conquer: Recursively sort both halves
  - Combine: Merge sorted halves (O(n) work)
  - Work done AFTER recursion
- Key difference: QuickSort does work in divide step, MergeSort in combine step

**20. Stack<T> Design** (2 pts):
```csharp
public class Stack<T>
{
    private T[] _items;
    private int _count;

    public Stack() { _items = new T[4]; }

    // Push: O(1) amortized (O(n) when resize)
    public void Push(T item)
    {
        if (_count == _items.Length)
            Array.Resize(ref _items, _items.Length * 2);
        _items[_count++] = item;
    }

    // Pop: O(1)
    public T Pop()
    {
        if (IsEmpty) throw new InvalidOperationException();
        return _items[--_count];
    }

    // Peek: O(1)
    public T Peek()
    {
        if (IsEmpty) throw new InvalidOperationException();
        return _items[_count - 1];
    }

    public bool IsEmpty => _count == 0;
}
```

**21. Big O Notation** (2 pts):
- **Definition**: Describes how runtime/space grows as input size increases
- **O(1) - Constant**: Same time regardless of input
  - Example: Array access `arr[5]`, hash table lookup
- **O(log n) - Logarithmic**: Halves problem size each step
  - Example: Binary search in sorted array of 1M items = ~20 comparisons
- **O(n) - Linear**: Proportional to input
  - Example: Finding max in unsorted array, must check all n elements
- **O(n log n) - Linearithmic**: Efficient sorting
  - Example: MergeSort, QuickSort average case
- **O(n²) - Quadratic**: Nested loops
  - Example: Bubble sort, checking all pairs
- **Growth**: O(1) < O(log n) < O(n) < O(n log n) < O(n²) < O(2ⁿ)

**22. Linked List vs Array** (2 pts):
**Choose Linked List when**:
- Frequent insertions/deletions at beginning or middle: O(1) vs O(n)
- Unknown/variable size: No resize overhead
- Don't need random access: Sequential access is fine
- Memory fragmentation OK: Allocates per node

**Scenarios**:
1. **Undo/Redo system**: Constant insertion/deletion at current position
2. **Music playlist**: Insert/remove songs anywhere without shifting
3. **Browser history**: Navigate forward/back, insert at current position

**Array advantages**:
- Random access O(1) vs O(n)
- Better cache locality (contiguous memory)
- Less memory overhead (no next pointers)

### Code Implementation

**23. Binary Search** (2 pts):
```csharp
public static int BinarySearch(int[] arr, int target)
{
    int left = 0;
    int right = arr.Length - 1;

    while (left <= right)
    {
        int mid = left + (right - left) / 2; // Avoid overflow

        if (arr[mid] == target)
            return mid; // Found!

        if (arr[mid] < target)
            left = mid + 1; // Search right half
        else
            right = mid - 1; // Search left half
    }

    return -1; // Not found
}

// Example usage
int[] arr = { 1, 3, 5, 7, 9, 11, 13 };
int index = BinarySearch(arr, 7); // Returns 3
int notFound = BinarySearch(arr, 4); // Returns -1
```

**24. Partition Function** (2 pts):
```csharp
public static int Partition(int[] arr, int low, int high)
{
    int pivot = arr[high]; // Choose last element as pivot
    int i = low - 1; // Index of smaller element

    for (int j = low; j < high; j++)
    {
        // If current element <= pivot
        if (arr[j] <= pivot)
        {
            i++;
            // Swap arr[i] and arr[j]
            int temp = arr[i];
            arr[i] = arr[j];
            arr[j] = temp;
        }
    }

    // Swap arr[i+1] and arr[high] (pivot)
    int temp2 = arr[i + 1];
    arr[i + 1] = arr[high];
    arr[high] = temp2;

    return i + 1; // Return pivot's final position
}

// Example
int[] arr = { 10, 7, 8, 9, 1, 5 };
int pivotIndex = Partition(arr, 0, arr.Length - 1);
// arr is now: [1, 5, 7, 9, 10, 8] with pivot (5) at index 1
```

**25. Stack<T> Implementation** (2 pts):
```csharp
public class Stack<T>
{
    private T[] _items;
    private int _count;
    private const int DefaultCapacity = 4;

    public Stack()
    {
        _items = new T[DefaultCapacity];
        _count = 0;
    }

    public void Push(T item)
    {
        if (_count == _items.Length)
        {
            // Double capacity when full
            T[] newArray = new T[_items.Length * 2];
            Array.Copy(_items, newArray, _items.Length);
            _items = newArray;
        }

        _items[_count++] = item;
    }

    public T Pop()
    {
        if (IsEmpty)
            throw new InvalidOperationException("Stack is empty");

        T item = _items[--_count];
        _items[_count] = default(T); // Clear reference for GC
        return item;
    }

    public T Peek()
    {
        if (IsEmpty)
            throw new InvalidOperationException("Stack is empty");

        return _items[_count - 1];
    }

    public bool IsEmpty => _count == 0;

    public int Count => _count;
}

// Usage
var stack = new Stack<int>();
stack.Push(10);
stack.Push(20);
stack.Push(30);
Console.WriteLine(stack.Pop()); // 30
Console.WriteLine(stack.Peek()); // 20
```

**26. Queue<T> with Linked List** (2 pts):
```csharp
public class Queue<T>
{
    private class Node
    {
        public T Data { get; set; }
        public Node Next { get; set; }

        public Node(T data)
        {
            Data = data;
            Next = null;
        }
    }

    private Node _front; // First node (dequeue from here)
    private Node _rear;  // Last node (enqueue here)
    private int _count;

    public Queue()
    {
        _front = null;
        _rear = null;
        _count = 0;
    }

    public void Enqueue(T item)
    {
        Node newNode = new Node(item);

        if (_rear == null) // Queue was empty
        {
            _front = newNode;
            _rear = newNode;
        }
        else
        {
            _rear.Next = newNode;
            _rear = newNode;
        }

        _count++;
    }

    public T Dequeue()
    {
        if (IsEmpty)
            throw new InvalidOperationException("Queue is empty");

        T data = _front.Data;
        _front = _front.Next;

        if (_front == null) // Queue became empty
            _rear = null;

        _count--;
        return data;
    }

    public T Peek()
    {
        if (IsEmpty)
            throw new InvalidOperationException("Queue is empty");

        return _front.Data;
    }

    public bool IsEmpty => _count == 0;

    public int Count => _count;
}

// Usage
var queue = new Queue<string>();
queue.Enqueue("First");
queue.Enqueue("Second");
queue.Enqueue("Third");
Console.WriteLine(queue.Dequeue()); // "First"
Console.WriteLine(queue.Peek());    // "Second"
```

## Grading Rubric

| Section | Max Points | Criteria |
|---------|-----------|----------|
| Multiple Choice | 7.5 | 0.5 per correct answer |
| Short Answer (each) | 2 × 7 = 14 | Full: Complete + examples. Partial: 1.0-1.5. Wrong: 0 |
| Code Implementation (each) | 2 × 4 = 8 | Full: Working + efficient. Partial: 1.0-1.5. Wrong: 0 |
| **Total** | **30** | **Pass: 24 points (80%)** |

---

## Study Resources

**Week 13 - Binary Search**:
- `samples/99-Exercises/Algorithms/01-BinarySearch/`
- Theory: Divide-and-conquer, O(log n) complexity

**Week 14 - QuickSort**:
- `samples/99-Exercises/Algorithms/02-QuickSort/`
- Partition schemes: Lomuto vs Hoare

**Week 15 - MergeSort**:
- `samples/99-Exercises/Algorithms/03-MergeSort/`
- Stable sorting, O(n) space

**Week 16 - Data Structures**:
- Stack: LIFO, O(1) push/pop
- Queue: FIFO, O(1) enqueue/dequeue
- Linked List: Dynamic size, O(1) insert at head

---

## Performance Comparison Table

| Algorithm | Best | Average | Worst | Space | Stable | In-Place |
|-----------|------|---------|-------|-------|--------|----------|
| Binary Search | O(1) | O(log n) | O(log n) | O(1) | N/A | Yes |
| QuickSort | O(n log n) | O(n log n) | O(n²) | O(log n) | No | Yes |
| MergeSort | O(n log n) | O(n log n) | O(n log n) | O(n) | Yes | No |
| BubbleSort | O(n) | O(n²) | O(n²) | O(1) | Yes | Yes |

---

## Next Steps

**If you passed (≥24 pts)**: Proceed to Month 5 (Advanced Design Patterns & SOLID)

**If you didn't pass (<24 pts)**: Review weak areas:
- Score 0-10: Review all algorithms from scratch
- Score 11-18: Practice implementations
- Score 19-23: Focus on complexity analysis

---

*Assessment Version: 1.0*
*Last Updated: 2025-12-02*
