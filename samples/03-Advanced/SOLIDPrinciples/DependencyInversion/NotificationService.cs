namespace SOLIDPrinciples.DependencyInversion;

// ❌ BAD: High-level module depends on low-level concrete class
public class BadEmailNotifier
{
    public void SendEmail(string message)
    {
        Console.WriteLine($"Sending email: {message}");
    }
}

public class BadNotificationService
{
    private readonly BadEmailNotifier _emailNotifier = new(); // Tight coupling!

    public void Notify(string message)
    {
        _emailNotifier.SendEmail(message); // Can't change notification method!
    }
}

// ✅ GOOD: Both high-level and low-level depend on abstraction
public interface INotifier
{
    void Notify(string message);
}

// Low-level modules
public class EmailNotifier : INotifier
{
    public void Notify(string message)
    {
        Console.WriteLine($"✅ Email: {message}");
    }
}

public class SmsNotifier : INotifier
{
    public void Notify(string message)
    {
        Console.WriteLine($"✅ SMS: {message}");
    }
}

public class PushNotifier : INotifier
{
    public void Notify(string message)
    {
        Console.WriteLine($"✅ Push notification: {message}");
    }
}

// High-level module depends on abstraction
public class NotificationService
{
    private readonly INotifier _notifier;

    public NotificationService(INotifier notifier) // Dependency injection!
    {
        _notifier = notifier;
    }

    public void Notify(string message)
    {
        _notifier.Notify(message);
    }
}

// WHY?
// - High-level modules don't depend on low-level details
// - Both depend on abstractions (INotifier)
// - Easy to swap notification methods
// - Enables dependency injection
// - More testable (can mock INotifier)
