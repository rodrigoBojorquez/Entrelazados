using DatePlanner.Domain.TurnGames.Rules.HigherNumberWins;
using DatePlanner.Domain.TurnGames.Rules.RockPaperScissors;

namespace DatePlanner.Domain.TurnGames.Rules;

internal static class TurnGameRulesCatalog
{
    private static readonly ITurnGameRules[] Rules =
    [
        new RockPaperScissorsRules(),
        new HigherNumberWinsRules()
    ];

    internal static bool TryGetCurrent(TurnGameType gameType, out ITurnGameRules rules)
    {
        ITurnGameRules? found = Rules.FirstOrDefault(x => x.GameType == gameType);

        rules = found!;
        return found is not null;
    }

    internal static ITurnGameRules Get(TurnGameType gameType)
    {
        return Rules.SingleOrDefault(x => x.GameType == gameType)
               ?? throw new InvalidOperationException($"Rules not found for {gameType}.");
    }
}
