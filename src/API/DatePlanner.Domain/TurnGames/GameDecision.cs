namespace DatePlanner.Domain.TurnGames;

public abstract record GameDecision
{
    private GameDecision()
    {
    }

    public sealed record ContinuePlaying : GameDecision;

    public sealed record Won : GameDecision
    {
        internal Won(PlayerPosition winner)
        {
            if (!Enum.IsDefined(winner))
            {
                throw new ArgumentOutOfRangeException(nameof(winner));
            }

            Winner = winner;
        }

        public PlayerPosition Winner { get; }
    }

    public static GameDecision Continue() => new ContinuePlaying();

    public static GameDecision WonBy(PlayerPosition winner) => new Won(winner);
}
