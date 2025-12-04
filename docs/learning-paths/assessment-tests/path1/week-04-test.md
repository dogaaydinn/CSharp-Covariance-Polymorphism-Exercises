# Week 4 Assessment Test - Boxing, Unboxing, and Performance

**Week**: 4 | **Duration**: 30 min | **Pass**: 70% | **Points**: 10

## Multiple Choice (5 pts)

1. What is boxing?
   - a) Converting reference type to value type
   - b) Converting value type to reference type
   - c) Wrapping objects in containers
   - d) Type checking

2. Where are boxed values stored?
   - a) Stack
   - b) Heap
   - c) Registers
   - d) Cache

3. Which operation causes boxing?
   - a) `int x = 5;`
   - b) `object obj = 5;`
   - c) `int y = x;`
   - d) `int z = x + 1;`

4. What's the performance cost of boxing?
   - a) None
   - b) Heap allocation + GC pressure
   - c) Only CPU time
   - d) Only memory

5. How do you avoid boxing in collections?
   - a) Use ArrayList
   - b) Use List<T>
   - c) Use object[]
   - d) Boxing is unavoidable

## Short Answer (4.5 pts)

6. (1.5 pts) Explain the difference between `ArrayList` and `List<T>` in terms of boxing. Why is `List<T>` preferred?

7. (1.5 pts) How does boxing affect performance in a loop that runs 1 million times?

8. (1.5 pts) What is BenchmarkDotNet and why would you use it?

## Code Analysis (1.5 pts)

9. Identify all boxing operations:
```csharp
int x = 100;
object obj1 = x;          // Line 1
int y = (int)obj1;        // Line 2
Console.WriteLine(x);     // Line 3
ArrayList list = new();
list.Add(x);              // Line 4
List<int> generic = new();
generic.Add(x);           // Line 5
```

## Answer Key

1. **b** | 2. **b** | 3. **b** | 4. **b** | 5. **b**

6. `ArrayList` stores `object`, boxing value types. `List<T>` uses generics, no boxing for value types. `List<int>` stores ints directly, faster and less memory.

7. 1M boxing operations = 1M heap allocations = high GC pressure = pauses = poor performance. Can slow by 10-100x.

8. BenchmarkDotNet is a performance testing library that provides accurate measurements of code execution time, memory allocations, and GC collections.

9. Boxing occurs at: Line 1 (int → object), Line 3 (int → object for WriteLine), Line 4 (int → object for ArrayList). No boxing at Line 2 (unboxing) or Line 5 (generic).

**Resources**: `samples/02-Intermediate/BoxingUnboxing/`, `samples/03-Advanced/PerformanceBenchmarks/`
