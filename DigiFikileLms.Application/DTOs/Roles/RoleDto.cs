using DigiFikileLms.Application.DTOs.Permission;

namespace DigiFikileLms.Application.DTOs.Roles;

/// <summary>
/// A fine-grained authorization capability exposed to clients.
/// </summary>
/// 

public class RoleDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int PermissionCount { get; set; }
    public List<PermissionDto> Permissions { get; set; } = new();
    public DateTime CreatedAt { get; set; }


    // ✅ Parameterless constructor (required for serialization)
    public RoleDto() { }

    // ✅ Constructor with 3 arguments (matching your usage)
    public RoleDto(int id, string name, string? description)
    {
        Id = id;
        Name = name;
        Description = description;
    }

}

/// <summary>
/// A role together with its assigned permissions.
/// </summary>
public record RoleDetailDto(
    int Id,
    string Name,
    string? Description,
    IReadOnlyList<PermissionDto> Permissions);