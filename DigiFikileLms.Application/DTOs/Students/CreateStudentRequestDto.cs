namespace DigiFikileLms.Application.DTOs.Students;

/// <summary>
/// Request DTO for creating a new student (Admin only)
/// </summary>
public record CreateStudentRequestDto(
    string Name,
    string Surname,
    string Email,
    string Password,
    string? Phone = null,
    string? Address = null,
    string? StudentNumber = null
);
