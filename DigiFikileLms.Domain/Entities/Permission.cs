using DigiFikileLms.Domain.Common;

namespace DigiFikileLms.Domain.Entities;

/// <summary>
/// A fine-grained authorization capability (e.g. manage_users, grade_assessments).
/// Permissions are grouped into Roles through the RolePermissions join table.
/// </summary>
public class Permission : BaseEntity
{
    private Permission() { }

    public static Permission Create(string code, string name, string? description = null)
    {
        return new Permission
        {
            Code = code,
            Name = name,
            Description = description
        };
    }

    public string Code { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public string? Category { get; private set; }
    public string? Description { get; private set; }

    public void UpdateDetails(string code, string name, string? description)
    {
        Code = code;
        Name = name;
        Description = description;
        UpdateTimestamp();
    }
}