using DatePlanner.Domain.Abstractions;

namespace DatePlanner.Domain.Authentication;

public static class RefreshTokenErrors
{
    public static Error InvalidExpiration => Error.Problem(
        "RefreshToken.InvalidExpiration",
        "La expiración del refresh token debe ser posterior a su creación.");

    public static Error AlreadyRevoked => Error.Conflict(
        "RefreshToken.AlreadyRevoked",
        "El refresh token ya fue revocado.");
}
