using DigiFikileLms.Application.Common;
using DigiFikileLms.Application.DTOs.Auth;
using DigiFikileLms.Application.Features.Auth.Commands;
using DigiFikileLms.Application.Interfaces;
using DigiFikileLms.Domain.Interfaces;
using MediatR;

namespace DigiFikileLms.Application.Features.Auth;

public class ForgotPasswordCommandHandler
    : IRequestHandler<ForgotPasswordCommand, BaseResponse<PasswordResetChallengeDto>>
{
    private const int DefaultExpiryMinutes = 10;
    private readonly IUserAccountRepository _userRepository;
    private readonly IPasswordResetService _passwordResetService;

    public ForgotPasswordCommandHandler(
        IUserAccountRepository userRepository,
        IPasswordResetService passwordResetService)
    {
        _userRepository = userRepository;
        _passwordResetService = passwordResetService;
    }

    public async Task<BaseResponse<PasswordResetChallengeDto>> Handle(
        ForgotPasswordCommand request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Email) ||
            !System.Net.Mail.MailAddress.TryCreate(request.Email.Trim(), out _))
        {
            return BaseResponse<PasswordResetChallengeDto>.Failure("A valid e-mail address is required");
        }

        var email = request.Email.Trim();
        var expiresInMinutes = DefaultExpiryMinutes;
        var user = await _userRepository.GetByEmailAsync(email, cancellationToken);

        // Deliberately return the same response for known and unknown addresses so this endpoint
        // does not become an account-enumeration oracle.
        if (user is not null && user.IsActive)
            expiresInMinutes = await _passwordResetService.SendResetOtpAsync(user, cancellationToken);

        return BaseResponse<PasswordResetChallengeDto>.Success(new PasswordResetChallengeDto
        {
            Message = "If an account exists for that e-mail address, a verification code has been sent.",
            OtpSentTo = EmailMasker.Mask(email),
            OtpExpiresInMinutes = expiresInMinutes
        });
    }
}

public class VerifyPasswordResetOtpCommandHandler
    : IRequestHandler<VerifyPasswordResetOtpCommand, BaseResponse<PasswordResetVerificationDto>>
{
    private readonly IUserAccountRepository _userRepository;
    private readonly IPasswordResetService _passwordResetService;

    public VerifyPasswordResetOtpCommandHandler(
        IUserAccountRepository userRepository,
        IPasswordResetService passwordResetService)
    {
        _userRepository = userRepository;
        _passwordResetService = passwordResetService;
    }

    public async Task<BaseResponse<PasswordResetVerificationDto>> Handle(
        VerifyPasswordResetOtpCommand request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Email) || !AuthLoginPolicy.IsValidOtp(request.Otp))
            return InvalidCode;

        var user = await _userRepository.GetByEmailAsync(request.Email.Trim(), cancellationToken);
        if (user is null || !user.IsActive)
            return InvalidCode;

        var resetToken = await _passwordResetService.VerifyResetOtpAsync(
            user.Email,
            request.Otp.Trim(),
            cancellationToken);

        if (string.IsNullOrEmpty(resetToken))
            return InvalidCode;

        return BaseResponse<PasswordResetVerificationDto>.Success(new PasswordResetVerificationDto
        {
            ResetToken = resetToken
        });
    }

    private static BaseResponse<PasswordResetVerificationDto> InvalidCode =>
        BaseResponse<PasswordResetVerificationDto>.Failure("Invalid or expired code");
}

public class CompletePasswordResetCommandHandler : IRequestHandler<CompletePasswordResetCommand, BaseResponse<bool>>
{
    private readonly IUserAccountRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IPasswordResetService _passwordResetService;

    public CompletePasswordResetCommandHandler(
        IUserAccountRepository userRepository,
        IPasswordHasher passwordHasher,
        IPasswordResetService passwordResetService)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _passwordResetService = passwordResetService;
    }

    public async Task<BaseResponse<bool>> Handle(
        CompletePasswordResetCommand request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(request.NewPassword) || request.NewPassword.Length < 8)
            return BaseResponse<bool>.Failure("The new password must be at least 8 characters long");

        if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.ResetToken))
            return BaseResponse<bool>.Failure("The password reset session is invalid or has expired");

        var user = await _userRepository.GetByEmailAsync(request.Email.Trim(), cancellationToken);
        if (user is null || !user.IsActive)
            return BaseResponse<bool>.Failure("The password reset session is invalid or has expired");

        var validToken = await _passwordResetService.ConsumeResetTokenAsync(
            user.Email,
            request.ResetToken,
            cancellationToken);

        if (!validToken)
            return BaseResponse<bool>.Failure("The password reset session is invalid or has expired");

        user.UpdatePassword(_passwordHasher.HashPassword(request.NewPassword));
        await _userRepository.SaveChangesAsync(cancellationToken);

        return BaseResponse<bool>.Success(true);
    }
}
