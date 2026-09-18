/// <summary>
/// FILE: DigiFikileLms.Application/DTOs/LmsDtos.cs
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

namespace DigiFikileLms.Application.DTOs;

/// <summary>
/// TYPE: UserDto
/// PURPOSE: A transport-safe DTO representing user data outside the Domain.
/// LMS ROLE: Supports the Lms area while respecting the Application layer boundary.
///
/// IMPLEMENTATION GUIDE:
///   - Keep it immutable where practical.
///   - Include only data the caller needs.
///   - Map from entities in an AutoMapper profile.
///   - Treat changes as API-contract changes.
///
/// NEVER:
///   - Add business behaviour or persistence annotations.
///   - Expose secrets, password hashes, or EF navigation graphs.
/// </summary>
///deleted UserDto record here

/// <summary>
/// TYPE: RoleDto
/// PURPOSE: A transport-safe DTO representing role data outside the Domain.
/// LMS ROLE: Supports the Lms area while respecting the Application layer boundary.
///
/// IMPLEMENTATION GUIDE:
///   - Keep it immutable where practical.
///   - Include only data the caller needs.
///   - Map from entities in an AutoMapper profile.
///   - Treat changes as API-contract changes.
///
/// NEVER:
///   - Add business behaviour or persistence annotations.
///   - Expose secrets, password hashes, or EF navigation graphs.
/// </summary>
//public record RoleDto(int Id, string Name, string? Description);

/// <summary>
/// TYPE: CourseDto
/// PURPOSE: A transport-safe DTO representing course data outside the Domain.
/// LMS ROLE: Supports the Lms area while respecting the Application layer boundary.
///
/// IMPLEMENTATION GUIDE:
///   - Keep it immutable where practical.
///   - Include only data the caller needs.
///   - Map from entities in an AutoMapper profile.
///   - Treat changes as API-contract changes.
///
/// NEVER:
///   - Add business behaviour or persistence annotations.
///   - Expose secrets, password hashes, or EF navigation graphs.
/// </summary>

///comment this out due to ambiguity with CourseDto in DigiFikileLms.Application/DTOs/Courses/CourseDto.cs
//public record CourseDto(
//int Id,
//string Title,
//string Description,
//int InstructorId,
//string Status);

/// <summary>
/// TYPE: ModuleDto
/// PURPOSE: A transport-safe DTO representing module data outside the Domain.
/// LMS ROLE: Supports the Lms area while respecting the Application layer boundary.
///
/// IMPLEMENTATION GUIDE:
///   - Keep it immutable where practical.
///   - Include only data the caller needs.
///   - Map from entities in an AutoMapper profile.
///   - Treat changes as API-contract changes.
///
/// NEVER:
///   - Add business behaviour or persistence annotations.
///   - Expose secrets, password hashes, or EF navigation graphs.
/// </summary>
public record ModuleDto(int Id, int CourseId, string Title, int OrderIndex);

/// <summary>
/// TYPE: LessonDto
/// PURPOSE: A transport-safe DTO representing lesson data outside the Domain.
/// LMS ROLE: Supports the Lms area while respecting the Application layer boundary.
///
/// IMPLEMENTATION GUIDE:
///   - Keep it immutable where practical.
///   - Include only data the caller needs.
///   - Map from entities in an AutoMapper profile.
///   - Treat changes as API-contract changes.
///
/// NEVER:
///   - Add business behaviour or persistence annotations.
///   - Expose secrets, password hashes, or EF navigation graphs.
/// </summary>
public record LessonDto(
    int Id,
    int ModuleId,
    string Title,
    string ContentType,
    string? ContentUrl,
    int OrderIndex,
    int DurationMinutes);

/// <summary>
/// TYPE: EnrollmentDto
/// PURPOSE: A transport-safe DTO representing enrollment data outside the Domain.
/// LMS ROLE: Supports the Lms area while respecting the Application layer boundary.
///
/// IMPLEMENTATION GUIDE:
///   - Keep it immutable where practical.
///   - Include only data the caller needs.
///   - Map from entities in an AutoMapper profile.
///   - Treat changes as API-contract changes.
///
/// NEVER:
///   - Add business behaviour or persistence annotations.
///   - Expose secrets, password hashes, or EF navigation graphs.
/// </summary>
public record EnrollmentDto(
    int Id,
    int CourseId,
    int UserId,
    string Status,
    DateTime EnrolledAt);

/// <summary>
/// TYPE: AssessmentDto
/// PURPOSE: A transport-safe DTO representing assessment data outside the Domain.
/// LMS ROLE: Supports the Lms area while respecting the Application layer boundary.
///
/// IMPLEMENTATION GUIDE:
///   - Keep it immutable where practical.
///   - Include only data the caller needs.
///   - Map from entities in an AutoMapper profile.
///   - Treat changes as API-contract changes.
///
/// NEVER:
///   - Add business behaviour or persistence annotations.
///   - Expose secrets, password hashes, or EF navigation graphs.
/// </summary>
public record AssessmentDto(
    int Id,
    int CourseId,
    string Title,
    string Type,
    decimal MaxScore,
    decimal PassingScore,
    DateTime? DueDate);

/// <summary>
/// TYPE: AssessmentAttemptDto
/// PURPOSE: A transport-safe DTO representing assessment attempt data outside the Domain.
/// LMS ROLE: Supports the Lms area while respecting the Application layer boundary.
///
/// IMPLEMENTATION GUIDE:
///   - Keep it immutable where practical.
///   - Include only data the caller needs.
///   - Map from entities in an AutoMapper profile.
///   - Treat changes as API-contract changes.
///
/// NEVER:
///   - Add business behaviour or persistence annotations.
///   - Expose secrets, password hashes, or EF navigation graphs.
/// </summary>
public record AssessmentAttemptDto(
    int Id,
    int AssessmentId,
    int UserId,
    DateTime StartedAt,
    DateTime? SubmittedAt,
    decimal? Score);

/// <summary>
/// TYPE: ProgressDto
/// PURPOSE: A transport-safe DTO representing progress data outside the Domain.
/// LMS ROLE: Supports the Lms area while respecting the Application layer boundary.
///
/// IMPLEMENTATION GUIDE:
///   - Keep it immutable where practical.
///   - Include only data the caller needs.
///   - Map from entities in an AutoMapper profile.
///   - Treat changes as API-contract changes.
///
/// NEVER:
///   - Add business behaviour or persistence annotations.
///   - Expose secrets, password hashes, or EF navigation graphs.
/// </summary>
public record ProgressDto(
    int Id,
    int UserId,
    int LessonId,
    decimal PercentComplete,
    string Status,
    bool IsCompleted,
    DateTime LastAccessedAt);

/// <summary>
/// TYPE: AuthTokenDto
/// PURPOSE: A transport-safe DTO representing auth token data outside the Domain.
/// LMS ROLE: Supports the Lms area while respecting the Application layer boundary.
///
/// IMPLEMENTATION GUIDE:
///   - Keep it immutable where practical.
///   - Include only data the caller needs.
///   - Map from entities in an AutoMapper profile.
///   - Treat changes as API-contract changes.
///
/// NEVER:
///   - Add business behaviour or persistence annotations.
///   - Expose secrets, password hashes, or EF navigation graphs.
/// </summary>
public record AuthTokenDto(string AccessToken, DateTime ExpiresAt);

/// <summary>
/// TYPE: ReportSummaryDto
/// PURPOSE: A transport-safe DTO representing report summary data outside the Domain.
/// LMS ROLE: Supports the Lms area while respecting the Application layer boundary.
///
/// IMPLEMENTATION GUIDE:
///   - Keep it immutable where practical.
///   - Include only data the caller needs.
///   - Map from entities in an AutoMapper profile.
///   - Treat changes as API-contract changes.
///
/// NEVER:
///   - Add business behaviour or persistence annotations.
///   - Expose secrets, password hashes, or EF navigation graphs.
/// </summary>
public record ReportSummaryDto(
    int TotalUsers,
    int TotalCourses,
    int ActiveEnrollments,
    int CompletedAssessments);
