using System.Net;
using System.Text.Json;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using MicroVideoPlatform.Analytics.Function.Services;

namespace MicroVideoPlatform.Analytics.Function.Functions;

/// <summary>
/// Azure Function for video recommendations.
///
/// ENDPOINTS:
/// - GET /api/recommendations/{videoId} - Get recommendations based on video
/// - POST /api/recommendations/personalized - Get personalized recommendations for user
/// </summary>
public class RecommendationFunction
{
    private readonly ILogger<RecommendationFunction> _logger;
    private readonly VideoRecommendationService _recommendationService;

    public RecommendationFunction(
        ILogger<RecommendationFunction> logger,
        VideoRecommendationService recommendationService)
    {
        _logger = logger;
        _recommendationService = recommendationService;
    }

    /// <summary>
    /// Gets video recommendations based on a source video.
    /// GET /api/recommendations/{videoId}?topN=10
    /// </summary>
    [Function("GetRecommendations")]
    public HttpResponseData GetRecommendations(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "recommendations/{videoId}")] HttpRequestData req,
        string videoId)
    {
        _logger.LogInformation("Getting recommendations for video: {VideoId}", videoId);

        try
        {
            // Parse query parameters
            var query = System.Web.HttpUtility.ParseQueryString(req.Url.Query);
            int topN = int.TryParse(query["topN"], out var n) ? n : 10;

            // Get recommendations
            var recommendations = _recommendationService.GetRecommendations(videoId, topN);

            var result = new
            {
                sourceVideoId = videoId,
                recommendationsCount = recommendations.Count,
                recommendations = recommendations
            };

            // Return results
            var response = req.CreateResponse(HttpStatusCode.OK);
            response.WriteAsJsonAsync(result).Wait();

            return response;
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex, "Model not trained");

            var errorResponse = req.CreateResponse(HttpStatusCode.ServiceUnavailable);
            errorResponse.WriteAsJsonAsync(new { error = "Recommendation model not ready", message = ex.Message }).Wait();
            return errorResponse;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting recommendations");

            var errorResponse = req.CreateResponse(HttpStatusCode.InternalServerError);
            errorResponse.WriteAsJsonAsync(new { error = "Internal server error", message = ex.Message }).Wait();
            return errorResponse;
        }
    }

    /// <summary>
    /// Gets personalized recommendations based on user watch history.
    /// POST /api/recommendations/personalized
    /// Body: { "userId": "user123", "watchedVideoIds": ["vid1", "vid2"], "topN": 10 }
    /// </summary>
    [Function("GetPersonalizedRecommendations")]
    public async Task<HttpResponseData> GetPersonalizedRecommendations(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = "recommendations/personalized")] HttpRequestData req)
    {
        _logger.LogInformation("Getting personalized recommendations");

        try
        {
            // Parse request body
            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var request = JsonSerializer.Deserialize<PersonalizedRecommendationRequest>(requestBody,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (request == null || string.IsNullOrWhiteSpace(request.UserId))
            {
                var badRequest = req.CreateResponse(HttpStatusCode.BadRequest);
                await badRequest.WriteAsJsonAsync(new { error = "Invalid request: userId is required" });
                return badRequest;
            }

            if (request.WatchedVideoIds == null || !request.WatchedVideoIds.Any())
            {
                var badRequest = req.CreateResponse(HttpStatusCode.BadRequest);
                await badRequest.WriteAsJsonAsync(new { error = "Invalid request: watchedVideoIds array is required" });
                return badRequest;
            }

            // Get personalized recommendations
            var recommendations = _recommendationService.GetPersonalizedRecommendations(
                request.UserId,
                request.WatchedVideoIds,
                request.TopN ?? 10);

            var result = new
            {
                userId = request.UserId,
                basedOnVideos = request.WatchedVideoIds.Count,
                recommendationsCount = recommendations.Count,
                recommendations = recommendations
            };

            // Return results
            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(result);

            return response;
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex, "Model not trained");

            var errorResponse = req.CreateResponse(HttpStatusCode.ServiceUnavailable);
            await errorResponse.WriteAsJsonAsync(new { error = "Recommendation model not ready", message = ex.Message });
            return errorResponse;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting personalized recommendations");

            var errorResponse = req.CreateResponse(HttpStatusCode.InternalServerError);
            await errorResponse.WriteAsJsonAsync(new { error = "Internal server error", message = ex.Message });
            return errorResponse;
        }
    }
}

/// <summary>
/// Request model for personalized recommendations.
/// </summary>
public class PersonalizedRecommendationRequest
{
    public string UserId { get; set; } = string.Empty;
    public List<string> WatchedVideoIds { get; set; } = new();
    public int? TopN { get; set; }
}
