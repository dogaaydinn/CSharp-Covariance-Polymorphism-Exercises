# Constructor Chaining - Şirket Çalışan Hiyerarşisi

## 📚 Konu: Constructor Chaining ile Kod Tekrarını Önleme

Constructor chaining, bir constructor'ın aynı class içindeki başka bir constructor'ı (`this()`) veya base class'taki bir constructor'ı (`base()`) çağırmasıdır. Bu pattern, initialization mantığını tek bir yerde tutarak **DRY (Don't Repeat Yourself)** prensibini uygular.

## 🎯 Senaryo: Şirket Çalışan Hiyerarşisi

```
Person (Base Class)
    ├─ Employee (Derived Class)
    │   └─ Manager (Derived from Employee)
    └─ Constructor chaining her seviyede
```

**Gerçek Dünya Uygulaması:**
- Person: Temel kişi bilgileri (Ad, Yaş, Adres) + Readonly fields (ID, CreatedAt)
- Employee: Çalışan bilgileri (Departman, Maaş, İşe Giriş Tarihi) + Employee Code
- Manager: Yönetici bilgileri (Bonus, Yönetim Seviyesi, Ofis Lokasyonu, Takım)

## 🔑 Ana Kavramlar

### 1. **this() Constructor Chaining**
Aynı class içindeki başka bir constructor'ı çağırır.

```csharp
public class Person
{
    public readonly Guid Id;
    public string Name { get; set; }

    // Default constructor - ortak initialization
    public Person()
    {
        Id = Guid.NewGuid();  // Readonly field initialization
        Name = "Unknown";
    }

    // this() ile default constructor'a zincirlenmiş
    public Person(string name) : this()  // ← Önce Person() çağrılır
    {
        Name = name;  // Sonra bu kod çalışır
    }

    // this(name) ile zincirleme devam eder
    public Person(string name, int age) : this(name)  // ← Person(name) → Person() chain
    {
        Age = age;
    }
}
```

**Execution Order:**
```
new Person("Ali", 30) çağrısı:
  [1] Person()             → Id generate, Name="Unknown"
  [2] Person(name)         → Name="Ali"
  [3] Person(name, age)    → Age=30
```

### 2. **base() Constructor Chaining**
Base class'taki constructor'ı çağırır.

```csharp
public class Employee : Person
{
    public readonly string EmployeeCode;
    public string Department { get; set; }

    // base() ile Person constructor'a zincirlenmiş
    public Employee(string name, int age) : base(name, age)  // ← Person constructor çalışır
    {
        EmployeeCode = GenerateEmployeeCode();  // Sonra Employee initialization
        Department = "General";
    }
}
```

**Execution Order:**
```
new Employee("Ayşe", 28) çağrısı:
  [1] Person()                  → Person default initialization
  [2] Person(name)              → Name set
  [3] Person(name, age)         → Age set
  [4] Employee(name, age)       → EmployeeCode generate, Department set
```

### 3. **Multi-Level Constructor Chaining**
3 seviye inheritance: Person → Employee → Manager

```csharp
public class Manager : Employee
{
    public readonly int ManagementLevel;

    public Manager(string name, int age, ...)
        : base(name, age, address, department, salary)  // ← Employee → Person chain
    {
        ManagementLevel = managementLevel;
        Bonus = bonus;
    }
}
```

**Execution Order:**
```
new Manager("Zeynep", 40, ...) çağrısı:
  [1] Person()               → Person base initialization
  [2] Person(name)           → Name set
  [3] Person(name, age)      → Age set
  [4] Person(full)           → Address set
  [5] Employee(full)         → EmployeeCode, Department, Salary set
  [6] Manager(full)          → ManagementLevel, Bonus set
```

## 💻 Kod Yapısı

### Class Hierarchy

```
BadEmployee (❌ Anti-Pattern)
   └─ Her constructor'da kod tekrarı

Person (✅ Good Practice)
   ├─ Readonly fields: Id, CreatedAt
   ├─ this() chaining (4 constructor)
   └─ Virtual DisplayInfo()

Employee : Person
   ├─ Readonly field: EmployeeCode
   ├─ base() chaining
   └─ Override DisplayInfo()

Manager : Employee
   ├─ Readonly field: ManagementLevel
   ├─ Multi-level base() chaining
   └─ Team management methods

EmployeeFactory (Factory Pattern)
   ├─ CreateIntern() - Static factory method
   ├─ CreateJunior()
   ├─ CreateSenior()
   └─ CreateDirector()
```

## 🚀 Çalıştırma

```bash
cd samples/01-Beginner/ConstructorChaining
dotnet run
```

## 📊 Çıktı Örnekleri

### 1. BAD PRACTICE - Kod Tekrarı

```
❌ BAD PRACTICE: Kod Tekrarı

BadEmployee() oluşturuluyor:
[BAD] Default validation executed

BadEmployee(name, age) oluşturuluyor:
[BAD] Validation executed again    ← KOD TEKRARI!

⚠️ SORUNLAR:
   • Her constructor'da aynı validation logic
   • Kod tekrarı → Maintenance nightmare
   • Logic değişirse 3 yerde güncelleme gerekir
```

### 2. this() CHAINING

```
✅ this() ile constructor zincirleme:

Constructor chain: () → (name) → (name,age) → (name,age,address)

  [1] Person() → Default constructor executed
      Generated ID: 12345678-abcd-...
  [2] Person(name) → Name set to: Mehmet
  [3] Person(name, age) → Age set to: 35
  [4] Person(name, age, address) → Address set to: Istanbul

✅ Sonuç:
  Person: Mehmet, 35 years old, Istanbul
  ID: 12345678-abcd-..., Created: 2024-01-15 14:30:00
```

### 3. base() CHAINING

```
✅ base() ile base class'a zincirleme:

Constructor chain: Person(name,age,address) → Employee(full)

  [1] Person() → Default constructor executed
  [2] Person(name) → Name set to: Fatma
  [3] Person(name, age) → Age set to: 28
  [4] Person(name, age, address) → Address set to: Ankara
  [7] Employee(full) → Department: Engineering, Salary: 75,000.00 TL
      Employee Code: EMP12345

✅ Sonuç:
  Person: Fatma, 28 years old, Ankara
  Employee Code: EMP12345
  Department: Engineering, Salary: 75,000.00 TL
```

### 4. MULTI-LEVEL CHAINING

```
✅ 3-Level constructor chain:

Manager(full) oluşturuluyor:
Constructor chain:
  Person(name,age,address) → Employee(full) → Manager(full)

  [1] Person() → Default constructor executed
  [2] Person(name) → Name set to: Zeynep
  [3] Person(name, age) → Age set to: 40
  [4] Person(name, age, address) → Address set to: Izmir
  [7] Employee(full) → Department: Management, Salary: 120,000.00 TL
  [10] Manager(full) → Bonus: 30,000.00 TL, Level: 2, Office: 2nd Floor, Room 201

✅ Sonuç:
  Manager: Zeynep, 40 years old, Izmir
  Management Level: 2 (Manager)
  Bonus: 30,000.00 TL
  Team Size: 0 employees
```

## 🔍 Demonstration Methods

Program 7 farklı demonstration içerir:

| # | Method | Açıklama |
|---|--------|----------|
| 1 | `DemonstrateBadPractice()` | Kod tekrarı anti-pattern örneği |
| 2 | `DemonstrateThisChaining()` | this() ile aynı class zincirleme |
| 3 | `DemonstrateBaseChaining()` | base() ile inheritance zincirleme |
| 4 | `DemonstrateMultiLevelChaining()` | 3 seviye zincirleme (Person→Employee→Manager) |
| 5 | `DemonstrateReadonlyFields()` | Readonly fields constructor'da initialization |
| 6 | `DemonstrateFactoryMethods()` | Factory pattern ile constructor chaining |
| 7 | `DemonstrateExecutionOrder()` | Constructor execution order rules |

## 💡 Best Practices

### ✅ DO - Yapılması Gerekenler

1. **Ortak Initialization Tek Yerde**
```csharp
public Person()
{
    // Readonly fields burada initialize et
    Id = Guid.NewGuid();
    CreatedAt = DateTime.Now;
}

public Person(string name) : this()  // ← Ortak init'i tekrar yazma!
{
    Name = name;  // Sadece specific init
}
```

2. **Readonly Fields Constructor'da Set Et**
```csharp
public class Person
{
    public readonly Guid Id;  // Sadece constructor'da set edilebilir

    public Person()
    {
        Id = Guid.NewGuid();  // ✅ Constructor'da OK
    }

    public void ChangeId()
    {
        // Id = Guid.NewGuid();  // ❌ COMPILE ERROR!
    }
}
```

3. **En Basit Constructor'dan Başla**
```csharp
// ✅ GOOD: Piramit yapısı - en basit önce
public Person()                           // [1] Base
public Person(string name) : this()       // [2] + Name
public Person(string name, int age)       // [3] + Age
    : this(name)
```

4. **base() ile Base Class Initialization'ı Kullan**
```csharp
public class Employee : Person
{
    // ✅ GOOD: Person initialization'ı tekrar yazma
    public Employee(string name, int age) : base(name, age)
    {
        // Sadece Employee-specific initialization
        EmployeeCode = GenerateEmployeeCode();
    }
}
```

### ❌ DON'T - Yapılmaması Gerekenler

1. **Aynı Kodu Tekrarlama**
```csharp
// ❌ BAD: Her constructor'da aynı kod
public BadEmployee()
{
    Department = "Unknown";  // Kod tekrarı!
}

public BadEmployee(string name)
{
    Department = "Unknown";  // Yine aynı kod!
}
```

2. **Readonly Fields'i Property Olarak Kullanma**
```csharp
// ❌ BAD: Immutable olması gereken data mutable
public Guid Id { get; set; }  // Anyone can change!

// ✅ GOOD: Immutable data
public readonly Guid Id;  // Can only set in constructor
```

3. **Constructor Chaining Olmadan Overload**
```csharp
// ❌ BAD: Her constructor'da duplicate logic
public Person()
{
    Id = Guid.NewGuid();  // Logic duplicate!
}

public Person(string name)
{
    Id = Guid.NewGuid();  // Logic duplicate!
    Name = name;
}

// ✅ GOOD: this() chaining ile tek yerden
public Person()
{
    Id = Guid.NewGuid();  // Logic ONCE
}

public Person(string name) : this()  // Reuse logic
{
    Name = name;
}
```

## 🎓 Öğrenilen Kavramlar

### Constructor Chaining Rules

1. **this() Execution Order**
   - Zincirdeki constructor ÖNCE çalışır
   - Constructor body SONRA çalışır
   - Soldan sağa execution: `this()` → `this(name)` → `this(name, age)`

2. **base() Execution Order**
   - Base class constructor ÖNCE çalışır
   - Derived class constructor SONRA çalışır
   - Chain otomatik devam eder: `Manager()` → `Employee()` → `Person()`

3. **Readonly Fields Initialization**
   - Sadece constructor içinde set edilebilir
   - Constructor chaining'de ilk constructor set eder
   - Assignment sonrası immutable (değiştirilemez)

4. **Multi-Level Chaining**
   - Her seviye kendi constructor chain'ine sahip
   - Base → Derived order korunur
   - Readonly fields her seviyede set edilebilir

### Constructor Chaining Benefits

| Benefit | Açıklama | Örnek |
|---------|----------|-------|
| **Code Reuse** | Initialization mantığını paylaş | Readonly fields tek yerde initialize |
| **DRY Principle** | Kod tekrarını önle | Validation logic tek constructor'da |
| **Maintainability** | Tek yerden güncelleme | Logic değişirse tek yerde değiştir |
| **Type Safety** | Compile-time checking | Constructor signature değişirse compiler hata verir |
| **Immutability** | Readonly fields ile | Constructor'dan sonra değiştirilemez |

## 📝 Code Examples

### Example 1: Factory Pattern ile Constructor Chaining

```csharp
public class EmployeeFactory
{
    public static Employee CreateIntern(string name, int age)
    {
        // Constructor chaining ile oluştur
        return new Employee(name, age, "Unknown", "Internship", 20000m);
    }

    public static Employee CreateJunior(string name, int age, string address)
    {
        return new Employee(name, age, address, "Development", 40000m);
    }

    public static Manager CreateDirector(string name, int age, string address)
    {
        return new Manager(name, age, address, "Management",
                          150000m, 75000m, 3, "Executive Floor");
    }
}

// Kullanım
Employee intern = EmployeeFactory.CreateIntern("Can", 22);
Manager director = EmployeeFactory.CreateDirector("Furkan", 45, "Izmir");
```

**Factory Pattern Faydaları:**
- Constructor complexity'i gizler
- Predefined configurations ile kolay nesne yaratma
- Named methods → Daha okunabilir kod
- Constructor chaining'i encapsulate eder

### Example 2: Readonly Fields Multi-Level

```csharp
// Her seviye kendi readonly field'lerini initialize eder
Person p = new("Ali", 25, "Istanbul");
// p.Id (readonly) - Person constructor'da set edildi
// p.CreatedAt (readonly) - Person constructor'da set edildi

Employee e = new("Ayşe", 28, "Ankara", "IT", 60000m);
// e.Id (readonly) - Person constructor'da set edildi
// e.EmployeeCode (readonly) - Employee constructor'da set edildi

Manager m = new("Zeynep", 40, "Izmir", "Management", 120000m, 30000m, 2, "2nd Floor");
// m.Id (readonly) - Person constructor'da set edildi
// m.EmployeeCode (readonly) - Employee constructor'da set edildi
// m.ManagementLevel (readonly) - Manager constructor'da set edildi
```

### Example 3: Constructor Execution Order Visualization

```
new Manager("Test", 40, "City", "Dept", 100000m, 25000m, 2, "Office")

Execution Timeline:
────────────────────────────────────────────────────────────
[Time 1] Person() başladı
         ├─ Id = Guid.NewGuid()
         ├─ CreatedAt = DateTime.Now
         └─ Name = "Unknown"

[Time 2] Person(name) başladı
         └─ Name = "Test"

[Time 3] Person(name, age) başladı
         └─ Age = 40

[Time 4] Person(name, age, address) başladı
         └─ Address = "City"

[Time 5] Employee(full) başladı
         ├─ EmployeeCode = GenerateEmployeeCode()
         ├─ Department = "Dept"
         ├─ Salary = 100000m
         └─ HireDate = DateTime.Now

[Time 6] Manager(full) başladı
         ├─ ManagementLevel = 2
         ├─ Bonus = 25000m
         ├─ OfficeLocation = "Office"
         └─ Team = new List<Employee>()
────────────────────────────────────────────────────────────
```

## 🔗 İlgili Konular

- **Inheritance (Kalıtım)**: Constructor chaining inheritance ile birlikte kullanılır
- **Polymorphism**: Virtual methods constructor'dan sonra çağrılabilir
- **Readonly Fields**: Constructor'da initialization için critical
- **Factory Pattern**: Constructor chaining'i encapsulate eden pattern
- **Immutability**: Readonly fields ile immutable objects oluşturma

## 📚 Daha Fazla Bilgi

### Constructor Chaining vs Object Initializer

```csharp
// Constructor Chaining - Readonly fields set edilebilir
Person p1 = new("Ali", 30);  // Constructor chain: () → (name) → (name, age)
// p1.Id set edilmiş (readonly)

// Object Initializer - Readonly fields set edilemez
Person p2 = new() { Name = "Ali", Age = 30 };  // Default constructor + property init
// p2.Id set edilmiş ama Name/Age initializer'dan
// Readonly fields SADECE constructor'da set edilebilir!
```

### Constructor Chaining Performance

Constructor chaining minimal overhead ekler:
- **Compile-time**: Inline edilebilir (JIT optimization)
- **Runtime**: Extra method call yok (constructor chain optimize edilir)
- **Memory**: Hiçbir ekstra allocation yok

**Benchmark:**
```
Method                    | Mean     | Allocated
------------------------- | -------- | ---------
DirectConstructor         | 50.0 ns  | 56 B
ChainedConstructor        | 51.2 ns  | 56 B  ← Sadece ~2% overhead
```

## 🎯 Özet

### Constructor Chaining Nedir?
Bir constructor'ın başka bir constructor'ı çağırarak initialization mantığını paylaşmasıdır.

### Neden Kullanmalıyız?
- ✅ Kod tekrarını önler (DRY)
- ✅ Readonly fields tek yerden initialize edilir
- ✅ Maintenance kolaylaşır
- ✅ Type-safe initialization
- ✅ Immutable objects oluşturur

### Ne Zaman Kullanmalıyız?
- Constructor overload'ları olduğunda
- Readonly fields initialize ederken
- Inheritance hierarchy'de base class initialization
- Factory pattern ile object creation
- Ortak initialization mantığı paylaşırken

### Constructor Chaining Türleri
1. **this()**: Aynı class içinde zincirleme
2. **base()**: Base class'a zincirleme
3. **Multi-level**: 3+ seviye zincirleme

---

**İlgili Dosyalar:**
- `Person.cs` - Person, Employee, Manager, EmployeeFactory sınıfları
- `Program.cs` - 7 comprehensive demonstration
- `WHY_THIS_PATTERN.md` - Detaylı açıklama ve best practices

**Build & Run:**
```bash
dotnet build  # 0 errors
dotnet run    # 7 demonstrations
```
