using DigiFikileLms.Application.Common;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;


/// <summary>
/// FILE: DigiFikileLms.Application/DependencyInjection.cs
/// LAYER: Application
///
/// WHAT THIS FILE IS:
///   An application-layer source file that describes a use case, transport-safe DTO, validation rule, mapping, response contract, or abstraction required by a use case.
///   Its immediate area is Dependency Injection; read the individual type comments below before extending it.
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

namespace DigiFikileLms.Application;

/// <summary>
/// TYPE: DependencyInjection
/// PURPOSE: The application component responsible for dependency injection.
/// LMS ROLE: Supports the Dependency Injection area while respecting the Application layer boundary.
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
public static class DependencyInjection
{
    /// <summary>
    /// Executes the add application services operation for this type. Keep the operation focused and preserve this layer's dependency boundary.
    /// </summary>
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(assembly);
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        });

        services.AddValidatorsFromAssembly(assembly);
        services.AddAutoMapper(assembly);

        return services;
    }
}

/// <summary>
/// TYPE: ValidationBehavior
/// PURPOSE: The application component responsible for validation behavior.
/// LMS ROLE: Supports the Dependency Injection area while respecting the Application layer boundary.
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
public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators) =>
        _validators = validators;

    /// <summary>
    /// Executes the handle operation for this type. Pass the supplied CancellationToken to downstream asynchronous work; do not block the thread.
    /// </summary>
    public async Task<TResponse> Handle(
        TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (!_validators.Any()) return await next();

        var context = new ValidationContext<TRequest>(request);
        var validationResults = await Task.WhenAll(
            _validators.Select(v => v.ValidateAsync(context, cancellationToken)));

        var failures = validationResults
            .SelectMany(r => r.Errors)
            .Where(f => f is not null)
            .ToList();

        if (failures.Count != 0)
        {
            var errorDictionary = failures
                .GroupBy(f => f.PropertyName, f => f.ErrorMessage)
                .ToDictionary(g => g.Key, g => g.ToArray());

            throw new Common.ValidationException(errorDictionary);
        }

        return await next();
    }
}
