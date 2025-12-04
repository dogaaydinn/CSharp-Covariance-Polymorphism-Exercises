# Sample Projects Specification - 18 Proje Detayları

## ✅ Tamamlanan Projeler (2/18)

### 1. PolymorphismBasics ✅
- **Konu**: Virtual/override, base class
- **Dosyalar**: Animal.cs, Zoo.cs, Program.cs, README.md, WHY_THIS_PATTERN.md, .csproj
- **Satır Sayısı**: ~250 satır (toplam)
- **Durum**: ✅ Tamamlandı

### 2. CastingExamples ✅
- **Konu**: as, is, pattern matching
- **Dosyalar**: Employee.cs, Program.cs, README.md, WHY_THIS_PATTERN.md, .csproj
- **Satır Sayısı**: ~240 satır (toplam)
- **Durum**: ✅ Tamamlandı

---

## 📋 Kalan Beginner Projeleri (8/10)

### 3. OverrideVirtual
**Konu**: Method hiding (new) vs override farkı

**Scenario**: Banka hesap sistemi
```csharp
// Account.cs
public class Account
{
    public virtual decimal CalculateInterest() { }
}

public class SavingsAccount : Account
{
    public override decimal CalculateInterest()  // Override
    {
        return Balance * 0.05m;  // %5 faiz
    }
}

public class CheckingAccount : Account
{
    public new decimal CalculateInterest()  // Method hiding
    {
        return Balance * 0.01m;  // %1 faiz
    }
}
```

**Program.cs Senaryosu**:
```csharp
// SCENARIO: Override vs new keyword farkını göster
// BAD PRACTICE: new keyword ile method hiding - polymorphic davranış bozulur
// GOOD PRACTICE: override kullanarak polymorphic davranış koru

Account savings = new SavingsAccount();
savings.CalculateInterest();  // Override çalışır ✅

Account checking = new CheckingAccount();
checking.CalculateInterest();  // Base metod çalışır! ❌ (new kullanıldı)
```

**WHY_THIS_PATTERN.md Noktaları**:
- Method hiding ile polymorphism bozulur
- Override her zaman polymorphic davranış sağlar
- Compiler warning: CS0114 (method hides inherited member)

---

### 4. InterfaceBasics
**Konu**: Interface implementation, explicit interface

**Scenario**: Veritabanı sağlayıcıları
```csharp
// IDatabase.cs
public interface IDatabase
{
    void Connect();
    void ExecuteQuery(string sql);
    void Disconnect();
}

public interface INoSqlDatabase
{
    void Connect();  // Aynı isim!
    void InsertDocument(string json);
}

// SqlDatabase.cs
public class SqlDatabase : IDatabase
{
    public void Connect() { }
    public void ExecuteQuery(string sql) { }
    public void Disconnect() { }
}

// MongoDatabase.cs - Explicit implementation
public class MongoDatabase : IDatabase, INoSqlDatabase
{
    // Explicit implementation - çakışmayı önler
    void IDatabase.Connect()
    {
        Console.WriteLine("SQL bağlantısı");
    }

    void INoSqlDatabase.Connect()
    {
        Console.WriteLine("NoSQL bağlantısı");
    }

    public void ExecuteQuery(string sql) { }
    public void InsertDocument(string json) { }
    public void Disconnect() { }
}
```

**Program.cs**: Explicit interface kullanımını göster
```csharp
MongoDatabase mongo = new();
// mongo.Connect();  // Hata! Explicit interface

IDatabase db = mongo;
db.Connect();  // SQL bağlantısı

INoSqlDatabase nosql = mongo;
nosql.Connect();  // NoSQL bağlantısı
```

---

### 5. AbstractClassExample
**Konu**: Abstract class vs interface farkı

**Scenario**: Shape hierarchy
```csharp
// Shape.cs
public abstract class Shape
{
    public string Color { get; set; }  // Ortak özellik

    // Abstract metod - türetilmiş sınıf implement etmeli
    public abstract double CalculateArea();

    // Virtual metod - override edilebilir
    public virtual void Draw()
    {
        Console.WriteLine($"Çiziliyor: {GetType().Name}, Renk: {Color}");
    }

    // Concrete metod - tüm şekiller için ortak
    public void DisplayInfo()
    {
        Console.WriteLine($"Alan: {CalculateArea():F2}");
    }
}

// Circle.cs
public class Circle : Shape
{
    public double Radius { get; set; }

    public override double CalculateArea()
    {
        return Math.PI * Radius * Radius;
    }

    public override void Draw()
    {
        Console.WriteLine($"🔵 Daire çiziliyor (r={Radius})");
    }
}

// Rectangle.cs
public class Rectangle : Shape
{
    public double Width { get; set; }
    public double Height { get; set; }

    public override double CalculateArea()
    {
        return Width * Height;
    }

    public override void Draw()
    {
        Console.WriteLine($"🟦 Dikdörtgen çiziliyor ({Width}x{Height})");
    }
}
```

**WHY_THIS_PATTERN.md**:
- Abstract class: State (fields) + behavior (methods)
- Interface: Sadece contract (C# 8+ default implementation)
- Abstract class: Single inheritance
- Interface: Multiple inheritance

---

### 6. TypeChecking
**Konu**: GetType(), typeof, is operatörleri

**Scenario**: Araç türü tespiti
```csharp
// Vehicle.cs
public abstract class Vehicle
{
    public string Brand { get; set; }
}

public class Car : Vehicle { public int Doors { get; set; } }
public class Truck : Vehicle { public double LoadCapacity { get; set; } }
public class Motorcycle : Vehicle { public int EngineCC { get; set; } }

// Program.cs
static void AnalyzeVehicle(Vehicle vehicle)
{
    // 1. typeof - Compile-time type checking
    Type carType = typeof(Car);
    Console.WriteLine($"Car type: {carType.Name}");

    // 2. GetType() - Runtime type checking
    Type runtimeType = vehicle.GetType();
    Console.WriteLine($"Runtime type: {runtimeType.Name}");

    // 3. is operatörü - Type checking
    if (vehicle is Car)
        Console.WriteLine("Bu bir araba");

    // 4. Type comparison
    if (vehicle.GetType() == typeof(Car))  // Exact type
        Console.WriteLine("Tam olarak Car tipi");

    if (vehicle is Car car)  // Pattern matching
        Console.WriteLine($"Kapı sayısı: {car.Doors}");
}
```

---

### 7. MethodOverloading
**Konu**: Parametre sayısı/türüne göre overload

**Scenario**: Calculator class
```csharp
// Calculator.cs
public class Calculator
{
    // 1. Parametre sayısına göre overload
    public int Add(int a, int b)
    {
        return a + b;
    }

    public int Add(int a, int b, int c)
    {
        return a + b + c;
    }

    // 2. Parametre türüne göre overload
    public double Add(double a, double b)
    {
        return a + b;
    }

    // 3. Params keyword
    public int Add(params int[] numbers)
    {
        return numbers.Sum();
    }

    // 4. Optional parameters (C# 4.0+)
    public int Multiply(int a, int b = 1, int c = 1)
    {
        return a * b * c;
    }

    // 5. Named arguments
    public double Calculate(double value, double rate, int years)
    {
        return value * Math.Pow(1 + rate, years);
    }
}

// Program.cs
Calculator calc = new();

// Overload resolution
calc.Add(1, 2);           // int Add(int, int)
calc.Add(1, 2, 3);        // int Add(int, int, int)
calc.Add(1.5, 2.5);       // double Add(double, double)
calc.Add(1, 2, 3, 4, 5);  // int Add(params int[])

// Optional parameters
calc.Multiply(5);          // 5 * 1 * 1
calc.Multiply(5, 2);       // 5 * 2 * 1
calc.Multiply(5, 2, 3);    // 5 * 2 * 3

// Named arguments
calc.Calculate(value: 1000, rate: 0.05, years: 10);
calc.Calculate(years: 10, value: 1000, rate: 0.05);  // Sıra önemsiz
```

---

### 8. ConstructorChaining
**Konu**: this() ve base() kullanımı

**Scenario**: Person → Employee → Manager chain
```csharp
// Person.cs
public class Person
{
    public string Name { get; set; }
    public int Age { get; set; }

    public Person()
    {
        Console.WriteLine("Person() constructor");
    }

    public Person(string name) : this()
    {
        Name = name;
        Console.WriteLine($"Person(name) constructor: {name}");
    }

    public Person(string name, int age) : this(name)
    {
        Age = age;
        Console.WriteLine($"Person(name, age) constructor: {age}");
    }
}

// Employee.cs
public class Employee : Person
{
    public string Department { get; set; }

    public Employee(string name, string dept) : base(name)
    {
        Department = dept;
        Console.WriteLine($"Employee constructor: {dept}");
    }

    public Employee(string name, int age, string dept) : base(name, age)
    {
        Department = dept;
        Console.WriteLine($"Employee constructor: {dept}");
    }
}

// Manager.cs
public class Manager : Employee
{
    public decimal Bonus { get; set; }

    public Manager(string name, int age, string dept, decimal bonus)
        : base(name, age, dept)
    {
        Bonus = bonus;
        Console.WriteLine($"Manager constructor: {bonus:C}");
    }
}

// Program.cs - Constructor çağrı sırası
Manager mgr = new("Ali", 35, "IT", 5000m);
// Output:
// Person() constructor
// Person(name) constructor: Ali
// Person(name, age) constructor: 35
// Employee constructor: IT
// Manager constructor: ₺5.000,00
```

---

### 9. PropertyExamples
**Konu**: Auto-property, getter/setter logic, init-only

**Scenario**: Product with validation
```csharp
// Product.cs
public class Product
{
    // 1. Auto-property (basit)
    public int Id { get; set; }

    // 2. Read-only property
    public string Code { get; }

    // 3. Init-only property (C# 9+)
    public string Category { get; init; }

    // 4. Property with validation
    private decimal _price;
    public decimal Price
    {
        get => _price;
        set
        {
            if (value < 0)
                throw new ArgumentException("Fiyat negatif olamaz");
            _price = value;
        }
    }

    // 5. Computed property (expression body)
    public decimal PriceWithTax => Price * 1.18m;

    // 6. Property with backing field
    private int _stock;
    public int Stock
    {
        get => _stock;
        set
        {
            if (value < 0) value = 0;
            if (value > 10000) value = 10000;
            _stock = value;
        }
    }

    // 7. Required property (C# 11+)
    public required string Name { get; set; }

    public Product(string code)
    {
        Code = code;
    }
}

// Program.cs
Product product = new("PRD001")
{
    Name = "Laptop",  // Required
    Category = "Electronics",  // Init-only
    Price = 5000m,
    Stock = 50
};

Console.WriteLine($"Vergi dahil fiyat: {product.PriceWithTax:C}");

// product.Category = "Other";  // Hata! Init-only
product.Price = -100;  // Exception: Fiyat negatif olamaz
```

---

### 10. IndexerExample
**Konu**: Class indexer kullanımı

**Scenario**: Custom SmartArray
```csharp
// SmartArray.cs
public class SmartArray<T>
{
    private List<T> _items = new();

    // Integer indexer
    public T this[int index]
    {
        get
        {
            if (index < 0 || index >= _items.Count)
                throw new IndexOutOfRangeException();
            return _items[index];
        }
        set
        {
            if (index < 0 || index >= _items.Count)
                throw new IndexOutOfRangeException();
            _items[index] = value;
        }
    }

    // Range indexer (C# 8+)
    public List<T> this[Range range]
    {
        get
        {
            var (start, length) = range.GetOffsetAndLength(_items.Count);
            return _items.Skip(start).Take(length).ToList();
        }
    }

    // String indexer (key-based access)
    public T? this[string key]
    {
        get
        {
            // Basit implementasyon
            int index = int.Parse(key.Replace("item", ""));
            return this[index];
        }
    }

    public void Add(T item) => _items.Add(item);
    public int Count => _items.Count;
}

// Program.cs
SmartArray<string> array = new();
array.Add("Item 0");
array.Add("Item 1");
array.Add("Item 2");
array.Add("Item 3");

// Integer indexer
Console.WriteLine(array[0]);  // Item 0

// Range indexer
var range = array[1..3];  // Items 1-2
Console.WriteLine(string.Join(", ", range));

// String indexer
Console.WriteLine(array["item2"]);  // Item 2

// Foreach support
foreach (var item in array)
{
    Console.WriteLine(item);
}
```

---

## 📋 02-Intermediate Projeleri (8 adet)

### 11. GenericConstraints
**Konu**: where T: class, new(), struct, interface

```csharp
// Repository.cs
public interface IEntity
{
    int Id { get; set; }
}

// Generic repository with constraints
public class Repository<T> where T : class, IEntity, new()
{
    private List<T> _items = new();

    public void Add(T item)
    {
        if (item.Id == 0)  // IEntity constraint
            item.Id = _items.Count + 1;
        _items.Add(item);
    }

    public T Create()
    {
        return new T();  // new() constraint
    }
}

// Value type constraint
public class ValueContainer<T> where T : struct
{
    public T Value { get; set; }

    public bool HasValue => !EqualityComparer<T>.Default.Equals(Value, default);
}

// Multiple constraints
public class Manager<TEntity, TKey>
    where TEntity : class, IEntity, new()
    where TKey : struct
{
    // Implementation
}
```

---

### 12. CovarianceContravariance
**Konu**: out/in modifiers

```csharp
// IProducer.cs - Covariance (out)
public interface IProducer<out T>
{
    T Produce();  // Sadece return type
}

// IConsumer.cs - Contravariance (in)
public interface IConsumer<in T>
{
    void Consume(T item);  // Sadece parameter
}

// Animal.cs
public class Animal { }
public class Dog : Animal { }
public class Cat : Animal { }

// Implementations
public class DogProducer : IProducer<Dog>
{
    public Dog Produce() => new Dog();
}

public class AnimalConsumer : IConsumer<Animal>
{
    public void Consume(Animal animal) { }
}

// Program.cs - Variance kullanımı
IProducer<Dog> dogProducer = new DogProducer();
IProducer<Animal> animalProducer = dogProducer;  // ✅ Covariance

AnimalConsumer animalConsumer = new();
IConsumer<Dog> dogConsumer = animalConsumer;  // ✅ Contravariance
```

---

### 13-18. Diğer Intermediate Projeleri

**13. BoxingPerformance**: Value vs reference type performance
**14. NullableReferenceTypes**: Nullable context, warnings
**15. PatternMatching**: Switch expressions, property patterns
**16. ExtensionMethods**: Static class extensions
**17. DelegateExample**: Func, Action, Predicate
**18. EventHandlerPattern**: Event ve delegate

*Her proje için yukarıdaki şablonu takip edin.*

---

## 📁 Dosya Yapısı Şablonu

Her proje için:
```
ProjectName/
├── ProjectName.csproj         (SDK-style, .NET 8)
├── README.md                  (kullanım, çıktı, öğrenilen)
├── WHY_THIS_PATTERN.md        (neden bu pattern, avantajlar)
├── Program.cs                 (SCENARIO, BAD/GOOD practice)
├── MainClass.cs               (Ana sınıf)
└── SupportClass.cs            (Yardımcı sınıflar)
```

## ✅ Tamamlanma Kontrolü

- [x] PolymorphismBasics (1/10 Beginner) ✅
- [x] CastingExamples (2/10 Beginner) ✅
- [ ] OverrideVirtual (3/10 Beginner)
- [ ] InterfaceBasics (4/10 Beginner)
- [ ] AbstractClassExample (5/10 Beginner)
- [ ] TypeChecking (6/10 Beginner)
- [ ] MethodOverloading (7/10 Beginner)
- [ ] ConstructorChaining (8/10 Beginner)
- [ ] PropertyExamples (9/10 Beginner)
- [ ] IndexerExample (10/10 Beginner)
- [ ] 8 Intermediate Projects

**İlerleme**: 2/18 (11% tamamlandı)

---

## 🎯 Sonraki Adımlar

1. Kalan 8 Beginner projesini yukarıdaki spesifikasyonlara göre oluştur
2. 8 Intermediate projesini oluştur
3. Tüm projeleri test et (`dotnet build` ve `dotnet run`)
4. samples/README.md güncelle
5. Commit ve push

**Tahmini Süre**: Her proje ~30-45 dakika (toplam ~12-15 saat)
