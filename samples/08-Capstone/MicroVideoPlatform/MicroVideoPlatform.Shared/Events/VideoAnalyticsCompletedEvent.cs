using MicroVideoPlatform.Shared.DTOs;

namespace MicroVideoPlatform.Shared.Events;

/// <summary>
/// Event raised when video analytics/classification completes
/// </summary>
public sealed record VideoAnalyticsCompletedEvent : DomainEventBase
{
    /// <summary>
    /// ID of the analyzed video
    /// </summary>
    public required Guid VideoId { get; init; }

    /// <summary>
    /// Analytics results
    /// </summary>
    public required VideoAnalyticsResultDto AnalyticsResult { get; init; }

    /// <summary>
    /// Indicates if the analysis was successful
    /// </summary>
    public required bool IsSuccessful { get; init; }

    /// <summary>
    /// Error message if analysis failed
    /// </summary>
    public string? ErrorMessage { get; init; }

    /// <summary>
    /// Time taken for analysis in milliseconds
    /// </summary>
    public required long AnalysisTimeMs { get; init; }

    /// <summary>
    /// Service that performed the analysis
    /// </summary>
    public required string AnalyzedBy { get; init; }
}
