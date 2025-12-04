# Source Generators Guide

## Overview

Source Generators are a powerful C# compiler feature that allows you to generate code at compile-time. This guide covers creating custom source generators for common patterns.

## Table of Contents

- [What are Source Generators?](#what-are-source-generators)
- [Setting Up a Source Generator Project](#setting-up-a-source-generator-project)
- [Common Use Cases](#common-use-cases)
- [Example: AutoMapper Generator](#example-automapper-generator)
- [Example: Logger Generator](#example-logger-generator)
- [Best Practices](#best-practices)
- [Testing Source Generators](#testing-source-generators)

## What are Source Generators?

Source generators run during compilation and can:
- Generate additional source files
- Analyze code and emit diagnostics
- Reduce runtime reflection
- Improve startup performance
- Enable compile-time validation

### Benefits
- ✅ **Zero runtime overhead** - Code generated at compile-time
- ✅ **Type safety** - Errors caught during compilation
- ✅ **Better IDE support** - IntelliSense for generated code
- ✅ **Performance** - No reflection needed at runtime

## Setting Up a Source Generator Project

### 1. Create Source Generator Project

```bash
# Create a .NET Standard 2.0 library
dotnet new classlib -n AdvancedConcepts.SourceGenerators -f netstandard2.0

# Add required packages
cd AdvancedConcepts.SourceGenerators
dotnet add package Microsoft.CodeAnalysis.CSharp --version 4.5.0
dotnet add package Microsoft.CodeAnalysis.Analyzers --version 3.3.4
```

### 2. Configure Project File

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>netstandard2.0</TargetFramework>
    <LangVersion>latest</LangVersion>
    <IsRoslynComponent>true</IsRoslynComponent>
    <EnforceExtendedAnalyzerRules>true</EnforceExtendedAnalyzerRules>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Microsoft.CodeAnalysis.CSharp" Version="4.5.0" PrivateAssets="all" />
    <PackageReference Include="Microsoft.CodeAnalysis.Analyzers" Version="3.3.4" PrivateAssets="all" />
  </ItemGroup>
</Project>
```

### 3. Reference in Consumer Project

```xml
<ItemGroup>
  <ProjectReference Include="..\AdvancedConcepts.SourceGenerators\AdvancedConcepts.SourceGenerators.csproj"
                    OutputItemType="Analyzer"
                    ReferenceOutputAssembly="false" />
</ItemGroup>
```

## Common Use Cases

### 1. DTO Mapping (AutoMapper-like)

Automatically generate mapping code between DTOs:

```csharp
[AutoMap(typeof(UserDto))]
public partial class User
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
}

// Generated code:
public partial class User
{
    public static User FromDto(UserDto dto)
    {
        return new User
        {
            Id = dto.Id,
            Name = dto.Name,
            Email = dto.Email
        };
    }

    public UserDto ToDto()
    {
        return new UserDto
        {
            Id = this.Id,
            Name = this.Name,
            Email = this.Email
        };
    }
}
```

### 2. Compile-Time Logging

Generate optimized logging code:

```csharp
public partial class UserService
{
    [LoggerMessage(EventId = 1, Level = LogLevel.Information, Message = "User {UserId} logged in")]
    public static partial void LogUserLogin(ILogger logger, int userId);
}

// Generated code includes optimized logging without string interpolation overhead
```

### 3. Builder Pattern Generation

Automatically create fluent builder APIs:

```csharp
[GenerateBuilder]
public partial class Product
{
    public string Name { get; set; }
    public decimal Price { get; set; }
    public string Category { get; set; }
}

// Usage:
var product = new ProductBuilder()
    .WithName("Laptop")
    .WithPrice(999.99m)
    .WithCategory("Electronics")
    .Build();
```

### 4. Validation Generation

Generate validation code from attributes:

```csharp
public class User
{
    [Required]
    [StringLength(100)]
    public string Name { get; set; }

    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Range(18, 120)]
    public int Age { get; set; }
}

// Generated validator class
```

### 5. Serialization Optimization

Generate optimized JSON serialization code:

```csharp
[JsonSerializable(typeof(User))]
public partial class UserContext : JsonSerializerContext
{
    // Generated code for optimized serialization
}
```

## Example: AutoMapper Generator

### Attribute Definition

```csharp
namespace AdvancedConcepts.SourceGenerators
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class AutoMapAttribute : Attribute
    {
        public Type TargetType { get; }

        public AutoMapAttribute(Type targetType)
        {
            TargetType = targetType;
        }
    }
}
```

### Source Generator Implementation

```csharp
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AdvancedConcepts.SourceGenerators
{
    [Generator]
    public class AutoMapGenerator : ISourceGenerator
    {
        public void Initialize(GeneratorInitializationContext context)
        {
            // Register a syntax receiver to find candidates
            context.RegisterForSyntaxNotifications(() => new AutoMapSyntaxReceiver());
        }

        public void Execute(GeneratorExecutionContext context)
        {
            if (context.SyntaxReceiver is not AutoMapSyntaxReceiver receiver)
                return;

            foreach (var classDeclaration in receiver.CandidateClasses)
            {
                var model = context.Compilation.GetSemanticModel(classDeclaration.SyntaxTree);
                var classSymbol = model.GetDeclaredSymbol(classDeclaration);

                if (classSymbol == null)
                    continue;

                var attributes = classSymbol.GetAttributes()
                    .Where(a => a.AttributeClass?.Name == "AutoMapAttribute");

                foreach (var attribute in attributes)
                {
                    if (attribute.ConstructorArguments.Length > 0)
                    {
                        var targetType = attribute.ConstructorArguments[0].Value as INamedTypeSymbol;
                        if (targetType != null)
                        {
                            var source = GenerateMappingCode(classSymbol, targetType);
                            context.AddSource($"{classSymbol.Name}_AutoMap.g.cs", SourceText.From(source, Encoding.UTF8));
                        }
                    }
                }
            }
        }

        private string GenerateMappingCode(INamedTypeSymbol sourceType, INamedTypeSymbol targetType)
        {
            var sourceProperties = sourceType.GetMembers()
                .OfType<IPropertySymbol>()
                .Where(p => p.DeclaredAccessibility == Accessibility.Public);

            var targetProperties = targetType.GetMembers()
                .OfType<IPropertySymbol>()
                .Where(p => p.DeclaredAccessibility == Accessibility.Public)
                .ToDictionary(p => p.Name);

            var sb = new StringBuilder();
            sb.AppendLine("// <auto-generated />");
            sb.AppendLine($"namespace {sourceType.ContainingNamespace}");
            sb.AppendLine("{");
            sb.AppendLine($"    public partial class {sourceType.Name}");
            sb.AppendLine("    {");

            // Generate FromDto method
            sb.AppendLine($"        public static {sourceType.Name} From{targetType.Name}({targetType} dto)");
            sb.AppendLine("        {");
            sb.AppendLine($"            return new {sourceType.Name}");
            sb.AppendLine("            {");

            foreach (var prop in sourceProperties)
            {
                if (targetProperties.TryGetValue(prop.Name, out var targetProp))
                {
                    sb.AppendLine($"                {prop.Name} = dto.{targetProp.Name},");
                }
            }

            sb.AppendLine("            };");
            sb.AppendLine("        }");

            // Generate ToDto method
            sb.AppendLine();
            sb.AppendLine($"        public {targetType} To{targetType.Name}()");
            sb.AppendLine("        {");
            sb.AppendLine($"            return new {targetType}");
            sb.AppendLine("            {");

            foreach (var prop in sourceProperties)
            {
                if (targetProperties.TryGetValue(prop.Name, out var targetProp))
                {
                    sb.AppendLine($"                {targetProp.Name} = this.{prop.Name},");
                }
            }

            sb.AppendLine("            };");
            sb.AppendLine("        }");

            sb.AppendLine("    }");
            sb.AppendLine("}");

            return sb.ToString();
        }
    }

    class AutoMapSyntaxReceiver : ISyntaxReceiver
    {
        public List<ClassDeclarationSyntax> CandidateClasses { get; } = new List<ClassDeclarationSyntax>();

        public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
        {
            if (syntaxNode is ClassDeclarationSyntax classDeclaration &&
                classDeclaration.AttributeLists.Count > 0)
            {
                CandidateClasses.Add(classDeclaration);
            }
        }
    }
}
```

## Example: Logger Generator

### Implementation

```csharp
[Generator]
public class LoggerMessageGenerator : ISourceGenerator
{
    public void Initialize(GeneratorInitializationContext context)
    {
        // Register syntax receiver
    }

    public void Execute(GeneratorExecutionContext context)
    {
        // Find methods with [LoggerMessage] attribute
        // Generate optimized logging code
        // Avoid string interpolation overhead
        // Use LoggerMessage.Define for performance
    }
}
```

### Generated Code Example

```csharp
// Input:
[LoggerMessage(EventId = 1, Level = LogLevel.Information, Message = "Processing order {OrderId} for user {UserId}")]
public static partial void LogOrderProcessing(ILogger logger, int orderId, int userId);

// Generated:
private static readonly Action<ILogger, int, int, Exception?> _logOrderProcessing =
    LoggerMessage.Define<int, int>(
        LogLevel.Information,
        new EventId(1),
        "Processing order {OrderId} for user {UserId}");

public static partial void LogOrderProcessing(ILogger logger, int orderId, int userId)
{
    _logOrderProcessing(logger, orderId, userId, null);
}
```

## Best Practices

### 1. Performance
- ✅ Use `StringBuilder` for code generation
- ✅ Cache compilation objects
- ✅ Minimize allocations
- ✅ Use incremental generators when possible

### 2. Diagnostics
- ✅ Report clear error messages
- ✅ Include source location information
- ✅ Provide fix suggestions
- ✅ Use diagnostic severity appropriately

### 3. Generated Code Quality
- ✅ Add `<auto-generated>` header
- ✅ Use `#nullable` directives appropriately
- ✅ Follow coding conventions
- ✅ Add XML documentation comments

### 4. Debugging
- ✅ Use `Debugger.Launch()` for debugging
- ✅ Write generated code to disk during development
- ✅ Add comprehensive unit tests
- ✅ Test with various input scenarios

## Testing Source Generators

### Unit Test Example

```csharp
[Fact]
public void AutoMapGenerator_GeneratesMappingMethods()
{
    // Arrange
    var source = @"
        using AdvancedConcepts.SourceGenerators;

        [AutoMap(typeof(UserDto))]
        public partial class User
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }

        public class UserDto
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }
    ";

    // Act
    var (diagnostics, output) = TestHelper.GetGeneratedOutput<AutoMapGenerator>(source);

    // Assert
    Assert.Empty(diagnostics);
    Assert.Contains("public static User FromUserDto(UserDto dto)", output);
    Assert.Contains("public UserDto ToUserDto()", output);
}
```

## Advanced Topics

### Incremental Generators (Recommended)

```csharp
[Generator]
public class IncrementalAutoMapGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // More efficient than ISourceGenerator
        var classDeclarations = context.SyntaxProvider
            .CreateSyntaxProvider(
                predicate: static (s, _) => IsSyntaxTargetForGeneration(s),
                transform: static (ctx, _) => GetSemanticTargetForGeneration(ctx))
            .Where(static m => m is not null);

        context.RegisterSourceOutput(classDeclarations, static (spc, source) => Execute(source, spc));
    }

    private static bool IsSyntaxTargetForGeneration(SyntaxNode node)
    {
        return node is ClassDeclarationSyntax { AttributeLists.Count: > 0 };
    }

    private static ClassDeclarationSyntax? GetSemanticTargetForGeneration(GeneratorSyntaxContext context)
    {
        var classDeclaration = (ClassDeclarationSyntax)context.Node;

        foreach (var attributeList in classDeclaration.AttributeLists)
        {
            foreach (var attribute in attributeList.Attributes)
            {
                if (context.SemanticModel.GetSymbolInfo(attribute).Symbol is IMethodSymbol attributeSymbol)
                {
                    var attributeContainingType = attributeSymbol.ContainingType;
                    var fullName = attributeContainingType.ToDisplayString();

                    if (fullName == "AdvancedConcepts.SourceGenerators.AutoMapAttribute")
                    {
                        return classDeclaration;
                    }
                }
            }
        }

        return null;
    }

    private static void Execute(ClassDeclarationSyntax classDeclaration, SourceProductionContext context)
    {
        // Generate code
    }
}
```

## Resources

- [Source Generators Cookbook](https://github.com/dotnet/roslyn/blob/main/docs/features/source-generators.cookbook.md)
- [Source Generators Design](https://github.com/dotnet/roslyn/blob/main/docs/features/source-generators.md)
- [Incremental Generators](https://github.com/dotnet/roslyn/blob/main/docs/features/incremental-generators.md)

---

**Last Updated:** 2025-11-30
**Version:** 1.0
