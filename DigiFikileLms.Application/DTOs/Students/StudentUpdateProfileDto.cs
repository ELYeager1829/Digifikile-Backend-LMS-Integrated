namespace DigiFikileLms.Application.DTOs.Students;

/// <summary>
/// Request DTO for updating a student's profile
/// </summary>
public record StudentUpdateProfileDto(
    string? Phone,
    string? Address
);
