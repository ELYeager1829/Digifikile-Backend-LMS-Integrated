namespace DigiFikileLms.Application.DTOs.Moderators;

/// <summary>
/// Transport-safe representation of a Moderator profile.
/// </summary>
public record ModeratorDto(
    int Id,
    int UserId,
    string StaffNumber,
    string Name,
    string Surname,
    string Email,
    DateTime CreatedAt);