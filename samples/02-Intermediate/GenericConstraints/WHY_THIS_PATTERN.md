# 🔗 Why Generic Constraints? - Deep Dive

## 📚 Table of Contents

1. [What are Generic Constraints?](#what-are-generic-constraints)
2. [Why Constraints Over No Constraints?](#why-constraints-over-no-constraints)
3. [All 9 Constraint Types Explained](#all-9-constraint-types-explained)
4. [When to Use Each Constraint Type](#when-to-use-each-constraint-type)
5. [Real-World Scenarios](#real-world-scenarios)
6. [Performance Considerations](#performance-considerations)
7. [Common Mistakes](#common-mistakes)
8. [Best Practices](#best-practices)
9. [Constraint Comparison Table](#constraint-comparison-table)

---

## What are Generic Constraints?

**Generic constraints** are compile-time rules that restrict what types can be used as type arguments in generic classes, methods, and interfaces. They provide type safety while maintaining the flexibility of generics.

### The Problem Without Constraints

```csharp
// ❌ NO CONSTRAINT - Unrestricted generic type
public class Repository<T>
{
    private List<T> _items = new();

    public void Add(T item)
    {
        // ❌ Can't access ANY properties or methods
        // item.Id = 1;              // ERROR: T doesn't have Id
        // item.Save();              // ERROR: T doesn't have Save()
        // var copy = new T();       // ERROR: T might not have constructor

        _items.Add(item);  // ✅ Only this works - no guarantees about T
    }

    public T? GetById(int id)
    {
        // ❌ Can't filter by Id - T might not have Id property
        // return _items.FirstOrDefault(x => x.Id == id);  // ERROR!

        return default;  // Only option - return default value
    }
}
```

**Problems**:
- No compile-time safety
- Can't access properties/methods
- Can't create instances
- Can't perform comparisons
- IDE has no IntelliSense support
- API users don't know what types are valid

### The Solution With Constraints

```csharp
// ✅ WITH CONSTRAINTS - Type-safe generic
public class Repository<T> where T : class, IEntity, new()
{
    private List<T> _items = new();
    private int _nextId = 1;

    public void Add(T item)
    {
        // ✅ Can access IEntity.Id property
        if (item.Id == 0)
            item.Id = _nextId++;

        _items.Add(item);
        Console.WriteLine($"Added {typeof(T).Name} with ID {item.Id}");
    }

    public T? GetById(int id)
    {
        // ✅ Can filter using IEntity.Id
        return _items.FirstOrDefault(x => x.Id == id);
    }

    public T Create()
    {
        // ✅ Can create new instance with new() constraint
        var item = new T();
        return item;
    }
}

// Usage - Compiler enforces constraints
var userRepo = new Repository<User>();     // ✅ User implements IEntity and has new()
var productRepo = new Repository<Product>(); // ✅ Product implements IEntity and has new()
// var stringRepo = new Repository<string>(); // ❌ ERROR: string doesn't implement IEntity
```

**Benefits**:
- ✅ Compile-time type safety
- ✅ IntelliSense support
- ✅ Can access constrained members
- ✅ Can create instances
- ✅ Self-documenting API
- ✅ Early error detection

---

## Why Constraints Over No Constraints?

### 1. Compile-Time Safety vs Runtime Errors

#### ❌ Without Constraints (Runtime Disaster)
```csharp
public class BadCache<T>
{
    private Dictionary<string, T> _cache = new();

    public string GetKey(T item)
    {
        // ❌ What if T doesn't have ToString()?
        // ❌ What if T is null?
        // ❌ What if ToString() returns null?
        return item.ToString()!;  // 💣 NullReferenceException at runtime!
    }

    public T CreateDefault()
    {
        // ❌ Can't create instance
        // return new T();  // ERROR: T doesn't have new() constraint

        return default!;  // 💣 Returns null for reference types!
    }
}

// Usage - Compiles but crashes at runtime
var cache = new BadCache<string>();
cache.GetKey(null);  // 💥 CRASH! NullReferenceException
```

#### ✅ With Constraints (Compile-Time Safety)
```csharp
public class GoodCache<T> where T : class, IIdentifiable, new()
{
    private Dictionary<string, T> _cache = new();

    public string GetKey(T item)
    {
        // ✅ IIdentifiable guarantees Id property exists
        return $"{typeof(T).Name}_{item.Id}";
    }

    public T CreateDefault()
    {
        // ✅ new() constraint guarantees we can create instance
        return new T();
    }
}

// Usage - Compiler prevents invalid types
var cache = new GoodCache<User>();     // ✅ Compiles
// var badCache = new GoodCache<string>(); // ❌ Compiler error - caught early!
```

### 2. IntelliSense and Developer Experience

#### ❌ Without Constraints
```csharp
public void ProcessItem<T>(T item)
{
    // No IntelliSense - developer has to guess what properties exist
    // item.  <-- No suggestions!
}
```

#### ✅ With Constraints
```csharp
public void ProcessItem<T>(T item) where T : IEntity, IValidatable
{
    // ✅ IntelliSense shows: Id, Validate(), and all other IEntity/IValidatable members
    item.Id = 1;
    item.Validate();  // IntelliSense guides the developer
}
```

### 3. Self-Documenting APIs

#### ❌ Without Constraints - Unclear API
```csharp
// What types can I pass? No idea from signature alone!
public class DataService<T>
{
    public void Save(T item) { }
}
```

#### ✅ With Constraints - Clear API
```csharp
// Crystal clear: T must be a class, implement IEntity, and have parameterless constructor
public class DataService<T> where T : class, IEntity, new()
{
    public void Save(T item) { }
}

// Developers immediately know valid types without reading docs
```

---

## All 9 Constraint Types Explained

### 1. `where T : class` - Reference Type Constraint

**What it means**: T must be a reference type (class, interface, delegate, array)

**Guarantees**:
- T is nullable (can be null)
- T is stored on the heap
- T supports reference equality

**Use when**: Working with object references, need null checks, implementing caching

```csharp
public class ReferenceCache<T> where T : class
{
    private T? _cachedValue;  // ✅ Can be null

    public void Cache(T value)
    {
        _cachedValue = value;  // ✅ Can assign reference
    }

    public bool IsCached(T value)
    {
        // ✅ Can use reference equality
        return ReferenceEquals(_cachedValue, value);
    }

    public T? GetCached()
    {
        return _cachedValue;  // ✅ Can return null
    }
}

// Valid types
var stringCache = new ReferenceCache<string>();    // ✅ string is class
var userCache = new ReferenceCache<User>();        // ✅ User is class
var listCache = new ReferenceCache<List<int>>();   // ✅ List<T> is class

// Invalid types
// var intCache = new ReferenceCache<int>();       // ❌ int is struct
// var dateCache = new ReferenceCache<DateTime>(); // ❌ DateTime is struct
```

**Real-world example**: Entity Framework DbSet<T>
```csharp
public class DbSet<TEntity> where TEntity : class
{
    // EF Core requires entities to be reference types
}
```

### 2. `where T : struct` - Value Type Constraint

**What it means**: T must be a value type (primitive, enum, custom struct)

**Guarantees**:
- T is never null (non-nullable by default)
- T is stored on the stack (if local variable)
- T has value semantics (copy on assignment)

**Use when**: Working with primitives, ensuring non-null values, performance-critical code

```csharp
public class ValueContainer<T> where T : struct
{
    private T _value;

    public T Value
    {
        get => _value;
        set => _value = value;  // ✅ Never null
    }

    public bool IsDefault()
    {
        // ✅ Can safely compare with default
        return EqualityComparer<T>.Default.Equals(_value, default);
    }

    public void Reset()
    {
        _value = default;  // ✅ Safe - sets to 0, false, (0,0), etc.
    }
}

// Valid types
var intContainer = new ValueContainer<int>();           // ✅ int is struct
var dateContainer = new ValueContainer<DateTime>();     // ✅ DateTime is struct
var pointContainer = new ValueContainer<Point>();       // ✅ custom struct

// Invalid types
// var stringContainer = new ValueContainer<string>();  // ❌ string is class
// var userContainer = new ValueContainer<User>();      // ❌ User is class
```

**Real-world example**: Nullable<T>
```csharp
public struct Nullable<T> where T : struct
{
    // Framework's implementation of nullable value types
}

int? nullableInt = null;  // int? is Nullable<int>
```

### 3. `where T : new()` - Parameterless Constructor Constraint

**What it means**: T must have a public parameterless constructor

**Guarantees**:
- Can create instances with `new T()`
- T can be initialized without parameters

**Use when**: Factory patterns, repository creation, dependency injection

```csharp
public class Factory<T> where T : new()
{
    public T Create()
    {
        // ✅ Can create new instance
        var instance = new T();
        Console.WriteLine($"Created new {typeof(T).Name}");
        return instance;
    }

    public List<T> CreateMany(int count)
    {
        var items = new List<T>();
        for (int i = 0; i < count; i++)
        {
            items.Add(new T());  // ✅ Repeatedly create instances
        }
        return items;
    }
}

// Valid types
public class User
{
    public User() { }  // ✅ Parameterless constructor
}

public class Product
{
    public Product() { }  // ✅ Parameterless constructor
    public Product(string name) { }  // Additional constructors OK
}

var userFactory = new Factory<User>();     // ✅ Works
var productFactory = new Factory<Product>(); // ✅ Works

// Invalid types
public class Order
{
    public Order(int id) { }  // ❌ NO parameterless constructor
}

// var orderFactory = new Factory<Order>(); // ❌ Compiler error
```

**Important**: `new()` constraint must be LAST when combined with other constraints

```csharp
// ✅ CORRECT ORDER
public class Repository<T> where T : class, IEntity, new() { }

// ❌ WRONG ORDER
// public class Repository<T> where T : new(), class, IEntity { } // ERROR!
```

**Real-world example**: ASP.NET Core Activator
```csharp
public class Activator
{
    public static T CreateInstance<T>() where T : new()
    {
        return new T();
    }
}
```

### 4. `where T : IInterface` - Interface Constraint

**What it means**: T must implement the specified interface

**Guarantees**:
- T has all interface members
- Can call interface methods/properties on T

**Use when**: Need specific behavior, polymorphism, sorting, comparison

```csharp
public interface IEntity
{
    int Id { get; set; }
    DateTime CreatedAt { get; set; }
}

public class EntityRepository<T> where T : IEntity
{
    private List<T> _items = new();

    public void Add(T entity)
    {
        // ✅ Can access IEntity.Id and CreatedAt
        if (entity.Id == 0)
            entity.Id = GenerateId();

        entity.CreatedAt = DateTime.Now;
        _items.Add(entity);
    }

    public T? GetById(int id)
    {
        // ✅ Can filter using IEntity.Id
        return _items.FirstOrDefault(x => x.Id == id);
    }
}

// Valid types
public class User : IEntity
{
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public string Name { get; set; }
}

var repo = new EntityRepository<User>();  // ✅ User implements IEntity
```

**Multiple interfaces allowed**:
```csharp
public class ValidationRepository<T>
    where T : IEntity, IValidatable, IComparable<T>
{
    public void AddIfValid(T entity)
    {
        // ✅ Can use all interface members
        if (entity.Validate())  // IValidatable
        {
            entity.Id = 1;      // IEntity
            var comparison = entity.CompareTo(default!);  // IComparable<T>
        }
    }
}
```

**Real-world example**: LINQ OrderBy
```csharp
public static IOrderedEnumerable<T> OrderBy<T, TKey>(
    this IEnumerable<T> source,
    Func<T, TKey> keySelector)
    where TKey : IComparable<TKey>  // ✅ Ensures keys can be compared
{
    // Implementation uses TKey.CompareTo()
}
```

### 5. `where T : BaseClass` - Base Class Constraint

**What it means**: T must inherit from the specified base class

**Guarantees**:
- T has all base class members
- Can access protected members
- Can call base class methods

**Use when**: Need common functionality, audit tracking, polymorphic behavior

```csharp
public abstract class BaseEntity
{
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    protected virtual void OnBeforeSave() { }
}

public class AuditRepository<T> where T : BaseEntity, new()
{
    private List<T> _items = new();

    public void Add(T entity)
    {
        // ✅ Can access BaseEntity properties
        entity.CreatedAt = DateTime.Now;
        entity.UpdatedAt = null;

        // ✅ Can call protected methods
        entity.OnBeforeSave();

        _items.Add(entity);
    }

    public void Update(T entity)
    {
        // ✅ Automatic audit tracking
        entity.UpdatedAt = DateTime.Now;

        var existing = _items.FirstOrDefault(x => x.Id == entity.Id);
        if (existing != null)
        {
            int index = _items.IndexOf(existing);
            _items[index] = entity;
        }
    }
}

// Valid types
public class User : BaseEntity
{
    public string Name { get; set; }
}

public class Product : BaseEntity
{
    public decimal Price { get; set; }
}

var userRepo = new AuditRepository<User>();     // ✅ User inherits BaseEntity
var productRepo = new AuditRepository<Product>(); // ✅ Product inherits BaseEntity

// Invalid types
public class Order  // ❌ Doesn't inherit BaseEntity
{
    public int Id { get; set; }
}

// var orderRepo = new AuditRepository<Order>(); // ❌ Compiler error
```

**Real-world example**: ASP.NET Core Controller
```csharp
public class ApiController<TEntity> : ControllerBase
    where TEntity : BaseEntity, new()
{
    // All entities have Id, CreatedAt, UpdatedAt
}
```

### 6. `where T : unmanaged` - Unmanaged Constraint (C# 7.3+)

**What it means**: T must be an unmanaged type (no references to managed heap)

**Unmanaged types**:
- Primitives: int, float, double, bool, byte, etc.
- Enums
- Pointers
- Structs containing only unmanaged types

**Guarantees**:
- Can use unsafe code
- Can get pointer to T
- T has fixed size in memory
- No garbage collection overhead

**Use when**: High-performance scenarios, P/Invoke, unsafe code, memory manipulation

```csharp
public class UnmanagedBuffer<T> where T : unmanaged
{
    private T[] _buffer;

    public UnmanagedBuffer(int size)
    {
        _buffer = new T[size];
    }

    // ✅ Can use unsafe code with unmanaged constraint
    public unsafe void* GetPointer()
    {
        fixed (T* ptr = _buffer)
        {
            return ptr;
        }
    }

    public unsafe void WriteToMemory(T value, int index)
    {
        fixed (T* ptr = &_buffer[index])
        {
            *ptr = value;  // Direct memory write
        }
    }

    public int SizeInBytes()
    {
        unsafe
        {
            return sizeof(T) * _buffer.Length;  // ✅ Can use sizeof
        }
    }
}

// Valid types
var intBuffer = new UnmanagedBuffer<int>(100);        // ✅ int is unmanaged
var floatBuffer = new UnmanagedBuffer<float>(100);    // ✅ float is unmanaged
var pointBuffer = new UnmanagedBuffer<Point>(100);    // ✅ Point (struct with int fields)

public struct Point  // ✅ Unmanaged struct
{
    public int X;
    public int Y;
}

// Invalid types
public struct Person  // ❌ Managed struct (contains string reference)
{
    public string Name;  // ❌ string is managed
    public int Age;
}

// var personBuffer = new UnmanagedBuffer<Person>(); // ❌ Compiler error
```

**Real-world example**: Span<T> and Memory<T>
```csharp
public readonly ref struct Span<T>
{
    // Uses unmanaged constraint internally for some operations
    public unsafe Span(void* pointer, int length) where T : unmanaged
    {
        // Direct memory access
    }
}
```

### 7. `where T : struct, IComparable<T>` - Comparable Value Type

**What it means**: T must be a value type AND implement IComparable<T>

**Use when**: Sorting value types, min/max operations, range checks

```csharp
public class ComparableValueRepository<T>
    where T : struct, IComparable<T>
{
    private List<T> _items = new();

    public void Add(T item)
    {
        _items.Add(item);
    }

    // ✅ Can use CompareTo for sorting
    public T GetMin()
    {
        if (_items.Count == 0)
            return default;

        T min = _items[0];
        foreach (var item in _items)
        {
            if (item.CompareTo(min) < 0)
                min = item;
        }
        return min;
    }

    public T GetMax()
    {
        if (_items.Count == 0)
            return default;

        T max = _items[0];
        foreach (var item in _items)
        {
            if (item.CompareTo(max) > 0)
                max = item;
        }
        return max;
    }

    // ✅ Range queries with value semantics
    public IEnumerable<T> GetRange(T min, T max)
    {
        return _items.Where(x =>
            x.CompareTo(min) >= 0 &&
            x.CompareTo(max) <= 0);
    }
}

// Valid types
var intRepo = new ComparableValueRepository<int>();           // ✅ int is struct + IComparable<int>
var dateRepo = new ComparableValueRepository<DateTime>();     // ✅ DateTime is struct + IComparable<DateTime>
var tempRepo = new ComparableValueRepository<Temperature>();  // ✅ custom struct with IComparable

public struct Temperature : IComparable<Temperature>
{
    public double Celsius { get; set; }

    public int CompareTo(Temperature other)
    {
        return Celsius.CompareTo(other.Celsius);
    }
}
```

### 8. Multiple Type Parameters - Independent Constraints

**What it means**: Different type parameters can have different constraints

**Use when**: Key-value storage, generic data structures, complex generic algorithms

```csharp
public class Manager<TEntity, TKey>
    where TEntity : class, IEntity, new()      // TEntity constraints
    where TKey : struct, IComparable<TKey>     // TKey constraints
{
    private Dictionary<TKey, TEntity> _storage = new();

    public void Store(TKey key, TEntity entity)
    {
        _storage[key] = entity;
    }

    public TEntity? Retrieve(TKey key)
    {
        return _storage.TryGetValue(key, out var entity) ? entity : null;
    }

    // ✅ Can create TEntity with new()
    public TEntity CreateAndStore(TKey key)
    {
        var entity = new TEntity();
        _storage[key] = entity;
        return entity;
    }

    // ✅ Can use TKey.CompareTo() for range queries
    public IEnumerable<TEntity> GetRange(TKey minKey, TKey maxKey)
    {
        return _storage
            .Where(kvp => kvp.Key.CompareTo(minKey) >= 0 &&
                          kvp.Key.CompareTo(maxKey) <= 0)
            .Select(kvp => kvp.Value);
    }

    // ✅ Can access TEntity.Id
    public TEntity? GetByEntityId(int entityId)
    {
        return _storage.Values.FirstOrDefault(e => e.Id == entityId);
    }
}

// Usage with different constraint combinations
var manager1 = new Manager<User, int>();       // ✅ User is class+IEntity+new(), int is struct+IComparable
var manager2 = new Manager<Product, Guid>();   // ✅ Product is class+IEntity+new(), Guid is struct+IComparable
var manager3 = new Manager<Order, DateTime>(); // ✅ Order is class+IEntity+new(), DateTime is struct+IComparable
```

**Real-world example**: Dictionary<TKey, TValue>
```csharp
public class Dictionary<TKey, TValue>
    where TKey : notnull  // TKey must be non-nullable
{
    // TKey and TValue have independent constraints
}
```

### 9. `default` Keyword - Type-Dependent Behavior

**What it means**: `default(T)` returns the default value for type T

**Behavior varies by constraint**:
- **Reference types (class)**: `default(T)` = `null`
- **Value types (struct)**: `default(T)` = `0`, `false`, `(0,0)`, etc.
- **Unconstrained**: `default(T)` depends on actual type at runtime

```csharp
public class DefaultValueDemonstrator<T>
{
    // No constraint - default behavior depends on T
    public T GetDefaultValue()
    {
        var defaultValue = default(T);
        Console.WriteLine($"default({typeof(T).Name}) = {defaultValue?.ToString() ?? "null"}");
        return defaultValue!;
    }

    public bool IsDefault(T value)
    {
        // ✅ Safe way to check if value is default
        bool isDefault = EqualityComparer<T>.Default.Equals(value, default);
        Console.WriteLine($"IsDefault({value}) = {isDefault}");
        return isDefault;
    }
}

// Examples
var stringDemo = new DefaultValueDemonstrator<string>();
stringDemo.GetDefaultValue();  // Output: default(String) = null

var intDemo = new DefaultValueDemonstrator<int>();
intDemo.GetDefaultValue();     // Output: default(Int32) = 0

var dateDemo = new DefaultValueDemonstrator<DateTime>();
dateDemo.GetDefaultValue();    // Output: default(DateTime) = 01/01/0001 00:00:00

var pointDemo = new DefaultValueDemonstrator<Point>();
pointDemo.GetDefaultValue();   // Output: default(Point) = (0, 0)
```

**With class constraint**:
```csharp
public class ReferenceTypeDefault<T> where T : class
{
    public T? GetDefault()
    {
        return default;  // Always null for reference types
    }

    public bool IsNull(T? value)
    {
        return value == default;  // Same as: value == null
    }
}
```

**With struct constraint**:
```csharp
public class ValueTypeDefault<T> where T : struct
{
    public T GetDefault()
    {
        return default;  // Never null - returns zero-initialized value
    }

    public bool IsZero(T value)
    {
        return EqualityComparer<T>.Default.Equals(value, default);
    }
}
```

---

## When to Use Each Constraint Type

### Decision Tree

```
Do you need to create instances?
├─ YES → Use: where T : new()
└─ NO  → Continue

Is T always a reference type?
├─ YES → Use: where T : class
└─ NO  → Continue

Is T always a value type?
├─ YES → Use: where T : struct
└─ NO  → Continue

Do you need specific properties/methods?
├─ YES → Use: where T : IInterface or where T : BaseClass
└─ NO  → Continue

Do you need to compare/sort?
├─ YES → Use: where T : IComparable<T>
└─ NO  → Continue

Do you need unsafe code/pointers?
├─ YES → Use: where T : unmanaged
└─ NO  → No constraint needed (or use least restrictive)
```

### Constraint Selection Guide

| **Scenario** | **Constraint** | **Reason** |
|-------------|----------------|------------|
| Repository with CRUD | `where T : class, IEntity, new()` | Need reference type, Id property, and can create instances |
| Nullable wrapper | `where T : struct` | Value types can be wrapped in Nullable<T> |
| Factory pattern | `where T : new()` | Must create instances |
| Sorting/comparison | `where T : IComparable<T>` | Need CompareTo() method |
| Entity with audit fields | `where T : BaseEntity, new()` | Need common base properties |
| High-performance buffer | `where T : unmanaged` | Need pointer access |
| Cache with nullability | `where T : class` | Need to store null values |
| Range queries on primitives | `where T : struct, IComparable<T>` | Need value semantics + comparison |
| Dependency injection | `where T : class, new()` | Need to instantiate services |

---

## Real-World Scenarios

### Scenario 1: Entity Framework-Style Repository

**Requirements**:
- Work with database entities
- Entities have Id property
- Need to create new entities
- Entities are reference types

**Solution**:
```csharp
public interface IEntity
{
    int Id { get; set; }
}

public class EFRepository<T> where T : class, IEntity, new()
{
    private DbContext _context;
    private DbSet<T> _dbSet;

    public EFRepository(DbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    // ✅ Can use Id from IEntity
    public T? GetById(int id)
    {
        return _dbSet.FirstOrDefault(e => e.Id == id);
    }

    // ✅ Can create with new()
    public T Create()
    {
        var entity = new T();
        _dbSet.Add(entity);
        return entity;
    }

    // ✅ Reference type allows null checks
    public void AddOrUpdate(T entity)
    {
        if (entity == null)
            throw new ArgumentNullException(nameof(entity));

        var existing = GetById(entity.Id);
        if (existing == null)
            _dbSet.Add(entity);
        else
            _context.Entry(existing).CurrentValues.SetValues(entity);
    }
}
```

### Scenario 2: Priority Queue with Comparable Items

**Requirements**:
- Items need to be sorted by priority
- Support any comparable type
- Efficient insertion and removal

**Solution**:
```csharp
public class PriorityQueue<T> where T : IComparable<T>
{
    private List<T> _heap = new();

    public void Enqueue(T item)
    {
        _heap.Add(item);
        HeapifyUp(_heap.Count - 1);
    }

    public T Dequeue()
    {
        if (_heap.Count == 0)
            throw new InvalidOperationException("Queue is empty");

        T result = _heap[0];
        _heap[0] = _heap[_heap.Count - 1];
        _heap.RemoveAt(_heap.Count - 1);

        if (_heap.Count > 0)
            HeapifyDown(0);

        return result;
    }

    private void HeapifyUp(int index)
    {
        while (index > 0)
        {
            int parent = (index - 1) / 2;

            // ✅ Can use CompareTo with IComparable constraint
            if (_heap[index].CompareTo(_heap[parent]) >= 0)
                break;

            Swap(index, parent);
            index = parent;
        }
    }

    private void Swap(int i, int j)
    {
        T temp = _heap[i];
        _heap[i] = _heap[j];
        _heap[j] = temp;
    }

    private void HeapifyDown(int index) { /* ... */ }
}

// Usage with any comparable type
var intQueue = new PriorityQueue<int>();
var dateQueue = new PriorityQueue<DateTime>();
var customQueue = new PriorityQueue<Priority>();

public struct Priority : IComparable<Priority>
{
    public int Value { get; set; }
    public int CompareTo(Priority other) => Value.CompareTo(other.Value);
}
```

### Scenario 3: High-Performance Math Library

**Requirements**:
- Work with numeric types (int, float, double)
- Zero-allocation performance
- Pointer-based operations for SIMD

**Solution**:
```csharp
public class VectorMath<T> where T : unmanaged
{
    // ✅ Can use unsafe code with unmanaged constraint
    public unsafe void Add(T[] source1, T[] source2, T[] destination)
    {
        if (source1.Length != source2.Length || source1.Length != destination.Length)
            throw new ArgumentException("Arrays must be same length");

        fixed (T* ptr1 = source1, ptr2 = source2, ptrDest = destination)
        {
            int elementSize = sizeof(T);
            int byteCount = source1.Length * elementSize;

            // Direct memory operations for maximum performance
            for (int i = 0; i < source1.Length; i++)
            {
                // Platform-specific SIMD operations could go here
                byte* p1 = (byte*)(ptr1 + i);
                byte* p2 = (byte*)(ptr2 + i);
                byte* pDest = (byte*)(ptrDest + i);

                // Simplified - real implementation would use SIMD
                Buffer.MemoryCopy(p1, pDest, elementSize, elementSize);
            }
        }
    }

    public unsafe int GetSizeInBytes(int elementCount)
    {
        return sizeof(T) * elementCount;  // ✅ sizeof works with unmanaged
    }
}

// Usage with numeric types
var intMath = new VectorMath<int>();
var floatMath = new VectorMath<float>();
var doubleMath = new VectorMath<double>();
```

### Scenario 4: Caching with Audit Trail

**Requirements**:
- Cache entities with timestamps
- Track when items were cached/updated
- Support any entity with audit fields

**Solution**:
```csharp
public abstract class AuditableEntity
{
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
}

public class AuditCache<T> where T : AuditableEntity, new()
{
    private Dictionary<int, CacheEntry<T>> _cache = new();

    private class CacheEntry<TEntity>
    {
        public TEntity Entity { get; set; }
        public DateTime CachedAt { get; set; }
        public int HitCount { get; set; }
    }

    public void Add(T entity, string username)
    {
        // ✅ Can access AuditableEntity properties
        entity.CreatedAt = DateTime.Now;
        entity.CreatedBy = username;

        _cache[entity.Id] = new CacheEntry<T>
        {
            Entity = entity,
            CachedAt = DateTime.Now,
            HitCount = 0
        };
    }

    public T? Get(int id, string username)
    {
        if (_cache.TryGetValue(id, out var entry))
        {
            entry.HitCount++;

            // ✅ Can update audit fields
            entry.Entity.UpdatedAt = DateTime.Now;

            return entry.Entity;
        }
        return null;
    }

    // ✅ Can create new instance with new()
    public T CreateNew(string username)
    {
        var entity = new T();
        entity.CreatedAt = DateTime.Now;
        entity.CreatedBy = username;
        return entity;
    }

    public IEnumerable<T> GetByCreator(string username)
    {
        // ✅ Can filter by CreatedBy property
        return _cache.Values
            .Where(e => e.Entity.CreatedBy == username)
            .Select(e => e.Entity);
    }
}
```

### Scenario 5: Generic Builder Pattern

**Requirements**:
- Fluent API for object construction
- Must create instances
- Type-safe property setting

**Solution**:
```csharp
public class Builder<T> where T : class, new()
{
    private T _instance;
    private List<Action<T>> _actions = new();

    public Builder()
    {
        // ✅ Can create instance with new()
        _instance = new T();
    }

    public Builder<T> With(Action<T> action)
    {
        _actions.Add(action);
        return this;  // Fluent API
    }

    public T Build()
    {
        // ✅ Apply all actions to reference type
        foreach (var action in _actions)
        {
            action(_instance);
        }
        return _instance;
    }
}

// Usage
public class User
{
    public string Name { get; set; }
    public string Email { get; set; }
    public int Age { get; set; }
}

var user = new Builder<User>()
    .With(u => u.Name = "John")
    .With(u => u.Email = "john@example.com")
    .With(u => u.Age = 30)
    .Build();
```

---

## Performance Considerations

### 1. Boxing with Struct Constraint

#### ❌ Without Constraint - Boxing Overhead
```csharp
public class SlowContainer<T>  // No constraint
{
    private object _value;  // ❌ Forces boxing for value types

    public void Set(T value)
    {
        _value = value;  // 💥 Boxing if T is value type!
    }

    public T Get()
    {
        return (T)_value;  // 💥 Unboxing if T is value type!
    }
}

// Performance test
var container = new SlowContainer<int>();
for (int i = 0; i < 1_000_000; i++)
{
    container.Set(i);     // 1 million boxing operations
    int value = container.Get();  // 1 million unboxing operations
}
// Result: ~50ms, high GC pressure
```

#### ✅ With Constraint - No Boxing
```csharp
public class FastContainer<T> where T : struct
{
    private T _value;  // ✅ No boxing - direct storage

    public void Set(T value)
    {
        _value = value;  // ✅ No boxing
    }

    public T Get()
    {
        return _value;  // ✅ No unboxing
    }
}

// Performance test
var container = new FastContainer<int>();
for (int i = 0; i < 1_000_000; i++)
{
    container.Set(i);     // Zero boxing
    int value = container.Get();  // Zero unboxing
}
// Result: ~5ms, zero GC pressure
// 10x faster!
```

**Benchmark Results**:
```
| Method              | Mean     | Allocated |
|---------------------|----------|-----------|
| WithoutConstraint   | 52.3 ms  | 64 MB     |
| WithStructConstraint| 5.1 ms   | 0 MB      |
```

### 2. Interface Constraint - Virtual Call Overhead

#### Trade-off: Type Safety vs Performance
```csharp
// ✅ Type-safe but has virtual call overhead
public class InterfaceRepository<T> where T : IEntity
{
    public int GetId(T entity)
    {
        return entity.Id;  // Virtual call through interface
    }
}

// ✅ Faster but less type-safe
public class ConcreteRepository
{
    public int GetId(User user)
    {
        return user.Id;  // Direct property access
    }
}
```

**Performance difference**: ~1-2ns per call (negligible unless in tight loops)

### 3. `new()` Constraint - Activation Overhead

```csharp
public class Factory<T> where T : new()
{
    public T Create()
    {
        return new T();  // Uses Activator.CreateInstance internally
    }
}

// Benchmark: Creating 1 million instances
// new T():              ~15ms
// Activator:            ~15ms (same - uses new() internally)
// Direct new User():    ~8ms
```

**Optimization**: If creating many instances, consider object pooling:
```csharp
public class PooledFactory<T> where T : class, new()
{
    private Stack<T> _pool = new();

    public T Rent()
    {
        return _pool.Count > 0 ? _pool.Pop() : new T();
    }

    public void Return(T item)
    {
        _pool.Push(item);
    }
}
```

### 4. Unmanaged Constraint - Zero-Overhead

```csharp
public class UnmanagedVector<T> where T : unmanaged
{
    private T[] _data;

    public unsafe void ProcessWithPointers()
    {
        fixed (T* ptr = _data)
        {
            // ✅ Direct memory access - fastest possible
            for (int i = 0; i < _data.Length; i++)
            {
                // Process ptr[i] directly
            }
        }
    }
}
```

**Performance**: Same as hand-written unsafe C code

### Performance Summary

| **Constraint** | **Performance Impact** | **When to Worry** |
|---------------|------------------------|-------------------|
| `where T : class` | None | Never |
| `where T : struct` | Positive (prevents boxing) | Always use for value types |
| `where T : new()` | Minor (activation overhead) | Only in tight loops creating millions of instances |
| `where T : IInterface` | Minor (virtual call) | Tight loops with billions of calls |
| `where T : BaseClass` | Minor (virtual call) | Same as interface |
| `where T : unmanaged` | None (enables optimizations) | High-performance scenarios |

---

## Common Mistakes

### Mistake 1: Wrong Constraint Order

#### ❌ WRONG - `new()` not last
```csharp
// ❌ Compiler error CS0401
// public class Repository<T> where T : new(), class, IEntity { }

// ❌ Compiler error CS0401
// public class Repository<T> where T : class, new(), IEntity { }
```

#### ✅ CORRECT - `new()` must be last
```csharp
public class Repository<T> where T : class, IEntity, new() { }
```

**Rule**: Constraint order must be:
1. Base class (if any)
2. Interfaces (if any)
3. `class` or `struct` (if specified)
4. `new()` (if specified) - **MUST BE LAST**

### Mistake 2: Over-Constraining

#### ❌ TOO RESTRICTIVE
```csharp
// ❌ BAD: Too many constraints limit reusability
public class OverConstrainedRepository<T>
    where T : class, IEntity, IValidatable, IComparable<T>, ICloneable, new()
{
    // Few types will satisfy all these constraints!
}
```

#### ✅ BETTER - Only necessary constraints
```csharp
// ✅ GOOD: Only essential constraints
public class SimpleRepository<T>
    where T : class, IEntity, new()
{
    // More types can be used
}
```

**Rule**: Use the **minimum** constraints needed for your implementation.

### Mistake 3: Forgetting `new()` When Creating Instances

#### ❌ WRONG - Missing `new()` constraint
```csharp
public class Factory<T> where T : class
{
    public T Create()
    {
        // ❌ Compiler error CS0304: Cannot create an instance of T
        // return new T();

        return default!;  // 💣 Always returns null!
    }
}
```

#### ✅ CORRECT - Add `new()` constraint
```csharp
public class Factory<T> where T : class, new()
{
    public T Create()
    {
        return new T();  // ✅ Works
    }
}
```

### Mistake 4: Mixing `class` and `struct` Constraints

#### ❌ WRONG - Contradictory constraints
```csharp
// ❌ Compiler error CS0449: class or struct constraint must come before other constraints
// public class Bad<T> where T : class, struct { }  // Can't be both!
```

#### ✅ CORRECT - Choose one
```csharp
public class ForClasses<T> where T : class { }
public class ForStructs<T> where T : struct { }
```

### Mistake 5: Assuming `default(T)` is Always Null

#### ❌ WRONG ASSUMPTION
```csharp
public class NullChecker<T>
{
    public bool IsNull(T value)
    {
        // ❌ WRONG: default(T) is NOT always null!
        // For int, default(T) = 0
        // For bool, default(T) = false
        // For Point, default(T) = (0, 0)
        return value == null;  // 💣 Won't compile for value types
    }
}
```

#### ✅ CORRECT - Use EqualityComparer
```csharp
public class DefaultChecker<T>
{
    public bool IsDefault(T value)
    {
        // ✅ CORRECT: Works for all types
        return EqualityComparer<T>.Default.Equals(value, default);
    }
}
```

### Mistake 6: Not Using `class` Constraint When Needed

#### ❌ PROBLEMATIC
```csharp
public class Cache<T>  // No constraint
{
    private T? _value;  // ⚠️ Warning: T? might not be nullable

    public void Clear()
    {
        _value = null;  // ❌ Error if T is value type
    }
}
```

#### ✅ FIXED
```csharp
public class Cache<T> where T : class  // ✅ T is reference type
{
    private T? _value;  // ✅ Correctly nullable

    public void Clear()
    {
        _value = null;  // ✅ Always valid
    }
}
```

### Mistake 7: Forgetting Interface Implementation

#### ❌ RUNTIME ERROR
```csharp
public class Sorter<T>  // ❌ No constraint
{
    public void Sort(List<T> items)
    {
        // 💣 Runtime error if T doesn't implement IComparable
        items.Sort();
    }
}

// Usage
var sorter = new Sorter<MyClass>();  // MyClass doesn't implement IComparable
sorter.Sort(myList);  // 💥 InvalidOperationException at runtime!
```

#### ✅ COMPILE-TIME SAFETY
```csharp
public class Sorter<T> where T : IComparable<T>  // ✅ Constraint
{
    public void Sort(List<T> items)
    {
        items.Sort();  // ✅ Guaranteed to work
    }
}

// Usage
// var sorter = new Sorter<MyClass>();  // ❌ Compiler error - caught early!
```

---

## Best Practices

### 1. Use Minimum Necessary Constraints

✅ **GOOD**: Only constraints you actually use
```csharp
// Only need Id property and ability to create instances
public class Repository<T> where T : IEntity, new()
{
    public T Create() => new T();
    public void Add(T entity) { /* uses entity.Id */ }
}
```

❌ **BAD**: Over-constraining
```csharp
// Don't add constraints you don't use
public class Repository<T>
    where T : class, IEntity, IValidatable, IComparable<T>, new()
{
    // Only uses IEntity and new() - other constraints are unnecessary!
}
```

### 2. Document Why Constraints Are Needed

```csharp
/// <summary>
/// Generic repository for entities.
/// </summary>
/// <typeparam name="T">
/// Entity type.
/// Constraints:
/// - class: Entities are reference types (can be tracked)
/// - IEntity: Provides Id property for lookups
/// - new(): Enables Create() method for entity instantiation
/// </typeparam>
public class Repository<T> where T : class, IEntity, new()
{
    // Implementation
}
```

### 3. Provide Specific Type Versions for Common Cases

```csharp
// ✅ Generic version with constraints
public class Repository<T> where T : class, IEntity, new()
{
    // Implementation
}

// ✅ Specific versions for common types (better performance)
public class UserRepository : Repository<User>
{
    // Additional User-specific methods
}

public class ProductRepository : Repository<Product>
{
    // Additional Product-specific methods
}
```

### 4. Use Constraint Inheritance

```csharp
// Base class establishes constraints
public abstract class BaseRepository<T> where T : class, IEntity, new()
{
    protected List<T> Items { get; } = new();

    public virtual void Add(T entity)
    {
        Items.Add(entity);
    }
}

// Derived class inherits constraints automatically
public class AuditRepository<T> : BaseRepository<T>
    where T : class, IEntity, IAuditable, new()  // ✅ Can add MORE constraints
{
    public override void Add(T entity)
    {
        entity.CreatedAt = DateTime.Now;  // ✅ Can use IAuditable
        base.Add(entity);
    }
}
```

### 5. Combine with Generic Methods

```csharp
public class DataService<T> where T : class, IEntity
{
    // ✅ Method can add additional constraints
    public void Process<TSpecific>(TSpecific entity)
        where TSpecific : T, IValidatable  // TSpecific is T + IValidatable
    {
        if (entity.Validate())  // ✅ Can call Validate
        {
            // Process entity.Id  // ✅ Can use Id from T's IEntity constraint
        }
    }
}
```

### 6. Use Generic Constraints for Dependency Injection

```csharp
public interface IRepository<T> where T : class
{
    T? GetById(int id);
    void Add(T entity);
}

public class Repository<T> : IRepository<T>
    where T : class, IEntity, new()
{
    // Implementation
}

// DI Container registration
services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

// Usage in controller
public class ApiController<T> : ControllerBase
    where T : class, IEntity, new()
{
    private readonly IRepository<T> _repository;

    public ApiController(IRepository<T> repository)
    {
        _repository = repository;
    }

    [HttpPost]
    public IActionResult Create()
    {
        var entity = new T();  // ✅ new() constraint
        _repository.Add(entity);
        return Ok(entity);
    }
}
```

### 7. Prefer Interface Constraints Over Base Class

✅ **PREFER**: Interface constraints (more flexible)
```csharp
public class Repository<T>
    where T : IEntity, IAuditable, new()
{
    // Classes can implement multiple interfaces
}
```

❌ **AVOID** (when possible): Base class constraints (less flexible)
```csharp
public class Repository<T>
    where T : BaseEntity, new()
{
    // Classes can only inherit from ONE base class
}
```

**Exception**: Use base class when you need protected members or concrete implementation.

### 8. Use `class?` for Nullable Reference Types (C# 8+)

```csharp
#nullable enable

// ✅ GOOD: Explicit nullability
public class Cache<T> where T : class
{
    private T? _value;  // Nullable reference type

    public T? Get() => _value;  // Can return null

    public void Set(T value)  // Non-null parameter
    {
        _value = value ?? throw new ArgumentNullException(nameof(value));
    }
}
```

---

## Constraint Comparison Table

| **Constraint** | **Syntax** | **Guarantees** | **Use Case** | **Performance** | **Common With** |
|----------------|-----------|----------------|--------------|-----------------|-----------------|
| **Class** | `where T : class` | T is reference type, nullable | Caching, repositories, null checks | No overhead | `new()`, `IEntity` |
| **Struct** | `where T : struct` | T is value type, never null | Primitives, custom structs, no boxing | Best (no boxing) | `IComparable<T>` |
| **New** | `where T : new()` | T has parameterless constructor | Factories, DI, object creation | Minor activation cost | `class`, `IEntity` |
| **Interface** | `where T : IInterface` | T implements interface | Polymorphism, sorting, validation | Minor virtual call | `class`, `new()` |
| **Base Class** | `where T : BaseClass` | T inherits from base class | Common functionality, audit | Minor virtual call | `new()` |
| **Unmanaged** | `where T : unmanaged` | T has no managed references | Unsafe code, P/Invoke, SIMD | Best (pointer access) | None (standalone) |
| **Multiple Interfaces** | `where T : I1, I2, I3` | T implements all interfaces | Complex behavior requirements | Cumulative virtual calls | `class`, `new()` |
| **Multiple Type Params** | `<T, U> where T : ... where U : ...` | Independent constraints per type | Generic data structures | Varies by constraint | Each other |
| **Default** | `default(T)` | Type-dependent default value | Initialization, reset | No overhead | All constraints |

### Constraint Combinations (Most Common)

| **Combination** | **Example** | **Use Case** |
|-----------------|-------------|--------------|
| `class, IEntity, new()` | `Repository<T>` | Entity repositories with CRUD |
| `struct, IComparable<T>` | `PriorityQueue<T>` | Sortable value types |
| `BaseEntity, new()` | `AuditRepository<T>` | Entities with audit tracking |
| `class, IValidatable, new()` | `ValidationService<T>` | Validatable objects |
| `unmanaged` (alone) | `VectorMath<T>` | High-performance math |
| `class` (alone) | `Cache<T>` | Simple reference type caching |
| `struct` (alone) | `Nullable<T>` | Nullable value types |

---

## Summary

### Key Takeaways

1. **Compile-Time Safety**: Constraints catch errors at compile time, not runtime
2. **IntelliSense Support**: IDE can suggest members based on constraints
3. **Self-Documenting**: Constraints document valid types in the signature
4. **Performance**: Proper constraints can prevent boxing and enable optimizations
5. **Flexibility**: Use minimum necessary constraints for maximum reusability

### When to Use Constraints

✅ **ALWAYS** use constraints when:
- You need to call specific methods/properties on T
- You need to create instances of T
- You need specific type characteristics (reference/value)
- You need type safety and early error detection

❌ **DON'T** use constraints when:
- You only store/pass through values (no operations on T)
- You want maximum flexibility
- You're implementing a pure container (like Tuple<T>)

### Constraint Hierarchy (Least to Most Restrictive)

```
No constraint
    ↓
class (or struct)
    ↓
IInterface
    ↓
BaseClass
    ↓
new()
    ↓
Multiple constraints (class + IEntity + IValidatable + new())
```

### Final Recommendation

**Start with no constraints**, then add the minimum constraints needed for your implementation. Constraints should serve your code, not restrict it unnecessarily.

```csharp
// Step 1: Start simple
public class Repository<T> { }

// Step 2: Need to access Id? Add IEntity
public class Repository<T> where T : IEntity { }

// Step 3: Need to create instances? Add new()
public class Repository<T> where T : IEntity, new() { }

// Step 4: Need reference semantics? Add class
public class Repository<T> where T : class, IEntity, new() { }

// Stop! Don't add more unless you actually need them
```

---

## Additional Resources

- **C# Language Specification**: [Generic Type Parameter Constraints](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/generics/constraints-on-type-parameters)
- **Performance**: [Boxing and Unboxing](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/types/boxing-and-unboxing)
- **Unsafe Code**: [Unmanaged Types](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/unmanaged-types)
- **Design Patterns**: Repository Pattern, Factory Pattern, Builder Pattern

---

**Remember**: Constraints are a powerful tool for creating type-safe, flexible, and performant generic code. Use them wisely! ✨
