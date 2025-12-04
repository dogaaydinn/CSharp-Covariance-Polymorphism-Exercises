using MicroVideoPlatform.Shared.DTOs;

namespace MicroVideoPlatform.Shared.Contracts;

/// <summary>
/// Contract for video analytics and classification operations
/// </summary>
public interface IAnalyticsService
{
    /// <summary>
    /// Analyzes and classifies video content using ML.NET
    /// </summary>
    /// <param name="videoId">Video ID</param>
    /// <param name="filePath">Path to video file</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Analytics result DTO</returns>
    Task<VideoAnalyticsResultDto> AnalyzeVideoAsync(
        Guid videoId,
        string filePath,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets analytics results for a video
    /// </summary>
    /// <param name="videoId">Video ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Analytics result DTO or null if not analyzed</returns>
    Task<VideoAnalyticsResultDto?> GetAnalyticsAsync(
        Guid videoId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all videos in a specific category
    /// </summary>
    /// <param name="category">Category name</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of video DTOs in the category</returns>
    Task<IEnumerable<VideoDto>> GetVideosByCategoryAsync(
        string category,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets available categories with video counts
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Dictionary of category names and counts</returns>
    Task<Dictionary<string, int>> GetCategoryStatisticsAsync(CancellationToken cancellationToken = default);
}
