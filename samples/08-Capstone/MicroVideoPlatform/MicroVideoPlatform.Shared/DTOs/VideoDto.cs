using MicroVideoPlatform.Shared.Enums;

namespace MicroVideoPlatform.Shared.DTOs;

/// <summary>
/// Data transfer object for video information
/// </summary>
public sealed record VideoDto
{
    /// <summary>
    /// Unique identifier for the video
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// Video title
    /// </summary>
    public required string Title { get; init; }

    /// <summary>
    /// Video description
    /// </summary>
    public string? Description { get; init; }

    /// <summary>
    /// Original file name
    /// </summary>
    public required string FileName { get; init; }

    /// <summary>
    /// File size in bytes
    /// </summary>
    public long FileSizeBytes { get; init; }

    /// <summary>
    /// Video duration in seconds
    /// </summary>
    public int? DurationSeconds { get; init; }

    /// <summary>
    /// Video resolution (e.g., "1920x1080")
    /// </summary>
    public string? Resolution { get; init; }

    /// <summary>
    /// Video format (e.g., "mp4", "avi")
    /// </summary>
    public string? Format { get; init; }

    /// <summary>
    /// Current processing status
    /// </summary>
    public VideoStatus Status { get; init; }

    /// <summary>
    /// URL to video thumbnail
    /// </summary>
    public string? ThumbnailUrl { get; init; }

    /// <summary>
    /// Video category (determined by ML analysis)
    /// </summary>
    public string? Category { get; init; }

    /// <summary>
    /// Video tags for search and filtering
    /// </summary>
    public string[]? Tags { get; init; }

    /// <summary>
    /// Number of times video has been viewed
    /// </summary>
    public int ViewsCount { get; init; }

    /// <summary>
    /// Number of likes
    /// </summary>
    public int LikesCount { get; init; }

    /// <summary>
    /// Email of user who uploaded the video
    /// </summary>
    public string? UploadedBy { get; init; }

    /// <summary>
    /// Timestamp when video was uploaded
    /// </summary>
    public DateTime UploadedAt { get; init; }

    /// <summary>
    /// Timestamp when video processing completed
    /// </summary>
    public DateTime? ProcessedAt { get; init; }

    /// <summary>
    /// Timestamp of last modification
    /// </summary>
    public DateTime LastModifiedAt { get; init; }

    /// <summary>
    /// Additional metadata as key-value pairs
    /// </summary>
    public Dictionary<string, string>? Metadata { get; init; }

    /// <summary>
    /// Human-readable file size (e.g., "52.4 MB")
    /// </summary>
    public string FileSizeFormatted => FormatFileSize(FileSizeBytes);

    /// <summary>
    /// Human-readable duration (e.g., "12:34")
    /// </summary>
    public string? DurationFormatted => DurationSeconds.HasValue
        ? TimeSpan.FromSeconds(DurationSeconds.Value).ToString(@"mm\:ss")
        : null;

    private static string FormatFileSize(long bytes)
    {
        string[] sizes = { "B", "KB", "MB", "GB", "TB" };
        double len = bytes;
        int order = 0;
        while (len >= 1024 && order < sizes.Length - 1)
        {
            order++;
            len /= 1024;
        }
        return $"{len:0.##} {sizes[order]}";
    }
}
