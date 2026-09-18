namespace DigiFikileLms.Application.DTOs.Assessment;

public record GradeAssessmentDto(
    int SubmissionId,
    int Percentage,
    string Grade,
    string? Feedback = null
);

