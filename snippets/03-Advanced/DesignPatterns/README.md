# Design Patterns Tutorial

A comprehensive, reference-quality guide to Gang of Four design patterns in C#. This tutorial demonstrates 9 essential patterns with production-ready implementations and real-world examples.

## Overview

Design patterns are reusable solutions to common software design problems. They represent best practices evolved over time by experienced developers. This tutorial covers three categories of patterns:

- **Creational Patterns** - Object creation mechanisms
- **Structural Patterns** - Object composition and relationships
- **Behavioral Patterns** - Object interaction and communication

## Quick Start

```bash
cd samples/03-Advanced/DesignPatterns
dotnet run
```

The program will execute all 9 pattern examples with detailed output showing how each pattern works.

## Pattern Catalog

### Creational Patterns (3)

#### 1. Singleton Pattern
**One-line description:** Ensures a class has only one instance and provides global access point.

**When to use:**
- Configuration managers
- Database connections
- Thread pools
- Logging services
- Caching mechanisms

**Example use case:**
```csharp
var db = DatabaseConnection.Instance;
db.Connect();
db.Query("SELECT * FROM users");
```

**Trade-offs:**
- ✅ Guaranteed single instance
- ✅ Lazy initialization possible
- ✅ Global access point
- ❌ Violates Single Responsibility Principle (manages its own creation)
- ❌ Can make unit testing difficult
- ❌ Can hide dependencies

**Thread safety:** This implementation uses `Lazy<T>` which is thread-safe by default.

---

#### 2. Factory Pattern
**One-line description:** Creates objects without specifying exact class type.

**When to use:**
- Object creation depends on runtime conditions
- Need to decouple client from concrete classes
- Want centralized object creation logic
- Creating families of related objects (Abstract Factory)

**Example use case:**
```csharp
// Simple Factory
IVehicle car = VehicleFactory.CreateVehicle("car");
car.Drive();

// Abstract Factory
IVehicleFactory factory = new LuxuryVehicleFactory();
var engine = factory.CreateEngine();
var transmission = factory.CreateTransmission();
```

**Trade-offs:**
- ✅ Loose coupling between client and concrete classes
- ✅ Single Responsibility Principle
- ✅ Open/Closed Principle
- ✅ Easy to extend with new types
- ❌ Can increase code complexity
- ❌ May create unnecessary abstractions

**Alternatives:** Dependency Injection, Builder pattern

---

#### 3. Builder Pattern
**One-line description:** Constructs complex objects step-by-step with fluent interface.

**When to use:**
- Objects have many optional parameters
- Need different representations of same object
- Construction algorithm should be independent of parts
- Want to construct immutable objects

**Example use case:**
```csharp
var pizza = new PizzaBuilder()
    .SetSize("Large")
    .SetCrust("Thin")
    .AddTopping("Pepperoni")
    .AddTopping("Mushrooms")
    .AddExtraCheese()
    .Build();
```

**Trade-offs:**
- ✅ Constructs objects step-by-step
- ✅ Fluent, readable API
- ✅ Reuses construction code
- ✅ Can create different representations
- ❌ Increases code complexity
- ❌ Requires creating separate builder class

**Alternatives:** Constructor with many parameters, Object initializers, Factory pattern

---

### Structural Patterns (3)

#### 4. Decorator Pattern
**One-line description:** Adds behavior to objects dynamically without modifying their structure.

**When to use:**
- Add responsibilities to objects dynamically
- Extend functionality without subclassing
- Avoid class explosion from inheritance
- Mix and match features flexibly

**Example use case:**
```csharp
ICoffee coffee = new SimpleCoffee();
coffee = new MilkDecorator(coffee);
coffee = new SugarDecorator(coffee);
coffee = new WhippedCreamDecorator(coffee);
// Final: Coffee with Milk, Sugar, and Whipped Cream
```

**Trade-offs:**
- ✅ Open/Closed Principle
- ✅ More flexible than inheritance
- ✅ Compose behaviors at runtime
- ✅ Single Responsibility Principle
- ❌ Hard to remove specific decorator from stack
- ❌ Order of decorators matters
- ❌ Complex initialization code

**Alternatives:** Inheritance (static), Strategy pattern (single behavior)

**Real-world examples:** .NET Stream classes (BufferedStream, CryptoStream, GZipStream)

---

#### 5. Adapter Pattern
**One-line description:** Makes incompatible interfaces work together.

**When to use:**
- Integrate legacy code with new systems
- Use third-party libraries with different interfaces
- Make incompatible interfaces compatible
- Create reusable classes that cooperate with unrelated classes

**Example use case:**
```csharp
// Old system
var legacySystem = new LegacyPaymentSystem();

// Adapt to modern interface
IPaymentProcessor processor = new LegacyPaymentAdapter(legacySystem);
processor.ProcessPayment(100m, "USD", "user@example.com");
```

**Trade-offs:**
- ✅ Single Responsibility Principle
- ✅ Open/Closed Principle
- ✅ Reuse existing functionality
- ✅ Integrate incompatible code
- ❌ Increases overall complexity
- ❌ Sometimes simpler to refactor existing code

**Alternatives:** Facade pattern (simplifies interface), Refactoring (if you control the code)

**Real-world examples:** Database adapters (ADO.NET providers), Legacy system integration

---

#### 6. Proxy Pattern
**One-line description:** Provides surrogate to control access to another object.

**When to use:**
- Lazy initialization (virtual proxy)
- Access control (protection proxy)
- Remote object access (remote proxy)
- Logging, caching, or monitoring access

**Types:**
- **Virtual Proxy** - Lazy loading of expensive objects
- **Protection Proxy** - Access control based on permissions
- **Remote Proxy** - Local representative of remote object
- **Caching Proxy** - Caches results to improve performance

**Example use case:**
```csharp
// Virtual Proxy - loads image only when needed
IImage image = new ImageProxy("large-photo.jpg");
// Image not loaded yet...
image.Display(); // Loads now!

// Caching Proxy - improves performance
IDataService service = new CachingDataServiceProxy(new DatabaseService());
service.GetData("key"); // Database query
service.GetData("key"); // From cache!
```

**Trade-offs:**
- ✅ Control object access
- ✅ Add functionality transparently
- ✅ Lazy initialization
- ✅ Open/Closed Principle
- ❌ Increases complexity
- ❌ May introduce latency

**Alternatives:** Decorator pattern (focuses on adding behavior)

**Real-world examples:** Entity Framework lazy loading, WCF proxies, caching layers

---

### Behavioral Patterns (3)

#### 7. Strategy Pattern
**One-line description:** Defines family of algorithms and makes them interchangeable.

**When to use:**
- Multiple variants of an algorithm exist
- Have many conditional statements switching between algorithms
- Want to isolate algorithm implementation
- Need to change algorithm at runtime

**Example use case:**
```csharp
var cart = new ShoppingCart();
cart.AddItem("Laptop", 999.99m);

// Choose payment method at runtime
cart.SetPaymentStrategy(new CreditCardPayment(...));
cart.Checkout();

// Change strategy
cart.SetPaymentStrategy(new PayPalPayment(...));
cart.Checkout();
```

**Trade-offs:**
- ✅ Open/Closed Principle
- ✅ Runtime algorithm switching
- ✅ Isolates algorithm implementation
- ✅ Eliminates conditional statements
- ❌ Clients must be aware of different strategies
- ❌ Increases number of objects
- ❌ Communication overhead between strategy and context

**Alternatives:** Simple function parameters, Template Method pattern

**Real-world examples:** Sorting algorithms, compression algorithms, validation strategies

---

#### 8. Observer Pattern
**One-line description:** Defines one-to-many dependency where state changes notify all dependents.

**When to use:**
- Change in one object requires changing others
- Object should notify others without knowing who they are
- Need event-driven architecture
- Implement pub/sub systems

**Example use case:**
```csharp
var weatherStation = new WeatherStation();

// Subscribe observers
weatherStation.Attach(new CurrentConditionsDisplay("Display 1"));
weatherStation.Attach(new StatisticsDisplay("Display 2"));
weatherStation.Attach(new ForecastDisplay("Display 3"));

// All observers notified automatically
weatherStation.SetMeasurements(25.5f, 65f, 1013.1f);
```

**Trade-offs:**
- ✅ Open/Closed Principle
- ✅ Establishes relationships at runtime
- ✅ Loose coupling
- ✅ Dynamic subscription management
- ❌ Observers notified in random order
- ❌ Can cause memory leaks if not unsubscribed
- ❌ Unexpected cascading updates

**Alternatives:** Event delegates (C# events), Message queues, Reactive Extensions (Rx)

**Real-world examples:** .NET events, UI frameworks (WPF, WinForms), message brokers

---

#### 9. Chain of Responsibility Pattern
**One-line description:** Passes request along chain of handlers until one handles it.

**When to use:**
- Multiple objects can handle a request
- Handler isn't known in advance
- Want to avoid coupling sender to receiver
- Need processing pipeline or middleware

**Example use case:**
```csharp
// Build authentication chain
var userCheck = new UserExistenceHandler();
var passwordCheck = new PasswordHandler();
var rateLimit = new RateLimitHandler();
var ipCheck = new IpWhitelistHandler();

userCheck.SetNext(passwordCheck)
         .SetNext(rateLimit)
         .SetNext(ipCheck);

// Request passes through entire chain
bool authenticated = userCheck.Authenticate(request);
```

**Trade-offs:**
- ✅ Decouples sender from receivers
- ✅ Add/remove handlers dynamically
- ✅ Single Responsibility Principle
- ✅ Open/Closed Principle
- ❌ Request may go unhandled
- ❌ Can be hard to debug
- ❌ May impact performance with long chains

**Alternatives:** Command pattern, Template Method pattern

**Real-world examples:** ASP.NET Core middleware, logging pipelines, event bubbling in UI

---

## Learning Path

### Beginner (Start Here)
1. **Singleton** - Simplest creational pattern
2. **Factory** - Basic object creation abstraction
3. **Strategy** - Simple behavioral pattern

### Intermediate
4. **Decorator** - Dynamic behavior composition
5. **Observer** - Event handling and pub/sub
6. **Adapter** - Interface translation

### Advanced
7. **Builder** - Complex object construction
8. **Proxy** - Advanced access control
9. **Chain of Responsibility** - Request processing pipeline

## When NOT to Use Patterns

Remember: Patterns are tools, not rules. Avoid using patterns when:

- **Overengineering**: Simple problem doesn't need complex solution
- **Premature optimization**: Don't add patterns "just in case"
- **Learning exercise**: Don't use patterns just to practice them
- **Performance critical**: Some patterns add overhead
- **Team unfamiliar**: Patterns can reduce readability if team doesn't know them

**Golden rule**: Use patterns to solve real problems, not for the pattern's sake.

## Pattern Comparison

### Decorator vs Strategy
- **Decorator**: Adds features to object, can stack multiple decorators
- **Strategy**: Changes entire algorithm, only one strategy at a time

### Factory vs Builder
- **Factory**: Simple object creation, typically in one step
- **Builder**: Complex object construction, step-by-step process

### Adapter vs Proxy
- **Adapter**: Changes interface to make it compatible
- **Proxy**: Same interface but controls access or adds functionality

### Observer vs Chain of Responsibility
- **Observer**: One-to-many, all observers notified
- **Chain**: One-to-one, only one handler processes request

## Project Structure

```
DesignPatterns/
├── Creational/
│   ├── SingletonPattern.cs      (~200 lines)
│   ├── FactoryPattern.cs        (~300 lines)
│   └── BuilderPattern.cs        (~350 lines)
├── Structural/
│   ├── DecoratorPattern.cs      (~350 lines)
│   ├── AdapterPattern.cs        (~400 lines)
│   └── ProxyPattern.cs          (~450 lines)
├── Behavioral/
│   ├── StrategyPattern.cs       (~400 lines)
│   ├── ObserverPattern.cs       (~450 lines)
│   └── ChainOfResponsibilityPattern.cs (~400 lines)
├── Program.cs                   (~160 lines)
└── README.md                    (this file)
```

**Total lines of code**: ~3,460 lines

## Key Principles

All patterns in this tutorial follow SOLID principles:

- **S**ingle Responsibility Principle
- **O**pen/Closed Principle
- **L**iskov Substitution Principle
- **I**nterface Segregation Principle
- **D**ependency Inversion Principle

## Code Quality

Each pattern implementation includes:

- ✅ Complete XML documentation
- ✅ Clear UML structure diagrams (in comments)
- ✅ Multiple real-world examples
- ✅ Problem statement and motivation
- ✅ Thread safety considerations (where applicable)
- ✅ Error handling
- ✅ Best practices

## Running Examples

Run all patterns:
```bash
dotnet run
```

Build only:
```bash
dotnet build
```

Clean build:
```bash
dotnet clean
dotnet build
```

## Further Reading

### Books
- **Design Patterns: Elements of Reusable Object-Oriented Software** - Gang of Four (GoF)
- **Head First Design Patterns** - Freeman & Robson
- **Patterns of Enterprise Application Architecture** - Martin Fowler

### Online Resources
- [Refactoring.Guru - Design Patterns](https://refactoring.guru/design-patterns)
- [Source Making - Design Patterns](https://sourcemaking.com/design_patterns)
- [Microsoft Docs - Cloud Design Patterns](https://docs.microsoft.com/en-us/azure/architecture/patterns/)

### Other Pattern Categories
This tutorial covers Gang of Four patterns. Other important categories:

- **Architectural Patterns**: MVC, MVVM, Microservices, Event Sourcing
- **Concurrency Patterns**: Thread Pool, Producer-Consumer, Read-Write Lock
- **Cloud Patterns**: Circuit Breaker, Retry, Bulkhead, CQRS
- **Enterprise Integration Patterns**: Message Router, Content Filter, Pipes and Filters

## Contributing

This is a learning resource. If you find issues or have suggestions:

1. Review the implementation
2. Check if the pattern solves a real problem
3. Ensure changes maintain clarity and educational value
4. Test the code works correctly

## Summary

Design patterns are powerful tools but should be used judiciously:

1. **Understand the problem first** - Don't force patterns
2. **Start simple** - Add complexity only when needed
3. **Know the trade-offs** - Every pattern has costs
4. **Consider alternatives** - Multiple solutions often exist
5. **Think about maintainability** - Code must be understandable

Happy coding!

---

**Note**: This tutorial emphasizes practical, production-ready implementations. Each pattern is demonstrated with realistic scenarios and includes considerations for real-world usage, not just academic examples.
