# Neden Generic Constraints?

## ❌ Kötü: Constraint Yok
```csharp
public class Repository<T> {
    public void Add(T item) {
        // item.Id = 1;  // ❌ Error! T'de Id yok
    }
}
```

## ✅ İyi: Constraint ile
```csharp
public class Repository<T> where T : IEntity {
    public void Add(T item) {
        item.Id = 1;  // ✅ IEntity garantisi
    }
}
```

## ✨ Faydalar
1. **Compile-time Safety**: Type güvenliği
2. **IntelliSense**: IDE desteği
3. **No Runtime Errors**: Derleme zamanı kontrol
4. **Better APIs**: Documented constraints

## 🎯 Ne Zaman Kullan?
- Generic class/method yazarken
- Type'ın özelliklerini kullanacaksan
- new() ile instance oluşturacaksan
