using DigiFikileLms.Domain.Common;

namespace DigiFikileLms.Domain.Entities;

/// <summary>
/// A named grouping of access rights in the LMS (Student, Facilitator, Moderator,
/// SetaAdministrator, TrainingProvider). Roles gather Permissions and are assigned
/// to UserAccounts through RoleId.
/// </summary>
public class Role : BaseEntity
{
    private readonly List<Permission> _permissions = new();
    private readonly List<RolePermission> _rolePermissions = new();
    private readonly List<UserRoleAssignment> _userRoles = new();  //to be

    private Role() { }

    public static Role Create(string name, string? description = null)
    {
        return new Role
        {
            Name = name,
            Description = description
        };
    }

    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }

    public IReadOnlyCollection<Permission> Permissions => _permissions.AsReadOnly();

    public virtual ICollection<RolePermission> RolePermissions => _rolePermissions.AsReadOnly();  // ✅ Add this
    public virtual ICollection<UserRoleAssignment> UserRoles => _userRoles.AsReadOnly();

    public void UpdateDetails(string name, string? description)
    {
        Name = name;
        Description = description;
        UpdateTimestamp();
    }

    public void AddPermission(Permission permission)
    {
        if (!_permissions.Any(p => p.Id == permission.Id))
        {
            _permissions.Add(permission);
            UpdateTimestamp();
        }
    }

    public void RemovePermission(Permission permission)
    {
        _permissions.RemoveAll(p => p.Id == permission.Id);
        UpdateTimestamp();
    }

    public bool HasPermission(string code)
    {
        return _permissions.Any(p => p.Code == code);
    }
}