using System;
using System.Collections.Generic;

namespace GenericConstraints;

/// <summary>
/// Generic Constraints Tutorial
/// Deep dive into where T : constraints
/// </summary>
class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("=".PadRight(70, '='));
        Console.WriteLine("GENERIC CONSTRAINTS TUTORIAL");
        Console.WriteLine("=".PadRight(70, '='));
        Console.WriteLine();

        RunClassConstraintExample();
        RunStructConstraintExample();
        RunNewConstraintExample();
        RunBaseClassConstraintExample();
        RunInterfaceConstraintExample();
        RunMultipleConstraintsExample();
        RunRealWorldExample();

        Console.WriteLine();
        Console.WriteLine("=".PadRight(70, '='));
        Console.WriteLine("Tutorial Complete!");
        Console.WriteLine("=".PadRight(70, '='));
    }

    #region 1. Class Constraint (where T : class)

    static void RunClassConstraintExample()
    {
        Console.WriteLine();
        Console.WriteLine("1. CLASS CONSTRAINT - where T : class");
        Console.WriteLine("   Restricts T to reference types only");
        Console.WriteLine("-".PadRight(70, '-'));

        Console.WriteLine();
        Console.WriteLine("✅ Valid usage:");
        var processor1 = new ReferenceTypeProcessor<string>();
        processor1.Process("Hello");

        var processor2 = new ReferenceTypeProcessor<List<int>>();
        processor2.Process(new List<int> { 1, 2, 3 });

        Console.WriteLine();
        Console.WriteLine("❌ Invalid usage (compile error):");
        Console.WriteLine("  var processor = new ReferenceTypeProcessor<int>();");
        Console.WriteLine("  // Error: 'int' must be reference type");

        Console.WriteLine();
        Console.WriteLine("BENEFITS:");
        Console.WriteLine("  - Can use null");
        Console.WriteLine("  - Can use 'as' operator");
        Console.WriteLine("  - Enables nullable reference types (T?)");
    }

    #endregion

    #region 2. Struct Constraint (where T : struct)

    static void RunStructConstraintExample()
    {
        Console.WriteLine();
        Console.WriteLine("2. STRUCT CONSTRAINT - where T : struct");
        Console.WriteLine("   Restricts T to value types only");
        Console.WriteLine("-".PadRight(70, '-'));

        Console.WriteLine();
        Console.WriteLine("✅ Valid usage:");
        var processor1 = new ValueTypeProcessor<int>();
        processor1.Process(42);

        var processor2 = new ValueTypeProcessor<DateTime>();
        processor2.Process(DateTime.Now);

        var processor3 = new ValueTypeProcessor<Point>();
        processor3.Process(new Point(10, 20));

        Console.WriteLine();
        Console.WriteLine("❌ Invalid usage (compile error):");
        Console.WriteLine("  var processor = new ValueTypeProcessor<string>();");
        Console.WriteLine("  // Error: 'string' must be value type");

        Console.WriteLine();
        Console.WriteLine("BENEFITS:");
        Console.WriteLine("  - No boxing/unboxing");
        Console.WriteLine("  - Performance optimization");
        Console.WriteLine("  - Can use Nullable<T>");
    }

    #endregion

    #region 3. Constructor Constraint (where T : new())

    static void RunNewConstraintExample()
    {
        Console.WriteLine();
        Console.WriteLine("3. CONSTRUCTOR CONSTRAINT - where T : new()");
        Console.WriteLine("   T must have public parameterless constructor");
        Console.WriteLine("-".PadRight(70, '-'));

        Console.WriteLine();
        Console.WriteLine("✅ Factory pattern with new() constraint:");
        var factory = new Factory<Product>();
        Product product1 = factory.Create();
        Console.WriteLine($"  Created: {product1.Name}");

        Product product2 = factory.Create();
        Console.WriteLine($"  Created: {product2.Name}");

        Console.WriteLine();
        Console.WriteLine("❌ Invalid usage:");
        Console.WriteLine("  var factory = new Factory<NoDefaultConstructor>();");
        Console.WriteLine("  // Error: No parameterless constructor");

        Console.WriteLine();
        Console.WriteLine("USAGE:");
        Console.WriteLine("  - Factory patterns");
        Console.WriteLine("  - Object pooling");
        Console.WriteLine("  - Dependency injection");
    }

    #endregion

    #region 4. Base Class Constraint (where T : BaseClass)

    static void RunBaseClassConstraintExample()
    {
        Console.WriteLine();
        Console.WriteLine("4. BASE CLASS CONSTRAINT - where T : BaseClass");
        Console.WriteLine("   T must inherit from specified base class");
        Console.WriteLine("-".PadRight(70, '-'));

        Console.WriteLine();
        Console.WriteLine("✅ Repository pattern with base class constraint:");
        var dogRepo = new Repository<Dog>();
        Dog dog = new Dog { Id = 1, Name = "Buddy", Breed = "Golden Retriever" };
        dogRepo.Add(dog);

        var catRepo = new Repository<Cat>();
        Cat cat = new Cat { Id = 2, Name = "Whiskers", Color = "Orange" };
        catRepo.Add(cat);

        var allDogs = dogRepo.GetAll();
        Console.WriteLine($"  Dogs in repository: {allDogs.Count}");

        Console.WriteLine();
        Console.WriteLine("❌ Invalid usage:");
        Console.WriteLine("  var repo = new Repository<string>();");
        Console.WriteLine("  // Error: 'string' does not inherit from Entity");

        Console.WriteLine();
        Console.WriteLine("BENEFITS:");
        Console.WriteLine("  - Access base class members");
        Console.WriteLine("  - Polymorphic behavior");
        Console.WriteLine("  - Type safety");
    }

    #endregion

    #region 5. Interface Constraint (where T : IInterface)

    static void RunInterfaceConstraintExample()
    {
        Console.WriteLine();
        Console.WriteLine("5. INTERFACE CONSTRAINT - where T : IInterface");
        Console.WriteLine("   T must implement specified interface");
        Console.WriteLine("-".PadRight(70, '-'));

        Console.WriteLine();
        Console.WriteLine("✅ Generic sorting with IComparable:");
        var sorter = new Sorter<int>();
        int[] numbers = { 5, 2, 8, 1, 9, 3 };
        var sorted = sorter.Sort(numbers);
        Console.WriteLine($"  Sorted numbers: {string.Join(", ", sorted)}");

        Console.WriteLine();
        Console.WriteLine("✅ Custom type with IComparable:");
        var personSorter = new Sorter<Person>();
        Person[] people = {
            new Person { Name = "Alice", Age = 30 },
            new Person { Name = "Bob", Age = 25 },
            new Person { Name = "Charlie", Age = 35 }
        };
        var sortedPeople = personSorter.Sort(people);
        foreach (var p in sortedPeople)
        {
            Console.WriteLine($"  {p.Name}, Age {p.Age}");
        }

        Console.WriteLine();
        Console.WriteLine("BENEFITS:");
        Console.WriteLine("  - Guaranteed interface implementation");
        Console.WriteLine("  - Can call interface methods");
        Console.WriteLine("  - Compile-time safety");
    }

    #endregion

    #region 6. Multiple Constraints

    static void RunMultipleConstraintsExample()
    {
        Console.WriteLine();
        Console.WriteLine("6. MULTIPLE CONSTRAINTS");
        Console.WriteLine("   Combine multiple constraints with ,");
        Console.WriteLine("-".PadRight(70, '-'));

        Console.WriteLine();
        Console.WriteLine("✅ Multiple constraints example:");
        Console.WriteLine("  where T : class, IDisposable, new()");

        var manager = new ResourceManager<DatabaseConnection>();
        using (var resource = manager.AcquireResource())
        {
            resource?.Use();
        }

        Console.WriteLine();
        Console.WriteLine("CONSTRAINT ORDERING:");
        Console.WriteLine("  1. class or struct (if specified)");
        Console.WriteLine("  2. Base class (if specified)");
        Console.WriteLine("  3. Interfaces (comma-separated)");
        Console.WriteLine("  4. new() (must be last)");

        Console.WriteLine();
        Console.WriteLine("EXAMPLES:");
        Console.WriteLine("  where T : class, IComparable, new()");
        Console.WriteLine("  where T : Entity, IValidatable, new()");
        Console.WriteLine("  where T : struct, IFormattable");
    }

    #endregion

    #region 7. Real-World Example

    static void RunRealWorldExample()
    {
        Console.WriteLine();
        Console.WriteLine("7. REAL-WORLD EXAMPLE - Generic Repository");
        Console.WriteLine("   Practical usage of multiple constraints");
        Console.WriteLine("-".PadRight(70, '-'));

        Console.WriteLine();
        Console.WriteLine("✅ Generic repository with constraints:");

        var productRepo = new GenericRepository<RealWorldProduct>();
        var product = new RealWorldProduct { Id = 1, Name = "Laptop", Price = 999.99m };
        productRepo.Insert(product);

        var found = productRepo.GetById(1);
        Console.WriteLine($"  Found: {found?.Name} (${found?.Price})");

        var allProducts = productRepo.GetAll();
        Console.WriteLine($"  Total products: {allProducts.Count}");

        product.Price = 899.99m;
        productRepo.Update(product);
        Console.WriteLine($"  Updated price: ${product.Price}");

        Console.WriteLine();
        Console.WriteLine("CONSTRAINT BENEFITS:");
        Console.WriteLine("  ✓ Type safety at compile time");
        Console.WriteLine("  ✓ No boxing/unboxing");
        Console.WriteLine("  ✓ IntelliSense support");
        Console.WriteLine("  ✓ Reusable code");
    }

    #endregion
}

#region Constraint Examples - Class

class ReferenceTypeProcessor<T> where T : class
{
    public void Process(T item)
    {
        if (item == null)  // Can check for null (reference type)
        {
            Console.WriteLine("  Item is null");
            return;
        }

        Console.WriteLine($"  Processing reference type: {item.GetType().Name}");
    }
}

#endregion

#region Constraint Examples - Struct

struct Point
{
    public int X { get; set; }
    public int Y { get; set; }

    public Point(int x, int y)
    {
        X = x;
        Y = y;
    }
}

class ValueTypeProcessor<T> where T : struct
{
    public void Process(T item)
    {
        Console.WriteLine($"  Processing value type: {typeof(T).Name} = {item}");
    }
}

#endregion

#region Constraint Examples - new()

class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = "New Product";
    public decimal Price { get; set; }
}

class Factory<T> where T : new()
{
    public T Create()
    {
        return new T();  // Can call parameterless constructor
    }
}

#endregion

#region Constraint Examples - Base Class

abstract class Entity
{
    public int Id { get; set; }
}

class Dog : Entity
{
    public string Name { get; set; } = "";
    public string Breed { get; set; } = "";
}

class Cat : Entity
{
    public string Name { get; set; } = "";
    public string Color { get; set; } = "";
}

class Repository<T> where T : Entity
{
    private readonly List<T> _items = new();

    public void Add(T item)
    {
        Console.WriteLine($"  Adding {typeof(T).Name} with ID {item.Id}");
        _items.Add(item);
    }

    public List<T> GetAll() => _items;
}

#endregion

#region Constraint Examples - Interface

class Person : IComparable<Person>
{
    public string Name { get; set; } = "";
    public int Age { get; set; }

    public int CompareTo(Person? other)
    {
        return Age.CompareTo(other?.Age ?? 0);
    }
}

class Sorter<T> where T : IComparable<T>
{
    public T[] Sort(T[] items)
    {
        var sorted = (T[])items.Clone();
        Array.Sort(sorted);
        return sorted;
    }
}

#endregion

#region Constraint Examples - Multiple

class DatabaseConnection : IDisposable
{
    public DatabaseConnection()
    {
        Console.WriteLine("  DatabaseConnection created");
    }

    public void Use()
    {
        Console.WriteLine("  Using database connection...");
    }

    public void Dispose()
    {
        Console.WriteLine("  DatabaseConnection disposed");
    }
}

class ResourceManager<T> where T : class, IDisposable, new()
{
    public T? AcquireResource()
    {
        return new T();  // Can create instance (new constraint)
    }
}

#endregion

#region Real-World Example

interface IEntity
{
    int Id { get; set; }
}

class GenericRepository<T> where T : class, IEntity, new()
{
    private readonly Dictionary<int, T> _storage = new();

    public void Insert(T entity)
    {
        Console.WriteLine($"  Inserting {typeof(T).Name} with ID {entity.Id}");
        _storage[entity.Id] = entity;
    }

    public T? GetById(int id)
    {
        return _storage.TryGetValue(id, out var entity) ? entity : null;
    }

    public List<T> GetAll()
    {
        return _storage.Values.ToList();
    }

    public void Update(T entity)
    {
        if (_storage.ContainsKey(entity.Id))
        {
            _storage[entity.Id] = entity;
            Console.WriteLine($"  Updated {typeof(T).Name} with ID {entity.Id}");
        }
    }

    public void Delete(int id)
    {
        _storage.Remove(id);
        Console.WriteLine($"  Deleted {typeof(T).Name} with ID {id}");
    }
}

class RealWorldProduct : IEntity
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public decimal Price { get; set; }
}

#endregion
