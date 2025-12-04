using System;
using System.Collections.Generic;
using System.Linq;

namespace DesignPatterns.Behavioral;

/// <summary>
/// STRATEGY PATTERN - Defines family of algorithms and makes them interchangeable
///
/// Problem:
/// - Need different variants of an algorithm
/// - Have many conditional statements switching between algorithms
/// - Want to isolate algorithm implementation from code that uses it
/// - Need to change algorithm at runtime
///
/// UML Structure:
/// ┌──────────────┐        ┌──────────────┐
/// │   Context    │───────>│   Strategy   │ (interface)
/// └──────────────┘        └──────────────┘
///                                △
///                                │ implements
///                    ┌───────────┼───────────┐
///             ┌──────┴──────┐    │    ┌──────┴──────┐
///             │ConcreteA    │    │    │ConcreteB    │
///             │Strategy     │    │    │Strategy     │
///             └─────────────┘    │    └─────────────┘
///                          ┌─────┴─────┐
///                          │ConcreteC  │
///                          │Strategy   │
///                          └───────────┘
///
/// When to Use:
/// - Many related classes differ only in behavior
/// - Need different variants of algorithm
/// - Algorithm uses data that clients shouldn't know about
/// - Class defines many behaviors as conditional statements
///
/// Benefits:
/// - Open/Closed Principle
/// - Runtime algorithm switching
/// - Isolates algorithm implementation
/// - Eliminates conditional statements
/// </summary>

#region Payment Strategy

/// <summary>
/// Strategy interface
/// </summary>
public interface IPaymentStrategy
{
    bool ProcessPayment(decimal amount);
    string GetPaymentMethod();
    bool ValidatePaymentDetails();
}

/// <summary>
/// Concrete strategy: Credit Card
/// </summary>
public class CreditCardPayment : IPaymentStrategy
{
    private readonly string _cardNumber;
    private readonly string _cvv;
    private readonly string _expiryDate;
    private readonly string _cardHolderName;

    public CreditCardPayment(string cardNumber, string cvv, string expiryDate, string cardHolderName)
    {
        _cardNumber = cardNumber;
        _cvv = cvv;
        _expiryDate = expiryDate;
        _cardHolderName = cardHolderName;
    }

    public bool ValidatePaymentDetails()
    {
        Console.WriteLine("  [Strategy] Validating credit card details...");

        // Simple validation
        if (_cardNumber.Length != 16)
        {
            Console.WriteLine("  [Strategy] Invalid card number");
            return false;
        }

        if (_cvv.Length != 3)
        {
            Console.WriteLine("  [Strategy] Invalid CVV");
            return false;
        }

        Console.WriteLine("  [Strategy] Credit card validated successfully");
        return true;
    }

    public bool ProcessPayment(decimal amount)
    {
        if (!ValidatePaymentDetails())
            return false;

        Console.WriteLine($"  [Strategy] Processing ${amount:F2} via Credit Card");
        Console.WriteLine($"  [Strategy] Card: ****-****-****-{_cardNumber.Substring(12)}");
        Console.WriteLine($"  [Strategy] Holder: {_cardHolderName}");
        Console.WriteLine("  [Strategy] Payment successful!");

        return true;
    }

    public string GetPaymentMethod() => "Credit Card";
}

/// <summary>
/// Concrete strategy: PayPal
/// </summary>
public class PayPalPayment : IPaymentStrategy
{
    private readonly string _email;
    private readonly string _password;

    public PayPalPayment(string email, string password)
    {
        _email = email;
        _password = password;
    }

    public bool ValidatePaymentDetails()
    {
        Console.WriteLine("  [Strategy] Validating PayPal credentials...");

        if (!_email.Contains("@"))
        {
            Console.WriteLine("  [Strategy] Invalid email");
            return false;
        }

        Console.WriteLine("  [Strategy] PayPal credentials validated");
        return true;
    }

    public bool ProcessPayment(decimal amount)
    {
        if (!ValidatePaymentDetails())
            return false;

        Console.WriteLine($"  [Strategy] Processing ${amount:F2} via PayPal");
        Console.WriteLine($"  [Strategy] Account: {_email}");
        Console.WriteLine("  [Strategy] Payment successful!");

        return true;
    }

    public string GetPaymentMethod() => "PayPal";
}

/// <summary>
/// Concrete strategy: Cryptocurrency
/// </summary>
public class CryptoPayment : IPaymentStrategy
{
    private readonly string _walletAddress;
    private readonly string _cryptocurrency;

    public CryptoPayment(string walletAddress, string cryptocurrency = "Bitcoin")
    {
        _walletAddress = walletAddress;
        _cryptocurrency = cryptocurrency;
    }

    public bool ValidatePaymentDetails()
    {
        Console.WriteLine("  [Strategy] Validating wallet address...");

        if (string.IsNullOrWhiteSpace(_walletAddress))
        {
            Console.WriteLine("  [Strategy] Invalid wallet address");
            return false;
        }

        Console.WriteLine("  [Strategy] Wallet address validated");
        return true;
    }

    public bool ProcessPayment(decimal amount)
    {
        if (!ValidatePaymentDetails())
            return false;

        Console.WriteLine($"  [Strategy] Processing ${amount:F2} via {_cryptocurrency}");
        Console.WriteLine($"  [Strategy] Wallet: {_walletAddress.Substring(0, 8)}...");
        Console.WriteLine("  [Strategy] Broadcasting transaction to blockchain...");
        Console.WriteLine("  [Strategy] Payment successful!");

        return true;
    }

    public string GetPaymentMethod() => $"Cryptocurrency ({_cryptocurrency})";
}

/// <summary>
/// Concrete strategy: Bank Transfer
/// </summary>
public class BankTransferPayment : IPaymentStrategy
{
    private readonly string _accountNumber;
    private readonly string _routingNumber;
    private readonly string _bankName;

    public BankTransferPayment(string accountNumber, string routingNumber, string bankName)
    {
        _accountNumber = accountNumber;
        _routingNumber = routingNumber;
        _bankName = bankName;
    }

    public bool ValidatePaymentDetails()
    {
        Console.WriteLine("  [Strategy] Validating bank account details...");
        Console.WriteLine("  [Strategy] Bank account validated");
        return true;
    }

    public bool ProcessPayment(decimal amount)
    {
        if (!ValidatePaymentDetails())
            return false;

        Console.WriteLine($"  [Strategy] Processing ${amount:F2} via Bank Transfer");
        Console.WriteLine($"  [Strategy] Bank: {_bankName}");
        Console.WriteLine($"  [Strategy] Account: ****{_accountNumber.Substring(_accountNumber.Length - 4)}");
        Console.WriteLine("  [Strategy] Transfer initiated (may take 1-3 business days)");
        Console.WriteLine("  [Strategy] Payment successful!");

        return true;
    }

    public string GetPaymentMethod() => "Bank Transfer";
}

/// <summary>
/// Context - Shopping Cart
/// </summary>
public class ShoppingCart
{
    private readonly List<(string name, decimal price)> _items = new();
    private IPaymentStrategy? _paymentStrategy;

    public void AddItem(string name, decimal price)
    {
        _items.Add((name, price));
        Console.WriteLine($"  [Strategy] Added to cart: {name} - ${price:F2}");
    }

    public void RemoveItem(string name)
    {
        var item = _items.FirstOrDefault(i => i.name == name);
        if (item != default)
        {
            _items.Remove(item);
            Console.WriteLine($"  [Strategy] Removed from cart: {name}");
        }
    }

    public void SetPaymentStrategy(IPaymentStrategy strategy)
    {
        _paymentStrategy = strategy ?? throw new ArgumentNullException(nameof(strategy));
        Console.WriteLine($"  [Strategy] Payment method set to: {strategy.GetPaymentMethod()}");
    }

    public decimal GetTotal() => _items.Sum(i => i.price);

    public void DisplayCart()
    {
        Console.WriteLine("  [Strategy] Shopping Cart:");
        foreach (var item in _items)
        {
            Console.WriteLine($"    - {item.name}: ${item.price:F2}");
        }
        Console.WriteLine($"    Total: ${GetTotal():F2}");
    }

    public bool Checkout()
    {
        if (_paymentStrategy == null)
        {
            Console.WriteLine("  [Strategy] Error: No payment method selected");
            return false;
        }

        if (_items.Count == 0)
        {
            Console.WriteLine("  [Strategy] Error: Cart is empty");
            return false;
        }

        Console.WriteLine();
        Console.WriteLine("  [Strategy] Processing checkout...");
        DisplayCart();
        Console.WriteLine();

        return _paymentStrategy.ProcessPayment(GetTotal());
    }
}

#endregion

#region Sorting Strategy

/// <summary>
/// Strategy interface for sorting
/// </summary>
public interface ISortStrategy<T>
{
    void Sort(List<T> data);
    string GetAlgorithmName();
}

/// <summary>
/// Concrete strategy: Bubble Sort
/// </summary>
public class BubbleSort : ISortStrategy<int>
{
    public void Sort(List<int> data)
    {
        Console.WriteLine("  [Strategy] Using Bubble Sort algorithm");
        int n = data.Count;
        for (int i = 0; i < n - 1; i++)
        {
            for (int j = 0; j < n - i - 1; j++)
            {
                if (data[j] > data[j + 1])
                {
                    (data[j], data[j + 1]) = (data[j + 1], data[j]);
                }
            }
        }
        Console.WriteLine("  [Strategy] Bubble sort completed");
    }

    public string GetAlgorithmName() => "Bubble Sort (O(n²))";
}

/// <summary>
/// Concrete strategy: Quick Sort
/// </summary>
public class QuickSort : ISortStrategy<int>
{
    public void Sort(List<int> data)
    {
        Console.WriteLine("  [Strategy] Using Quick Sort algorithm");
        QuickSortRecursive(data, 0, data.Count - 1);
        Console.WriteLine("  [Strategy] Quick sort completed");
    }

    private void QuickSortRecursive(List<int> data, int low, int high)
    {
        if (low < high)
        {
            int pi = Partition(data, low, high);
            QuickSortRecursive(data, low, pi - 1);
            QuickSortRecursive(data, pi + 1, high);
        }
    }

    private int Partition(List<int> data, int low, int high)
    {
        int pivot = data[high];
        int i = low - 1;

        for (int j = low; j < high; j++)
        {
            if (data[j] < pivot)
            {
                i++;
                (data[i], data[j]) = (data[j], data[i]);
            }
        }

        (data[i + 1], data[high]) = (data[high], data[i + 1]);
        return i + 1;
    }

    public string GetAlgorithmName() => "Quick Sort (O(n log n))";
}

/// <summary>
/// Concrete strategy: Merge Sort
/// </summary>
public class MergeSort : ISortStrategy<int>
{
    public void Sort(List<int> data)
    {
        Console.WriteLine("  [Strategy] Using Merge Sort algorithm");
        MergeSortRecursive(data, 0, data.Count - 1);
        Console.WriteLine("  [Strategy] Merge sort completed");
    }

    private void MergeSortRecursive(List<int> data, int left, int right)
    {
        if (left < right)
        {
            int middle = (left + right) / 2;
            MergeSortRecursive(data, left, middle);
            MergeSortRecursive(data, middle + 1, right);
            Merge(data, left, middle, right);
        }
    }

    private void Merge(List<int> data, int left, int middle, int right)
    {
        int n1 = middle - left + 1;
        int n2 = right - middle;

        var leftArray = new int[n1];
        var rightArray = new int[n2];

        Array.Copy(data.ToArray(), left, leftArray, 0, n1);
        Array.Copy(data.ToArray(), middle + 1, rightArray, 0, n2);

        int i = 0, j = 0, k = left;

        while (i < n1 && j < n2)
        {
            if (leftArray[i] <= rightArray[j])
            {
                data[k++] = leftArray[i++];
            }
            else
            {
                data[k++] = rightArray[j++];
            }
        }

        while (i < n1) data[k++] = leftArray[i++];
        while (j < n2) data[k++] = rightArray[j++];
    }

    public string GetAlgorithmName() => "Merge Sort (O(n log n))";
}

/// <summary>
/// Context for sorting
/// </summary>
public class DataSorter
{
    private ISortStrategy<int>? _sortStrategy;

    public void SetStrategy(ISortStrategy<int> strategy)
    {
        _sortStrategy = strategy ?? throw new ArgumentNullException(nameof(strategy));
        Console.WriteLine($"  [Strategy] Sort algorithm set to: {strategy.GetAlgorithmName()}");
    }

    public void SortData(List<int> data)
    {
        if (_sortStrategy == null)
        {
            throw new InvalidOperationException("Sort strategy not set");
        }

        Console.WriteLine($"  [Strategy] Before: [{string.Join(", ", data)}]");
        _sortStrategy.Sort(data);
        Console.WriteLine($"  [Strategy] After:  [{string.Join(", ", data)}]");
    }
}

#endregion

/// <summary>
/// Example demonstrating Strategy pattern
/// </summary>
public static class StrategyExample
{
    public static void Run()
    {
        Console.WriteLine();
        Console.WriteLine("7. STRATEGY PATTERN - Defines family of interchangeable algorithms");
        Console.WriteLine("-".PadRight(70, '-'));
        Console.WriteLine();

        // Example 1: Payment Strategies
        Console.WriteLine("Example 1: Payment Processing");
        Console.WriteLine();

        var cart = new ShoppingCart();
        cart.AddItem("Laptop", 999.99m);
        cart.AddItem("Mouse", 29.99m);
        cart.AddItem("Keyboard", 79.99m);

        Console.WriteLine();

        // Pay with credit card
        Console.WriteLine("  Checkout #1: Credit Card");
        cart.SetPaymentStrategy(new CreditCardPayment(
            "1234567890123456",
            "123",
            "12/25",
            "John Doe"
        ));
        cart.Checkout();

        Console.WriteLine();

        // Pay with PayPal
        Console.WriteLine("  Checkout #2: PayPal");
        cart.SetPaymentStrategy(new PayPalPayment("john.doe@example.com", "password123"));
        cart.Checkout();

        Console.WriteLine();

        // Pay with Cryptocurrency
        Console.WriteLine("  Checkout #3: Cryptocurrency");
        cart.SetPaymentStrategy(new CryptoPayment("1A1zP1eP5QGefi2DMPTfTL5SLmv7DivfNa", "Bitcoin"));
        cart.Checkout();

        Console.WriteLine();

        // Pay with Bank Transfer
        Console.WriteLine("  Checkout #4: Bank Transfer");
        cart.SetPaymentStrategy(new BankTransferPayment("123456789", "987654321", "Example Bank"));
        cart.Checkout();

        Console.WriteLine();

        // Example 2: Sorting Strategies
        Console.WriteLine("Example 2: Sorting Algorithms");
        Console.WriteLine();

        var sorter = new DataSorter();
        var data = new List<int> { 64, 34, 25, 12, 22, 11, 90 };

        Console.WriteLine("  Using Bubble Sort:");
        sorter.SetStrategy(new BubbleSort());
        sorter.SortData(new List<int>(data)); // Create copy to sort

        Console.WriteLine();

        Console.WriteLine("  Using Quick Sort:");
        sorter.SetStrategy(new QuickSort());
        sorter.SortData(new List<int>(data));

        Console.WriteLine();

        Console.WriteLine("  Using Merge Sort:");
        sorter.SetStrategy(new MergeSort());
        sorter.SortData(new List<int>(data));

        Console.WriteLine();
        Console.WriteLine("  Key Benefit: Change algorithms at runtime without modifying context!");
    }
}
