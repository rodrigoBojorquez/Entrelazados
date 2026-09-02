namespace DatePlanner.Domain.TurnGames;

public class RuleDecision
{
    private RuleDecision(
        RuleDecisionKind kind,
        PlayerPosition? winner)
    {
        Kind = kind;
        Winner = winner;
    }

    public RuleDecisionKind Kind { get; }

    public PlayerPosition? Winner { get; }

    public static RuleDecision Pending() =>
        new(RuleDecisionKind.Pending, null);

    public static RuleDecision Draw() =>
        new(RuleDecisionKind.Draw, null);

    public static RuleDecision WonBy(PlayerPosition player) =>
        new(RuleDecisionKind.Winner, player);
}