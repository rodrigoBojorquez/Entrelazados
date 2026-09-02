using DatePlanner.Domain.Abstractions;

namespace DatePlanner.Domain.TurnGames;

/// <summary>
/// Is more a proof of concept rather than an actual production implementation.
/// More interesting games should be selected and implemented in future versions of the app.
/// </summary>
public class DiceRules : ITurnGameRules
{
    public TurnGameType GameType => TurnGameType.Dice;
    
    private const int TotalRounds = 3;
    
    public Result ValidateNextMove(PlayerPosition player, string moveCode, IReadOnlyList<MoveView> existingMoves)
    {
        if (!int.TryParse(moveCode, out int number) ||
            number is < 1 or > 6)
        {
            return Result.Failure(TurnGameErrors.InvalidMove);
        }

        if (existingMoves.Any(x => x.Player == player))
        {
            return Result.Failure(TurnGameErrors.PlayerAlreadyMoved);
        }

        return Result.Success();
    }

    public RuleDecision EvaluateRound(IReadOnlyList<MoveView> moves)
    {
        if (moves.Count != 2)
        {
            return RuleDecision.Pending();
        }

        int playerOneValue = int.Parse(
            moves.Single(x => x.Player == PlayerPosition.PlayerOne).MoveCode);

        int playerTwoValue = int.Parse(
            moves.Single(x => x.Player == PlayerPosition.PlayerTwo).MoveCode);

        if (playerOneValue == playerTwoValue)
        {
            return RuleDecision.Draw();
        }

        return RuleDecision.WonBy(
            playerOneValue > playerTwoValue
                ? PlayerPosition.PlayerOne
                : PlayerPosition.PlayerTwo);
    }

    public RuleDecision EvaluateGame(IReadOnlyList<RoundView> finishedRounds)
    {
        if (finishedRounds.Count < TotalRounds)
        {
            return RuleDecision.Pending();
        }

        int playerOneWins = finishedRounds.Count(x => x.Winner == PlayerPosition.PlayerOne);

        int playerTwoWins = finishedRounds.Count(x => x.Winner == PlayerPosition.PlayerTwo);

        if (playerOneWins == playerTwoWins)
        {
            return RuleDecision.Draw();
        }

        return RuleDecision.WonBy(
            playerOneWins > playerTwoWins
                ? PlayerPosition.PlayerOne
                : PlayerPosition.PlayerTwo);
    }
}