# Neden Abstract Class vs Interface?

## 🤔 Problem: Ortak Davranış ve Contract

### ❌ Kötü Yaklaşım: Her Şey için Interface

```csharp
// Kötü: Interface ile ortak davranış - KOD TEKRARI!
public interface IShape
{
    string Color { get; set; }  // Her class tekrar implement eder
    void DisplayInfo();         // Her class tekrar implement eder
    double CalculateArea();
}

public class Circle : IShape
{
    public string Color { get; set; }  // Tekrar!

    public void DisplayInfo()  // Tekrar!
    {
        Console.WriteLine($"Color: {Color}, Area: {CalculateArea()}");
    }

    public double CalculateArea() { return Math.PI * Radius * Radius; }
}

public class Rectangle : IShape
{
    public string Color { get; set; }  // Tekrar!

    public void DisplayInfo()  // Tekrar! (Aynı kod)
    {
        Console.WriteLine($"Color: {Color}, Area: {CalculateArea()}");
    }

    public double CalculateArea() { return Width * Height; }
}
```

**Problemler:**
1. 💥 Kod tekrarı - Her class aynı kodu yazar
2. 🐛 Bakım zorluğu - DisplayInfo değişirse her yerde değiştir
3. 📦 State paylaşılamaz - Her class kendi field'ını tutar
4. 🔧 Constructor logic paylaşılamaz

### ✅ İyi Yaklaşım: Abstract Class + Interface

```csharp
// İyi: Abstract class ile ortak davranış
public abstract class Shape
{
    // Ortak state - Tek yerde tanımlanır
    public string Color { get; set; }

    // Constructor - Ortak initialization
    protected Shape(string color)
    {
        Color = color;
    }

    // Abstract - Alt sınıflar implement eder
    public abstract double CalculateArea();

    // Concrete - Ortak davranış, tek yerde tanımlanır
    public void DisplayInfo()
    {
        Console.WriteLine($"Color: {Color}, Area: {CalculateArea():F2}");
    }
}

// Interface - Contract
public interface IDrawable
{
    void Draw();
    void Erase();
}

// Circle - Abstract class + Interface
public class Circle : Shape, IDrawable
{
    public double Radius { get; set; }

    public Circle(double radius, string color) : base(color)
    {
        Radius = radius;
    }

    // Abstract metod
    public override double CalculateArea()
    {
        return Math.PI * Radius * Radius;
    }

    // Interface metod
    public void Draw() { Console.WriteLine("Drawing circle"); }
    public void Erase() { Console.WriteLine("Erasing circle"); }
}
```

## ✨ Abstract Class'ın Faydaları

### 1. Ortak State (Fields)

```csharp
public abstract class Shape
{
    private static int _shapeCount = 0;  // Static field
    public int Id { get; private set; }  // Instance field
    public string Color { get; set; }    // Property

    protected Shape(string color)
    {
        Id = ++_shapeCount;
        Color = color;
    }
}

// Her şekil otomatik ID alır, Color'u paylaşır
Circle circle = new(5.0, "Red");
Console.WriteLine(circle.Id);     // 1
Console.WriteLine(circle.Color);  // Red
```

### 2. Ortak Behavior (Concrete Methods)

```csharp
public abstract class Shape
{
    // Concrete method - Tüm şekiller için aynı
    public void DisplayInfo()
    {
        Console.WriteLine($"{GetType().Name}: {Color}, Area: {CalculateArea():F2}");
    }

    public abstract double CalculateArea();
}

// Tüm şekiller DisplayInfo'yu kullanır
Circle circle = new(5.0, "Red");
circle.DisplayInfo();  // Circle: Red, Area: 78.54

Rectangle rect = new(4, 6, "Blue");
rect.DisplayInfo();    // Rectangle: Blue, Area: 24.00
```

### 3. Constructor Chaining

```csharp
public abstract class Shape
{
    protected Shape()
    {
        Console.WriteLine("Shape constructor");
    }

    protected Shape(string color) : this()
    {
        Color = color;
        Console.WriteLine($"Color set: {color}");
    }
}

// Alt sınıflar base constructor'ı çağırır
public class Circle : Shape
{
    public Circle(double radius, string color) : base(color)
    {
        Radius = radius;
    }
}
```

## ✨ Interface'in Faydaları

### 1. Multiple Inheritance

```csharp
// Abstract class: Single inheritance
public class Circle : Shape { }  // ✅ OK
public class Circle : Shape, AnotherClass { }  // ❌ Hata!

// Interface: Multiple inheritance
public class Circle : Shape, IDrawable, IMeasurable, ISerializable { }  // ✅ OK
```

### 2. Farklı Hierarchy'ler Arası Contract

```csharp
public interface IDrawable
{
    void Draw();
}

// Farklı hierarchy'lerdeki class'lar aynı interface'i implement eder
public class Circle : Shape, IDrawable { }      // Shape hierarchy
public class Button : UIControl, IDrawable { }  // UIControl hierarchy
public class Icon : Asset, IDrawable { }        // Asset hierarchy

// Polymorphic kullanım
List<IDrawable> drawables = new()
{
    new Circle(5.0, "Red"),
    new Button("OK"),
    new Icon("save.png")
};

foreach (var drawable in drawables)
{
    drawable.Draw();  // Hepsi IDrawable
}
```

### 3. Dependency Injection

```csharp
// Interface - Contract
public interface IRepository
{
    void Save(object entity);
    object Load(int id);
}

// Service - Interface'e bağımlı
public class UserService
{
    private readonly IRepository _repository;

    public UserService(IRepository repository)
    {
        _repository = repository;  // Interface injection
    }
}

// Farklı implementasyonlar
public class SqlRepository : IRepository { }
public class MongoRepository : IRepository { }
public class MemoryRepository : IRepository { }

// Runtime'da değiştirilebilir
var service1 = new UserService(new SqlRepository());
var service2 = new UserService(new MongoRepository());
```

## 🏗️ Gerçek Dünya Örnekleri

### 1. .NET Framework - Stream Classes

```csharp
// Abstract class - Ortak davranış
public abstract class Stream
{
    public abstract int Read(byte[] buffer, int offset, int count);
    public abstract void Write(byte[] buffer, int offset, int count);

    // Concrete helper methods
    public void CopyTo(Stream destination) { }
    public Task CopyToAsync(Stream destination) { }
}

// Concrete classes
public class FileStream : Stream { }
public class MemoryStream : Stream { }
public class NetworkStream : Stream { }
```

### 2. ASP.NET Core - Controller Base

```csharp
// Abstract class - Ortak davranış
public abstract class ControllerBase
{
    // Properties
    public HttpContext HttpContext { get; }
    public ModelStateDictionary ModelState { get; }

    // Helper methods
    public OkResult Ok() { }
    public BadRequestResult BadRequest() { }
}

// Your controllers
public class UserController : ControllerBase
{
    public IActionResult GetUser(int id)
    {
        return Ok(user);  // Base class method
    }
}
```

### 3. Repository Pattern

```csharp
// Abstract class - Ortak CRUD
public abstract class Repository<T>
{
    protected DbContext Context { get; }

    protected Repository(DbContext context)
    {
        Context = context;
    }

    // Concrete methods
    public virtual void Add(T entity)
    {
        Context.Set<T>().Add(entity);
        Context.SaveChanges();
    }

    // Abstract methods
    public abstract T GetById(int id);
}

// Interface - Contract
public interface IUserRepository
{
    User GetByEmail(string email);
    List<User> GetActiveUsers();
}

// Implementation
public class UserRepository : Repository<User>, IUserRepository
{
    public UserRepository(DbContext context) : base(context) { }

    public override User GetById(int id)
    {
        return Context.Users.Find(id);
    }

    public User GetByEmail(string email)
    {
        return Context.Users.FirstOrDefault(u => u.Email == email);
    }

    public List<User> GetActiveUsers()
    {
        return Context.Users.Where(u => u.IsActive).ToList();
    }
}
```

## 📊 Karar Ağacı

```
Ortak davranış paylaşılacak mı?
├─ EVET → Abstract Class kullan
│   ├─ Constructor logic var mı?
│   │   └─ EVET → Abstract Class
│   ├─ Fields (state) var mı?
│   │   └─ EVET → Abstract Class
│   └─ Concrete methods var mı?
│       └─ EVET → Abstract Class
│
└─ HAYIR → Interface kullan
    ├─ Multiple inheritance gerekli mi?
    │   └─ EVET → Interface
    ├─ Sadece contract mi?
    │   └─ EVET → Interface
    └─ Farklı hierarchy'ler mi?
        └─ EVET → Interface

ÖNERİ: İkisini birlikte kullan!
└─ Abstract class: Ortak state/behavior
└─ Interface: Contract ve multiple inheritance
```

## 💡 Best Practices

### 1. IS-A vs CAN-DO Testi

```csharp
// IS-A → Abstract class
"Circle IS-A Shape" → ✅ Abstract class

// CAN-DO → Interface
"Shape CAN-BE drawn" → ✅ Interface
```

### 2. Liskov Substitution Test

```csharp
// Abstract class - Alt sınıf, üst sınıf yerine kullanılabilir mi?
Shape shape = new Circle(5.0, "Red");
shape.DisplayInfo();  // ✅ Çalışır

// Interface - Class, interface contract'ını sağlıyor mu?
IDrawable drawable = new Circle(5.0, "Red");
drawable.Draw();  // ✅ Çalışır
```

### 3. İkisini Birlikte Kullan

```csharp
// ✅ EN İYİ YAKLAŞIM
public abstract class Shape  // Ortak state/behavior
{
    public string Color { get; set; }
    public abstract double CalculateArea();
    public void DisplayInfo() { }
}

public interface IDrawable  // Contract
{
    void Draw();
    void Erase();
}

public interface IMeasurable  // Contract
{
    double CalculateArea();
    double CalculatePerimeter();
}

// Circle - Hem abstract class hem interface'leri kullanır
public class Circle : Shape, IDrawable, IMeasurable
{
    // En esnek ve güçlü yaklaşım!
}
```

## 🎯 Özet

**Abstract Class Kullan:**
1. Ortak state (fields) gerekiyorsa
2. Ortak behavior (concrete methods) gerekiyorsa
3. Constructor logic paylaşılacaksa
4. IS-A ilişkisi varsa

**Interface Kullan:**
1. Sadece contract tanımlanacaksa
2. Multiple inheritance gerekiyorsa
3. Farklı hierarchy'ler arası ortak davranış
4. CAN-DO ilişkisi varsa
5. Dependency Injection için

**İkisini Birlikte Kullan:**
- Abstract class: Ortak state/behavior
- Interface: Contract ve capability
- En esnek ve güçlü yaklaşım!

> **Kural:** Abstract class ortak davranış için, interface contract için. İkisini birlikte kullanarak en güçlü ve esnek tasarımı elde edersiniz. 🏗️
