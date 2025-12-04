using System;

namespace AdvancedConcepts.SourceGenerators;

/// <summary>
/// Generates high-performance logging code using source generators.
/// Similar to Microsoft.Extensions.Logging.LoggerMessageAttribute but with additional features.
/// </summary>
/// <example>
/// <code>
/// public static partial class Log
/// {
///     [LoggerMessage(
///         EventId = 1,
///         Level = LogLevel.Information,
///         Message = "Processing request for {UserId} at {Timestamp}")]
///     public static partial void ProcessingRequest(
///         ILogger logger, int userId, DateTime timestamp);
/// }
///
/// // Generated code creates optimized logging method with pre-compiled format string
/// </code>
/// </example>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
public sealed class LoggerMessageAttribute : Attribute
{
    /// <summary>
    /// The event ID for this log message.
    /// </summary>
    public int EventId { get; set; }

    /// <summary>
    /// The log level.
    /// </summary>
    public LogLevel Level { get; set; } = LogLevel.Information;

    /// <summary>
    /// The message template.
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Optional event name. If not specified, uses method name.
    /// </summary>
    public string? EventName { get; set; }

    /// <summary>
    /// If true, skips null check for logger parameter. Default is false.
    /// </summary>
    public bool SkipEnabledCheck { get; set; } = false;
}

/// <summary>
/// Log levels matching Microsoft.Extensions.Logging.LogLevel.
/// </summary>
public enum LogLevel
{
    Trace = 0,
    Debug = 1,
    Information = 2,
    Warning = 3,
    Error = 4,
    Critical = 5,
    None = 6
}
