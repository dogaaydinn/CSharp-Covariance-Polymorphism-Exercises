namespace MicroVideoPlatform.Shared.Common;

/// <summary>
/// Result pattern for handling success/failure outcomes
/// </summary>
/// <typeparam name="T">Type of value on success</typeparam>
public sealed record Result<T>
{
    /// <summary>
    /// Indicates if the operation was successful
    /// </summary>
    public bool IsSuccess { get; init; }

    /// <summary>
    /// Indicates if the operation failed
    /// </summary>
    public bool IsFailure => !IsSuccess;

    /// <summary>
    /// Value returned on success
    /// </summary>
    public T? Value { get; init; }

    /// <summary>
    /// Error message on failure
    /// </summary>
    public string? Error { get; init; }

    /// <summary>
    /// Additional error details
    /// </summary>
    public Dictionary<string, string>? ErrorDetails { get; init; }

    private Result(bool isSuccess, T? value, string? error, Dictionary<string, string>? errorDetails = null)
    {
        IsSuccess = isSuccess;
        Value = value;
        Error = error;
        ErrorDetails = errorDetails;
    }

    /// <summary>
    /// Creates a successful result
    /// </summary>
    /// <param name="value">Success value</param>
    /// <returns>Success result</returns>
    public static Result<T> Success(T value) => new(true, value, null);

    /// <summary>
    /// Creates a failure result
    /// </summary>
    /// <param name="error">Error message</param>
    /// <param name="errorDetails">Additional error details</param>
    /// <returns>Failure result</returns>
    public static Result<T> Failure(string error, Dictionary<string, string>? errorDetails = null) =>
        new(false, default, error, errorDetails);

    /// <summary>
    /// Executes a function on the value if successful
    /// </summary>
    /// <typeparam name="TOut">Output type</typeparam>
    /// <param name="func">Function to execute</param>
    /// <returns>New result with transformed value</returns>
    public Result<TOut> Map<TOut>(Func<T, TOut> func)
    {
        if (IsFailure || Value is null)
            return Result<TOut>.Failure(Error ?? "No value", ErrorDetails);

        try
        {
            return Result<TOut>.Success(func(Value));
        }
        catch (Exception ex)
        {
            return Result<TOut>.Failure(ex.Message);
        }
    }

    /// <summary>
    /// Matches the result to one of two functions
    /// </summary>
    /// <typeparam name="TOut">Output type</typeparam>
    /// <param name="onSuccess">Function to execute on success</param>
    /// <param name="onFailure">Function to execute on failure</param>
    /// <returns>Result of the matched function</returns>
    public TOut Match<TOut>(Func<T, TOut> onSuccess, Func<string, TOut> onFailure)
    {
        return IsSuccess && Value is not null
            ? onSuccess(Value)
            : onFailure(Error ?? "Unknown error");
    }
}

/// <summary>
/// Non-generic result for operations without a return value
/// </summary>
public sealed record Result
{
    /// <summary>
    /// Indicates if the operation was successful
    /// </summary>
    public bool IsSuccess { get; init; }

    /// <summary>
    /// Indicates if the operation failed
    /// </summary>
    public bool IsFailure => !IsSuccess;

    /// <summary>
    /// Error message on failure
    /// </summary>
    public string? Error { get; init; }

    /// <summary>
    /// Additional error details
    /// </summary>
    public Dictionary<string, string>? ErrorDetails { get; init; }

    private Result(bool isSuccess, string? error, Dictionary<string, string>? errorDetails = null)
    {
        IsSuccess = isSuccess;
        Error = error;
        ErrorDetails = errorDetails;
    }

    /// <summary>
    /// Creates a successful result
    /// </summary>
    /// <returns>Success result</returns>
    public static Result Success() => new(true, null);

    /// <summary>
    /// Creates a failure result
    /// </summary>
    /// <param name="error">Error message</param>
    /// <param name="errorDetails">Additional error details</param>
    /// <returns>Failure result</returns>
    public static Result Failure(string error, Dictionary<string, string>? errorDetails = null) =>
        new(false, error, errorDetails);
}
