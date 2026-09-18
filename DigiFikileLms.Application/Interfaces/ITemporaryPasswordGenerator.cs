namespace DigiFikileLms.Application.Interfaces;

/// <summary>
/// TYPE: ITemporaryPasswordGenerator
/// PURPOSE: An inward-facing abstraction for the temporary password a System Administrator
///          hands over when a new administrative account is provisioned.
/// LMS ROLE: Lets the SystemAdmin use case publish sign-in credentials without the
///          Application layer choosing a random-number implementation.
///
/// IMPLEMENTATION GUIDE:
///   - Implement this contract in DigiFikileLms.Infrastructure/Services/TemporaryPasswordGenerator.cs.
///   - The generated value must be unpredictable and must satisfy the password policy
///     (at least 8 characters, mixed cases, digits and symbols).
///
/// NEVER:
///   - Log or persist the plain-text value; only the hash is stored.
/// </summary>
public interface ITemporaryPasswordGenerator
{
    /// <summary>
    /// Creates a new unpredictable temporary password that satisfies the password policy.
    /// </summary>
    string Generate();
}
