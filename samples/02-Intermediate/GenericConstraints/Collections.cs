namespace GenericConstraints;

// ═══════════════════════════════════════════════════════════
// INTERFACES & BASE CLASSES
// ═══════════════════════════════════════════════════════════

/// <summary>
/// Entity interface - All entities must have an Id
/// </summary>
public interface IEntity
{
    int Id { get; set; }
}

/// <summary>
/// Named entity interface - Entities with names
/// </summary>
public interface INamedEntity : IEntity
{
    string Name { get; set; }
}

/// <summary>
/// Generic repository interface
/// </summary>
public interface IRepository<T> where T : class
{
    T? GetById(int id);
    IEnumerable<T> GetAll();
    void Add(T entity);
    void Update(T entity);
    void Delete(int id);
    int Count { get; }
}

/// <summary>
/// Base entity class with audit fields
/// </summary>
public abstract class BaseEntity : IEntity
{
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime? UpdatedAt { get; set; }
}

// ═══════════════════════════════════════════════════════════
// ❌ BAD PRACTICE: No Constraints
// ═══════════════════════════════════════════════════════════

/// <summary>
/// ❌ BAD: No constraints - Runtime errors possible!
/// Problem: Can't guarantee T has Id, can't create new instances
/// </summary>
public class BadRepository<T>
{
    private readonly List<T> _items = new();

    // ❌ BAD: What if T doesn't have Id property? Compile error!
    // public T? GetById(int id) => _items.FirstOrDefault(x => x.Id == id);  // ERROR!

    public void Add(T item)
    {
        _items.Add(item);
        // ❌ BAD: Can't access any properties, no guarantee what T is
        Console.WriteLine($"  Added item (can't show details - no constraints!)");
    }

    // ❌ BAD: Can't create new instance without new() constraint
    // public T Create() => new T();  // ERROR!

    public List<T> GetAll() => _items;
}

// ═══════════════════════════════════════════════════════════
// ✅ GOOD PRACTICE: Class Constraint
// ═══════════════════════════════════════════════════════════

/// <summary>
/// ✅ GOOD: Class constraint ensures T is reference type
/// Use case: Repository for entity classes
/// </summary>
public class Repository<T> where T : class, IEntity, new()
{
    private readonly List<T> _items = new();
    private int _nextId = 1;

    // ✅ GOOD: Can use IEntity.Id property
    public T? GetById(int id)
    {
        var item = _items.FirstOrDefault(x => x.Id == id);
        if (item != null)
            Console.WriteLine($"  🔍 Found: ID {id}");
        else
            Console.WriteLine($"  ❌ Not found: ID {id}");
        return item;
    }

    public IEnumerable<T> GetAll() => _items;

    public void Add(T entity)
    {
        if (entity.Id == 0)
            entity.Id = _nextId++;

        _items.Add(entity);
        Console.WriteLine($"  ✅ Added: {typeof(T).Name} (ID: {entity.Id})");
    }

    public void Update(T entity)
    {
        var existing = GetById(entity.Id);
        if (existing != null)
        {
            var index = _items.IndexOf(existing);
            _items[index] = entity;
            Console.WriteLine($"  ✏️  Updated: {typeof(T).Name} (ID: {entity.Id})");
        }
    }

    public void Delete(int id)
    {
        var item = GetById(id);
        if (item != null)
        {
            _items.Remove(item);
            Console.WriteLine($"  🗑️  Deleted: ID {id}");
        }
    }

    // ✅ GOOD: Can create new instance with new() constraint
    public T Create()
    {
        var item = new T();  // Requires new() constraint
        Console.WriteLine($"  🏗️  Created new {typeof(T).Name}");
        return item;
    }

    public int Count => _items.Count;
}

// ═══════════════════════════════════════════════════════════
// ✅ GOOD PRACTICE: Struct Constraint
// ═══════════════════════════════════════════════════════════

/// <summary>
/// ✅ GOOD: Struct constraint ensures T is value type
/// Use case: Containers for primitives, enums, custom structs
/// </summary>
public class ValueContainer<T> where T : struct
{
    private T _value;
    private bool _hasValue;

    public T Value
    {
        get => _value;
        set
        {
            _value = value;
            _hasValue = true;
        }
    }

    public bool HasValue => _hasValue;

    // ✅ GOOD: Can use default keyword with struct
    public T GetValueOrDefault(T defaultValue = default)
    {
        return _hasValue ? _value : defaultValue;
    }

    public void Clear()
    {
        _value = default;  // ✅ default works with struct constraint
        _hasValue = false;
        Console.WriteLine($"  🧹 Cleared value (set to default {typeof(T).Name})");
    }

    public void Display()
    {
        if (_hasValue)
            Console.WriteLine($"  📦 Value: {_value} (Type: {typeof(T).Name})");
        else
            Console.WriteLine($"  📭 No value (Type: {typeof(T).Name})");
    }
}

// ═══════════════════════════════════════════════════════════
// ✅ GOOD PRACTICE: IComparable Constraint
// ═══════════════════════════════════════════════════════════

/// <summary>
/// ✅ GOOD: IComparable constraint enables sorting/comparison
/// Use case: Min/max operations, sorting, range checks
/// </summary>
public class ComparableRepository<T> where T : IComparable<T>
{
    private readonly List<T> _items = new();

    public void Add(T item)
    {
        _items.Add(item);
        Console.WriteLine($"  ✅ Added: {item}");
    }

    // ✅ GOOD: Can use CompareTo with IComparable constraint
    public T? GetMin()
    {
        if (_items.Count == 0) return default;

        T min = _items[0];
        foreach (var item in _items)
        {
            if (item.CompareTo(min) < 0)
                min = item;
        }

        Console.WriteLine($"  🔽 Min: {min}");
        return min;
    }

    public T? GetMax()
    {
        if (_items.Count == 0) return default;

        T max = _items[0];
        foreach (var item in _items)
        {
            if (item.CompareTo(max) > 0)
                max = item;
        }

        Console.WriteLine($"  🔼 Max: {max}");
        return max;
    }

    // ✅ GOOD: Can sort with IComparable
    public List<T> GetSorted()
    {
        var sorted = _items.OrderBy(x => x).ToList();
        Console.WriteLine($"  🔄 Sorted {sorted.Count} items");
        return sorted;
    }

    public IEnumerable<T> GetRange(T min, T max)
    {
        return _items.Where(x => x.CompareTo(min) >= 0 && x.CompareTo(max) <= 0);
    }

    public int Count => _items.Count;
}

// ═══════════════════════════════════════════════════════════
// ✅ ADVANCED: Multiple Constraints
// ═══════════════════════════════════════════════════════════

/// <summary>
/// ✅ ADVANCED: Multiple type parameters with different constraints
/// Use case: Key-value storage with type safety
/// </summary>
public class Manager<TEntity, TKey>
    where TEntity : class, IEntity, new()
    where TKey : struct, IComparable<TKey>
{
    private readonly Dictionary<TKey, TEntity> _storage = new();

    public void Store(TKey key, TEntity entity)
    {
        _storage[key] = entity;
        Console.WriteLine($"  📦 Stored {typeof(TEntity).Name} with key {key}");
    }

    public TEntity? Retrieve(TKey key)
    {
        if (_storage.TryGetValue(key, out var entity))
        {
            Console.WriteLine($"  ✅ Retrieved: ID {entity.Id}");
            return entity;
        }

        Console.WriteLine($"  ❌ Not found: key {key}");
        return null;
    }

    // ✅ GOOD: Can create entity with new() constraint
    public TEntity CreateAndStore(TKey key)
    {
        var entity = new TEntity();
        _storage[key] = entity;
        Console.WriteLine($"  🏗️  Created and stored new {typeof(TEntity).Name} with key {key}");
        return entity;
    }

    // ✅ GOOD: Can use IComparable on TKey
    public IEnumerable<TEntity> GetRange(TKey minKey, TKey maxKey)
    {
        return _storage
            .Where(kvp => kvp.Key.CompareTo(minKey) >= 0 && kvp.Key.CompareTo(maxKey) <= 0)
            .Select(kvp => kvp.Value);
    }

    public int Count => _storage.Count;
}

// ═══════════════════════════════════════════════════════════
// ✅ ADVANCED: Base Class Constraint
// ═══════════════════════════════════════════════════════════

/// <summary>
/// ✅ ADVANCED: Base class constraint with audit tracking
/// Use case: Repositories that need audit fields
/// </summary>
public class AuditRepository<T> where T : BaseEntity, new()
{
    private readonly List<T> _items = new();

    public void Add(T entity)
    {
        // ✅ GOOD: Can access BaseEntity properties
        entity.CreatedAt = DateTime.Now;
        entity.UpdatedAt = null;

        _items.Add(entity);
        Console.WriteLine($"  ✅ Added {typeof(T).Name} - Created: {entity.CreatedAt:yyyy-MM-dd HH:mm:ss}");
    }

    public void Update(T entity)
    {
        var existing = _items.FirstOrDefault(x => x.Id == entity.Id);
        if (existing != null)
        {
            // ✅ GOOD: Track update time
            entity.UpdatedAt = DateTime.Now;

            int index = _items.IndexOf(existing);
            _items[index] = entity;
            Console.WriteLine($"  ✏️  Updated {typeof(T).Name} - Updated: {entity.UpdatedAt:yyyy-MM-dd HH:mm:ss}");
        }
    }

    public IEnumerable<T> GetAll() => _items;
    public int Count => _items.Count;
}

// ═══════════════════════════════════════════════════════════
// ✅ ADVANCED: Unmanaged Constraint (C# 7.3+)
// ═══════════════════════════════════════════════════════════

/// <summary>
/// ✅ ADVANCED: Unmanaged constraint for high-performance scenarios
/// Use case: Unsafe code, P/Invoke, performance-critical operations
/// </summary>
public class UnmanagedBuffer<T> where T : unmanaged
{
    private readonly T[] _buffer;

    public UnmanagedBuffer(int size)
    {
        _buffer = new T[size];
        Console.WriteLine($"  🗂️  Created unmanaged buffer: {typeof(T).Name}[{size}]");
    }

    public T this[int index]
    {
        get => _buffer[index];
        set => _buffer[index] = value;
    }

    // ✅ GOOD: Can use unsafe code with unmanaged constraint
    public unsafe void* GetPointer()
    {
        fixed (T* ptr = _buffer)
        {
            return ptr;
        }
    }

    public int Length => _buffer.Length;

    public void Fill(T value)
    {
        for (int i = 0; i < _buffer.Length; i++)
            _buffer[i] = value;

        Console.WriteLine($"  ✅ Filled buffer with {value}");
    }
}

// ═══════════════════════════════════════════════════════════
// ✅ ADVANCED: Default Keyword Usage
// ═══════════════════════════════════════════════════════════

/// <summary>
/// ✅ GOOD: Default keyword with different constraint types
/// Demonstrates how default behaves with class/struct/unconstrained
/// </summary>
public class DefaultValueDemonstrator<T>
{
    // No constraint - default can be null or 0 depending on T
    public T GetDefaultValue()
    {
        var defaultValue = default(T);
        Console.WriteLine($"  🔍 default({typeof(T).Name}) = {defaultValue?.ToString() ?? "null"}");
        return defaultValue!;
    }

    public bool IsDefault(T value)
    {
        bool isDefault = EqualityComparer<T>.Default.Equals(value, default);
        Console.WriteLine($"  🔍 IsDefault({value}) = {isDefault}");
        return isDefault;
    }
}

// ═══════════════════════════════════════════════════════════
// ENTITY CLASSES
// ═══════════════════════════════════════════════════════════

public class User : BaseEntity, INamedEntity
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;

    public User() { }  // Parameterless constructor for new() constraint

    public override string ToString() => $"User(Id:{Id}, Name:{Name}, Email:{Email})";
}

public class Product : BaseEntity, INamedEntity
{
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Stock { get; set; }

    public Product() { }  // Parameterless constructor for new() constraint

    public override string ToString() => $"Product(Id:{Id}, Name:{Name}, Price:{Price:C}, Stock:{Stock})";
}

public class Order : BaseEntity
{
    public int UserId { get; set; }
    public decimal TotalAmount { get; set; }
    public OrderStatus Status { get; set; }

    public Order() { }  // Parameterless constructor for new() constraint

    public override string ToString() => $"Order(Id:{Id}, UserId:{UserId}, Total:{TotalAmount:C}, Status:{Status})";
}

public enum OrderStatus
{
    Pending,
    Processing,
    Shipped,
    Delivered,
    Cancelled
}

// ═══════════════════════════════════════════════════════════
// CUSTOM STRUCTS
// ═══════════════════════════════════════════════════════════

public struct Point : IComparable<Point>
{
    public int X { get; set; }
    public int Y { get; set; }

    public Point(int x, int y)
    {
        X = x;
        Y = y;
    }

    // Distance from origin for comparison
    public double DistanceFromOrigin => Math.Sqrt(X * X + Y * Y);

    public int CompareTo(Point other)
    {
        return DistanceFromOrigin.CompareTo(other.DistanceFromOrigin);
    }

    public override string ToString() => $"({X}, {Y})";
}

public struct Temperature : IComparable<Temperature>
{
    public double Celsius { get; set; }

    public Temperature(double celsius)
    {
        Celsius = celsius;
    }

    public double Fahrenheit => Celsius * 9 / 5 + 32;
    public double Kelvin => Celsius + 273.15;

    public int CompareTo(Temperature other)
    {
        return Celsius.CompareTo(other.Celsius);
    }

    public override string ToString() => $"{Celsius:F1}°C ({Fahrenheit:F1}°F)";
}
