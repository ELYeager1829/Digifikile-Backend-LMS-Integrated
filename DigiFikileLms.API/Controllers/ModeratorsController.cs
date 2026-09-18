using DigiFikileLms.Application.Features.Moderators.Commands;
using DigiFikileLms.Application.Features.Moderators.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DigiFikileLms.API.Controllers;

/// <summary>
/// HTTP adapter for moderator endpoints (the renamed Assessor concept).
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ModeratorsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ModeratorsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [Authorize(Roles = "SetaAdministrator")]
    public async Task<IActionResult> GetAll(CancellationToken ct, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var result = await _mediator.Send(new GetAllModeratorsQuery(page, pageSize), ct);
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "SetaAdministrator")]
    public async Task<IActionResult> GetById(int id, CancellationToken ct)
    {
        var result = await _mediator.Send(new GetModeratorByIdQuery(id), ct);
        return result.IsSuccess ? Ok(result) : NotFound(result);
    }

    [HttpGet("user/{userId}")]
    [Authorize(Roles = "SetaAdministrator")]
    public async Task<IActionResult> GetByUserId(int userId, CancellationToken ct)
    {
        var result = await _mediator.Send(new GetModeratorByUserIdQuery(userId), ct);
        return result.IsSuccess ? Ok(result) : NotFound(result);
    }

    [HttpPost]
    [Authorize(Roles = "SetaAdministrator")]
    public async Task<IActionResult> Create([FromBody] CreateModeratorCommand command, CancellationToken ct)
    {
        var result = await _mediator.Send(command, ct);
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "SetaAdministrator")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateModeratorCommand command, CancellationToken ct)
    {
        var result = await _mediator.Send(new UpdateModeratorCommand(
            id, command.Name, command.Surname, command.Email, command.Phone, command.Address), ct);
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "SetaAdministrator")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        var result = await _mediator.Send(new DeleteModeratorCommand(id), ct);
        return result.IsSuccess ? Ok(result) : NotFound(result);
    }
}