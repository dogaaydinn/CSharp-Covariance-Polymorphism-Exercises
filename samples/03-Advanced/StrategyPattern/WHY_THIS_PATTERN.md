# Why Strategy Pattern? A Deep Dive

This document explores the Strategy Pattern in depth, covering the problems it solves, when to use it, real-world scenarios, and advanced techniques.

## Table of Contents

1. [The Problem](#1-the-problem)
2. [The Solution](#2-the-solution)
3. [When to Use Strategy Pattern](#3-when-to-use-strategy-pattern)
4. [When NOT to Use Strategy Pattern](#4-when-not-to-use-strategy-pattern)
5. [Real-World Scenarios](#5-real-world-scenarios)
6. [Performance Analysis](#6-performance-analysis)
7. [Common Mistakes](#7-common-mistakes)
8. [Migration Strategy](#8-migration-strategy)
9. [Comparison with Other Patterns](#9-comparison-with-other-patterns)
10. [Advanced Techniques](#10-advanced-techniques)
11. [Conclusion](#11-conclusion)

---

## 1. The Problem

### The Nightmare: Conditional Complexity

Imagine you're building a payment system. Your first implementation might look like this:

```csharp
public class PaymentProcessor
{
    public decimal ProcessPayment(string paymentMethod, decimal amount, Dictionary<string, string> config)
    {
        if (paymentMethod == "CreditCard")
        {
            // Validate credit card
            if (!IsValidCreditCard(config["cardNumber"]))
                throw new Exception("Invalid credit card");

            // Calculate fee
            var fee = amount * 0.029m + 0.30m;

            // Process payment
            Console.WriteLine($"Processing credit card: {config["cardNumber"]}");
            LogTransaction("CreditCard", amount, fee);
            SendReceipt(config["email"], amount + fee);

            return amount + fee;
        }
        else if (paymentMethod == "PayPal")
        {
            // Validate PayPal account
            if (!IsValidEmail(config["email"]))
                throw new Exception("Invalid email");

            // Calculate fee
            var fee = amount * 0.029m + 0.30m;

            // Process payment
            Console.WriteLine($"Processing PayPal: {config["email"]}");
            LogTransaction("PayPal", amount, fee);
            SendReceipt(config["email"], amount + fee);

            return amount + fee;
        }
        else if (paymentMethod == "Crypto")
        {
            // Validate wallet
            if (!IsValidWallet(config["walletAddress"]))
                throw new Exception("Invalid wallet");

            // Calculate fee (different!)
            var fee = amount * 0.005m;

            // Process payment (different logic!)
            Console.WriteLine($"Processing crypto: {config["walletAddress"]}");
            WaitForBlockchainConfirmation();
            LogTransaction("Crypto", amount, fee);
            SendReceipt(config["email"], amount + fee);

            return amount + fee;
        }
        else if (paymentMethod == "BankTransfer")
        {
            // Validate bank account
            if (!IsValidBankAccount(config["accountNumber"]))
                throw new Exception("Invalid bank account");

            // Calculate fee (even more different!)
            var fee = amount > 2000m ? amount * 0.0025m : 5.00m;

            // Process payment (very different logic!)
            Console.WriteLine($"Processing bank transfer: {config["accountNumber"]}");
            InitiateACHTransfer(config["accountNumber"], config["routingNumber"], amount);
            LogTransaction("BankTransfer", amount, fee);
            SendReceipt(config["email"], amount + fee);

            return amount + fee;
        }
        else
        {
            throw new NotSupportedException($"Payment method '{paymentMethod}' not supported");
        }
    }
}
```

### Problems with This Approach

#### Problem 1: Violates Open/Closed Principle

```csharp
// ❌ Every new payment method requires modifying this class
public decimal ProcessPayment(string paymentMethod, ...)
{
    if (paymentMethod == "CreditCard") { ... }
    else if (paymentMethod == "PayPal") { ... }
    else if (paymentMethod == "Crypto") { ... }
    else if (paymentMethod == "ApplePay") { ... } // ← New method requires code change
    else if (paymentMethod == "GooglePay") { ... } // ← Another change
    else if (paymentMethod == "Venmo") { ... } // ← More changes
    // This class keeps growing forever!
}
```

**Impact**:
- Every new payment method requires modifying existing, tested code
- Risk of breaking existing functionality
- Merge conflicts in teams
- Class grows unbounded

#### Problem 2: Violates Single Responsibility Principle

```csharp
// ❌ One class doing too many things
public class PaymentProcessor
{
    // Responsibility 1: Credit card processing
    private bool IsValidCreditCard(string cardNumber) { ... }
    private void ProcessCreditCard(decimal amount) { ... }

    // Responsibility 2: PayPal processing
    private bool IsValidEmail(string email) { ... }
    private void ProcessPayPal(decimal amount) { ... }

    // Responsibility 3: Crypto processing
    private bool IsValidWallet(string wallet) { ... }
    private void ProcessCrypto(decimal amount) { ... }
    private void WaitForBlockchainConfirmation() { ... }

    // Responsibility 4: Bank transfer processing
    private bool IsValidBankAccount(string account) { ... }
    private void InitiateACHTransfer(...) { ... }

    // This class has 4+ reasons to change!
}
```

**Impact**:
- Hard to understand (400+ lines of code in one class)
- Hard to test (must mock all payment methods)
- Team conflicts (multiple developers editing same file)
- Coupling (changing credit card logic might break PayPal)

#### Problem 3: String-Based Selection

```csharp
// ❌ No compile-time safety
ProcessPayment("CreditCard", 100m, config);
ProcessPayment("Credit Card", 100m, config); // Typo! Runtime error
ProcessPayment("creditcard", 100m, config);  // Case sensitivity! Runtime error
ProcessPayment("CC", 100m, config);          // Abbreviation! Runtime error
```

**Impact**:
- No IntelliSense support
- No compile-time checking
- Errors discovered at runtime
- Hard to refactor (find all string references)

#### Problem 4: Difficult to Test

```csharp
// ❌ Testing one payment method requires setting up entire class
[Fact]
public void CreditCard_CalculatesFee_Correctly()
{
    var processor = new PaymentProcessor(); // Must create entire processor
    var config = new Dictionary<string, string>
    {
        ["cardNumber"] = "1234-5678-9012-3456",
        ["email"] = "test@example.com"
    };

    var total = processor.ProcessPayment("CreditCard", 100m, config);

    Assert.Equal(103.20m, total);

    // Problems:
    // - Can't test credit card in isolation
    // - Must provide config for all payment methods
    // - Hard to mock dependencies
    // - Test setup is complex
}
```

#### Problem 5: Cannot Change at Runtime Elegantly

```csharp
// ❌ Awkward runtime switching
var paymentMethod = "CreditCard";
var total1 = processor.ProcessPayment(paymentMethod, amount, creditCardConfig);

// User changes mind
paymentMethod = "PayPal"; // Just a string change
var total2 = processor.ProcessPayment(paymentMethod, amount, payPalConfig);

// Problems:
// - No type safety
// - Easy to pass wrong config for payment method
// - No IDE support
// - String comparisons everywhere
```

#### Problem 6: Code Duplication

```csharp
// ❌ Same logic repeated everywhere
if (paymentMethod == "CreditCard")
{
    var fee = amount * 0.029m + 0.30m;
    LogTransaction("CreditCard", amount, fee);
    SendReceipt(config["email"], amount + fee);
    return amount + fee;
}
else if (paymentMethod == "PayPal")
{
    var fee = amount * 0.029m + 0.30m; // ← Duplicate
    LogTransaction("PayPal", amount, fee); // ← Duplicate
    SendReceipt(config["email"], amount + fee); // ← Duplicate
    return amount + fee; // ← Duplicate
}
```

### The Pain Points in Real Projects

1. **Maintenance Nightmare**: "I need to change credit card processing, but I'm afraid to touch this 1000-line method"
2. **Testing Hell**: "I need to test just PayPal, but I have to set up the entire payment processor"
3. **Merge Conflicts**: "Three developers are all editing PaymentProcessor.cs"
4. **No Extension**: "Adding Apple Pay requires modifying 5 different places in the code"
5. **Bug Propagation**: "I fixed a bug in crypto processing, but I accidentally broke bank transfers"

---

## 2. The Solution

The Strategy Pattern elegantly solves all these problems by:
1. **Encapsulating each algorithm** (payment method) in its own class
2. **Making them interchangeable** through a common interface
3. **Enabling runtime selection** without string comparisons
4. **Following SOLID principles** perfectly

### Before and After Comparison

#### Before: Monolithic If-Else

```csharp
// ❌ 400 lines of if-else spaghetti
public class PaymentProcessor
{
    public decimal ProcessPayment(string method, decimal amount, Dictionary<string, string> config)
    {
        if (method == "CreditCard") { /* 100 lines */ }
        else if (method == "PayPal") { /* 100 lines */ }
        else if (method == "Crypto") { /* 100 lines */ }
        else if (method == "BankTransfer") { /* 100 lines */ }
    }
}
```

#### After: Strategy Pattern

```csharp
// ✅ Clean interface
public interface IPaymentStrategy
{
    string Name { get; }
    PaymentResult ProcessPayment(decimal amount);
    decimal CalculateFee(decimal amount);
    bool ValidatePayment(decimal amount);
}

// ✅ Each strategy in its own class (25 lines each)
public class CreditCardPaymentStrategy : IPaymentStrategy
{
    private readonly string _cardNumber;
    private readonly string _cardHolder;

    public CreditCardPaymentStrategy(string cardNumber, string cardHolder)
    {
        _cardNumber = cardNumber;
        _cardHolder = cardHolder;
    }

    public string Name => "Credit Card";

    public PaymentResult ProcessPayment(decimal amount)
    {
        // Only credit card logic here
        var fee = CalculateFee(amount);
        // Process...
        return new PaymentResult { /* ... */ };
    }

    public decimal CalculateFee(decimal amount) => amount * 0.029m + 0.30m;

    public bool ValidatePayment(decimal amount) => amount > 0 && amount <= 50000m;
}

// ✅ Context for using strategies
public class PaymentContext
{
    private IPaymentStrategy _strategy;

    public void SetStrategy(IPaymentStrategy strategy)
    {
        _strategy = strategy ?? throw new ArgumentNullException(nameof(strategy));
    }

    public PaymentResult ProcessPayment(decimal amount)
    {
        return _strategy.ProcessPayment(amount);
    }
}
```

### How It Solves Each Problem

#### ✅ Solution 1: Open/Closed Principle

```csharp
// ✅ Adding new payment method: Create new class, no modifications
public class ApplePayStrategy : IPaymentStrategy
{
    public string Name => "Apple Pay";

    public PaymentResult ProcessPayment(decimal amount)
    {
        // Apple Pay specific logic
        return new PaymentResult { /* ... */ };
    }

    public decimal CalculateFee(decimal amount) => amount * 0.025m;

    public bool ValidatePayment(decimal amount) => amount > 0;
}

// Use it immediately, no existing code changed
var applePay = new ApplePayStrategy();
context.SetStrategy(applePay);
context.ProcessPayment(100m);
```

#### ✅ Solution 2: Single Responsibility Principle

```csharp
// ✅ Each class has ONE reason to change
public class CreditCardPaymentStrategy : IPaymentStrategy
{
    // Only changes if credit card processing changes
}

public class PayPalPaymentStrategy : IPaymentStrategy
{
    // Only changes if PayPal processing changes
}

public class CryptoPaymentStrategy : IPaymentStrategy
{
    // Only changes if crypto processing changes
}
```

#### ✅ Solution 3: Type-Safe Selection

```csharp
// ✅ Compile-time type checking
var strategy = new CreditCardPaymentStrategy("1234-5678-9012-3456", "John Doe");
context.SetStrategy(strategy); // ← Type-safe, IntelliSense works

// vs

// ❌ Runtime string checking
context.ProcessPayment("CreditCard", ...); // ← No type safety
```

#### ✅ Solution 4: Easy Testing

```csharp
// ✅ Test each strategy in isolation
[Fact]
public void CreditCard_CalculatesFee_Correctly()
{
    // Arrange: Only need the strategy
    var strategy = new CreditCardPaymentStrategy("1234-5678-9012-3456", "Test");

    // Act: Direct call, no setup
    var fee = strategy.CalculateFee(100m);

    // Assert: Simple and focused
    Assert.Equal(3.20m, fee);
}

// ✅ Mock strategies easily
[Fact]
public void Context_ProcessesPayment_WithStrategy()
{
    var mockStrategy = new Mock<IPaymentStrategy>();
    mockStrategy.Setup(s => s.ProcessPayment(100m))
                .Returns(new PaymentResult { Success = true });

    var context = new PaymentContext();
    context.SetStrategy(mockStrategy.Object);

    var result = context.ProcessPayment(100m);

    Assert.True(result.Success);
}
```

#### ✅ Solution 5: Elegant Runtime Switching

```csharp
// ✅ Type-safe, clear, refactorable
var context = new PaymentContext();

// Start with credit card
var creditCard = new CreditCardPaymentStrategy("****-1111", "Alice");
context.SetStrategy(creditCard);
var result1 = context.ProcessPayment(250m);

// Switch to crypto
var crypto = new CryptoPaymentStrategy("0x123...abc");
context.SetStrategy(crypto);
var result2 = context.ProcessPayment(250m);

Console.WriteLine($"Saved: ${result1.Fee - result2.Fee:F2}");
```

#### ✅ Solution 6: No Duplication

```csharp
// ✅ Common behavior in base class or decorator
public abstract class BasePaymentStrategy : IPaymentStrategy
{
    public abstract string Name { get; }
    public abstract PaymentResult ProcessPayment(decimal amount);
    public abstract decimal CalculateFee(decimal amount);

    public virtual bool ValidatePayment(decimal amount)
    {
        return amount > 0; // Common validation
    }
}

// Or use decorator for cross-cutting concerns
public class LoggingPaymentStrategy : PaymentStrategyDecorator
{
    public override PaymentResult ProcessPayment(decimal amount)
    {
        Log($"Processing {Name} payment for ${amount}");
        var result = base.ProcessPayment(amount);
        Log($"Result: {result.Success}");
        return result;
    }
}
```

---

## 3. When to Use Strategy Pattern

### Perfect Use Cases

#### 1. Multiple Algorithms for the Same Task

```csharp
// Sorting strategies
public interface ISortingStrategy<T>
{
    void Sort(T[] array);
}

public class QuickSortStrategy<T> : ISortingStrategy<T> where T : IComparable<T>
{
    public void Sort(T[] array) { /* QuickSort implementation */ }
}

public class MergeSortStrategy<T> : ISortingStrategy<T> where T : IComparable<T>
{
    public void Sort(T[] array) { /* MergeSort implementation */ }
}

// Use different strategy based on array size
var strategy = array.Length < 10
    ? new InsertionSortStrategy<int>()
    : new QuickSortStrategy<int>();
```

#### 2. Conditional Complexity Elimination

```csharp
// Before: Complex if-else
public decimal CalculateShippingCost(string method, decimal weight, string destination)
{
    if (method == "Standard")
    {
        if (destination == "Domestic")
            return weight * 5m;
        else
            return weight * 15m;
    }
    else if (method == "Express")
    {
        if (destination == "Domestic")
            return weight * 10m + 20m;
        else
            return weight * 30m + 50m;
    }
    // ... more complexity
}

// After: Strategy pattern
public interface IShippingStrategy
{
    decimal CalculateCost(decimal weight, string destination);
}

public class StandardShippingStrategy : IShippingStrategy
{
    public decimal CalculateCost(decimal weight, string destination)
    {
        return destination == "Domestic" ? weight * 5m : weight * 15m;
    }
}

public class ExpressShippingStrategy : IShippingStrategy
{
    public decimal CalculateCost(decimal weight, string destination)
    {
        return destination == "Domestic" ? weight * 10m + 20m : weight * 30m + 50m;
    }
}
```

#### 3. Runtime Behavior Selection

```csharp
// Game AI behavior changes based on player proximity
public interface IAIBehaviorStrategy
{
    void Execute(Character npc, Character player);
}

public class AggressiveBehavior : IAIBehaviorStrategy
{
    public void Execute(Character npc, Character player)
    {
        npc.MoveTo(player.Position);
        npc.Attack(player);
    }
}

public class DefensiveBehavior : IAIBehaviorStrategy
{
    public void Execute(Character npc, Character player)
    {
        if (npc.Health < 30)
            npc.Flee();
        else
            npc.Block();
    }
}

// Switch behavior at runtime
var distance = Vector3.Distance(npc.Position, player.Position);
npc.SetBehavior(distance < 10 ? new AggressiveBehavior() : new PatrolBehavior());
```

#### 4. Plugin/Extension Architecture

```csharp
// Allow third-party payment providers
public interface IPaymentProvider
{
    string ProviderName { get; }
    PaymentResult Process(decimal amount);
}

// First-party providers
public class StripeProvider : IPaymentProvider { }
public class PayPalProvider : IPaymentProvider { }

// Third-party provider can be added
public class CustomPaymentGateway : IPaymentProvider
{
    // Custom implementation
}

// Register all providers
var providers = new List<IPaymentProvider>
{
    new StripeProvider(),
    new PayPalProvider(),
    new CustomPaymentGateway() // ← Third-party extension
};
```

### Indicators That You Need Strategy Pattern

1. **You have a method with multiple if-else or switch cases** based on a type/enum
2. **Each branch has significantly different logic** (not just simple value differences)
3. **You anticipate adding more cases** in the future
4. **You want to test each case independently**
5. **Different clients need different implementations**
6. **The algorithm needs to be selected at runtime**

### Example Decision Tree

```
Do you have multiple ways to do the same thing?
├─ No → Don't use Strategy Pattern
└─ Yes → Do the implementations differ significantly?
    ├─ No (just different values) → Use configuration/parameters
    └─ Yes → Will you add more implementations?
        ├─ Unlikely → Simple if-else might be fine
        └─ Likely → Will they be selected at runtime?
            ├─ No → Consider Factory Pattern
            └─ Yes → ✅ USE STRATEGY PATTERN
```

---

## 4. When NOT to Use Strategy Pattern

### Anti-Pattern: Over-Engineering Simple Logic

```csharp
// ❌ DON'T: Too simple for strategy pattern
public interface IAdditionStrategy
{
    int Add(int a, int b);
}

public class SimpleAdditionStrategy : IAdditionStrategy
{
    public int Add(int a, int b) => a + b;
}

// ✅ DO: Just use a method
public static class MathHelper
{
    public static int Add(int a, int b) => a + b;
}
```

**Why**: The strategy pattern adds unnecessary complexity for trivial operations.

### Anti-Pattern: Value-Only Differences

```csharp
// ❌ DON'T: Creating strategies for configuration
public interface IDiscountStrategy
{
    decimal Percentage { get; }
}

public class TenPercentDiscount : IDiscountStrategy
{
    public decimal Percentage => 0.10m;
}

public class TwentyPercentDiscount : IDiscountStrategy
{
    public decimal Percentage => 0.20m;
}

// ✅ DO: Use configuration
public class DiscountCalculator
{
    public decimal CalculateDiscount(decimal amount, decimal percentage)
    {
        return amount * percentage;
    }
}

var discount = calculator.CalculateDiscount(100m, 0.10m);
```

**Why**: Different values don't need different classes, just use parameters.

### Anti-Pattern: Single Implementation

```csharp
// ❌ DON'T: Strategy with only one implementation
public interface IPaymentStrategy
{
    void Process(decimal amount);
}

public class OnlyPaymentStrategy : IPaymentStrategy
{
    public void Process(decimal amount)
    {
        // Only one way to pay
    }
}

// ✅ DO: Direct implementation
public class PaymentProcessor
{
    public void ProcessPayment(decimal amount)
    {
        // Direct implementation
    }
}
```

**Why**: No need for abstraction if there's only one implementation.

### Anti-Pattern: Fixed Algorithm Selection

```csharp
// ❌ DON'T: Strategy when selection is hardcoded
public class ReportGenerator
{
    private readonly IReportStrategy _strategy = new PdfReportStrategy(); // Always PDF

    public void Generate()
    {
        _strategy.GenerateReport();
    }
}

// ✅ DO: Direct implementation
public class PdfReportGenerator
{
    public void GenerateReport()
    {
        // PDF generation
    }
}
```

**Why**: If the strategy never changes, there's no benefit to the pattern.

### When to Prefer Alternatives

| Scenario | Use Instead | Reason |
|----------|-------------|--------|
| Simple value differences | **Configuration/Parameters** | Avoid class explosion |
| Algorithm structure is fixed | **Template Method Pattern** | Inherit and override specific steps |
| State transitions | **State Pattern** | State changes trigger behavior changes |
| Single use | **Direct Implementation** | YAGNI principle |
| Complex object creation | **Factory Pattern** | Separate creation from usage |

---

## 5. Real-World Scenarios

### Scenario 1: E-Commerce Checkout System

**Context**: Online store needs flexible payment processing.

**Implementation**:

```csharp
// Strategy interface
public interface IPaymentStrategy
{
    string Name { get; }
    PaymentResult ProcessPayment(Order order);
    bool IsAvailable(Order order);
    decimal CalculateProcessingFee(decimal amount);
}

// Concrete strategies
public class CreditCardStrategy : IPaymentStrategy
{
    public string Name => "Credit Card";

    public bool IsAvailable(Order order)
    {
        // Credit cards available worldwide
        return true;
    }

    public decimal CalculateProcessingFee(decimal amount)
    {
        return amount * 0.029m + 0.30m; // Stripe-like fees
    }

    public PaymentResult ProcessPayment(Order order)
    {
        // Stripe API integration
        var stripeClient = new StripeClient(_apiKey);
        var charge = stripeClient.CreateCharge(order.Total);
        return new PaymentResult
        {
            Success = charge.Status == "succeeded",
            TransactionId = charge.Id,
            Fee = CalculateProcessingFee(order.Total)
        };
    }
}

public class PayPalStrategy : IPaymentStrategy
{
    public string Name => "PayPal";

    public bool IsAvailable(Order order)
    {
        // PayPal not available in all countries
        var restrictedCountries = new[] { "CU", "IR", "KP", "SD", "SY" };
        return !restrictedCountries.Contains(order.ShippingAddress.CountryCode);
    }

    public decimal CalculateProcessingFee(decimal amount)
    {
        return amount * 0.029m + 0.30m;
    }

    public PaymentResult ProcessPayment(Order order)
    {
        // PayPal SDK integration
        var paypalClient = new PayPalHttpClient(_environment);
        var request = new OrdersCreateRequest();
        // ... PayPal specific code
    }
}

public class CryptoStrategy : IPaymentStrategy
{
    public string Name => "Cryptocurrency (Bitcoin)";

    public bool IsAvailable(Order order)
    {
        // Only for orders > $100 (gas fees)
        return order.Total > 100m;
    }

    public decimal CalculateProcessingFee(decimal amount)
    {
        return amount * 0.005m; // 0.5% - much lower
    }

    public PaymentResult ProcessPayment(Order order)
    {
        // Bitcoin payment processor integration
        var bitcoinClient = new BitPayClient(_apiKey);
        var invoice = bitcoinClient.CreateInvoice(order.Total, "USD");
        return new PaymentResult
        {
            Success = true,
            TransactionId = invoice.Id,
            PaymentUrl = invoice.Url, // User redirected here
            Fee = CalculateProcessingFee(order.Total)
        };
    }
}

// Usage in checkout controller
public class CheckoutController : Controller
{
    [HttpPost]
    public IActionResult ProcessPayment(int orderId, string paymentMethod)
    {
        var order = _orderService.GetOrder(orderId);

        // Select strategy based on user choice
        IPaymentStrategy strategy = paymentMethod switch
        {
            "creditcard" => new CreditCardStrategy(),
            "paypal" => new PayPalStrategy(),
            "crypto" => new CryptoStrategy(),
            _ => throw new NotSupportedException()
        };

        // Validate availability
        if (!strategy.IsAvailable(order))
        {
            return BadRequest($"{strategy.Name} is not available for this order");
        }

        // Show fee to user
        var fee = strategy.CalculateProcessingFee(order.Total);
        ViewBag.ProcessingFee = fee;

        // Process payment
        var result = strategy.ProcessPayment(order);

        if (result.Success)
        {
            _orderService.MarkAsPaid(orderId, result.TransactionId);
            return RedirectToAction("OrderConfirmation", new { orderId });
        }
        else
        {
            return View("PaymentFailed", result);
        }
    }
}
```

**Benefits**:
- Easy to add new payment providers (Apple Pay, Google Pay, etc.)
- Each provider has isolated code (Stripe changes don't affect PayPal)
- Testing each provider independently
- User can compare fees before selecting

---

### Scenario 2: Data Export System

**Context**: Application needs to export data in multiple formats.

**Implementation**:

```csharp
public interface IExportStrategy<T>
{
    string FileExtension { get; }
    string ContentType { get; }
    byte[] Export(IEnumerable<T> data);
}

public class CsvExportStrategy<T> : IExportStrategy<T>
{
    public string FileExtension => ".csv";
    public string ContentType => "text/csv";

    public byte[] Export(IEnumerable<T> data)
    {
        var csv = new StringBuilder();
        var properties = typeof(T).GetProperties();

        // Header
        csv.AppendLine(string.Join(",", properties.Select(p => p.Name)));

        // Rows
        foreach (var item in data)
        {
            var values = properties.Select(p => p.GetValue(item)?.ToString() ?? "");
            csv.AppendLine(string.Join(",", values));
        }

        return Encoding.UTF8.GetBytes(csv.ToString());
    }
}

public class JsonExportStrategy<T> : IExportStrategy<T>
{
    public string FileExtension => ".json";
    public string ContentType => "application/json";

    public byte[] Export(IEnumerable<T> data)
    {
        var json = JsonSerializer.Serialize(data, new JsonSerializerOptions
        {
            WriteIndented = true
        });
        return Encoding.UTF8.GetBytes(json);
    }
}

public class ExcelExportStrategy<T> : IExportStrategy<T>
{
    public string FileExtension => ".xlsx";
    public string ContentType => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

    public byte[] Export(IEnumerable<T> data)
    {
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Data");

        // Use ClosedXML to generate Excel
        var properties = typeof(T).GetProperties();

        // Headers
        for (int i = 0; i < properties.Length; i++)
        {
            worksheet.Cell(1, i + 1).Value = properties[i].Name;
        }

        // Data
        int row = 2;
        foreach (var item in data)
        {
            for (int i = 0; i < properties.Length; i++)
            {
                worksheet.Cell(row, i + 1).Value = properties[i].GetValue(item)?.ToString();
            }
            row++;
        }

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }
}

// Usage
public class DataController : Controller
{
    [HttpGet]
    public IActionResult ExportUsers(string format = "csv")
    {
        var users = _userService.GetAllUsers();

        IExportStrategy<User> strategy = format.ToLower() switch
        {
            "csv" => new CsvExportStrategy<User>(),
            "json" => new JsonExportStrategy<User>(),
            "excel" => new ExcelExportStrategy<User>(),
            _ => throw new NotSupportedException($"Format '{format}' not supported")
        };

        var data = strategy.Export(users);
        var fileName = $"users_{DateTime.Now:yyyyMMdd}{strategy.FileExtension}";

        return File(data, strategy.ContentType, fileName);
    }
}
```

**Benefits**:
- Adding PDF export is just a new class
- Each format handled independently
- Same interface for all formats
- Easy to test each format

---

### Scenario 3: Notification System

**Context**: Send notifications through multiple channels.

**Implementation**:

```csharp
public interface INotificationStrategy
{
    Task<bool> SendAsync(string recipient, string subject, string message);
    bool IsAvailable();
    int Priority { get; } // Lower = higher priority
}

public class EmailNotificationStrategy : INotificationStrategy
{
    private readonly IEmailService _emailService;
    public int Priority => 1;

    public bool IsAvailable()
    {
        return _emailService.IsConfigured();
    }

    public async Task<bool> SendAsync(string recipient, string subject, string message)
    {
        try
        {
            await _emailService.SendEmailAsync(recipient, subject, message);
            return true;
        }
        catch
        {
            return false;
        }
    }
}

public class SmsNotificationStrategy : INotificationStrategy
{
    private readonly ISmsService _smsService;
    public int Priority => 2;

    public bool IsAvailable()
    {
        return _smsService.IsConfigured() && _smsService.HasCredits();
    }

    public async Task<bool> SendAsync(string recipient, string subject, string message)
    {
        // SMS has character limits
        var smsText = $"{subject}: {message.Substring(0, Math.Min(message.Length, 140))}";

        try
        {
            await _smsService.SendSmsAsync(recipient, smsText);
            return true;
        }
        catch
        {
            return false;
        }
    }
}

public class PushNotificationStrategy : INotificationStrategy
{
    private readonly IPushService _pushService;
    public int Priority => 3;

    public bool IsAvailable()
    {
        return _pushService.IsConfigured();
    }

    public async Task<bool> SendAsync(string recipient, string subject, string message)
    {
        try
        {
            await _pushService.SendPushAsync(recipient, subject, message);
            return true;
        }
        catch
        {
            return false;
        }
    }
}

// Notification sender with fallback
public class NotificationSender
{
    private readonly List<INotificationStrategy> _strategies;

    public NotificationSender(IEnumerable<INotificationStrategy> strategies)
    {
        _strategies = strategies.OrderBy(s => s.Priority).ToList();
    }

    public async Task<bool> SendWithFallbackAsync(string recipient, string subject, string message)
    {
        foreach (var strategy in _strategies)
        {
            if (strategy.IsAvailable())
            {
                var success = await strategy.SendAsync(recipient, subject, message);
                if (success)
                    return true;
            }
        }

        return false; // All strategies failed
    }
}

// Usage
var sender = new NotificationSender(new INotificationStrategy[]
{
    new EmailNotificationStrategy(_emailService),
    new SmsNotificationStrategy(_smsService),
    new PushNotificationStrategy(_pushService)
});

// Try email first, fall back to SMS, then push
await sender.SendWithFallbackAsync(
    recipient: "user@example.com",
    subject: "Order Shipped",
    message: "Your order #12345 has been shipped!"
);
```

**Benefits**:
- Automatic fallback if one channel fails
- Priority-based selection
- Easy to add Slack, Discord, etc.
- Each channel tested independently

---

## 6. Performance Analysis

### Benchmark: Strategy Pattern vs If-Else

**Test Setup**:

```csharp
[MemoryDiagnoser]
public class StrategyPatternBenchmarks
{
    private PaymentContext _context;
    private IPaymentStrategy _creditCardStrategy;
    private LegacyPaymentProcessor _legacyProcessor;

    [GlobalSetup]
    public void Setup()
    {
        _creditCardStrategy = new CreditCardPaymentStrategy("1234-5678-9012-3456", "Test User");
        _context = new PaymentContext();
        _context.SetStrategy(_creditCardStrategy);
        _legacyProcessor = new LegacyPaymentProcessor();
    }

    [Benchmark(Baseline = true)]
    public decimal StrategyPattern()
    {
        var result = _context.ProcessPayment(100m);
        return result.TotalAmount;
    }

    [Benchmark]
    public decimal IfElseChain()
    {
        return _legacyProcessor.ProcessPayment("CreditCard", 100m);
    }
}
```

**Results**:

```
| Method          | Mean      | Error    | StdDev   | Ratio | Gen0   | Allocated |
|---------------- |----------:|---------:|---------:|------:|-------:|----------:|
| StrategyPattern | 1.689 μs  | 0.025 μs | 0.021 μs |  1.00 | 0.0458 |     288 B |
| IfElseChain     | 0.782 μs  | 0.012 μs | 0.010 μs |  0.46 | 0.0191 |     120 B |
```

**Analysis**:

1. **Strategy Pattern is ~2.16x slower**
   - Absolute difference: ~0.9 microseconds per operation
   - For 1 million operations: ~0.9 seconds difference
   - For typical web application (1000 req/min): Negligible

2. **Memory Allocation**
   - Strategy: 288 bytes (PaymentResult object + virtual method dispatch)
   - If-Else: 120 bytes (smaller result object)
   - Difference: 168 bytes per operation

3. **Why the Overhead?**
   - Virtual method dispatch (interface call)
   - Object creation (strategy instances)
   - Indirection through context

### Real-World Impact

```csharp
// Scenario: E-commerce site with 10,000 orders/day

// Daily overhead:
// 10,000 orders * 0.9 μs = 9 ms/day total overhead

// Server resources saved by maintainability:
// - Fewer bugs (easier to test)
// - Faster feature development
// - Less developer time debugging

// Conclusion: Performance overhead is negligible compared to business benefits
```

### When Performance Matters

If you process **millions of operations per second**, consider optimizations:

#### 1. Strategy Pooling

```csharp
// Reuse strategy instances
public class StrategyPool
{
    private static readonly CreditCardPaymentStrategy _creditCardStrategy = new();
    private static readonly PayPalPaymentStrategy _payPalStrategy = new();

    public static IPaymentStrategy GetStrategy(string type)
    {
        return type switch
        {
            "CreditCard" => _creditCardStrategy,
            "PayPal" => _payPalStrategy,
            _ => throw new NotSupportedException()
        };
    }
}
```

#### 2. Struct-Based Strategies (for simple cases)

```csharp
// Value type strategy (no heap allocation)
public struct CreditCardCalculator : IFeeCalculator
{
    public decimal CalculateFee(decimal amount) => amount * 0.029m + 0.30m;
}
```

#### 3. Static Dispatch

```csharp
// For hot paths, consider static dispatch
public static class PaymentStrategies
{
    public static decimal ProcessCreditCard(decimal amount) { /* ... */ }
    public static decimal ProcessPayPal(decimal amount) { /* ... */ }
}
```

---

## 7. Common Mistakes

### Mistake 1: Exposing Strategy Implementation Details

```csharp
// ❌ BAD: Leaking implementation details
public interface IPaymentStrategy
{
    string Name { get; }
    string ApiKey { get; set; } // ← Don't expose this!
    string ApiUrl { get; set; }  // ← Implementation detail
    PaymentResult ProcessPayment(decimal amount);
}

// ✅ GOOD: Hide implementation details
public interface IPaymentStrategy
{
    string Name { get; }
    PaymentResult ProcessPayment(decimal amount);
}

public class CreditCardPaymentStrategy : IPaymentStrategy
{
    private readonly string _apiKey; // ← Private
    private readonly string _apiUrl; // ← Private

    public CreditCardPaymentStrategy(string apiKey, string apiUrl)
    {
        _apiKey = apiKey;
        _apiUrl = apiUrl;
    }
}
```

### Mistake 2: Strategy with Side Effects

```csharp
// ❌ BAD: Strategy modifies global state
public class CreditCardStrategy : IPaymentStrategy
{
    private static int _transactionCount = 0; // ← Shared state!

    public PaymentResult ProcessPayment(decimal amount)
    {
        _transactionCount++; // ← Side effect!
        // ...
    }
}

// ✅ GOOD: Pure strategy, side effects in context
public class CreditCardStrategy : IPaymentStrategy
{
    public PaymentResult ProcessPayment(decimal amount)
    {
        // No side effects, just return result
        return new PaymentResult { /* ... */ };
    }
}

public class PaymentContext
{
    private int _transactionCount = 0;

    public PaymentResult ProcessPayment(decimal amount)
    {
        var result = _strategy.ProcessPayment(amount);
        if (result.Success)
            _transactionCount++; // ← Side effect in context
        return result;
    }
}
```

### Mistake 3: God Strategy Interface

```csharp
// ❌ BAD: Interface trying to do everything
public interface IPaymentStrategy
{
    PaymentResult ProcessPayment(decimal amount);
    void Refund(string transactionId);
    void PartialRefund(string transactionId, decimal amount);
    PaymentStatus CheckStatus(string transactionId);
    void GenerateReport();
    void SendReceipt(string email);
    void ValidateCardNumber(string cardNumber);
    void EncryptData(string data);
    // ... 20 more methods
}

// ✅ GOOD: Focused interface
public interface IPaymentStrategy
{
    PaymentResult ProcessPayment(decimal amount);
    decimal CalculateFee(decimal amount);
    bool ValidatePayment(decimal amount);
}

// Separate interfaces for other concerns
public interface IRefundablePayment
{
    RefundResult ProcessRefund(string transactionId, decimal amount);
}

public interface IPaymentStatusChecker
{
    PaymentStatus GetStatus(string transactionId);
}
```

### Mistake 4: Creating Strategies for Configuration

```csharp
// ❌ BAD: Strategy for every discount percentage
public class TenPercentDiscountStrategy : IDiscountStrategy { }
public class FifteenPercentDiscountStrategy : IDiscountStrategy { }
public class TwentyPercentDiscountStrategy : IDiscountStrategy { }
// ... 50 more strategies

// ✅ GOOD: Parameterized strategy
public class PercentageDiscountStrategy : IDiscountStrategy
{
    private readonly decimal _percentage;

    public PercentageDiscountStrategy(decimal percentage)
    {
        _percentage = percentage;
    }

    public decimal ApplyDiscount(decimal amount)
    {
        return amount * (1 - _percentage);
    }
}

// Usage
var discount10 = new PercentageDiscountStrategy(0.10m);
var discount15 = new PercentageDiscountStrategy(0.15m);
```

### Mistake 5: Not Using Dependency Injection

```csharp
// ❌ BAD: Creating dependencies inside strategy
public class EmailNotificationStrategy : INotificationStrategy
{
    public async Task SendAsync(string recipient, string message)
    {
        var emailService = new SmtpEmailService(); // ← Hard dependency
        await emailService.SendAsync(recipient, message);
    }
}

// ✅ GOOD: Inject dependencies
public class EmailNotificationStrategy : INotificationStrategy
{
    private readonly IEmailService _emailService;

    public EmailNotificationStrategy(IEmailService emailService)
    {
        _emailService = emailService; // ← Injected
    }

    public async Task SendAsync(string recipient, string message)
    {
        await _emailService.SendAsync(recipient, message);
    }
}

// Registration in DI container
services.AddScoped<IEmailService, SmtpEmailService>();
services.AddScoped<INotificationStrategy, EmailNotificationStrategy>();
```

---

## 8. Migration Strategy

### Step 1: Identify the Code Smell

Look for these patterns in your codebase:

```csharp
// Pattern 1: Long if-else chains
if (type == "A") { /* 50 lines */ }
else if (type == "B") { /* 50 lines */ }
else if (type == "C") { /* 50 lines */ }

// Pattern 2: Switch statements with complex logic
switch (algorithm)
{
    case Algorithm.QuickSort:
        /* 30 lines */
        break;
    case Algorithm.MergeSort:
        /* 30 lines */
        break;
}

// Pattern 3: Type checking
if (obj is TypeA typeA)
{
    // TypeA specific logic
}
else if (obj is TypeB typeB)
{
    // TypeB specific logic
}
```

### Step 2: Extract Common Interface

```csharp
// 1. Identify common operations
// Before:
public void ProcessPaymentA(decimal amount) { }
public void ProcessPaymentB(decimal amount) { }
public void ProcessPaymentC(decimal amount) { }

// 2. Define interface
public interface IPaymentStrategy
{
    PaymentResult ProcessPayment(decimal amount);
}
```

### Step 3: Create Concrete Strategies

```csharp
// Move each if-else branch to its own class
public class PaymentStrategyA : IPaymentStrategy
{
    public PaymentResult ProcessPayment(decimal amount)
    {
        // Move code from if (type == "A") block here
        return new PaymentResult { /* ... */ };
    }
}

public class PaymentStrategyB : IPaymentStrategy
{
    public PaymentResult ProcessPayment(decimal amount)
    {
        // Move code from else if (type == "B") block here
        return new PaymentResult { /* ... */ };
    }
}
```

### Step 4: Refactor Client Code

```csharp
// Before
public class PaymentProcessor
{
    public decimal ProcessPayment(string type, decimal amount)
    {
        if (type == "A")
        {
            // 50 lines of code
        }
        else if (type == "B")
        {
            // 50 lines of code
        }
    }
}

// After
public class PaymentProcessor
{
    private readonly Dictionary<string, IPaymentStrategy> _strategies;

    public PaymentProcessor()
    {
        _strategies = new Dictionary<string, IPaymentStrategy>
        {
            ["A"] = new PaymentStrategyA(),
            ["B"] = new PaymentStrategyB()
        };
    }

    public decimal ProcessPayment(string type, decimal amount)
    {
        if (!_strategies.ContainsKey(type))
            throw new NotSupportedException($"Payment type '{type}' not supported");

        var strategy = _strategies[type];
        var result = strategy.ProcessPayment(amount);
        return result.TotalAmount;
    }
}
```

### Step 5: Gradual Migration

```csharp
// Support both old and new code during transition
public class PaymentProcessor
{
    private readonly Dictionary<string, IPaymentStrategy> _strategies;

    public decimal ProcessPayment(string type, decimal amount)
    {
        // New way (if strategy exists)
        if (_strategies.ContainsKey(type))
        {
            var strategy = _strategies[type];
            var result = strategy.ProcessPayment(amount);
            return result.TotalAmount;
        }

        // Old way (fallback during migration)
        return ProcessPaymentLegacy(type, amount);
    }

    [Obsolete("Use strategies instead")]
    private decimal ProcessPaymentLegacy(string type, decimal amount)
    {
        // Keep old if-else logic temporarily
        if (type == "C") { /* legacy code */ }
        else if (type == "D") { /* legacy code */ }
        throw new NotSupportedException();
    }
}
```

### Step 6: Add Tests

```csharp
// Test each strategy independently
public class PaymentStrategyATests
{
    [Fact]
    public void ProcessPayment_WithValidAmount_ReturnsSuccess()
    {
        // Arrange
        var strategy = new PaymentStrategyA();

        // Act
        var result = strategy.ProcessPayment(100m);

        // Assert
        Assert.True(result.Success);
        Assert.Equal(100m, result.Amount);
    }
}
```

### Step 7: Remove Legacy Code

```csharp
// Once all types migrated to strategies, delete legacy method
public class PaymentProcessor
{
    private readonly Dictionary<string, IPaymentStrategy> _strategies;

    public decimal ProcessPayment(string type, decimal amount)
    {
        if (!_strategies.ContainsKey(type))
            throw new NotSupportedException($"Payment type '{type}' not supported");

        var strategy = _strategies[type];
        var result = strategy.ProcessPayment(amount);
        return result.TotalAmount;
    }

    // ProcessPaymentLegacy() deleted!
}
```

---

## 9. Comparison with Other Patterns

### Strategy vs Template Method

| Aspect | Strategy Pattern | Template Method Pattern |
|--------|------------------|-------------------------|
| **Inheritance** | Uses composition | Uses inheritance |
| **Flexibility** | Change entire algorithm | Change specific steps |
| **Runtime** | Can change at runtime | Fixed at compile-time |
| **Complexity** | More classes | Fewer classes |

**Strategy Example**:
```csharp
public interface ISortingStrategy<T>
{
    void Sort(T[] array);
}

public class QuickSort<T> : ISortingStrategy<T>
{
    public void Sort(T[] array)
    {
        // Entire algorithm here
    }
}

var sorter = new Sorter<int>(new QuickSort<int>());
sorter.ChangeStrategy(new MergeSort<int>()); // ← Can change
```

**Template Method Example**:
```csharp
public abstract class SortingAlgorithm<T>
{
    public void Sort(T[] array) // Template method
    {
        Partition(array);
        QuickSortRecursive(array);
        Merge(array);
    }

    protected abstract void Partition(T[] array); // Hook
    protected abstract void Merge(T[] array); // Hook
}

public class QuickSort<T> : SortingAlgorithm<T>
{
    protected override void Partition(T[] array) { }
    protected override void Merge(T[] array) { }
}
```

**When to Use**:
- **Strategy**: Need to swap entire algorithm at runtime
- **Template Method**: Algorithm structure is fixed, vary specific steps

---

### Strategy vs State

| Aspect | Strategy Pattern | State Pattern |
|--------|------------------|---------------|
| **Purpose** | Interchangeable algorithms | State-dependent behavior |
| **Changes** | Client changes strategy | Object changes its own state |
| **Knowledge** | Strategies independent | States know about transitions |

**Strategy Example**:
```csharp
// Client explicitly selects strategy
var context = new PaymentContext();
context.SetStrategy(new CreditCardStrategy()); // ← Client decides
context.ProcessPayment(100m);
context.SetStrategy(new PayPalStrategy()); // ← Client changes
```

**State Example**:
```csharp
// Object changes its own state
public class Order
{
    private IOrderState _state = new PendingState();

    public void ConfirmPayment()
    {
        _state = _state.ConfirmPayment(this); // ← State transitions itself
    }

    public void Ship()
    {
        _state = _state.Ship(this); // ← State knows next state
    }
}

public interface IOrderState
{
    IOrderState ConfirmPayment(Order order);
    IOrderState Ship(Order order);
}

public class PendingState : IOrderState
{
    public IOrderState ConfirmPayment(Order order)
    {
        return new PaidState(); // ← Knows next state
    }
}
```

**When to Use**:
- **Strategy**: Client controls which algorithm to use
- **State**: Object's behavior changes based on internal state

---

### Strategy vs Factory

| Aspect | Strategy Pattern | Factory Pattern |
|--------|------------------|-----------------|
| **Purpose** | Behavior variation | Object creation |
| **Focus** | How to execute | What to create |
| **Client** | Uses created objects | Gets instances |

**Often Used Together**:
```csharp
// Factory creates strategies
public class PaymentStrategyFactory
{
    public IPaymentStrategy CreateStrategy(string type, Dictionary<string, string> config)
    {
        return type.ToLower() switch
        {
            "creditcard" => new CreditCardStrategy(config["cardNumber"], config["holder"]),
            "paypal" => new PayPalStrategy(config["email"]),
            "crypto" => new CryptoStrategy(config["wallet"]),
            _ => throw new NotSupportedException()
        };
    }
}

// Usage
var factory = new PaymentStrategyFactory();
var strategy = factory.CreateStrategy("creditcard", config);
var context = new PaymentContext();
context.SetStrategy(strategy);
```

---

## 10. Advanced Techniques

### Technique 1: Strategy Registry

```csharp
public class StrategyRegistry<TKey, TStrategy> where TStrategy : class
{
    private readonly Dictionary<TKey, Func<TStrategy>> _factories = new();

    public void Register(TKey key, Func<TStrategy> factory)
    {
        _factories[key] = factory;
    }

    public TStrategy Get(TKey key)
    {
        if (!_factories.TryGetValue(key, out var factory))
            throw new KeyNotFoundException($"Strategy for '{key}' not found");

        return factory();
    }

    public IEnumerable<TKey> GetAvailableKeys() => _factories.Keys;
}

// Usage
var registry = new StrategyRegistry<string, IPaymentStrategy>();
registry.Register("creditcard", () => new CreditCardStrategy());
registry.Register("paypal", () => new PayPalStrategy());

var strategy = registry.Get("creditcard");
```

### Technique 2: Composite Strategy

```csharp
// Execute multiple strategies in sequence
public class CompositePaymentStrategy : IPaymentStrategy
{
    private readonly List<IPaymentStrategy> _strategies = new();

    public void AddStrategy(IPaymentStrategy strategy)
    {
        _strategies.Add(strategy);
    }

    public PaymentResult ProcessPayment(decimal amount)
    {
        var results = new List<PaymentResult>();

        foreach (var strategy in _strategies)
        {
            var result = strategy.ProcessPayment(amount);
            results.Add(result);

            if (!result.Success)
                break; // Stop on first failure
        }

        return new PaymentResult
        {
            Success = results.All(r => r.Success),
            Message = string.Join("; ", results.Select(r => r.Message))
        };
    }
}

// Usage: Split payment across multiple cards
var composite = new CompositePaymentStrategy();
composite.AddStrategy(new CreditCardStrategy("card1", "User1"));
composite.AddStrategy(new CreditCardStrategy("card2", "User1"));
composite.ProcessPayment(500m); // $250 on each card
```

### Technique 3: Lazy Strategy Loading

```csharp
public class LazyPaymentStrategy : IPaymentStrategy
{
    private readonly Lazy<IPaymentStrategy> _lazyStrategy;

    public LazyPaymentStrategy(Func<IPaymentStrategy> factory)
    {
        _lazyStrategy = new Lazy<IPaymentStrategy>(factory);
    }

    public PaymentResult ProcessPayment(decimal amount)
    {
        // Strategy only created when first used
        return _lazyStrategy.Value.ProcessPayment(amount);
    }
}

// Usage: Defer expensive initialization
var strategy = new LazyPaymentStrategy(() =>
{
    // This only runs when first payment is processed
    var apiKey = LoadApiKeyFromVault();
    return new CreditCardStrategy(apiKey);
});
```

### Technique 4: Async Strategy Pattern

```csharp
public interface IAsyncPaymentStrategy
{
    Task<PaymentResult> ProcessPaymentAsync(decimal amount, CancellationToken cancellationToken = default);
}

public class AsyncCreditCardStrategy : IAsyncPaymentStrategy
{
    private readonly HttpClient _httpClient;

    public async Task<PaymentResult> ProcessPaymentAsync(decimal amount, CancellationToken cancellationToken)
    {
        // Async API call
        var response = await _httpClient.PostAsync(
            "https://api.stripe.com/charges",
            CreateChargeRequest(amount),
            cancellationToken);

        var json = await response.Content.ReadAsStringAsync();
        return ParseResponse(json);
    }
}

// Usage
var strategy = new AsyncCreditCardStrategy(httpClient);
var result = await strategy.ProcessPaymentAsync(100m);
```

### Technique 5: Decorator Chain

```csharp
// Build a chain of decorators
var baseStrategy = new CreditCardStrategy();
var withLogging = new LoggingDecorator(baseStrategy);
var withRetry = new RetryDecorator(withLogging, maxRetries: 3);
var withTimeout = new TimeoutDecorator(withRetry, timeout: TimeSpan.FromSeconds(30));
var withCaching = new CachingDecorator(withTimeout);

// All decorators applied in order
var result = withCaching.ProcessPayment(100m);
```

---

## 11. Conclusion

The Strategy Pattern is one of the most practical and frequently used design patterns because it solves a common problem: **how to choose between different algorithms or behaviors without creating unmaintainable if-else chains**.

### Key Takeaways

1. **Use Strategy Pattern When**:
   - You have multiple ways to do the same thing
   - Each way has significantly different logic
   - You want to add more implementations in the future
   - You need to select behavior at runtime
   - You want to test each implementation independently

2. **Avoid Strategy Pattern When**:
   - You have only one implementation (YAGNI)
   - The differences are just configuration values
   - The algorithm never changes at runtime
   - The complexity doesn't justify the abstraction

3. **Benefits**:
   - ✅ Open/Closed Principle: Add new strategies without modifying existing code
   - ✅ Single Responsibility: Each strategy has one reason to change
   - ✅ Testability: Test strategies independently
   - ✅ Flexibility: Swap algorithms at runtime
   - ✅ Clarity: Clear separation of concerns

4. **Trade-offs**:
   - ❌ More classes: Each strategy is a separate class
   - ❌ Slight overhead: Virtual method calls and indirection
   - ❌ Client knowledge: Client must know about strategies

### Final Thoughts

The Strategy Pattern is not about following a textbook implementation—it's about **solving real problems in maintainable ways**. Don't use it because "it's a pattern"; use it because:

- Your if-else chain is getting out of hand
- You're adding payment methods every sprint
- Testing is becoming a nightmare
- Three developers are editing the same giant method

When these problems arise, Strategy Pattern is your friend. It transforms unmaintainable conditional logic into clean, extensible, testable code.

Remember: **Patterns are tools, not rules.** Use them when they solve your problem, not because they exist.

---

**Happy coding! May your strategies be many and your if-else chains be few.**
