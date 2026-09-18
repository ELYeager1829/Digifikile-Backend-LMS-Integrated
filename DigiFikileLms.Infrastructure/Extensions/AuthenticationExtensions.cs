using System.Security.Claims;
using System.Text;
using DigiFikileLms.Domain.Enums;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace DigiFikileLms.Infrastructure.Extensions;

public static class AuthenticationExtensions
{
    public static TokenValidationParameters CreateTokenValidationParameters(IConfiguration configuration)
    {
        string Required(string key) => !string.IsNullOrWhiteSpace(configuration[key])
            ? configuration[key]! : throw new InvalidOperationException($"{key} is not configured.");
        var secret = Required("Jwt:SecretKey");
        if (Encoding.UTF8.GetByteCount(secret) < 32)
            throw new InvalidOperationException("Jwt:SecretKey must contain at least 32 UTF-8 bytes.");
        return new TokenValidationParameters
        {
            ValidateIssuer = true, ValidateAudience = true, ValidateLifetime = true,
            ValidateIssuerSigningKey = true, RequireExpirationTime = true,
            ValidIssuer = Required("Jwt:Issuer"), ValidAudience = Required("Jwt:Audience"),
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret)),
            ValidAlgorithms = new[] { SecurityAlgorithms.HmacSha256 },
            RoleClaimType = ClaimTypes.Role, NameClaimType = ClaimTypes.Name,
            ClockSkew = TimeSpan.FromMinutes(1)
        };
    }
    public static IServiceCollection AddDigiFikileLmsAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        var validation = CreateTokenValidationParameters(configuration);
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
        {
            options.MapInboundClaims = true;
            options.TokenValidationParameters = validation;
        });
        services.AddAuthorization(options =>
        {
            options.FallbackPolicy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
            foreach (var role in Enum.GetValues<UserRole>())
                options.AddPolicy(role.ToString(), policy => policy.RequireAuthenticatedUser().RequireRole(role.ToString()));
        });
        return services;
    }
}