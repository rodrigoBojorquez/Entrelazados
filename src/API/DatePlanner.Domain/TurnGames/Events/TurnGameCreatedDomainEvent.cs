using DatePlanner.Domain.Abstractions;

namespace DatePlanner.Domain.TurnGames.Events;

public sealed class TurnGameCreatedDomainEvent : DomainEvent
{
    internal TurnGameCreatedDomainEvent(Guid turnGameId)
    {
        TurnGameId = turnGameId;
    }

    public Guid TurnGameId { get; }
}
