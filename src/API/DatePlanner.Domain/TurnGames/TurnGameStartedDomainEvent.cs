using DatePlanner.Domain.Abstractions;

namespace DatePlanner.Domain.TurnGames;

public class TurnGameStartedDomainEvent(Guid turnGameId) : DomainEvent
{
    public Guid TurnGameId { get; init; }
}