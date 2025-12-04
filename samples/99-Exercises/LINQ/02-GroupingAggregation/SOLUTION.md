# ⚠️ SPOILER WARNING ⚠️

**DO NOT READ THIS FILE UNTIL YOU'VE TRIED TO SOLVE THE EXERCISES YOURSELF!**

This file contains complete solutions to all TODO exercises. Try to complete them on your own first!

---

# LINQ Grouping & Aggregation - Complete Solutions

## TODO 1: GroupByCategory

```csharp
public static IEnumerable<IGrouping<string, Product>> GroupByCategory(List<Product> products)
{
    return products.GroupBy(p => p.Category);
}
```

**Explanation:**
- `GroupBy(p => p.Category)` groups products by their Category property
- Returns `IEnumerable<IGrouping<string, Product>>` where:
  - The key is `string` (category name)
  - Each group contains `Product` items
- `IGrouping<TKey, TElement>` is a special interface that has a `Key` property and is enumerable
- No `.ToList()` is needed here because the return type is `IEnumerable<IGrouping>`

**Usage example:**
```csharp
var groups = GroupByCategory(products);
foreach (var group in groups)
{
    Console.WriteLine($"{group.Key}: {group.Count()} products");
}
```

---

## TODO 2: CalculateAveragePricePerCategory

```csharp
public static Dictionary<string, decimal> CalculateAveragePricePerCategory(List<Product> products)
{
    return products
        .GroupBy(p => p.Category)
        .ToDictionary(
            g => g.Key,
            g => g.Average(p => p.Price)
        );
}
```

**Explanation:**
- First, group products by category
- Then convert to dictionary using `ToDictionary()`
- **Key selector**: `g => g.Key` extracts the category name from each group
- **Value selector**: `g => g.Average(p => p.Price)` calculates average price for products in that group
- `g` represents each `IGrouping<string, Product>` in the sequence

**Step-by-step breakdown:**
```csharp
// Step 1: Group
var groups = products.GroupBy(p => p.Category);
// Result: IEnumerable<IGrouping<string, Product>>

// Step 2: Convert to dictionary with averages
var dict = groups.ToDictionary(
    group => group.Key,                    // "Electronics", "Furniture", etc.
    group => group.Average(p => p.Price)   // Average price in each group
);
```

---

## TODO 3: GetCategoryStats

```csharp
public static List<CategoryStats> GetCategoryStats(List<Product> products)
{
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
}
```

**Explanation:**
- Group products by category
- Use `Select()` to transform each group into a `CategoryStats` object
- Perform multiple aggregations on each group:
  - `g.Count()` - number of products in the category
  - `g.Sum(p => p.Price * p.StockQuantity)` - total inventory value
  - `g.Average(p => p.Price)` - average price
  - `g.Min(p => p.Price)` - cheapest product price
  - `g.Max(p => p.Price)` - most expensive product price
- `.ToList()` materializes the query into a list

**Key insight**: You can call multiple aggregation functions on the same group within a single `Select()` projection.

**Example calculation for Electronics**:
```
Products: Laptop($1200), Mouse($25), Keyboard($75), Monitor($300), Tablet($600), Webcam($80)
- Count: 6
- Average: (1200 + 25 + 75 + 300 + 600 + 80) / 6 = 380
- Min: 25
- Max: 1200
- TotalValue: (1200*5) + (25*50) + (75*30) + (300*10) + (600*12) + (80*20) = 21,300
```

---

## TODO 4: GetTopCategoriesByTotalValue

```csharp
public static List<string> GetTopCategoriesByTotalValue(List<Product> products, int topN)
{
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
}
```

**Explanation:**
1. **Group by category**: Creates groups of products
2. **Select to anonymous type**: Create temporary objects with Category and TotalValue
3. **OrderByDescending**: Sort by TotalValue (highest first)
4. **Take(topN)**: Get only the top N results
5. **Select category name**: Extract just the category string
6. **ToList()**: Materialize the result

**Why use anonymous type?**
- We need both Category and TotalValue for sorting
- After sorting, we only want the Category names
- Anonymous types are perfect for this intermediate transformation

**Alternative without anonymous type** (less readable):
```csharp
return products
    .GroupBy(p => p.Category)
    .OrderByDescending(g => g.Sum(p => p.Price * p.StockQuantity))
    .Take(topN)
    .Select(g => g.Key)
    .ToList();
```

---

## TODO 5: CountProductsBySupplier

```csharp
public static Dictionary<string, int> CountProductsBySupplier(List<Product> products)
{
    return products
        .GroupBy(p => p.Supplier)
        .ToDictionary(
            g => g.Key,
            g => g.Count()
        );
}
```

**Explanation:**
- Group products by Supplier name
- Convert to dictionary where:
  - **Key**: Supplier name (string)
  - **Value**: Number of products (int)
- `g.Count()` returns the number of items in each group

**Example result**:
```csharp
{
    "TechCorp": 4,
    "FurniturePlus": 5,
    "OfficePro": 4,
    "DisplayCo": 2
}
```

---

## Alternative Solutions

### TODO 2 (Alternative with Query Syntax)
```csharp
public static Dictionary<string, decimal> CalculateAveragePricePerCategory(List<Product> products)
{
    return (from p in products
            group p by p.Category into g
            select new { Category = g.Key, Avg = g.Average(p => p.Price) })
           .ToDictionary(x => x.Category, x => x.Avg);
}
```

### TODO 3 (Alternative with Explicit Variable)
```csharp
public static List<CategoryStats> GetCategoryStats(List<Product> products)
{
    var grouped = products.GroupBy(p => p.Category);

    var stats = grouped.Select(g => new CategoryStats
    {
        Category = g.Key,
        ProductCount = g.Count(),
        TotalValue = g.Sum(p => p.Price * p.StockQuantity),
        AveragePrice = g.Average(p => p.Price),
        MinPrice = g.Min(p => p.Price),
        MaxPrice = g.Max(p => p.Price)
    });

    return stats.ToList();
}
```

### TODO 4 (Alternative with Let Clause in Query Syntax)
```csharp
public static List<string> GetTopCategoriesByTotalValue(List<Product> products, int topN)
{
    return (from p in products
            group p by p.Category into g
            let totalValue = g.Sum(p => p.Price * p.StockQuantity)
            orderby totalValue descending
            select g.Key)
           .Take(topN)
           .ToList();
}
```

---

## Complete Program.cs (All TODOs Solved)

```csharp
namespace GroupingAggregation;

public class Program
{
    public static void Main(string[] args)
    {
        Console.WriteLine("LINQ Grouping & Aggregation Exercise");
        Console.WriteLine("Run 'dotnet test' to check your solutions\n");

        var products = GetSampleProducts();

        Console.WriteLine("=== Sample Data ===");
        Console.WriteLine($"Total Products: {products.Count}");
        Console.WriteLine($"Categories: {string.Join(", ", products.Select(p => p.Category).Distinct())}");

        Console.WriteLine("\n=== Products Grouped by Category ===");
        var grouped = GroupByCategory(products);
        foreach (var group in grouped)
        {
            Console.WriteLine($"\n{group.Key}:");
            foreach (var product in group)
            {
                Console.WriteLine($"  - {product.Name} (${product.Price})");
            }
        }

        Console.WriteLine("\n=== Average Price per Category ===");
        var avgPrices = CalculateAveragePricePerCategory(products);
        foreach (var kvp in avgPrices)
        {
            Console.WriteLine($"{kvp.Key}: ${kvp.Value:F2}");
        }

        Console.WriteLine("\n=== Category Statistics ===");
        var stats = GetCategoryStats(products);
        foreach (var stat in stats)
        {
            Console.WriteLine(stat);
        }

        Console.WriteLine("\n=== Top 2 Categories by Total Value ===");
        var topCategories = GetTopCategoriesByTotalValue(products, 2);
        foreach (var category in topCategories)
        {
            Console.WriteLine($"- {category}");
        }

        Console.WriteLine("\n=== Product Count by Supplier ===");
        var supplierCounts = CountProductsBySupplier(products);
        foreach (var kvp in supplierCounts)
        {
            Console.WriteLine($"{kvp.Key}: {kvp.Value} products");
        }
    }

    // SOLUTION 1
    public static IEnumerable<IGrouping<string, Product>> GroupByCategory(List<Product> products)
    {
        return products.GroupBy(p => p.Category);
    }

    // SOLUTION 2
    public static Dictionary<string, decimal> CalculateAveragePricePerCategory(List<Product> products)
    {
        return products
            .GroupBy(p => p.Category)
            .ToDictionary(
                g => g.Key,
                g => g.Average(p => p.Price)
            );
    }

    // SOLUTION 3
    public static List<CategoryStats> GetCategoryStats(List<Product> products)
    {
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
    }

    // SOLUTION 4
    public static List<string> GetTopCategoriesByTotalValue(List<Product> products, int topN)
    {
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
    }

    // SOLUTION 5
    public static Dictionary<string, int> CountProductsBySupplier(List<Product> products)
    {
        return products
            .GroupBy(p => p.Supplier)
            .ToDictionary(
                g => g.Key,
                g => g.Count()
            );
    }

    public static List<Product> GetSampleProducts()
    {
        return new List<Product>
        {
            new Product { Id = 1, Name = "Laptop", Category = "Electronics", Price = 1200, StockQuantity = 5, AddedDate = new DateTime(2024, 1, 15), IsActive = true, Supplier = "TechCorp" },
            new Product { Id = 2, Name = "Mouse", Category = "Electronics", Price = 25, StockQuantity = 50, AddedDate = new DateTime(2024, 2, 1), IsActive = true, Supplier = "TechCorp" },
            new Product { Id = 3, Name = "Keyboard", Category = "Electronics", Price = 75, StockQuantity = 30, AddedDate = new DateTime(2024, 1, 20), IsActive = true, Supplier = "TechCorp" },
            new Product { Id = 4, Name = "Monitor", Category = "Electronics", Price = 300, StockQuantity = 10, AddedDate = new DateTime(2024, 3, 5), IsActive = true, Supplier = "DisplayCo" },
            new Product { Id = 5, Name = "Desk", Category = "Furniture", Price = 450, StockQuantity = 3, AddedDate = new DateTime(2024, 2, 10), IsActive = true, Supplier = "FurniturePlus" },
            new Product { Id = 6, Name = "Chair", Category = "Furniture", Price = 200, StockQuantity = 8, AddedDate = new DateTime(2024, 2, 15), IsActive = true, Supplier = "FurniturePlus" },
            new Product { Id = 7, Name = "Lamp", Category = "Furniture", Price = 40, StockQuantity = 15, AddedDate = new DateTime(2024, 1, 5), IsActive = false, Supplier = "FurniturePlus" },
            new Product { Id = 8, Name = "Notebook", Category = "Stationery", Price = 5, StockQuantity = 100, AddedDate = new DateTime(2024, 3, 1), IsActive = true, Supplier = "OfficePro" },
            new Product { Id = 9, Name = "Pen Set", Category = "Stationery", Price = 15, StockQuantity = 200, AddedDate = new DateTime(2024, 2, 20), IsActive = true, Supplier = "OfficePro" },
            new Product { Id = 10, Name = "Bookshelf", Category = "Furniture", Price = 350, StockQuantity = 2, AddedDate = new DateTime(2024, 3, 10), IsActive = true, Supplier = "FurniturePlus" },
            new Product { Id = 11, Name = "Tablet", Category = "Electronics", Price = 600, StockQuantity = 12, AddedDate = new DateTime(2024, 2, 25), IsActive = true, Supplier = "TechCorp" },
            new Product { Id = 12, Name = "Printer Paper", Category = "Stationery", Price = 20, StockQuantity = 75, AddedDate = new DateTime(2024, 1, 30), IsActive = true, Supplier = "OfficePro" },
            new Product { Id = 13, Name = "Desk Organizer", Category = "Stationery", Price = 12, StockQuantity = 40, AddedDate = new DateTime(2024, 2, 5), IsActive = true, Supplier = "OfficePro" },
            new Product { Id = 14, Name = "Webcam", Category = "Electronics", Price = 80, StockQuantity = 20, AddedDate = new DateTime(2024, 3, 15), IsActive = true, Supplier = "DisplayCo" },
            new Product { Id = 15, Name = "Standing Desk", Category = "Furniture", Price = 800, StockQuantity = 4, AddedDate = new DateTime(2024, 3, 20), IsActive = true, Supplier = "FurniturePlus" }
        };
    }
}
```

---

## Bonus Solutions

### Bonus 1: GetProductsWithAboveAveragePriceInCategory

```csharp
public static Dictionary<string, List<Product>> GetProductsWithAboveAveragePriceInCategory(List<Product> products)
{
    return products
        .GroupBy(p => p.Category)
        .ToDictionary(
            g => g.Key,
            g =>
            {
                var avgPrice = g.Average(p => p.Price);
                return g.Where(p => p.Price > avgPrice).ToList();
            }
        );
}
```

### Bonus 2: GetSuppliersWithMostProducts

```csharp
public static List<string> GetSuppliersWithMostProducts(List<Product> products, int minCount)
{
    return products
        .GroupBy(p => p.Supplier)
        .Where(g => g.Count() > minCount)
        .Select(g => g.Key)
        .ToList();
}
```

### Bonus 3: GetCategoryWithHighestAveragePrice

```csharp
public static string GetCategoryWithHighestAveragePrice(List<Product> products)
{
    return products
        .GroupBy(p => p.Category)
        .Select(g => new
        {
            Category = g.Key,
            AvgPrice = g.Average(p => p.Price)
        })
        .OrderByDescending(x => x.AvgPrice)
        .First()
        .Category;
}
```

### Bonus 4: GetMonthlyProductAdditionStats

```csharp
public static Dictionary<string, int> GetMonthlyProductAdditionStats(List<Product> products)
{
    return products
        .GroupBy(p => p.AddedDate.ToString("yyyy-MM"))
        .ToDictionary(
            g => g.Key,
            g => g.Count()
        );
}
```

---

## Key Takeaways

1. **GroupBy creates IGrouping**: Each group has a `Key` and is enumerable
2. **Multiple aggregations**: You can call Count(), Sum(), Average(), Min(), Max() on groups
3. **ToDictionary conversion**: Easily convert grouped data to dictionaries
4. **Anonymous types**: Perfect for intermediate transformations
5. **Method chaining**: Complex queries are built step by step
6. **Deferred execution**: Queries don't execute until ToList(), ToDictionary(), etc.

---

## Common Patterns Summary

### Simple Grouping
```csharp
products.GroupBy(p => p.Category)
```

### Group and Count
```csharp
products.GroupBy(p => p.Category).ToDictionary(g => g.Key, g => g.Count())
```

### Group and Average
```csharp
products.GroupBy(p => p.Category).ToDictionary(g => g.Key, g => g.Average(p => p.Price))
```

### Group, Aggregate, and Order
```csharp
products
    .GroupBy(p => p.Category)
    .Select(g => new { Category = g.Key, Total = g.Sum(...) })
    .OrderByDescending(x => x.Total)
```

### Multiple Aggregations
```csharp
products
    .GroupBy(p => p.Category)
    .Select(g => new Stats {
        Count = g.Count(),
        Sum = g.Sum(...),
        Avg = g.Average(...)
    })
```

---

**Congratulations on completing the exercise! 🎉**

*Now move on to the next exercise: `03-Joins`*
