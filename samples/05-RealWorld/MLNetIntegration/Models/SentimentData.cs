using Microsoft.ML.Data;

namespace MLNetIntegration.Models;

/// <summary>
/// Input data model for sentiment analysis (Binary Classification)
/// </summary>
public class SentimentData
{
    [LoadColumn(0)]
    public string SentimentText { get; set; } = "";

    [LoadColumn(1), ColumnName("Label")]
    public bool Sentiment { get; set; }
}

/// <summary>
/// Prediction output model for sentiment analysis
/// </summary>
public class SentimentPrediction
{
    [ColumnName("PredictedLabel")]
    public bool Prediction { get; set; }

    public float Probability { get; set; }

    public float Score { get; set; }
}
