using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace Analytics.Function;

public class AnalyticsFunction
{
    private readonly ILogger<AnalyticsFunction> _logger;
    private static readonly Dictionary<Guid, int> _viewCounts = new();

    public AnalyticsFunction(ILogger<AnalyticsFunction> logger)
    {
        _logger = logger;
    }

    [Function("TrackView")]
    public async Task<HttpResponseData> TrackView(
        [HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequestData req)
    {
        var request = await req.ReadFromJsonAsync<TrackViewRequest>();

        if (request == null || request.VideoId == Guid.Empty)
        {
            var badResponse = req.CreateResponse(System.Net.HttpStatusCode.BadRequest);
            await badResponse.WriteAsJsonAsync(new { error = "Invalid request" });
            return badResponse;
        }

        // Track view count
        if (!_viewCounts.ContainsKey(request.VideoId))
            _viewCounts[request.VideoId] = 0;

        _viewCounts[request.VideoId]++;

        _logger.LogInformation("Video {VideoId} viewed. Total views: {ViewCount}",
            request.VideoId,
            _viewCounts[request.VideoId]);

        var response = req.CreateResponse(System.Net.HttpStatusCode.OK);
        await response.WriteAsJsonAsync(new
        {
            videoId = request.VideoId,
            viewCount = _viewCounts[request.VideoId],
            message = "View tracked successfully"
        });

        return response;
    }

    [Function("GetAnalytics")]
    public async Task<HttpResponseData> GetAnalytics(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "analytics/{videoId:guid}")] HttpRequestData req,
        Guid videoId)
    {
        var viewCount = _viewCounts.TryGetValue(videoId, out var count) ? count : 0;

        var response = req.CreateResponse(System.Net.HttpStatusCode.OK);
        await response.WriteAsJsonAsync(new
        {
            videoId,
            viewCount,
            analyticsType = "serverless-function"
        });

        return response;
    }

    [Function("GetAllAnalytics")]
    public async Task<HttpResponseData> GetAllAnalytics(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "analytics")] HttpRequestData req)
    {
        var analytics = _viewCounts.Select(kvp => new
        {
            videoId = kvp.Key,
            viewCount = kvp.Value
        }).ToList();

        var response = req.CreateResponse(System.Net.HttpStatusCode.OK);
        await response.WriteAsJsonAsync(analytics);

        return response;
    }
}

public record TrackViewRequest(Guid VideoId);
