using VideoService.API.Models;

namespace VideoService.API.Services;

/// <summary>
/// Client for communicating with the video processing microservice.
/// Demonstrates service discovery using HttpClient with service names.
/// </summary>
public class VideoProcessingClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<VideoProcessingClient> _logger;

    public VideoProcessingClient(HttpClient httpClient, ILogger<VideoProcessingClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<bool> ProcessVideoAsync(int videoId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Requesting video processing for video {VideoId}", videoId);

            // In a real scenario, this would call the processing service
            // With Aspire service discovery, "http://videoprocessing" resolves automatically
            var response = await _httpClient.PostAsJsonAsync(
                $"/api/process/{videoId}",
                new { },
                cancellationToken);

            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to process video {VideoId}", videoId);
            return false;
        }
    }

    public async Task<VideoStatus> GetProcessingStatusAsync(int videoId, CancellationToken cancellationToken = default)
    {
        try
        {
            var status = await _httpClient.GetFromJsonAsync<VideoStatus>(
                $"/api/status/{videoId}",
                cancellationToken);

            return status;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get status for video {VideoId}", videoId);
            return VideoStatus.Failed;
        }
    }
}
