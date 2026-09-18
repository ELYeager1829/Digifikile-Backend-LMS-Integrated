using DigiFikileLms.Application.Common;
using DigiFikileLms.Application.DTOs;
using MediatR;

namespace DigiFikileLms.Application.Features.Auth.Commands;

/// <summary>
/// Login step two for every role: exchange the six-digit PIN that was mailed to the account
/// for the JWT. The e-mail address identifies the account that is waiting for a PIN.
/// </summary>
public record VerifyLoginOtpCommand(
    string Email,
    string Otp
) : IRequest<BaseResponse<AuthResponseDto>>;

/// <summary>
/// Issues a replacement six-digit PIN for a login that is already in progress. Used when the
/// first PIN was never received or has expired.
/// </summary>
public record ResendLoginOtpCommand(
    string Email
) : IRequest<BaseResponse<AuthResponseDto>>;

/// <summary>
/// System Administrator login step one. Mirrors the Student and SETA administrator logins:
/// credentials plus role are checked and then a six-digit PIN is mailed to the account.
/// </summary>
public record SystemAdminLoginCommand(
    string Email,
    string Password
) : IRequest<BaseResponse<AuthResponseDto>>;
