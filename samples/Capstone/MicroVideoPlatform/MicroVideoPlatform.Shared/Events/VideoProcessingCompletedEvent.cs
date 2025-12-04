using MicroVideoPlatform.Shared.DTOs;

namespace MicroVideoPlatform.Shared.Events;

/// <summary>
/// Event raised when video processing completes successfully
/// </summary>
public sealed record VideoProcessingCompletedEvent : DomainEventBase
{
    /// <summary>
    /// ID of the processed video
    /// </summary>
    public required Guid VideoId { get; init; }

    /// <summary>
    /// Extracted video metadata
    /// </summary>
    public required VideoMetadataDto Metadata { get; init; }

    /// <summary>
    /// Processing status information
    /// </summary>
    public required VideoProcessingStatusDto ProcessingStatus { get; init; }

    /// <summary>
    /// Analytics results (if available)
    /// </summary>
    public VideoAnalyticsResultDto? AnalyticsResult { get; init; }

    /// <summary>
    /// URL to generated thumbnail
    /// </summary>
    public string? ThumbnailUrl { get; init; }

    /// <summary>
    /// Total processing time in milliseconds
    /// </summary>
    public required long ProcessingTimeMs { get; init; }

    /// <summary>
    /// Worker ID that processed the video
    /// </summary>
    public required string WorkerId { get; init; }

    /// <summary>
    /// Processing stages completed
    /// </summary>
    public string[]? CompletedStages { get; init; }
}
