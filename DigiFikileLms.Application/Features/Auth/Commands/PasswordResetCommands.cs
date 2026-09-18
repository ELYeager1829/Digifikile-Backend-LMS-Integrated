using DigiFikileLms.Application.Common;
using DigiFikileLms.Application.DTOs.Auth;
using MediatR;

namespace DigiFikileLms.Application.Features.Auth.Commands;

public record ForgotPasswordCommand(string Email)
    : IRequest<BaseResponse<PasswordResetChallengeDto>>;

public record VerifyPasswordResetOtpCommand(string Email, string Otp)
    : IRequest<BaseResponse<PasswordResetVerificationDto>>;

public record CompletePasswordResetCommand(string Email, string ResetToken, string NewPassword)
    : IRequest<BaseResponse<bool>>;
