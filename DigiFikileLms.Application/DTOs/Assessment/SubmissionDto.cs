namespace DigiFikileLms.Application.DTOs;

public class SubmissionDto
{
    public int Id { get; set; }
    public int AssessmentId { get; set; }
    public int StudentId { get; set; }
    public DateTime SubmissionDate { get; set; }
    public int Attempts { get; set; }
}