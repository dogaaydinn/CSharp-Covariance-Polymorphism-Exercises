namespace ObserverPattern;

// Stock data model
public class Stock
{
    public string Symbol { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public DateTime Timestamp { get; set; }

    public Stock(string symbol, decimal price)
    {
        Symbol = symbol;
        Price = price;
        Timestamp = DateTime.UtcNow;
    }
}

// TODO 1: StockTicker - Observable (subject)
public class StockTicker : IObservable<Stock>
{
    private List<IObserver<Stock>> _observers = new();
    private Stock? _currentStock;

    public IDisposable Subscribe(IObserver<Stock> observer)
    {
        // TODO: Add observer to list
        // Return an Unsubscriber object
        throw new NotImplementedException();
    }

    public void UpdatePrice(Stock stock)
    {
        // TODO: Update current stock and notify all observers
        throw new NotImplementedException();
    }

    public void StopUpdates()
    {
        // TODO: Call OnCompleted() on all observers
        throw new NotImplementedException();
    }

    private class Unsubscriber : IDisposable
    {
        private List<IObserver<Stock>> _observers;
        private IObserver<Stock> _observer;

        public Unsubscriber(List<IObserver<Stock>> observers, IObserver<Stock> observer)
        {
            _observers = observers;
            _observer = observer;
        }

        public void Dispose()
        {
            // TODO: Remove observer from list
            throw new NotImplementedException();
        }
    }
}

// TODO 2: StockObserver - Observer
public class StockObserver : IObserver<Stock>
{
    public string Name { get; }
    public List<Stock> ReceivedStocks { get; } = new();
    public bool IsCompleted { get; private set; }

    public StockObserver(string name)
    {
        Name = name;
    }

    public void OnNext(Stock value)
    {
        // TODO: Handle new stock update
        throw new NotImplementedException();
    }

    public void OnError(Exception error)
    {
        // TODO: Handle errors
        throw new NotImplementedException();
    }

    public void OnCompleted()
    {
        // TODO: Handle completion
        throw new NotImplementedException();
    }
}

// TODO 3: Event-based observer
public class EventBasedStockTicker
{
    public event EventHandler<Stock>? StockPriceChanged;

    public void UpdatePrice(Stock stock)
    {
        // TODO: Raise the event
        throw new NotImplementedException();
    }
}

// TODO 4: Threshold observer
public class ThresholdObserver : IObserver<Stock>
{
    private decimal _threshold;
    public List<Stock> TriggeredStocks { get; } = new();

    public ThresholdObserver(decimal threshold)
    {
        _threshold = threshold;
    }

    public void OnNext(Stock value)
    {
        // TODO: Only trigger if price exceeds threshold
        throw new NotImplementedException();
    }

    public void OnError(Exception error) { }
    public void OnCompleted() { }
}
