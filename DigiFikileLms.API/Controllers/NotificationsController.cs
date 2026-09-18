using DigiFikileLms.Application.DTOs.Notification;
using DigiFikileLms.Application.Features.Notifications;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DigiFikileLms.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NotificationsController : ControllerBase
{
    private readonly IMediator _mediator;

    public NotificationsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // ================================================================
    // GET USER NOTIFICATIONS
    // ================================================================
    [HttpGet("user/{userId}")]
    [Authorize]
    public async Task<IActionResult> GetByUser(int userId)
    {
        var query = new GetUserNotificationsQuery(userId);
        var result = await _mediator.Send(query);
        return result.IsSuccess ? Ok(result) : NotFound(result);
    }

    // ================================================================
    // GET UNREAD NOTIFICATION COUNT
    // ================================================================
    [HttpGet("user/{userId}/unread-count")]
    [Authorize]
    public async Task<IActionResult> GetUnreadCount(int userId)
    {
        var query = new GetUnreadNotificationCountQuery(userId);
        var result = await _mediator.Send(query);
        return result.IsSuccess ? Ok(result) : NotFound(result);
    }

    // ================================================================
    // SEND NOTIFICATION - FIXED
    // ================================================================
    [HttpPost]
    [Authorize(Roles = "SetaAdministrator")]
    public async Task<IActionResult> Send([FromBody] CreateNotificationDto request)
    {
        var command = new SendNotificationCommand(request.UserId, request.Title, request.Message);
        var result = await _mediator.Send(command);
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }

    // ================================================================
    // MARK NOTIFICATION AS READ
    // ================================================================
    [HttpPut("{id}/read")]
    [Authorize]
    public async Task<IActionResult> MarkAsRead(int id)
    {
        var command = new MarkNotificationReadCommand(id);
        var result = await _mediator.Send(command);
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }

    // ================================================================
    // MARK ALL NOTIFICATIONS AS READ
    // ================================================================
    [HttpPut("user/{userId}/read-all")]
    [Authorize]
    public async Task<IActionResult> MarkAllAsRead(int userId)
    {
        var command = new MarkAllNotificationsReadCommand(userId);
        var result = await _mediator.Send(command);
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }
}

