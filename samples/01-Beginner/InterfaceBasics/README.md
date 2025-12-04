# Interface Basics - Interface Implementation

## 📚 Konu
Interface implementation patterns: Implicit vs Explicit implementation, çoklu interface kullanımı.

## 🎯 Amaç
Interface'lerin doğru kullanımını, implicit/explicit implementation farkını ve çoklu interface desteğini öğrenmek.

## 🔑 Anahtar Kavramlar
- **Interface**: Contract tanımlar, implementasyon içermez (C# 8+ hariç)
- **Implicit Implementation**: Public metodlar, hem interface hem class referansından çağrılabilir
- **Explicit Implementation**: Sadece interface referansından çağrılabilir
- **Multiple Interfaces**: Bir class birden fazla interface implement edebilir
- **Method Collision**: Aynı isimli metodlar explicit implementation ile çözülür
- **Interface Segregation**: SOLID prensiplerinden biri

## 💻 Kullanım

```bash
cd samples/01-Beginner/InterfaceBasics
dotnet run
```

## 📊 Örnek Çıktı

```
=== Interface Implementation Patterns ===

=== 1. Implicit Interface Implementation (SqlDatabase) ===

Class referansı üzerinden:
📡 SQL Server'a bağlanılıyor: Server=localhost;Database=MyDb
✅ SQL bağlantısı başarılı
🔍 SQL Query çalıştırılıyor: SELECT * FROM Users
✅ Query başarılı
⚙️  Stored Procedure çağrılıyor: sp_GetUsers
🔌 SQL bağlantısı kapatılıyor...
✅ Bağlantı kapatıldı

=== 2. Explicit Interface Implementation (MongoDatabase) ===

IDatabase referansı:
📡 MongoDB'ye (SQL modunda) bağlanılıyor: mongodb://localhost:27017
✅ SQL-like bağlantı başarılı
🔍 MongoDB Query (SQL syntax): db.users.find()
✅ Query başarılı

INoSqlDatabase referansı:
📡 MongoDB'ye (NoSQL modunda) bağlanılıyor: mongodb://localhost:27017
✅ NoSQL bağlantı başarılı
📝 Document ekleniyor: {name: 'Ahmet', age: 30}
✅ Document eklendi
```

## 🎓 Öğrenilen Kavramlar

### 1. Implicit Implementation (Varsayılan)
```csharp
public class SqlDatabase : IDatabase
{
    // ✅ Public metod - Her yerden çağrılabilir
    public void Connect()
    {
        Console.WriteLine("Bağlanılıyor...");
    }
}

// Kullanım
SqlDatabase sql = new();
sql.Connect();  // ✅ Çalışır

IDatabase db = sql;
db.Connect();   // ✅ Çalışır
```

### 2. Explicit Implementation (Özel Durumlar)
```csharp
public class MongoDatabase : IDatabase, INoSqlDatabase
{
    // ❌ Sadece interface referansından çağrılabilir
    void IDatabase.Connect()
    {
        Console.WriteLine("SQL modunda bağlanıyor...");
    }

    void INoSqlDatabase.Connect()
    {
        Console.WriteLine("NoSQL modunda bağlanıyor...");
    }
}

// Kullanım
MongoDatabase mongo = new();
// mongo.Connect();  // ❌ Derleme hatası!

IDatabase db = mongo;
db.Connect();  // ✅ SQL modu

INoSqlDatabase nosql = mongo;
nosql.Connect();  // ✅ NoSQL modu
```

### 3. Çoklu Interface Implementation
```csharp
public class MongoDatabase : IDatabase, INoSqlDatabase, ICacheProvider
{
    // Üç interface'i birden implement eder
}

// Kullanım - Aynı instance farklı interface'ler olarak
MongoDatabase mongo = new();

IDatabase db = mongo;           // Database olarak
ICacheProvider cache = mongo;    // Cache olarak
INoSqlDatabase nosql = mongo;    // NoSQL olarak
```

## ⚠️ Yaygın Hatalar

### ❌ Kötü: Metod Çakışması Çözülmemiş
```csharp
public class MongoDatabase : IDatabase, INoSqlDatabase
{
    // ❌ Hata! İki interface'de de Connect() var
    public void Connect()
    {
        // Hangisi? Belirsiz!
    }
}
```

### ✅ İyi: Explicit Implementation ile Çözüm
```csharp
public class MongoDatabase : IDatabase, INoSqlDatabase
{
    // ✅ İyi: Her interface için ayrı implementation
    void IDatabase.Connect() { }
    void INoSqlDatabase.Connect() { }
}
```

### ❌ Kötü: Explicit Metodu Class Referansından Çağırmak
```csharp
MongoDatabase mongo = new();
mongo.Connect();  // ❌ Derleme hatası!
```

### ✅ İyi: Interface Referansı Kullan
```csharp
IDatabase db = new MongoDatabase();
db.Connect();  // ✅ Çalışır
```

## ⚡ Performans Notları

1. **Implicit Implementation**: O(1) - Direct method call
2. **Explicit Implementation**: O(1) - Interface method table lookup
3. **Performans farkı**: Minimal (~nanosaniye seviyesi)

## 🔄 İlişkili Konular
- [PolymorphismBasics](../PolymorphismBasics/) - Virtual/override temelleri
- [AbstractClassExample](../AbstractClassExample/) - Abstract class vs interface
- [DependencyInjection](../../03-Advanced/DependencyInjection/) - DI ile interface kullanımı

## 📚 Önemli Noktalar

### Interface vs Abstract Class

| Özellik | Interface | Abstract Class |
|---------|-----------|----------------|
| **State (Fields)** | ❌ Yok | ✅ Var |
| **Constructor** | ❌ Yok | ✅ Var |
| **Multiple Inheritance** | ✅ Var | ❌ Yok (single) |
| **Access Modifiers** | ❌ Yok (public) | ✅ Var |
| **Default Implementation** | ✅ C# 8+ | ✅ Her zaman |

### Ne Zaman Implicit, Ne Zaman Explicit?

**Implicit (Varsayılan):**
- Tek interface implement ediyorsanız
- Metod çakışması yoksa
- Public erişim istiyorsanız
- **%95 durumda bu yeterlidir**

**Explicit (Özel Durumlar):**
- Metod çakışması varsa (aynı isim)
- Interface metodunu gizlemek istiyorsanız
- API'yi sadece interface üzerinden açmak istiyorsanız
- Interface segregation uygularken

## 💡 Best Practices

1. **Implicit implementation tercih edin** (varsayılan olarak)
2. **Metod çakışmasında explicit kullanın**
3. **Interface segregation principle uygulayın** (küçük, odaklanmış interface'ler)
4. **Dependency Injection için interface kullanın**
5. **Test edilebilirlik için interface'ler tanımlayın**

## 🎯 Gerçek Dünya Kullanımı

### 1. Repository Pattern
```csharp
public interface IRepository<T>
{
    void Add(T entity);
    T GetById(int id);
    void Update(T entity);
    void Delete(int id);
}

public class SqlRepository<T> : IRepository<T> { }
public class MongoRepository<T> : IRepository<T> { }
```

### 2. Dependency Injection
```csharp
// Startup.cs
services.AddScoped<IDatabase, SqlDatabase>();

// Controller
public class UserController
{
    private readonly IDatabase _database;

    public UserController(IDatabase database)
    {
        _database = database;  // Interface kullan
    }
}
```

### 3. Unit Testing
```csharp
// Mock object
public class MockDatabase : IDatabase
{
    public void Connect() { /* Test için boş */ }
    public void ExecuteQuery(string sql) { /* Test için boş */ }
}

// Test
var mockDb = new MockDatabase();
var service = new UserService(mockDb);  // Mock inject et
```
