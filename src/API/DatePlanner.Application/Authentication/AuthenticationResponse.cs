namespace DatePlanner.Application.Authentication;

public sealed record AuthenticationResponse(
    string TokenType,
    string AccessToken,
    DateTime AccessTokenExpiresAtUtc,
    string RefreshToken,
    DateTime RefreshTokenExpiresAtUtc);
