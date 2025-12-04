using MicroVideoPlatform.Shared.DTOs;

namespace MicroVideoPlatform.Shared.Contracts;

/// <summary>
/// Contract for video processing operations
/// </summary>
public interface IVideoProcessingService
{
    /// <summary>
    /// Processes a video (transcoding, metadata extraction, etc.)
    /// </summary>
    /// <param name="videoId">ID of video to process</param>
    /// <param name="filePath">Path to video file</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Processing status DTO</returns>
    Task<VideoProcessingStatusDto> ProcessVideoAsync(
        Guid videoId,
        string filePath,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Extracts metadata from a video file
    /// </summary>
    /// <param name="filePath">Path to video file</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Video metadata DTO</returns>
    Task<VideoMetadataDto> ExtractMetadataAsync(
        string filePath,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Generates thumbnail for a video
    /// </summary>
    /// <param name="videoId">Video ID</param>
    /// <param name="filePath">Path to video file</param>
    /// <param name="timeOffsetSeconds">Time offset in seconds for thumbnail capture</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Path to generated thumbnail</returns>
    Task<string> GenerateThumbnailAsync(
        Guid videoId,
        string filePath,
        int timeOffsetSeconds = 5,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets processing status for a video
    /// </summary>
    /// <param name="videoId">Video ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Processing status DTO</returns>
    Task<VideoProcessingStatusDto?> GetProcessingStatusAsync(
        Guid videoId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Cancels video processing
    /// </summary>
    /// <param name="videoId">Video ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if cancelled, false if not found or already completed</returns>
    Task<bool> CancelProcessingAsync(Guid videoId, CancellationToken cancellationToken = default);
}
