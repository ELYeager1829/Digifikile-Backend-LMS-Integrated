namespace DigiFikileLms.Application.DTOs.Auth;

public class StudentRegisterRequestDto
{
    public string Name { get; set; } = string.Empty;
    public string Surname { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string StudentNumber { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Address { get; set; }
}

/// <summary>
/// Request DTO for student login using student number
/// </summary>
public record StudentLoginRequestDto(
    string StudentNumber,
    string Password
);

