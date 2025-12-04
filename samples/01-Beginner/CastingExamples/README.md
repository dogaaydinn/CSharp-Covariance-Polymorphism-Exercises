# Casting Examples - Tip Dönüşüm Örnekleri

## 📚 Konu
`as`, `is` operatörleri, explicit/implicit casting ve modern pattern matching.

## 🎯 Amaç
C#'ta güvenli ve etkili tip dönüşümü yöntemlerini öğrenmek.

## 🔑 Anahtar Kavramlar
- **Upcasting**: Derived → Base (her zaman güvenli, implicit)
- **Downcasting**: Base → Derived (riskli, kontrol gerekir)
- **as Operator**: Güvenli downcasting (başarısız olursa null)
- **is Operator**: Tip kontrolü (boolean döner)
- **Pattern Matching**: Kontrol + cast birleşimi
- **Switch Expression**: Çoklu tip kontrolü

## 💻 Kullanım

```bash
cd samples/01-Beginner/CastingExamples
dotnet run
```

## 📊 Örnek Çıktı

```
=== C# Casting Örnekleri: as, is, Pattern Matching ===

=== 1. Upcasting (Implicit - Güvenli) ===

✅ Manager → Employee (upcasting):
   Türetilmiş sınıf: Manager
   Base referans: Manager
   Runtime type korunur: True

=== 2. Downcasting: as Operatörü (Güvenli) ===

as operatörü - Güvenli downcasting:

✅ Ayşe Yılmaz bir Manager
Ayşe Yılmaz bir toplantı düzenliyor 📊

❌ Mehmet Kaya Manager değil (null döndü)

=== 3. Downcasting: Explicit Cast (Riskli) ===

✅ Başarılı cast: Ayşe Yılmaz
Ayşe Yılmaz bir toplantı düzenliyor 📊

❌ Exception yakalandı: InvalidCastException
   Mesaj: Unable to cast object of type 'Employee' to type 'Manager'.
   Çözüm: 'as' operatörü veya pattern matching kullan!

=== 5. Pattern Matching (Modern C#) ===

✅ Ayşe Yılmaz bir Manager
   Ekip: 0 kişi, Bonus: ₺5.000,00
Ayşe Yılmaz bir toplantı düzenliyor 📊

✅ Mehmet Kaya bir Developer
   Dil: C#, Deneyim: 5 yıl
Mehmet Kaya yeni özellik geliştiriyor...
```

## 🎓 Öğrenilen Kavramlar

### 1. as Operatörü (Güvenli)
```csharp
Manager? manager = employee as Manager;
if (manager != null)
{
    manager.HoldMeeting();  // Güvenli erişim
}
```

### 2. Pattern Matching (En İyi)
```csharp
if (employee is Manager mgr)
{
    mgr.HoldMeeting();  // Tek satırda kontrol + cast
}
```

### 3. Switch Expression
```csharp
string role = employee switch
{
    Manager m => $"Yönetici {m.Bonus:C}",
    Developer d => $"Dev {d.ProgrammingLanguage}",
    _ => "Çalışan"
};
```

## ⚠️ Yaygın Hatalar

### ❌ Kötü: Kontrolsüz Explicit Cast
```csharp
Manager manager = (Manager)employee;  // Exception atabilir!
```

### ✅ İyi: as Operatörü
```csharp
Manager? manager = employee as Manager;
if (manager != null) { }
```

### ✅ En İyi: Pattern Matching
```csharp
if (employee is Manager mgr)
{
    // mgr kullanıma hazır
}
```

## ⚡ Performans Notları

1. **is operatörü**: O(1) - Çok hızlı
2. **as operatörü**: O(1) - Tek type check
3. **Pattern matching**: O(1) - Compiler optimize eder
4. **Explicit cast**: O(1) - Ancak exception riski

## 🔄 İlişkili Konular
- [PolymorphismBasics](../PolymorphismBasics/) - Virtual/override
- [TypeChecking](../TypeChecking/) - GetType vs typeof
- [PatternMatching](../../02-Intermediate/PatternMatching/) - Advanced patterns
