using DigiFikileLms.Domain.Enums;

namespace DigiFikileLms.Application.DTOs.Complaint;

/// <summary>
/// Request body for laying a complaint. The complainant is the authenticated caller; no user id
/// is accepted from the request.
/// </summary>
public class LayComplaintRequestDto
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    /// <summary>Optional free-text grouping, for example "Fees", "Harassment" or "Content".</summary>
    public string? Category { get; set; }
}

/// <summary>
/// Request body for a System Administrator moving a complaint to a new state. The status binds
/// directly to the ComplaintStatus enum, so an unknown name is rejected by model binding with a
/// 400 before the handler runs.
/// </summary>
public class UpdateComplaintStatusRequestDto
{
    public ComplaintStatus Status { get; set; }
    public string? ResolutionNotes { get; set; }
}

/// <summary>
/// Transport-safe view of a complaint. Entity fields are copied, never serialised from the
/// tracked domain object.
/// </summary>
public class ComplaintDto
{
    public int Id { get; set; }
    public int ComplainantUserId { get; set; }
    public string ComplainantName { get; set; } = string.Empty;
    public string ComplainantEmail { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? Category { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? ResolutionNotes { get; set; }
    public DateTime? ResolvedAt { get; set; }
    public DateTime CreatedAt { get; set; }
}