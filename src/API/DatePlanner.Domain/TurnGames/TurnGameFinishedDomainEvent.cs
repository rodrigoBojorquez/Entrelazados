using DatePlanner.Domain.Abstractions;

namespace DatePlanner.Domain.TurnGames;

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
