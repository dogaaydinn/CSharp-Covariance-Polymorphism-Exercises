# LINQ: Basic Queries Exercise

## 📚 Learning Objectives

By completing this exercise, you will learn:
- **Filtering** with `Where()` method
- **Ordering** with `OrderBy()`, `ThenBy()`, and `OrderByDescending()`
- **Projection** with `Select()` method
- **Method chaining** in LINQ
- **Anonymous types** for data transformation

## 🎯 Exercise Tasks

You need to complete **6 TODO methods** in `Program.cs`:

1. ✅ **GetExpensiveProducts()** - Filter products with price > 100
2. ✅ **GetInStockProducts()** - Filter products that are in stock AND active
3. ✅ **OrderByCategoryThenPrice()** - Multi-level sorting
4. ✅ **OrderByMostRecent()** - Sort by date descending
5. ✅ **GetProductNames()** - Project to list of strings
6. ✅ **GetProductSummaries()** - Project to anonymous types

## 🚀 Getting Started

### Step 1: Open the Project
```bash
cd samples/99-Exercises/LINQ/01-BasicQueries
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

## 💡 Hints and Tips

### TODO 1: GetExpensiveProducts
```csharp
// HINT: Use Where() with a lambda expression
return products.Where(p => p.Price > 100).ToList();
```

### TODO 2: GetInStockProducts
```csharp
// HINT: Combine multiple conditions with &&
return products.Where(p => p.StockQuantity > 0 && p.IsActive).ToList();
```

### TODO 3: OrderByCategoryThenPrice
```csharp
// HINT: Chain OrderBy() and ThenBy()
return products
    .OrderBy(p => p.Category)
    .ThenBy(p => p.Price)
    .ToList();
```

### TODO 4: OrderByMostRecent
```csharp
// HINT: Use OrderByDescending() for newest first
return products.OrderByDescending(p => p.AddedDate).ToList();
```

### TODO 5: GetProductNames
```csharp
// HINT: Select() extracts a single property
return products.Select(p => p.Name).ToList();
```

### TODO 6: GetProductSummaries
```csharp
// HINT: Create anonymous type with new { ... }
return products
    .Select(p => new { Name = p.Name, Price = p.Price })
    .Cast<object>()
    .ToList();
```

## ✅ Success Criteria

All tests should pass:
- [ ] `GetExpensiveProducts_ShouldReturnOnlyProductsOver100` ✅
- [ ] `GetExpensiveProducts_ShouldReturnEmptyList_WhenNoExpensiveProducts` ✅
- [ ] `GetInStockProducts_ShouldReturnOnlyInStockAndActiveProducts` ✅
- [ ] `OrderByCategoryThenPrice_ShouldOrderCorrectly` ✅
- [ ] `OrderByMostRecent_ShouldOrderByDateDescending` ✅
- [ ] `GetProductNames_ShouldReturnAllProductNames` ✅
- [ ] `GetProductNames_ShouldReturnEmptyList_WhenNoProducts` ✅
- [ ] `GetProductSummaries_ShouldReturnAnonymousTypesWithNameAndPrice` ✅
- [ ] `GetExpensiveProducts_ShouldHandleEmptyList` ✅
- [ ] `OrderByCategoryThenPrice_ShouldHandleSingleItem` ✅

**Total: 10 tests must pass!**

## 📖 LINQ Methods Reference

### Where()
Filters a sequence based on a predicate.
```csharp
var result = products.Where(p => p.Price > 100);
```

### OrderBy() / OrderByDescending()
Sorts elements in ascending/descending order.
```csharp
var result = products.OrderBy(p => p.Price);
var result2 = products.OrderByDescending(p => p.AddedDate);
```

### ThenBy() / ThenByDescending()
Performs a secondary sort (used after OrderBy).
```csharp
var result = products
    .OrderBy(p => p.Category)
    .ThenBy(p => p.Price);
```

### Select()
Projects each element into a new form.
```csharp
var names = products.Select(p => p.Name);
var summary = products.Select(p => new { p.Name, p.Price });
```

### ToList()
Executes the query and converts to List<T>.
```csharp
var result = products.Where(p => p.Price > 100).ToList();
```

## 🎓 What You'll Learn

- **Method Syntax**: Fluent API style LINQ queries
- **Lambda Expressions**: `p => p.Property` syntax
- **Deferred Execution**: Queries execute when enumerated
- **Method Chaining**: Building complex queries step by step
- **Type Inference**: C# compiler infers types automatically

## 🐛 Common Mistakes to Avoid

1. **Forgetting `.ToList()`**: LINQ queries are lazy, call `.ToList()` to execute
2. **Wrong comparison operators**: Use `>` not `>=` for "greater than"
3. **Using `=` instead of `==`**: Assignment vs. equality check
4. **Incorrect chaining**: `ThenBy()` must come after `OrderBy()`

## 🚀 Challenge: Bonus Tasks

Once you complete all TODOs, try these bonus challenges:

1. **GetTop3ExpensiveProducts()**: Return only the 3 most expensive products
   - Hint: Use `.Take(3)`

2. **GetProductsInPriceRange()**: Filter products between min and max price
   - Hint: Use `&&` with two conditions

3. **GetCategoriesWithProductCount()**: Group by category and count products
   - Hint: Use `.GroupBy()` and `.Select()`

## 📚 Additional Resources

- [LINQ Documentation](https://learn.microsoft.com/en-us/dotnet/csharp/linq/)
- [Lambda Expressions](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/operators/lambda-expressions)
- [LINQ Query Syntax vs Method Syntax](https://learn.microsoft.com/en-us/dotnet/csharp/programming-guide/concepts/linq/query-syntax-and-method-syntax-in-linq)

---

**Good luck! 🎉**

*Once you complete all TODOs, check `SOLUTION.md` for the complete solutions (but try to solve it yourself first!).*
