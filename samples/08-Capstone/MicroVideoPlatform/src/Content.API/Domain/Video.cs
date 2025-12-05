namespace Content.API.Domain;

public class Video
{
    public Guid Id { get; private set; }
    public string Title { get; private set; }
    public string Url { get; private set; }
    public VideoStatus Status { get; private set; }
    public DateTime UploadedAt { get; private set; }
    public int ViewCount { get; private set; }

    private Video() { }  // For EF Core

    public Video(string title, string url)
    {
        Id = Guid.NewGuid();
        Title = title;
        Url = url;
        Status = VideoStatus.Uploaded;
        UploadedAt = DateTime.UtcNow;
        ViewCount = 0;
    }

    public void MarkAsProcessed(string processedUrl)
    {
        Url = processedUrl;
        Status = VideoStatus.Processed;
    }

    public void IncrementViewCount()
    {
        ViewCount++;
    }
}

public enum VideoStatus
{
    Uploaded,
    Processing,
    Processed,
    Failed
}
