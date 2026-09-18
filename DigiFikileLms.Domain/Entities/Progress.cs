using DigiFikileLms.Domain.Common;
using DigiFikileLms.Domain.Enums;

namespace DigiFikileLms.Domain.Entities;

public class Progress : BaseEntity
{
    private Progress() { }

    public static Progress Create(
        Course course,
        Student student,
        ProgressStatus status = ProgressStatus.NotStarted,
        decimal? percentage = null)
    {
        return new Progress
        {
            CourseId = course.Id,
            Course = course,
            StudentId = student.Id,
            Student = student,
            Status = status,
            Percentage = percentage ?? 0
        };
    }

    public int CourseId { get; private set; }
    public int StudentId { get; private set; }
    public ProgressStatus Status { get; private set; }
    public decimal Percentage { get; private set; }

    public virtual Course Course { get; private set; } = null!;
    public virtual Student Student { get; private set; } = null!;

    public void UpdateProgress(decimal percentage, ProgressStatus status)
    {
        Percentage = Math.Clamp(percentage, 0, 100);
        Status = status;
        UpdateTimestamp();
    }

    public void MarkCompleted()
    {
        Percentage = 100;
        Status = ProgressStatus.Completed;
        UpdateTimestamp();
    }
}