using Xunit;
using FluentAssertions;
using AdvancedConcepts.Core.DesignPatterns.Creational;
using AdvancedConcepts.Core.DesignPatterns.Structural;
using AdvancedConcepts.Core.DesignPatterns.Behavioral;

namespace AdvancedConcepts.Tests;

/// <summary>
/// Tests for Design Patterns implementations
/// </summary>
public class DesignPatternsTests
{
    #region Singleton Pattern Tests

    [Fact]
    public void Singleton_ShouldReturnSameInstance()
    {
        // Arrange & Act
        var instance1 = Singleton.Instance;
        var instance2 = Singleton.Instance;

        // Assert
        instance1.Should().BeSameAs(instance2);
    }

    [Fact]
    public void Singleton_ShouldBeThreadSafe()
    {
        // Arrange
        var instances = new Singleton[10];
        var tasks = new Task[10];

        // Act
        for (int i = 0; i < 10; i++)
        {
            int index = i;
            tasks[i] = Task.Run(() => instances[index] = Singleton.Instance);
        }

        Task.WaitAll(tasks);

        // Assert
        instances.Should().OnlyHaveUniqueItems().And.HaveCount(1);
    }

    #endregion

    #region Factory Pattern Tests

    [Theory]
    [InlineData("Circle")]
    [InlineData("Rectangle")]
    [InlineData("Triangle")]
    public void Factory_ShouldCreateCorrectShapeType(string shapeType)
    {
        // Act
        var shape = ShapeFactory.CreateShape(shapeType);

        // Assert
        shape.Should().NotBeNull();
        shape.GetType().Name.Should().Be(shapeType);
    }

    [Fact]
    public void Factory_ShouldThrowForInvalidType()
    {
        // Act
        Action act = () => ShapeFactory.CreateShape("InvalidShape");

        // Assert
        act.Should().Throw<ArgumentException>();
    }

    #endregion

    #region Builder Pattern Tests

    [Fact]
    public void Builder_ShouldBuildComplexObject()
    {
        // Act
        var computer = new ComputerBuilder()
            .SetCPU("Intel i9")
            .SetRAM(32)
            .SetStorage(1000)
            .SetGPU("NVIDIA RTX 4090")
            .Build();

        // Assert
        computer.CPU.Should().Be("Intel i9");
        computer.RAM.Should().Be(32);
        computer.Storage.Should().Be(1000);
        computer.GPU.Should().Be("NVIDIA RTX 4090");
    }

    [Fact]
    public void Builder_ShouldAllowFluentAPI()
    {
        // Act
        var result = new ComputerBuilder()
            .SetCPU("AMD Ryzen 9")
            .SetRAM(16);

        // Assert
        result.Should().BeOfType<ComputerBuilder>();
    }

    #endregion

    #region Decorator Pattern Tests

    [Fact]
    public void Decorator_ShouldAddBehaviorDynamically()
    {
        // Arrange
        INotifier notifier = new EmailNotifier();

        // Act
        notifier = new SMSDecorator(notifier);
        notifier = new SlackDecorator(notifier);

        var result = notifier.Send("Test message");

        // Assert
        result.Should().Contain("Email");
        result.Should().Contain("SMS");
        result.Should().Contain("Slack");
    }

    [Fact]
    public void Decorator_ShouldMaintainBaseInterface()
    {
        // Arrange
        INotifier baseNotifier = new EmailNotifier();
        INotifier decorated = new SMSDecorator(baseNotifier);

        // Assert
        decorated.Should().BeAssignableTo<INotifier>();
    }

    #endregion

    #region Strategy Pattern Tests

    [Fact]
    public void Strategy_ShouldExecuteDifferentAlgorithms()
    {
        // Arrange
        var context = new PaymentContext();

        // Act & Assert - Credit Card
        context.SetStrategy(new CreditCardStrategy("1234"));
        var result1 = context.ExecutePayment(100m);
        result1.Should().Contain("Credit Card");

        // Act & Assert - PayPal
        context.SetStrategy(new PayPalStrategy("user@example.com"));
        var result2 = context.ExecutePayment(100m);
        result2.Should().Contain("PayPal");
    }

    [Fact]
    public void Strategy_ShouldAllowRuntimeStrategyChange()
    {
        // Arrange
        var context = new PaymentContext();
        context.SetStrategy(new CreditCardStrategy("1234"));

        // Act
        context.SetStrategy(new PayPalStrategy("test@test.com"));
        var result = context.ExecutePayment(50m);

        // Assert
        result.Should().Contain("PayPal");
    }

    #endregion

    #region Observer Pattern Tests

    [Fact]
    public void Observer_ShouldNotifyAllSubscribers()
    {
        // Arrange
        var subject = new WeatherSubject();
        var observer1 = new WeatherDisplay("Display1");
        var observer2 = new WeatherDisplay("Display2");

        subject.Attach(observer1);
        subject.Attach(observer2);

        // Act
        subject.SetTemperature(25.5f);

        // Assert
        observer1.LastTemperature.Should().Be(25.5f);
        observer2.LastTemperature.Should().Be(25.5f);
    }

    [Fact]
    public void Observer_ShouldSupportUnsubscribe()
    {
        // Arrange
        var subject = new WeatherSubject();
        var observer1 = new WeatherDisplay("Display1");
        var observer2 = new WeatherDisplay("Display2");

        subject.Attach(observer1);
        subject.Attach(observer2);
        subject.SetTemperature(20f);

        // Act
        subject.Detach(observer1);
        subject.SetTemperature(30f);

        // Assert
        observer1.LastTemperature.Should().Be(20f); // Not updated
        observer2.LastTemperature.Should().Be(30f); // Updated
    }

    #endregion

    #region Adapter Pattern Tests

    [Fact]
    public void Adapter_ShouldAdaptIncompatibleInterface()
    {
        // Arrange
        var legacyPrinter = new LegacyPrinter();
        IModernPrinter adapter = new PrinterAdapter(legacyPrinter);

        // Act
        var result = adapter.Print("Test Document");

        // Assert
        result.Should().Contain("Test Document");
    }

    #endregion
}

// Test helper classes
namespace AdvancedConcepts.Core.DesignPatterns.Creational
{
    public class Singleton
    {
        private static readonly Lazy<Singleton> _instance = new(() => new Singleton());
        public static Singleton Instance => _instance.Value;
        private Singleton() { }
    }

    public interface IShape { }
    public class Circle : IShape { }
    public class Rectangle : IShape { }
    public class Triangle : IShape { }

    public static class ShapeFactory
    {
        public static IShape CreateShape(string type) => type switch
        {
            "Circle" => new Circle(),
            "Rectangle" => new Rectangle(),
            "Triangle" => new Triangle(),
            _ => throw new ArgumentException($"Unknown shape: {type}")
        };
    }

    public class Computer
    {
        public string CPU { get; set; } = "";
        public int RAM { get; set; }
        public int Storage { get; set; }
        public string? GPU { get; set; }
    }

    public class ComputerBuilder
    {
        private readonly Computer _computer = new();

        public ComputerBuilder SetCPU(string cpu) { _computer.CPU = cpu; return this; }
        public ComputerBuilder SetRAM(int ram) { _computer.RAM = ram; return this; }
        public ComputerBuilder SetStorage(int storage) { _computer.Storage = storage; return this; }
        public ComputerBuilder SetGPU(string gpu) { _computer.GPU = gpu; return this; }
        public Computer Build() => _computer;
    }
}

namespace AdvancedConcepts.Core.DesignPatterns.Structural
{
    public interface INotifier
    {
        string Send(string message);
    }

    public class EmailNotifier : INotifier
    {
        public string Send(string message) => $"Email: {message}";
    }

    public abstract class NotifierDecorator : INotifier
    {
        protected readonly INotifier _notifier;
        public NotifierDecorator(INotifier notifier) => _notifier = notifier;
        public virtual string Send(string message) => _notifier.Send(message);
    }

    public class SMSDecorator : NotifierDecorator
    {
        public SMSDecorator(INotifier notifier) : base(notifier) { }
        public override string Send(string message) => $"{_notifier.Send(message)} + SMS: {message}";
    }

    public class SlackDecorator : NotifierDecorator
    {
        public SlackDecorator(INotifier notifier) : base(notifier) { }
        public override string Send(string message) => $"{_notifier.Send(message)} + Slack: {message}";
    }

    public interface IModernPrinter
    {
        string Print(string document);
    }

    public class LegacyPrinter
    {
        public string OldPrint(string text) => $"Printing: {text}";
    }

    public class PrinterAdapter : IModernPrinter
    {
        private readonly LegacyPrinter _printer;
        public PrinterAdapter(LegacyPrinter printer) => _printer = printer;
        public string Print(string document) => _printer.OldPrint(document);
    }
}

namespace AdvancedConcepts.Core.DesignPatterns.Behavioral
{
    public interface IPaymentStrategy
    {
        string Pay(decimal amount);
    }

    public class CreditCardStrategy : IPaymentStrategy
    {
        private readonly string _cardNumber;
        public CreditCardStrategy(string cardNumber) => _cardNumber = cardNumber;
        public string Pay(decimal amount) => $"Paid ${amount} with Credit Card {_cardNumber}";
    }

    public class PayPalStrategy : IPaymentStrategy
    {
        private readonly string _email;
        public PayPalStrategy(string email) => _email = email;
        public string Pay(decimal amount) => $"Paid ${amount} with PayPal {_email}";
    }

    public class PaymentContext
    {
        private IPaymentStrategy? _strategy;
        public void SetStrategy(IPaymentStrategy strategy) => _strategy = strategy;
        public string ExecutePayment(decimal amount) => _strategy?.Pay(amount) ?? "";
    }

    public interface IObserver
    {
        void Update(float temperature);
    }

    public interface ISubject
    {
        void Attach(IObserver observer);
        void Detach(IObserver observer);
    }

    public class WeatherSubject : ISubject
    {
        private readonly List<IObserver> _observers = new();
        public void Attach(IObserver observer) => _observers.Add(observer);
        public void Detach(IObserver observer) => _observers.Remove(observer);

        public void SetTemperature(float temp)
        {
            foreach (var observer in _observers)
                observer.Update(temp);
        }
    }

    public class WeatherDisplay : IObserver
    {
        public string Name { get; }
        public float LastTemperature { get; private set; }

        public WeatherDisplay(string name) => Name = name;
        public void Update(float temperature) => LastTemperature = temperature;
    }
}
