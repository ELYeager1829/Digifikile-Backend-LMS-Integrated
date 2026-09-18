using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;
using System.Net;

namespace DigiFikileLms.Infrastructure.Services;

public class EmailService
{
    private readonly IConfiguration _configuration;

    public EmailService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task SendEmailAsync(
        string recipientEmail,
        string subject,
        string htmlBody,
        CancellationToken cancellationToken = default)
    {
        var smtpHost = Required("SMTP_HOST");
        var smtpUsername = Required("SMTP_USERNAME");
        var smtpPassword = Required("SMTP_PASSWORD");
        var smtpPortValue = Required("SMTP_PORT");

        if (!int.TryParse(smtpPortValue, out var smtpPort))
            throw new InvalidOperationException("SMTP_PORT must be a valid integer.");

        var email = new MimeMessage();
        email.From.Add(new MailboxAddress("DigiFikile LMS", smtpUsername));
        email.To.Add(MailboxAddress.Parse(recipientEmail));
        email.Subject = subject;
        email.Body = new BodyBuilder { HtmlBody = htmlBody }.ToMessageBody();

        using var smtp = new SmtpClient();
        await smtp.ConnectAsync(smtpHost, smtpPort, SecureSocketOptions.StartTls, cancellationToken);
        await smtp.AuthenticateAsync(smtpUsername, smtpPassword, cancellationToken);
        await smtp.SendAsync(email, cancellationToken);
        await smtp.DisconnectAsync(true, cancellationToken);
    }

    public async Task SendOtpEmailAsync(
        string recipientEmail,
        string otp,
        string? firstName = null,
        int expiryMinutes = 10,
        CancellationToken cancellationToken = default)
    {
        var html = await LoadTemplateAsync("OtpEmail.html", cancellationToken);

        html = html
            .Replace("{{OTP}}", WebUtility.HtmlEncode(otp))
            .Replace("{{FirstName}}", WebUtility.HtmlEncode(firstName ?? "User"))
            .Replace("{{ExpiryMinutes}}", expiryMinutes.ToString())
            .Replace("{{year}}", DateTime.UtcNow.Year.ToString());

        await SendEmailAsync(
            recipientEmail,
            "Your DigiFikile Verification Code",
            html,
            cancellationToken);
    }

    public async Task SendAccountInvitationAsync(
        string recipientEmail,
        string firstName,
        string invitationLink,
        string? username = null,
        string? temporaryPassword = null,
        CancellationToken cancellationToken = default)
    {
        var html = await LoadTemplateAsync("AccountInvitation.html", cancellationToken);

        var credentialsBlock = string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(temporaryPassword)
            ? string.Empty
            : $"""
              <div style="background:#f8fafc;border:1px solid #e5e7eb;border-radius:8px;padding:16px;margin:20px 0;">
                <p style="margin:0 0 8px;font-size:14px;color:#475569;"><strong>Username:</strong> {WebUtility.HtmlEncode(username)}</p>
                <p style="margin:0;font-size:14px;color:#475569;"><strong>Temporary password:</strong> {WebUtility.HtmlEncode(temporaryPassword)}</p>
              </div>
              """;

        html = html
            .Replace("{{name}}", WebUtility.HtmlEncode(firstName))
            .Replace("{{FirstName}}", WebUtility.HtmlEncode(firstName))
            .Replace("{{action_link}}", WebUtility.HtmlEncode(invitationLink))
            .Replace("{{InvitationLink}}", WebUtility.HtmlEncode(invitationLink))
            .Replace("{{action_text}}", "Sign in & set your password")
            .Replace("{{credentials_block}}", credentialsBlock);

        await SendEmailAsync(
            recipientEmail,
            "Welcome to DigiFikile LMS",
            html,
            cancellationToken);
    }

    private string Required(string key) =>
        !string.IsNullOrWhiteSpace(_configuration[key])
            ? _configuration[key]!
            : throw new InvalidOperationException($"Missing required e-mail configuration value: {key}");

    private static async Task<string> LoadTemplateAsync(
        string templateName,
        CancellationToken cancellationToken = default)
    {
        var templatePath = Path.Combine(AppContext.BaseDirectory, "EmailTemplates", templateName);

        if (!File.Exists(templatePath))
            throw new FileNotFoundException($"Email template '{templateName}' was not found at '{templatePath}'.");

        return await File.ReadAllTextAsync(templatePath, cancellationToken);
    }
}
