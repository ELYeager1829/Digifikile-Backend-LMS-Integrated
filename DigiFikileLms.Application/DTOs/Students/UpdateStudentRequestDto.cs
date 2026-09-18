namespace DigiFikileLms.Application.DTOs.Students;

/// <summary>
/// Request DTO for updating a student (Admin only)
/// </summary>
public record UpdateStudentRequestDto(
    string? Name,
    string? Surname,
    string? Email,
    string? Phone,
    string? Address,
    bool? IsActive
);
