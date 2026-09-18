using DigiFikileLms.Application.Common;
using DigiFikileLms.Application.DTOs.Permission;
using DigiFikileLms.Application.DTOs.Roles;
using MediatR;


/// <summary>
/// FILE: DigiFikileLms.Application/Features/Roles/Queries/RoleQueries.cs
/// LAYER: Application
///
/// WHAT THIS FILE IS:
///   An application-layer source file that describes a use case, transport-safe DTO, validation rule, mapping, response contract, or abstraction required by a use case.
///   Its immediate area is Roles; read the individual type comments below before extending it.
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

namespace DigiFikileLms.Application.Features.Roles.Queries;

/// <summary>
/// TYPE: GetRolesQuery
/// PURPOSE: An immutable CQRS read request for get roles.
/// LMS ROLE: Supports the Roles area while respecting the Application layer boundary.
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
public record GetRolesQuery : IRequest<BaseResponse<IReadOnlyList<RoleDto>>>;

public record GetRoleByIdQuery(int Id) : IRequest<BaseResponse<RoleDetailDto>>;

public record GetRolePermissionsQuery(int RoleId) : IRequest<BaseResponse<IReadOnlyList<PermissionDto>>>;
