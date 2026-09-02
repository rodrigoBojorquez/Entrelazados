using DatePlanner.Domain.Abstractions;

namespace DatePlanner.Domain.TurnGames;

public sealed class TurnGameCreatedDomainEvent : DomainEvent
{
    internal TurnGameCreatedDomainEvent(Guid turnGameId)
    {
        TurnGameId = turnGameId;
    }

    public Guid TurnGameId { get; }
}
