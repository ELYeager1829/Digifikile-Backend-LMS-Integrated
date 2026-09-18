using DigiFikileLms.Application.Features.Roles.Commands;
using DigiFikileLms.Application.Features.Roles.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;


/// <summary>
/// FILE: DigiFikileLms.API/Controllers/RolesController.cs
/// LAYER: API
///
/// WHAT THIS FILE IS:
///   An ASP.NET Core delivery-layer source file that accepts HTTP traffic, configures the host, or adapts application outcomes into HTTP responses.
///   Its immediate area is Roles; read the individual type comments below before extending it.
///
/// WHY THIS LAYER:
///   API is the outermost presentation layer. It may call Application and register Infrastructure, but Domain and Application must never depend on ASP.NET Core.
///   Dependency direction is Domain &lt;- Application &lt;- Infrastructure / API. Dependencies must point inward.
///
/// HOW TO CODE HERE (for juniors):
///   - Keep controllers thin: bind and validate transport input, extract trusted claims, send one MediatR request, and translate the result to an HTTP response.
///   - Use Commands for writes and Queries for reads; let handlers own use-case orchestration.
///   - Pass HttpContext.RequestAborted or the action CancellationToken all the way through MediatR.
///   - Return DTOs rather than entities, and use BaseResponse&lt;T&gt; information consistently when selecting status codes.
///   - Put cross-cutting HTTP concerns such as exceptions, correlation IDs, and request logging in middleware.
///   - Keep Program.cs as the composition root: register dependencies and order middleware without implementing business rules.
///   - Apply SRP to each controller/middleware component and DIP by consuming MediatR or inner-layer abstractions.
///
/// DO NOT:
///   - Write business rules, EF queries, SQL, password hashing, JWT construction, or repository orchestration in controllers.
///   - Return Domain entities directly over the wire or leak stack traces and secrets to clients.
///   - Put HTTP concepts such as StatusCodes, claims, headers, or IActionResult in Domain or Application.
///   - Block on async work with .Result/.Wait(), drop CancellationToken, or duplicate FluentValidation rules in actions.
///
/// RELATED FILES:
///   - DigiFikileLms.Application/Features/** — commands, queries, and handlers invoked through MediatR.
///   - DigiFikileLms.Application/DTOs/** — response shapes safe to expose to clients.
///   - DigiFikileLms.Infrastructure/Extensions/** — outer-layer services registered by the composition root.
///
/// DIGIFIKILE LMS CONTEXT:
///   - This file exposes or supports the HTTP surface used to manage LMS authentication, users, roles, courses, content, enrollment, assessment, progress, and reports.
/// </summary>

namespace DigiFikileLms.API.Controllers;

/// <summary>
/// TYPE: RolesController
/// PURPOSE: The HTTP adapter for roles endpoints.
/// LMS ROLE: Supports the Roles area while respecting the API layer boundary.
///
/// IMPLEMENTATION GUIDE:
///   - Bind route/body/query data.
///   - Extract identity/role claims when required.
///   - Send one Command or Query through IMediator.
///   - Translate BaseResponse results into an appropriate HTTP status and DTO body.
///
/// NEVER:
///   - Call repositories or DbContext directly.
///   - Implement authorization or business decisions with controller-local logic.
/// </summary>
[Authorize(Roles = "SystemAdministrator")]
[ApiController]
[Route("api/[controller]")]
public class RolesController : ControllerBase
{
    private readonly IMediator _mediator;

    public RolesController(IMediator mediator) => _mediator = mediator;

    private int CurrentUserId() => int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var id) ? id : 0;

    /// <summary>
    /// Executes the get all operation for this type. Pass the supplied CancellationToken to downstream asynchronous work; do not block the thread.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct) =>
        Ok(await _mediator.Send(new GetRolesQuery(), ct));

    /// <summary>
    /// Executes the create operation for this type. Pass the supplied CancellationToken to downstream asynchronous work; do not block the thread.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateRoleCommand command, CancellationToken ct) =>
        Ok(await _mediator.Send(command, ct));

    // ================================================================
    // GET ROLE BY ID (with permissions)
    // ================================================================
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id, CancellationToken ct)
    {
        var result = await _mediator.Send(new GetRoleByIdQuery(id), ct);
        return result.IsSuccess ? Ok(result) : NotFound(result);
    }

    // ================================================================
    // UPDATE ROLE
    // ================================================================
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateRoleCommand command, CancellationToken ct)
    {
        var result = await _mediator.Send(new UpdateRoleCommand(id, command.Name, command.Description), ct);
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }

    // ================================================================
    // DELETE ROLE
    // ================================================================
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        var result = await _mediator.Send(new DeleteRoleCommand(id), ct);
        return result.IsSuccess ? Ok(result) : NotFound(result);
    }

    // ================================================================
    // GET ROLE PERMISSIONS
    // ================================================================
    [HttpGet("{id}/permissions")]
    public async Task<IActionResult> GetPermissions(int id, CancellationToken ct)
    {
        var result = await _mediator.Send(new GetRolePermissionsQuery(id), ct);
        return result.IsSuccess ? Ok(result) : NotFound(result);
    }

    // ================================================================
    // ASSIGN PERMISSION TO ROLE
    // ================================================================
    [HttpPost("{id}/permissions/{permissionId}")]
    public async Task<IActionResult> AssignPermission(int id, int permissionId, CancellationToken ct)
    {
        var result = await _mediator.Send(new AssignPermissionCommand(id, permissionId, CurrentUserId()), ct);
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }

    [HttpDelete("{id}/permissions/{permissionId}")]
    public async Task<IActionResult> RemovePermission(int id, int permissionId, CancellationToken ct)
    {
        var result = await _mediator.Send(new RemovePermissionCommand(id, permissionId, CurrentUserId()), ct);
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }
}
