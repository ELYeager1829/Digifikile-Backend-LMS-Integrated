using Microsoft.AspNetCore.Mvc;


/// <summary>
/// FILE: DigiFikileLms.API/Extensions/ServiceCollectionExtensions.cs
/// LAYER: API
///
/// WHAT THIS FILE IS:
///   An ASP.NET Core delivery-layer source file that accepts HTTP traffic, configures the host, or adapts application outcomes into HTTP responses.
///   Its immediate area is Service Collection Extensions; read the individual type comments below before extending it.
///
/// WHY THIS LAYER:
///   API is the outermost presentation layer. It may call Application and register Infrastructure, but Domain and Application must never depend on ASP.NET Core.
///   Dependency direction is Domain &lt;- Application &lt;- Infrastructure / API. Dependencies must point inward.
///
/// HOW TO CODE HERE (for juniors):
///   - Keep controllers thin: bind and validate transport input, extract trusted claims, send one MediatR request, and translate the result to an HTTP response.
///   - Use Commands for writes and Queries for reads; let handlers own use-case orchestration.
///   - Pass HttpContext.RequestAborted or the action CancellationToken all the way through MediatR.
///   - Return DTOs rather than entities, and use BaseResponse&lt;T&gt; information consistently when selecting status codes.
///   - Put cross-cutting HTTP concerns such as exceptions, correlation IDs, and request logging in middleware.
///   - Keep Program.cs as the composition root: register dependencies and order middleware without implementing business rules.
///   - Apply SRP to each controller/middleware component and DIP by consuming MediatR or inner-layer abstractions.
///
/// DO NOT:
///   - Write business rules, EF queries, SQL, password hashing, JWT construction, or repository orchestration in controllers.
///   - Return Domain entities directly over the wire or leak stack traces and secrets to clients.
///   - Put HTTP concepts such as StatusCodes, claims, headers, or IActionResult in Domain or Application.
///   - Block on async work with .Result/.Wait(), drop CancellationToken, or duplicate FluentValidation rules in actions.
///
/// RELATED FILES:
///   - DigiFikileLms.Application/Features/** — commands, queries, and handlers invoked through MediatR.
///   - DigiFikileLms.Application/DTOs/** — response shapes safe to expose to clients.
///   - DigiFikileLms.Infrastructure/Extensions/** — outer-layer services registered by the composition root.
///
/// DIGIFIKILE LMS CONTEXT:
///   - This file exposes or supports the HTTP surface used to manage LMS authentication, users, roles, courses, content, enrollment, assessment, progress, and reports.
/// </summary>

namespace DigiFikileLms.API.Extensions;

/// <summary>
/// TYPE: ServiceCollectionExtensions
/// PURPOSE: The api component responsible for service collection extensions.
/// LMS ROLE: Supports the Service Collection Extensions area while respecting the API layer boundary.
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
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Executes the add api services operation for this type. Keep the operation focused and preserve this layer's dependency boundary.
    /// </summary>
    public static IServiceCollection AddApiServices(
        this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddControllers()
            .ConfigureApiBehaviorOptions(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
            });

        services.AddProblemDetails();
        services.AddEndpointsApiExplorer();
        services.AddHealthChecks();

        services.AddCors(options =>
        {
            options.AddPolicy("DigiFikileLmsCors", policy =>
            {
                var allowedOrigins = configuration.GetSection("Cors:AllowedOrigins")
                    .Get<string[]>() ?? [];

                if (allowedOrigins.Length > 0)
                    policy.WithOrigins(allowedOrigins).AllowAnyHeader().AllowAnyMethod();
                else
                    policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
            });
        });

        return services;
    }
}
