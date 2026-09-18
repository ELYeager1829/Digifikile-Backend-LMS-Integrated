namespace DigiFikileLms.Domain.Enums;

/// <summary>
/// Enumeration of the states a complaint moves through after it is laid.
/// </summary>
public enum ComplaintStatus
{
    /// <summary>Laid and waiting for a System Administrator to pick it up.</summary>
    Open = 1,

    /// <summary>A System Administrator is reviewing the complaint.</summary>
    InReview = 2,

    /// <summary>The complaint was resolved; see ResolutionNotes.</summary>
    Resolved = 3,

    /// <summary>The complaint was dismissed without action; see ResolutionNotes.</summary>
    Dismissed = 4
}