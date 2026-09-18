using DigiFikileLms.Application.Common;
using DigiFikileLms.Application.DTOs;
using DigiFikileLms.Application.Features.Auth.Commands;
using DigiFikileLms.Application.Interfaces;
using DigiFikileLms.Domain.Enums;
using DigiFikileLms.Domain.Interfaces;
using MediatR;

namespace DigiFikileLms.Application.Features.Auth;

/// <summary>
/// Shared implementation of login step one for the roles that differ only by the role they accept.
/// Keeps the three handlers at the bottom of this file free of duplication.
/// </summary>
internal static class StaffLoginStep
{
    public static async Task<BaseResponse<AuthResponseDto>> ExecuteAsync(
        UserRole expectedRole,
        string email,
        string password,
        IUserAccountRepository userRepository,
        IPasswordHasher passwordHasher,
        IOtpService otpService,
        CancellationToken cancellationToken)
    {
        var failureMessage = $"Invalid {expectedRole} email or password";

        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrEmpty(password))
            return BaseResponse<AuthResponseDto>.Failure(failureMessage);

        var user = await userRepository.GetByEmailAsync(email.Trim(), cancellationToken);
        if (user == null || user.UserRole != expectedRole || !AuthLoginPolicy.IsActive(user) ||
            !passwordHasher.VerifyPassword(password, user.Password))
        {
            return BaseResponse<AuthResponseDto>.Failure(failureMessage);
        }

        if (passwordHasher.NeedsRehash(user.Password))
        {
            user.UpdatePassword(passwordHasher.HashPassword(password));
            await userRepository.SaveChangesAsync(cancellationToken);
        }

        // No JWT is issued here: every login continues with the six-digit OTP step.
        var expiresInMinutes = await otpService.SendLoginOtpAsync(user, cancellationToken);

        return BaseResponse<AuthResponseDto>.Success(AuthLoginPolicy.Challenge(user, expiresInMinutes));
    }
}

/// <summary>
/// Facilitator login step one. Only Facilitator accounts are accepted.
/// </summary>
public class FacilitatorLoginCommandHandler : IRequestHandler<FacilitatorLoginCommand, BaseResponse<AuthResponseDto>>
{
    private readonly IUserAccountRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IOtpService _otpService;

    public FacilitatorLoginCommandHandler(
        IUserAccountRepository userRepository,
        IPasswordHasher passwordHasher,
        IOtpService otpService)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _otpService = otpService;
    }

    public Task<BaseResponse<AuthResponseDto>> Handle(
        FacilitatorLoginCommand request,
        CancellationToken cancellationToken) =>
        StaffLoginStep.ExecuteAsync(
            UserRole.Facilitator,
            request.Email,
            request.Password,
            _userRepository,
            _passwordHasher,
            _otpService,
            cancellationToken);
}

/// <summary>
/// Moderator login step one. Only Moderator accounts are accepted.
/// </summary>
public class ModeratorLoginCommandHandler : IRequestHandler<ModeratorLoginCommand, BaseResponse<AuthResponseDto>>
{
    private readonly IUserAccountRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IOtpService _otpService;

    public ModeratorLoginCommandHandler(
        IUserAccountRepository userRepository,
        IPasswordHasher passwordHasher,
        IOtpService otpService)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _otpService = otpService;
    }

    public Task<BaseResponse<AuthResponseDto>> Handle(
        ModeratorLoginCommand request,
        CancellationToken cancellationToken) =>
        StaffLoginStep.ExecuteAsync(
            UserRole.Moderator,
            request.Email,
            request.Password,
            _userRepository,
            _passwordHasher,
            _otpService,
            cancellationToken);
}

/// <summary>
/// Training Provider login step one. Only TrainingProvider accounts are accepted.
/// </summary>
public class TrainingProviderLoginCommandHandler : IRequestHandler<TrainingProviderLoginCommand, BaseResponse<AuthResponseDto>>
{
    private readonly IUserAccountRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IOtpService _otpService;

    public TrainingProviderLoginCommandHandler(
        IUserAccountRepository userRepository,
        IPasswordHasher passwordHasher,
        IOtpService otpService)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _otpService = otpService;
    }

    public Task<BaseResponse<AuthResponseDto>> Handle(
        TrainingProviderLoginCommand request,
        CancellationToken cancellationToken) =>
        StaffLoginStep.ExecuteAsync(
            UserRole.TrainingProvider,
            request.Email,
            request.Password,
            _userRepository,
            _passwordHasher,
            _otpService,
            cancellationToken);
}