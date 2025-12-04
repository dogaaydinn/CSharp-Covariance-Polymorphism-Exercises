# Indexer Example - Custom Indexers

## 📚 Konu
Custom indexers ile array/dictionary-like erişim sağlama.

## 🔑 Kavramlar
- **Indexer**: this[index] syntax
- **Integer Indexer**: Array-like
- **String Indexer**: Dictionary-like
- **Multi-Parameter**: 2D/3D access
- **Range Indexer**: Slice operations

## 💻 Kullanım
```bash
cd samples/01-Beginner/IndexerExample
dotnet run
```

## 🎓 Örnekler
```csharp
// Integer indexer
public T this[int index] {
    get { return _items[index]; }
    set { _items[index] = value; }
}

// String indexer
public int this[string key] {
    get { return _dict[key]; }
    set { _dict[key] = value; }
}

// Multi-parameter
public T this[int row, int col] {
    get { return _data[row, col]; }
    set { _data[row, col] = value; }
}

// Range indexer
public List<T> this[Range range] {
    get { return _items[range]; }
}
```

## 💡 Best Practices
- Bounds checking ekle
- Validation yap
- Meaningful exceptions at
- Read-only için sadece get
