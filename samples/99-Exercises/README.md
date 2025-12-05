# 99-Exercises: Programming Practice Problems

Interactive coding exercises to master C# and software engineering fundamentals.

## 📚 Exercise Categories

### Algorithms (1-2)
1. **BinarySearch** - Search in sorted arrays (O(log n))
2. **QuickSort** - Efficient sorting algorithm

### Design Patterns (3-4)
3. **BuilderPattern** - Fluent object construction
4. **ObserverPattern** - Event-driven programming

### C# Features (5-9)
5. **LINQ-Queries** - Query data with LINQ
6. **Async-Await** - Asynchronous programming
7. **DependencyInjection** - IoC container patterns
8. **MiddlewarePipeline** - Request processing pipeline
9. **ExpressionTrees** - Dynamic LINQ queries

### Advanced Algorithms (10-12)
10. **DynamicProgramming** - Fibonacci, knapsack problems
11. **TreeTraversal** - DFS, BFS, in-order traversal
12. **GraphAlgorithms** - Shortest path, topological sort

### System Design (13-15)
13. **DesignTwitter** - Design a social media feed
14. **RateLimiter** - Implement token bucket algorithm
15. **CacheImplementation** - Build LRU cache

## 🎯 How to Use

Each exercise contains:
- `README.md` - Problem description and examples
- `Starter.cs` - Template code to complete
- `Tests.cs` - Unit tests to verify solution
- `Solution.cs` - Reference solution (spoilers!)

### Workflow

```bash
# 1. Read the problem
cat 01-BinarySearch/README.md

# 2. Complete the starter code
code 01-BinarySearch/Starter.cs

# 3. Run tests
dotnet test 01-BinarySearch/Tests.cs

# 4. Check solution if stuck
cat 01-BinarySearch/Solution.cs
```

## 📊 Difficulty Levels

| Exercise | Difficulty | Time | Key Concepts |
|----------|-----------|------|--------------|
| BinarySearch | ⭐ Easy | 15 min | Recursion, divide-and-conquer |
| QuickSort | ⭐⭐ Medium | 30 min | Partitioning, in-place sorting |
| BuilderPattern | ⭐ Easy | 20 min | Fluent API, method chaining |
| ObserverPattern | ⭐⭐ Medium | 30 min | Events, delegates |
| LINQ-Queries | ⭐ Easy | 15 min | Lambda expressions, LINQ |
| Async-Await | ⭐⭐ Medium | 30 min | Tasks, async/await |
| DependencyInjection | ⭐⭐⭐ Hard | 45 min | IoC, DI containers |
| MiddlewarePipeline | ⭐⭐ Medium | 30 min | Chain of responsibility |
| ExpressionTrees | ⭐⭐⭐⭐ Very Hard | 60 min | Reflection, dynamic queries |
| DynamicProgramming | ⭐⭐⭐ Hard | 45 min | Memoization, optimization |
| TreeTraversal | ⭐⭐ Medium | 30 min | Recursion, iterative traversal |
| GraphAlgorithms | ⭐⭐⭐ Hard | 45 min | BFS, DFS, Dijkstra |
| DesignTwitter | ⭐⭐⭐⭐ Very Hard | 60 min | System design, scalability |
| RateLimiter | ⭐⭐⭐ Hard | 45 min | Token bucket, rate limiting |
| CacheImplementation | ⭐⭐⭐ Hard | 45 min | LRU cache, hash map + linked list |

## 🎓 Learning Paths

### Path 1: Algorithms Fundamentals (2-3 hours)
- BinarySearch → QuickSort → TreeTraversal → GraphAlgorithms

### Path 2: C# Mastery (3-4 hours)
- LINQ-Queries → Async-Await → DependencyInjection → ExpressionTrees

### Path 3: Design Patterns (2-3 hours)
- BuilderPattern → ObserverPattern → MiddlewarePipeline

### Path 4: System Design (3-4 hours)
- RateLimiter → CacheImplementation → DesignTwitter → DynamicProgramming

## 🏆 Challenge Mode

Complete all exercises with:
- ✅ All tests passing
- ✅ O(n log n) or better time complexity (where applicable)
- ✅ No memory leaks
- ✅ Thread-safe implementations (for async exercises)

## 📚 Resources

- [LeetCode](https://leetcode.com) - More algorithm practice
- [Refactoring Guru](https://refactoring.guru/design-patterns) - Design patterns
- [System Design Primer](https://github.com/donnemartin/system-design-primer)

---

**Total**: 15 exercises with ~150 unit tests
