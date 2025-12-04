namespace BasicQueries;

/// <summary>
/// LINQ Basic Queries Exercise
/// Complete the TODO methods to make all tests pass.
/// </summary>
public class Program
{
    public static void Main(string[] args)
    {
        Console.WriteLine("LINQ Basic Queries Exercise");
        Console.WriteLine("Run 'dotnet test' to check your solutions");

        var products = GetSampleProducts();

        // Test your implementations
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

    // ========== TODO 1: FILTERING ==========
    /// <summary>
    /// Returns products with price greater than 100.
    /// TODO: Use LINQ Where() to filter products
    /// </summary>
    public static List<Product> GetExpensiveProducts(List<Product> products)
    {
        // INSTRUCTION: Replace the line below with a LINQ query
        // HINT: Use Where() method with a lambda expression
        // HINT: Filter condition: p.Price > 100
        return products; // ← TODO: Change this line
    }

    // ========== TODO 2: FILTERING WITH MULTIPLE CONDITIONS ==========
    /// <summary>
    /// Returns products that are in stock (StockQuantity > 0) and active.
    /// TODO: Use LINQ Where() with multiple conditions
    /// </summary>
    public static List<Product> GetInStockProducts(List<Product> products)
    {
        // INSTRUCTION: Use Where() with && (AND) operator
        // HINT: Check both StockQuantity > 0 AND IsActive == true
        throw new NotImplementedException(); // ← TODO: Replace with your solution
    }

    // ========== TODO 3: ORDERING ==========
    /// <summary>
    /// Orders products by category, then by price (ascending).
    /// TODO: Use OrderBy() and ThenBy()
    /// </summary>
    public static List<Product> OrderByCategoryThenPrice(List<Product> products)
    {
        // INSTRUCTION: Chain OrderBy() and ThenBy()
        // HINT: First order by Category, then by Price
        throw new NotImplementedException(); // ← TODO: Replace with your solution
    }

    // ========== TODO 4: ORDERING (DESCENDING) ==========
    /// <summary>
    /// Orders products by date added, most recent first.
    /// TODO: Use OrderByDescending()
    /// </summary>
    public static List<Product> OrderByMostRecent(List<Product> products)
    {
        // INSTRUCTION: Use OrderByDescending() on AddedDate
        // HINT: Most recent = newest date first (descending)
        throw new NotImplementedException(); // ← TODO: Replace with your solution
    }

    // ========== TODO 5: PROJECTION ==========
    /// <summary>
    /// Returns just the names of all products.
    /// TODO: Use Select() to project to a list of strings
    /// </summary>
    public static List<string> GetProductNames(List<Product> products)
    {
        // INSTRUCTION: Use Select() to extract just the Name property
        // HINT: Select(p => p.Name)
        throw new NotImplementedException(); // ← TODO: Replace with your solution
    }

    // ========== TODO 6: PROJECTION (ANONYMOUS TYPE) ==========
    /// <summary>
    /// Returns product summaries with Name and Price.
    /// TODO: Use Select() to create an anonymous type
    /// </summary>
    public static List<object> GetProductSummaries(List<Product> products)
    {
        // INSTRUCTION: Use Select() with anonymous type: new { Name = p.Name, Price = p.Price }
        // HINT: Cast to object in the list
        throw new NotImplementedException(); // ← TODO: Replace with your solution
    }

    // ========== SAMPLE DATA ==========
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
