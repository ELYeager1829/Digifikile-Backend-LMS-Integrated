using DigiFikileLms.Domain.Common;

namespace DigiFikileLms.Domain.Entities;

public class Report : BaseEntity
{
    private Report() { }

    public static Report Create(
        SetaAdministrator setaAdministrator,
        TrainingProvider trainingProvider,
        string title,
        string? description = null)
    {
        return new Report
        {
            SetaAdministratorId = setaAdministrator.Id,
            SetaAdministrator = setaAdministrator,
            TrainingProviderId = trainingProvider.Id,
            TrainingProvider = trainingProvider,
            Title = title,
            Description = description
        };
    }

    public int SetaAdministratorId { get; private set; }
    public int TrainingProviderId { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public string? Description { get; private set; }

    public virtual SetaAdministrator SetaAdministrator { get; private set; } = null!;
    public virtual TrainingProvider TrainingProvider { get; private set; } = null!;
}