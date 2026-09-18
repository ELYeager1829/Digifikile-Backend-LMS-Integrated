using DigiFikileLms.Domain.Common;

namespace DigiFikileLms.Domain.Entities;

public class RolePermission : BaseEntity
{
    private RolePermission() { }

    public static RolePermission Create(Role role, Permission permission)
    {
        return new RolePermission
        {
            RoleId = role.Id,
            Role = role,
            PermissionId = permission.Id,
            Permission = permission,
            AssignedAt = DateTime.UtcNow
        };
    }

    public int RoleId { get; private set; }
    public int PermissionId { get; private set; }
    public DateTime AssignedAt { get; private set; }

    public virtual Role Role { get; private set; } = null!;
    public virtual Permission Permission { get; private set; } = null!;
}