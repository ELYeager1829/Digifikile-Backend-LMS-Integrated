using DigiFikileLms.Application.Interfaces;
using DigiFikileLms.Domain.Entities;
using Microsoft.Extensions.Configuration;

namespace DigiFikileLms.Infrastructure.Services;

/// <summary>
/// Sends the initial SETA Administrator credentials and a link into the normal sign-in flow.
/// After OTP verification the frontend sends invited administrators directly to Change Password.
/// </summary>
public class CredentialsEmailService : ICredentialsEmailService
{
    private readonly EmailService _emailService;
    private readonly IConfiguration _configuration;

    public CredentialsEmailService(EmailService emailService, IConfiguration configuration)
    {
        _emailService = emailService;
        _configuration = configuration;
    }

    public Task SendTemporaryCredentialsAsync(
        UserAccount user,
        string temporaryPassword,
        CancellationToken cancellationToken = default)
    {
        var frontendBaseUrl = (_configuration["FRONTEND_BASE_URL"] ?? "http://localhost:5173").TrimEnd('/');
        var setupLink = $"{frontendBaseUrl}/login?setup=1&email={Uri.EscapeDataString(user.Email)}";

        return _emailService.SendAccountInvitationAsync(
            user.Email,
            user.Name,
            setupLink,
            user.Email,
            temporaryPassword,
            cancellationToken);
    }
}
