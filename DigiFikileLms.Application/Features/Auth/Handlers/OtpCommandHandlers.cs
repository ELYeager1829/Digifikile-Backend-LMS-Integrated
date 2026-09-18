using DigiFikileLms.Application.Common;
using DigiFikileLms.Application.DTOs;
using DigiFikileLms.Application.Features.Auth.Commands;
using DigiFikileLms.Application.Interfaces;
using DigiFikileLms.Domain.Enums;
using DigiFikileLms.Domain.Interfaces;
using MediatR;

namespace DigiFikileLms.Application.Features.Auth;

// ================================================================
// LOGIN STEP TWO - VERIFY THE 6-DIGIT OTP AND ISSUE THE JWT
// ================================================================

/// <summary>
/// Exchanges the six-digit PIN for the JWT. The PIN itself is produced, stored and delivered
/// by <see cref="IOtpService"/>; this handler only owns the LMS rules around it.
/// </summary>
public class VerifyLoginOtpCommandHandler : IRequestHandler<VerifyLoginOtpCommand, BaseResponse<AuthResponseDto>>
{
    private readonly IUserAccountRepository _userRepository;
    private readonly IOtpService _otpService;
    private readonly ITokenService _tokenService;

    public VerifyLoginOtpCommandHandler(
        IUserAccountRepository userRepository,
        IOtpService otpService,
        ITokenService tokenService)
    {
        _userRepository = userRepository;
        _otpService = otpService;
        _tokenService = tokenService;
    }

    public async Task<BaseResponse<AuthResponseDto>> Handle(
        VerifyLoginOtpCommand request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Email) || !AuthLoginPolicy.IsValidOtp(request.Otp))
            return BaseResponse<AuthResponseDto>.Failure("A valid e-mail address and the 6-digit code are required");

        var user = await _userRepository.GetByEmailAsync(request.Email.Trim(), cancellationToken);
        if (user == null || !AuthLoginPolicy.SupportsPasswordLogin(user.UserRole) || !AuthLoginPolicy.IsActive(user))
            return InvalidCode;

        if (!await _otpService.VerifyLoginOtpAsync(user.Email, request.Otp.Trim(), cancellationToken))
            return InvalidCode;

        // Reload with the profile navigations so the JWT can still carry the student_id claim.
        var account = await _userRepository.GetWithDetailsAsync(user.Id, cancellationToken) ?? user;

        return BaseResponse<AuthResponseDto>.Success(
            AuthLoginPolicy.Authenticated(account, _tokenService, account.Student?.StudentNumber));
    }

    private static BaseResponse<AuthResponseDto> InvalidCode =>
        BaseResponse<AuthResponseDto>.Failure("Invalid or expired code");
}

// ================================================================
// RESEND THE 6-DIGIT OTP FOR A LOGIN THAT IS ALREADY IN PROGRESS
// ================================================================

/// <summary>
/// Issues a replacement PIN for a pending login without repeating the password step.
/// </summary>
public class ResendLoginOtpCommandHandler : IRequestHandler<ResendLoginOtpCommand, BaseResponse<AuthResponseDto>>
{
    private readonly IUserAccountRepository _userRepository;
    private readonly IOtpService _otpService;

    public ResendLoginOtpCommandHandler(IUserAccountRepository userRepository, IOtpService otpService)
    {
        _userRepository = userRepository;
        _otpService = otpService;
    }

    public async Task<BaseResponse<AuthResponseDto>> Handle(
        ResendLoginOtpCommand request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Email))
            return BaseResponse<AuthResponseDto>.Failure("A valid e-mail address is required");

        var user = await _userRepository.GetByEmailAsync(request.Email.Trim(), cancellationToken);
        if (user == null || !AuthLoginPolicy.SupportsPasswordLogin(user.UserRole) || !AuthLoginPolicy.IsActive(user))
            return BaseResponse<AuthResponseDto>.Failure("Unable to send a new code for this account");

        var expiresInMinutes = await _otpService.SendLoginOtpAsync(user, cancellationToken);

        return BaseResponse<AuthResponseDto>.Success(AuthLoginPolicy.Challenge(user, expiresInMinutes));
    }
}

// ================================================================
// SYSTEM ADMINISTRATOR LOGIN STEP ONE - CREDENTIALS THEN OTP
// ================================================================

/// <summary>
/// First step of the SystemAdministrator login. Only SystemAdministrator accounts are accepted
/// and a six-digit PIN is mailed to the account before any JWT is issued.
/// </summary>
public class SystemAdminLoginCommandHandler : IRequestHandler<SystemAdminLoginCommand, BaseResponse<AuthResponseDto>>
{
    private readonly IUserAccountRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IOtpService _otpService;

    public SystemAdminLoginCommandHandler(
        IUserAccountRepository userRepository,
        IPasswordHasher passwordHasher,
        IOtpService otpService)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _otpService = otpService;
    }

    public async Task<BaseResponse<AuthResponseDto>> Handle(
        SystemAdminLoginCommand request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrEmpty(request.Password))
            return BaseResponse<AuthResponseDto>.Failure("Invalid administrator email or password");

        var user = await _userRepository.GetByEmailAsync(request.Email.Trim(), cancellationToken);
        if (user == null || user.UserRole != UserRole.SystemAdministrator ||
            user.SystemAdministrator?.IsActive == false ||
            !_passwordHasher.VerifyPassword(request.Password, user.Password))
        {
            return BaseResponse<AuthResponseDto>.Failure("Invalid administrator email or password");
        }

        if (_passwordHasher.NeedsRehash(user.Password))
        {
            user.UpdatePassword(_passwordHasher.HashPassword(request.Password));
            await _userRepository.SaveChangesAsync(cancellationToken);
        }

        var expiresInMinutes = await _otpService.SendLoginOtpAsync(user, cancellationToken);

        return BaseResponse<AuthResponseDto>.Success(AuthLoginPolicy.Challenge(user, expiresInMinutes));
    }
}