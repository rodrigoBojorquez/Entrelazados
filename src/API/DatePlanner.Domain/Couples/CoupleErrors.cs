using DatePlanner.Domain.Abstractions;

namespace DatePlanner.Domain.Couples;

public static class CoupleErrors
{
    public static Error PartnerIdIsRequired(string parameterName) => Error.Problem(
        "Couple.PartnerIdIsRequired",
        $"El identificador {parameterName} es obligatorio.");

    public static Error SamePersonCannotBePartners(Guid partnerId) => Error.Problem(
        "Couple.SamePersonCannotBePartners",
        $"La persona {partnerId} no puede ser pareja de sí misma");

    public static Error NotFound(Guid userId) => Error.NotFound(
        "Couple.NotFound",
        $"El usuario {userId} no pertenece a una pareja activa.");

    public static Error UserAlreadyHasPartner(Guid userId) => Error.Conflict(
        "Couple.UserAlreadyHasPartner",
        $"El usuario {userId} ya pertenece a una pareja activa.");
}
