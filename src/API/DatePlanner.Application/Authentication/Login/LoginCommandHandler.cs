using DatePlanner.Application.Common.Authentication;
using DatePlanner.Application.Common.Data;
using DatePlanner.Domain.Abstractions;
using DatePlanner.Domain.Authentication;
using DatePlanner.Domain.Users;
using MediatR;
using RefreshTokenEntity = DatePlanner.Domain.Authentication.RefreshToken;

namespace DatePlanner.Application.Authentication.Login;

public sealed class LoginCommandHandler(
    IUserRepository userRepository,
    IRefreshTokenRepository refreshTokenRepository,
    IPasswordHasher passwordHasher,
    ITokenProvider tokenProvider,
    IUnitOfWork unitOfWork)
    : IRequestHandler<LoginCommand, Result<AuthenticationResponse>>
{
    public async Task<Result<AuthenticationResponse>> Handle(
        LoginCommand request,
        CancellationToken cancellationToken)
    {
        string email = request.Email.Trim().ToLowerInvariant();
        User? user = await userRepository.GetUserByEmailAsync(email);

        if (user is null ||
            !user.VerifyPassword(request.Password, passwordHasher.Verify))
        {
            return Result.Failure<AuthenticationResponse>(
                AuthenticationErrors.InvalidCredentials);
        }

        AccessToken accessToken = tokenProvider.GenerateAccessToken(user);
        GeneratedRefreshToken generatedRefreshToken = tokenProvider.GenerateRefreshToken();
        Result<RefreshTokenEntity> creationResult = RefreshTokenEntity.Create(
            user.Id,
            Guid.NewGuid(),
            generatedRefreshToken.Hash,
            generatedRefreshToken.CreatedAtUtc,
            generatedRefreshToken.ExpiresAtUtc);

        if (creationResult.IsFailure)
        {
            return Result.Failure<AuthenticationResponse>(creationResult.Error);
        }

        refreshTokenRepository.Insert(creationResult.Value);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new AuthenticationResponse(
            "Bearer",
            accessToken.Value,
            accessToken.ExpiresAtUtc,
            generatedRefreshToken.Value,
            generatedRefreshToken.ExpiresAtUtc);
    }
}
