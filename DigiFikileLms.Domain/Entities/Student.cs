using DigiFikileLms.Domain.Common;

namespace DigiFikileLms.Domain.Entities;

public class Student : BaseEntity
{
    private readonly List<Enrollment> _enrollments = new();
    private readonly List<Assessment> _assessments = new();
    private readonly List<Submission> _submissions = new();
    private readonly List<Result> _results = new();
    private readonly List<Progress> _progressRecords = new();

    private Student() { }

    public static Student Create(UserAccount user, string? studentNumber = null)
    {
        return new Student
        {
            UserId = user.Id,
            User = user,
            StudentNumber = studentNumber ?? GenerateStudentNumber(),
            EnrolledAt = DateTime.UtcNow
        };
    }

    private static string GenerateStudentNumber()
    {
        return $"STU-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString().Substring(0, 6).ToUpper()}";
    }

    public int UserId { get; private set; }
    public string StudentNumber { get; private set; } = string.Empty;
    public DateTime EnrolledAt { get; private set; }

    public virtual UserAccount User { get; private set; } = null!;
    public IReadOnlyCollection<Enrollment> Enrollments => _enrollments.AsReadOnly();
    public IReadOnlyCollection<Assessment> Assessments => _assessments.AsReadOnly();
    public IReadOnlyCollection<Submission> Submissions => _submissions.AsReadOnly();
    public IReadOnlyCollection<Result> Results => _results.AsReadOnly();
    public IReadOnlyCollection<Progress> ProgressRecords => _progressRecords.AsReadOnly();

    public void AddEnrollment(Enrollment enrollment)
    {
        _enrollments.Add(enrollment);
        UpdateTimestamp();
    }

    public void AddProgress(Progress progress)
    {
        _progressRecords.Add(progress);
        UpdateTimestamp();
    }
}