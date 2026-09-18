namespace DigiFikileLms.Application.DTOs.Students;

/// <summary>
/// DTO for listing students (Admin only)
/// </summary>
public record StudentListDto(
    int Id,
    string StudentNumber,
    string Name,
    string Surname,
    string Email,
    DateTime EnrolledAt,
    int EnrollmentCount,
    decimal AverageProgress
);

