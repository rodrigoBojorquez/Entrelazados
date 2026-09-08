using DatePlanner.Application.TurnGames.PlayTurnGameMove;
using DatePlanner.Application.TurnGames.StartTurnGameRound;
using DatePlanner.Domain.Abstractions;
using DatePlanner.Domain.Couples;
using DatePlanner.Domain.TurnGames;
using FluentAssertions;

namespace DatePlanner.UnitTests.Application;

public sealed class TurnGameUseCaseTests
{
    [Fact]
    public async Task PlayTurnGameMove_ShouldMapCodeAndPersistSuccessfulMove()
    {
        Couple couple = Couple.Create(Guid.NewGuid(), Guid.NewGuid()).Value;
        TurnGame game = TurnGame.Create(couple, TurnGameType.RockPaperScissors).Value;
        var games = new FakeTurnGameRepository(game);
        var unitOfWork = new FakeUnitOfWork();
        var startHandler = new StartTurnGameRoundCommandHandler(games, unitOfWork);
        var playHandler = new PlayTurnGameMoveCommandHandler(games, unitOfWork);
        await startHandler.Handle(
            new StartTurnGameRoundCommand(game.PlayerOneId, game.Id),
            CancellationToken.None);

        Result result = await playHandler.Handle(
            new PlayTurnGameMoveCommand(game.PlayerOneId, game.Id, " PAPER "),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        game.Rounds.Single().Moves.Should().ContainSingle().Which.Code.Should().Be("paper");
        unitOfWork.SaveCalls.Should().Be(2);
    }

    [Fact]
    public async Task PlayTurnGameMove_ShouldRejectInvalidCodeWithoutSaving()
    {
        Couple couple = Couple.Create(Guid.NewGuid(), Guid.NewGuid()).Value;
        TurnGame game = TurnGame.Create(couple, TurnGameType.HigherNumberWins).Value;
        game.StartRound();
        var unitOfWork = new FakeUnitOfWork();
        var handler = new PlayTurnGameMoveCommandHandler(
            new FakeTurnGameRepository(game),
            unitOfWork);

        Result result = await handler.Handle(
            new PlayTurnGameMoveCommand(game.PlayerOneId, game.Id, "7"),
            CancellationToken.None);

        result.Error.Code.Should().Be("TurnGame.InvalidMove");
        game.Rounds.Single().Moves.Should().BeEmpty();
        unitOfWork.SaveCalls.Should().Be(0);
    }
}
