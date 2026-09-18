namespace DigiFikileLms.Application.DTOs.SystemAdmin;

public class AssignPermissionsRequestDto
{
    public List<int> PermissionIds { get; set; } = new();
}