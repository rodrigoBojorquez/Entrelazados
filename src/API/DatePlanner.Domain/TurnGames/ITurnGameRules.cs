using DatePlanner.Domain.Abstractions;

namespace DatePlanner.Domain.TurnGames;

public interface ITurnGameRules
{
    TurnGameType GameType { get; }
    
    Result ValidateNextMove(PlayerPosition player, string moveCode, IReadOnlyList<MoveView> existingMoves);
    
    RuleDecision EvaluateRound(IReadOnlyList<MoveView> moves);

    RuleDecision EvaluateGame(IReadOnlyList<RoundView> finishedRounds);
}

public sealed record MoveView(PlayerPosition Player, string MoveCode, int Sequence);

public sealed record RoundView(RuleDecisionKind Result, PlayerPosition? Winner);