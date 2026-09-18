using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using DigiFikileLms.Application.Interfaces;
using DigiFikileLms.Domain.Entities;
using DigiFikileLms.Infrastructure.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace DigiFikileLms.Infrastructure.Services;

public class PasswordHasherService : IPasswordHasher
{
    private const int Iterations = 210_000;
    private const string Prefix = "PBKDF2-SHA512";
    public string HashPassword(string password)
    {
        ArgumentException.ThrowIfNullOrEmpty(password);
        var salt = RandomNumberGenerator.GetBytes(16);
        var key = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, HashAlgorithmName.SHA512, 32);
        return string.Join("$", Prefix, Iterations, Convert.ToBase64String(salt), Convert.ToBase64String(key));
    }
    public bool NeedsRehash(string hash) => !hash.StartsWith(Prefix + "$", StringComparison.Ordinal);
    public bool VerifyPassword(string password, string hash)
    {
        if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(hash)) return false;
        try
        {
            // Existing Base64 credentials are upgraded by login handlers after verification.
            if (NeedsRehash(hash))
                return CryptographicOperations.FixedTimeEquals(Encoding.UTF8.GetBytes(password), Convert.FromBase64String(hash));
            var parts = hash.Split('$');
            if (parts.Length != 4 || !int.TryParse(parts[1], out var rounds) || rounds != Iterations) return false;
            var salt = Convert.FromBase64String(parts[2]);
            var expected = Convert.FromBase64String(parts[3]);
            if (salt.Length != 16 || expected.Length != 32) return false;
            var actual = Rfc2898DeriveBytes.Pbkdf2(password, salt, rounds, HashAlgorithmName.SHA512, 32);
            return CryptographicOperations.FixedTimeEquals(actual, expected);
        }
        catch (FormatException) { return false; }
    }
}

public class TokenService : ITokenService
{
    private readonly IConfiguration _configuration;
    public TokenService(IConfiguration configuration) => _configuration = configuration;
    public string GenerateToken(UserAccount user)
    {
        var validation = AuthenticationExtensions.CreateTokenValidationParameters(_configuration);
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.Name, $"{user.Name} {user.Surname}".Trim()),
            new(ClaimTypes.Role, user.UserRole.ToString()),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };
        if (user.Student != null) claims.Add(new Claim("student_id", user.Student.Id.ToString()));
        var minutes = _configuration.GetValue<int?>("Jwt:ExpiryMinutes") ?? 60;
        if (minutes <= 0) throw new InvalidOperationException("Jwt:ExpiryMinutes must be positive.");
        var token = new JwtSecurityToken(validation.ValidIssuer, validation.ValidAudience, claims,
            notBefore: DateTime.UtcNow, expires: DateTime.UtcNow.AddMinutes(minutes),
            signingCredentials: new SigningCredentials(validation.IssuerSigningKey, SecurityAlgorithms.HmacSha256));
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
    public string GenerateRefreshToken() => Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
    public bool ValidateToken(string token)
    {
        if (string.IsNullOrWhiteSpace(token)) return false;
        try
        {
            new JwtSecurityTokenHandler().ValidateToken(token,
                AuthenticationExtensions.CreateTokenValidationParameters(_configuration), out _);
            return true;
        }
        catch (SecurityTokenException) { return false; }
        catch (ArgumentException) { return false; }
    }
}