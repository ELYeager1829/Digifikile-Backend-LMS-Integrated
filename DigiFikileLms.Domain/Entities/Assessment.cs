using DigiFikileLms.Domain.Common;
using DigiFikileLms.Domain.Enums;

namespace DigiFikileLms.Domain.Entities;

public class Assessment : BaseEntity
{
    private readonly List<Submission> _submissions = new();
    private readonly List<Feedback> _feedbacks = new();

    private Assessment() { }

    public static Assessment Create(
        Module module,
        Student student,
        string title,
        AssessmentType assessmentType)
    {
        return new Assessment
        {
            ModuleId = module.Id,
            Module = module,
            StudentId = student.Id,
            Student = student,
            Title = title,
            AssessmentType = assessmentType
        };
    }

    public int ModuleId { get; private set; }
    public int StudentId { get; private set; }
    public AssessmentType AssessmentType { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public decimal? Grades { get; private set; }

    public virtual Module Module { get; private set; } = null!;
    public virtual Student Student { get; private set; } = null!;
    public IReadOnlyCollection<Submission> Submissions => _submissions.AsReadOnly();
    public IReadOnlyCollection<Feedback> Feedbacks => _feedbacks.AsReadOnly();

    public void AddSubmission(Submission submission)
    {
        _submissions.Add(submission);
        UpdateTimestamp();
    }

    public void UpdateGrade(decimal grade)
    {
        Grades = grade;
        UpdateTimestamp();
    }
}