# Month 5 Comprehensive Assessment - Design Patterns & SOLID Principles

**Month**: 5 (Weeks 17-20) | **Duration**: 90 min | **Pass**: 80% (24/30) | **Points**: 30

## Section 1: Multiple Choice (15 questions, 0.5 pts each = 7.5 pts)

1. Decorator pattern primary purpose:
   - a) Create objects | b) Add behavior dynamically | c) Notify observers | d) Build complex objects

2. Decorator vs Inheritance difference:
   - a) Same thing | b) Decorator is runtime, inheritance is compile-time | c) Decorator is faster | d) No difference

3. Can you chain multiple decorators?
   - a) No | b) Yes, unlimited | c) Max 2 | d) Only if same type

4. SRP (Single Responsibility Principle) means:
   - a) One method per class | b) One reason to change | c) One property | d) One interface

5. What violates SRP?
   - a) Class doing logging and business logic | b) Class with multiple methods | c) Class with properties | d) Abstract class

6. OCP (Open/Closed Principle) means:
   - a) Public methods only | b) Open for extension, closed for modification | c) All properties public | d) No inheritance

7. Which enables OCP?
   - a) Sealed classes | b) Static methods | c) Polymorphism and abstractions | d) Private fields

8. LSP (Liskov Substitution Principle) means:
   - a) Prefer interfaces | b) Derived class substitutable for base | c) Use abstract classes | d) Avoid inheritance

9. Classic LSP violation:
   - a) Square inheriting Rectangle | b) Dog inheriting Animal | c) List<T> inheriting IEnumerable<T> | d) All inheritance

10. ISP (Interface Segregation Principle) means:
    - a) One interface only | b) No fat interfaces, many small specific ones | c) Interfaces must be public | d) Avoid interfaces

11. DIP (Dependency Inversion Principle) means:
    - a) Avoid dependencies | b) Depend on abstractions, not concretions | c) Use static classes | d) Private dependencies

12. What is Dependency Injection?
    - a) Design pattern | b) Technique to implement DIP | c) Language feature | d) Anti-pattern

13. Constructor injection vs property injection:
    - a) Same | b) Constructor for required, property for optional | c) Property is better | d) Constructor is obsolete

14. Decorator pattern implements which SOLID principle?
    - a) SRP and OCP | b) Only DIP | c) Only LSP | d) None

15. Service locator vs DI:
    - a) Same | b) DI preferred (explicit dependencies) | c) Service locator better | d) Use both

## Section 2: Short Answer (7 questions, 2 pts each = 14 pts)

16. Explain Decorator pattern. How does it differ from simply using inheritance to add behavior?

17. Give a real-world example of SRP violation and show how to fix it.

18. Explain OCP with code example. How do you extend without modifying?

19. Explain the Square/Rectangle problem as LSP violation. Why does Square break LSP?

20. What's a "fat interface"? Give example and show how to apply ISP.

21. Explain high-level vs low-level modules in DIP. Show violation and fix.

22. Compare these DI lifetimes: Transient, Scoped, Singleton. When to use each?

## Section 3: Code Implementation (4 questions, 2 pts each = 8 pts)

23. Implement Decorator pattern for a coffee shop:
```csharp
// Requirements:
// - ICoffee interface with Cost() and Description()
// - SimpleCoffee base implementation
// - MilkDecorator adds $0.50
// - SugarDecorator adds $0.25
// - Chain: new SugarDecorator(new MilkDecorator(new SimpleCoffee()))
```

24. Refactor this SRP violation:
```csharp
public class User
{
    public string Name { get; set; }
    public string Email { get; set; }

    public void SaveToDatabase()
    {
        // Database logic here
        Console.WriteLine($"Saving {Name} to database");
    }

    public void SendEmail(string message)
    {
        // Email logic here
        Console.WriteLine($"Sending email to {Email}: {message}");
    }
}
```

25. Implement OCP with payment processing:
```csharp
// Bad code violating OCP
public class PaymentProcessor
{
    public void ProcessPayment(string type, decimal amount)
    {
        if (type == "CreditCard")
            Console.WriteLine($"Processing credit card: ${amount}");
        else if (type == "PayPal")
            Console.WriteLine($"Processing PayPal: ${amount}");
        // Adding new payment type requires modifying this class!
    }
}

// Refactor to follow OCP
```

26. Apply DIP to this code:
```csharp
// Violates DIP - high-level depends on low-level
public class EmailService
{
    public void SendEmail(string to, string message)
    {
        Console.WriteLine($"Email to {to}: {message}");
    }
}

public class UserController
{
    private EmailService _emailService = new EmailService(); // Direct dependency!

    public void RegisterUser(string email)
    {
        Console.WriteLine($"Registering {email}");
        _emailService.SendEmail(email, "Welcome!");
    }
}

// Refactor to follow DIP with dependency injection
```

## Answer Key

**MC**: 1.b | 2.b | 3.b | 4.b | 5.a | 6.b | 7.c | 8.b | 9.a | 10.b | 11.b | 12.b | 13.b | 14.a | 15.b

### Short Answer

**16. Decorator Pattern** (2 pts):
- **Purpose**: Add responsibilities to objects dynamically without affecting other objects
- **How it works**: Wrap object in decorator that implements same interface
- **vs Inheritance**:
  - Inheritance: Static, compile-time, all instances affected
  - Decorator: Dynamic, runtime, individual objects affected
  - Inheritance: Limited (can't inherit multiple behaviors)
  - Decorator: Flexible (chain multiple decorators)
- **Example**:
```csharp
// Inheritance: Must create class for every combination
class CoffeeWithMilk, CoffeeWithSugar, CoffeeWithMilkAndSugar

// Decorator: Compose at runtime
new SugarDecorator(new MilkDecorator(new Coffee()))
```

**17. SRP Violation Example** (2 pts):
**Violation**:
```csharp
public class Invoice
{
    public decimal Amount { get; set; }

    public decimal CalculateTax() { } // Reason 1: Business logic
    public void SaveToDatabase() { }  // Reason 2: Persistence
    public void PrintInvoice() { }    // Reason 3: Presentation
    public void SendEmail() { }       // Reason 4: Notification
}
// Has 4 reasons to change!
```

**Fixed**:
```csharp
public class Invoice
{
    public decimal Amount { get; set; }
    public decimal CalculateTax() { } // Only business logic
}

public class InvoiceRepository
{
    public void Save(Invoice invoice) { } // Only persistence
}

public class InvoicePrinter
{
    public void Print(Invoice invoice) { } // Only presentation
}

public class InvoiceEmailer
{
    public void Send(Invoice invoice) { } // Only notification
}
```
Each class has single responsibility!

**18. OCP Example** (2 pts):
**Closed for modification**: Don't change existing code
**Open for extension**: Add new functionality via inheritance/composition

**Example**:
```csharp
// ❌ Violates OCP
public class AreaCalculator
{
    public double Calculate(object shape)
    {
        if (shape is Circle c)
            return Math.PI * c.Radius * c.Radius;
        else if (shape is Rectangle r)
            return r.Width * r.Height;
        // Adding triangle requires modifying this method!
    }
}

// ✅ Follows OCP
public abstract class Shape
{
    public abstract double CalculateArea();
}

public class Circle : Shape
{
    public double Radius { get; set; }
    public override double CalculateArea()
        => Math.PI * Radius * Radius;
}

public class Rectangle : Shape
{
    public double Width { get; set; }
    public double Height { get; set; }
    public override double CalculateArea()
        => Width * Height;
}

// Add triangle WITHOUT modifying existing code
public class Triangle : Shape
{
    public double Base { get; set; }
    public double Height { get; set; }
    public override double CalculateArea()
        => 0.5 * Base * Height;
}
```

**19. Square/Rectangle LSP Violation** (2 pts):
**The Problem**:
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
        set { base.Width = base.Height = value; }
    }
    public override int Height
    {
        set { base.Width = base.Height = value; }
    }
}

// LSP violation demonstration
Rectangle rect = new Square();
rect.Width = 5;
rect.Height = 10;
// Expected: Area = 50 (5 * 10)
// Actual: Area = 100 (10 * 10) - Square overrode behavior!
```

**Why it breaks LSP**:
- Rectangle promises independent width/height
- Square violates this (width = height always)
- Code using Rectangle breaks when given Square
- Substitution fails!

**Solution**: Don't make Square inherit Rectangle. Use composition or separate hierarchy.

**20. Fat Interface & ISP** (2 pts):
**Fat Interface** (too many unrelated methods):
```csharp
// ❌ Fat interface - forces implementation of unused methods
public interface IWorker
{
    void Work();
    void Eat();
    void Sleep();
    void GetPaid();
}

public class Robot : IWorker
{
    public void Work() { /* OK */ }
    public void Eat() { throw new NotImplementedException(); } // Robot doesn't eat!
    public void Sleep() { throw new NotImplementedException(); } // Robot doesn't sleep!
    public void GetPaid() { throw new NotImplementedException(); } // Robot doesn't get paid!
}
```

**Apply ISP** (segregate into specific interfaces):
```csharp
// ✅ Segregated interfaces
public interface IWorkable
{
    void Work();
}

public interface IFeedable
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

public class Human : IWorkable, IFeedable, ISleepable, IPayable
{
    public void Work() { }
    public void Eat() { }
    public void Sleep() { }
    public void GetPaid() { }
}

public class Robot : IWorkable // Only implements what it needs!
{
    public void Work() { }
}
```

**21. DIP High-Level vs Low-Level** (2 pts):
**High-level modules**: Business logic, use cases (what to do)
**Low-level modules**: Infrastructure, details (how to do)

**Violation** (high-level depends on low-level):
```csharp
// Low-level
public class MySqlDatabase
{
    public void Save(string data) { }
}

// High-level depends on low-level!
public class OrderService
{
    private MySqlDatabase _db = new MySqlDatabase(); // Tight coupling!

    public void CreateOrder(Order order)
    {
        // Business logic
        _db.Save(order.ToString());
    }
}
// Problem: Can't switch to PostgreSQL without changing OrderService
```

**Fixed with DIP** (both depend on abstraction):
```csharp
// Abstraction
public interface IDatabase
{
    void Save(string data);
}

// Low-level implements abstraction
public class MySqlDatabase : IDatabase
{
    public void Save(string data) { /* MySQL implementation */ }
}

public class PostgresDatabase : IDatabase
{
    public void Save(string data) { /* Postgres implementation */ }
}

// High-level depends on abstraction
public class OrderService
{
    private readonly IDatabase _db;

    public OrderService(IDatabase db) // Dependency injection!
    {
        _db = db;
    }

    public void CreateOrder(Order order)
    {
        _db.Save(order.ToString());
    }
}

// Usage - can switch database without changing OrderService
var service1 = new OrderService(new MySqlDatabase());
var service2 = new OrderService(new PostgresDatabase());
```

**22. DI Lifetimes** (2 pts):
**Transient**:
- New instance every time requested
- Use for: Lightweight stateless services
- Example: Data validators, calculators
- `services.AddTransient<IMyService, MyService>()`

**Scoped**:
- One instance per request/scope
- Use for: Per-request state (DB context, request context)
- Example: DbContext in ASP.NET (one per HTTP request)
- `services.AddScoped<IMyService, MyService>()`

**Singleton**:
- Single instance for application lifetime
- Use for: Expensive to create, thread-safe, stateless
- Example: Caches, loggers, configuration
- `services.AddSingleton<IMyService, MyService>()`

**Comparison**:
```
Request 1: [Transient1, Transient2] [Scoped1      ] [Singleton]
Request 2: [Transient3, Transient4] [Scoped2      ] [Singleton]
Request 3: [Transient5, Transient6] [Scoped3      ] [Singleton]
```

### Code Implementation

**23. Coffee Decorator** (2 pts):
```csharp
public interface ICoffee
{
    decimal Cost();
    string Description();
}

public class SimpleCoffee : ICoffee
{
    public decimal Cost() => 2.00m;
    public string Description() => "Simple coffee";
}

public abstract class CoffeeDecorator : ICoffee
{
    protected readonly ICoffee _coffee;

    protected CoffeeDecorator(ICoffee coffee)
    {
        _coffee = coffee;
    }

    public virtual decimal Cost() => _coffee.Cost();
    public virtual string Description() => _coffee.Description();
}

public class MilkDecorator : CoffeeDecorator
{
    public MilkDecorator(ICoffee coffee) : base(coffee) { }

    public override decimal Cost() => _coffee.Cost() + 0.50m;
    public override string Description() => _coffee.Description() + ", Milk";
}

public class SugarDecorator : CoffeeDecorator
{
    public SugarDecorator(ICoffee coffee) : base(coffee) { }

    public override decimal Cost() => _coffee.Cost() + 0.25m;
    public override string Description() => _coffee.Description() + ", Sugar";
}

// Usage
ICoffee coffee = new SimpleCoffee();
Console.WriteLine($"{coffee.Description()}: ${coffee.Cost()}");
// Output: Simple coffee: $2.00

coffee = new MilkDecorator(coffee);
Console.WriteLine($"{coffee.Description()}: ${coffee.Cost()}");
// Output: Simple coffee, Milk: $2.50

coffee = new SugarDecorator(coffee);
Console.WriteLine($"{coffee.Description()}: ${coffee.Cost()}");
// Output: Simple coffee, Milk, Sugar: $2.75
```

**24. SRP Refactoring** (2 pts):
```csharp
// ✅ Refactored - each class has single responsibility

public class User
{
    public string Name { get; set; }
    public string Email { get; set; }
    // Only properties - data representation
}

public class UserRepository
{
    public void Save(User user)
    {
        // Database logic only
        Console.WriteLine($"Saving {user.Name} to database");
    }
}

public class EmailService
{
    public void SendEmail(string to, string message)
    {
        // Email logic only
        Console.WriteLine($"Sending email to {to}: {message}");
    }
}

public class UserService
{
    private readonly UserRepository _repository;
    private readonly EmailService _emailService;

    public UserService(UserRepository repository, EmailService emailService)
    {
        _repository = repository;
        _emailService = emailService;
    }

    public void RegisterUser(User user)
    {
        _repository.Save(user);
        _emailService.SendEmail(user.Email, "Welcome!");
    }
}

// Usage
var user = new User { Name = "John", Email = "john@example.com" };
var repository = new UserRepository();
var emailService = new EmailService();
var userService = new UserService(repository, emailService);
userService.RegisterUser(user);
```

**25. OCP Payment Processing** (2 pts):
```csharp
// ✅ Follows OCP - open for extension, closed for modification

public interface IPaymentMethod
{
    void ProcessPayment(decimal amount);
}

public class CreditCardPayment : IPaymentMethod
{
    public void ProcessPayment(decimal amount)
    {
        Console.WriteLine($"Processing credit card: ${amount}");
    }
}

public class PayPalPayment : IPaymentMethod
{
    public void ProcessPayment(decimal amount)
    {
        Console.WriteLine($"Processing PayPal: ${amount}");
    }
}

// Add new payment type WITHOUT modifying existing code
public class CryptoPayment : IPaymentMethod
{
    public void ProcessPayment(decimal amount)
    {
        Console.WriteLine($"Processing cryptocurrency: ${amount}");
    }
}

public class PaymentProcessor
{
    public void ProcessPayment(IPaymentMethod paymentMethod, decimal amount)
    {
        paymentMethod.ProcessPayment(amount);
    }
}

// Usage
var processor = new PaymentProcessor();
processor.ProcessPayment(new CreditCardPayment(), 100);
processor.ProcessPayment(new PayPalPayment(), 200);
processor.ProcessPayment(new CryptoPayment(), 300); // New type added!
```

**26. DIP Refactoring** (2 pts):
```csharp
// ✅ Follows DIP - depend on abstraction

// Abstraction
public interface INotificationService
{
    void Send(string to, string message);
}

// Low-level implementations
public class EmailService : INotificationService
{
    public void Send(string to, string message)
    {
        Console.WriteLine($"Email to {to}: {message}");
    }
}

public class SmsService : INotificationService
{
    public void Send(string to, string message)
    {
        Console.WriteLine($"SMS to {to}: {message}");
    }
}

// High-level depends on abstraction
public class UserController
{
    private readonly INotificationService _notificationService;

    // Dependency injection via constructor
    public UserController(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    public void RegisterUser(string contact)
    {
        Console.WriteLine($"Registering {contact}");
        _notificationService.Send(contact, "Welcome!");
    }
}

// Usage - can switch implementation easily
var emailController = new UserController(new EmailService());
emailController.RegisterUser("user@example.com");

var smsController = new UserController(new SmsService());
smsController.RegisterUser("+1234567890");
```

## Grading Rubric

| Section | Max Points | Criteria |
|---------|-----------|----------|
| Multiple Choice | 7.5 | 0.5 per correct answer |
| Short Answer (each) | 2 × 7 = 14 | Full: Complete + code. Partial: 1.0-1.5. Wrong: 0 |
| Code Implementation (each) | 2 × 4 = 8 | Full: Correct refactoring. Partial: 1.0-1.5. Wrong: 0 |
| **Total** | **30** | **Pass: 24 points (80%)** |

---

## SOLID Principles Quick Reference

| Principle | Acronym | Definition | Key Benefit |
|-----------|---------|------------|-------------|
| **Single Responsibility** | SRP | One class, one reason to change | Maintainability |
| **Open/Closed** | OCP | Open for extension, closed for modification | Extensibility |
| **Liskov Substitution** | LSP | Derived types substitutable for base | Correctness |
| **Interface Segregation** | ISP | Many specific interfaces > one fat interface | Flexibility |
| **Dependency Inversion** | DIP | Depend on abstractions, not concretions | Testability |

---

## Study Resources

**Week 17 - Decorator Pattern**:
- `samples/99-Exercises/DesignPatterns/03-Decorator/`
- Theory: Dynamic behavior addition

**Week 18 - SRP & OCP**:
- `src/AdvancedConcepts.Core/Advanced/SOLIDPrinciples/`
- Refactoring exercises

**Week 19 - LSP & ISP**:
- Square/Rectangle problem analysis
- Interface segregation examples

**Week 20 - DIP**:
- Dependency injection patterns
- ASP.NET Core DI container

---

## Next Steps

**If you passed (≥24 pts)**: Proceed to Month 6 (Capstone Project Preparation)

**If you didn't pass (<24 pts)**: Review weak areas:
- Score 0-10: Review all SOLID principles
- Score 11-18: Focus on code refactoring
- Score 19-23: Practice design patterns

---

*Assessment Version: 1.0*
*Last Updated: 2025-12-02*
