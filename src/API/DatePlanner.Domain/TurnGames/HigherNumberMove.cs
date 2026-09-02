using System.Globalization;

namespace DatePlanner.Domain.TurnGames;

public sealed record HigherNumberMove : ITurnGameMove
{
    private HigherNumberMove(int value)
    {
        Value = value;
    }

    public int Value { get; }

    public TurnGameType GameType => TurnGameType.HigherNumberWins;

    public string Code => Value.ToString(CultureInfo.InvariantCulture);

    public static HigherNumberMove One { get; } = new(1);
    public static HigherNumberMove Two { get; } = new(2);
    public static HigherNumberMove Three { get; } = new(3);
    public static HigherNumberMove Four { get; } = new(4);
    public static HigherNumberMove Five { get; } = new(5);
    public static HigherNumberMove Six { get; } = new(6);
}
