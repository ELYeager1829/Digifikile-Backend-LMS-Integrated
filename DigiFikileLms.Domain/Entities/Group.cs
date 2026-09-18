using DigiFikileLms.Domain.Common;


namespace DigiFikileLms.Domain.Entities;

public class Group : BaseEntity
{
    private readonly List<GroupEnrollment> _groupEnrollments = new();

    private Group() { }

    public static Group Create(
        int setaAdministratorId,
        int? setaProgrammeId,
        string groupName,
        string? description = null)
    {
        return new Group
        {
            setaAdministratorId = setaAdministratorId,
            setaProgrammeId = setaProgrammeId,
            GroupName = groupName,
            Description = description,
            CreatedAt = DateTime.UtcNow
        };
    }

    public int setaAdministratorId { get; private set; }
    public int? setaProgrammeId { get; private set; }
    public string GroupName { get; private set; } = string.Empty;
    public string? Description { get; private set; }

    // Navigation Properties
    public virtual SetaAdministrator setaAdministrator { get; private set; } = null!;
    public virtual SETAProgramme? setaProgramme { get; private set; }
    public virtual ICollection<GroupEnrollment> GroupEnrollments => _groupEnrollments.AsReadOnly();

    // Domain Behaviors
    public void UpdateDetails(string groupName, string? description)
    {
        GroupName = groupName;
        Description = description;
        UpdateTimestamp();
    }

    public void AddEnrollment(GroupEnrollment enrollment)
    {
        _groupEnrollments.Add(enrollment);
        UpdateTimestamp();
    }

    public void RemoveEnrollment(GroupEnrollment enrollment)
    {
        _groupEnrollments.Remove(enrollment);
        UpdateTimestamp();
    }
}