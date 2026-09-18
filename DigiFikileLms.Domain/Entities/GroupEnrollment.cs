using DigiFikileLms.Domain.Common;


namespace DigiFikileLms.Domain.Entities;

public class GroupEnrollment : BaseEntity
{
    private GroupEnrollment() { }

    public static GroupEnrollment Create(
        Group group,
        Student student)
    {
        return new GroupEnrollment
        {
            GroupId = group.Id,
            Group = group,
            StudentId = student.Id,
            Student = student,
            AssignedAt = DateTime.UtcNow
        };
    }

    public int GroupId { get; private set; }
    public int StudentId { get; private set; }
    public DateTime AssignedAt { get; private set; }

    // Navigation Properties
    public virtual Group Group { get; private set; } = null!;
    public virtual Student Student { get; private set; } = null!;
}