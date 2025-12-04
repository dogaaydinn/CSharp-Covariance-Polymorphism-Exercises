using Xunit;
using FluentAssertions;

namespace AdvancedConcepts.UnitTests.Advanced;

/// <summary>
/// Comprehensive tests for SOLID principles
/// Tests demonstrate proper application of each SOLID principle
/// </summary>
public class SOLIDPrinciplesTests
{
    #region Single Responsibility Principle (SRP) Tests

    [Fact]
    public void SRP_UserService_ShouldOnlyHandleUserOperations()
    {
        // Arrange
        var userService = new UserService();

        // Act & Assert - Should only do user-related operations
        var user = userService.CreateUser("John", "john@example.com");
        user.Should().NotBeNull();
        user.Name.Should().Be("John");
        user.Email.Should().Be("john@example.com");
    }

    [Fact]
    public void SRP_EmailService_ShouldOnlyHandleEmailOperations()
    {
        // Arrange
        var emailService = new EmailService();

        // Act & Assert - Should only send emails
        var result = emailService.SendEmail("test@example.com", "Subject", "Body");
        result.Should().BeTrue();
    }

    [Fact]
    public void SRP_LoggingService_ShouldOnlyHandleLogging()
    {
        // Arrange
        var loggingService = new LoggingService();

        // Act & Assert - Should only log messages
        Action act = () => loggingService.Log("Test message");
        act.Should().NotThrow();
    }

    [Fact]
    public void SRP_Violation_GodClass_ShouldBeAvoided()
    {
        // This test demonstrates what NOT to do
        // A God class doing too many things is a SRP violation
        var godClass = new GodClassExample();

        // God class has too many responsibilities
        godClass.Should().NotBeNull();
    }

    #endregion

    #region Open/Closed Principle (OCP) Tests

    [Fact]
    public void OCP_ShapeCalculator_ShouldBeOpenForExtension()
    {
        // Arrange
        var shapes = new List<IShape>
        {
            new Rectangle(5, 10),
            new Circle(7)
        };

        // Act
        var totalArea = shapes.Sum(s => s.CalculateArea());

        // Assert - Can add new shapes without modifying existing code
        totalArea.Should().BeGreaterThan(0);
    }

    [Fact]
    public void OCP_Rectangle_CalculateArea_ShouldBeCorrect()
    {
        // Arrange
        var rectangle = new Rectangle(5, 10);

        // Act
        var area = rectangle.CalculateArea();

        // Assert
        area.Should().Be(50);
    }

    [Fact]
    public void OCP_Circle_CalculateArea_ShouldBeCorrect()
    {
        // Arrange
        var circle = new Circle(5);

        // Act
        var area = circle.CalculateArea();

        // Assert
        area.Should().BeApproximately(78.54, 0.01);
    }

    [Fact]
    public void OCP_NewShapeAddition_ShouldNotModifyExistingCode()
    {
        // Arrange - Adding Triangle without modifying Rectangle or Circle
        var triangle = new Triangle(6, 8);

        // Act
        var area = triangle.CalculateArea();

        // Assert
        area.Should().Be(24);
    }

    #endregion

    #region Liskov Substitution Principle (LSP) Tests

    [Fact]
    public void LSP_Rectangle_ShouldBeSubstitutableWithShape()
    {
        // Arrange
        IShape shape = new Rectangle(4, 5);

        // Act
        var area = shape.CalculateArea();

        // Assert - Rectangle can be used wherever IShape is expected
        area.Should().Be(20);
    }

    [Fact]
    public void LSP_AllShapes_ShouldBeSubstitutable()
    {
        // Arrange
        var shapes = new List<IShape>
        {
            new Rectangle(3, 4),
            new Circle(3),
            new Triangle(4, 5)
        };

        // Act & Assert - All shapes should work with same interface
        foreach (var shape in shapes)
        {
            var area = shape.CalculateArea();
            area.Should().BeGreaterThan(0);
        }
    }

    [Fact]
    public void LSP_DerivedClass_ShouldNotWeakenPreconditions()
    {
        // Arrange
        Bird bird = new Sparrow();

        // Act & Assert - Derived class respects base class contract
        Action act = () => bird.Fly();
        act.Should().NotThrow();
    }

    [Fact]
    public void LSP_Violation_SquareInheritingRectangle_IsProblematic()
    {
        // This demonstrates LSP violation - Square as Rectangle subclass breaks expectations
        // When width and height must be equal, it violates Rectangle's behavior
        var square = new Square(5);
        square.Width.Should().Be(square.Height);
    }

    #endregion

    #region Interface Segregation Principle (ISP) Tests

    [Fact]
    public void ISP_Printer_ShouldOnlyImplementNeededInterfaces()
    {
        // Arrange
        IPrinter printer = new SimplePrinter();

        // Act & Assert - Only implements printing, not scanning
        var result = printer.Print("Document");
        result.Should().BeTrue();
    }

    [Fact]
    public void ISP_MultiFunctionDevice_ShouldImplementAllInterfaces()
    {
        // Arrange
        var mfd = new MultiFunctionDevice();

        // Act & Assert - Can do multiple things via separate interfaces
        mfd.Print("Document").Should().BeTrue();
        mfd.Scan().Should().NotBeEmpty();
        mfd.Fax("1234567890").Should().BeTrue();
    }

    [Fact]
    public void ISP_ClientsShouldNotDependOnUnusedInterfaces()
    {
        // Arrange - Simple printer doesn't need scanning capability
        IPrinter printer = new SimplePrinter();

        // Act & Assert - Compiler prevents calling non-existent Scan method
        printer.Should().BeAssignableTo<IPrinter>();
        printer.Should().NotBeAssignableTo<IScanner>();
    }

    [Fact]
    public void ISP_FineGrainedInterfaces_AllowFlexibleImplementation()
    {
        // Arrange
        var devices = new List<IPrinter>
        {
            new SimplePrinter(),
            new MultiFunctionDevice()
        };

        // Act & Assert
        devices.Should().HaveCount(2);
        devices.Should().AllBeAssignableTo<IPrinter>();
    }

    #endregion

    #region Dependency Inversion Principle (DIP) Tests

    [Fact]
    public void DIP_HighLevelModule_ShouldDependOnAbstraction()
    {
        // Arrange
        ILogger logger = new FileLogger();
        var service = new OrderService(logger);

        // Act
        service.ProcessOrder("Order123");

        // Assert - High-level OrderService depends on ILogger abstraction
        service.Should().NotBeNull();
    }

    [Fact]
    public void DIP_CanSwitchImplementations_ViaAbstraction()
    {
        // Arrange - Can switch from FileLogger to DatabaseLogger
        ILogger fileLogger = new FileLogger();
        ILogger dbLogger = new DatabaseLogger();

        var service1 = new OrderService(fileLogger);
        var service2 = new OrderService(dbLogger);

        // Act & Assert - Both work with same interface
        service1.Should().NotBeNull();
        service2.Should().NotBeNull();
    }

    [Fact]
    public void DIP_AbstractionNotDependOnDetails()
    {
        // Arrange
        INotificationService notificationService = new EmailNotificationService();
        var userController = new UserController(notificationService);

        // Act
        var result = userController.RegisterUser("John", "john@example.com");

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void DIP_TestabilityImprovedWithDI()
    {
        // Arrange - Can inject mock for testing
        ILogger mockLogger = new MockLogger();
        var service = new OrderService(mockLogger);

        // Act
        service.ProcessOrder("TestOrder");

        // Assert - Easy to test with mock
        ((MockLogger)mockLogger).LogCount.Should().BeGreaterThan(0);
    }

    #endregion

    #region Integration Tests - Multiple SOLID Principles

    [Fact]
    public void SOLID_Combined_ShapeProcessing_FollowsAllPrinciples()
    {
        // Demonstrates SRP, OCP, LSP together
        // Arrange
        var shapes = new List<IShape>
        {
            new Rectangle(5, 10),
            new Circle(7),
            new Triangle(6, 8)
        };

        var calculator = new AreaCalculator();

        // Act
        var totalArea = calculator.CalculateTotalArea(shapes);

        // Assert
        totalArea.Should().BeGreaterThan(0);
    }

    [Fact]
    public void SOLID_DependencyInjection_EnablesFlexibility()
    {
        // Demonstrates DIP, ISP together
        // Arrange
        ILogger logger = new FileLogger();
        INotificationService notifier = new EmailNotificationService();
        var service = new ComplexService(logger, notifier);

        // Act & Assert
        service.Should().NotBeNull();
    }

    #endregion

    #region Helper Classes for Tests

    // SRP Examples
    private class UserService
    {
        public User CreateUser(string name, string email) => new User { Name = name, Email = email };
    }

    private class EmailService
    {
        public bool SendEmail(string to, string subject, string body) => true;
    }

    private class LoggingService
    {
        public void Log(string message) { }
    }

    private class GodClassExample
    {
        // Anti-pattern: Does everything
    }

    private class User
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }

    // OCP Examples
    private interface IShape
    {
        double CalculateArea();
    }

    private class Rectangle : IShape
    {
        public double Width { get; set; }
        public double Height { get; set; }
        public Rectangle(double width, double height)
        {
            Width = width;
            Height = height;
        }
        public double CalculateArea() => Width * Height;
    }

    private class Circle : IShape
    {
        public double Radius { get; set; }
        public Circle(double radius) => Radius = radius;
        public double CalculateArea() => Math.PI * Radius * Radius;
    }

    private class Triangle : IShape
    {
        public double Base { get; set; }
        public double Height { get; set; }
        public Triangle(double baseLength, double height)
        {
            Base = baseLength;
            Height = height;
        }
        public double CalculateArea() => 0.5 * Base * Height;
    }

    // LSP Examples
    private class Bird
    {
        public virtual void Fly() { }
    }

    private class Sparrow : Bird
    {
        public override void Fly() { }
    }

    private class Square
    {
        public double Width { get; set; }
        public double Height { get; set; }
        public Square(double size)
        {
            Width = size;
            Height = size;
        }
    }

    // ISP Examples
    private interface IPrinter
    {
        bool Print(string document);
    }

    private interface IScanner
    {
        string Scan();
    }

    private interface IFax
    {
        bool Fax(string number);
    }

    private class SimplePrinter : IPrinter
    {
        public bool Print(string document) => true;
    }

    private class MultiFunctionDevice : IPrinter, IScanner, IFax
    {
        public bool Print(string document) => true;
        public string Scan() => "Scanned content";
        public bool Fax(string number) => true;
    }

    // DIP Examples
    private interface ILogger
    {
        void Log(string message);
    }

    private class FileLogger : ILogger
    {
        public void Log(string message) { }
    }

    private class DatabaseLogger : ILogger
    {
        public void Log(string message) { }
    }

    private class MockLogger : ILogger
    {
        public int LogCount { get; private set; }
        public void Log(string message) => LogCount++;
    }

    private class OrderService
    {
        private readonly ILogger _logger;
        public OrderService(ILogger logger) => _logger = logger;
        public void ProcessOrder(string orderId) => _logger.Log($"Processing {orderId}");
    }

    private interface INotificationService
    {
        void Notify(string message);
    }

    private class EmailNotificationService : INotificationService
    {
        public void Notify(string message) { }
    }

    private class UserController
    {
        private readonly INotificationService _notificationService;
        public UserController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }
        public bool RegisterUser(string name, string email)
        {
            _notificationService.Notify($"User {name} registered");
            return true;
        }
    }

    private class AreaCalculator
    {
        public double CalculateTotalArea(List<IShape> shapes) => shapes.Sum(s => s.CalculateArea());
    }

    private class ComplexService
    {
        private readonly ILogger _logger;
        private readonly INotificationService _notifier;
        public ComplexService(ILogger logger, INotificationService notifier)
        {
            _logger = logger;
            _notifier = notifier;
        }
    }

    #endregion
}
