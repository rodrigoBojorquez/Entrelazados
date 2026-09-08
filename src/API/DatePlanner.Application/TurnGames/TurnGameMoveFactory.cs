using System.Globalization;
using DatePlanner.Domain.Abstractions;
using DatePlanner.Domain.TurnGames;
using DatePlanner.Domain.TurnGames.Rules;
using DatePlanner.Domain.TurnGames.Rules.HigherNumberWins;
using DatePlanner.Domain.TurnGames.Rules.RockPaperScissors;

namespace DatePlanner.Application.TurnGames;

internal static class TurnGameMoveFactory
{
    internal static Result<ITurnGameMove> Create(TurnGameType gameType, string moveCode)
    {
        string normalizedCode = moveCode.Trim().ToLowerInvariant();

        ITurnGameMove? move = gameType switch
        {
            TurnGameType.RockPaperScissors => normalizedCode switch
            {
                "rock" => RockPaperScissorsMove.Rock,
                "paper" => RockPaperScissorsMove.Paper,
                "scissors" => RockPaperScissorsMove.Scissors,
                _ => null
            },
            TurnGameType.HigherNumberWins when int.TryParse(
                normalizedCode,
                NumberStyles.None,
                CultureInfo.InvariantCulture,
                out int value) => value switch
                {
                    1 => HigherNumberMove.One,
                    2 => HigherNumberMove.Two,
                    3 => HigherNumberMove.Three,
                    4 => HigherNumberMove.Four,
                    5 => HigherNumberMove.Five,
                    6 => HigherNumberMove.Six,
                    _ => null
                },
            _ => null
        };

        return move is null
            ? Result.Failure<ITurnGameMove>(TurnGameErrors.InvalidMove)
            : Result.Success<ITurnGameMove>(move);
    }
}
