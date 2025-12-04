# Neden Doğru Casting Yöntemleri?

## 🤔 Problem: Güvensiz Tip Dönüşümleri

### ❌ Kötü Yaklaşım

```csharp
public void ProcessEmployee(Employee emp)
{
    // KÖTÜ: Explicit cast - Runtime hatası riski!
    Manager manager = (Manager)emp;  // 💥 InvalidCastException!
    manager.HoldMeeting();
}

// Bu kod çalışma zamanında patlar:
Employee employee = new Employee("Ali", "IT", 10000m);
ProcessEmployee(employee);  // 💥 CRASH!
```

**Problemler:**
1. Runtime'da exception fırlatır
2. Kullanıcı deneyimini bozar
3. Production'da hata oluşur
4. Test edilmesi zor

### ✅ İyi Yaklaşım

```csharp
public void ProcessEmployee(Employee emp)
{
    // İYİ: as operatörü - Güvenli
    Manager? manager = emp as Manager;
    if (manager != null)
    {
        manager.HoldMeeting();  // Sadece Manager ise çalışır
    }
}

// VEYA

public void ProcessEmployee(Employee emp)
{
    // EN İYİ: Pattern matching
    if (emp is Manager mgr)
    {
        mgr.HoldMeeting();  // Tür kontrolü + cast tek satırda
    }
}
```

## ✨ Güvenli Casting Yöntemleri

### 1. **as Operatörü**

```csharp
// Başarısız olursa null döner, exception atmaz
Manager? manager = employee as Manager;

if (manager != null)
{
    // Güvenli kullanım
    manager.HoldMeeting();
}
```

**Avantajlar:**
- ✅ Asla exception atmaz
- ✅ Null check ile güvenli
- ✅ Performanslı (tek type check)

**Dezavantajlar:**
- ❌ Nullable reference type kontrolü gerekir
- ❌ Null check yazmayı unutabilirsiniz

### 2. **is Operatörü**

```csharp
// Sadece kontrol eder, cast etmez
if (employee is Manager)
{
    Manager manager = (Manager)employee;  // Ayrı cast gerekli
    manager.HoldMeeting();
}
```

**Avantajlar:**
- ✅ Güvenli type check
- ✅ Boolean döner

**Dezavantajlar:**
- ❌ Ayrı cast gerekir (eski yaklaşım)

### 3. **Pattern Matching (EN İYİ)**

```csharp
// Kontrol + cast birleşimi - Modern C#
if (employee is Manager mgr)
{
    mgr.HoldMeeting();  // Hemen kullanılabilir
}
```

**Avantajlar:**
- ✅ Tek satırda kontrol + cast
- ✅ Compiler desteği
- ✅ Null-safe
- ✅ Okunabilir kod

**En iyi yaklaşım budur! 🏆**

### 4. **Switch Expression Pattern**

```csharp
string info = employee switch
{
    Manager m => $"Yönetici: {m.Bonus:C}",
    Developer d => $"Developer: {d.ProgrammingLanguage}",
    HRSpecialist hr => $"İK: {hr.EmployeesManaged} çalışan",
    _ => "Genel çalışan"
};
```

**Avantajlar:**
- ✅ Çoklu tip kontrolü elegant
- ✅ Exhaustive checking
- ✅ Expression-based

## 🏗️ Gerçek Dünya Örnekleri

### 1. **Ödeme İşleme Sistemi**

```csharp
// ❌ KÖTÜ
public void ProcessPayment(Payment payment)
{
    CreditCardPayment cc = (CreditCardPayment)payment;  // 💥 Crash riski
    cc.ProcessCreditCard();
}

// ✅ İYİ
public void ProcessPayment(Payment payment)
{
    if (payment is CreditCardPayment cc)
    {
        cc.ProcessCreditCard();
    }
    else if (payment is PayPalPayment pp)
    {
        pp.ProcessPayPal();
    }
    else if (payment is CryptoPayment crypto)
    {
        crypto.ProcessCrypto();
    }
}
```

### 2. **Event Handling**

```csharp
// ✅ Pattern matching ile elegant event handling
public void HandleEvent(Event evt)
{
    switch (evt)
    {
        case ClickEvent click:
            Console.WriteLine($"Click at: {click.X}, {click.Y}");
            break;
        case KeyPressEvent key:
            Console.WriteLine($"Key pressed: {key.KeyCode}");
            break;
        case MouseMoveEvent move:
            Console.WriteLine($"Mouse moved to: {move.X}, {move.Y}");
            break;
    }
}
```

### 3. **Logging Sistemi**

```csharp
public void Log(LogEntry entry)
{
    string message = entry switch
    {
        ErrorLogEntry error => $"[ERROR] {error.Message} - {error.StackTrace}",
        WarningLogEntry warning => $"[WARN] {warning.Message}",
        InfoLogEntry info => $"[INFO] {info.Message}",
        _ => $"[UNKNOWN] {entry.Message}"
    };

    Console.WriteLine(message);
}
```

## 📊 Karşılaştırma Tablosu

| Yöntem | Güvenlik | Performans | Okunabilirlik | Önerilen? |
|--------|----------|------------|---------------|-----------|
| Explicit Cast `(T)obj` | ❌ Düşük | ⚡ En hızlı | 😐 Orta | ❌ Hayır |
| `as` Operator | ✅ Yüksek | ⚡ Hızlı | ✅ İyi | ✅ Evet |
| `is` Operator | ✅ Yüksek | ⚡ Hızlı | 😐 Orta | ⚠️ Eski |
| Pattern Matching | ✅ Yüksek | ⚡ Hızlı | ✅ Mükemmel | ✅ EN İYİ |
| Switch Expression | ✅ Yüksek | ⚡ Hızlı | ✅ Mükemmel | ✅ Çoklu tip için |

## 🎯 Ne Zaman Hangi Yöntemi Kullanmalı?

### ✅ Pattern Matching Kullan:
- Tek bir tip kontrolü için
- Modern C# (7.0+) projelerinde
- Okunabilirlik önemli olduğunda
- **Varsayılan tercih olarak**

```csharp
if (obj is Manager mgr)
{
    mgr.HoldMeeting();
}
```

### ✅ Switch Expression Kullan:
- Çoklu tip kontrolü için
- Exhaustive matching gerektiğinde
- Fonksiyonel stil tercih edildiğinde

```csharp
string result = obj switch
{
    Manager m => "Yönetici",
    Developer d => "Geliştirici",
    _ => "Bilinmeyen"
};
```

### ⚠️ as Operatörü Kullan:
- C# 6.0 veya daha eski projelerde
- Pattern matching mevcut değilse
- Null check ihtiyacı varsa

```csharp
Manager? mgr = obj as Manager;
if (mgr != null) { }
```

### ❌ Explicit Cast Kullanma:
- **Kesinlikle tipi bilmiyorsan**
- Production kodunda
- Exception handling olmadan

```csharp
// ASLA YAPMA (garantili değilse)
Manager mgr = (Manager)obj;  // 💥
```

## 🎯 Özet

Doğru casting yöntemleri:

1. **Güvenlik sağlar** - Runtime hataları önler
2. **Kod kalitesini artırır** - Okunabilir ve bakımı kolay
3. **Modern C# özellikleri** - Pattern matching kullan
4. **Performanslı** - Tüm yöntemler O(1)

> **Best Practice**: Pattern matching (`if (obj is Type t)`) veya switch expression kullan. Explicit cast'ten kaçın!

Güvenli casting, production-ready kod yazmanın temelidir. 🛡️
