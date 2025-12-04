using SOLIDPrinciples.SingleResponsibility;
using SOLIDPrinciples.OpenClosed;
using SOLIDPrinciples.LiskovSubstitution;
using SOLIDPrinciples.InterfaceSegregation;
using SOLIDPrinciples.DependencyInversion;

namespace SOLIDPrinciples;

/// <summary>
/// SOLID Principles Tutorial
/// Comprehensive examples demonstrating all 5 SOLID principles
/// with violation and correct implementations
/// </summary>
class Program
{
    static void Main(string[] args)
    {
        PrintHeader("SOLID PRINCIPLES TUTORIAL");
        PrintInfo("Learning the foundation of good object-oriented design");
        Console.WriteLine();

        PrintMenu();

        while (true)
        {
            Console.WriteLine();
            Console.Write("Enter your choice (1-6, or 0 to exit): ");
            var choice = Console.ReadLine();

            Console.WriteLine();

            switch (choice)
            {
                case "1":
                    RunSingleResponsibilityPrinciple();
                    break;
                case "2":
                    RunOpenClosedPrinciple();
                    break;
                case "3":
                    RunLiskovSubstitutionPrinciple();
                    break;
                case "4":
                    RunInterfaceSegregationPrinciple();
                    break;
                case "5":
                    RunDependencyInversionPrinciple();
                    break;
                case "6":
                    RunAllPrinciples();
                    break;
                case "0":
                    PrintHeader("THANKS FOR LEARNING!");
                    Console.WriteLine("Remember: SOLID principles help you write maintainable,");
                    Console.WriteLine("flexible, and testable code. Use them wisely!");
                    return;
                default:
                    Console.WriteLine("Invalid choice. Please try again.");
                    break;
            }

            Console.WriteLine();
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
            Console.Clear();
            PrintMenu();
        }
    }

    static void PrintMenu()
    {
        PrintHeader("MAIN MENU");
        Console.WriteLine("1. Single Responsibility Principle (SRP)");
        Console.WriteLine("   - A class should have only ONE reason to change");
        Console.WriteLine();
        Console.WriteLine("2. Open/Closed Principle (OCP)");
        Console.WriteLine("   - Open for extension, closed for modification");
        Console.WriteLine();
        Console.WriteLine("3. Liskov Substitution Principle (LSP)");
        Console.WriteLine("   - Subtypes must be substitutable for their base types");
        Console.WriteLine();
        Console.WriteLine("4. Interface Segregation Principle (ISP)");
        Console.WriteLine("   - Clients shouldn't depend on interfaces they don't use");
        Console.WriteLine();
        Console.WriteLine("5. Dependency Inversion Principle (DIP)");
        Console.WriteLine("   - Depend on abstractions, not concretions");
        Console.WriteLine();
        Console.WriteLine("6. Run ALL principles demonstrations");
        Console.WriteLine();
        Console.WriteLine("0. Exit");
        PrintDivider();
    }

    #region 1. Single Responsibility Principle

    static void RunSingleResponsibilityPrinciple()
    {
        PrintHeader("SINGLE RESPONSIBILITY PRINCIPLE (SRP)");
        PrintInfo("A class should have only ONE reason to change");
        PrintInfo("Each class should have a single, well-defined responsibility");
        Console.WriteLine();

        PrintSubHeader("Memory Aid: 'One Class, One Job'");
        Console.WriteLine("  Think: If you describe the class with 'AND', it's doing too much!");
        Console.WriteLine("  Example: 'UserManager saves users AND sends emails AND logs' - VIOLATION!");
        Console.WriteLine();

        // Demonstrate violations
        PrintSubHeader("VIOLATION Example");
        UserManagerProblemDemo.DemonstrateProblems();

        Console.WriteLine();
        WaitForKey();

        // Demonstrate correct implementation
        PrintSubHeader("CORRECT Implementation");
        SingleResponsibilityDemo.DemonstrateBenefits();
        SingleResponsibilityDemo.DemonstrateReusability();

        PrintSummary("SRP Benefits",
            "Easy to maintain - changes are isolated",
            "Easy to test - each component tested independently",
            "Reusable - components can be used in multiple contexts",
            "Single reason to change - clear responsibilities");
    }

    #endregion

    #region 2. Open/Closed Principle

    static void RunOpenClosedPrinciple()
    {
        PrintHeader("OPEN/CLOSED PRINCIPLE (OCP)");
        PrintInfo("Software entities should be open for EXTENSION but closed for MODIFICATION");
        PrintInfo("You should be able to add new functionality without changing existing code");
        Console.WriteLine();

        PrintSubHeader("Memory Aid: 'Extend, Don't Edit'");
        Console.WriteLine("  Think: Adding new features shouldn't require editing tested code!");
        Console.WriteLine("  Use: Inheritance, interfaces, and abstractions for extension");
        Console.WriteLine();

        // Demonstrate violations
        PrintSubHeader("VIOLATION Example");
        OpenClosedViolationDemo.DemonstrateProblems();

        Console.WriteLine();
        WaitForKey();

        // Demonstrate correct implementation
        PrintSubHeader("CORRECT Implementation");
        OpenClosedDemo.DemonstrateBenefits();

        PrintSummary("OCP Benefits",
            "Add new features without modifying existing code",
            "Reduced risk of breaking existing functionality",
            "Plugin architecture becomes possible",
            "Easier to maintain and extend");
    }

    #endregion

    #region 3. Liskov Substitution Principle

    static void RunLiskovSubstitutionPrinciple()
    {
        PrintHeader("LISKOV SUBSTITUTION PRINCIPLE (LSP)");
        PrintInfo("Objects of a subclass should be substitutable for objects of the superclass");
        PrintInfo("Derived classes must be usable through the base class interface");
        Console.WriteLine();

        PrintSubHeader("Memory Aid: 'Substitution Without Surprise'");
        Console.WriteLine("  Think: If it looks like a duck and quacks like a duck...");
        Console.WriteLine("  ...it should behave like a duck! No unexpected exceptions!");
        Console.WriteLine("  Rule: Child classes should strengthen postconditions, not preconditions");
        Console.WriteLine();

        // Demonstrate violations
        PrintSubHeader("VIOLATION Example");
        LiskovViolationDemo.DemonstrateProblems();

        Console.WriteLine();
        WaitForKey();

        // Demonstrate correct implementation
        PrintSubHeader("CORRECT Implementation");
        LiskovCorrectDemo.DemonstrateBenefits();

        PrintSummary("LSP Benefits",
            "Polymorphism works reliably",
            "No unexpected behaviors or exceptions",
            "Stronger type safety",
            "Easier to reason about code");
    }

    #endregion

    #region 4. Interface Segregation Principle

    static void RunInterfaceSegregationPrinciple()
    {
        PrintHeader("INTERFACE SEGREGATION PRINCIPLE (ISP)");
        PrintInfo("Clients should not be forced to depend on interfaces they don't use");
        PrintInfo("Better to have many small, specific interfaces than one large, general interface");
        Console.WriteLine();

        PrintSubHeader("Memory Aid: 'Many Small Menus, Not One Big Menu'");
        Console.WriteLine("  Think: Like a restaurant - vegetarians don't need the meat menu!");
        Console.WriteLine("  Create: Role-specific interfaces instead of fat interfaces");
        Console.WriteLine("  Avoid: Forcing implementations to throw NotSupportedException");
        Console.WriteLine();

        // Demonstrate violations
        PrintSubHeader("VIOLATION Example");
        InterfaceSegregationViolationDemo.DemonstrateProblems();

        Console.WriteLine();
        WaitForKey();

        // Demonstrate correct implementation
        PrintSubHeader("CORRECT Implementation");
        InterfaceSegregationCorrectDemo.DemonstrateBenefits();

        PrintSummary("ISP Benefits",
            "No forced implementations",
            "Clear capability contracts",
            "Low coupling",
            "Better composability");
    }

    #endregion

    #region 5. Dependency Inversion Principle

    static void RunDependencyInversionPrinciple()
    {
        PrintHeader("DEPENDENCY INVERSION PRINCIPLE (DIP)");
        PrintInfo("High-level modules should not depend on low-level modules");
        PrintInfo("Both should depend on abstractions (interfaces/abstract classes)");
        Console.WriteLine();

        PrintSubHeader("Memory Aid: 'Abstractions, Not Concretions'");
        Console.WriteLine("  Think: Business logic shouldn't know about technical details!");
        Console.WriteLine("  Use: Dependency Injection to provide implementations");
        Console.WriteLine("  Remember: Depend on WHAT (interface), not HOW (implementation)");
        Console.WriteLine();

        // Demonstrate violations
        PrintSubHeader("VIOLATION Example");
        DependencyInversionViolationDemo.DemonstrateAllProblems();

        Console.WriteLine();
        WaitForKey();

        // Demonstrate correct implementation
        PrintSubHeader("CORRECT Implementation");
        DependencyInversionCorrectDemo.DemonstrateBenefits();

        PrintSummary("DIP Benefits",
            "Loose coupling between modules",
            "Easy to test with mocks/stubs",
            "Easy to swap implementations",
            "Runtime composition possible");
    }

    #endregion

    #region Run All Principles

    static void RunAllPrinciples()
    {
        PrintHeader("COMPREHENSIVE SOLID PRINCIPLES DEMONSTRATION");
        Console.WriteLine("Running all 5 principles...");
        Console.WriteLine();

        RunSingleResponsibilityPrinciple();
        Console.WriteLine("\n\n");
        WaitForKey();

        RunOpenClosedPrinciple();
        Console.WriteLine("\n\n");
        WaitForKey();

        RunLiskovSubstitutionPrinciple();
        Console.WriteLine("\n\n");
        WaitForKey();

        RunInterfaceSegregationPrinciple();
        Console.WriteLine("\n\n");
        WaitForKey();

        RunDependencyInversionPrinciple();
        Console.WriteLine("\n\n");

        PrintHeader("SOLID PRINCIPLES SUMMARY");
        Console.WriteLine();

        Console.WriteLine("S - Single Responsibility Principle");
        Console.WriteLine("    One class, one job");
        Console.WriteLine();

        Console.WriteLine("O - Open/Closed Principle");
        Console.WriteLine("    Extend, don't edit");
        Console.WriteLine();

        Console.WriteLine("L - Liskov Substitution Principle");
        Console.WriteLine("    Substitution without surprise");
        Console.WriteLine();

        Console.WriteLine("I - Interface Segregation Principle");
        Console.WriteLine("    Many small menus, not one big menu");
        Console.WriteLine();

        Console.WriteLine("D - Dependency Inversion Principle");
        Console.WriteLine("    Abstractions, not concretions");
        Console.WriteLine();

        PrintDivider();
        Console.WriteLine("When to use SOLID principles:");
        Console.WriteLine("  - Always, but don't over-engineer!");
        Console.WriteLine("  - Start simple, refactor when complexity grows");
        Console.WriteLine("  - Balance pragmatism with good design");
        Console.WriteLine();

        Console.WriteLine("Common mistakes to avoid:");
        Console.WriteLine("  - Applying principles without understanding WHY");
        Console.WriteLine("  - Over-abstracting simple code");
        Console.WriteLine("  - Creating interfaces 'just because'");
        Console.WriteLine("  - Forgetting that SOLID is about maintainability");
        Console.WriteLine();

        Console.WriteLine("The Golden Rule:");
        Console.WriteLine("  SOLID principles help you write code that's easy to:");
        Console.WriteLine("    ✓ Understand");
        Console.WriteLine("    ✓ Maintain");
        Console.WriteLine("    ✓ Test");
        Console.WriteLine("    ✓ Extend");
        Console.WriteLine();
    }

    #endregion

    #region Helper Methods

    static void PrintHeader(string title)
    {
        var separator = new string('=', 80);
        Console.WriteLine(separator);
        Console.WriteLine($"  {title}");
        Console.WriteLine(separator);
    }

    static void PrintSubHeader(string title)
    {
        Console.WriteLine();
        var separator = new string('-', 80);
        Console.WriteLine(separator);
        Console.WriteLine($"  {title}");
        Console.WriteLine(separator);
    }

    static void PrintDivider()
    {
        Console.WriteLine(new string('-', 80));
    }

    static void PrintInfo(string message)
    {
        Console.WriteLine($"  {message}");
    }

    static void PrintSummary(string title, params string[] points)
    {
        Console.WriteLine();
        PrintSubHeader(title);
        foreach (var point in points)
        {
            Console.WriteLine($"  ✓ {point}");
        }
        Console.WriteLine();
    }

    static void WaitForKey()
    {
        Console.WriteLine();
        Console.WriteLine("Press any key to see the correct implementation...");
        Console.ReadKey();
    }

    #endregion
}
