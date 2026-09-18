namespace DigiFikileLms.Application.DTOs;

public class TrainingCompletionReportDto
{
    public int TotalStudents { get; set; }
    public int CompletedStudents { get; set; }
    public int InProgressStudents { get; set; }
    public int NotStartedStudents { get; set; }
    public decimal CompletionRate { get; set; }
    public List<CourseCompletionDto> CourseCompletions { get; set; } = new();
}

public class CourseCompletionDto
{
    public int CourseId { get; set; }
    public string CourseName { get; set; } = string.Empty;
    public int TotalEnrolled { get; set; }
    public int Completed { get; set; }
    public decimal CompletionRate { get; set; }
}