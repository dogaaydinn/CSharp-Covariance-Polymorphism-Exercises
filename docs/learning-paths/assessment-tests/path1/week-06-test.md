# Week 6 Assessment Test - LINQ Aggregations and Grouping

**Week**: 6 | **Duration**: 40 min | **Pass**: 70% | **Points**: 10

## Multiple Choice (5 pts)

1. What does `GroupBy()` return?
   - a) List<T>
   - b) IEnumerable<IGrouping<TKey, TElement>>
   - c) Dictionary<TKey, TValue>
   - d) Array

2. Which aggregation returns a single value?
   - a) Select()
   - b) Where()
   - c) Sum()
   - d) GroupBy()

3. What's the difference between `Count()` and `LongCount()`?
   - a) No difference
   - b) LongCount for collections > 2 billion
   - c) Count is faster
   - d) LongCount is for strings

4. How do you access grouped elements?
   - a) group.Elements
   - b) group.Items
   - c) group (itself is IEnumerable)
   - d) group.Values

5. What does `ToDictionary()` require?
   - a) Unique keys
   - b) Sorted data
   - c) Numeric keys
   - d) String keys

## Short Answer (4.5 pts)

6. (1.5 pts) Write LINQ to group products by category and get count per category.

7. (1.5 pts) How would you calculate average price per category using GroupBy?

8. (1.5 pts) Explain the difference between `ToDictionary()` and `ToLookup()`.

## Code Analysis (1.5 pts)

9. Complete this code to get top 3 categories by total sales:
```csharp
var result = sales
    .GroupBy(s => s.Category)
    // Your code here
```

## Answer Key

1. **b** | 2. **c** | 3. **b** | 4. **c** | 5. **a**

6. `products.GroupBy(p => p.Category).Select(g => new { Category = g.Key, Count = g.Count() })`

7. `products.GroupBy(p => p.Category).Select(g => new { Category = g.Key, AvgPrice = g.Average(p => p.Price) })`

8. `ToDictionary()` throws on duplicate keys, one value per key. `ToLookup()` allows multiple values per key, no exception.

9.
```csharp
.Select(g => new { Category = g.Key, TotalSales = g.Sum(s => s.Amount) })
.OrderByDescending(x => x.TotalSales)
.Take(3)
```

**Resources**: `samples/99-Exercises/LINQ/02-GroupingAggregation/`
