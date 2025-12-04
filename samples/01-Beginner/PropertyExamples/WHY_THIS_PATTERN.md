# Neden Properties?

## ❌ Kötü: Public Fields
```csharp
public class Product {
    public decimal price;  // Validation yok!
    public int stock;      // Negatif olabilir!
}
```

## ✅ İyi: Properties
```csharp
public class Product {
    private decimal _price;
    public decimal Price {
        get => _price;
        set {
            if (value < 0) throw new ArgumentException();
            _price = value;
        }
    }
}
```

## ✨ Faydalar
1. **Encapsulation**: Internal state gizli
2. **Validation**: set'te kontrol
3. **Computed Values**: Lazy calculation
4. **Backward Compatibility**: Field → property dönüşümü
5. **Side Effects**: Logging, events

## 🎯 Modern Patterns
- **Auto-property**: Basit durumlar
- **init**: Immutability
- **required**: Constructor validation
- **Computed**: Derived values
