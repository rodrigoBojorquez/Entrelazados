namespace DatePlanner.Domain.TurnGames;

public sealed record RockPaperScissorsMove : ITurnGameMove
{
    private RockPaperScissorsMove(string code)
    {
        Code = code;
    }

    public string Code { get; }

    public TurnGameType GameType => TurnGameType.RockPaperScissors;

    public static RockPaperScissorsMove Rock { get; } = new("rock");

    public static RockPaperScissorsMove Paper { get; } = new("paper");

    public static RockPaperScissorsMove Scissors { get; } = new("scissors");
}
