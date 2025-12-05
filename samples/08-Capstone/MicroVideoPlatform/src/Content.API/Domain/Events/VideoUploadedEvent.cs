namespace Content.API.Domain.Events;

public record VideoUploadedEvent(Guid VideoId, string Title, string Url, DateTime UploadedAt);

public record VideoProcessedEvent(Guid VideoId, string ProcessedUrl);

public record VideoViewedEvent(Guid VideoId, DateTime ViewedAt);
