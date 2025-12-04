# Month 3 Capstone: Event-Driven Notification System

**Difficulty**: ⭐⭐⭐☆☆ (Intermediate+)
**Estimated Time**: 25-30 hours
**Prerequisites**: Completed Week 9-12 of Path 1 (Generics & Design Patterns)

---

## 🎯 Project Overview

Build a complete event-driven notification system using Observer pattern, Builder pattern, generic covariance/contravariance, and demonstrating thread-safe operations.

### Learning Objectives

- ✅ IObservable<T> and IObserver<T> implementation
- ✅ Builder pattern for complex object construction
- ✅ Generic covariance (`IProducer<out T>`)
- ✅ Generic contravariance (`IConsumer<in T>`)
- ✅ Generic constraints (class, new(), interfaces)
- ✅ Subscription lifecycle management
- ✅ Thread-safe event handling

---

## 📋 Requirements

### Functional Requirements

1. **Notification Types**:
   - Email notification
   - SMS notification
   - Push notification
   - Logger (special observer)
   - Analytics tracker (special observer)

2. **Event Sources**:
   - User actions (login, logout, purchase, etc.)
   - System events (error, warning, info)
   - Custom events

3. **Builder Pattern**:
   - NotificationBuilder for constructing complex notifications
   - Fluent interface with chaining
   - Validation before `Build()`
   - Support for templates

4. **Observer Pattern**:
   - Multiple observers per event stream
   - Subscribe/Unsubscribe with IDisposable
   - OnNext, OnError, OnCompleted implementation
   - Filter notifications by type

5. **Variance Usage**:
   - Covariant `INotificationProducer<out T>`
   - Contravariant `INotificationHandler<in T>`
   - Demonstrate safe variance scenarios

6. **Generic Constraints**:
   - Repository<T> where T : INotification, new()
   - Validator<T> where T : class, IValidatable
   - Factory pattern with new() constraint

### Technical Requirements

- **Performance**: Handle 10,000+ events/second
- **Thread Safety**: Safe for concurrent access
- **Unit Tests**: 20+ tests
- **Design Patterns**: Builder, Observer, Factory, Repository
- **SOLID Principles**: Demonstrate all 5 principles

---

## 🏗️ Project Structure

```
NotificationSystem/
├── Models/
│   ├── INotification.cs (base interface)
│   ├── Notification.cs (abstract base)
│   ├── EmailNotification.cs
│   ├── SmsNotification.cs
│   ├── PushNotification.cs
│   ├── SystemEvent.cs
│   └── UserEvent.cs
├── Builders/
│   ├── NotificationBuilder.cs
│   ├── EmailNotificationBuilder.cs
│   └── SmsNotificationBuilder.cs
├── Observers/
│   ├── IObserver.cs
│   ├── EmailObserver.cs
│   ├── SmsObserver.cs
│   ├── PushObserver.cs
│   ├── LoggerObserver.cs
│   └── AnalyticsObserver.cs
├── Observables/
│   ├── IObservable.cs
│   ├── NotificationStream.cs
│   └── EventStream.cs
├── Variance/
│   ├── INotificationProducer.cs (out T)
│   ├── INotificationHandler.cs (in T)
│   ├── NotificationProducer.cs
│   └── NotificationHandler.cs
├── Services/
│   ├── NotificationService.cs
│   ├── SubscriptionManager.cs
│   └── NotificationRepository.cs
└── Tests/
    ├── ObserverTests.cs
    ├── BuilderTests.cs
    ├── VarianceTests.cs
    └── PerformanceTests.cs
```

---

## 🚀 Getting Started

### Step 1: Define Base Interfaces and Models

```csharp
// Models/INotification.cs
public interface INotification
{
    Guid Id { get; }
    string Title { get; }
    string Message { get; }
    DateTime Timestamp { get; }
    NotificationPriority Priority { get; }
}

// Models/Notification.cs
public abstract class Notification : INotification
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string Message { get; set; }
    public DateTime Timestamp { get; set; }
    public NotificationPriority Priority { get; set; }

    protected Notification()
    {
        Id = Guid.NewGuid();
        Timestamp = DateTime.UtcNow;
        Priority = NotificationPriority.Normal;
    }
}

// TODO: Implement EmailNotification
public class EmailNotification : Notification
{
    public string To { get; set; }
    public string From { get; set; }
    public string Subject { get; set; }
    public string Body { get; set; }
}

// TODO: Implement SmsNotification
public class SmsNotification : Notification
{
    public string PhoneNumber { get; set; }
    public string Text { get; set; }
}

// TODO: Implement PushNotification
public class PushNotification : Notification
{
    public string DeviceId { get; set; }
    public string Title { get; set; }
    public string Body { get; set; }
}
```

### Step 2: Implement Builder Pattern

```csharp
// Builders/NotificationBuilder.cs
public class NotificationBuilder
{
    private string _title;
    private string _message;
    private NotificationPriority _priority;

    // TODO: Implement fluent methods
    public NotificationBuilder WithTitle(string title)
    {
        _title = title;
        return this;
    }

    public NotificationBuilder WithMessage(string message)
    {
        _message = message;
        return this;
    }

    public NotificationBuilder WithPriority(NotificationPriority priority)
    {
        _priority = priority;
        return this;
    }

    // TODO: Add validation before Build()
    public Notification Build()
    {
        // Validate
        if (string.IsNullOrEmpty(_title))
            throw new InvalidOperationException("Title is required");

        // TODO: Create and return notification
        throw new NotImplementedException();
    }
}

// TODO: Implement EmailNotificationBuilder
public class EmailNotificationBuilder : NotificationBuilder
{
    private string _to;
    private string _from;
    private string _subject;
    private string _body;

    public EmailNotificationBuilder To(string email)
    {
        _to = email;
        return this;
    }

    // TODO: Implement remaining fluent methods
    // TODO: Override Build() to create EmailNotification
}
```

### Step 3: Implement Observer Pattern

```csharp
// Observers/IObserver.cs
public interface IObserver<in T>
{
    void OnNext(T value);
    void OnError(Exception error);
    void OnCompleted();
}

// Observables/IObservable.cs
public interface IObservable<out T>
{
    IDisposable Subscribe(IObserver<T> observer);
}

// Observables/NotificationStream.cs
public class NotificationStream<T> : IObservable<T> where T : INotification
{
    private readonly List<IObserver<T>> _observers = new();
    private readonly object _lock = new();

    public IDisposable Subscribe(IObserver<T> observer)
    {
        lock (_lock)
        {
            _observers.Add(observer);
        }
        return new Subscription(() => Unsubscribe(observer));
    }

    private void Unsubscribe(IObserver<T> observer)
    {
        lock (_lock)
        {
            _observers.Remove(observer);
        }
    }

    // TODO: Implement Publish method
    public void Publish(T notification)
    {
        lock (_lock)
        {
            foreach (var observer in _observers)
            {
                try
                {
                    observer.OnNext(notification);
                }
                catch (Exception ex)
                {
                    observer.OnError(ex);
                }
            }
        }
    }

    // TODO: Implement Complete method
    public void Complete()
    {
        // TODO: Call OnCompleted on all observers
        throw new NotImplementedException();
    }
}

// Subscription.cs (helper)
public class Subscription : IDisposable
{
    private readonly Action _unsubscribe;
    private bool _disposed;

    public Subscription(Action unsubscribe)
    {
        _unsubscribe = unsubscribe;
    }

    public void Dispose()
    {
        if (!_disposed)
        {
            _unsubscribe();
            _disposed = true;
        }
    }
}
```

### Step 4: Implement Concrete Observers

```csharp
// Observers/EmailObserver.cs
public class EmailObserver : IObserver<EmailNotification>
{
    public void OnNext(EmailNotification notification)
    {
        // TODO: Send email
        Console.WriteLine($"Sending email to {notification.To}: {notification.Subject}");
    }

    public void OnError(Exception error)
    {
        Console.WriteLine($"Email observer error: {error.Message}");
    }

    public void OnCompleted()
    {
        Console.WriteLine("Email observer completed");
    }
}

// TODO: Implement SmsObserver, PushObserver, LoggerObserver, AnalyticsObserver
```

### Step 5: Demonstrate Variance

```csharp
// Variance/INotificationProducer.cs (Covariant)
public interface INotificationProducer<out T> where T : INotification
{
    T Produce();
    IEnumerable<T> ProduceMany(int count);
}

// Variance/INotificationHandler.cs (Contravariant)
public interface INotificationHandler<in T> where T : INotification
{
    void Handle(T notification);
    void HandleMany(IEnumerable<T> notifications);
}

// Demonstrate covariance
INotificationProducer<EmailNotification> emailProducer = new EmailProducer();
INotificationProducer<INotification> notificationProducer = emailProducer; // Covariance!

// Demonstrate contravariance
INotificationHandler<INotification> generalHandler = new GeneralHandler();
INotificationHandler<EmailNotification> emailHandler = generalHandler; // Contravariance!
```

### Step 6: Implement Generic Repository with Constraints

```csharp
// Services/NotificationRepository.cs
public class NotificationRepository<T> where T : class, INotification, new()
{
    private readonly List<T> _notifications = new();

    public void Add(T notification)
    {
        // TODO: Add to collection
        _notifications.Add(notification);
    }

    public T GetById(Guid id)
    {
        // TODO: Find by ID
        return _notifications.FirstOrDefault(n => n.Id == id);
    }

    public IEnumerable<T> GetAll()
    {
        return _notifications;
    }

    public IEnumerable<T> GetByPriority(NotificationPriority priority)
    {
        // TODO: Filter by priority
        return _notifications.Where(n => n.Priority == priority);
    }

    // TODO: Factory method using new() constraint
    public T CreateNew()
    {
        return new T();
    }
}
```

---

## 🎯 Milestones

### Milestone 1: Core Models & Builder (Day 1-3)
- ✅ All notification models implemented
- ✅ Builder pattern with fluent interface
- ✅ Validation working

### Milestone 2: Observer Pattern (Day 4-6)
- ✅ IObservable/IObserver implemented
- ✅ Subscription management working
- ✅ All 5 observers functional
- ✅ Thread-safe implementation

### Milestone 3: Variance & Generics (Day 7-8)
- ✅ Covariant producer interface
- ✅ Contravariant handler interface
- ✅ Repository with constraints
- ✅ Demonstrate variance safety

### Milestone 4: Integration & Testing (Day 9-10)
- ✅ All components integrated
- ✅ 20+ unit tests passing
- ✅ Performance tested (10k+ events/sec)
- ✅ Console demo working

---

## ✅ Evaluation Criteria

| Criteria | Points |
|----------|--------|
| Observer Pattern Implementation | 25 |
| Builder Pattern Implementation | 15 |
| Variance Demonstrations | 20 |
| Generic Constraints | 15 |
| Thread Safety | 10 |
| Tests | 10 |
| Documentation | 5 |
| **TOTAL** | **100** |

**Pass: 75+**

---

## 💡 Tips

1. **Start with Observer**: Get pattern working first
2. **Builder Last**: Add fluent interface after core works
3. **Thread Safety**: Use `lock` for collections
4. **Variance**: Study covariance/contravariance rules carefully
5. **Test Early**: Write tests as you build
6. **Performance**: Profile with BenchmarkDotNet

---

## 📚 Resources

- `samples/02-Intermediate/CovarianceContravariance/`
- `samples/03-Advanced/GenericCovarianceContravariance/`
- `samples/99-Exercises/Generics/` (all 3)
- `samples/99-Exercises/DesignPatterns/01-Builder/`
- `samples/99-Exercises/DesignPatterns/02-Observer/`

---

*Template Version: 1.0*
*Last Updated: 2025-12-02*
