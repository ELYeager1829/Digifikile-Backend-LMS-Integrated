namespace DigiFikileLms.Application.DTOs;

public class StudentEnrollmentDto
{
    public int EnrollmentId { get; set; }
    public int CourseId { get; set; }
    public string CourseName { get; set; } = string.Empty;
    public string CourseCode { get; set; } = string.Empty;
    public DateTime EnrolledAt { get; set; }
    public int? NqfLevel { get; set; }
}