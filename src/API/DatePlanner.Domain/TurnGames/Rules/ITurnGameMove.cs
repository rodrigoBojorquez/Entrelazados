namespace DatePlanner.Domain.TurnGames.Rules;

public interface ITurnGameMove
{
    TurnGameType GameType { get; }
    
    string Code { get; }
}
