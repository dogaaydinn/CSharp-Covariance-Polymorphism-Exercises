namespace GenericConstraints;

public interface IEntity
{
    int Id { get; set; }
    string Name { get; set; }
}

// where T : class - Reference type constraint
public class Repository<T> where T : class, IEntity, new()
{
    private readonly List<T> _items = new();

    public void Add(T item)
    {
        if (item.Id == 0)
            item.Id = _items.Count + 1;
        _items.Add(item);
        Console.WriteLine($"✅ Added: {item.Name} (ID: {item.Id})");
    }

    public T Create()
    {
        var item = new T();  // new() constraint
        Console.WriteLine($"🏗️  Created new {typeof(T).Name}");
        return item;
    }

    public T? GetById(int id) => _items.FirstOrDefault(x => x.Id == id);
    public List<T> GetAll() => _items;
}

// where T : struct - Value type constraint
public class ValueContainer<T> where T : struct
{
    public T Value { get; set; }
    public bool HasValue => !EqualityComparer<T>.Default.Equals(Value, default);

    public void Display()
    {
        Console.WriteLine($"Value: {Value}, HasValue: {HasValue}, Type: {typeof(T).Name}");
    }
}

// Multiple type parameters with constraints
public class Manager<TEntity, TKey>
    where TEntity : class, IEntity, new()
    where TKey : struct, IComparable<TKey>
{
    private readonly Dictionary<TKey, TEntity> _storage = new();

    public void Store(TKey key, TEntity entity)
    {
        _storage[key] = entity;
        Console.WriteLine($"📦 Stored {entity.Name} with key {key}");
    }

    public TEntity? Retrieve(TKey key)
    {
        return _storage.TryGetValue(key, out var entity) ? entity : null;
    }
}

// Entities
public class User : IEntity
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}

public class Product : IEntity
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
}

// Unmanaged constraint (C# 7.3+)
public class UnmanagedBuffer<T> where T : unmanaged
{
    private T[] _buffer;

    public UnmanagedBuffer(int size)
    {
        _buffer = new T[size];
        Console.WriteLine($"🗂️  Created unmanaged buffer of {typeof(T).Name}[{size}]");
    }

    public unsafe void* GetPointer()
    {
        fixed (T* ptr = _buffer)
        {
            return ptr;
        }
    }
}
