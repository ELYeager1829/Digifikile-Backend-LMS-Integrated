using DigiFikileLms.Application.Features.SetaAdministrators.Commands;
using DigiFikileLms.Application.Features.SetaAdministrators.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DigiFikileLms.API.Controllers;

/// <summary>
/// HTTP adapter for SETA Administrator endpoints (the renamed Administrator concept).
/// </summary>
[Authorize(Roles = "SystemAdministrator")]
[ApiController]
[Route("api/[controller]")]
public class SetaAdministratorsController : ControllerBase
{
    private readonly IMediator _mediator;

    public SetaAdministratorsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var result = await _mediator.Send(new GetAllSetaAdministratorsQuery(page, pageSize), ct);
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id, CancellationToken ct)
    {
        var result = await _mediator.Send(new GetSetaAdministratorByIdQuery(id), ct);
        return result.IsSuccess ? Ok(result) : NotFound(result);
    }

    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetByUserId(int userId, CancellationToken ct)
    {
        var result = await _mediator.Send(new GetSetaAdministratorByUserIdQuery(userId), ct);
        return result.IsSuccess ? Ok(result) : NotFound(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateSetaAdministratorCommand command, CancellationToken ct)
    {
        var result = await _mediator.Send(command, ct);
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateSetaAdministratorCommand command, CancellationToken ct)
    {
        var result = await _mediator.Send(new UpdateSetaAdministratorCommand(
            id, command.Name, command.Surname, command.Email, command.Phone, command.Address, command.IsActive), ct);
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        var result = await _mediator.Send(new DeleteSetaAdministratorCommand(id), ct);
        return result.IsSuccess ? Ok(result) : NotFound(result);
    }
}