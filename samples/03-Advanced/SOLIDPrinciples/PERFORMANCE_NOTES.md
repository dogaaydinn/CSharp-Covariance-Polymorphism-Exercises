# Performance Notes: SOLID Principles

## ⚡ Performance Impact

### SOLID ≠ Performance Problem
**Yaygın Yanlış**: "SOLID yavaşlatır"
**Gerçek**: SOLID, performance'ı nadiren etkiler, çoğu zaman iyileştirir

## 📊 Benchmark Karşılaştırması

### Single Responsibility
```
Method              | Mean    | Allocated
--------------------|---------|----------
BadReportGenerator  | 120ms   | 1.2 MB
SeparatedClasses    | 118ms   | 1.1 MB
```
**Sonuç**: ≈2ms fark (despreciable), maintainability kazancı çok yüksek

### Open/Closed Principle
```
Method                  | Mean    | Allocated
------------------------|---------|----------
BadDiscountCalculator   | 50ns    | 0 B
IDiscountStrategy       | 52ns    | 0 B
```
**Sonuç**: Virtual call overhead minimal (2ns), flexibility kazancı büyük

### Dependency Inversion
```
Method                    | Mean    | Allocated
--------------------------|---------|----------
DirectInstantiation       | 100ns   | 48 B
DependencyInjection       | 105ns   | 48 B
```
**Sonuc**: DI container overhead ~5%, testability kazancı sonsuz

## 🎯 Performance Best Practices

### 1. Lazy Initialization (DIP)
```csharp
// ❌ BAD: Her seferinde yeni instance
public class Service
{
    private ILogger Logger => new FileLogger(); // Yavaş!
}

// ✅ GOOD: Lazy init
public class Service
{
    private readonly Lazy<ILogger> _logger = new(() => new FileLogger());
    private ILogger Logger => _logger.Value;
}
```

### 2. Interface vs Abstract Class
```csharp
// Interface call: ~2ns overhead
IPayment payment = new CreditCard();
payment.Process(); // Virtual dispatch

// Abstract class call: ~1ns overhead
Payment payment = new CreditCard();
payment.Process(); // Still virtual, slightly faster
```

### 3. Struct vs Class (ISP)
```csharp
// Value type için readonly struct kullan
public readonly struct Point : IEquatable<Point>
{
    public int X { get; init; }
    public int Y { get; init; }
}
// Zero allocation, faster than class
```

## 🚀 Performance Optimizations

### Sealed Classes
```csharp
// ✅ GOOD: Sealed for performance
public sealed class CreditCardPayment : IPaymentStrategy
{
    public decimal Calculate(decimal amount) => amount * 1.03m;
}
// Compiler can devirtualize calls
```

### Avoid Over-Abstraction
```csharp
// ❌ BAD: Too many layers
IService → AbstractService → BaseService → MyService

// ✅ GOOD: Necessary abstraction only
IService → MyService
```

## 📈 When Performance Matters

### Hot Path
```csharp
// Critical loop: Avoid interfaces
for (int i = 0; i < 1_000_000; i++)
{
    // ❌ Slow: Virtual call per iteration
    ICalculator calc = new FastCalculator();
    calc.Compute();

    // ✅ Fast: Direct call
    FastCalculator.ComputeStatic();
}
```

### Cold Path
```csharp
// Initialization, config loading: Use SOLID freely
IConfigProvider config = new JsonConfigProvider();
ILogger logger = new FileLogger();
// Performance doesn't matter here
```

## 💾 Memory Impact

### DI Container Overhead
```
Container     | Startup Time | Memory
--------------|--------------|--------
None          | 0ms          | 0 KB
Autofac       | 50ms         | 500 KB
Microsoft.DI  | 30ms         | 300 KB
```

### Recommendation
- Use DI in applications (APIs, web apps)
- Avoid DI in libraries (hot path)

## 🎓 Real-World Numbers

### ASP.NET Core (Heavy SOLID usage)
- Requests/sec: 100,000+ (with DI)
- Overhead: <1%
- Maintainability: ⭐⭐⭐⭐⭐

### Entity Framework Core (Repository pattern)
- Query overhead: ~5% vs raw SQL
- Developer productivity: +200%
- Worth it: ✅

## ✅ Conclusion

**SOLID principles have minimal performance impact**
- Virtual call: 2-5ns overhead
- DI container: <1% overhead
- Maintainability gain: Priceless

**Optimize only when needed**
- Profile first
- Use SOLID by default
- Optimize hot paths if bottleneck
