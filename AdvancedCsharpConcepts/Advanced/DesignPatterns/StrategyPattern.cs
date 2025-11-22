namespace AdvancedCsharpConcepts.Advanced.DesignPatterns;

/// <summary>
/// Strategy Pattern - Behavioral design pattern for interchangeable algorithms.
/// Silicon Valley best practice: Encapsulate algorithms and make them interchangeable.
/// </summary>
public static class StrategyPattern
{
    /// <summary>
    /// Strategy interface for different payment methods.
    /// </summary>
    public interface IPaymentStrategy
    {
        string Name { get; }
        bool ProcessPayment(decimal amount);
        bool ValidatePayment(decimal amount);
    }

    /// <summary>
    /// Concrete strategy: Credit Card payment.
    /// </summary>
    public class CreditCardPayment : IPaymentStrategy
    {
        public string CardNumber { get; }
        public string CVV { get; }
        public string Name { get; }

        public CreditCardPayment(string cardNumber, string cvv)
        {
            CardNumber = cardNumber ?? throw new ArgumentNullException(nameof(cardNumber));
            CVV = cvv ?? throw new ArgumentNullException(nameof(cvv));
            Name = "Credit Card";
        }

        public bool ValidatePayment(decimal amount)
        {
            if (amount <= 0)
                return false;

            // Simplified validation
            return CardNumber.Length >= 13 && CVV.Length == 3;
        }

        public bool ProcessPayment(decimal amount)
        {
            if (!ValidatePayment(amount))
                return false;

            Console.WriteLine($"Processing ${amount:F2} via Credit Card ending in {CardNumber[^4..]}");
            // Simulate processing
            return true;
        }
    }

    /// <summary>
    /// Concrete strategy: PayPal payment.
    /// </summary>
    public class PayPalPayment : IPaymentStrategy
    {
        public string Email { get; }
        public string Name { get; }

        public PayPalPayment(string email)
        {
            Email = email ?? throw new ArgumentNullException(nameof(email));
            Name = "PayPal";
        }

        public bool ValidatePayment(decimal amount)
        {
            if (amount <= 0)
                return false;

            // Simplified email validation
            return Email.Contains('@') && Email.Contains('.');
        }

        public bool ProcessPayment(decimal amount)
        {
            if (!ValidatePayment(amount))
                return false;

            Console.WriteLine($"Processing ${amount:F2} via PayPal account: {Email}");
            // Simulate processing
            return true;
        }
    }

    /// <summary>
    /// Concrete strategy: Cryptocurrency payment.
    /// </summary>
    public class CryptoPayment : IPaymentStrategy
    {
        public string WalletAddress { get; }
        public string CryptoType { get; }
        public string Name => $"{CryptoType} Crypto";

        public CryptoPayment(string walletAddress, string cryptoType = "Bitcoin")
        {
            WalletAddress = walletAddress ?? throw new ArgumentNullException(nameof(walletAddress));
            CryptoType = cryptoType ?? throw new ArgumentNullException(nameof(cryptoType));
        }

        public bool ValidatePayment(decimal amount)
        {
            if (amount <= 0)
                return false;

            // Simplified wallet address validation
            return WalletAddress.Length >= 26;
        }

        public bool ProcessPayment(decimal amount)
        {
            if (!ValidatePayment(amount))
                return false;

            Console.WriteLine($"Processing ${amount:F2} via {CryptoType} to wallet: {WalletAddress[..10]}...");
            // Simulate processing
            return true;
        }
    }

    /// <summary>
    /// Context class that uses a payment strategy.
    /// </summary>
    public class ShoppingCart
    {
        private readonly List<(string Item, decimal Price)> _items = new();
        private IPaymentStrategy? _paymentStrategy;

        public void AddItem(string item, decimal price)
        {
            if (string.IsNullOrWhiteSpace(item))
                throw new ArgumentNullException(nameof(item));
            if (price <= 0)
                throw new ArgumentException("Price must be positive", nameof(price));

            _items.Add((item, price));
        }

        public void SetPaymentStrategy(IPaymentStrategy strategy)
        {
            _paymentStrategy = strategy ?? throw new ArgumentNullException(nameof(strategy));
        }

        public decimal GetTotal()
        {
            return _items.Sum(item => item.Price);
        }

        public bool Checkout()
        {
            if (_paymentStrategy == null)
                throw new InvalidOperationException("Payment strategy not set");

            var total = GetTotal();
            Console.WriteLine($"\nCheckout Summary:");
            foreach (var (item, price) in _items)
            {
                Console.WriteLine($"  {item}: ${price:F2}");
            }
            Console.WriteLine($"Total: ${total:F2}");
            Console.WriteLine($"Payment Method: {_paymentStrategy.Name}");

            return _paymentStrategy.ProcessPayment(total);
        }
    }

    /// <summary>
    /// Demonstrates the Strategy Pattern.
    /// </summary>
    public static void RunExample()
    {
        Console.WriteLine("=== Strategy Pattern Examples ===\n");

        // Example 1: Credit Card payment
        Console.WriteLine("1. Credit Card Payment:");
        var cart1 = new ShoppingCart();
        cart1.AddItem("Laptop", 1299.99m);
        cart1.AddItem("Mouse", 49.99m);
        cart1.SetPaymentStrategy(new CreditCardPayment("1234567890123456", "123"));
        cart1.Checkout();

        // Example 2: PayPal payment
        Console.WriteLine("\n2. PayPal Payment:");
        var cart2 = new ShoppingCart();
        cart2.AddItem("Headphones", 199.99m);
        cart2.SetPaymentStrategy(new PayPalPayment("user@example.com"));
        cart2.Checkout();

        // Example 3: Cryptocurrency payment
        Console.WriteLine("\n3. Cryptocurrency Payment:");
        var cart3 = new ShoppingCart();
        cart3.AddItem("Graphics Card", 799.99m);
        cart3.SetPaymentStrategy(new CryptoPayment("1A1zP1eP5QGefi2DMPTfTL5SLmv7DivfNa", "Bitcoin"));
        cart3.Checkout();

        // Example 4: Switching strategies
        Console.WriteLine("\n4. Switching Payment Strategies:");
        var cart4 = new ShoppingCart();
        cart4.AddItem("Keyboard", 149.99m);

        // Try PayPal first
        cart4.SetPaymentStrategy(new PayPalPayment("buyer@example.com"));
        Console.WriteLine("Attempting PayPal...");
        cart4.Checkout();

        // Switch to Credit Card
        cart4.SetPaymentStrategy(new CreditCardPayment("9876543210987654", "456"));
        Console.WriteLine("\nSwitching to Credit Card...");
        cart4.Checkout();
    }
}
