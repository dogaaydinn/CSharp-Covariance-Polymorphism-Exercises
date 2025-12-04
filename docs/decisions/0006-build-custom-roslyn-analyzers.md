# 6. Build Custom Roslyn Analyzers

**Status:** ✅ Accepted

**Date:** 2024-12-01

**Deciders:** Architecture Team, Code Quality Team

**Technical Story:** Implementation in `src/AdvancedConcepts.Analyzers`

---

## Context and Problem Statement

Code quality and consistency are critical in enterprise applications. We need to enforce:
- Architecture rules (e.g., Domain layer can't reference Infrastructure)
- Naming conventions (e.g., interfaces must start with 'I')
- Performance best practices (e.g., avoid async void)
- Security patterns (e.g., validate user input)
- Custom business rules

**Traditional approach problems:**
- Manual code reviews miss issues
- StyleCop/SonarQube don't understand our domain
- Rules are documented but not enforced
- Issues discovered late (in PR or production)

**We need a solution that:**
- Enforces rules at compile-time
- Provides immediate feedback in IDE
- Integrates with existing build pipeline
- Can be customized for our specific needs

---

## Decision Drivers

* **Shift-Left** - Catch issues during development, not in PR
* **Consistency** - Enforce team standards automatically
* **Custom Rules** - Apply domain-specific constraints
* **Developer Experience** - Instant feedback with code fixes
* **CI/CD Integration** - Fail builds on violations

---

## Considered Options

* **Option 1** - Custom Roslyn Analyzers
* **Option 2** - StyleCop + SonarQube (off-the-shelf analyzers)
* **Option 3** - Manual code reviews only
* **Option 4** - Git pre-commit hooks with regex

---

## Decision Outcome

**Chosen option:** "Custom Roslyn Analyzers", because they provide compile-time enforcement of our specific architectural and business rules with instant IDE feedback and automated code fixes.

### Positive Consequences

* **Instant Feedback** - Red squiggles in IDE immediately
* **Automated Fixes** - Code fix providers can auto-correct violations
* **Compile-Time** - Violations cause build failures
* **Custom Rules** - Enforce our specific architecture
* **Educational** - Helps team learn best practices
* **IDE Integration** - Works in VS, VS Code, Rider

### Negative Consequences

* **Complex Implementation** - Roslyn API has steep learning curve
* **Maintenance** - Analyzers need updates for new rules
* **False Positives** - May need suppressions for edge cases
* **Build Time** - Adds ~1-2 seconds to compilation

---

## Pros and Cons of the Options

### Custom Roslyn Analyzers (Chosen)

**What Are Roslyn Analyzers?**

Roslyn analyzers are compiler plugins that analyze code during compilation and report diagnostics (warnings/errors) with optional automatic code fixes.

**Architecture:**
```
Your Code (*.cs)
    ↓
Roslyn Compiler
    ↓
Syntax Analysis → Semantic Analysis
    ↓
Custom Analyzers (inspect syntax trees)
    ↓
Diagnostics (errors/warnings)
    ↓
Code Fix Providers (optional automated fixes)
    ↓
IDE Display (red/green squiggles)
```

**Pros:**
* **Real-time feedback** - See violations while typing
* **Compile-time enforcement** - Can't build with errors
* **Code fix providers** - Auto-fix violations with Ctrl+.
* **Works everywhere** - VS, VS Code, Rider, dotnet build
* **Team-specific rules** - Enforce your architecture
* **Educational** - Explains why violation is bad

**Cons:**
* **Complex to write** - Roslyn API is difficult
* **Testing required** - Need unit tests for analyzers
* **Performance** - Slow analyzers impact IDE responsiveness
* **Versioning** - Roslyn APIs can change

**Example 1: Architecture Rule Analyzer**

```csharp
// Rule: Domain layer must not reference Infrastructure layer
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class DomainMustNotReferenceInfrastructureAnalyzer : DiagnosticAnalyzer
{
    public const string DiagnosticId = "ARCH001";

    private static readonly DiagnosticDescriptor Rule = new(
        id: DiagnosticId,
        title: "Domain layer must not reference Infrastructure",
        messageFormat: "Type '{0}' from Domain layer references '{1}' from Infrastructure layer. This violates Clean Architecture principles.",
        category: "Architecture",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "Domain layer should have no dependencies. It defines interfaces that Infrastructure implements.");

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();

        // Analyze every time a type is referenced
        context.RegisterSymbolAction(AnalyzeSymbol, SymbolKind.NamedType);
    }

    private void AnalyzeSymbol(SymbolAnalysisContext context)
    {
        var namedType = (INamedTypeSymbol)context.Symbol;

        // Check if this type is in Domain namespace
        if (!namedType.ContainingNamespace.ToString().Contains(".Domain"))
            return;

        // Check all referenced types
        foreach (var referencedType in GetReferencedTypes(namedType))
        {
            // Check if referenced type is in Infrastructure
            if (referencedType.ContainingNamespace.ToString().Contains(".Infrastructure"))
            {
                var diagnostic = Diagnostic.Create(
                    Rule,
                    namedType.Locations[0],
                    namedType.Name,
                    referencedType.Name);

                context.ReportDiagnostic(diagnostic);
            }
        }
    }
}

// Usage - This code will NOT compile:
namespace MyApp.Domain.Entities
{
    using MyApp.Infrastructure.Data; // ❌ ARCH001 Error!

    public class Product
    {
        public ApplicationDbContext Context { get; set; } // ❌ Domain can't reference Infrastructure!
    }
}

// IDE shows:
// Error ARCH001: Type 'Product' from Domain layer references 'ApplicationDbContext' from Infrastructure layer.
//                This violates Clean Architecture principles.
```

**Example 2: Performance Analyzer with Code Fix**

```csharp
// Rule: Avoid String Concatenation in Loops
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class AvoidStringConcatInLoopAnalyzer : DiagnosticAnalyzer
{
    public const string DiagnosticId = "PERF001";

    private static readonly DiagnosticDescriptor Rule = new(
        id: DiagnosticId,
        title: "Avoid string concatenation in loops",
        messageFormat: "String concatenation in loop detected. Use StringBuilder for better performance.",
        category: "Performance",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: "String is immutable. Each concatenation creates a new string object. Use StringBuilder instead.");

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();

        context.RegisterSyntaxNodeAction(AnalyzeNode, SyntaxKind.AddAssignmentExpression);
    }

    private void AnalyzeNode(SyntaxNodeAnalysisContext context)
    {
        var assignment = (AssignmentExpressionSyntax)context.Node;

        // Check if left side is string type
        var typeInfo = context.SemanticModel.GetTypeInfo(assignment.Left);
        if (typeInfo.Type?.SpecialType != SpecialType.System_String)
            return;

        // Check if we're inside a loop
        if (!IsInsideLoop(assignment))
            return;

        var diagnostic = Diagnostic.Create(Rule, assignment.GetLocation());
        context.ReportDiagnostic(diagnostic);
    }

    private bool IsInsideLoop(SyntaxNode node)
    {
        return node.Ancestors().Any(n =>
            n is ForStatementSyntax ||
            n is ForEachStatementSyntax ||
            n is WhileStatementSyntax ||
            n is DoStatementSyntax);
    }
}

// Code Fix Provider (automated fix)
[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(StringConcatCodeFixProvider))]
public class StringConcatCodeFixProvider : CodeFixProvider
{
    public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create("PERF001");

    public override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken);
        var diagnostic = context.Diagnostics.First();
        var diagnosticSpan = diagnostic.Location.SourceSpan;
        var node = root.FindToken(diagnosticSpan.Start).Parent;

        // Register code fix
        context.RegisterCodeFix(
            CodeAction.Create(
                title: "Convert to StringBuilder",
                createChangedDocument: c => ConvertToStringBuilder(context.Document, node, c),
                equivalenceKey: "ConvertToStringBuilder"),
            diagnostic);
    }

    private async Task<Document> ConvertToStringBuilder(Document document, SyntaxNode node, CancellationToken ct)
    {
        // Implementation that rewrites code to use StringBuilder
        // ...
    }
}

// Bad Code (triggers analyzer):
string result = "";
for (int i = 0; i < 1000; i++)
{
    result += i.ToString(); // ⚠️ PERF001 Warning with green squiggle
}

// Press Ctrl+. (Quick Fix) → "Convert to StringBuilder"
// Analyzer automatically fixes to:
var result = new StringBuilder();
for (int i = 0; i < 1000; i++)
{
    result.Append(i.ToString()); // ✅ Fixed!
}
```

**Example 3: Security Analyzer**

```csharp
// Rule: User Input Must Be Validated
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class ValidateUserInputAnalyzer : DiagnosticAnalyzer
{
    public const string DiagnosticId = "SEC001";

    private static readonly DiagnosticDescriptor Rule = new(
        id: DiagnosticId,
        title: "User input must be validated",
        messageFormat: "Parameter '{0}' receives user input but is not validated. Apply [Required] or [StringLength] attributes.",
        category: "Security",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: "All user input should be validated to prevent injection attacks.");

    public override void Initialize(AnalysisContext context)
    {
        context.RegisterSymbolAction(AnalyzeMethod, SymbolKind.Method);
    }

    private void AnalyzeMethod(SymbolAnalysisContext context)
    {
        var method = (IMethodSymbol)context.Symbol;

        // Check if method is HTTP POST/PUT endpoint
        if (!HasHttpAttribute(method, "HttpPost", "HttpPut"))
            return;

        foreach (var parameter in method.Parameters)
        {
            // Check if parameter has validation attributes
            if (!HasValidationAttribute(parameter))
            {
                var diagnostic = Diagnostic.Create(
                    Rule,
                    parameter.Locations[0],
                    parameter.Name);

                context.ReportDiagnostic(diagnostic);
            }
        }
    }
}

// Bad Code:
[HttpPost]
public IActionResult Create(string name, decimal price) // ⚠️ SEC001: Parameters not validated
{
    // Potential SQL injection, XSS, etc.
    return Ok();
}

// Good Code:
[HttpPost]
public IActionResult Create(
    [Required, StringLength(100)] string name,  // ✅ Validated
    [Range(0.01, 1000000)] decimal price)       // ✅ Validated
{
    return Ok();
}
```

### StyleCop + SonarQube (Off-the-Shelf)

**Pros:**
* Ready to use out-of-the-box
* Hundreds of rules included
* Industry-standard
* No development required

**Cons:**
* **Generic rules** - Can't enforce our architecture
* **Limited customization** - Can't add custom rules
* **Configuration overhead** - Many irrelevant rules
* **False positives** - Rules not tailored to our code

**Why Insufficient Alone:**
StyleCop enforces formatting and general best practices, but can't enforce **domain-specific rules** like "Domain can't reference Infrastructure" or "All API endpoints must return Result<T>". We use StyleCop **plus** custom analyzers.

### Manual Code Reviews Only

**Pros:**
* Human judgment
* Can catch complex issues
* No tooling required

**Cons:**
* **Inconsistent** - Reviewers miss issues
* **Slow feedback** - Only during PR review
* **Doesn't scale** - Reviewers become bottleneck
* **No automated fixes** - Manual corrections required

**Why Rejected:**
Manual reviews are essential but **insufficient alone**. Analyzers catch 80% of issues automatically, freeing reviewers to focus on complex logic and design.

### Git Pre-Commit Hooks with Regex

**Pros:**
* Simple to implement
* Fast execution
* No compilation required

**Cons:**
* **Regex is fragile** - Can't understand code semantics
* **False positives** - Regex matches comments, strings
* **No IDE integration** - Only runs on commit
* **No code fixes** - Can't auto-correct
* **Can be bypassed** - git commit --no-verify

**Example (fragile):**
```bash
# Pre-commit hook
if grep -r "catch (Exception)" src/; then
    echo "Error: Don't catch generic Exception!"
    exit 1
fi

# ❌ False positive: Matches comments
// Don't use: catch (Exception) ex

# ❌ False positive: Matches strings
var errorMessage = "catch (Exception) is bad";
```

**Why Rejected:**
Regex can't understand code structure. Roslyn analyzers understand syntax trees and can differentiate between code, comments, and strings.

---

## Real-World Analyzer Examples

### Example 4: Enforce Result<T> Pattern

```csharp
// Rule: API endpoints must return Result<T>, not bare types
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class ApiMustReturnResultAnalyzer : DiagnosticAnalyzer
{
    // Implementation checks that all [HttpGet/Post/Put/Delete] methods
    // return ActionResult<Result<T>>, not ActionResult<T>

    // Bad:
    [HttpGet]
    public ActionResult<Product> Get(int id) // ⚠️ Must return Result<Product>
    {
        return product; // What if not found? Throws exception or returns null?
    }

    // Good:
    [HttpGet]
    public ActionResult<Result<Product>> Get(int id) // ✅ Explicit success/failure
    {
        var product = _repo.GetById(id);
        return product != null
            ? Result<Product>.Success(product)
            : Result<Product>.Failure("Product not found");
    }
}
```

### Example 5: Enforce Async Naming Convention

```csharp
// Rule: Async methods must end with "Async"
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class AsyncMethodNamingAnalyzer : DiagnosticAnalyzer
{
    // Bad:
    public async Task<Product> GetProduct(int id) // ⚠️ Should be "GetProductAsync"
    {
        return await _repo.GetByIdAsync(id);
    }

    // Good:
    public async Task<Product> GetProductAsync(int id) // ✅
    {
        return await _repo.GetByIdAsync(id);
    }
}
```

### Example 6: Detect Async Void

```csharp
// Rule: Avoid async void (except event handlers)
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class AvoidAsyncVoidAnalyzer : DiagnosticAnalyzer
{
    // Bad:
    public async void ProcessData() // ❌ Error: async void swallows exceptions!
    {
        await _service.ProcessAsync();
    }

    // Good:
    public async Task ProcessDataAsync() // ✅
    {
        await _service.ProcessAsync();
    }

    // Exception (allowed):
    private async void Button_Click(object sender, EventArgs e) // ✅ Event handler
    {
        await ProcessDataAsync();
    }
}
```

---

## Implementation Guidelines

### Project Structure

```
Solution
├── YourApp.Analyzers
│   ├── Analyzers/
│   │   ├── ArchitectureAnalyzers/
│   │   │   ├── DomainMustNotReferenceInfrastructureAnalyzer.cs
│   │   │   └── ControllersMustBeInPresentationLayerAnalyzer.cs
│   │   ├── PerformanceAnalyzers/
│   │   │   ├── AvoidStringConcatInLoopAnalyzer.cs
│   │   │   └── UseAsyncMethodsAnalyzer.cs
│   │   └── SecurityAnalyzers/
│   │       └── ValidateUserInputAnalyzer.cs
│   ├── CodeFixes/
│   │   ├── StringConcatCodeFixProvider.cs
│   │   └── AsyncNamingCodeFixProvider.cs
│   └── YourApp.Analyzers.csproj
├── YourApp.Analyzers.Tests (xUnit)
└── YourApp (consumes analyzers)
```

### Analyzer Project Configuration

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

### Consuming Project

```xml
<ItemGroup>
  <ProjectReference Include="..\YourApp.Analyzers\YourApp.Analyzers.csproj"
                    OutputItemType="Analyzer"
                    ReferenceOutputAssembly="false" />
</ItemGroup>

<!-- Treat analyzer warnings as errors in CI -->
<PropertyGroup Condition="'$(CI)' == 'true'">
  <TreatWarningsAsErrors>true</TreatWarningsAsErrors>
</PropertyGroup>
```

### Suppressing False Positives

```csharp
// Suppress specific violation
#pragma warning disable ARCH001
public class Product
{
    public ApplicationDbContext Context { get; set; } // Intentional for this case
}
#pragma warning restore ARCH001

// Or use attribute
[SuppressMessage("Architecture", "ARCH001", Justification = "Legacy code migration")]
public class LegacyProduct { }
```

---

## Testing Analyzers

```csharp
[Fact]
public async Task DomainReferencingInfrastructure_ReportsDiagnostic()
{
    var test = @"
namespace MyApp.Domain
{
    using MyApp.Infrastructure;

    public class Product
    {
        public DbContext Context { get; set; }
    }
}";

    var expected = DiagnosticResult
        .CompilerError("ARCH001")
        .WithSpan(7, 16, 7, 23)
        .WithArguments("Product", "DbContext");

    await VerifyCS.VerifyAnalyzerAsync(test, expected);
}
```

---

## Links

* [Roslyn Analyzer Tutorial](https://learn.microsoft.com/en-us/dotnet/csharp/roslyn-sdk/tutorials/how-to-write-csharp-analyzer-code-fix)
* [Roslyn Analyzer Samples](https://github.com/dotnet/roslyn-sdk/tree/main/samples/CSharp/Analyzers)
* [Analyzer Cookbook](https://github.com/dotnet/roslyn/blob/main/docs/analyzers/Analyzer%20Cookbook.md)
* [Sample Implementation](../../src/AdvancedConcepts.Analyzers)

---

## Notes

**Common Analyzer Categories:**
- **Architecture** (ARCH001-099): Layer dependencies, folder structure
- **Performance** (PERF001-099): String concatenation, LINQ, async/await
- **Security** (SEC001-099): Validation, SQL injection, XSS
- **Design** (DESIGN001-099): Naming conventions, return types
- **Maintainability** (MAINT001-099): Complexity, duplication

**Severity Levels:**
- **Error**: Build fails (architecture violations)
- **Warning**: Build succeeds but should be fixed (performance issues)
- **Info**: Suggestions (optional improvements)

**Performance Considerations:**
- Analyzers run on every keystroke in IDE
- Keep analysis fast (< 10ms per file)
- Use `EnableConcurrentExecution()` for parallelism
- Cache expensive computations

**Review Date:** 2025-12-01
