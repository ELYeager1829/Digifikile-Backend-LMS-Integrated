namespace DigiFikileLms.Application.DTOs.SystemAdmin;

public class CreateRoleRequestDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
}