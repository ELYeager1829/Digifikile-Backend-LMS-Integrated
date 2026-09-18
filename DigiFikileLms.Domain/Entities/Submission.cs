using DigiFikileLms.Domain.Common;

namespace DigiFikileLms.Domain.Entities;

public class Submission : BaseEntity
{
    private Submission() { }

    public static Submission Create(
        Assessment assessment,
        Student student,
        int attempts = 1)
    {
        return new Submission
        {
            AssessmentId = assessment.Id,
            Assessment = assessment,
            StudentId = student.Id,
            Student = student,
            SubmissionDate = DateTime.UtcNow,
            Attempts = attempts
        };
    }

    public int AssessmentId { get; private set; }
    public DateTime SubmissionDate { get; private set; }
    public int Attempts { get; private set; }
    public int StudentId { get; private set; }

    public virtual Assessment Assessment { get; private set; } = null!;
    public virtual Student Student { get; private set; } = null!;

    public void IncrementAttempts()
    {
        Attempts++;
        UpdateTimestamp();
    }
}