namespace DatePlanner.Domain.TurnGames;

public sealed record RockPaperScissorsMove(string Code) : ITurnGameMove
{
    public TurnGameType GameType => TurnGameType.RockPaperScissors;

    public static RockPaperScissorsMove Rock { get; } = new("rock");

    public static RockPaperScissorsMove Paper { get; } = new("paper");

    public static RockPaperScissorsMove Scissors { get; } = new("scissors");
}