namespace DigiFikileLms.Application.Common;

/// <summary>
/// TYPE: EmailMasker
/// PURPOSE: Masks an e-mail address before it is shown in a login challenge response.
/// LMS ROLE: Supports the Auth area while respecting the Application layer boundary.
///
/// IMPLEMENTATION GUIDE:
///   - Keep the mask stable so the client can display "code sent to j***e@example.test".
///   - Never return the unmasked address before the OTP step has succeeded.
/// </summary>
public static class EmailMasker
{
    /// <summary>
    /// Masks the local part of an e-mail address, for example
    /// <c>jane.doe@example.test</c> becomes <c>j******e@example.test</c>.
    /// </summary>
    public static string Mask(string? email)
    {
        if (string.IsNullOrWhiteSpace(email)) return string.Empty;

        var trimmed = email.Trim();
        var atIndex = trimmed.LastIndexOf('@');
        if (atIndex <= 0) return new string('*', trimmed.Length);

        var localPart = trimmed[..atIndex];
        var domain = trimmed[atIndex..];

        if (localPart.Length <= 2) return new string('*', localPart.Length) + domain;

        return localPart[0] + new string('*', localPart.Length - 2) + localPart[^1] + domain;
    }
}
