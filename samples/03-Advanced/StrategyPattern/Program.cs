// Strategy Pattern: Runtime algorithm selection

namespace StrategyPattern;

public class Program
{
    public static void Main()
    {
        Console.WriteLine("=== Strategy Pattern Demo ===\n");

        // ❌ BAD: if-else chain
        Console.WriteLine("❌ BAD APPROACH - If/Else Chain:");
        var badPayment = new BadPaymentProcessor();
        badPayment.Process(100, "credit");
        badPayment.Process(100, "paypal");
        badPayment.Process(100, "crypto");

        // ✅ GOOD: Strategy Pattern
        Console.WriteLine("\n✅ GOOD APPROACH - Strategy Pattern:");

        IPaymentStrategy creditCard = new CreditCardStrategy();
        IPaymentStrategy paypal = new PayPalStrategy();
        IPaymentStrategy crypto = new CryptoStrategy();

        var processor = new PaymentProcessor(creditCard);
        processor.ProcessPayment(100);

        processor.SetStrategy(paypal);
        processor.ProcessPayment(100);

        processor.SetStrategy(crypto);
        processor.ProcessPayment(100);

        // Advanced: Strategy with configuration
        Console.WriteLine("\n✅ ADVANCED - Strategy with Context:");
        var cartTotal = 250m;

        IShippingStrategy standard = new StandardShipping();
        IShippingStrategy express = new ExpressShipping();
        IShippingStrategy overnight = new OvernightShipping();

        var order = new Order(cartTotal, standard);
        Console.WriteLine($"Standard shipping: ${order.CalculateTotal():F2}");

        order.SetShippingStrategy(express);
        Console.WriteLine($"Express shipping: ${order.CalculateTotal():F2}");

        order.SetShippingStrategy(overnight);
        Console.WriteLine($"Overnight shipping: ${order.CalculateTotal():F2}");

        Console.WriteLine("\n=== Strategy Pattern Applied ===");
    }
}

// BEFORE REFACTORING (Bad)
public class BadPaymentProcessor
{
    public void Process(decimal amount, string method)
    {
        if (method == "credit")
        {
            Console.WriteLine($"Processing ${amount} via Credit Card");
            Console.WriteLine("Applying 3% fee");
        }
        else if (method == "paypal")
        {
            Console.WriteLine($"Processing ${amount} via PayPal");
            Console.WriteLine("Applying 2.9% + $0.30 fee");
        }
        else if (method == "crypto")
        {
            Console.WriteLine($"Processing ${amount} via Cryptocurrency");
            Console.WriteLine("Applying 1% fee");
        }
        // Adding new payment method requires modifying this class!
    }
}

// AFTER REFACTORING (Good)
public interface IPaymentStrategy
{
    void Pay(decimal amount);
    decimal CalculateFee(decimal amount);
}

public class CreditCardStrategy : IPaymentStrategy
{
    public void Pay(decimal amount)
    {
        var fee = CalculateFee(amount);
        var total = amount + fee;
        Console.WriteLine($"✅ Credit Card: ${total:F2} (${amount:F2} + ${fee:F2} fee)");
    }

    public decimal CalculateFee(decimal amount) => amount * 0.03m; // 3%
}

public class PayPalStrategy : IPaymentStrategy
{
    public void Pay(decimal amount)
    {
        var fee = CalculateFee(amount);
        var total = amount + fee;
        Console.WriteLine($"✅ PayPal: ${total:F2} (${amount:F2} + ${fee:F2} fee)");
    }

    public decimal CalculateFee(decimal amount) => amount * 0.029m + 0.30m; // 2.9% + $0.30
}

public class CryptoStrategy : IPaymentStrategy
{
    public void Pay(decimal amount)
    {
        var fee = CalculateFee(amount);
        var total = amount + fee;
        Console.WriteLine($"✅ Crypto: ${total:F2} (${amount:F2} + ${fee:F2} fee)");
    }

    public decimal CalculateFee(decimal amount) => amount * 0.01m; // 1%
}

// Context class
public class PaymentProcessor
{
    private IPaymentStrategy _strategy;

    public PaymentProcessor(IPaymentStrategy strategy)
    {
        _strategy = strategy;
    }

    public void SetStrategy(IPaymentStrategy strategy)
    {
        _strategy = strategy;
    }

    public void ProcessPayment(decimal amount)
    {
        _strategy.Pay(amount);
    }
}

// Advanced Example: Shipping Strategies
public interface IShippingStrategy
{
    decimal CalculateCost(decimal orderTotal);
    string GetDescription();
}

public class StandardShipping : IShippingStrategy
{
    public decimal CalculateCost(decimal orderTotal) => orderTotal > 50 ? 0 : 5.99m;
    public string GetDescription() => "Standard (5-7 days)";
}

public class ExpressShipping : IShippingStrategy
{
    public decimal CalculateCost(decimal orderTotal) => 12.99m;
    public string GetDescription() => "Express (2-3 days)";
}

public class OvernightShipping : IShippingStrategy
{
    public decimal CalculateCost(decimal orderTotal) => 24.99m;
    public string GetDescription() => "Overnight (next day)";
}

public class Order
{
    private readonly decimal _cartTotal;
    private IShippingStrategy _shippingStrategy;

    public Order(decimal cartTotal, IShippingStrategy shippingStrategy)
    {
        _cartTotal = cartTotal;
        _shippingStrategy = shippingStrategy;
    }

    public void SetShippingStrategy(IShippingStrategy strategy)
    {
        _shippingStrategy = strategy;
    }

    public decimal CalculateTotal()
    {
        var shippingCost = _shippingStrategy.CalculateCost(_cartTotal);
        return _cartTotal + shippingCost;
    }
}

// BENCHMARK COMPARISON
// Approach     | Maintainability | Testability | Extensibility
// -------------|-----------------|-------------|---------------
// If/Else      | Low             | Hard        | Requires modification
// Strategy     | High            | Easy        | Just add new class
