using MicroVideoPlatform.Shared.Enums;

namespace MicroVideoPlatform.Shared.Events;

/// <summary>
/// Event raised when video processing fails
/// </summary>
public sealed record VideoProcessingFailedEvent : DomainEventBase
{
    /// <summary>
    /// ID of the video that failed to process
    /// </summary>
    public required Guid VideoId { get; init; }

    /// <summary>
    /// Error message describing the failure
    /// </summary>
    public required string ErrorMessage { get; init; }

    /// <summary>
    /// Stage at which processing failed
    /// </summary>
    public ProcessingStage? FailedStage { get; init; }

    /// <summary>
    /// Exception type if available
    /// </summary>
    public string? ExceptionType { get; init; }

    /// <summary>
    /// Stack trace for debugging (should not be exposed to users)
    /// </summary>
    public string? StackTrace { get; init; }

    /// <summary>
    /// Time when processing started
    /// </summary>
    public required DateTime StartedAt { get; init; }

    /// <summary>
    /// Time when failure occurred
    /// </summary>
    public required DateTime FailedAt { get; init; }

    /// <summary>
    /// Worker ID that was processing the video
    /// </summary>
    public required string WorkerId { get; init; }

    /// <summary>
    /// Number of retry attempts made
    /// </summary>
    public int RetryCount { get; init; }

    /// <summary>
    /// Indicates if the processing should be retried
    /// </summary>
    public bool ShouldRetry { get; init; }

    /// <summary>
    /// Additional error context
    /// </summary>
    public Dictionary<string, string>? ErrorContext { get; init; }

    /// <summary>
    /// Time spent processing before failure (milliseconds)
    /// </summary>
    public long ProcessingTimeBeforeFailureMs => (long)(FailedAt - StartedAt).TotalMilliseconds;
}
