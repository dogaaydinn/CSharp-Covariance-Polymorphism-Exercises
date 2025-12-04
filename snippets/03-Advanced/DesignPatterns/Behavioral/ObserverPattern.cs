using System;
using System.Collections.Generic;

namespace DesignPatterns.Behavioral;

/// <summary>
/// OBSERVER PATTERN - Defines one-to-many dependency where state changes notify all dependents
///
/// Problem:
/// - Objects need to be notified when another object changes
/// - Want loose coupling between subject and observers
/// - Unknown number of observers need updates
/// - Need pub/sub or event handling mechanism
///
/// UML Structure:
/// ┌──────────────┐        ┌──────────────┐
/// │   Subject    │───────>│   Observer   │ (interface)
/// ├──────────────┤        └──────────────┘
/// │ +Attach()    │               △
/// │ +Detach()    │               │ implements
/// │ +Notify()    │               │
/// └──────────────┘        ┌──────┴──────┬──────────┐
///        △                │ ConcreteA   │ConcreteB │
///        │                │ Observer    │Observer  │
/// ┌──────┴──────┐         └─────────────┴──────────┘
/// │  Concrete   │
/// │  Subject    │
/// └─────────────┘
///
/// When to Use:
/// - Change in one object requires changing others
/// - Object should notify others without knowing who they are
/// - Need event-driven architecture
/// - Implement pub/sub systems
///
/// Benefits:
/// - Open/Closed Principle
/// - Establishes relationships at runtime
/// - Loose coupling between subject and observers
/// - Dynamic subscription management
/// </summary>

#region Weather Station Example

/// <summary>
/// Observer interface
/// </summary>
public interface IWeatherObserver
{
    void Update(float temperature, float humidity, float pressure);
    string GetObserverName();
}

/// <summary>
/// Subject interface
/// </summary>
public interface IWeatherSubject
{
    void Attach(IWeatherObserver observer);
    void Detach(IWeatherObserver observer);
    void Notify();
}

/// <summary>
/// Concrete Subject - Weather Station
/// </summary>
public class WeatherStation : IWeatherSubject
{
    private readonly List<IWeatherObserver> _observers = new();
    private float _temperature;
    private float _humidity;
    private float _pressure;

    public void Attach(IWeatherObserver observer)
    {
        if (!_observers.Contains(observer))
        {
            _observers.Add(observer);
            Console.WriteLine($"  [Observer] {observer.GetObserverName()} subscribed to weather updates");
        }
    }

    public void Detach(IWeatherObserver observer)
    {
        if (_observers.Remove(observer))
        {
            Console.WriteLine($"  [Observer] {observer.GetObserverName()} unsubscribed from weather updates");
        }
    }

    public void Notify()
    {
        Console.WriteLine($"  [Observer] Notifying {_observers.Count} observer(s)...");
        foreach (var observer in _observers)
        {
            observer.Update(_temperature, _humidity, _pressure);
        }
    }

    public void SetMeasurements(float temperature, float humidity, float pressure)
    {
        Console.WriteLine();
        Console.WriteLine($"  [Observer] Weather changed: {temperature}°C, {humidity}%, {pressure} hPa");
        _temperature = temperature;
        _humidity = humidity;
        _pressure = pressure;
        Notify();
    }

    public (float temperature, float humidity, float pressure) GetMeasurements()
        => (_temperature, _humidity, _pressure);
}

/// <summary>
/// Concrete Observer - Current Conditions Display
/// </summary>
public class CurrentConditionsDisplay : IWeatherObserver
{
    private readonly string _name;
    private float _temperature;
    private float _humidity;

    public CurrentConditionsDisplay(string name)
    {
        _name = name;
    }

    public void Update(float temperature, float humidity, float pressure)
    {
        _temperature = temperature;
        _humidity = humidity;
        Console.WriteLine($"  [Observer] {_name}: Current conditions - {_temperature}°C, {_humidity}% humidity");
    }

    public string GetObserverName() => _name;
}

/// <summary>
/// Concrete Observer - Statistics Display
/// </summary>
public class StatisticsDisplay : IWeatherObserver
{
    private readonly string _name;
    private readonly List<float> _temperatureHistory = new();
    private float _maxTemp = float.MinValue;
    private float _minTemp = float.MaxValue;

    public StatisticsDisplay(string name)
    {
        _name = name;
    }

    public void Update(float temperature, float humidity, float pressure)
    {
        _temperatureHistory.Add(temperature);

        if (temperature > _maxTemp) _maxTemp = temperature;
        if (temperature < _minTemp) _minTemp = temperature;

        float avgTemp = _temperatureHistory.Sum() / _temperatureHistory.Count;

        Console.WriteLine($"  [Observer] {_name}: Avg: {avgTemp:F1}°C, Max: {_maxTemp:F1}°C, Min: {_minTemp:F1}°C");
    }

    public string GetObserverName() => _name;
}

/// <summary>
/// Concrete Observer - Forecast Display
/// </summary>
public class ForecastDisplay : IWeatherObserver
{
    private readonly string _name;
    private float _lastPressure;
    private float _currentPressure;

    public ForecastDisplay(string name)
    {
        _name = name;
    }

    public void Update(float temperature, float humidity, float pressure)
    {
        _lastPressure = _currentPressure;
        _currentPressure = pressure;

        string forecast = (_currentPressure > _lastPressure)
            ? "Improving weather on the way!"
            : (_currentPressure < _lastPressure)
                ? "Watch out for cooler, rainy weather"
                : "More of the same";

        Console.WriteLine($"  [Observer] {_name}: Forecast - {forecast} (Pressure: {_currentPressure} hPa)");
    }

    public string GetObserverName() => _name;
}

#endregion

#region Stock Market Example

/// <summary>
/// Observer interface for stock updates
/// </summary>
public interface IStockObserver
{
    void OnPriceChange(string symbol, decimal price, decimal change);
    string GetInvestorName();
}

/// <summary>
/// Subject - Stock Exchange
/// </summary>
public class StockExchange
{
    private readonly Dictionary<string, List<IStockObserver>> _observers = new();
    private readonly Dictionary<string, decimal> _stockPrices = new();

    public void Subscribe(string symbol, IStockObserver observer)
    {
        if (!_observers.ContainsKey(symbol))
        {
            _observers[symbol] = new List<IStockObserver>();
        }

        if (!_observers[symbol].Contains(observer))
        {
            _observers[symbol].Add(observer);
            Console.WriteLine($"  [Observer] {observer.GetInvestorName()} subscribed to {symbol}");
        }
    }

    public void Unsubscribe(string symbol, IStockObserver observer)
    {
        if (_observers.ContainsKey(symbol) && _observers[symbol].Remove(observer))
        {
            Console.WriteLine($"  [Observer] {observer.GetInvestorName()} unsubscribed from {symbol}");
        }
    }

    public void UpdateStockPrice(string symbol, decimal newPrice)
    {
        decimal oldPrice = _stockPrices.ContainsKey(symbol) ? _stockPrices[symbol] : newPrice;
        decimal change = newPrice - oldPrice;
        _stockPrices[symbol] = newPrice;

        Console.WriteLine();
        Console.WriteLine($"  [Observer] Stock Update: {symbol} = ${newPrice:F2} (Change: {change:+0.00;-0.00;0.00})");

        if (_observers.ContainsKey(symbol))
        {
            Console.WriteLine($"  [Observer] Notifying {_observers[symbol].Count} subscriber(s)...");
            foreach (var observer in _observers[symbol])
            {
                observer.OnPriceChange(symbol, newPrice, change);
            }
        }
    }

    public decimal GetStockPrice(string symbol)
    {
        return _stockPrices.ContainsKey(symbol) ? _stockPrices[symbol] : 0m;
    }
}

/// <summary>
/// Concrete Observer - Individual Investor
/// </summary>
public class Investor : IStockObserver
{
    private readonly string _name;
    private readonly Dictionary<string, int> _portfolio = new();

    public Investor(string name)
    {
        _name = name;
    }

    public void AddToPortfolio(string symbol, int shares)
    {
        _portfolio[symbol] = shares;
    }

    public void OnPriceChange(string symbol, decimal price, decimal change)
    {
        if (_portfolio.ContainsKey(symbol))
        {
            int shares = _portfolio[symbol];
            decimal totalValue = shares * price;
            decimal totalChange = shares * change;

            Console.WriteLine($"  [Observer] {_name}: {symbol} portfolio value = ${totalValue:F2} " +
                            $"(Change: {totalChange:+$0.00;-$0.00;$0.00})");
        }
    }

    public string GetInvestorName() => _name;
}

/// <summary>
/// Concrete Observer - Trading Bot
/// </summary>
public class TradingBot : IStockObserver
{
    private readonly string _botName;
    private readonly decimal _buyThreshold;
    private readonly decimal _sellThreshold;

    public TradingBot(string botName, decimal buyThreshold, decimal sellThreshold)
    {
        _botName = botName;
        _buyThreshold = buyThreshold;
        _sellThreshold = sellThreshold;
    }

    public void OnPriceChange(string symbol, decimal price, decimal change)
    {
        if (change <= _buyThreshold)
        {
            Console.WriteLine($"  [Observer] {_botName}: AUTO BUY signal for {symbol} at ${price:F2}");
        }
        else if (change >= _sellThreshold)
        {
            Console.WriteLine($"  [Observer] {_botName}: AUTO SELL signal for {symbol} at ${price:F2}");
        }
        else
        {
            Console.WriteLine($"  [Observer] {_botName}: HOLD {symbol} at ${price:F2}");
        }
    }

    public string GetInvestorName() => _botName;
}

#endregion

#region Event-Driven Example

/// <summary>
/// Observer interface for events
/// </summary>
public interface IEventObserver
{
    void OnEvent(string eventType, string message);
    string GetListenerName();
}

/// <summary>
/// Subject - Event Manager (Pub/Sub system)
/// </summary>
public class EventManager
{
    private readonly Dictionary<string, List<IEventObserver>> _subscribers = new();

    public void Subscribe(string eventType, IEventObserver observer)
    {
        if (!_subscribers.ContainsKey(eventType))
        {
            _subscribers[eventType] = new List<IEventObserver>();
        }

        if (!_subscribers[eventType].Contains(observer))
        {
            _subscribers[eventType].Add(observer);
            Console.WriteLine($"  [Observer] {observer.GetListenerName()} subscribed to '{eventType}' events");
        }
    }

    public void Unsubscribe(string eventType, IEventObserver observer)
    {
        if (_subscribers.ContainsKey(eventType) && _subscribers[eventType].Remove(observer))
        {
            Console.WriteLine($"  [Observer] {observer.GetListenerName()} unsubscribed from '{eventType}' events");
        }
    }

    public void Publish(string eventType, string message)
    {
        Console.WriteLine();
        Console.WriteLine($"  [Observer] Publishing '{eventType}' event: {message}");

        if (_subscribers.ContainsKey(eventType))
        {
            Console.WriteLine($"  [Observer] Notifying {_subscribers[eventType].Count} subscriber(s)...");
            foreach (var observer in _subscribers[eventType])
            {
                observer.OnEvent(eventType, message);
            }
        }
        else
        {
            Console.WriteLine("  [Observer] No subscribers for this event type");
        }
    }

    public int GetSubscriberCount(string eventType)
    {
        return _subscribers.ContainsKey(eventType) ? _subscribers[eventType].Count : 0;
    }
}

/// <summary>
/// Concrete Observer - Email Notifier
/// </summary>
public class EmailNotifier : IEventObserver
{
    private readonly string _email;

    public EmailNotifier(string email)
    {
        _email = email;
    }

    public void OnEvent(string eventType, string message)
    {
        Console.WriteLine($"  [Observer] EmailNotifier: Sending email to {_email}");
        Console.WriteLine($"    Subject: {eventType}");
        Console.WriteLine($"    Body: {message}");
    }

    public string GetListenerName() => $"EmailNotifier({_email})";
}

/// <summary>
/// Concrete Observer - Logger
/// </summary>
public class EventLogger : IEventObserver
{
    private readonly string _logName;
    private readonly List<string> _logs = new();

    public EventLogger(string logName)
    {
        _logName = logName;
    }

    public void OnEvent(string eventType, string message)
    {
        string logEntry = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {eventType}: {message}";
        _logs.Add(logEntry);
        Console.WriteLine($"  [Observer] EventLogger: {logEntry}");
    }

    public string GetListenerName() => $"EventLogger({_logName})";

    public List<string> GetLogs() => new List<string>(_logs);
}

/// <summary>
/// Concrete Observer - SMS Notifier
/// </summary>
public class SmsNotifier : IEventObserver
{
    private readonly string _phoneNumber;

    public SmsNotifier(string phoneNumber)
    {
        _phoneNumber = phoneNumber;
    }

    public void OnEvent(string eventType, string message)
    {
        Console.WriteLine($"  [Observer] SmsNotifier: Sending SMS to {_phoneNumber}");
        Console.WriteLine($"    Message: [{eventType}] {message}");
    }

    public string GetListenerName() => $"SmsNotifier({_phoneNumber})";
}

#endregion

/// <summary>
/// Example demonstrating Observer pattern
/// </summary>
public static class ObserverExample
{
    public static void Run()
    {
        Console.WriteLine();
        Console.WriteLine("8. OBSERVER PATTERN - Notifies multiple objects of state changes");
        Console.WriteLine("-".PadRight(70, '-'));
        Console.WriteLine();

        // Example 1: Weather Station
        Console.WriteLine("Example 1: Weather Station");
        Console.WriteLine();

        var weatherStation = new WeatherStation();

        var currentDisplay = new CurrentConditionsDisplay("Current Display");
        var statsDisplay = new StatisticsDisplay("Statistics Display");
        var forecastDisplay = new ForecastDisplay("Forecast Display");

        weatherStation.Attach(currentDisplay);
        weatherStation.Attach(statsDisplay);
        weatherStation.Attach(forecastDisplay);

        weatherStation.SetMeasurements(25.5f, 65f, 1013.1f);
        weatherStation.SetMeasurements(27.8f, 70f, 1012.5f);

        Console.WriteLine();
        weatherStation.Detach(statsDisplay);

        weatherStation.SetMeasurements(22.3f, 90f, 1008.2f);

        Console.WriteLine();

        // Example 2: Stock Market
        Console.WriteLine("Example 2: Stock Market");
        Console.WriteLine();

        var stockExchange = new StockExchange();

        var investor1 = new Investor("Alice");
        investor1.AddToPortfolio("AAPL", 100);
        investor1.AddToPortfolio("GOOGL", 50);

        var investor2 = new Investor("Bob");
        investor2.AddToPortfolio("AAPL", 200);

        var tradingBot = new TradingBot("AutoTrader", -5m, 5m);

        stockExchange.Subscribe("AAPL", investor1);
        stockExchange.Subscribe("AAPL", investor2);
        stockExchange.Subscribe("AAPL", tradingBot);
        stockExchange.Subscribe("GOOGL", investor1);

        stockExchange.UpdateStockPrice("AAPL", 150.00m);
        stockExchange.UpdateStockPrice("AAPL", 145.00m);
        stockExchange.UpdateStockPrice("GOOGL", 2800.00m);

        Console.WriteLine();

        // Example 3: Event-Driven System
        Console.WriteLine("Example 3: Event-Driven Notification System");
        Console.WriteLine();

        var eventManager = new EventManager();

        var emailNotifier = new EmailNotifier("admin@example.com");
        var logger = new EventLogger("system.log");
        var smsNotifier = new SmsNotifier("+1-555-0123");

        // Subscribe to different events
        eventManager.Subscribe("user.registered", emailNotifier);
        eventManager.Subscribe("user.registered", logger);

        eventManager.Subscribe("order.placed", emailNotifier);
        eventManager.Subscribe("order.placed", smsNotifier);
        eventManager.Subscribe("order.placed", logger);

        eventManager.Subscribe("system.error", logger);
        eventManager.Subscribe("system.error", smsNotifier);

        // Publish events
        eventManager.Publish("user.registered", "New user: john@example.com");
        eventManager.Publish("order.placed", "Order #12345 - Total: $99.99");
        eventManager.Publish("system.error", "Database connection failed");

        Console.WriteLine();
        Console.WriteLine("  Key Benefit: Loose coupling - subjects don't know about observers!");
    }
}
