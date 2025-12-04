using Microsoft.ML.Data;

namespace MicroVideoPlatform.Analytics.Function.Models;

/// <summary>
/// Video data model for ML.NET training and prediction.
/// Used for content-based filtering in recommendation system.
/// </summary>
public class VideoData
{
    [LoadColumn(0)]
    public string VideoId { get; set; } = string.Empty;

    [LoadColumn(1)]
    public string Title { get; set; } = string.Empty;

    [LoadColumn(2)]
    public string Description { get; set; } = string.Empty;

    [LoadColumn(3)]
    public string Category { get; set; } = string.Empty;

    [LoadColumn(4)]
    public string Tags { get; set; } = string.Empty; // Comma-separated tags

    [LoadColumn(5)]
    public float ViewsCount { get; set; }

    [LoadColumn(6)]
    public float LikesCount { get; set; }

    [LoadColumn(7)]
    public float DurationSeconds { get; set; }

    /// <summary>
    /// Feature vector combining title + description + tags.
    /// Used for text featurization.
    /// </summary>
    public string CombinedFeatures => $"{Title} {Description} {Tags}";
}

/// <summary>
/// User-video interaction data for collaborative filtering.
/// </summary>
public class UserVideoInteraction
{
    [LoadColumn(0)]
    public string UserId { get; set; } = string.Empty;

    [LoadColumn(1)]
    public string VideoId { get; set; } = string.Empty;

    [LoadColumn(2)]
    public float Rating { get; set; } // Implicit rating: watch time / video duration

    [LoadColumn(3)]
    public float Label { get; set; } // 1 if watched > 50%, 0 otherwise
}

/// <summary>
/// Video recommendation prediction result.
/// </summary>
public class VideoRecommendation
{
    public string VideoId { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public float Score { get; set; } // Similarity score or predicted rating
    public string ReasonCode { get; set; } = string.Empty; // "content_based" | "collaborative" | "hybrid"
}
