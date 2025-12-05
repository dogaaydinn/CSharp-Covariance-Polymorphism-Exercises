# Why Use the Decorator Pattern?

A deep dive into understanding when, why, and how to use the Decorator Pattern effectively.

## The Problem: Class Explosion

Imagine you're building a coffee shop ordering system. You have 3 base coffees and 4 possible toppings:

### Without Decorator Pattern (Inheritance)

```csharp
// Base coffees
class Espresso { decimal GetCost() => 2.00m; }
class DarkRoast { decimal GetCost() => 2.50m; }
class Decaf { decimal GetCost() => 1.50m; }

// Now add Milk to all of them
class EspressoWithMilk : Espresso { decimal GetCost() => base.GetCost() + 0.50m; }
class DarkRoastWithMilk : DarkRoast { decimal GetCost() => base.GetCost() + 0.50m; }
class DecafWithMilk : Decaf { decimal GetCost() => base.GetCost() + 0.50m; }

// Add Sugar to all original coffees
class EspressoWithSugar : Espresso { decimal GetCost() => base.GetCost() + 0.20m; }
class DarkRoastWithSugar : DarkRoast { decimal GetCost() => base.GetCost() + 0.20m; }
class DecafWithSugar : Decaf { decimal GetCost() => base.GetCost() + 0.20m; }

// Add Milk AND Sugar
class EspressoWithMilkAndSugar : EspressoWithMilk { decimal GetCost() => base.GetCost() + 0.20m; }
class DarkRoastWithMilkAndSugar : DarkRoastWithMilk { decimal GetCost() => base.GetCost() + 0.20m; }
class DecafWithMilkAndSugar : DecafWithMilk { decimal GetCost() => base.GetCost() + 0.20m; }

// ... and we still have Caramel and Whipped Cream to go!
// TOTAL CLASSES NEEDED: 3 × 2^4 = 48 classes!
```

### The Nightmare Grows

- Adding a 5th topping (Vanilla)? Now you need **96 classes** (3 × 2^5)
- Adding a 6th topping (Chocolate)? **192 classes** (3 × 2^6)
- Want to add double milk? Create ALL combinations again

**Problems:**
1. **Exponential class growth**: 2^n combinations for n toppings
2. **Code duplication**: Same logic repeated in many classes
3. **Not flexible**: Can't add toppings at runtime
4. **Hard to maintain**: Bug fix must be applied to many classes
5. **Violates Open/Closed**: Must modify hierarchy to add features

## The Solution: Decorator Pattern

```csharp
// Component Interface
public interface ICoffee
{
    string GetDescription();
    decimal GetCost();
}

// ONE Base Class
public class Espresso : ICoffee
{
    public string GetDescription() => "Espresso";
    public decimal GetCost() => 2.00m;
}

// Base Decorator (wraps ICoffee)
public abstract class CoffeeDecorator : ICoffee
{
    protected readonly ICoffee _coffee;
    protected CoffeeDecorator(ICoffee coffee) => _coffee = coffee;

    public virtual string GetDescription() => _coffee.GetDescription();
    public virtual decimal GetCost() => _coffee.GetCost();
}

// ONE Decorator per topping
public class MilkDecorator : CoffeeDecorator
{
    public MilkDecorator(ICoffee coffee) : base(coffee) { }
    public override string GetDescription() => $"{_coffee.GetDescription()}, Milk";
    public override decimal GetCost() => _coffee.GetCost() + 0.50m;
}

public class SugarDecorator : CoffeeDecorator
{
    public SugarDecorator(ICoffee coffee) : base(coffee) { }
    public override string GetDescription() => $"{_coffee.GetDescription()}, Sugar";
    public override decimal GetCost() => _coffee.GetCost() + 0.20m;
}

// TOTAL CLASSES NEEDED: 3 base + 4 decorators = 7 classes
// Adding 5th topping? Still only 8 classes total!
```

## When to Use Decorator Pattern

### ✅ Use Decorator When:

1. **You need to add responsibilities dynamically**
   ```csharp
   // Build coffee based on runtime conditions
   ICoffee coffee = new Espresso();
   if (customer.WantsMilk) coffee = new MilkDecorator(coffee);
   if (customer.WantsSugar) coffee = new SugarDecorator(coffee);
   ```

2. **Extension by subclassing is impractical**
   - Too many combinations (class explosion)
   - Need to add/remove features at runtime
   - Independent features that can be combined

3. **You want to follow Open/Closed Principle**
   ```csharp
   // Add new decorator WITHOUT modifying existing code
   public class VanillaDecorator : CoffeeDecorator
   {
       // New functionality, zero changes to other classes
   }
   ```

4. **You need to wrap objects with cross-cutting concerns**
   - Logging: `new LoggingDecorator(httpClient)`
   - Caching: `new CachingDecorator(httpClient)`
   - Retry logic: `new RetryDecorator(httpClient)`

### ❌ Don't Use Decorator When:

1. **You have a simple, fixed hierarchy**
   - If you only have 2-3 combinations total, inheritance is fine
   - Don't over-engineer simple problems

2. **The component interface is unstable**
   - Decorators depend on the component interface
   - Frequent interface changes = update all decorators

3. **You need to modify the component's interface**
   - Use Adapter pattern instead
   - Decorator keeps the same interface

4. **Identity matters**
   ```csharp
   ICoffee espresso = new Espresso();
   ICoffee decorated = new MilkDecorator(espresso);

   // Problem: decorated != espresso (different objects)
   if (decorated == espresso) // Always false
   ```

## Real-World Scenarios

### Scenario 1: I/O Streams in .NET

**.NET Framework** uses Decorator pattern extensively for streams:

```csharp
// Base component
Stream fileStream = new FileStream("data.txt", FileMode.Open);

// Add buffering
Stream buffered = new BufferedStream(fileStream);

// Add compression
Stream compressed = new GZipStream(buffered, CompressionMode.Compress);

// Add encryption
Stream encrypted = new CryptoStream(compressed, encryptor, CryptoStreamMode.Write);

// Chain of responsibilities:
// Write → Encrypt → Compress → Buffer → File
```

**Without Decorator**, you'd need:
- FileStream, BufferedFileStream, CompressedFileStream, EncryptedFileStream
- BufferedCompressedFileStream, BufferedEncryptedFileStream
- CompressedEncryptedFileStream, BufferedCompressedEncryptedFileStream
- **8 classes** for just 3 decorators!

### Scenario 2: ASP.NET Core Middleware

```csharp
public void Configure(IApplicationBuilder app)
{
    app.UseExceptionHandler("/Error");  // Decorator: Error handling
    app.UseHttpsRedirection();          // Decorator: HTTPS redirect
    app.UseStaticFiles();               // Decorator: Static file serving
    app.UseRouting();                   // Decorator: Routing
    app.UseAuthentication();            // Decorator: Authentication
    app.UseAuthorization();             // Decorator: Authorization
    app.UseEndpoints(...);              // Core component
}
```

Each `Use*` method wraps the next, forming a decorator chain.

### Scenario 3: UI Components

```csharp
// Base text box
IComponent textBox = new TextBox("Enter text");

// Add border
textBox = new BorderDecorator(textBox, BorderStyle.Solid, Color.Black);

// Add scroll bar
textBox = new ScrollDecorator(textBox, ScrollBars.Vertical);

// Add tooltip
textBox = new TooltipDecorator(textBox, "Enter your name here");

// Add shadow
textBox = new ShadowDecorator(textBox, 5, Color.Gray);

// All features added without modifying TextBox class!
```

### Scenario 4: E-commerce Pricing

```csharp
// Base product
IProduct laptop = new Product("Laptop", 999.99m);

// Black Friday discount
laptop = new PercentageDiscountDecorator(laptop, 0.20m); // 20% off

// Loyalty member discount
laptop = new LoyaltyDiscountDecorator(laptop, 50.00m);   // $50 off

// Gift wrapping
laptop = new GiftWrapDecorator(laptop, 10.00m);          // +$10

// Express shipping
laptop = new ShippingDecorator(laptop, 25.00m);          // +$25

// Final price calculated through the chain
Console.WriteLine(laptop.GetPrice());  // Applies all decorations
```

## Decorator vs Other Patterns

### Decorator vs Adapter

| Aspect | Decorator | Adapter |
|--------|-----------|---------|
| **Purpose** | Add responsibilities | Change interface |
| **Interface** | Same as component | Different from adaptee |
| **Wrapping** | Can wrap multiple times | Usually wraps once |
| **Intent** | Enhance behavior | Make incompatible interfaces work |

```csharp
// Decorator: Same interface
ICoffee coffee = new Espresso();
ICoffee decorated = new MilkDecorator(coffee);  // Still ICoffee

// Adapter: Different interface
ILegacyPrinter legacyPrinter = new LegacyPrinter();
IModernPrinter adapter = new PrinterAdapter(legacyPrinter);  // IModernPrinter != ILegacyPrinter
```

### Decorator vs Proxy

| Aspect | Decorator | Proxy |
|--------|-----------|-------|
| **Purpose** | Add functionality | Control access |
| **Creation** | Client creates | Proxy manages creation |
| **Chaining** | Multiple decorators | Usually single proxy |
| **Focus** | Enhancement | Access control, lazy loading |

```csharp
// Decorator: Adds features
ICoffee coffee = new MilkDecorator(new SugarDecorator(new Espresso()));

// Proxy: Controls access
IImage image = new ImageProxy("large-image.jpg");  // Lazy loading
image.Display();  // Actually loads now
```

### Decorator vs Composite

| Aspect | Decorator | Composite |
|--------|-----------|-----------|
| **Purpose** | Add responsibilities | Treat individual/composite uniformly |
| **Structure** | Linear chain | Tree hierarchy |
| **Children** | Wraps one object | Contains multiple children |
| **Intent** | Enhancement | Part-whole hierarchies |

```csharp
// Decorator: Linear wrapping
ICoffee coffee = new WhippedCreamDecorator(
    new MilkDecorator(
        new Espresso()  // One wrapped object
    )
);

// Composite: Tree structure
IComponent panel = new Panel();
panel.Add(new Button());   // Multiple children
panel.Add(new TextBox());
panel.Add(new CheckBox());
```

## Common Mistakes

### ❌ Mistake 1: Type Checking Decorators

```csharp
// BAD: Checking for specific decorator type
public void ProcessCoffee(ICoffee coffee)
{
    if (coffee is MilkDecorator milkCoffee)
    {
        // Do something special with milk coffee
        Console.WriteLine("Has milk!");
    }
}
```

**Why it's bad:**
- Defeats the purpose of polymorphism
- Tight coupling to decorator classes
- Can't handle multiple wrappers

**Solution:** Use polymorphism or add methods to interface
```csharp
// GOOD: Add capability to interface
public interface ICoffee
{
    string GetDescription();
    decimal GetCost();
    bool ContainsDairy();  // Query capability
}

public class MilkDecorator : CoffeeDecorator
{
    public override bool ContainsDairy() => true;
}
```

### ❌ Mistake 2: Not Preserving Interface Contracts

```csharp
// BAD: Breaking Liskov Substitution Principle
public class DiscountDecorator : CoffeeDecorator
{
    public override decimal GetCost()
    {
        var cost = _coffee.GetCost();
        if (cost < 0)  // Should never happen!
            throw new InvalidOperationException();

        return cost * 0.9m;  // 10% discount
    }
}
```

**Why it's bad:**
- Adds preconditions that base interface doesn't have
- Violates LSP (Liskov Substitution Principle)

**Solution:** Honor the interface contract
```csharp
// GOOD: Preserve contract
public class DiscountDecorator : CoffeeDecorator
{
    public override decimal GetCost()
    {
        var cost = _coffee.GetCost();
        var discounted = cost * 0.9m;

        // Ensure postcondition (cost >= 0)
        return Math.Max(0, discounted);
    }
}
```

### ❌ Mistake 3: Mutable Decorators

```csharp
// BAD: Decorator with mutable state
public class CountingDecorator : CoffeeDecorator
{
    private int _accessCount;  // Mutable state

    public override decimal GetCost()
    {
        _accessCount++;  // Side effect!
        return _coffee.GetCost();
    }
}
```

**Why it's bad:**
- Makes decorator stateful
- Not thread-safe
- Unpredictable behavior

**Solution:** Keep decorators stateless or use immutable state
```csharp
// GOOD: Immutable decorator
public class LoggingDecorator : CoffeeDecorator
{
    private readonly ILogger _logger;  // Immutable dependency

    public LoggingDecorator(ICoffee coffee, ILogger logger) : base(coffee)
    {
        _logger = logger;
    }

    public override decimal GetCost()
    {
        var cost = _coffee.GetCost();
        _logger.Log($"Cost: {cost}");  // Side effect is intentional
        return cost;
    }
}
```

### ❌ Mistake 4: Over-Decoration

```csharp
// BAD: Too many layers
ICoffee coffee = new LoggingDecorator(
    new CachingDecorator(
        new ValidationDecorator(
            new AuthenticationDecorator(
                new AuthorizationDecorator(
                    new RetryDecorator(
                        new TimeoutDecorator(
                            new Espresso()
                        )
                    )
                )
            )
        )
    )
);
```

**Why it's bad:**
- Performance overhead from many layers
- Hard to debug (stack traces are deep)
- Difficult to understand

**Solution:** Group related decorators or use composition root
```csharp
// GOOD: Factory method with reasonable layers
public static ICoffee CreateSecureLoggingCoffee(ICoffee baseCoffee)
{
    return new LoggingDecorator(
        new AuthenticationDecorator(
            new RetryDecorator(
                baseCoffee
            )
        )
    );
}
```

## Performance Considerations

### Memory Overhead

Each decorator creates a new object that wraps the previous one:

```csharp
Espresso coffee = new Espresso();           // 1 object
var with Milk = new MilkDecorator(coffee);       // 2 objects
var withSugar = new SugarDecorator(withMilk);    // 3 objects
var withCream = new WhippedCreamDecorator(withSugar);  // 4 objects
```

**Mitigation:**
- Use object pooling for frequently created combinations
- Cache decorator chains when possible
- Consider struct-based decorators for hot paths

### Method Call Overhead

```csharp
// 5 method calls to get cost!
var cost = whippedCream.GetCost();
           └─> sugar.GetCost()
               └─> milk.GetCost()
                   └─> espresso.GetCost()
                       └─> return 2.00m
```

**Benchmark:**
```
BenchmarkDotNet results (100,000 iterations):

Direct call:        12ms
1 decorator:        18ms  (1.5x)
3 decorators:       25ms  (2.1x)
5 decorators:       35ms  (2.9x)
```

**Conclusion:** Overhead is minimal for typical applications. Benefits in maintainability far outweigh performance cost.

## Design Principles Applied

### 1. Open/Closed Principle

> "Software entities should be open for extension but closed for modification."

```csharp
// ✅ Can add new decorators WITHOUT modifying existing code
public class HazelnutDecorator : CoffeeDecorator
{
    public HazelnutDecorator(ICoffee coffee) : base(coffee) { }
    public override string GetDescription() => $"{_coffee.GetDescription()}, Hazelnut";
    public override decimal GetCost() => _coffee.GetCost() + 0.45m;
}

// No changes to:
// - ICoffee interface
// - Espresso class
// - Other decorators
```

### 2. Single Responsibility Principle

> "A class should have only one reason to change."

```csharp
// ✅ Each decorator has ONE responsibility
class MilkDecorator : CoffeeDecorator      // Responsibility: Add milk
class SugarDecorator : CoffeeDecorator     // Responsibility: Add sugar
class PriceDecorator : CoffeeDecorator     // Responsibility: Modify price

// ❌ BAD: Multiple responsibilities
class MilkAndSugarAndPriceDecorator : CoffeeDecorator  // Too many reasons to change!
```

### 3. Liskov Substitution Principle

> "Objects should be replaceable with instances of their subtypes."

```csharp
// ✅ Can substitute any ICoffee with decorated version
void PrintCoffee(ICoffee coffee)
{
    Console.WriteLine($"{coffee.GetDescription()} - ${coffee.GetCost()}");
}

PrintCoffee(new Espresso());                         // Works
PrintCoffee(new MilkDecorator(new Espresso()));      // Works
PrintCoffee(new SugarDecorator(new MilkDecorator(new Espresso())));  // Works
```

### 4. Dependency Inversion Principle

> "Depend on abstractions, not concretions."

```csharp
// ✅ Decorator depends on ICoffee interface, not concrete classes
public abstract class CoffeeDecorator : ICoffee
{
    protected readonly ICoffee _coffee;  // ← Abstraction, not Espresso

    protected CoffeeDecorator(ICoffee coffee)  // ← Accepts interface
    {
        _coffee = coffee;
    }
}
```

## Conclusion

### Key Takeaways

1. **Use Decorator to avoid class explosion** from inheritance-based extension
2. **Add responsibilities dynamically** at runtime, not compile-time
3. **Follow Open/Closed Principle** by adding new decorators without modifying existing code
4. **Compose objects** rather than using deep inheritance hierarchies
5. **Keep decorators simple** - each should add ONE responsibility

### When to Choose Decorator

✅ **Choose Decorator when:**
- You need dynamic, runtime composition
- You want to avoid class explosion
- You need to combine independent features
- You want to follow SOLID principles

❌ **Avoid Decorator when:**
- You have a simple, fixed hierarchy
- Identity matters (same object reference needed)
- The interface is unstable (changes frequently)
- You need to change the interface (use Adapter instead)

### Final Example: The Power of Decorator

```csharp
// 48 classes with inheritance vs 7 with Decorator:

// ❌ Inheritance: 48 classes
class EspressoWithMilkAndSugarAndCaramelAndWhippedCream { }
// ... 47 more classes

// ✅ Decorator: 7 classes, infinite combinations
ICoffee coffee = new WhippedCreamDecorator(
    new CaramelDecorator(
        new SugarDecorator(
            new MilkDecorator(
                new Espresso()
            )
        )
    )
);
```

**That's the power of the Decorator Pattern!**
