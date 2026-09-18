namespace DigiFikileLms.Application.DTOs.Auth;

/// <summary>
/// Request DTO for admin login using email
/// </summary>
public record AdminLoginRequestDto(
    string Email,
    string Password
);
