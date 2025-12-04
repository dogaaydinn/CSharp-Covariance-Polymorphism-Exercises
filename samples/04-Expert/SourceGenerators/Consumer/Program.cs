using Consumer;

// ═══════════════════════════════════════════════════════════════════════════
// ROSLYN SOURCE GENERATORS - Auto-Generate Code at Compile Time
// ═══════════════════════════════════════════════════════════════════════════
//
// PROBLEM: Repetitive boilerplate code (ToString, Equals, etc.)
// SOLUTION: Source generators create code during compilation
//
// BENEFITS:
// ✅ Zero runtime overhead (code generated at compile time)
// ✅ Type-safe (full Roslyn semantic analysis)
// ✅ IDE-friendly (IntelliSense sees generated code)
// ✅ No reflection needed (unlike runtime code generation)
//
// WHEN TO USE:
// - Reducing boilerplate (ToString, serialization)
// - Code generation based on attributes
// - Performance-critical scenarios (no reflection)
// - Creating DSLs or strongly-typed APIs
//
// ═══════════════════════════════════════════════════════════════════════════

Console.WriteLine("🔧 ROSLYN SOURCE GENERATORS - ToString Auto-Generation\n");

// ═══ EXAMPLE 1: Person class with auto-generated ToString ═══
Console.WriteLine("═══ Example 1: Person ═══");

var person = new Person("John Doe", 30, "john@example.com");

// ❌ WITHOUT SOURCE GENERATOR:
// ToString() would return: "Consumer.Person" (default object.ToString())
//
// ✅ WITH SOURCE GENERATOR:
// ToString() returns: "Person { Name = John Doe, Age = 30, Email = john@example.com }"

Console.WriteLine(person.ToString());
// Output: Person { Name = John Doe, Age = 30, Email = john@example.com }

Console.WriteLine();

// ═══ EXAMPLE 2: Product class ═══
Console.WriteLine("═══ Example 2: Product ═══");

var product = new Product("Laptop", 999.99m);
Console.WriteLine(product.ToString());
// Output: Product { Name = Laptop, Price = 999.99 }

Console.WriteLine();

// ═══ EXAMPLE 3: Order class with DateTime ═══
Console.WriteLine("═══ Example 3: Order ═══");

var order = new Order(1001, "Alice Smith", 1500.50m, DateTime.Now);
Console.WriteLine(order.ToString());
// Output: Order { OrderId = 1001, CustomerName = Alice Smith, TotalAmount = 1500.50, OrderDate = 2024-12-04 ... }

Console.WriteLine();

// ═══════════════════════════════════════════════════════════════════════════
// HOW IT WORKS UNDER THE HOOD
// ═══════════════════════════════════════════════════════════════════════════
//
// COMPILE TIME:
// 1. Compiler invokes ToStringGenerator
// 2. Generator scans syntax tree for [GenerateToString] attributes
// 3. Generator creates Person.g.cs with ToString() implementation
// 4. Generated file is compiled with your code
//
// GENERATED CODE (Person.g.cs):
// ```csharp
// partial class Person
// {
//     public override string ToString()
//     {
//         return $"Person { Name = {Name}, Age = {Age}, Email = {Email} }";
//     }
// }
// ```
//
// ═══════════════════════════════════════════════════════════════════════════

Console.WriteLine("═══ Comparison: Manual vs Generated ═══");

// ❌ BAD: Manual ToString (repetitive, error-prone)
/*
public class PersonManual
{
    public string Name { get; set; }
    public int Age { get; set; }
    public string Email { get; set; }

    public override string ToString()  // ← Must write this for EVERY class
    {
        return $"Person {{ Name = {Name}, Age = {Age}, Email = {Email} }}";
    }
}
*/

// ✅ GOOD: Source generator (write once, use everywhere)
/*
[GenerateToString]  // ← Just add attribute, generator does the rest
public partial class Person
{
    public string Name { get; set; }
    public int Age { get; set; }
    public string Email { get; set; }
}
*/

Console.WriteLine("✅ No manual ToString() needed!");
Console.WriteLine("✅ Consistent format across all classes");
Console.WriteLine("✅ Changes to properties automatically reflected");

Console.WriteLine();

// ═══════════════════════════════════════════════════════════════════════════
// PERFORMANCE NOTES
// ═══════════════════════════════════════════════════════════════════════════
//
// BENCHMARK COMPARISON:
//                          | Manual ToString | Source Generator | Reflection
// -------------------------+----------------+-----------------+------------
// Compilation overhead     | None           | ~100-500ms      | None
// Runtime overhead         | None           | None            | ~1000x slower
// Memory allocations       | Minimal        | Minimal         | High
// Type safety              | ✅             | ✅              | ❌
// IDE IntelliSense         | ✅             | ✅              | ❌
//
// KEY INSIGHT: Source generators add ZERO runtime cost
// - Generation happens once at compile time
// - Generated code is identical to hand-written code
// - No reflection or runtime analysis needed
//
// ═══════════════════════════════════════════════════════════════════════════

Console.WriteLine("═══ Performance Benefits ═══");
Console.WriteLine("⚡ Compile-time generation: ~200ms one-time cost");
Console.WriteLine("⚡ Runtime performance: Identical to manual code");
Console.WriteLine("⚡ Memory: No additional allocations");
Console.WriteLine("⚡ Type safety: Full compiler checks");

// ═══════════════════════════════════════════════════════════════════════════
// OUTPUT ANALYSIS
// ═══════════════════════════════════════════════════════════════════════════
//
// 1. [GenerateToString] attribute triggers code generation
// 2. Generator runs during build, creates .g.cs files
// 3. Generated ToString() methods are available at runtime
// 4. No reflection needed - everything is strongly typed
// 5. IDE can show generated code (obj/Debug/net8.0/generated/)
//
// REAL-WORLD USAGE:
// - Entity Framework models (auto-generate navigation properties)
// - JSON serialization (auto-generate serializers)
// - gRPC/Protobuf (generate client code from .proto files)
// - Strongly-typed configuration (generate classes from JSON schemas)
// - Regular expressions (compile regex at build time)
//
// ═══════════════════════════════════════════════════════════════════════════
