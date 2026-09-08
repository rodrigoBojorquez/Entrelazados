using DatePlanner.Application.Common.Authentication;
using DatePlanner.Application.Common.Data;
using DatePlanner.Domain.Abstractions;
using DatePlanner.Domain.Authentication;
using DatePlanner.Domain.Users;
using MediatR;
using RefreshTokenEntity = DatePlanner.Domain.Authentication.RefreshToken;

namespace DatePlanner.Application.Authentication.RefreshToken;

public sealed class RefreshTokenCommandHandler(
    IRefreshTokenRepository refreshTokenRepository,
    IUserRepository userRepository,
    ITokenProvider tokenProvider,
    TimeProvider timeProvider,
    IUnitOfWork unitOfWork)
    : IRequestHandler<RefreshTokenCommand, Result<AuthenticationResponse>>
{
    public async Task<Result<AuthenticationResponse>> Handle(
        RefreshTokenCommand request,
        CancellationToken cancellationToken)
    {
        string tokenHash = tokenProvider.HashRefreshToken(request.RefreshToken);
        RefreshTokenEntity? currentToken =
            await refreshTokenRepository.GetByTokenHashAsync(tokenHash);

        if (currentToken is null)
        {
            return InvalidRefreshToken();
        }

        DateTime utcNow = timeProvider.GetUtcNow().UtcDateTime;

        if (currentToken.RevokedAtUtc is not null)
        {
            if (currentToken.ReplacedByTokenId is not null)
            {
                await RevokeTokenFamily(currentToken.FamilyId, utcNow, cancellationToken);
            }

            return InvalidRefreshToken();
        }

        if (!currentToken.IsActive(utcNow))
        {
            return InvalidRefreshToken();
        }

        User? user = await userRepository.GetUserByIdAsync(currentToken.UserId);

        if (user is null)
        {
            return InvalidRefreshToken();
        }

        GeneratedRefreshToken generatedRefreshToken = tokenProvider.GenerateRefreshToken();
        Result<RefreshTokenEntity> creationResult = RefreshTokenEntity.Create(
            user.Id,
            currentToken.FamilyId,
            generatedRefreshToken.Hash,
            generatedRefreshToken.CreatedAtUtc,
            generatedRefreshToken.ExpiresAtUtc);

        if (creationResult.IsFailure)
        {
            return Result.Failure<AuthenticationResponse>(creationResult.Error);
        }

        Result rotationResult = currentToken.Rotate(
            creationResult.Value.Id,
            generatedRefreshToken.CreatedAtUtc);

        if (rotationResult.IsFailure)
        {
            return InvalidRefreshToken();
        }

        refreshTokenRepository.Insert(creationResult.Value);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        AccessToken accessToken = tokenProvider.GenerateAccessToken(user);

        return new AuthenticationResponse(
            "Bearer",
            accessToken.Value,
            accessToken.ExpiresAtUtc,
            generatedRefreshToken.Value,
            generatedRefreshToken.ExpiresAtUtc);
    }

    private async Task RevokeTokenFamily(
        Guid familyId,
        DateTime utcNow,
        CancellationToken cancellationToken)
    {
        IReadOnlyCollection<RefreshTokenEntity> activeTokens =
            await refreshTokenRepository.GetActiveByFamilyIdAsync(familyId, utcNow);

        foreach (RefreshTokenEntity activeToken in activeTokens)
        {
            activeToken.Revoke(utcNow);
        }

        if (activeTokens.Count > 0)
        {
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }

    private static Result<AuthenticationResponse> InvalidRefreshToken() =>
        Result.Failure<AuthenticationResponse>(AuthenticationErrors.InvalidRefreshToken);
}
