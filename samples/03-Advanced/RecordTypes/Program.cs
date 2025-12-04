// Record Types: Immutable data with value semantics

namespace RecordTypes;

public class Program
{
    public static void Main()
    {
        Console.WriteLine("=== Record Types Demo ===\n");

        // ❌ BAD: Class with manual equality
        Console.WriteLine("❌ Class - Reference equality:");
        var class1 = new PersonClass("John", 30);
        var class2 = new PersonClass("John", 30);
        Console.WriteLine($"Same data, equal? {class1 == class2}"); // False!

        // ✅ GOOD: Record - Value equality
        Console.WriteLine("\n✅ Record - Value equality:");
        var record1 = new PersonRecord("John", 30);
        var record2 = new PersonRecord("John", 30);
        Console.WriteLine($"Same data, equal? {record1 == record2}"); // True!

        // Record features
        Console.WriteLine("\n✅ Record Features:");

        // Deconstruction
        var (name, age) = record1;
        Console.WriteLine($"Deconstructed: {name}, {age}");

        // With expressions (non-destructive mutation)
        var older = record1 with { Age = 31 };
        Console.WriteLine($"Original: {record1}");
        Console.WriteLine($"Modified copy: {older}");

        // ToString override
        Console.WriteLine($"ToString: {record1}");

        // Record struct (C# 10+)
        Console.WriteLine("\n✅ Record Struct:");
        var point1 = new Point(10, 20);
        var point2 = new Point(10, 20);
        Console.WriteLine($"Points equal: {point1 == point2}");

        Console.WriteLine("\n=== Record Types Applied ===");
    }
}

// ❌ Class: Reference equality
public class PersonClass
{
    public string Name { get; }
    public int Age { get; }

    public PersonClass(string name, int age)
    {
        Name = name;
        Age = age;
    }

    // Need to manually override Equals, GetHashCode, ==, !=, ToString...
}

// ✅ Record: Value equality, immutability, concise syntax
public record PersonRecord(string Name, int Age);

// Expanded form (same as above):
public record PersonRecordExpanded
{
    public string Name { get; init; }
    public int Age { get; init; }

    public PersonRecordExpanded(string name, int age)
    {
        Name = name;
        Age = age;
    }
}

// Record with additional members
public record Product(string Name, decimal Price)
{
    public decimal PriceWithTax => Price * 1.20m; // Computed property

    public bool IsExpensive() => Price > 100; // Method
}

// Record inheritance
public record Employee(string Name, int Age, string Department)
    : PersonRecord(Name, Age);

// Record struct (C# 10+) - Value type with record features
public readonly record struct Point(int X, int Y);

// Mutable record struct
public record struct MutablePoint(int X, int Y);

// Use cases demonstration
public class UseCases
{
    // DTOs (Data Transfer Objects)
    public record UserDto(int Id, string Username, string Email);

    // API responses
    public record ApiResponse<T>(bool Success, T? Data, string? Error);

    // Domain events
    public record OrderPlacedEvent(int OrderId, DateTime Timestamp, decimal Total);

    // Configuration
    public record AppSettings(string DatabaseConnection, int MaxRetries);

    // Example usage
    public void Demo()
    {
        var user = new UserDto(1, "john_doe", "john@example.com");

        var success = new ApiResponse<UserDto>(true, user, null);
        var error = new ApiResponse<UserDto>(false, null, "User not found");

        var @event = new OrderPlacedEvent(123, DateTime.UtcNow, 99.99m);

        // With expressions
        var updatedUser = user with { Email = "newemail@example.com" };

        // Pattern matching with records
        var message = success switch
        {
            { Success: true, Data: not null } => $"User: {success.Data.Username}",
            { Success: false, Error: var err } => $"Error: {err}",
            _ => "Unknown"
        };
    }
}

// COMPARISON
// Feature              | Class  | Record | Record Struct
// ---------------------|--------|--------|---------------
// Value equality       | Manual | ✅     | ✅
// Immutable by default | No     | ✅     | No
// With expressions     | No     | ✅     | ✅
// Deconstruction       | Manual | ✅     | ✅
// ToString override    | Manual | ✅     | ✅
// Heap allocation      | Yes    | Yes    | No
