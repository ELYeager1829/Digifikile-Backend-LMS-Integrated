using DigiFikileLms.Domain.Entities;

namespace DigiFikileLms.Application.Interfaces;

/// <summary>
/// TYPE: IOtpService
/// PURPOSE: An inward-facing abstraction for the six-digit one-time PIN (OTP) that every
///          account must present after the password step of a login.
/// LMS ROLE: Lets the Auth use cases demand a second factor while keeping the Application
///          layer independent of how the PIN is produced, stored or delivered
///          (EmailJS, SMTP, SMS ... are outer-layer choices).
///
/// IMPLEMENTATION GUIDE:
///   - Implement this contract in DigiFikileLms.Infrastructure/Services/OtpService.cs.
///   - SendLoginOtpAsync must create a random six-digit PIN, keep it only for a short
///     expiry window, and deliver it to the user's own e-mail address.
///   - VerifyLoginOtpAsync must compare in constant time, reject expired PINs, and consume
///     the PIN so a code can never be replayed.
///   - Throw nothing for an ordinary mismatch; return false.
///
/// NEVER:
///   - Return, log, or echo the PIN back through an API response.
///   - Reuse a PIN that was already verified.
/// </summary>
public interface IOtpService
{
    /// <summary>
    /// Issues a fresh six-digit PIN for the supplied account and delivers it to the account's
    /// e-mail address. Called only after the password step has already succeeded.
    /// </summary>
    /// <param name="user">The account that just passed the password step.</param>
    /// <param name="cancellationToken">Forwarded to every asynchronous dependency.</param>
    /// <returns>The number of minutes the PIN stays valid, so the API can tell the client.</returns>
    Task<int> SendLoginOtpAsync(UserAccount user, CancellationToken cancellationToken = default);

    /// <summary>
    /// Verifies the PIN submitted for the pending login of the supplied e-mail address and
    /// consumes it when it matches.
    /// </summary>
    /// <param name="email">E-mail address that the PIN was issued to.</param>
    /// <param name="otp">The six digits entered by the user.</param>
    /// <param name="cancellationToken">Forwarded to every asynchronous dependency.</param>
    /// <returns>True only for a pending, unexpired, matching PIN.</returns>
    Task<bool> VerifyLoginOtpAsync(string email, string otp, CancellationToken cancellationToken = default);
}
