namespace DigiFikileLms.Application.DTOs.Assessment;

public record SubmitAssessmentDto(
    int StudentId,
    object Answers
);
