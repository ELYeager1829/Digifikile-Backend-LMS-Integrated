namespace DigiFikileLms.Application.DTOs.Students;

/// <summary>
/// DTO for student profile information
/// </summary>
public record StudentProfileDto(
    int Id,
    string StudentNumber,
    string Name,
    string Surname,
    string Email,
    string? Phone,
    string? Address,
    DateTime EnrolledAt,
    bool IsActive = true
);
