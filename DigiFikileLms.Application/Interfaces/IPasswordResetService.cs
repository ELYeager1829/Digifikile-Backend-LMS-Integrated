using DigiFikileLms.Domain.Entities;

namespace DigiFikileLms.Application.Interfaces;

/// <summary>
/// Keeps password-recovery OTPs separate from login OTPs so a recovery code can never be
/// exchanged for an authenticated login token.
/// </summary>
public interface IPasswordResetService
{
    Task<int> SendResetOtpAsync(UserAccount user, CancellationToken cancellationToken = default);
    Task<string?> VerifyResetOtpAsync(string email, string otp, CancellationToken cancellationToken = default);
    Task<bool> ConsumeResetTokenAsync(string email, string resetToken, CancellationToken cancellationToken = default);
}
