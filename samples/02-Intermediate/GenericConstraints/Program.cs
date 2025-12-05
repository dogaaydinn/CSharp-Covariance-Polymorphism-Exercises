// SCENARIO: Generic Repository Pattern with Constraints
// BAD PRACTICE: Generic without constraints - compilation errors, no type safety
// GOOD PRACTICE: Constraints (class, struct, new(), interface, base class, unmanaged)
// ADVANCED: Multiple constraints, default keyword, generic interface implementation

using GenericConstraints;

class Program
{
    static void Main()
    {
        Console.WriteLine("╔═══════════════════════════════════════════════════════════╗");
        Console.WriteLine("║   🔗  GENERIC CONSTRAINTS - REPOSITORY PATTERN        ║");
        Console.WriteLine("╚═══════════════════════════════════════════════════════════╝");
        Console.WriteLine();

        // 1. BAD PRACTICE
        Console.WriteLine("═══ 1. ❌ BAD PRACTICE: No Constraints ═══\n");
        DemonstrateBadPractice();

        Console.WriteLine("\n" + new string('─', 60) + "\n");

        // 2. Class + Interface + new() constraint
        Console.WriteLine("═══ 2. ✅ GOOD: Class + Interface + new() Constraint ═══\n");
        DemonstrateClassConstraint();

        Console.WriteLine("\n" + new string('─', 60) + "\n");

        // 3. Struct constraint
        Console.WriteLine("═══ 3. ✅ GOOD: Struct Constraint ═══\n");
        DemonstrateStructConstraint();

        Console.WriteLine("\n" + new string('─', 60) + "\n");

        // 4. IComparable constraint
        Console.WriteLine("═══ 4. ✅ GOOD: IComparable Constraint ═══\n");
        DemonstrateComparableConstraint();

        Console.WriteLine("\n" + new string('─', 60) + "\n");

        // 5. Multiple constraints
        Console.WriteLine("═══ 5. ✅ ADVANCED: Multiple Constraints ═══\n");
        DemonstrateMultipleConstraints();

        Console.WriteLine("\n" + new string('─', 60) + "\n");

        // 6. Base class constraint
        Console.WriteLine("═══ 6. ✅ ADVANCED: Base Class Constraint ═══\n");
        DemonstrateBaseClassConstraint();

        Console.WriteLine("\n" + new string('─', 60) + "\n");

        // 7. Unmanaged constraint
        Console.WriteLine("═══ 7. ✅ ADVANCED: Unmanaged Constraint ═══\n");
        DemonstrateUnmanagedConstraint();

        Console.WriteLine("\n" + new string('─', 60) + "\n");

        // 8. Default keyword usage
        Console.WriteLine("═══ 8. ✅ ADVANCED: Default Keyword Usage ═══\n");
        DemonstrateDefaultKeyword();

        Console.WriteLine("\n" + new string('─', 60) + "\n");

        // 9. Real-world Repository CRUD
        Console.WriteLine("═══ 9. ✅ REAL-WORLD: Repository CRUD Operations ═══\n");
        DemonstrateRepositoryCRUD();

        // Final Summary
        Console.WriteLine("\n╔═══════════════════════════════════════════════════════════╗");
        Console.WriteLine("║                    📊 ÖZET                                ║");
        Console.WriteLine("╚═══════════════════════════════════════════════════════════╝");
        Console.WriteLine();
        Console.WriteLine("✅ ÖĞRENİLENLER:");
        Console.WriteLine("   • where T : class - Reference type constraint");
        Console.WriteLine("   • where T : struct - Value type constraint");
        Console.WriteLine("   • where T : new() - Parameterless constructor constraint");
        Console.WriteLine("   • where T : IInterface - Interface implementation constraint");
        Console.WriteLine("   • where T : BaseClass - Base class constraint");
        Console.WriteLine("   • where T : unmanaged - Unmanaged type constraint (C# 7.3+)");
        Console.WriteLine("   • Multiple constraints on same type parameter");
        Console.WriteLine("   • Multiple type parameters with different constraints");
        Console.WriteLine("   • default keyword behavior with constraints");
        Console.WriteLine();
        Console.WriteLine("💡 BEST PRACTICES:");
        Console.WriteLine("   • Always use constraints to enforce compile-time safety");
        Console.WriteLine("   • Use class constraint for repositories (reference types)");
        Console.WriteLine("   • Use new() constraint when you need to create instances");
        Console.WriteLine("   • Use interface constraints to guarantee capabilities (IComparable)");
        Console.WriteLine("   • Use base class constraints for shared functionality");
        Console.WriteLine("   • Combine constraints for maximum type safety");
    }

    /// <summary>
    /// ❌ BAD PRACTICE: Generic without constraints
    /// Problem: Can't access properties, can't create instances
    /// </summary>
    static void DemonstrateBadPractice()
    {
        Console.WriteLine("💀 Bad Practice: Generic without constraints\n");

        var bad = new BadRepository<User>();
        var user = new User { Name = "Ali", Email = "ali@test.com" };
        bad.Add(user);

        Console.WriteLine("\n⚠️  SORUNLAR:");
        Console.WriteLine("   ❌ Can't access Id property → No guarantee T has Id");
        Console.WriteLine("   ❌ Can't call GetById() → Compile error!");
        Console.WriteLine("   ❌ Can't create new instances → No new() constraint");
        Console.WriteLine("   ❌ No type safety → T could be anything (int, string, etc.)");
        Console.WriteLine("   ❌ Runtime errors possible");

        Console.WriteLine("\n💡 SOLUTION: Use constraints!");
        Console.WriteLine("   ✅ where T : class, IEntity, new()");
    }

    /// <summary>
    /// ✅ GOOD: Class + Interface + new() constraints
    /// </summary>
    static void DemonstrateClassConstraint()
    {
        Console.WriteLine("🎯 Repository<T> where T : class, IEntity, new()\n");

        var userRepo = new Repository<User>();

        // ✅ Can create new instance with new() constraint
        var user1 = userRepo.Create();
        user1.Name = "Alice";
        user1.Email = "alice@example.com";
        userRepo.Add(user1);

        // Add more users
        var user2 = new User { Name = "Bob", Email = "bob@example.com" };
        userRepo.Add(user2);

        var user3 = new User { Name = "Charlie", Email = "charlie@example.com" };
        userRepo.Add(user3);

        Console.WriteLine($"\n✅ Total users: {userRepo.Count}");

        // ✅ Can call GetById() - IEntity constraint guarantees Id property
        Console.WriteLine("\n📋 Retrieving users:");
        var retrieved = userRepo.GetById(1);
        var notFound = userRepo.GetById(999);

        Console.WriteLine("\n💡 CLASS + INTERFACE + new() BENEFITS:");
        Console.WriteLine("   ✅ class: Ensures T is reference type (can be null)");
        Console.WriteLine("   ✅ IEntity: Guarantees Id property access");
        Console.WriteLine("   ✅ new(): Allows creating new instances with Create()");
        Console.WriteLine("   ✅ Compile-time safety → No runtime errors");
    }

    /// <summary>
    /// ✅ GOOD: Struct constraint for value types
    /// </summary>
    static void DemonstrateStructConstraint()
    {
        Console.WriteLine("🎯 ValueContainer<T> where T : struct\n");

        // Primitive types
        var intContainer = new ValueContainer<int> { Value = 42 };
        intContainer.Display();

        var doubleContainer = new ValueContainer<double> { Value = 3.14159 };
        doubleContainer.Display();

        // Custom struct
        var pointContainer = new ValueContainer<Point> { Value = new Point(10, 20) };
        pointContainer.Display();

        // Default value
        Console.WriteLine("\n🔍 Testing default values:");
        var emptyInt = new ValueContainer<int>();
        emptyInt.Display();

        int defaultValue = emptyInt.GetValueOrDefault(999);
        Console.WriteLine($"  📦 GetValueOrDefault(999): {defaultValue}");

        // Clear value
        intContainer.Clear();
        intContainer.Display();

        Console.WriteLine("\n💡 STRUCT CONSTRAINT BENEFITS:");
        Console.WriteLine("   ✅ Ensures T is value type (int, double, Point, etc.)");
        Console.WriteLine("   ✅ No null values → Value types can't be null");
        Console.WriteLine("   ✅ default keyword gives zero/false for value types");
        Console.WriteLine("   ✅ Stack allocation → Better performance");
        Console.WriteLine("   ✅ Use case: Nullable<T>, primitives, custom structs");

        Console.WriteLine("\n❌ Can't use reference types:");
        Console.WriteLine("   // var stringContainer = new ValueContainer<string>();  // ERROR!");
        Console.WriteLine("   // var userContainer = new ValueContainer<User>();      // ERROR!");
    }

    /// <summary>
    /// ✅ GOOD: IComparable constraint enables sorting/comparison
    /// </summary>
    static void DemonstrateComparableConstraint()
    {
        Console.WriteLine("🎯 ComparableRepository<T> where T : IComparable<T>\n");

        Console.WriteLine("📊 Integer comparison:");
        var intRepo = new ComparableRepository<int>();
        intRepo.Add(42);
        intRepo.Add(15);
        intRepo.Add(99);
        intRepo.Add(7);
        intRepo.Add(63);

        var min = intRepo.GetMin();
        var max = intRepo.GetMax();
        var sorted = intRepo.GetSorted();
        Console.WriteLine($"  🔢 Sorted: [{string.Join(", ", sorted)}]");

        Console.WriteLine("\n🌡️  Temperature comparison:");
        var tempRepo = new ComparableRepository<Temperature>();
        tempRepo.Add(new Temperature(25));
        tempRepo.Add(new Temperature(0));
        tempRepo.Add(new Temperature(100));
        tempRepo.Add(new Temperature(-10));
        tempRepo.Add(new Temperature(37));

        var minTemp = tempRepo.GetMin();
        var maxTemp = tempRepo.GetMax();
        var sortedTemp = tempRepo.GetSorted();
        Console.WriteLine($"  🌡️  Sorted: [{string.Join(", ", sortedTemp)}]");

        // Range query
        Console.WriteLine("\n🔍 Range query (0°C to 50°C):");
        var range = tempRepo.GetRange(new Temperature(0), new Temperature(50));
        foreach (var temp in range)
            Console.WriteLine($"     • {temp}");

        Console.WriteLine("\n💡 IComparable CONSTRAINT BENEFITS:");
        Console.WriteLine("   ✅ Enables Min/Max operations");
        Console.WriteLine("   ✅ Enables sorting (OrderBy)");
        Console.WriteLine("   ✅ Enables range queries");
        Console.WriteLine("   ✅ Type-safe comparisons");
        Console.WriteLine("   ✅ Works with: int, string, DateTime, custom types");
    }

    /// <summary>
    /// ✅ ADVANCED: Multiple type parameters with different constraints
    /// </summary>
    static void DemonstrateMultipleConstraints()
    {
        Console.WriteLine("🎯 Manager<TEntity, TKey>");
        Console.WriteLine("   where TEntity : class, IEntity, new()");
        Console.WriteLine("   where TKey : struct, IComparable<TKey>\n");

        var manager = new Manager<Product, int>();

        // Create and store with new() constraint
        var product1 = manager.CreateAndStore(1);
        product1.Id = 101;

        // Store existing entities
        var product2 = new Product
        {
            Id = 102,
            Name = "Laptop",
            Price = 15000m,
            Stock = 10
        };
        manager.Store(2, product2);

        var product3 = new Product
        {
            Id = 103,
            Name = "Mouse",
            Price = 150m,
            Stock = 50
        };
        manager.Store(3, product3);

        // Retrieve
        Console.WriteLine("\n📋 Retrieving products:");
        var retrieved = manager.Retrieve(2);
        if (retrieved != null)
            Console.WriteLine($"     {retrieved}");

        // Range query (using IComparable on TKey)
        Console.WriteLine("\n🔍 Range query (keys 1 to 3):");
        var rangeProducts = manager.GetRange(1, 3);
        foreach (var p in rangeProducts)
            Console.WriteLine($"     {p}");

        Console.WriteLine($"\n✅ Total stored: {manager.Count}");

        Console.WriteLine("\n💡 MULTIPLE CONSTRAINTS BENEFITS:");
        Console.WriteLine("   ✅ TEntity constraints:");
        Console.WriteLine("      • class: Reference type");
        Console.WriteLine("      • IEntity: Has Id property");
        Console.WriteLine("      • new(): Can create instances");
        Console.WriteLine("   ✅ TKey constraints:");
        Console.WriteLine("      • struct: Value type (int, Guid, DateTime)");
        Console.WriteLine("      • IComparable: Can compare keys for range queries");
        Console.WriteLine("   ✅ Type safety for both parameters independently");
    }

    /// <summary>
    /// ✅ ADVANCED: Base class constraint with audit tracking
    /// </summary>
    static void DemonstrateBaseClassConstraint()
    {
        Console.WriteLine("🎯 AuditRepository<T> where T : BaseEntity, new()\n");

        var orderRepo = new AuditRepository<Order>();

        // Add with automatic CreatedAt
        var order1 = new Order
        {
            Id = 1,
            UserId = 101,
            TotalAmount = 999.99m,
            Status = OrderStatus.Pending
        };
        orderRepo.Add(order1);

        Thread.Sleep(100);  // Small delay to show different timestamps

        var order2 = new Order
        {
            Id = 2,
            UserId = 102,
            TotalAmount = 1599.99m,
            Status = OrderStatus.Processing
        };
        orderRepo.Add(order2);

        // Update with automatic UpdatedAt
        Console.WriteLine("\n✏️  Updating order:");
        Thread.Sleep(100);
        order1.Status = OrderStatus.Shipped;
        orderRepo.Update(order1);

        // Display all with audit info
        Console.WriteLine("\n📋 All orders with audit info:");
        foreach (var order in orderRepo.GetAll())
        {
            Console.WriteLine($"     {order}");
            Console.WriteLine($"        Created: {order.CreatedAt:yyyy-MM-dd HH:mm:ss.fff}");
            if (order.UpdatedAt.HasValue)
                Console.WriteLine($"        Updated: {order.UpdatedAt:yyyy-MM-dd HH:mm:ss.fff}");
        }

        Console.WriteLine($"\n✅ Total orders: {orderRepo.Count}");

        Console.WriteLine("\n💡 BASE CLASS CONSTRAINT BENEFITS:");
        Console.WriteLine("   ✅ Access to BaseEntity properties (Id, CreatedAt, UpdatedAt)");
        Console.WriteLine("   ✅ Automatic audit tracking");
        Console.WriteLine("   ✅ Shared functionality across all entities");
        Console.WriteLine("   ✅ Inheritance hierarchy enforcement");
        Console.WriteLine("   ✅ Use case: Audit logs, soft delete, versioning");
    }

    /// <summary>
    /// ✅ ADVANCED: Unmanaged constraint for high-performance scenarios
    /// </summary>
    static void DemonstrateUnmanagedConstraint()
    {
        Console.WriteLine("🎯 UnmanagedBuffer<T> where T : unmanaged\n");

        Console.WriteLine("📦 Creating int buffer:");
        var intBuffer = new UnmanagedBuffer<int>(10);
        intBuffer.Fill(42);
        Console.WriteLine($"  Buffer[0] = {intBuffer[0]}");
        Console.WriteLine($"  Buffer[5] = {intBuffer[5]}");

        Console.WriteLine("\n📦 Creating double buffer:");
        var doubleBuffer = new UnmanagedBuffer<double>(5);
        doubleBuffer[0] = 3.14;
        doubleBuffer[1] = 2.71;
        Console.WriteLine($"  Buffer[0] = {doubleBuffer[0]}");
        Console.WriteLine($"  Buffer[1] = {doubleBuffer[1]}");

        Console.WriteLine("\n📦 Creating Point buffer:");
        var pointBuffer = new UnmanagedBuffer<Point>(3);
        pointBuffer[0] = new Point(10, 20);
        pointBuffer[1] = new Point(30, 40);
        Console.WriteLine($"  Buffer[0] = {pointBuffer[0]}");
        Console.WriteLine($"  Buffer[1] = {pointBuffer[1]}");

        // Unsafe pointer access
        unsafe
        {
            void* ptr = intBuffer.GetPointer();
            Console.WriteLine($"\n🔗 Pointer address: 0x{((IntPtr)ptr).ToString("X")}");
        }

        Console.WriteLine("\n💡 UNMANAGED CONSTRAINT BENEFITS:");
        Console.WriteLine("   ✅ Allows unsafe code and pointer operations");
        Console.WriteLine("   ✅ No GC overhead → Better performance");
        Console.WriteLine("   ✅ P/Invoke compatibility");
        Console.WriteLine("   ✅ Allowed types: int, double, bool, Point (no references)");
        Console.WriteLine("   ✅ Use case: Performance-critical code, interop, graphics");

        Console.WriteLine("\n❌ Can't use reference types or types with references:");
        Console.WriteLine("   // var stringBuffer = new UnmanagedBuffer<string>(10);  // ERROR!");
        Console.WriteLine("   // var userBuffer = new UnmanagedBuffer<User>(10);      // ERROR!");
    }

    /// <summary>
    /// ✅ ADVANCED: Default keyword behavior with different constraints
    /// </summary>
    static void DemonstrateDefaultKeyword()
    {
        Console.WriteLine("🎯 Default keyword with different types\n");

        Console.WriteLine("🔍 Reference types (class):");
        var defaultDemo1 = new DefaultValueDemonstrator<User>();
        var defaultUser = defaultDemo1.GetDefaultValue();  // null
        defaultDemo1.IsDefault(defaultUser!);
        defaultDemo1.IsDefault(new User { Name = "Test" });

        Console.WriteLine("\n🔍 Value types (struct):");
        var defaultDemo2 = new DefaultValueDemonstrator<int>();
        var defaultInt = defaultDemo2.GetDefaultValue();  // 0
        defaultDemo2.IsDefault(defaultInt);
        defaultDemo2.IsDefault(42);

        Console.WriteLine("\n🔍 Custom struct:");
        var defaultDemo3 = new DefaultValueDemonstrator<Point>();
        var defaultPoint = defaultDemo3.GetDefaultValue();  // (0, 0)
        defaultDemo3.IsDefault(defaultPoint);
        defaultDemo3.IsDefault(new Point(10, 20));

        Console.WriteLine("\n🔍 Nullable value type:");
        var defaultDemo4 = new DefaultValueDemonstrator<int?>();
        var defaultNullable = defaultDemo4.GetDefaultValue();  // null
        defaultDemo4.IsDefault(defaultNullable);
        defaultDemo4.IsDefault(0);

        Console.WriteLine("\n💡 DEFAULT KEYWORD BEHAVIOR:");
        Console.WriteLine("   ✅ Reference types (class) → default = null");
        Console.WriteLine("   ✅ Value types (struct) → default = 0, false, (0,0), etc.");
        Console.WriteLine("   ✅ Nullable<T> → default = null");
        Console.WriteLine("   ✅ Generic T unconstrained → default depends on actual type");
        Console.WriteLine("   ✅ Use EqualityComparer<T>.Default for null-safe comparison");
    }

    /// <summary>
    /// ✅ REAL-WORLD: Complete CRUD operations with Repository<T>
    /// </summary>
    static void DemonstrateRepositoryCRUD()
    {
        Console.WriteLine("🎯 Real-world Repository CRUD Operations\n");

        var productRepo = new Repository<Product>();

        // CREATE
        Console.WriteLine("📝 CREATE operations:");
        var laptop = productRepo.Create();
        laptop.Name = "MacBook Pro";
        laptop.Price = 35000m;
        laptop.Stock = 5;
        productRepo.Add(laptop);

        var mouse = new Product
        {
            Name = "Logitech MX Master",
            Price = 1200m,
            Stock = 20
        };
        productRepo.Add(mouse);

        var keyboard = new Product
        {
            Name = "Keychron K8",
            Price = 1800m,
            Stock = 15
        };
        productRepo.Add(keyboard);

        // READ
        Console.WriteLine("\n📖 READ operations:");
        var product1 = productRepo.GetById(1);
        var product999 = productRepo.GetById(999);

        Console.WriteLine("\n  📋 All products:");
        foreach (var p in productRepo.GetAll())
            Console.WriteLine($"     {p}");

        // UPDATE
        Console.WriteLine("\n✏️  UPDATE operations:");
        if (product1 != null)
        {
            product1.Price = 34000m;  // Price drop!
            product1.Stock = 3;       // Stock decreased
            productRepo.Update(product1);
        }

        // DELETE
        Console.WriteLine("\n🗑️  DELETE operations:");
        productRepo.Delete(2);  // Delete mouse

        // Final state
        Console.WriteLine("\n📊 Final repository state:");
        Console.WriteLine($"  Total products: {productRepo.Count}");
        Console.WriteLine("\n  📋 Remaining products:");
        foreach (var p in productRepo.GetAll())
            Console.WriteLine($"     {p}");

        Console.WriteLine("\n💡 REPOSITORY PATTERN BENEFITS:");
        Console.WriteLine("   ✅ Generic CRUD operations for all entity types");
        Console.WriteLine("   ✅ Type safety with constraints");
        Console.WriteLine("   ✅ Automatic ID generation");
        Console.WriteLine("   ✅ Clean separation of concerns");
        Console.WriteLine("   ✅ Testable and maintainable");
        Console.WriteLine("   ✅ Production-ready pattern for data access layer");
    }
}
