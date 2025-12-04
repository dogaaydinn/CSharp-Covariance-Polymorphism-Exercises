# Why Learn SOLID Principles?

## The Problem: Unmaintainable Spaghetti Code

Your codebase has become a nightmare:

```csharp
public class OrderService
{
    // Violates SRP - Does EVERYTHING!
    public void ProcessOrder(Order order)
    {
        // Validate order
        if (string.IsNullOrEmpty(order.CustomerEmail))
            throw new Exception("Invalid email");

        // Calculate price with discount logic
        decimal total = order.Items.Sum(x => x.Price * x.Quantity);
        if (order.CustomerType == "Premium")
            total *= 0.9m;
        else if (order.CustomerType == "Gold")
            total *= 0.85m;

        // Send email notification (tight coupling to EmailService!)
        var emailService = new EmailService();
        emailService.Send(order.CustomerEmail, $"Order total: {total}");

        // Save to database (hardcoded to SQL Server!)
        var connection = new SqlConnection("...");
        connection.Open();
        // ... raw SQL commands ...

        // Log to file (hardcoded to file system!)
        File.AppendAllText("orders.log", $"Order {order.Id} processed");
    }
}
```

**Problems:**
- ❌ **Impossible to test** - Hardcoded dependencies (EmailService, SqlConnection, File)
- ❌ **Impossible to extend** - Adding new customer types requires modifying this class
- ❌ **Impossible to reuse** - Validation, pricing, notification logic all mixed together
- ❌ **Fragile** - One change breaks everything
- ❌ **Violates all 5 SOLID principles!**

---

## The Solution: SOLID Principles

SOLID is an acronym for five principles that make code maintainable:

1. **S**ingle Responsibility Principle
2. **O**pen/Closed Principle
3. **L**iskov Substitution Principle
4. **I**nterface Segregation Principle
5. **D**ependency Inversion Principle

---

## S - Single Responsibility Principle (SRP)

> **"A class should have one, and only one, reason to change."**

### Violation Example

```csharp
// BAD: This class has THREE reasons to change!
public class Employee
{
    // Reason 1: Business logic changes
    public decimal CalculatePay() { /* payroll logic */ }

    // Reason 2: Report format changes
    public string GenerateReport() { /* report generation */ }

    // Reason 3: Database schema changes
    public void SaveToDatabase() { /* persistence logic */ }
}
```

**Problems:**
- Changing payroll logic might break reports
- Changing report format might break database save
- Changing database schema might break payroll
- **Fragile!** Changes ripple through unrelated concerns

### Correct Example

```csharp
// GOOD: One responsibility per class
public class Employee
{
    public string Name { get; set; }
    public decimal Salary { get; set; }
    // Just data, no behavior
}

// Separate classes for separate concerns
public class PayrollCalculator
{
    public decimal CalculatePay(Employee employee) { /* payroll logic */ }
}

public class EmployeeReportGenerator
{
    public string GenerateReport(Employee employee) { /* report generation */ }
}

public class EmployeeRepository
{
    public void Save(Employee employee) { /* persistence logic */ }
}
```

**Benefits:**
- ✅ Each class has one reason to change
- ✅ Changes isolated to single class
- ✅ Easy to test each concern separately
- ✅ Classes can be reused independently

### When You Know You're Violating SRP

- Class name has "and" in it (OrderAndPaymentProcessor)
- Class has methods for unrelated concerns
- Methods access different sets of fields
- Hard to name the class clearly

**Real Example in Repo:**
- Location: `samples/03-Advanced/SOLIDPrinciples/SingleResponsibility/`
- See both Violation.cs and Correct.cs

---

## O - Open/Closed Principle (OCP)

> **"Software entities should be open for extension, but closed for modification."**

### Violation Example

```csharp
// BAD: Must modify class to add new shape!
public class AreaCalculator
{
    public double CalculateArea(object shape)
    {
        if (shape is Circle circle)
            return Math.PI * circle.Radius * circle.Radius;
        else if (shape is Rectangle rect)
            return rect.Width * rect.Height;
        else if (shape is Triangle tri) // Adding triangle requires modifying this class!
            return 0.5 * tri.Base * tri.Height;
        else
            throw new NotSupportedException();
    }
}
```

**Problems:**
- Adding new shapes requires modifying AreaCalculator
- Risk of breaking existing functionality
- Violates "closed for modification"

### Correct Example

```csharp
// GOOD: Add new shapes without modifying existing code!
public interface IShape
{
    double CalculateArea();
}

public class Circle : IShape
{
    public double Radius { get; set; }
    public double CalculateArea() => Math.PI * Radius * Radius;
}

public class Rectangle : IShape
{
    public double Width { get; set; }
    public double Height { get; set; }
    public double CalculateArea() => Width * Height;
}

// Add new shape - NO modification to existing code!
public class Triangle : IShape
{
    public double Base { get; set; }
    public double Height { get; set; }
    public double CalculateArea() => 0.5 * Base * Height;
}

// Calculator doesn't need to change!
public class AreaCalculator
{
    public double CalculateArea(IShape shape) => shape.CalculateArea();
}
```

**Benefits:**
- ✅ Add new shapes without changing existing code
- ✅ Existing code stays stable
- ✅ Reduced risk of bugs
- ✅ Polymorphism enables extension

### How to Achieve OCP

1. **Use interfaces/abstract classes**
2. **Rely on abstractions, not concrete types**
3. **Use design patterns:** Strategy, Factory, Template Method

**Real Example in Repo:**
- Location: `samples/03-Advanced/SOLIDPrinciples/OpenClosed/`
- See both Violation.cs and Correct.cs

---

## L - Liskov Substitution Principle (LSP)

> **"Derived classes must be substitutable for their base classes."**

### Violation Example

```csharp
// BAD: Derived class breaks base class contract!
public class Bird
{
    public virtual void Fly() { /* flying logic */ }
}

public class Penguin : Bird
{
    public override void Fly()
    {
        throw new NotSupportedException("Penguins can't fly!"); // ❌ Violates LSP!
    }
}

// Usage breaks!
void MakeBirdFly(Bird bird)
{
    bird.Fly(); // ��� Throws exception if bird is Penguin!
}
```

**Problems:**
- Penguin breaks the contract that all Birds can fly
- Clients must check runtime type before calling Fly()
- Violates polymorphism expectations

### Correct Example

```csharp
// GOOD: Proper abstraction
public abstract class Bird
{
    public abstract void Move();
}

public class FlyingBird : Bird
{
    public override void Move()
    {
        Fly();
    }

    protected virtual void Fly() { /* flying logic */ }
}

public class Penguin : Bird
{
    public override void Move()
    {
        Swim(); // Penguins swim instead of fly!
    }

    private void Swim() { /* swimming logic */ }
}

// Usage works for all birds!
void MakeBirdMove(Bird bird)
{
    bird.Move(); // ✅ Works for all birds, no exceptions!
}
```

**Benefits:**
- ✅ All derived classes honor base class contract
- ✅ Polymorphism works correctly
- ✅ No runtime surprises
- ✅ Clients don't need type checks

### LSP Guidelines

1. **Derived class should not:**
   - Throw new exceptions not thrown by base
   - Weaken preconditions
   - Strengthen postconditions

2. **Derived class should:**
   - Accept all inputs base class accepts
   - Produce all outputs base class promises
   - Maintain invariants

**Real Example in Repo:**
- Location: `samples/03-Advanced/SOLIDPrinciples/LiskovSubstitution/`
- See Rectangle/Square problem demonstration

---

## I - Interface Segregation Principle (ISP)

> **"Clients should not be forced to depend on interfaces they don't use."**

### Violation Example

```csharp
// BAD: Fat interface forces implementations to implement unused methods!
public interface IWorker
{
    void Work();
    void Eat();
    void Sleep();
    void GetPaid();
}

public class Robot : IWorker
{
    public void Work() { /* work logic */ }
    public void Eat() { throw new NotSupportedException(); } // ❌ Robots don't eat!
    public void Sleep() { throw new NotSupportedException(); } // ❌ Robots don't sleep!
    public void GetPaid() { throw new NotSupportedException(); } // ❌ Robots don't get paid!
}
```

**Problems:**
- Robot forced to implement methods it doesn't need
- Violates LSP (throws NotSupportedException)
- Fat interfaces hard to implement

### Correct Example

```csharp
// GOOD: Segregate interfaces by client needs
public interface IWorkable
{
    void Work();
}

public interface IEatable
{
    void Eat();
}

public interface ISleepable
{
    void Sleep();
}

public interface IPayable
{
    void GetPaid();
}

// Human implements all interfaces
public class Human : IWorkable, IEatable, ISleepable, IPayable
{
    public void Work() { /* work logic */ }
    public void Eat() { /* eat logic */ }
    public void Sleep() { /* sleep logic */ }
    public void GetPaid() { /* payment logic */ }
}

// Robot only implements what it needs
public class Robot : IWorkable
{
    public void Work() { /* work logic */ }
}
```

**Benefits:**
- ✅ Classes implement only what they need
- ✅ Smaller, focused interfaces
- ✅ Easier to implement and test
- ✅ Honors LSP

### How to Apply ISP

1. **Create small, focused interfaces**
2. **Use interface composition** (multiple interfaces per class)
3. **Role-based interfaces** (IReadable, IWritable vs IFile)

**Real Example in Repo:**
- Location: `samples/03-Advanced/SOLIDPrinciples/InterfaceSegregation/`
- See printer example with segregated interfaces

---

## D - Dependency Inversion Principle (DIP)

> **"Depend on abstractions, not concretions."**

### Violation Example

```csharp
// BAD: High-level module depends on low-level module!
public class OrderService
{
    private readonly SqlServerRepository _repository; // Concrete type!
    private readonly EmailService _emailService;       // Concrete type!

    public OrderService()
    {
        _repository = new SqlServerRepository(); // Hard to test!
        _emailService = new EmailService();       // Hard to test!
    }

    public void ProcessOrder(Order order)
    {
        _repository.Save(order);
        _emailService.Send(order.CustomerEmail, "Order confirmed");
    }
}
```

**Problems:**
- OrderService coupled to SQL Server (can't switch to NoSQL)
- OrderService coupled to EmailService (can't switch to SMS)
- Impossible to unit test (can't mock dependencies)
- Changes to low-level modules force changes to high-level

### Correct Example

```csharp
// GOOD: Depend on abstractions!
public interface IRepository
{
    void Save(Order order);
}

public interface INotificationService
{
    void Send(string recipient, string message);
}

public class OrderService
{
    private readonly IRepository _repository;
    private readonly INotificationService _notificationService;

    // Dependencies injected via constructor
    public OrderService(IRepository repository, INotificationService notificationService)
    {
        _repository = repository;
        _notificationService = notificationService;
    }

    public void ProcessOrder(Order order)
    {
        _repository.Save(order);
        _notificationService.Send(order.CustomerEmail, "Order confirmed");
    }
}

// Easy to test with mocks!
var mockRepo = new Mock<IRepository>();
var mockNotification = new Mock<INotificationService>();
var service = new OrderService(mockRepo.Object, mockNotification.Object);
```

**Benefits:**
- ✅ High-level and low-level modules decoupled
- ✅ Easy to test (inject mocks)
- ✅ Easy to swap implementations
- ✅ Follows "Depend on abstractions" principle

### DIP Implementation Patterns

1. **Constructor Injection** (most common)
2. **Property Injection**
3. **Method Injection**
4. **Use DI Containers** (Microsoft.Extensions.DependencyInjection)

**Real Example in Repo:**
- Location: `samples/03-Advanced/SOLIDPrinciples/DependencyInversion/`
- Location: `samples/05-RealWorld/MicroserviceTemplate/` - Full DI example

---

## How SOLID Principles Work Together

```csharp
// All 5 principles in action!

// SRP: Each class has one responsibility
// ISP: Small, focused interfaces
public interface IOrderValidator { bool Validate(Order order); }
public interface IPricingCalculator { decimal Calculate(Order order); }
public interface INotificationService { void Notify(string message); }

// DIP: High-level depends on abstractions
public class OrderService
{
    private readonly IOrderValidator _validator;
    private readonly IPricingCalculator _calculator;
    private readonly INotificationService _notificationService;

    public OrderService(
        IOrderValidator validator,
        IPricingCalculator calculator,
        INotificationService notificationService)
    {
        _validator = validator;
        _calculator = calculator;
        _notificationService = notificationService;
    }

    public void ProcessOrder(Order order)
    {
        if (!_validator.Validate(order))
            throw new InvalidOperationException("Invalid order");

        var total = _calculator.Calculate(order);
        _notificationService.Notify($"Order total: {total}");
    }
}

// OCP: Add new pricing strategies without modifying existing code
public class StandardPricingCalculator : IPricingCalculator
{
    public decimal Calculate(Order order) => order.Total;
}

public class PremiumPricingCalculator : IPricingCalculator
{
    public decimal Calculate(Order order) => order.Total * 0.9m; // 10% discount
}

// LSP: All implementations honor their contracts
// (No throwing NotSupportedException!)
```

---

## When to Apply SOLID

### ✅ Apply When:

1. **Building large applications**
2. **Code will be maintained for years**
3. **Multiple developers on team**
4. **Requirements change frequently**
5. **Need high testability**

### ⚠️ Don't Over-Apply When:

1. **Prototypes** - SOLID adds complexity
2. **Scripts** - SOLID overkill for 100-line scripts
3. **Simple CRUD apps** - Repository + service layer enough
4. **Stable requirements** - If it won't change, SOLID less valuable

---

## Common Violations

### God Class (Violates SRP)
```csharp
public class ApplicationManager { /* 5000 lines, does everything */ }
```

### if/else Chains (Violates OCP)
```csharp
if (type == "A") { /* ... */ }
else if (type == "B") { /* ... */ }
// Adding "C" requires modifying this code
```

### Rectangle-Square Problem (Violates LSP)
```csharp
class Square : Rectangle { /* SetWidth also sets Height - breaks LSP */ }
```

### Fat Interfaces (Violates ISP)
```csharp
interface IRepository { /* 30 methods, most implementations don't need all */ }
```

### new Keyword Everywhere (Violates DIP)
```csharp
var repo = new SqlRepository(); // Hardcoded dependency
```

---

## SOLID Benefits Summary

| Principle | Primary Benefit | Key Technique |
|-----------|----------------|---------------|
| **SRP** | Maintainability | One class, one job |
| **OCP** | Extensibility | Interfaces + polymorphism |
| **LSP** | Correctness | Proper inheritance |
| **ISP** | Flexibility | Small, focused interfaces |
| **DIP** | Testability | Dependency injection |

---

## Key Takeaways

1. **SRP**: One class, one responsibility, one reason to change
2. **OCP**: Extend via new classes, not modifying existing
3. **LSP**: Derived classes must honor base class contracts
4. **ISP**: Many small interfaces > one big interface
5. **DIP**: Depend on abstractions (interfaces), not concrete types

**All 5 principles aim for:** Maintainable, testable, flexible code.

---

## Learning Path

1. **Start**: Understand SRP (`SingleResponsibility/`)
2. **Next**: Learn OCP (`OpenClosed/`)
3. **Then**: Master LSP (`LiskovSubstitution/`)
4. **Continue**: Apply ISP (`InterfaceSegregation/`)
5. **Advanced**: Implement DIP (`DependencyInversion/`)
6. **Real-World**: Study `samples/05-RealWorld/MicroserviceTemplate/` - All principles applied

---

## Further Reading

- **In This Repo**:
  - `samples/03-Advanced/DesignPatterns/` - Patterns implement SOLID
  - `samples/05-RealWorld/MicroserviceTemplate/` - Clean Architecture with SOLID

- **External**:
  - "Clean Code" by Robert C. Martin (Uncle Bob)
  - "Agile Software Development" by Robert C. Martin (original SOLID book)
  - "Design Patterns" by Gang of Four

---

**Next Step**: Review all 5 folders (`SingleResponsibility/`, `OpenClosed/`, etc.) and compare Violation.cs vs Correct.cs for each principle.
