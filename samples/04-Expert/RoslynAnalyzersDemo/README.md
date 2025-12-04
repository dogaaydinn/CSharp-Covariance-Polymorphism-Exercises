# Roslyn Analyzers Demo - Expert Tutorial

> **Level:** Expert  
> **Prerequisites:** C# knowledge, understanding of compilation  
> **Estimated Time:** 1 hour

## 📚 Overview

This demo showcases how custom Roslyn analyzers catch code issues at compile time. Learn how to use, configure, and benefit from static code analysis integrated directly into your build process.

## 🎯 Learning Objectives

- ✅ Understand what Roslyn analyzers are
- ✅ See analyzers catch issues at compile time
- ✅ Use code fixes (light bulb actions)
- ✅ Configure analyzer severity
- ✅ Create safer, more maintainable code

## 🚀 Quick Start

```bash
cd samples/04-Expert/RoslynAnalyzersDemo
dotnet build  # See analyzer warnings
dotnet run
```

## 📊 Available Analyzers

This project references custom analyzers from `src/AdvancedConcepts.Analyzers/`:

### AC001: BoxingAnalyzer
Detects boxing allocations (value type → object).
```csharp
// ❌ Warning AC001
object obj = 123;  // Boxing!

// ✅ Fixed
int value = 123;
```

### AC002: CovarianceAnalyzer
Detects covariance violations.
```csharp
// ❌ Error AC002
IList<object> list = new List<string>();  // Not covariant!

// ✅ Fixed
IEnumerable<object> enumerable = new List<string>();  // Covariant!
```

### AC003: EmptyCatchAnalyzer
Detects empty catch blocks that swallow exceptions.
```csharp
// ❌ Warning AC003
try { } catch { }  // Swallows errors!

// ✅ Fixed
try { } catch (Exception ex) { Log(ex); }
```

### AC004: SealedTypeAnalyzer
Suggests sealing classes not intended for inheritance.
```csharp
// ⚠️  Info AC004
public class MyClass { }  // Consider sealing

// ✅ Fixed
public sealed class MyClass { }
```

## 🔧 Configuration

### Enable/Disable Analyzers

In `.editorconfig` or `Directory.Build.props`:
```ini
# Disable specific analyzer
dotnet_diagnostic.AC001.severity = none

# Make warning an error
dotnet_diagnostic.AC003.severity = error

# Suggestion only
dotnet_diagnostic.AC004.severity = suggestion
```

### Suppress in Code
```csharp
#pragma warning disable AC001
object obj = 123;  // Boxing allowed here
#pragma warning restore AC001
```

## 📈 Benefits

**Time Savings:**
- Catch bugs before they run
- Automate code review checks
- Instant feedback while coding

**Code Quality:**
- Enforce best practices
- Consistent style across team
- Educational for junior developers

**Production Impact:**
- 20-40% fewer bugs
- 30-50% faster code reviews
- Better maintainability

## 🔗 Related

- **Source Generators:** `src/AdvancedConcepts.SourceGenerators/`
- **Analyzer Implementation:** `src/AdvancedConcepts.Analyzers/`
- [Roslyn Documentation](https://github.com/dotnet/roslyn)

---

**Happy Analyzing! 🔍**
