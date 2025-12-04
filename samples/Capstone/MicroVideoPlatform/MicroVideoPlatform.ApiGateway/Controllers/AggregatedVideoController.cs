using Microsoft.AspNetCore.Mvc;
using MicroVideoPlatform.ApiGateway.Models;
using MicroVideoPlatform.ApiGateway.Services;

namespace MicroVideoPlatform.ApiGateway.Controllers;

/// <summary>
/// Controller for aggregated video endpoints that combine data from multiple microservices.
/// </summary>
[ApiController]
[Route("api/aggregated")]
public class AggregatedVideoController : ControllerBase
{
    private readonly VideoAggregationService _aggregationService;
    private readonly ILogger<AggregatedVideoController> _logger;

    public AggregatedVideoController(
        VideoAggregationService aggregationService,
        ILogger<AggregatedVideoController> logger)
    {
        _aggregationService = aggregationService;
        _logger = logger;
    }

    /// <summary>
    /// Gets complete video details aggregated from all microservices.
    /// Combines: Video info (Content.API) + Recommendations (Analytics) + Processing status (Worker) + Metadata
    /// </summary>
    /// <param name="videoId">The video ID</param>
    /// <returns>Aggregated video response</returns>
    [HttpGet("video/{videoId}")]
    [ProducesResponseType(typeof(AggregatedVideoResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<AggregatedVideoResponse>> GetAggregatedVideoDetails(string videoId)
    {
        _logger.LogInformation("GET /api/aggregated/video/{VideoId} - Aggregating video details", videoId);

        try
        {
            var result = await _aggregationService.GetVideoDetailsAsync(videoId);

            if (result.Video == null)
            {
                _logger.LogWarning("Video {VideoId} not found", videoId);
                return NotFound(new { message = $"Video {videoId} not found" });
            }

            _logger.LogInformation(
                "Successfully aggregated video {VideoId}. Has {RecommendationCount} recommendations, {SimilarCount} similar videos",
                videoId,
                result.Recommendations?.Count ?? 0,
                result.SimilarVideos?.Count ?? 0
            );

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error aggregating video details for {VideoId}", videoId);
            return StatusCode(500, new { message = "An error occurred while aggregating video details" });
        }
    }

    /// <summary>
    /// Processes a video upload across multiple services.
    /// Orchestrates: Content.API (metadata) -> Processing.Worker (encoding) -> Analytics.Function (recommendations)
    /// </summary>
    /// <param name="request">Video upload request</param>
    /// <returns>Upload response with processing job IDs</returns>
    [HttpPost("upload")]
    [ProducesResponseType(typeof(VideoUploadResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<VideoUploadResponse>> ProcessVideoUpload([FromBody] VideoUploadRequest request)
    {
        _logger.LogInformation("POST /api/aggregated/upload - Processing video upload: {Title}", request.Metadata.Title);

        if (string.IsNullOrWhiteSpace(request.Metadata.Title))
        {
            return BadRequest(new { message = "Video title is required" });
        }

        if (string.IsNullOrWhiteSpace(request.FilePath))
        {
            return BadRequest(new { message = "File path is required" });
        }

        try
        {
            var result = await _aggregationService.ProcessVideoUploadAsync(request);

            _logger.LogInformation(
                "Video upload processed successfully. VideoId: {VideoId}, ProcessingJobId: {ProcessingJobId}",
                result.VideoId,
                result.ProcessingJobId
            );

            return CreatedAtAction(
                nameof(GetAggregatedVideoDetails),
                new { videoId = result.VideoId },
                result
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing video upload");
            return StatusCode(500, new { message = "An error occurred while processing the video upload" });
        }
    }

    /// <summary>
    /// Health check endpoint for this controller.
    /// </summary>
    [HttpGet("health")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult HealthCheck()
    {
        return Ok(new
        {
            status = "Healthy",
            service = "AggregatedVideoController",
            timestamp = DateTime.UtcNow
        });
    }
}
