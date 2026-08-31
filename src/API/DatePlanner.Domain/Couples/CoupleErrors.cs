using DatePlanner.Domain.Abstractions;

namespace DatePlanner.Domain.Couples;

public static class CoupleErrors
{
    public static Error SamePersonCannotBePartners(Guid partnerId) => Error.Problem(
        "Couple.SamePersonCannotBePartners",
        $"La persona {partnerId} no puede ser pareja de sí misma");
}