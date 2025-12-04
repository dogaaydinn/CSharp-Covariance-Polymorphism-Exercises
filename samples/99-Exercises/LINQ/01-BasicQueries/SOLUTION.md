# ⚠️ SPOILER WARNING ⚠️

**DO NOT READ THIS FILE UNTIL YOU'VE TRIED TO SOLVE THE EXERCISES YOURSELF!**

This file contains complete solutions to all TODO exercises. Try to complete them on your own first!

---

# LINQ Basic Queries - Complete Solutions

## TODO 1: GetExpensiveProducts

```csharp
public static List<Product> GetExpensiveProducts(List<Product> products)
{
    return products.Where(p => p.Price > 100).ToList();
}
```

**Explanation:**
- `Where()` filters the collection based on a predicate
- `p => p.Price > 100` is a lambda expression that returns true for products over $100
- `.ToList()` materializes the query into a List<Product>

---

## TODO 2: GetInStockProducts

```csharp
public static List<Product> GetInStockProducts(List<Product> products)
{
    return products
        .Where(p => p.StockQuantity > 0 && p.IsActive)
        .ToList();
}
```

**Explanation:**
- Combines two conditions with `&&` (logical AND)
- Both conditions must be true for the product to be included
- `StockQuantity > 0` ensures product is available
- `IsActive` ensures product is not discontinued

---

## TODO 3: OrderByCategoryThenPrice

```csharp
public static List<Product> OrderByCategoryThenPrice(List<Product> products)
{
    return products
        .OrderBy(p => p.Category)
        .ThenBy(p => p.Price)
        .ToList();
}
```

**Explanation:**
- `OrderBy()` sorts by the first criterion (Category)
- `ThenBy()` sorts by the second criterion (Price) within each category
- This creates a hierarchical sort: first by category alphabetically, then by price within each category

---

## TODO 4: OrderByMostRecent

```csharp
public static List<Product> OrderByMostRecent(List<Product> products)
{
    return products
        .OrderByDescending(p => p.AddedDate)
        .ToList();
}
```

**Explanation:**
- `OrderByDescending()` sorts in descending order (largest to smallest)
- For dates, descending means newest dates first
- This returns products with the most recent AddedDate at the beginning

---

## TODO 5: GetProductNames

```csharp
public static List<string> GetProductNames(List<Product> products)
{
    return products
        .Select(p => p.Name)
        .ToList();
}
```

**Explanation:**
- `Select()` projects each element into a new form
- `p => p.Name` extracts just the Name property
- Result is `List<string>` instead of `List<Product>`

---

## TODO 6: GetProductSummaries

```csharp
public static List<object> GetProductSummaries(List<Product> products)
{
    return products
        .Select(p => new { Name = p.Name, Price = p.Price })
        .Cast<object>()
        .ToList();
}
```

**Explanation:**
- `new { Name = p.Name, Price = p.Price }` creates an anonymous type
- Anonymous types are read-only and have auto-implemented properties
- `.Cast<object>()` is needed because anonymous types can't be used as return types directly
- Result is a list of objects that each have Name and Price properties

---

## Alternative Solutions

### TODO 1 (Alternative with Query Syntax)
```csharp
public static List<Product> GetExpensiveProducts(List<Product> products)
{
    return (from p in products
            where p.Price > 100
            select p).ToList();
}
```

### TODO 2 (Alternative with Explicit Lambda)
```csharp
public static List<Product> GetInStockProducts(List<Product> products)
{
    Func<Product, bool> predicate = p => p.StockQuantity > 0 && p.IsActive;
    return products.Where(predicate).ToList();
}
```

### TODO 3 (Alternative Single-Line)
```csharp
public static List<Product> OrderByCategoryThenPrice(List<Product> products)
{
    return products.OrderBy(p => p.Category).ThenBy(p => p.Price).ToList();
}
```

---

## Complete Program.cs (All TODOs Solved)

```csharp
namespace BasicQueries;

public class Program
{
    public static void Main(string[] args)
    {
        Console.WriteLine("LINQ Basic Queries Exercise");
        Console.WriteLine("Run 'dotnet test' to check your solutions");

        var products = GetSampleProducts();

        Console.WriteLine("\n=== Expensive Products (Price > 100) ===");
        var expensive = GetExpensiveProducts(products);
        foreach (var p in expensive)
        {
            Console.WriteLine(p);
        }

        Console.WriteLine("\n=== In Stock Products ===");
        var inStock = GetInStockProducts(products);
        foreach (var p in inStock)
        {
            Console.WriteLine(p);
        }
    }

    // SOLUTION 1
    public static List<Product> GetExpensiveProducts(List<Product> products)
    {
        return products.Where(p => p.Price > 100).ToList();
    }

    // SOLUTION 2
    public static List<Product> GetInStockProducts(List<Product> products)
    {
        return products
            .Where(p => p.StockQuantity > 0 && p.IsActive)
            .ToList();
    }

    // SOLUTION 3
    public static List<Product> OrderByCategoryThenPrice(List<Product> products)
    {
        return products
            .OrderBy(p => p.Category)
            .ThenBy(p => p.Price)
            .ToList();
    }

    // SOLUTION 4
    public static List<Product> OrderByMostRecent(List<Product> products)
    {
        return products
            .OrderByDescending(p => p.AddedDate)
            .ToList();
    }

    // SOLUTION 5
    public static List<string> GetProductNames(List<Product> products)
    {
        return products
            .Select(p => p.Name)
            .ToList();
    }

    // SOLUTION 6
    public static List<object> GetProductSummaries(List<Product> products)
    {
        return products
            .Select(p => new { Name = p.Name, Price = p.Price })
            .Cast<object>()
            .ToList();
    }

    public static List<Product> GetSampleProducts()
    {
        return new List<Product>
        {
            new Product { Id = 1, Name = "Laptop", Category = "Electronics", Price = 1200, StockQuantity = 5, AddedDate = new DateTime(2024, 1, 15), IsActive = true },
            new Product { Id = 2, Name = "Mouse", Category = "Electronics", Price = 25, StockQuantity = 50, AddedDate = new DateTime(2024, 2, 1), IsActive = true },
            new Product { Id = 3, Name = "Keyboard", Category = "Electronics", Price = 75, StockQuantity = 30, AddedDate = new DateTime(2024, 1, 20), IsActive = true },
            new Product { Id = 4, Name = "Monitor", Category = "Electronics", Price = 300, StockQuantity = 10, AddedDate = new DateTime(2024, 3, 5), IsActive = true },
            new Product { Id = 5, Name = "Desk", Category = "Furniture", Price = 450, StockQuantity = 3, AddedDate = new DateTime(2024, 2, 10), IsActive = true },
            new Product { Id = 6, Name = "Chair", Category = "Furniture", Price = 200, StockQuantity = 8, AddedDate = new DateTime(2024, 2, 15), IsActive = true },
            new Product { Id = 7, Name = "Lamp", Category = "Furniture", Price = 40, StockQuantity = 0, AddedDate = new DateTime(2024, 1, 5), IsActive = false },
            new Product { Id = 8, Name = "Notebook", Category = "Stationery", Price = 5, StockQuantity = 100, AddedDate = new DateTime(2024, 3, 1), IsActive = true },
            new Product { Id = 9, Name = "Pen Set", Category = "Stationery", Price = 15, StockQuantity = 200, AddedDate = new DateTime(2024, 2, 20), IsActive = true },
            new Product { Id = 10, Name = "Bookshelf", Category = "Furniture", Price = 350, StockQuantity = 2, AddedDate = new DateTime(2024, 3, 10), IsActive = true }
        };
    }
}
```

---

## Bonus Solutions

### Bonus 1: GetTop3ExpensiveProducts
```csharp
public static List<Product> GetTop3ExpensiveProducts(List<Product> products)
{
    return products
        .OrderByDescending(p => p.Price)
        .Take(3)
        .ToList();
}
```

### Bonus 2: GetProductsInPriceRange
```csharp
public static List<Product> GetProductsInPriceRange(List<Product> products, decimal min, decimal max)
{
    return products
        .Where(p => p.Price >= min && p.Price <= max)
        .ToList();
}
```

### Bonus 3: GetCategoriesWithProductCount
```csharp
public static Dictionary<string, int> GetCategoriesWithProductCount(List<Product> products)
{
    return products
        .GroupBy(p => p.Category)
        .ToDictionary(g => g.Key, g => g.Count());
}
```

---

## Key Takeaways

1. **Method Chaining**: LINQ methods can be chained together fluently
2. **Lambda Expressions**: Compact syntax for inline delegates
3. **Deferred Execution**: Queries don't execute until enumerated (ToList, foreach, etc.)
4. **Type Safety**: Compiler checks types at compile time
5. **Readability**: LINQ makes queries more readable than loops

---

**Congratulations on completing the exercise! 🎉**

*Now move on to the next exercise: `02-GroupingAggregation`*
