using DigiFikileLms.Domain.Common;

namespace DigiFikileLms.Domain.Entities;

public class Enrollment : BaseEntity
{
    private Enrollment() { }

    public static Enrollment Create(Student student, Course course)
    {
        return new Enrollment
        {
            StudentId = student.Id,
            Student = student,
            CourseId = course.Id,
            Course = course,
            EnrolledAt = DateTime.UtcNow
        };
    }

    public int StudentId { get; private set; }
    public int CourseId { get; private set; }
    public DateTime EnrolledAt { get; private set; }

    public virtual Student Student { get; private set; } = null!;
    public virtual Course Course { get; private set; } = null!;
}