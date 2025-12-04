# Generic Constraints Exercise

## 📚 Learning Objectives
- **Generic Constraints**: `where T : ...` syntax
- **Class/Struct Constraints**: Reference vs value types
- **Base Class Constraints**: Inheritance requirements
- **Interface Constraints**: Capability requirements
- **new() Constraint**: Parameterless constructor requirement
- **Multiple Constraints**: Combining constraints

## 🎯 Exercise Tasks

Complete **6 TODO methods**:
1. ✅ **Repository<T>** - `where T : class, new()`
2. ✅ **ValueTypeProcessor<T>** - `where T : struct`
3. ✅ **EntityRepository<T>** - `where T : Entity, new()`
4. ✅ **SortedList<T>** - `where T : IComparable<T>`
5. ✅ **DisposableManager<T>** - `where T : class, IDisposable, new()`
6. ✅ **CreateInstance<T>()** - Generic factory method

## 🚀 Getting Started

```bash
cd samples/99-Exercises/Generics/03-GenericConstraints
dotnet test  # Should see ~15 FAILED tests
```

## 🧠 Constraint Types

### 1. **class** - Reference Type Constraint
```csharp
public class Repository<T> where T : class
{
    T? item = null;  // Can be null - reference type
}
```

### 2. **struct** - Value Type Constraint
```csharp
public class Processor<T> where T : struct
{
    T value = default;  // Always has a value - value type
}
```

### 3. **new()** - Constructor Constraint
```csharp
public T Create<T>() where T : new()
{
    return new T();  // Can create instances
}
```

### 4. **BaseClass** - Inheritance Constraint
```csharp
public class Repository<T> where T : Entity
{
    public int GetId(T entity) => entity.Id;  // Can access Entity members
}
```

### 5. **Interface** - Capability Constraint
```csharp
public void Sort<T>(List<T> items) where T : IComparable<T>
{
    items.Sort();  // Can use IComparable<T> methods
}
```

### 6. **Multiple Constraints**
```csharp
public class Manager<T> where T : class, IDisposable, new()
{
    // T must be: reference type, disposable, and have parameterless constructor
}
```

## 💡 Quick Solutions

### TODO 1: Repository<T>
```csharp
public class Repository<T> where T : class, new()
{
    private readonly List<T> _items = new();

    public void Add(T item) => _items.Add(item);

    public T Create() => new T();  // new() constraint

    public List<T> GetAll() => _items;
}
```

### TODO 2: ValueTypeProcessor<T>
```csharp
public class ValueTypeProcessor<T> where T : struct
{
    public T GetDefault() => default(T);

    public bool IsDefault(T value) => EqualityComparer<T>.Default.Equals(value, default);
}
```

### TODO 3: EntityRepository<T>
```csharp
public class EntityRepository<T> where T : Entity, new()
{
    private readonly List<T> _entities = new();

    public void Add(T entity) => _entities.Add(entity);

    public T? FindById(int id) => _entities.FirstOrDefault(e => e.Id == id);

    public List<T> GetRecent(int count) =>
        _entities.OrderByDescending(e => e.CreatedAt).Take(count).ToList();
}
```

### TODO 4: SortedList<T>
```csharp
public class SortedList<T> where T : IComparable<T>
{
    private readonly List<T> _items = new();

    public void Add(T item)
    {
        _items.Add(item);
        _items.Sort();  // IComparable<T> enables sorting
    }

    public T? GetMin() => _items.Count > 0 ? _items[0] : default;
    public T? GetMax() => _items.Count > 0 ? _items[^1] : default;
}
```

### TODO 5: DisposableManager<T>
```csharp
public class DisposableManager<T> where T : class, IDisposable, new()
{
    private readonly List<T> _resources = new();

    public T CreateAndTrack()
    {
        var resource = new T();  // new() constraint
        _resources.Add(resource);
        return resource;
    }

    public void DisposeAll()
    {
        foreach (var resource in _resources)
            resource.Dispose();  // IDisposable constraint
    }
}
```

### TODO 6: CreateInstance<T>()
```csharp
public static T CreateInstance<T>() where T : new()
{
    return new T();
}
```

## 📊 Constraint Combinations

| Constraints | Meaning | Example |
|-------------|---------|---------|
| `where T : class` | Reference type | `T? x = null` |
| `where T : struct` | Value type | Cannot be null |
| `where T : new()` | Has constructor | `new T()` |
| `where T : BaseClass` | Inherits from | Access base members |
| `where T : IInterface` | Implements | Use interface methods |
| `where T : class, new()` | Ref + constructor | Common pattern |
| `where T : class, IDisposable, new()` | Multiple | Most restrictive |

## 🎓 Interview Tips

**Q: Why use constraints?**
- Type safety at compile time
- Access to specific members/methods
- Enable generic algorithms (sorting, comparing)

**Q: Order of constraints?**
```csharp
// CORRECT order:
where T : class, IInterface, new()

// WRONG:
where T : new(), class  // new() must be last!
```

**Q: Can you have `class` and `struct` together?**
No! They're mutually exclusive.

---

**Good luck! 🎉** Check `SOLUTION.md` after trying.
