using DatePlanner.Infrastructure.Common.Authentication;
using Microsoft.AspNetCore.SignalR;

namespace DatePlanner.Api.Endpoints.TurnGames;

internal sealed class SignalRUserIdProvider : IUserIdProvider
{
    public string? GetUserId(HubConnectionContext connection) =>
        connection.User?.FindFirst(CustomClaims.Sub)?.Value;
}
