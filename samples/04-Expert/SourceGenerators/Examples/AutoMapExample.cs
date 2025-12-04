using System;
using AdvancedConcepts.SourceGenerators;

namespace SourceGenerators.Examples;

/// <summary>
/// Demonstrates the AutoMapGenerator - Automatic DTO Mapping without Reflection
///
/// The AutoMapGenerator creates compile-time extension methods for mapping between types.
/// This eliminates the runtime overhead of reflection-based mappers like AutoMapper.
///
/// Performance Benefits:
/// - Zero runtime overhead (no reflection)
/// - Zero allocations for mapping logic
/// - Type-safe at compile time
/// - As fast as hand-written mapping code
/// </summary>
public static class AutoMapExample
{
    public static void Run()
    {
        Console.WriteLine("EXAMPLE 1: Basic AutoMap Usage");
        Console.WriteLine("─".PadRight(70, '─'));
        RunBasicMapping();
        Console.WriteLine();

        Console.WriteLine("EXAMPLE 2: Complex Domain Model Mapping");
        Console.WriteLine("─".PadRight(70, '─'));
        RunComplexMapping();
        Console.WriteLine();

        Console.WriteLine("EXAMPLE 3: Collection Mapping");
        Console.WriteLine("─".PadRight(70, '─'));
        RunCollectionMapping();
        Console.WriteLine();

        Console.WriteLine("EXAMPLE 4: Performance Comparison");
        Console.WriteLine("─".PadRight(70, '─'));
        RunPerformanceComparison();
        Console.WriteLine();
    }

    private static void RunBasicMapping()
    {
        Console.WriteLine("Creating a User entity...");
        var user = new User
        {
            Id = 123,
            FirstName = "John",
            LastName = "Doe",
            Email = "john.doe@example.com",
            DateOfBirth = new DateTime(1990, 5, 15),
            IsActive = true,
            PasswordHash = "super-secret-hash", // Won't be mapped
            CreatedAt = DateTime.Now
        };

        Console.WriteLine($"User: {user.FirstName} {user.LastName} (ID: {user.Id})");
        Console.WriteLine($"Email: {user.Email}");
        Console.WriteLine($"Active: {user.IsActive}");
        Console.WriteLine($"Password Hash: {user.PasswordHash} (will NOT be mapped)");
        Console.WriteLine();

        // Use the GENERATED extension method!
        Console.WriteLine("Mapping to UserDto using GENERATED ToUserDto() method...");
        var dto = user.ToUserDto();

        Console.WriteLine($"UserDto: {dto.FullName} (ID: {dto.Id})");
        Console.WriteLine($"Email: {dto.Email}");
        Console.WriteLine($"Age: {dto.Age} years old");
        Console.WriteLine($"Active: {dto.IsActive}");
        Console.WriteLine($"Has PasswordHash?: {dto.GetType().GetProperty("PasswordHash") != null}");
        Console.WriteLine();

        // Reverse mapping also generated!
        Console.WriteLine("Mapping back to User using GENERATED ToUser() method...");
        var userBack = dto.ToUser();
        Console.WriteLine($"User: {userBack.FirstName} {userBack.LastName}");
        Console.WriteLine($"Password Hash after reverse mapping: '{userBack.PasswordHash}' (empty - as expected)");
        Console.WriteLine();

        Console.WriteLine("✓ Generated extension methods are located in:");
        Console.WriteLine("  obj/Debug/net8.0/generated/.../User_AutoMap.g.cs");
    }

    private static void RunComplexMapping()
    {
        Console.WriteLine("Creating a complex Order with nested objects...");
        var order = new Order
        {
            OrderId = "ORD-001",
            CustomerName = "Jane Smith",
            CustomerEmail = "jane@example.com",
            OrderDate = DateTime.Now,
            TotalAmount = 499.99m,
            Status = "Pending",
            ShippingAddress = "123 Main St, City, State 12345"
        };

        Console.WriteLine($"Order ID: {order.OrderId}");
        Console.WriteLine($"Customer: {order.CustomerName} ({order.CustomerEmail})");
        Console.WriteLine($"Total: ${order.TotalAmount:F2}");
        Console.WriteLine($"Status: {order.Status}");
        Console.WriteLine($"Shipping: {order.ShippingAddress}");
        Console.WriteLine();

        // Map to API response DTO
        Console.WriteLine("Mapping to OrderResponse DTO for API...");
        var response = order.ToOrderResponse();

        Console.WriteLine($"Response ID: {response.OrderId}");
        Console.WriteLine($"Customer: {response.CustomerName}");
        Console.WriteLine($"Total: ${response.TotalAmount:F2}");
        Console.WriteLine($"Display Status: {response.DisplayStatus}");
        Console.WriteLine();

        // Map to different view model
        Console.WriteLine("Mapping to OrderSummary ViewModel...");
        var summary = order.ToOrderSummary();

        Console.WriteLine($"Summary ID: {summary.OrderNumber}");
        Console.WriteLine($"Customer: {summary.CustomerName}");
        Console.WriteLine($"Amount: ${summary.Amount:F2}");
        Console.WriteLine();
    }

    private static void RunCollectionMapping()
    {
        Console.WriteLine("Creating a list of products...");
        var products = new[]
        {
            new Product { Id = 1, Name = "Laptop", Price = 999.99m, Stock = 10, Category = "Electronics" },
            new Product { Id = 2, Name = "Mouse", Price = 29.99m, Stock = 50, Category = "Electronics" },
            new Product { Id = 3, Name = "Desk", Price = 299.99m, Stock = 5, Category = "Furniture" }
        };

        Console.WriteLine($"Products count: {products.Length}");
        foreach (var product in products)
        {
            Console.WriteLine($"  - {product.Name}: ${product.Price:F2} (Stock: {product.Stock})");
        }
        Console.WriteLine();

        // Map collection using LINQ + generated methods
        Console.WriteLine("Mapping to ProductDto collection using GENERATED methods...");
        var productDtos = products.Select(p => p.ToProductDto()).ToList();

        Console.WriteLine($"ProductDto count: {productDtos.Count}");
        foreach (var dto in productDtos)
        {
            Console.WriteLine($"  - {dto.Name}: ${dto.Price:F2} ({dto.StockStatus})");
        }
        Console.WriteLine();
    }

    private static void RunPerformanceComparison()
    {
        const int iterations = 100_000;

        var user = new User
        {
            Id = 123,
            FirstName = "John",
            LastName = "Doe",
            Email = "john.doe@example.com",
            DateOfBirth = new DateTime(1990, 5, 15),
            IsActive = true,
            CreatedAt = DateTime.Now
        };

        // Warm up
        for (int i = 0; i < 1000; i++)
        {
            var dto1 = user.ToUserDto();
            var dto2 = ManualMapping(user);
        }

        Console.WriteLine($"Mapping {iterations:N0} times...");
        Console.WriteLine();

        // Test generated mapping
        var sw = System.Diagnostics.Stopwatch.StartNew();
        for (int i = 0; i < iterations; i++)
        {
            var dto = user.ToUserDto();
        }
        sw.Stop();
        var generatedTime = sw.Elapsed.TotalMilliseconds;
        Console.WriteLine($"Generated AutoMap:  {generatedTime:F2} ms ({iterations / generatedTime:F0} ops/ms)");

        // Test manual mapping
        sw.Restart();
        for (int i = 0; i < iterations; i++)
        {
            var dto = ManualMapping(user);
        }
        sw.Stop();
        var manualTime = sw.Elapsed.TotalMilliseconds;
        Console.WriteLine($"Manual Mapping:     {manualTime:F2} ms ({iterations / manualTime:F0} ops/ms)");

        Console.WriteLine();
        var difference = Math.Abs(generatedTime - manualTime);
        var percentage = (difference / Math.Max(generatedTime, manualTime)) * 100;

        if (percentage < 5)
        {
            Console.WriteLine($"✓ Generated code is EQUIVALENT to manual mapping!");
            Console.WriteLine($"  (Difference: {difference:F2} ms, {percentage:F1}%)");
        }
        else if (generatedTime < manualTime)
        {
            Console.WriteLine($"✓ Generated code is FASTER by {percentage:F1}%!");
        }
        else
        {
            Console.WriteLine($"✓ Generated code is within {percentage:F1}% of manual mapping");
        }

        Console.WriteLine();
        Console.WriteLine("KEY INSIGHT:");
        Console.WriteLine("The generated mapping code is as fast as hand-written code");
        Console.WriteLine("because it IS hand-written code - generated at compile-time!");
    }

    private static UserDto ManualMapping(User user)
    {
        return new UserDto
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            DateOfBirth = user.DateOfBirth,
            IsActive = user.IsActive
        };
    }
}

#region Domain Models

/// <summary>
/// Domain entity with [AutoMap] attribute.
/// The generator will create ToUserDto() and ToUser() extension methods.
/// </summary>
[AutoMap(typeof(UserDto), GenerateReverseMap = true)]
public class User
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }

    [AutoMapIgnore] // This property won't be mapped for security
    public string PasswordHash { get; set; } = string.Empty;
}

/// <summary>
/// Data Transfer Object for User.
/// Generated mapping will exclude PasswordHash and CreatedAt.
/// </summary>
public class UserDto
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FullName => $"{FirstName} {LastName}"; // Computed property
    public string Email { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
    public int Age => DateTime.Now.Year - DateOfBirth.Year; // Computed property
    public bool IsActive { get; set; }
}

/// <summary>
/// Order entity demonstrating multiple AutoMap targets.
/// </summary>
[AutoMap(typeof(OrderResponse))]
[AutoMap(typeof(OrderSummary))]
public class Order
{
    public string OrderId { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerEmail { get; set; } = string.Empty;
    public DateTime OrderDate { get; set; }
    public decimal TotalAmount { get; set; }
    public string Status { get; set; } = string.Empty;
    public string ShippingAddress { get; set; } = string.Empty;
}

/// <summary>
/// Full order response for API.
/// </summary>
public class OrderResponse
{
    public string OrderId { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerEmail { get; set; } = string.Empty;
    public DateTime OrderDate { get; set; }
    public decimal TotalAmount { get; set; }
    public string Status { get; set; } = string.Empty;
    public string DisplayStatus => $"Order {Status}"; // Computed
}

/// <summary>
/// Simplified order summary.
/// </summary>
public class OrderSummary
{
    [AutoMapProperty("OrderId")] // Map from OrderId to OrderNumber
    public string OrderNumber { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;

    [AutoMapProperty("TotalAmount")] // Map from TotalAmount to Amount
    public decimal Amount { get; set; }
}

/// <summary>
/// Product entity.
/// </summary>
[AutoMap(typeof(ProductDto))]
public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public string Category { get; set; } = string.Empty;
}

/// <summary>
/// Product DTO with computed properties.
/// </summary>
public class ProductDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public string StockStatus => Stock > 10 ? "In Stock" : Stock > 0 ? "Low Stock" : "Out of Stock";
}

#endregion
