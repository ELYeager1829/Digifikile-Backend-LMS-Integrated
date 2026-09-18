namespace DigiFikileLms.Application.DTOs.SystemAdmin;

/// <summary>
/// Request DTO used by a System Administrator to register a SETA Administrator account.
/// No password is supplied: the platform generates a temporary one and returns it so the
/// account holder can be told about it through the configured SMTP mail flow.
/// </summary>
public class ProvisionSetaAdminDto
{
    public string Name { get; set; } = string.Empty;
    public string Surname { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public List<SystemAdminPermissionDto> Permissions { get; set; } = new();
}

/// <summary>
/// Response DTO carrying the sign-in credentials that a newly registered SETA Administrator must
/// receive. <see cref="TemporaryPassword"/> is the only moment the clear-text value exists;
/// only its hash is persisted.
/// </summary>
public class SetaAdminCredentialsDto
{
    public int UserId { get; set; }
    public int SetaAdministratorId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Surname { get; set; } = string.Empty;

    /// <summary>
    /// The sign-in username. SETA administrators sign in with their e-mail address through
    /// POST /api/Auth/admin/login, so the username and the e-mail address hold the same value.
    /// </summary>
    public string Username { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// System generated password. It is e-mailed to the account holder by the configured mail
    /// provider; the response only carries it when that mail could not be sent, so the System
    /// Administrator can hand it over manually. The account holder replaces it with a private one
    /// through POST /api/Auth/password/change after the first login.
    /// </summary>
    public string? TemporaryPassword { get; set; }

    /// <summary>True when the credentials were handed to the mail provider without an error.</summary>
    public bool CredentialsEmailed { get; set; }

    /// <summary>Human readable outcome of the mail attempt, suitable for showing to the caller.</summary>
    public string EmailStatus { get; set; } = string.Empty;

    /// <summary>Always true for this endpoint so the client can force a password change.</summary>
    public bool PasswordIsTemporary { get; set; } = true;

    /// <summary>Relative path the account holder calls, with the temporary password, to replace it.</summary>
    public string ChangePasswordUrl { get; set; } = "/api/Auth/password/change";

    public DateTime CreatedAt { get; set; }
}


/// <summary>
/// Request DTO used by System Administrators to update an existing SETA Administrator account.
/// Email may be changed when it does not already belong to another account.
/// </summary>
public class UpdateSetaAdminDto
{
    public string Name { get; set; } = string.Empty;
    public string Surname { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public bool? IsActive { get; set; }
}
