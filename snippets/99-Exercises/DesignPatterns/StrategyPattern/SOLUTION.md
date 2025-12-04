# 💡 Solution: Strategy Pattern - Payment Processing System

**⚠️ SPOILER ALERT:** Only read this if you've attempted the exercise first!

---

## Complete Solution

### ShoppingCart.cs (Complete)

```csharp
namespace StrategyPattern;

public class ShoppingCart
{
    private readonly List<CartItem> _items = new();
    private readonly IPaymentStrategy _paymentStrategy;

    public ShoppingCart(IPaymentStrategy paymentStrategy)
    {
        _paymentStrategy = paymentStrategy;
    }

    public void AddItem(CartItem item)
    {
        _items.Add(item);
    }

    public string ProcessPayment()
    {
        decimal total = _items.Sum(item => item.Price);
        string description = $"Order with {_items.Count} item(s)";
        return _paymentStrategy.ProcessPayment(total, description);
    }

    public decimal GetTotal()
    {
        return _items.Sum(item => item.Price);
    }

    public int GetItemCount()
    {
        return _items.Count;
    }
}
```

### BankTransferPaymentStrategy.cs (Complete)

```csharp
namespace StrategyPattern;

public class BankTransferPaymentStrategy : IPaymentStrategy
{
    private readonly string _accountNumber;

    public BankTransferPaymentStrategy(string accountNumber)
    {
        _accountNumber = accountNumber;
    }

    public string ProcessPayment(decimal amount, string description)
    {
        return $"Bank transfer of ${amount:F2} from account {_accountNumber}. " +
               $"Reference: {description}";
    }
}
```

---

## Step-by-Step Explanation

### Part 1: ShoppingCart Constructor

**What was required:**
```csharp
// Add a private readonly field
private readonly IPaymentStrategy _paymentStrategy;

// Implement constructor
public ShoppingCart(IPaymentStrategy paymentStrategy)
{
    _paymentStrategy = paymentStrategy;
}
```

**Why this works:**
1. **Dependency Injection:** The `ShoppingCart` receives its dependency (payment strategy) through the constructor
2. **Loose Coupling:** `ShoppingCart` depends on the `IPaymentStrategy` interface, not concrete implementations
3. **Testability:** Easy to test by passing mock strategies
4. **Flexibility:** Can change payment method by passing different strategy

**Key Insight:**
The `ShoppingCart` doesn't know (or care) which specific payment strategy it's using. It just knows it has **something** that implements `IPaymentStrategy`. This is the essence of the Strategy pattern.

---

### Part 2: ProcessPayment Method

**What was required:**
```csharp
public string ProcessPayment()
{
    // Step 1: Calculate total
    decimal total = _items.Sum(item => item.Price);

    // Step 2: Create description
    string description = $"Order with {_items.Count} item(s)";

    // Step 3: Delegate to strategy
    return _paymentStrategy.ProcessPayment(total, description);
}
```

**Why this works:**
1. **LINQ Sum:** Efficiently calculates total from all cart items
2. **String Interpolation:** Creates a readable description
3. **Delegation:** The actual payment logic is delegated to the strategy

**Key Insight:**
The `ShoppingCart` handles **cart logic** (calculating total, managing items). The `IPaymentStrategy` handles **payment logic** (how to process the payment). This is **Separation of Concerns**.

---

### Part 3: BankTransferPaymentStrategy

**What was required:**
```csharp
public string ProcessPayment(decimal amount, string description)
{
    return $"Bank transfer of ${amount:F2} from account {_accountNumber}. " +
           $"Reference: {description}";
}
```

**Why this works:**
1. **String Formatting:** `{amount:F2}` formats decimal to 2 decimal places ($299.99)
2. **Includes all required info:** amount, account number, description
3. **Follows the same pattern** as other strategies

**Key Insight:**
Each strategy implements the same interface but provides **different behavior**. This makes them **interchangeable**.

---

## How the Strategy Pattern Works

### Class Diagram

```
┌─────────────────────────┐
│   <<interface>>         │
│   IPaymentStrategy      │
├─────────────────────────┤
│ + ProcessPayment(...)   │
└───────────▲─────────────┘
            │
            │ implements
            │
    ┌───────┼───────┬──────────┐
    │       │       │          │
┌───▼───────┴──┐ ┌──▼────────┐ ┌──▼───────────┐
│ CreditCard   │ │  PayPal   │ │ BankTransfer │
│ Strategy     │ │ Strategy  │ │  Strategy    │
└──────────────┘ └───────────┘ └──────────────┘

┌────────────────────────────┐
│     ShoppingCart           │
├────────────────────────────┤
│ - _paymentStrategy         │ ───────► IPaymentStrategy
│ - _items: List<CartItem>   │
├────────────────────────────┤
│ + AddItem(item)            │
│ + ProcessPayment()         │
└────────────────────────────┘
```

### Sequence Diagram

```
User                ShoppingCart           IPaymentStrategy    CreditCardStrategy
 │                       │                       │                    │
 ├──new ShoppingCart(creditCardStrategy)────────────────────────────►│
 │                       │                       │                    │
 ├──AddItem("Laptop")──►│                       │                    │
 │                       │                       │                    │
 ├──ProcessPayment()────►│                       │                    │
 │                       ├──Sum(items)           │                    │
 │                       │                       │                    │
 │                       ├──ProcessPayment(999.99, "Order...")────────►│
 │                       │                       │                    │
 │                       │◄──"Credit card payment..."─────────────────┤
 │◄──────────────────────┤                       │                    │
```

---

## What Makes This a Good Solution?

### 1. Open/Closed Principle
**Open for extension, closed for modification**

```csharp
// Adding a new payment method doesn't require changing ShoppingCart! ✅
public class BitcoinPaymentStrategy : IPaymentStrategy
{
    public string ProcessPayment(decimal amount, string description)
    {
        return $"Bitcoin payment of ${amount:F2} to address {_walletAddress}";
    }
}

// Just use it:
var cart = new ShoppingCart(new BitcoinPaymentStrategy("1A1zP1eP5Q..."));
```

**Without Strategy Pattern (bad):**
```csharp
// Have to modify ShoppingCart every time! ❌
public void ProcessPayment(string type)
{
    if (type == "CreditCard") { ... }
    else if (type == "PayPal") { ... }
    else if (type == "Bitcoin") { ... } // Added new if-else
}
```

### 2. Single Responsibility Principle
Each class has ONE reason to change:
- `ShoppingCart`: Changes when cart logic changes
- `CreditCardPaymentStrategy`: Changes when credit card processing changes
- `PayPalPaymentStrategy`: Changes when PayPal API changes

### 3. Dependency Inversion Principle
High-level module (`ShoppingCart`) depends on abstraction (`IPaymentStrategy`), not concrete classes.

### 4. Testability
Easy to test with mocks:
```csharp
var mockStrategy = Substitute.For<IPaymentStrategy>();
mockStrategy.ProcessPayment(Arg.Any<decimal>(), Arg.Any<string>())
    .Returns("Mock payment processed");

var cart = new ShoppingCart(mockStrategy);
```

---

## Common Mistakes

### Mistake 1: Storing strategy as concrete type
```csharp
// ❌ BAD
private readonly CreditCardPaymentStrategy _paymentStrategy;

// ✅ GOOD
private readonly IPaymentStrategy _paymentStrategy;
```

### Mistake 2: Not using readonly
```csharp
// ❌ BAD - strategy can be changed after construction
private IPaymentStrategy _paymentStrategy;

// ✅ GOOD - strategy is immutable
private readonly IPaymentStrategy _paymentStrategy;
```

### Mistake 3: Having default constructor
```csharp
// ❌ BAD - allows creating cart without strategy
public ShoppingCart() { }

// ✅ GOOD - forces strategy to be provided
public ShoppingCart(IPaymentStrategy paymentStrategy) { ... }
```

### Mistake 4: Duplicating logic across strategies
```csharp
// ❌ BAD - validation repeated in every strategy
public class CreditCardPaymentStrategy : IPaymentStrategy
{
    public string ProcessPayment(decimal amount, string description)
    {
        if (amount <= 0) throw new Exception("Invalid amount"); // Duplicated!
        // ...
    }
}

// ✅ BETTER - validate in ShoppingCart (context)
public class ShoppingCart
{
    public string ProcessPayment()
    {
        if (_items.Count == 0) throw new Exception("Cart is empty");
        // ...
    }
}
```

---

## Real-World Applications

### 1. E-Commerce Platforms
- Amazon, eBay: Multiple payment methods (credit card, PayPal, gift cards, cryptocurrency)
- Each payment method is a strategy
- Can add new payment methods without changing core checkout logic

### 2. Compression Tools
```csharp
ICompressionStrategy compressionStrategy = format switch
{
    "zip" => new ZipCompressionStrategy(),
    "rar" => new RarCompressionStrategy(),
    "7z" => new SevenZipCompressionStrategy(),
    _ => throw new NotSupportedException()
};

var compressor = new FileCompressor(compressionStrategy);
compressor.Compress(files);
```

### 3. Logging Systems
```csharp
ILogStrategy logStrategy = environment switch
{
    "Development" => new ConsoleLogStrategy(),
    "Staging" => new FileLogStrategy(),
    "Production" => new DatabaseLogStrategy(),
    _ => new NullLogStrategy()
};

var logger = new Logger(logStrategy);
logger.Log("Application started");
```

### 4. Sorting Algorithms
```csharp
ISortStrategy sortStrategy = dataSize switch
{
    < 10 => new InsertionSortStrategy(),
    < 1000 => new QuickSortStrategy(),
    _ => new MergeSortStrategy()
};

var sorter = new DataSorter(sortStrategy);
sorter.Sort(data);
```

---

## Alternative Approaches

### Approach 1: Using Func<> (Functional Approach)
```csharp
public class ShoppingCart
{
    private readonly Func<decimal, string, string> _processPayment;

    public ShoppingCart(Func<decimal, string, string> processPayment)
    {
        _processPayment = processPayment;
    }

    public string ProcessPayment()
    {
        decimal total = _items.Sum(item => item.Price);
        return _processPayment(total, $"Order with {_items.Count} item(s)");
    }
}

// Usage:
var cart = new ShoppingCart((amount, desc) =>
    $"Credit card payment of ${amount:F2}. {desc}");
```

**Pros:** More concise, no need for separate classes
**Cons:** Harder to maintain, no encapsulation of strategy data

### Approach 2: Using Enum/Switch (Not Recommended)
```csharp
public enum PaymentType { CreditCard, PayPal, BankTransfer }

public class ShoppingCart
{
    private readonly PaymentType _paymentType;

    public string ProcessPayment()
    {
        return _paymentType switch
        {
            PaymentType.CreditCard => ProcessCreditCard(),
            PaymentType.PayPal => ProcessPayPal(),
            PaymentType.BankTransfer => ProcessBankTransfer(),
            _ => throw new NotSupportedException()
        };
    }
}
```

**Pros:** Simple for small number of strategies
**Cons:** Violates Open/Closed Principle, hard to extend, not testable

---

## Verification

Run tests to verify your solution:
```bash
dotnet test

# Expected output:
# Passed! - Failed:     0, Passed:     8, Skipped:     0, Total:     8
```

If all tests pass, congratulations! You've successfully implemented the Strategy pattern! 🎉

---

## Next Steps

1. Try the bonus challenges in `INSTRUCTIONS.md`
2. Move to the next exercise: Factory Pattern
3. Read the Gang of Four "Design Patterns" book
4. Apply Strategy pattern in your own projects

---

## Key Takeaways

✅ **Strategy pattern** separates algorithms from the context that uses them
✅ **Interfaces** enable runtime polymorphism
✅ **Dependency injection** makes code flexible and testable
✅ **Open/Closed Principle** allows extending behavior without modifying existing code
✅ **Real-world use cases** include payments, compression, sorting, logging

**Remember:** Design patterns are tools, not rules. Use them when they solve a problem, not because they're "best practice".

---

**Congratulations on completing this exercise! 🎉**
