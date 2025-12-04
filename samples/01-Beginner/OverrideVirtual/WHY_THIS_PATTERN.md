# Neden Override ve Method Hiding Farkı Önemli?

## 🤔 Problem: Polymorphism'in Bozulması

### ❌ Kötü Yaklaşım: Method Hiding

```csharp
public class Account
{
    public virtual decimal CalculateInterest() => 0m;
}

public class CheckingAccount : Account
{
    // ❌ KÖTÜ: new keyword - Method hiding
    public new decimal CalculateInterest()
    {
        return Balance * 0.01m;
    }
}

// Kullanım
List<Account> accounts = new()
{
    new CheckingAccount { Balance = 10000m }
};

foreach (var account in accounts)
{
    // BUG! Base metod çalışır, 0 döner
    decimal interest = account.CalculateInterest();  // 0 ❌
    // Beklenen: 100 (10000 * 0.01)
    // Gerçek: 0
}
```

**Problemler:**
1. 💥 Beklenmeyen davranış - polymorphism çalışmaz
2. 🐛 Bug kaynağı - hata bulmak zor
3. ⚠️ Compiler warning verir (CS0114)
4. 🔍 Runtime'da tespit edilir (unit test gerekir)

### ✅ İyi Yaklaşım: Override Kullan

```csharp
public class Account
{
    public virtual decimal CalculateInterest() => 0m;
}

public class CheckingAccount : Account
{
    // ✅ İYİ: override keyword
    public override decimal CalculateInterest()
    {
        return Balance * 0.01m;
    }
}

// Kullanım
List<Account> accounts = new()
{
    new CheckingAccount { Balance = 10000m }
};

foreach (var account in accounts)
{
    // Doğru! CheckingAccount metodu çalışır
    decimal interest = account.CalculateInterest();  // 100 ✅
}
```

## ✨ Override'ın Faydaları

### 1. Polymorphism Korunur

```csharp
// Base referans - Runtime type'ın metodu çalışır
Account account = new SavingsAccount();
account.CalculateInterest();  // SavingsAccount metodu ✅

account = new InvestmentAccount();
account.CalculateInterest();  // InvestmentAccount metodu ✅
```

### 2. Liskov Substitution Principle

```csharp
// Alt sınıf, üst sınıf yerine kullanılabilir
void ProcessAccount(Account account)
{
    // Hangi türden olursa olsun doğru çalışır
    decimal interest = account.CalculateInterest();
}

ProcessAccount(new SavingsAccount());      // ✅
ProcessAccount(new CheckingAccount());     // ✅
ProcessAccount(new InvestmentAccount());   // ✅
```

### 3. Compiler Desteği

```csharp
// Base class değişirse compiler hata verir
public class Account
{
    // Metod imzası değişti
    public virtual decimal CalculateInterest(int months) => 0m;
}

public class SavingsAccount : Account
{
    // ✅ Compiler hata: Override imzası uyuşmuyor
    public override decimal CalculateInterest() => Balance * 0.05m;
}
```

## 🏗️ Gerçek Dünya Örnekleri

### 1. Banka Sistemi (Bu Örnek)

```csharp
// Faiz hesaplama - Her hesap türü farklı hesaplar
public abstract class Account
{
    public virtual decimal CalculateInterest() => 0m;
}

public class SavingsAccount : Account
{
    public override decimal CalculateInterest() => Balance * 0.05m;  // %5
}

public class InvestmentAccount : Account
{
    public override decimal CalculateInterest() => Balance * 0.08m * RiskFactor;
}

// Toplu işlem
List<Account> accounts = GetAllAccounts();
decimal totalInterest = accounts.Sum(a => a.CalculateInterest());  // ✅ Çalışır
```

### 2. Ödeme Sistemi

```csharp
public abstract class Payment
{
    public abstract void Process();
}

public class CreditCardPayment : Payment
{
    public override void Process()  // ✅ Override
    {
        ChargeCard();
    }
}

public class PayPalPayment : Payment
{
    public override void Process()  // ✅ Override
    {
        TransferViaPayPal();
    }
}

// Polymorphic kullanım
void ProcessPayment(Payment payment)
{
    payment.Process();  // Hangi tip olursa olsun doğru çalışır
}
```

### 3. UI Framework

```csharp
public abstract class Control
{
    public virtual void Render()
    {
        // Base rendering
    }
}

public class Button : Control
{
    public override void Render()  // ✅ Override
    {
        base.Render();
        RenderButton();
    }
}

public class TextBox : Control
{
    public override void Render()  // ✅ Override
    {
        base.Render();
        RenderTextBox();
    }
}

// Framework kodu
void DrawControl(Control control)
{
    control.Render();  // Polymorphic - doğru render metodu çağrılır
}
```

## 📊 Override vs Method Hiding Karşılaştırma

| Özellik | Override | Method Hiding (new) |
|---------|----------|---------------------|
| **Polymorphism** | ✅ Çalışır | ❌ Bozulur |
| **Base Referans** | Türetilmiş metod çalışır | Base metod çalışır |
| **Compiler Warning** | ✅ Warning yok | ⚠️ CS0114 Warning |
| **Liskov Principle** | ✅ Uyumlu | ❌ İhlal eder |
| **Kullanım Alanı** | %99 durumda bu | Çok nadir |
| **Best Practice** | ✅ Önerilen | ❌ Kaçınılmalı |

## 🎯 Ne Zaman Override, Ne Zaman New?

### ✅ Override Kullan (Neredeyse Her Zaman)

```csharp
// Base class virtual metod sunuyor
public class Account
{
    public virtual decimal CalculateInterest() => 0m;
}

// Türetilmiş sınıf - OVERRIDE kullan
public class SavingsAccount : Account
{
    public override decimal CalculateInterest()  // ✅ Doğru
    {
        return Balance * 0.05m;
    }
}
```

**Kullanım Durumları:**
- Base class `virtual` metod tanımlamışsa
- Polymorphic davranış gerekiyorsa
- Liskov Substitution istiyorsanız
- **Varsayılan seçim olarak**

### ⚠️ New Kullan (Çok Nadir)

```csharp
// Base class'ı değiştiremiyorsunuz (3rd party)
public class ThirdPartyAccount
{
    public decimal CalculateInterest() => 0m;  // virtual DEĞİL
}

// İmza çakışıyor ama override edemezsiniz
public class MyAccount : ThirdPartyAccount
{
    public new decimal CalculateInterest()  // ⚠️ Kasıtlı hiding
    {
        return Balance * 0.03m;
    }
}
```

**Kullanım Durumları:**
- Base class'ı değiştiremiyorsunuz
- Base metod `virtual` değil
- Kasıtlı olarak gizlemek istiyorsunuz
- Non-polymorphic davranış istiyorsanız

## 🚨 Method Hiding Hataları

### Hata 1: Beklenmeyen Davranış

```csharp
public class CheckingAccount : Account
{
    public new decimal CalculateInterest() => Balance * 0.01m;
}

CheckingAccount checking = new() { Balance = 10000m };
Console.WriteLine(checking.CalculateInterest());  // 100 ✅

Account account = checking;
Console.WriteLine(account.CalculateInterest());   // 0 ❌ (base metod)

// Aynı nesne, farklı sonuç! 🐛
```

### Hata 2: Koleksiyon İşlemlerde Bug

```csharp
List<Account> accounts = new()
{
    new SavingsAccount() { Balance = 50000m },      // Override kullanır
    new CheckingAccount() { Balance = 25000m },     // New kullanır (hiding)
    new InvestmentAccount() { Balance = 100000m }   // Override kullanır
};

// Faiz hesaplama - CheckingAccount için 0 döner! 🐛
var totalInterest = accounts.Sum(a => a.CalculateInterest());
```

### Hata 3: Unit Test Geçer, Production Hata Verir

```csharp
// Unit test - direct reference
[Test]
public void CheckingAccount_CalculatesInterest()
{
    var checking = new CheckingAccount { Balance = 10000m };
    Assert.AreEqual(100m, checking.CalculateInterest());  // ✅ Geçer
}

// Production kod - polymorphic
public decimal CalculateTotalInterest(List<Account> accounts)
{
    return accounts.Sum(a => a.CalculateInterest());  // ❌ CheckingAccount için 0
}
```

## 💡 Best Practices

### 1. Her Zaman Override Kullan

```csharp
// ✅ DOĞRU
public class SavingsAccount : Account
{
    public override decimal CalculateInterest() { }
}
```

### 2. Compiler Warning'leri Dikkate Al

```csharp
// ⚠️ Warning CS0114: Method hides inherited member
public class CheckingAccount : Account
{
    public decimal CalculateInterest() { }  // new veya override ekle
}
```

### 3. Unit Test ile Polymorphic Davranışı Test Et

```csharp
[Test]
public void PolymorphicBehavior_Works()
{
    Account account = new SavingsAccount { Balance = 10000m };

    // Polymorphic çağrı test et
    decimal interest = account.CalculateInterest();

    Assert.AreEqual(500m, interest);  // SavingsAccount metodu çalışmalı
}
```

### 4. Base Class Virtual Metodları İşaretle

```csharp
// ✅ Virtual metodları açıkça belirt
public class Account
{
    public virtual decimal CalculateInterest() => 0m;  // virtual ekle
}
```

## 🎯 Özet

**Override Kullanmanın Sebepleri:**

1. **Polymorphism çalışır** - Base referans üzerinden doğru metod çağrılır
2. **Liskov Substitution** - Alt sınıf, üst sınıf yerine kullanılabilir
3. **Compiler desteği** - İmza uyuşmazlığında hata verir
4. **Bug önler** - Beklenmeyen davranış olmaz
5. **Best practice** - Industry standard

**Method Hiding'den Kaçınma:**

1. **Polymorphism bozar** - Beklenmeyen davranış
2. **Bug kaynağı** - Koleksiyon işlemlerde hata
3. **Liskov ihlali** - Prensiplere aykırı
4. **Compiler warning** - CS0114
5. **Kötü practice** - Önerilmez

> **Kural:** Base class `virtual` metod sunuyorsa, **her zaman** `override` kullan. `new` keyword sadece çok özel durumlarda (base class değiştiremediğinizde) kullanılmalıdır.

Override, polymorphism'in kalbidir. Doğru kullanımı, bakımı kolay ve hatasız kod yazmanın temelidir. 🛡️
