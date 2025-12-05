# Why Factory Pattern? - Deep Dive

## Table of Contents

1. [What is the Factory Pattern?](#what-is-the-factory-pattern)
2. [The Problem: Why We Need Factories](#the-problem-why-we-need-factories)
3. [Three Factory Pattern Variations](#three-factory-pattern-variations)
4. [Why Use Factory Pattern?](#why-use-factory-pattern)
5. [When to Use (and When NOT to Use)](#when-to-use-and-when-not-to-use)
6. [Real-World Scenarios](#real-world-scenarios)
7. [Performance Implications](#performance-implications)
8. [Common Mistakes and How to Avoid Them](#common-mistakes-and-how-to-avoid-them)
9. [Migration Strategies](#migration-strategies)
10. [Comparison with Other Patterns](#comparison-with-other-patterns)
11. [Advanced Techniques](#advanced-techniques)

---

## What is the Factory Pattern?

The **Factory Pattern** is a creational design pattern that provides an interface for creating objects, but lets subclasses or factory classes decide which class to instantiate. It encapsulates object creation logic, promoting loose coupling and adhering to SOLID principles.

### The Core Idea

Instead of:
```csharp
// Direct instantiation - tight coupling
var theme = new DarkTheme();
var button = new DarkButton();
```

Use:
```csharp
// Factory Pattern - loose coupling
ITheme theme = factory.CreateTheme();
IButton button = factory.CreateButton();
```

### Three Main Variations

1. **Simple Factory** (not a GoF pattern, but widely used)
   - Static method creates objects based on parameters
   - Simplest form, minimal boilerplate

2. **Factory Method** (GoF pattern)
   - Abstract method in base class
   - Subclasses override to create specific types

3. **Abstract Factory** (GoF pattern)
   - Creates families of related objects
   - Multiple factory methods in one interface

---

## The Problem: Why We Need Factories

### Problem 1: Tight Coupling

**Without Factory**:
```csharp
public class Application
{
    private DarkButton _button;        // Tight coupling to concrete class
    private DarkTextbox _textbox;      // Tight coupling to concrete class
    private DarkPanel _panel;          // Tight coupling to concrete class

    public Application()
    {
        _button = new DarkButton();    // Hard-coded instantiation
        _textbox = new DarkTextbox();
        _panel = new DarkPanel();
    }

    public void ChangeToLightTheme()
    {
        // Problem: Must change all instantiations!
        _button = new LightButton();     // ❌ Violates Open/Closed
        _textbox = new LightTextbox();   // ❌ Violates Open/Closed
        _panel = new LightPanel();       // ❌ Violates Open/Closed
    }
}
```

**Issues**:
- ❌ Application knows about concrete classes (DarkButton, LightButton)
- ❌ Adding new theme requires modifying Application class
- ❌ Violates Open/Closed Principle
- ❌ Hard to test (can't mock concrete classes)
- ❌ Tight coupling makes changes risky

**With Factory**:
```csharp
public class Application
{
    private readonly IUIFactory _factory;
    private IButton _button;
    private ITextbox _textbox;
    private IPanel _panel;

    public Application(IUIFactory factory)  // Dependency Injection
    {
        _factory = factory;
        _button = _factory.CreateButton();
        _textbox = _factory.CreateTextbox();
        _panel = _factory.CreatePanel();
    }

    public void ChangeTheme(IUIFactory newFactory)
    {
        _factory = newFactory;
        _button = _factory.CreateButton();     // ✅ No code changes needed
        _textbox = _factory.CreateTextbox();   // ✅ Works with any factory
        _panel = _factory.CreatePanel();       // ✅ Open/Closed compliant
    }
}

// Usage
var app = new Application(new DarkUIFactory());
app.ChangeTheme(new LightUIFactory());  // Easy theme switching
```

**Benefits**:
- ✅ Application depends on interfaces (IUIFactory, IButton)
- ✅ Adding new theme doesn't modify Application
- ✅ Follows Open/Closed Principle
- ✅ Easy to test (mock IUIFactory)
- ✅ Loose coupling enables flexibility

### Problem 2: Complex Creation Logic

**Without Factory**:
```csharp
public ITheme CreateTheme(string type, bool accessibility, int fontSize)
{
    // Complex creation logic scattered throughout codebase
    if (accessibility && fontSize > 16)
    {
        return new HighContrastTheme
        {
            BackgroundColor = "#000000",
            TextColor = "#FFFF00",
            FontSize = fontSize,
            // ... 50 more properties
        };
    }
    else if (type == "dark")
    {
        return new DarkTheme
        {
            BackgroundColor = "#1E1E1E",
            // ... complex initialization
        };
    }
    // ... more complex logic
}
```

**Issues**:
- ❌ Creation logic mixed with business logic
- ❌ Difficult to maintain
- ❌ Hard to test independently
- ❌ Violates Single Responsibility Principle

**With Factory**:
```csharp
public class ConfigurableThemeFactory
{
    private readonly ThemeConfiguration _config;

    public ConfigurableThemeFactory(ThemeConfiguration config)
    {
        _config = config;
    }

    public ITheme CreateConfiguredTheme()
    {
        // Centralized, testable creation logic
        if (_config.Accessibility == AccessibilityLevel.High)
        {
            return CreateHighContrastTheme();
        }

        return _config.ThemeType switch
        {
            ThemeType.Dark => CreateDarkTheme(),
            ThemeType.Light => CreateLightTheme(),
            _ => CreateDefaultTheme()
        };
    }

    private ITheme CreateHighContrastTheme()
    {
        // Encapsulated creation logic
        return new HighContrastTheme
        {
            BackgroundColor = "#000000",
            TextColor = "#FFFF00",
            FontSize = _config.FontSize,
            // Centralized configuration
        };
    }
}
```

**Benefits**:
- ✅ Creation logic centralized in factory
- ✅ Easy to maintain and test
- ✅ Follows Single Responsibility Principle
- ✅ Configuration-driven creation

### Problem 3: Inconsistent Object Families

**Without Factory**:
```csharp
// Mixing incompatible components - no compile-time safety
var theme = new DarkTheme();
var button = new LightButton();    // ❌ Inconsistent! Light button with Dark theme
var panel = new DarkPanel();

// Visual inconsistency at runtime
```

**With Abstract Factory**:
```csharp
IUIFactory factory = new DarkUIFactory();

// All components guaranteed to be compatible
var theme = factory.CreateTheme();     // DarkTheme
var button = factory.CreateButton();   // DarkButton
var panel = factory.CreatePanel();     // DarkPanel

// ✅ Consistency enforced by design
```

**Benefits**:
- ✅ Enforces consistency across related objects
- ✅ Compile-time safety
- ✅ Prevents mixing incompatible components

---

## Three Factory Pattern Variations

### 1. Simple Factory

**What it is**: A single class (often static) with methods that create objects based on parameters.

**Structure**:
```csharp
public static class SimpleThemeFactory
{
    public static ITheme CreateTheme(ThemeType type)
    {
        return type switch
        {
            ThemeType.Dark => new DarkTheme(),
            ThemeType.Light => new LightTheme(),
            ThemeType.HighContrast => new HighContrastTheme(),
            _ => throw new ArgumentException($"Unknown type: {type}")
        };
    }
}
```

**Pros**:
- ✅ Simple to implement and understand
- ✅ Centralizes creation logic
- ✅ Easy to use: `var theme = SimpleThemeFactory.CreateTheme(ThemeType.Dark);`
- ✅ Good for limited, stable product types

**Cons**:
- ❌ Violates Open/Closed Principle (must modify to add types)
- ❌ Can become a God Object if handles too many types
- ❌ Not easily extensible

**When to Use**:
- You have 2-5 product types
- Product types rarely change
- Creation logic is straightforward
- You value simplicity over extensibility

**Real-World Example**:
```csharp
// .NET Framework's DbProviderFactory (simplified)
public static class DbProviderFactory
{
    public static DbConnection CreateConnection(string provider)
    {
        return provider.ToLower() switch
        {
            "sqlserver" => new SqlConnection(),
            "postgresql" => new NpgsqlConnection(),
            "mysql" => new MySqlConnection(),
            _ => throw new ArgumentException($"Unknown provider: {provider}")
        };
    }
}
```

### 2. Factory Method

**What it is**: An abstract method in a base class that subclasses override to create specific objects.

**Structure**:
```csharp
// Abstract Creator
public abstract class ThemeCreator
{
    // Factory Method - subclasses implement
    public abstract ITheme CreateTheme();

    // Template method using factory method
    public void RenderUI()
    {
        var theme = CreateTheme();  // Calls overridden method
        theme.Apply();
        Console.WriteLine($"UI rendered with {theme.Name}");
    }
}

// Concrete Creators
public class DarkThemeCreator : ThemeCreator
{
    public override ITheme CreateTheme() => new DarkTheme();
}

public class LightThemeCreator : ThemeCreator
{
    public override ITheme CreateTheme() => new LightTheme();
}
```

**Pros**:
- ✅ Follows Open/Closed Principle (add types without modifying existing code)
- ✅ Single Responsibility: Each creator has one job
- ✅ Extensible: Add new creators easily
- ✅ Works well with Template Method pattern

**Cons**:
- ❌ More classes to maintain (one per product type)
- ❌ Can be over-engineering for simple cases
- ❌ More complex than Simple Factory

**When to Use**:
- You expect new product types to be added
- You want to follow SOLID principles strictly
- Product creation involves complex logic that varies per type
- You need extensibility

**Real-World Example**:
```csharp
// ASP.NET Core's ILoggerProvider (simplified)
public abstract class LoggerProvider : ILoggerProvider
{
    public abstract ILogger CreateLogger(string categoryName);

    public void Dispose()
    {
        // Common cleanup logic
    }
}

public class ConsoleLoggerProvider : LoggerProvider
{
    public override ILogger CreateLogger(string categoryName)
        => new ConsoleLogger(categoryName);
}

public class FileLoggerProvider : LoggerProvider
{
    public override ILogger CreateLogger(string categoryName)
        => new FileLogger(categoryName);
}
```

### 3. Abstract Factory

**What it is**: An interface for creating families of related objects without specifying their concrete classes.

**Structure**:
```csharp
// Abstract Factory Interface
public interface IUIFactory
{
    ITheme CreateTheme();
    IButton CreateButton();
    IPanel CreatePanel();
}

// Concrete Factory for Dark theme family
public class DarkUIFactory : IUIFactory
{
    public ITheme CreateTheme() => new DarkTheme();
    public IButton CreateButton() => new DarkButton();
    public IPanel CreatePanel() => new DarkPanel();
}

// Concrete Factory for Light theme family
public class LightUIFactory : IUIFactory
{
    public ITheme CreateTheme() => new LightTheme();
    public IButton CreateButton() => new LightButton();
    public IPanel CreatePanel() => new LightPanel();
}

// Client code
public class Application
{
    private readonly IUIFactory _factory;

    public Application(IUIFactory factory)
    {
        _factory = factory;
    }

    public void BuildUI()
    {
        var theme = _factory.CreateTheme();
        var button = _factory.CreateButton();
        var panel = _factory.CreatePanel();

        // All components guaranteed to be compatible
        theme.Apply();
        button.Render();
        panel.Render();
    }
}
```

**Pros**:
- ✅ Ensures consistency across product families
- ✅ Follows Open/Closed Principle
- ✅ Isolates concrete classes from client code
- ✅ Easy to switch entire product families
- ✅ Enforces compatibility constraints

**Cons**:
- ❌ Most complex of the three patterns
- ❌ Many interfaces and classes
- ❌ Adding new products to all families requires updating all factories
- ❌ Can be over-engineering for simple scenarios

**When to Use**:
- Products must work together as a family
- You have multiple related products
- You need to enforce consistency
- You want to support multiple variants (themes, platforms, etc.)

**Real-World Example**:
```csharp
// Cross-platform UI framework (simplified)
public interface IUIFactory
{
    IButton CreateButton();
    ITextBox CreateTextBox();
    ICheckBox CreateCheckBox();
}

public class WindowsUIFactory : IUIFactory
{
    public IButton CreateButton() => new WindowsButton();
    public ITextBox CreateTextBox() => new WindowsTextBox();
    public ICheckBox CreateCheckBox() => new WindowsCheckBox();
}

public class MacUIFactory : IUIFactory
{
    public IButton CreateButton() => new MacButton();
    public ITextBox CreateTextBox() => new MacTextBox();
    public ICheckBox CreateCheckBox() => new MacCheckBox();
}

// Usage
IUIFactory factory = GetPlatformFactory();  // Windows or Mac
var app = new Application(factory);
app.BuildUI();  // All components match the platform
```

---

## Why Use Factory Pattern?

### Benefit 1: Loose Coupling

**Before (Tight Coupling)**:
```csharp
public class OrderProcessor
{
    private SqlServerRepository _repository;  // ❌ Tight coupling

    public OrderProcessor()
    {
        _repository = new SqlServerRepository();  // ❌ Hard-coded
    }

    public void ProcessOrder(Order order)
    {
        _repository.Save(order);
    }
}

// Problem: Can't switch to PostgreSQL without changing OrderProcessor
```

**After (Loose Coupling with Factory)**:
```csharp
public class OrderProcessor
{
    private readonly IRepository _repository;  // ✅ Interface

    public OrderProcessor(IRepositoryFactory factory)
    {
        _repository = factory.CreateRepository();  // ✅ Factory decides
    }

    public void ProcessOrder(Order order)
    {
        _repository.Save(order);
    }
}

// Benefit: Easy to switch databases by changing factory
var processor = new OrderProcessor(new PostgreSqlRepositoryFactory());
```

**Real Impact**:
- ✅ Change database without touching OrderProcessor
- ✅ Easy to test with mock factory
- ✅ Supports multiple databases in same application

### Benefit 2: Single Responsibility Principle

**Before (Mixed Responsibilities)**:
```csharp
public class ReportGenerator
{
    public Report Generate(ReportType type, Data data)
    {
        Report report;

        // ❌ Creation logic mixed with generation logic
        if (type == ReportType.Pdf)
        {
            report = new PdfReport
            {
                PageSize = "A4",
                Orientation = "Portrait",
                // ... complex initialization
            };
        }
        else if (type == ReportType.Excel)
        {
            report = new ExcelReport
            {
                SheetName = "Report",
                // ... complex initialization
            };
        }

        // Business logic
        report.AddData(data);
        report.Format();
        return report;
    }
}
```

**After (Separated Responsibilities)**:
```csharp
// Factory handles creation
public interface IReportFactory
{
    Report CreateReport();
}

// Generator handles generation
public class ReportGenerator
{
    private readonly IReportFactory _factory;

    public ReportGenerator(IReportFactory factory)
    {
        _factory = factory;
    }

    public Report Generate(Data data)
    {
        var report = _factory.CreateReport();  // ✅ Factory's job

        // ✅ Generator only does generation
        report.AddData(data);
        report.Format();
        return report;
    }
}
```

**Real Impact**:
- ✅ ReportGenerator only generates reports
- ✅ Factory only creates reports
- ✅ Each class has one reason to change
- ✅ Easier to test each responsibility independently

### Benefit 3: Open/Closed Principle

**Before (Violates Open/Closed)**:
```csharp
public class NotificationSender
{
    public void Send(Notification notification, string channel)
    {
        // ❌ Must modify this method to add new channels
        if (channel == "email")
        {
            var sender = new EmailSender();
            sender.Send(notification);
        }
        else if (channel == "sms")
        {
            var sender = new SmsSender();
            sender.Send(notification);
        }
        else if (channel == "push")  // ❌ Adding new channel requires modification
        {
            var sender = new PushSender();
            sender.Send(notification);
        }
    }
}
```

**After (Follows Open/Closed)**:
```csharp
// Base factory
public abstract class NotificationSenderFactory
{
    public abstract INotificationSender CreateSender();
}

// Existing factories
public class EmailSenderFactory : NotificationSenderFactory
{
    public override INotificationSender CreateSender() => new EmailSender();
}

public class SmsSenderFactory : NotificationSenderFactory
{
    public override INotificationSender CreateSender() => new SmsSender();
}

// ✅ Add new channel without modifying existing code
public class PushSenderFactory : NotificationSenderFactory
{
    public override INotificationSender CreateSender() => new PushSender();
}

// Client
public class NotificationSender
{
    private readonly NotificationSenderFactory _factory;

    public NotificationSender(NotificationSenderFactory factory)
    {
        _factory = factory;
    }

    public void Send(Notification notification)
    {
        var sender = _factory.CreateSender();
        sender.Send(notification);  // ✅ No changes needed
    }
}
```

**Real Impact**:
- ✅ Add new notification channels without modifying existing code
- ✅ Existing code is closed for modification
- ✅ New functionality through extension
- ✅ Reduces regression risk

### Benefit 4: Dependency Inversion Principle

**Before (High-level depends on low-level)**:
```csharp
// High-level module
public class PaymentProcessor
{
    private StripePaymentGateway _gateway;  // ❌ Depends on concrete class

    public PaymentProcessor()
    {
        _gateway = new StripePaymentGateway();  // ❌ Low-level detail
    }

    public void ProcessPayment(Payment payment)
    {
        _gateway.Charge(payment);
    }
}
```

**After (Both depend on abstraction)**:
```csharp
// Abstraction
public interface IPaymentGateway
{
    void Charge(Payment payment);
}

// Factory interface
public interface IPaymentGatewayFactory
{
    IPaymentGateway CreateGateway();
}

// High-level module depends on abstraction
public class PaymentProcessor
{
    private readonly IPaymentGateway _gateway;

    public PaymentProcessor(IPaymentGatewayFactory factory)
    {
        _gateway = factory.CreateGateway();  // ✅ Depends on interface
    }

    public void ProcessPayment(Payment payment)
    {
        _gateway.Charge(payment);
    }
}

// Low-level modules implement abstraction
public class StripeGatewayFactory : IPaymentGatewayFactory
{
    public IPaymentGateway CreateGateway() => new StripePaymentGateway();
}

public class PayPalGatewayFactory : IPaymentGatewayFactory
{
    public IPaymentGateway CreateGateway() => new PayPalPaymentGateway();
}
```

**Real Impact**:
- ✅ High-level and low-level modules depend on abstraction
- ✅ Easy to swap payment gateways
- ✅ Better testability (mock factory)
- ✅ Follows Dependency Inversion Principle

### Benefit 5: Testability

**Before (Hard to Test)**:
```csharp
public class UserService
{
    private readonly Database _db;

    public UserService()
    {
        _db = new Database("production-connection-string");  // ❌ Hard-coded
    }

    public User GetUser(int id)
    {
        return _db.Query<User>($"SELECT * FROM Users WHERE Id = {id}");
    }
}

// Testing is difficult
[Fact]
public void GetUser_ReturnsUser()
{
    var service = new UserService();  // ❌ Uses production database!
    var user = service.GetUser(1);
    // Can't test without hitting production database
}
```

**After (Easy to Test with Factory)**:
```csharp
public class UserService
{
    private readonly IDatabase _db;

    public UserService(IDatabaseFactory factory)
    {
        _db = factory.CreateDatabase();
    }

    public User GetUser(int id)
    {
        return _db.Query<User>($"SELECT * FROM Users WHERE Id = {id}");
    }
}

// Testing is easy with mock factory
[Fact]
public void GetUser_ReturnsUser()
{
    // Arrange
    var mockFactory = new Mock<IDatabaseFactory>();
    var mockDb = new Mock<IDatabase>();
    mockDb.Setup(db => db.Query<User>(It.IsAny<string>()))
          .Returns(new User { Id = 1, Name = "Test" });
    mockFactory.Setup(f => f.CreateDatabase()).Returns(mockDb.Object);

    var service = new UserService(mockFactory.Object);

    // Act
    var user = service.GetUser(1);

    // Assert
    Assert.Equal("Test", user.Name);
    // ✅ No database interaction!
}
```

**Real Impact**:
- ✅ Unit tests don't need real dependencies
- ✅ Faster test execution
- ✅ More reliable tests (no external dependencies)
- ✅ Easy to test edge cases

---

## When to Use (and When NOT to Use)

### ✅ Use Simple Factory When

1. **Limited Product Types** (2-5 types)
   ```csharp
   // Good use case
   public static class LogLevelFactory
   {
       public static ILogger CreateLogger(LogLevel level)
       {
           return level switch
           {
               LogLevel.Debug => new DebugLogger(),
               LogLevel.Info => new InfoLogger(),
               LogLevel.Error => new ErrorLogger(),
               _ => new DefaultLogger()
           };
       }
   }
   ```

2. **Simple Creation Logic**
   ```csharp
   // Good - straightforward creation
   public static class ShapeFactory
   {
       public static IShape CreateShape(ShapeType type, double size)
       {
           return type switch
           {
               ShapeType.Circle => new Circle(size),
               ShapeType.Square => new Square(size),
               _ => throw new ArgumentException()
           };
       }
   }
   ```

3. **Infrequent Changes**
   - Product types rarely added/removed
   - Creation logic is stable

### ✅ Use Factory Method When

1. **Expecting New Types**
   ```csharp
   // Good - easy to extend
   public abstract class DocumentParser
   {
       public abstract IDocument Parse(string content);

       public void ProcessDocument(string content)
       {
           var document = Parse(content);  // Subclass decides type
           document.Validate();
           document.Save();
       }
   }

   // Add new parser without modifying existing code
   public class PdfParser : DocumentParser
   {
       public override IDocument Parse(string content)
           => new PdfDocument(content);
   }

   public class WordParser : DocumentParser
   {
       public override IDocument Parse(string content)
           => new WordDocument(content);
   }
   ```

2. **Need SOLID Compliance**
   - Following Open/Closed Principle is important
   - Single Responsibility is critical

3. **Complex Subclass-Specific Logic**
   ```csharp
   public abstract class GameCharacterFactory
   {
       public abstract ICharacter CreateCharacter();

       protected abstract void InitializeStats(ICharacter character);
       protected abstract void LoadAssets(ICharacter character);
   }

   public class WarriorFactory : GameCharacterFactory
   {
       public override ICharacter CreateCharacter()
       {
           var warrior = new Warrior();
           InitializeStats(warrior);
           LoadAssets(warrior);
           return warrior;
       }

       protected override void InitializeStats(ICharacter character)
       {
           // Warrior-specific stat initialization
       }
   }
   ```

### ✅ Use Abstract Factory When

1. **Related Products Must Work Together**
   ```csharp
   // Good - ensures UI consistency
   public interface IUIFactory
   {
       IButton CreateButton();
       ITextBox CreateTextBox();
       ICheckBox CreateCheckBox();
   }

   // All Windows components work together
   public class WindowsUIFactory : IUIFactory { ... }

   // All Mac components work together
   public class MacUIFactory : IUIFactory { ... }
   ```

2. **Multiple Product Families**
   ```csharp
   // Good - database driver families
   public interface IDatabaseFactory
   {
       IConnection CreateConnection();
       ICommand CreateCommand();
       ITransaction CreateTransaction();
       IDataAdapter CreateAdapter();
   }

   public class SqlServerFactory : IDatabaseFactory { ... }
   public class PostgreSqlFactory : IDatabaseFactory { ... }
   public class MySqlFactory : IDatabaseFactory { ... }
   ```

3. **Need to Enforce Consistency**
   - Products must be compatible
   - Mixing products would cause errors

### ❌ DON'T Use Factory When

1. **Only One or Two Simple Types**
   ```csharp
   // Bad - over-engineering
   public interface IStringFormatterFactory
   {
       IStringFormatter CreateFormatter();
   }

   public class UpperCaseFormatterFactory : IStringFormatterFactory
   {
       public IStringFormatter CreateFormatter()
           => new UpperCaseFormatter();
   }

   // Just use: string.ToUpper() ❌
   ```

2. **No Variation in Creation**
   ```csharp
   // Bad - unnecessary factory
   public interface IUserFactory
   {
       User CreateUser(string name, string email);
   }

   public class UserFactory : IUserFactory
   {
       public User CreateUser(string name, string email)
           => new User { Name = name, Email = email };
   }

   // Just use: new User { Name = name, Email = email } ❌
   ```

3. **Creation Logic Never Changes**
   ```csharp
   // Bad - factory adds no value
   public static class DateTimeFactory
   {
       public static DateTime CreateNow()
           => DateTime.Now;
   }

   // Just use: DateTime.Now ❌
   ```

4. **Too Many Product Types** (20+)
   ```csharp
   // Bad - Simple Factory becomes unmaintainable
   public static class VehicleFactory
   {
       public static IVehicle CreateVehicle(VehicleType type)
       {
           return type switch
           {
               VehicleType.Car => new Car(),
               VehicleType.Truck => new Truck(),
               VehicleType.Motorcycle => new Motorcycle(),
               // ... 17 more types ❌
               _ => throw new ArgumentException()
           };
       }
   }

   // Use Factory Method or plugin architecture instead
   ```

---

## Real-World Scenarios

### Scenario 1: Multi-Tenant SaaS Application

**Context**: A SaaS application serves multiple clients, each with different database configurations and business rules.

**Challenge**:
- Each tenant may use different database (SQL Server, PostgreSQL, MongoDB)
- Business logic varies per tenant
- Need to switch context based on current tenant

**Solution with Factory Pattern**:

```csharp
// Abstract Factory for tenant-specific components
public interface ITenantFactory
{
    IDatabase CreateDatabase();
    IEmailService CreateEmailService();
    IPaymentGateway CreatePaymentGateway();
    IBusinessRules CreateBusinessRules();
}

// Factory for Enterprise tenants
public class EnterpriseTenantFactory : ITenantFactory
{
    private readonly TenantConfiguration _config;

    public EnterpriseTenantFactory(TenantConfiguration config)
    {
        _config = config;
    }

    public IDatabase CreateDatabase()
        => new SqlServerDatabase(_config.ConnectionString);

    public IEmailService CreateEmailService()
        => new SendGridEmailService(_config.SendGridApiKey);

    public IPaymentGateway CreatePaymentGateway()
        => new StripeGateway(_config.StripeKey);

    public IBusinessRules CreateBusinessRules()
        => new EnterpriseBusinessRules(_config.CustomRules);
}

// Factory for Startup tenants
public class StartupTenantFactory : ITenantFactory
{
    private readonly TenantConfiguration _config;

    public StartupTenantFactory(TenantConfiguration config)
    {
        _config = config;
    }

    public IDatabase CreateDatabase()
        => new PostgreSqlDatabase(_config.ConnectionString);

    public IEmailService CreateEmailService()
        => new SmtpEmailService(_config.SmtpSettings);

    public IPaymentGateway CreatePaymentGateway()
        => new PayPalGateway(_config.PayPalKey);

    public IBusinessRules CreateBusinessRules()
        => new StartupBusinessRules();
}

// Tenant context manages current tenant
public class TenantContext
{
    private readonly Dictionary<string, ITenantFactory> _factories = new();

    public void RegisterTenant(string tenantId, ITenantFactory factory)
    {
        _factories[tenantId] = factory;
    }

    public ITenantFactory GetFactory(string tenantId)
    {
        if (_factories.TryGetValue(tenantId, out var factory))
        {
            return factory;
        }
        throw new InvalidOperationException($"Tenant {tenantId} not registered");
    }
}

// Usage in application
public class OrderService
{
    private readonly TenantContext _tenantContext;

    public OrderService(TenantContext tenantContext)
    {
        _tenantContext = tenantContext;
    }

    public async Task ProcessOrder(Order order, string tenantId)
    {
        // Get tenant-specific factory
        var factory = _tenantContext.GetFactory(tenantId);

        // Create tenant-specific components
        var database = factory.CreateDatabase();
        var emailService = factory.CreateEmailService();
        var paymentGateway = factory.CreatePaymentGateway();
        var businessRules = factory.CreateBusinessRules();

        // Business logic uses tenant-specific implementations
        businessRules.ValidateOrder(order);
        await paymentGateway.ChargeAsync(order.Total);
        await database.SaveOrderAsync(order);
        await emailService.SendConfirmationAsync(order);
    }
}
```

**Benefits**:
- ✅ Each tenant gets appropriate components
- ✅ Easy to onboard new tenant types
- ✅ Tenant isolation guaranteed
- ✅ Configuration-driven tenant setup

### Scenario 2: Cross-Platform Mobile App

**Context**: A Xamarin/MAUI app that runs on iOS, Android, and Windows, each with platform-specific implementations.

**Challenge**:
- Different UI components per platform
- Platform-specific file access
- Different notification systems
- Need consistent API across platforms

**Solution with Abstract Factory**:

```csharp
// Abstract factory for platform components
public interface IPlatformFactory
{
    IFileSystem CreateFileSystem();
    INotificationService CreateNotificationService();
    ICamera CreateCamera();
    IGpsService CreateGpsService();
}

// iOS implementation
public class iOSPlatformFactory : IPlatformFactory
{
    public IFileSystem CreateFileSystem()
        => new iOSFileSystem();

    public INotificationService CreateNotificationService()
        => new iOSNotificationService();

    public ICamera CreateCamera()
        => new iOSCamera();

    public IGpsService CreateGpsService()
        => new iOSGpsService();
}

// Android implementation
public class AndroidPlatformFactory : IPlatformFactory
{
    public IFileSystem CreateFileSystem()
        => new AndroidFileSystem();

    public INotificationService CreateNotificationService()
        => new AndroidNotificationService();

    public ICamera CreateCamera()
        => new AndroidCamera();

    public IGpsService CreateGpsService()
        => new AndroidGpsService();
}

// Windows implementation
public class WindowsPlatformFactory : IPlatformFactory
{
    public IFileSystem CreateFileSystem()
        => new WindowsFileSystem();

    public INotificationService CreateNotificationService()
        => new WindowsNotificationService();

    public ICamera CreateCamera()
        => new WindowsCamera();

    public IGpsService CreateGpsService()
        => new WindowsGpsService();
}

// Platform-independent application code
public class App
{
    private readonly IPlatformFactory _platformFactory;
    private readonly IFileSystem _fileSystem;
    private readonly INotificationService _notifications;

    public App(IPlatformFactory platformFactory)
    {
        _platformFactory = platformFactory;
        _fileSystem = platformFactory.CreateFileSystem();
        _notifications = platformFactory.CreateNotificationService();
    }

    public async Task TakePhotoAsync()
    {
        var camera = _platformFactory.CreateCamera();
        var photo = await camera.CaptureAsync();

        await _fileSystem.SaveAsync("photo.jpg", photo);
        await _notifications.ShowAsync("Photo saved!");
    }

    public async Task GetLocationAsync()
    {
        var gps = _platformFactory.CreateGpsService();
        var location = await gps.GetCurrentLocationAsync();

        await _notifications.ShowAsync($"Location: {location}");
    }
}

// Platform-specific initialization
public class Startup
{
    public static void Initialize()
    {
        IPlatformFactory factory;

        #if IOS
        factory = new iOSPlatformFactory();
        #elif ANDROID
        factory = new AndroidPlatformFactory();
        #elif WINDOWS
        factory = new WindowsPlatformFactory();
        #endif

        var app = new App(factory);
        // App works on all platforms with appropriate components
    }
}
```

**Benefits**:
- ✅ Platform-independent application code
- ✅ All platform-specific code isolated in factories
- ✅ Easy to add new platforms
- ✅ Compile-time safety

### Scenario 3: Data Export System

**Context**: An analytics platform that exports data to multiple formats (PDF, Excel, CSV, JSON, XML).

**Challenge**:
- Different export logic per format
- Each format requires specific configuration
- Some formats support advanced features (charts, styling)
- Need to add new formats without modifying existing code

**Solution with Factory Method**:

```csharp
// Abstract exporter with factory method
public abstract class DataExporter
{
    // Factory method - subclasses provide specific exporters
    protected abstract IDocumentWriter CreateWriter();

    // Template method using factory method
    public async Task<byte[]> ExportAsync(DataSet data)
    {
        var writer = CreateWriter();

        // Common export logic
        await writer.InitializeAsync();
        await WriteHeaderAsync(writer, data);
        await WriteDataAsync(writer, data);
        await WriteFooterAsync(writer, data);
        await writer.FinalizeAsync();

        return writer.GetBytes();
    }

    protected virtual Task WriteHeaderAsync(IDocumentWriter writer, DataSet data)
    {
        writer.WriteTitle(data.Title);
        writer.WriteMetadata(data.Metadata);
        return Task.CompletedTask;
    }

    protected virtual Task WriteDataAsync(IDocumentWriter writer, DataSet data)
    {
        foreach (var row in data.Rows)
        {
            writer.WriteRow(row);
        }
        return Task.CompletedTask;
    }

    protected virtual Task WriteFooterAsync(IDocumentWriter writer, DataSet data)
    {
        writer.WriteFooter($"Generated on {DateTime.Now}");
        return Task.CompletedTask;
    }
}

// PDF exporter
public class PdfDataExporter : DataExporter
{
    private readonly PdfConfiguration _config;

    public PdfDataExporter(PdfConfiguration config)
    {
        _config = config;
    }

    protected override IDocumentWriter CreateWriter()
        => new PdfDocumentWriter(_config);

    // Override to add PDF-specific features
    protected override async Task WriteHeaderAsync(IDocumentWriter writer, DataSet data)
    {
        await base.WriteHeaderAsync(writer, data);

        if (writer is PdfDocumentWriter pdfWriter)
        {
            // PDF-specific: Add chart
            pdfWriter.AddChart(data.CreateChart());
        }
    }
}

// Excel exporter
public class ExcelDataExporter : DataExporter
{
    private readonly ExcelConfiguration _config;

    public ExcelDataExporter(ExcelConfiguration config)
    {
        _config = config;
    }

    protected override IDocumentWriter CreateWriter()
        => new ExcelDocumentWriter(_config);

    // Override to add Excel-specific features
    protected override async Task WriteDataAsync(IDocumentWriter writer, DataSet data)
    {
        if (writer is ExcelDocumentWriter excelWriter)
        {
            // Excel-specific: Multiple sheets
            foreach (var table in data.Tables)
            {
                excelWriter.CreateSheet(table.Name);
                await base.WriteDataAsync(excelWriter, table.Data);
            }
        }
    }
}

// CSV exporter (simpler, no special features)
public class CsvDataExporter : DataExporter
{
    protected override IDocumentWriter CreateWriter()
        => new CsvDocumentWriter();
}

// JSON exporter
public class JsonDataExporter : DataExporter
{
    private readonly JsonSerializerOptions _options;

    public JsonDataExporter(JsonSerializerOptions options)
    {
        _options = options;
    }

    protected override IDocumentWriter CreateWriter()
        => new JsonDocumentWriter(_options);
}

// Usage
public class ReportService
{
    public async Task<byte[]> GenerateReport(DataSet data, ExportFormat format)
    {
        DataExporter exporter = format switch
        {
            ExportFormat.Pdf => new PdfDataExporter(new PdfConfiguration()),
            ExportFormat.Excel => new ExcelDataExporter(new ExcelConfiguration()),
            ExportFormat.Csv => new CsvDataExporter(),
            ExportFormat.Json => new JsonDataExporter(new JsonSerializerOptions()),
            _ => throw new ArgumentException($"Unsupported format: {format}")
        };

        return await exporter.ExportAsync(data);
    }
}

// Easy to add new format without modifying existing code
public class XmlDataExporter : DataExporter
{
    protected override IDocumentWriter CreateWriter()
        => new XmlDocumentWriter();
}
```

**Benefits**:
- ✅ Common export logic in base class
- ✅ Format-specific logic in subclasses
- ✅ Easy to add new formats
- ✅ Template method + Factory method combination

---

## Performance Implications

### Performance Benchmark Results

From Program.cs (100,000 iterations):

```
Simple Factory:      ~15ms
Factory Method:      ~18ms
Abstract Factory:    ~20ms
```

**Analysis**:
- Performance difference is **negligible** (~5ms per 100k calls)
- For typical applications (< 1000 objects/second), no noticeable impact
- **Design considerations** should outweigh performance concerns

### Memory Considerations

#### 1. Factory Instance Reuse

**Bad - Creates new factory each time**:
```csharp
for (int i = 0; i < 1000; i++)
{
    IUIFactory factory = new DarkUIFactory();  // ❌ 1000 allocations
    var theme = factory.CreateTheme();
}

// Memory: ~16 KB (1000 factories * 16 bytes each)
```

**Good - Reuse factory instance**:
```csharp
IUIFactory factory = new DarkUIFactory();  // ✅ 1 allocation

for (int i = 0; i < 1000; i++)
{
    var theme = factory.CreateTheme();
}

// Memory: ~16 bytes (1 factory)
```

#### 2. Singleton Factories

```csharp
public class DarkUIFactory : IUIFactory
{
    // Singleton pattern for factory
    private static readonly Lazy<DarkUIFactory> _instance =
        new Lazy<DarkUIFactory>(() => new DarkUIFactory());

    public static DarkUIFactory Instance => _instance.Value;

    private DarkUIFactory() { }  // Private constructor

    public ITheme CreateTheme() => new DarkTheme();
    public IButton CreateButton() => new DarkButton();
    public IPanel CreatePanel() => new DarkPanel();
}

// Usage
var theme = DarkUIFactory.Instance.CreateTheme();
```

**Benefits**:
- ✅ Single factory instance across application
- ✅ Minimal memory footprint
- ✅ Thread-safe with Lazy<T>

#### 3. Object Pooling with Factories

```csharp
public class PooledThemeFactory
{
    private readonly ObjectPool<DarkTheme> _darkThemePool;
    private readonly ObjectPool<LightTheme> _lightThemePool;

    public PooledThemeFactory()
    {
        _darkThemePool = new ObjectPool<DarkTheme>(() => new DarkTheme());
        _lightThemePool = new ObjectPool<LightTheme>(() => new LightTheme());
    }

    public ITheme CreateTheme(ThemeType type)
    {
        return type switch
        {
            ThemeType.Dark => _darkThemePool.Get(),   // Reuse from pool
            ThemeType.Light => _lightThemePool.Get(),  // Reuse from pool
            _ => throw new ArgumentException()
        };
    }

    public void ReturnTheme(ITheme theme)
    {
        if (theme is DarkTheme darkTheme)
            _darkThemePool.Return(darkTheme);
        else if (theme is LightTheme lightTheme)
            _lightThemePool.Return(lightTheme);
    }
}
```

**Benefits**:
- ✅ Reduced GC pressure
- ✅ Better performance for frequently created objects
- ✅ Useful for short-lived objects

### When Performance Matters

**High-frequency creation** (> 10,000 objects/second):
- Use Simple Factory (fastest)
- Consider object pooling
- Profile to identify bottlenecks

**Low-frequency creation** (< 1,000 objects/second):
- Use any pattern based on design needs
- Performance difference is negligible
- Prioritize maintainability

**Lazy initialization**:
```csharp
public class LazyThemeFactory
{
    private Lazy<ITheme> _darkTheme = new Lazy<ITheme>(() => new DarkTheme());

    public ITheme CreateDarkTheme()
    {
        return _darkTheme.Value;  // Created only once, on first access
    }
}
```

---

## Common Mistakes and How to Avoid Them

### Mistake 1: Over-Engineering with Factories

**Problem**:
```csharp
// ❌ Unnecessary factory for simple case
public interface IStringWrapperFactory
{
    StringWrapper CreateWrapper(string value);
}

public class StringWrapperFactory : IStringWrapperFactory
{
    public StringWrapper CreateWrapper(string value)
    {
        return new StringWrapper(value);  // Just wraps a string!
    }
}

// Usage (over-complicated)
var factory = new StringWrapperFactory();
var wrapper = factory.CreateWrapper("hello");
```

**Solution**:
```csharp
// ✅ Just use the constructor
var wrapper = new StringWrapper("hello");

// Or if you need abstraction, use a simple method
public static class StringWrapperExtensions
{
    public static StringWrapper Wrap(this string value)
        => new StringWrapper(value);
}

// Usage
var wrapper = "hello".Wrap();
```

**Guideline**: Only use factories when creation logic is complex or varies.

### Mistake 2: God Factory (Factory Creating Too Many Types)

**Problem**:
```csharp
// ❌ Factory doing too much
public static class ObjectFactory
{
    public static object CreateObject(string type)
    {
        return type switch
        {
            "user" => new User(),
            "order" => new Order(),
            "product" => new Product(),
            "invoice" => new Invoice(),
            "payment" => new Payment(),
            "shipment" => new Shipment(),
            // ... 50 more types ❌
            _ => throw new ArgumentException()
        };
    }
}
```

**Solution**:
```csharp
// ✅ Separate factories by domain
public static class UserFactory
{
    public static User CreateUser(string email) => new User { Email = email };
    public static User CreateAdmin(string email) => new User { Email = email, IsAdmin = true };
}

public static class OrderFactory
{
    public static Order CreateOrder(User user) => new Order { User = user };
    public static Order CreateDraftOrder(User user) => new Order { User = user, Status = OrderStatus.Draft };
}

// Or use Factory Method with polymorphism
public abstract class EntityFactory<T> where T : class
{
    public abstract T Create();
}

public class UserFactory : EntityFactory<User> { ... }
public class OrderFactory : EntityFactory<Order> { ... }
```

**Guideline**: Keep factories focused on related types (Single Responsibility).

### Mistake 3: Mixing Creation with Business Logic

**Problem**:
```csharp
// ❌ Factory doing more than creating
public class UserFactory
{
    public User CreateUser(string email, string password)
    {
        var user = new User
        {
            Email = email,
            Password = password
        };

        // ❌ Business logic in factory
        user.Validate();
        user.SendWelcomeEmail();
        user.SaveToDatabase();

        return user;
    }
}
```

**Solution**:
```csharp
// ✅ Factory only creates
public class UserFactory
{
    public User CreateUser(string email, string password)
    {
        return new User
        {
            Email = email,
            Password = password,
            CreatedAt = DateTime.UtcNow
        };
    }
}

// ✅ Service handles business logic
public class UserService
{
    private readonly UserFactory _factory;
    private readonly IUserRepository _repository;
    private readonly IEmailService _emailService;

    public async Task<User> RegisterUserAsync(string email, string password)
    {
        var user = _factory.CreateUser(email, password);

        user.Validate();
        await _repository.SaveAsync(user);
        await _emailService.SendWelcomeEmailAsync(user);

        return user;
    }
}
```

**Guideline**: Factories create, services orchestrate business logic.

### Mistake 4: Exposing Factory Internals

**Problem**:
```csharp
// ❌ Leaking implementation details
public class ThemeFactory
{
    public Dictionary<string, Type> ThemeTypes { get; set; }  // ❌ Exposed!

    public ITheme CreateTheme(string type)
    {
        if (ThemeTypes.TryGetValue(type, out var themeType))
        {
            return (ITheme)Activator.CreateInstance(themeType);
        }
        throw new ArgumentException();
    }
}

// Client can modify factory internals
var factory = new ThemeFactory();
factory.ThemeTypes["dark"] = typeof(LightTheme);  // ❌ Breaks factory!
```

**Solution**:
```csharp
// ✅ Encapsulate internals
public class ThemeFactory
{
    private readonly Dictionary<string, Type> _themeTypes;

    public ThemeFactory()
    {
        _themeTypes = new Dictionary<string, Type>
        {
            ["dark"] = typeof(DarkTheme),
            ["light"] = typeof(LightTheme)
        };
    }

    public ITheme CreateTheme(string type)
    {
        if (_themeTypes.TryGetValue(type, out var themeType))
        {
            return (ITheme)Activator.CreateInstance(themeType);
        }
        throw new ArgumentException($"Unknown theme: {type}");
    }

    // Safe registration method with validation
    public void RegisterTheme(string type, Type themeType)
    {
        if (!typeof(ITheme).IsAssignableFrom(themeType))
        {
            throw new ArgumentException($"{themeType} does not implement ITheme");
        }

        _themeTypes[type] = themeType;
    }
}
```

**Guideline**: Keep factory internals private, provide controlled registration if needed.

### Mistake 5: Not Handling Factory Creation Failures

**Problem**:
```csharp
// ❌ No error handling
public static class DatabaseFactory
{
    public static IDatabase CreateDatabase(string connectionString)
    {
        // ❌ What if connection string is invalid?
        return new SqlDatabase(connectionString);
    }
}

// Usage - exception bubbles up
var db = DatabaseFactory.CreateDatabase(invalidConnectionString);  // Throws!
```

**Solution**:
```csharp
// ✅ Validate and provide clear errors
public static class DatabaseFactory
{
    public static IDatabase CreateDatabase(string connectionString)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new ArgumentException(
                "Connection string cannot be null or empty",
                nameof(connectionString));
        }

        try
        {
            var db = new SqlDatabase(connectionString);
            db.TestConnection();  // Validate connection
            return db;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException(
                $"Failed to create database with connection string '{connectionString}'",
                ex);
        }
    }

    // Or use Try pattern
    public static bool TryCreateDatabase(
        string connectionString,
        out IDatabase database,
        out string errorMessage)
    {
        database = null;
        errorMessage = null;

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            errorMessage = "Connection string cannot be null or empty";
            return false;
        }

        try
        {
            database = new SqlDatabase(connectionString);
            database.TestConnection();
            return true;
        }
        catch (Exception ex)
        {
            errorMessage = ex.Message;
            return false;
        }
    }
}
```

**Guideline**: Always validate inputs and handle creation failures gracefully.

---

## Migration Strategies

### Strategy 1: Gradual Refactoring (Strangler Fig Pattern)

**Step 1: Identify Creation Points**
```csharp
// Before - direct instantiation scattered throughout codebase
public class OrderController
{
    public IActionResult CreateOrder()
    {
        var order = new Order();  // Direct instantiation
        var validator = new OrderValidator();  // Direct instantiation
        var repository = new OrderRepository();  // Direct instantiation

        if (validator.Validate(order))
        {
            repository.Save(order);
        }

        return Ok(order);
    }
}
```

**Step 2: Introduce Simple Factory**
```csharp
// Simple factory as first step
public static class OrderFactory
{
    public static Order CreateOrder()
    {
        return new Order
        {
            Id = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
            Status = OrderStatus.Pending
        };
    }

    public static OrderValidator CreateValidator()
    {
        return new OrderValidator();
    }

    public static OrderRepository CreateRepository()
    {
        return new OrderRepository();
    }
}

// Refactored controller
public class OrderController
{
    public IActionResult CreateOrder()
    {
        var order = OrderFactory.CreateOrder();  // ✅ Using factory
        var validator = OrderFactory.CreateValidator();
        var repository = OrderFactory.CreateRepository();

        if (validator.Validate(order))
        {
            repository.Save(order);
        }

        return Ok(order);
    }
}
```

**Step 3: Extract Interfaces**
```csharp
// Define interfaces
public interface IOrderValidator
{
    bool Validate(Order order);
}

public interface IOrderRepository
{
    void Save(Order order);
}

// Simple factory returns interfaces
public static class OrderFactory
{
    public static Order CreateOrder() { ... }

    public static IOrderValidator CreateValidator()
        => new OrderValidator();

    public static IOrderRepository CreateRepository()
        => new OrderRepository();
}
```

**Step 4: Move to Dependency Injection**
```csharp
// Dependency injection replaces factories
public class OrderController
{
    private readonly IOrderValidator _validator;
    private readonly IOrderRepository _repository;

    public OrderController(
        IOrderValidator validator,
        IOrderRepository repository)
    {
        _validator = validator;
        _repository = repository;
    }

    public IActionResult CreateOrder()
    {
        var order = new Order  // Simple objects can still use new
        {
            Id = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow
        };

        if (_validator.Validate(order))
        {
            _repository.Save(order);
        }

        return Ok(order);
    }
}

// Register in DI container
services.AddScoped<IOrderValidator, OrderValidator>();
services.AddScoped<IOrderRepository, OrderRepository>();
```

### Strategy 2: Wrapper Pattern for Legacy Code

**Problem**: Legacy codebase with hard-coded dependencies.

```csharp
// Legacy code (can't modify)
public class LegacyEmailSender
{
    public void Send(string to, string subject, string body)
    {
        // Hard-coded SMTP settings
        var smtp = new SmtpClient("smtp.company.com", 587);
        // ... send email
    }
}

// Legacy service using hard-coded sender
public class LegacyNotificationService
{
    public void SendNotification(User user, string message)
    {
        var emailSender = new LegacyEmailSender();  // ❌ Hard-coded
        emailSender.Send(user.Email, "Notification", message);
    }
}
```

**Solution**: Wrap legacy code with factory.

```csharp
// Define abstraction
public interface IEmailSender
{
    void Send(string to, string subject, string body);
}

// Adapter for legacy sender
public class LegacyEmailSenderAdapter : IEmailSender
{
    private readonly LegacyEmailSender _legacySender;

    public LegacyEmailSenderAdapter()
    {
        _legacySender = new LegacyEmailSender();
    }

    public void Send(string to, string subject, string body)
    {
        _legacySender.Send(to, subject, body);
    }
}

// Modern implementation
public class ModernEmailSender : IEmailSender
{
    private readonly IEmailService _emailService;

    public ModernEmailSender(IEmailService emailService)
    {
        _emailService = emailService;
    }

    public void Send(string to, string subject, string body)
    {
        _emailService.SendAsync(to, subject, body).Wait();
    }
}

// Factory selects implementation
public interface IEmailSenderFactory
{
    IEmailSender CreateEmailSender();
}

public class EmailSenderFactory : IEmailSenderFactory
{
    private readonly IConfiguration _config;
    private readonly IEmailService _emailService;

    public EmailSenderFactory(IConfiguration config, IEmailService emailService)
    {
        _config = config;
        _emailService = emailService;
    }

    public IEmailSender CreateEmailSender()
    {
        var useLegacy = _config.GetValue<bool>("Email:UseLegacy");

        return useLegacy
            ? new LegacyEmailSenderAdapter()  // Wrap legacy
            : new ModernEmailSender(_emailService);  // Use modern
    }
}

// Refactored service
public class NotificationService
{
    private readonly IEmailSenderFactory _emailSenderFactory;

    public NotificationService(IEmailSenderFactory emailSenderFactory)
    {
        _emailSenderFactory = emailSenderFactory;
    }

    public void SendNotification(User user, string message)
    {
        var emailSender = _emailSenderFactory.CreateEmailSender();
        emailSender.Send(user.Email, "Notification", message);
    }
}
```

**Benefits**:
- ✅ No changes to legacy code
- ✅ Gradual migration (toggle via config)
- ✅ New code uses modern patterns
- ✅ Easy rollback if issues occur

---

## Comparison with Other Patterns

### Factory Pattern vs. Builder Pattern

**Factory Pattern**:
- Creates complete object in one step
- Different types of objects
- Focus: **What** object to create

```csharp
// Factory - creates different types
ITheme theme = themeFactory.CreateTheme(ThemeType.Dark);
// Returns complete DarkTheme object
```

**Builder Pattern**:
- Creates complex object step-by-step
- Same type, different configurations
- Focus: **How** to construct object

```csharp
// Builder - configures complex object
var theme = new ThemeBuilder()
    .SetBackground("#1E1E1E")
    .SetText("#FFFFFF")
    .SetAccent("#007ACC")
    .EnableAnimations()
    .SetFontSize(14)
    .Build();
// Returns configured Theme object
```

**When to use which**:
- **Factory**: Multiple types, simple creation
- **Builder**: Complex configuration, fluent API

### Factory Pattern vs. Prototype Pattern

**Factory Pattern**:
- Creates new instances from scratch
- Uses constructors

```csharp
public ITheme CreateTheme()
{
    return new DarkTheme();  // New instance created
}
```

**Prototype Pattern**:
- Clones existing instances
- Uses `Clone()` method

```csharp
public ITheme CreateTheme()
{
    return _prototypeTheme.Clone();  // Copy existing instance
}
```

**When to use which**:
- **Factory**: Creation from scratch
- **Prototype**: Object is expensive to create, prefer cloning

### Factory Pattern vs. Dependency Injection

**Factory Pattern**:
- You control when objects are created
- Can create multiple instances
- Creation logic in factory

```csharp
public void ProcessOrders(List<Order> orders)
{
    foreach (var order in orders)
    {
        // Create new validator for each order
        var validator = _validatorFactory.CreateValidator();
        validator.Validate(order);
    }
}
```

**Dependency Injection**:
- Container controls when objects are created
- Usually one instance per scope
- Creation logic in container configuration

```csharp
public class OrderProcessor
{
    private readonly IOrderValidator _validator;

    public OrderProcessor(IOrderValidator validator)  // Injected
    {
        _validator = validator;
    }

    public void ProcessOrders(List<Order> orders)
    {
        foreach (var order in orders)
        {
            _validator.Validate(order);  // Reuses same instance
        }
    }
}
```

**When to use which**:
- **Factory**: Need control over instance creation, multiple instances
- **DI**: Standard dependencies, container manages lifecycle

**Combining Both**:
```csharp
// Factory injected via DI
public class OrderProcessor
{
    private readonly IOrderValidatorFactory _validatorFactory;

    public OrderProcessor(IOrderValidatorFactory validatorFactory)
    {
        _validatorFactory = validatorFactory;  // DI provides factory
    }

    public void ProcessOrders(List<Order> orders)
    {
        foreach (var order in orders)
        {
            // Factory creates instances on demand
            var validator = _validatorFactory.CreateValidator();
            validator.Validate(order);
        }
    }
}
```

---

## Advanced Techniques

### Technique 1: Generic Factory

```csharp
// Generic factory for any type
public interface IFactory<T>
{
    T Create();
}

// Implementation
public class Factory<T> : IFactory<T> where T : new()
{
    public T Create()
    {
        return new T();
    }
}

// With parameters
public interface IFactory<TProduct, TParam>
{
    TProduct Create(TParam parameter);
}

public class ThemeFactory : IFactory<ITheme, ThemeConfiguration>
{
    public ITheme Create(ThemeConfiguration config)
    {
        return config.ThemeType switch
        {
            ThemeType.Dark => new DarkTheme(config),
            ThemeType.Light => new LightTheme(config),
            _ => throw new ArgumentException()
        };
    }
}
```

### Technique 2: Reflection-Based Factory

```csharp
public class ReflectionFactory
{
    private readonly Dictionary<string, Type> _typeRegistry = new();

    public void RegisterType<T>(string key) where T : class
    {
        _typeRegistry[key] = typeof(T);
    }

    public T Create<T>(string key, params object[] args) where T : class
    {
        if (_typeRegistry.TryGetValue(key, out var type))
        {
            return (T)Activator.CreateInstance(type, args);
        }

        throw new ArgumentException($"Type '{key}' not registered");
    }
}

// Usage
var factory = new ReflectionFactory();
factory.RegisterType<DarkTheme>("dark");
factory.RegisterType<LightTheme>("light");

var theme = factory.Create<ITheme>("dark");
```

### Technique 3: Plugin-Based Factory

```csharp
// Plugin interface
public interface IThemePlugin
{
    string Name { get; }
    ITheme CreateTheme();
}

// Plugin factory
public class PluginThemeFactory
{
    private readonly Dictionary<string, IThemePlugin> _plugins = new();

    public void LoadPlugins(string pluginDirectory)
    {
        var assemblies = Directory.GetFiles(pluginDirectory, "*.dll")
            .Select(Assembly.LoadFrom);

        foreach (var assembly in assemblies)
        {
            var pluginTypes = assembly.GetTypes()
                .Where(t => typeof(IThemePlugin).IsAssignableFrom(t) && !t.IsInterface);

            foreach (var pluginType in pluginTypes)
            {
                var plugin = (IThemePlugin)Activator.CreateInstance(pluginType);
                _plugins[plugin.Name] = plugin;
            }
        }
    }

    public ITheme CreateTheme(string pluginName)
    {
        if (_plugins.TryGetValue(pluginName, out var plugin))
        {
            return plugin.CreateTheme();
        }

        throw new ArgumentException($"Plugin '{pluginName}' not found");
    }
}
```

---

## Summary

### Key Principles

1. **Encapsulation**: Hide object creation complexity
2. **Abstraction**: Return interfaces, not concrete classes
3. **Flexibility**: Easy to add new types
4. **Maintainability**: Centralize creation logic
5. **Testability**: Easy to mock factories

### Decision Tree

```
Need object creation abstraction?
├─ No  → Use direct instantiation (new)
└─ Yes → Continue
    │
    ├─ 1-2 simple types?
    │   └─ No  → Continue
    │   └─ Yes → Use direct instantiation
    │
    ├─ Related product families?
    │   └─ Yes → Use Abstract Factory
    │   └─ No  → Continue
    │
    ├─ Need extensibility (Open/Closed)?
    │   └─ Yes → Use Factory Method
    │   └─ No  → Continue
    │
    └─ Simple creation logic?
        └─ Yes → Use Simple Factory
        └─ No  → Use Factory Method
```

### Final Recommendations

1. **Start Simple**: Begin with Simple Factory
2. **Refactor When Needed**: Move to Factory Method when extensibility is required
3. **Use Abstract Factory Sparingly**: Only for related product families
4. **Combine with DI**: Inject factories, not products
5. **Don't Over-Engineer**: Not every object needs a factory

---

**Next Steps**:
1. Review the demo code in Program.cs
2. Try adding a new theme (Solarized, Nord, etc.)
3. Implement a factory for your own domain
4. Experiment with combining Factory + DI
5. Study real-world examples (ASP.NET Core, Entity Framework)

**Remember**: The best pattern is the simplest one that solves your problem!
