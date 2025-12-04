# NEDEN GENERIC CONSTRAINTS (where T : ...) KULLANIYORUZ?

## 🎯 PROBLEM TANIMI

**Gerçek Dünya Senaryosu:**

Bir fintech şirketinde backend geliştirici olarak çalışıyorsunuz. Şirket, farklı veri kaynaklarından (SQL, NoSQL, API, Cache) veri okuyan bir data access layer geliştiriyor. Her kaynak için ayrı repository class'ı yazmak yerine, generic bir `Repository<T>` yazmaya karar verdiniz.

İlk denemede şöyle bir şey yazdınız:

```csharp
// ❌ BAD: Hiçbir kısıtlama yok
public class Repository<T>
{
    public T GetById(int id)
    {
        // T'nin Id property'si var mı? BİLMİYORUZ!
        // return _dbContext.Set<T>().Find(id); // ÇALIŞMAZ!
    }

    public void Save(T entity)
    {
        // T'yi insert edebilir miyiz? BİLMİYORUZ!
        // _dbContext.Set<T>().Add(entity); // ÇALIŞMAZ!
    }
}
```

**Teknik Problem:**

**Problem 1: Compile-Time'da T'nin Ne Olduğunu Bilmiyoruz**

```csharp
// ❌ BAD: T herhangi bir şey olabilir
public class Repository<T>
{
    public void PrintId(T entity)
    {
        // ❌ HATA: T'nin Id property'si olduğunu nereden biliyoruz?
        Console.WriteLine(entity.Id); // COMPILER ERROR!
    }

    public T Create()
    {
        // ❌ HATA: T'nin parametresiz constructor'ı var mı?
        return new T(); // COMPILER ERROR!
    }

    public void Compare(T a, T b)
    {
        // ❌ HATA: T comparable mı?
        if (a > b) { } // COMPILER ERROR!
    }
}
```

**Neden kötü?**
- Compiler hiçbir şey garanti etmiyor
- Runtime'da crash olur
- IntelliSense yardımcı olamaz (T'nin member'larını göremez)

**Problem 2: Object Casting Hell**

```csharp
// ❌ BAD: Kısıtlama olmadan casting cehenneminde kalırsın
public class Repository<T>
{
    public void Save(T entity)
    {
        // T'nin IEntity olduğunu varsayıyoruz ama garanti yok!
        var e = (IEntity)entity; // RUNTIME CRASH RİSKİ!
        _dbContext.Entry(e).State = EntityState.Modified;
    }
}

// Kullanım:
var repo = new Repository<string>(); // String IEntity DEĞİL!
repo.Save("hello"); // RUNTIME CRASH! 💥
```

**Problem 3: Invalid Type Arguments**

```csharp
// ❌ BAD: Mantıksız type'lar kullanılabilir
var intRepo = new Repository<int>(); // int bir entity değil!
var delegateRepo = new Repository<Action>(); // Delegate bir entity değil!
var interfaceRepo = new Repository<IEntity>(); // Interface instantiate edilemez!

// Hepsi compile oluyor ama runtime'da patlıyor!
```

**Problem 4: Yanlış Kullanımı Engelleyemiyoruz**

```csharp
// ❌ BAD: Kullanıcı hatalı kullanım yapabiliyor
public class Stack<T>
{
    public void Push(T item) { }
    public T Pop() { }
    public T Peek() { }
}

// Sorun: Stack thread-safe değilse, birisi concurrent kullanabilir
var stack = new Stack<int>();
// Multi-threaded environment'ta race condition! 💥
```

---

## 💡 ÇÖZÜM: GENERIC CONSTRAINTS (where T : ...)

**Pattern'in Özü:**

`where` keyword'ü ile generic type parameter'ına **kısıtlamalar** koyarız. Bu sayede compile-time'da T'nin ne olduğunu ve ne yapabileceğini garanti edebiliriz.

**Nasıl çalışır:**

1. Generic type tanımlarken `where T : [constraint]` eklersin
2. Compiler, yalnızca constraint'i sağlayan type'ların kullanılmasına izin verir
3. Constraint sayesinde T'nin member'larına güvenle erişebilirsin
4. Runtime crash riski ortadan kalkar

**Ne zaman kullanılır:**

- Generic class/method içinde T'nin **belirli özellikleri** olmasını garanti etmek istiyorsanız
- T'nin **belirli member'larına** erişmeniz gerekiyorsa
- T'nin **instantiate** edilebilir olması gerekiyorsa (`new()`)
- T'nin **reference type veya value type** olmasını garanti etmek istiyorsanız
- T'nin **belirli interface'leri** implement etmesini zorunlu kılmak istiyorsanız

---

## 📝 BU REPO'DAKİ IMPLEMENTASYON

### Constraint Türleri

```csharp
// samples/02-Intermediate/GenericConstraints/Examples.cs

// ============================================
// 1. CLASS CONSTRAINT: T bir reference type olmalı
// ============================================
public class Repository<T> where T : class
{
    // ✅ T kesinlikle class (not struct, not primitive)
    public void Save(T entity)
    {
        if (entity == null) // ✅ null check yapabiliriz
        {
            throw new ArgumentNullException();
        }
        // Safe to use as reference type
    }
}

// ✅ Kullanım:
var userRepo = new Repository<User>(); // OK: User bir class
// ❌ var intRepo = new Repository<int>(); // COMPILER ERROR! int value type

// ============================================
// 2. STRUCT CONSTRAINT: T bir value type olmalı
// ============================================
public class NumericCalculator<T> where T : struct
{
    // ✅ T kesinlikle value type (int, double, DateTime, etc.)
    public T Default => default(T); // Always safe, never null

    public bool IsZero(T value)
    {
        // ✅ Value type'lar default comparison yapabilir
        return value.Equals(default(T));
    }
}

// ✅ Kullanım:
var calc = new NumericCalculator<int>(); // OK
// ❌ var calc2 = new NumericCalculator<string>(); // ERROR! string reference type

// ============================================
// 3. NEW() CONSTRAINT: T parametresiz constructor'a sahip olmalı
// ============================================
public class Factory<T> where T : new()
{
    // ✅ T'yi instantiate edebiliriz!
    public T Create()
    {
        return new T(); // ✅ Compile oluyor!
    }

    public List<T> CreateMany(int count)
    {
        var list = new List<T>();
        for (int i = 0; i < count; i++)
        {
            list.Add(new T()); // ✅ Güvenli!
        }
        return list;
    }
}

// ✅ Kullanım:
public class User
{
    public User() { } // ✅ Parametresiz constructor var
}

var factory = new Factory<User>();
var user = factory.Create(); // ✅ Works!

// ❌ Hatalı kullanım:
public class Product
{
    public Product(string name) { } // ❌ Parametresiz constructor YOK!
}
// var factory2 = new Factory<Product>(); // COMPILER ERROR!

// ============================================
// 4. INTERFACE CONSTRAINT: T belirli interface'i implement etmeli
// ============================================
public interface IEntity
{
    int Id { get; set; }
    DateTime CreatedAt { get; set; }
}

public class Repository<T> where T : IEntity
{
    // ✅ T'nin Id property'si olduğunu garantileyebiliriz!
    public T GetById(int id)
    {
        return _dbContext.Set<T>()
            .FirstOrDefault(e => e.Id == id); // ✅ e.Id compile oluyor!
    }

    public void PrintCreationDate(T entity)
    {
        // ✅ entity.CreatedAt erişimi güvenli
        Console.WriteLine($"Created: {entity.CreatedAt}");
    }
}

// ✅ Kullanım:
public class User : IEntity
{
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public string Name { get; set; }
}

var repo = new Repository<User>(); // ✅ User implements IEntity
// ❌ var repo2 = new Repository<string>(); // ERROR! string doesn't implement IEntity

// ============================================
// 5. BASE CLASS CONSTRAINT: T belirli class'tan türemeli
// ============================================
public abstract class Entity
{
    public int Id { get; set; }
    public virtual void Validate() { }
}

public class Repository<T> where T : Entity
{
    // ✅ T kesinlikle Entity veya türevi
    public void Save(T entity)
    {
        entity.Validate(); // ✅ Entity'nin metodunu çağırabiliriz
        _dbContext.Set<T>().Add(entity);
    }
}

// ✅ Kullanım:
public class User : Entity { }
var repo = new Repository<User>(); // OK

// ❌ Hatalı:
public class Settings { } // Entity'den türemiyor
// var repo2 = new Repository<Settings>(); // COMPILER ERROR!

// ============================================
// 6. MULTIPLE CONSTRAINTS: Birden fazla kısıtlama
// ============================================
public class AdvancedRepository<T>
    where T : Entity, IValidatable, new()
{
    // ✅ T hem Entity'den türemeli
    // ✅ T hem IValidatable implement etmeli
    // ✅ T hem parametresiz constructor'a sahip olmalı

    public T CreateAndValidate()
    {
        var entity = new T(); // ✅ new() constraint
        entity.Validate(); // ✅ Entity base class
        entity.IsValid(); // ✅ IValidatable interface
        return entity;
    }
}

// ============================================
// 7. UNMANAGED CONSTRAINT: T unmanaged type olmalı (C# 7.3+)
// ============================================
public class HighPerformanceBuffer<T> where T : unmanaged
{
    // ✅ T kesinlikle unmanaged (pointer kullanılabilir)
    private unsafe T* _buffer;

    public unsafe void AllocateBuffer(int size)
    {
        _buffer = (T*)Marshal.AllocHGlobal(size * sizeof(T));
    }
}

// ✅ Kullanım: int, double, bool, struct (no reference fields)
var buffer = new HighPerformanceBuffer<int>(); // OK
// ❌ var buffer2 = new HighPerformanceBuffer<string>(); // ERROR! string is managed

// ============================================
// 8. GENERIC METHOD CONSTRAINTS
// ============================================
public class DataProcessor
{
    // ✅ Sadece bu metod için constraint
    public T Process<T>(T data) where T : ISerializable
    {
        // T kesinlikle ISerializable
        var serialized = Serialize(data);
        return Deserialize<T>(serialized);
    }

    // ✅ Farklı metod, farklı constraint
    public void Print<T>(T item) where T : struct
    {
        Console.WriteLine(item);
    }
}
```

---

## 📚 ADIM ADIM NASIL UYGULANIR

### Adım 1: Generic Type'ın Neye İhtiyacı Olduğunu Belirle

```csharp
// SORU: Bu generic class içinde T ile ne yapacaksın?

public class MyClass<T>
{
    public void Method1(T item)
    {
        // item.Id diyecek misin? → IEntity constraint gerekir
        // new T() yapacak mısın? → new() constraint gerekir
        // item == null check yapacak mısın? → class constraint gerekir
    }
}
```

### Adım 2: Uygun Constraint'i Ekle

```csharp
// Örnek 1: Id property'sine ihtiyaç var
public interface IEntity
{
    int Id { get; set; }
}

public class Repository<T> where T : IEntity
{
    // Artık T.Id kullanabiliriz
}

// Örnek 2: Instantiate etmemiz gerekiyor
public class Factory<T> where T : new()
{
    public T Create() => new T();
}

// Örnek 3: Hem interface hem new() gerekiyor
public class SmartFactory<T>
    where T : IEntity, new()
{
    public T CreateWithId(int id)
    {
        var entity = new T();
        entity.Id = id;
        return entity;
    }
}
```

### Adım 3: Multiple Constraints'i Doğru Sırada Yaz

```csharp
// ✅ DOĞRU SIRA:
// 1. class veya struct (ilk olmalı)
// 2. Base class (varsa)
// 3. Interface'ler (istediğin kadar)
// 4. new() (son olmalı)

public class MyClass<T>
    where T : class,        // 1. Reference type
              Entity,       // 2. Base class
              IValidatable, // 3. Interface 1
              ISerializable,// 4. Interface 2
              new()         // 5. Constructor (en son!)
{
    // İmplementasyon
}
```

### Adım 4: Hatalı Kullanımı Test Et

```csharp
// Constraint'leri test et
public class Repository<T> where T : class, IEntity, new()
{
    // Implementation
}

// ✅ OK:
public class User : IEntity
{
    public int Id { get; set; }
    public User() { }
}
var repo = new Repository<User>();

// ❌ Hatalar:
// var repo1 = new Repository<int>(); // ERROR: int is not class
// var repo2 = new Repository<string>(); // ERROR: string doesn't implement IEntity
public class Product : IEntity
{
    public int Id { get; set; }
    public Product(string name) { } // ❌ Parametresiz constructor yok!
}
// var repo3 = new Repository<Product>(); // ERROR: no parameterless constructor
```

---

## ⚖️ TRADE-OFF ANALİZİ

### ✅ Avantajları

**✅ Compile-Time Type Safety**
- **Neden avantaj?** Runtime crash'ler yerine compiler hataları
- **Örnek:** `Repository<string>` yazarsan compiler "string IEntity değil" der, runtime'da crash olmaz
- **Ölçülebilir etki:** Production bug'ları %50+ azalır

**✅ IntelliSense Desteği**
- **Neden avantaj?** IDE, T'nin member'larını gösterebilir
- **Örnek:**
```csharp
public void Method<T>(T entity) where T : IEntity
{
    entity. // ← IntelliSense "Id", "CreatedAt" gösterir!
}
```

**✅ Refactoring Kolaylığı**
- **Hangi durumda kritik?** Interface değişirse, compiler tüm kullanımları işaretler
- **Performance etkisi:** Yok! Constraints compile-time'da kontrol edilir

**✅ API Kötüye Kullanımını Engeller**
- **Neden avantaj?** Kullanıcılar yanlış type'lar kullanamaz
```csharp
// ❌ Bu mümkün değil artık:
var repo = new Repository<int>(); // Compiler error!
```

---

### ❌ Dezavantajları

**❌ Flexibility Kaybı**
- **Ne zaman problem olur?** Çok katı constraints, valid kullanımları engelleyebilir
```csharp
// ❌ Çok katı:
public class Processor<T>
    where T : class, ISerializable, IComparable, ICloneable, new()
{
    // Az sayıda type bu kadar constraint'i sağlar!
}
```

**❌ Complexity Artışı**
- **Ne zaman problem olur?** Multiple constraints kodu okumayı zorlaştırır
```csharp
// ❌ Karmaşık:
public class Service<TEntity, TDto, TValidator>
    where TEntity : Entity, IValidatable, new()
    where TDto : class, IMapFrom<TEntity>
    where TValidator : IValidator<TDto>, new()
{
    // Kafalar karışık!
}
```

**❌ Öğrenme Eğrisi**
- **Ne zaman problem olur?** Junior developer'lar constraint syntax'ını öğrenmeli
- **Çözüm:** Basit örneklerle başla, karmaşık constraint'leri dokümante et

---

## 🚫 NE ZAMAN KULLANMAMALISIN?

### Senaryo 1: T ile Hiçbir İşlem Yapmıyorsan

```csharp
// ❌ GEREKSIZ: T'yi sadece tutuyoruz, hiçbir member'ına erişmiyoruz
public class Wrapper<T> where T : class
{
    public T Value { get; set; }
}

// ✅ DAHA İYİ: Constraint gereksiz
public class Wrapper<T>
{
    public T Value { get; set; }
}
```

### Senaryo 2: Object ile Çözülebiliyorsa

```csharp
// ❌ OVERKILL: Sadece object olarak tutacaksak
public class Container<T>
{
    private T _value;
    public object GetAsObject() => _value;
}

// ✅ DAHA İYİ: Direkt object kullan
public class Container
{
    private object _value;
    public object GetValue() => _value;
}
```

### Senaryo 3: Çok Fazla Constraint

```csharp
// ❌ OVERKILL: 5+ constraint kullanma ihtiyacı varsa design smell'i
public class Service<T>
    where T : Entity, IValidatable, ISerializable, IComparable, ICloneable, new()
{
    // Alternatif: Composition, birden fazla generic type, veya non-generic
}
```

---

## 🔄 ALTERNATİF PATTERN'LER

### Alternatif 1: Non-Generic + Inheritance

**Ne zaman tercih edilir?**
- Type sayısı az ve sabit (2-3 tip)
- Her tip için farklı logic gerekiyor

**Farkı:**
```csharp
// Generic + Constraint:
public class Repository<T> where T : IEntity
{
    public void Save(T entity) { /* generic implementation */ }
}

// Non-Generic + Inheritance:
public abstract class RepositoryBase
{
    public abstract void Save(IEntity entity);
}
public class UserRepository : RepositoryBase
{
    public override void Save(IEntity entity)
    {
        // User-specific logic
    }
}
```

### Alternatif 2: Runtime Type Checking

**Ne zaman tercih edilir?**
- Prototype aşamasında
- Type mix'i runtime'da belirleniyor

**Farkı:**
```csharp
// Generic + Constraint:
public class Processor<T> where T : ISerializable
{
    public void Process(T item) { item.Serialize(); }
}

// Runtime Checking:
public class Processor
{
    public void Process(object item)
    {
        if (item is ISerializable serializable)
        {
            serializable.Serialize();
        }
        else
        {
            throw new ArgumentException("Must be ISerializable");
        }
    }
}
```

### Alternatif 3: Marker Interfaces (Empty Interfaces)

**Ne zaman tercih edilir?**
- Sadece type marking gerekiyor, member'lar önemli değil

**Farkı:**
```csharp
// Constraint with members:
public interface IEntity
{
    int Id { get; set; }
}
public class Repository<T> where T : IEntity { }

// Marker interface:
public interface IEntity { } // Empty!
public class Repository<T> where T : IEntity
{
    // T'nin member'larına erişemezsin, sadece type check
}
```

---

## 📊 KARAR MATRİSİ

| Kriter | Generic + Constraint | Non-Generic | Runtime Check | Marker Interface |
|--------|---------------------|-------------|---------------|------------------|
| **Type Safety** | ⭐⭐⭐⭐⭐ | ⭐⭐⭐⭐☆ | ⭐⭐☆☆☆ | ⭐⭐⭐⭐☆ |
| **Performance** | ⭐⭐⭐⭐⭐ | ⭐⭐⭐⭐☆ | ⭐⭐⭐☆☆ | ⭐⭐⭐⭐⭐ |
| **Flexibility** | ⭐⭐⭐☆☆ | ⭐⭐⭐⭐☆ | ⭐⭐⭐⭐⭐ | ⭐⭐⭐☆☆ |
| **Code Reuse** | ⭐⭐⭐⭐⭐ | ⭐⭐⭐☆☆ | ⭐⭐⭐⭐☆ | ⭐⭐⭐⭐⭐ |
| **Simplicity** | ⭐⭐⭐☆☆ | ⭐⭐⭐⭐☆ | ⭐⭐⭐☆☆ | ⭐⭐⭐⭐☆ |

---

## 🎯 GERÇEK DÜNYA ÖRNEKLERİ

### Örnek 1: Entity Framework Core

```csharp
// ✅ EF Core DbSet constraint kullanır
public class DbSet<TEntity> where TEntity : class
{
    public void Add(TEntity entity) { }
    public void Remove(TEntity entity) { }
}

// Neden class constraint? Çünkü:
// - Entity'ler reference type olmalı
// - Value type'lar track edilemez
```

### Örnek 2: ASP.NET Core Dependency Injection

```csharp
// ✅ DI container constraint kullanır
public interface IServiceCollection
{
    void AddScoped<TService, TImplementation>()
        where TService : class
        where TImplementation : class, TService;
}

// Kullanım:
services.AddScoped<IUserService, UserService>();
// ✅ Her ikisi de class
// ✅ UserService implements IUserService
```

### Örnek 3: LINQ OrderBy

```csharp
// ✅ LINQ, IComparable constraint kullanır
public static IOrderedEnumerable<TSource> OrderBy<TSource, TKey>(
    this IEnumerable<TSource> source,
    Func<TSource, TKey> keySelector)
    where TKey : IComparable<TKey>
{
    // TKey comparable olmalı ki sıralayabilelim
}
```

---

## 💼 KARİYER ETKİSİ

**Bu pattern'i bilmek sizi nereye götürür?**

### Junior Developer (0-2 yıl)
- **Görev:** Var olan generic class'ları kullanmak
- **Mülakat:** "where T : class ne demektir?"
- **Maaş etkisi:** Generic anlayışı → $65-85K

### Mid-Level Developer (2-5 yıl)
- **Görev:** Generic class'lar tasarlamak, doğru constraint'leri seçmek
- **Mülakat:** "Ne zaman new() constraint kullanırsınız?"
- **Maaş etkisi:** Advanced generics → $85-130K

### Senior Developer (5+ yıl)
- **Görev:** Framework design, complex generic libraries
- **Mülakat:** "Covariance/contravariance ile constraint ilişkisi?"
- **Maaş etkisi:** Generic library design → $130-190K+

---

## 📚 SONRAKI ADIMLAR

**Bu pattern'i öğrendikten sonra:**

1. **İlgili Konular:**
   - `samples/02-Intermediate/CovarianceContravariance/` → `in` ve `out` ile constraint ilişkisi
   - `samples/03-Advanced/HighPerformance/` → `unmanaged` constraint ile performans optimizasyonu

2. **Pratik:**
```bash
cd samples/02-Intermediate/GenericConstraints
dotnet run
# Farklı constraint'leri deneyin
```

3. **Egzersiz:**
   - Kendi generic Repository<T> class'ınızı yazın
   - IEntity, IValidatable interface'lerini ekleyin
   - Constraint violation durumlarını test edin

---

**Özet:** Generic constraints, compile-time'da type safety sağlar. Runtime crash'leri engeller, IntelliSense desteği verir ve API kötüye kullanımını önler. Fazla kullanılmamalı (gereksiz complexity), ama orta-büyük projelerde generic code yazarken vazgeçilmezdir. 🚀
