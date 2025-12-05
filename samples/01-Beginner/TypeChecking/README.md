# Type Checking - typeof, GetType(), is, as, Pattern Matching

## 📚 Konu

**Runtime ve Compile-Time Type Checking**: C#'ta type kontrolü ve casting işlemleri.

Bu proje, C#'ın type checking mekanizmalarını **otopark ücret hesaplama sistemi** senaryosu üzerinden öğretir. Her araç tipine göre farklı ücret hesaplama yaparak, type checking'in gerçek dünya kullanımını gösterir.

## 🎯 Amaç

- `typeof` operatörü ile compile-time type literal almayı öğrenmek
- `GetType()` metodu ile runtime type bilgisini öğrenmek
- `is` operatörü ile güvenli type checking yapmayı öğrenmek
- `as` operatörü ile safe casting yapmayı öğrenmek
- Explicit type casting ve exception handling'i öğrenmek
- C# 9+ pattern matching özelliklerini öğrenmek (property patterns, relational patterns)
- Type-based logic ile gerçek dünya senaryolarını çözmek

## 🔑 Anahtar Kavramlar

### 1. typeof Operator (Compile-Time)
```csharp
Type carType = typeof(Car);
Console.WriteLine(carType.Name);        // "Car"
Console.WriteLine(carType.IsAbstract);  // False
Console.WriteLine(carType.BaseType);    // Vehicle
```

**Özellikler:**
- Compile-time'da çalışır
- Type literal alır (sınıf adı)
- `System.Type` nesnesi döner
- Reflection için kullanılır

### 2. GetType() Method (Runtime)
```csharp
Vehicle vehicle = new Car("Toyota", "34ABC123", 2023, 4, "Hybrid");
Type runtimeType = vehicle.GetType();   // Car (runtime'da belirlenir)
Console.WriteLine(runtimeType.Name);     // "Car"
```

**Özellikler:**
- Runtime'da çalışır
- Instance'tan type bilgisi döner
- Polymorphic referanslarda gerçek tipi bulur
- Exact type comparison için kullanılır

### 3. is Operator (Type Checking)
```csharp
// Basit type checking
if (vehicle is Car)
{
    Console.WriteLine("Bu bir araba!");
}

// Pattern matching ile değişken ataması
if (vehicle is Car car)
{
    Console.WriteLine($"Araba: {car.Doors} kapı, {car.FuelType}");
}
```

**Özellikler:**
- Inheritance-aware (base type check de true döner)
- Boolean döner
- Pattern matching destekler
- Null-safe (null için false döner)

### 4. as Operator (Safe Casting)
```csharp
// Safe casting - exception atmaz
Car? car = vehicle as Car;
if (car != null)
{
    Console.WriteLine($"Doors: {car.Doors}");
}

// Null-conditional operator ile
string? fuelType = (vehicle as Car)?.FuelType;
```

**Özellikler:**
- Casting başarısız olursa `null` döner
- `InvalidCastException` atmaz
- Reference types için çalışır
- Null-conditional operator (?.) ile kullanılabilir

### 5. Explicit Type Casting
```csharp
// Explicit cast - exception atabilir!
if (vehicle is Car)
{
    Car car = (Car)vehicle;  // Güvenli - önce kontrol edildi
    Console.WriteLine($"Doors: {car.Doors}");
}

// ❌ BAD: Önce kontrol etmeden cast
// Car car = (Car)vehicle;  // InvalidCastException atabilir!
```

**Özellikler:**
- `(Type)object` syntax'ı kullanır
- Casting başarısız olursa `InvalidCastException` atar
- Önce `is` ile kontrol etmek gerekir
- Ya da `as` kullan (daha güvenli)

### 6. Pattern Matching (C# 9+)
```csharp
// Switch expression with type patterns
string info = vehicle switch
{
    Car { Doors: 4, FuelType: "Hybrid" } => "Hibrit araba",
    Car { FuelType: "Diesel" } c => $"Dizel araba: {c.Doors} kapı",
    Truck { Axles: > 2 } => "Ağır kamyon",
    Motorcycle { HasSidecar: true } m => $"Sepetli motor: {m.EngineCC}cc",
    _ => "Bilinmeyen araç"
};

// Relational patterns
string ageCategory = vehicle.Year switch
{
    >= 2024 => "Sıfır araç",
    >= 2020 => "Yeni araç",
    _ => "Eski araç"
};
```

**Özellikler:**
- Type patterns: `Car c => ...`
- Property patterns: `{ Doors: 4 }`
- Relational patterns: `>= 2024`, `> 2`
- Logical patterns: `or`, `and`, `not`
- Discard pattern: `_` (default case)

## 💻 Kullanım

```bash
cd samples/01-Beginner/TypeChecking
dotnet build
dotnet run
```

## 📊 Program Çıktısı

Program 7 bölümden oluşur:

1. **typeof Operator**: Compile-time type bilgisi (Name, FullName, IsAbstract, BaseType)
2. **GetType() Method**: Runtime type bilgisi (her araç için)
3. **is Operator**: Type checking ve pattern matching
4. **as Operator**: Safe casting örnekleri
5. **Type Casting**: Explicit casting ve exception handling
6. **Pattern Matching**: C# 9+ features (property patterns, relational patterns)
7. **Otopark Ücret Hesaplama**: Type-based logic ile farklı ücretler

### Örnek Çıktı (Ücret Hesaplama):

```
═══ 7. 💰 OTOPARK ÜCRET HESAPLAMA ═══

🎫 34ABC123 (Car):
   Marka: Toyota Corolla
   Süre: 3.0 saat
   Saatlik ücret: 10.00 TL
   Brüt tutar: 30.00 TL
   İndirim: %20 (6.00 TL)        ← Hibrit araç indirimi!
   💰 Ödenecek: 24.00 TL

🎫 06TIR456 (Truck):
   Marka: Volvo FH16
   Süre: 3.0 saat
   Saatlik ücret: 30.00 TL       ← 3 akslı kamyon (ağır araç)
   Brüt tutar: 90.00 TL
   💰 Ödenecek: 90.00 TL

📊 TOPLAM GELİR: 242.40 TL
📊 ARAÇ SAYISI: 5 araç
📊 ORTALAMA ÜCRET: 48.48 TL/araç

📈 Araç Tipi Dağılımı:
   🚗 Araba: 2 (40.0%)
   🚚 Kamyon: 1 (20.0%)
   🏍️  Motor: 2 (40.0%)
```

## 🎓 Öğrenme Hedefleri

Bu projeyi tamamladıktan sonra:

1. ✅ `typeof` ve `GetType()` farkını anlayacaksınız
2. ✅ Compile-time vs runtime type checking'i kavrayacaksınız
3. ✅ `is` operatörü ile güvenli type checking yapacaksınız
4. ✅ `as` operatörü ile safe casting yapacaksınız
5. ✅ Explicit casting ve exception handling'i öğreneceksiniz
6. ✅ Pattern matching (C# 9+) özelliklerini kullanacaksınız
7. ✅ Type-based logic ile gerçek dünya problemlerini çözeceksiniz
8. ✅ Polymorphism ile type checking'i birleştireceksiniz

## 💡 Best Practices

### ✅ GOOD Practices

```csharp
// 1. is ile pattern matching (modern, güvenli)
if (vehicle is Car car)
{
    Console.WriteLine($"Doors: {car.Doors}");
}

// 2. as ile null-conditional operator
string? fuelType = (vehicle as Car)?.FuelType;

// 3. Switch expression ile pattern matching
string info = vehicle switch
{
    Car { FuelType: "Hybrid" } => "Çevreci araba",
    Truck { Axles: > 2 } => "Ağır kamyon",
    _ => "Standart araç"
};

// 4. typeof ile exact type comparison
if (vehicle.GetType() == typeof(Car))
{
    // Exact Car tipi (alt sınıflar false döner)
}
```

### ❌ BAD Practices

```csharp
// 1. Önce kontrol etmeden cast - DANGEROUS!
Car car = (Car)vehicle;  // InvalidCastException atabilir!

// 2. String comparison ile type kontrolü
if (vehicle.GetType().Name == "Car")  // Kırılgan, tip güvenli değil
{
    // BAD!
}

// 3. Gereksiz casting
if (vehicle is Car)
{
    Car car = (Car)vehicle;  // is zaten type check yaptı, as kullan!
}

// 4. GetType() yerine is kullanmalısın (inheritance-aware)
if (vehicle.GetType() == typeof(Vehicle))  // Alt sınıflar false döner
{
    // BAD - inheritance'ı göz ardı eder
}
```

## 🔍 typeof vs GetType() vs is

| Özellik | typeof | GetType() | is |
|---------|--------|-----------|-----|
| **Ne zaman çalışır?** | Compile-time | Runtime | Runtime |
| **Ne alır?** | Type literal (Car) | Instance | Instance |
| **Ne döner?** | Type | Type | Boolean |
| **Inheritance?** | ❌ No | ❌ No | ✅ Yes |
| **Null-safe?** | N/A | ❌ Throws | ✅ Yes |
| **Pattern matching?** | ❌ No | ❌ No | ✅ Yes |
| **Kullanım alanı** | Reflection | Runtime type | Type checking |

### Örnek Karşılaştırma:

```csharp
Vehicle vehicle = new Car("Toyota", "34ABC123", 2023, 4, "Hybrid");

// typeof - Compile-time
Type carType = typeof(Car);                // OK
// Type vehicleInstance = typeof(vehicle);  // ❌ COMPILE ERROR!

// GetType() - Runtime
Type runtimeType = vehicle.GetType();      // Car (runtime'da belirlenir)

// is - Runtime, inheritance-aware
bool isVehicle = vehicle is Vehicle;       // true (base type)
bool isCar = vehicle is Car;               // true (exact type)
bool isTruck = vehicle is Truck;           // false

// Exact type comparison
bool exactCar = vehicle.GetType() == typeof(Car);      // true
bool exactVehicle = vehicle.GetType() == typeof(Vehicle);  // false!
```

## 🎯 Ne Zaman Kullanmalı?

### typeof Kullanmalısın:
- Reflection için Type nesnesi almak istiyorsan
- Generic type constraints kontrol edeceksen
- Compile-time'da type bilgisine ihtiyacın varsa
- Type parametrelerini karşılaştıracaksan

### GetType() Kullanmalısın:
- Runtime'da instance'ın gerçek tipini öğrenmek istiyorsan
- Exact type comparison yapmak istiyorsan (inheritance'ı göz ardı et)
- Polymorphic referanslarda gerçek tipi bulmak istiyorsan

### is Kullanmalısın:
- Type checking yapmak istiyorsan (en yaygın kullanım)
- Pattern matching ile değişken ataması yapacaksan
- Inheritance-aware kontrol istiyorsan (en esnek)
- Null-safe type checking istiyorsan

### as Kullanmalısın:
- Safe casting yapmak istiyorsan (exception atmaz)
- Null-conditional operator (?.) ile kullanacaksan
- Casting başarısız olduğunda null almak istiyorsan

### Pattern Matching Kullanmalısın:
- Modern, okunabilir kod yazmak istiyorsan
- Property-based type checking yapacaksan
- Switch expression kullanacaksan
- Multiple type checks yapacaksan

## 🏗️ Proje Yapısı

```
TypeChecking/
├── TypeChecking.csproj       # .NET 8 project file
├── Vehicle.cs                # Base class + derived classes (Car, Truck, Motorcycle)
├── Program.cs                # 7 demonstration methods (379 lines)
├── README.md                 # Bu dosya
└── WHY_THIS_PATTERN.md       # Detaylı açıklamalar
```

## 🚀 İleri Seviye Konular

Bu projeyi tamamladıktan sonra:

1. **Reflection**: Type nesnesi ile runtime'da kod analizi
2. **Generic Constraints**: `where T : Vehicle` gibi kısıtlamalar
3. **Dynamic Types**: `dynamic` keyword ve late binding
4. **Expression Trees**: Type checking ile expression building
5. **Source Generators**: Compile-time code generation

## 📚 Kaynaklar

- [Microsoft Docs - typeof operator](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/operators/type-testing-and-cast)
- [Microsoft Docs - is operator](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/operators/is)
- [Microsoft Docs - as operator](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/operators/type-testing-and-cast#as-operator)
- [Microsoft Docs - Pattern Matching](https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/functional/pattern-matching)
- [C# 9.0 Pattern Matching Enhancements](https://learn.microsoft.com/en-us/dotnet/csharp/whats-new/csharp-9#pattern-matching-enhancements)

---

**🎯 Sonuç**: Bu proje, C#'ın type checking mekanizmalarını gerçek dünya senaryosu (otopark ücret hesaplama) üzerinden kapsamlı bir şekilde öğretir. Her operatörün ne zaman kullanılacağını ve best practices'i gösterir.
