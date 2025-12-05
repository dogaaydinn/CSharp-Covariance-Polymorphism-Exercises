# Dynamic Programming

## Problems

### 1. Fibonacci with Memoization
```csharp
Fib(10) => 55
Without DP: O(2^n), exponential time
With DP: O(n), linear time
```

### 2. Longest Common Subsequence
```csharp
LCS("ABCDGH", "AEDFHR") => "ADH" (length 3)
```

### 3. 0/1 Knapsack
```csharp
Items: [(weight: 2, value: 3), (weight: 3, value: 4), (weight: 4, value: 5)]
Capacity: 5
Max value: 7 (items 1 and 2)
```

## Key Technique
- Identify overlapping subproblems
- Store results (memoization or tabulation)
- Build solution from smaller problems
