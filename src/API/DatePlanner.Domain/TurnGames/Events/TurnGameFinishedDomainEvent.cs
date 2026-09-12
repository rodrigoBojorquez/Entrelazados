using DatePlanner.Domain.Abstractions;

namespace DatePlanner.Domain.TurnGames.Events;

public sealed class TurnGameFinishedDomainEvent : DomainEvent
{
    internal TurnGameFinishedDomainEvent(
        Guid turnGameId,
        Guid coupleId,
        Guid winnerId,
        TurnGamePurpose purpose)
    {
        TurnGameId = turnGameId;
        CoupleId = coupleId;
        WinnerId = winnerId;
        Purpose = purpose;
    }

    public Guid TurnGameId { get; }

    public Guid CoupleId { get; }

    public Guid WinnerId { get; }

    public TurnGamePurpose Purpose { get; }
}
