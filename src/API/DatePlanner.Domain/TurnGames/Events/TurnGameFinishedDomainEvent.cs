using DatePlanner.Domain.Abstractions;

namespace DatePlanner.Domain.TurnGames.Events;

public sealed class TurnGameFinishedDomainEvent : DomainEvent
{
    internal TurnGameFinishedDomainEvent(
        Guid turnGameId,
        Guid coupleId,
        Guid winnerId)
    {
        TurnGameId = turnGameId;
        CoupleId = coupleId;
        WinnerId = winnerId;
    }

    public Guid TurnGameId { get; }

    public Guid CoupleId { get; }

    public Guid WinnerId { get; }
}
