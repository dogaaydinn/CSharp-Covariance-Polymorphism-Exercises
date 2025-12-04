using Microsoft.Extensions.Logging;
using Microsoft.ML;
using Microsoft.ML.Data;
using MicroVideoPlatform.Analytics.Function.Models;

namespace MicroVideoPlatform.Analytics.Function.Services;

/// <summary>
/// ML.NET-based sentiment analysis service for video comments.
///
/// ALGORITHM:
/// 1. Text Featurization: Convert comment text to TF-IDF vectors
/// 2. Binary Classification: Train model to predict positive/negative sentiment
/// 3. Prediction: Return sentiment label and confidence score
///
/// TRAINING DATA FORMAT:
/// CommentId, Text, Label (true=positive, false=negative)
/// </summary>
public class VideoCommentAnalyzer
{
    private readonly MLContext _mlContext;
    private readonly ILogger<VideoCommentAnalyzer> _logger;
    private ITransformer? _model;
    private PredictionEngine<CommentData, SentimentPrediction>? _predictionEngine;

    public VideoCommentAnalyzer(ILogger<VideoCommentAnalyzer> logger)
    {
        _mlContext = new MLContext(seed: 0);
        _logger = logger;
    }

    /// <summary>
    /// Trains the sentiment analysis model on labeled comment data.
    /// </summary>
    public void TrainModel(IEnumerable<CommentData> trainingData)
    {
        _logger.LogInformation("Training sentiment analysis model on {Count} comments", trainingData.Count());

        // Load data
        var data = _mlContext.Data.LoadFromEnumerable(trainingData);

        // Split data into training (80%) and test (20%) sets
        var split = _mlContext.Data.TrainTestSplit(data, testFraction: 0.2);

        // Build pipeline:
        // 1. Text featurization (TF-IDF)
        // 2. Concatenate features
        // 3. Binary classification (SDCA - Stochastic Dual Coordinate Ascent)
        var pipeline = _mlContext.Transforms.Text.FeaturizeText(
                outputColumnName: "Features",
                inputColumnName: nameof(CommentData.Text))
            .Append(_mlContext.BinaryClassification.Trainers.SdcaLogisticRegression(
                labelColumnName: nameof(CommentData.Label),
                featureColumnName: "Features"))
            .Append(_mlContext.Transforms.Conversion.MapKeyToValue("PredictedLabel"));

        // Train the model
        _model = pipeline.Fit(split.TrainSet);

        // Evaluate on test set
        var predictions = _model.Transform(split.TestSet);
        var metrics = _mlContext.BinaryClassification.Evaluate(
            predictions,
            labelColumnName: nameof(CommentData.Label));

        _logger.LogInformation(
            "Model training completed. Accuracy: {Accuracy:P2}, AUC: {Auc:P2}, F1: {F1:P2}",
            metrics.Accuracy,
            metrics.AreaUnderRocCurve,
            metrics.F1Score);

        // Create prediction engine
        _predictionEngine = _mlContext.Model.CreatePredictionEngine<CommentData, SentimentPrediction>(_model);

        _logger.LogInformation("Prediction engine created");
    }

    /// <summary>
    /// Loads a pre-trained model from file.
    /// </summary>
    public void LoadModel(string modelPath)
    {
        _logger.LogInformation("Loading model from {Path}", modelPath);

        _model = _mlContext.Model.Load(modelPath, out var modelInputSchema);
        _predictionEngine = _mlContext.Model.CreatePredictionEngine<CommentData, SentimentPrediction>(_model);

        _logger.LogInformation("Model loaded successfully");
    }

    /// <summary>
    /// Saves the trained model to file.
    /// </summary>
    public void SaveModel(string modelPath)
    {
        if (_model == null)
        {
            throw new InvalidOperationException("Model has not been trained yet.");
        }

        _logger.LogInformation("Saving model to {Path}", modelPath);

        using var fileStream = File.Create(modelPath);
        _mlContext.Model.Save(_model, null, fileStream);

        _logger.LogInformation("Model saved successfully");
    }

    /// <summary>
    /// Analyzes sentiment of a single comment.
    /// </summary>
    public SentimentPrediction AnalyzeComment(string commentText)
    {
        if (_predictionEngine == null)
        {
            throw new InvalidOperationException("Model has not been trained or loaded yet.");
        }

        var commentData = new CommentData { Text = commentText };
        var prediction = _predictionEngine.Predict(commentData);

        _logger.LogDebug(
            "Analyzed comment: '{Text}' -> {Sentiment} ({Confidence:P2})",
            commentText.Length > 50 ? commentText[..50] + "..." : commentText,
            prediction.Sentiment,
            prediction.Confidence / 100);

        return prediction;
    }

    /// <summary>
    /// Analyzes sentiment of multiple comments in batch.
    /// More efficient than calling AnalyzeComment multiple times.
    /// </summary>
    public CommentAnalysisResponse AnalyzeBatch(CommentAnalysisRequest request)
    {
        if (_model == null)
        {
            throw new InvalidOperationException("Model has not been trained or loaded yet.");
        }

        _logger.LogInformation(
            "Analyzing batch of {Count} comments for video {VideoId}",
            request.Comments.Count,
            request.VideoId);

        // Convert to ML.NET format
        var commentData = request.Comments.Select(c => new CommentData
        {
            CommentId = c.CommentId,
            Text = c.Text
        }).ToList();

        // Load as IDataView for batch prediction
        var dataView = _mlContext.Data.LoadFromEnumerable(commentData);

        // Transform (predict)
        var predictions = _model.Transform(dataView);

        // Convert back to list
        var results = _mlContext.Data.CreateEnumerable<SentimentPredictionWithId>(
            predictions,
            reuseRowObject: false).ToList();

        // Build response
        var commentSentiments = results.Zip(request.Comments, (pred, comment) =>
            new CommentSentiment
            {
                CommentId = comment.CommentId,
                Text = comment.Text,
                Sentiment = pred.Sentiment,
                Confidence = pred.Confidence,
                Score = pred.Score
            }).ToList();

        int positiveCount = commentSentiments.Count(c => c.Sentiment == "Positive");
        int negativeCount = commentSentiments.Count(c => c.Sentiment == "Negative");

        // Calculate overall sentiment score (-1 to +1)
        float overallScore = commentSentiments.Count > 0
            ? commentSentiments.Average(c => c.Sentiment == "Positive" ? c.Confidence / 100 : -(c.Confidence / 100))
            : 0;

        var response = new CommentAnalysisResponse
        {
            VideoId = request.VideoId,
            TotalComments = request.Comments.Count,
            PositiveCount = positiveCount,
            NegativeCount = negativeCount,
            OverallSentimentScore = overallScore,
            CommentSentiments = commentSentiments
        };

        _logger.LogInformation(
            "Batch analysis completed: {Positive} positive, {Negative} negative, overall score: {Score:F2}",
            positiveCount,
            negativeCount,
            overallScore);

        return response;
    }

    /// <summary>
    /// Gets overall sentiment statistics for a video.
    /// </summary>
    public (int positive, int negative, float overallScore) GetSentimentStatistics(
        List<SentimentPrediction> predictions)
    {
        int positive = predictions.Count(p => p.Prediction);
        int negative = predictions.Count(p => !p.Prediction);

        // Calculate weighted average score
        float overallScore = predictions.Count > 0
            ? predictions.Average(p => p.Prediction ? p.Probability : -p.Probability)
            : 0;

        return (positive, negative, overallScore);
    }
}

/// <summary>
/// Internal class for batch prediction results with CommentId.
/// </summary>
internal class SentimentPredictionWithId : SentimentPrediction
{
    public string CommentId { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
}
