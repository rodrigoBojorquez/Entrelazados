using DatePlanner.Domain.Abstractions;

namespace DatePlanner.Domain.TurnGames.Rounds;

public class TurnGameMove : Entity
{
    public Guid Id { get; private set; }

    public Guid TurnGameRoundId { get; private set; }

    public Guid PlayerId { get; private set; }

    public int Sequence { get; private set; }

    public string Code { get; private set; } = string.Empty;
    
    public DateTime PlayedAtUtc { get; private set; }
    
    private TurnGameMove() {}

    internal static TurnGameMove Create(
        Guid roundId,
        Guid playerId,
        int sequence,
        string moveCode,
        DateTime playedAtUtc)
    {
        TurnGameMove move = new TurnGameMove
        {
            Id = Guid.NewGuid(),
            TurnGameRoundId = roundId,
            PlayerId = playerId,
            Sequence = sequence,
            Code = moveCode,
            PlayedAtUtc = playedAtUtc
        };

        return move;
    }

}
