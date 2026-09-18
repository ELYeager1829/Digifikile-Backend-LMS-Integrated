namespace DigiFikileLms.Application.DTOs;

/// <summary>
/// Response DTO containing authentication token and user information.
/// Every login is a two-step handshake: the first call returns an OTP challenge
/// (OtpRequired = true with an empty Token), and the six-digit PIN is exchanged for the JWT
/// through POST /api/Auth/otp/verify.
/// </summary>
public class AuthResponseDto
{
    public string Token { get; set; } = string.Empty;
    public int UserId { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string? StudentNumber { get; set; }

    /// <summary>
    /// True while the account still has to submit the six-digit PIN. The client must then call
    /// POST /api/Auth/otp/verify with this e-mail address and the PIN.
    /// </summary>
    public bool OtpRequired { get; set; }

    /// <summary>
    /// Masked address the PIN was sent to, for example j***e@example.test. Null once the login
    /// has been completed.
    /// </summary>
    public string? OtpSentTo { get; set; }

    /// <summary>
    /// How long the PIN stays valid, in minutes. Null once the login has been completed.
    /// </summary>
    public int? OtpExpiresInMinutes { get; set; }
}