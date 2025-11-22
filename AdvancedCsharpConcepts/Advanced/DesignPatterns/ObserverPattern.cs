namespace AdvancedCsharpConcepts.Advanced.DesignPatterns;

/// <summary>
/// Observer Pattern - Behavioral design pattern for event notification.
/// NVIDIA best practice: Asynchronous event-driven architecture.
/// </summary>
public static class ObserverPattern
{
    /// <summary>
    /// Observer interface for receiving updates.
    /// </summary>
    public interface IObserver<T>
    {
        string Name { get; }
        void Update(T data);
    }

    /// <summary>
    /// Subject interface for managing observers.
    /// </summary>
    public interface ISubject<T>
    {
        void Attach(IObserver<T> observer);
        void Detach(IObserver<T> observer);
        void Notify(T data);
    }

    /// <summary>
    /// Stock data model.
    /// </summary>
    public record StockData(string Symbol, decimal Price, DateTime Timestamp)
    {
        public decimal Change { get; init; }
        public decimal ChangePercent { get; init; }
    }

    /// <summary>
    /// Concrete subject: Stock market ticker.
    /// </summary>
    public class StockTicker : ISubject<StockData>
    {
        private readonly List<IObserver<StockData>> _observers = new();
        private readonly Dictionary<string, decimal> _lastPrices = new();

        public void Attach(IObserver<StockData> observer)
        {
            if (observer == null)
                throw new ArgumentNullException(nameof(observer));

            if (!_observers.Contains(observer))
            {
                _observers.Add(observer);
                Console.WriteLine($"[StockTicker] Observer '{observer.Name}' attached. Total observers: {_observers.Count}");
            }
        }

        public void Detach(IObserver<StockData> observer)
        {
            if (observer == null)
                throw new ArgumentNullException(nameof(observer));

            if (_observers.Remove(observer))
            {
                Console.WriteLine($"[StockTicker] Observer '{observer.Name}' detached. Total observers: {_observers.Count}");
            }
        }

        public void Notify(StockData data)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            Console.WriteLine($"\n[StockTicker] Broadcasting update for {data.Symbol}: ${data.Price:F2}");
            Console.WriteLine($"Notifying {_observers.Count} observer(s)...");

            foreach (var observer in _observers)
            {
                try
                {
                    observer.Update(data);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[StockTicker] Error notifying {observer.Name}: {ex.Message}");
                }
            }
        }

        public void UpdateStock(string symbol, decimal price)
        {
            if (string.IsNullOrWhiteSpace(symbol))
                throw new ArgumentNullException(nameof(symbol));
            if (price < 0)
                throw new ArgumentException("Price cannot be negative", nameof(price));

            var lastPrice = _lastPrices.GetValueOrDefault(symbol, price);
            var change = price - lastPrice;
            var changePercent = lastPrice > 0 ? (change / lastPrice) * 100 : 0;

            var stockData = new StockData(symbol, price, DateTime.Now)
            {
                Change = change,
                ChangePercent = changePercent
            };

            _lastPrices[symbol] = price;
            Notify(stockData);
        }
    }

    /// <summary>
    /// Concrete observer: Mobile app notification.
    /// </summary>
    public class MobileApp : IObserver<StockData>
    {
        public string Name { get; }
        private readonly decimal _priceAlertThreshold;

        public MobileApp(string userName, decimal priceAlertThreshold = 0)
        {
            Name = $"MobileApp-{userName}";
            _priceAlertThreshold = priceAlertThreshold;
        }

        public void Update(StockData data)
        {
            Console.WriteLine($"  📱 [{Name}] Stock Update: {data.Symbol} @ ${data.Price:F2} " +
                            $"({(data.Change >= 0 ? "+" : "")}{data.ChangePercent:F2}%)");

            if (_priceAlertThreshold > 0 && Math.Abs(data.ChangePercent) >= _priceAlertThreshold)
            {
                Console.WriteLine($"  🔔 [{Name}] ALERT: {data.Symbol} changed by {data.ChangePercent:F2}%!");
            }
        }
    }

    /// <summary>
    /// Concrete observer: Email notification service.
    /// </summary>
    public class EmailNotifier : IObserver<StockData>
    {
        public string Name { get; }
        private readonly string _emailAddress;
        private readonly List<string> _watchList;

        public EmailNotifier(string emailAddress, params string[] watchList)
        {
            Name = $"EmailNotifier-{emailAddress}";
            _emailAddress = emailAddress;
            _watchList = new List<string>(watchList);
        }

        public void Update(StockData data)
        {
            if (_watchList.Contains(data.Symbol))
            {
                Console.WriteLine($"  📧 [{Name}] Sending email to {_emailAddress}:");
                Console.WriteLine($"      Subject: {data.Symbol} Price Alert");
                Console.WriteLine($"      Body: {data.Symbol} is now ${data.Price:F2} " +
                                $"({(data.Change >= 0 ? "↑" : "↓")} {Math.Abs(data.Change):F2})");
            }
        }
    }

    /// <summary>
    /// Concrete observer: Analytics dashboard.
    /// </summary>
    public class AnalyticsDashboard : IObserver<StockData>
    {
        public string Name { get; } = "AnalyticsDashboard";
        private readonly Dictionary<string, List<decimal>> _priceHistory = new();

        public void Update(StockData data)
        {
            if (!_priceHistory.ContainsKey(data.Symbol))
            {
                _priceHistory[data.Symbol] = new List<decimal>();
            }

            _priceHistory[data.Symbol].Add(data.Price);

            var history = _priceHistory[data.Symbol];
            var average = history.Average();
            var min = history.Min();
            var max = history.Max();

            Console.WriteLine($"  📊 [{Name}] Analytics for {data.Symbol}:");
            Console.WriteLine($"      Current: ${data.Price:F2} | Avg: ${average:F2} | " +
                            $"Min: ${min:F2} | Max: ${max:F2} | Samples: {history.Count}");
        }

        public Dictionary<string, List<decimal>> GetHistory() => _priceHistory;
    }

    /// <summary>
    /// Demonstrates the Observer Pattern.
    /// </summary>
    public static void RunExample()
    {
        Console.WriteLine("=== Observer Pattern Examples ===\n");

        // Create subject
        var stockTicker = new StockTicker();

        // Create observers
        var mobileApp1 = new MobileApp("Alice", priceAlertThreshold: 5.0m);
        var mobileApp2 = new MobileApp("Bob", priceAlertThreshold: 10.0m);
        var emailNotifier = new EmailNotifier("investor@example.com", "AAPL", "GOOGL");
        var dashboard = new AnalyticsDashboard();

        // Attach observers
        Console.WriteLine("--- Attaching Observers ---");
        stockTicker.Attach(mobileApp1);
        stockTicker.Attach(mobileApp2);
        stockTicker.Attach(emailNotifier);
        stockTicker.Attach(dashboard);

        // Simulate stock updates
        Console.WriteLine("\n--- Stock Updates ---");
        stockTicker.UpdateStock("AAPL", 150.00m);
        stockTicker.UpdateStock("GOOGL", 2800.00m);
        stockTicker.UpdateStock("AAPL", 145.50m);  // -3% change
        stockTicker.UpdateStock("TSLA", 700.00m);
        stockTicker.UpdateStock("AAPL", 160.00m);  // +9.97% change - triggers alert!

        // Detach an observer
        Console.WriteLine("\n--- Detaching Observer ---");
        stockTicker.Detach(mobileApp2);

        // More updates
        Console.WriteLine("\n--- More Updates ---");
        stockTicker.UpdateStock("GOOGL", 2900.00m);
        stockTicker.UpdateStock("AAPL", 155.00m);

        // Display analytics summary
        Console.WriteLine("\n--- Analytics Summary ---");
        var history = dashboard.GetHistory();
        foreach (var (symbol, prices) in history)
        {
            Console.WriteLine($"{symbol}: {prices.Count} updates, " +
                            $"Range: ${prices.Min():F2} - ${prices.Max():F2}");
        }
    }
}
