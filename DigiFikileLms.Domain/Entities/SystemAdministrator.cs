using DigiFikileLms.Domain.Common;

namespace DigiFikileLms.Domain.Entities;

public class SystemAdministrator : BaseEntity
{
    private readonly List<SystemLog> _systemLogs = new();

    private SystemAdministrator() { }

    public static SystemAdministrator Create(UserAccount user)
    {
        return new SystemAdministrator
        {
            UserId = user.Id,
            User = user,
            IsActive = true,
            LastLogin = null
        };
    }

    public int UserId { get; private set; }
    public DateTime? LastLogin { get; private set; }
    public bool IsActive { get; private set; } = true;

    // Navigation Properties
    public virtual UserAccount User { get; private set; } = null!;
    public virtual ICollection<SystemLog> SystemLogs => _systemLogs.AsReadOnly();

    // Domain Behaviors
    public void RecordLogin()
    {
        LastLogin = DateTime.UtcNow;
        UpdateTimestamp();
    }

    public void Deactivate()
    {
        IsActive = false;
        UpdateTimestamp();
    }

    public void Activate()
    {
        IsActive = true;
        UpdateTimestamp();
    }
}