using DatePlanner.Application.Authentication;
using DatePlanner.Application.Authentication.Login;
using DatePlanner.Application.Authentication.RefreshToken;
using DatePlanner.Domain.Abstractions;
using DatePlanner.Domain.Authentication;
using DatePlanner.Domain.Users;
using FluentAssertions;
using RefreshTokenEntity = DatePlanner.Domain.Authentication.RefreshToken;

namespace DatePlanner.UnitTests.Application;

public sealed class AuthenticationUseCaseTests
{
    private static readonly DateTime UtcNow = new(2026, 9, 8, 20, 0, 0, DateTimeKind.Utc);

    [Fact]
    public async Task Login_ShouldIssueTokenPairAndPersistOnlyRefreshTokenHash()
    {
        User user = User.Create("ana@example.com", "Ana", "Lopez", "hashed:password");
        var refreshTokens = new FakeRefreshTokenRepository();
        var unitOfWork = new FakeUnitOfWork();
        var handler = new LoginCommandHandler(
            new FakeUserRepository(user),
            refreshTokens,
            new FakePasswordHasher(),
            new FakeTokenProvider(UtcNow),
            unitOfWork);

        Result<AuthenticationResponse> result = await handler.Handle(
            new LoginCommand(" ANA@EXAMPLE.COM ", "password"),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.TokenType.Should().Be("Bearer");
        result.Value.AccessToken.Should().Be($"access-token:{user.Id}");
        result.Value.RefreshToken.Should().Be("refresh-token-1");
        RefreshTokenEntity persisted = refreshTokens.RefreshTokens.Should().ContainSingle().Subject;
        persisted.TokenHash.Should().Be("hash:refresh-token-1");
        persisted.TokenHash.Should().NotBe(result.Value.RefreshToken);
        unitOfWork.SaveCalls.Should().Be(1);
    }

    [Fact]
    public async Task Login_ShouldReturnSameUnauthorizedErrorForUnknownEmailAndWrongPassword()
    {
        User user = User.Create("ana@example.com", "Ana", "Lopez", "hashed:password");
        var handler = new LoginCommandHandler(
            new FakeUserRepository(user),
            new FakeRefreshTokenRepository(),
            new FakePasswordHasher(),
            new FakeTokenProvider(UtcNow),
            new FakeUnitOfWork());

        Result<AuthenticationResponse> unknownEmail = await handler.Handle(
            new LoginCommand("other@example.com", "password"),
            CancellationToken.None);
        Result<AuthenticationResponse> wrongPassword = await handler.Handle(
            new LoginCommand("ana@example.com", "wrong"),
            CancellationToken.None);

        unknownEmail.Error.Should().Be(AuthenticationErrors.InvalidCredentials);
        wrongPassword.Error.Should().Be(AuthenticationErrors.InvalidCredentials);
        unknownEmail.Error.Type.Should().Be(ErrorType.Unauthorized);
    }

    [Fact]
    public async Task RefreshToken_ShouldRotateRefreshTokenAndIssueNewPair()
    {
        User user = User.Create("ana@example.com", "Ana", "Lopez", "hashed:password");
        Guid familyId = Guid.NewGuid();
        RefreshTokenEntity current = RefreshTokenEntity.Create(
            user.Id,
            familyId,
            "hash:old-token",
            UtcNow.AddDays(-1),
            UtcNow.AddDays(6)).Value;
        var refreshTokens = new FakeRefreshTokenRepository(current);
        var unitOfWork = new FakeUnitOfWork();
        var handler = new RefreshTokenCommandHandler(
            refreshTokens,
            new FakeUserRepository(user),
            new FakeTokenProvider(UtcNow),
            new FakeTimeProvider(new DateTimeOffset(UtcNow)),
            unitOfWork);

        Result<AuthenticationResponse> result = await handler.Handle(
            new RefreshTokenCommand("old-token"),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        current.RevokedAtUtc.Should().Be(UtcNow);
        current.ReplacedByTokenId.Should().NotBeNull();
        RefreshTokenEntity replacement = refreshTokens.RefreshTokens.Single(token => token.Id != current.Id);
        replacement.Id.Should().Be(current.ReplacedByTokenId!.Value);
        replacement.FamilyId.Should().Be(familyId);
        result.Value.RefreshToken.Should().Be("refresh-token-1");
        unitOfWork.SaveCalls.Should().Be(1);
    }

    [Fact]
    public async Task RefreshToken_ShouldRevokeActiveFamily_WhenRotatedTokenIsReused()
    {
        User user = User.Create("ana@example.com", "Ana", "Lopez", "hashed:password");
        Guid familyId = Guid.NewGuid();
        RefreshTokenEntity compromised = RefreshTokenEntity.Create(
            user.Id,
            familyId,
            "hash:compromised-token",
            UtcNow.AddDays(-1),
            UtcNow.AddDays(6)).Value;
        RefreshTokenEntity active = RefreshTokenEntity.Create(
            user.Id,
            familyId,
            "hash:active-token",
            UtcNow,
            UtcNow.AddDays(7)).Value;
        compromised.Rotate(active.Id, UtcNow.AddHours(-1));
        var refreshTokens = new FakeRefreshTokenRepository(compromised, active);
        var unitOfWork = new FakeUnitOfWork();
        var handler = new RefreshTokenCommandHandler(
            refreshTokens,
            new FakeUserRepository(user),
            new FakeTokenProvider(UtcNow),
            new FakeTimeProvider(new DateTimeOffset(UtcNow)),
            unitOfWork);

        Result<AuthenticationResponse> result = await handler.Handle(
            new RefreshTokenCommand("compromised-token"),
            CancellationToken.None);

        result.Error.Should().Be(AuthenticationErrors.InvalidRefreshToken);
        active.RevokedAtUtc.Should().Be(UtcNow);
        unitOfWork.SaveCalls.Should().Be(1);
    }
}
