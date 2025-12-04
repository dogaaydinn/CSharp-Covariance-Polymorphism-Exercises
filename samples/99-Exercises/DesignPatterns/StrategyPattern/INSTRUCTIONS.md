# 🎯 Exercise: Strategy Pattern - Payment Processing System

**Difficulty:** 🟢 Beginner
**Estimated Time:** 15-20 minutes
**Tests:** 6 tests

---

## 📚 Learning Objectives

After completing this exercise, you will understand:
- ✅ How to implement the Strategy design pattern
- ✅ When to use interfaces for behavior abstraction
- ✅ How to achieve runtime algorithm selection
- ✅ Dependency injection and loose coupling

---

## 🎯 Problem Statement

You're building an **e-commerce shopping cart system** that needs to support **multiple payment methods**:
- 💳 Credit Card payments
- 💰 PayPal payments
- 🏦 Bank Transfer payments

Each payment method has different processing logic:
- **Credit Card:** Validates card number, checks expiry date, processes via payment gateway
- **PayPal:** Authenticates with PayPal account, transfers funds
- **Bank Transfer:** Verifies bank account, initiates ACH transfer

**Bad Approach (what we DON'T want):**
```csharp
public class ShoppingCart
{
    public void Checkout(string paymentType)
    {
        if (paymentType == "CreditCard")
        {
            // Credit card logic
        }
        else if (paymentType == "PayPal")
        {
            // PayPal logic
        }
        else if (paymentType == "BankTransfer")
        {
            // Bank transfer logic
        }
        // Adding new payment method requires modifying this class! ❌
    }
}
```

**Good Approach (Strategy Pattern):**
```csharp
public class ShoppingCart
{
    private readonly IPaymentStrategy _paymentStrategy;

    public ShoppingCart(IPaymentStrategy paymentStrategy)
    {
        _paymentStrategy = paymentStrategy;
    }

    public void Checkout()
    {
        _paymentStrategy.ProcessPayment(total);
        // Adding new payment method = new strategy class! ✅
    }
}
```

---

## 📋 Your Task

### Step 1: Review the Provided Code

**Already implemented for you:**
- ✅ `IPaymentStrategy.cs` - Interface defining payment strategy contract
- ✅ `CreditCardPaymentStrategy.cs` - Credit card implementation
- ✅ `PayPalPaymentStrategy.cs` - PayPal implementation
- ✅ `BankTransferPaymentStrategy.cs` - Bank transfer implementation (INCOMPLETE - you'll finish this)

### Step 2: Complete the `ShoppingCart` Class

**File:** `ShoppingCart.cs`

**What's missing:**
1. **Constructor:** Accept `IPaymentStrategy` as a dependency
2. **ProcessPayment method:** Use the strategy to process payment

**Requirements:**
- Store the payment strategy in a private readonly field
- The `ProcessPayment` method should:
  - Calculate total from all cart items
  - Call the strategy's `ProcessPayment` method with total and description
  - Return the result

### Step 3: Complete the `BankTransferPaymentStrategy` Class

**File:** `BankTransferPaymentStrategy.cs`

**What's missing:**
- Implement the `ProcessPayment` method
- It should return a message like: `"Bank transfer of ${amount:F2} from account {accountNumber}. Reference: {description}"`

### Step 4: Run the Tests

```bash
# Navigate to exercise directory
cd samples/99-Exercises/DesignPatterns/StrategyPattern

# Run tests
dotnet test

# Expected output (before solving):
# ❌ Failed: 6 tests
# Total: 6 tests

# Expected output (after solving):
# ✅ Passed: 6 tests
# Total: 6 tests
```

---

## 🧪 Test Cases

Your solution must pass these tests:

### Test 1: Credit Card Payment
```csharp
[Fact]
public void CreditCard_ProcessesPaymentCorrectly()
{
    // Given: A shopping cart with credit card payment
    var cart = new ShoppingCart(new CreditCardPaymentStrategy("1234-5678-9012-3456", "12/25"));
    cart.AddItem(new CartItem("Laptop", 999.99m));

    // When: Processing payment
    var result = cart.ProcessPayment();

    // Then: Payment is processed via credit card
    result.Should().Contain("Credit card");
    result.Should().Contain("999.99");
}
```

### Test 2: Multiple Items Total
```csharp
[Fact]
public void MultipleItems_CalculatesTotalCorrectly()
{
    // Given: Cart with multiple items
    var cart = new ShoppingCart(new PayPalPaymentStrategy("user@example.com"));
    cart.AddItem(new CartItem("Laptop", 999.99m));
    cart.AddItem(new CartItem("Mouse", 29.99m));
    cart.AddItem(new CartItem("Keyboard", 79.99m));

    // When: Processing payment
    var result = cart.ProcessPayment();

    // Then: Total is calculated correctly
    result.Should().Contain("1109.97"); // 999.99 + 29.99 + 79.99
}
```

### Test 3: Bank Transfer Payment
```csharp
[Fact]
public void BankTransfer_ProcessesPaymentCorrectly()
{
    // Given: Cart with bank transfer payment
    var cart = new ShoppingCart(new BankTransferPaymentStrategy("US1234567890"));
    cart.AddItem(new CartItem("Monitor", 299.99m));

    // When: Processing payment
    var result = cart.ProcessPayment();

    // Then: Bank transfer is processed
    result.Should().Contain("Bank transfer");
    result.Should().Contain("299.99");
    result.Should().Contain("US1234567890");
}
```

*(Plus 3 more tests for edge cases)*

---

## 💡 Hints

### Hint 1: Constructor Dependency Injection
```csharp
private readonly IPaymentStrategy _paymentStrategy;

public ShoppingCart(IPaymentStrategy paymentStrategy)
{
    // TODO: Store the payment strategy
    // Remember: Use the private field above
}
```

### Hint 2: Calculating Total
```csharp
public string ProcessPayment()
{
    // Step 1: Calculate total from all items
    decimal total = _items.Sum(item => item.Price);

    // Step 2: Create a description
    string description = $"Order with {_items.Count} item(s)";

    // Step 3: Use the strategy to process payment
    // TODO: Call the strategy's ProcessPayment method
}
```

### Hint 3: Strategy Pattern Key Insight
The `ShoppingCart` class doesn't know (and doesn't care) **which specific payment strategy** it's using. It just knows it has **some strategy** that implements `IPaymentStrategy`.

This is the essence of the Strategy pattern: **separating the algorithm from the context that uses it**.

---

## 🎓 Key Concepts

### Strategy Pattern Definition
> The Strategy pattern defines a family of algorithms, encapsulates each one, and makes them interchangeable. Strategy lets the algorithm vary independently from clients that use it.

### When to Use Strategy Pattern
- ✅ You have multiple algorithms for a specific task
- ✅ You want to switch algorithms at runtime
- ✅ You want to avoid if-else or switch statements for algorithm selection
- ✅ You want to add new algorithms without modifying existing code (Open/Closed Principle)

### Benefits
- ✅ **Open/Closed Principle:** Open for extension, closed for modification
- ✅ **Single Responsibility:** Each strategy has one responsibility
- ✅ **Runtime Flexibility:** Change strategies at runtime
- ✅ **Testability:** Easy to test each strategy independently

### Real-World Examples
- **Compression:** ZIP, RAR, 7Z algorithms
- **Sorting:** QuickSort, MergeSort, BubbleSort
- **Logging:** File, Console, Database loggers
- **Authentication:** OAuth, JWT, Basic Auth

---

## ✅ Acceptance Criteria

Your solution is complete when:
1. ✅ All 6 tests pass (`dotnet test`)
2. ✅ `ShoppingCart` accepts `IPaymentStrategy` in constructor
3. ✅ `ProcessPayment` correctly calculates total and uses strategy
4. ✅ `BankTransferPaymentStrategy` is fully implemented
5. ✅ No compilation errors
6. ✅ Code follows naming conventions

---

## 🚀 Bonus Challenges (Optional)

After completing the main exercise, try these challenges:

### Challenge 1: Add a New Strategy
Implement `CryptocurrencyPaymentStrategy` that processes Bitcoin payments.

### Challenge 2: Strategy with Validation
Add a `Validate()` method to `IPaymentStrategy` and implement validation logic in each strategy.

### Challenge 3: Composite Pattern
Create a `CompositePaymentStrategy` that combines multiple strategies (e.g., pay 50% with credit card, 50% with PayPal).

### Challenge 4: Discount Strategy
Create an `IDiscountStrategy` interface and apply discounts before payment (e.g., PercentageDiscount, FixedAmountDiscount).

---

## 📚 Further Reading

- [Refactoring Guru - Strategy Pattern](https://refactoring.guru/design-patterns/strategy)
- [Microsoft Docs - Design Patterns](https://docs.microsoft.com/en-us/dotnet/standard/design-guidelines/)
- [Head First Design Patterns (book)](https://www.oreilly.com/library/view/head-first-design/0596007124/)

---

## 🆘 Getting Stuck?

If you've been stuck for more than 15 minutes:
1. Re-read the hints section above
2. Check the test cases - they show exactly what's expected
3. Look at how `CreditCardPaymentStrategy` is implemented
4. Review `SOLUTION.md` (but try to solve it first!)

---

**Good luck! 🍀 Remember: struggling is part of learning!**

---

## 📊 Progress Tracker

Mark your progress as you go:

- [ ] Read and understood the instructions
- [ ] Reviewed the provided code (`IPaymentStrategy`, implementations)
- [ ] Completed `ShoppingCart` constructor
- [ ] Completed `ShoppingCart.ProcessPayment` method
- [ ] Completed `BankTransferPaymentStrategy.ProcessPayment` method
- [ ] All tests passing (`dotnet test`)
- [ ] Understood why Strategy pattern is useful
- [ ] (Bonus) Attempted bonus challenges
