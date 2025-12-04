namespace SOLIDPrinciples.SingleResponsibility;

// ❌ BAD: Violates SRP - one class doing multiple things
public class BadReportGenerator
{
    public void GenerateAndSendReport(string data)
    {
        // Report generation logic
        string report = $"Report: {data}";
        Console.WriteLine($"Generated: {report}");

        // Email sending logic (should be separate!)
        Console.WriteLine($"Sending email with report...");
        Console.WriteLine($"Email sent!");
    }
}

// ✅ GOOD: Single Responsibility - only generates reports
public class ReportGenerator
{
    public string Generate(string data)
    {
        string report = $"Report: {data}";
        Console.WriteLine($"✅ Generated: {report}");
        return report;
    }
}

// ✅ GOOD: Single Responsibility - only sends emails
public class EmailSender
{
    public void Send(string content)
    {
        Console.WriteLine($"✅ Sending email: {content}");
        Console.WriteLine($"✅ Email sent successfully!");
    }
}

// WHY?
// - ReportGenerator can change if report format changes
// - EmailSender can change if email provider changes
// - Changes are isolated, easier to test
// - Each class has one reason to change
