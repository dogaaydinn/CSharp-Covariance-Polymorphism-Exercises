namespace Joins;

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int SupplierId { get; set; }
    public int CategoryId { get; set; }
    public decimal Price { get; set; }

    public override string ToString() => $"[{Id}] {Name} - ${Price}";
}

public class Supplier
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;

    public override string ToString() => $"{Name} ({Country})";
}

public class Category
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    public override string ToString() => Name;
}

public class Order
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public DateTime OrderDate { get; set; }
    public string CustomerName { get; set; } = string.Empty;

    public override string ToString() => $"Order #{Id} - {Quantity} units on {OrderDate:yyyy-MM-dd}";
}

// Result classes for join operations
public class ProductWithSupplier
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string SupplierName { get; set; } = string.Empty;
    public string SupplierCountry { get; set; } = string.Empty;

    public override string ToString() => $"{ProductName} - Supplier: {SupplierName} ({SupplierCountry})";
}

public class ProductWithOrders
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public int? OrderId { get; set; }
    public int? Quantity { get; set; }
    public string? CustomerName { get; set; }

    public override string ToString() => OrderId.HasValue
        ? $"{ProductName} - Order #{OrderId}: {Quantity} units for {CustomerName}"
        : $"{ProductName} - No orders";
}

public class ProductDetail
{
    public string ProductName { get; set; } = string.Empty;
    public string CategoryName { get; set; } = string.Empty;
    public string SupplierName { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int TotalOrders { get; set; }

    public override string ToString() =>
        $"{ProductName} ({CategoryName}) - {SupplierName} - ${Price} - {TotalOrders} orders";
}

public class CategoryWithProducts
{
    public string CategoryName { get; set; } = string.Empty;
    public List<string> ProductNames { get; set; } = new();

    public override string ToString() =>
        $"{CategoryName}: [{string.Join(", ", ProductNames)}]";
}
