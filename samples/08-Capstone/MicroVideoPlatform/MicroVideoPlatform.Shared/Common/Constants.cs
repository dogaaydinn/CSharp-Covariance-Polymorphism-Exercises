namespace MicroVideoPlatform.Shared.Common;

/// <summary>
/// Shared constants across all services
/// </summary>
public static class Constants
{
    /// <summary>
    /// RabbitMQ event routing keys
    /// </summary>
    public static class Events
    {
        public const string VideoUploaded = "video.uploaded";
        public const string VideoProcessingCompleted = "video.processing.completed";
        public const string VideoProcessingFailed = "video.processing.failed";
        public const string VideoAnalyticsCompleted = "video.analytics.completed";
    }

    /// <summary>
    /// RabbitMQ queue names
    /// </summary>
    public static class Queues
    {
        public const string VideoUploaded = "video.uploaded";
        public const string VideoProcessingCompleted = "video.processing.completed";
        public const string VideoProcessingFailed = "video.processing.failed";
        public const string VideoAnalyticsRequested = "video.analytics.requested";
        public const string DeadLetterQueue = "video.dlq";
    }

    /// <summary>
    /// RabbitMQ exchange names
    /// </summary>
    public static class Exchanges
    {
        public const string VideoEvents = "video.events";
        public const string DeadLetterExchange = "video.dlx";
    }

    /// <summary>
    /// Cache key prefixes for Redis
    /// </summary>
    public static class CacheKeys
    {
        public const string VideoPrefix = "video:";
        public const string VideosAllPrefix = "videos:all:";
        public const string VideosByCategoryPrefix = "videos:category:";
        public const string VideosByTagPrefix = "videos:tag:";
        public const string VideoCountPrefix = "videos:count:";
        public const string AnalyticsPrefix = "analytics:";
        public const string ProcessingStatusPrefix = "processing:";

        public static string Video(Guid id) => $"{VideoPrefix}{id}";
        public static string VideosByCategory(string category) => $"{VideosByCategoryPrefix}{category}";
        public static string VideosByTag(string tag) => $"{VideosByTagPrefix}{tag}";
        public static string Analytics(Guid videoId) => $"{AnalyticsPrefix}{videoId}";
        public static string ProcessingStatus(Guid videoId) => $"{ProcessingStatusPrefix}{videoId}";
    }

    /// <summary>
    /// Cache expiration times (in seconds)
    /// </summary>
    public static class CacheExpiration
    {
        public const int Video = 300;           // 5 minutes
        public const int VideosList = 60;       // 1 minute
        public const int Analytics = 600;       // 10 minutes
        public const int ProcessingStatus = 30; // 30 seconds
        public const int Statistics = 120;      // 2 minutes
    }

    /// <summary>
    /// File size limits
    /// </summary>
    public static class FileLimits
    {
        public const long MaxVideoSizeBytes = 2L * 1024 * 1024 * 1024; // 2 GB
        public const long MinVideoSizeBytes = 1024; // 1 KB
    }

    /// <summary>
    /// Supported video formats
    /// </summary>
    public static class SupportedFormats
    {
        public static readonly string[] VideoExtensions = { ".mp4", ".avi", ".mkv", ".mov", ".webm", ".flv" };
        public static readonly string[] VideoMimeTypes =
        {
            "video/mp4",
            "video/x-msvideo",
            "video/x-matroska",
            "video/quicktime",
            "video/webm",
            "video/x-flv"
        };
    }

    /// <summary>
    /// Video categories
    /// </summary>
    public static class Categories
    {
        public const string Education = "Education";
        public const string Entertainment = "Entertainment";
        public const string Gaming = "Gaming";
        public const string Technology = "Technology";
        public const string Music = "Music";
        public const string Sports = "Sports";
        public const string News = "News";
        public const string HowTo = "HowTo";
        public const string Vlogs = "Vlogs";
        public const string Comedy = "Comedy";
        public const string Animation = "Animation";
        public const string Documentary = "Documentary";
        public const string Other = "Other";

        public static readonly string[] All =
        {
            Education, Entertainment, Gaming, Technology, Music,
            Sports, News, HowTo, Vlogs, Comedy, Animation, Documentary, Other
        };
    }

    /// <summary>
    /// API endpoint paths
    /// </summary>
    public static class ApiEndpoints
    {
        public const string Videos = "/api/videos";
        public const string VideoById = "/api/videos/{id}";
        public const string VideoUpload = "/api/videos/upload";
        public const string VideoSearch = "/api/videos/search";
        public const string Analytics = "/api/analytics";
        public const string AnalyticsByVideoId = "/api/analytics/{videoId}";
        public const string Health = "/health";
    }

    /// <summary>
    /// Processing configuration
    /// </summary>
    public static class Processing
    {
        public const int DefaultThumbnailOffsetSeconds = 5;
        public const int MaxRetryAttempts = 3;
        public const int RetryDelaySeconds = 5;
        public const int ProcessingTimeoutMinutes = 30;
    }

    /// <summary>
    /// JWT claims
    /// </summary>
    public static class Claims
    {
        public const string UserId = "userId";
        public const string Email = "email";
        public const string Role = "role";
    }

    /// <summary>
    /// User roles
    /// </summary>
    public static class Roles
    {
        public const string Admin = "Admin";
        public const string User = "User";
        public const string Guest = "Guest";
    }
}
