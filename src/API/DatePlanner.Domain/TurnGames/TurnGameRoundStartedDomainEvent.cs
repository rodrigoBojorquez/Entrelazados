using DatePlanner.Domain.Abstractions;

namespace DatePlanner.Domain.TurnGames;

public class TurnGameRoundStartedDomainEvent(Guid turnGameId) : DomainEvent
{
    public Guid TurnGameId { get; init; }
}