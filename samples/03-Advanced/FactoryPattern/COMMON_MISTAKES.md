# Common Mistakes: Factory Pattern

## 1️⃣ Over-Engineering

### ❌ Mistake: Factory for Everything
```csharp
// Creating factory for every single class
public interface IUserFactory
{
    User CreateUser();
}

public interface IProductFactory
{
    Product CreateProduct();
}

public interface IOrderFactory
{
    Order CreateOrder();
}

// Too much ceremony for simple objects!
```

### ✅ Fix: Use Factory When Needed
```csharp
// Only create factory when:
// 1. Multiple implementations exist
// 2. Creation logic is complex
// 3. Runtime type selection needed

public class UserService
{
    public User CreateUser(string name, string email)
    {
        return new User(name, email); // Simple = no factory needed
    }
}
```

**Rule**: YAGNI (You Aren't Gonna Need It)

## 2️⃣ God Factory

### ❌ Mistake: One Factory for Everything
```csharp
public class ObjectFactory
{
    public object Create(string type)
    {
        return type switch
        {
            "user" => new User(),
            "product" => new Product(),
            "order" => new Order(),
            "payment" => new Payment(),
            // ... 50 more types
            _ => throw new ArgumentException()
        };
    }
}
// Violates Single Responsibility!
```

### ✅ Fix: Separate Factories
```csharp
public interface IUserFactory { User Create(); }
public interface IProductFactory { Product Create(); }
public interface IPaymentFactory { Payment Create(); }

// Each factory has one responsibility
```

## 3️⃣ Leaky Abstraction

### ❌ Mistake: Exposing Implementation Details
```csharp
public interface IDatabaseFactory
{
    SqlConnection CreateConnection(); // ❌ SQL-specific!
}

// Now clients know it's SQL Server
// Can't switch to PostgreSQL
```

### ✅ Fix: Generic Abstraction
```csharp
public interface IDatabaseFactory
{
    IDbConnection CreateConnection(); // ✅ Generic
}

// Works with any database
public class SqlServerFactory : IDatabaseFactory
{
    public IDbConnection CreateConnection()
        => new SqlConnection(_connectionString);
}

public class PostgreSqlFactory : IDatabaseFactory
{
    public IDbConnection CreateConnection()
        => new NpgsqlConnection(_connectionString);
}
```

## 4️⃣ Not Using DI

### ❌ Mistake: Manual Factory Selection
```csharp
public class OrderService
{
    private readonly IPaymentFactory _factory;

    public OrderService()
    {
        // Hardcoded factory selection!
        _factory = new CreditCardPaymentFactory();
    }
}
```

### ✅ Fix: Inject Factory
```csharp
public class OrderService
{
    private readonly IPaymentFactory _factory;

    public OrderService(IPaymentFactory factory) // DI
    {
        _factory = factory;
    }
}

// In Startup.cs:
services.AddSingleton<IPaymentFactory, CreditCardPaymentFactory>();
```

## 5️⃣ Forgetting to Dispose

### ❌ Mistake: Factory Creates IDisposable
```csharp
public class ConnectionFactory
{
    public IDbConnection Create()
    {
        return new SqlConnection(); // Who disposes this?
    }
}

// Usage:
var conn = factory.Create();
conn.Open();
// Forgot to dispose! Memory leak!
```

### ✅ Fix: Clear Ownership
```csharp
// Option 1: Caller owns disposal
public class ConnectionFactory
{
    public IDbConnection Create()
    {
        return new SqlConnection();
    }
}

// Caller responsibility:
using var conn = factory.Create();
conn.Open();

// Option 2: Factory manages lifecycle
public class ConnectionPool : IDisposable
{
    private readonly List<IDbConnection> _connections = new();

    public IDbConnection Rent()
    {
        var conn = new SqlConnection();
        _connections.Add(conn);
        return conn;
    }

    public void Dispose()
    {
        foreach (var conn in _connections)
        {
            conn.Dispose();
        }
    }
}
```

## 6️⃣ Using Reflection Unnecessarily

### ❌ Mistake: Reflection in Factory
```csharp
public class WidgetFactory
{
    public IWidget Create(string typeName)
    {
        var type = Type.GetType(typeName); // Slow!
        return (IWidget)Activator.CreateInstance(type);
    }
}
```

### ✅ Fix: Dictionary of Factories
```csharp
public class WidgetFactory
{
    private static readonly Dictionary<string, Func<IWidget>> _factories = new()
    {
        ["button"] = () => new Button(),
        ["textbox"] = () => new Textbox(),
        ["checkbox"] = () => new Checkbox()
    };

    public IWidget Create(string type)
    {
        return _factories[type](); // Fast!
    }
}
```

## 7️⃣ Not Thread-Safe Singleton

### ❌ Mistake: Non-Thread-Safe Factory
```csharp
public class ThemeFactory
{
    private static ThemeFactory? _instance;

    public static ThemeFactory Instance
    {
        get
        {
            if (_instance == null) // ❌ Race condition!
            {
                _instance = new ThemeFactory();
            }
            return _instance;
        }
    }
}
```

### ✅ Fix: Thread-Safe Singleton
```csharp
// Option 1: Lazy<T>
public class ThemeFactory
{
    private static readonly Lazy<ThemeFactory> _instance
        = new(() => new ThemeFactory());

    public static ThemeFactory Instance => _instance.Value;
}

// Option 2: Static constructor
public class ThemeFactory
{
    private static readonly ThemeFactory _instance = new();

    public static ThemeFactory Instance => _instance;
}
```

## 8️⃣ Mixing Factory Types

### ❌ Mistake: Confused Pattern Usage
```csharp
// Is this Simple Factory or Factory Method?
public abstract class ThemeFactory
{
    public static IButton CreateButton(string type) // Static = Simple Factory
    {
        return type switch
        {
            "dark" => new DarkButton(),
            _ => new LightButton()
        };
    }

    public abstract ITextbox CreateTextbox(); // Abstract = Factory Method
}
// Confusing! Pick one approach
```

### ✅ Fix: Be Consistent
```csharp
// Pure Factory Method
public abstract class ThemeFactory
{
    public abstract IButton CreateButton();
    public abstract ITextbox CreateTextbox();
}

public class DarkThemeFactory : ThemeFactory
{
    public override IButton CreateButton() => new DarkButton();
    public override ITextbox CreateTextbox() => new DarkTextbox();
}
```

## 9️⃣ Returning Nulls

### ❌ Mistake: Factory Returns Null
```csharp
public class PaymentFactory
{
    public IPayment? Create(string type)
    {
        return type switch
        {
            "credit" => new CreditCardPayment(),
            "paypal" => new PayPalPayment(),
            _ => null // ❌ Null reference exception waiting to happen!
        };
    }
}
```

### ✅ Fix: Throw or Return Default
```csharp
// Option 1: Throw exception
public class PaymentFactory
{
    public IPayment Create(string type)
    {
        return type switch
        {
            "credit" => new CreditCardPayment(),
            "paypal" => new PayPalPayment(),
            _ => throw new ArgumentException($"Unknown type: {type}")
        };
    }
}

// Option 2: Null Object Pattern
public class NullPayment : IPayment
{
    public void Process() { } // Do nothing
}

public class PaymentFactory
{
    public IPayment Create(string type)
    {
        return type switch
        {
            "credit" => new CreditCardPayment(),
            "paypal" => new PayPalPayment(),
            _ => new NullPayment()
        };
    }
}
```

## 🎓 Checklist

Before committing factory code:
- [ ] Factory gerçekten gerekli mi? (YAGNI)
- [ ] Her factory tek sorumluluk mu? (SRP)
- [ ] Abstraction implementation detayı sızdırmıyor mu?
- [ ] DI ile entegre edilmiş mi?
- [ ] IDisposable nesneler için ownership net mi?
- [ ] Thread-safe mi? (singleton için)
- [ ] Reflection yerine dictionary kullanılmış mı?
- [ ] Null dönmüyor mu?

## 💡 Pro Tips

1. **Start simple**: Direct instantiation → Simple Factory → Factory Method
2. **Refactor when needed**: İkinci implementation geldiğinde factory ekle
3. **Use DI**: Modern C#'ta DI container = factory replacement
4. **Document ownership**: Kim dispose edecek?
5. **Profile first**: Reflection gerçekten yavaşsa dictionary'ye geç

## 📚 Further Reading

- Design Patterns: Elements of Reusable Object-Oriented Software (Gang of Four)
- Clean Code by Robert Martin
- Dependency Injection Principles, Practices, and Patterns
