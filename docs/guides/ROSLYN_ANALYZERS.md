# Custom Roslyn Analyzers Guide

## Overview

Roslyn Analyzers enable custom compile-time code analysis and automatic code fixes. This guide covers creating custom analyzers for performance, design, and security.

## Table of Contents

- [What are Roslyn Analyzers?](#what-are-roslyn-analyzers)
- [Setting Up an Analyzer Project](#setting-up-an-analyzer-project)
- [Performance Analyzers](#performance-analyzers)
- [Design Analyzers](#design-analyzers)
- [Security Analyzers](#security-analyzers)
- [Code Fixes](#code-fixes)
- [Testing Analyzers](#testing-analyzers)

## What are Roslyn Analyzers?

Roslyn Analyzers provide:
- **Compile-time analysis** - Catch issues during build
- **IDE integration** - Real-time feedback in Visual Studio/Rider
- **Code fixes** - Automatic refactoring suggestions
- **Custom rules** - Project-specific guidelines enforcement

### Benefits
- ✅ **Early detection** - Find issues before runtime
- ✅ **Consistency** - Enforce coding standards
- ✅ **Education** - Guide developers to best practices
- ✅ **Automation** - Reduce manual code review effort

## Setting Up an Analyzer Project

### 1. Create Analyzer Project

```bash
# Use the analyzer template
dotnet new analyzer -n AdvancedConcepts.Analyzers

cd AdvancedConcepts.Analyzers
```

### 2. Project Structure

```
AdvancedConcepts.Analyzers/
├── AdvancedConcepts.Analyzers/
│   ├── Analyzers/
│   │   ├── PerformanceAnalyzers/
│   │   ├── DesignAnalyzers/
│   │   └── SecurityAnalyzers/
│   ├── CodeFixes/
│   └── Resources.resx
├── AdvancedConcepts.Analyzers.Test/
└── AdvancedConcepts.Analyzers.Package/
```

### 3. Configure Project File

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>netstandard2.0</TargetFramework>
    <IsPackable>true</IsPackable>
    <DevelopmentDependency>true</DevelopmentDependency>
    <IncludeBuildOutput>false</IncludeBuildOutput>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Microsoft.CodeAnalysis.CSharp.Workspaces" Version="4.5.0" PrivateAssets="all" />
    <PackageReference Include="Microsoft.CodeAnalysis.Analyzers" Version="3.3.4" PrivateAssets="all" />
  </ItemGroup>

  <ItemGroup>
    <None Include="$(OutputPath)\$(AssemblyName).dll" Pack="true" PackagePath="analyzers/dotnet/cs" Visible="false" />
  </ItemGroup>
</Project>
```

## Performance Analyzers

### 1. Allocation Detection Analyzer

Detects unnecessary allocations in hot paths.

```csharp
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;

namespace AdvancedConcepts.Analyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class AllocationAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "AC001";

        private static readonly LocalizableString Title = "Avoid allocation in hot path";
        private static readonly LocalizableString MessageFormat = "Consider using {0} to avoid allocation";
        private static readonly LocalizableString Description = "Allocations in hot paths can impact performance.";
        private const string Category = "Performance";

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(
            DiagnosticId,
            Title,
            MessageFormat,
            Category,
            DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            description: Description);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();

            context.RegisterSyntaxNodeAction(AnalyzeArraySlicing, SyntaxKind.ElementAccessExpression);
        }

        private static void AnalyzeArraySlicing(SyntaxNodeAnalysisContext context)
        {
            var elementAccess = (ElementAccessExpressionSyntax)context.Node;

            // Check if this is array slicing that could use Span<T>
            if (IsArraySlicingCandidate(elementAccess, context))
            {
                var diagnostic = Diagnostic.Create(
                    Rule,
                    elementAccess.GetLocation(),
                    "Span<T> or Memory<T>");

                context.ReportDiagnostic(diagnostic);
            }
        }

        private static bool IsArraySlicingCandidate(ElementAccessExpressionSyntax elementAccess, SyntaxNodeAnalysisContext context)
        {
            // Implementation: Detect patterns like array[start..end] that could use Span
            return false; // Simplified for example
        }
    }
}
```

### 2. LINQ Performance Analyzer

Detects inefficient LINQ usage.

```csharp
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class LinqPerformanceAnalyzer : DiagnosticAnalyzer
{
    public const string DiagnosticId = "AC002";

    private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(
        DiagnosticId,
        "Inefficient LINQ usage",
        "Consider using {0} instead of {1} for better performance",
        "Performance",
        DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);

        context.RegisterSyntaxNodeAction(AnalyzeLinqUsage, SyntaxKind.InvocationExpression);
    }

    private static void AnalyzeLinqUsage(SyntaxNodeAnalysisContext context)
    {
        var invocation = (InvocationExpressionSyntax)context.Node;

        // Example: Detect Count() > 0 instead of Any()
        if (IsCountComparisonWithZero(invocation))
        {
            var diagnostic = Diagnostic.Create(
                Rule,
                invocation.GetLocation(),
                "Any()",
                "Count() > 0");

            context.ReportDiagnostic(diagnostic);
        }

        // Example: Detect Where().First() instead of First(predicate)
        if (IsWhereFirstPattern(invocation, context))
        {
            var diagnostic = Diagnostic.Create(
                Rule,
                invocation.GetLocation(),
                "First(predicate)",
                "Where().First()");

            context.ReportDiagnostic(diagnostic);
        }
    }

    private static bool IsCountComparisonWithZero(InvocationExpressionSyntax invocation)
    {
        // Check if parent is BinaryExpression with Count() > 0 pattern
        if (invocation.Parent is BinaryExpressionSyntax binaryExpression)
        {
            if (invocation.Expression is MemberAccessExpressionSyntax memberAccess &&
                memberAccess.Name.Identifier.Text == "Count")
            {
                return binaryExpression.Right is LiteralExpressionSyntax literal &&
                       literal.Token.ValueText == "0";
            }
        }
        return false;
    }

    private static bool IsWhereFirstPattern(InvocationExpressionSyntax invocation, SyntaxNodeAnalysisContext context)
    {
        // Detect Where().First() pattern
        if (invocation.Expression is MemberAccessExpressionSyntax memberAccess &&
            memberAccess.Name.Identifier.Text == "First")
        {
            if (memberAccess.Expression is InvocationExpressionSyntax previousInvocation &&
                previousInvocation.Expression is MemberAccessExpressionSyntax previousMember &&
                previousMember.Name.Identifier.Text == "Where")
            {
                return true;
            }
        }
        return false;
    }
}
```

### 3. Async/Await Pattern Analyzer

Detects async anti-patterns.

```csharp
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class AsyncAwaitAnalyzer : DiagnosticAnalyzer
{
    public const string DiagnosticId = "AC003";

    private static readonly DiagnosticDescriptor AsyncVoidRule = new DiagnosticDescriptor(
        DiagnosticId,
        "Avoid async void methods",
        "Method '{0}' should return Task instead of void",
        "Performance",
        DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    private static readonly DiagnosticDescriptor ConfigureAwaitRule = new DiagnosticDescriptor(
        "AC004",
        "Use ConfigureAwait(false)",
        "Consider using ConfigureAwait(false) in library code",
        "Performance",
        DiagnosticSeverity.Info,
        isEnabledByDefault: true);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
        ImmutableArray.Create(AsyncVoidRule, ConfigureAwaitRule);

    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);

        context.RegisterSyntaxNodeAction(AnalyzeMethod, SyntaxKind.MethodDeclaration);
        context.RegisterSyntaxNodeAction(AnalyzeAwait, SyntaxKind.AwaitExpression);
    }

    private static void AnalyzeMethod(SyntaxNodeAnalysisContext context)
    {
        var methodDeclaration = (MethodDeclarationSyntax)context.Node;

        // Detect async void (except event handlers)
        if (methodDeclaration.Modifiers.Any(SyntaxKind.AsyncKeyword) &&
            methodDeclaration.ReturnType is PredefinedTypeSyntax predefined &&
            predefined.Keyword.IsKind(SyntaxKind.VoidKeyword) &&
            !IsEventHandler(methodDeclaration, context))
        {
            var diagnostic = Diagnostic.Create(
                AsyncVoidRule,
                methodDeclaration.Identifier.GetLocation(),
                methodDeclaration.Identifier.Text);

            context.ReportDiagnostic(diagnostic);
        }
    }

    private static void AnalyzeAwait(SyntaxNodeAnalysisContext context)
    {
        var awaitExpression = (AwaitExpressionSyntax)context.Node;

        // Check if ConfigureAwait is used
        if (awaitExpression.Expression is InvocationExpressionSyntax invocation)
        {
            if (invocation.Expression is MemberAccessExpressionSyntax memberAccess &&
                memberAccess.Name.Identifier.Text != "ConfigureAwait")
            {
                // Suggest ConfigureAwait(false) for library code
                if (IsLibraryCode(context))
                {
                    var diagnostic = Diagnostic.Create(
                        ConfigureAwaitRule,
                        awaitExpression.GetLocation());

                    context.ReportDiagnostic(diagnostic);
                }
            }
        }
    }

    private static bool IsEventHandler(MethodDeclarationSyntax method, SyntaxNodeAnalysisContext context)
    {
        // Check if method is an event handler (has EventHandler signature)
        return false; // Simplified
    }

    private static bool IsLibraryCode(SyntaxNodeAnalysisContext context)
    {
        // Check if this is library code (not application code)
        return !context.Options.AnalyzerConfigOptionsProvider
            .GetOptions(context.Node.SyntaxTree)
            .TryGetValue("is_library_code", out _);
    }
}
```

## Design Analyzers

### 1. SOLID Violations Analyzer

```csharp
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class SolidViolationAnalyzer : DiagnosticAnalyzer
{
    public const string SrpViolationId = "AC010";
    public const string DipViolationId = "AC011";

    private static readonly DiagnosticDescriptor SrpRule = new DiagnosticDescriptor(
        SrpViolationId,
        "Possible Single Responsibility Principle violation",
        "Class '{0}' may have too many responsibilities",
        "Design",
        DiagnosticSeverity.Info,
        isEnabledByDefault: true);

    private static readonly DiagnosticDescriptor DipRule = new DiagnosticDescriptor(
        DipViolationId,
        "Dependency Inversion Principle violation",
        "Class '{0}' depends on concrete type '{1}' instead of abstraction",
        "Design",
        DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
        ImmutableArray.Create(SrpRule, DipRule);

    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);

        context.RegisterSymbolAction(AnalyzeClass, SymbolKind.NamedType);
    }

    private static void AnalyzeClass(SymbolAnalysisContext context)
    {
        var namedType = (INamedTypeSymbol)context.Symbol;

        if (namedType.TypeKind != TypeKind.Class)
            return;

        // Check SRP: Too many public methods might indicate multiple responsibilities
        var publicMethods = namedType.GetMembers()
            .OfType<IMethodSymbol>()
            .Where(m => m.DeclaredAccessibility == Accessibility.Public &&
                       !m.IsImplicitlyDeclared)
            .Count();

        if (publicMethods > 10) // Configurable threshold
        {
            var diagnostic = Diagnostic.Create(
                SrpRule,
                namedType.Locations[0],
                namedType.Name);

            context.ReportDiagnostic(diagnostic);
        }

        // Check DIP: Dependencies on concrete types
        CheckDependencyInversion(namedType, context);
    }

    private static void CheckDependencyInversion(INamedTypeSymbol namedType, SymbolAnalysisContext context)
    {
        // Check constructor parameters for concrete type dependencies
        foreach (var constructor in namedType.Constructors)
        {
            foreach (var parameter in constructor.Parameters)
            {
                if (parameter.Type is INamedTypeSymbol parameterType &&
                    parameterType.TypeKind == TypeKind.Class &&
                    !parameterType.IsAbstract &&
                    !IsFrameworkType(parameterType))
                {
                    var diagnostic = Diagnostic.Create(
                        DipRule,
                        parameter.Locations[0],
                        namedType.Name,
                        parameterType.Name);

                    context.ReportDiagnostic(diagnostic);
                }
            }
        }
    }

    private static bool IsFrameworkType(INamedTypeSymbol type)
    {
        // Check if type is from framework (System.*, Microsoft.*)
        return type.ContainingNamespace.ToString().StartsWith("System") ||
               type.ContainingNamespace.ToString().StartsWith("Microsoft");
    }
}
```

## Security Analyzers

### SQL Injection Detection

```csharp
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class SqlInjectionAnalyzer : DiagnosticAnalyzer
{
    public const string DiagnosticId = "AC020";

    private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(
        DiagnosticId,
        "Possible SQL injection vulnerability",
        "SQL query uses string concatenation with user input. Use parameterized queries instead.",
        "Security",
        DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);

        context.RegisterSyntaxNodeAction(AnalyzeStringConcatenation,
            SyntaxKind.AddExpression,
            SyntaxKind.InterpolatedStringExpression);
    }

    private static void AnalyzeStringConcatenation(SyntaxNodeAnalysisContext context)
    {
        var expression = context.Node;

        // Check if this is used in SQL context
        if (IsUsedInSqlContext(expression, context))
        {
            var diagnostic = Diagnostic.Create(Rule, expression.GetLocation());
            context.ReportDiagnostic(diagnostic);
        }
    }

    private static bool IsUsedInSqlContext(SyntaxNode node, SyntaxNodeAnalysisContext context)
    {
        // Check if parent invocation is ExecuteQuery, ExecuteNonQuery, etc.
        var parent = node.Parent;
        while (parent != null)
        {
            if (parent is InvocationExpressionSyntax invocation &&
                invocation.Expression is MemberAccessExpressionSyntax memberAccess)
            {
                var methodName = memberAccess.Name.Identifier.Text;
                if (methodName.Contains("Execute") || methodName.Contains("Query"))
                {
                    return true;
                }
            }
            parent = parent.Parent;
        }
        return false;
    }
}
```

## Code Fixes

### Example: ConfigureAwait Code Fix

```csharp
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(ConfigureAwaitCodeFixProvider)), Shared]
public class ConfigureAwaitCodeFixProvider : CodeFixProvider
{
    public sealed override ImmutableArray<string> FixableDiagnosticIds =>
        ImmutableArray.Create(AsyncAwaitAnalyzer.ConfigureAwaitDiagnosticId);

    public sealed override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

    public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

        var diagnostic = context.Diagnostics.First();
        var diagnosticSpan = diagnostic.Location.SourceSpan;

        var awaitExpression = root.FindToken(diagnosticSpan.Start)
            .Parent.AncestorsAndSelf()
            .OfType<AwaitExpressionSyntax>()
            .First();

        context.RegisterCodeFix(
            CodeAction.Create(
                title: "Add ConfigureAwait(false)",
                createChangedDocument: c => AddConfigureAwaitAsync(context.Document, awaitExpression, c),
                equivalenceKey: "AddConfigureAwait"),
            diagnostic);
    }

    private async Task<Document> AddConfigureAwaitAsync(
        Document document,
        AwaitExpressionSyntax awaitExpression,
        CancellationToken cancellationToken)
    {
        var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

        // Create ConfigureAwait(false) invocation
        var configureAwait = SyntaxFactory.InvocationExpression(
            SyntaxFactory.MemberAccessExpression(
                SyntaxKind.SimpleMemberAccessExpression,
                awaitExpression.Expression,
                SyntaxFactory.IdentifierName("ConfigureAwait")),
            SyntaxFactory.ArgumentList(
                SyntaxFactory.SingletonSeparatedList(
                    SyntaxFactory.Argument(
                        SyntaxFactory.LiteralExpression(
                            SyntaxKind.FalseLiteralExpression)))));

        var newAwaitExpression = awaitExpression.WithExpression(configureAwait);
        var newRoot = root.ReplaceNode(awaitExpression, newAwaitExpression);

        return document.WithSyntaxRoot(newRoot);
    }
}
```

## Testing Analyzers

### Unit Test Example

```csharp
using Microsoft.CodeAnalysis.Testing;
using Xunit;

public class AllocationAnalyzerTests
{
    [Fact]
    public async Task ArraySlicing_ReportsWarning()
    {
        var code = @"
class Program
{
    void Method()
    {
        var array = new int[100];
        var slice = [|array[10..20]|]; // Should suggest Span<T>
    }
}";

        var expected = VerifyCS.Diagnostic(AllocationAnalyzer.DiagnosticId)
            .WithLocation(0)
            .WithArguments("Span<T> or Memory<T>");

        await VerifyCS.VerifyAnalyzerAsync(code, expected);
    }
}
```

## Best Practices

### 1. Performance
- ✅ Use concurrent execution
- ✅ Avoid allocations in analyzers
- ✅ Cache expensive computations
- ✅ Use symbol analysis over syntax when possible

### 2. User Experience
- ✅ Provide clear diagnostic messages
- ✅ Include helpful documentation
- ✅ Offer code fixes when possible
- ✅ Use appropriate severity levels

### 3. Configuration
- ✅ Support .editorconfig configuration
- ✅ Allow disabling rules
- ✅ Provide severity customization
- ✅ Document all options

## Resources

- [Roslyn Analyzers Documentation](https://learn.microsoft.com/en-us/dotnet/csharp/roslyn-sdk/)
- [Analyzer API Reference](https://learn.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.diagnostics)
- [Code Fix Provider Guide](https://github.com/dotnet/roslyn/blob/main/docs/wiki/How-To-Write-a-C%23-Analyzer-and-Code-Fix.md)

---

**Last Updated:** 2025-11-30
**Version:** 1.0
