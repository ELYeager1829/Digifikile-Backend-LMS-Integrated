namespace DigiFikileLms.Application.DTOs;

public class UserDto
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Surname { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public int? RoleId { get; set; }
    public bool IsActive { get; set; }
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? StudentNumber { get; set; }
    public List<string> Permissions { get; set; } = new();
}
