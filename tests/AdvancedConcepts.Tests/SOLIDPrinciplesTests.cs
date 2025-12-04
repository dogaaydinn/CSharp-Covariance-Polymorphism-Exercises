using Xunit;
using FluentAssertions;

namespace AdvancedConcepts.Tests;

/// <summary>
/// Tests demonstrating SOLID principles
/// </summary>
public class SOLIDPrinciplesTests
{
    #region Single Responsibility Principle (SRP)

    [Fact]
    public void SRP_UserService_ShouldOnlyHandleUserOperations()
    {
        // Arrange
        var userService = new UserService();
        var user = new User { Id = 1, Name = "John", Email = "john@test.com" };

        // Act
        var result = userService.CreateUser(user);

        // Assert
        result.Should().BeTrue();
        userService.GetUser(1).Should().NotBeNull();
    }

    [Fact]
    public void SRP_EmailService_ShouldOnlyHandleEmails()
    {
        // Arrange
        var emailService = new EmailService();

        // Act
        var result = emailService.SendEmail("test@test.com", "Subject", "Body");

        // Assert
        result.Should().BeTrue();
    }

    #endregion

    #region Open/Closed Principle (OCP)

    [Fact]
    public void OCP_AreaCalculator_ShouldWorkWithNewShapes()
    {
        // Arrange
        var calculator = new AreaCalculator();
        var shapes = new List<IShape>
        {
            new Circle(5),
            new Rectangle(4, 6),
            new Triangle(3, 4)  // New shape added without modifying calculator
        };

        // Act
        var totalArea = calculator.CalculateTotalArea(shapes);

        // Assert
        totalArea.Should().BeGreaterThan(0);
    }

    #endregion

    #region Liskov Substitution Principle (LSP)

    [Fact]
    public void LSP_Bird_SubtypesShouldBeSubstitutable()
    {
        // Arrange
        var birds = new List<Bird>
        {
            new Sparrow(),
            new Eagle(),
            new Penguin()  // Even penguins (can't fly) follow LSP
        };

        // Act & Assert
        foreach (var bird in birds)
        {
            bird.Eat();  // All birds can eat
            var canMove = bird.Move();  // All birds can move
            canMove.Should().BeTrue();
        }
    }

    [Fact]
    public void LSP_FlyingBird_OnlyFlyingBirdsCanFly()
    {
        // Arrange
        var sparrow = new Sparrow();
        var eagle = new Eagle();

        // Act
        var sparrowFlies = (sparrow as IFlyable)?.Fly() ?? false;
        var eagleFlies = (eagle as IFlyable)?.Fly() ?? false;

        // Assert
        sparrowFlies.Should().BeTrue();
        eagleFlies.Should().BeTrue();
    }

    #endregion

    #region Interface Segregation Principle (ISP)

    [Fact]
    public void ISP_Printer_OnlyImplementsNeededInterfaces()
    {
        // Arrange
        var basicPrinter = new BasicPrinter();
        var multiFunctionPrinter = new MultiFunctionPrinter();

        // Act & Assert
        basicPrinter.Should().BeAssignableTo<IPrint>();
        basicPrinter.Should().NotBeAssignableTo<IScan>();

        multiFunctionPrinter.Should().BeAssignableTo<IPrint>();
        multiFunctionPrinter.Should().BeAssignableTo<IScan>();
        multiFunctionPrinter.Should().BeAssignableTo<IFax>();
    }

    #endregion

    #region Dependency Inversion Principle (DIP)

    [Fact]
    public void DIP_OrderProcessor_DependsOnAbstraction()
    {
        // Arrange
        var emailNotifier = new EmailNotifier();
        var processor = new OrderProcessor(emailNotifier);
        var order = new Order { Id = 1, Total = 100m };

        // Act
        var result = processor.ProcessOrder(order);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void DIP_CanSwitchImplementationsEasily()
    {
        // Arrange
        var smsNotifier = new SMSNotifier();
        var processor = new OrderProcessor(smsNotifier);
        var order = new Order { Id = 2, Total = 200m };

        // Act
        var result = processor.ProcessOrder(order);

        // Assert
        result.Should().BeTrue();
    }

    #endregion
}

// Test implementations
#region SRP - Single Responsibility

public class User
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Email { get; set; } = "";
}

public class UserService
{
    private readonly Dictionary<int, User> _users = new();

    public bool CreateUser(User user)
    {
        _users[user.Id] = user;
        return true;
    }

    public User? GetUser(int id) => _users.GetValueOrDefault(id);
}

public class EmailService
{
    public bool SendEmail(string to, string subject, string body)
    {
        // Simulated email sending
        return !string.IsNullOrEmpty(to);
    }
}

#endregion

#region OCP - Open/Closed

public interface IShape
{
    double CalculateArea();
}

public class Circle : IShape
{
    private readonly double _radius;
    public Circle(double radius) => _radius = radius;
    public double CalculateArea() => Math.PI * _radius * _radius;
}

public class Rectangle : IShape
{
    private readonly double _width;
    private readonly double _height;
    public Rectangle(double width, double height) => (_width, _height) = (width, height);
    public double CalculateArea() => _width * _height;
}

public class Triangle : IShape
{
    private readonly double _baseLength;
    private readonly double _height;
    public Triangle(double baseLength, double height) => (_baseLength, _height) = (baseLength, height);
    public double CalculateArea() => 0.5 * _baseLength * _height;
}

public class AreaCalculator
{
    public double CalculateTotalArea(IEnumerable<IShape> shapes) =>
        shapes.Sum(s => s.CalculateArea());
}

#endregion

#region LSP - Liskov Substitution

public abstract class Bird
{
    public abstract void Eat();
    public abstract bool Move();
}

public interface IFlyable
{
    bool Fly();
}

public class Sparrow : Bird, IFlyable
{
    public override void Eat() { }
    public override bool Move() => true;
    public bool Fly() => true;
}

public class Eagle : Bird, IFlyable
{
    public override void Eat() { }
    public override bool Move() => true;
    public bool Fly() => true;
}

public class Penguin : Bird
{
    public override void Eat() { }
    public override bool Move() => true;  // Penguins swim/walk
}

#endregion

#region ISP - Interface Segregation

public interface IPrint
{
    void Print(string document);
}

public interface IScan
{
    void Scan(string document);
}

public interface IFax
{
    void Fax(string document);
}

public class BasicPrinter : IPrint
{
    public void Print(string document) { }
}

public class MultiFunctionPrinter : IPrint, IScan, IFax
{
    public void Print(string document) { }
    public void Scan(string document) { }
    public void Fax(string document) { }
}

#endregion

#region DIP - Dependency Inversion

public interface INotifier
{
    void Notify(string message);
}

public class EmailNotifier : INotifier
{
    public void Notify(string message) { }
}

public class SMSNotifier : INotifier
{
    public void Notify(string message) { }
}

public class Order
{
    public int Id { get; set; }
    public decimal Total { get; set; }
}

public class OrderProcessor
{
    private readonly INotifier _notifier;

    public OrderProcessor(INotifier notifier)
    {
        _notifier = notifier;
    }

    public bool ProcessOrder(Order order)
    {
        // Process order logic
        _notifier.Notify($"Order {order.Id} processed");
        return true;
    }
}

#endregion
