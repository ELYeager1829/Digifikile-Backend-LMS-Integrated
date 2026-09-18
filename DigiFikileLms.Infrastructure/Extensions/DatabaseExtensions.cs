using DigiFikileLms.Infrastructure.Persistence.Context;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;


/// <summary>
/// FILE: DigiFikileLms.Infrastructure/Extensions/DatabaseExtensions.cs
/// LAYER: Infrastructure
///
/// WHAT THIS FILE IS:
///   An outer-layer adapter that connects the application to PostgreSQL/EF Core or another technical service such as JWT generation and password hashing.
///   Its immediate area is Database Extensions; read the individual type comments below before extending it.
///
/// WHY THIS LAYER:
///   Infrastructure owns replaceable technology details. It depends on inward-facing Domain/Application interfaces and implements them; inner layers must never reference this project.
///   Dependency direction is Domain &lt;- Application &lt;- Infrastructure / API. Dependencies must point inward.
///
/// HOW TO CODE HERE (for juniors):
///   - Implement Domain repository and Application service interfaces exactly as their contracts describe.
///   - Use EF Core asynchronously and pass CancellationToken to database calls.
///   - Keep PostgreSQL schema, relationships, indexes, conversions, and migrations in Persistence.
///   - Translate technical configuration into strongly configured services through focused dependency-injection extensions.
///   - Keep implementations replaceable and testable; constructors should receive DbContext, options, clocks, or other dependencies.
///   - Apply SRP to separate persistence, authentication, hashing, and startup concerns; DIP is satisfied by registering implementations against inner-layer interfaces.
///
/// DO NOT:
///   - Move business rules into EF configurations, repositories, JWT services, or startup extensions.
///   - Reference controllers, HttpContext, IActionResult, or choose HTTP status codes.
///   - Return Infrastructure-specific models to Application when a Domain entity or Application DTO/contract exists.
///   - Use synchronous database I/O, swallow cancellation, concatenate SQL, log secrets, or store plaintext passwords.
///
/// RELATED FILES:
///   - DigiFikileLms.Domain/Interfaces/** — repository contracts implemented by Persistence/Repositories.
///   - DigiFikileLms.Application/Interfaces/** — technical service contracts implemented by Services.
///   - DigiFikileLms.API/Program.cs — invokes registration and database startup extensions.
///
/// DIGIFIKILE LMS CONTEXT:
///   - This file provides technical capabilities needed to persist and secure courses, users, roles, enrollments, assessments, and progress while preserving the inner layers' independence.
/// </summary>

namespace DigiFikileLms.Infrastructure.Extensions;

/// <summary>
/// TYPE: DatabaseExtensions
/// PURPOSE: The infrastructure component responsible for database extensions.
/// LMS ROLE: Supports the Database Extensions area while respecting the Infrastructure layer boundary.
///
/// IMPLEMENTATION GUIDE:
///   - Keep this type focused on the responsibility stated above.
///   - Follow neighbouring types for naming and dependency direction.
///   - Pass cancellation through asynchronous boundaries.
///
/// NEVER:
///   - Mix responsibilities from another Clean Architecture layer.
///   - Introduce a concrete outward dependency into an inner layer.
/// </summary>
public static class DatabaseExtensions
{
    /// <summary>
    /// Executes the initialise database async operation for this type. Pass the supplied CancellationToken to downstream asynchronous work; do not block the thread.
    /// </summary>
    public static async Task InitialiseDatabaseAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var initialiser = scope.ServiceProvider.GetRequiredService<DbInitializer>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<DbInitializer>>();

        try
        {
            await initialiser.InitialiseAsync();

            if (app.Environment.EnvironmentName == "Development")
                await initialiser.SeedDemoDataAsync("dev-seed-admin-id");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "DigiFikile LMS database initialisation failed.");
        }
    }
}
