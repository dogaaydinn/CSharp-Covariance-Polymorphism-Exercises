# Neden Type Checking?

## 🤔 Problem
Runtime'da nesnenin gerçek tipini bilmek ve type-safe işlemler yapmak.

## ❌ Kötü Yaklaşım
```csharp
// String comparison - KÖTÜ!
if (vehicle.GetType().Name == "Car") { }
```

## ✅ İyi Yaklaşım
```csharp
// Type-safe checking
if (vehicle is Car car) {
    // car kullanıma hazır
}
```

## ✨ Faydalar
1. **Type safety**: Compile-time checking
2. **Performance**: Optimized type checks
3. **Pattern matching**: Modern C# features
4. **Inheritance aware**: is operatörü inheritance'ı bilir

## 🎯 Ne Zaman Kullanmalı?
- **typeof**: Reflection, generic constraints
- **GetType()**: Runtime type inspection
- **is**: Type checking ve casting
- **Pattern matching**: Modern, okunabilir kod
