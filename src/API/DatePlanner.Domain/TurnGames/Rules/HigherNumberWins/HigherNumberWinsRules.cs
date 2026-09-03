using DatePlanner.Domain.Abstractions;
using DatePlanner.Domain.TurnGames.Rules;

namespace DatePlanner.Domain.TurnGames.Rules.HigherNumberWins;

/// <summary>
/// Each player chooses a number from one to six. The higher number wins the round.
/// The first player to win two rounds wins the game; drawn rounds do not count.
/// </summary>
public sealed class HigherNumberWinsRules : ITurnGameRules
{
    private const int RequiredRoundWinsToWinGame = 2;

    public TurnGameType GameType => TurnGameType.HigherNumberWins;

    public Result ValidateNextMove(
        PlayerPosition player,
        string moveCode,
        IReadOnlyList<MoveView> existingMoves)
    {
        if (!int.TryParse(moveCode, out int number) || number is < 1 or > 6)
        {
            return Result.Failure(TurnGameErrors.InvalidMove);
        }

        if (existingMoves.Any(x => x.Player == player))
        {
            return Result.Failure(TurnGameErrors.PlayerAlreadyMoved);
        }

        return Result.Success();
    }

    public RoundDecision EvaluateRound(IReadOnlyList<MoveView> moves)
    {
        if (moves.Count != 2)
        {
            return RoundDecision.Pending();
        }

        int playerOneValue = int.Parse(
            moves.Single(x => x.Player == PlayerPosition.PlayerOne).MoveCode);

        int playerTwoValue = int.Parse(
            moves.Single(x => x.Player == PlayerPosition.PlayerTwo).MoveCode);

        if (playerOneValue == playerTwoValue)
        {
            return RoundDecision.Draw();
        }

        return RoundDecision.WonBy(
            playerOneValue > playerTwoValue
                ? PlayerPosition.PlayerOne
                : PlayerPosition.PlayerTwo);
    }

    public GameDecision EvaluateGame(IReadOnlyList<RoundView> finishedRounds)
    {
        int playerOneWins = finishedRounds.Count(
            x => x.Winner == PlayerPosition.PlayerOne);

        if (playerOneWins >= RequiredRoundWinsToWinGame)
        {
            return GameDecision.WonBy(PlayerPosition.PlayerOne);
        }

        int playerTwoWins = finishedRounds.Count(
            x => x.Winner == PlayerPosition.PlayerTwo);

        if (playerTwoWins >= RequiredRoundWinsToWinGame)
        {
            return GameDecision.WonBy(PlayerPosition.PlayerTwo);
        }

        return GameDecision.Continue();
    }
}
