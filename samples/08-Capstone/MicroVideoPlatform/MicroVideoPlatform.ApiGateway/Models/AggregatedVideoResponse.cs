using MicroVideoPlatform.Shared.DTOs;

namespace MicroVideoPlatform.ApiGateway.Models;

/// <summary>
/// Aggregated response combining data from multiple microservices.
/// </summary>
public class AggregatedVideoResponse
{
    public VideoDto? Video { get; set; }
    public List<VideoRecommendation>? Recommendations { get; set; }
    public ProcessingStatusDto? ProcessingStatus { get; set; }
    public List<VideoDto>? SimilarVideos { get; set; }
    public List<CommentDto>? PopularComments { get; set; }
    public VideoMetadataDto? Metadata { get; set; }
}

public class VideoRecommendation
{
    public string VideoId { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public float Score { get; set; }
    public string ReasonCode { get; set; } = string.Empty;
}

public class ProcessingStatusDto
{
    public string VideoId { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public int Progress { get; set; }
    public string? ErrorMessage { get; set; }
    public DateTime? CompletedAt { get; set; }
}

public class CommentDto
{
    public Guid Id { get; set; }
    public string VideoId { get; set; } = string.Empty;
    public string AuthorId { get; set; } = string.Empty;
    public string AuthorName { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
    public int LikesCount { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class VideoMetadataDto
{
    public BasicVideoInfoDto? BasicInfo { get; set; }
    public EngagementStatsDto? EngagementStats { get; set; }
    public ProcessingInfoDto? ProcessingInfo { get; set; }
}

public class BasicVideoInfoDto
{
    public string VideoId { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public long FileSizeBytes { get; set; }
    public int DurationSeconds { get; set; }
    public string Resolution { get; set; } = string.Empty;
    public string Format { get; set; } = string.Empty;
}

public class EngagementStatsDto
{
    public string VideoId { get; set; } = string.Empty;
    public int TotalViews { get; set; }
    public int UniqueViewers { get; set; }
    public double AverageWatchTimeSeconds { get; set; }
    public double EngagementRate { get; set; }
    public int ShareCount { get; set; }
    public Dictionary<string, int> ViewsByRegion { get; set; } = new();
}

public class ProcessingInfoDto
{
    public string VideoId { get; set; } = string.Empty;
    public List<string> AvailableQualities { get; set; } = new();
    public bool ThumbnailGenerated { get; set; }
    public bool SubtitlesGenerated { get; set; }
    public DateTime ProcessedAt { get; set; }
}

/// <summary>
/// Request for video upload aggregation.
/// </summary>
public class VideoUploadRequest
{
    public VideoMetadataInput Metadata { get; set; } = new();
    public string FilePath { get; set; } = string.Empty;
}

public class VideoMetadataInput
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Tags { get; set; } = string.Empty;
}

/// <summary>
/// Response for video upload aggregation.
/// </summary>
public class VideoUploadResponse
{
    public string VideoId { get; set; } = string.Empty;
    public bool ContentStatus { get; set; }
    public string? ProcessingJobId { get; set; }
    public string? RecommendationTaskId { get; set; }
    public DateTime EstimatedCompletionTime { get; set; }
}

/// <summary>
/// User claims extracted from JWT token.
/// </summary>
public class UserClaims
{
    public string UserId { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public List<string> Roles { get; set; } = new();
    public string Tier { get; set; } = "Standard"; // Standard, Premium, Admin
}
