namespace MicroVideoPlatform.Shared.DTOs;

/// <summary>
/// Data transfer object for video analytics/classification results
/// </summary>
public sealed record VideoAnalyticsResultDto
{
    /// <summary>
    /// Video ID that was analyzed
    /// </summary>
    public required Guid VideoId { get; init; }

    /// <summary>
    /// Predicted category for the video
    /// </summary>
    public required string PredictedCategory { get; init; }

    /// <summary>
    /// Confidence score for the prediction (0.0 - 1.0)
    /// </summary>
    public required decimal ConfidenceScore { get; init; }

    /// <summary>
    /// Alternative categories with confidence scores
    /// </summary>
    public Dictionary<string, decimal>? AlternativeCategories { get; init; }

    /// <summary>
    /// Time when analysis was performed
    /// </summary>
    public DateTime AnalyzedAt { get; init; }

    /// <summary>
    /// ML model version used for analysis
    /// </summary>
    public string? ModelVersion { get; init; }

    /// <summary>
    /// Extracted features from the video
    /// </summary>
    public Dictionary<string, string>? Features { get; init; }

    /// <summary>
    /// Tags automatically generated from analysis
    /// </summary>
    public string[]? GeneratedTags { get; init; }

    /// <summary>
    /// Indicates if content is appropriate (safe for work)
    /// </summary>
    public bool? IsAppropriate { get; init; }

    /// <summary>
    /// Content rating (e.g., "G", "PG", "PG-13", "R")
    /// </summary>
    public string? ContentRating { get; init; }

    /// <summary>
    /// Confidence score as percentage (0-100)
    /// </summary>
    public decimal ConfidencePercentage => ConfidenceScore * 100;

    /// <summary>
    /// Indicates if the prediction is reliable (confidence >= 70%)
    /// </summary>
    public bool IsReliablePrediction => ConfidenceScore >= 0.70m;

    /// <summary>
    /// Top 3 alternative categories
    /// </summary>
    public Dictionary<string, decimal> TopAlternatives => AlternativeCategories?
        .OrderByDescending(x => x.Value)
        .Take(3)
        .ToDictionary(x => x.Key, x => x.Value)
        ?? new Dictionary<string, decimal>();
}
