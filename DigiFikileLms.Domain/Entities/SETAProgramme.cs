using DigiFikileLms.Domain.Common;

namespace DigiFikileLms.Domain.Entities;

public class SETAProgramme : BaseEntity
{
    private SETAProgramme() { }

    public static SETAProgramme Create(
        SetaAdministrator setaAdministrator,
        string programmeName,
        string? programmeType = null,
        string? setaId = null)
    {
        return new SETAProgramme
        {
            SetaAdministratorId = setaAdministrator.Id,
            SetaAdministrator = setaAdministrator,
            ProgrammeName = programmeName,
            ProgrammeType = programmeType,
            SetaId = setaId
        };
    }

    public int SetaAdministratorId { get; private set; }
    public string ProgrammeName { get; private set; } = string.Empty;
    public string? ProgrammeType { get; private set; }
    public string? SetaId { get; private set; }

    public virtual SetaAdministrator SetaAdministrator { get; private set; } = null!;
}