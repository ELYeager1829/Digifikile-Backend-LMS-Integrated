using DigiFikileLms.Application.Common;
using DigiFikileLms.Application.DTOs;
using MediatR;

namespace DigiFikileLms.Application.Features.Auth.Commands;

/// <summary>
/// Facilitator login step one: the credentials are checked, then a six-digit PIN is mailed to the
/// account and the JWT is issued by POST /api/Auth/otp/verify.
/// </summary>
public record FacilitatorLoginCommand(
    string Email,
    string Password
) : IRequest<BaseResponse<AuthResponseDto>>;

/// <summary>
/// Moderator login step one. Same two-step handshake as every other role.
/// </summary>
public record ModeratorLoginCommand(
    string Email,
    string Password
) : IRequest<BaseResponse<AuthResponseDto>>;

/// <summary>
/// Training Provider login step one. Same two-step handshake as every other role.
/// </summary>
public record TrainingProviderLoginCommand(
    string Email,
    string Password
) : IRequest<BaseResponse<AuthResponseDto>>;