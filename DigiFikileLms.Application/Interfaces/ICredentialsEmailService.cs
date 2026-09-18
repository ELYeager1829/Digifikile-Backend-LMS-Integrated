using DigiFikileLms.Domain.Entities;

namespace DigiFikileLms.Application.Interfaces;

/// <summary>
/// TYPE: ICredentialsEmailService
/// PURPOSE: An inward-facing abstraction for mailing the sign-in credentials of a newly
///          provisioned administrative account to the person who will use them.
/// LMS ROLE: Lets the SystemAdmin use case hand over a username and temporary password while
///          the Application layer stays independent of the mail provider (EmailJS, SMTP ...).
///
/// IMPLEMENTATION GUIDE:
///   - Implement this contract in DigiFikileLms.Infrastructure/Services/CredentialsEmailService.cs.
///   - Send the account's sign-in username (its e-mail address), the temporary password, and the
///     path the account holder calls to replace it, so the message is self-contained.
///   - Throw when the provider rejects the message; the caller reports the failure instead of
///     rolling back the already created account.
///
/// NEVER:
///   - Log the temporary password.
///   - Send credentials anywhere other than the account's own e-mail address.
/// </summary>
public interface ICredentialsEmailService
{
    /// <summary>
    /// Mails the temporary sign-in credentials for a provisioned account.
    /// </summary>
    /// <param name="user">The account that was just created.</param>
    /// <param name="temporaryPassword">The clear-text password the account holder must replace.</param>
    /// <param name="cancellationToken">Forwarded to every asynchronous dependency.</param>
    Task SendTemporaryCredentialsAsync(
        UserAccount user,
        string temporaryPassword,
        CancellationToken cancellationToken = default);
}
