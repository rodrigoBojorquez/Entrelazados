using DatePlanner.Domain.Abstractions;

namespace DatePlanner.Domain.TurnGames;

public sealed class TurnGameStartedDomainEvent : DomainEvent
{
    internal TurnGameStartedDomainEvent(Guid turnGameId)
    {
        TurnGameId = turnGameId;
    }

    public Guid TurnGameId { get; }
}
