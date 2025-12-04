# LINQ: Grouping & Aggregation Exercise

## 📚 Learning Objectives

By completing this exercise, you will learn:
- **Grouping** with `GroupBy()` method
- **Aggregation functions**: `Count()`, `Sum()`, `Average()`, `Min()`, `Max()`
- Working with `IGrouping<TKey, TElement>` interface
- Converting groups to dictionaries with `ToDictionary()`
- Creating complex statistical summaries
- Combining grouping with ordering and filtering

## 🎯 Exercise Tasks

You need to complete **5 TODO methods** in `Program.cs`:

1. ✅ **GroupByCategory()** - Basic grouping by category
2. ✅ **CalculateAveragePricePerCategory()** - Group and aggregate to dictionary
3. ✅ **GetCategoryStats()** - Complex multi-aggregation per group
4. ✅ **GetTopCategoriesByTotalValue()** - Group, aggregate, order, and take top N
5. ✅ **CountProductsBySupplier()** - Group and count

## 🚀 Getting Started

### Step 1: Navigate to the Project
```bash
cd samples/99-Exercises/LINQ/02-GroupingAggregation
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

### TODO 1: GroupByCategory

**Goal**: Group all products by their category.

**Hint**: The `GroupBy()` method takes a key selector and returns `IEnumerable<IGrouping<TKey, TElement>>`.

```csharp
// Solution approach:
return products.GroupBy(p => p.Category);
```

**What is IGrouping?**
- `IGrouping<string, Product>` represents a group with a `Key` (category name) and collection of `Product` items
- You can iterate over each group to access its products
- Use `.Key` property to get the category name

### TODO 2: CalculateAveragePricePerCategory

**Goal**: Calculate the average price for each category and return as a dictionary.

**Hint**: Combine `GroupBy()`, `ToDictionary()`, and `Average()`.

```csharp
// Solution approach:
return products
    .GroupBy(p => p.Category)
    .ToDictionary(
        g => g.Key,                    // Dictionary key: category name
        g => g.Average(p => p.Price)   // Dictionary value: average price
    );
```

**Key concepts**:
- `g.Key` is the category name
- `g.Average(p => p.Price)` calculates average of all prices in that group
- `ToDictionary()` converts the grouped data to a dictionary

### TODO 3: GetCategoryStats

**Goal**: Create comprehensive statistics for each category with multiple aggregations.

**Hint**: After grouping, use `Select()` to create `CategoryStats` objects with multiple aggregation functions.

```csharp
// Solution approach:
return products
    .GroupBy(p => p.Category)
    .Select(g => new CategoryStats
    {
        Category = g.Key,
        ProductCount = g.Count(),
        TotalValue = g.Sum(p => p.Price * p.StockQuantity),
        AveragePrice = g.Average(p => p.Price),
        MinPrice = g.Min(p => p.Price),
        MaxPrice = g.Max(p => p.Price)
    })
    .ToList();
```

**Aggregation functions**:
- `Count()` - number of items in group
- `Sum()` - sum of values
- `Average()` - mean of values
- `Min()` - smallest value
- `Max()` - largest value

### TODO 4: GetTopCategoriesByTotalValue

**Goal**: Find the top N categories by total inventory value (Price × StockQuantity).

**Hint**: Group, calculate sum, order descending, take N, select category names.

```csharp
// Solution approach:
return products
    .GroupBy(p => p.Category)
    .Select(g => new
    {
        Category = g.Key,
        TotalValue = g.Sum(p => p.Price * p.StockQuantity)
    })
    .OrderByDescending(x => x.TotalValue)
    .Take(topN)
    .Select(x => x.Category)
    .ToList();
```

**Key steps**:
1. Group by category
2. Calculate total value for each group
3. Order by total value (descending)
4. Take top N results
5. Extract just the category name

### TODO 5: CountProductsBySupplier

**Goal**: Count how many products each supplier provides.

**Hint**: Similar to TODO 2, but use `Count()` instead of `Average()`.

```csharp
// Solution approach:
return products
    .GroupBy(p => p.Supplier)
    .ToDictionary(
        g => g.Key,        // Dictionary key: supplier name
        g => g.Count()     // Dictionary value: product count
    );
```

## ✅ Success Criteria

All tests should pass:
- [ ] `GroupByCategory_ShouldReturnGroupsForEachCategory` ✅
- [ ] `GroupByCategory_ShouldHaveCorrectProductCounts` ✅
- [ ] `GroupByCategory_ShouldContainCorrectProducts` ✅
- [ ] `CalculateAveragePricePerCategory_ShouldReturnCorrectAverages` ✅
- [ ] `CalculateAveragePricePerCategory_ShouldReturnDictionary` ✅
- [ ] `GetCategoryStats_ShouldReturnStatsForAllCategories` ✅
- [ ] `GetCategoryStats_ElectronicsShouldHaveCorrectStats` ✅
- [ ] `GetCategoryStats_FurnatureShouldHaveCorrectStats` ✅
- [ ] `GetCategoryStats_StationeryShouldHaveCorrectStats` ✅
- [ ] `GetTopCategoriesByTotalValue_ShouldReturnTop2Categories` ✅
- [ ] `GetTopCategoriesByTotalValue_ShouldReturnAllWhenTopNIsLarge` ✅
- [ ] `GetTopCategoriesByTotalValue_ShouldReturnTop1` ✅
- [ ] `CountProductsBySupplier_ShouldReturnCorrectCounts` ✅
- [ ] `CountProductsBySupplier_ShouldReturnDictionary` ✅
- [ ] `GroupByCategory_ShouldHandleEmptyList` ✅
- [ ] `CalculateAveragePricePerCategory_ShouldHandleEmptyList` ✅
- [ ] `GetCategoryStats_ShouldHandleSingleCategory` ✅

**Total: 17 tests must pass!**

## 📖 LINQ Methods Reference

### GroupBy()
Groups elements by a specified key.
```csharp
var grouped = products.GroupBy(p => p.Category);
// Returns IEnumerable<IGrouping<string, Product>>

foreach (var group in grouped)
{
    Console.WriteLine($"Category: {group.Key}");
    Console.WriteLine($"Count: {group.Count()}");
    foreach (var product in group)
    {
        Console.WriteLine($"  - {product.Name}");
    }
}
```

### Aggregation Functions

#### Count()
Counts the number of elements.
```csharp
int count = products.Count();
int electronicsCount = products.Count(p => p.Category == "Electronics");
```

#### Sum()
Calculates the sum of values.
```csharp
decimal totalPrice = products.Sum(p => p.Price);
decimal totalValue = products.Sum(p => p.Price * p.StockQuantity);
```

#### Average()
Calculates the mean of values.
```csharp
decimal avgPrice = products.Average(p => p.Price);
```

#### Min() / Max()
Finds the minimum or maximum value.
```csharp
decimal minPrice = products.Min(p => p.Price);
decimal maxPrice = products.Max(p => p.Price);
```

### ToDictionary()
Converts a sequence to a dictionary.
```csharp
var dict = products
    .GroupBy(p => p.Category)
    .ToDictionary(
        g => g.Key,                  // Key selector
        g => g.Average(p => p.Price) // Value selector
    );
```

### Anonymous Types
Create temporary objects without defining a class.
```csharp
var summary = products.Select(p => new
{
    Name = p.Name,
    Price = p.Price,
    TotalValue = p.Price * p.StockQuantity
});
```

## 🎓 What You'll Learn

### Understanding IGrouping
```csharp
IEnumerable<IGrouping<string, Product>> groups = products.GroupBy(p => p.Category);

foreach (IGrouping<string, Product> group in groups)
{
    string categoryName = group.Key;              // Access the key
    int productCount = group.Count();             // Count items in group
    decimal avgPrice = group.Average(p => p.Price); // Aggregate within group
}
```

### Multiple Aggregations
```csharp
var stats = products
    .GroupBy(p => p.Category)
    .Select(g => new
    {
        Category = g.Key,
        Count = g.Count(),
        Sum = g.Sum(p => p.Price),
        Avg = g.Average(p => p.Price),
        Min = g.Min(p => p.Price),
        Max = g.Max(p => p.Price)
    });
```

### Chaining Operations
```csharp
var topCategories = products
    .GroupBy(p => p.Category)               // 1. Group
    .Select(g => new { Category = g.Key, Total = g.Sum(...) }) // 2. Aggregate
    .OrderByDescending(x => x.Total)        // 3. Order
    .Take(3)                                 // 4. Take top 3
    .Select(x => x.Category)                // 5. Project to names
    .ToList();                               // 6. Execute
```

## 🐛 Common Mistakes to Avoid

1. **Forgetting that GroupBy returns IGrouping**:
   ```csharp
   // ❌ Wrong - trying to access Product properties directly
   var groups = products.GroupBy(p => p.Category);
   var firstPrice = groups.First().Price; // ERROR!

   // ✅ Correct - IGrouping is a collection
   var firstGroup = groups.First();
   var firstPrice = firstGroup.First().Price;
   ```

2. **Not materializing with ToList()**:
   ```csharp
   // ❌ Wrong - returns IEnumerable, may cause multiple enumerations
   return products.GroupBy(p => p.Category);

   // ✅ Correct - materializes immediately
   return products.GroupBy(p => p.Category).ToList();
   ```

3. **Confusing Sum and Count**:
   ```csharp
   // ❌ Wrong - Count() doesn't take a parameter
   g.Count(p => p.Price); // ERROR!

   // ✅ Correct
   g.Count()              // Number of items
   g.Sum(p => p.Price)    // Sum of prices
   ```

4. **Forgetting to use ToDictionary() for dictionary results**:
   ```csharp
   // ❌ Wrong - returns IEnumerable<IGrouping>, not Dictionary
   return products.GroupBy(p => p.Category);

   // ✅ Correct - converts to Dictionary
   return products.GroupBy(p => p.Category).ToDictionary(g => g.Key, g => g.Count());
   ```

## 🚀 Challenge: Bonus Tasks

Once you complete all TODOs, try these bonus challenges:

1. **GetProductsWithAboveAveragePriceInCategory()**: For each category, find products with above-average price
   - Hint: Group by category, calculate average per group, then filter

2. **GetSuppliersWithMostProducts()**: Find suppliers that provide more than N products
   - Hint: Group by supplier, count, filter Where(g => g.Count() > N)

3. **GetCategoryWithHighestAveragePrice()**: Find which category has the highest average price
   - Hint: Group, calculate averages, order descending, take first

4. **GetMonthlyProductAdditionStats()**: Group products by the month they were added
   - Hint: Use `AddedDate.Month` or `AddedDate.ToString("yyyy-MM")` as grouping key

## 📚 Additional Resources

- [LINQ GroupBy Documentation](https://learn.microsoft.com/en-us/dotnet/api/system.linq.enumerable.groupby)
- [Aggregation Operations](https://learn.microsoft.com/en-us/dotnet/csharp/programming-guide/concepts/linq/aggregation-operations)
- [Anonymous Types](https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/types/anonymous-types)
- [Working with IGrouping](https://learn.microsoft.com/en-us/dotnet/api/system.linq.igrouping-2)

---

**Good luck! 🎉**

*Once you complete all TODOs, check `SOLUTION.md` for the complete solutions (but try to solve it yourself first!).*
