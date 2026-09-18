using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;


/// <summary>
/// FILE: DigiFikileLms.Infrastructure/Persistence/Context/DbInitializer.cs
/// LAYER: Infrastructure
///
/// WHAT THIS FILE IS:
///   An outer-layer adapter that connects the application to PostgreSQL/EF Core or another technical service such as JWT generation and password hashing.
///   Its immediate area is Db Initializer; read the individual type comments below before extending it.
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

namespace DigiFikileLms.Infrastructure.Persistence.Context;

/// <summary>
/// TYPE: DbInitializer
/// PURPOSE: The infrastructure component responsible for db initializer.
/// LMS ROLE: Supports the Db Initializer area while respecting the Infrastructure layer boundary.
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
public class DbInitializer
{
    private readonly DigiFikileLmsDbContext _context;
    private readonly ILogger<DbInitializer> _logger;

    public DbInitializer(DigiFikileLmsDbContext context, ILogger<DbInitializer> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Executes the initialise async operation for this type. Pass the supplied CancellationToken to downstream asynchronous work; do not block the thread.
    /// </summary>
    public async Task InitialiseAsync()
    {
        await _context.Database.MigrateAsync();
        _logger.LogInformation("DigiFikile LMS database migrations applied successfully.");
    }

    /// <summary>
    /// Executes the seed demo data async operation for this type. Pass the supplied CancellationToken to downstream asynchronous work; do not block the thread.
    /// </summary>
    public async Task SeedDemoDataAsync(string demoAdminId)
    {
        if (await _context.UserAccounts.AnyAsync()) return;

        _logger.LogInformation("Seeding DigiFikile LMS reference data.");

        await SeedDefaultRolesAsync();
        await SeedAdminUserAsync(demoAdminId);
        await SeedSampleCourseAsync();

        _logger.LogInformation("DigiFikile LMS seed data completed.");
    }

    private Task SeedDefaultRolesAsync() =>
        throw new NotImplementedException("TODO: Seed Admin, Instructor, Learner roles from UserRoleType enum.");

    private Task SeedAdminUserAsync(string demoAdminId) =>
        throw new NotImplementedException("TODO: Create default admin user with hashed password.");

    private Task SeedSampleCourseAsync() =>
        throw new NotImplementedException("TODO: Create sample course with module and lesson for onboarding.");
}
