using DatePlanner.Application.Common.Notifications;
using Microsoft.AspNetCore.SignalR;

namespace DatePlanner.Api.Endpoints.TurnGames;

internal sealed class SignalRTurnGameNotifier(
    IHubContext<TurnGameHub, ITurnGameClient> hubContext)
    : ITurnGameNotifier
{
    public Task NotifyGameScheduledAsync(
        Guid turnGameId,
        Guid partnerOneId,
        Guid partnerTwoId,
        CancellationToken cancellationToken)
    {
        var notification = new ScheduledTurnGameNotification(turnGameId);
        string[] users = [partnerOneId.ToString(), partnerTwoId.ToString()];

        return hubContext.Clients.Users(users).TurnGameScheduled(notification);
    }
}
