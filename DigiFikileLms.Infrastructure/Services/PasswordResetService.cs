using System.Security.Cryptography;
using System.Text;
using DigiFikileLms.Application.Interfaces;
using DigiFikileLms.Domain.Entities;
using Microsoft.Extensions.Caching.Memory;

namespace DigiFikileLms.Infrastructure.Services;

public class PasswordResetService : IPasswordResetService
{
    private const int ExpiryMinutes = 10;
    private readonly IMemoryCache _cache;
    private readonly EmailService _emailService;

    public PasswordResetService(IMemoryCache cache, EmailService emailService)
    {
        _cache = cache;
        _emailService = emailService;
    }

    public async Task<int> SendResetOtpAsync(
        UserAccount user,
        CancellationToken cancellationToken = default)
    {
        var otp = RandomNumberGenerator.GetInt32(0, 1_000_000).ToString("D6");
        var key = ResetOtpKey(user.Email);
        _cache.Set(key, otp, TimeSpan.FromMinutes(ExpiryMinutes));

        try
        {
            await _emailService.SendOtpEmailAsync(
                user.Email,
                otp,
                user.Name,
                ExpiryMinutes,
                cancellationToken);
        }
        catch
        {
            _cache.Remove(key);
            throw;
        }

        return ExpiryMinutes;
    }

    public Task<string?> VerifyResetOtpAsync(
        string email,
        string otp,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var otpKey = ResetOtpKey(email);
        if (!_cache.TryGetValue<string>(otpKey, out var expectedOtp) || expectedOtp is null)
            return Task.FromResult<string?>(null);

        var expectedBytes = Encoding.UTF8.GetBytes(expectedOtp);
        var submittedBytes = Encoding.UTF8.GetBytes(otp.Trim());
        var matches = expectedBytes.Length == submittedBytes.Length &&
                      CryptographicOperations.FixedTimeEquals(expectedBytes, submittedBytes);

        if (!matches)
            return Task.FromResult<string?>(null);

        _cache.Remove(otpKey);

        var resetToken = Convert.ToHexString(RandomNumberGenerator.GetBytes(32));
        _cache.Set(ResetTokenKey(email), resetToken, TimeSpan.FromMinutes(ExpiryMinutes));
        return Task.FromResult<string?>(resetToken);
    }

    public Task<bool> ConsumeResetTokenAsync(
        string email,
        string resetToken,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var key = ResetTokenKey(email);
        if (!_cache.TryGetValue<string>(key, out var expectedToken) || expectedToken is null)
            return Task.FromResult(false);

        var expectedBytes = Encoding.UTF8.GetBytes(expectedToken);
        var submittedBytes = Encoding.UTF8.GetBytes(resetToken.Trim());
        var matches = expectedBytes.Length == submittedBytes.Length &&
                      CryptographicOperations.FixedTimeEquals(expectedBytes, submittedBytes);

        if (matches)
            _cache.Remove(key);

        return Task.FromResult(matches);
    }

    private static string ResetOtpKey(string email) =>
        $"password-reset-otp:{email.Trim().ToLowerInvariant()}";

    private static string ResetTokenKey(string email) =>
        $"password-reset-token:{email.Trim().ToLowerInvariant()}";
}
