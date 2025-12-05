# Roslyn Analyzer Demo

> **Expert-Level C# Pattern** - Custom code analyzers for enforcing coding standards and best practices at compile time.

## 📋 Quick Reference

**What:** Custom Roslyn diagnostic analyzer with automatic code fixes
**When:** Enforce team coding standards, prevent common mistakes, improve code quality
**Why:** Catch issues at compile time, automate code reviews, ensure consistency
**Level:** Expert (requires understanding of Roslyn APIs and compiler internals)

## 🎯 What This Example Demonstrates

### Core Components

1. **AsyncNamingAnalyzer** - Diagnostic analyzer that enforces async method naming
   - Detects methods returning Task/Task&lt;T&gt; without "Async" suffix
   - Reports warnings with detailed error messages
   - Integrates with IDE (Visual Studio, VS Code, Rider)

2. **AsyncNamingCodeFixProvider** - Automatic refactoring tool
   - One-click fix to add "Async" suffix
   - Renames all references across the project
   - Batch fix all violations at once

3. **Consumer Project** - Working demonstration
   - Good examples (no warnings)
   - Bad examples (triggers analyzer)
   - Real-world usage patterns

## 🚀 Getting Started

### Build and Run

```bash
# Build the analyzer
cd samples/04-Expert/RoslynAnalyzerDemo
dotnet build

# Run the consumer (see analyzer in action)
cd Consumer
dotnet build  # You'll see warnings for BadExample.cs
dotnet run
```

### Expected Output

```
Build output:
  BadExample.cs(12,23): warning ASYNC001: Method 'FetchUser' returns Task but doesn't end with 'Async'
  BadExample.cs(18,31): warning ASYNC001: Method 'GetData' returns Task but doesn't end with 'Async'
  BadExample.cs(24,27): warning ASYNC001: Method 'SaveRecord' returns Task but doesn't end with 'Async'
  BadExample.cs(30,32): warning ASYNC001: Method 'LoadConfig' returns Task but doesn't end with 'Async'

Program output:
  === Roslyn Analyzer Demo ===

  Fetching data...
  ✅ Data fetched successfully
  Processing items...
  ✅ Processed 42 items

  ✅ All async methods follow naming conventions!
```

## 📖 How It Works

### 1. Analyzer Registration

```csharp
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class AsyncNamingAnalyzer : DiagnosticAnalyzer
{
    public override void Initialize(AnalysisContext context)
    {
        // Register callback for method declarations
        context.RegisterSyntaxNodeAction(
            AnalyzeMethodDeclaration,
            SyntaxKind.MethodDeclaration
        );
    }
}
```

**Key Points:**
- Attribute marks class as C# analyzer
- `RegisterSyntaxNodeAction` subscribes to AST node events
- Called for every method declaration during compilation

### 2. Semantic Analysis

```csharp
private static void AnalyzeMethodDeclaration(SyntaxNodeAnalysisContext context)
{
    var methodSymbol = context.SemanticModel.GetDeclaredSymbol(methodDeclaration);

    if (methodSymbol.Name.EndsWith("Async"))
        return; // Already correct

    if (IsTaskType(methodSymbol.ReturnType))
    {
        context.ReportDiagnostic(diagnostic); // Report violation
    }
}
```

**Key Concepts:**
- **Syntax** - What the code looks like (AST structure)
- **Semantic** - What the code means (types, symbols, references)
- **Symbol** - Represents a declared entity (method, type, field)

### 3. Code Fix Provider

```csharp
[ExportCodeFixProvider(LanguageNames.CSharp), Shared]
public class AsyncNamingCodeFixProvider : CodeFixProvider
{
    public override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
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

**Key Points:**
- Uses Roslyn's `Renamer` API to update all references
- Works across entire solution, not just current file
- Preserves code formatting and comments

## 🔍 Project Structure

```
RoslynAnalyzerDemo/
├── Analyzer/                          # Analyzer library (netstandard2.0)
│   ├── Analyzer.csproj               # Must target netstandard2.0
│   ├── AsyncNamingAnalyzer.cs        # Diagnostic analyzer
│   └── AsyncNamingCodeFixProvider.cs # Automatic fix provider
│
├── Consumer/                          # Test project (net8.0)
│   ├── Consumer.csproj               # References analyzer as ProjectReference
│   ├── Program.cs                    # Main demo
│   ├── BadExample.cs                 # Triggers warnings
│   └── GoodExample.cs                # No warnings
│
└── README.md, docs...
```

### Why netstandard2.0?

Analyzers run **inside the compiler process**, which must support all IDEs:
- Visual Studio 2019+ (runs on .NET Framework)
- VS Code + C# extension
- JetBrains Rider

**netstandard2.0** is the common denominator that works everywhere.

## 💡 Key Concepts

### Diagnostic Severity Levels

```csharp
DiagnosticSeverity.Error    // ❌ Prevents compilation
DiagnosticSeverity.Warning  // ⚠️  Shows warning (default)
DiagnosticSeverity.Info     // ℹ️  Informational message
DiagnosticSeverity.Hidden   // 🔇 Silent (code fix only)
```

### Analysis Context Types

| Context | Use Case | Example |
|---------|----------|---------|
| `SyntaxNodeAction` | Analyze specific node types | Method declarations, if statements |
| `SymbolAction` | Analyze symbols | Classes, methods, properties |
| `OperationAction` | Analyze operations | Assignments, invocations |
| `CompilationAction` | Whole-compilation analysis | Project-wide rules |

### Performance Considerations

1. **Register Specific Node Types**
   ```csharp
   // ✅ GOOD: Only analyzes methods
   context.RegisterSyntaxNodeAction(Analyze, SyntaxKind.MethodDeclaration);

   // ❌ BAD: Analyzes every node (slow!)
   context.RegisterSyntaxNodeAction(Analyze, SyntaxKind.IdentifierName);
   ```

2. **Enable Concurrent Execution**
   ```csharp
   context.EnableConcurrentExecution(); // Parallel analysis
   ```

3. **Skip Generated Code**
   ```csharp
   context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
   ```

## 🎓 Learning Checklist

### Beginner Level
- [ ] Understand what Roslyn analyzers do
- [ ] Run the demo and see warnings
- [ ] Use code fix to automatically rename methods
- [ ] Identify violations in your own code

### Intermediate Level
- [ ] Read AsyncNamingAnalyzer.cs line by line
- [ ] Understand syntax vs semantic analysis
- [ ] Modify the analyzer to check for different patterns
- [ ] Create a new diagnostic rule (e.g., class naming)

### Advanced Level
- [ ] Implement a new analyzer from scratch
- [ ] Write unit tests using Roslyn test framework
- [ ] Handle edge cases (generic methods, async lambdas)
- [ ] Optimize performance for large codebases

### Expert Level
- [ ] Build multi-rule analyzer suite for your team
- [ ] Package as NuGet and distribute
- [ ] Integrate with CI/CD pipeline
- [ ] Write analyzers that work with other analyzers

## 🛠️ Extending This Example

### Add New Rules

Create analyzers for:
- **Naming conventions** - PascalCase for classes, camelCase for fields
- **API design** - IDisposable implementation checks
- **Security** - Detect SQL injection, XSS vulnerabilities
- **Performance** - Find LINQ queries that cause N+1 problems

### Example: Detect Sync-over-Async

```csharp
// Detect .Result and .Wait() on Task (deadlock risk)
private static void AnalyzeInvocation(SyntaxNodeAnalysisContext context)
{
    var invocation = (InvocationExpressionSyntax)context.Node;
    var memberAccess = invocation.Expression as MemberAccessExpressionSyntax;

    if (memberAccess?.Name.Identifier.Text is "Result" or "Wait")
    {
        // Report ASYNC002: Sync-over-async detected (deadlock risk)
        context.ReportDiagnostic(...);
    }
}
```

## 📚 Real-World Usage

### Microsoft's Built-in Analyzers

- **CA1001** - Types that own disposable fields should be disposable
- **CA1014** - Mark assemblies with CLSCompliant
- **CA1031** - Do not catch general exception types
- **CA1062** - Validate arguments of public methods
- **IDE0008** - Use explicit type instead of 'var'

### Popular Third-Party Analyzers

| Analyzer | Purpose | Rules |
|----------|---------|-------|
| **StyleCop.Analyzers** | Code style consistency | 200+ |
| **Roslynator** | Code quality & refactoring | 500+ |
| **SonarAnalyzer** | Security & reliability | 600+ |
| **Meziantou.Analyzer** | Best practices | 150+ |

### Team Adoption Strategy

1. **Start Small** - 1-2 rules, Warning severity
2. **Educate Team** - Explain why rules exist
3. **Enable Code Fixes** - Make compliance easy
4. **Gradual Rollout** - Add rules incrementally
5. **CI Enforcement** - Fail builds on errors

## 🔗 Related Patterns

| Pattern | Purpose | Relationship |
|---------|---------|--------------|
| **Source Generators** | Generate code at compile time | Complement analyzers |
| **EditorConfig** | Configure analyzer rules | `.editorconfig` settings |
| **Code Metrics** | Measure code complexity | Analyzers can enforce limits |
| **Linting** | Style and quality checks | Analyzers are C# linters |

## 📖 Additional Resources

### Official Documentation
- [Roslyn Analyzers Overview](https://learn.microsoft.com/en-us/dotnet/csharp/roslyn-sdk/)
- [Tutorial: Write your first analyzer](https://learn.microsoft.com/en-us/dotnet/csharp/roslyn-sdk/tutorials/how-to-write-csharp-analyzer-code-fix)
- [Diagnostic Analyzer API](https://learn.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.diagnostics.diagnosticanalyzer)

### Books & Courses
- **Roslyn Succinctly** by Alessandro Del Sole (Free ebook)
- **C# Compiler Platform (Roslyn) API** on Pluralsight

## 🎯 Next Steps

1. **Explore** `samples/04-Expert/SourceGenerators/` - Code generation complement
2. **Read** `WHY_THIS_PATTERN.md` - Deeper dive into problem/solution
3. **Study** `CAREER_IMPACT.md` - Salary data and interview prep
4. **Review** `PERFORMANCE_NOTES.md` - Analyzer performance optimization
5. **Avoid** `COMMON_MISTAKES.md` - Pitfalls and how to fix them

---

**📝 Summary:** Custom Roslyn analyzers enforce coding standards at compile time, catching issues before code review. They integrate seamlessly with all IDEs and can provide automatic fixes. Essential for maintaining code quality at scale.

**🎓 Key Takeaway:** Analyzers shift code quality checks left - from code review to compile time. They're like unit tests for coding standards, running automatically on every build.
