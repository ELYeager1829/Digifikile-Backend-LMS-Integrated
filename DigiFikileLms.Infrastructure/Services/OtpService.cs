using System.Security.Cryptography;
using System.Text;
using DigiFikileLms.Application.Interfaces;
using DigiFikileLms.Domain.Entities;
using Microsoft.Extensions.Caching.Memory;

namespace DigiFikileLms.Infrastructure.Services;

/// <summary>
/// Generates, stores, mails, verifies and consumes the six-digit login OTP.
/// Login OTPs are held in process memory for a short period and are never returned by the API.
/// </summary>
public class OtpService : IOtpService
{
    private const int ExpiryMinutes = 10;
    private readonly IMemoryCache _cache;
    private readonly EmailService _emailService;

    public OtpService(IMemoryCache cache, EmailService emailService)
    {
        _cache = cache;
        _emailService = emailService;
    }

    public async Task<int> SendLoginOtpAsync(
        UserAccount user,
        CancellationToken cancellationToken = default)
    {
        var otp = RandomNumberGenerator.GetInt32(0, 1_000_000).ToString("D6");
        var key = LoginOtpKey(user.Email);

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

    public Task<bool> VerifyLoginOtpAsync(
        string email,
        string otp,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var key = LoginOtpKey(email);
        if (!_cache.TryGetValue<string>(key, out var expectedOtp) || expectedOtp is null)
            return Task.FromResult(false);

        var expectedBytes = Encoding.UTF8.GetBytes(expectedOtp);
        var submittedBytes = Encoding.UTF8.GetBytes(otp.Trim());
        var matches = expectedBytes.Length == submittedBytes.Length &&
                      CryptographicOperations.FixedTimeEquals(expectedBytes, submittedBytes);

        if (matches)
            _cache.Remove(key);

        return Task.FromResult(matches);
    }

    private static string LoginOtpKey(string email) =>
        $"login-otp:{email.Trim().ToLowerInvariant()}";
}
