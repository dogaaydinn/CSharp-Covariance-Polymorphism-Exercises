using System.Net.Http.Json;
using System.Text.Json;

namespace MicroVideoPlatform.Web.UI.Services;

public class AnalyticsApiClient : IAnalyticsApiClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<AnalyticsApiClient> _logger;
    private readonly JsonSerializerOptions _jsonOptions;

    public AnalyticsApiClient(HttpClient httpClient, ILogger<AnalyticsApiClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
    }

    public async Task<List<VideoRecommendation>> GetRecommendationsAsync(string videoId, int topN = 10)
    {
        try
        {
            var response = await _httpClient.GetAsync($"/api/recommendations/{videoId}?topN={topN}");
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Failed to get recommendations for video {VideoId}", videoId);
                return new List<VideoRecommendation>();
            }

            var result = await response.Content.ReadFromJsonAsync<RecommendationResponse>(_jsonOptions);
            return result?.Recommendations ?? new List<VideoRecommendation>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting recommendations for video {VideoId}", videoId);
            return new List<VideoRecommendation>();
        }
    }

    public async Task<List<VideoRecommendation>> GetPersonalizedRecommendationsAsync(string userId, List<string> watchedVideoIds, int topN = 10)
    {
        try
        {
            var request = new PersonalizedRecommendationRequest
            {
                UserId = userId,
                WatchedVideoIds = watchedVideoIds,
                TopN = topN
            };

            var response = await _httpClient.PostAsJsonAsync("/api/recommendations/personalized", request, _jsonOptions);
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Failed to get personalized recommendations for user {UserId}", userId);
                return new List<VideoRecommendation>();
            }

            var result = await response.Content.ReadFromJsonAsync<RecommendationResponse>(_jsonOptions);
            return result?.Recommendations ?? new List<VideoRecommendation>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting personalized recommendations for user {UserId}", userId);
            return new List<VideoRecommendation>();
        }
    }

    public async Task<CommentSentimentResponse> AnalyzeCommentsAsync(string videoId, List<CommentForAnalysis> comments)
    {
        try
        {
            var request = new CommentAnalysisRequest
            {
                VideoId = videoId,
                Comments = comments
            };

            var response = await _httpClient.PostAsJsonAsync("/api/analyze/comments", request, _jsonOptions);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<CommentSentimentResponse>(_jsonOptions);
            return result ?? new CommentSentimentResponse { VideoId = videoId };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error analyzing comments for video {VideoId}", videoId);
            return new CommentSentimentResponse { VideoId = videoId };
        }
    }

    private class RecommendationResponse
    {
        public List<VideoRecommendation> Recommendations { get; set; } = new();
    }

    private class PersonalizedRecommendationRequest
    {
        public string UserId { get; set; } = string.Empty;
        public List<string> WatchedVideoIds { get; set; } = new();
        public int TopN { get; set; }
    }

    private class CommentAnalysisRequest
    {
        public string VideoId { get; set; } = string.Empty;
        public List<CommentForAnalysis> Comments { get; set; } = new();
    }
}
