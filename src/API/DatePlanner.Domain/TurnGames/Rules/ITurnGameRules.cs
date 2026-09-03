using DatePlanner.Domain.Abstractions;

namespace DatePlanner.Domain.TurnGames.Rules;

public interface ITurnGameRules
{
    TurnGameType GameType { get; }
    
    Result ValidateNextMove(PlayerPosition player, string moveCode, IReadOnlyList<MoveView> existingMoves);
    
    RoundDecision EvaluateRound(IReadOnlyList<MoveView> moves);

    GameDecision EvaluateGame(IReadOnlyList<RoundView> finishedRounds);
}

public sealed record MoveView(PlayerPosition Player, string MoveCode, int Sequence);

public sealed record RoundView(RoundDecisionKind Result, PlayerPosition? Winner);
