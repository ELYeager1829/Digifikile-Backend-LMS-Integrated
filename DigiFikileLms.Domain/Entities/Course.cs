using DigiFikileLms.Domain.Common;

namespace DigiFikileLms.Domain.Entities;

public class Course : BaseEntity
{
    private readonly List<Module> _modules = new();
    private readonly List<Enrollment> _enrollments = new();
    private readonly List<Progress> _progressRecords = new();

    private Course() { }

    public static Course Create(
        string courseName,
        string? saqaId = null,
        string? curriculumCode = null,
        int? nqfLevel = null,
        Facilitator? facilitator = null,
        Student? student = null)
    {
        return new Course
        {
            CourseName = courseName,
            Code = GenerateCourseCode(courseName),
            SaqaId = saqaId,
            CurriculumCode = curriculumCode,
            NqfLevel = nqfLevel,
            Facilitator = facilitator,
            FacilitatorId = facilitator?.Id,
            Student = student,
            StudentId = student?.Id,
            Status = "Draft"
        };
    }

    private static string GenerateCourseCode(string courseName)
    {
        var prefix = string.Concat(courseName.Split(' ')
            .Where(w => w.Length > 0)
            .Select(w => char.ToUpper(w[0])))
            .Take(4);
        return $"{new string(prefix.ToArray())}-{DateTime.UtcNow:yyyy}";
    }

    public string CourseName { get; private set; } = string.Empty;
    public string Code { get; private set; } = string.Empty;
    public string? SaqaId { get; private set; }
    public string? CurriculumCode { get; private set; }
    public int? NqfLevel { get; private set; }
    public string Status { get; private set; } = "Draft";
    public bool IsArchived { get; private set; }

    public int? FacilitatorId { get; private set; }
    public int? StudentId { get; private set; }

    public virtual Facilitator? Facilitator { get; private set; }
    public virtual Student? Student { get; private set; }

    public IReadOnlyCollection<Module> Modules => _modules.AsReadOnly();
    public IReadOnlyCollection<Enrollment> Enrollments => _enrollments.AsReadOnly();
    public IReadOnlyCollection<Progress> ProgressRecords => _progressRecords.AsReadOnly();

    public void AddModule(Module module)
    {
        _modules.Add(module);
        UpdateTimestamp();
    }

    public void AddEnrollment(Enrollment enrollment)
    {
        _enrollments.Add(enrollment);
        UpdateTimestamp();
    }

    public void Publish()
    {
        if (Status == "Published")
            throw new InvalidOperationException("Course is already published");

        if (!Modules.Any())
            throw new InvalidOperationException("Cannot publish course without modules");

        Status = "Published";
        UpdateTimestamp();
    }

    public void Unpublish()
    {
        if (Status != "Published")
            throw new InvalidOperationException("Only published courses can be unpublished");

        Status = "Draft";
        UpdateTimestamp();
    }

    public void Archive()
    {
        if (IsArchived)
            throw new InvalidOperationException("Course is already archived");

        IsArchived = true;
        UpdateTimestamp();
    }

    public void UpdateDetails(string courseName, string? curriculumCode, int? nqfLevel, string? saqaId = null)
    {
        CourseName = courseName;
        CurriculumCode = curriculumCode;
        NqfLevel = nqfLevel;
        SaqaId = saqaId ?? SaqaId;
        UpdateTimestamp();
    }
}

