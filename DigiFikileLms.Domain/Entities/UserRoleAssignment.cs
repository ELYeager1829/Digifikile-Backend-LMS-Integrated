using DigiFikileLms.Domain.Common;

namespace DigiFikileLms.Domain.Entities;

public class UserRoleAssignment : BaseEntity
{
    private UserRoleAssignment() { }

    public static UserRoleAssignment Create(UserAccount user, Role role)
    {
        return new UserRoleAssignment
        {
            UserId = user.Id,
            User = user,
            RoleId = role.Id,
            Role = role,
            AssignedAt = DateTime.UtcNow
        };
    }

    public int UserId { get; private set; }
    public int RoleId { get; private set; }
    public DateTime AssignedAt { get; private set; }

    public virtual UserAccount User { get; private set; } = null!;
    public virtual Role Role { get; private set; } = null!;
}