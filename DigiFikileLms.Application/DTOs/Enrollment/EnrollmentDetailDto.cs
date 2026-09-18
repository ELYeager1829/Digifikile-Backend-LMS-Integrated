namespace DigiFikileLms.Application.DTOs;

public class EnrollmentDetailDto
{
    public int EnrollmentId { get; set; }
    public int StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public string StudentNumber { get; set; } = string.Empty;
    public string CourseName { get; set; } = string.Empty;
    public DateTime EnrolledAt { get; set; }
}