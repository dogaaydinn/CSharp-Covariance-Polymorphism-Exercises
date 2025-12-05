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
        var sum = Calculator.CalculateSum(numbers);
        var product = Calculator.CalculateProduct(numbers);

        Console.WriteLine($"   Sum: {sum}");
        Console.WriteLine($"   Product: {product}");
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
        CollectionProcessor.ProcessItemsWithSpan(items);
    }
}
