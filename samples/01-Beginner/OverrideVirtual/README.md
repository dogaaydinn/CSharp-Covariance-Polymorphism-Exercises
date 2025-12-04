# Override Virtual - Method Hiding vs Override

## 📚 Konu
`override` keyword vs `new` keyword (method hiding) farkı ve polymorphism'e etkisi.

## 🎯 Amaç
Method overriding ve method hiding arasındaki kritik farkı anlamak ve neden `override` kullanmanız gerektiğini öğrenmek.

## 🔑 Anahtar Kavramlar
- **Virtual Method**: Base class'ta `virtual` keyword ile tanımlanan, override edilebilir metod
- **Override**: Türetilmiş sınıfta `override` keyword ile virtual metodun yeniden tanımlanması
- **Method Hiding**: Türetilmiş sınıfta `new` keyword ile base metodun gizlenmesi
- **Polymorphism**: Base referans üzerinden türetilmiş sınıf davranışının çalışması
- **Static Type**: Değişkenin derleme zamanı tipi
- **Runtime Type**: Nesnenin çalışma zamanı tipi

## 💻 Kullanım

```bash
cd samples/01-Beginner/OverrideVirtual
dotnet run
```

## 📊 Örnek Çıktı

```
=== Override vs New (Method Hiding) Karşılaştırması ===

=== 1. Türetilmiş Sınıf Referansı ile Çağrı ===

SavingsAccount (override kullanır):
Hesap: 1001, Sahip: Ayşe Yılmaz, Bakiye: ₺50.000,00
   Tür: Tasarruf Hesabı, Faiz: %5,00
   [Override] Tasarruf faizi hesaplandı: ₺2.500,00
Faiz: ₺2.500,00

CheckingAccount (new kullanır - method hiding):
Hesap: 1002, Sahip: Mehmet Kaya, Bakiye: ₺25.000,00
   Tür: Vadesiz Hesap, Aylık Ücret: ₺50,00
   [New/Hidden] Vadesiz faizi hesaplandı: ₺250,00
Faiz: ₺250,00

=== 2. Base Sınıf Referansı ile Çağrı (Polymorphism) ===

✅ SavingsAccount (override):
   Static Type: Account
   Runtime Type: SavingsAccount
   [Override] Tasarruf faizi hesaplandı: ₺2.500,00
   → Türetilmiş sınıf metodu çalıştı: ₺2.500,00

❌ CheckingAccount (new - method hiding):
   Static Type: Account
   Runtime Type: CheckingAccount
   → Base metod çalıştı (sıfır döndü): ₺0,00
   → BEKLENMEDİK DAVRANIŞ! Polymorphism bozuldu!
```

## 🎓 Öğrenilen Kavramlar

### 1. Override Keyword (Doğru Yaklaşım)
```csharp
public class SavingsAccount : Account
{
    // ✅ Override: Polymorphic davranış korunur
    public override decimal CalculateInterest()
    {
        return Balance * 0.05m;
    }
}

Account account = new SavingsAccount();
account.CalculateInterest();  // SavingsAccount metodu çalışır ✅
```

### 2. New Keyword - Method Hiding (Yanlış Yaklaşım)
```csharp
public class CheckingAccount : Account
{
    // ❌ Method Hiding: Polymorphism bozulur
    public new decimal CalculateInterest()
    {
        return Balance * 0.01m;
    }
}

Account account = new CheckingAccount();
account.CalculateInterest();  // Account (base) metodu çalışır! ❌
```

### 3. Polymorphism'in Çalışma Prensibi
```csharp
// Static Type: Account
// Runtime Type: SavingsAccount
Account account = new SavingsAccount();

// Override kullanıldı: Runtime type'ın metodu çalışır ✅
decimal interest = account.CalculateInterest();

// New kullanıldı: Static type'ın metodu çalışır ❌
// Beklenmeyen davranış!
```

## ⚠️ Yaygın Hatalar

### ❌ Kötü: Method Hiding
```csharp
public class CheckingAccount : Account
{
    // Compiler Warning: CS0114
    public new decimal CalculateInterest()  // Method hiding
    {
        return Balance * 0.01m;
    }
}

// Polymorphic kullanımda hata!
List<Account> accounts = new()
{
    new CheckingAccount()
};

foreach (var acc in accounts)
{
    acc.CalculateInterest();  // Base metod çalışır (0 döner) ❌
}
```

### ✅ İyi: Override Kullan
```csharp
public class CheckingAccount : Account
{
    // Override: Polymorphic davranış korunur
    public override decimal CalculateInterest()
    {
        return Balance * 0.01m;
    }
}

// Polymorphic kullanım doğru çalışır
List<Account> accounts = new()
{
    new CheckingAccount()
};

foreach (var acc in accounts)
{
    acc.CalculateInterest();  // CheckingAccount metodu çalışır ✅
}
```

## ⚡ Performans Notları

1. **Override**: O(1) - Virtual method table (vtable) lookup
2. **Method Hiding**: O(1) - Static binding, vtable kullanılmaz
3. **Override약간 daha yavaş** (~nanosaniye seviyesi) ancak polymorphism için gerekli

## 🔄 İlişkili Konular
- [PolymorphismBasics](../PolymorphismBasics/) - Virtual/override temelleri
- [AbstractClassExample](../AbstractClassExample/) - Abstract metodlar
- [InterfaceBasics](../InterfaceBasics/) - Interface implementation

## 📚 Önemli Noktalar

1. **Override her zaman kullan**: Base class `virtual` tanımladıysa, türetilmiş sınıfta `override` kullan
2. **Method hiding neredeyse hiç kullanılmaz**: Sadece kasıtlı olarak base metodu gizlemek istediğinde
3. **Compiler Warning**: CS0114 - Method hides inherited member, `new` keyword ekle veya `override` kullan
4. **Polymorphism için override şart**: Aksi halde beklenen davranışı alamazsınız
5. **Liskov Substitution Principle**: Override kullanarak bu prensibi koruyun

## 🎯 Ne Zaman Hangisini Kullanmalı?

### ✅ Override Kullan:
- Base class `virtual` metod tanımlamışsa
- Polymorphic davranış istiyorsanız
- **%99 durumda bu doğru seçimdir**

### ⚠️ New Kullan (Çok Nadir):
- Kasıtlı olarak base metodunu gizlemek istiyorsanız
- Non-polymorphic davranış istiyorsanız
- Base class'ı değiştiremiyorsanız ve metod imzası çakışıyorsa

### ❌ Asla Kullanma:
- Polymorphism gereken yerde `new` kullanma
- `virtual` metod varken `new` ile gizleme

## 💡 Best Practices

1. Base class metodunu `virtual` yap
2. Türetilmiş sınıfta `override` kullan
3. `new` keyword'den kaçın
4. Compiler warning'leri dikkate al (CS0114)
5. Unit test ile polymorphic davranışı test et
