# SOLID Principles Tutorial

Comprehensive examples demonstrating all 5 SOLID principles with violation and correct implementations in C#.

## Overview

SOLID is an acronym for five design principles intended to make software designs more understandable, flexible, and maintainable. These principles were introduced by Robert C. Martin (Uncle Bob) and form the foundation of good object-oriented design.

## Table of Contents

1. [Single Responsibility Principle (SRP)](#1-single-responsibility-principle-srp)
2. [Open/Closed Principle (OCP)](#2-openclosed-principle-ocp)
3. [Liskov Substitution Principle (LSP)](#3-liskov-substitution-principle-lsp)
4. [Interface Segregation Principle (ISP)](#4-interface-segregation-principle-isp)
5. [Dependency Inversion Principle (DIP)](#5-dependency-inversion-principle-dip)
6. [Running the Examples](#running-the-examples)
7. [When to Use SOLID](#when-to-use-solid)
8. [Common Mistakes](#common-mistakes)

---

## 1. Single Responsibility Principle (SRP)

> **A class should have only ONE reason to change**

### Simple Explanation

Each class should have a single, well-defined responsibility. If you can describe what a class does using "AND", it's probably doing too much!

### Memory Aid: "One Class, One Job"

Think of it like employees in a company:
- A chef cooks
- A waiter serves
- A cashier handles payments

You wouldn't ask the chef to also handle payments and clean tables. Same logic applies to classes.

### When It Matters Most

- **Maintainability**: Changes to one responsibility don't affect others
- **Testing**: Each component can be tested independently
- **Reusability**: Components can be used in multiple contexts
- **Team collaboration**: Different developers can work on different responsibilities

### Examples in This Project

**Violation** (`SingleResponsibility/Violation.cs`):
```csharp
public class UserManagerViolation
{
    public void ValidateUser() { }      // Responsibility #1: Validation
    public void SaveToDatabase() { }    // Responsibility #2: Persistence
    public void SendWelcomeEmail() { }  // Responsibility #3: Communication
    public void LogUserCreation() { }   // Responsibility #4: Logging
}
```

**Problems**:
- Hard to test (must mock database, email, logging together)
- Changes to email logic might break database logic
- Can't reuse validation without bringing entire class

**Correct** (`SingleResponsibility/Correct.cs`):
```csharp
public class UserValidator { }           // Only validates
public class UserRepository { }          // Only persists data
public class EmailService { }            // Only sends emails
public class UserLogger { }              // Only logs events
public class UserRegistrationService { } // Orchestrates the workflow
```

**Benefits**:
- Easy to test each component separately
- Changes are isolated to specific classes
- Components are reusable across the application

### Common Mistakes

1. **God Objects**: One class doing everything
2. **Mixed Concerns**: Business logic mixed with infrastructure
3. **Utility Classes**: Dumping unrelated methods into a single class
4. **Over-segregation**: Creating too many tiny classes (use judgment!)

---

## 2. Open/Closed Principle (OCP)

> **Open for extension, closed for modification**

### Simple Explanation

You should be able to add new functionality without changing existing, tested code. Think: "Extend, Don't Edit"

### Memory Aid: "Plugin Architecture"

Like browser extensions:
- The browser is CLOSED for modification (you don't edit Chrome's source code)
- The browser is OPEN for extension (you can add new plugins)

Your code should work the same way!

### When It Matters Most

- **Adding features**: New requirements shouldn't force code changes
- **Stability**: Existing code remains tested and stable
- **Plugins**: Enable plugin/module architecture
- **Risk reduction**: Less chance of breaking existing functionality

### Examples in This Project

**Violation** (`OpenClosed/Violation.cs`):
```csharp
public class PaymentProcessorViolation
{
    public void ProcessPayment(string type, decimal amount)
    {
        if (type == "CreditCard") { /* ... */ }
        else if (type == "PayPal") { /* ... */ }
        // Need Bitcoin? MUST MODIFY THIS METHOD!
    }
}
```

**Problems**:
- Must modify the class for each new payment method
- Risk of breaking existing payment methods
- Growing if-else chains
- Can't extend at runtime

**Correct** (`OpenClosed/Correct.cs`):
```csharp
public abstract class PaymentMethod
{
    public abstract void ExecutePayment(decimal amount);
}

public class CreditCardPayment : PaymentMethod { }
public class PayPalPayment : PaymentMethod { }
public class BitcoinPayment : PaymentMethod { }  // Just add new class!
```

**Benefits**:
- Add new payment methods without modifying existing code
- Each payment method is isolated
- Can load payment methods from plugins
- Existing code remains stable

### Common Mistakes

1. **Premature Abstraction**: Creating abstractions before you need them
2. **Over-engineering**: Every class doesn't need an interface
3. **Ignoring Simple Cases**: Sometimes a simple if-else is fine
4. **Wrong Abstractions**: Creating abstractions that don't fit the domain

---

## 3. Liskov Substitution Principle (LSP)

> **Subtypes must be substitutable for their base types**

### Simple Explanation

If you have a base class and a derived class, you should be able to use the derived class wherever the base class is expected, without surprises.

### Memory Aid: "Substitution Without Surprise"

**The Duck Test**: If it looks like a duck and quacks like a duck, it should BEHAVE like a duck!

No unexpected exceptions, no broken contracts.

### When It Matters Most

- **Polymorphism**: Using base class references safely
- **Design by Contract**: Derived classes honor parent contracts
- **Predictability**: No surprises when substituting types
- **Type Safety**: Compiler helps prevent violations

### Examples in This Project

**Violation** (`LiskovSubstitution/Violation.cs`):
```csharp
public class Rectangle
{
    public virtual int Width { get; set; }
    public virtual int Height { get; set; }
}

public class Square : Rectangle
{
    public override int Width
    {
        set { base.Width = base.Height = value; }  // VIOLATES LSP!
    }
}

// Client code breaks:
Rectangle rect = new Square();
rect.Width = 5;
rect.Height = 10;
// Expected area: 50
// Actual area: 100  (SURPRISE!)
```

**Problems**:
- Setting Width also changes Height (unexpected behavior)
- Client code can't rely on Rectangle's contract
- Polymorphism doesn't work as expected

**Correct** (`LiskovSubstitution/Correct.cs`):
```csharp
public abstract class Shape
{
    public abstract int CalculateArea();
}

public class Rectangle : Shape
{
    public int Width { get; }
    public int Height { get; }
    public Rectangle(int width, int height) { /* ... */ }
}

public class Square : Shape
{
    public int Side { get; }
    public Square(int side) { /* ... */ }
}
```

**Benefits**:
- Square doesn't inherit from Rectangle (no forced relationship)
- Each shape has its own contract
- No surprises when using polymorphism
- Type safety

### Common Mistakes

1. **Rectangle/Square Problem**: Forcing relationships that don't fit
2. **Throwing Exceptions**: Derived class throws exception parent doesn't
3. **Strengthening Preconditions**: Derived class requires more than parent
4. **Weakening Postconditions**: Derived class guarantees less than parent

---

## 4. Interface Segregation Principle (ISP)

> **Clients shouldn't depend on interfaces they don't use**

### Simple Explanation

Better to have many small, specific interfaces than one large, general interface. Like restaurant menus: vegetarians don't need to see the meat menu!

### Memory Aid: "Many Small Menus, Not One Big Menu"

Think about ordering at a restaurant:
- **Bad**: One massive menu with everything (confusing, overwhelming)
- **Good**: Separate menus: Appetizers, Main Course, Desserts, Drinks

Same with interfaces: segregate by role/capability!

### When It Matters Most

- **Forced Implementations**: Avoid implementing methods you don't support
- **Clear Contracts**: Interfaces clearly indicate capabilities
- **Composability**: Mix and match interfaces as needed
- **Low Coupling**: Changes to unused interfaces don't affect clients

### Examples in This Project

**Violation** (`InterfaceSegregation/Violation.cs`):
```csharp
public interface IWorker
{
    void Work();
    void Eat();
    void Sleep();
}

public class Robot : IWorker
{
    public void Work() { /* OK */ }
    public void Eat() { throw new NotSupportedException(); }  // FORCED!
    public void Sleep() { throw new NotSupportedException(); } // FORCED!
}
```

**Problems**:
- Robot forced to implement Eat() and Sleep()
- Must throw NotSupportedException (smell!)
- Interface suggests capabilities that don't exist
- Client code can't tell what's supported

**Correct** (`InterfaceSegregation/Correct.cs`):
```csharp
public interface IWorkable { void Work(); }
public interface IFeedable { void Eat(); }
public interface IRestable { void Sleep(); }

public class Human : IWorkable, IFeedable, IRestable { /* All methods */ }
public class Robot : IWorkable { /* Only Work() */ }
```

**Benefits**:
- No forced implementations
- Clear capability contracts
- Compiler prevents calling unsupported operations
- Easy to compose interfaces

### Common Mistakes

1. **Fat Interfaces**: One interface with too many methods
2. **One Interface Per Method**: Over-segregating (use judgment!)
3. **God Interfaces**: Interfaces that try to do everything
4. **Ignoring Cohesion**: Grouping unrelated methods

---

## 5. Dependency Inversion Principle (DIP)

> **Depend on abstractions, not concretions**

### Simple Explanation

High-level modules (business logic) should NOT depend on low-level modules (technical details). Both should depend on abstractions (interfaces).

### Memory Aid: "Abstractions, Not Concretions"

Think about a light switch:
- The switch doesn't care if it controls an LED, incandescent, or fluorescent bulb
- The switch depends on the ABSTRACTION: "something that can turn on/off"
- You can swap bulbs without changing the switch

Same with code: depend on WHAT (interface), not HOW (implementation)!

### When It Matters Most

- **Testing**: Easy to mock dependencies
- **Flexibility**: Easy to swap implementations
- **Decoupling**: Business logic independent of infrastructure
- **Runtime Composition**: Configure different implementations per environment

### Examples in This Project

**Violation** (`DependencyInversion/Violation.cs`):
```csharp
public class OrderServiceViolation
{
    // Direct dependencies on concrete classes
    private readonly EmailService _emailService = new();
    private readonly SmsService _smsService = new();

    public void PlaceOrder(decimal amount)
    {
        // Business logic...
        _emailService.SendEmail(/* ... */);  // TIGHTLY COUPLED!
        _smsService.SendSms(/* ... */);      // TIGHTLY COUPLED!
    }
}
```

**Problems**:
- Can't test without real email/SMS services
- Can't swap to different email provider
- Can't add new notification types without modifying class
- Business logic knows about infrastructure details

**Correct** (`DependencyInversion/Correct.cs`):
```csharp
// Abstraction
public interface INotificationService
{
    void SendNotification(string recipient, string message);
}

// Low-level implementations
public class EmailNotificationService : INotificationService { }
public class SmsNotificationService : INotificationService { }
public class PushNotificationService : INotificationService { }

// High-level module depends on abstraction
public class OrderService
{
    private readonly INotificationService _notificationService;

    public OrderService(INotificationService notificationService)
    {
        _notificationService = notificationService;  // INJECTED!
    }

    public void PlaceOrder(decimal amount)
    {
        // Business logic...
        _notificationService.SendNotification(/* ... */);
    }
}
```

**Benefits**:
- Easy to test (inject mocks)
- Easy to swap implementations (email → SMS → push)
- Business logic independent of technical details
- Can configure per environment (dev/test/prod)

### Common Mistakes

1. **No Abstractions**: Depending directly on concrete classes
2. **Service Locator**: Using service locator instead of injection
3. **New Keyword Everywhere**: Creating dependencies with `new`
4. **Leaky Abstractions**: Abstractions that expose implementation details

---

## Running the Examples

### Prerequisites

- .NET 8.0 or later
- C# 12 or later

### Build and Run

```bash
cd samples/03-Advanced/SOLIDPrinciples
dotnet build
dotnet run
```

### Interactive Menu

The application provides an interactive menu where you can:
1. Explore each SOLID principle individually
2. See violation examples (what NOT to do)
3. See correct implementations (what TO do)
4. Understand the problems and benefits
5. Run all principles in sequence

### Example Session

```
================================================================================
  SOLID PRINCIPLES TUTORIAL
================================================================================
  Learning the foundation of good object-oriented design

Main Menu:
1. Single Responsibility Principle (SRP)
2. Open/Closed Principle (OCP)
3. Liskov Substitution Principle (LSP)
4. Interface Segregation Principle (ISP)
5. Dependency Inversion Principle (DIP)
6. Run ALL principles demonstrations
0. Exit
```

---

## Project Structure

```
SOLIDPrinciples/
├── Program.cs                          # Interactive menu and demonstrations
├── README.md                           # This file
├── SingleResponsibility/
│   ├── Violation.cs                   # God object example
│   └── Correct.cs                     # Separated responsibilities
├── OpenClosed/
│   ├── Violation.cs                   # Modification required for new features
│   └── Correct.cs                     # Extension via abstraction
├── LiskovSubstitution/
│   ├── Violation.cs                   # Rectangle/Square problem
│   └── Correct.cs                     # Proper inheritance hierarchy
├── InterfaceSegregation/
│   ├── Violation.cs                   # Fat interfaces
│   └── Correct.cs                     # Segregated interfaces
└── DependencyInversion/
    ├── Violation.cs                   # Tight coupling to concrete classes
    └── Correct.cs                     # Dependency injection with interfaces
```

---

## When to Use SOLID

### Always Use (But Don't Over-Engineer!)

SOLID principles should guide your design decisions, but remember:

1. **Start Simple**: Don't create abstractions until you need them
2. **Refactor When Needed**: Apply SOLID when complexity grows
3. **Balance Pragmatism**: Sometimes a simple solution is better
4. **Context Matters**: SOLID for large systems, simpler approaches for scripts

### Signs You Need SOLID

- Classes are becoming too large
- Changes require modifying multiple unrelated classes
- Testing is difficult
- Code is hard to understand
- Adding features breaks existing functionality

### Signs You're Over-Engineering

- Interfaces with only one implementation (and no plans for more)
- Deeply nested abstractions
- More boilerplate than business logic
- Team members confused by the architecture

---

## Common Mistakes

### 1. Applying SOLID Without Understanding Why

**Bad**: Creating interfaces just because "that's what you're supposed to do"

**Good**: Understanding the problem each principle solves

### 2. Over-Abstraction

**Bad**:
```csharp
public interface IUserEmailAddressStringProvider
{
    string GetUserEmailAddressString();
}
```

**Good**:
```csharp
public interface IUserRepository
{
    User GetUser(int id);
}
```

### 3. Premature Optimization

**Bad**: Creating abstractions for code that might change

**Good**: Creating abstractions when you have concrete evidence of need

### 4. Ignoring Context

**Bad**: Applying all SOLID principles to a 50-line script

**Good**: Applying SOLID principles to complex, maintainable systems

### 5. Creating Interfaces for Everything

**Bad**: Every class has an interface, even simple DTOs

**Good**: Interfaces for dependencies, extensibility, and testing needs

---

## The Golden Rule

**SOLID principles help you write code that's easy to:**
- ✓ Understand
- ✓ Maintain
- ✓ Test
- ✓ Extend

**Remember**: These are guidelines, not laws. Use judgment, consider context, and always prioritize readable, maintainable code.

---

## Quick Reference Card

| Principle | Memory Aid | Key Question |
|-----------|-----------|--------------|
| **S**RP | "One Class, One Job" | Can I describe this class without using "AND"? |
| **O**CP | "Extend, Don't Edit" | Can I add features without modifying existing code? |
| **L**SP | "Substitution Without Surprise" | Can I use the derived class everywhere the base class is used? |
| **I**SP | "Many Small Menus" | Am I forced to implement methods I don't use? |
| **D**IP | "Abstractions, Not Concretions" | Do I depend on interfaces or concrete classes? |

---

## Additional Resources

### Books
- **Clean Code** by Robert C. Martin
- **Agile Software Development, Principles, Patterns, and Practices** by Robert C. Martin
- **Design Patterns** by Gang of Four

### Online
- [SOLID Principles Explained](https://stackify.com/solid-design-principles/)
- [Uncle Bob's Blog](http://blog.cleancoder.com/)
- [Pluralsight SOLID Courses](https://www.pluralsight.com/search?q=solid%20principles)

### Practice
- Refactor existing code to follow SOLID
- Review pull requests for SOLID violations
- Discuss SOLID with your team
- Use this sample project as a reference

---

## Summary

SOLID principles are fundamental to writing maintainable, flexible, and testable object-oriented code. They help you:

1. **SRP**: Keep classes focused and cohesive
2. **OCP**: Design for extension without modification
3. **LSP**: Use inheritance correctly and predictably
4. **ISP**: Create focused, role-based interfaces
5. **DIP**: Decouple high-level and low-level modules

Master these principles, and you'll write better code that stands the test of time!

---

**Happy Coding! 🚀**
