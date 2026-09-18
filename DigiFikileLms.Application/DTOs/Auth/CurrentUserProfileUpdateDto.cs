namespace DigiFikileLms.Application.DTOs.Auth;

/// <summary>Fields a signed-in user may update on their own profile.</summary>
public record CurrentUserProfileUpdateDto(
    string Name,
    string Surname,
    string? Phone,
    string? Address
);
