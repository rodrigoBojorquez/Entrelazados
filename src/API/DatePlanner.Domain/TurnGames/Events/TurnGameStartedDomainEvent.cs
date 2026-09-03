using DatePlanner.Domain.Abstractions;

namespace DatePlanner.Domain.TurnGames.Events;

public sealed class TurnGameStartedDomainEvent : DomainEvent
{
    internal TurnGameStartedDomainEvent(Guid turnGameId)
    {
        TurnGameId = turnGameId;
    }

    public Guid TurnGameId { get; }
}
