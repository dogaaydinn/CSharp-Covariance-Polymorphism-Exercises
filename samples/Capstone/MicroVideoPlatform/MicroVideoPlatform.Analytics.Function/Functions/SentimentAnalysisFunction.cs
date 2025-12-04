using System.Net;
using System.Text.Json;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using MicroVideoPlatform.Analytics.Function.Models;
using MicroVideoPlatform.Analytics.Function.Services;

namespace MicroVideoPlatform.Analytics.Function.Functions;

/// <summary>
/// Azure Function for sentiment analysis of video comments.
///
/// ENDPOINTS:
/// - POST /api/analyze/comments - Batch comment analysis
/// - POST /api/analyze/comment - Single comment analysis
/// </summary>
public class SentimentAnalysisFunction
{
    private readonly ILogger<SentimentAnalysisFunction> _logger;
    private readonly VideoCommentAnalyzer _commentAnalyzer;

    public SentimentAnalysisFunction(
        ILogger<SentimentAnalysisFunction> logger,
        VideoCommentAnalyzer commentAnalyzer)
    {
        _logger = logger;
        _commentAnalyzer = commentAnalyzer;

        // Initialize with sample training data (in production, load from database or file)
        InitializeModel();
    }

    /// <summary>
    /// Analyzes sentiment of multiple comments in batch.
    /// POST /api/analyze/comments
    /// </summary>
    [Function("AnalyzeComments")]
    public async Task<HttpResponseData> AnalyzeComments(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = "analyze/comments")] HttpRequestData req)
    {
        _logger.LogInformation("Processing batch comment analysis request");

        try
        {
            // Parse request body
            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var request = JsonSerializer.Deserialize<CommentAnalysisRequest>(requestBody,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (request == null || !request.Comments.Any())
            {
                var badRequest = req.CreateResponse(HttpStatusCode.BadRequest);
                await badRequest.WriteAsJsonAsync(new { error = "Invalid request: comments array is required" });
                return badRequest;
            }

            // Analyze batch
            var response = _commentAnalyzer.AnalyzeBatch(request);

            // Return results
            var httpResponse = req.CreateResponse(HttpStatusCode.OK);
            await httpResponse.WriteAsJsonAsync(response);

            return httpResponse;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error analyzing comments");

            var errorResponse = req.CreateResponse(HttpStatusCode.InternalServerError);
            await errorResponse.WriteAsJsonAsync(new { error = "Internal server error", message = ex.Message });
            return errorResponse;
        }
    }

    /// <summary>
    /// Analyzes sentiment of a single comment.
    /// POST /api/analyze/comment
    /// Body: { "text": "This video is amazing!" }
    /// </summary>
    [Function("AnalyzeComment")]
    public async Task<HttpResponseData> AnalyzeComment(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = "analyze/comment")] HttpRequestData req)
    {
        _logger.LogInformation("Processing single comment analysis request");

        try
        {
            // Parse request body
            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var request = JsonSerializer.Deserialize<SingleCommentRequest>(requestBody,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (request == null || string.IsNullOrWhiteSpace(request.Text))
            {
                var badRequest = req.CreateResponse(HttpStatusCode.BadRequest);
                await badRequest.WriteAsJsonAsync(new { error = "Invalid request: text field is required" });
                return badRequest;
            }

            // Analyze single comment
            var prediction = _commentAnalyzer.AnalyzeComment(request.Text);

            var result = new
            {
                text = request.Text,
                sentiment = prediction.Sentiment,
                confidence = prediction.Confidence,
                score = prediction.Score,
                analyzedAt = DateTime.UtcNow
            };

            // Return result
            var httpResponse = req.CreateResponse(HttpStatusCode.OK);
            await httpResponse.WriteAsJsonAsync(result);

            return httpResponse;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error analyzing comment");

            var errorResponse = req.CreateResponse(HttpStatusCode.InternalServerError);
            await errorResponse.WriteAsJsonAsync(new { error = "Internal server error", message = ex.Message });
            return errorResponse;
        }
    }

    /// <summary>
    /// Initializes the sentiment analysis model with sample data.
    /// In production, load from file or train on real data from database.
    /// </summary>
    private void InitializeModel()
    {
        try
        {
            // Sample training data (in production, load from database)
            var trainingData = new List<CommentData>
            {
                // Positive examples
                new() { CommentId = "1", Text = "This video is amazing! Great content!", Label = true },
                new() { CommentId = "2", Text = "Love it! Very helpful tutorial.", Label = true },
                new() { CommentId = "3", Text = "Excellent work, keep it up!", Label = true },
                new() { CommentId = "4", Text = "Best video I've seen on this topic", Label = true },
                new() { CommentId = "5", Text = "Thank you for sharing this!", Label = true },
                new() { CommentId = "6", Text = "Very informative and well explained", Label = true },
                new() { CommentId = "7", Text = "Great quality content, subscribed!", Label = true },
                new() { CommentId = "8", Text = "This helped me a lot, thanks!", Label = true },
                new() { CommentId = "9", Text = "Perfect! Exactly what I needed", Label = true },
                new() { CommentId = "10", Text = "Outstanding video, loved it", Label = true },

                // Negative examples
                new() { CommentId = "11", Text = "This is terrible, waste of time", Label = false },
                new() { CommentId = "12", Text = "Boring content, don't recommend", Label = false },
                new() { CommentId = "13", Text = "Not helpful at all", Label = false },
                new() { CommentId = "14", Text = "Disliked, very poor quality", Label = false },
                new() { CommentId = "15", Text = "This video is misleading", Label = false },
                new() { CommentId = "16", Text = "Awful content, unsubscribed", Label = false },
                new() { CommentId = "17", Text = "Worst tutorial ever", Label = false },
                new() { CommentId = "18", Text = "Complete waste of my time", Label = false },
                new() { CommentId = "19", Text = "Disappointed with this video", Label = false },
                new() { CommentId = "20", Text = "Horrible, do not watch", Label = false }
            };

            // Train model
            _commentAnalyzer.TrainModel(trainingData);

            _logger.LogInformation("Sentiment analysis model initialized with {Count} training examples", trainingData.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to initialize sentiment analysis model");
        }
    }
}

/// <summary>
/// Request model for single comment analysis.
/// </summary>
public class SingleCommentRequest
{
    public string Text { get; set; } = string.Empty;
}
