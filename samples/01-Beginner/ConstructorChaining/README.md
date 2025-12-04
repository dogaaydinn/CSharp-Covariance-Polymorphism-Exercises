# Constructor Chaining - this() ve base()

## 📚 Konu
Constructor chaining ile kod tekrarını önleme ve initialization mantığını paylaşma.

## 🔑 Kavramlar
- **this()**: Aynı class'taki başka constructor'ı çağırır
- **base()**: Base class constructor'ını çağırır
- **Constructor Execution Order**: Base → Derived
- **Code Reuse**: Initialization mantığını paylaş

## 💻 Kullanım
```bash
cd samples/01-Beginner/ConstructorChaining
dotnet run
```

## 🎓 Örnekler
```csharp
// this() chaining
public Person() { }
public Person(string name) : this() { }
public Person(string name, int age) : this(name) { }

// base() chaining
public Employee(string name) : base(name) { }

// Multi-level
Person → Employee → Manager
```

## 💡 Best Practices
- En genel constructor'dan başla
- Ortak initialization mantığını paylaş
- Execution order'ı bil: Base → Derived
