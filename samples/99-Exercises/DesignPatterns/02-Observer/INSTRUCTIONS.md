# Observer Pattern Exercise

## 🎯 Learning Objectives
- **Observer Pattern**: One-to-many dependency
- **IObservable<T>**: .NET's built-in observer interface
- **Event-driven**: Publish-subscribe architecture
- **Decoupling**: Subjects and observers are independent

## 📋 Exercise Tasks (Models.cs)

1. **StockTicker** - Implement IObservable<Stock>
2. **StockObserver** - Implement IObserver<Stock>
3. **EventBasedStockTicker** - Using C# events
4. **ThresholdObserver** - Conditional observer

## 🚀 Getting Started

```bash
cd samples/99-Exercises/DesignPatterns/02-Observer
dotnet test  # Should see 10 FAILED tests
```

## 💡 Solution Hints

See full implementations in Models.cs TODO comments.

**Key Methods**:
- Subscribe(): Add observer, return IDisposable
- OnNext(): Handle new data
- OnCompleted(): Handle stream completion

## ✅ Success: All 10 tests pass!
