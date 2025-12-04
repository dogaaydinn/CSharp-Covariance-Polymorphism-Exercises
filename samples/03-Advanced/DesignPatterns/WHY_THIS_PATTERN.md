# Why Learn Design Patterns?

## The Problem: Reinventing the Wheel (Badly)

Without knowing design patterns, you'll write code like this:

```csharp
// Creating objects everywhere with 'new'
public class OrderService
{
    public void ProcessOrder(int orderId)
    {
        var emailSender = new EmailSender(); // Tight coupling!
        var smsSender = new SmsSender();     // Hard to test!
        var logger = new FileLogger();       // Can't swap implementations!

        // Complex object creation logic scattered everywhere
        var order = new Order();
        order.Id = orderId;
        order.Status = "Pending";
        order.CreatedDate = DateTime.Now;
        // 20 more lines of setup...
    }
}
```

**Problems:**
- ❌ **Tight coupling** - Can't change implementations
- ❌ **Hard to test** - Can't mock dependencies
- ❌ **Duplicated logic** - Object creation repeated everywhere
- ❌ **Violates SOLID** - Single Responsibility, Open/Closed, Dependency Inversion all violated

---

## The Solution: Design Patterns

Design patterns are **proven solutions** to common problems. They're not code you copy-paste, but **thinking patterns** you apply.

### Pattern Categories

1. **Creational Patterns** - Object creation
   - Factory, Builder, Singleton, Prototype

2. **Structural Patterns** - Object composition
   - Adapter, Decorator, Proxy, Facade

3. **Behavioral Patterns** - Object collaboration
   - Strategy, Observer, Command, Chain of Responsibility

---

## Factory Pattern: "Let Someone Else Create Objects"

### The Problem
```csharp
// Object creation logic scattered everywhere
public void ProcessPayment(string paymentType)
{
    IPaymentProcessor processor;

    if (paymentType == "CreditCard")
    {
        processor = new CreditCardProcessor();
        ((CreditCardProcessor)processor).CardNumber = "...";
        ((CreditCardProcessor)processor).CVV = "...";
    }
    else if (paymentType == "PayPal")
    {
        processor = new PayPalProcessor();
        ((PayPalProcessor)processor).Email = "...";
    }
    // ... repeated in every method that needs payment processing
}
```

### The Solution
```csharp
// Factory handles creation
public void ProcessPayment(string paymentType)
{
    IPaymentProcessor processor = PaymentFactory.Create(paymentType);
    processor.Process(amount);
}

// Creation logic in ONE place
public static class PaymentFactory
{
    public static IPaymentProcessor Create(string type)
    {
        return type switch
        {
            "CreditCard" => new CreditCardProcessor(),
            "PayPal" => new PayPalProcessor(),
            "Crypto" => new CryptoProcessor(),
            _ => throw new ArgumentException($"Unknown payment type: {type}")
        };
    }
}
```

**Benefits:**
- ✅ Creation logic in one place
- ✅ Easy to add new types
- ✅ Clients don't know about concrete types
- ✅ Testable (can inject mock factory)

**When to Use:**
- Multiple related types need creation
- Creation logic is complex
- Want to hide concrete types from clients

**Alternatives:**
- Dependency Injection container (more flexible)
- Abstract Factory (multiple product families)

**Real-World Example in Repo:**
- Location: `samples/03-Advanced/DesignPatterns/Creational/FactoryPattern.cs`
- Usage: `samples/08-Capstone/MicroVideoPlatform/` - Video processor factory

---

## Builder Pattern: "Step-by-Step Construction"

### The Problem
```csharp
// Telescoping constructor anti-pattern
public class Pizza
{
    public Pizza(string size, bool cheese, bool pepperoni, bool mushrooms, 
                 bool onions, bool bacon, bool pineapple, bool extraSauce)
    {
        // Which parameter is which?!
    }
}

// Usage - nightmare to read
var pizza = new Pizza("Large", true, true, false, true, false, true, false);
```

### The Solution
```csharp
// Fluent builder
var pizza = new PizzaBuilder()
    .WithSize("Large")
    .WithCheese()
    .WithPepperoni()
    .WithOnions()
    .WithPineapple()
    .WithExtraSauce()
    .Build();
```

**Benefits:**
- ✅ Readable, self-documenting code
- ✅ Optional parameters without overloads
- ✅ Validation before build
- ✅ Immutable result objects

**When to Use:**
- Objects with many optional parameters
- Complex construction steps
- Want immutable objects
- Need validation before creation

**Alternatives:**
- Object initializer syntax (simpler but less control)
- Constructor with optional parameters (max 3-4 params)

**Real-World Example in Repo:**
- Location: `samples/03-Advanced/DesignPatterns/Creational/BuilderPattern.cs`
- Usage: `samples/99-Exercises/DesignPatterns/01-Builder/` - Practice exercises

---

## Singleton Pattern: "One and Only One Instance"

### The Problem
```csharp
// Multiple instances of something that should be single
var logger1 = new Logger();
var logger2 = new Logger(); // Oops, two loggers writing to same file!

// Config loaded multiple times
var config1 = new Configuration(); // Reads from file
var config2 = new Configuration(); // Reads again! Wasteful!
```

### The Solution
```csharp
public sealed class Logger
{
    private static readonly Lazy<Logger> _instance = 
        new Lazy<Logger>(() => new Logger());

    public static Logger Instance => _instance.Value;

    private Logger() { } // Private constructor

    public void Log(string message) { /* ... */ }
}

// Usage
Logger.Instance.Log("Application started");
```

**Benefits:**
- ✅ Guaranteed single instance
- ✅ Global access point
- ✅ Lazy initialization
- ✅ Thread-safe with Lazy<T>

**Trade-offs:**
- ⚠️ Global state (can make testing harder)
- ⚠️ Violates Single Responsibility (manages own lifecycle)
- ⚠️ Hidden dependencies (not obvious in constructor)

**When to Use:**
- Resource-intensive objects (database connections, loggers)
- Configuration that's loaded once
- Shared resources (thread pools, caches)

**Alternatives:**
- Dependency Injection with singleton lifetime (better!)
- Static class (if no state needed)

**Modern Approach:**
```csharp
// Better: Use DI container
services.AddSingleton<ILogger, Logger>();
```

---

## Strategy Pattern: "Swap Algorithms at Runtime"

### The Problem
```csharp
// Algorithms hardcoded in class
public class ShoppingCart
{
    public decimal CalculateTotal(string discountType)
    {
        if (discountType == "Percentage")
        {
            // Percentage discount logic
        }
        else if (discountType == "FixedAmount")
        {
            // Fixed discount logic
        }
        else if (discountType == "BuyOneGetOne")
        {
            // BOGO logic
        }
        // Adding new discount type requires modifying this class!
    }
}
```

### The Solution
```csharp
// Strategy interface
public interface IDiscountStrategy
{
    decimal ApplyDiscount(decimal total);
}

// Concrete strategies
public class PercentageDiscount : IDiscountStrategy
{
    public decimal ApplyDiscount(decimal total) => total * 0.9m;
}

// Context uses strategy
public class ShoppingCart
{
    private readonly IDiscountStrategy _discountStrategy;

    public ShoppingCart(IDiscountStrategy discountStrategy)
    {
        _discountStrategy = discountStrategy;
    }

    public decimal CalculateTotal() => _discountStrategy.ApplyDiscount(subtotal);
}
```

**Benefits:**
- ✅ Open/Closed Principle (open for extension, closed for modification)
- ✅ Easy to add new strategies
- ✅ Strategies can be swapped at runtime
- ✅ Easy to test each strategy independently

**When to Use:**
- Multiple algorithms for same operation
- Want to swap behavior at runtime
- Avoid long if/else chains

**Alternatives:**
- Inheritance (less flexible, couples strategy to context)
- Delegates/lambdas (for simple strategies)

**Real-World Example in Repo:**
- Location: `samples/03-Advanced/DesignPatterns/Behavioral/StrategyPattern.cs`
- Usage: `samples/99-Exercises/DesignPatterns/StrategyPattern/` - Payment strategies

---

## Observer Pattern: "Notify Multiple Objects of Changes"

### The Problem
```csharp
// Tight coupling between event source and listeners
public class StockPrice
{
    private decimal _price;

    public void UpdatePrice(decimal newPrice)
    {
        _price = newPrice;

        // Hardcoded notifications!
        emailService.SendAlert($"Price changed to {_price}");
        smsService.SendAlert($"Price changed to {_price}");
        dashboardService.UpdateDisplay(_price);

        // Adding new listener requires modifying this class!
    }
}
```

### The Solution
```csharp
// Observer interface
public interface IStockObserver
{
    void OnPriceChanged(decimal newPrice);
}

// Subject
public class StockPrice
{
    private readonly List<IStockObserver> _observers = new();
    private decimal _price;

    public void Attach(IStockObserver observer) => _observers.Add(observer);
    public void Detach(IStockObserver observer) => _observers.Remove(observer);

    public void UpdatePrice(decimal newPrice)
    {
        _price = newPrice;
        foreach (var observer in _observers)
        {
            observer.OnPriceChanged(_price);
        }
    }
}
```

**Benefits:**
- ✅ Loose coupling between subject and observers
- ✅ Add/remove observers dynamically
- ✅ Subject doesn't know concrete observer types
- ✅ Supports one-to-many relationships

**When to Use:**
- One object changes, many objects need to react
- Event-driven systems
- Want loose coupling between components

**Alternatives:**
- C# events (simpler, built-in)
- IObservable<T>/IObserver<T> (Reactive Extensions)
- Message bus/Event bus (decoupled, persistent)

**Real-World Example in Repo:**
- Location: `samples/03-Advanced/DesignPatterns/Behavioral/ObserverPattern.cs`
- Usage: `samples/08-Capstone/MicroVideoPlatform/Shared/Events/` - Domain events

---

## Decorator Pattern: "Add Behavior Without Inheritance"

### The Problem
```csharp
// Deep inheritance hierarchy for every combination
public class SimpleCoffee { }
public class CoffeeWithMilk : SimpleCoffee { }
public class CoffeeWithSugar : SimpleCoffee { }
public class CoffeeWithMilkAndSugar : CoffeeWithMilk { } // Combinatorial explosion!
public class CoffeeWithMilkAndSugarAndWhippedCream : CoffeeWithMilkAndSugar { } // 😱
```

### The Solution
```csharp
// Base component
public interface ICoffee
{
    string GetDescription();
    decimal GetCost();
}

// Concrete component
public class SimpleCoffee : ICoffee
{
    public string GetDescription() => "Simple coffee";
    public decimal GetCost() => 2.00m;
}

// Decorator
public abstract class CoffeeDecorator : ICoffee
{
    protected readonly ICoffee _coffee;
    protected CoffeeDecorator(ICoffee coffee) => _coffee = coffee;

    public virtual string GetDescription() => _coffee.GetDescription();
    public virtual decimal GetCost() => _coffee.GetCost();
}

// Concrete decorators
public class MilkDecorator : CoffeeDecorator
{
    public MilkDecorator(ICoffee coffee) : base(coffee) { }
    public override string GetDescription() => _coffee.GetDescription() + ", Milk";
    public override decimal GetCost() => _coffee.GetCost() + 0.50m;
}

// Usage - chain decorators
ICoffee coffee = new SimpleCoffee();
coffee = new MilkDecorator(coffee);
coffee = new SugarDecorator(coffee);
coffee = new WhippedCreamDecorator(coffee);

Console.WriteLine($"{coffee.GetDescription()}: ${coffee.GetCost()}");
// Output: "Simple coffee, Milk, Sugar, Whipped Cream: $3.50"
```

**Benefits:**
- ✅ Add behavior dynamically at runtime
- ✅ Avoid combinatorial explosion of subclasses
- ✅ Single Responsibility (each decorator has one job)
- ✅ Open/Closed Principle

**When to Use:**
- Need to add responsibilities dynamically
- Inheritance leads to too many subclasses
- Want to compose behavior

**Alternatives:**
- Composition (simpler, less flexible)
- Aspect-Oriented Programming (cross-cutting concerns)

**Real-World Example in Repo:**
- Location: `samples/03-Advanced/DesignPatterns/Structural/DecoratorPattern.cs`
- Usage: Logging, caching, compression decorators

---

## Pattern Selection Guide

| Problem | Pattern | Benefits | Trade-offs |
|---------|---------|----------|------------|
| Complex object creation | **Builder** | Readable, flexible | More classes |
| Need one instance | **Singleton** | Resource-efficient | Global state |
| Create related objects | **Factory** | Loose coupling | Abstraction overhead |
| Swap algorithms | **Strategy** | Flexible, testable | More classes |
| Notify multiple objects | **Observer** | Loose coupling | Memory leaks if not unsubscribed |
| Add behavior dynamically | **Decorator** | Flexible composition | Complexity |

---

## Common Anti-Patterns to Avoid

### 1. Over-Engineering
```csharp
// Don't need a factory for simple creation
// BAD
var user = UserFactory.Create(name, email);

// GOOD
var user = new User(name, email);
```

### 2. Pattern Obsession
Not every problem needs a pattern! Sometimes simple code is best.

### 3. Singleton Abuse
Use Dependency Injection instead of Singleton for most cases.

---

## Real-World Usage in This Repository

### 1. Factory Pattern
- **Location**: `samples/03-Advanced/DesignPatterns/Creational/FactoryPattern.cs`
- **Real Usage**: Video processor creation in MicroVideoPlatform

### 2. Builder Pattern
- **Location**: `samples/03-Advanced/DesignPatterns/Creational/BuilderPattern.cs`
- **Exercise**: `samples/99-Exercises/DesignPatterns/01-Builder/`

### 3. Strategy Pattern
- **Location**: `samples/03-Advanced/DesignPatterns/Behavioral/StrategyPattern.cs`
- **Exercise**: `samples/99-Exercises/DesignPatterns/StrategyPattern/`

### 4. Observer Pattern
- **Location**: `samples/03-Advanced/DesignPatterns/Behavioral/ObserverPattern.cs`
- **Real Usage**: Domain events in MicroVideoPlatform

---

## Key Takeaways

1. **Patterns are solutions, not code** - Understand the problem first
2. **Don't force patterns** - Use when problem matches
3. **SOLID principles drive patterns** - Most patterns follow SOLID
4. **Start simple** - Add patterns as complexity grows
5. **Know alternatives** - Sometimes simpler solution is better

---

## Learning Path

1. **Start**: Factory and Builder (most common)
2. **Next**: Strategy and Observer (behavioral patterns)
3. **Then**: Decorator and Adapter (structural)
4. **Advanced**: Chain of Responsibility, Command
5. **Practice**: Complete exercises in `samples/99-Exercises/DesignPatterns/`
6. **Real-World**: Study `samples/08-Capstone/MicroVideoPlatform/`

---

## Further Reading

- **In This Repo**:
  - `samples/03-Advanced/SOLIDPrinciples/` - SOLID enables patterns
  - `samples/05-RealWorld/MicroserviceTemplate/` - Patterns in Clean Architecture

- **External**:
  - "Head First Design Patterns" (best beginner book)
  - "Design Patterns" by Gang of Four (original reference)
  - refactoring.guru/design-patterns (visual guides)

---

**Next Step**: Run `dotnet run` to see all patterns in action, then implement the exercises in `samples/99-Exercises/DesignPatterns/`.
