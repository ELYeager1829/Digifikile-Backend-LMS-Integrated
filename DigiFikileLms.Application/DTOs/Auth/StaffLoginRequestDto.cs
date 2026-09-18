namespace DigiFikileLms.Application.DTOs.Auth;

/// <summary>
/// Request DTO shared by the Facilitator, Moderator and Training Provider logins: staff sign in
/// with the e-mail address on their account and then submit the six-digit PIN through
/// POST /api/Auth/otp/verify.
/// </summary>
public record StaffLoginRequestDto(
    string Email,
    string Password
);