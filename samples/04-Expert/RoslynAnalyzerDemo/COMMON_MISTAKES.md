# Common Mistakes When Building Roslyn Analyzers

## Mistake #1: Wrong Target Framework

### The Problem

**❌ WRONG:**
```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>  <!-- ❌ -->
  </PropertyGroup>
</Project>
```

**Error:**
```
Analyzer 'AsyncNamingAnalyzer' failed to load:
Could not load file or assembly 'System.Runtime, Version=8.0.0.0'
```

### Why It Fails

Analyzers run **inside the compiler process**, which must support:
- Visual Studio 2019+ (runs on .NET Framework 4.7.2)
- VS Code + OmniSharp
- Rider

Only **netstandard2.0** works everywhere.

### The Fix

**✅ CORRECT:**
```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>netstandard2.0</TargetFramework>  <!-- ✅ -->
    <LangVersion>12.0</LangVersion>
    <Nullable>enable</Nullable>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Microsoft.CodeAnalysis.CSharp" Version="4.8.0" />
  </ItemGroup>
</Project>
```

**Key Points:**
- **Analyzer:** Must be `netstandard2.0`
- **Consumer:** Can be any framework (net8.0, net6.0, etc.)
- **Language Features:** C# 12 works in netstandard2.0 (syntax only)

---

## Mistake #2: Forgetting to Set PrivateAssets

### The Problem

**❌ WRONG:**
```xml
<ItemGroup>
  <PackageReference Include="Microsoft.CodeAnalysis.CSharp" Version="4.8.0" />
  <!-- Missing PrivateAssets="all" -->
</ItemGroup>
```

**Result:**
```
Consumer project now references Microsoft.CodeAnalysis.CSharp (10+ MB)
Binary size bloat: 2 MB → 15 MB!
```

### Why It Happens

Without `PrivateAssets="all"`, NuGet includes analyzer dependencies in consuming projects.

### The Fix

**✅ CORRECT:**
```xml
<ItemGroup>
  <PackageReference Include="Microsoft.CodeAnalysis.CSharp" Version="4.8.0"
                    PrivateAssets="all" />  <!-- ✅ Don't flow to consumers -->
  <PackageReference Include="Microsoft.CodeAnalysis.Analyzers" Version="3.3.4"
                    PrivateAssets="all" />
</ItemGroup>
```

**Key Point:** Analyzer dependencies are **build-time only**, not runtime.

---

## Mistake #3: Not Enabling Concurrent Execution

### The Problem

**❌ WRONG:**
```csharp
public override void Initialize(AnalysisContext context)
{
    // Missing context.EnableConcurrentExecution()
    context.RegisterSyntaxNodeAction(Analyze, SyntaxKind.MethodDeclaration);
}
```

**Performance:** 4x slower on quad-core machines!

### The Fix

**✅ CORRECT:**
```csharp
public override void Initialize(AnalysisContext context)
{
    context.EnableConcurrentExecution();  // ✅ Always include this!
    context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);

    context.RegisterSyntaxNodeAction(Analyze, SyntaxKind.MethodDeclaration);
}
```

**Impact:** 2-4x faster builds (uses all CPU cores).

---

## Mistake #4: Analyzing Generated Code

### The Problem

**❌ WRONG:**
```csharp
public override void Initialize(AnalysisContext context)
{
    // Analyzes auto-generated files (migrations, designer files)
    context.RegisterSyntaxNodeAction(Analyze, SyntaxKind.MethodDeclaration);
}
```

**Result:**
- Warnings in auto-generated files users can't fix
- 20-50% slower builds (wasted effort)
- False positives in migration files

### The Fix

**✅ CORRECT:**
```csharp
public override void Initialize(AnalysisContext context)
{
    context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);  // ✅

    context.RegisterSyntaxNodeAction(Analyze, SyntaxKind.MethodDeclaration);
}
```

**Key Point:** Skip generated code unless you specifically need to analyze it.

---

## Mistake #5: Using String Comparisons on Types

### The Problem

**❌ WRONG:**
```csharp
private void AnalyzeMethod(SyntaxNodeAnalysisContext context)
{
    var method = (MethodDeclarationSyntax)context.Node;
    var returnType = method.ReturnType.ToString();  // ❌ Syntax-based!

    if (returnType == "Task")  // ❌ Fails for aliases, fully-qualified names
    {
        // This won't catch:
        // - Task<string> (has generic parameter)
        // - System.Threading.Tasks.Task (fully qualified)
        // - using MyTask = Task; (alias)
    }
}
```

### The Fix

**✅ CORRECT:**
```csharp
private void AnalyzeMethod(SyntaxNodeAnalysisContext context)
{
    var method = (MethodDeclarationSyntax)context.Node;
    var methodSymbol = context.SemanticModel.GetDeclaredSymbol(method);

    if (methodSymbol is null)
        return;

    // ✅ Use semantic analysis (symbol-based)
    var returnType = methodSymbol.ReturnType;

    if (IsTaskType(returnType))  // Checks actual type, not syntax
    {
        // Catches all variants:
        // - Task, Task<T>
        // - ValueTask, ValueTask<T>
        // - Fully-qualified, aliased, etc.
    }
}

private static bool IsTaskType(ITypeSymbol typeSymbol)
{
    var typeName = typeSymbol.OriginalDefinition.ToDisplayString();

    return typeName == "System.Threading.Tasks.Task" ||
           typeName == "System.Threading.Tasks.Task<TResult>" ||
           typeName == "System.Threading.Tasks.ValueTask" ||
           typeName == "System.Threading.Tasks.ValueTask<TResult>";
}
```

**Key Point:** Use **semantic analysis** for type checking, not string comparison.

---

## Mistake #6: Not Handling Null Symbols

### The Problem

**❌ WRONG:**
```csharp
private void AnalyzeMethod(SyntaxNodeAnalysisContext context)
{
    var method = (MethodDeclarationSyntax)context.Node;

    var symbol = context.SemanticModel.GetDeclaredSymbol(method);

    // ❌ NullReferenceException if symbol is null!
    if (symbol.ReturnType.Name == "Task")
    {
        // ...
    }
}
```

**When Does This Happen?**
- Syntax errors in code being analyzed
- Partial methods without implementation
- External assembly methods without metadata

### The Fix

**✅ CORRECT:**
```csharp
private void AnalyzeMethod(SyntaxNodeAnalysisContext context)
{
    var method = (MethodDeclarationSyntax)context.Node;

    var symbol = context.SemanticModel.GetDeclaredSymbol(method);

    // ✅ Always null-check symbols!
    if (symbol is null)
        return;

    if (IsTaskType(symbol.ReturnType))
    {
        // ...
    }
}
```

**Key Point:** **Always** null-check symbols from semantic model.

---

## Mistake #7: Registering Wrong Action Types

### The Problem

**❌ WRONG - Using IdentifierName for Method Analysis:**
```csharp
// This fires for EVERY identifier in the code!
context.RegisterSyntaxNodeAction(context =>
{
    var identifier = (IdentifierNameSyntax)context.Node;

    // Trying to detect async methods this way is very inefficient
    if (identifier.Identifier.Text.EndsWith("Async"))
    {
        // This catches variable names, parameter names, EVERYTHING
        // Not just method names!
    }
}, SyntaxKind.IdentifierName);  // ❌ Too broad!
```

**Performance:** Called 50,000+ times in large files!

### The Fix

**✅ CORRECT - Use Specific Node Type:**
```csharp
// Only fires for method declarations (~50 times in large files)
context.RegisterSyntaxNodeAction(context =>
{
    var method = (MethodDeclarationSyntax)context.Node;

    if (method.Identifier.Text.EndsWith("Async"))
    {
        // Now we're specifically analyzing method names
    }
}, SyntaxKind.MethodDeclaration);  // ✅ Specific!
```

**Impact:** **100x faster**

---

## Mistake #8: Forgetting Code Fix Provider

### The Problem

**❌ WRONG:**
```csharp
// Only analyzer, no code fix
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class AsyncNamingAnalyzer : DiagnosticAnalyzer
{
    // Detects violations, but users have to fix manually ❌
}
```

**User Experience:**
```
⚠️  ASYNC001: Method 'FetchData' should end with 'Async'

User: "OK... now I have to:
1. Rename method manually
2. Find all references
3. Update each one
4. Hope I didn't miss any"
```

### The Fix

**✅ CORRECT:**
```csharp
[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(AsyncNamingCodeFixProvider)), Shared]
public class AsyncNamingCodeFixProvider : CodeFixProvider
{
    public override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        // Provide automatic fix!
        context.RegisterCodeFix(
            CodeAction.Create(
                title: "Add 'Async' suffix",
                createChangedSolution: c => AddAsyncSuffixAsync(...),
                equivalenceKey: nameof(AsyncNamingCodeFixProvider)
            ),
            diagnostic
        );
    }
}
```

**User Experience:**
```
⚠️  ASYNC001: Method 'FetchData' should end with 'Async'
    💡 Quick Actions (Ctrl+.)
       → Add 'Async' suffix

User: *Clicks once* ✅ Fixed!
```

**Key Point:** Always provide code fixes when possible. Users love one-click fixes!

---

## Mistake #9: Poor Diagnostic Messages

### The Problem

**❌ WRONG:**
```csharp
private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(
    id: "ASYNC001",
    title: "Bad method name",  // ❌ Vague
    messageFormat: "Fix this",  // ❌ Not helpful
    category: "Naming",
    defaultSeverity: DiagnosticSeverity.Warning,
    isEnabledByDefault: true
);
```

**User sees:**
```
⚠️  ASYNC001: Fix this
    ^
    What should I fix? How?
```

### The Fix

**✅ CORRECT:**
```csharp
private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(
    id: "ASYNC001",
    title: "Async method should end with 'Async' suffix",  // ✅ Clear
    messageFormat: "Method '{0}' returns Task but doesn't end with 'Async'",  // ✅ Specific
    category: "Naming",
    defaultSeverity: DiagnosticSeverity.Warning,
    isEnabledByDefault: true,
    description: "Methods returning Task or Task<T> should have names ending with 'Async' to indicate asynchronous behavior."  // ✅ Explanation
);

// Use like this:
context.ReportDiagnostic(Diagnostic.Create(
    Rule,
    method.Identifier.GetLocation(),
    method.Identifier.Text  // Passed as {0} in messageFormat
));
```

**User sees:**
```
⚠️  ASYNC001: Method 'FetchData' returns Task but doesn't end with 'Async'
    ^
    Clear problem, clear solution!
```

**Good Message Checklist:**
- ✅ **What** is wrong: "Method returns Task"
- ✅ **Why** it's wrong: "doesn't end with 'Async'"
- ✅ **How** to fix: (code fix provides automatic fix)
- ✅ **Where** exactly: Highlights method name, not entire method

---

## Mistake #10: Not Testing Edge Cases

### The Problem

**❌ WRONG:**
```csharp
// Only tested happy path:
public async Task FetchDataAsync() { }  // ✅ Works

// Didn't test:
public async Task<Task> GetNestedTask() { }  // What about nested Task?
public async ValueTask ProcessAsync() { }    // ValueTask?
public Task Property => Task.CompletedTask;  // Properties?
public Task this[int i] => Task.CompletedTask;  // Indexers?
```

### The Fix

**✅ CORRECT - Comprehensive Tests:**
```csharp
[Fact]
public async Task DetectsTaskWithoutAsyncSuffix()
{
    var code = @"
using System.Threading.Tasks;
class Test {
    public async Task FetchData() { }  // Should warn
}";

    var diagnostic = await GetDiagnosticsAsync(code);
    Assert.Single(diagnostic);
}

[Fact]
public async Task AllowsValueTaskWithAsyncSuffix()
{
    var code = @"
using System.Threading.Tasks;
class Test {
    public async ValueTask ProcessAsync() { }  // Should NOT warn
}";

    var diagnostics = await GetDiagnosticsAsync(code);
    Assert.Empty(diagnostics);
}

[Fact]
public async Task IgnoresTaskProperties()
{
    var code = @"
using System.Threading.Tasks;
class Test {
    public Task MyProperty => Task.CompletedTask;  // Should NOT warn (not a method)
}";

    var diagnostics = await GetDiagnosticsAsync(code);
    Assert.Empty(diagnostics);
}

[Fact]
public async Task HandlesNestedTask()
{
    var code = @"
using System.Threading.Tasks;
class Test {
    public async Task<Task> GetNestedTask() { }  // Should warn
}";

    var diagnostic = await GetDiagnosticsAsync(code);
    Assert.Single(diagnostic);
}
```

**Edge Cases to Test:**
- ✅ Task, Task&lt;T&gt;, ValueTask, ValueTask&lt;T&gt;
- ✅ Nested tasks (Task&lt;Task&gt;)
- ✅ Properties vs methods
- ✅ Indexers, operators
- ✅ Partial methods
- ✅ Interface methods vs implementations
- ✅ Generic methods
- ✅ Async lambdas

---

## Mistake #11: Hardcoding Diagnostic IDs

### The Problem

**❌ WRONG:**
```csharp
// Analyzer
public const string DiagnosticId = "ASYNC001";

// Code fix provider (different file)
public override ImmutableArray<string> FixableDiagnosticIds =>
    ImmutableArray.Create("ASYNC001");  // ❌ Hardcoded, can get out of sync!
```

### The Fix

**✅ CORRECT:**
```csharp
// Analyzer
public class AsyncNamingAnalyzer : DiagnosticAnalyzer
{
    public const string DiagnosticId = "ASYNC001";
}

// Code fix provider
public class AsyncNamingCodeFixProvider : CodeFixProvider
{
    public override ImmutableArray<string> FixableDiagnosticIds =>
        ImmutableArray.Create(AsyncNamingAnalyzer.DiagnosticId);  // ✅ Reference!
}
```

**Key Point:** Use constants, avoid magic strings.

---

## Mistake #12: Not Configuring .editorconfig Support

### The Problem

**❌ WRONG:**
```csharp
// Analyzer has fixed severity, no way to configure
private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(
    id: "ASYNC001",
    defaultSeverity: DiagnosticSeverity.Error,  // ❌ Can't change!
    isEnabledByDefault: true
);
```

**Users can't customize:**
- Can't downgrade to Warning
- Can't disable for specific files
- Can't enable only for specific projects

### The Fix

**✅ CORRECT:**

Analyzers automatically support .editorconfig! Just use default severity:

```csharp
private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(
    id: "ASYNC001",
    defaultSeverity: DiagnosticSeverity.Warning,  // ✅ Default, can override
    isEnabledByDefault: true
);
```

**Users can now configure:**

**.editorconfig:**
```ini
# Make it an error for this project
dotnet_diagnostic.ASYNC001.severity = error

# Disable for generated files
[**/Migrations/*.cs]
dotnet_diagnostic.ASYNC001.severity = none

# Warning in test projects
[**/*Tests/*.cs]
dotnet_diagnostic.ASYNC001.severity = warning
```

---

## Debugging Checklist

### Analyzer Not Running?

**Check:**
1. ✅ Is target framework `netstandard2.0`?
2. ✅ Is analyzer referenced with `OutputItemType="Analyzer"`?
   ```xml
   <ProjectReference Include="../Analyzer/Analyzer.csproj"
                     OutputItemType="Analyzer"
                     ReferenceOutputAssembly="false" />
   ```
3. ✅ Did you rebuild after changes? (Ctrl+Shift+B)
4. ✅ Try clean rebuild: `dotnet clean && dotnet build`
5. ✅ Check Visual Studio > View > Error List > Show Warnings

### Analyzer Crashes?

**Check:**
1. ✅ Null-check all symbols
2. ✅ Handle syntax errors gracefully
3. ✅ Don't throw exceptions (they're swallowed silently!)
4. ✅ Attach debugger:
   ```csharp
   #if DEBUG
       if (!Debugger.IsAttached)
           Debugger.Launch();
   #endif
   ```

### Performance Issues?

**Check:**
1. ✅ Enabled concurrent execution?
2. ✅ Skipping generated code?
3. ✅ Registering specific node types?
4. ✅ Using syntax predicates?
5. ✅ Profile with BenchmarkDotNet

---

## Best Practices Summary

**✅ DO:**
- Use `netstandard2.0` for analyzers
- Enable concurrent execution
- Skip generated code
- Null-check symbols
- Provide code fixes
- Write clear diagnostic messages
- Test edge cases
- Profile performance

**❌ DON'T:**
- Use string comparisons for types (use symbols)
- Forget `PrivateAssets="all"` on dependencies
- Analyze every identifier (use specific node types)
- Throw exceptions (they're swallowed)
- Hardcode diagnostic IDs
- Ignore performance

---

## Quick Reference

### Analyzer Template

```csharp
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class MyAnalyzer : DiagnosticAnalyzer
{
    public const string DiagnosticId = "MY001";

    private static readonly DiagnosticDescriptor Rule = new(
        id: DiagnosticId,
        title: "Clear title",
        messageFormat: "Method '{0}' violates rule because {1}",
        category: "Naming",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: "Detailed explanation"
    );

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
        ImmutableArray.Create(Rule);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();

        context.RegisterSyntaxNodeAction(AnalyzeNode, SyntaxKind.MethodDeclaration);
    }

    private static void AnalyzeNode(SyntaxNodeAnalysisContext context)
    {
        var method = (MethodDeclarationSyntax)context.Node;

        // Syntax-based filtering (fast)
        if (!method.Identifier.Text.Contains("Something"))
            return;

        // Semantic analysis (slower, only when needed)
        var symbol = context.SemanticModel.GetDeclaredSymbol(method);
        if (symbol is null)
            return;

        // Check rule violation
        if (ViolatesRule(symbol))
        {
            var diagnostic = Diagnostic.Create(
                Rule,
                method.Identifier.GetLocation(),
                method.Identifier.Text,
                "reason"
            );

            context.ReportDiagnostic(diagnostic);
        }
    }

    private static bool ViolatesRule(IMethodSymbol symbol)
    {
        // Your rule logic here
        return false;
    }
}
```

---

**Conclusion:** Avoid these common mistakes and your analyzers will be fast, reliable, and loved by your team!

**Next:** Start building your own custom analyzers using the patterns in this example!
