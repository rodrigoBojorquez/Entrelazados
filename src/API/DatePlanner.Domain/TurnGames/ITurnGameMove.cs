namespace DatePlanner.Domain.TurnGames;

public interface ITurnGameMove
{
    TurnGameType GameType { get; }
    
    string Code { get; }
}