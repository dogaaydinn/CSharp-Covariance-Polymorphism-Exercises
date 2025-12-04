using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace NativeAOT;

/// <summary>
/// Native AOT Tutorial - Ahead-of-Time Compilation
/// Demonstrates Native AOT features, limitations, and optimizations
/// </summary>
class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("═══════════════════════════════════════════════════════════════════");
        Console.WriteLine("NATIVE AOT TUTORIAL");
        Console.WriteLine("Ahead-of-Time Compilation for .NET");
        Console.WriteLine("═══════════════════════════════════════════════════════════════════\n");

        WhatIsNativeAOT();
        ReflectionIssuesDemo();
        JsonSerializationWorkarounds();
        TrimWarningsDemo();
        PerformanceComparison();
        BestPractices();

        Console.WriteLine("\n\n═══════════════════════════════════════════════════════════════════");
        Console.WriteLine("Tutorial Complete!");
        Console.WriteLine("═══════════════════════════════════════════════════════════════════");
    }

    #region 1. What is Native AOT?

    static void WhatIsNativeAOT()
    {
        Console.WriteLine("\n1. WHAT IS NATIVE AOT?");
        Console.WriteLine("─────────────────────────────────────────────────────────────────\n");

        Console.WriteLine("Native AOT (Ahead-of-Time) compiles C# to native machine code.");
        Console.WriteLine("\nTraditional .NET:");
        Console.WriteLine("  C# → IL (Intermediate Language) → JIT compiles at runtime");
        Console.WriteLine("  Requires .NET Runtime installed");
        Console.WriteLine("  Slower startup, larger deployment");

        Console.WriteLine("\nNative AOT:");
        Console.WriteLine("  C# → Native machine code at build time");
        Console.WriteLine("  No .NET Runtime needed (self-contained)");
        Console.WriteLine("  ✅ Faster startup (instant!)");
        Console.WriteLine("  ✅ Lower memory usage");
        Console.WriteLine("  ✅ Smaller deployment size");
        Console.WriteLine("  ❌ No runtime reflection");
        Console.WriteLine("  ❌ No dynamic code generation");

        Console.WriteLine("\nBest Use Cases:");
        Console.WriteLine("  • Microservices (fast cold start)");
        Console.WriteLine("  • CLI tools (instant startup)");
        Console.WriteLine("  • Cloud functions (serverless)");
        Console.WriteLine("  • IoT / Edge devices");
        Console.WriteLine("  • Containers (smaller images)");
    }

    #endregion

    #region 2. Reflection Issues

    static void ReflectionIssuesDemo()
    {
        Console.WriteLine("\n\n2. REFLECTION LIMITATIONS");
        Console.WriteLine("─────────────────────────────────────────────────────────────────\n");

        Console.WriteLine("Native AOT cannot use runtime reflection!");
        Console.WriteLine("\n❌ This code would FAIL with Native AOT:");
        Console.WriteLine("  var type = Type.GetType(\"MyNamespace.MyClass\");");
        Console.WriteLine("  var instance = Activator.CreateInstance(type);");
        Console.WriteLine("  var method = type.GetMethod(\"MyMethod\");");
        Console.WriteLine("  method.Invoke(instance, null);");

        Console.WriteLine("\nWhy? Native AOT needs to know all types at compile time.");
        Console.WriteLine("Runtime reflection means unknown types = trimmed away!");

        // Example: Working without reflection
        Console.WriteLine("\n✅ This works (compile-time known):");
        var person = new Person { Name = "Alice", Age = 30 };
        Console.WriteLine($"  Created person: {person.Name}, Age {person.Age}");

        // Simulate what would fail
        Console.WriteLine("\n❌ What DOESN'T work:");
        Console.WriteLine("  • Type.GetType() for unknown types");
        Console.WriteLine("  • Activator.CreateInstance() with reflection");
        Console.WriteLine("  • Assembly.Load() at runtime");
        Console.WriteLine("  • Dynamic code generation (Reflection.Emit)");
    }

    #endregion

    #region 3. JSON Serialization Workarounds

    static void JsonSerializationWorkarounds()
    {
        Console.WriteLine("\n\n3. JSON SERIALIZATION IN NATIVE AOT");
        Console.WriteLine("─────────────────────────────────────────────────────────────────\n");

        Console.WriteLine("System.Text.Json uses reflection by default.");
        Console.WriteLine("Solution: Source Generators!\n");

        // ❌ Old way (doesn't work with AOT)
        Console.WriteLine("❌ Old way (uses reflection):");
        Console.WriteLine("  var json = JsonSerializer.Serialize(person);");
        Console.WriteLine("  // Fails with AOT!");

        // ✅ New way (works with AOT)
        Console.WriteLine("\n✅ New way (source generator):");
        Console.WriteLine("  [JsonSerializable(typeof(Person))]");
        Console.WriteLine("  partial class MyJsonContext : JsonSerializerContext { }");
        Console.WriteLine("  ");
        Console.WriteLine("  var json = JsonSerializer.Serialize(person, MyJsonContext.Default.Person);");

        var person = new Person { Name = "Bob", Age = 25 };
        
        try
        {
            // This uses source generator (AOT-compatible)
            string json = JsonSerializer.Serialize(person, MyJsonContext.Default.Person);
            Console.WriteLine($"\n  Serialized: {json}");

            Person? deserialized = JsonSerializer.Deserialize(json, MyJsonContext.Default.Person);
            Console.WriteLine($"  Deserialized: {deserialized?.Name}, Age {deserialized?.Age}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"  Error: {ex.Message}");
        }

        Console.WriteLine("\nKey Points:");
        Console.WriteLine("  ✓ Use [JsonSerializable] attribute");
        Console.WriteLine("  ✓ Create a JsonSerializerContext");
        Console.WriteLine("  ✓ Pass context to Serialize/Deserialize");
        Console.WriteLine("  ✓ Source generator creates AOT-compatible code");
    }

    #endregion

    #region 4. Trim Warnings

    static void TrimWarningsDemo()
    {
        Console.WriteLine("\n\n4. TRIMMING ANALYSIS");
        Console.WriteLine("─────────────────────────────────────────────────────────────────\n");

        Console.WriteLine("Native AOT trims unused code to reduce binary size.");
        Console.WriteLine("\nTrimming Process:");
        Console.WriteLine("  1. Analyzes code from entry point (Main)");
        Console.WriteLine("  2. Marks all reachable code");
        Console.WriteLine("  3. Removes unreachable code");
        Console.WriteLine("  4. Warns about potentially unsafe trims");

        Console.WriteLine("\n⚠️ Common Trim Warnings:");
        Console.WriteLine("  IL2026: Using member with RequiresUnreferencedCode");
        Console.WriteLine("  IL2067: Using value with DynamicallyAccessedMembers");
        Console.WriteLine("  IL2070: 'this' parameter cannot satisfy requirements");

        Console.WriteLine("\nHow to Fix:");
        Console.WriteLine("  1. Use [DynamicallyAccessedMembers] attributes");
        Console.WriteLine("  2. Use [RequiresUnreferencedCode] to suppress");
        Console.WriteLine("  3. Replace reflection with source generators");
        Console.WriteLine("  4. Use [UnconditionalSuppressMessage] if safe");

        Console.WriteLine("\nExample:");
        Console.WriteLine("  [RequiresUnreferencedCode(\"Uses reflection\")]");
        Console.WriteLine("  public void MyReflectionMethod() { ... }");
    }

    #endregion

    #region 5. Performance Comparison

    static void PerformanceComparison()
    {
        Console.WriteLine("\n\n5. PERFORMANCE COMPARISON");
        Console.WriteLine("─────────────────────────────────────────────────────────────────\n");

        Console.WriteLine("Typical Performance Improvements:\n");

        Console.WriteLine("STARTUP TIME:");
        Console.WriteLine("  Traditional .NET:  ~500ms");
        Console.WriteLine("  Native AOT:        ~5ms");
        Console.WriteLine("  Improvement:       100x faster! 🚀");

        Console.WriteLine("\nMEMORY USAGE:");
        Console.WriteLine("  Traditional .NET:  ~30-50 MB");
        Console.WriteLine("  Native AOT:        ~5-10 MB");
        Console.WriteLine("  Improvement:       5x less memory! 💾");

        Console.WriteLine("\nBINARY SIZE:");
        Console.WriteLine("  Traditional .NET:  ~200+ MB (with runtime)");
        Console.WriteLine("  Native AOT:        ~5-15 MB (self-contained)");
        Console.WriteLine("  Improvement:       10-40x smaller! 📦");

        Console.WriteLine("\nEXECUTION SPEED:");
        Console.WriteLine("  Traditional .NET:  Baseline");
        Console.WriteLine("  Native AOT:        Similar (±10%)");
        Console.WriteLine("  Note:              AOT is optimized at build time");

        Console.WriteLine("\nReal-World Impact:");
        Console.WriteLine("  • Docker images: 200MB → 15MB");
        Console.WriteLine("  • Cold start (serverless): 500ms → 5ms");
        Console.WriteLine("  • Memory per instance: 50MB → 10MB");
        Console.WriteLine("  • Result: 5x more instances on same hardware!");
    }

    #endregion

    #region 6. Best Practices

    static void BestPractices()
    {
        Console.WriteLine("\n\n6. NATIVE AOT BEST PRACTICES");
        Console.WriteLine("─────────────────────────────────────────────────────────────────\n");

        Console.WriteLine("✅ DO:");
        Console.WriteLine("  • Use source generators instead of reflection");
        Console.WriteLine("  • Use JsonSerializerContext for JSON");
        Console.WriteLine("  • Test with PublishAot=true early");
        Console.WriteLine("  • Review trim warnings carefully");
        Console.WriteLine("  • Use [DynamicallyAccessedMembers] attributes");
        Console.WriteLine("  • Prefer compile-time known types");
        Console.WriteLine("  • Use System.Text.Json (not Newtonsoft.Json)");

        Console.WriteLine("\n❌ DON'T:");
        Console.WriteLine("  • Don't use Type.GetType() with strings");
        Console.WriteLine("  • Don't use Activator.CreateInstance() dynamically");
        Console.WriteLine("  • Don't load assemblies at runtime");
        Console.WriteLine("  • Don't use Reflection.Emit");
        Console.WriteLine("  • Don't ignore trim warnings");
        Console.WriteLine("  • Don't use COM interop (Windows only)");

        Console.WriteLine("\n🎯 WHEN TO USE NATIVE AOT:");
        Console.WriteLine("  ✅ Microservices (fast startup)");
        Console.WriteLine("  ✅ CLI tools (instant launch)");
        Console.WriteLine("  ✅ Cloud functions / Serverless");
        Console.WriteLine("  ✅ Containers / Docker");
        Console.WriteLine("  ✅ IoT / Edge devices");

        Console.WriteLine("\n⚠️ WHEN NOT TO USE:");
        Console.WriteLine("  ❌ Heavy reflection usage");
        Console.WriteLine("  ❌ Dynamic plugin systems");
        Console.WriteLine("  ❌ Code that generates code at runtime");
        Console.WriteLine("  ❌ Desktop apps (startup time less critical)");
        Console.WriteLine("  ❌ Apps using Newtonsoft.Json heavily");

        Console.WriteLine("\n📦 PUBLISHING:");
        Console.WriteLine("  dotnet publish -c Release -r win-x64");
        Console.WriteLine("  dotnet publish -c Release -r linux-x64");
        Console.WriteLine("  dotnet publish -c Release -r osx-arm64");
        Console.WriteLine("\n  Result: Single .exe file, no .NET runtime needed!");
    }

    #endregion

    // Example classes for demonstrations
    public class Person
    {
        public string Name { get; set; } = "";
        public int Age { get; set; }
    }
}

// JSON Source Generator Context (AOT-compatible)
[JsonSerializable(typeof(NativeAOT.Program.Person))]
partial class MyJsonContext : JsonSerializerContext
{
}
