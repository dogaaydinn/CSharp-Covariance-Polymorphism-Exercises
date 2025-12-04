namespace SOLIDPrinciples.OpenClosed;

/// <summary>
/// VIOLATION: Must modify existing code to add new functionality.
/// Problem: Classes are not open for extension, forcing modifications.
/// Consequences:
/// - Every new payment method requires modifying this class
/// - High risk of breaking existing payment methods
/// - Violates "closed for modification" principle
/// - Can't add new features without changing tested code
/// </summary>
public class PaymentProcessorViolation
{
    public void ProcessPayment(string paymentType, decimal amount, string details)
    {
        Console.WriteLine($"\n[VIOLATION] Processing {paymentType} payment: ${amount}");

        // Must add new if-else branches for each payment type
        if (paymentType == "CreditCard")
        {
            ProcessCreditCard(amount, details);
        }
        else if (paymentType == "PayPal")
        {
            ProcessPayPal(amount, details);
        }
        else if (paymentType == "BankTransfer")
        {
            ProcessBankTransfer(amount, details);
        }
        // What if we need to add Bitcoin? Must modify this method!
        // What if we need to add ApplePay? Must modify this method!
        else
        {
            Console.WriteLine("  ERROR: Unknown payment type!");
        }
    }

    private void ProcessCreditCard(decimal amount, string cardNumber)
    {
        Console.WriteLine($"  Processing credit card: {cardNumber}");
        Console.WriteLine($"  Charging ${amount} to card");
        Console.WriteLine("  SUCCESS: Credit card payment processed");
    }

    private void ProcessPayPal(decimal amount, string email)
    {
        Console.WriteLine($"  Processing PayPal: {email}");
        Console.WriteLine($"  Transferring ${amount} via PayPal");
        Console.WriteLine("  SUCCESS: PayPal payment processed");
    }

    private void ProcessBankTransfer(decimal amount, string accountNumber)
    {
        Console.WriteLine($"  Processing bank transfer: {accountNumber}");
        Console.WriteLine($"  Transferring ${amount} via bank");
        Console.WriteLine("  SUCCESS: Bank transfer processed");
    }

    // Problem: To add Bitcoin payment, we must modify this class!
    // This means retesting all existing payment methods!
}

/// <summary>
/// Another violation example: Report generator that must be modified for new formats
/// </summary>
public class ReportGeneratorViolation
{
    public void GenerateReport(string reportType, List<string> data)
    {
        Console.WriteLine($"\n[VIOLATION] Generating {reportType} report");

        // Must modify this method for each new report format
        if (reportType == "PDF")
        {
            GeneratePdfReport(data);
        }
        else if (reportType == "Excel")
        {
            GenerateExcelReport(data);
        }
        else if (reportType == "CSV")
        {
            GenerateCsvReport(data);
        }
        // Need JSON format? Must modify this class!
        // Need XML format? Must modify this class!
        else
        {
            Console.WriteLine("  ERROR: Unknown report type!");
        }
    }

    private void GeneratePdfReport(List<string> data)
    {
        Console.WriteLine("  Creating PDF document");
        Console.WriteLine($"  Adding {data.Count} records");
        Console.WriteLine("  SUCCESS: PDF report generated");
    }

    private void GenerateExcelReport(List<string> data)
    {
        Console.WriteLine("  Creating Excel workbook");
        Console.WriteLine($"  Adding {data.Count} rows");
        Console.WriteLine("  SUCCESS: Excel report generated");
    }

    private void GenerateCsvReport(List<string> data)
    {
        Console.WriteLine("  Creating CSV file");
        Console.WriteLine($"  Writing {data.Count} lines");
        Console.WriteLine("  SUCCESS: CSV report generated");
    }
}

/// <summary>
/// Demonstrates the problems with Open/Closed Principle violations
/// </summary>
public class OpenClosedViolationDemo
{
    public static void DemonstrateProblems()
    {
        Console.WriteLine("\n=== PROBLEMS WITH VIOLATING OPEN/CLOSED PRINCIPLE ===");

        var paymentProcessor = new PaymentProcessorViolation();

        Console.WriteLine("\nProblem 1: Must modify existing code for new features");
        Console.WriteLine("  Want to add Bitcoin? Must edit PaymentProcessorViolation class");
        Console.WriteLine("  Want to add ApplePay? Must edit PaymentProcessorViolation class");

        Console.WriteLine("\nProblem 2: High risk of breaking existing functionality");
        Console.WriteLine("  Adding Bitcoin might accidentally break credit card processing!");
        Console.WriteLine("  Must retest ALL payment methods after ANY change");

        Console.WriteLine("\nProblem 3: Growing if-else chains");
        Console.WriteLine("  ProcessPayment method becomes longer and longer");
        Console.WriteLine("  Harder to read and maintain");

        Console.WriteLine("\nProblem 4: Can't extend at runtime");
        Console.WriteLine("  Can't add new payment methods without recompiling");
        Console.WriteLine("  Can't load payment methods from plugins");

        // Demonstrate the violation
        Console.WriteLine("\n--- Running Violation Examples ---");
        paymentProcessor.ProcessPayment("CreditCard", 100.00m, "1234-5678-9012-3456");
        paymentProcessor.ProcessPayment("PayPal", 50.00m, "user@example.com");

        // Try to add new payment type - FAILS!
        Console.WriteLine("\n--- Trying to add Bitcoin payment ---");
        Console.WriteLine("ERROR: Must modify PaymentProcessorViolation class!");
        Console.WriteLine("       Must add new if-else branch");
        Console.WriteLine("       Must retest all existing payment methods");

        // Report generator example
        var reportGenerator = new ReportGeneratorViolation();
        reportGenerator.GenerateReport("PDF", new List<string> { "Data1", "Data2", "Data3" });

        Console.WriteLine("\n--- Trying to add JSON report ---");
        Console.WriteLine("ERROR: Must modify ReportGeneratorViolation class!");
    }
}

/// <summary>
/// Another common violation: Discount calculator with hardcoded rules
/// </summary>
public class DiscountCalculatorViolation
{
    public decimal CalculateDiscount(string customerType, decimal orderAmount)
    {
        Console.WriteLine($"\n[VIOLATION] Calculating discount for {customerType}");

        // Must modify this method for each new customer type or promotion
        if (customerType == "Regular")
        {
            var discount = orderAmount * 0.05m; // 5% discount
            Console.WriteLine($"  Regular customer: 5% discount = ${discount}");
            return discount;
        }
        else if (customerType == "Premium")
        {
            var discount = orderAmount * 0.10m; // 10% discount
            Console.WriteLine($"  Premium customer: 10% discount = ${discount}");
            return discount;
        }
        else if (customerType == "VIP")
        {
            var discount = orderAmount * 0.20m; // 20% discount
            Console.WriteLine($"  VIP customer: 20% discount = ${discount}");
            return discount;
        }
        // Need to add Gold customer? Must modify this method!
        // Need to add seasonal promotion? Must modify this method!
        else
        {
            Console.WriteLine($"  Unknown customer type: No discount");
            return 0;
        }
    }
}
