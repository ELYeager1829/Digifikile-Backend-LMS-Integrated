using System.Diagnostics;


/// <summary>
/// FILE: DigiFikileLms.API/Middleware/RequestLoggingMiddleware.cs
/// LAYER: API
///
/// WHAT THIS FILE IS:
///   An ASP.NET Core delivery-layer source file that accepts HTTP traffic, configures the host, or adapts application outcomes into HTTP responses.
///   Its immediate area is Request Logging Middleware; read the individual type comments below before extending it.
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

namespace DigiFikileLms.API.Middleware;

/// <summary>
/// TYPE: RequestLoggingMiddleware
/// PURPOSE: ASP.NET Core middleware responsible only for request logging.
/// LMS ROLE: Supports the Request Logging Middleware area while respecting the API layer boundary.
///
/// IMPLEMENTATION GUIDE:
///   - Perform work before and/or after calling the next delegate.
///   - Keep the pipeline non-blocking.
///   - Use scoped services correctly and avoid leaking sensitive request data.
///
/// NEVER:
///   - Implement use cases.
///   - Forget to invoke the next middleware unless intentionally terminating the request.
/// </summary>
public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    /// <summary>
    /// Executes the invoke async operation for this type. Pass the supplied CancellationToken to downstream asynchronous work; do not block the thread.
    /// </summary>
    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();
        var correlationId = context.Request.Headers["X-Correlation-Id"].FirstOrDefault() ?? "none";

        _logger.LogInformation("→ {Method} {Path} [{CorrelationId}]",
            context.Request.Method, context.Request.Path, correlationId);

        try
        {
            await _next(context);
        }
        finally
        {
            stopwatch.Stop();
            var elapsed = stopwatch.ElapsedMilliseconds;

            if (elapsed > 2000)
                _logger.LogWarning("SLOW {Method} {Path} → {StatusCode} in {Elapsed}ms [{CorrelationId}]",
                    context.Request.Method, context.Request.Path,
                    context.Response.StatusCode, elapsed, correlationId);
            else
                _logger.LogInformation("← {Method} {Path} → {StatusCode} in {Elapsed}ms",
                    context.Request.Method, context.Request.Path,
                    context.Response.StatusCode, elapsed);
        }
    }
}
