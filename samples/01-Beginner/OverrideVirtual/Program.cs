// SCENARIO: Banka hesap yönetim sistemi - override vs new keyword farkı
// BAD PRACTICE: 'new' keyword ile method hiding - polymorphic davranış bozulur
// GOOD PRACTICE: 'override' kullanarak polymorphic davranışı koru

using OverrideVirtual;

class Program
{
    static void Main()
    {
        Console.WriteLine("=== Override vs New (Method Hiding) Karşılaştırması ===\n");

        // Hesapları oluştur
        SavingsAccount savings = new(1001, "Ayşe Yılmaz", 50000m, 0.05m);        // %5 faiz
        CheckingAccount checking = new(1002, "Mehmet Kaya", 25000m, 50m);       // %1 faiz
        InvestmentAccount investment = new(1003, "Zeynep Demir", 100000m, "Hisse Senedi", 1.5m);

        Console.WriteLine("=== 1. Türetilmiş Sınıf Referansı ile Çağrı ===\n");
        DemonstrateDirectCall(savings, checking, investment);

        Console.WriteLine("\n=== 2. Base Sınıf Referansı ile Çağrı (Polymorphism) ===\n");
        DemonstratePolymorphicCall(savings, checking, investment);

        Console.WriteLine("\n=== 3. Method Hiding Problemi ===\n");
        DemonstrateMethodHidingProblem();

        Console.WriteLine("\n=== 4. Gerçek Dünya Senaryosu: Toplu Faiz Hesaplama ===\n");
        CalculateAllInterests();

        // Analiz
        Console.WriteLine("\n=== Output Analysis ===");
        Console.WriteLine("1. Override: Base referans üzerinden çağrıldığında türetilmiş sınıf metodu çalışır ✅");
        Console.WriteLine("2. New (hiding): Base referans üzerinden çağrıldığında base metod çalışır ❌");
        Console.WriteLine("3. Method hiding polymorphism'i bozar - beklenmeyen davranışlara yol açar");
        Console.WriteLine("4. Compiler Warning CS0114: Method hides inherited member");

        Console.WriteLine("\n💡 Best Practice:");
        Console.WriteLine("   ✅ Kullan: override keyword - Polymorphic davranış için");
        Console.WriteLine("   ❌ Kullanma: new keyword - Polymorphism'i bozar, bug kaynağı");
        Console.WriteLine("   ⚠️  new keyword sadece kasıtlı olarak base metodu gizlemek istediğinde kullan");
    }

    /// <summary>
    /// Türetilmiş sınıf referansı ile doğrudan çağrı
    /// Her iki durumda da (override ve new) türetilmiş metod çalışır
    /// </summary>
    static void DemonstrateDirectCall(SavingsAccount savings, CheckingAccount checking,
        InvestmentAccount investment)
    {
        Console.WriteLine("Türetilmiş sınıf referansı ile çağrı:\n");

        Console.WriteLine("SavingsAccount (override kullanır):");
        savings.DisplayInfo();
        decimal savingsInterest = savings.CalculateInterest();
        Console.WriteLine($"Faiz: {savingsInterest:C}\n");

        Console.WriteLine("CheckingAccount (new kullanır - method hiding):");
        checking.DisplayInfo();
        decimal checkingInterest = checking.CalculateInterest();
        Console.WriteLine($"Faiz: {checkingInterest:C}\n");

        Console.WriteLine("InvestmentAccount (override kullanır):");
        investment.DisplayInfo();
        decimal investmentInterest = investment.CalculateInterest();
        Console.WriteLine($"Getiri: {investmentInterest:C}\n");
    }

    /// <summary>
    /// Base sınıf referansı ile polymorphic çağrı
    /// FARK BURADA ORTAYA ÇIKAR!
    /// </summary>
    static void DemonstratePolymorphicCall(SavingsAccount savings, CheckingAccount checking,
        InvestmentAccount investment)
    {
        Console.WriteLine("Base sınıf referansı ile polymorphic çağrı:\n");

        // Base referans - override çalışır ✅
        Account account1 = savings;
        Console.WriteLine("✅ SavingsAccount (override):");
        Console.WriteLine($"   Static Type: {nameof(Account)}");
        Console.WriteLine($"   Runtime Type: {account1.GetType().Name}");
        decimal interest1 = account1.CalculateInterest();
        Console.WriteLine($"   → Türetilmiş sınıf metodu çalıştı: {interest1:C}\n");

        // Base referans - new keyword, base metod çalışır! ❌
        Account account2 = checking;
        Console.WriteLine("❌ CheckingAccount (new - method hiding):");
        Console.WriteLine($"   Static Type: {nameof(Account)}");
        Console.WriteLine($"   Runtime Type: {account2.GetType().Name}");
        decimal interest2 = account2.CalculateInterest();
        Console.WriteLine($"   → Base metod çalıştı (sıfır döndü): {interest2:C}");
        Console.WriteLine($"   → BEKLENMEDİK DAVRANIŞ! Polymorphism bozuldu!\n");

        // Base referans - override çalışır ✅
        Account account3 = investment;
        Console.WriteLine("✅ InvestmentAccount (override):");
        Console.WriteLine($"   Static Type: {nameof(Account)}");
        Console.WriteLine($"   Runtime Type: {account3.GetType().Name}");
        decimal interest3 = account3.CalculateInterest();
        Console.WriteLine($"   → Türetilmiş sınıf metodu çalıştı: {interest3:C}\n");
    }

    /// <summary>
    /// Method hiding'in neden sorunlu olduğunu gösterir
    /// </summary>
    static void DemonstrateMethodHidingProblem()
    {
        CheckingAccount checking = new(2001, "Ali Çelik", 10000m, 25m);

        Console.WriteLine("Method Hiding Problemi:\n");

        // Senaryo 1: CheckingAccount referansı
        Console.WriteLine("1️⃣  CheckingAccount referansı:");
        decimal directResult = checking.CalculateInterest();
        Console.WriteLine($"   Sonuç: {directResult:C} ✅\n");

        // Senaryo 2: Account referansı (polymorphic)
        Account accountRef = checking;
        Console.WriteLine("2️⃣  Account referansı (aynı nesne):");
        decimal polymorphicResult = accountRef.CalculateInterest();
        Console.WriteLine($"   Sonuç: {polymorphicResult:C} ❌");
        Console.WriteLine($"   → Aynı nesne, farklı sonuç! Bug kaynağı!\n");

        // Karşılaştırma
        Console.WriteLine($"⚠️  Sonuçlar eşit mi? {directResult == polymorphicResult}");
        Console.WriteLine($"   Direct call: {directResult:C}");
        Console.WriteLine($"   Polymorphic call: {polymorphicResult:C}");
        Console.WriteLine($"   → Method hiding polymorphism'i bozar!");
    }

    /// <summary>
    /// Gerçek dünya senaryosu: Tüm hesapların faizini hesapla
    /// </summary>
    static void CalculateAllInterests()
    {
        // Banka, tüm hesapları Account koleksiyonunda tutar (polymorphism)
        List<Account> accounts = new()
        {
            new SavingsAccount(3001, "Elif Şahin", 75000m, 0.06m),
            new CheckingAccount(3002, "Can Öztürk", 30000m, 40m),
            new InvestmentAccount(3003, "Deniz Yıldız", 150000m, "Tahvil", 1.2m),
            new SavingsAccount(3004, "Fatma Aydın", 50000m, 0.05m)
        };

        decimal totalInterest = 0m;

        foreach (var account in accounts)
        {
            Console.WriteLine($"{account.Owner} ({account.GetType().Name}):");
            decimal interest = account.CalculateInterest();
            totalInterest += interest;
            Console.WriteLine($"Faiz/Getiri: {interest:C}\n");
        }

        Console.WriteLine($"💰 Toplam Faiz/Getiri: {totalInterest:C}");
        Console.WriteLine("\n⚠️  DİKKAT: CheckingAccount için sıfır faiz hesaplandı!");
        Console.WriteLine("   Çünkü 'new' keyword kullanıldı ve polymorphism bozuldu.");
        Console.WriteLine("   Çözüm: 'new' yerine 'override' kullan!");
    }
}
