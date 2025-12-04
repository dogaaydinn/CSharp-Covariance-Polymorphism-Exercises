using Microsoft.Extensions.Logging;
using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Transforms.Text;
using MicroVideoPlatform.Analytics.Function.Models;

namespace MicroVideoPlatform.Analytics.Function.Services;

/// <summary>
/// ML.NET-based video recommendation service using content-based filtering
/// and cosine similarity for finding similar videos.
///
/// ALGORITHM:
/// 1. Text Featurization: Convert video metadata (title + description + tags) to TF-IDF vectors
/// 2. Cosine Similarity: Calculate similarity between vectors
/// 3. Ranking: Sort by similarity score and return top N recommendations
/// </summary>
public class VideoRecommendationService
{
    private readonly MLContext _mlContext;
    private readonly ILogger<VideoRecommendationService> _logger;
    private ITransformer? _model;
    private IDataView? _trainingData;

    public VideoRecommendationService(ILogger<VideoRecommendationService> logger)
    {
        _mlContext = new MLContext(seed: 0);
        _logger = logger;
    }

    /// <summary>
    /// Trains the recommendation model on video metadata.
    /// Uses TF-IDF for text featurization.
    /// </summary>
    public void TrainModel(IEnumerable<VideoData> videos)
    {
        _logger.LogInformation("Training recommendation model on {Count} videos", videos.Count());

        // Convert to IDataView
        _trainingData = _mlContext.Data.LoadFromEnumerable(videos);

        // Build pipeline:
        // 1. Text featurization (TF-IDF)
        // 2. Normalize features
        var pipeline = _mlContext.Transforms.Text.FeaturizeText(
                outputColumnName: "Features",
                options: new TextFeaturizingEstimator.Options
                {
                    OutputTokensColumnName = "Tokens",
                    StopWordsRemoverOptions = new StopWordsRemovingEstimator.Options(),
                    CaseMode = TextNormalizingEstimator.CaseMode.Lower,
                    KeepDiacritics = false,
                    KeepPunctuations = false,
                    KeepNumbers = true
                },
                inputColumnName: nameof(VideoData.CombinedFeatures))
            .Append(_mlContext.Transforms.NormalizeMinMax(
                outputColumnName: "NormalizedFeatures",
                inputColumnName: "Features"));

        // Train the model
        _model = pipeline.Fit(_trainingData);

        _logger.LogInformation("Model training completed");
    }

    /// <summary>
    /// Gets video recommendations based on cosine similarity.
    /// </summary>
    /// <param name="sourceVideoId">Video ID to find recommendations for</param>
    /// <param name="topN">Number of recommendations to return</param>
    /// <returns>List of recommended videos sorted by similarity score</returns>
    public List<VideoRecommendation> GetRecommendations(string sourceVideoId, int topN = 10)
    {
        if (_model == null || _trainingData == null)
        {
            throw new InvalidOperationException("Model has not been trained yet. Call TrainModel first.");
        }

        _logger.LogInformation("Generating recommendations for video {VideoId}", sourceVideoId);

        // Transform all videos to feature vectors
        var transformedData = _model.Transform(_trainingData);

        // Get feature vectors
        var features = _mlContext.Data.CreateEnumerable<VideoFeatures>(
            transformedData,
            reuseRowObject: false).ToList();

        // Find source video features
        var sourceVideo = features.FirstOrDefault(f => f.VideoId == sourceVideoId);
        if (sourceVideo == null)
        {
            _logger.LogWarning("Source video {VideoId} not found", sourceVideoId);
            return new List<VideoRecommendation>();
        }

        // Calculate cosine similarity for all videos
        var recommendations = features
            .Where(f => f.VideoId != sourceVideoId) // Exclude source video
            .Select(f => new VideoRecommendation
            {
                VideoId = f.VideoId,
                Title = f.Title,
                Category = f.Category,
                Score = CosineSimilarity(sourceVideo.NormalizedFeatures, f.NormalizedFeatures),
                ReasonCode = "content_based"
            })
            .OrderByDescending(r => r.Score)
            .Take(topN)
            .ToList();

        _logger.LogInformation("Generated {Count} recommendations", recommendations.Count);

        return recommendations;
    }

    /// <summary>
    /// Gets personalized recommendations based on user watch history.
    /// Uses collaborative filtering approach.
    /// </summary>
    public List<VideoRecommendation> GetPersonalizedRecommendations(
        string userId,
        List<string> watchedVideoIds,
        int topN = 10)
    {
        if (_model == null || _trainingData == null)
        {
            throw new InvalidOperationException("Model has not been trained yet.");
        }

        _logger.LogInformation(
            "Generating personalized recommendations for user {UserId} based on {Count} watched videos",
            userId,
            watchedVideoIds.Count);

        // Transform all videos to feature vectors
        var transformedData = _model.Transform(_trainingData);
        var features = _mlContext.Data.CreateEnumerable<VideoFeatures>(
            transformedData,
            reuseRowObject: false).ToList();

        // Get feature vectors for watched videos
        var watchedFeatures = features
            .Where(f => watchedVideoIds.Contains(f.VideoId))
            .ToList();

        if (!watchedFeatures.Any())
        {
            _logger.LogWarning("No watched videos found in training data");
            return new List<VideoRecommendation>();
        }

        // Calculate average feature vector (user profile)
        var userProfile = AverageFeatureVector(watchedFeatures.Select(f => f.NormalizedFeatures).ToList());

        // Find similar videos
        var recommendations = features
            .Where(f => !watchedVideoIds.Contains(f.VideoId)) // Exclude watched videos
            .Select(f => new VideoRecommendation
            {
                VideoId = f.VideoId,
                Title = f.Title,
                Category = f.Category,
                Score = CosineSimilarity(userProfile, f.NormalizedFeatures),
                ReasonCode = "personalized"
            })
            .OrderByDescending(r => r.Score)
            .Take(topN)
            .ToList();

        _logger.LogInformation("Generated {Count} personalized recommendations", recommendations.Count);

        return recommendations;
    }

    /// <summary>
    /// Calculates cosine similarity between two feature vectors.
    /// Formula: cos(θ) = (A · B) / (||A|| * ||B||)
    /// Range: -1 (opposite) to 1 (identical)
    /// </summary>
    private float CosineSimilarity(float[] vectorA, float[] vectorB)
    {
        if (vectorA.Length != vectorB.Length)
        {
            throw new ArgumentException("Vectors must have the same length");
        }

        // Dot product
        float dotProduct = 0;
        for (int i = 0; i < vectorA.Length; i++)
        {
            dotProduct += vectorA[i] * vectorB[i];
        }

        // Magnitudes
        float magnitudeA = (float)Math.Sqrt(vectorA.Sum(x => x * x));
        float magnitudeB = (float)Math.Sqrt(vectorB.Sum(x => x * x));

        // Avoid division by zero
        if (magnitudeA == 0 || magnitudeB == 0)
        {
            return 0;
        }

        return dotProduct / (magnitudeA * magnitudeB);
    }

    /// <summary>
    /// Calculates average feature vector from multiple vectors.
    /// Used to create user profile from watched videos.
    /// </summary>
    private float[] AverageFeatureVector(List<float[]> vectors)
    {
        if (!vectors.Any())
        {
            return Array.Empty<float>();
        }

        int length = vectors[0].Length;
        float[] average = new float[length];

        for (int i = 0; i < length; i++)
        {
            average[i] = vectors.Average(v => v[i]);
        }

        return average;
    }
}

/// <summary>
/// Internal class to hold video features after transformation.
/// </summary>
internal class VideoFeatures
{
    public string VideoId { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;

    [VectorType]
    public float[] Features { get; set; } = Array.Empty<float>();

    [VectorType]
    public float[] NormalizedFeatures { get; set; } = Array.Empty<float>();
}
