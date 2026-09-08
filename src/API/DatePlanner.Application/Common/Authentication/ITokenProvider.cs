using DatePlanner.Domain.Users;

namespace DatePlanner.Application.Common.Authentication;

public interface ITokenProvider
{
    AccessToken GenerateAccessToken(User user);

    GeneratedRefreshToken GenerateRefreshToken();

    string HashRefreshToken(string refreshToken);
}

public sealed record AccessToken(string Value, DateTime ExpiresAtUtc);

public sealed record GeneratedRefreshToken(
    string Value,
    string Hash,
    DateTime CreatedAtUtc,
    DateTime ExpiresAtUtc);
