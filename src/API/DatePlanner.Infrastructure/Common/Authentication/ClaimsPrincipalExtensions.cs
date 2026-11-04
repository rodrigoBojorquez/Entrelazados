using System.Security.Claims;
using DatePlanner.Application.Common.Exceptions;

namespace DatePlanner.Infrastructure.Common.Authentication;

public static class ClaimsPrincipalExtensions
{
    public static Guid GetUserId(this ClaimsPrincipal? principal)
    {
        string? userId = principal?.FindFirst(CustomClaims.Sub)?.Value;

        return Guid.TryParse(userId, out Guid parsedUserId) ?
            parsedUserId :
            throw new DatePlannerException("User identifier is unavailable");
    }
}
