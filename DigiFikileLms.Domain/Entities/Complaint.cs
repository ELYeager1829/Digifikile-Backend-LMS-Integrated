using DigiFikileLms.Domain.Common;
using DigiFikileLms.Domain.Enums;

namespace DigiFikileLms.Domain.Entities;

/// <summary>
/// A complaint laid by any account in the system. System Administrators see and work through
/// every complaint; the complainant sees only their own.
/// </summary>
public class Complaint : BaseEntity
{
    private Complaint() { }

    /// <summary>
    /// Lays a new complaint. Every complaint starts in the Open state; only a System
    /// Administrator moves it forward through UpdateStatus.
    /// </summary>
    public static Complaint Create(
        int complainantUserId,
        string title,
        string description,
        string? category = null)
    {
        return new Complaint
        {
            ComplainantUserId = complainantUserId,
            Title = title.Trim(),
            Description = description.Trim(),
            Category = string.IsNullOrWhiteSpace(category) ? null : category.Trim(),
            Status = ComplaintStatus.Open
        };
    }

    public int ComplainantUserId { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public string? Category { get; private set; }
    public ComplaintStatus Status { get; private set; }
    public string? ResolutionNotes { get; private set; }
    public DateTime? ResolvedAt { get; private set; }

    public virtual UserAccount Complainant { get; private set; } = null!;

    /// <summary>
    /// Moves the complaint to a new state. Terminal states (Resolved, Dismissed) stamp the time
    /// they were closed; moving a closed complaint back reopens that window.
    /// </summary>
    public void UpdateStatus(ComplaintStatus status, string? resolutionNotes = null)
    {
        Status = status;
        ResolutionNotes = string.IsNullOrWhiteSpace(resolutionNotes) ? null : resolutionNotes.Trim();
        ResolvedAt = status is ComplaintStatus.Resolved or ComplaintStatus.Dismissed
            ? DateTime.UtcNow
            : null;
        UpdateTimestamp();
    }
}