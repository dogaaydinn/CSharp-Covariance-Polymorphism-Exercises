// Observer Pattern: Event-driven communication

namespace ObserverPattern;

public class Program
{
    public static void Main()
    {
        Console.WriteLine("=== Observer Pattern Demo ===\n");

        // ❌ BAD: Tight coupling
        Console.WriteLine("❌ BAD - Tight coupling:");
        var badStock = new BadStock("AAPL", 150.00m);
        badStock.Price = 155.00m; // No notifications

        // ✅ GOOD: Observer Pattern
        Console.WriteLine("\n✅ GOOD - Observer Pattern:");
        var stock = new Stock("AAPL", 150.00m);

        var investor1 = new Investor("Warren Buffett");
        var investor2 = new Investor("Peter Lynch");
        var alertSystem = new PriceAlertSystem();

        stock.Attach(investor1);
        stock.Attach(investor2);
        stock.Attach(alertSystem);

        stock.Price = 155.00m;
        stock.Price = 160.00m;

        stock.Detach(investor1);
        Console.WriteLine("\n--- Warren Buffett unsubscribed ---");
        stock.Price = 165.00m;

        Console.WriteLine("\n=== Observer Pattern Applied ===");
    }
}

// BEFORE (Bad)
public class BadStock
{
    public string Symbol { get; }
    private decimal _price;

    public BadStock(string symbol, decimal price)
    {
        Symbol = symbol;
        _price = price;
    }

    public decimal Price
    {
        get => _price;
        set
        {
            _price = value;
            // No way to notify interested parties!
        }
    }
}

// AFTER (Good)
public interface IObserver
{
    void Update(string symbol, decimal price);
}

public interface ISubject
{
    void Attach(IObserver observer);
    void Detach(IObserver observer);
    void Notify();
}

public class Stock : ISubject
{
    private readonly List<IObserver> _observers = new();
    private decimal _price;

    public string Symbol { get; }

    public Stock(string symbol, decimal price)
    {
        Symbol = symbol;
        _price = price;
    }

    public decimal Price
    {
        get => _price;
        set
        {
            if (_price != value)
            {
                _price = value;
                Notify();
            }
        }
    }

    public void Attach(IObserver observer)
    {
        _observers.Add(observer);
        Console.WriteLine($"✅ Observer attached: {observer.GetType().Name}");
    }

    public void Detach(IObserver observer)
    {
        _observers.Remove(observer);
        Console.WriteLine($"✅ Observer detached: {observer.GetType().Name}");
    }

    public void Notify()
    {
        Console.WriteLine($"\n📢 Stock {Symbol} price changed to ${Price:F2}");
        foreach (var observer in _observers)
        {
            observer.Update(Symbol, Price);
        }
    }
}

public class Investor : IObserver
{
    public string Name { get; }

    public Investor(string name)
    {
        Name = name;
    }

    public void Update(string symbol, decimal price)
    {
        Console.WriteLine($"  👤 {Name} notified: {symbol} is now ${price:F2}");
    }
}

public class PriceAlertSystem : IObserver
{
    private const decimal AlertThreshold = 160.00m;

    public void Update(string symbol, decimal price)
    {
        if (price > AlertThreshold)
        {
            Console.WriteLine($"  🚨 ALERT: {symbol} exceeded ${AlertThreshold} - now ${price:F2}");
        }
    }
}

// Modern C# with Events
public class ModernStock
{
    private decimal _price;

    public string Symbol { get; }
    public event EventHandler<PriceChangedEventArgs>? PriceChanged;

    public ModernStock(string symbol, decimal price)
    {
        Symbol = symbol;
        _price = price;
    }

    public decimal Price
    {
        get => _price;
        set
        {
            if (_price != value)
            {
                var oldPrice = _price;
                _price = value;
                OnPriceChanged(new PriceChangedEventArgs(Symbol, oldPrice, value));
            }
        }
    }

    protected virtual void OnPriceChanged(PriceChangedEventArgs e)
    {
        PriceChanged?.Invoke(this, e);
    }
}

public class PriceChangedEventArgs : EventArgs
{
    public string Symbol { get; }
    public decimal OldPrice { get; }
    public decimal NewPrice { get; }

    public PriceChangedEventArgs(string symbol, decimal oldPrice, decimal newPrice)
    {
        Symbol = symbol;
        OldPrice = oldPrice;
        NewPrice = newPrice;
    }
}
