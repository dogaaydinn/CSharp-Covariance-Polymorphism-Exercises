# 5. Implement Custom Source Generators

**Status:** ✅ Accepted

**Date:** 2024-12-01

**Deciders:** Architecture Team, Compiler Team

**Technical Story:** Implementation in `src/AdvancedConcepts.SourceGenerators`

---

## Context and Problem Statement

C# Source Generators allow compile-time code generation, reducing runtime reflection overhead and enabling advanced metaprogramming scenarios. We need to decide whether to implement custom source generators for repetitive patterns like:
- AutoMapper mappings
- Repository boilerplate
- Data validation
- Serialization code
- Builder patterns

**Traditional approach problems:**
- Reflection at runtime adds overhead (boxing, dynamic dispatch)
- Manual boilerplate code is error-prone
- Generic constraints can't express all patterns
- Runtime code generation (Expression trees) is complex

**We need a solution that:**
- Generates code at compile-time
- Eliminates runtime reflection
- Provides IntelliSense support
- Catches errors at compile-time

---

## Decision Drivers

* **Performance** - Eliminate reflection overhead
* **Developer Experience** - Reduce boilerplate code
* **Type Safety** - Catch errors at compile-time, not runtime
* **Maintainability** - Generated code is visible and debuggable
* **Modern C#** - Leverage cutting-edge .NET features

---

## Considered Options

* **Option 1** - Custom Source Generators (.NET 5+)
* **Option 2** - Reflection at runtime (traditional approach)
* **Option 3** - T4 Templates (legacy code generation)
* **Option 4** - Third-party code generators (AutoMapper.SourceGenerator, etc.)

---

## Decision Outcome

**Chosen option:** "Custom Source Generators", because they provide compile-time code generation with zero runtime overhead, full IntelliSense support, and complete control over generated code.

### Positive Consequences

* **Zero Reflection Overhead** - All code generated at compile-time
* **IntelliSense Support** - Generated code is visible to IDE
* **Compile-Time Errors** - Catch issues during build, not runtime
* **AOT Compatible** - Works with Native AOT compilation
* **Educational Value** - Demonstrates advanced compiler concepts
* **Performance Gains** - 10-100x faster than reflection for mapping/serialization

### Negative Consequences

* **Complex Implementation** - Roslyn API has steep learning curve
* **Build Time Impact** - Adds time to compilation (usually < 1 second)
* **Debugging Challenges** - Generated code can be hard to debug
* **Breaking Changes** - Roslyn APIs can change between versions
* **Tooling Requirements** - Requires .NET 5+ SDK

---

## Pros and Cons of the Options

### Custom Source Generators (Chosen)

**What Are Source Generators?**

Source generators are compiler plugins that run during compilation and can inspect your code and generate additional C# files that are compiled alongside your project.

**Architecture:**
```
Your Code (*.cs)
    ↓
Roslyn Compiler
    ↓
Source Generator (analyzes syntax trees)
    ↓
Generated Code (*.g.cs)
    ↓
Final Compilation (your code + generated code)
```

**Pros:**
* **Compile-time execution** - Zero runtime overhead
* **Type-safe** - Generated code is checked by compiler
* **IntelliSense support** - Can navigate to generated code
* **AOT compatible** - Works with Native AOT and trimming
* **Incremental** - Only regenerates when source changes
* **No dependencies at runtime** - Generator is dev-only dependency

**Cons:**
* **Complex to write** - Roslyn syntax trees are difficult
* **Slow iteration** - Must rebuild to see generated code
* **Versioning issues** - Roslyn APIs change
* **Limited diagnostics** - Debugging generators is hard

**Example Implementation:**

```csharp
// MapFrom attribute (user code)
[AttributeUsage(AttributeTargets.Class)]
public class AutoMapAttribute : Attribute
{
    public Type SourceType { get; }
    public AutoMapAttribute(Type sourceType) => SourceType = sourceType;
}

// User code
public class ProductDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
}

[AutoMap(typeof(Product))]
public partial class ProductViewModel
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string DisplayPrice { get; set; } // Computed property
}

// Source Generator (simplified)
[Generator]
public class AutoMapGenerator : ISourceGenerator
{
    public void Execute(GeneratorExecutionContext context)
    {
        // Find all classes with [AutoMap] attribute
        var classesToGenerate = FindClassesWithAutoMapAttribute(context);

        foreach (var classSymbol in classesToGenerate)
        {
            var sourceType = GetSourceType(classSymbol);
            var source = GenerateMapperMethod(classSymbol, sourceType);

            // Add generated code to compilation
            context.AddSource($"{classSymbol.Name}.Mapper.g.cs", source);
        }
    }

    private string GenerateMapperMethod(INamedTypeSymbol target, INamedTypeSymbol source)
    {
        return $@"
public partial class {target.Name}
{{
    public static {target.Name} MapFrom({source.Name} source)
    {{
        return new {target.Name}
        {{
            Id = source.Id,
            Name = source.Name,
            DisplayPrice = $""${{{source.Name}.Price:F2}}""
        }};
    }}
}}";
    }
}

// Generated code (ProductViewModel.Mapper.g.cs)
public partial class ProductViewModel
{
    public static ProductViewModel MapFrom(Product source)
    {
        return new ProductViewModel
        {
            Id = source.Id,
            Name = source.Name,
            DisplayPrice = $"${source.Price:F2}"
        };
    }
}

// Usage (no reflection!)
var product = new Product { Id = 1, Name = "Widget", Price = 29.99m };
var viewModel = ProductViewModel.MapFrom(product); // Compile-time generated method!
```

**Performance Comparison:**
```csharp
// Reflection-based mapping (traditional)
public T Map<T>(object source) where T : new()
{
    var target = new T();
    foreach (var sourceProp in source.GetType().GetProperties())
    {
        var targetProp = typeof(T).GetProperty(sourceProp.Name);
        targetProp?.SetValue(target, sourceProp.GetValue(source));
    }
    return target;
}
// Benchmark: ~500ns per mapping, allocates ~2KB

// Source Generator mapping
var viewModel = ProductViewModel.MapFrom(product);
// Benchmark: ~5ns per mapping, allocates ~64 bytes (object only)
// 100x faster, 30x less memory!
```

### Reflection at Runtime (Traditional)

**Pros:**
* Flexible - works with any type
* No code generation needed
* Easy to understand

**Cons:**
* **Slow** - 10-100x slower than generated code
* **Boxing** - Value types get boxed to object
* **No compile-time checks** - Errors discovered at runtime
* **AOT incompatible** - Doesn't work with Native AOT
* **No IntelliSense** - Properties accessed by string names

**Example (what we avoid):**
```csharp
// Reflection-based mapper (slow)
public class ReflectionMapper
{
    public TTarget Map<TSource, TTarget>(TSource source)
        where TTarget : new()
    {
        var target = new TTarget();
        var sourceProps = typeof(TSource).GetProperties(); // ❌ Slow!
        var targetProps = typeof(TTarget).GetProperties(); // ❌ Slow!

        foreach (var sourceProp in sourceProps)
        {
            var targetProp = targetProps.FirstOrDefault(p => p.Name == sourceProp.Name);
            if (targetProp != null && targetProp.CanWrite)
            {
                var value = sourceProp.GetValue(source); // ❌ Reflection!
                targetProp.SetValue(target, value);      // ❌ Reflection!
            }
        }
        return target;
    }
}

// Usage
var mapper = new ReflectionMapper();
var viewModel = mapper.Map<Product, ProductViewModel>(product);
// ❌ No compile-time type checking
// ❌ Property name typos not caught
// ❌ 100x slower than source generator
```

**Why Rejected:**
Reflection has legitimate uses (plugin systems, serializers), but for **known types at compile-time**, source generators are superior in every way.

### T4 Templates (Legacy)

**What is T4?**
Text Template Transformation Toolkit - Visual Studio's legacy code generation system.

**Pros:**
* Integrated with Visual Studio
* Supports any text output
* Can generate multiple files

**Cons:**
* **Legacy technology** - No longer actively developed
* **No IntelliSense in templates** - T4 syntax is painful
* **VS-only** - Doesn't work in VS Code or Rider
* **Manual execution** - Doesn't auto-regenerate on build
* **No incremental compilation** - Regenerates everything

**Example:**
```xml
<#@ template language="C#" #>
<#@ output extension=".cs" #>
<#
    var types = new[] { "Product", "Order", "Customer" };
#>
public static class Mappers
{
<# foreach (var type in types) { #>
    public static <#= type #>Dto MapFrom(<#= type #> source) { ... }
<# } #>
}
```

**Why Rejected:**
T4 templates are legacy technology. Source Generators provide the same benefits with better IDE integration and automatic regeneration.

### Third-Party Generators

**Examples:**
- AutoMapper.SourceGenerator
- Mapperly
- StronglyTypedId

**Pros:**
* Ready to use out-of-the-box
* Well-tested
* Community support

**Cons:**
* **Limited customization** - Can't adapt to specific needs
* **External dependency** - Another NuGet package
* **Learning curve** - Need to learn library-specific syntax
* **May not cover all use cases**

**Why Not Primary Choice:**
Third-party generators are excellent for common scenarios, but for **educational purposes** and **custom patterns specific to our domain**, implementing our own generators demonstrates deeper understanding.

---

## Real-World Source Generator Examples

### Example 1: Repository Boilerplate Generator

**Problem:** Every repository has the same CRUD methods.

**Attribute:**
```csharp
[GenerateRepository]
public partial class ProductRepository
{
    // Source generator will add:
    // - GetByIdAsync
    // - GetAllAsync
    // - AddAsync
    // - UpdateAsync
    // - DeleteAsync
}
```

**Generated Code:**
```csharp
public partial class ProductRepository : IProductRepository
{
    private readonly ApplicationDbContext _context;

    public async Task<Product?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        return await _context.Products
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id, ct);
    }

    public async Task<List<Product>> GetAllAsync(CancellationToken ct = default)
    {
        return await _context.Products
            .AsNoTracking()
            .ToListAsync(ct);
    }

    public async Task<Product> AddAsync(Product entity, CancellationToken ct = default)
    {
        _context.Products.Add(entity);
        await _context.SaveChangesAsync(ct);
        return entity;
    }

    // UpdateAsync, DeleteAsync...
}
```

### Example 2: Builder Pattern Generator

**Problem:** Writing builder patterns is tedious.

**Attribute:**
```csharp
[GenerateBuilder]
public partial class Product
{
    public int Id { get; init; }
    public string Name { get; init; }
    public decimal Price { get; init; }
}
```

**Generated Code:**
```csharp
public partial class Product
{
    public class Builder
    {
        private int _id;
        private string _name = string.Empty;
        private decimal _price;

        public Builder WithId(int value) { _id = value; return this; }
        public Builder WithName(string value) { _name = value; return this; }
        public Builder WithPrice(decimal value) { _price = value; return this; }

        public Product Build() => new()
        {
            Id = _id,
            Name = _name,
            Price = _price
        };
    }
}

// Usage
var product = new Product.Builder()
    .WithId(1)
    .WithName("Widget")
    .WithPrice(29.99m)
    .Build();
```

### Example 3: Validation Generator

**Problem:** Writing validators is repetitive.

**Attribute:**
```csharp
public class Product
{
    [Required, StringLength(100)]
    public string Name { get; set; }

    [Range(0.01, 1000000)]
    public decimal Price { get; set; }
}
```

**Generated Code:**
```csharp
public partial class ProductValidator
{
    public static ValidationResult Validate(Product product)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(product.Name))
            errors.Add("Name is required");
        else if (product.Name.Length > 100)
            errors.Add("Name must be 100 characters or less");

        if (product.Price < 0.01m || product.Price > 1000000m)
            errors.Add("Price must be between $0.01 and $1,000,000");

        return new ValidationResult(errors.Count == 0, errors);
    }
}
```

---

## Implementation Guidelines

### Project Structure

```
Solution
├── YourApp (references SourceGenerators as analyzer)
├── YourApp.SourceGenerators (generator implementation)
│   ├── Generators/
│   │   ├── AutoMapGenerator.cs
│   │   ├── RepositoryGenerator.cs
│   │   └── BuilderGenerator.cs
│   ├── Attributes/ (included in generator output)
│   │   ├── AutoMapAttribute.cs
│   │   └── GenerateBuilderAttribute.cs
│   └── YourApp.SourceGenerators.csproj
└── YourApp.SourceGenerators.Tests
```

### Generator Project Configuration

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>netstandard2.0</TargetFramework>
    <EnforceExtendedAnalyzerRules>true</EnforceExtendedAnalyzerRules>
    <IsRoslynComponent>true</IsRoslynComponent>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Microsoft.CodeAnalysis.CSharp" Version="4.8.0" PrivateAssets="all" />
    <PackageReference Include="Microsoft.CodeAnalysis.Analyzers" Version="3.3.4" PrivateAssets="all" />
  </ItemGroup>
</Project>
```

### Consuming Project Configuration

```xml
<ItemGroup>
  <ProjectReference Include="..\YourApp.SourceGenerators\YourApp.SourceGenerators.csproj"
                    OutputItemType="Analyzer"
                    ReferenceOutputAssembly="false" />
</ItemGroup>

<!-- Optional: See generated files -->
<PropertyGroup>
  <EmitCompilerGeneratedFiles>true</EmitCompilerGeneratedFiles>
  <CompilerGeneratedFilesOutputPath>$(BaseIntermediateOutputPath)Generated</CompilerGeneratedFilesOutputPath>
</PropertyGroup>
```

---

## Performance Benchmarks

```
BenchmarkDotNet v0.13.11, Windows 11
Intel Core i7-12700K, 1 CPU, 20 logical cores

|                Method |      Mean |     Error |    StdDev |   Gen0 | Allocated |
|---------------------- |----------:|----------:|----------:|-------:|----------:|
| ReflectionMapper      | 512.3 ns  |  8.21 ns  |  7.68 ns  | 0.0815 |    2048 B |
| ExpressionTreeMapper  | 127.6 ns  |  1.94 ns  |  1.81 ns  | 0.0153 |     384 B |
| SourceGeneratorMapper |   4.8 ns  |  0.07 ns  |  0.06 ns  | 0.0026 |      64 B |

Conclusion: Source Generator is 100x faster and uses 30x less memory than reflection
```

---

## Links

* [Source Generators Cookbook](https://github.com/dotnet/roslyn/blob/main/docs/features/source-generators.cookbook.md)
* [Source Generators Documentation](https://learn.microsoft.com/en-us/dotnet/csharp/roslyn-sdk/source-generators-overview)
* [Roslyn API Documentation](https://learn.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis)
* [Sample Implementation](../../src/AdvancedConcepts.SourceGenerators)
* [Andrew Lock's Source Generator Series](https://andrewlock.net/series/creating-a-source-generator/)

---

## Notes

**When to Use Source Generators:**
- ✅ Eliminating boilerplate for known types at compile-time
- ✅ High-performance scenarios where reflection is too slow
- ✅ Native AOT applications
- ✅ Code that needs compile-time validation
- ✅ Repetitive patterns (DTOs, builders, repositories)

**When NOT to Use:**
- ❌ Dynamic types only known at runtime
- ❌ Simple one-off code generation (use snippets)
- ❌ Plugin systems where types are loaded dynamically
- ❌ When third-party generators already solve the problem

**Common Pitfalls:**
1. **Not making classes partial** - Generated code extends class, must be partial
2. **Forgetting netstandard2.0** - Generators must target netstandard2.0
3. **Slow generators** - Generator runs on every compilation, keep it fast
4. **Not handling incremental compilation** - Implement IIncrementalGenerator for performance
5. **Complex logic in generators** - Keep generators simple, move logic to runtime

**Debugging Tips:**
```csharp
// View generated files
// obj/Debug/net8.0/generated/YourApp.SourceGenerators/...

// Debug source generator
#if DEBUG
    if (!System.Diagnostics.Debugger.IsAttached)
    {
        System.Diagnostics.Debugger.Launch();
    }
#endif
```

**Review Date:** 2025-12-01
