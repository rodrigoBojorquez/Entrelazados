using DatePlanner.Domain.Abstractions;

namespace DatePlanner.Domain.Authentication;

public static class AuthenticationErrors
{
    public static Error InvalidCredentials => Error.Unauthorized(
        "Authentication.InvalidCredentials",
        "El correo o la contraseña no son válidos.");

    public static Error InvalidRefreshToken => Error.Unauthorized(
        "Authentication.InvalidRefreshToken",
        "El refresh token no es válido o ya expiró.");
}
