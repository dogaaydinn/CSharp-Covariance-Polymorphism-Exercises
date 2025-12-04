# Generic Constraints - where T: Constraints

## 📚 Konu
Generic type constraints: class, struct, new(), interface, unmanaged.

## 🔑 Kavramlar
- **where T : class**: Reference type
- **where T : struct**: Value type
- **where T : new()**: Parameterless constructor
- **where T : Interface**: Interface implementation
- **where T : unmanaged**: Unmanaged types (C# 7.3+)

## 💻 Kullanım
```bash
cd samples/02-Intermediate/GenericConstraints
dotnet run
```

## 🎓 Örnekler
```csharp
// Class constraint
public class Repository<T> where T : class, IEntity, new()

// Struct constraint
public class ValueContainer<T> where T : struct

// Multiple constraints
public class Manager<TEntity, TKey>
    where TEntity : class, IEntity, new()
    where TKey : struct
```

## 💡 Best Practices
- Constraints ile compile-time safety
- En spesifik constraint'i kullan
- Multiple constraints: virgül ile ayır
