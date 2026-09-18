using Microsoft.AspNetCore.Authorization;
using DigiFikileLms.Application.Features.Content.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;


/// <summary>
/// FILE: DigiFikileLms.API/Controllers/ModulesController.cs
/// LAYER: API
///
/// WHAT THIS FILE IS:
///   An ASP.NET Core delivery-layer source file that accepts HTTP traffic, configures the host, or adapts application outcomes into HTTP responses.
///   Its immediate area is Modules; read the individual type comments below before extending it.
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
/// TYPE: ModulesController
/// PURPOSE: The HTTP adapter for modules endpoints.
/// LMS ROLE: Supports the Modules area while respecting the API layer boundary.
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
[Authorize(Roles = "SetaAdministrator,Facilitator")]
[ApiController]
[Route("api/courses/{courseId:guid}/modules")]
public class ModulesController : ControllerBase
{
    private readonly IMediator _mediator;

    public ModulesController(IMediator mediator) => _mediator = mediator;

    /// <summary>
    /// Executes the create operation for this type. Pass the supplied CancellationToken to downstream asynchronous work; do not block the thread.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Create(Guid courseId, [FromBody] CreateModuleRequest request, CancellationToken ct) =>
        Ok(await _mediator.Send(new CreateModuleCommand(courseId, request.Title, request.OrderIndex), ct));
}

/// <summary>
/// TYPE: CreateModuleRequest
/// PURPOSE: The api component responsible for create module request.
/// LMS ROLE: Supports the Modules area while respecting the API layer boundary.
///
/// IMPLEMENTATION GUIDE:
///   - Keep this type focused on the responsibility stated above.
///   - Follow neighbouring types for naming and dependency direction.
///   - Pass cancellation through asynchronous boundaries.
///
/// NEVER:
///   - Mix responsibilities from another Clean Architecture layer.
///   - Introduce a concrete outward dependency into an inner layer.
/// </summary>
public record CreateModuleRequest(string Title, int OrderIndex);
