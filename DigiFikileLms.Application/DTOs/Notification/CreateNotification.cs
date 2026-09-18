namespace DigiFikileLms.Application.DTOs.Notification;

public record CreateNotificationDto(
    int UserId,
    string Title,
    string Message,
    string Type
);

