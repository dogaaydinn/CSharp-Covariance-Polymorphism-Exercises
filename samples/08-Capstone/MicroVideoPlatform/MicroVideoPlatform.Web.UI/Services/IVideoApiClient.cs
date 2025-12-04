using MicroVideoPlatform.Shared.DTOs;
using MicroVideoPlatform.Shared.Enums;

namespace MicroVideoPlatform.Web.UI.Services;

/// <summary>
/// HTTP client for Content.API communication.
/// </summary>
public interface IVideoApiClient
{
    Task<List<VideoDto>> GetVideosAsync(VideoStatus? status = null, string? category = null, int skip = 0, int take = 20);
    Task<VideoDto?> GetVideoByIdAsync(Guid id);
    Task<VideoDto> CreateVideoAsync(VideoDto video);
    Task<VideoDto> UpdateVideoAsync(VideoDto video);
    Task<bool> DeleteVideoAsync(Guid id);
    Task<List<VideoDto>> SearchVideosAsync(string searchTerm);
    Task IncrementViewCountAsync(Guid id);
}
