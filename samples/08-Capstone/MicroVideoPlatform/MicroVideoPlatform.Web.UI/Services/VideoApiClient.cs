using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using MicroVideoPlatform.Shared.DTOs;
using MicroVideoPlatform.Shared.Enums;

namespace MicroVideoPlatform.Web.UI.Services;

/// <summary>
/// HTTP client implementation for Content.API.
/// Handles all video-related API calls.
/// </summary>
public class VideoApiClient : IVideoApiClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<VideoApiClient> _logger;
    private readonly JsonSerializerOptions _jsonOptions;

    public VideoApiClient(HttpClient httpClient, ILogger<VideoApiClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
    }

    public async Task<List<VideoDto>> GetVideosAsync(VideoStatus? status = null, string? category = null, int skip = 0, int take = 20)
    {
        try
        {
            var queryParams = new List<string>();
            if (status.HasValue) queryParams.Add($"status={status.Value}");
            if (!string.IsNullOrEmpty(category)) queryParams.Add($"category={category}");
            queryParams.Add($"skip={skip}");
            queryParams.Add($"take={take}");

            var query = string.Join("&", queryParams);
            var response = await _httpClient.GetAsync($"/api/videos?{query}");
            response.EnsureSuccessStatusCode();

            var videos = await response.Content.ReadFromJsonAsync<List<VideoDto>>(_jsonOptions);
            return videos ?? new List<VideoDto>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching videos");
            return new List<VideoDto>();
        }
    }

    public async Task<VideoDto?> GetVideoByIdAsync(Guid id)
    {
        try
        {
            var response = await _httpClient.GetAsync($"/api/videos/{id}");
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Video {Id} not found", id);
                return null;
            }

            return await response.Content.ReadFromJsonAsync<VideoDto>(_jsonOptions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching video {Id}", id);
            return null;
        }
    }

    public async Task<VideoDto> CreateVideoAsync(VideoDto video)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("/api/videos", video, _jsonOptions);
            response.EnsureSuccessStatusCode();

            var createdVideo = await response.Content.ReadFromJsonAsync<VideoDto>(_jsonOptions);
            return createdVideo ?? throw new InvalidOperationException("Failed to create video");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating video");
            throw;
        }
    }

    public async Task<VideoDto> UpdateVideoAsync(VideoDto video)
    {
        try
        {
            var response = await _httpClient.PutAsJsonAsync($"/api/videos/{video.Id}", video, _jsonOptions);
            response.EnsureSuccessStatusCode();

            var updatedVideo = await response.Content.ReadFromJsonAsync<VideoDto>(_jsonOptions);
            return updatedVideo ?? throw new InvalidOperationException("Failed to update video");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating video {Id}", video.Id);
            throw;
        }
    }

    public async Task<bool> DeleteVideoAsync(Guid id)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"/api/videos/{id}");
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting video {Id}", id);
            return false;
        }
    }

    public async Task<List<VideoDto>> SearchVideosAsync(string searchTerm)
    {
        try
        {
            var response = await _httpClient.GetAsync($"/api/videos/search?term={Uri.EscapeDataString(searchTerm)}");
            response.EnsureSuccessStatusCode();

            var videos = await response.Content.ReadFromJsonAsync<List<VideoDto>>(_jsonOptions);
            return videos ?? new List<VideoDto>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching videos with term: {Term}", searchTerm);
            return new List<VideoDto>();
        }
    }

    public async Task IncrementViewCountAsync(Guid id)
    {
        try
        {
            await _httpClient.PostAsync($"/api/videos/{id}/views", null);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error incrementing view count for video {Id}", id);
        }
    }
}
