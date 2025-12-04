using SourceGenerators;

namespace Consumer;

// ✅ SCENARIO: Auto-generate ToString() using source generator
// Instead of manually writing ToString(), we use [GenerateToString] attribute
// The Roslyn source generator creates the implementation at compile time

/// <summary>
/// Example class using source generator for ToString()
/// The 'partial' keyword is required for source generators to extend the class
/// </summary>
[GenerateToString(IncludePropertyNames = true)]
public partial class Person
{
    public string Name { get; set; } = string.Empty;
    public int Age { get; set; }
    public string Email { get; set; } = string.Empty;

    public Person(string name, int age, string email)
    {
        Name = name;
        Age = age;
        Email = email;
    }
}

/// <summary>
/// Another example with fewer properties
/// </summary>
[GenerateToString]
public partial class Product
{
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }

    public Product(string name, decimal price)
    {
        Name = name;
        Price = price;
    }
}

/// <summary>
/// Example with nested properties
/// </summary>
[GenerateToString]
public partial class Order
{
    public int OrderId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public DateTime OrderDate { get; set; }

    public Order(int orderId, string customerName, decimal totalAmount, DateTime orderDate)
    {
        OrderId = orderId;
        CustomerName = customerName;
        TotalAmount = totalAmount;
        OrderDate = orderDate;
    }
}
