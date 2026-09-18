using System.Security.Cryptography;
using DigiFikileLms.Application.Interfaces;

namespace DigiFikileLms.Infrastructure.Services;

/// <summary>
/// TYPE: TemporaryPasswordGenerator
/// PURPOSE: Creates the unpredictable temporary password that a newly provisioned SETA
///          Administrator signs in with before replacing it.
/// LMS ROLE: Supplies an outer-layer implementation of ITemporaryPasswordGenerator.
///
/// IMPLEMENTATION GUIDE:
///   - Characters come from RandomNumberGenerator, never from System.Random.
///   - Uppercase letters, lowercase letters, digits and symbols are all forced once so the
///     password always satisfies the password policy.
///   - Ambiguous characters (O/0, I/l) are avoided so the value can be read from a mail safely.
///
/// NEVER:
///   - Log the generated value.
///   - Persist it: only the PBKDF2 hash produced by IPasswordHasher is stored.
/// </summary>
public class TemporaryPasswordGenerator : ITemporaryPasswordGenerator
{
    private const int Length = 12;
    private const string Upper = "ABCDEFGHJKLMNPQRSTUVWXYZ";
    private const string Lower = "abcdefghijkmnopqrstuvwxyz";
    private const string Digits = "23456789";
    private const string Symbols = "!@#$%&*?";

    /// <summary>
    /// Creates a temporary password of 12 characters containing at least one uppercase letter,
    /// one lowercase letter, one digit and one symbol.
    /// </summary>
    public string Generate()
    {
        var all = Upper + Lower + Digits + Symbols;

        var characters = new List<char>
        {
            Pick(Upper),
            Pick(Lower),
            Pick(Digits),
            Pick(Symbols)
        };

        while (characters.Count < Length)
            characters.Add(Pick(all));

        Shuffle(characters);

        return new string(characters.ToArray());
    }

    private static char Pick(string set) => set[RandomNumberGenerator.GetInt32(set.Length)];

    private static void Shuffle(List<char> characters)
    {
        for (var i = characters.Count - 1; i > 0; i--)
        {
            var j = RandomNumberGenerator.GetInt32(i + 1);
            (characters[i], characters[j]) = (characters[j], characters[i]);
        }
    }
}
