namespace DigiFikileLms.Application.DTOs.SystemAdmin;

public class SystemAdminDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Surname { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastLogin { get; set; }
}


public class CreateSetaAdminDto
{
    public string Name { get; set; } = string.Empty;
    public string Surname { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public List<SystemAdminPermissionDto> Permissions { get; set; } = new();
}

public class SystemAdminPermissionDto
{
    public string PermissionName { get; set; } = string.Empty;
    public bool IsGranted { get; set; }
}