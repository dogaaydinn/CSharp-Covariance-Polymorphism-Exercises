using System.Collections.Concurrent;

namespace ObserverPattern;

/// <summary>
/// Observer Pattern Demo - Stock Market Notification System
///
/// Demonstrates:
/// - Push vs Pull models
/// - Event-based implementation
/// - Unsubscribing mechanism
/// - Real-time stock price tracking
/// - Multiple observer types
/// </summary>
class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("=== Observer Pattern Demo - Stock Market Notification System ===\n");

        DemonstrateBasicObserverPattern();
        Console.WriteLine("\n" + new string('=', 60) + "\n");

        DemonstratePushVsPullModel();
        Console.WriteLine("\n" + new string('=', 60) + "\n");

        DemonstrateEventBasedImplementation();
        Console.WriteLine("\n" + new string('=', 60) + "\n");

        DemonstrateMultipleObservers();
        Console.WriteLine("\n" + new string('=', 60) + "\n");

        DemonstrateUnsubscribing();
        Console.WriteLine("\n" + new string('=', 60) + "\n");

        DemonstrateRealTimeStockUpdates();
        Console.WriteLine("\n" + new string('=', 60) + "\n");

        DemonstrateProblemWithoutObserver();

        Console.WriteLine("\n=== Demo Complete ===");
    }

    /// <summary>
    /// 1. Basic Observer Pattern
    /// Subject (Stock) maintains list of observers and notifies them of changes
    /// </summary>
    static void DemonstrateBasicObserverPattern()
    {
        Console.WriteLine("1. BASIC OBSERVER PATTERN");
        Console.WriteLine("Subject notifies all registered observers when state changes\n");

        // Create stock (subject)
        var appleStock = new Stock("AAPL", 150.00m);

        // Create observers
        var trader = new StockTrader("John Trader");
        var analyst = new StockAnalyzer("Sarah Analyst");

        // Register observers
        appleStock.Attach(trader);
        appleStock.Attach(analyst);

        Console.WriteLine("Initial state:");
        Console.WriteLine($"   {appleStock.Symbol}: ${appleStock.Price:F2}\n");

        Console.WriteLine("Updating price to $155.00...\n");
        appleStock.Price = 155.00m;

        Console.WriteLine("\nUpdating price to $160.00...\n");
        appleStock.Price = 160.00m;

        Console.WriteLine("\n✓ All observers notified automatically");
    }

    /// <summary>
    /// 2. Push vs Pull Model
    /// Push: Subject sends data to observers
    /// Pull: Observers retrieve data from subject
    /// </summary>
    static void DemonstratePushVsPullModel()
    {
        Console.WriteLine("2. PUSH VS PULL MODEL");
        Console.WriteLine("Comparing two notification approaches\n");

        // Push Model
        Console.WriteLine("PUSH MODEL: Subject sends all data to observers");
        var pushStock = new PushStock("GOOGL", 2800.00m);
        var pushObserver = new PushStockObserver("Push Investor");
        pushStock.Attach(pushObserver);

        Console.WriteLine($"   Initial: {pushStock.Symbol} @ ${pushStock.Price:F2}");
        pushStock.UpdatePrice(2850.00m, 1500000);
        Console.WriteLine();

        // Pull Model
        Console.WriteLine("PULL MODEL: Observers retrieve data from subject");
        var pullStock = new PullStock("GOOGL", 2800.00m);
        var pullObserver = new PullStockObserver("Pull Investor", pullStock);
        pullStock.Attach(pullObserver);

        Console.WriteLine($"   Initial: {pullStock.Symbol} @ ${pullStock.Price:F2}");
        pullStock.UpdatePrice(2850.00m, 1500000);

        Console.WriteLine("\n✓ Push: More data sent, less work for observers");
        Console.WriteLine("✓ Pull: Less coupling, observers get only what they need");
    }

    /// <summary>
    /// 3. Event-Based Implementation
    /// Using C# events for loose coupling
    /// </summary>
    static void DemonstrateEventBasedImplementation()
    {
        Console.WriteLine("3. EVENT-BASED IMPLEMENTATION");
        Console.WriteLine("Using .NET events for decoupled notifications\n");

        var stock = new EventBasedStock("MSFT", 300.00m);

        // Subscribe with lambda expressions
        stock.PriceChanged += (sender, e) =>
        {
            Console.WriteLine($"   [Lambda] Price changed: {e.OldPrice:C} → {e.NewPrice:C} (Change: {e.ChangePercent:F2}%)");
        };

        // Subscribe with method
        void OnPriceAlert(object? sender, StockEventArgs e)
        {
            if (Math.Abs(e.ChangePercent) > 5)
            {
                Console.WriteLine($"   [Alert] SIGNIFICANT CHANGE: {e.ChangePercent:F2}% - Action required!");
            }
        }

        stock.PriceChanged += OnPriceAlert;

        // Subscribe with class-based handler
        var emailNotifier = new EmailNotifier("admin@stockmarket.com");
        stock.PriceChanged += emailNotifier.OnPriceChanged;

        Console.WriteLine("Price updates:");
        stock.Price = 315.00m; // 5% increase
        stock.Price = 330.00m; // 4.76% increase
        stock.Price = 350.00m; // 6.06% increase - triggers alert

        Console.WriteLine("\n✓ Events provide loose coupling and easy subscription management");
    }

    /// <summary>
    /// 4. Multiple Observers with Different Behaviors
    /// </summary>
    static void DemonstrateMultipleObservers()
    {
        Console.WriteLine("4. MULTIPLE OBSERVERS");
        Console.WriteLine("Different observers react differently to same event\n");

        var stock = new Stock("TSLA", 700.00m);

        // Different observer types
        var dayTrader = new DayTrader("Mike", buyThreshold: 680m, sellThreshold: 720m);
        var longTermInvestor = new LongTermInvestor("Alice", targetPrice: 800m);
        var riskManager = new RiskManager("Risk Dept", stopLoss: 650m);
        var priceDisplay = new PriceDisplayObserver();

        stock.Attach(dayTrader);
        stock.Attach(longTermInvestor);
        stock.Attach(riskManager);
        stock.Attach(priceDisplay);

        Console.WriteLine($"Starting price: ${stock.Price:F2}\n");

        Console.WriteLine("Price drops to $675 (testing buy threshold):");
        stock.Price = 675.00m;

        Console.WriteLine("\nPrice rises to $725 (testing sell threshold):");
        stock.Price = 725.00m;

        Console.WriteLine("\nPrice crashes to $640 (testing stop loss):");
        stock.Price = 640.00m;

        Console.WriteLine("\n✓ Each observer implements custom logic based on same notification");
    }

    /// <summary>
    /// 5. Unsubscribing Mechanism
    /// Observers can detach from subject
    /// </summary>
    static void DemonstrateUnsubscribing()
    {
        Console.WriteLine("5. UNSUBSCRIBING MECHANISM");
        Console.WriteLine("Observers can stop receiving notifications\n");

        var stock = new Stock("AMZN", 3200.00m);

        var observer1 = new StockTrader("Trader 1");
        var observer2 = new StockTrader("Trader 2");
        var observer3 = new StockTrader("Trader 3");

        // Subscribe all
        stock.Attach(observer1);
        stock.Attach(observer2);
        stock.Attach(observer3);

        Console.WriteLine("All observers subscribed:");
        stock.Price = 3250.00m;

        Console.WriteLine("\nTrader 2 unsubscribes:");
        stock.Detach(observer2);
        stock.Price = 3300.00m;

        Console.WriteLine("\nTrader 1 and 3 unsubscribe:");
        stock.Detach(observer1);
        stock.Detach(observer3);
        stock.Price = 3350.00m;
        Console.WriteLine("   (No observers - no notifications)");

        Console.WriteLine("\n✓ Dynamic subscription management prevents memory leaks");
    }

    /// <summary>
    /// 6. Real-Time Stock Updates Simulation
    /// </summary>
    static void DemonstrateRealTimeStockUpdates()
    {
        Console.WriteLine("6. REAL-TIME STOCK UPDATES");
        Console.WriteLine("Simulating live market data feed\n");

        var stockMarket = new StockMarket();

        // Add stocks
        var stocks = new[]
        {
            new Stock("AAPL", 150.00m),
            new Stock("GOOGL", 2800.00m),
            new Stock("MSFT", 300.00m)
        };

        foreach (var stock in stocks)
        {
            stockMarket.AddStock(stock);
        }

        // Create portfolio tracker
        var portfolio = new PortfolioTracker();
        portfolio.TrackStock(stocks[0], 100); // 100 shares of AAPL
        portfolio.TrackStock(stocks[1], 10);  // 10 shares of GOOGL
        portfolio.TrackStock(stocks[2], 50);  // 50 shares of MSFT

        Console.WriteLine("Initial Portfolio Value:");
        portfolio.DisplayPortfolio();

        Console.WriteLine("\nMarket Update #1:");
        stocks[0].Price = 155.00m; // AAPL +3.33%
        stocks[1].Price = 2850.00m; // GOOGL +1.79%
        stocks[2].Price = 295.00m;  // MSFT -1.67%

        Console.WriteLine("\nUpdated Portfolio Value:");
        portfolio.DisplayPortfolio();

        Console.WriteLine("\n✓ Observer pattern enables reactive portfolio management");
    }

    /// <summary>
    /// 7. Problem Without Observer Pattern
    /// </summary>
    static void DemonstrateProblemWithoutObserver()
    {
        Console.WriteLine("7. PROBLEM WITHOUT OBSERVER PATTERN");
        Console.WriteLine("Tight coupling and manual notification management\n");

        var legacySystem = new LegacyStockSystem();

        Console.WriteLine("❌ Legacy approach (tight coupling):");
        Console.WriteLine("```csharp");
        Console.WriteLine("public class LegacyStock");
        Console.WriteLine("{");
        Console.WriteLine("    private decimal _price;");
        Console.WriteLine("    private StockTrader _trader;    // ← Hard dependency");
        Console.WriteLine("    private StockAnalyzer _analyzer; // ← Hard dependency");
        Console.WriteLine("    private EmailService _email;     // ← Hard dependency");
        Console.WriteLine("");
        Console.WriteLine("    public void SetPrice(decimal price)");
        Console.WriteLine("    {");
        Console.WriteLine("        _price = price;");
        Console.WriteLine("        _trader.NotifyPriceChange(price);     // ← Manual call");
        Console.WriteLine("        _analyzer.NotifyPriceChange(price);   // ← Manual call");
        Console.WriteLine("        _email.SendPriceAlert(price);         // ← Manual call");
        Console.WriteLine("        // Must modify code to add new observers!");
        Console.WriteLine("    }");
        Console.WriteLine("}");
        Console.WriteLine("```\n");

        Console.WriteLine("Problems:");
        Console.WriteLine("   1. Tight coupling: Stock knows about all observers");
        Console.WriteLine("   2. Not extensible: Adding observer requires code change");
        Console.WriteLine("   3. Hard to test: Must mock all dependencies");
        Console.WriteLine("   4. Memory leaks: No unsubscribe mechanism");
        Console.WriteLine("   5. Code duplication: Manual notification everywhere\n");

        Console.WriteLine("✅ Observer pattern solves all these issues:");
        Console.WriteLine("   1. Loose coupling: Subject doesn't know observer types");
        Console.WriteLine("   2. Open/Closed: Add observers without modifying subject");
        Console.WriteLine("   3. Testable: Mock observers easily");
        Console.WriteLine("   4. Clean unsubscribe: Detach() method");
        Console.WriteLine("   5. DRY: Notification logic in one place");
    }
}

#region Observer Pattern Components

/// <summary>
/// Observer interface - Defines update method
/// </summary>
public interface IStockObserver
{
    void Update(Stock stock);
    string Name { get; }
}

/// <summary>
/// Subject class - Stock that notifies observers
/// </summary>
public class Stock
{
    private readonly List<IStockObserver> _observers = new();
    private decimal _price;

    public string Symbol { get; }
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

    public Stock(string symbol, decimal initialPrice)
    {
        Symbol = symbol;
        _price = initialPrice;
    }

    public void Attach(IStockObserver observer)
    {
        if (!_observers.Contains(observer))
        {
            _observers.Add(observer);
        }
    }

    public void Detach(IStockObserver observer)
    {
        _observers.Remove(observer);
    }

    protected void Notify()
    {
        foreach (var observer in _observers.ToList()) // ToList() for safe iteration
        {
            observer.Update(this);
        }
    }
}

/// <summary>
/// Concrete Observer - Stock Trader
/// </summary>
public class StockTrader : IStockObserver
{
    public string Name { get; }

    public StockTrader(string name)
    {
        Name = name;
    }

    public void Update(Stock stock)
    {
        Console.WriteLine($"   [{Name}] {stock.Symbol} price updated to ${stock.Price:F2}");
    }
}

/// <summary>
/// Concrete Observer - Stock Analyst
/// </summary>
public class StockAnalyzer : IStockObserver
{
    public string Name { get; }
    private decimal _previousPrice;

    public StockAnalyzer(string name)
    {
        Name = name;
    }

    public void Update(Stock stock)
    {
        if (_previousPrice > 0)
        {
            var changePercent = ((stock.Price - _previousPrice) / _previousPrice) * 100;
            var trend = changePercent > 0 ? "UP" : "DOWN";
            Console.WriteLine($"   [{Name}] Analysis: {stock.Symbol} is {trend} {Math.Abs(changePercent):F2}%");
        }
        _previousPrice = stock.Price;
    }
}

#endregion

#region Push vs Pull Models

/// <summary>
/// Push Model - Subject sends data to observers
/// </summary>
public class PushStock
{
    private readonly List<IPushStockObserver> _observers = new();
    private decimal _price;
    private long _volume;

    public string Symbol { get; }
    public decimal Price => _price;
    public long Volume => _volume;

    public PushStock(string symbol, decimal initialPrice)
    {
        Symbol = symbol;
        _price = initialPrice;
    }

    public void Attach(IPushStockObserver observer)
    {
        _observers.Add(observer);
    }

    public void Detach(IPushStockObserver observer)
    {
        _observers.Remove(observer);
    }

    public void UpdatePrice(decimal newPrice, long volume)
    {
        var oldPrice = _price;
        _price = newPrice;
        _volume = volume;

        // PUSH: Send all data to observers
        var data = new StockData
        {
            Symbol = Symbol,
            Price = _price,
            OldPrice = oldPrice,
            Volume = volume,
            Timestamp = DateTime.Now
        };

        foreach (var observer in _observers)
        {
            observer.Update(data); // ← Pushing data
        }
    }
}

public interface IPushStockObserver
{
    void Update(StockData data); // ← Receives all data
}

public class PushStockObserver : IPushStockObserver
{
    private readonly string _name;

    public PushStockObserver(string name) => _name = name;

    public void Update(StockData data)
    {
        var change = data.Price - data.OldPrice;
        Console.WriteLine($"   [{_name}] Received: {data.Symbol} ${data.Price:F2} (Δ{change:+0.00;-0.00}) Vol: {data.Volume:N0}");
    }
}

/// <summary>
/// Pull Model - Observers retrieve data from subject
/// </summary>
public class PullStock
{
    private readonly List<IPullStockObserver> _observers = new();
    private decimal _price;
    private long _volume;

    public string Symbol { get; }
    public decimal Price => _price;
    public long Volume => _volume;
    public DateTime LastUpdate { get; private set; }

    public PullStock(string symbol, decimal initialPrice)
    {
        Symbol = symbol;
        _price = initialPrice;
    }

    public void Attach(IPullStockObserver observer)
    {
        _observers.Add(observer);
    }

    public void Detach(IPullStockObserver observer)
    {
        _observers.Remove(observer);
    }

    public void UpdatePrice(decimal newPrice, long volume)
    {
        _price = newPrice;
        _volume = volume;
        LastUpdate = DateTime.Now;

        foreach (var observer in _observers)
        {
            observer.Update(); // ← No data sent, observer pulls
        }
    }
}

public interface IPullStockObserver
{
    void Update(); // ← No parameters, observer pulls data
}

public class PullStockObserver : IPullStockObserver
{
    private readonly string _name;
    private readonly PullStock _stock; // ← Reference to subject

    public PullStockObserver(string name, PullStock stock)
    {
        _name = name;
        _stock = stock;
    }

    public void Update()
    {
        // PULL: Observer retrieves only needed data
        var price = _stock.Price;
        var symbol = _stock.Symbol;
        Console.WriteLine($"   [{_name}] Pulled data: {symbol} ${price:F2}");
    }
}

public class StockData
{
    public string Symbol { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public decimal OldPrice { get; set; }
    public long Volume { get; set; }
    public DateTime Timestamp { get; set; }
}

#endregion

#region Event-Based Implementation

/// <summary>
/// Event-based stock using C# events
/// </summary>
public class EventBasedStock
{
    private decimal _price;

    public string Symbol { get; }
    public decimal Price
    {
        get => _price;
        set
        {
            if (_price != value)
            {
                var oldPrice = _price;
                _price = value;
                OnPriceChanged(new StockEventArgs
                {
                    Symbol = Symbol,
                    OldPrice = oldPrice,
                    NewPrice = value,
                    ChangePercent = ((value - oldPrice) / oldPrice) * 100,
                    Timestamp = DateTime.Now
                });
            }
        }
    }

    public event EventHandler<StockEventArgs>? PriceChanged;

    public EventBasedStock(string symbol, decimal initialPrice)
    {
        Symbol = symbol;
        _price = initialPrice;
    }

    protected virtual void OnPriceChanged(StockEventArgs e)
    {
        PriceChanged?.Invoke(this, e);
    }
}

public class StockEventArgs : EventArgs
{
    public string Symbol { get; set; } = string.Empty;
    public decimal OldPrice { get; set; }
    public decimal NewPrice { get; set; }
    public decimal ChangePercent { get; set; }
    public DateTime Timestamp { get; set; }
}

public class EmailNotifier
{
    private readonly string _email;

    public EmailNotifier(string email) => _email = email;

    public void OnPriceChanged(object? sender, StockEventArgs e)
    {
        if (sender is EventBasedStock stock)
        {
            Console.WriteLine($"   [Email to {_email}] {stock.Symbol}: {e.OldPrice:C} → {e.NewPrice:C}");
        }
    }
}

#endregion

#region Multiple Observer Types

/// <summary>
/// Day trader - Buys low, sells high
/// </summary>
public class DayTrader : IStockObserver
{
    public string Name { get; }
    private readonly decimal _buyThreshold;
    private readonly decimal _sellThreshold;

    public DayTrader(string name, decimal buyThreshold, decimal sellThreshold)
    {
        Name = name;
        _buyThreshold = buyThreshold;
        _sellThreshold = sellThreshold;
    }

    public void Update(Stock stock)
    {
        if (stock.Price <= _buyThreshold)
        {
            Console.WriteLine($"   [{Name}] 🟢 BUY SIGNAL: {stock.Symbol} @ ${stock.Price:F2} (threshold: ${_buyThreshold:F2})");
        }
        else if (stock.Price >= _sellThreshold)
        {
            Console.WriteLine($"   [{Name}] 🔴 SELL SIGNAL: {stock.Symbol} @ ${stock.Price:F2} (threshold: ${_sellThreshold:F2})");
        }
        else
        {
            Console.WriteLine($"   [{Name}] ⚪ HOLD: {stock.Symbol} @ ${stock.Price:F2}");
        }
    }
}

/// <summary>
/// Long-term investor - Monitors target price
/// </summary>
public class LongTermInvestor : IStockObserver
{
    public string Name { get; }
    private readonly decimal _targetPrice;

    public LongTermInvestor(string name, decimal targetPrice)
    {
        Name = name;
        _targetPrice = targetPrice;
    }

    public void Update(Stock stock)
    {
        var percentToTarget = ((stock.Price - _targetPrice) / _targetPrice) * 100;
        if (stock.Price >= _targetPrice)
        {
            Console.WriteLine($"   [{Name}] ✅ TARGET REACHED: {stock.Symbol} @ ${stock.Price:F2} (target: ${_targetPrice:F2})");
        }
        else
        {
            Console.WriteLine($"   [{Name}] 📊 Tracking: {stock.Symbol} @ ${stock.Price:F2} ({percentToTarget:F1}% from target)");
        }
    }
}

/// <summary>
/// Risk manager - Monitors stop loss
/// </summary>
public class RiskManager : IStockObserver
{
    public string Name { get; }
    private readonly decimal _stopLoss;

    public RiskManager(string name, decimal stopLoss)
    {
        Name = name;
        _stopLoss = stopLoss;
    }

    public void Update(Stock stock)
    {
        if (stock.Price <= _stopLoss)
        {
            Console.WriteLine($"   [{Name}] ⚠️  STOP LOSS TRIGGERED: {stock.Symbol} @ ${stock.Price:F2} (stop: ${_stopLoss:F2})");
        }
        else
        {
            var distance = ((stock.Price - _stopLoss) / _stopLoss) * 100;
            Console.WriteLine($"   [{Name}] ✓ Risk OK: {stock.Symbol} @ ${stock.Price:F2} ({distance:F1}% above stop loss)");
        }
    }
}

/// <summary>
/// Price display - Simple price monitor
/// </summary>
public class PriceDisplayObserver : IStockObserver
{
    public string Name => "Price Display";

    public void Update(Stock stock)
    {
        Console.WriteLine($"   [Display] {stock.Symbol}: ${stock.Price:F2}");
    }
}

#endregion

#region Real-Time Stock Market

/// <summary>
/// Stock market aggregator
/// </summary>
public class StockMarket
{
    private readonly List<Stock> _stocks = new();

    public void AddStock(Stock stock)
    {
        _stocks.Add(stock);
    }

    public void RemoveStock(Stock stock)
    {
        _stocks.Remove(stock);
    }

    public IReadOnlyList<Stock> GetStocks() => _stocks.AsReadOnly();
}

/// <summary>
/// Portfolio tracker - Monitors multiple stocks
/// </summary>
public class PortfolioTracker
{
    private readonly Dictionary<Stock, int> _holdings = new(); // Stock -> Shares
    private readonly Dictionary<Stock, decimal> _initialPrices = new();

    public void TrackStock(Stock stock, int shares)
    {
        _holdings[stock] = shares;
        _initialPrices[stock] = stock.Price;
        stock.Attach(new PortfolioObserver(this, stock));
    }

    public void DisplayPortfolio()
    {
        decimal totalValue = 0;
        decimal totalCost = 0;

        foreach (var (stock, shares) in _holdings)
        {
            var currentValue = stock.Price * shares;
            var initialValue = _initialPrices[stock] * shares;
            var gainLoss = currentValue - initialValue;
            var gainLossPercent = (gainLoss / initialValue) * 100;

            totalValue += currentValue;
            totalCost += initialValue;

            var sign = gainLoss >= 0 ? "+" : "";
            Console.WriteLine($"   {stock.Symbol}: {shares} shares @ ${stock.Price:F2} = ${currentValue:F2} ({sign}${gainLoss:F2} / {sign}{gainLossPercent:F2}%)");
        }

        var totalGainLoss = totalValue - totalCost;
        var totalGainLossPercent = (totalGainLoss / totalCost) * 100;
        var sign2 = totalGainLoss >= 0 ? "+" : "";

        Console.WriteLine($"   ---");
        Console.WriteLine($"   Total Value: ${totalValue:F2} ({sign2}${totalGainLoss:F2} / {sign2}{totalGainLossPercent:F2}%)");
    }

    private class PortfolioObserver : IStockObserver
    {
        private readonly PortfolioTracker _tracker;
        private readonly Stock _stock;

        public PortfolioObserver(PortfolioTracker tracker, Stock stock)
        {
            _tracker = tracker;
            _stock = stock;
        }

        public string Name => "Portfolio Tracker";

        public void Update(Stock stock)
        {
            // Silent update, portfolio recalculated on demand
        }
    }
}

#endregion

#region Legacy Code (Anti-Pattern)

/// <summary>
/// Legacy stock system without observer pattern
/// </summary>
public class LegacyStockSystem
{
    // Tight coupling example (not actually implemented to avoid complexity)
}

#endregion
