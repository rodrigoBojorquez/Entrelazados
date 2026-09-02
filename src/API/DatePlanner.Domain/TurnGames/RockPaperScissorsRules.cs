using DatePlanner.Domain.Abstractions;

namespace DatePlanner.Domain.TurnGames;

public sealed class RockPaperScissorsRules : ITurnGameRules
{
    private static readonly HashSet<string> ValidMoves = ["rock", "paper", "scissors"];

    private const int RequiredRoundWinsToWinGame = 2;

    public TurnGameType GameType => TurnGameType.RockPaperScissors;

    public Result ValidateNextMove(PlayerPosition player, string moveCode, IReadOnlyList<MoveView> existingMoves)
    {
        if (!ValidMoves.Contains(moveCode))
        {
            return Result.Failure(TurnGameErrors.InvalidMove);
        }

        if (existingMoves.Any(em => em.Player == player))
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

        MoveView playerOne = moves.Single(m => m.Player == PlayerPosition.PlayerOne);
        MoveView playerTwo = moves.Single(m => m.Player == PlayerPosition.PlayerTwo);

        if (playerOne.MoveCode == playerTwo.MoveCode)
        {
            return RoundDecision.Draw();
        }

        bool playerOneWins = (playerOne.MoveCode == "rock" && playerTwo.MoveCode == "scissors") ||
                             (playerOne.MoveCode == "scissors" && playerTwo.MoveCode == "paper") ||
                             (playerOne.MoveCode == "paper" && playerTwo.MoveCode == "rock");
        
        return RoundDecision.WonBy(playerOneWins ? PlayerPosition.PlayerOne : PlayerPosition.PlayerTwo);
    }

    public GameDecision EvaluateGame(IReadOnlyList<RoundView> finishedRounds)
    {
        int playerOneWins = finishedRounds.Count(r => r.Winner == PlayerPosition.PlayerOne);
        int playerTwoWins = finishedRounds.Count(r => r.Winner == PlayerPosition.PlayerTwo);

        if (playerOneWins >= RequiredRoundWinsToWinGame)
        {
            return GameDecision.WonBy(PlayerPosition.PlayerOne);
        }

        if (playerTwoWins >= RequiredRoundWinsToWinGame)
        {
            return GameDecision.WonBy(PlayerPosition.PlayerTwo);
        }

        return GameDecision.Continue();
    }
}
