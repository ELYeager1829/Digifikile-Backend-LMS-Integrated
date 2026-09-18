using DigiFikileLms.Application.Common;
using DigiFikileLms.Application.DTOs;
using MediatR;

namespace DigiFikileLms.Application.Features.Notifications;

public record GetUserNotificationsQuery(
    int UserId
) : IRequest<BaseResponse<List<NotificationDto>>>;

public record GetUnreadNotificationCountQuery(
    int UserId
) : IRequest<BaseResponse<int>>;