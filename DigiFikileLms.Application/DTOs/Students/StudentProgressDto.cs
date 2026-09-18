namespace DigiFikileLms.Application.DTOs.Students;

/// <summary>
/// DTO for student progress information
/// </summary>
public record StudentProgressDto(
    int CourseId,
    string CourseName,
    string Status,
    decimal Percentage,
    DateTime? LastUpdated
);
