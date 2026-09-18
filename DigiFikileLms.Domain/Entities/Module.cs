using DigiFikileLms.Domain.Common;

namespace DigiFikileLms.Domain.Entities;

public class Module : BaseEntity
{
    private readonly List<Assessment> _assessments = new();

    private Module() { }

    public static Module Create(
        Course course,
        string moduleName,
        string? saqaId = null,
        int? credits = null,
        string? description = null)
    {
        return new Module
        {
            CourseId = course.Id,
            Course = course,
            ModuleName = moduleName,
            SaqaId = saqaId,
            Credits = credits,
            Description = description
        };
    }

    public int CourseId { get; private set; }
    public string ModuleName { get; private set; } = string.Empty;
    public string? SaqaId { get; private set; }
    public int? Credits { get; private set; }
    public string? Description { get; private set; }

    public virtual Course Course { get; private set; } = null!;
    public IReadOnlyCollection<Assessment> Assessments => _assessments.AsReadOnly();

    public void AddAssessment(Assessment assessment)
    {
        _assessments.Add(assessment);
        UpdateTimestamp();
    }
}