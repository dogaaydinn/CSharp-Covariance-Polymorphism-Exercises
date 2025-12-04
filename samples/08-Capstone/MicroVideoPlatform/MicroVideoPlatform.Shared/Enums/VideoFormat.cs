namespace MicroVideoPlatform.Shared.Enums;

/// <summary>
/// Represents supported video formats
/// </summary>
public enum VideoFormat
{
    /// <summary>
    /// MPEG-4 Part 14 (most common)
    /// </summary>
    MP4,

    /// <summary>
    /// Audio Video Interleave
    /// </summary>
    AVI,

    /// <summary>
    /// Matroska Video
    /// </summary>
    MKV,

    /// <summary>
    /// QuickTime File Format
    /// </summary>
    MOV,

    /// <summary>
    /// WebM (VP8/VP9)
    /// </summary>
    WEBM,

    /// <summary>
    /// Flash Video (legacy)
    /// </summary>
    FLV,

    /// <summary>
    /// Unknown or unsupported format
    /// </summary>
    Unknown
}
