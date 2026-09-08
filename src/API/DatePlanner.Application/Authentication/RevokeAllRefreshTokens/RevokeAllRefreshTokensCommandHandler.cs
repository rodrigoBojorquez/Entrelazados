using DatePlanner.Application.Common.Data;
using DatePlanner.Domain.Abstractions;
using DatePlanner.Domain.Authentication;
using MediatR;
using RefreshTokenEntity = DatePlanner.Domain.Authentication.RefreshToken;

namespace DatePlanner.Application.Authentication.RevokeAllRefreshTokens;

public sealed class RevokeAllRefreshTokensCommandHandler(
    IRefreshTokenRepository refreshTokenRepository,
    TimeProvider timeProvider,
    IUnitOfWork unitOfWork)
    : IRequestHandler<RevokeAllRefreshTokensCommand, Result>
{
    public async Task<Result> Handle(
        RevokeAllRefreshTokensCommand request,
        CancellationToken cancellationToken)
    {
        DateTime utcNow = timeProvider.GetUtcNow().UtcDateTime;
        IReadOnlyCollection<RefreshTokenEntity> activeTokens =
            await refreshTokenRepository.GetActiveByUserIdAsync(request.UserId, utcNow);

        foreach (RefreshTokenEntity refreshToken in activeTokens)
        {
            refreshToken.Revoke(utcNow);
        }

        if (activeTokens.Count > 0)
        {
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }

        return Result.Success();
    }
}
