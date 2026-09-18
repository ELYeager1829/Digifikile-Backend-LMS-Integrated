namespace DigiFikileLms.Application.DTOs.Courses;

/// <summary>
/// Request DTO for creating a new course
/// </summary>
public record CreateCourseDto(
    string CourseName,
    string? SaqaId,
    string? CurriculumCode,
    int? NqfLevel,
    int? FacilitatorId
);

