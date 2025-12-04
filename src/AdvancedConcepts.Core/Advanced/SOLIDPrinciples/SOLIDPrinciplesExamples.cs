namespace AdvancedCsharpConcepts.Advanced.SOLIDPrinciples;

/// <summary>
/// SOLID Principles - Enterprise architecture fundamentals.
/// Silicon Valley/NVIDIA best practices for maintainable, scalable systems.
/// </summary>
public static class SOLIDPrinciplesExamples
{
    #region Single Responsibility Principle (SRP)

    /// <summary>
    /// BAD: Class with multiple responsibilities.
    /// </summary>
    public class BadUserService
    {
        public void CreateUser(string name, string email)
        {
            // Validation
            if (string.IsNullOrEmpty(name)) throw new ArgumentException("Name required");

            // Database logic
            Console.WriteLine($"Saving user {name} to database");

            // Email logic
            Console.WriteLine($"Sending welcome email to {email}");

            // Logging
            Console.WriteLine($"User {name} created at {DateTime.Now}");
        }
    }

    /// <summary>
    /// GOOD: Each class has a single responsibility.
    /// </summary>
    public class User
    {
        public required string Name { get; init; }
        public required string Email { get; init; }
    }

    public interface IUserValidator
    {
        void Validate(User user);
    }

    public class UserValidator : IUserValidator
    {
        public void Validate(User user)
        {
            if (string.IsNullOrEmpty(user.Name))
                throw new ArgumentException("Name is required");
            if (string.IsNullOrEmpty(user.Email))
                throw new ArgumentException("Email is required");
        }
    }

    public interface IUserRepository
    {
        void Save(User user);
    }

    public class UserRepository : IUserRepository
    {
        public void Save(User user)
        {
            Console.WriteLine($"   → Saving user {user.Name} to database");
        }
    }

    public interface IEmailService
    {
        void SendWelcomeEmail(User user);
    }

    public class EmailService : IEmailService
    {
        public void SendWelcomeEmail(User user)
        {
            Console.WriteLine($"   → Sending welcome email to {user.Email}");
        }
    }

    public class GoodUserService
    {
        private readonly IUserValidator _validator;
        private readonly IUserRepository _repository;
        private readonly IEmailService _emailService;

        public GoodUserService(
            IUserValidator validator,
            IUserRepository repository,
            IEmailService emailService)
        {
            _validator = validator;
            _repository = repository;
            _emailService = emailService;
        }

        public void CreateUser(User user)
        {
            _validator.Validate(user);
            _repository.Save(user);
            _emailService.SendWelcomeEmail(user);
        }
    }

    #endregion

    #region Open/Closed Principle (OCP)

    /// <summary>
    /// BAD: Modifying class for every new shape type.
    /// </summary>
    public class BadAreaCalculator
    {
        public double CalculateArea(object shape)
        {
            if (shape is Circle circle)
                return Math.PI * circle.Radius * circle.Radius;
            if (shape is Rectangle rectangle)
                return rectangle.Width * rectangle.Height;
            // Need to modify this method for every new shape!
            throw new NotSupportedException("Unknown shape");
        }
    }

    /// <summary>
    /// GOOD: Open for extension, closed for modification.
    /// </summary>
    public interface IShape
    {
        double CalculateArea();
    }

    public class Circle : IShape
    {
        public double Radius { get; init; }
        public double CalculateArea() => Math.PI * Radius * Radius;
    }

    public class Rectangle : IShape
    {
        public double Width { get; init; }
        public double Height { get; init; }
        public double CalculateArea() => Width * Height;
    }

    public class Triangle : IShape
    {
        public double Base { get; init; }
        public double Height { get; init; }
        public double CalculateArea() => 0.5 * Base * Height;
    }

    public class GoodAreaCalculator
    {
        public double CalculateTotalArea(IEnumerable<IShape> shapes)
        {
            return shapes.Sum(s => s.CalculateArea());
        }
    }

    #endregion

    #region Liskov Substitution Principle (LSP)

    /// <summary>
    /// BAD: Derived class violates base class contract.
    /// </summary>
    public class BadBird
    {
        public virtual void Fly()
        {
            Console.WriteLine("Flying...");
        }
    }

    public class BadPenguin : BadBird
    {
        public override void Fly()
        {
            throw new NotSupportedException("Penguins can't fly!");
        }
    }

    /// <summary>
    /// GOOD: Proper abstraction hierarchy.
    /// </summary>
    public interface IBird
    {
        void Eat();
    }

    public interface IFlyingBird : IBird
    {
        void Fly();
    }

    public interface ISwimmingBird : IBird
    {
        void Swim();
    }

    public class Sparrow : IFlyingBird
    {
        public void Eat() => Console.WriteLine("   → Sparrow eating");
        public void Fly() => Console.WriteLine("   → Sparrow flying");
    }

    public class Penguin : ISwimmingBird
    {
        public void Eat() => Console.WriteLine("   → Penguin eating");
        public void Swim() => Console.WriteLine("   → Penguin swimming");
    }

    #endregion

    #region Interface Segregation Principle (ISP)

    /// <summary>
    /// BAD: Fat interface forces clients to implement unused methods.
    /// </summary>
    public interface IBadWorker
    {
        void Work();
        void Eat();
        void Sleep();
    }

    public class BadHumanWorker : IBadWorker
    {
        public void Work() => Console.WriteLine("Working");
        public void Eat() => Console.WriteLine("Eating lunch");
        public void Sleep() => Console.WriteLine("Sleeping");
    }

    public class BadRobotWorker : IBadWorker
    {
        public void Work() => Console.WriteLine("Working");
        public void Eat() => throw new NotSupportedException("Robots don't eat!");
        public void Sleep() => throw new NotSupportedException("Robots don't sleep!");
    }

    /// <summary>
    /// GOOD: Segregated interfaces - clients depend only on what they use.
    /// </summary>
    public interface IWorkable
    {
        void Work();
    }

    public interface IEatable
    {
        void Eat();
    }

    public interface ISleepable
    {
        void Sleep();
    }

    public class GoodHumanWorker : IWorkable, IEatable, ISleepable
    {
        public void Work() => Console.WriteLine("   → Human working");
        public void Eat() => Console.WriteLine("   → Human eating");
        public void Sleep() => Console.WriteLine("   → Human sleeping");
    }

    public class GoodRobotWorker : IWorkable
    {
        public void Work() => Console.WriteLine("   → Robot working 24/7");
    }

    #endregion

    #region Dependency Inversion Principle (DIP)

    /// <summary>
    /// BAD: High-level module depends on low-level module.
    /// </summary>
    public class BadSqlDatabase
    {
        public void Save(string data)
        {
            Console.WriteLine($"Saving to SQL: {data}");
        }
    }

    public class BadBusinessLogic
    {
        private readonly BadSqlDatabase _database = new();

        public void ProcessData(string data)
        {
            // Business logic
            var processed = data.ToUpper();
            _database.Save(processed); // Tightly coupled!
        }
    }

    /// <summary>
    /// GOOD: Both depend on abstraction.
    /// </summary>
    public interface IDatabase
    {
        void Save(string data);
    }

    public class SqlDatabase : IDatabase
    {
        public void Save(string data)
        {
            Console.WriteLine($"   → Saving to SQL: {data}");
        }
    }

    public class MongoDatabase : IDatabase
    {
        public void Save(string data)
        {
            Console.WriteLine($"   → Saving to MongoDB: {data}");
        }
    }

    public class InMemoryDatabase : IDatabase
    {
        public void Save(string data)
        {
            Console.WriteLine($"   → Saving to memory: {data}");
        }
    }

    public class GoodBusinessLogic
    {
        private readonly IDatabase _database;

        public GoodBusinessLogic(IDatabase database)
        {
            _database = database;
        }

        public void ProcessData(string data)
        {
            var processed = data.ToUpper();
            _database.Save(processed);
        }
    }

    #endregion

    /// <summary>
    /// Demonstrates all SOLID principles.
    /// </summary>
    public static void RunExamples()
    {
        Console.WriteLine("=== SOLID Principles Examples ===\n");

        // 1. Single Responsibility Principle
        Console.WriteLine("1. Single Responsibility Principle (SRP):");
        var validator = new UserValidator();
        var repository = new UserRepository();
        var emailService = new EmailService();
        var userService = new GoodUserService(validator, repository, emailService);

        var user = new User { Name = "Alice", Email = "alice@example.com" };
        userService.CreateUser(user);

        // 2. Open/Closed Principle
        Console.WriteLine("\n2. Open/Closed Principle (OCP):");
        var shapes = new List<IShape>
        {
            new Circle { Radius = 5 },
            new Rectangle { Width = 4, Height = 6 },
            new Triangle { Base = 3, Height = 4 }
        };

        var areaCalculator = new GoodAreaCalculator();
        var totalArea = areaCalculator.CalculateTotalArea(shapes);
        Console.WriteLine($"   → Total area: {totalArea:F2}");

        // 3. Liskov Substitution Principle
        Console.WriteLine("\n3. Liskov Substitution Principle (LSP):");
        var sparrow = new Sparrow();
        sparrow.Eat();
        sparrow.Fly();

        var penguin = new Penguin();
        penguin.Eat();
        penguin.Swim();

        // 4. Interface Segregation Principle
        Console.WriteLine("\n4. Interface Segregation Principle (ISP):");
        var human = new GoodHumanWorker();
        human.Work();
        human.Eat();

        var robot = new GoodRobotWorker();
        robot.Work();

        // 5. Dependency Inversion Principle
        Console.WriteLine("\n5. Dependency Inversion Principle (DIP):");
        var businessLogic1 = new GoodBusinessLogic(new SqlDatabase());
        businessLogic1.ProcessData("Order #123");

        var businessLogic2 = new GoodBusinessLogic(new MongoDatabase());
        businessLogic2.ProcessData("Order #456");

        var businessLogic3 = new GoodBusinessLogic(new InMemoryDatabase());
        businessLogic3.ProcessData("Order #789");

        Console.WriteLine("\n✓ All SOLID principles demonstrated!");
    }
}
