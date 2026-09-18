using DigiFikileLms.Domain.Common;

namespace DigiFikileLms.Domain.Entities;

/// <summary>
/// Replaces the legacy "Administrator" concept. A SetaAdministrator governs the
/// SETA programme catalogue and owns compliance Reports.
/// </summary>
public class SetaAdministrator : BaseEntity
{
    private readonly List<SETAProgramme> _setaProgrammes = new();
    private readonly List<Report> _reports = new();

    private SetaAdministrator() { }

    public static SetaAdministrator Create(UserAccount user)
    {
        return new SetaAdministrator
        {
            UserId = user.Id,
            User = user,
            IsActive = true
        };
    }

    public int UserId { get; private set; }
    public DateTime? LastLogin { get; private set; }
    public bool IsActive { get; private set; }
    public UserAccount User { get; private set; } = null!;
    public IReadOnlyCollection<SETAProgramme> SETAProgrammes => _setaProgrammes.AsReadOnly();
    public IReadOnlyCollection<Report> Reports => _reports.AsReadOnly();

    public void RecordLogin()
    {
        LastLogin = DateTime.UtcNow;
        UpdateTimestamp();
    }

    public void Activate()
    {
        IsActive = true;
        UpdateTimestamp();
    }

    public void Deactivate()
    {
        IsActive = false;
        UpdateTimestamp();
    }
}
