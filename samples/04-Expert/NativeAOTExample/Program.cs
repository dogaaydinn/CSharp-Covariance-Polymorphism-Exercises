using System;
using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace NativeAOTExample;

/// <summary>
/// Demonstrates Native AOT compilation for faster startup and smaller binaries.
///
/// Build:
///   dotnet build
///
/// Publish (creates native executable):
///   dotnet publish -c Release
///
/// Compare:
///   - Standard .NET: ~150MB, 200ms startup
///   - Native AOT: ~5-10MB, 5-10ms startup
/// </summary>
class Program
{
    static void Main(string[] args)
    {
        var stopwatch = Stopwatch.StartNew();

        Console.WriteLine("=== Native AOT Example ===\n");

        // 1. Basic operations (AOT-friendly)
        DemonstrateBasicOperations();

        // 2. JSON serialization (using source generator for AOT)
        DemonstrateJsonSerialization();

        // 3. Collection processing
        DemonstrateCollectionOperations();

        stopwatch.Stop();
        Console.WriteLine($"\n✅ Total execution time: {stopwatch.ElapsedMilliseconds}ms");
        Console.WriteLine($"✅ Native AOT enables fast startup and small binary size!");
    }

    static void DemonstrateBasicOperations()
    {
        Console.WriteLine("1. Basic Operations (AOT-Compatible)");

        // ✅ GOOD: Simple, type-safe operations
        var numbers = new[] { 1, 2, 3, 4, 5 };
        var sum = CalculateSum(numbers);
        var product = CalculateProduct(numbers);

        Console.WriteLine($"   Sum: {sum}");
        Console.WriteLine($"   Product: {product}");
    }

    static int CalculateSum(int[] numbers)
    {
        var total = 0;
        foreach (var n in numbers)
            total += n;
        return total;
    }

    static int CalculateProduct(int[] numbers)
    {
        var product = 1;
        foreach (var n in numbers)
            product *= n;
        return product;
    }

    static void DemonstrateJsonSerialization()
    {
        Console.WriteLine("\n2. JSON Serialization (Source Generator)");

        var person = new Person
        {
            Name = "Alice Johnson",
            Age = 30,
            Email = "alice@example.com"
        };

        // ✅ GOOD: Uses source generator (JsonSerializerContext)
        // No reflection needed at runtime - AOT compatible!
        var json = JsonSerializer.Serialize(person, AppJsonContext.Default.Person);
        Console.WriteLine($"   Serialized: {json}");

        var deserialized = JsonSerializer.Deserialize(json, AppJsonContext.Default.Person);
        Console.WriteLine($"   Deserialized: {deserialized?.Name}, Age {deserialized?.Age}");
    }

    static void DemonstrateCollectionOperations()
    {
        Console.WriteLine("\n3. Collection Operations (Zero Allocations)");

        var items = new[] { "Apple", "Banana", "Cherry", "Date", "Elderberry" };

        // ✅ GOOD: Span<T> for zero-allocation processing
        ProcessItemsWithSpan(items);
    }

    static void ProcessItemsWithSpan(string[] items)
    {
        ReadOnlySpan<string> span = items;

        Console.WriteLine($"   Processing {span.Length} items:");
        for (var i = 0; i < span.Length; i++)
        {
            Console.WriteLine($"   - {span[i]} ({span[i].Length} chars)");
        }
    }
}

/// <summary>
/// Person model - simple, AOT-compatible.
/// </summary>
public class Person
{
    public string? Name { get; set; }
    public int Age { get; set; }
    public string? Email { get; set; }
}

/// <summary>
/// JSON serialization context for AOT compilation.
/// Replaces reflection with compile-time code generation.
/// </summary>
[JsonSerializable(typeof(Person))]
[JsonSerializable(typeof(string))]
[JsonSerializable(typeof(int))]
internal partial class AppJsonContext : JsonSerializerContext
{
}
