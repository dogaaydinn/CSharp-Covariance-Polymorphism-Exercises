namespace VideoService.API.Models;

public class Video
{
    public int Id { get; set; }
    public required string Title { get; set; }
    public required string Description { get; set; }
    public required string Url { get; set; }
    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
    public VideoStatus Status { get; set; } = VideoStatus.Pending;
    public int ViewCount { get; set; }
}

public enum VideoStatus
{
    Pending,
    Processing,
    Ready,
    Failed
}
