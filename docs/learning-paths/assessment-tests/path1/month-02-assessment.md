# Month 2 Comprehensive Assessment - LINQ Mastery

**Month**: 2 (Weeks 5-8) | **Duration**: 75 min | **Pass**: 80% (20/25) | **Points**: 25

## Section 1: Multiple Choice (12 questions, 0.5 pts each = 6 pts)

1. Deferred execution means:
   - a) Query never executes | b) Query executes when enumerated | c) Query executes immediately | d) Query is delayed

2. Which forces immediate execution?
   - a) Where() | b) Select() | c) ToList() | d) OrderBy()

3. IGrouping<TKey, TElement> represents:
   - a) Dictionary | b) Group with key and elements | c) Array | d) Set

4. Left outer join requires:
   - a) Join() | b) GroupJoin() + SelectMany() + DefaultIfEmpty() | c) LeftJoin() | d) Merge()

5. What does `Average()` return for empty collection?
   - a) 0 | b) null | c) Exception | d) NaN

6. `ToDictionary()` on duplicate keys:
   - a) Keeps first | b) Keeps last | c) Throws exception | d) Creates list

7. Anonymous types are:
   - a) Named types | b) Compiler-generated types | c) Generic types | d) Abstract types

8. Func<int, bool> represents:
   - a) Function taking bool returning int | b) Function taking int returning bool | c) Action | d) Predicate only

9. Closure captures:
   - a) Nothing | b) Local variables from outer scope | c) Only parameters | d) Global variables

10. Higher-order function:
    - a) Fast function | b) Function taking/returning functions | c) Static function | d) Abstract function

11. `SelectMany()` does:
    - a) Filters | b) Flattens nested collections | c) Projects | d) Groups

12. Query syntax vs method syntax:
    - a) Query more powerful | b) Method more powerful | c) Equivalent, method more flexible | d) Different execution

## Section 2: Short Answer (5 questions, 2 pts each = 10 pts)

13. Write LINQ to get products where price > 100, group by category, calculate average price per category, order by average descending.

14. Explain deferred execution and its implications. Give example where it causes unexpected behavior.

15. Implement left outer join between Products and Orders tables. Show products with no orders.

16. What's a closure? Write code demonstrating closure problem in a loop and its solution.

17. Compare `First()`, `FirstOrDefault()`, `Single()`, `SingleOrDefault()`. When does each throw?

## Section 3: Code Implementation (3 questions, 3 pts each = 9 pts)

18. Implement `Map()`, `Filter()`, `Reduce()` extension methods from scratch (functional programming primitives).

19. Write query to find top 5 customers by total order value, including customer name, total value, order count.

20. Fix this deferred execution bug:
```csharp
var numbers = new List<int> { 1, 2, 3 };
var query = numbers.Where(n => n > threshold);
threshold = 5; // Bug: threshold changed after query created
foreach (var n in query) Console.WriteLine(n);
```

## Answer Key (Summary)

**MC**: 1.b | 2.c | 3.b | 4.b | 5.c | 6.c | 7.b | 8.b | 9.b | 10.b | 11.b | 12.c

**13**:
```csharp
products
    .Where(p => p.Price > 100)
    .GroupBy(p => p.Category)
    .Select(g => new { Category = g.Key, AvgPrice = g.Average(p => p.Price) })
    .OrderByDescending(x => x.AvgPrice)
```

**14**: Deferred execution delays query until enumeration. Unexpected: If source changes after query created but before enumeration, query sees new data. Solution: Force with ToList().

**15**:
```csharp
products.GroupJoin(
    orders,
    p => p.Id,
    o => o.ProductId,
    (p, orders) => new { Product = p, Orders = orders }
).SelectMany(
    x => x.Orders.DefaultIfEmpty(),
    (p, o) => new { p.Product.Name, OrderId = o?.Id ?? 0 }
)
```

**16**: Closure captures variable reference. Loop bug:
```csharp
// Bug
for (int i = 0; i < 5; i++)
    actions.Add(() => Console.WriteLine(i)); // All print 5!

// Fix
for (int i = 0; i < 5; i++) {
    int copy = i;
    actions.Add(() => Console.WriteLine(copy)); // Each prints correct value
}
```

**17**:
- First(): Throws if empty
- FirstOrDefault(): Returns default if empty
- Single(): Throws if empty OR >1 element
- SingleOrDefault(): Throws if >1 element, returns default if empty

**18**:
```csharp
public static IEnumerable<TResult> Map<T, TResult>(
    this IEnumerable<T> source, Func<T, TResult> selector)
{
    foreach (var item in source)
        yield return selector(item);
}

public static IEnumerable<T> Filter<T>(
    this IEnumerable<T> source, Func<T, bool> predicate)
{
    foreach (var item in source)
        if (predicate(item))
            yield return item;
}

public static TResult Reduce<T, TResult>(
    this IEnumerable<T> source, TResult seed, Func<TResult, T, TResult> func)
{
    var result = seed;
    foreach (var item in source)
        result = func(result, item);
    return result;
}
```

**19**:
```csharp
orders
    .GroupBy(o => o.CustomerId)
    .Select(g => new {
        CustomerId = g.Key,
        TotalValue = g.Sum(o => o.Amount),
        OrderCount = g.Count()
    })
    .Join(customers, x => x.CustomerId, c => c.Id, (x, c) => new { c.Name, x.TotalValue, x.OrderCount })
    .OrderByDescending(x => x.TotalValue)
    .Take(5)
```

**20**: Fix - capture threshold value:
```csharp
var threshold = 2;
var thresholdCopy = threshold; // Capture current value
var query = numbers.Where(n => n > thresholdCopy);
threshold = 5; // Now doesn't affect query
```

**Resources**: `samples/99-Exercises/LINQ/` (all 3 exercises)
