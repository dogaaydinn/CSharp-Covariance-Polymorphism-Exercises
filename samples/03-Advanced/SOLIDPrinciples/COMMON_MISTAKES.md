# Common Mistakes: SOLID Principles

## 1️⃣ Single Responsibility Principle

### ❌ Mistake: "God Object"
```csharp
public class UserManager
{
    public void Register(User user) { }
    public void SendEmail(string email) { }
    public void LogActivity(string activity) { }
    public void ValidateInput(string input) { }
    public void SaveToDatabase(User user) { }
    // TOO MANY RESPONSIBILITIES!
}
```

### ✅ Fix: Separate Classes
```csharp
public class UserRegistration { }
public class EmailService { }
public class ActivityLogger { }
public class InputValidator { }
public class UserRepository { }
```

## 2️⃣ Open/Closed Principle

### ❌ Mistake: Modifying Existing Code
```csharp
public class DiscountCalculator
{
    public decimal Calculate(string type, decimal amount)
    {
        if (type == "Student") return amount * 0.8m;
        if (type == "Senior") return amount * 0.7m;
        // Adding "VIP" requires modifying this method!
        if (type == "VIP") return amount * 0.5m;
        return amount;
    }
}
```

### ✅ Fix: Extension via Inheritance
```csharp
public interface IDiscount
{
    decimal Apply(decimal amount);
}

public class VipDiscount : IDiscount
{
    public decimal Apply(decimal amount) => amount * 0.5m;
}
// Add new discount without touching existing code
```

## 3️⃣ Liskov Substitution Principle

### ❌ Mistake: Breaking Parent Contract
```csharp
public class Rectangle
{
    public virtual int Width { get; set; }
    public virtual int Height { get; set; }
}

public class Square : Rectangle
{
    public override int Width
    {
        set { base.Width = base.Height = value; } // Breaks LSP!
    }
}

// Usage breaks:
Rectangle rect = new Square();
rect.Width = 5;
rect.Height = 10;
// Square is now 10x10, not 5x10!
```

### ✅ Fix: Separate Abstractions
```csharp
public interface IShape
{
    int Area();
}

public class Rectangle : IShape
{
    public int Width { get; set; }
    public int Height { get; set; }
    public int Area() => Width * Height;
}

public class Square : IShape
{
    public int Side { get; set; }
    public int Area() => Side * Side;
}
```

## 4️⃣ Interface Segregation Principle

### ❌ Mistake: Fat Interfaces
```csharp
public interface IWorker
{
    void Work();
    void Eat();
    void Sleep();
}

public class Robot : IWorker
{
    public void Work() { }
    public void Eat() => throw new NotImplementedException(); // ❌
    public void Sleep() => throw new NotImplementedException(); // ❌
}
```

### ✅ Fix: Role Interfaces
```csharp
public interface IWorkable { void Work(); }
public interface IEatable { void Eat(); }
public interface ISleepable { void Sleep(); }

public class Robot : IWorkable
{
    public void Work() { } // Only what it needs
}

public class Human : IWorkable, IEatable, ISleepable
{
    public void Work() { }
    public void Eat() { }
    public void Sleep() { }
}
```

## 5️⃣ Dependency Inversion Principle

### ❌ Mistake: Direct Dependencies
```csharp
public class OrderService
{
    private readonly SqlDatabase _database = new SqlDatabase();
    private readonly EmailSender _email = new EmailSender();
    // Tightly coupled to concrete classes!

    public void PlaceOrder(Order order)
    {
        _database.Save(order);
        _email.Send(order.Customer.Email, "Order placed");
    }
}
```

### ✅ Fix: Depend on Abstractions
```csharp
public interface IDatabase { void Save(Order order); }
public interface IEmailService { void Send(string to, string message); }

public class OrderService
{
    private readonly IDatabase _database;
    private readonly IEmailService _email;

    public OrderService(IDatabase database, IEmailService email)
    {
        _database = database;
        _email = email;
    }

    public void PlaceOrder(Order order)
    {
        _database.Save(order);
        _email.Send(order.Customer.Email, "Order placed");
    }
}
```

## 🚨 Common Anti-Patterns

### 1. Over-Engineering
```csharp
// ❌ TOO MUCH for a simple app
IAbstractFactoryProviderManager<IServiceLocatorFactory>
```
**Fix**: YAGNI (You Aren't Gonna Need It) - başla basit, refactor et gerektiğinde

### 2. Leaky Abstractions
```csharp
public interface IDataAccess
{
    SqlConnection GetConnection(); // ❌ SQL detayı sızdırıyor!
}
```
**Fix**: Generic abstraction
```csharp
public interface IDataAccess
{
    Task<T> GetByIdAsync<T>(int id);
}
```

### 3. New in Constructors
```csharp
public class Service
{
    private readonly ILogger _logger = new FileLogger(); // ❌ DIP ihlali
}
```
**Fix**: Constructor injection
```csharp
public class Service
{
    private readonly ILogger _logger;
    public Service(ILogger logger) => _logger = logger;
}
```

## 📋 Checklist

Before committing:
- [ ] Her class tek sorumluluk mu?
- [ ] Yeni özellik için mevcut class'ı değiştirdin mi?
- [ ] Alt sınıf, üst sınıf gibi kullanılabilir mi?
- [ ] Interface'de kullanılmayan metod var mı?
- [ ] Concrete class'a direkt bağımlılık var mı?

## 💡 Pro Tips

1. **Start simple**: Her projede tüm SOLID'i uygulama
2. **Refactor gradually**: Kod büyüdükçe SOLID uygula
3. **Test first**: TDD ile SOLID otomatik gelir
4. **Code review**: SOLID ihlallerini yakalama pratiği yap

## 📚 Further Reading

- Clean Code by Robert Martin
- Refactoring by Martin Fowler
- Design Patterns by Gang of Four
