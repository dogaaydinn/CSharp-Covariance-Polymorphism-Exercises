# 🔗 Generic Constraints - Repository Pattern

## 📚 Overview

**GenericConstraints** demonstrates all C# generic constraint types with a production-ready Repository Pattern. Learn compile-time type safety through constraints.

**Lines of Code**: 1,016 (504 Collections.cs + 512 Program.cs)
**Build Status**: ✅ 0 errors

## 🎯 Constraint Types

1. **`where T : class`** - Reference type (User, Product)
2. **`where T : struct`** - Value type (int, Point)
3. **`where T : new()`** - Parameterless constructor
4. **`where T : IInterface`** - Interface (IEntity, IComparable)
5. **`where T : BaseClass`** - Base class (BaseEntity)
6. **`where T : unmanaged`** - Unmanaged type (C# 7.3+)
7. **Multiple constraints** - Combined type safety
8. **Multiple type parameters** - Independent constraints
9. **Default keyword** - Type-dependent behavior

## 💻 Quick Start

```bash
cd samples/02-Intermediate/GenericConstraints
dotnet build
dotnet run
```

## 🎓 Key Examples

### Repository Pattern (CRUD)
```csharp
public class Repository<T> where T : class, IEntity, new()
{
    public T Create() => new T();  // ✅ new() constraint
    public T? GetById(int id) => _items.FirstOrDefault(x => x.Id == id);  // ✅ IEntity
    public void Add(T entity) => _items.Add(entity);  // ✅ class constraint
}

// Usage - Type-safe CRUD
var userRepo = new Repository<User>();
var newUser = userRepo.Create();
userRepo.Add(newUser);
var retrieved = userRepo.GetById(1);
```

### Multiple Constraints
```csharp
public class Manager<TEntity, TKey>
    where TEntity : class, IEntity, new()      // Entity constraints
    where TKey : struct, IComparable<TKey>     // Key constraints
{
    public TEntity CreateAndStore(TKey key)
    {
        var entity = new TEntity();  // ✅ Can create with new()
        _storage[key] = entity;
        return entity;
    }

    public IEnumerable<TEntity> GetRange(TKey min, TKey max)
    {
        return _storage
            .Where(kvp => kvp.Key.CompareTo(min) >= 0)  // ✅ Can compare keys
            .Select(kvp => kvp.Value);
    }
}
```

### Struct Constraint
```csharp
public class ValueContainer<T> where T : struct
{
    private T _value;

    public T GetValueOrDefault(T defaultValue = default)
    {
        return _hasValue ? _value : defaultValue;  // ✅ default works with struct
    }
}

// Usage
var intContainer = new ValueContainer<int> { Value = 42 };
var pointContainer = new ValueContainer<Point> { Value = new Point(10, 20) };
```

### IComparable Constraint
```csharp
public class ComparableRepository<T> where T : IComparable<T>
{
    public T? GetMin()
    {
        T min = _items[0];
        foreach (var item in _items)
        {
            if (item.CompareTo(min) < 0)  // ✅ Can use CompareTo
                min = item;
        }
        return min;
    }
}
```

### Base Class Constraint (Audit)
```csharp
public class AuditRepository<T> where T : BaseEntity, new()
{
    public void Add(T entity)
    {
        entity.CreatedAt = DateTime.Now;  // ✅ Access BaseEntity properties
        entity.UpdatedAt = null;
        _items.Add(entity);
    }
}
```

### Unmanaged Constraint (Performance)
```csharp
public class UnmanagedBuffer<T> where T : unmanaged
{
    public unsafe void* GetPointer()  // ✅ Can use unsafe code
    {
        fixed (T* ptr = _buffer)
            return ptr;
    }
}
```

## 📊 9 Demonstrations

1. ❌ **Bad Practice** - No constraints → runtime errors
2. ✅ **Class Constraint** - Repository<T> with class + IEntity + new()
3. ✅ **Struct Constraint** - ValueContainer<T> for primitives
4. ✅ **IComparable** - Min/Max/Sort operations
5. ✅ **Multiple Constraints** - Manager<TEntity, TKey>
6. ✅ **Base Class** - AuditRepository with automatic tracking
7. ✅ **Unmanaged** - High-performance buffer with unsafe code
8. ✅ **Default Keyword** - null vs 0 vs default
9. ✅ **Repository CRUD** - Complete create/read/update/delete

## 💡 Best Practices

✅ **DO**:
- Use constraints for compile-time safety
- Combine constraints for maximum type safety
- Use `class` for repositories (reference types)
- Use `new()` when creating instances
- Order: BaseClass, interfaces, `new()` last

❌ **DON'T**:
- Skip constraints (no type safety!)
- Over-constrain (only necessary constraints)
- Forget `new()` must be last
- Use unmanaged unnecessarily

## 🔗 Related

- Repository Pattern - Generic data access
- Factory Pattern - new() constraint
- CQRS - Separate repositories

**See**: [WHY_THIS_PATTERN.md](WHY_THIS_PATTERN.md) for detailed explanation
