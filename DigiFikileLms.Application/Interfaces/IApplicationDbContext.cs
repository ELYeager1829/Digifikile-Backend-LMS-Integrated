using DigiFikileLms.Domain.Entities;
using Microsoft.EntityFrameworkCore;


/// <summary>
/// FILE: DigiFikileLms.Application/Interfaces/IApplicationDbContext.cs
/// LAYER: Application
///
/// WHAT THIS FILE IS:
///   An application-layer source file that describes a use case, transport-safe DTO, validation rule, mapping, response contract, or abstraction required by a use case.
///   Its immediate area is I Application Db Context; read the individual type comments below before extending it.
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

namespace DigiFikileLms.Application.Interfaces;

/// <summary>
/// TYPE: IApplicationDbContext
/// PURPOSE: The EF Core unit-of-work and model gateway for DigiFikile LMS persistence.
/// LMS ROLE: Supports the I Application Db Context area while respecting the Application layer boundary.
///
/// IMPLEMENTATION GUIDE:
///   - Expose DbSet properties for aggregate persistence.
///   - Apply configurations from the Infrastructure assembly.
///   - Populate technical audit fields without deciding business policy.
///   - Use migrations for schema changes.
///
/// NEVER:
///   - Serve as an Application dependency by concrete type.
///   - Contain controller or business workflow logic.
/// </summary>
public interface IApplicationDbContext
{
    DbSet<Role> Roles { get; }
    DbSet<Permission> Permissions { get; }
    DbSet<UserAccount> UserAccounts { get; }
    DbSet<Student> Students { get; }
    DbSet<Facilitator> Facilitators { get; }
    DbSet<Moderator> Moderators { get; }
    DbSet<SetaAdministrator> SetaAdministrators { get; }
    DbSet<TrainingProvider> TrainingProviders { get; }
    DbSet<Department> Departments { get; }
    DbSet<SETAProgramme> SETAProgrammes { get; }
    DbSet<Course> Courses { get; }
    DbSet<Module> Modules { get; }
    DbSet<Enrollment> Enrollments { get; }
    DbSet<Assessment> Assessments { get; }
    DbSet<Submission> Submissions { get; }
    DbSet<Result> Results { get; }
    DbSet<Feedback> Feedbacks { get; }
    DbSet<Certificate> Certificates { get; }
    DbSet<Progress> ProgressRecords { get; }
    DbSet<Notification> Notifications { get; }
    DbSet<Report> Reports { get; }
    DbSet<Complaint> Complaints { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
