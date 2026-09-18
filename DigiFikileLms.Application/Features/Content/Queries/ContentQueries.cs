using DigiFikileLms.Application.Common;
using DigiFikileLms.Application.DTOs;
using MediatR;


/// <summary>
/// FILE: DigiFikileLms.Application/Features/Content/Queries/ContentQueries.cs
/// LAYER: Application
///
/// WHAT THIS FILE IS:
///   An application-layer source file that describes a use case, transport-safe DTO, validation rule, mapping, response contract, or abstraction required by a use case.
///   Its immediate area is Content; read the individual type comments below before extending it.
///
/// WHY THIS LAYER:
///   Application coordinates what DigiFikile LMS does while remaining independent of HTTP and concrete infrastructure. It may depend on Domain, while API and Infrastructure depend inward on it.
///   Dependency direction is Domain &lt;- Application &lt;- Infrastructure / API. Dependencies must point inward.
///
/// HOW TO CODE HERE (for juniors):
///   - Represent writes as immutable Command records and reads as immutable Query records; implement each with a focused MediatR handler.
///   - Let FluentValidation reject malformed input before a handler runs; still enforce true business invariants in Domain.
///   - Orchestrate repositories and service interfaces in handlers, passing CancellationToken through every async call.
///   - Return BaseResponse&lt;T&gt; consistently so expected success and failure information has one application-level shape.
///   - Map entities to DTOs with AutoMapper; never expose tracked domain entities over the network.
///   - Apply SRP by keeping one use-case concern per handler and DIP by depending on interfaces rather than EF, JWT, or hashing implementations.
///   - Name requests with an action or question, for example CreateCourseCommand or GetCourseByIdQuery.
///
/// DO NOT:
///   - Reference controllers, HttpContext, IActionResult, StatusCodes, concrete DbContext classes, SQL, or EF Core query APIs.
///   - Generate JWTs, hash passwords, or perform persistence directly; call an Application or Domain interface.
///   - Mix writes into queries or use a command for a read-only operation.
///   - Ignore CancellationToken, return entities as DTOs, or duplicate validation across controllers and handlers.
///
/// RELATED FILES:
///   - DigiFikileLms.Domain/** — supplies entities, enums, and repository contracts.
///   - DigiFikileLms.Infrastructure/** — implements service and persistence abstractions required here.
///   - DigiFikileLms.API/Controllers/** — translates HTTP requests into these commands and queries.
///
/// DIGIFIKILE LMS CONTEXT:
///   - This file helps execute LMS workflows for authentication, courses, content, enrollment, assessment, progress, reporting, users, or roles without coupling those workflows to delivery or storage technology.
/// </summary>

namespace DigiFikileLms.Application.Features.Content.Queries;

/// <summary>
/// TYPE: GetCourseContentQuery
/// PURPOSE: An immutable CQRS read request for get course content.
/// LMS ROLE: Supports the Content area while respecting the Application layer boundary.
///
/// IMPLEMENTATION GUIDE:
///   - Describe filters, identity, and paging explicitly.
///   - Keep the request side-effect free.
///   - Return DTO-shaped data through its handler.
///
/// NEVER:
///   - Change domain state.
///   - Return tracked entities to the API.
/// </summary>
public record GetCourseContentQuery(Guid CourseId) : IRequest<BaseResponse<IReadOnlyList<ModuleDto>>>;

/// <summary>
/// TYPE: GetCourseContentQueryHandler
/// PURPOSE: The MediatR handler that orchestrates the Get Course Content Query use case.
/// LMS ROLE: Supports the Content area while respecting the Application layer boundary.
///
/// IMPLEMENTATION GUIDE:
///   - Receive already validated request data.
///   - Load or change state through interfaces.
///   - Invoke domain behaviour rather than setting protected state.
///   - Map output to a DTO and return BaseResponse&lt;T&gt;.
///   - Forward cancellationToken to every asynchronous dependency.
///
/// NEVER:
///   - Use HttpContext or select HTTP status codes.
///   - Call concrete EF Core, JWT, or hashing types.
/// </summary>
public class GetCourseContentQueryHandler : IRequestHandler<GetCourseContentQuery, BaseResponse<IReadOnlyList<ModuleDto>>>
{
    /// <summary>
    /// Executes the handle operation for this type. Pass the supplied CancellationToken to downstream asynchronous work; do not block the thread.
    /// </summary>
    public Task<BaseResponse<IReadOnlyList<ModuleDto>>> Handle(GetCourseContentQuery request, CancellationToken cancellationToken) =>
        throw new NotImplementedException("TODO: Load modules and nested lessons for course.");
}
