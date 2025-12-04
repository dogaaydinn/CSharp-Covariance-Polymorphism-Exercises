namespace SOLIDPrinciples.SingleResponsibility;

/// <summary>
/// CORRECT: Each class has a single, well-defined responsibility.
/// Benefits:
/// - Easy to maintain: Changes are isolated to specific classes
/// - Easy to test: Each component can be tested independently
/// - Low coupling: Components are independent
/// - Single reason to change: Each class has one clear purpose
/// </summary>

#region Separate Responsibilities

/// <summary>
/// Responsibility: Validate user input
/// Changes only when: Validation rules change
/// </summary>
public class UserValidator
{
    public ValidationResult Validate(string email, string password)
    {
        Console.WriteLine($"[CORRECT] UserValidator: Validating {email}");

        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(email) || !email.Contains("@"))
        {
            errors.Add("Invalid email format");
        }

        if (string.IsNullOrWhiteSpace(password) || password.Length < 8)
        {
            errors.Add("Password must be at least 8 characters");
        }

        var isValid = errors.Count == 0;
        Console.WriteLine($"  Result: {(isValid ? "VALID" : "INVALID")}");

        return new ValidationResult(isValid, errors);
    }
}

/// <summary>
/// Responsibility: Persist user data
/// Changes only when: Database schema or persistence logic changes
/// </summary>
public class UserRepository
{
    private readonly List<User> _users = new();

    public void Save(User user)
    {
        Console.WriteLine($"[CORRECT] UserRepository: Saving {user.Email}");
        Console.WriteLine("  - Opening database connection");
        Console.WriteLine("  - Executing INSERT statement");
        Console.WriteLine("  - Closing connection");

        _users.Add(user);
        Console.WriteLine("  SUCCESS: User saved to database");
    }

    public User? GetByEmail(string email)
    {
        return _users.FirstOrDefault(u => u.Email == email);
    }
}

/// <summary>
/// Responsibility: Send emails
/// Changes only when: Email template or SMTP configuration changes
/// </summary>
public class EmailService
{
    public void SendWelcomeEmail(string email)
    {
        Console.WriteLine($"[CORRECT] EmailService: Sending welcome email to {email}");
        Console.WriteLine("  - Connecting to SMTP server");
        Console.WriteLine("  - Formatting email template");
        Console.WriteLine("  - Sending email");
        Console.WriteLine("  SUCCESS: Welcome email sent");
    }

    public void SendPasswordResetEmail(string email, string resetToken)
    {
        Console.WriteLine($"[CORRECT] EmailService: Sending password reset to {email}");
        Console.WriteLine($"  - Reset token: {resetToken}");
        Console.WriteLine("  SUCCESS: Password reset email sent");
    }
}

/// <summary>
/// Responsibility: Log application events
/// Changes only when: Logging format or destination changes
/// </summary>
public class UserLogger
{
    public void LogRegistration(string email)
    {
        var timestamp = DateTime.UtcNow;
        Console.WriteLine($"[CORRECT] UserLogger: [{timestamp:yyyy-MM-dd HH:mm:ss}] User registered: {email}");
    }

    public void LogLogin(string email)
    {
        var timestamp = DateTime.UtcNow;
        Console.WriteLine($"[CORRECT] UserLogger: [{timestamp:yyyy-MM-dd HH:mm:ss}] User logged in: {email}");
    }

    public void LogError(string message)
    {
        var timestamp = DateTime.UtcNow;
        Console.WriteLine($"[CORRECT] UserLogger: [{timestamp:yyyy-MM-dd HH:mm:ss}] ERROR: {message}");
    }
}

/// <summary>
/// Responsibility: Orchestrate user registration workflow
/// Changes only when: Registration business logic changes
/// </summary>
public class UserRegistrationService
{
    private readonly UserValidator _validator;
    private readonly UserRepository _repository;
    private readonly EmailService _emailService;
    private readonly UserLogger _logger;

    public UserRegistrationService(
        UserValidator validator,
        UserRepository repository,
        EmailService emailService,
        UserLogger logger)
    {
        _validator = validator;
        _repository = repository;
        _emailService = emailService;
        _logger = logger;
    }

    public RegistrationResult RegisterUser(string email, string password)
    {
        Console.WriteLine($"\n[CORRECT - SEPARATED RESPONSIBILITIES] Registering user: {email}");
        Console.WriteLine("Each responsibility is handled by a dedicated class!\n");

        // Step 1: Validate
        var validationResult = _validator.Validate(email, password);
        if (!validationResult.IsValid)
        {
            _logger.LogError($"Validation failed for {email}: {string.Join(", ", validationResult.Errors)}");
            return RegistrationResult.Failure(validationResult.Errors);
        }

        // Step 2: Save to database
        var user = new User { Email = email, Password = password };
        _repository.Save(user);

        // Step 3: Send welcome email
        _emailService.SendWelcomeEmail(email);

        // Step 4: Log the event
        _logger.LogRegistration(email);

        Console.WriteLine($"\nRegistration complete for: {email}");
        return RegistrationResult.Success(user);
    }
}

#endregion

#region Supporting Classes

public class User
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public class ValidationResult
{
    public bool IsValid { get; }
    public List<string> Errors { get; }

    public ValidationResult(bool isValid, List<string> errors)
    {
        IsValid = isValid;
        Errors = errors;
    }
}

public class RegistrationResult
{
    public bool IsSuccess { get; }
    public User? User { get; }
    public List<string> Errors { get; }

    private RegistrationResult(bool isSuccess, User? user, List<string> errors)
    {
        IsSuccess = isSuccess;
        User = user;
        Errors = errors;
    }

    public static RegistrationResult Success(User user) =>
        new(true, user, new List<string>());

    public static RegistrationResult Failure(List<string> errors) =>
        new(false, null, errors);
}

#endregion

/// <summary>
/// Demonstrates the benefits of Single Responsibility Principle
/// </summary>
public class SingleResponsibilityDemo
{
    public static void DemonstrateBenefits()
    {
        Console.WriteLine("\n=== BENEFITS OF SINGLE RESPONSIBILITY ===");

        // Create separate components
        var validator = new UserValidator();
        var repository = new UserRepository();
        var emailService = new EmailService();
        var logger = new UserLogger();

        var registrationService = new UserRegistrationService(
            validator,
            repository,
            emailService,
            logger);

        Console.WriteLine("\nBenefit 1: Easy to test each component independently");
        Console.WriteLine("  - Test UserValidator without database");
        Console.WriteLine("  - Test UserRepository without email service");
        Console.WriteLine("  - Mock dependencies easily");

        Console.WriteLine("\nBenefit 2: Easy to reuse components");
        Console.WriteLine("  - Use UserValidator in login, password reset, etc.");
        Console.WriteLine("  - Use EmailService for any email (not just registration)");

        Console.WriteLine("\nBenefit 3: Single reason to change");
        Console.WriteLine("  - Email template changes? Only modify EmailService");
        Console.WriteLine("  - Database schema changes? Only modify UserRepository");
        Console.WriteLine("  - Validation rules change? Only modify UserValidator");

        Console.WriteLine("\nBenefit 4: Low risk of breaking existing functionality");
        Console.WriteLine("  - Changes are isolated to specific classes");

        // Demonstrate the correct approach
        Console.WriteLine("\n--- Running Separated Responsibilities ---");
        var result = registrationService.RegisterUser("user@example.com", "securepass123");

        if (result.IsSuccess)
        {
            Console.WriteLine($"\n✓ User registered successfully: {result.User?.Email}");
        }
    }

    public static void DemonstrateReusability()
    {
        Console.WriteLine("\n=== DEMONSTRATING REUSABILITY ===");

        // Reuse validator in different context
        var validator = new UserValidator();
        Console.WriteLine("\nReusing validator for login:");
        validator.Validate("login@example.com", "password123");

        // Reuse email service for different purposes
        var emailService = new EmailService();
        Console.WriteLine("\nReusing email service for password reset:");
        emailService.SendPasswordResetEmail("user@example.com", "reset-token-12345");

        // Reuse logger for different events
        var logger = new UserLogger();
        Console.WriteLine("\nReusing logger for login event:");
        logger.LogLogin("user@example.com");
    }
}
