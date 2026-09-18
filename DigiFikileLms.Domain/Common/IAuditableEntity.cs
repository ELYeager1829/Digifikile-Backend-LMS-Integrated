/// <summary>
/// FILE: DigiFikileLms.Domain/Common/IAuditableEntity.cs
/// LAYER: Domain
///
/// WHAT THIS FILE IS:
///   A framework-independent domain source file that defines the LMS's core vocabulary, state, rules, or persistence contracts.
///   Its immediate area is I Auditable Entity; read the individual type comments below before extending it.
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

namespace DigiFikileLms.Domain.Common;

/// <summary>
/// TYPE: IAuditableEntity
/// PURPOSE: An inward-facing abstraction for i auditable entity.
/// LMS ROLE: Supports the I Auditable Entity area while respecting the Domain layer boundary.
///
/// IMPLEMENTATION GUIDE:
///   - Keep the contract small and consumer-focused.
///   - Model asynchronous I/O with Task and CancellationToken.
///   - Register an outer-layer implementation through dependency injection.
///
/// NEVER:
///   - Leak concrete vendor/framework types.
///   - Add unrelated methods that violate interface segregation.
/// </summary>
public interface IAuditableEntity
{
    DateTime CreatedAt { get; set; }
    DateTime? UpdatedAt { get; set; }
    string? CreatedBy { get; set; }
    string? UpdatedBy { get; set; }
}
