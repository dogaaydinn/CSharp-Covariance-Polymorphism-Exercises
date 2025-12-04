# Month 4 Capstone: Algorithm Visualization & Education Tool

**Difficulty**: ⭐⭐⭐⭐☆ (Advanced)
**Estimated Time**: 30-35 hours
**Prerequisites**: Completed Week 13-16 of Path 1 (Algorithms & Data Structures)

---

## 🎯 Project Overview

Build a comprehensive algorithm visualization and benchmarking tool that demonstrates sorting/searching algorithms with step-by-step execution, performance comparisons, and educational content.

### Learning Objectives

- ✅ Implement 10+ algorithms from scratch
- ✅ Analyze time/space complexity
- ✅ Use BenchmarkDotNet for performance testing
- ✅ Create step-by-step visualizations
- ✅ Implement custom data structures

---

## 📋 Requirements

### 1. Algorithms to Implement

**Searching** (3):
- Binary Search (iterative & recursive)
- Modified Binary Search (first/last occurrence)
- Interpolation Search

**Sorting** (7):
- QuickSort (Lomuto partition)
- QuickSort (Hoare partition)
- MergeSort (top-down)
- MergeSort (bottom-up)
- HeapSort
- InsertionSort
- BubbleSort (for comparison)

**Data Structures** (4):
- Stack<T> from scratch
- Queue<T> from scratch
- LinkedList<T> from scratch
- PriorityQueue<T>

### 2. Visualization Features

- Step-by-step execution mode
- Show array state after each step
- Highlight current elements being compared
- Count comparisons and swaps
- Execution time tracking
- Memory usage tracking

### 3. Educational Content

- Algorithm explanation for each
- Time complexity (best/average/worst)
- Space complexity
- When to use each algorithm
- Pros and cons
- Real-world applications

### 4. Benchmarking

- Test with different input sizes (100, 1K, 10K, 100K)
- Test with different input types (sorted, reverse, random, nearly sorted)
- Generate comparison charts (console table format)
- Export results to CSV

### 5. Console Interface

```
=== ALGORITHM VISUALIZER ===
1. Searching Algorithms
2. Sorting Algorithms
3. Data Structures Demo
4. Run Benchmarks
5. Compare Algorithms
6. Educational Mode
7. Exit

Choose algorithm:
1. Binary Search
2. QuickSort
3. MergeSort
...

Choose visualization speed:
1. Slow (1 step/second)
2. Medium (5 steps/second)
3. Fast (10 steps/second)
4. Instant (no delay)
```

---

## 🏗️ Project Structure

```
AlgorithmVisualizer/
├── Algorithms/
│   ├── Searching/
│   │   ├── BinarySearch.cs
│   │   ├── InterpolationSearch.cs
│   │   └── ModifiedBinarySearch.cs
│   ├── Sorting/
│   │   ├── QuickSort.cs
│   │   ├── MergeSort.cs
│   │   ├── HeapSort.cs
│   │   └── Others.cs
│   └── IAlgorithm.cs
├── DataStructures/
│   ├── CustomStack.cs
│   ├── CustomQueue.cs
│   ├── CustomLinkedList.cs
│   └── PriorityQueue.cs
├── Visualization/
│   ├── StepRecorder.cs
│   ├── ConsoleVisualizer.cs
│   └── AlgorithmStep.cs
├── Benchmarks/
│   ├── SortingBenchmarks.cs
│   ├── SearchingBenchmarks.cs
│   └── BenchmarkRunner.cs
├── Education/
│   ├── AlgorithmInfo.cs
│   └── ComplexityAnalyzer.cs
├── Utilities/
│   ├── DataGenerator.cs
│   └── PerformanceMetrics.cs
└── Tests/
    ├── AlgorithmTests.cs
    └── DataStructureTests.cs
```

---

## 🚀 Implementation Guide

### Step 1: Define Algorithm Interface

```csharp
public interface IAlgorithm<T> where T : IComparable<T>
{
    string Name { get; }
    string Description { get; }
    ComplexityInfo Complexity { get; }

    void Execute(T[] data);
    IEnumerable<AlgorithmStep<T>> ExecuteWithSteps(T[] data);
}

public class ComplexityInfo
{
    public string TimeComplexityBest { get; set; }
    public string TimeComplexityAverage { get; set; }
    public string TimeComplexityWorst { get; set; }
    public string SpaceComplexity { get; set; }
    public bool IsStable { get; set; }
    public bool IsInPlace { get; set; }
}

public class AlgorithmStep<T>
{
    public int StepNumber { get; set; }
    public string Description { get; set; }
    public T[] ArrayState { get; set; }
    public int[] HighlightedIndices { get; set; }
    public int Comparisons { get; set; }
    public int Swaps { get; set; }
}
```

### Step 2: Implement QuickSort with Visualization

```csharp
public class QuickSort<T> : IAlgorithm<T> where T : IComparable<T>
{
    private List<AlgorithmStep<T>> _steps;
    private int _comparisons;
    private int _swaps;

    public string Name => "QuickSort (Lomuto Partition)";

    public ComplexityInfo Complexity => new()
    {
        TimeComplexityBest = "O(n log n)",
        TimeComplexityAverage = "O(n log n)",
        TimeComplexityWorst = "O(n²)",
        SpaceComplexity = "O(log n)",
        IsStable = false,
        IsInPlace = true
    };

    public void Execute(T[] data)
    {
        QuickSortRecursive(data, 0, data.Length - 1);
    }

    public IEnumerable<AlgorithmStep<T>> ExecuteWithSteps(T[] data)
    {
        _steps = new List<AlgorithmStep<T>>();
        _comparisons = 0;
        _swaps = 0;

        RecordStep(data, "Initial array", Array.Empty<int>());
        QuickSortRecursive(data, 0, data.Length - 1);
        RecordStep(data, "Final sorted array", Array.Empty<int>());

        return _steps;
    }

    private void QuickSortRecursive(T[] arr, int low, int high)
    {
        if (low < high)
        {
            int pivotIndex = Partition(arr, low, high);
            RecordStep(arr, $"Partition complete, pivot at {pivotIndex}", new[] { pivotIndex });

            QuickSortRecursive(arr, low, pivotIndex - 1);
            QuickSortRecursive(arr, pivotIndex + 1, high);
        }
    }

    private int Partition(T[] arr, int low, int high)
    {
        T pivot = arr[high];
        int i = low - 1;

        for (int j = low; j < high; j++)
        {
            _comparisons++;
            if (arr[j].CompareTo(pivot) <= 0)
            {
                i++;
                Swap(arr, i, j);
                _swaps++;
                RecordStep(arr, $"Swapped {arr[i]} and {arr[j]}", new[] { i, j });
            }
        }

        Swap(arr, i + 1, high);
        _swaps++;
        return i + 1;
    }

    private void Swap(T[] arr, int i, int j)
    {
        (arr[i], arr[j]) = (arr[j], arr[i]);
    }

    private void RecordStep(T[] arr, string description, int[] highlightedIndices)
    {
        _steps.Add(new AlgorithmStep<T>
        {
            StepNumber = _steps.Count + 1,
            Description = description,
            ArrayState = (T[])arr.Clone(),
            HighlightedIndices = highlightedIndices,
            Comparisons = _comparisons,
            Swaps = _swaps
        });
    }
}
```

### Step 3: Implement Visualization

```csharp
public class ConsoleVisualizer
{
    public void Visualize<T>(IEnumerable<AlgorithmStep<T>> steps, int delayMs = 500)
    {
        foreach (var step in steps)
        {
            Console.Clear();
            Console.WriteLine($"Step {step.StepNumber}: {step.Description}");
            Console.WriteLine($"Comparisons: {step.Comparisons} | Swaps: {step.Swaps}");
            Console.WriteLine();

            DisplayArray(step.ArrayState, step.HighlightedIndices);

            Thread.Sleep(delayMs);
        }
    }

    private void DisplayArray<T>(T[] array, int[] highlightedIndices)
    {
        for (int i = 0; i < array.Length; i++)
        {
            if (highlightedIndices.Contains(i))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write($"[{array[i]}] ");
                Console.ResetColor();
            }
            else
            {
                Console.Write($"{array[i]} ");
            }
        }
        Console.WriteLine();
    }
}
```

### Step 4: Implement Benchmarks

```csharp
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

[MemoryDiagnoser]
public class SortingBenchmarks
{
    private int[] _data;

    [Params(100, 1000, 10000)]
    public int N;

    [GlobalSetup]
    public void Setup()
    {
        _data = DataGenerator.GenerateRandomArray(N);
    }

    [Benchmark]
    public void QuickSort_Lomuto()
    {
        var arr = (int[])_data.Clone();
        new QuickSort<int>().Execute(arr);
    }

    [Benchmark]
    public void MergeSort_TopDown()
    {
        var arr = (int[])_data.Clone();
        new MergeSort<int>().Execute(arr);
    }

    // TODO: Add more benchmarks
}
```

---

## 🎯 Milestones

1. **Day 1-5**: Implement all algorithms
2. **Day 6-7**: Add visualization system
3. **Day 8-9**: Implement custom data structures
4. **Day 10-11**: Add benchmarking
5. **Day 12-13**: Educational content
6. **Day 14**: Testing and polish

---

## ✅ Evaluation Criteria

| Criteria | Points |
|----------|--------|
| Algorithms Implemented (10+) | 30 |
| Visualization System | 20 |
| Benchmarks | 15 |
| Data Structures | 15 |
| Educational Content | 10 |
| Tests | 10 |
| **TOTAL** | **100** |

---

## 📚 Resources

- `samples/99-Exercises/Algorithms/` (all 3 exercises)
- BenchmarkDotNet: https://benchmarkdotnet.org/
- Algorithm visualizations: https://visualgo.net/

---

*Template Version: 1.0*
