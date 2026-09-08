using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using DatePlanner.Application.Common.Authentication;
using DatePlanner.Domain.Users;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace DatePlanner.Infrastructure.Common.Authentication;

internal sealed class TokenProvider(
    IOptions<AuthenticationOptions> options,
    TimeProvider timeProvider) : ITokenProvider
{
    private readonly AuthenticationOptions _options = options.Value;

    public AccessToken GenerateAccessToken(User user)
    {
        DateTime utcNow = timeProvider.GetUtcNow().UtcDateTime;
        DateTime expiresAtUtc = utcNow.AddMinutes(_options.JwtExpireMinutes);
        byte[] key = Encoding.UTF8.GetBytes(_options.Key);

        Claim[] claims =
        [
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email),
            new(JwtRegisteredClaimNames.GivenName, user.FirstName),
            new(JwtRegisteredClaimNames.FamilyName, user.LastName),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        ];

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Issuer = _options.Issuer,
            Audience = _options.Audience,
            IssuedAt = utcNow,
            NotBefore = utcNow,
            Expires = expiresAtUtc,
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256)
        };

        string token = new JsonWebTokenHandler().CreateToken(tokenDescriptor);

        return new AccessToken(token, expiresAtUtc);
    }

    public GeneratedRefreshToken GenerateRefreshToken()
    {
        DateTime createdAtUtc = timeProvider.GetUtcNow().UtcDateTime;
        DateTime expiresAtUtc = createdAtUtc.AddDays(_options.RefreshTokenExpireDays);
        string value = Base64UrlEncode(RandomNumberGenerator.GetBytes(64));

        return new GeneratedRefreshToken(
            value,
            HashRefreshToken(value),
            createdAtUtc,
            expiresAtUtc);
    }

    public string HashRefreshToken(string refreshToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(refreshToken);

        return Base64UrlEncode(SHA256.HashData(Encoding.UTF8.GetBytes(refreshToken)));
    }

    private static string Base64UrlEncode(byte[] value) =>
        Convert.ToBase64String(value)
            .TrimEnd('=')
            .Replace('+', '-')
            .Replace('/', '_');
}
