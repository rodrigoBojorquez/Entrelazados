namespace DatePlanner.Infrastructure.Common.Authentication;

internal sealed class AuthenticationOptions
{
    internal const string SectionName = "Authentication";

    public string Key { get; init; } = string.Empty;

    public string Issuer { get; init; } = string.Empty;

    public string Audience { get; init; } = string.Empty;

    public int JwtExpireMinutes { get; init; }

    public int RefreshTokenExpireDays { get; init; }
}
