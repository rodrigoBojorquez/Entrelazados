using DatePlanner.Application.Common.Authentication;
using DatePlanner.Application.Common.Data;
using DatePlanner.Domain.Abstractions;
using DatePlanner.Domain.Authentication;
using MediatR;
using RefreshTokenEntity = DatePlanner.Domain.Authentication.RefreshToken;

namespace DatePlanner.Application.Authentication.RevokeRefreshToken;

public sealed class RevokeRefreshTokenCommandHandler(
    IRefreshTokenRepository refreshTokenRepository,
    ITokenProvider tokenProvider,
    TimeProvider timeProvider,
    IUnitOfWork unitOfWork)
    : IRequestHandler<RevokeRefreshTokenCommand, Result>
{
    public async Task<Result> Handle(
        RevokeRefreshTokenCommand request,
        CancellationToken cancellationToken)
    {
        string tokenHash = tokenProvider.HashRefreshToken(request.RefreshToken);
        RefreshTokenEntity? refreshToken =
            await refreshTokenRepository.GetByTokenHashAsync(tokenHash);

        if (refreshToken is null || refreshToken.RevokedAtUtc is not null)
        {
            return Result.Success();
        }

        refreshToken.Revoke(timeProvider.GetUtcNow().UtcDateTime);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
