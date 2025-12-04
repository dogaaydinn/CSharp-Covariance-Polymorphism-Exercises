# Week 5 Assessment Test - LINQ Fundamentals

**Week**: 5 | **Duration**: 35 min | **Pass**: 70% | **Points**: 10

## Multiple Choice (5 pts)

1. What does LINQ stand for?
   - a) Language Integrated Query
   - b) Linear Query
   - c) List Query
   - d) Linked Query

2. What is deferred execution in LINQ?
   - a) Query executes immediately
   - b) Query executes when results are enumerated
   - c) Query never executes
   - d) Query executes in background

3. Which forces immediate execution?
   - a) Where()
   - b) Select()
   - c) ToList()
   - d) OrderBy()

4. What does `Select()` do?
   - a) Filters elements
   - b) Sorts elements
   - c) Projects/transforms elements
   - d) Groups elements

5. Query vs Method syntax: which is true?
   - a) Query syntax is more powerful
   - b) Method syntax is more powerful
   - c) They're equivalent, method syntax more flexible
   - d) They use different execution models

## Short Answer (4.5 pts)

6. (1.5 pts) Write LINQ to get products where price > 50, ordered by name, select only names.

7. (1.5 pts) Explain the difference between `First()` and `FirstOrDefault()`. When would each throw an exception?

8. (1.5 pts) What are anonymous types in LINQ? Give an example.

## Code Analysis (1.5 pts)

9. What's the output and why?
```csharp
var numbers = new List<int> { 1, 2, 3 };
var query = numbers.Where(n => n > 1);
numbers.Add(4);
numbers.Add(5);
foreach (var n in query) Console.Write(n + " ");
```

## Answer Key

1. **a** | 2. **b** | 3. **c** | 4. **c** | 5. **c**

6. `products.Where(p => p.Price > 50).OrderBy(p => p.Name).Select(p => p.Name)`

7. `First()` throws if empty, `FirstOrDefault()` returns default (null/0). Use First when data must exist, FirstOrDefault when it might not.

8. Anonymous types are compiler-generated types for projections: `select new { p.Name, p.Price }` creates type with Name and Price properties.

9. **Output**: `2 3 4 5` - Deferred execution means query sees all numbers including newly added 4 and 5 when enumerated.

**Resources**: `samples/99-Exercises/LINQ/01-BasicQueries/`, LEARNING_PATHS.md Month 2 Week 1
