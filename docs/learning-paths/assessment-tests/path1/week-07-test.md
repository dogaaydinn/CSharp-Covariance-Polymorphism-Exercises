# Week 7 Assessment Test - LINQ Joins

**Week**: 7 | **Duration**: 45 min | **Pass**: 70% | **Points**: 10

## Multiple Choice (5 pts)

1. Inner join returns:
   - a) All left records
   - b) All right records
   - c) Only matching records from both
   - d) All records from both

2. How do you perform left outer join in LINQ?
   - a) Join()
   - b) GroupJoin() + SelectMany() + DefaultIfEmpty()
   - c) LeftJoin()
   - d) Merge()

3. What does `DefaultIfEmpty()` do in joins?
   - a) Returns empty collection
   - b) Returns null for non-matching right side
   - c) Throws exception
   - d) Skips non-matching records

4. Can you join more than 2 collections?
   - a) No, max 2
   - b) Yes, chain multiple joins
   - c) Only with special syntax
   - d) Only with SQL

5. Join performance consideration:
   - a) Always fast
   - b) O(n*m) for nested loops, O(n+m) with hash
   - c) Same as Where
   - d) No performance impact

## Short Answer (4.5 pts)

6. (1.5 pts) Write inner join between Products and Categories on CategoryId.

7. (1.5 pts) Explain when you'd use GroupJoin vs Join.

8. (1.5 pts) How would you join 3 tables: Orders, OrderDetails, Products?

## Code Analysis (1.5 pts)

9. Convert to left join:
```csharp
var result = products.Join(
    orders,
    p => p.Id,
    o => o.ProductId,
    (p, o) => new { p.Name, o.Quantity }
);
```

## Answer Key

1. **c** | 2. **b** | 3. **b** | 4. **b** | 5. **b**

6.
```csharp
products.Join(
    categories,
    p => p.CategoryId,
    c => c.Id,
    (p, c) => new { p.Name, Category = c.Name }
)
```

7. `Join` for 1:1 or M:1. `GroupJoin` for 1:M, creates hierarchical structure with groups.

8.
```csharp
orders
    .Join(orderDetails, o => o.Id, od => od.OrderId, (o, od) => new { o, od })
    .Join(products, x => x.od.ProductId, p => p.Id, (x, p) => new { x.o, x.od, p })
```

9.
```csharp
var result = products.GroupJoin(
    orders,
    p => p.Id,
    o => o.ProductId,
    (p, orders) => new { p, orders }
).SelectMany(
    x => x.orders.DefaultIfEmpty(),
    (p, o) => new { p.p.Name, Quantity = o?.Quantity ?? 0 }
);
```

**Resources**: `samples/99-Exercises/LINQ/03-Joins/`
