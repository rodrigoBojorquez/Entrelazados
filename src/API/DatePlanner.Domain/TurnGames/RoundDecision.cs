namespace DatePlanner.Domain.TurnGames;

public sealed class RoundDecision
{
    private RoundDecision(
        RoundDecisionKind kind,
        PlayerPosition? winner)
    {
        Kind = kind;
        Winner = winner;
    }

    public RoundDecisionKind Kind { get; }

    public PlayerPosition? Winner { get; }

    public static RoundDecision Pending() =>
        new(RoundDecisionKind.Pending, null);

    public static RoundDecision Draw() =>
        new(RoundDecisionKind.Draw, null);

    public static RoundDecision WonBy(PlayerPosition player)
    {
        if (!Enum.IsDefined(player))
        {
            throw new ArgumentOutOfRangeException(nameof(player));
        }

        return new RoundDecision(RoundDecisionKind.Winner, player);
    }
}
