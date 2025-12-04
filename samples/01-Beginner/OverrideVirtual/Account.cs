namespace OverrideVirtual;

/// <summary>
/// Base hesap sınıfı - Virtual method tanımlar
/// </summary>
public class Account
{
    public int AccountNumber { get; set; }
    public string Owner { get; set; }
    public decimal Balance { get; set; }

    public Account(int accountNumber, string owner, decimal balance)
    {
        AccountNumber = accountNumber;
        Owner = owner;
        Balance = balance;
    }

    /// <summary>
    /// Virtual method - Türetilmiş sınıflar override edebilir
    /// </summary>
    public virtual decimal CalculateInterest()
    {
        // Base implementasyon - varsayılan %0
        return 0m;
    }

    public virtual void DisplayInfo()
    {
        Console.WriteLine($"Hesap: {AccountNumber}, Sahip: {Owner}, Bakiye: {Balance:C}");
    }
}

/// <summary>
/// Tasarruf hesabı - DOĞRU yaklaşım: override kullanır
/// </summary>
public class SavingsAccount : Account
{
    public decimal InterestRate { get; set; }

    public SavingsAccount(int accountNumber, string owner, decimal balance, decimal interestRate)
        : base(accountNumber, owner, balance)
    {
        InterestRate = interestRate;
    }

    /// <summary>
    /// ✅ OVERRIDE: Polymorphic davranış korunur
    /// Base referans üzerinden çağrıldığında bu metod çalışır
    /// </summary>
    public override decimal CalculateInterest()
    {
        decimal interest = Balance * InterestRate;
        Console.WriteLine($"   [Override] Tasarruf faizi hesaplandı: {interest:C}");
        return interest;
    }

    public override void DisplayInfo()
    {
        base.DisplayInfo();
        Console.WriteLine($"   Tür: Tasarruf Hesabı, Faiz: %{InterestRate * 100:F2}");
    }
}

/// <summary>
/// Vadesiz hesap - YANLIŞ yaklaşım: new keyword kullanır (method hiding)
/// </summary>
public class CheckingAccount : Account
{
    public decimal MonthlyFee { get; set; }

    public CheckingAccount(int accountNumber, string owner, decimal balance, decimal monthlyFee)
        : base(accountNumber, owner, balance)
    {
        MonthlyFee = monthlyFee;
    }

    /// <summary>
    /// ❌ METHOD HIDING: Polymorphic davranış bozulur!
    /// Base referans üzerinden çağrıldığında base metod çalışır
    /// Compiler Warning: CS0114
    /// </summary>
    public new decimal CalculateInterest()
    {
        decimal interest = Balance * 0.01m; // %1 düşük faiz
        Console.WriteLine($"   [New/Hidden] Vadesiz faizi hesaplandı: {interest:C}");
        return interest;
    }

    public override void DisplayInfo()
    {
        base.DisplayInfo();
        Console.WriteLine($"   Tür: Vadesiz Hesap, Aylık Ücret: {MonthlyFee:C}");
    }
}

/// <summary>
/// Yatırım hesabı - DOĞRU yaklaşım: override kullanır
/// </summary>
public class InvestmentAccount : Account
{
    public string InvestmentType { get; set; }
    public decimal RiskFactor { get; set; }

    public InvestmentAccount(int accountNumber, string owner, decimal balance,
        string investmentType, decimal riskFactor)
        : base(accountNumber, owner, balance)
    {
        InvestmentType = investmentType;
        RiskFactor = riskFactor;
    }

    /// <summary>
    /// ✅ OVERRIDE: Polymorphic davranış korunur
    /// </summary>
    public override decimal CalculateInterest()
    {
        // Risk faktörüne göre değişken faiz
        decimal interest = Balance * 0.08m * RiskFactor;
        Console.WriteLine($"   [Override] Yatırım getirisi hesaplandı: {interest:C} (Risk: x{RiskFactor})");
        return interest;
    }

    public override void DisplayInfo()
    {
        base.DisplayInfo();
        Console.WriteLine($"   Tür: Yatırım Hesabı ({InvestmentType}), Risk: x{RiskFactor}");
    }
}
