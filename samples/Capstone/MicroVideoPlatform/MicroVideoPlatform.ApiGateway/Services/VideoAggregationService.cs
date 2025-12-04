using System.Net.Http.Json;
using System.Text.Json;
using MicroVideoPlatform.ApiGateway.Models;
using MicroVideoPlatform.Shared.DTOs;

namespace MicroVideoPlatform.ApiGateway.Services;

/// <summary>
/// API Composition service that aggregates data from multiple microservices.
/// Implements the API Gateway Aggregation pattern.
/// </summary>
public class VideoAggregationService
{
    private readonly HttpClient _contentClient;
    private readonly HttpClient _analyticsClient;
    private readonly HttpClient _processingClient;
    private readonly ILogger<VideoAggregationService> _logger;
    private readonly JsonSerializerOptions _jsonOptions;

    public VideoAggregationService(
        IHttpClientFactory httpClientFactory,
        ILogger<VideoAggregationService> logger)
    {
        _contentClient = httpClientFactory.CreateClient("ContentApi");
        _analyticsClient = httpClientFactory.CreateClient("AnalyticsFunction");
        _processingClient = httpClientFactory.CreateClient("ProcessingWorker");
        _logger = logger;

        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
    }

    /// <summary>
    /// Aggregates complete video details from all microservices.
    /// Calls multiple services in parallel for optimal performance.
    /// </summary>
    public async Task<AggregatedVideoResponse> GetVideoDetailsAsync(string videoId)
    {
        _logger.LogInformation("Aggregating video details for {VideoId}", videoId);

        try
        {
            // Parallel calls to all services
            var videoTask = GetVideoFromContentApiAsync(videoId);
            var recommendationsTask = GetRecommendationsAsync(videoId);
            var processingTask = GetProcessingStatusAsync(videoId);
            var similarVideosTask = GetSimilarVideosAsync(videoId);
            var commentsTask = GetPopularCommentsAsync(videoId);
            var metadataTask = GetVideoMetadataAsync(videoId);

            // Wait for all tasks to complete
            await Task.WhenAll(
                videoTask,
                recommendationsTask,
                processingTask,
                similarVideosTask,
                commentsTask,
                metadataTask
            );

            // Aggregate results
            var response = new AggregatedVideoResponse
            {
                Video = await videoTask,
                Recommendations = await recommendationsTask,
                ProcessingStatus = await processingTask,
                SimilarVideos = await similarVideosTask,
                PopularComments = await commentsTask,
                Metadata = await metadataTask
            };

            _logger.LogInformation(
                "Successfully aggregated video details for {VideoId}. " +
                "Video: {HasVideo}, Recommendations: {RecommendationCount}, " +
                "SimilarVideos: {SimilarCount}, Comments: {CommentCount}",
                videoId,
                response.Video != null,
                response.Recommendations?.Count ?? 0,
                response.SimilarVideos?.Count ?? 0,
                response.PopularComments?.Count ?? 0
            );

            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error aggregating video details for {VideoId}", videoId);
            throw;
        }
    }

    /// <summary>
    /// Processes video upload across multiple services.
    /// Orchestrates the upload workflow: Content -> Processing -> Analytics.
    /// </summary>
    public async Task<VideoUploadResponse> ProcessVideoUploadAsync(VideoUploadRequest request)
    {
        _logger.LogInformation("Processing video upload: {Title}", request.Metadata.Title);

        try
        {
            // Step 1: Save video metadata to Content.API
            var videoDto = new VideoDto
            {
                Id = Guid.NewGuid(),
                Title = request.Metadata.Title,
                Description = request.Metadata.Description,
                Category = request.Metadata.Category,
                Tags = request.Metadata.Tags,
                UploadDate = DateTime.UtcNow,
                Status = Shared.Enums.VideoStatus.Processing
            };

            var contentResponse = await _contentClient.PostAsJsonAsync("/api/videos", videoDto, _jsonOptions);
            contentResponse.EnsureSuccessStatusCode();

            var createdVideo = await contentResponse.Content.ReadFromJsonAsync<VideoDto>(_jsonOptions);
            var videoId = createdVideo?.Id.ToString() ?? Guid.NewGuid().ToString();

            // Step 2 & 3: Start processing and recommendation calculation in parallel
            var processingTask = StartVideoProcessingAsync(videoId, request.FilePath);
            var analyticsTask = StartRecommendationCalculationAsync(videoId, request.Metadata);

            await Task.WhenAll(processingTask, analyticsTask);

            var response = new VideoUploadResponse
            {
                VideoId = videoId,
                ContentStatus = true,
                ProcessingJobId = await processingTask,
                RecommendationTaskId = await analyticsTask,
                EstimatedCompletionTime = DateTime.UtcNow.AddMinutes(5)
            };

            _logger.LogInformation(
                "Video upload processed successfully. VideoId: {VideoId}, " +
                "ProcessingJobId: {ProcessingJobId}, RecommendationTaskId: {RecommendationTaskId}",
                response.VideoId,
                response.ProcessingJobId,
                response.RecommendationTaskId
            );

            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing video upload");
            throw;
        }
    }

    private async Task<VideoDto?> GetVideoFromContentApiAsync(string videoId)
    {
        try
        {
            var response = await _contentClient.GetAsync($"/api/videos/{videoId}");
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Failed to get video {VideoId} from Content.API: {StatusCode}",
                    videoId, response.StatusCode);
                return null;
            }

            return await response.Content.ReadFromJsonAsync<VideoDto>(_jsonOptions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calling Content.API for video {VideoId}", videoId);
            return null;
        }
    }

    private async Task<List<VideoRecommendation>?> GetRecommendationsAsync(string videoId)
    {
        try
        {
            var response = await _analyticsClient.GetAsync($"/api/recommendations/{videoId}?topN=4");
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Failed to get recommendations for video {VideoId}: {StatusCode}",
                    videoId, response.StatusCode);
                return new List<VideoRecommendation>();
            }

            var result = await response.Content.ReadFromJsonAsync<RecommendationResponse>(_jsonOptions);
            return result?.Recommendations;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calling Analytics.Function for recommendations");
            return new List<VideoRecommendation>();
        }
    }

    private async Task<ProcessingStatusDto?> GetProcessingStatusAsync(string videoId)
    {
        try
        {
            var response = await _processingClient.GetAsync($"/api/processing/status/{videoId}");
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Failed to get processing status for video {VideoId}: {StatusCode}",
                    videoId, response.StatusCode);
                return null;
            }

            return await response.Content.ReadFromJsonAsync<ProcessingStatusDto>(_jsonOptions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calling Processing.Worker for status");
            return null;
        }
    }

    private async Task<List<VideoDto>?> GetSimilarVideosAsync(string videoId)
    {
        try
        {
            var response = await _analyticsClient.GetAsync($"/api/recommendations/similar/{videoId}?limit=6");
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Failed to get similar videos for {VideoId}: {StatusCode}",
                    videoId, response.StatusCode);
                return new List<VideoDto>();
            }

            return await response.Content.ReadFromJsonAsync<List<VideoDto>>(_jsonOptions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting similar videos");
            return new List<VideoDto>();
        }
    }

    private async Task<List<CommentDto>?> GetPopularCommentsAsync(string videoId)
    {
        try
        {
            var response = await _contentClient.GetAsync($"/api/videos/{videoId}/comments/popular?limit=5");
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Failed to get popular comments for {VideoId}: {StatusCode}",
                    videoId, response.StatusCode);
                return new List<CommentDto>();
            }

            return await response.Content.ReadFromJsonAsync<List<CommentDto>>(_jsonOptions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting popular comments");
            return new List<CommentDto>();
        }
    }

    private async Task<VideoMetadataDto?> GetVideoMetadataAsync(string videoId)
    {
        try
        {
            // Aggregate metadata from multiple sources in parallel
            var basicInfoTask = _contentClient.GetAsync($"/api/videos/{videoId}/metadata");
            var engagementTask = _analyticsClient.GetAsync($"/api/analytics/videos/{videoId}/engagement");
            var processingInfoTask = _processingClient.GetAsync($"/api/processing/metadata/{videoId}");

            await Task.WhenAll(basicInfoTask, engagementTask, processingInfoTask);

            var metadata = new VideoMetadataDto();

            if (basicInfoTask.Result.IsSuccessStatusCode)
                metadata.BasicInfo = await basicInfoTask.Result.Content.ReadFromJsonAsync<BasicVideoInfoDto>(_jsonOptions);

            if (engagementTask.Result.IsSuccessStatusCode)
                metadata.EngagementStats = await engagementTask.Result.Content.ReadFromJsonAsync<EngagementStatsDto>(_jsonOptions);

            if (processingInfoTask.Result.IsSuccessStatusCode)
                metadata.ProcessingInfo = await processingInfoTask.Result.Content.ReadFromJsonAsync<ProcessingInfoDto>(_jsonOptions);

            return metadata;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error aggregating video metadata");
            return null;
        }
    }

    private async Task<string?> StartVideoProcessingAsync(string videoId, string filePath)
    {
        try
        {
            var request = new { VideoId = videoId, FilePath = filePath };
            var response = await _processingClient.PostAsJsonAsync("/api/processing/start", request, _jsonOptions);
            response.EnsureSuccessStatusCode();

            var jobId = await response.Content.ReadAsStringAsync();
            return jobId;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error starting video processing");
            return null;
        }
    }

    private async Task<string?> StartRecommendationCalculationAsync(string videoId, VideoMetadataInput metadata)
    {
        try
        {
            var request = new { VideoId = videoId, Metadata = metadata };
            var response = await _analyticsClient.PostAsJsonAsync("/api/recommendations/calculate", request, _jsonOptions);
            response.EnsureSuccessStatusCode();

            var taskId = await response.Content.ReadAsStringAsync();
            return taskId;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error starting recommendation calculation");
            return null;
        }
    }

    private class RecommendationResponse
    {
        public List<VideoRecommendation> Recommendations { get; set; } = new();
    }
}
