using DigiFikileLms.Application.Common;
using DigiFikileLms.Application.DTOs;
using DigiFikileLms.Application.DTOs.Auth;
using DigiFikileLms.Application.Interfaces;
using DigiFikileLms.Domain.Entities;
using DigiFikileLms.Domain.Enums;
using DigiFikileLms.Domain.Interfaces;

using MediatR;


/// <summary>
/// FILE: DigiFikileLms.Application/Features/Auth/Commands/AuthCommands.cs
/// LAYER: Application
///
/// WHAT THIS FILE IS:
///   An application-layer source file that describes a use case, transport-safe DTO, validation rule, mapping, response contract, or abstraction required by a use case.
///   Its immediate area is Auth; read the individual type comments below before extending it.
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

namespace DigiFikileLms.Application.Features.Auth.Commands;


/// <summary>
/// TYPE: RegisterUserCommand
/// PURPOSE: An immutable CQRS write request for register user.
/// LMS ROLE: Supports the Auth area while respecting the Application layer boundary.
///
/// IMPLEMENTATION GUIDE:
///   - Keep the record limited to data needed by this write.
///   - Use clear scalar or request-model values.
///   - Add validation in Validators and behaviour in its handler/domain entity.
///
/// NEVER:
///   - Perform work inside the record.
///   - Use a Command for a read-only operation.
/// </summary>
public record RegisterCommand(
    string Name,
    string Surname,
    string Email,
    string Password,
    string UserRole
) : IRequest<BaseResponse<AuthResponseDto>>;


public record StudentLoginCommand(
    string StudentNumber,
    string Password
) : IRequest<BaseResponse<AuthResponseDto>>;


public record AdminLoginCommand(
    string Email,
    string Password
) : IRequest<BaseResponse<AuthResponseDto>>;

public record RegisterStudentCommand(
    string Name,
    string Surname,
    string Email,
    string Password,
    string StudentNumber,
    string? Phone,
    string? Address
) : IRequest<BaseResponse<AuthResponseDto>>;

public class RegisterStudentCommandHandler : IRequestHandler<RegisterStudentCommand, BaseResponse<AuthResponseDto>>
{
    private readonly IUserAccountRepository _userRepository;
    private readonly IStudentRepository _studentRepository;
    private readonly IPasswordHasher _passwordHasher;

    public RegisterStudentCommandHandler(
        IUserAccountRepository userRepository,
        IStudentRepository studentRepository,
        IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _studentRepository = studentRepository;
        _passwordHasher = passwordHasher;
    }

    public async Task<BaseResponse<AuthResponseDto>> Handle(
        RegisterStudentCommand request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Name) || string.IsNullOrWhiteSpace(request.Surname) ||
            string.IsNullOrWhiteSpace(request.Email) || !System.Net.Mail.MailAddress.TryCreate(request.Email.Trim(), out _) ||
            string.IsNullOrWhiteSpace(request.StudentNumber) || string.IsNullOrEmpty(request.Password) || request.Password.Length < 8)
            return BaseResponse<AuthResponseDto>.Failure("Name, surname, valid email, student number and password of at least 8 characters are required");

        if (await _userRepository.ExistsByEmailAsync(request.Email.Trim(), cancellationToken))
            return BaseResponse<AuthResponseDto>.Failure("Email already registered");

        if (await _studentRepository.GetByStudentNumberAsync(request.StudentNumber.Trim(), cancellationToken) != null)
            return BaseResponse<AuthResponseDto>.Failure("Student number already registered");

        var user = UserAccount.Create(
            request.Name,
            request.Surname,
            request.Email.Trim(),
            _passwordHasher.HashPassword(request.Password),
            UserRole.Student,
            request.Phone,
            request.Address);

        await _userRepository.AddAsync(user, cancellationToken);
        // Both tracked entities are persisted by one SaveChanges transaction.
        var student = Student.Create(user, request.StudentNumber.Trim());
        await _studentRepository.AddAsync(student, cancellationToken);
        await _studentRepository.SaveChangesAsync(cancellationToken);

        return BaseResponse<AuthResponseDto>.Success(new AuthResponseDto
        {
            UserId = user.Id,
            Email = user.Email,
            Name = user.Name,
            Role = user.UserRole.ToString(),
            StudentNumber = student.StudentNumber
        });
    }
}

public class StudentLoginCommandHandler : IRequestHandler<StudentLoginCommand, BaseResponse<AuthResponseDto>>
{
    private readonly IStudentRepository _studentRepository;
    private readonly IPasswordHasher _passwordHasher;

    private readonly IOtpService _otpService;

    public StudentLoginCommandHandler(IStudentRepository studentRepository, IPasswordHasher passwordHasher, IOtpService otpService)
    {
        _otpService = otpService;
        _studentRepository = studentRepository;
        _passwordHasher = passwordHasher;
    }

    public async Task<BaseResponse<AuthResponseDto>> Handle(
        StudentLoginCommand request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.StudentNumber) || string.IsNullOrEmpty(request.Password))
            return BaseResponse<AuthResponseDto>.Failure("Invalid student number or password");
        var student = await _studentRepository.GetByStudentNumberAsync(request.StudentNumber.Trim(), cancellationToken);
        if (student?.User == null || student.User.UserRole != UserRole.Student ||
            !_passwordHasher.VerifyPassword(request.Password, student.User.Password))
        {
            return BaseResponse<AuthResponseDto>.Failure("Invalid student number or password");
        }

        if (_passwordHasher.NeedsRehash(student.User.Password))
        {
            student.User.UpdatePassword(_passwordHasher.HashPassword(request.Password));
            await _studentRepository.SaveChangesAsync(cancellationToken);
        }
        // Every login continues with the 6-digit OTP step, so no JWT is issued here.
        var expiresInMinutes = await _otpService.SendLoginOtpAsync(student.User, cancellationToken);

        return BaseResponse<AuthResponseDto>.Success(
            AuthLoginPolicy.Challenge(student.User, expiresInMinutes, student.StudentNumber));
    }
}

public class AdminLoginCommandHandler : IRequestHandler<AdminLoginCommand, BaseResponse<AuthResponseDto>>
{
    private readonly IUserAccountRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;

    private readonly IOtpService _otpService;

    public AdminLoginCommandHandler(IUserAccountRepository userRepository, IPasswordHasher passwordHasher, IOtpService otpService)
    {
        _otpService = otpService;
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
    }

    public async Task<BaseResponse<AuthResponseDto>> Handle(
        AdminLoginCommand request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrEmpty(request.Password))
            return BaseResponse<AuthResponseDto>.Failure("Invalid administrator email or password");
        var user = await _userRepository.GetByEmailAsync(request.Email.Trim(), cancellationToken);
        if (user == null || user.UserRole != UserRole.SetaAdministrator ||
            user.SetaAdministrator?.IsActive == false ||
            !_passwordHasher.VerifyPassword(request.Password, user.Password))
        {
            return BaseResponse<AuthResponseDto>.Failure("Invalid administrator email or password");
        }

        if (_passwordHasher.NeedsRehash(user.Password))
        {
            user.UpdatePassword(_passwordHasher.HashPassword(request.Password));
            await _userRepository.SaveChangesAsync(cancellationToken);
        }
        // Every login continues with the 6-digit OTP step, so no JWT is issued here.
        var expiresInMinutes = await _otpService.SendLoginOtpAsync(user, cancellationToken);

        return BaseResponse<AuthResponseDto>.Success(AuthLoginPolicy.Challenge(user, expiresInMinutes));
    }
}


public record ResetPasswordCommand(
    string Email
) : IRequest<BaseResponse<bool>>;


public record ChangePasswordCommand(
    int UserId,
    string CurrentPassword,
    string NewPassword
) : IRequest<BaseResponse<bool>>;

public record RefreshTokenCommand(
    string RefreshToken
) : IRequest<BaseResponse<AuthResponseDto>>;

/// <summary>
/// TYPE: RegisterUserCommandHandler
/// PURPOSE: The MediatR handler that orchestrates the Register User Command use case.
/// LMS ROLE: Supports the Auth area while respecting the Application layer boundary.
///
/// IMPLEMENTATION GUIDE:
///   - Receive already validated request data.
///   - Load or change state through interfaces.
///   - Invoke domain behaviour rather than setting protected state.
///   - Map output to a DTO and return BaseResponse&lt;T&gt;.
///   - Forward cancellationToken to every asynchronous dependency.
///
/// NEVER:
///   - Use HttpContext or select HTTP status codes.
///   - Call concrete EF Core, JWT, or hashing types.
/// </summary>
////
///public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, BaseResponse<UserDto>>
//{
    /// <summary>
    /// Executes the handle operation for this type. Pass the supplied CancellationToken to downstream asynchronous work; do not block the thread.
    /// </summary>
 ///   public Task<BaseResponse<UserDto>> Handle(RegisterUserCommand request, CancellationToken cancellationToken) =>
     ///   throw new NotImplementedException("TODO: Hash password, create user, assign Learner role, return UserDto.");
//}

/// <summary>
/// TYPE: LoginCommand
/// PURPOSE: An immutable CQRS write request for login.
/// LMS ROLE: Supports the Auth area while respecting the Application layer boundary.
///
/// IMPLEMENTATION GUIDE:
///   - Keep the record limited to data needed by this write.
///   - Use clear scalar or request-model values.
///   - Add validation in Validators and behaviour in its handler/domain entity.
///
/// NEVER:
///   - Perform work inside the record.
///   - Use a Command for a read-only operation.
/// </summary>
///public record LoginCommand(string Email, string Password) : IRequest<BaseResponse<AuthTokenDto>>;

/// <summary>
/// TYPE: LoginCommandHandler
/// PURPOSE: The MediatR handler that orchestrates the Login Command use case.
/// LMS ROLE: Supports the Auth area while respecting the Application layer boundary.
///
/// IMPLEMENTATION GUIDE:
///   - Receive already validated request data.
///   - Load or change state through interfaces.
///   - Invoke domain behaviour rather than setting protected state.
///   - Map output to a DTO and return BaseResponse&lt;T&gt;.
///   - Forward cancellationToken to every asynchronous dependency.
///
/// NEVER:
///   - Use HttpContext or select HTTP status codes.
///   - Call concrete EF Core, JWT, or hashing types.
/// </summary>
///public class LoginCommandHandler : IRequestHandler<LoginCommand, BaseResponse<AuthTokenDto>>
///{
    /// <summary>
    /// Executes the handle operation for this type. Pass the supplied CancellationToken to downstream asynchronous work; do not block the thread.
    /// </summary>
///    public Task<BaseResponse<AuthTokenDto>> Handle(LoginCommand request, CancellationToken cancellationToken) =>
   ///     throw new NotImplementedException("TODO: Verify credentials and return JWT via ITokenService.");
//}
