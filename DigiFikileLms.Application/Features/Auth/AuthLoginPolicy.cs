using DigiFikileLms.Application.Common;
using DigiFikileLms.Application.DTOs;
using DigiFikileLms.Application.Interfaces;
using DigiFikileLms.Domain.Entities;
using DigiFikileLms.Domain.Enums;

namespace DigiFikileLms.Application.Features.Auth;

/// <summary>
/// TYPE: AuthLoginPolicy
/// PURPOSE: The shared rules of the two-step login handshake used by every role.
/// LMS ROLE: Supports the Auth area while respecting the Application layer boundary.
///
/// IMPLEMENTATION GUIDE:
///   - Which roles may sign in with a password, whether their profile is active, what shape a
///     PIN must have, and the response produced for each step of the handshake live here.
///   - Keep this internal: it is an implementation detail of the Auth handlers.
/// </summary>
internal static class AuthLoginPolicy
{
    /// <summary>Number of digits a login PIN must contain.</summary>
    public const int OtpLength = 6;

    /// <summary>
    /// Roles that expose a password login endpoint. Every role in <see cref="UserRole"/> has one
    /// now, so this guard exists to force a deliberate decision whenever a role is added.
    /// </summary>
    public static bool SupportsPasswordLogin(UserRole role) =>
        role is UserRole.Student
            or UserRole.Facilitator
            or UserRole.Moderator
            or UserRole.SetaAdministrator
            or UserRole.TrainingProvider
            or UserRole.SystemAdministrator;

    /// <summary>
    /// A deactivated account blocks the login. Administrator profiles carry their own flag and are
    /// only consulted when they are loaded, so an absent profile never blocks the login.
    /// </summary>
    public static bool IsActive(UserAccount user) => user.UserRole switch
    {
        UserRole.SetaAdministrator => user.IsActive && user.SetaAdministrator?.IsActive != false,
        UserRole.SystemAdministrator => user.IsActive && user.SystemAdministrator?.IsActive != false,
        _ => user.IsActive
    };

    /// <summary>True when the supplied value is exactly six digits.</summary>
    public static bool IsValidOtp(string? otp) =>
        !string.IsNullOrWhiteSpace(otp) &&
        otp.Trim().Length == OtpLength &&
        otp.Trim().All(char.IsDigit);

    /// <summary>
    /// Step one of the handshake: credentials were accepted and a PIN is on its way, so no
    /// JWT is returned yet.
    /// </summary>
    public static AuthResponseDto Challenge(UserAccount user, int expiresInMinutes, string? studentNumber = null) => new()
    {
        Token = string.Empty,
        UserId = user.Id,
        Email = user.Email,
        Name = user.Name,
        Role = user.UserRole.ToString(),
        StudentNumber = studentNumber ?? user.Student?.StudentNumber,
        OtpRequired = true,
        OtpSentTo = EmailMasker.Mask(user.Email),
        OtpExpiresInMinutes = expiresInMinutes
    };

    /// <summary>
    /// Step two of the handshake: the PIN matched, so the account receives its JWT.
    /// </summary>
    public static AuthResponseDto Authenticated(UserAccount user, ITokenService tokenService, string? studentNumber = null) => new()
    {
        Token = tokenService.GenerateToken(user),
        UserId = user.Id,
        Email = user.Email,
        Name = user.Name,
        Role = user.UserRole.ToString(),
        StudentNumber = studentNumber ?? user.Student?.StudentNumber
    };
}
