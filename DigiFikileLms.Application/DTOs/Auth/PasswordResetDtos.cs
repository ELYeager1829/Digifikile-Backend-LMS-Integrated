namespace DigiFikileLms.Application.DTOs.Auth;

public record ForgotPasswordRequestDto(string Email);

public record VerifyPasswordResetOtpRequestDto(string Email, string Otp);

public record ResetPasswordRequestDto(string Email, string ResetToken, string NewPassword);

public class PasswordResetChallengeDto
{
    public string Message { get; set; } = string.Empty;
    public string OtpSentTo { get; set; } = string.Empty;
    public int OtpExpiresInMinutes { get; set; }
}

public class PasswordResetVerificationDto
{
    public string ResetToken { get; set; } = string.Empty;
}
