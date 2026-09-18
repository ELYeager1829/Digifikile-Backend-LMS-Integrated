using DigiFikileLms.Application.Common;
using DigiFikileLms.Application.DTOs.Courses;
using MediatR;


/// <summary>
/// FILE: DigiFikileLms.Application/Features/Courses/Commands/CourseCommands.cs
/// LAYER: Application
///
/// WHAT THIS FILE IS:
///   An application-layer source file that describes a use case, transport-safe DTO, validation rule, mapping, response contract, or abstraction required by a use case.
///   Its immediate area is Courses; read the individual type comments below before extending it.
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

namespace DigiFikileLms.Application.Features.Courses.Commands;

/// <summary>
/// TYPE: CreateCourseCommand
/// PURPOSE: An immutable CQRS write request for create course.
/// LMS ROLE: Supports the Courses area while respecting the Application layer boundary.
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
/// 
/// 
public record CreateCourseCommand(
    string CourseName,
    int FacilitatorId,
    string? SaqaId = null,
    string? CurriculumCode = null,
    int? NqfLevel = null
) : IRequest<BaseResponse<CourseDto>>;

public record UpdateCourseCommand(
    int Id,
    string? CourseName,
    string? SaqaId,
    string? CurriculumCode,
    int? NqfLevel
) : IRequest<BaseResponse<CourseDto>>;

public record PublishCourseCommand(
    int Id
) : IRequest<BaseResponse<bool>>;

public record ArchiveCourseCommand(
    int Id
) : IRequest<BaseResponse<bool>>;

public record DeleteCourseCommand(
    int Id
) : IRequest<BaseResponse<bool>>;


// ================================================================
// NOTE: Command handlers live in DigiFikileLms.Application.Features.Courses
//       (CourseHandlers.cs) to avoid duplicate MediatR registrations.
//       This file only declares the immutable command records.
// ================================================================

