namespace DigiFikileLms.Application.DTOs.Progress;

public record UpdateProgressDto(
    int StudentId,
    int CourseId,
    int Percentage,
    string Status
);

