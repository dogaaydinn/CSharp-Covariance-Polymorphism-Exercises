namespace StrategyPattern;

/// <summary>
/// Strategy Pattern Demo - Payment Strategies
///
/// This demo demonstrates the Strategy Pattern using a shopping cart payment system.
/// The Strategy Pattern defines a family of algorithms, encapsulates each one,
/// and makes them interchangeable at runtime.
///
/// Key Components:
/// - IPaymentStrategy: Common interface for all payment methods
/// - Concrete Strategies: CreditCard, PayPal, Crypto, BankTransfer
/// - PaymentContext: Manages and executes payment strategies
/// - ShoppingCart: Real-world usage scenario
/// </summary>
class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("=== Strategy Pattern Demo - Payment Strategies ===\n");

        DemonstrateBasicStrategy();
        Console.WriteLine("\n" + new string('=', 60) + "\n");

        DemonstrateRuntimeStrategySwitching();
        Console.WriteLine("\n" + new string('=', 60) + "\n");

        DemonstrateShoppingCartPayment();
        Console.WriteLine("\n" + new string('=', 60) + "\n");

        DemonstrateStrategySelection();
        Console.WriteLine("\n" + new string('=', 60) + "\n");

        DemonstrateCompositeStrategies();
        Console.WriteLine("\n" + new string('=', 60) + "\n");

        DemonstrateProblemWithoutStrategy();
        Console.WriteLine("\n" + new string('=', 60) + "\n");

        DemonstratePerformanceComparison();

        Console.WriteLine("\n=== Demo Complete ===");
    }

    /// <summary>
    /// Demonstrates basic strategy pattern usage
    /// </summary>
    static void DemonstrateBasicStrategy()
    {
        Console.WriteLine("1. BASIC STRATEGY PATTERN");
        Console.WriteLine("Different payment methods with different processing logic\n");

        var amount = 100.00m;

        // Create different payment strategies
        var creditCard = new CreditCardPaymentStrategy("1234-5678-9012-3456", "John Doe");
        var payPal = new PayPalPaymentStrategy("john@example.com");
        var crypto = new CryptoPaymentStrategy("0x742d35Cc6634C0532925a3b844Bc9e7595f0bEb");
        var bankTransfer = new BankTransferPaymentStrategy("US123456789", "SWIFT123");

        // Use context to process payments
        var context = new PaymentContext();

        Console.WriteLine("Processing with different strategies:");
        context.SetStrategy(creditCard);
        context.ProcessPayment(amount);

        context.SetStrategy(payPal);
        context.ProcessPayment(amount);

        context.SetStrategy(crypto);
        context.ProcessPayment(amount);

        context.SetStrategy(bankTransfer);
        context.ProcessPayment(amount);

        Console.WriteLine("\n✓ Same interface, different implementations");
    }

    /// <summary>
    /// Demonstrates runtime strategy switching
    /// </summary>
    static void DemonstrateRuntimeStrategySwitching()
    {
        Console.WriteLine("2. RUNTIME STRATEGY SWITCHING");
        Console.WriteLine("Change payment method during application execution\n");

        var context = new PaymentContext();
        var amount = 250.00m;

        Console.WriteLine($"Initial payment amount: ${amount}\n");

        // Start with credit card
        Console.WriteLine("User selects: Credit Card");
        context.SetStrategy(new CreditCardPaymentStrategy("4111-1111-1111-1111", "Alice Smith"));
        var result1 = context.ProcessPayment(amount);
        Console.WriteLine($"   Total charged: ${result1.TotalAmount:F2}\n");

        // User changes mind, switches to PayPal
        Console.WriteLine("User changes to: PayPal");
        context.SetStrategy(new PayPalPaymentStrategy("alice@example.com"));
        var result2 = context.ProcessPayment(amount);
        Console.WriteLine($"   Total charged: ${result2.TotalAmount:F2}\n");

        // User switches to crypto for lower fees
        Console.WriteLine("User switches to: Cryptocurrency (lower fees)");
        context.SetStrategy(new CryptoPaymentStrategy("0x123...abc"));
        var result3 = context.ProcessPayment(amount);
        Console.WriteLine($"   Total charged: ${result3.TotalAmount:F2}");

        Console.WriteLine($"\n✓ Fee comparison:");
        Console.WriteLine($"   Credit Card: ${result1.Fee:F2}");
        Console.WriteLine($"   PayPal: ${result2.Fee:F2}");
        Console.WriteLine($"   Crypto: ${result3.Fee:F2}");
        Console.WriteLine($"   Savings with Crypto: ${result1.Fee - result3.Fee:F2}");
    }

    /// <summary>
    /// Demonstrates shopping cart with payment strategy
    /// </summary>
    static void DemonstrateShoppingCartPayment()
    {
        Console.WriteLine("3. SHOPPING CART PAYMENT");
        Console.WriteLine("Real-world e-commerce scenario\n");

        // Create shopping cart
        var cart = new ShoppingCart();
        cart.AddItem(new CartItem("Laptop", 999.99m, 1));
        cart.AddItem(new CartItem("Mouse", 29.99m, 2));
        cart.AddItem(new CartItem("Keyboard", 79.99m, 1));

        Console.WriteLine("Shopping Cart:");
        foreach (var item in cart.GetItems())
        {
            Console.WriteLine($"   {item.Quantity}x {item.Name} @ ${item.Price:F2} = ${item.GetTotal():F2}");
        }
        Console.WriteLine($"   Subtotal: ${cart.GetSubtotal():F2}\n");

        // Try different payment methods
        Console.WriteLine("Payment Method Options:\n");

        // Option 1: Credit Card
        Console.WriteLine("1. Credit Card:");
        var ccStrategy = new CreditCardPaymentStrategy("5555-4444-3333-2222", "Bob Wilson");
        cart.SetPaymentStrategy(ccStrategy);
        var ccResult = cart.Checkout();
        Console.WriteLine($"   Subtotal: ${ccResult.Amount:F2}");
        Console.WriteLine($"   Fee: ${ccResult.Fee:F2}");
        Console.WriteLine($"   Total: ${ccResult.TotalAmount:F2}\n");

        // Option 2: PayPal
        Console.WriteLine("2. PayPal:");
        var ppStrategy = new PayPalPaymentStrategy("bob@example.com");
        cart.SetPaymentStrategy(ppStrategy);
        var ppResult = cart.Checkout();
        Console.WriteLine($"   Subtotal: ${ppResult.Amount:F2}");
        Console.WriteLine($"   Fee: ${ppResult.Fee:F2}");
        Console.WriteLine($"   Total: ${ppResult.TotalAmount:F2}\n");

        // Option 3: Cryptocurrency
        Console.WriteLine("3. Cryptocurrency:");
        var cryptoStrategy = new CryptoPaymentStrategy("0xabc...xyz");
        cart.SetPaymentStrategy(cryptoStrategy);
        var cryptoResult = cart.Checkout();
        Console.WriteLine($"   Subtotal: ${cryptoResult.Amount:F2}");
        Console.WriteLine($"   Fee: ${cryptoResult.Fee:F2}");
        Console.WriteLine($"   Total: ${cryptoResult.TotalAmount:F2}\n");

        // Option 4: Bank Transfer
        Console.WriteLine("4. Bank Transfer:");
        var bankStrategy = new BankTransferPaymentStrategy("DE89370400440532013000", "DEUTDEFF");
        cart.SetPaymentStrategy(bankStrategy);
        var bankResult = cart.Checkout();
        Console.WriteLine($"   Subtotal: ${bankResult.Amount:F2}");
        Console.WriteLine($"   Fee: ${bankResult.Fee:F2}");
        Console.WriteLine($"   Total: ${bankResult.TotalAmount:F2}");

        Console.WriteLine($"\n✓ Best option: {GetBestPaymentMethod(ccResult, ppResult, cryptoResult, bankResult)}");
    }

    static string GetBestPaymentMethod(params PaymentResult[] results)
    {
        var min = results.MinBy(r => r.TotalAmount);
        return min?.PaymentMethod ?? "Unknown";
    }

    /// <summary>
    /// Demonstrates strategy selection based on amount
    /// </summary>
    static void DemonstrateStrategySelection()
    {
        Console.WriteLine("4. INTELLIGENT STRATEGY SELECTION");
        Console.WriteLine("Automatically choose best payment method\n");

        var selector = new PaymentStrategySelector();

        var amounts = new[] { 10m, 50m, 250m, 1000m, 5000m };

        foreach (var amount in amounts)
        {
            var strategy = selector.SelectBestStrategy(amount);
            var context = new PaymentContext(strategy);
            var result = context.ProcessPayment(amount);

            Console.WriteLine($"Amount: ${amount,7:F2} → {result.PaymentMethod,-20} (Fee: ${result.Fee,6:F2}, Total: ${result.TotalAmount,8:F2})");
        }

        Console.WriteLine("\n✓ Strategy selection based on amount optimizes fees");
    }

    /// <summary>
    /// Demonstrates composite strategies (discounts + loyalty)
    /// </summary>
    static void DemonstrateCompositeStrategies()
    {
        Console.WriteLine("5. COMPOSITE STRATEGIES");
        Console.WriteLine("Combine multiple strategies (payment + discount + loyalty)\n");

        var cart = new ShoppingCart();
        cart.AddItem(new CartItem("Premium Headphones", 299.99m, 1));
        cart.AddItem(new CartItem("Phone Case", 19.99m, 2));

        var subtotal = cart.GetSubtotal();
        Console.WriteLine($"Cart Subtotal: ${subtotal:F2}\n");

        // Base payment strategy
        var baseStrategy = new CreditCardPaymentStrategy("4111-1111-1111-1111", "Premium Customer");

        // Add discount strategy
        var discountStrategy = new DiscountPaymentStrategy(baseStrategy, 0.10m); // 10% discount

        // Add loyalty points strategy
        var loyaltyStrategy = new LoyaltyPointsPaymentStrategy(discountStrategy, 500); // 500 points = $5

        cart.SetPaymentStrategy(loyaltyStrategy);
        var result = cart.Checkout();

        Console.WriteLine("Payment Breakdown:");
        Console.WriteLine($"   Original Amount: ${subtotal:F2}");
        Console.WriteLine($"   Discount (10%): -${subtotal * 0.10m:F2}");
        Console.WriteLine($"   Loyalty Points: -$5.00");
        Console.WriteLine($"   Subtotal: ${result.Amount:F2}");
        Console.WriteLine($"   Processing Fee: ${result.Fee:F2}");
        Console.WriteLine($"   Final Total: ${result.TotalAmount:F2}");

        Console.WriteLine("\n✓ Decorator pattern combined with strategy for flexible pricing");
    }

    /// <summary>
    /// Shows the problem without strategy pattern
    /// </summary>
    static void DemonstrateProblemWithoutStrategy()
    {
        Console.WriteLine("6. PROBLEM WITHOUT STRATEGY PATTERN");
        Console.WriteLine("Code becomes unmaintainable with if-else chains\n");

        var processor = new LegacyPaymentProcessor();

        Console.WriteLine("Legacy code with if-else:");
        Console.WriteLine("```csharp");
        Console.WriteLine("if (paymentMethod == \"CreditCard\") { ... }");
        Console.WriteLine("else if (paymentMethod == \"PayPal\") { ... }");
        Console.WriteLine("else if (paymentMethod == \"Crypto\") { ... }");
        Console.WriteLine("else if (paymentMethod == \"BankTransfer\") { ... }");
        Console.WriteLine("// Adding new method requires modifying this method!");
        Console.WriteLine("```\n");

        processor.ProcessPayment(100m, "CreditCard");
        processor.ProcessPayment(100m, "PayPal");
        processor.ProcessPayment(100m, "Crypto");
        processor.ProcessPayment(100m, "BankTransfer");

        Console.WriteLine("\n❌ Problems:");
        Console.WriteLine("   - Violates Open/Closed Principle");
        Console.WriteLine("   - Hard to test individual payment methods");
        Console.WriteLine("   - Difficult to add new payment methods");
        Console.WriteLine("   - Cannot change payment method at runtime easily");
        Console.WriteLine("   - Mixed responsibilities");
    }

    /// <summary>
    /// Performance comparison between approaches
    /// </summary>
    static void DemonstratePerformanceComparison()
    {
        Console.WriteLine("7. PERFORMANCE COMPARISON");
        Console.WriteLine("Strategy pattern vs if-else performance\n");

        var iterations = 100000;
        var amount = 100m;

        // Strategy pattern
        var sw = System.Diagnostics.Stopwatch.StartNew();
        var context = new PaymentContext(new CreditCardPaymentStrategy("1234", "Test"));
        for (int i = 0; i < iterations; i++)
        {
            context.ProcessPayment(amount);
        }
        sw.Stop();
        var strategyTime = sw.ElapsedMilliseconds;

        // If-else approach
        sw.Restart();
        var legacy = new LegacyPaymentProcessor();
        for (int i = 0; i < iterations; i++)
        {
            legacy.ProcessPayment(amount, "CreditCard");
        }
        sw.Stop();
        var ifElseTime = sw.ElapsedMilliseconds;

        Console.WriteLine($"Processing {iterations:N0} payments:\n");
        Console.WriteLine($"Strategy Pattern: {strategyTime}ms");
        Console.WriteLine($"If-Else Chain:    {ifElseTime}ms");
        Console.WriteLine($"Difference:       {Math.Abs(strategyTime - ifElseTime)}ms");

        Console.WriteLine("\n✓ Performance is comparable, design benefits outweigh any overhead");
    }
}

#region Payment Strategy Interface and Implementations

/// <summary>
/// Payment Strategy Interface
/// Defines the contract for all payment methods
/// </summary>
public interface IPaymentStrategy
{
    string Name { get; }
    PaymentResult ProcessPayment(decimal amount);
    decimal CalculateFee(decimal amount);
    bool ValidatePayment(decimal amount);
}

/// <summary>
/// Payment result containing transaction details
/// </summary>
public class PaymentResult
{
    public bool Success { get; set; }
    public string PaymentMethod { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public decimal Fee { get; set; }
    public decimal TotalAmount => Amount + Fee;
    public string TransactionId { get; set; } = string.Empty;
    public DateTime ProcessedAt { get; set; }
    public string Message { get; set; } = string.Empty;
}

/// <summary>
/// Credit Card Payment Strategy
/// Fee: 2.9% + $0.30
/// </summary>
public class CreditCardPaymentStrategy : IPaymentStrategy
{
    private readonly string _cardNumber;
    private readonly string _cardHolder;

    public string Name => "Credit Card";

    public CreditCardPaymentStrategy(string cardNumber, string cardHolder)
    {
        _cardNumber = cardNumber;
        _cardHolder = cardHolder;
    }

    public PaymentResult ProcessPayment(decimal amount)
    {
        if (!ValidatePayment(amount))
        {
            return new PaymentResult
            {
                Success = false,
                PaymentMethod = Name,
                Amount = amount,
                Message = "Payment validation failed"
            };
        }

        var fee = CalculateFee(amount);

        Console.WriteLine($"   Processing Credit Card payment for {_cardHolder}");
        Console.WriteLine($"   Card: ****{_cardNumber.Substring(_cardNumber.Length - 4)}");

        return new PaymentResult
        {
            Success = true,
            PaymentMethod = Name,
            Amount = amount,
            Fee = fee,
            TransactionId = Guid.NewGuid().ToString("N").Substring(0, 16).ToUpper(),
            ProcessedAt = DateTime.Now,
            Message = "Payment processed successfully"
        };
    }

    public decimal CalculateFee(decimal amount)
    {
        return amount * 0.029m + 0.30m; // 2.9% + $0.30
    }

    public bool ValidatePayment(decimal amount)
    {
        // Credit card validation: reasonable limits
        return amount > 0 && amount <= 10000m;
    }
}

/// <summary>
/// PayPal Payment Strategy
/// Fee: 2.9% + $0.30 (similar to credit card)
/// </summary>
public class PayPalPaymentStrategy : IPaymentStrategy
{
    private readonly string _email;

    public string Name => "PayPal";

    public PayPalPaymentStrategy(string email)
    {
        _email = email;
    }

    public PaymentResult ProcessPayment(decimal amount)
    {
        if (!ValidatePayment(amount))
        {
            return new PaymentResult
            {
                Success = false,
                PaymentMethod = Name,
                Amount = amount,
                Message = "Payment validation failed"
            };
        }

        var fee = CalculateFee(amount);

        Console.WriteLine($"   Processing PayPal payment for {_email}");
        Console.WriteLine($"   Redirecting to PayPal...");

        return new PaymentResult
        {
            Success = true,
            PaymentMethod = Name,
            Amount = amount,
            Fee = fee,
            TransactionId = "PP" + Guid.NewGuid().ToString("N").Substring(0, 14).ToUpper(),
            ProcessedAt = DateTime.Now,
            Message = "PayPal payment completed"
        };
    }

    public decimal CalculateFee(decimal amount)
    {
        return amount * 0.029m + 0.30m; // 2.9% + $0.30
    }

    public bool ValidatePayment(decimal amount)
    {
        // PayPal validation: check account and limits
        return amount > 0 && amount <= 15000m;
    }
}

/// <summary>
/// Cryptocurrency Payment Strategy
/// Fee: 0.5% (lower than traditional methods)
/// </summary>
public class CryptoPaymentStrategy : IPaymentStrategy
{
    private readonly string _walletAddress;

    public string Name => "Cryptocurrency";

    public CryptoPaymentStrategy(string walletAddress)
    {
        _walletAddress = walletAddress;
    }

    public PaymentResult ProcessPayment(decimal amount)
    {
        if (!ValidatePayment(amount))
        {
            return new PaymentResult
            {
                Success = false,
                PaymentMethod = Name,
                Amount = amount,
                Message = "Payment validation failed"
            };
        }

        var fee = CalculateFee(amount);

        Console.WriteLine($"   Processing Crypto payment");
        Console.WriteLine($"   Wallet: {_walletAddress.Substring(0, 6)}...{_walletAddress.Substring(_walletAddress.Length - 4)}");
        Console.WriteLine($"   Blockchain confirmation pending...");

        return new PaymentResult
        {
            Success = true,
            PaymentMethod = Name,
            Amount = amount,
            Fee = fee,
            TransactionId = "0x" + Guid.NewGuid().ToString("N").ToUpper(),
            ProcessedAt = DateTime.Now,
            Message = "Cryptocurrency payment initiated"
        };
    }

    public decimal CalculateFee(decimal amount)
    {
        return amount * 0.005m; // 0.5% - much lower than traditional methods
    }

    public bool ValidatePayment(decimal amount)
    {
        // Crypto validation: minimum amount due to network fees
        return amount >= 10m && amount <= 100000m;
    }
}

/// <summary>
/// Bank Transfer Payment Strategy
/// Fee: Fixed $5 or 0.25% for large amounts
/// </summary>
public class BankTransferPaymentStrategy : IPaymentStrategy
{
    private readonly string _accountNumber;
    private readonly string _routingNumber;

    public string Name => "Bank Transfer";

    public BankTransferPaymentStrategy(string accountNumber, string routingNumber)
    {
        _accountNumber = accountNumber;
        _routingNumber = routingNumber;
    }

    public PaymentResult ProcessPayment(decimal amount)
    {
        if (!ValidatePayment(amount))
        {
            return new PaymentResult
            {
                Success = false,
                PaymentMethod = Name,
                Amount = amount,
                Message = "Payment validation failed"
            };
        }

        var fee = CalculateFee(amount);

        Console.WriteLine($"   Processing Bank Transfer");
        Console.WriteLine($"   Account: ****{_accountNumber.Substring(_accountNumber.Length - 4)}");
        Console.WriteLine($"   Routing: {_routingNumber}");
        Console.WriteLine($"   Processing time: 1-3 business days");

        return new PaymentResult
        {
            Success = true,
            PaymentMethod = Name,
            Amount = amount,
            Fee = fee,
            TransactionId = "BT" + DateTime.Now.ToString("yyyyMMddHHmmss"),
            ProcessedAt = DateTime.Now,
            Message = "Bank transfer initiated"
        };
    }

    public decimal CalculateFee(decimal amount)
    {
        // Fixed $5 for amounts under $2000, 0.25% for larger amounts
        return amount < 2000m ? 5.00m : amount * 0.0025m;
    }

    public bool ValidatePayment(decimal amount)
    {
        // Bank transfer validation: minimum amount
        return amount >= 50m && amount <= 50000m;
    }
}

#endregion

#region Payment Context

/// <summary>
/// Payment Context
/// Maintains a reference to a Strategy object and delegates payment processing
/// </summary>
public class PaymentContext
{
    private IPaymentStrategy? _strategy;

    public PaymentContext() { }

    public PaymentContext(IPaymentStrategy strategy)
    {
        _strategy = strategy;
    }

    public void SetStrategy(IPaymentStrategy strategy)
    {
        _strategy = strategy ?? throw new ArgumentNullException(nameof(strategy));
    }

    public PaymentResult ProcessPayment(decimal amount)
    {
        if (_strategy == null)
        {
            throw new InvalidOperationException("Payment strategy not set");
        }

        return _strategy.ProcessPayment(amount);
    }

    public decimal CalculateFee(decimal amount)
    {
        if (_strategy == null)
        {
            throw new InvalidOperationException("Payment strategy not set");
        }

        return _strategy.CalculateFee(amount);
    }
}

#endregion

#region Shopping Cart

/// <summary>
/// Shopping cart item
/// </summary>
public class CartItem
{
    public string Name { get; set; }
    public decimal Price { get; set; }
    public int Quantity { get; set; }

    public CartItem(string name, decimal price, int quantity)
    {
        Name = name;
        Price = price;
        Quantity = quantity;
    }

    public decimal GetTotal() => Price * Quantity;
}

/// <summary>
/// Shopping Cart with payment strategy
/// Real-world example of strategy pattern usage
/// </summary>
public class ShoppingCart
{
    private readonly List<CartItem> _items = new();
    private IPaymentStrategy? _paymentStrategy;

    public void AddItem(CartItem item)
    {
        _items.Add(item);
    }

    public void RemoveItem(CartItem item)
    {
        _items.Remove(item);
    }

    public IReadOnlyList<CartItem> GetItems() => _items.AsReadOnly();

    public decimal GetSubtotal()
    {
        return _items.Sum(item => item.GetTotal());
    }

    public void SetPaymentStrategy(IPaymentStrategy strategy)
    {
        _paymentStrategy = strategy;
    }

    public PaymentResult Checkout()
    {
        if (_paymentStrategy == null)
        {
            throw new InvalidOperationException("Payment strategy not set");
        }

        var subtotal = GetSubtotal();
        return _paymentStrategy.ProcessPayment(subtotal);
    }
}

#endregion

#region Strategy Selection

/// <summary>
/// Intelligent payment strategy selector
/// Chooses best strategy based on amount and other factors
/// </summary>
public class PaymentStrategySelector
{
    public IPaymentStrategy SelectBestStrategy(decimal amount)
    {
        // For small amounts: use PayPal (convenient)
        if (amount < 50m)
        {
            return new PayPalPaymentStrategy("auto-selected@example.com");
        }

        // For medium amounts: use Credit Card (fast)
        if (amount < 500m)
        {
            return new CreditCardPaymentStrategy("auto-card", "Auto Selected");
        }

        // For large amounts under $2000: use Crypto (low fees)
        if (amount < 2000m)
        {
            return new CryptoPaymentStrategy("0xauto");
        }

        // For very large amounts: use Bank Transfer (lowest percentage fee)
        return new BankTransferPaymentStrategy("auto-account", "auto-routing");
    }
}

#endregion

#region Decorator Strategies (Composite)

/// <summary>
/// Discount payment strategy (decorator)
/// Applies discount before processing payment
/// </summary>
public class DiscountPaymentStrategy : IPaymentStrategy
{
    private readonly IPaymentStrategy _baseStrategy;
    private readonly decimal _discountPercentage;

    public string Name => $"{_baseStrategy.Name} with Discount";

    public DiscountPaymentStrategy(IPaymentStrategy baseStrategy, decimal discountPercentage)
    {
        _baseStrategy = baseStrategy;
        _discountPercentage = discountPercentage;
    }

    public PaymentResult ProcessPayment(decimal amount)
    {
        var discountedAmount = amount * (1 - _discountPercentage);
        return _baseStrategy.ProcessPayment(discountedAmount);
    }

    public decimal CalculateFee(decimal amount)
    {
        var discountedAmount = amount * (1 - _discountPercentage);
        return _baseStrategy.CalculateFee(discountedAmount);
    }

    public bool ValidatePayment(decimal amount)
    {
        var discountedAmount = amount * (1 - _discountPercentage);
        return _baseStrategy.ValidatePayment(discountedAmount);
    }
}

/// <summary>
/// Loyalty points payment strategy (decorator)
/// Deducts loyalty points from total
/// </summary>
public class LoyaltyPointsPaymentStrategy : IPaymentStrategy
{
    private readonly IPaymentStrategy _baseStrategy;
    private readonly int _points;
    private const decimal PointsToMoneyRatio = 0.01m; // 100 points = $1

    public string Name => $"{_baseStrategy.Name} with Loyalty Points";

    public LoyaltyPointsPaymentStrategy(IPaymentStrategy baseStrategy, int points)
    {
        _baseStrategy = baseStrategy;
        _points = points;
    }

    public PaymentResult ProcessPayment(decimal amount)
    {
        var discount = _points * PointsToMoneyRatio;
        var finalAmount = Math.Max(0, amount - discount);
        return _baseStrategy.ProcessPayment(finalAmount);
    }

    public decimal CalculateFee(decimal amount)
    {
        var discount = _points * PointsToMoneyRatio;
        var finalAmount = Math.Max(0, amount - discount);
        return _baseStrategy.CalculateFee(finalAmount);
    }

    public bool ValidatePayment(decimal amount)
    {
        var discount = _points * PointsToMoneyRatio;
        var finalAmount = Math.Max(0, amount - discount);
        return _baseStrategy.ValidatePayment(finalAmount);
    }
}

#endregion

#region Legacy Code (Problem)

/// <summary>
/// Legacy payment processor without strategy pattern
/// Demonstrates the problem: if-else chain that violates Open/Closed Principle
/// </summary>
public class LegacyPaymentProcessor
{
    public void ProcessPayment(decimal amount, string paymentMethod)
    {
        if (paymentMethod == "CreditCard")
        {
            var fee = amount * 0.029m + 0.30m;
            Console.WriteLine($"   ❌ Credit Card: ${amount + fee:F2} (legacy if-else)");
        }
        else if (paymentMethod == "PayPal")
        {
            var fee = amount * 0.029m + 0.30m;
            Console.WriteLine($"   ❌ PayPal: ${amount + fee:F2} (legacy if-else)");
        }
        else if (paymentMethod == "Crypto")
        {
            var fee = amount * 0.005m;
            Console.WriteLine($"   ❌ Crypto: ${amount + fee:F2} (legacy if-else)");
        }
        else if (paymentMethod == "BankTransfer")
        {
            var fee = amount < 2000m ? 5.00m : amount * 0.0025m;
            Console.WriteLine($"   ❌ Bank Transfer: ${amount + fee:F2} (legacy if-else)");
        }
        else
        {
            throw new ArgumentException($"Unknown payment method: {paymentMethod}");
        }

        // Problem: Adding new payment method requires modifying this method!
        // Violates Open/Closed Principle
    }
}

#endregion
