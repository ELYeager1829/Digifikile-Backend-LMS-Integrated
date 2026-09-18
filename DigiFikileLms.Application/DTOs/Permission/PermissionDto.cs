namespace DigiFikileLms.Application.DTOs.Permission;

public class PermissionDto
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Category { get; set; }
    public DateTime CreatedAt { get; set; }

    // ✅ Parameterless constructor
    public PermissionDto() { }

    // ✅ Constructor with 4 arguments (matching your usage)
    public PermissionDto(int id, string code, string name, string? description)
    {
        Id = id;
        Code = code;
        Name = name;
        Description = description;
    }
}