using DatePlanner.Application.TurnGames.GetTurnGame;

namespace DatePlanner.Api.Endpoints.TurnGames;

public interface ITurnGameClient
{
    Task TurnGameUpdated(TurnGameResponse turnGame);

    Task TurnGameFinished(TurnGameResponse turnGame);

    Task TurnGameScheduled(ScheduledTurnGameNotification notification);

    Task TurnGameSelected(TurnGameResponse turnGame);

    Task TurnGamePresenceChanged(TurnGamePresenceResponse presence);
}
