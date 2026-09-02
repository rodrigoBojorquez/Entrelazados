namespace DatePlanner.Domain.TurnGames;

public sealed record DiceMove(int Value) : ITurnGameMove
{
    public TurnGameType GameType => TurnGameType.Dice;

    public string Code => Value.ToString(System.Globalization.CultureInfo.InvariantCulture);

    public static DiceMove One { get; } = new(1);
    public static DiceMove Two { get; } = new(2);
    public static DiceMove Three { get; } = new(3);
    public static DiceMove Four { get; } = new(4);
    public static DiceMove Five { get; } = new(5);
    public static DiceMove Six { get; } = new(6);
}