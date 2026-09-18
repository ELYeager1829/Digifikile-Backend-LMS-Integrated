namespace DigiFikileLms.Application.DTOs.Courses;

public record UpdateCourseDto(
    string? CourseName,
    string? SaqaId,
    string? CurriculumCode,
    int? NqfLevel,
    string? Status
);

