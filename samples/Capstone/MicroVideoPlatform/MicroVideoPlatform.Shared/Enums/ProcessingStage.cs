namespace MicroVideoPlatform.Shared.Enums;

/// <summary>
/// Represents the stage of video processing
/// </summary>
public enum ProcessingStage
{
    /// <summary>
    /// Validating uploaded video file
    /// </summary>
    Validation,

    /// <summary>
    /// Extracting video metadata (duration, resolution, etc.)
    /// </summary>
    MetadataExtraction,

    /// <summary>
    /// Transcoding video to target formats
    /// </summary>
    Transcoding,

    /// <summary>
    /// Generating video thumbnails
    /// </summary>
    ThumbnailGeneration,

    /// <summary>
    /// Running ML analysis for content classification
    /// </summary>
    ContentAnalysis,

    /// <summary>
    /// Finalizing and persisting results
    /// </summary>
    Finalization
}
