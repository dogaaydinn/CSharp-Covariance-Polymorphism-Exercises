namespace SOLIDPrinciples.SingleResponsibility;

/// <summary>
/// VIOLATION: This class has multiple responsibilities.
/// Problem: A "God Object" that does everything related to user management.
/// Consequences:
/// - Hard to maintain: Changes to email logic might break database logic
/// - Hard to test: Must mock database, email, and validation together
/// - High coupling: Everything depends on this one class
/// - Multiple reasons to change: Email format, database schema, validation rules
/// </summary>
public class UserManagerViolation
{
    private readonly List<string> _users = new();

    /// <summary>
    /// Responsibility #1: User validation
    /// </summary>
    public bool ValidateUser(string email, string password)
    {
        Console.WriteLine($"[VIOLATION] Validating user: {email}");

        // Email validation logic
        if (string.IsNullOrWhiteSpace(email) || !email.Contains("@"))
        {
            Console.WriteLine("  ERROR: Invalid email format");
            return false;
        }

        // Password validation logic
        if (string.IsNullOrWhiteSpace(password) || password.Length < 8)
        {
            Console.WriteLine("  ERROR: Password must be at least 8 characters");
            return false;
        }

        Console.WriteLine("  SUCCESS: User is valid");
        return true;
    }

    /// <summary>
    /// Responsibility #2: Database operations
    /// </summary>
    public void SaveToDatabase(string email, string password)
    {
        Console.WriteLine($"[VIOLATION] Saving to database: {email}");

        // Database connection logic
        Console.WriteLine("  - Opening database connection");
        Console.WriteLine("  - Creating SQL INSERT statement");
        Console.WriteLine("  - Executing query");
        Console.WriteLine("  - Closing connection");

        _users.Add(email);
        Console.WriteLine("  SUCCESS: User saved to database");
    }

    /// <summary>
    /// Responsibility #3: Email notifications
    /// </summary>
    public void SendWelcomeEmail(string email)
    {
        Console.WriteLine($"[VIOLATION] Sending welcome email to: {email}");

        // Email service logic
        Console.WriteLine("  - Connecting to SMTP server");
        Console.WriteLine("  - Formatting email template");
        Console.WriteLine("  - Sending email");
        Console.WriteLine("  SUCCESS: Welcome email sent");
    }

    /// <summary>
    /// Responsibility #4: Logging
    /// </summary>
    public void LogUserCreation(string email)
    {
        Console.WriteLine($"[VIOLATION] Logging user creation: {email}");

        // Logging logic
        var timestamp = DateTime.UtcNow;
        Console.WriteLine($"  - [{timestamp}] New user registered: {email}");
        Console.WriteLine("  - Writing to log file");
        Console.WriteLine("  SUCCESS: Event logged");
    }

    /// <summary>
    /// Responsibility #5: Business logic orchestration
    /// </summary>
    public bool RegisterUser(string email, string password)
    {
        Console.WriteLine($"\n[VIOLATION - GOD OBJECT] Registering user: {email}");
        Console.WriteLine("This class handles validation, database, email, and logging!");
        Console.WriteLine();

        if (!ValidateUser(email, password))
        {
            return false;
        }

        SaveToDatabase(email, password);
        SendWelcomeEmail(email);
        LogUserCreation(email);

        Console.WriteLine($"\nRegistration complete for: {email}");
        return true;
    }
}

/// <summary>
/// Real-world consequence example
/// </summary>
public class UserManagerProblemDemo
{
    public static void DemonstrateProblems()
    {
        Console.WriteLine("\n=== PROBLEMS WITH GOD OBJECT ===");

        var manager = new UserManagerViolation();

        // Problem 1: Can't test validation without database/email dependencies
        Console.WriteLine("\nProblem 1: Testing nightmare");
        Console.WriteLine("  To test validation, you must mock database and email!");

        // Problem 2: Can't reuse validation logic elsewhere
        Console.WriteLine("\nProblem 2: Can't reuse components");
        Console.WriteLine("  What if you need validation in another context?");
        Console.WriteLine("  You're forced to use the entire UserManager!");

        // Problem 3: Multiple reasons to change
        Console.WriteLine("\nProblem 3: Multiple reasons to change");
        Console.WriteLine("  - Database schema changes? Modify this class");
        Console.WriteLine("  - Email template changes? Modify this class");
        Console.WriteLine("  - Validation rules change? Modify this class");
        Console.WriteLine("  - Logging format changes? Modify this class");

        // Problem 4: High risk of breaking existing functionality
        Console.WriteLine("\nProblem 4: High risk of bugs");
        Console.WriteLine("  Changing email logic might accidentally break database logic!");

        // Demonstrate the violation
        Console.WriteLine("\n--- Running God Object ---");
        manager.RegisterUser("user@example.com", "securepass123");
    }
}
