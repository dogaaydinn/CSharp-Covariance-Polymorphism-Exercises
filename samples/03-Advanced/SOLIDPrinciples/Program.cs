// SOLID Principles Demonstration
// Her prensip için BEFORE/AFTER örnekleri

using SOLIDPrinciples.SingleResponsibility;
using SOLIDPrinciples.OpenClosed;
using SOLIDPrinciples.LiskovSubstitution;
using SOLIDPrinciples.InterfaceSegregation;
using SOLIDPrinciples.DependencyInversion;

namespace SOLIDPrinciples;

public class Program
{
    public static void Main()
    {
        Console.WriteLine("=== SOLID Principles Demo ===\n");

        // 1. Single Responsibility Principle
        Console.WriteLine("1️⃣  SINGLE RESPONSIBILITY PRINCIPLE");
        Console.WriteLine("❌ BAD: One class doing everything");
        var badReport = new BadReportGenerator();
        badReport.GenerateAndSendReport("Sales data");

        Console.WriteLine("\n✅ GOOD: Separated responsibilities");
        var reportGen = new ReportGenerator();
        var emailSender = new EmailSender();
        var report = reportGen.Generate("Sales data");
        emailSender.Send(report);

        // 2. Open/Closed Principle
        Console.WriteLine("\n2️⃣  OPEN/CLOSED PRINCIPLE");
        Console.WriteLine("❌ BAD: Modifying class for new discount types");
        var badCalculator = new BadDiscountCalculator();
        Console.WriteLine($"Student discount: {badCalculator.Calculate(100, "Student")}");

        Console.WriteLine("\n✅ GOOD: Extending with new classes");
        IDiscountStrategy studentDiscount = new StudentDiscount();
        IDiscountStrategy seniorDiscount = new SeniorDiscount();
        Console.WriteLine($"Student: {studentDiscount.Calculate(100)}");
        Console.WriteLine($"Senior: {seniorDiscount.Calculate(100)}");

        // 3. Liskov Substitution Principle
        Console.WriteLine("\n3️⃣  LISKOV SUBSTITUTION PRINCIPLE");
        Console.WriteLine("❌ BAD: Penguin breaks Bird contract");
        try
        {
            BadBird penguin = new BadPenguin();
            penguin.Fly(); // Throws exception!
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }

        Console.WriteLine("\n✅ GOOD: Proper abstraction");
        ISwimmingBird properPenguin = new Penguin();
        properPenguin.Swim();

        // 4. Interface Segregation Principle
        Console.WriteLine("\n4️⃣  INTERFACE SEGREGATION PRINCIPLE");
        Console.WriteLine("❌ BAD: Fat interface forcing empty implementations");
        IBadPrinter basicPrinter = new BasicPrinter();
        basicPrinter.Print("Doc");
        // basicPrinter.Scan("Doc"); // Would throw NotImplementedException

        Console.WriteLine("\n✅ GOOD: Segregated interfaces");
        IPrinter printer = new SimplePrinter();
        printer.Print("Document");

        IMultifunctionDevice mfp = new MultifunctionPrinter();
        mfp.Print("Document");
        mfp.Scan("Document");
        mfp.Fax("Document");

        // 5. Dependency Inversion Principle
        Console.WriteLine("\n5️⃣  DEPENDENCY INVERSION PRINCIPLE");
        Console.WriteLine("❌ BAD: High-level depends on low-level concrete class");
        var badService = new BadNotificationService();
        badService.Notify("Hello");

        Console.WriteLine("\n✅ GOOD: Both depend on abstraction");
        INotifier emailNotifier = new EmailNotifier();
        INotifier smsNotifier = new SmsNotifier();
        var goodService1 = new NotificationService(emailNotifier);
        var goodService2 = new NotificationService(smsNotifier);
        goodService1.Notify("Via Email");
        goodService2.Notify("Via SMS");

        Console.WriteLine("\n=== SOLID Principles Applied! ===");
    }
}
