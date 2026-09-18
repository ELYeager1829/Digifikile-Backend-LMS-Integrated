using DigiFikileLms.Domain.Common;
using DigiFikileLms.Domain.Enums;

namespace DigiFikileLms.Domain.Entities;

public class UserAccount : BaseEntity
{
    private readonly List<Notification> _notifications = new();
    public bool IsActive { get; private set; } = true;
    private UserAccount() { }

    public static UserAccount Create(
        string name,
        string surname,
        string email,
        string password,
        UserRole userRole,
        string? phone = null,
        string? address = null)
    {
        return new UserAccount
        {
            Name = name,
            Surname = surname,
            Email = email,
            Phone = phone,
            Address = address,
            Password = password,
            UserRole = userRole,
            DateCreated = DateTime.UtcNow
        };
    }

    public string Name { get; private set; } = string.Empty;
    public string Surname { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string? Phone { get; private set; }
    public string? Address { get; private set; }
    public string Password { get; private set; } = string.Empty;
    public UserRole UserRole { get; private set; }
    public DateTime DateCreated { get; private set; }

    public int? RoleId { get; private set; }
    public virtual Role? Role { get; private set; }

    public virtual SystemAdministrator? SystemAdministrator { get; private set; }
    public virtual SetaAdministrator? SetaAdministrator { get; private set; }
    public virtual Student? Student { get; private set; }
    public virtual Facilitator? Facilitator { get; private set; }
    public virtual Moderator? Moderator { get; private set; }
    public virtual TrainingProvider? TrainingProvider { get; private set; }

    public IReadOnlyCollection<Notification> Notifications => _notifications.AsReadOnly();

    public void AssignRole(Role role)
    {
        RoleId = role?.Id;
        Role = role;
        UpdateTimestamp();
    }

    public void ChangeUserRole(UserRole userRole)
    {
        UserRole = userRole;
        UpdateTimestamp();
    }

    public void UpdateProfile(string name, string surname, string? phone, string? address)
    {
        Name = name;
        Surname = surname;
        Phone = phone;
        Address = address;
        UpdateTimestamp();
    }

    public void UpdateEmail(string email)
    {
        Email = email;
        UpdateTimestamp();
    }

    public void UpdatePassword(string password)
    {
        Password = password;
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