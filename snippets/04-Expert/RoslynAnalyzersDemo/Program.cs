using System;

namespace RoslynAnalyzersDemo;

/// <summary>
/// Roslyn Analyzers Demo - Using Custom Code Analyzers
/// Demonstrates how custom analyzers catch code issues at compile time
/// </summary>
class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("═══════════════════════════════════════════════════════════════════");
        Console.WriteLine("ROSLYN ANALYZERS DEMO");
        Console.WriteLine("Custom Code Analyzers in Action");
        Console.WriteLine("═══════════════════════════════════════════════════════════════════\n");

        Console.WriteLine("This project demonstrates how Roslyn analyzers catch issues");
        Console.WriteLine("at compile time, before code even runs!\n");

        WhatAreAnalyzers();
        HowAnalyzersWork();
        BenefitsOfAnalyzers();
        AvailableAnalyzers();

        Console.WriteLine("\n\n═══════════════════════════════════════════════════════════════════");
        Console.WriteLine("Demo Complete!");
        Console.WriteLine("═══════════════════════════════════════════════════════════════════");
    }

    static void WhatAreAnalyzers()
    {
        Console.WriteLine("1. WHAT ARE ROSLYN ANALYZERS?");
        Console.WriteLine("─────────────────────────────────────────────────────────────────\n");

        Console.WriteLine("Roslyn Analyzers analyze your code while you type/build.");
        Console.WriteLine("\nTraditional Bug Detection:");
        Console.WriteLine("  Write code → Compile → Run → Bug appears! 🐛");
        Console.WriteLine("  Time to fix: Minutes to hours");

        Console.WriteLine("\nWith Analyzers:");
        Console.WriteLine("  Write code → Analyzer warns immediately! ⚠️");
        Console.WriteLine("  Time to fix: Seconds");

        Console.WriteLine("\nTypes of Analyzers:");
        Console.WriteLine("  • Code Quality (performance, maintainability)");
        Console.WriteLine("  • Security (SQL injection, XSS)");
        Console.WriteLine("  • Style (naming conventions, formatting)");
        Console.WriteLine("  • Best Practices (SOLID principles)");
    }

    static void HowAnalyzersWork()
    {
        Console.WriteLine("\n\n2. HOW ANALYZERS WORK");
        Console.WriteLine("─────────────────────────────────────────────────────────────────\n");

        Console.WriteLine("Roslyn Compiler:");
        Console.WriteLine("  C# Code → Syntax Tree → Semantic Model → IL");

        Console.WriteLine("\nAnalyzers hook into this pipeline:");
        Console.WriteLine("  1. Syntax Tree Analysis");
        Console.WriteLine("     - Looks at code structure");
        Console.WriteLine("     - Example: Empty catch blocks");

        Console.WriteLine("\n  2. Semantic Analysis");
        Console.WriteLine("     - Understands meaning");
        Console.WriteLine("     - Example: Unused variables, boxing");

        Console.WriteLine("\n  3. Symbol Analysis");
        Console.WriteLine("     - Type information");
        Console.WriteLine("     - Example: Covariance violations");

        Console.WriteLine("\nOutput:");
        Console.WriteLine("  • Warnings in IDE (green squiggles)");
        Console.WriteLine("  • Build warnings/errors");
        Console.WriteLine("  • Code fixes (light bulb 💡)");
    }

    static void BenefitsOfAnalyzers()
    {
        Console.WriteLine("\n\n3. BENEFITS OF ANALYZERS");
        Console.WriteLine("─────────────────────────────────────────────────────────────────\n");

        Console.WriteLine("✅ Catch bugs early:");
        Console.WriteLine("  • Before code runs");
        Console.WriteLine("  • Before PR review");
        Console.WriteLine("  • Before production");

        Console.WriteLine("\n✅ Enforce best practices:");
        Console.WriteLine("  • Consistent code style");
        Console.WriteLine("  • Performance patterns");
        Console.WriteLine("  • Security practices");

        Console.WriteLine("\n✅ Automate code reviews:");
        Console.WriteLine("  • Analyzer checks 100% of code");
        Console.WriteLine("  • Humans focus on logic, not style");
        Console.WriteLine("  • Faster PR turnaround");

        Console.WriteLine("\n✅ Education:");
        Console.WriteLine("  • Developers learn from warnings");
        Console.WriteLine("  • Instant feedback loop");
        Console.WriteLine("  • Team knowledge sharing");

        Console.WriteLine("\nReal Impact:");
        Console.WriteLine("  • 30-50% reduction in code review time");
        Console.WriteLine("  • 20-40% fewer bugs in production");
        Console.WriteLine("  • Faster onboarding for new developers");
    }

    static void AvailableAnalyzers()
    {
        Console.WriteLine("\n\n4. AVAILABLE ANALYZERS IN THIS PROJECT");
        Console.WriteLine("─────────────────────────────────────────────────────────────────\n");

        Console.WriteLine("This project includes custom analyzers:\n");

        Console.WriteLine("1. BoxingAnalyzer (AC001)");
        Console.WriteLine("   Detects boxing allocations");
        Console.WriteLine("   ❌ object obj = 123;  // Warning: Boxing!");
        Console.WriteLine("   ✅ int value = 123;");

        Console.WriteLine("\n2. CovarianceAnalyzer (AC002)");
        Console.WriteLine("   Detects incorrect covariance usage");
        Console.WriteLine("   ❌ IEnumerable<object> list = new List<string>(); // OK");
        Console.WriteLine("   ❌ IList<object> list = new List<string>(); // Error!");

        Console.WriteLine("\n3. EmptyCatchAnalyzer (AC003)");
        Console.WriteLine("   Detects empty catch blocks");
        Console.WriteLine("   ❌ try { } catch { }  // Warning: Swallowing errors!");
        Console.WriteLine("   ✅ try { } catch (Exception ex) { Log(ex); }");

        Console.WriteLine("\n4. SealedTypeAnalyzer (AC004)");
        Console.WriteLine("   Suggests sealing classes not intended for inheritance");
        Console.WriteLine("   ⚠️  public class MyClass { }  // Consider sealing");
        Console.WriteLine("   ✅ public sealed class MyClass { }");

        Console.WriteLine("\n\nTo see analyzers in action:");
        Console.WriteLine("  1. Open this project in Visual Studio / VS Code");
        Console.WriteLine("  2. Write code that triggers warnings");
        Console.WriteLine("  3. See green squiggles and warnings");
        Console.WriteLine("  4. Use Quick Fixes (💡 light bulb)");

        Console.WriteLine("\n\nUseful Commands:");
        Console.WriteLine("  dotnet build              # See all warnings");
        Console.WriteLine("  dotnet build -warnaserror # Treat warnings as errors");
        Console.WriteLine("  dotnet format analyzers   # Auto-fix some issues");
    }

    // Example: Code that would trigger analyzers (commented to avoid warnings in this demo)
    
    /*
    // AC001: Boxing warning
    public void BoxingExample()
    {
        object obj = 123;  // ⚠️  AC001: Boxing allocation detected
    }

    // AC003: Empty catch block
    public void EmptyCatchExample()
    {
        try
        {
            // Some code
        }
        catch  // ⚠️  AC003: Empty catch block swallows errors
        {
        }
    }

    // AC004: Should be sealed
    public class NotSealedClass  // ⚠️  AC004: Consider making this class sealed
    {
        public void Method() { }
    }
    */
}

// ✅ Good: Sealed class (no AC004 warning)
public sealed class SealedExampleClass
{
    public void Method()
    {
        Console.WriteLine("This class is sealed - no inheritance allowed");
    }
}

// ✅ Good: Proper exception handling (no AC003 warning)
public class ProperExceptionHandling
{
    public void SafeMethod()
    {
        try
        {
            // Risky code
            int.Parse("not a number");
        }
        catch (FormatException ex)
        {
            // Proper error handling
            Console.WriteLine($"Error: {ex.Message}");
        }
    }
}
