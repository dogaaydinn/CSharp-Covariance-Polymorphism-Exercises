using MicroVideoPlatform.Shared.Enums;

namespace MicroVideoPlatform.Shared.DTOs;

/// <summary>
/// Data transfer object for video processing status
/// </summary>
public sealed record VideoProcessingStatusDto
{
    /// <summary>
    /// Video ID being processed
    /// </summary>
    public required Guid VideoId { get; init; }

    /// <summary>
    /// Current processing status
    /// </summary>
    public required VideoStatus Status { get; init; }

    /// <summary>
    /// Current processing stage
    /// </summary>
    public ProcessingStage? CurrentStage { get; init; }

    /// <summary>
    /// Processing progress percentage (0-100)
    /// </summary>
    public int ProgressPercentage { get; init; }

    /// <summary>
    /// Time when processing started
    /// </summary>
    public DateTime StartedAt { get; init; }

    /// <summary>
    /// Time when processing completed (if completed)
    /// </summary>
    public DateTime? CompletedAt { get; init; }

    /// <summary>
    /// Error message if processing failed
    /// </summary>
    public string? ErrorMessage { get; init; }

    /// <summary>
    /// Processing time in milliseconds
    /// </summary>
    public long? ProcessingTimeMs { get; init; }

    /// <summary>
    /// Worker ID that processed the video
    /// </summary>
    public string? WorkerId { get; init; }

    /// <summary>
    /// Additional processing metadata
    /// </summary>
    public Dictionary<string, string>? Metadata { get; init; }

    /// <summary>
    /// Human-readable processing time
    /// </summary>
    public string? ProcessingTimeFormatted => ProcessingTimeMs.HasValue
        ? TimeSpan.FromMilliseconds(ProcessingTimeMs.Value).ToString(@"hh\:mm\:ss")
        : null;

    /// <summary>
    /// Indicates if processing is in progress
    /// </summary>
    public bool IsProcessing => Status == VideoStatus.Processing;

    /// <summary>
    /// Indicates if processing is complete
    /// </summary>
    public bool IsCompleted => Status == VideoStatus.Completed;

    /// <summary>
    /// Indicates if processing failed
    /// </summary>
    public bool IsFailed => Status == VideoStatus.Failed;
}
