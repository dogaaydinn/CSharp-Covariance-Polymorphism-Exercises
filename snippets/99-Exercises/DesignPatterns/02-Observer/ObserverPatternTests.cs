using FluentAssertions;
using NUnit.Framework;

namespace ObserverPattern.Tests;

[TestFixture]
public class ObserverPatternTests
{
    [Test]
    public void StockTicker_Subscribe_ShouldAddObserver()
    {
        var ticker = new StockTicker();
        var observer = new StockObserver("Test");

        var subscription = ticker.Subscribe(observer);

        subscription.Should().NotBeNull();
    }

    [Test]
    public void StockTicker_UpdatePrice_ShouldNotifyObservers()
    {
        var ticker = new StockTicker();
        var observer = new StockObserver("Test");
        ticker.Subscribe(observer);
        var stock = new Stock("AAPL", 150.00m);

        ticker.UpdatePrice(stock);

        observer.ReceivedStocks.Should().Contain(stock);
    }

    [Test]
    public void StockObserver_OnNext_ShouldStoreStock()
    {
        var observer = new StockObserver("Test");
        var stock = new Stock("MSFT", 300.00m);

        observer.OnNext(stock);

        observer.ReceivedStocks.Should().HaveCount(1);
        observer.ReceivedStocks[0].Should().Be(stock);
    }

    [Test]
    public void StockObserver_OnCompleted_ShouldSetFlag()
    {
        var observer = new StockObserver("Test");

        observer.OnCompleted();

        observer.IsCompleted.Should().BeTrue();
    }

    [Test]
    public void StockTicker_Unsubscribe_ShouldStopNotifications()
    {
        var ticker = new StockTicker();
        var observer = new StockObserver("Test");
        var subscription = ticker.Subscribe(observer);

        subscription.Dispose();
        ticker.UpdatePrice(new Stock("GOOGL", 2800.00m));

        observer.ReceivedStocks.Should().BeEmpty();
    }

    [Test]
    public void EventBasedStockTicker_ShouldRaiseEvent()
    {
        var ticker = new EventBasedStockTicker();
        Stock? receivedStock = null;
        ticker.StockPriceChanged += (sender, stock) => receivedStock = stock;
        var stock = new Stock("TSLA", 700.00m);

        ticker.UpdatePrice(stock);

        receivedStock.Should().Be(stock);
    }

    [Test]
    public void ThresholdObserver_OnlyTriggersAboveThreshold()
    {
        var observer = new ThresholdObserver(100m);

        observer.OnNext(new Stock("LOW", 50m));
        observer.OnNext(new Stock("HIGH", 150m));

        observer.TriggeredStocks.Should().HaveCount(1);
        observer.TriggeredStocks[0].Symbol.Should().Be("HIGH");
    }

    [Test]
    public void MultipleObservers_AllReceiveUpdates()
    {
        var ticker = new StockTicker();
        var obs1 = new StockObserver("Observer1");
        var obs2 = new StockObserver("Observer2");
        ticker.Subscribe(obs1);
        ticker.Subscribe(obs2);
        var stock = new Stock("NFLX", 500.00m);

        ticker.UpdatePrice(stock);

        obs1.ReceivedStocks.Should().Contain(stock);
        obs2.ReceivedStocks.Should().Contain(stock);
    }

    [Test]
    public void StockTicker_StopUpdates_CompletesAllObservers()
    {
        var ticker = new StockTicker();
        var observer = new StockObserver("Test");
        ticker.Subscribe(observer);

        ticker.StopUpdates();

        observer.IsCompleted.Should().BeTrue();
    }

    [Test]
    public void ObserverPattern_Integration()
    {
        var ticker = new StockTicker();
        var observer = new StockObserver("Investor");
        ticker.Subscribe(observer);

        ticker.UpdatePrice(new Stock("AAPL", 150m));
        ticker.UpdatePrice(new Stock("MSFT", 300m));
        ticker.StopUpdates();

        observer.ReceivedStocks.Should().HaveCount(2);
        observer.IsCompleted.Should().BeTrue();
    }
}
