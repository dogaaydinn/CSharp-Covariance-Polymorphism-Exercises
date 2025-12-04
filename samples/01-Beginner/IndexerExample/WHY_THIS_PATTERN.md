# Neden Indexers?

## ❌ Kötü: Method Syntax
```csharp
var item = list.GetItem(5);
list.SetItem(5, newValue);
// Verbose ve unintuitive
```

## ✅ İyi: Indexer Syntax
```csharp
var item = list[5];
list[5] = newValue;
// Array-like, natural syntax
```

## ✨ Faydalar
1. **Natural Syntax**: Array/dictionary-like
2. **Encapsulation**: Internal structure gizli
3. **Validation**: Bounds checking
4. **Multiple Types**: int, string, Range, custom
5. **Multi-dimensional**: [row, col]

## 🎯 Ne Zaman Kullan?
- Collection-like class'lar
- Dictionary-like access
- Matrix/Grid structures
- Custom data structures
