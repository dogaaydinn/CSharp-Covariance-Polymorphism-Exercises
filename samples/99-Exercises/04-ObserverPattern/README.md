# Observer Pattern

## Problem
Implement observer pattern for event notifications.

## Example
```csharp
var stock = new Stock("AAPL", 150);
var investor1 = new Investor("Alice");
var investor2 = new Investor("Bob");

stock.Attach(investor1);
stock.Attach(investor2);

stock.Price = 155; // Both investors notified
```

## Components
- Subject (Observable): Maintains list of observers
- Observer: Receives notifications
- ConcreteSubject: Stock
- ConcreteObserver: Investor

## C# Alternative
Use events and delegates:
```csharp
public event EventHandler<PriceChangedEventArgs> PriceChanged;
```
