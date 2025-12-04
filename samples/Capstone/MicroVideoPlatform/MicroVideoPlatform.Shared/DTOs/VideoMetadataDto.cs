namespace MicroVideoPlatform.Shared.DTOs;

/// <summary>
/// Data transfer object for video metadata extraction results
/// </summary>
public sealed record VideoMetadataDto
{
    /// <summary>
    /// Video duration in seconds
    /// </summary>
    public required int DurationSeconds { get; init; }

    /// <summary>
    /// Video resolution (e.g., "1920x1080")
    /// </summary>
    public required string Resolution { get; init; }

    /// <summary>
    /// Video width in pixels
    /// </summary>
    public required int Width { get; init; }

    /// <summary>
    /// Video height in pixels
    /// </summary>
    public required int Height { get; init; }

    /// <summary>
    /// Video format/codec
    /// </summary>
    public required string Format { get; init; }

    /// <summary>
    /// Video bitrate in kbps
    /// </summary>
    public int? BitrateKbps { get; init; }

    /// <summary>
    /// Frames per second
    /// </summary>
    public double? FrameRate { get; init; }

    /// <summary>
    /// Audio codec
    /// </summary>
    public string? AudioCodec { get; init; }

    /// <summary>
    /// Audio bitrate in kbps
    /// </summary>
    public int? AudioBitrateKbps { get; init; }

    /// <summary>
    /// Audio sample rate in Hz
    /// </summary>
    public int? AudioSampleRate { get; init; }

    /// <summary>
    /// Number of audio channels
    /// </summary>
    public int? AudioChannels { get; init; }

    /// <summary>
    /// Additional technical metadata
    /// </summary>
    public Dictionary<string, string>? AdditionalMetadata { get; init; }

    /// <summary>
    /// Human-readable resolution description
    /// </summary>
    public string ResolutionQuality => Height switch
    {
        >= 2160 => "4K Ultra HD",
        >= 1440 => "2K QHD",
        >= 1080 => "Full HD",
        >= 720 => "HD",
        >= 480 => "SD",
        _ => "Low Resolution"
    };
}
