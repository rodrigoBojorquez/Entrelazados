using DatePlanner.Application.TurnGames.CreateTurnGame;
using DatePlanner.Application.TurnGames.ExpireTurnGameRound;
using DatePlanner.Application.TurnGames.GetTurnGame;
using DatePlanner.Application.TurnGames.PlayTurnGameMove;
using DatePlanner.Application.TurnGames.SelectTurnGameType;
using DatePlanner.Application.TurnGames.StartTurnGameRound;
using DatePlanner.Domain.Abstractions;
using DatePlanner.Domain.TurnGames;
using DatePlanner.Infrastructure.Common.Authentication;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace DatePlanner.Api.Endpoints.TurnGames;

[Authorize]
public sealed class TurnGameHub(
    ISender sender,
    TurnGamePresenceTracker presenceTracker) : Hub<ITurnGameClient>
{
    public async Task<TurnGameHubResponse<TurnGameResponse>> CreateTurnGame(TurnGameType gameType)
    {
        Guid userId = Context.User.GetUserId();
        CancellationToken cancellationToken = Context.ConnectionAborted;
        Result<Guid> result = await sender.Send(
            new CreateTurnGameCommand(userId, gameType),
            cancellationToken);

        if (result.IsFailure)
        {
            return TurnGameHubResponse<TurnGameResponse>.Failure(result);
        }

        TurnGameHubResponse<TurnGameResponse> response =
            await GetAndNotifyPlayers(result.Value, userId, cancellationToken);

        if (response.IsSuccess && response.Value is not null)
        {
            await JoinPlayerGroups(response.Value, userId, cancellationToken);
            await NotifyGameSelected(response.Value, cancellationToken);
        }

        return response;
    }

    public async Task<TurnGameHubResponse<TurnGameResponse>> SelectScheduledGame(
        Guid turnGameId,
        TurnGameType gameType)
    {
        Guid userId = Context.User.GetUserId();
        CancellationToken cancellationToken = Context.ConnectionAborted;
        Result result = await sender.Send(
            new SelectTurnGameTypeCommand(userId, turnGameId, gameType),
            cancellationToken);

        if (result.IsFailure)
        {
            return TurnGameHubResponse<TurnGameResponse>.Failure(result);
        }

        Result<TurnGameResponse> selectedResult =
            await GetTurnGame(turnGameId, userId, cancellationToken);

        if (selectedResult.IsFailure)
        {
            return TurnGameHubResponse<TurnGameResponse>.Failure(selectedResult);
        }

        await NotifyGameSelected(selectedResult.Value, cancellationToken);
        return TurnGameHubResponse<TurnGameResponse>.Success(selectedResult.Value);
    }

    public async Task<TurnGameHubResponse<TurnGameResponse>> JoinTurnGame(Guid turnGameId)
    {
        Guid userId = Context.User.GetUserId();
        CancellationToken cancellationToken = Context.ConnectionAborted;
        Result<TurnGameResponse> result = await GetTurnGame(turnGameId, userId, cancellationToken);

        if (result.IsFailure)
        {
            return TurnGameHubResponse<TurnGameResponse>.Failure(result);
        }

        TurnGamePresenceJoinResult presenceJoin =
            await JoinPlayerGroups(result.Value, userId, cancellationToken);

        if (presenceJoin.BecameFullyConnected && result.Value.Status == TurnGameStatus.Ready)
        {
            Result startResult = await sender.Send(
                new StartTurnGameRoundCommand(userId, turnGameId),
                cancellationToken);

            if (startResult.IsSuccess || startResult.Error.Code == "TurnGame.ActiveRoundExists")
            {
                return await GetAndNotifyPlayers(turnGameId, userId, cancellationToken);
            }
        }

        return TurnGameHubResponse<TurnGameResponse>.Success(result.Value);
    }

    public Task LeaveTurnGame(Guid turnGameId)
    {
        Guid userId = Context.User.GetUserId();

        return Groups.RemoveFromGroupAsync(
            Context.ConnectionId,
            PlayerGroup(turnGameId, userId),
            Context.ConnectionAborted);
    }

    public async Task<TurnGameHubResponse<TurnGameResponse>> GetTurnGame(Guid turnGameId)
    {
        Guid userId = Context.User.GetUserId();
        Result<TurnGameResponse> result = await GetTurnGame(
            turnGameId,
            userId,
            Context.ConnectionAborted);

        return result.IsSuccess
            ? TurnGameHubResponse<TurnGameResponse>.Success(result.Value)
            : TurnGameHubResponse<TurnGameResponse>.Failure(result);
    }

    public async Task<TurnGameHubResponse<TurnGameResponse>> StartRound(Guid turnGameId)
    {
        Guid userId = Context.User.GetUserId();
        CancellationToken cancellationToken = Context.ConnectionAborted;
        Result<TurnGameResponse> gameResult = await GetTurnGame(
            turnGameId,
            userId,
            cancellationToken);

        if (gameResult.IsFailure)
        {
            return TurnGameHubResponse<TurnGameResponse>.Failure(gameResult);
        }

        TurnGameResponse game = gameResult.Value;
        if (!presenceTracker.AreBothPlayersConnected(
                turnGameId,
                game.PlayerOneId,
                game.PlayerTwoId))
        {
            return TurnGameHubResponse<TurnGameResponse>.Failure(
                Result.Failure(TurnGameErrors.BothPlayersMustBeConnected));
        }

        await JoinPlayerGroups(game, userId, cancellationToken);

        Result result = await sender.Send(
            new StartTurnGameRoundCommand(userId, turnGameId),
            cancellationToken);

        if (result.IsFailure)
        {
            return TurnGameHubResponse<TurnGameResponse>.Failure(result);
        }

        return await GetAndNotifyPlayers(turnGameId, userId, cancellationToken);
    }

    public async Task<TurnGameHubResponse<TurnGameResponse>> PlayMove(
        Guid turnGameId,
        string moveCode)
    {
        Guid userId = Context.User.GetUserId();
        CancellationToken cancellationToken = Context.ConnectionAborted;
        Result result = await sender.Send(
            new PlayTurnGameMoveCommand(userId, turnGameId, moveCode),
            cancellationToken);

        if (result.IsFailure)
        {
            return TurnGameHubResponse<TurnGameResponse>.Failure(result);
        }

        await JoinPlayerGroups(
            (await GetTurnGame(turnGameId, userId, cancellationToken)).Value,
            userId,
            cancellationToken);

        return await GetAndNotifyPlayers(turnGameId, userId, cancellationToken);
    }

    public async Task<TurnGameHubResponse<TurnGameResponse>> ExpireRound(
        Guid turnGameId,
        Guid roundId)
    {
        Guid userId = Context.User.GetUserId();
        CancellationToken cancellationToken = Context.ConnectionAborted;
        Result result = await sender.Send(
            new ExpireTurnGameRoundCommand(userId, turnGameId, roundId),
            cancellationToken);

        if (result.IsFailure)
        {
            return TurnGameHubResponse<TurnGameResponse>.Failure(result);
        }

        return await GetAndNotifyPlayers(turnGameId, userId, cancellationToken);
    }

    private async Task<TurnGameHubResponse<TurnGameResponse>> GetAndNotifyPlayers(
        Guid turnGameId,
        Guid actingPlayerId,
        CancellationToken cancellationToken)
    {
        Result<TurnGameResponse> actingPlayerResult = await GetTurnGame(
            turnGameId,
            actingPlayerId,
            cancellationToken);

        if (actingPlayerResult.IsFailure)
        {
            return TurnGameHubResponse<TurnGameResponse>.Failure(actingPlayerResult);
        }

        TurnGameResponse actingPlayerView = actingPlayerResult.Value;
        Guid otherPlayerId = actingPlayerView.PlayerOneId == actingPlayerId
            ? actingPlayerView.PlayerTwoId
            : actingPlayerView.PlayerOneId;
        Result<TurnGameResponse> otherPlayerResult = await GetTurnGame(
            turnGameId,
            otherPlayerId,
            cancellationToken);

        await NotifyPlayer(
            PlayerGroup(turnGameId, actingPlayerId),
            actingPlayerView);

        if (otherPlayerResult.IsSuccess)
        {
            await NotifyPlayer(
                PlayerGroup(turnGameId, otherPlayerId),
                otherPlayerResult.Value);
        }

        return TurnGameHubResponse<TurnGameResponse>.Success(actingPlayerView);
    }

    private async Task NotifyPlayer(
        string groupName,
        TurnGameResponse turnGame)
    {
        await Clients.Group(groupName).TurnGameUpdated(turnGame);

        if (turnGame.Status == TurnGameStatus.Finished)
        {
            await Clients.Group(groupName).TurnGameFinished(turnGame);
        }
    }

    private Task<Result<TurnGameResponse>> GetTurnGame(
        Guid turnGameId,
        Guid userId,
        CancellationToken cancellationToken) =>
        sender.Send(new GetTurnGameQuery(userId, turnGameId), cancellationToken);

    private async Task<TurnGamePresenceJoinResult> JoinPlayerGroups(
        TurnGameResponse turnGame,
        Guid userId,
        CancellationToken cancellationToken)
    {
        await Groups.AddToGroupAsync(
            Context.ConnectionId,
            PlayerGroup(turnGame.Id, userId),
            cancellationToken);

        await Groups.AddToGroupAsync(
            Context.ConnectionId,
            GameGroup(turnGame.Id),
            cancellationToken);

        TurnGamePresenceJoinResult presenceJoin = presenceTracker.Join(
            turnGame.Id,
            userId,
            Context.ConnectionId,
            turnGame.PlayerOneId,
            turnGame.PlayerTwoId);

        await Clients.Group(GameGroup(turnGame.Id))
            .TurnGamePresenceChanged(presenceJoin.Presence);
        return presenceJoin;
    }

    private async Task NotifyGameSelected(
        TurnGameResponse turnGame,
        CancellationToken cancellationToken)
    {
        string playerOne = turnGame.PlayerOneId.ToString();
        string playerTwo = turnGame.PlayerTwoId.ToString();
        await Clients.Users([playerOne, playerTwo]).TurnGameSelected(turnGame);
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        Guid userId = Context.User.GetUserId();
        IReadOnlyCollection<Guid> games = presenceTracker.Leave(Context.ConnectionId);

        foreach (Guid turnGameId in games)
        {
            Result<TurnGameResponse> result = await GetTurnGame(
                turnGameId,
                userId,
                CancellationToken.None);

            if (result.IsSuccess)
            {
                TurnGameResponse game = result.Value;
                TurnGamePresenceResponse presence = presenceTracker.GetSnapshot(
                    game.Id,
                    game.PlayerOneId,
                    game.PlayerTwoId);
                await Clients.Group(GameGroup(game.Id)).TurnGamePresenceChanged(presence);
            }
        }

        await base.OnDisconnectedAsync(exception);
    }

    private static string PlayerGroup(Guid turnGameId, Guid userId) =>
        $"turn-game:{turnGameId:N}:player:{userId:N}";

    private static string GameGroup(Guid turnGameId) => $"turn-game:{turnGameId:N}";
}
