namespace Joins;

public class Program
{
    public static void Main(string[] args)
    {
        Console.WriteLine("LINQ Joins Exercise");
        Console.WriteLine("Run 'dotnet test' to check your solutions\n");

        var products = GetSampleProducts();
        var suppliers = GetSampleSuppliers();
        var categories = GetSampleCategories();
        var orders = GetSampleOrders();

        Console.WriteLine($"Products: {products.Count}");
        Console.WriteLine($"Suppliers: {suppliers.Count}");
        Console.WriteLine($"Categories: {categories.Count}");
        Console.WriteLine($"Orders: {orders.Count}");

        // Uncomment these when you implement the methods
        /*
        Console.WriteLine("\n=== Products with Suppliers (Inner Join) ===");
        var productsWithSuppliers = InnerJoinProductsWithSuppliers(products, suppliers);
        foreach (var item in productsWithSuppliers.Take(5))
        {
            Console.WriteLine(item);
        }

        Console.WriteLine("\n=== Products with Orders (Left Join) ===");
        var productsWithOrders = LeftJoinProductsWithOrders(products, orders);
        foreach (var item in productsWithOrders.Take(5))
        {
            Console.WriteLine(item);
        }
        */
    }

    // TODO 1: Inner join products with suppliers
    // HINT: Use Join() method to match products with their suppliers
    // RETURN: List of ProductWithSupplier containing product and supplier information
    public static List<ProductWithSupplier> InnerJoinProductsWithSuppliers(
        List<Product> products,
        List<Supplier> suppliers)
    {
        // TODO: Join products with suppliers on SupplierId
        // Example structure:
        // return products
        //     .Join(suppliers,
        //         product => product.SupplierId,     // Outer key selector
        //         supplier => supplier.Id,           // Inner key selector
        //         (product, supplier) => new ProductWithSupplier { ... })
        //     .ToList();
        throw new NotImplementedException();
    }

    // TODO 2: Left join products with orders (show products even if they have no orders)
    // HINT: Use GroupJoin() + SelectMany() with DefaultIfEmpty() for left outer join
    // RETURN: List of ProductWithOrders, including products with no orders (OrderId will be null)
    public static List<ProductWithOrders> LeftJoinProductsWithOrders(
        List<Product> products,
        List<Order> orders)
    {
        // TODO: Left join products with orders
        // HINT: GroupJoin creates groups, then SelectMany with DefaultIfEmpty flattens them
        // Example structure:
        // return products
        //     .GroupJoin(orders,
        //         product => product.Id,
        //         order => order.ProductId,
        //         (product, orderGroup) => new { product, orderGroup })
        //     .SelectMany(
        //         x => x.orderGroup.DefaultIfEmpty(),
        //         (x, order) => new ProductWithOrders { ... })
        //     .ToList();
        throw new NotImplementedException();
    }

    // TODO 3: Multiple joins - combine products, categories, suppliers, and orders
    // HINT: Chain multiple Join() operations
    // RETURN: List of ProductDetail with information from all 4 tables
    public static List<ProductDetail> MultipleJoins(
        List<Product> products,
        List<Category> categories,
        List<Supplier> suppliers,
        List<Order> orders)
    {
        // TODO: Join products with categories, suppliers, and orders
        // HINT: You can chain joins like this:
        // products.Join(suppliers, ...).Join(categories, ...).GroupJoin(orders, ...)
        // For TotalOrders, use GroupJoin and Count() the order group
        throw new NotImplementedException();
    }

    // TODO 4: Group join - group products by category
    // HINT: Use GroupJoin() to get categories with their products
    // RETURN: List of CategoryWithProducts where each category contains list of product names
    public static List<CategoryWithProducts> GroupJoinProductsByCategory(
        List<Category> categories,
        List<Product> products)
    {
        // TODO: Group join categories with products
        // Example structure:
        // return categories
        //     .GroupJoin(products,
        //         category => category.Id,
        //         product => product.CategoryId,
        //         (category, productGroup) => new CategoryWithProducts
        //         {
        //             CategoryName = category.Name,
        //             ProductNames = productGroup.Select(p => p.Name).ToList()
        //         })
        //     .ToList();
        throw new NotImplementedException();
    }

    // Sample data generation methods
    public static List<Product> GetSampleProducts()
    {
        return new List<Product>
        {
            new Product { Id = 1, Name = "Laptop", SupplierId = 1, CategoryId = 1, Price = 1200 },
            new Product { Id = 2, Name = "Mouse", SupplierId = 1, CategoryId = 1, Price = 25 },
            new Product { Id = 3, Name = "Keyboard", SupplierId = 1, CategoryId = 1, Price = 75 },
            new Product { Id = 4, Name = "Monitor", SupplierId = 2, CategoryId = 1, Price = 300 },
            new Product { Id = 5, Name = "Desk", SupplierId = 3, CategoryId = 2, Price = 450 },
            new Product { Id = 6, Name = "Chair", SupplierId = 3, CategoryId = 2, Price = 200 },
            new Product { Id = 7, Name = "Lamp", SupplierId = 3, CategoryId = 2, Price = 40 },
            new Product { Id = 8, Name = "Notebook", SupplierId = 4, CategoryId = 3, Price = 5 },
            new Product { Id = 9, Name = "Pen Set", SupplierId = 4, CategoryId = 3, Price = 15 },
            new Product { Id = 10, Name = "Tablet", SupplierId = 1, CategoryId = 1, Price = 600 },
            new Product { Id = 11, Name = "Webcam", SupplierId = 2, CategoryId = 1, Price = 80 },
            new Product { Id = 12, Name = "Bookshelf", SupplierId = 3, CategoryId = 2, Price = 350 },
        };
    }

    public static List<Supplier> GetSampleSuppliers()
    {
        return new List<Supplier>
        {
            new Supplier { Id = 1, Name = "TechCorp", Country = "USA" },
            new Supplier { Id = 2, Name = "DisplayCo", Country = "Japan" },
            new Supplier { Id = 3, Name = "FurniturePlus", Country = "Sweden" },
            new Supplier { Id = 4, Name = "OfficePro", Country = "Germany" },
        };
    }

    public static List<Category> GetSampleCategories()
    {
        return new List<Category>
        {
            new Category { Id = 1, Name = "Electronics", Description = "Electronic devices and accessories" },
            new Category { Id = 2, Name = "Furniture", Description = "Office and home furniture" },
            new Category { Id = 3, Name = "Stationery", Description = "Office supplies and stationery" },
            new Category { Id = 4, Name = "Books", Description = "Books and publications" }, // No products in this category
        };
    }

    public static List<Order> GetSampleOrders()
    {
        return new List<Order>
        {
            new Order { Id = 1, ProductId = 1, Quantity = 2, OrderDate = new DateTime(2024, 1, 15), CustomerName = "Alice" },
            new Order { Id = 2, ProductId = 1, Quantity = 1, OrderDate = new DateTime(2024, 2, 10), CustomerName = "Bob" },
            new Order { Id = 3, ProductId = 2, Quantity = 5, OrderDate = new DateTime(2024, 1, 20), CustomerName = "Charlie" },
            new Order { Id = 4, ProductId = 4, Quantity = 1, OrderDate = new DateTime(2024, 2, 5), CustomerName = "Diana" },
            new Order { Id = 5, ProductId = 5, Quantity = 3, OrderDate = new DateTime(2024, 1, 25), CustomerName = "Eve" },
            new Order { Id = 6, ProductId = 8, Quantity = 10, OrderDate = new DateTime(2024, 2, 15), CustomerName = "Frank" },
            new Order { Id = 7, ProductId = 9, Quantity = 20, OrderDate = new DateTime(2024, 2, 20), CustomerName = "Grace" },
            new Order { Id = 8, ProductId = 1, Quantity = 1, OrderDate = new DateTime(2024, 3, 1), CustomerName = "Henry" },
            new Order { Id = 9, ProductId = 10, Quantity = 2, OrderDate = new DateTime(2024, 2, 25), CustomerName = "Iris" },
            // Note: Products 3, 6, 7, 11, 12 have no orders
        };
    }
}
