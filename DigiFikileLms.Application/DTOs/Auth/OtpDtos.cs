namespace DigiFikileLms.Application.DTOs.Auth;

/// <summary>
/// Request DTO for the second login step: the six-digit PIN that was sent to the account's
/// e-mail address after the password step succeeded.
/// </summary>
public record VerifyOtpRequestDto(
    string Email,
    string Otp
);

/// <summary>
/// Request DTO that asks for a replacement six-digit PIN for a login that is already in
/// progress. The password step is not repeated.
/// </summary>
public record ResendOtpRequestDto(
    string Email
);

/// <summary>
/// Request DTO for an authenticated password change. Used by administrators who received a
/// temporary password and must replace it with a private one.
/// </summary>
public record ChangePasswordRequestDto(
    string CurrentPassword,
    string NewPassword
);
