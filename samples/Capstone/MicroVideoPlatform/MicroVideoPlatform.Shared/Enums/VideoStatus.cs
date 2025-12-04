namespace MicroVideoPlatform.Shared.Enums;

/// <summary>
/// Represents the processing status of a video
/// </summary>
public enum VideoStatus
{
    /// <summary>
    /// Video has been uploaded and is waiting to be processed
    /// </summary>
    Pending = 0,

    /// <summary>
    /// Video is currently being processed (transcoding, analysis, etc.)
    /// </summary>
    Processing = 1,

    /// <summary>
    /// Video has been successfully processed and is ready for viewing
    /// </summary>
    Completed = 2,

    /// <summary>
    /// Video processing failed
    /// </summary>
    Failed = 3
}
