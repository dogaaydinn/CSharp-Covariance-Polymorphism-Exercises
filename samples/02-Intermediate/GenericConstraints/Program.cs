// SCENARIO: Generic constraints - where T: class, new(), struct, interface
// BAD PRACTICE: Generic without constraints - runtime errors
// GOOD PRACTICE: Constraints ile compile-time safety

using GenericConstraints;

class Program
{
    static void Main()
    {
        Console.WriteLine("=== Generic Constraints ===\n");

        Console.WriteLine("=== 1. Class + Interface + new() Constraint ===\n");
        DemonstrateClassConstraint();

        Console.WriteLine("\n=== 2. Struct Constraint ===\n");
        DemonstrateStructConstraint();

        Console.WriteLine("\n=== 3. Multiple Type Parameters ===\n");
        DemonstrateMultipleConstraints();

        Console.WriteLine("\n=== 4. Unmanaged Constraint ===\n");
        DemonstrateUnmanagedConstraint();

        Console.WriteLine("\n=== Analysis ===");
        Console.WriteLine("• where T : class - Reference type");
        Console.WriteLine("• where T : struct - Value type");
        Console.WriteLine("• where T : new() - Parameterless constructor");
        Console.WriteLine("• where T : Interface - Interface implementation");
        Console.WriteLine("• where T : unmanaged - Unmanaged types only");
    }

    static void DemonstrateClassConstraint()
    {
        var userRepo = new Repository<User>();

        // new() constraint kullanımı
        var newUser = userRepo.Create();
        newUser.Name = "Ali";
        newUser.Email = "ali@example.com";

        userRepo.Add(newUser);

        var user2 = new User { Name = "Ayşe", Email = "ayse@example.com" };
        userRepo.Add(user2);

        Console.WriteLine($"\nTotal users: {userRepo.GetAll().Count}");
    }

    static void DemonstrateStructConstraint()
    {
        var intContainer = new ValueContainer<int> { Value = 42 };
        intContainer.Display();

        var doubleContainer = new ValueContainer<double> { Value = 3.14 };
        doubleContainer.Display();

        // Default value
        var emptyContainer = new ValueContainer<int>();
        emptyContainer.Display();
    }

    static void DemonstrateMultipleConstraints()
    {
        var manager = new Manager<Product, int>();

        var product1 = new Product { Name = "Laptop", Price = 15000m };
        var product2 = new Product { Name = "Mouse", Price = 150m };

        manager.Store(1, product1);
        manager.Store(2, product2);

        var retrieved = manager.Retrieve(1);
        Console.WriteLine($"✅ Retrieved: {retrieved?.Name} - {retrieved?.Price:C}");
    }

    static void DemonstrateUnmanagedConstraint()
    {
        var buffer = new UnmanagedBuffer<int>(100);
        Console.WriteLine("✅ Unmanaged buffer created (int is unmanaged)");

        // Çalışır: int, double, bool, struct (no references)
        // var buffer2 = new UnmanagedBuffer<string>(10);  // ❌ Error! string not unmanaged
    }
}
