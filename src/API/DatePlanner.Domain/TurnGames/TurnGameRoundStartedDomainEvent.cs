using DatePlanner.Domain.Abstractions;

namespace DatePlanner.Domain.TurnGames;

public sealed class TurnGameRoundStartedDomainEvent : DomainEvent
{
    internal TurnGameRoundStartedDomainEvent(
        Guid turnGameId,
        Guid roundId,
        int roundNumber)
    {
        TurnGameId = turnGameId;
        RoundId = roundId;
        RoundNumber = roundNumber;
    }

    public Guid TurnGameId { get; }

    public Guid RoundId { get; }

    public int RoundNumber { get; }
}
