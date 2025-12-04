# Generic Constraints Tutorial

## 📖 Overview

This sample demonstrates **generic constraints** in C# - one of the most powerful features for creating type-safe, reusable code. Learn how to restrict generic type parameters with `where` clauses to create flexible yet safe APIs.

## 🎯 Learning Objectives

After completing this tutorial, you will understand:

- ✅ **Class Constraint** (`where T : class`) - Reference type requirements
- ✅ **Struct Constraint** (`where T : struct`) - Value type requirements
- ✅ **New Constraint** (`where T : new()`) - Default constructor requirements
- ✅ **Base Class Constraint** (`where T : BaseClass`) - Inheritance requirements
- ✅ **Interface Constraint** (`where T : IInterface`) - Interface implementation requirements
- ✅ **Multiple Constraints** - Combining multiple restrictions
- ✅ **notnull Constraint** - Non-nullable reference type requirements (C# 8+)

## 🚀 Quick Start

```bash
# Run the sample
cd samples/02-Intermediate/GenericConstraints
dotnet run
```

## 📚 What You'll Learn

### 1. Class Constraint (`where T : class`)

Restricts `T` to **reference types only** (classes, interfaces, delegates, arrays).

```csharp
public class ReferenceTypeProcessor<T> where T : class
{
    public void Process(T? item)
    {
        if (item == null)
            Console.WriteLine("Null reference");
        else
            Console.WriteLine($"Processing {item.GetType().Name}");
    }
}

// Valid usage:
var processor1 = new ReferenceTypeProcessor<string>();
var processor2 = new ReferenceTypeProcessor<List<int>>();

// Invalid (compilation error):
// var processor3 = new ReferenceTypeProcessor<int>(); // int is value type
```

**Use Cases:**
- Repository patterns (entities are classes)
- Caching systems (cache reference types)
- Serialization frameworks
- When you need null checks

### 2. Struct Constraint (`where T : struct`)

Restricts `T` to **value types only** (structs, enums, numeric types).

```csharp
public class ValueTypeProcessor<T> where T : struct
{
    public T GetDefaultValue() => default; // Always non-null

    public void Display(T value)
    {
        Console.WriteLine($"Value: {value}, Type: {typeof(T).Name}");
    }
}

// Valid usage:
var processor1 = new ValueTypeProcessor<int>();
var processor2 = new ValueTypeProcessor<DateTime>();
var processor3 = new ValueTypeProcessor<Point>(); // Custom struct

// Invalid (compilation error):
// var processor4 = new ValueTypeProcessor<string>(); // string is reference type
```

**Use Cases:**
- Mathematical operations (int, double, decimal)
- High-performance collections (avoid boxing)
- Data structures for primitives
- When you need guaranteed non-null values

### 3. New Constraint (`where T : new()`)

Requires `T` to have a **public parameterless constructor**.

```csharp
public class Factory<T> where T : new()
{
    public T CreateInstance()
    {
        return new T(); // Can instantiate!
    }

    public List<T> CreateBatch(int count)
    {
        return Enumerable.Range(0, count)
            .Select(_ => new T())
            .ToList();
    }
}

// Valid usage:
public class Product { } // Has implicit parameterless constructor
var factory = new Factory<Product>();
var product = factory.CreateInstance();

// Invalid (compilation error):
public class Order
{
    public Order(int id) { } // No parameterless constructor
}
// var orderFactory = new Factory<Order>(); // ERROR
```

**Use Cases:**
- Factory patterns
- Object pooling
- Dependency injection containers
- Plugin systems

### 4. Base Class Constraint (`where T : BaseClass`)

Requires `T` to **inherit from a specific base class**.

```csharp
public abstract class Entity
{
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class Repository<T> where T : Entity
{
    private List<T> _items = new();

    public void Add(T item)
    {
        item.CreatedAt = DateTime.UtcNow; // Can access Entity members!
        _items.Add(item);
    }

    public T? GetById(int id)
    {
        return _items.FirstOrDefault(x => x.Id == id);
    }
}

// Valid usage:
public class Product : Entity
{
    public string Name { get; set; } = "";
}

var repo = new Repository<Product>();
repo.Add(new Product { Id = 1, Name = "Laptop" });
```

**Use Cases:**
- Repository patterns with base entity classes
- ORM frameworks
- Business logic layers
- When you need common functionality across types

### 5. Interface Constraint (`where T : IInterface`)

Requires `T` to **implement a specific interface**.

```csharp
public interface IComparable<T>
{
    int CompareTo(T? other);
}

public class Sorter<T> where T : IComparable<T>
{
    public List<T> Sort(List<T> items)
    {
        // Can call CompareTo because we know T implements IComparable<T>
        return items.OrderBy(x => x).ToList();
    }
}

// Valid usage:
public class Person : IComparable<Person>
{
    public string Name { get; set; } = "";
    public int Age { get; set; }

    public int CompareTo(Person? other)
    {
        return Age.CompareTo(other?.Age ?? 0);
    }
}

var sorter = new Sorter<Person>();
var sorted = sorter.Sort(people);
```

**Use Cases:**
- LINQ-like operations (IEnumerable<T>, IComparable<T>)
- Serialization (ISerializable)
- Cloning (ICloneable)
- Disposal (IDisposable)

### 6. Multiple Constraints

Combine multiple constraints for precise type requirements.

```csharp
public class AdvancedRepository<T>
    where T : Entity, IComparable<T>, new()
{
    public T CreateAndAdd()
    {
        var item = new T();              // new() constraint
        item.CreatedAt = DateTime.UtcNow; // Entity constraint
        return item;
    }

    public List<T> GetSortedItems()
    {
        return _items.OrderBy(x => x).ToList(); // IComparable<T> constraint
    }
}
```

**Constraint Order Rules:**
1. Base class constraint (if any) must be first
2. Interface constraints next
3. `new()` constraint must be last

```csharp
// Correct order:
where T : BaseClass, IInterface1, IInterface2, new()

// Incorrect (compilation error):
where T : new(), BaseClass // ERROR: new() must be last
```

### 7. notnull Constraint (C# 8+)

Requires `T` to be a **non-nullable type**.

```csharp
public class NullSafeProcessor<T> where T : notnull
{
    public void Process(T item)
    {
        // No null checks needed - T cannot be null
        Console.WriteLine(item.ToString());
    }
}

// Valid usage:
var processor1 = new NullSafeProcessor<int>(); // Value types are non-null
var processor2 = new NullSafeProcessor<string>(); // OK with nullable reference types enabled
```

## 🎨 Real-World Examples

### Example 1: Repository Pattern with Constraints

```csharp
public interface IEntity
{
    int Id { get; set; }
}

public class GenericRepository<T> where T : class, IEntity, new()
{
    private readonly List<T> _db = new();

    public void Add(T entity)
    {
        if (entity.Id == 0)
            entity.Id = _db.Count + 1;
        _db.Add(entity);
    }

    public T? GetById(int id) => _db.FirstOrDefault(x => x.Id == id);

    public T CreateNew() => new T();
}
```

### Example 2: Serialization with Interface Constraints

```csharp
public class JsonSerializer<T> where T : ISerializable
{
    public string Serialize(T obj)
    {
        // Can safely call ISerializable methods
        return obj.ToJson();
    }
}
```

### Example 3: Numeric Operations with Constraints

```csharp
public class Calculator<T> where T : struct, IComparable<T>, IConvertible
{
    public T Add(T a, T b)
    {
        dynamic da = a;
        dynamic db = b;
        return da + db;
    }

    public T Max(T a, T b) => a.CompareTo(b) > 0 ? a : b;
}
```

## 🔍 When to Use Each Constraint

| Constraint | Use When | Example Scenarios |
|------------|----------|-------------------|
| `class` | Need reference types, null checks | Caching, repositories |
| `struct` | Need value types, no null | Math operations, primitives |
| `new()` | Need to instantiate T | Factories, object pools |
| `BaseClass` | Share common functionality | Entity framework, ORM |
| `IInterface` | Enforce specific behavior | LINQ, serialization |
| `Multiple` | Precise type requirements | Advanced repositories |
| `notnull` | Prevent null bugs | Null-safe APIs |

## ⚡ Performance Considerations

### Constraint Benefits:
- ✅ **No Runtime Type Checks** - Constraints verified at compile-time
- ✅ **No Casting Overhead** - Type safety guaranteed
- ✅ **Better JIT Optimization** - More specific code generation
- ✅ **Avoid Boxing** - `struct` constraint prevents boxing

### Example: Boxing Avoidance

```csharp
// Without constraint - BOXING OCCURS
public class BadProcessor<T>
{
    public void Display(T value)
    {
        Console.WriteLine(value); // Boxing if T is int!
    }
}

// With constraint - NO BOXING
public class GoodProcessor<T> where T : struct
{
    public void Display(T value)
    {
        Console.WriteLine(value); // No boxing, JIT optimized
    }
}
```

## 📖 Key Takeaways

1. **Type Safety** - Constraints provide compile-time type safety
2. **Flexibility** - Reusable code that works with many types
3. **Performance** - No runtime type checks or casting overhead
4. **Intent** - Clearly express type requirements in API
5. **Constraints Compose** - Combine multiple constraints for precise types

## 🔗 Related Samples

- **[CovarianceContravariance](../CovarianceContravariance/)** - Generic variance with `in`/`out`
- **[BoxingPerformance](../BoxingPerformance/)** - Why `struct` constraint avoids boxing
- **[DesignPatterns](../../03-Advanced/DesignPatterns/)** - Factory and Repository patterns
- **[PerformanceOptimization](../../03-Advanced/PerformanceOptimization/)** - Generic optimization techniques

## 🎓 Further Learning

- [Microsoft Docs: Constraints on Type Parameters](https://learn.microsoft.com/en-us/dotnet/csharp/programming-guide/generics/constraints-on-type-parameters)
- [C# Language Specification: Generic Constraints](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/language-specification/)
- [Effective C#: Generic Constraints](https://www.oreilly.com/library/view/effective-c-50/9780134579290/)

## 💡 Pro Tips

1. **Start Broad** - Begin with no constraints, add only when needed
2. **Document Why** - Explain why each constraint is necessary
3. **Test Edge Cases** - Verify constraint behavior with unusual types
4. **Consider Interfaces** - Prefer interface constraints over base classes
5. **Use notnull** - Enable nullable reference types and use `notnull` constraint

---

**Ready to master generic constraints?** Run the sample and experiment with different constraint combinations!

```bash
dotnet run
```
