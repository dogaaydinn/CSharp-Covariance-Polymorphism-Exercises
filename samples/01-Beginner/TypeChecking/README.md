# Type Checking - typeof, GetType(), is

## 📚 Konu
Runtime ve compile-time type checking: typeof, GetType(), is operatörü.

## 🔑 Anahtar Kavramlar
- **typeof**: Compile-time type literal
- **GetType()**: Runtime type bilgisi
- **is**: Type checking (inheritance-aware)
- **==**: Exact type comparison

## 💻 Kullanım
```bash
cd samples/01-Beginner/TypeChecking
dotnet run
```

## 🎓 Örnekler

```csharp
// typeof - Compile time
Type carType = typeof(Car);

// GetType() - Runtime
Car car = new();
Type runtimeType = car.GetType();

// is - Type checking
if (vehicle is Car) { }

// Exact comparison
if (vehicle.GetType() == typeof(Car)) { }

// Pattern matching
if (vehicle is Car car) { }
```

## 💡 Best Practices
- `typeof`: Type literal'i almak için
- `GetType()`: Runtime type'ı öğrenmek için
- `is`: Type checking ve pattern matching için
- Avoid string comparison for types
