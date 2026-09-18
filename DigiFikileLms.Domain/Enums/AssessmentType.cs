/// <summary>
/// FILE: DigiFikileLms.Domain/Enums/AssessmentType.cs
/// LAYER: Domain
///
/// WHAT THIS FILE IS:
///   A framework-independent domain source file that defines the LMS's core vocabulary, state, rules, or persistence contracts.
///   Its immediate area is Assessment Type; read the individual type comments below before extending it.
///
/// WHY THIS LAYER:
///   Domain is the centre of Clean Architecture. Everything may depend on these abstractions, but this project must not depend on Application, Infrastructure, API, EF Core, HTTP, or framework-specific packages.
///   Dependency direction is Domain &lt;- Application &lt;- Infrastructure / API. Dependencies must point inward.
///
/// HOW TO CODE HERE (for juniors):
///   - Model business meaning explicitly with entities, value objects, enums, and small repository interfaces.
///   - Keep invariants close to the entity; prefer intention-revealing factory methods such as Create(...) when construction must validate state.
///   - Use private setters or behaviour methods to protect state instead of allowing arbitrary mutation.
///   - Keep repository contracts persistence-agnostic and asynchronous; accept CancellationToken for I/O-shaped operations.
///   - Apply SRP by giving each type one domain responsibility, and DIP by exposing abstractions that outer layers implement.
///   - Use names from the DigiFikile LMS language: Course, Module, Lesson, Enrollment, Assessment, Progress, User, and Role.
///
/// DO NOT:
///   - Reference DbContext, EF Core attributes, SQL, controllers, MediatR, HTTP status codes, JWT libraries, or configuration.
///   - Return API DTOs or BaseResponse&lt;T&gt;; Domain models business truth and does not know how a caller presents it.
///   - Make every setter public merely to satisfy persistence; configure EF Core in Infrastructure instead.
///   - Hide business rules in controllers, handlers, or repository implementations when they belong to the entity.
///
/// RELATED FILES:
///   - DigiFikileLms.Application/Features/** — use cases consume these domain types and contracts.
///   - DigiFikileLms.Infrastructure/Persistence/** — EF Core maps entities and implements repository interfaces.
///   - DigiFikileLms.API/Controllers/** — reaches the domain only through Application use cases.
///
/// DIGIFIKILE LMS CONTEXT:
///   - This file contributes to the stable model shared by course delivery, users and roles, assessments, enrollments, and learner progress.
/// </summary>

namespace DigiFikileLms.Domain.Enums;

/// <summary>
/// TYPE: AssessmentType
/// PURPOSE: The closed domain vocabulary for assessment type.
/// LMS ROLE: Supports the Assessment Type area while respecting the Domain layer boundary.
///
/// IMPLEMENTATION GUIDE:
///   - Add a value only when the business recognises a distinct state.
///   - Use stable, intention-revealing names.
///   - Update mappings, validation, and persistence configuration when values change.
///
/// NEVER:
///   - Use magic strings elsewhere for the same concept.
///   - Attach HTTP or database-specific meaning to enum values.
/// </summary>

public enum AssessmentType
{
    Quiz = 1,
    Assignment = 2,
    Exam = 3,
    Practical = 4
}
