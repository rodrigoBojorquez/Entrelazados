using DatePlanner.Application.TurnGames.CreateTurnGame;
using DatePlanner.Application.TurnGames.GetTurnGame;
using DatePlanner.Application.TurnGames.PlayTurnGameMove;
using DatePlanner.Application.TurnGames.StartTurnGameRound;
using DatePlanner.Domain.Abstractions;
using DatePlanner.Domain.TurnGames;
using DatePlanner.Infrastructure.Common.Authentication;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace DatePlanner.Api.Endpoints.TurnGames;

[Authorize]
public sealed class TurnGameHub(ISender sender) : Hub<ITurnGameClient>
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

        await JoinPlayerGroup(result.Value, userId, cancellationToken);

        return await GetAndNotifyPlayers(result.Value, userId, cancellationToken);
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

        await JoinPlayerGroup(turnGameId, userId, cancellationToken);

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
        Result result = await sender.Send(
            new StartTurnGameRoundCommand(userId, turnGameId),
            cancellationToken);

        if (result.IsFailure)
        {
            return TurnGameHubResponse<TurnGameResponse>.Failure(result);
        }

        await JoinPlayerGroup(turnGameId, userId, cancellationToken);

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

        await JoinPlayerGroup(turnGameId, userId, cancellationToken);

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

    private Task JoinPlayerGroup(
        Guid turnGameId,
        Guid userId,
        CancellationToken cancellationToken) =>
        Groups.AddToGroupAsync(
            Context.ConnectionId,
            PlayerGroup(turnGameId, userId),
            cancellationToken);

    private static string PlayerGroup(Guid turnGameId, Guid userId) =>
        $"turn-game:{turnGameId:N}:player:{userId:N}";
}
