//using DigiFikileLms.Application.Features.Assessments.Commands;
using DigiFikileLms.Application.Features.Auth.Commands;
using DigiFikileLms.Application.Features.Content.Commands;
using DigiFikileLms.Application.Features.Courses.Commands;
using DigiFikileLms.Application.Features.Roles.Commands;
using DigiFikileLms.Application.Features.Users.Commands;
using FluentValidation;


/// <summary>
/// FILE: DigiFikileLms.Application/Validators/LmsValidators.cs
/// LAYER: Application
///
/// WHAT THIS FILE IS:
///   An application-layer source file that describes a use case, transport-safe DTO, validation rule, mapping, response contract, or abstraction required by a use case.
///   Its immediate area is Lms; read the individual type comments below before extending it.
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

namespace DigiFikileLms.Application.Validators;

/// <summary>
/// TYPE: RegisterUserCommandValidator
/// PURPOSE: A FluentValidation boundary validator for Register User Command input.
/// LMS ROLE: Supports the Lms area while respecting the Application layer boundary.
///
/// IMPLEMENTATION GUIDE:
///   - Validate required shape, length, range, and format.
///   - Write deterministic rules and useful messages.
///   - Leave database-dependent business invariants to the handler/domain.
///
/// NEVER:
///   - Perform writes.
///   - Duplicate domain invariants or controller checks.
/// </summary>
/// 
/// 
/// 
/// 
/// 
/// 
/**
public class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
{
    public RegisterUserCommandValidator()
    {
        // TODO: RuleFor(x => x.Email).NotEmpty().EmailAddress();
        // TODO: RuleFor(x => x.Password).MinimumLength(8);
    }
}

/// <summary>
/// TYPE: CreateUserCommandValidator
/// PURPOSE: A FluentValidation boundary validator for Create User Command input.
/// LMS ROLE: Supports the Lms area while respecting the Application layer boundary.
///
/// IMPLEMENTATION GUIDE:
///   - Validate required shape, length, range, and format.
///   - Write deterministic rules and useful messages.
///   - Leave database-dependent business invariants to the handler/domain.
///
/// NEVER:
///   - Perform writes.
///   - Duplicate domain invariants or controller checks.
/// </summary>
public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserCommandValidator()
    {
        // TODO: RuleFor(x => x.Email).NotEmpty().EmailAddress();
    }
}

/// <summary>
/// TYPE: CreateCourseCommandValidator
/// PURPOSE: A FluentValidation boundary validator for Create Course Command input.
/// LMS ROLE: Supports the Lms area while respecting the Application layer boundary.
///
/// IMPLEMENTATION GUIDE:
///   - Validate required shape, length, range, and format.
///   - Write deterministic rules and useful messages.
///   - Leave database-dependent business invariants to the handler/domain.
///
/// NEVER:
///   - Perform writes.
///   - Duplicate domain invariants or controller checks.
/// </summary>
public class CreateCourseCommandValidator : AbstractValidator<CreateCourseCommand>
{
    public CreateCourseCommandValidator()
    {
        // TODO: RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
    }
}

/// <summary>
/// TYPE: CreateAssessmentCommandValidator
/// PURPOSE: A FluentValidation boundary validator for Create Assessment Command input.
/// LMS ROLE: Supports the Lms area while respecting the Application layer boundary.
///
/// IMPLEMENTATION GUIDE:
///   - Validate required shape, length, range, and format.
///   - Write deterministic rules and useful messages.
///   - Leave database-dependent business invariants to the handler/domain.
///
/// NEVER:
///   - Perform writes.
///   - Duplicate domain invariants or controller checks.
/// </summary>
public class CreateAssessmentCommandValidator : AbstractValidator<CreateAssessmentCommand>
{
    public CreateAssessmentCommandValidator()
    {
        // TODO: RuleFor(x => x.MaxScore).GreaterThan(0);
    }
}

/// <summary>
/// TYPE: CreateModuleCommandValidator
/// PURPOSE: A FluentValidation boundary validator for Create Module Command input.
/// LMS ROLE: Supports the Lms area while respecting the Application layer boundary.
///
/// IMPLEMENTATION GUIDE:
///   - Validate required shape, length, range, and format.
///   - Write deterministic rules and useful messages.
///   - Leave database-dependent business invariants to the handler/domain.
///
/// NEVER:
///   - Perform writes.
///   - Duplicate domain invariants or controller checks.
/// </summary>
public class CreateModuleCommandValidator : AbstractValidator<CreateModuleCommand>
{
    public CreateModuleCommandValidator()
    {
        // TODO: RuleFor(x => x.Title).NotEmpty();
    }
}

/// <summary>
/// TYPE: CreateLessonCommandValidator
/// PURPOSE: A FluentValidation boundary validator for Create Lesson Command input.
/// LMS ROLE: Supports the Lms area while respecting the Application layer boundary.
///
/// IMPLEMENTATION GUIDE:
///   - Validate required shape, length, range, and format.
///   - Write deterministic rules and useful messages.
///   - Leave database-dependent business invariants to the handler/domain.
///
/// NEVER:
///   - Perform writes.
///   - Duplicate domain invariants or controller checks.
/// </summary>
public class CreateLessonCommandValidator : AbstractValidator<CreateLessonCommand>
{
    public CreateLessonCommandValidator()
    {
        // TODO: RuleFor(x => x.Title).NotEmpty();
    }
}

/// <summary>
/// TYPE: CreateRoleCommandValidator
/// PURPOSE: A FluentValidation boundary validator for Create Role Command input.
/// LMS ROLE: Supports the Lms area while respecting the Application layer boundary.
///
/// IMPLEMENTATION GUIDE:
///   - Validate required shape, length, range, and format.
///   - Write deterministic rules and useful messages.
///   - Leave database-dependent business invariants to the handler/domain.
///
/// NEVER:
///   - Perform writes.
///   - Duplicate domain invariants or controller checks.
/// </summary>
public class CreateRoleCommandValidator : AbstractValidator<CreateRoleCommand>
{
    public CreateRoleCommandValidator()
    {
        // TODO: RuleFor(x => x.Name).NotEmpty();
    }
}
**/