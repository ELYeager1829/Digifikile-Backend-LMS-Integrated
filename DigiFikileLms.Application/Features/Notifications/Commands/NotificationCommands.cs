using DigiFikileLms.Application.Common;
using DigiFikileLms.Application.DTOs;
using MediatR;

namespace DigiFikileLms.Application.Features.Notifications;

public record SendNotificationCommand(
    int UserId,
    string Title,
    string Message
) : IRequest<BaseResponse<bool>>;

public record MarkNotificationReadCommand(
    int NotificationId
) : IRequest<BaseResponse<bool>>;

public record MarkAllNotificationsReadCommand(
    int UserId
) : IRequest<BaseResponse<bool>>;