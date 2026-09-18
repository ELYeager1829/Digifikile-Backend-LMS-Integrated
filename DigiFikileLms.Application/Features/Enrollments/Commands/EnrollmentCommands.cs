using DigiFikileLms.Application.Common;
using DigiFikileLms.Application.DTOs;
using MediatR;


/// <summary>
/// FILE: DigiFikileLms.Application/Features/Enrollments/Commands/EnrollmentCommands.cs
/// LAYER: Application
///
/// WHAT THIS FILE IS:
///   An application-layer source file that describes a use case, transport-safe DTO, validation rule, mapping, response contract, or abstraction required by a use case.
///   Its immediate area is Enrollments; read the individual type comments below before extending it.
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

namespace DigiFikileLms.Application.Features.Enrollments;
///namespace DigiFikileLms.Application.Features.Enrollments.Commands;

/// <summary>
/// TYPE: EnrollInCourseCommand
/// PURPOSE: An immutable CQRS write request for enroll in course.
/// LMS ROLE: Supports the Enrollments area while respecting the Application layer boundary.
///
/// IMPLEMENTATION GUIDE:
///   - Keep the record limited to data needed by this write.
///   - Use clear scalar or request-model values.
///   - Add validation in Validators and behaviour in its handler/domain entity.
///
/// NEVER:
///   - Perform work inside the record.
///   - Use a Command for a read-only operation.
/// </summary>
public record EnrollInCourseCommand(Guid CourseId, Guid UserId) : IRequest<BaseResponse<EnrollmentDto>>;

//start
public record EnrollStudentCommand(
    int StudentId,
    int CourseId
) : IRequest<BaseResponse<StudentEnrollmentDto>>;

public record WithdrawEnrollmentCommand(
    int EnrollmentId
) : IRequest<BaseResponse<bool>>;

public record CompleteEnrollmentCommand(
    int EnrollmentId
) : IRequest<BaseResponse<bool>>;
/// <summary>
/// TYPE: EnrollInCourseCommandHandler
/// PURPOSE: The MediatR handler that orchestrates the Enroll In Course Command use case.
/// LMS ROLE: Supports the Enrollments area while respecting the Application layer boundary.
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
public class EnrollInCourseCommandHandler : IRequestHandler<EnrollInCourseCommand, BaseResponse<EnrollmentDto>>
{
    /// <summary>
    /// Executes the handle operation for this type. Pass the supplied CancellationToken to downstream asynchronous work; do not block the thread.
    /// </summary>
    public Task<BaseResponse<EnrollmentDto>> Handle(EnrollInCourseCommand request, CancellationToken cancellationToken) =>
        throw new NotImplementedException("TODO: Create enrollment if course is published.");
}
