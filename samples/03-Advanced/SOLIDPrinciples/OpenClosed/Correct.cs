namespace SOLIDPrinciples.OpenClosed;

/// <summary>
/// CORRECT: Open for extension, closed for modification.
/// Benefits:
/// - Add new payment methods without modifying existing code
/// - Existing code remains stable and tested
/// - Can extend functionality through inheritance or composition
/// - Plugin architecture becomes possible
/// </summary>

#region Payment Processing Example

/// <summary>
/// Abstract base class - closed for modification, open for extension
/// </summary>
public abstract class PaymentMethod
{
    public abstract string Name { get; }

    public void Process(decimal amount, string details)
    {
        Console.WriteLine($"\n[CORRECT] Processing {Name} payment: ${amount}");
        ValidatePayment(amount, details);
        ExecutePayment(amount, details);
        Console.WriteLine($"  SUCCESS: {Name} payment processed");
    }

    protected virtual void ValidatePayment(decimal amount, string details)
    {
        if (amount <= 0)
        {
            throw new ArgumentException("Amount must be positive");
        }
    }

    protected abstract void ExecutePayment(decimal amount, string details);
}

// Existing payment methods - NEVER need to modify these
public class CreditCardPayment : PaymentMethod
{
    public override string Name => "Credit Card";

    protected override void ExecutePayment(decimal amount, string cardNumber)
    {
        Console.WriteLine($"  Charging ${amount} to card {cardNumber}");
        Console.WriteLine("  Contacting payment gateway");
        Console.WriteLine("  Transaction approved");
    }
}

public class PayPalPayment : PaymentMethod
{
    public override string Name => "PayPal";

    protected override void ExecutePayment(decimal amount, string email)
    {
        Console.WriteLine($"  Transferring ${amount} to PayPal account {email}");
        Console.WriteLine("  Connecting to PayPal API");
        Console.WriteLine("  Transfer completed");
    }
}

public class BankTransferPayment : PaymentMethod
{
    public override string Name => "Bank Transfer";

    protected override void ExecutePayment(decimal amount, string accountNumber)
    {
        Console.WriteLine($"  Initiating bank transfer of ${amount} to {accountNumber}");
        Console.WriteLine("  Processing through banking network");
        Console.WriteLine("  Transfer scheduled");
    }
}

// NEW PAYMENT METHOD - Just extend, no modification!
public class BitcoinPayment : PaymentMethod
{
    public override string Name => "Bitcoin";

    protected override void ExecutePayment(decimal amount, string walletAddress)
    {
        Console.WriteLine($"  Converting ${amount} to BTC");
        Console.WriteLine($"  Sending to wallet {walletAddress}");
        Console.WriteLine("  Transaction broadcast to blockchain");
    }

    protected override void ValidatePayment(decimal amount, string details)
    {
        base.ValidatePayment(amount, details);

        // Bitcoin-specific validation
        if (!details.StartsWith("bc1") && !details.StartsWith("1") && !details.StartsWith("3"))
        {
            throw new ArgumentException("Invalid Bitcoin wallet address");
        }
    }
}

// NEW PAYMENT METHOD - Just extend!
public class ApplePayPayment : PaymentMethod
{
    public override string Name => "Apple Pay";

    protected override void ExecutePayment(decimal amount, string deviceToken)
    {
        Console.WriteLine($"  Processing Apple Pay transaction: ${amount}");
        Console.WriteLine($"  Device token: {deviceToken}");
        Console.WriteLine("  Payment authorized via Touch ID/Face ID");
    }
}

/// <summary>
/// Payment processor that works with any PaymentMethod - never needs modification!
/// </summary>
public class PaymentProcessor
{
    private readonly List<PaymentMethod> _supportedMethods = new();

    public void RegisterPaymentMethod(PaymentMethod method)
    {
        _supportedMethods.Add(method);
        Console.WriteLine($"[CORRECT] Registered new payment method: {method.Name}");
    }

    public void ProcessPayment(PaymentMethod paymentMethod, decimal amount, string details)
    {
        paymentMethod.Process(amount, details);
    }
}

#endregion

#region Report Generation Example

/// <summary>
/// Report generator interface - open for extension
/// </summary>
public interface IReportGenerator
{
    string FormatName { get; }
    void Generate(List<string> data);
}

// Existing report formats - NEVER need to modify
public class PdfReportGenerator : IReportGenerator
{
    public string FormatName => "PDF";

    public void Generate(List<string> data)
    {
        Console.WriteLine($"\n[CORRECT] Generating {FormatName} report");
        Console.WriteLine("  Creating PDF document");
        Console.WriteLine($"  Adding {data.Count} records");
        Console.WriteLine("  Applying formatting and styles");
        Console.WriteLine("  SUCCESS: PDF report generated");
    }
}

public class ExcelReportGenerator : IReportGenerator
{
    public string FormatName => "Excel";

    public void Generate(List<string> data)
    {
        Console.WriteLine($"\n[CORRECT] Generating {FormatName} report");
        Console.WriteLine("  Creating Excel workbook");
        Console.WriteLine($"  Adding {data.Count} rows");
        Console.WriteLine("  Applying cell formatting");
        Console.WriteLine("  SUCCESS: Excel report generated");
    }
}

// NEW FORMAT - Just implement interface!
public class JsonReportGenerator : IReportGenerator
{
    public string FormatName => "JSON";

    public void Generate(List<string> data)
    {
        Console.WriteLine($"\n[CORRECT] Generating {FormatName} report");
        Console.WriteLine("  Creating JSON structure");
        Console.WriteLine($"  Serializing {data.Count} objects");
        Console.WriteLine("  Formatting with indentation");
        Console.WriteLine("  SUCCESS: JSON report generated");
    }
}

// NEW FORMAT - Just implement interface!
public class XmlReportGenerator : IReportGenerator
{
    public string FormatName => "XML";

    public void Generate(List<string> data)
    {
        Console.WriteLine($"\n[CORRECT] Generating {FormatName} report");
        Console.WriteLine("  Creating XML document");
        Console.WriteLine($"  Adding {data.Count} elements");
        Console.WriteLine("  Validating against schema");
        Console.WriteLine("  SUCCESS: XML report generated");
    }
}

/// <summary>
/// Report service that works with any IReportGenerator - never needs modification!
/// </summary>
public class ReportService
{
    private readonly List<IReportGenerator> _generators = new();

    public void RegisterGenerator(IReportGenerator generator)
    {
        _generators.Add(generator);
        Console.WriteLine($"[CORRECT] Registered report format: {generator.FormatName}");
    }

    public void GenerateReport(IReportGenerator generator, List<string> data)
    {
        generator.Generate(data);
    }
}

#endregion

#region Discount Calculation Example

/// <summary>
/// Discount strategy interface - open for extension
/// </summary>
public interface IDiscountStrategy
{
    string Name { get; }
    decimal CalculateDiscount(decimal orderAmount);
}

// Existing strategies - NEVER need to modify
public class RegularCustomerDiscount : IDiscountStrategy
{
    public string Name => "Regular Customer";

    public decimal CalculateDiscount(decimal orderAmount)
    {
        var discount = orderAmount * 0.05m; // 5%
        Console.WriteLine($"  {Name}: 5% discount = ${discount:F2}");
        return discount;
    }
}

public class PremiumCustomerDiscount : IDiscountStrategy
{
    public string Name => "Premium Customer";

    public decimal CalculateDiscount(decimal orderAmount)
    {
        var discount = orderAmount * 0.10m; // 10%
        Console.WriteLine($"  {Name}: 10% discount = ${discount:F2}");
        return discount;
    }
}

// NEW STRATEGY - Just implement interface!
public class VipCustomerDiscount : IDiscountStrategy
{
    public string Name => "VIP Customer";

    public decimal CalculateDiscount(decimal orderAmount)
    {
        var discount = orderAmount * 0.20m; // 20%
        Console.WriteLine($"  {Name}: 20% discount = ${discount:F2}");
        return discount;
    }
}

// NEW STRATEGY - Seasonal promotion!
public class BlackFridayDiscount : IDiscountStrategy
{
    public string Name => "Black Friday Sale";

    public decimal CalculateDiscount(decimal orderAmount)
    {
        // Progressive discount based on order amount
        decimal discountPercent = orderAmount switch
        {
            >= 500 => 0.30m,  // 30% for orders >= $500
            >= 200 => 0.20m,  // 20% for orders >= $200
            >= 100 => 0.15m,  // 15% for orders >= $100
            _ => 0.10m        // 10% for all other orders
        };

        var discount = orderAmount * discountPercent;
        Console.WriteLine($"  {Name}: {discountPercent * 100}% discount = ${discount:F2}");
        return discount;
    }
}

// NEW STRATEGY - Loyalty points!
public class LoyaltyPointsDiscount : IDiscountStrategy
{
    private readonly int _pointsAvailable;
    private const decimal PointValue = 0.01m; // $0.01 per point

    public string Name => "Loyalty Points";

    public LoyaltyPointsDiscount(int pointsAvailable)
    {
        _pointsAvailable = pointsAvailable;
    }

    public decimal CalculateDiscount(decimal orderAmount)
    {
        var maxDiscount = _pointsAvailable * PointValue;
        var discount = Math.Min(maxDiscount, orderAmount * 0.50m); // Max 50% discount
        Console.WriteLine($"  {Name}: Using {_pointsAvailable} points = ${discount:F2}");
        return discount;
    }
}

/// <summary>
/// Discount calculator that works with any strategy - never needs modification!
/// </summary>
public class DiscountCalculator
{
    public decimal ApplyDiscount(IDiscountStrategy strategy, decimal orderAmount)
    {
        Console.WriteLine($"\n[CORRECT] Applying {strategy.Name} discount");
        return strategy.CalculateDiscount(orderAmount);
    }

    // Can even combine multiple strategies!
    public decimal ApplyMultipleDiscounts(List<IDiscountStrategy> strategies, decimal orderAmount)
    {
        Console.WriteLine($"\n[CORRECT] Applying multiple discounts to ${orderAmount}");
        var totalDiscount = 0m;

        foreach (var strategy in strategies)
        {
            totalDiscount += strategy.CalculateDiscount(orderAmount - totalDiscount);
        }

        Console.WriteLine($"  Total discount: ${totalDiscount:F2}");
        return totalDiscount;
    }
}

#endregion

/// <summary>
/// Demonstrates the benefits of Open/Closed Principle
/// </summary>
public class OpenClosedDemo
{
    public static void DemonstrateBenefits()
    {
        Console.WriteLine("\n=== BENEFITS OF OPEN/CLOSED PRINCIPLE ===");

        Console.WriteLine("\nBenefit 1: Add new features without modifying existing code");
        Console.WriteLine("  - Existing payment methods remain untouched and stable");
        Console.WriteLine("  - No need to retest existing functionality");

        Console.WriteLine("\nBenefit 2: Easy to extend at runtime");
        Console.WriteLine("  - Can load payment methods from plugins");
        Console.WriteLine("  - Can add new strategies dynamically");

        Console.WriteLine("\nBenefit 3: Reduced risk of bugs");
        Console.WriteLine("  - New code can't break old code");
        Console.WriteLine("  - Each extension is isolated");

        DemonstratePaymentProcessing();
        DemonstrateReportGeneration();
        DemonstrateDiscountStrategies();
    }

    private static void DemonstratePaymentProcessing()
    {
        Console.WriteLine("\n--- Payment Processing Example ---");

        var processor = new PaymentProcessor();

        // Register payment methods (could be loaded from plugins!)
        processor.RegisterPaymentMethod(new CreditCardPayment());
        processor.RegisterPaymentMethod(new PayPalPayment());
        processor.RegisterPaymentMethod(new BitcoinPayment());
        processor.RegisterPaymentMethod(new ApplePayPayment());

        // Process payments - works with any payment method!
        processor.ProcessPayment(new CreditCardPayment(), 100.00m, "1234-5678-9012-3456");
        processor.ProcessPayment(new BitcoinPayment(), 250.00m, "bc1qxy2kgdygjrsqtzq2n0yrf2493p83kkfjhx0wlh");
    }

    private static void DemonstrateReportGeneration()
    {
        Console.WriteLine("\n--- Report Generation Example ---");

        var reportService = new ReportService();
        var data = new List<string> { "Record1", "Record2", "Record3", "Record4", "Record5" };

        // Register report generators
        reportService.RegisterGenerator(new PdfReportGenerator());
        reportService.RegisterGenerator(new ExcelReportGenerator());
        reportService.RegisterGenerator(new JsonReportGenerator());
        reportService.RegisterGenerator(new XmlReportGenerator());

        // Generate reports - works with any format!
        reportService.GenerateReport(new PdfReportGenerator(), data);
        reportService.GenerateReport(new JsonReportGenerator(), data);
    }

    private static void DemonstrateDiscountStrategies()
    {
        Console.WriteLine("\n--- Discount Strategies Example ---");

        var calculator = new DiscountCalculator();
        var orderAmount = 300.00m;

        // Apply different discount strategies
        calculator.ApplyDiscount(new RegularCustomerDiscount(), orderAmount);
        calculator.ApplyDiscount(new VipCustomerDiscount(), orderAmount);
        calculator.ApplyDiscount(new BlackFridayDiscount(), orderAmount);
        calculator.ApplyDiscount(new LoyaltyPointsDiscount(5000), orderAmount);

        // Combine multiple strategies!
        Console.WriteLine("\n--- Combining Multiple Strategies ---");
        var strategies = new List<IDiscountStrategy>
        {
            new VipCustomerDiscount(),
            new LoyaltyPointsDiscount(2000)
        };
        calculator.ApplyMultipleDiscounts(strategies, orderAmount);
    }
}
