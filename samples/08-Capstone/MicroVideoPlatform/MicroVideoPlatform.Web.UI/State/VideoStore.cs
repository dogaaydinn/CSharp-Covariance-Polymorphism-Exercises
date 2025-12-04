using MicroVideoPlatform.Shared.DTOs;
using MicroVideoPlatform.Shared.Enums;
using MicroVideoPlatform.Web.UI.Services;

namespace MicroVideoPlatform.Web.UI.State;

/// <summary>
/// Video-specific state management and caching.
/// Provides centralized video data management across components.
/// </summary>
public class VideoStore
{
    private readonly IVideoApiClient _videoApiClient;
    private readonly Dictionary<Guid, VideoDto> _videoCache = new();
    private List<VideoDto>? _allVideos;
    private DateTime? _cacheTimestamp;
    private readonly TimeSpan _cacheExpiration = TimeSpan.FromMinutes(5);

    public event Action? OnChange;

    public VideoStore(IVideoApiClient videoApiClient)
    {
        _videoApiClient = videoApiClient;
    }

    /// <summary>
    /// Gets all cached videos.
    /// </summary>
    public IReadOnlyList<VideoDto> Videos => _allVideos?.AsReadOnly() ?? new List<VideoDto>().AsReadOnly();

    /// <summary>
    /// Gets whether cache is valid.
    /// </summary>
    public bool IsCacheValid => _cacheTimestamp.HasValue && DateTime.UtcNow - _cacheTimestamp.Value < _cacheExpiration;

    /// <summary>
    /// Loads videos from API and updates cache.
    /// </summary>
    public async Task LoadVideosAsync(VideoStatus? status = null, string? category = null, bool forceRefresh = false)
    {
        if (!forceRefresh && IsCacheValid)
        {
            return; // Use cached data
        }

        var videos = await _videoApiClient.GetVideosAsync(status, category, skip: 0, take: 100);
        _allVideos = videos;
        _cacheTimestamp = DateTime.UtcNow;

        // Update individual cache
        foreach (var video in videos)
        {
            _videoCache[video.Id] = video;
        }

        NotifyStateChanged();
    }

    /// <summary>
    /// Gets a single video by ID.
    /// Checks cache first, then fetches from API if needed.
    /// </summary>
    public async Task<VideoDto?> GetVideoAsync(Guid id)
    {
        // Check cache first
        if (_videoCache.TryGetValue(id, out var cachedVideo))
        {
            return cachedVideo;
        }

        // Fetch from API
        var video = await _videoApiClient.GetVideoByIdAsync(id);
        if (video != null)
        {
            _videoCache[id] = video;
        }

        return video;
    }

    /// <summary>
    /// Searches videos.
    /// </summary>
    public async Task<List<VideoDto>> SearchVideosAsync(string searchTerm)
    {
        var videos = await _videoApiClient.SearchVideosAsync(searchTerm);
        return videos;
    }

    /// <summary>
    /// Creates a new video.
    /// </summary>
    public async Task<VideoDto> CreateVideoAsync(VideoDto video)
    {
        var createdVideo = await _videoApiClient.CreateVideoAsync(video);

        // Add to cache
        _videoCache[createdVideo.Id] = createdVideo;
        _allVideos?.Add(createdVideo);

        NotifyStateChanged();
        return createdVideo;
    }

    /// <summary>
    /// Updates an existing video.
    /// </summary>
    public async Task<VideoDto> UpdateVideoAsync(VideoDto video)
    {
        var updatedVideo = await _videoApiClient.UpdateVideoAsync(video);

        // Update cache
        _videoCache[updatedVideo.Id] = updatedVideo;
        if (_allVideos != null)
        {
            var index = _allVideos.FindIndex(v => v.Id == updatedVideo.Id);
            if (index >= 0)
            {
                _allVideos[index] = updatedVideo;
            }
        }

        NotifyStateChanged();
        return updatedVideo;
    }

    /// <summary>
    /// Deletes a video.
    /// </summary>
    public async Task<bool> DeleteVideoAsync(Guid id)
    {
        var success = await _videoApiClient.DeleteVideoAsync(id);

        if (success)
        {
            // Remove from cache
            _videoCache.Remove(id);
            _allVideos?.RemoveAll(v => v.Id == id);

            NotifyStateChanged();
        }

        return success;
    }

    /// <summary>
    /// Increments view count for a video.
    /// </summary>
    public async Task IncrementViewCountAsync(Guid id)
    {
        await _videoApiClient.IncrementViewCountAsync(id);

        // Update cached video
        if (_videoCache.TryGetValue(id, out var video))
        {
            video.ViewsCount++;
            NotifyStateChanged();
        }
    }

    /// <summary>
    /// Clears all cached data.
    /// </summary>
    public void ClearCache()
    {
        _videoCache.Clear();
        _allVideos = null;
        _cacheTimestamp = null;
        NotifyStateChanged();
    }

    private void NotifyStateChanged() => OnChange?.Invoke();
}
