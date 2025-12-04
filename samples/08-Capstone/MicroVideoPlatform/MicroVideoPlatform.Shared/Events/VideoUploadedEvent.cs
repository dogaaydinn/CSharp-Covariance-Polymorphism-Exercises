using MicroVideoPlatform.Shared.DTOs;

namespace MicroVideoPlatform.Shared.Events;

/// <summary>
/// Event raised when a new video is uploaded to the platform
/// </summary>
public sealed record VideoUploadedEvent : DomainEventBase
{
    /// <summary>
    /// ID of the uploaded video
    /// </summary>
    public required Guid VideoId { get; init; }

    /// <summary>
    /// Video information
    /// </summary>
    public required VideoDto Video { get; init; }

    /// <summary>
    /// File path where the video is stored
    /// </summary>
    public required string FilePath { get; init; }

    /// <summary>
    /// User who uploaded the video
    /// </summary>
    public required string UploadedBy { get; init; }

    /// <summary>
    /// Indicates if the video should be processed immediately
    /// </summary>
    public bool RequiresProcessing { get; init; } = true;

    /// <summary>
    /// Priority level for processing (1-10, where 10 is highest)
    /// </summary>
    public int ProcessingPriority { get; init; } = 5;
}
