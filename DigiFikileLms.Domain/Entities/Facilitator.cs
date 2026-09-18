using DigiFikileLms.Domain.Common;

namespace DigiFikileLms.Domain.Entities;

public class Facilitator : BaseEntity
{
    private readonly List<Course> _courses = new();

    private Facilitator() { }

    public static Facilitator Create(UserAccount user, string? staffNumber = null)
    {
        return new Facilitator
        {
            UserId = user.Id,
            User = user,
            StaffNumber = staffNumber ?? GenerateStaffNumber()
        };
    }

    private static string GenerateStaffNumber()
    {
        return $"FAC-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString().Substring(0, 6).ToUpper()}";
    }

    public int UserId { get; private set; }
    public string StaffNumber { get; private set; } = string.Empty;

    public virtual UserAccount User { get; private set; } = null!;
    public IReadOnlyCollection<Course> Courses => _courses.AsReadOnly();

    public void AddCourse(Course course)
    {
        _courses.Add(course);
        UpdateTimestamp();
    }
}