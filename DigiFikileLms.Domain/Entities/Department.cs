using DigiFikileLms.Domain.Common;

namespace DigiFikileLms.Domain.Entities;

public class Department : BaseEntity
{
    private Department() { }

    public static Department Create(
        TrainingProvider trainingProvider,
        string name,
        string? sdlNumber = null,
        string? contactEmail = null,
        string? description = null)
    {
        return new Department
        {
            TrainingProviderId = trainingProvider.Id,
            TrainingProvider = trainingProvider,
            Name = name,
            SDLNumber = sdlNumber,
            ContactEmail = contactEmail,
            Description = description
        };
    }

    public int TrainingProviderId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string? SDLNumber { get; private set; }
    public string? ContactEmail { get; private set; }
    public string? Description { get; private set; }

    public virtual TrainingProvider TrainingProvider { get; private set; } = null!;
}