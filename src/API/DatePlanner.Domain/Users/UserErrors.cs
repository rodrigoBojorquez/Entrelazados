using DatePlanner.Domain.Abstractions;

namespace DatePlanner.Domain.Users;

public static class UserErrors
{
    public static Error NotFound(Guid userId) => Error.NotFound(
        "User.NotFound",
        $"No existe el usuario {userId}.");

    public static Error EmailAlreadyExists(string email) => Error.Conflict(
        "User.EmailAlreadyExists",
        $"Ya existe un usuario registrado con el correo {email}.");

    public static Error PartnerCodeNotFound(string partnerCode) => Error.NotFound(
        "User.PartnerCodeNotFound",
        $"No existe un usuario con el código de pareja {partnerCode}.");
}
