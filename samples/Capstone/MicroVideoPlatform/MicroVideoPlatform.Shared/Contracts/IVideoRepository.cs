using MicroVideoPlatform.Shared.DTOs;
using MicroVideoPlatform.Shared.Enums;

namespace MicroVideoPlatform.Shared.Contracts;

/// <summary>
/// Contract for video repository operations
/// </summary>
public interface IVideoRepository
{
    /// <summary>
    /// Gets a video by its ID
    /// </summary>
    /// <param name="id">Video ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Video DTO or null if not found</returns>
    Task<VideoDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all videos with optional filtering and pagination
    /// </summary>
    /// <param name="status">Filter by status</param>
    /// <param name="category">Filter by category</param>
    /// <param name="skip">Number of records to skip</param>
    /// <param name="take">Number of records to take</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of video DTOs</returns>
    Task<IEnumerable<VideoDto>> GetAllAsync(
        VideoStatus? status = null,
        string? category = null,
        int skip = 0,
        int take = 100,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a new video record
    /// </summary>
    /// <param name="video">Video DTO to create</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Created video DTO with generated ID</returns>
    Task<VideoDto> CreateAsync(VideoDto video, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing video record
    /// </summary>
    /// <param name="video">Video DTO with updated values</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated video DTO</returns>
    Task<VideoDto> UpdateAsync(VideoDto video, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a video record
    /// </summary>
    /// <param name="id">Video ID to delete</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if deleted, false if not found</returns>
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Searches videos by title or description
    /// </summary>
    /// <param name="searchTerm">Search term</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of matching video DTOs</returns>
    Task<IEnumerable<VideoDto>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets videos by tag
    /// </summary>
    /// <param name="tag">Tag to filter by</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of video DTOs with the tag</returns>
    Task<IEnumerable<VideoDto>> GetByTagAsync(string tag, CancellationToken cancellationToken = default);

    /// <summary>
    /// Increments the view count for a video
    /// </summary>
    /// <param name="id">Video ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Task representing the async operation</returns>
    Task IncrementViewCountAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets total count of videos
    /// </summary>
    /// <param name="status">Optional status filter</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Total count</returns>
    Task<int> GetCountAsync(VideoStatus? status = null, CancellationToken cancellationToken = default);
}
