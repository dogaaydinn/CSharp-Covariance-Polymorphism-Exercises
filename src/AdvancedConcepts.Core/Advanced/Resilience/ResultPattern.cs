namespace AdvancedCsharpConcepts.Advanced.Resilience;

/// <summary>
/// Result Pattern - Railway Oriented Programming.
/// NVIDIA/Silicon Valley best practice: Explicit error handling without exceptions.
/// </summary>
public readonly struct Result<T, TError>
{
    private readonly T? _value;
    private readonly TError? _error;
    private readonly bool _isSuccess;

    private Result(T value)
    {
        _value = value;
        _error = default;
        _isSuccess = true;
    }

    private Result(TError error)
    {
        _value = default;
        _error = error;
        _isSuccess = false;
    }

    public bool IsSuccess => _isSuccess;
    public bool IsFailure => !_isSuccess;

    public T Value => _isSuccess
        ? _value!
        : throw new InvalidOperationException("Cannot access Value of a failed result");

    public TError Error => !_isSuccess
        ? _error!
        : throw new InvalidOperationException("Cannot access Error of a successful result");

    public static Result<T, TError> Success(T value) => new(value);
    public static Result<T, TError> Failure(TError error) => new(error);

    /// <summary>
    /// Railway Oriented Programming: Chain operations that may fail.
    /// </summary>
    public Result<TNext, TError> Then<TNext>(Func<T, Result<TNext, TError>> next)
    {
        return _isSuccess ? next(_value!) : Result<TNext, TError>.Failure(_error!);
    }

    /// <summary>
    /// Map successful value to a new type.
    /// </summary>
    public Result<TNext, TError> Map<TNext>(Func<T, TNext> mapper)
    {
        return _isSuccess
            ? Result<TNext, TError>.Success(mapper(_value!))
            : Result<TNext, TError>.Failure(_error!);
    }

    /// <summary>
    /// Execute action on success or failure.
    /// </summary>
    public Result<T, TError> Match(Action<T> onSuccess, Action<TError> onFailure)
    {
        if (_isSuccess)
            onSuccess(_value!);
        else
            onFailure(_error!);

        return this;
    }

    /// <summary>
    /// Transform result to a value.
    /// </summary>
    public TResult Match<TResult>(Func<T, TResult> onSuccess, Func<TError, TResult> onFailure)
    {
        return _isSuccess ? onSuccess(_value!) : onFailure(_error!);
    }

    /// <summary>
    /// Tap into the result without changing it (for side effects like logging).
    /// </summary>
    public Result<T, TError> Tap(Action<T> action)
    {
        if (_isSuccess)
            action(_value!);
        return this;
    }

    public override string ToString()
    {
        return _isSuccess
            ? $"Success: {_value}"
            : $"Failure: {_error}";
    }
}

/// <summary>
/// Simple Result without error details (for boolean success/failure).
/// </summary>
public readonly struct Result
{
    private readonly bool _isSuccess;
    private readonly string? _error;

    private Result(bool isSuccess, string? error = null)
    {
        _isSuccess = isSuccess;
        _error = error;
    }

    public bool IsSuccess => _isSuccess;
    public bool IsFailure => !_isSuccess;
    public string Error => _error ?? string.Empty;

    public static Result Success() => new(true);
    public static Result Failure(string error) => new(false, error);

    public Result<T, string> WithValue<T>(T value)
    {
        return _isSuccess
            ? Result<T, string>.Success(value)
            : Result<T, string>.Failure(_error!);
    }
}

/// <summary>
/// Example domain errors.
/// </summary>
public record ValidationError(string Field, string Message);
public record NotFoundError(string EntityType, string Id);
public record UnauthorizedError(string Resource);

/// <summary>
/// Railway Oriented Programming Examples.
/// </summary>
public static class ResultPatternExamples
{
    public record User(int Id, string Name, string Email, int Age);

    /// <summary>
    /// Example: User registration with validation chain.
    /// </summary>
    public static Result<User, ValidationError> RegisterUser(string name, string email, int age)
    {
        return ValidateName(name)
            .Then(_ => ValidateEmail(email))
            .Then(_ => ValidateAge(age))
            .Map(_ => new User(Random.Shared.Next(1000, 9999), name, email, age));
    }

    private static Result<string, ValidationError> ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Result<string, ValidationError>.Failure(
                new ValidationError("Name", "Name cannot be empty"));

        if (name.Length < 2)
            return Result<string, ValidationError>.Failure(
                new ValidationError("Name", "Name must be at least 2 characters"));

        return Result<string, ValidationError>.Success(name);
    }

    private static Result<string, ValidationError> ValidateEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return Result<string, ValidationError>.Failure(
                new ValidationError("Email", "Email cannot be empty"));

        if (!email.Contains('@'))
            return Result<string, ValidationError>.Failure(
                new ValidationError("Email", "Email must contain @"));

        return Result<string, ValidationError>.Success(email);
    }

    private static Result<int, ValidationError> ValidateAge(int age)
    {
        if (age < 18)
            return Result<int, ValidationError>.Failure(
                new ValidationError("Age", "Must be at least 18 years old"));

        if (age > 120)
            return Result<int, ValidationError>.Failure(
                new ValidationError("Age", "Age seems invalid"));

        return Result<int, ValidationError>.Success(age);
    }

    /// <summary>
    /// Example: Find user by ID (demonstrating NotFound errors).
    /// </summary>
    public static Result<User, NotFoundError> FindUserById(int id)
    {
        // Simulate database lookup
        if (id == 123)
            return Result<User, NotFoundError>.Success(
                new User(123, "John Doe", "john@example.com", 25));

        return Result<User, NotFoundError>.Failure(
            new NotFoundError("User", id.ToString()));
    }

    /// <summary>
    /// Example: Update user with authorization check.
    /// </summary>
    public static Result<User, UnauthorizedError> UpdateUser(User user, int currentUserId)
    {
        if (user.Id != currentUserId)
            return Result<User, UnauthorizedError>.Failure(
                new UnauthorizedError($"Cannot update user {user.Id}"));

        // Simulate update
        return Result<User, UnauthorizedError>.Success(user);
    }

    /// <summary>
    /// Demonstrates Railway Oriented Programming patterns.
    /// </summary>
    public static void RunExamples()
    {
        Console.WriteLine("=== Result Pattern (Railway Oriented Programming) ===\n");

        // Example 1: Successful registration
        Console.WriteLine("1. Successful User Registration:");
        var result1 = RegisterUser("Alice", "alice@example.com", 25);
        result1.Match(
            user => Console.WriteLine($"   ✓ User registered: {user.Name} ({user.Email})"),
            error => Console.WriteLine($"   ✗ Validation failed: {error.Field} - {error.Message}")
        );

        // Example 2: Validation failure
        Console.WriteLine("\n2. Failed Registration (invalid email):");
        var result2 = RegisterUser("Bob", "invalid-email", 30);
        result2.Match(
            user => Console.WriteLine($"   ✓ User registered: {user.Name}"),
            error => Console.WriteLine($"   ✗ Validation failed: {error.Field} - {error.Message}")
        );

        // Example 3: Age validation failure
        Console.WriteLine("\n3. Failed Registration (age too young):");
        var result3 = RegisterUser("Charlie", "charlie@example.com", 16);
        result3.Match(
            user => Console.WriteLine($"   ✓ User registered: {user.Name}"),
            error => Console.WriteLine($"   ✗ Validation failed: {error.Field} - {error.Message}")
        );

        // Example 4: Not Found error
        Console.WriteLine("\n4. Find User (Not Found):");
        var result4 = FindUserById(999);
        result4.Match(
            user => Console.WriteLine($"   ✓ Found user: {user.Name}"),
            error => Console.WriteLine($"   ✗ {error.EntityType} not found: ID {error.Id}")
        );

        // Example 5: Successful find
        Console.WriteLine("\n5. Find User (Success):");
        var result5 = FindUserById(123);
        result5.Match(
            user => Console.WriteLine($"   ✓ Found user: {user.Name} ({user.Email})"),
            error => Console.WriteLine($"   ✗ {error.EntityType} not found: ID {error.Id}")
        );

        // Example 6: Chaining operations
        Console.WriteLine("\n6. Chaining Operations:");
        var chainedResult = FindUserById(123)
            .Map(user => user with { Email = "newemail@example.com" })
            .Tap(user => Console.WriteLine($"   → Updated email to: {user.Email}"))
            .Match(
                user => $"Success: {user.Name}",
                error => $"Failed: {error.EntityType} not found"
            );
        Console.WriteLine($"   Result: {chainedResult}");

        Console.WriteLine("\n✓ Railway Oriented Programming patterns demonstrated!");
    }
}
