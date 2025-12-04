using System;
using System.Collections.Generic;

namespace DesignPatterns.Behavioral;

/// <summary>
/// CHAIN OF RESPONSIBILITY PATTERN - Passes request along chain of handlers
///
/// Problem:
/// - Multiple objects can handle a request
/// - Handler isn't known in advance
/// - Want to avoid coupling sender to receiver
/// - Need to process request through multiple handlers
///
/// UML Structure:
/// ┌──────────────┐        ┌──────────────┐
/// │   Client     │───────>│   Handler    │ (abstract)
/// └──────────────┘        ├──────────────┤
///                         │ +next        │
///                         │ +SetNext()   │
///                         │ +Handle()    │
///                         └──────────────┘
///                                △
///                                │ extends
///                    ┌───────────┼───────────┐
///             ┌──────┴──────┐    │    ┌──────┴──────┐
///             │ ConcreteA   │    │    │ ConcreteB   │
///             │ Handler     │    │    │ Handler     │
///             └─────────────┘    │    └─────────────┘
///                          ┌─────┴─────┐
///                          │ ConcreteC │
///                          │ Handler   │
///                          └───────────┘
///
/// When to Use:
/// - Multiple objects can handle a request
/// - Handler should be determined automatically
/// - Want to issue request without specifying receiver
/// - Need processing pipeline or middleware
///
/// Benefits:
/// - Decouples sender from receivers
/// - Adds/removes handlers dynamically
/// - Single Responsibility Principle
/// - Open/Closed Principle
/// </summary>

#region Support Ticket Example

/// <summary>
/// Request object
/// </summary>
public class SupportTicket
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Priority Priority { get; set; }
    public string Category { get; set; } = string.Empty;
    public bool Resolved { get; set; }

    public override string ToString()
    {
        return $"Ticket #{Id}: {Title} (Priority: {Priority}, Category: {Category})";
    }
}

public enum Priority
{
    Low,
    Medium,
    High,
    Critical
}

/// <summary>
/// Abstract Handler
/// </summary>
public abstract class SupportHandler
{
    protected SupportHandler? _nextHandler;

    public SupportHandler SetNext(SupportHandler handler)
    {
        _nextHandler = handler;
        return handler;
    }

    public abstract void Handle(SupportTicket ticket);

    protected void PassToNext(SupportTicket ticket)
    {
        if (_nextHandler != null)
        {
            _nextHandler.Handle(ticket);
        }
        else
        {
            Console.WriteLine($"  [Chain] No handler available for: {ticket}");
        }
    }
}

/// <summary>
/// Concrete Handler: Level 1 Support
/// </summary>
public class Level1Support : SupportHandler
{
    public override void Handle(SupportTicket ticket)
    {
        Console.WriteLine($"  [Chain] Level 1 Support reviewing: {ticket}");

        if (ticket.Priority == Priority.Low && ticket.Category == "General")
        {
            Console.WriteLine("  [Chain] Level 1 Support: RESOLVED - Basic issue handled");
            ticket.Resolved = true;
        }
        else
        {
            Console.WriteLine("  [Chain] Level 1 Support: Escalating to Level 2...");
            PassToNext(ticket);
        }
    }
}

/// <summary>
/// Concrete Handler: Level 2 Support
/// </summary>
public class Level2Support : SupportHandler
{
    public override void Handle(SupportTicket ticket)
    {
        Console.WriteLine($"  [Chain] Level 2 Support reviewing: {ticket}");

        if ((ticket.Priority == Priority.Low || ticket.Priority == Priority.Medium) &&
            ticket.Category != "Critical Bug")
        {
            Console.WriteLine("  [Chain] Level 2 Support: RESOLVED - Technical issue resolved");
            ticket.Resolved = true;
        }
        else
        {
            Console.WriteLine("  [Chain] Level 2 Support: Escalating to Level 3...");
            PassToNext(ticket);
        }
    }
}

/// <summary>
/// Concrete Handler: Level 3 Support (Senior Engineers)
/// </summary>
public class Level3Support : SupportHandler
{
    public override void Handle(SupportTicket ticket)
    {
        Console.WriteLine($"  [Chain] Level 3 Support reviewing: {ticket}");

        if (ticket.Priority != Priority.Critical)
        {
            Console.WriteLine("  [Chain] Level 3 Support: RESOLVED - Complex issue resolved");
            ticket.Resolved = true;
        }
        else
        {
            Console.WriteLine("  [Chain] Level 3 Support: Escalating to Management...");
            PassToNext(ticket);
        }
    }
}

/// <summary>
/// Concrete Handler: Management
/// </summary>
public class ManagementSupport : SupportHandler
{
    public override void Handle(SupportTicket ticket)
    {
        Console.WriteLine($"  [Chain] Management reviewing: {ticket}");
        Console.WriteLine("  [Chain] Management: RESOLVED - Critical issue addressed by leadership");
        ticket.Resolved = true;
    }
}

#endregion

#region Authentication Chain

/// <summary>
/// Request for authentication
/// </summary>
public class AuthRequest
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
    public string IpAddress { get; set; } = string.Empty;
    public bool IsAuthenticated { get; set; }
    public string FailureReason { get; set; } = string.Empty;
}

/// <summary>
/// Abstract Handler for authentication
/// </summary>
public abstract class AuthHandler
{
    protected AuthHandler? _nextHandler;

    public AuthHandler SetNext(AuthHandler handler)
    {
        _nextHandler = handler;
        return handler;
    }

    public abstract bool Authenticate(AuthRequest request);

    protected bool PassToNext(AuthRequest request)
    {
        if (_nextHandler != null)
        {
            return _nextHandler.Authenticate(request);
        }

        // All checks passed
        request.IsAuthenticated = true;
        return true;
    }
}

/// <summary>
/// Concrete Handler: User Existence Check
/// </summary>
public class UserExistenceHandler : AuthHandler
{
    private readonly HashSet<string> _validUsers = new() { "admin", "user1", "user2", "john_doe" };

    public override bool Authenticate(AuthRequest request)
    {
        Console.WriteLine("  [Chain] Checking if user exists...");

        if (!_validUsers.Contains(request.Username))
        {
            Console.WriteLine($"  [Chain] FAILED - User '{request.Username}' does not exist");
            request.FailureReason = "User not found";
            return false;
        }

        Console.WriteLine($"  [Chain] User '{request.Username}' exists. Proceeding...");
        return PassToNext(request);
    }
}

/// <summary>
/// Concrete Handler: Password Validation
/// </summary>
public class PasswordHandler : AuthHandler
{
    private readonly Dictionary<string, string> _passwords = new()
    {
        { "admin", "admin123" },
        { "user1", "pass1" },
        { "user2", "pass2" },
        { "john_doe", "secret" }
    };

    public override bool Authenticate(AuthRequest request)
    {
        Console.WriteLine("  [Chain] Validating password...");

        if (!_passwords.ContainsKey(request.Username) ||
            _passwords[request.Username] != request.Password)
        {
            Console.WriteLine("  [Chain] FAILED - Invalid password");
            request.FailureReason = "Invalid credentials";
            return false;
        }

        Console.WriteLine("  [Chain] Password valid. Proceeding...");
        return PassToNext(request);
    }
}

/// <summary>
/// Concrete Handler: Rate Limiting
/// </summary>
public class RateLimitHandler : AuthHandler
{
    private readonly Dictionary<string, DateTime> _lastAttempts = new();
    private readonly TimeSpan _minInterval = TimeSpan.FromSeconds(1);

    public override bool Authenticate(AuthRequest request)
    {
        Console.WriteLine("  [Chain] Checking rate limit...");

        if (_lastAttempts.ContainsKey(request.Username))
        {
            var elapsed = DateTime.Now - _lastAttempts[request.Username];
            if (elapsed < _minInterval)
            {
                Console.WriteLine($"  [Chain] FAILED - Too many requests. Wait {_minInterval.TotalSeconds}s");
                request.FailureReason = "Rate limit exceeded";
                return false;
            }
        }

        _lastAttempts[request.Username] = DateTime.Now;
        Console.WriteLine("  [Chain] Rate limit OK. Proceeding...");
        return PassToNext(request);
    }
}

/// <summary>
/// Concrete Handler: IP Whitelist
/// </summary>
public class IpWhitelistHandler : AuthHandler
{
    private readonly HashSet<string> _allowedIps = new()
    {
        "192.168.1.1",
        "192.168.1.100",
        "10.0.0.1"
    };

    public override bool Authenticate(AuthRequest request)
    {
        Console.WriteLine("  [Chain] Checking IP whitelist...");

        if (!_allowedIps.Contains(request.IpAddress))
        {
            Console.WriteLine($"  [Chain] FAILED - IP {request.IpAddress} not whitelisted");
            request.FailureReason = "IP address not allowed";
            return false;
        }

        Console.WriteLine($"  [Chain] IP {request.IpAddress} is whitelisted. Proceeding...");
        return PassToNext(request);
    }
}

#endregion

#region Expense Approval Chain

/// <summary>
/// Expense request
/// </summary>
public class ExpenseRequest
{
    public string EmployeeName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public bool Approved { get; set; }
    public string ApprovedBy { get; set; } = string.Empty;

    public override string ToString()
    {
        return $"Expense: {Description} - ${Amount:F2} (Employee: {EmployeeName})";
    }
}

/// <summary>
/// Abstract Handler for expense approval
/// </summary>
public abstract class ExpenseApprover
{
    protected ExpenseApprover? _nextApprover;
    protected string _name;
    protected decimal _approvalLimit;

    protected ExpenseApprover(string name, decimal approvalLimit)
    {
        _name = name;
        _approvalLimit = approvalLimit;
    }

    public ExpenseApprover SetNext(ExpenseApprover approver)
    {
        _nextApprover = approver;
        return approver;
    }

    public void ProcessRequest(ExpenseRequest request)
    {
        Console.WriteLine($"  [Chain] {_name} reviewing: {request}");

        if (request.Amount <= _approvalLimit)
        {
            Console.WriteLine($"  [Chain] APPROVED by {_name} (Limit: ${_approvalLimit:F2})");
            request.Approved = true;
            request.ApprovedBy = _name;
        }
        else if (_nextApprover != null)
        {
            Console.WriteLine($"  [Chain] Amount exceeds {_name}'s limit. Forwarding...");
            _nextApprover.ProcessRequest(request);
        }
        else
        {
            Console.WriteLine($"  [Chain] REJECTED - Amount ${request.Amount:F2} exceeds all approval limits");
            request.Approved = false;
        }
    }
}

/// <summary>
/// Concrete Handler: Team Lead
/// </summary>
public class TeamLead : ExpenseApprover
{
    public TeamLead(string name) : base(name, 1000m) { }
}

/// <summary>
/// Concrete Handler: Manager
/// </summary>
public class Manager : ExpenseApprover
{
    public Manager(string name) : base(name, 5000m) { }
}

/// <summary>
/// Concrete Handler: Director
/// </summary>
public class Director : ExpenseApprover
{
    public Director(string name) : base(name, 20000m) { }
}

/// <summary>
/// Concrete Handler: CFO
/// </summary>
public class CFO : ExpenseApprover
{
    public CFO(string name) : base(name, 100000m) { }
}

#endregion

/// <summary>
/// Example demonstrating Chain of Responsibility pattern
/// </summary>
public static class ChainOfResponsibilityExample
{
    public static void Run()
    {
        Console.WriteLine();
        Console.WriteLine("9. CHAIN OF RESPONSIBILITY - Passes requests through handler chain");
        Console.WriteLine("-".PadRight(70, '-'));
        Console.WriteLine();

        // Example 1: Support Ticket System
        Console.WriteLine("Example 1: Support Ticket Escalation");
        Console.WriteLine();

        // Build the chain
        var level1 = new Level1Support();
        var level2 = new Level2Support();
        var level3 = new Level3Support();
        var management = new ManagementSupport();

        level1.SetNext(level2).SetNext(level3).SetNext(management);

        // Test different tickets
        var tickets = new List<SupportTicket>
        {
            new() { Id = 1, Title = "Password reset", Priority = Priority.Low, Category = "General" },
            new() { Id = 2, Title = "Software installation", Priority = Priority.Medium, Category = "Technical" },
            new() { Id = 3, Title = "Server crash", Priority = Priority.High, Category = "Critical Bug" },
            new() { Id = 4, Title = "Data breach", Priority = Priority.Critical, Category = "Security" }
        };

        foreach (var ticket in tickets)
        {
            Console.WriteLine($"  Processing: {ticket}");
            level1.Handle(ticket);
            Console.WriteLine($"  Status: {(ticket.Resolved ? "RESOLVED" : "UNRESOLVED")}");
            Console.WriteLine();
        }

        // Example 2: Authentication Chain
        Console.WriteLine("Example 2: Multi-Step Authentication");
        Console.WriteLine();

        // Build authentication chain
        var userCheck = new UserExistenceHandler();
        var passwordCheck = new PasswordHandler();
        var rateLimit = new RateLimitHandler();
        var ipCheck = new IpWhitelistHandler();

        userCheck.SetNext(passwordCheck).SetNext(rateLimit).SetNext(ipCheck);

        // Test authentication requests
        var authRequests = new List<AuthRequest>
        {
            new() { Username = "john_doe", Password = "secret", IpAddress = "192.168.1.1" },
            new() { Username = "invalid_user", Password = "pass", IpAddress = "192.168.1.1" },
            new() { Username = "admin", Password = "wrong", IpAddress = "192.168.1.1" },
            new() { Username = "user1", Password = "pass1", IpAddress = "10.0.0.5" }
        };

        foreach (var request in authRequests)
        {
            Console.WriteLine($"  Authenticating: {request.Username} from {request.IpAddress}");
            bool success = userCheck.Authenticate(request);

            if (success)
            {
                Console.WriteLine("  [Chain] SUCCESS - Authentication complete!");
            }
            else
            {
                Console.WriteLine($"  [Chain] FAILED - {request.FailureReason}");
            }
            Console.WriteLine();
        }

        // Example 3: Expense Approval Chain
        Console.WriteLine("Example 3: Expense Approval Workflow");
        Console.WriteLine();

        // Build approval chain
        var teamLead = new TeamLead("Alice (Team Lead)");
        var manager = new Manager("Bob (Manager)");
        var director = new Director("Carol (Director)");
        var cfo = new CFO("Dave (CFO)");

        teamLead.SetNext(manager).SetNext(director).SetNext(cfo);

        // Test different expense amounts
        var expenses = new List<ExpenseRequest>
        {
            new() { EmployeeName = "John", Description = "Office supplies", Amount = 500m },
            new() { EmployeeName = "Sarah", Description = "Team dinner", Amount = 2500m },
            new() { EmployeeName = "Mike", Description = "New equipment", Amount = 15000m },
            new() { EmployeeName = "Lisa", Description = "Conference sponsorship", Amount = 50000m },
            new() { EmployeeName = "Tom", Description = "Office renovation", Amount = 150000m }
        };

        foreach (var expense in expenses)
        {
            Console.WriteLine($"  Submitting: {expense}");
            teamLead.ProcessRequest(expense);

            if (expense.Approved)
            {
                Console.WriteLine($"  Result: APPROVED by {expense.ApprovedBy}");
            }
            else
            {
                Console.WriteLine("  Result: REJECTED");
            }
            Console.WriteLine();
        }

        Console.WriteLine("  Key Benefit: Decouples sender from receivers - flexible processing pipeline!");
    }
}
