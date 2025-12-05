namespace PatternMatching;

// ═══════════════════════════════════════════════════════════
// INVOICE SYSTEM MODELS
// ═══════════════════════════════════════════════════════════

/// <summary>
/// Invoice type enumeration
/// </summary>
public enum InvoiceType
{
    Standard,
    Premium,
    Wholesale,
    Subscription,
    OneTime
}

/// <summary>
/// Customer priority level
/// </summary>
public enum Priority
{
    Low,
    Normal,
    High,
    VIP
}

/// <summary>
/// Customer record (using record for positional patterns)
/// </summary>
public record Customer(string Name, Priority Priority, bool IsCorporate, int YearsActive);

/// <summary>
/// Base invoice class
/// </summary>
public abstract class Invoice
{
    public int Id { get; set; }
    public Customer Customer { get; set; }
    public decimal Amount { get; set; }
    public DateTime IssuedDate { get; set; }
    public bool IsPaid { get; set; }

    protected Invoice(int id, Customer customer, decimal amount)
    {
        Id = id;
        Customer = customer;
        Amount = amount;
        IssuedDate = DateTime.Now;
        IsPaid = false;
    }
}

/// <summary>
/// Standard invoice
/// </summary>
public class StandardInvoice : Invoice
{
    public int PaymentTermDays { get; set; }

    public StandardInvoice(int id, Customer customer, decimal amount, int paymentTermDays = 30)
        : base(id, customer, amount)
    {
        PaymentTermDays = paymentTermDays;
    }
}

/// <summary>
/// Premium invoice with discount
/// </summary>
public class PremiumInvoice : Invoice
{
    public decimal DiscountPercent { get; set; }

    public PremiumInvoice(int id, Customer customer, decimal amount, decimal discountPercent)
        : base(id, customer, amount)
    {
        DiscountPercent = discountPercent;
    }

    public decimal GetDiscountedAmount() => Amount * (1 - DiscountPercent / 100);
}

/// <summary>
/// Wholesale invoice with bulk discount
/// </summary>
public class WholesaleInvoice : Invoice
{
    public int Quantity { get; set; }
    public decimal BulkDiscountPercent { get; set; }

    public WholesaleInvoice(int id, Customer customer, decimal amount, int quantity, decimal bulkDiscount)
        : base(id, customer, amount)
    {
        Quantity = quantity;
        BulkDiscountPercent = bulkDiscount;
    }
}

/// <summary>
/// Subscription invoice (recurring)
/// </summary>
public class SubscriptionInvoice : Invoice
{
    public int BillingCycleMonths { get; set; }
    public DateTime? NextBillingDate { get; set; }

    public SubscriptionInvoice(int id, Customer customer, decimal amount, int billingCycleMonths)
        : base(id, customer, amount)
    {
        BillingCycleMonths = billingCycleMonths;
        NextBillingDate = IssuedDate.AddMonths(billingCycleMonths);
    }
}

// ═══════════════════════════════════════════════════════════
// PATTERN MATCHING DEMONSTRATIONS
// ═══════════════════════════════════════════════════════════

class Program
{
    static void Main()
    {
        Console.WriteLine("╔═══════════════════════════════════════════════════════════╗");
        Console.WriteLine("║   🔍 PATTERN MATCHING - INVOICE PROCESSING SYSTEM     ║");
        Console.WriteLine("╚═══════════════════════════════════════════════════════════╝\n");

        Console.WriteLine("═══ 1. Switch Expressions (C# 8.0) ═══\n");
        DemonstrateSwitchExpressions();

        Console.WriteLine("\n═══ 2. Property Patterns ═══\n");
        DemonstratePropertyPatterns();

        Console.WriteLine("\n═══ 3. Type Patterns ═══\n");
        DemonstrateTypePatterns();

        Console.WriteLine("\n═══ 4. Relational Patterns (C# 9.0) ═══\n");
        DemonstrateRelationalPatterns();

        Console.WriteLine("\n═══ 5. Logical Patterns (and, or, not) ═══\n");
        DemonstrateLogicalPatterns();

        Console.WriteLine("\n═══ 6. Positional Patterns (Records) ═══\n");
        DemonstratePositionalPatterns();

        Console.WriteLine("\n═══ 7. List Patterns (C# 11.0) ═══\n");
        DemonstrateListPatterns();

        Console.WriteLine("\n═══ 8. Var Patterns ═══\n");
        DemonstrateVarPatterns();

        Console.WriteLine("\n═══ 9. Discard Patterns (_) ═══\n");
        DemonstrateDiscardPatterns();

        Console.WriteLine("\n═══ 10. Combined Patterns (Recursive) ═══\n");
        DemonstrateCombinedPatterns();

        Console.WriteLine("\n✅ ÖĞRENİLENLER:");
        Console.WriteLine("   • Switch expressions - Modern switch syntax");
        Console.WriteLine("   • Property patterns - Match on property values");
        Console.WriteLine("   • Type patterns - Match on type with declaration");
        Console.WriteLine("   • Relational patterns - <, >, <=, >= operators");
        Console.WriteLine("   • Logical patterns - and, or, not keywords");
        Console.WriteLine("   • Positional patterns - Deconstruction matching");
        Console.WriteLine("   • List patterns - Match array/list elements");
        Console.WriteLine("   • Var patterns - Capture values");
        Console.WriteLine("   • Discard patterns - Ignore with _");
        Console.WriteLine("   • Recursive patterns - Nested property matching");
    }

    // ═══════════════════════════════════════════════════════════
    // 1. SWITCH EXPRESSIONS
    // ═══════════════════════════════════════════════════════════

    static void DemonstrateSwitchExpressions()
    {
        Console.WriteLine("Switch expressions: Modern, concise switch syntax\n");

        var invoices = CreateSampleInvoices();

        foreach (var invoice in invoices)
        {
            // ✅ OLD WAY: Traditional switch statement
            string oldWay;
            switch (invoice)
            {
                case StandardInvoice:
                    oldWay = "Standard";
                    break;
                case PremiumInvoice:
                    oldWay = "Premium";
                    break;
                case WholesaleInvoice:
                    oldWay = "Wholesale";
                    break;
                default:
                    oldWay = "Other";
                    break;
            }

            // ✅ NEW WAY: Switch expression (C# 8.0)
            var invoiceType = invoice switch
            {
                StandardInvoice => "Standard",
                PremiumInvoice => "Premium",
                WholesaleInvoice => "Wholesale",
                SubscriptionInvoice => "Subscription",
                _ => "Unknown"  // _ = discard (default)
            };

            // Calculate processing fee using switch expression
            var processingFee = invoice.Amount switch
            {
                < 100 => 5.00m,
                < 500 => 10.00m,
                < 1000 => 15.00m,
                _ => 20.00m
            };

            Console.WriteLine($"  Invoice #{invoice.Id}: {invoiceType} - " +
                            $"Amount: ${invoice.Amount:F2} - Fee: ${processingFee:F2}");
        }
    }

    // ═══════════════════════════════════════════════════════════
    // 2. PROPERTY PATTERNS
    // ═══════════════════════════════════════════════════════════

    static void DemonstratePropertyPatterns()
    {
        Console.WriteLine("Property patterns: Match on property values\n");

        var invoices = CreateSampleInvoices();

        foreach (var invoice in invoices)
        {
            // ✅ Property pattern: Match on properties
            var status = invoice switch
            {
                { IsPaid: true } => "✅ Paid",
                { IsPaid: false, Amount: > 1000 } => "⚠️ Unpaid (Large)",
                { IsPaid: false, Amount: > 500 } => "⚠️ Unpaid (Medium)",
                { IsPaid: false } => "⚠️ Unpaid (Small)",
                _ => "Unknown"
            };

            // Match on nested properties (Customer.Priority)
            var urgency = invoice switch
            {
                { Customer.Priority: Priority.VIP } => "🔴 URGENT",
                { Customer.Priority: Priority.High } => "🟠 High",
                { Customer.Priority: Priority.Normal } => "🟡 Normal",
                _ => "🟢 Low"
            };

            Console.WriteLine($"  Invoice #{invoice.Id}: {status} - {urgency} - " +
                            $"Customer: {invoice.Customer.Name}");
        }
    }

    // ═══════════════════════════════════════════════════════════
    // 3. TYPE PATTERNS
    // ═══════════════════════════════════════════════════════════

    static void DemonstrateTypePatterns()
    {
        Console.WriteLine("Type patterns: Match on type and capture variable\n");

        var invoices = CreateSampleInvoices();

        foreach (var invoice in invoices)
        {
            // ✅ Type pattern: Match type and declare variable
            var description = invoice switch
            {
                StandardInvoice std => $"Standard (Payment term: {std.PaymentTermDays} days)",
                PremiumInvoice prm => $"Premium (Discount: {prm.DiscountPercent}% - " +
                                     $"Final: ${prm.GetDiscountedAmount():F2})",
                WholesaleInvoice whl => $"Wholesale (Qty: {whl.Quantity} - " +
                                       $"Discount: {whl.BulkDiscountPercent}%)",
                SubscriptionInvoice sub => $"Subscription (Cycle: {sub.BillingCycleMonths} months - " +
                                          $"Next: {sub.NextBillingDate?.ToShortDateString()})",
                _ => "Unknown type"
            };

            Console.WriteLine($"  Invoice #{invoice.Id}: {description}");
        }
    }

    // ═══════════════════════════════════════════════════════════
    // 4. RELATIONAL PATTERNS (C# 9.0)
    // ═══════════════════════════════════════════════════════════

    static void DemonstrateRelationalPatterns()
    {
        Console.WriteLine("Relational patterns: Use <, >, <=, >= operators\n");

        var invoices = CreateSampleInvoices();

        foreach (var invoice in invoices)
        {
            // ✅ Relational patterns with <, >, <=, >=
            var tier = invoice.Amount switch
            {
                < 100 => "Micro",
                >= 100 and < 500 => "Small",
                >= 500 and < 1000 => "Medium",
                >= 1000 and < 5000 => "Large",
                >= 5000 => "Enterprise"
            };

            // Calculate tax based on amount
            var taxRate = invoice.Amount switch
            {
                <= 100 => 0.05m,
                > 100 and <= 500 => 0.08m,
                > 500 and <= 1000 => 0.10m,
                > 1000 => 0.12m
            };

            var tax = invoice.Amount * taxRate;

            Console.WriteLine($"  Invoice #{invoice.Id}: {tier} tier - " +
                            $"Amount: ${invoice.Amount:F2} - " +
                            $"Tax ({taxRate * 100}%): ${tax:F2}");
        }
    }

    // ═══════════════════════════════════════════════════════════
    // 5. LOGICAL PATTERNS (and, or, not)
    // ═══════════════════════════════════════════════════════════

    static void DemonstrateLogicalPatterns()
    {
        Console.WriteLine("Logical patterns: Combine patterns with and, or, not\n");

        var invoices = CreateSampleInvoices();

        foreach (var invoice in invoices)
        {
            // ✅ Logical patterns: and, or, not
            var risk = invoice switch
            {
                // High risk: Unpaid AND (large amount OR VIP customer)
                { IsPaid: false, Amount: > 1000 } or
                { IsPaid: false, Customer.Priority: Priority.VIP } => "🔴 High Risk",

                // Medium risk: Unpaid AND medium amount
                { IsPaid: false, Amount: > 500 and < 1000 } => "🟠 Medium Risk",

                // Low risk: Paid OR small amount
                { IsPaid: true } or { Amount: < 500 } => "🟢 Low Risk",

                _ => "⚪ Unknown"
            };

            // Complex condition: NOT paid AND (corporate OR VIP)
            var requiresFollowup = invoice switch
            {
                { IsPaid: false } and ({ Customer.IsCorporate: true } or { Customer.Priority: Priority.VIP })
                    => true,
                _ => false
            };

            var followupText = requiresFollowup ? "📞 Requires Followup" : "";

            Console.WriteLine($"  Invoice #{invoice.Id}: {risk} - " +
                            $"${invoice.Amount:F2} {followupText}");
        }
    }

    // ═══════════════════════════════════════════════════════════
    // 6. POSITIONAL PATTERNS (Records)
    // ═══════════════════════════════════════════════════════════

    static void DemonstratePositionalPatterns()
    {
        Console.WriteLine("Positional patterns: Deconstruct and match record components\n");

        var customers = new[]
        {
            new Customer("Alice Corp", Priority.VIP, true, 5),
            new Customer("Bob's Shop", Priority.High, false, 3),
            new Customer("Charlie LLC", Priority.Normal, true, 1),
            new Customer("Dave Store", Priority.Low, false, 0)
        };

        foreach (var customer in customers)
        {
            // ✅ Positional pattern: Deconstruct record
            var category = customer switch
            {
                (_, Priority.VIP, true, > 3) => "🌟 Platinum Partner",
                (_, Priority.VIP, _, _) => "⭐ VIP Customer",
                (_, Priority.High, true, > 2) => "💼 Premium Corporate",
                (_, _, true, _) => "🏢 Corporate",
                (_, _, _, > 5) => "🎖️ Loyal Customer",
                (_, _, _, 0) => "🆕 New Customer",
                _ => "👤 Regular Customer"
            };

            // Deconstruct specific values
            var (name, priority, isCorporate, years) = customer;

            Console.WriteLine($"  {name}: {category}");
            Console.WriteLine($"    Priority: {priority}, Corporate: {isCorporate}, " +
                            $"Years Active: {years}");
        }
    }

    // ═══════════════════════════════════════════════════════════
    // 7. LIST PATTERNS (C# 11.0)
    // ═══════════════════════════════════════════════════════════

    static void DemonstrateListPatterns()
    {
        Console.WriteLine("List patterns: Match on array/list elements (C# 11+)\n");

        // Invoice batches (arrays of amounts)
        var batches = new[]
        {
            new[] { 100m, 200m, 300m },
            new[] { 500m },
            new[] { 1000m, 2000m },
            new[] { 50m, 60m, 70m, 80m, 90m },
            Array.Empty<decimal>()
        };

        for (int i = 0; i < batches.Length; i++)
        {
            var batch = batches[i];

            // ✅ List patterns (C# 11)
            var analysis = batch switch
            {
                [] => "Empty batch",
                [var single] => $"Single invoice: ${single:F2}",
                [var first, var second] => $"Two invoices: ${first:F2}, ${second:F2}",
                [var first, .., var last] => $"Multiple invoices: ${first:F2} ... ${last:F2} " +
                                           $"({batch.Length} total)",
                _ => "Unknown"
            };

            var total = batch.Sum();

            Console.WriteLine($"  Batch {i + 1}: {analysis} - Total: ${total:F2}");

            // Match on specific patterns
            var batchType = batch switch
            {
                [> 1000, ..] => "🔴 Starts with large invoice",
                [.., > 1000] => "🔴 Ends with large invoice",
                [< 100, < 100, ..] => "🟢 Starts with small invoices",
                _ => "⚪ Mixed"
            };

            Console.WriteLine($"    Type: {batchType}");
        }
    }

    // ═══════════════════════════════════════════════════════════
    // 8. VAR PATTERNS
    // ═══════════════════════════════════════════════════════════

    static void DemonstrateVarPatterns()
    {
        Console.WriteLine("Var patterns: Capture matched values\n");

        var invoices = CreateSampleInvoices();

        foreach (var invoice in invoices)
        {
            // ✅ Var pattern: Capture value for reuse
            var message = invoice switch
            {
                { Amount: var amt } when amt > 1000 =>
                    $"Large invoice of ${amt:F2} requires manager approval",

                PremiumInvoice { DiscountPercent: var disc } when disc > 10 =>
                    $"High discount of {disc}% applied",

                WholesaleInvoice { Quantity: var qty, Amount: var amt } =>
                    $"Bulk order: {qty} units at ${amt / qty:F2} each",

                { Customer.Priority: var prio, Amount: var amt } when prio == Priority.VIP =>
                    $"VIP customer invoice: ${amt:F2}",

                _ => "Standard processing"
            };

            Console.WriteLine($"  Invoice #{invoice.Id}: {message}");
        }
    }

    // ═══════════════════════════════════════════════════════════
    // 9. DISCARD PATTERNS (_)
    // ═══════════════════════════════════════════════════════════

    static void DemonstrateDiscardPatterns()
    {
        Console.WriteLine("Discard patterns: Ignore values with underscore\n");

        var invoices = CreateSampleInvoices();

        foreach (var invoice in invoices)
        {
            // ✅ Discard pattern: Ignore specific values
            var processing = invoice switch
            {
                // Match type but ignore the variable
                StandardInvoice _ => "Process as standard",
                PremiumInvoice _ => "Process with discount",

                // Ignore specific property but match others
                { IsPaid: true, Amount: _, Customer: _ } => "Already paid - archive",

                // Positional pattern with discards
                SubscriptionInvoice { BillingCycleMonths: var months } =>
                    $"Recurring - Next in {months} months",

                _ => "Default processing"
            };

            Console.WriteLine($"  Invoice #{invoice.Id}: {processing}");
        }
    }

    // ═══════════════════════════════════════════════════════════
    // 10. COMBINED PATTERNS (Recursive)
    // ═══════════════════════════════════════════════════════════

    static void DemonstrateCombinedPatterns()
    {
        Console.WriteLine("Combined patterns: Complex nested pattern matching\n");

        var invoices = CreateSampleInvoices();

        foreach (var invoice in invoices)
        {
            // ✅ Recursive/Combined patterns: Deep property matching
            var action = invoice switch
            {
                // Complex: Type + Property + Relational + Logical
                PremiumInvoice
                {
                    IsPaid: false,
                    DiscountPercent: > 15,
                    Amount: > 500 and < 2000,
                    Customer: { Priority: Priority.VIP or Priority.High, IsCorporate: true }
                } => "🎁 Send thank you gift + Apply extra 5% discount",

                // Nested property matching
                WholesaleInvoice
                {
                    Quantity: > 100,
                    Amount: > 5000,
                    Customer: { IsCorporate: true, YearsActive: > 3 }
                } => "💼 Assign dedicated account manager",

                // Combined with var pattern
                StandardInvoice
                {
                    IsPaid: false,
                    PaymentTermDays: var days,
                    Customer.Priority: Priority.VIP
                } when days < 30 =>
                    $"⚡ Expedite - VIP customer ({days} day terms)",

                // Type + Amount range + Customer properties
                SubscriptionInvoice
                {
                    Amount: >= 100 and < 500,
                    Customer: { Priority: not Priority.Low, YearsActive: > 0 }
                } => "📧 Send renewal offer with 10% discount",

                // Unpaid + High amount + Not corporate
                { IsPaid: false, Amount: > 1000, Customer.IsCorporate: false } =>
                    "📞 Contact for payment plan options",

                // Paid + Corporate + High amount
                { IsPaid: true, Amount: > 2000, Customer: { IsCorporate: true } } =>
                    "✅ Send receipt + Request testimonial",

                // Default
                _ => "📋 Standard processing"
            };

            Console.WriteLine($"  Invoice #{invoice.Id} ({invoice.GetType().Name}):");
            Console.WriteLine($"    Amount: ${invoice.Amount:F2}, Paid: {invoice.IsPaid}");
            Console.WriteLine($"    Customer: {invoice.Customer.Name} ({invoice.Customer.Priority})");
            Console.WriteLine($"    Action: {action}\n");
        }
    }

    // ═══════════════════════════════════════════════════════════
    // HELPER METHODS
    // ═══════════════════════════════════════════════════════════

    static List<Invoice> CreateSampleInvoices()
    {
        var customers = new[]
        {
            new Customer("Alice Corp", Priority.VIP, true, 5),
            new Customer("Bob's Shop", Priority.High, false, 3),
            new Customer("Charlie LLC", Priority.Normal, true, 1),
            new Customer("Dave Store", Priority.Low, false, 0)
        };

        return new List<Invoice>
        {
            new StandardInvoice(1, customers[0], 1500m, 30),
            new PremiumInvoice(2, customers[1], 800m, 15m),
            new WholesaleInvoice(3, customers[2], 5000m, 150, 20m),
            new SubscriptionInvoice(4, customers[3], 299m, 12),
            new StandardInvoice(5, customers[0], 250m, 15) { IsPaid = true },
            new PremiumInvoice(6, customers[1], 1200m, 10m),
            new WholesaleInvoice(7, customers[2], 8000m, 200, 25m) { IsPaid = true }
        };
    }
}
