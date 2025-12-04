# Property Examples - Property Patterns

## 📚 Konu
Property patterns: Auto-property, validation, computed properties, init-only, required.

## 🔑 Kavramlar
- **Auto-Property**: Basit getter/setter
- **Validation**: set içinde kontrol
- **Computed Property**: Expression-bodied
- **init**: Immutable after construction
- **required**: C# 11+ zorunlu property

## 💻 Kullanım
```bash
cd samples/01-Beginner/PropertyExamples
dotnet run
```

## 🎓 Örnekler
```csharp
// Auto-property
public string Name { get; set; }

// Validation
public decimal Price {
    get => _price;
    set {
        if (value < 0) throw new ArgumentException();
        _price = value;
    }
}

// Computed
public decimal Total => Price * Quantity;

// init-only (C# 9+)
public string Category { get; init; }

// required (C# 11+)
public required string Barcode { get; init; }
```

## 💡 Best Practices
- Validation için property kullan
- Computed values için expression-bodied
- Immutable için init
- Constructor validation için required
