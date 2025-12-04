# Common Mistakes: Source Generators

## ❌ Mistake #1: Forgetting `partial` Keyword

### The Problem

```csharp
[GenerateToString]
public class Person  // ❌ Missing 'partial'
{
    public string Name { get; set; }
}
```

**Error:**
```
error CS0260: Missing partial modifier on declaration of type 'Person';
another partial declaration of this type exists
```

### Why It Happens

Source generators **extend** existing classes. Without `partial`, the compiler sees two separate `Person` classes:
1. Your original class
2. The generated class

### The Fix

```csharp
[GenerateToString]
public partial class Person  // ✅ Added 'partial'
{
    public string Name { get; set; }
}
```

**Rule:** Any class using source generators MUST be `partial`.

---

## ❌ Mistake #2: Generator Not Running

### The Problem

```csharp
[GenerateToString]
public partial class Person
{
    public string Name { get; set; }
}

// ToString() not generated - calls default Object.ToString()
var person = new Person { Name = "John" };
Console.WriteLine(person);  // Outputs: "Consumer.Person" (not generated!)
```

### Common Causes

#### Cause 1: Wrong Project Reference

```xml
<!-- ❌ BAD: Incorrect reference -->
<ProjectReference Include="..\Generator\Generator.csproj" />

<!-- ✅ GOOD: Must specify OutputItemType -->
<ProjectReference Include="..\Generator\Generator.csproj"
                  OutputItemType="Analyzer"          <!-- Required! -->
                  ReferenceOutputAssembly="true" />
```

#### Cause 2: Wrong Target Framework

```xml
<!-- ❌ BAD: Generator targets net8.0 -->
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>  <!-- Wrong! -->
  </PropertyGroup>
</Project>

<!-- ✅ GOOD: Must target netstandard2.0 -->
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>netstandard2.0</TargetFramework>  <!-- Correct! -->
  </PropertyGroup>
</Project>
```

**Why netstandard2.0?** Roslyn runs on netstandard2.0, so generators must too.

#### Cause 3: Missing Roslyn Packages

```xml
<!-- ❌ BAD: Missing required packages -->
<ItemGroup>
  <!-- Nothing here -->
</ItemGroup>

<!-- ✅ GOOD: Required Roslyn packages -->
<ItemGroup>
  <PackageReference Include="Microsoft.CodeAnalysis.CSharp"
                    Version="4.8.0"
                    PrivateAssets="all" />
  <PackageReference Include="Microsoft.CodeAnalysis.Analyzers"
                    Version="3.3.4"
                    PrivateAssets="all" />
</ItemGroup>
```

### The Fix

1. **Clean build output:**
   ```bash
   dotnet clean
   rm -rf bin/ obj/
   ```

2. **Verify project settings:**
   - Generator: `netstandard2.0`
   - Consumer: Reference with `OutputItemType="Analyzer"`
   - Roslyn packages installed

3. **Rebuild:**
   ```bash
   dotnet build
   ```

4. **Check generated files:**
   ```bash
   ls Consumer/obj/Debug/net8.0/generated/Generator/*/
   ```

---

## ❌ Mistake #3: Using Semantic Analysis in Predicates

### The Problem

```csharp
// ❌ BAD: Semantic analysis in predicate (SLOW!)
private static bool IsSyntaxTargetForGeneration(
    SyntaxNode node,
    CancellationToken ct)
{
    if (node is not ClassDeclarationSyntax classDecl)
        return false;

    // Accessing semantic model in predicate - DON'T DO THIS!
    var model = compilation.GetSemanticModel(classDecl.SyntaxTree);
    var symbol = model.GetDeclaredSymbol(classDecl);
    return symbol?.GetAttributes()
        .Any(a => a.AttributeClass?.Name == "GenerateToStringAttribute") ?? false;
}
```

**Performance Impact:**
- Predicate runs on **every syntax node** (10,000+ times)
- Semantic analysis is **100x slower** than syntax checks
- Build time increases from 500ms to 30+ seconds

### Why It Happens

Developers don't understand the two-phase filtering:
1. **Predicate**: Fast syntax-only filter
2. **Transform**: Expensive semantic analysis

### The Fix

```csharp
// ✅ GOOD: Syntax-only predicate
private static bool IsSyntaxTargetForGeneration(SyntaxNode node)
{
    // Quick check: is it a class with at least one attribute?
    return node is ClassDeclarationSyntax { AttributeLists.Count: > 0 };
}

// ✅ GOOD: Semantic analysis in transform
private static ClassDeclarationSyntax? GetSemanticTargetForGeneration(
    GeneratorSyntaxContext context)
{
    var classDecl = (ClassDeclarationSyntax)context.Node;

    // NOW we can use semantic model (only for filtered classes)
    foreach (var attributeList in classDecl.AttributeLists)
    {
        foreach (var attribute in attributeList.Attributes)
        {
            var symbol = context.SemanticModel.GetSymbolInfo(attribute).Symbol;
            // ... check attribute type ...
        }
    }
}
```

---

## ❌ Mistake #4: Not Handling Namespaces

### The Problem

```csharp
// Generator creates code without namespace:
partial class Person
{
    public override string ToString() { ... }
}

// But original class HAS namespace:
namespace MyApp.Models
{
    [GenerateToString]
    public partial class Person { }
}

// Error: Partial declarations must have same namespace!
```

### The Fix

```csharp
private static string GenerateToStringMethod(INamedTypeSymbol classSymbol)
{
    var namespaceName = classSymbol.ContainingNamespace.IsGlobalNamespace
        ? null
        : classSymbol.ContainingNamespace.ToDisplayString();

    var sb = new StringBuilder();

    // ✅ Include namespace if class has one
    if (namespaceName != null)
    {
        sb.AppendLine($"namespace {namespaceName}");
        sb.AppendLine("{");
    }

    sb.AppendLine($"    partial class {classSymbol.Name}");
    sb.AppendLine("    {");
    sb.AppendLine("        public override string ToString() { ... }");
    sb.AppendLine("    }");

    if (namespaceName != null)
    {
        sb.AppendLine("}");
    }

    return sb.ToString();
}
```

---

## ❌ Mistake #5: Generating Duplicate Code

### The Problem

```csharp
// Generator runs twice, creates TWO ToString() methods
partial class Person
{
    public override string ToString() { ... }  // From run #1
    public override string ToString() { ... }  // From run #2 - ERROR!
}
```

**Error:**
```
error CS0111: Type 'Person' already defines a member called 'ToString'
with the same parameter types
```

### Why It Happens

- Generator invoked multiple times
- AddSource() called with same filename twice
- Multiple generators creating same method

### The Fix

```csharp
// ✅ Use unique filenames with .g.cs suffix
context.AddSource(
    $"{classSymbol.Name}.g.cs",  // Unique per class
    SourceText.From(source, Encoding.UTF8));

// ✅ Check if method already exists
var hasToString = classSymbol.GetMembers("ToString")
    .Any(m => m is IMethodSymbol method && method.Parameters.Length == 0);

if (hasToString)
{
    // Don't generate - class already has ToString()
    return;
}
```

---

## ❌ Mistake #6: Not Using `PrivateAssets="all"`

### The Problem

```xml
<!-- ❌ BAD: Missing PrivateAssets -->
<PackageReference Include="Microsoft.CodeAnalysis.CSharp" Version="4.8.0" />
```

**Result:** Roslyn assemblies included in your published application (20+ MB bloat!)

### The Fix

```xml
<!-- ✅ GOOD: Roslyn only needed at build time -->
<PackageReference Include="Microsoft.CodeAnalysis.CSharp"
                  Version="4.8.0"
                  PrivateAssets="all" />  <!-- Don't include in output -->
```

**Effect:** Published app size reduced by 20+ MB.

---

## ❌ Mistake #7: Incorrect String Escaping

### The Problem

```csharp
// ❌ BAD: Generates invalid C# code
var source = "return $\"Person { Name = " + name + " }\";";

// Generated code (BROKEN):
return $"Person { Name = John }";  // Unescaped variable!
```

### The Fix

```csharp
// ✅ GOOD: Proper escaping with interpolated verbatim string
var source = $$"""
    public override string ToString()
    {
        return $"{{ClassName}} { Name = {Name} }";
    }
    """;

// Or use string concatenation carefully:
sb.AppendLine("            return $\"" + className + " { \" +");
sb.AppendLine($"                \"Name = {{Name}}\" +");  // Double {{ for literal {
sb.AppendLine("                \" }\";");
```

---

## ❌ Mistake #8: Generator Crashes IDE

### The Problem

```csharp
// ❌ BAD: Infinite loop in generator
private static bool IsSyntaxTargetForGeneration(SyntaxNode node)
{
    while (true)  // IDE freezes!
    {
        // Oops...
    }
}
```

Or:

```csharp
// ❌ BAD: Throws unhandled exception
var symbol = context.SemanticModel.GetDeclaredSymbol(node);
var name = symbol.Name;  // NullReferenceException if symbol is null!
```

### Why It Happens

- Generators run **on every keystroke** in IDE
- Unhandled exceptions crash generator
- Infinite loops freeze IDE

### The Fix

```csharp
// ✅ GOOD: Defensive programming
private static bool IsSyntaxTargetForGeneration(SyntaxNode node)
{
    try
    {
        // Quick checks only - no loops
        return node is ClassDeclarationSyntax { AttributeLists.Count: > 0 };
    }
    catch
    {
        // Never crash generator
        return false;
    }
}

// ✅ GOOD: Null checks
var symbol = context.SemanticModel.GetDeclaredSymbol(node);
if (symbol is null)
    return null;

var name = symbol.Name;  // Safe now
```

---

## ❌ Mistake #9: Using ISourceGenerator (Legacy)

### The Problem

```csharp
// ❌ BAD: Old API (slow, non-incremental)
[Generator]
public class ToStringGenerator : ISourceGenerator
{
    public void Execute(GeneratorExecutionContext context)
    {
        // Runs on EVERY build, even if nothing changed
    }
}
```

### The Fix

```csharp
// ✅ GOOD: Modern API (fast, incremental)
[Generator]
public class ToStringGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // Only runs when relevant code changes
    }
}
```

**Performance:** IIncrementalGenerator is **10-100x faster** for incremental builds.

---

## ❌ Mistake #10: Not Providing Diagnostics

### The Problem

Generator silently fails, developers have no idea why:

```csharp
// Generator ignores this - no error shown
[GenerateToString]
public class Person  // ❌ Missing 'partial' but no diagnostic!
{
    public string Name { get; set; }
}
```

### The Fix

```csharp
// ✅ GOOD: Report helpful diagnostics
private static void Execute(...)
{
    foreach (var classDecl in classes)
    {
        var classSymbol = compilation.GetSemanticModel(classDecl.SyntaxTree)
            .GetDeclaredSymbol(classDecl);

        // Check if class is partial
        if (!classDecl.Modifiers.Any(m => m.IsKind(SyntaxKind.PartialKeyword)))
        {
            context.ReportDiagnostic(Diagnostic.Create(
                new DiagnosticDescriptor(
                    "SG001",
                    "Class must be partial",
                    "Class '{0}' has [GenerateToString] but is not partial. Add 'partial' keyword.",
                    "SourceGenerator",
                    DiagnosticSeverity.Error,
                    isEnabledByDefault: true),
                classDecl.GetLocation(),
                classSymbol.Name));

            continue;  // Skip generation
        }

        // ... generate code ...
    }
}
```

**Result:** Clear error message in IDE and build output!

---

## 🎯 Best Practices Checklist

- [ ] Always use `partial` keyword on generated classes
- [ ] Reference generator with `OutputItemType="Analyzer"`
- [ ] Target `netstandard2.0` for generator project
- [ ] Use `PrivateAssets="all"` for Roslyn packages
- [ ] Keep predicates syntax-only (no semantic analysis)
- [ ] Handle namespaces in generated code
- [ ] Use unique filenames for generated files (.g.cs suffix)
- [ ] Escape strings properly in generated code
- [ ] Never throw unhandled exceptions in generators
- [ ] Use IIncrementalGenerator (not ISourceGenerator)
- [ ] Provide helpful diagnostic messages
- [ ] Test generator with large codebases

---

## 🐛 Debugging Checklist

**Generator not running?**
1. Check `OutputItemType="Analyzer"` in project reference
2. Verify generator targets `netstandard2.0`
3. Clean and rebuild: `dotnet clean && dotnet build`
4. Check generated files: `ls obj/Debug/net8.0/generated/`

**Build is slow?**
1. Use IIncrementalGenerator
2. Profile predicate performance
3. Avoid semantic analysis in predicates
4. Cache compilation symbols

**IDE freezing?**
1. Add try-catch to predicates
2. Avoid loops in generator
3. Test with large files

---

**Key Takeaway:** Most source generator issues come from incorrect project configuration or slow predicates. Follow the checklist above and you'll avoid 90% of problems!
