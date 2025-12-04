using MicroVideoPlatform.Shared.Enums;

namespace MicroVideoPlatform.Content.API.Domain.Entities;

/// <summary>
/// Video entity representing a video in the system
/// </summary>
public sealed class Video
{
    public Guid Id { get; set; }
    public required string Title { get; set; }
    public string? Description { get; set; }
    public required string FileName { get; set; }
    public long FileSizeBytes { get; set; }
    public int? DurationSeconds { get; set; }
    public string? Resolution { get; set; }
    public string? Format { get; set; }
    public VideoStatus Status { get; set; } = VideoStatus.Pending;
    public string? ThumbnailUrl { get; set; }
    public string? Category { get; set; }
    public string[]? Tags { get; set; }
    public int ViewsCount { get; set; }
    public int LikesCount { get; set; }
    public string? UploadedBy { get; set; }
    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ProcessedAt { get; set; }
    public DateTime LastModifiedAt { get; set; } = DateTime.UtcNow;
    public Dictionary<string, string>? Metadata { get; set; }
}
