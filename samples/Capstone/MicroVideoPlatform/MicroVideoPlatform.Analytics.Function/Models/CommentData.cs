using Microsoft.ML.Data;

namespace MicroVideoPlatform.Analytics.Function.Models;

/// <summary>
/// Comment data model for sentiment analysis training.
/// </summary>
public class CommentData
{
    [LoadColumn(0)]
    public string CommentId { get; set; } = string.Empty;

    [LoadColumn(1)]
    public string Text { get; set; } = string.Empty;

    [LoadColumn(2)]
    public bool Label { get; set; } // true = positive, false = negative
}

/// <summary>
/// Sentiment analysis prediction result.
/// </summary>
public class SentimentPrediction
{
    [ColumnName("PredictedLabel")]
    public bool Prediction { get; set; }

    [ColumnName("Probability")]
    public float Probability { get; set; }

    [ColumnName("Score")]
    public float Score { get; set; }

    /// <summary>
    /// Gets sentiment as human-readable string.
    /// </summary>
    public string Sentiment => Prediction ? "Positive" : "Negative";

    /// <summary>
    /// Gets confidence percentage (0-100).
    /// </summary>
    public float Confidence => Probability * 100;
}

/// <summary>
/// Batch comment analysis request.
/// </summary>
public class CommentAnalysisRequest
{
    public string VideoId { get; set; } = string.Empty;
    public List<CommentItem> Comments { get; set; } = new();
}

/// <summary>
/// Single comment item for batch processing.
/// </summary>
public class CommentItem
{
    public string CommentId { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
    public string AuthorId { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// Batch comment analysis response.
/// </summary>
public class CommentAnalysisResponse
{
    public string VideoId { get; set; } = string.Empty;
    public int TotalComments { get; set; }
    public int PositiveCount { get; set; }
    public int NegativeCount { get; set; }
    public float OverallSentimentScore { get; set; } // -1 to +1
    public List<CommentSentiment> CommentSentiments { get; set; } = new();
}

/// <summary>
/// Individual comment sentiment result.
/// </summary>
public class CommentSentiment
{
    public string CommentId { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
    public string Sentiment { get; set; } = string.Empty;
    public float Confidence { get; set; }
    public float Score { get; set; }
}
