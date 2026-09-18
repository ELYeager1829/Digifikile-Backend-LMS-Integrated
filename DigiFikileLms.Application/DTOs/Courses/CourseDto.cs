namespace DigiFikileLms.Application.DTOs.Courses;

/// <summary>
/// DTO for course information
/// </summary>
public class CourseDto
{
    public int Id { get; set; }
    public string CourseName { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string? SaqaId { get; set; }
    public string? CurriculumCode { get; set; }
    public int? NqfLevel { get; set; }
    public int? FacilitatorId { get; set; }
    public DateTime CreatedAt { get; set; }
}

