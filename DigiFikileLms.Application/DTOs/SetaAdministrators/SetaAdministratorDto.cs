namespace DigiFikileLms.Application.DTOs.SetaAdministrators;

/// <summary>
/// Transport-safe representation of a SETA Administrator profile.
/// </summary>
public record SetaAdministratorDto(
    int Id,
    int UserId,
    string Name,
    string Surname,
    string Email,
    bool IsActive,
    DateTime? LastLogin,
    DateTime CreatedAt);