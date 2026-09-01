using DatePlanner.Domain.Abstractions;

namespace DatePlanner.Domain.TurnGames;

public class TurnGameFinishedDomainEvent(Guid turnGameId) : DomainEvent
{
    private Guid TurnGameId { get; init; }
}