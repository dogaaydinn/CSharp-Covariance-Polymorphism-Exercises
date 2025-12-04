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

        // Uncomment these when you implement the methods
        /*
        Console.WriteLine("\n=== Products Grouped by Category ===");
        var grouped = GroupByCategory(products);
        foreach (var group in grouped)
        {
            Console.WriteLine($"\n{group.Key}:");
            foreach (var product in group)
            {
                Console.WriteLine($"  - {product.Name}");
            }
        }

        Console.WriteLine("\n=== Average Price per Category ===");
        var avgPrices = CalculateAveragePricePerCategory(products);
        foreach (var kvp in avgPrices)
        {
            Console.WriteLine($"{kvp.Key}: ${kvp.Value:F2}");
        }
        */
    }

    // TODO 1: Group products by category
    // HINT: Use GroupBy() method
    // RETURN: IEnumerable<IGrouping<string, Product>> where string is the category
    public static IEnumerable<IGrouping<string, Product>> GroupByCategory(List<Product> products)
    {
        // TODO: Implement grouping by Category property
        // Example: products.GroupBy(p => p.Category)
        throw new NotImplementedException();
    }

    // TODO 2: Calculate average price for each category
    // HINT: Use GroupBy() then Select() with Average()
    // RETURN: Dictionary<string, decimal> where key is category, value is average price
    public static Dictionary<string, decimal> CalculateAveragePricePerCategory(List<Product> products)
    {
        // TODO: Group by category, then calculate average price for each group
        // HINT: After GroupBy(), use ToDictionary() with g.Key and g.Average(p => p.Price)
        throw new NotImplementedException();
    }

    // TODO 3: Get comprehensive statistics for each category
    // HINT: Use GroupBy() then Select() to create CategoryStats objects
    // RETURN: List of CategoryStats with Count, TotalValue, AveragePrice, MinPrice, MaxPrice
    public static List<CategoryStats> GetCategoryStats(List<Product> products)
    {
        // TODO: Group by category and calculate multiple aggregations
        // For each group, create a CategoryStats object with:
        // - Category name
        // - Count of products
        // - Total value (Sum of Price * StockQuantity)
        // - Average price
        // - Min price
        // - Max price
        throw new NotImplementedException();
    }

    // TODO 4: Get top N categories by total inventory value
    // HINT: Group by category, calculate total value, order descending, take N
    // RETURN: List of category names ordered by total inventory value (Price * StockQuantity)
    public static List<string> GetTopCategoriesByTotalValue(List<Product> products, int topN)
    {
        // TODO:
        // 1. Group products by category
        // 2. For each group, calculate total value: Sum(Price * StockQuantity)
        // 3. Order by total value descending
        // 4. Take top N categories
        // 5. Select just the category name
        throw new NotImplementedException();
    }

    // TODO 5: Count products by supplier
    // HINT: Use GroupBy() with Supplier, then create dictionary with counts
    // RETURN: Dictionary<string, int> where key is supplier name, value is product count
    public static Dictionary<string, int> CountProductsBySupplier(List<Product> products)
    {
        // TODO: Group by Supplier and count products in each group
        // HINT: Use ToDictionary(g => g.Key, g => g.Count())
        throw new NotImplementedException();
    }

    // Helper method to generate sample data
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

// Helper class for category statistics
public class CategoryStats
{
    public string Category { get; set; } = string.Empty;
    public int ProductCount { get; set; }
    public decimal TotalValue { get; set; }
    public decimal AveragePrice { get; set; }
    public decimal MinPrice { get; set; }
    public decimal MaxPrice { get; set; }

    public override string ToString()
    {
        return $"{Category}: {ProductCount} products, Avg: ${AveragePrice:F2}, Total Value: ${TotalValue:F2}";
    }
}
