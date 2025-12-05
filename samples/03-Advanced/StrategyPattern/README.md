# Strategy Pattern - Payment Processing System

A comprehensive demonstration of the Strategy Pattern using a real-world payment processing scenario. This project shows how to encapsulate interchangeable algorithms, enable runtime strategy selection, and build flexible, maintainable code.

## Quick Start

```bash
# Build and run
dotnet build
dotnet run

# Expected output: 7 comprehensive demonstrations
# - Basic strategy usage
# - Runtime strategy switching
# - Shopping cart integration
# - Intelligent strategy selection
# - Composite strategies with decorators
# - Problem without strategy pattern
# - Performance comparison
```

## Core Concepts

### What is the Strategy Pattern?

The Strategy Pattern is a behavioral design pattern that:
- **Encapsulates algorithms** into separate classes
- **Makes algorithms interchangeable** through a common interface
- **Enables runtime selection** of which algorithm to use
- **Follows Open/Closed Principle** - open for extension, closed for modification

### Key Components

```
┌─────────────────┐
│  PaymentContext │  ← Client uses this
└────────┬────────┘
         │ uses
         ▼
  ┌──────────────────┐
  │ IPaymentStrategy │  ← Common interface
  └─────────┬────────┘
            │ implemented by
      ┌─────┴─────────────────┬──────────────────┬──────────────────┐
      ▼                       ▼                  ▼                  ▼
┌──────────────┐    ┌──────────────┐    ┌──────────────┐    ┌──────────────┐
│ CreditCard   │    │   PayPal     │    │    Crypto    │    │ BankTransfer │
│  Strategy    │    │   Strategy   │    │   Strategy   │    │   Strategy   │
└──────────────┘    └──────────────┘    └──────────────┘    └──────────────┘
```

## Project Structure

```
StrategyPattern/
├── Program.cs                    # Complete implementation
│   ├── Demonstrations (7 methods)
│   │   ├── 1. Basic Strategy
│   │   ├── 2. Runtime Switching
│   │   ├── 3. Shopping Cart
│   │   ├── 4. Intelligent Selection
│   │   ├── 5. Composite Strategies
│   │   ├── 6. Problem Without Strategy
│   │   └── 7. Performance Comparison
│   │
│   ├── Strategy Pattern Components
│   │   ├── IPaymentStrategy (interface)
│   │   ├── PaymentResult (result object)
│   │   ├── CreditCardPaymentStrategy
│   │   ├── PayPalPaymentStrategy
│   │   ├── CryptoPaymentStrategy
│   │   └── BankTransferPaymentStrategy
│   │
│   ├── Context Classes
│   │   ├── PaymentContext
│   │   └── ShoppingCart
│   │
│   ├── Advanced Features
│   │   ├── CartItem
│   │   ├── PaymentStrategySelector (intelligent selection)
│   │   ├── DiscountPaymentStrategy (decorator)
│   │   └── LoyaltyPointsPaymentStrategy (decorator)
│   │
│   └── Legacy Code (anti-pattern)
│       └── LegacyPaymentProcessor
│
├── README.md                     # This file
└── WHY_THIS_PATTERN.md          # Deep dive explanation
```

## Code Examples

### 1. Basic Strategy Pattern

```csharp
// Define the strategy interface
public interface IPaymentStrategy
{
    string Name { get; }
    PaymentResult ProcessPayment(decimal amount);
    decimal CalculateFee(decimal amount);
    bool ValidatePayment(decimal amount);
}

// Implement concrete strategies
public class CreditCardPaymentStrategy : IPaymentStrategy
{
    private readonly string _cardNumber;
    private readonly string _cardHolder;

    public string Name => "Credit Card";

    public PaymentResult ProcessPayment(decimal amount)
    {
        if (!ValidatePayment(amount))
            throw new InvalidOperationException("Invalid payment amount");

        var fee = CalculateFee(amount);

        Console.WriteLine($"   Processing Credit Card payment for {_cardHolder}");
        Console.WriteLine($"   Card: ****{_cardNumber.Substring(_cardNumber.Length - 4)}");

        return new PaymentResult
        {
            Success = true,
            PaymentMethod = Name,
            Amount = amount,
            Fee = fee,
            TransactionId = "CC-" + Guid.NewGuid().ToString("N").Substring(0, 12).ToUpper(),
            ProcessedAt = DateTime.Now,
            Message = "Credit card payment processed successfully"
        };
    }

    public decimal CalculateFee(decimal amount)
    {
        return amount * 0.029m + 0.30m; // 2.9% + $0.30
    }

    public bool ValidatePayment(decimal amount)
    {
        return amount > 0 && amount <= 50000m;
    }
}

// Use the strategy
var context = new PaymentContext();
var creditCard = new CreditCardPaymentStrategy("1234-5678-9012-3456", "John Doe");
context.SetStrategy(creditCard);
var result = context.ProcessPayment(100m);
```

### 2. Runtime Strategy Switching

```csharp
var amount = 250m;
var context = new PaymentContext();

// Start with credit card
context.SetStrategy(new CreditCardPaymentStrategy("****-****-****-1111", "Alice Smith"));
var result1 = context.ProcessPayment(amount);
Console.WriteLine($"Credit Card Fee: ${result1.Fee:F2}");

// Switch to PayPal
context.SetStrategy(new PayPalPaymentStrategy("alice@example.com"));
var result2 = context.ProcessPayment(amount);
Console.WriteLine($"PayPal Fee: ${result2.Fee:F2}");

// Switch to crypto (lower fees)
context.SetStrategy(new CryptoPaymentStrategy("0x123abc...xyz"));
var result3 = context.ProcessPayment(amount);
Console.WriteLine($"Crypto Fee: ${result3.Fee:F2}");

// Output:
// Credit Card Fee: $7.55
// PayPal Fee: $7.55
// Crypto Fee: $1.25
// Savings with Crypto: $6.30
```

### 3. Shopping Cart with Payment Strategy

```csharp
var cart = new ShoppingCart();

// Add items
cart.AddItem("Laptop", 999.99m, 1);
cart.AddItem("Mouse", 29.99m, 2);
cart.AddItem("Keyboard", 79.99m, 1);

Console.WriteLine($"Subtotal: ${cart.GetSubtotal():F2}");

// Try different payment methods
cart.SetPaymentStrategy(new CreditCardPaymentStrategy("****-****-****-2222", "Bob Wilson"));
var result1 = cart.Checkout();
Console.WriteLine($"Credit Card Total: ${result1.TotalAmount:F2}");

cart.SetPaymentStrategy(new CryptoPaymentStrategy("0xabc...xyz"));
var result2 = cart.Checkout();
Console.WriteLine($"Crypto Total: ${result2.TotalAmount:F2}");

// Output:
// Subtotal: $1139.96
// Credit Card Total: $1173.32 (fee: $33.36)
// Crypto Total: $1145.66 (fee: $5.70)
```

## Payment Strategies Comparison

| Strategy | Fee Structure | Processing Time | Best For |
|----------|---------------|-----------------|----------|
| **Credit Card** | 2.9% + $0.30 | Instant | General purchases, wide acceptance |
| **PayPal** | 2.9% + $0.30 | Instant | Small amounts, quick checkout |
| **Cryptocurrency** | 0.5% | 10-60 minutes | Large amounts, lower fees, international |
| **Bank Transfer** | $5 or 0.25% | 1-3 business days | Very large amounts, lowest fees |

## Design Principles Demonstrated

### 1. Open/Closed Principle (OCP)
- **Open for extension**: Add new payment strategies without modifying existing code
- **Closed for modification**: Existing strategies don't change when adding new ones

```csharp
// Adding a new strategy requires NO changes to existing code
public class ApplePayStrategy : IPaymentStrategy
{
    // Implementation...
}

// Use it immediately
var applePay = new ApplePayStrategy();
context.SetStrategy(applePay);
```

### 2. Single Responsibility Principle (SRP)
- Each strategy class has ONE reason to change
- `CreditCardPaymentStrategy` only changes if credit card processing logic changes
- `PayPalPaymentStrategy` only changes if PayPal processing logic changes

### 3. Dependency Inversion Principle (DIP)
- `PaymentContext` depends on `IPaymentStrategy` abstraction, not concrete implementations
- High-level policy (payment processing) doesn't depend on low-level details (specific payment methods)

### 4. Liskov Substitution Principle (LSP)
- Any `IPaymentStrategy` implementation can replace another
- Client code works correctly with any strategy

### 5. Interface Segregation Principle (ISP)
- `IPaymentStrategy` is focused and cohesive
- Clients only depend on methods they actually use

## Use Cases

### When to Use Strategy Pattern

1. **Multiple algorithms for the same task**
   - Different sorting algorithms (QuickSort, MergeSort, BubbleSort)
   - Different compression algorithms (ZIP, GZIP, BZIP2)
   - Different encryption algorithms (AES, RSA, Blowfish)

2. **Runtime algorithm selection**
   - Payment methods based on user choice
   - Shipping methods based on destination
   - Pricing strategies based on customer type

3. **Avoid conditional complexity**
   - Replace large if-else or switch statements
   - Eliminate type checking and casting
   - Simplify complex conditional logic

4. **Make algorithms testable**
   - Test each algorithm independently
   - Mock strategies in unit tests
   - Verify correct strategy is selected

### Real-World Examples

1. **E-commerce Systems**
   - Payment processing (credit card, PayPal, cryptocurrency)
   - Shipping calculation (standard, express, overnight)
   - Discount calculation (percentage, fixed, bulk, seasonal)

2. **Game Development**
   - AI behavior (aggressive, defensive, random)
   - Character movement (walk, run, fly, swim)
   - Attack patterns (melee, ranged, magic)

3. **Data Processing**
   - File format conversion (JSON, XML, CSV)
   - Data validation (regex, schema, custom)
   - Report generation (PDF, Excel, HTML)

4. **Notifications**
   - Delivery method (email, SMS, push notification)
   - Message formatting (plain text, HTML, markdown)
   - Scheduling (immediate, batched, delayed)

## Common Pitfalls and Solutions

### Pitfall 1: Too Many Strategies

Use composition or configuration instead of creating a strategy for every variation.

### Pitfall 2: Leaking Strategy Selection Logic

Encapsulate selection logic in a factory or selector class.

### Pitfall 3: Stateful Strategies

Make strategies stateless or use the prototype pattern for state management.

### Pitfall 4: Null Strategy Checks

Use the Null Object Pattern to provide a default no-op strategy.

## Performance Considerations

### Benchmark Results

```
Processing 100,000 payments:

Strategy Pattern: 169ms
If-Else Chain:    78ms
Difference:       91ms (54% overhead)
```

### Analysis

- Slight performance overhead (~0.00091ms per operation)
- Design benefits outweigh minimal overhead for typical business applications
- For ultra-high-frequency operations, consider strategy pooling

## Testing Strategies

```csharp
[Fact]
public void CreditCard_CalculatesFeeCorrectly()
{
    // Arrange
    var strategy = new CreditCardPaymentStrategy("1234-5678-9012-3456", "Test User");

    // Act
    var fee = strategy.CalculateFee(100m);

    // Assert
    Assert.Equal(3.20m, fee); // 100 * 0.029 + 0.30 = 3.20
}

[Fact]
public void Context_UsesSetStrategy()
{
    // Arrange
    var mockStrategy = new Mock<IPaymentStrategy>();
    mockStrategy.Setup(s => s.ProcessPayment(It.IsAny<decimal>()))
                .Returns(new PaymentResult { Success = true });

    var context = new PaymentContext();
    context.SetStrategy(mockStrategy.Object);

    // Act
    var result = context.ProcessPayment(100m);

    // Assert
    Assert.True(result.Success);
    mockStrategy.Verify(s => s.ProcessPayment(100m), Times.Once);
}
```

## Advanced Patterns

### Strategy + Factory Pattern

Create strategies through a centralized factory for better encapsulation.

### Strategy + Decorator Pattern

Wrap strategies with decorators to add logging, retry logic, or other cross-cutting concerns.

### Strategy + Chain of Responsibility

Combine with validation chains for robust payment processing.

## Learning Path

### Beginner Level
1. Understand the problem: Why if-else chains are bad
2. Learn the pattern: Interface + multiple implementations
3. Basic usage: Create strategies and context
4. Runtime switching: Change strategies dynamically

### Intermediate Level
5. Design principles: OCP, SRP, DIP
6. Intelligent selection: Factories and selectors
7. Composition: Decorator pattern with strategies
8. Testing: Unit tests for strategies and context

### Advanced Level
9. Performance optimization: Strategy pooling, struct strategies
10. Advanced patterns: Strategy + Factory + Decorator
11. Real-world scenarios: E-commerce, games, data processing
12. Anti-patterns: Avoid common pitfalls

## Related Patterns

| Pattern | Relationship | When to Use |
|---------|-------------|-------------|
| **Factory** | Creates strategies | When strategy creation is complex |
| **Decorator** | Wraps strategies | When adding features to strategies |
| **Template Method** | Alternative to Strategy | When algorithm structure is fixed |
| **State** | Similar concept | When behavior changes based on state |
| **Command** | Encapsulates actions | When you need undo/redo |

## Further Reading

- [Design Patterns: Elements of Reusable Object-Oriented Software](https://en.wikipedia.org/wiki/Design_Patterns) (Gang of Four)
- [Head First Design Patterns](https://www.oreilly.com/library/view/head-first-design/0596007124/) (O'Reilly)
- [Refactoring Guru - Strategy Pattern](https://refactoring.guru/design-patterns/strategy)
- [WHY_THIS_PATTERN.md](./WHY_THIS_PATTERN.md) - Deep dive into this implementation

## License

This code is part of the C# Advanced Concepts learning repository and is provided for educational purposes.
