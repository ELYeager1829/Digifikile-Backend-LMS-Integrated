namespace DigiFikileLms.Application.DTOs.Assessment;

public record CreateAssessmentDto(
    int ModuleId,
    int FacilitatorId,
    string Title,
    string AssessmentType,
    decimal? MaxScore = null,
    decimal? PassingScore = null,
    DateTime? DueDate = null
);

