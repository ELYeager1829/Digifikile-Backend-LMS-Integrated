/// <summary>
/// FILE: DigiFikileLms.Application/Common/ApplicationException.cs
/// LAYER: Application
///
/// WHAT THIS FILE IS:
///   An application-layer source file that describes a use case, transport-safe DTO, validation rule, mapping, response contract, or abstraction required by a use case.
///   Its immediate area is Application Exception; read the individual type comments below before extending it.
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

namespace DigiFikileLms.Application.Common;

/// <summary>
/// TYPE: NotFoundException
/// PURPOSE: An application-level exception representing not found failure.
/// LMS ROLE: Supports the Application Exception area while respecting the Application layer boundary.
///
/// IMPLEMENTATION GUIDE:
///   - Use only for exceptional flow represented by this type.
///   - Keep messages safe for logging/presentation policy.
///   - Let API middleware translate it at the boundary.
///
/// NEVER:
///   - Reference ASP.NET status-code constants.
///   - Use exceptions instead of expected BaseResponse&lt;T&gt; failures without a clear policy.
/// </summary>
public class NotFoundException : Exception
{
    public NotFoundException(string entityName, object id)
        : base($"{entityName} with Id '{id}' was not found.") { }
}

/// <summary>
/// TYPE: ForbiddenException
/// PURPOSE: An application-level exception representing forbidden failure.
/// LMS ROLE: Supports the Application Exception area while respecting the Application layer boundary.
///
/// IMPLEMENTATION GUIDE:
///   - Use only for exceptional flow represented by this type.
///   - Keep messages safe for logging/presentation policy.
///   - Let API middleware translate it at the boundary.
///
/// NEVER:
///   - Reference ASP.NET status-code constants.
///   - Use exceptions instead of expected BaseResponse&lt;T&gt; failures without a clear policy.
/// </summary>
public class ForbiddenException : Exception
{
    public ForbiddenException(string message = "You do not have permission to perform this action.")
        : base(message) { }
}

/// <summary>
/// TYPE: ValidationException
/// PURPOSE: An application-level exception representing validation failure.
/// LMS ROLE: Supports the Application Exception area while respecting the Application layer boundary.
///
/// IMPLEMENTATION GUIDE:
///   - Use only for exceptional flow represented by this type.
///   - Keep messages safe for logging/presentation policy.
///   - Let API middleware translate it at the boundary.
///
/// NEVER:
///   - Reference ASP.NET status-code constants.
///   - Use exceptions instead of expected BaseResponse&lt;T&gt; failures without a clear policy.
/// </summary>
public class ValidationException : Exception
{
    /// <summary>
    /// Gets or controls the errors value represented by this type. Keep the value contract-focused and avoid leaking framework or persistence details.
    /// </summary>
    public IDictionary<string, string[]> Errors { get; }

    public ValidationException(IDictionary<string, string[]> errors)
        : base("One or more validation failures have occurred.")
    {
        Errors = errors;
    }
}

/// <summary>
/// TYPE: DomainRuleViolationException
/// PURPOSE: An application-level exception representing domain rule violation failure.
/// LMS ROLE: Supports the Application Exception area while respecting the Application layer boundary.
///
/// IMPLEMENTATION GUIDE:
///   - Use only for exceptional flow represented by this type.
///   - Keep messages safe for logging/presentation policy.
///   - Let API middleware translate it at the boundary.
///
/// NEVER:
///   - Reference ASP.NET status-code constants.
///   - Use exceptions instead of expected BaseResponse&lt;T&gt; failures without a clear policy.
/// </summary>
public class DomainRuleViolationException : Exception
{
    public DomainRuleViolationException(string message) : base(message) { }
}
