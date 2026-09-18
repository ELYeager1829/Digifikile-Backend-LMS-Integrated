namespace DigiFikileLms.Application.DTOs.Progress;

public record MarkLessonCompleteDto(
    int StudentId,
    int CourseId,
    int LessonId
);


