namespace MicroVideoPlatform.Web.UI.Services;

/// <summary>
/// HTTP client for Analytics.Function communication.
/// </summary>
public interface IAnalyticsApiClient
{
    Task<List<VideoRecommendation>> GetRecommendationsAsync(string videoId, int topN = 10);
    Task<List<VideoRecommendation>> GetPersonalizedRecommendationsAsync(string userId, List<string> watchedVideoIds, int topN = 10);
    Task<CommentSentimentResponse> AnalyzeCommentsAsync(string videoId, List<CommentForAnalysis> comments);
}

public class VideoRecommendation
{
    public string VideoId { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public float Score { get; set; }
    public string ReasonCode { get; set; } = string.Empty;
}

public class CommentForAnalysis
{
    public string CommentId { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
    public string AuthorId { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

public class CommentSentimentResponse
{
    public string VideoId { get; set; } = string.Empty;
    public int TotalComments { get; set; }
    public int PositiveCount { get; set; }
    public int NegativeCount { get; set; }
    public float OverallSentimentScore { get; set; }
}
