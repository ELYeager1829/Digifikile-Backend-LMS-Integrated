namespace DigiFikileLms.Application.DTOs.Assessment;

public class AssessmentDto
{
    public int Id { get; set; }
    public int ModuleId { get; set; }
    public int FacilitatorId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string AssessmentType { get; set; } = string.Empty;
    public string? Instructions { get; set; }
    public decimal? MaxScore { get; set; }
    public decimal? PassingScore { get; set; }
    public DateTime? DueDate { get; set; }
    public bool IsPublished { get; set; }
    public DateTime CreatedAt { get; set; }
}

