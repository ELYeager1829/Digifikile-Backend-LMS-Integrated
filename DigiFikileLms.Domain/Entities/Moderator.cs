using DigiFikileLms.Domain.Common;

namespace DigiFikileLms.Domain.Entities;

/// <summary>
/// Replaces the legacy "Assessor" concept. A Moderator reviews assessments,
/// records results and provides Feedback on student submissions.
/// </summary>
public class Moderator : BaseEntity
{
    private readonly List<Feedback> _feedbacks = new();

    private Moderator() { }

    public static Moderator Create(UserAccount user, string? staffNumber = null)
    {
        return new Moderator
        {
            UserId = user.Id,
            User = user,
            StaffNumber = staffNumber ?? GenerateStaffNumber()
        };
    }

    private static string GenerateStaffNumber()
    {
        return $"MOD-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString().Substring(0, 6).ToUpper()}";
    }

    public int UserId { get; private set; }
    public string StaffNumber { get; private set; } = string.Empty;

    public virtual UserAccount User { get; private set; } = null!;
    public IReadOnlyCollection<Feedback> Feedbacks => _feedbacks.AsReadOnly();

    public void AddFeedback(Feedback feedback)
    {
        _feedbacks.Add(feedback);
        UpdateTimestamp();
    }
}
