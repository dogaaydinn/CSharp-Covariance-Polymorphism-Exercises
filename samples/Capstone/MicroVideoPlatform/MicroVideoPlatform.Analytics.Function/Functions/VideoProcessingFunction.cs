using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using MicroVideoPlatform.Analytics.Function.Models;
using MicroVideoPlatform.Analytics.Function.Services;
using Npgsql;
using Dapper;

namespace MicroVideoPlatform.Analytics.Function.Functions;

/// <summary>
/// Azure Function that processes videos periodically to update recommendations.
///
/// TRIGGER: Timer (every 5 minutes)
/// PURPOSE: Keep recommendation model fresh with latest video data
/// </summary>
public class VideoProcessingFunction
{
    private readonly ILogger<VideoProcessingFunction> _logger;
    private readonly VideoRecommendationService _recommendationService;
    private readonly string _connectionString;

    public VideoProcessingFunction(
        ILogger<VideoProcessingFunction> logger,
        VideoRecommendationService recommendationService)
    {
        _logger = logger;
        _recommendationService = recommendationService;
        _connectionString = Environment.GetEnvironmentVariable("PostgreSQL_ConnectionString")
            ?? throw new InvalidOperationException("PostgreSQL_ConnectionString not configured");
    }

    /// <summary>
    /// Runs every 5 minutes to update recommendation model.
    /// Cron expression: 0 */5 * * * * (every 5 minutes)
    /// </summary>
    [Function("VideoProcessingFunction")]
    public async Task Run([TimerTrigger("0 */5 * * * *")] TimerInfo myTimer)
    {
        _logger.LogInformation("VideoProcessingFunction executed at: {Time}", DateTime.UtcNow);

        try
        {
            // Fetch latest video data from database
            var videos = await FetchVideosFromDatabaseAsync();

            if (videos.Count == 0)
            {
                _logger.LogWarning("No videos found in database");
                return;
            }

            // Train/update recommendation model
            _recommendationService.TrainModel(videos);

            _logger.LogInformation("Successfully updated recommendation model with {Count} videos", videos.Count);

            // TODO: Optionally save model to blob storage for persistence
            // await SaveModelToStorageAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating recommendation model");
        }

        if (myTimer.ScheduleStatus is not null)
        {
            _logger.LogInformation("Next timer schedule at: {NextRun}", myTimer.ScheduleStatus.Next);
        }
    }

    /// <summary>
    /// Fetches video data from PostgreSQL database.
    /// </summary>
    private async Task<List<VideoData>> FetchVideosFromDatabaseAsync()
    {
        using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();

        const string query = @"
            SELECT
                id::text as VideoId,
                title as Title,
                description as Description,
                category as Category,
                COALESCE(tags::text, '') as Tags,
                views_count::real as ViewsCount,
                likes_count::real as LikesCount,
                duration_seconds::real as DurationSeconds
            FROM videos
            WHERE status = 'Ready'
            ORDER BY uploaded_at DESC
            LIMIT 1000";

        var videos = await connection.QueryAsync<VideoData>(query);

        return videos.ToList();
    }
}

/// <summary>
/// TimerInfo class for timer trigger.
/// </summary>
public class TimerInfo
{
    public TimerScheduleStatus? ScheduleStatus { get; set; }
}

public class TimerScheduleStatus
{
    public DateTime Last { get; set; }
    public DateTime Next { get; set; }
    public DateTime LastUpdated { get; set; }
}
