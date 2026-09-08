using DatePlanner.Application.Authentication;
using DatePlanner.Application.Authentication.GetActiveSessions;
using DatePlanner.Application.Authentication.Login;
using DatePlanner.Application.Authentication.RefreshToken;
using DatePlanner.Application.Authentication.RevokeAllRefreshTokens;
using DatePlanner.Application.Authentication.RevokeRefreshToken;
using DatePlanner.Domain.Abstractions;
using DatePlanner.IntegrationTests.Abstractions;
using FluentAssertions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace DatePlanner.IntegrationTests.Authentication;

public sealed class AuthenticationTests(IntegrationTestWebAppFactory factory)
    : BaseIntegrationTest(factory)
{
    [Fact]
    public async Task Login_ShouldIssueJwtAcceptedByBearerAndCreateActiveSession()
    {
        RegisteredUser user = await RegisterUserAsync();

        Result<AuthenticationResponse> loginResult = await LoginAsync(user);

        loginResult.IsSuccess.Should().BeTrue();
        loginResult.Value.TokenType.Should().Be("Bearer");
        loginResult.Value.AccessToken.Should().NotBeNullOrWhiteSpace();
        loginResult.Value.RefreshToken.Should().NotBeNullOrWhiteSpace();
        loginResult.Value.RefreshToken.Should().NotBe(loginResult.Value.AccessToken);

        JwtBearerOptions bearerOptions = Services
            .GetRequiredService<IOptionsMonitor<JwtBearerOptions>>()
            .Get(JwtBearerDefaults.AuthenticationScheme);
        TokenValidationResult validationResult = await new JsonWebTokenHandler()
            .ValidateTokenAsync(
                loginResult.Value.AccessToken,
                bearerOptions.TokenValidationParameters);

        validationResult.IsValid.Should().BeTrue();
        validationResult.ClaimsIdentity.FindFirst("sub")!.Value.Should().Be(user.Id.ToString());

        Result<IReadOnlyCollection<ActiveSessionResponse>> sessionsResult =
            await Sender.Send(new GetActiveSessionsQuery(user.Id));

        sessionsResult.IsSuccess.Should().BeTrue();
        sessionsResult.Value.Should().ContainSingle();
    }

    [Fact]
    public async Task Login_ShouldReturnUnauthorizedFailure_WhenPasswordIsIncorrect()
    {
        RegisteredUser user = await RegisterUserAsync();

        Result<AuthenticationResponse> result = await Sender.Send(
            new LoginCommand(user.Email, "IncorrectPassword123!"));

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Authentication.InvalidCredentials");
        result.Error.Type.Should().Be(ErrorType.Unauthorized);
    }

    [Fact]
    public async Task RefreshToken_ShouldRotateTokenAndRevokeFamily_WhenOldTokenIsReused()
    {
        RegisteredUser user = await RegisterUserAsync();
        Result<AuthenticationResponse> loginResult = await LoginAsync(user);

        Result<AuthenticationResponse> refreshResult = await Sender.Send(
            new RefreshTokenCommand(loginResult.Value.RefreshToken));

        refreshResult.IsSuccess.Should().BeTrue();
        refreshResult.Value.RefreshToken.Should().NotBe(loginResult.Value.RefreshToken);

        Result<AuthenticationResponse> reuseResult = await Sender.Send(
            new RefreshTokenCommand(loginResult.Value.RefreshToken));
        Result<IReadOnlyCollection<ActiveSessionResponse>> sessionsResult =
            await Sender.Send(new GetActiveSessionsQuery(user.Id));

        reuseResult.IsFailure.Should().BeTrue();
        reuseResult.Error.Code.Should().Be("Authentication.InvalidRefreshToken");
        sessionsResult.Value.Should().BeEmpty();
    }

    [Fact]
    public async Task RevokeRefreshToken_ShouldEndOnlySpecifiedSession()
    {
        RegisteredUser user = await RegisterUserAsync();
        Result<AuthenticationResponse> firstLogin = await LoginAsync(user);
        Result<AuthenticationResponse> secondLogin = await LoginAsync(user);

        Result revokeResult = await Sender.Send(
            new RevokeRefreshTokenCommand(firstLogin.Value.RefreshToken));
        Result<AuthenticationResponse> revokedRefreshResult = await Sender.Send(
            new RefreshTokenCommand(firstLogin.Value.RefreshToken));
        Result<IReadOnlyCollection<ActiveSessionResponse>> sessionsResult =
            await Sender.Send(new GetActiveSessionsQuery(user.Id));

        revokeResult.IsSuccess.Should().BeTrue();
        revokedRefreshResult.IsFailure.Should().BeTrue();
        sessionsResult.Value.Should().ContainSingle();

        Result<AuthenticationResponse> activeRefreshResult = await Sender.Send(
            new RefreshTokenCommand(secondLogin.Value.RefreshToken));
        activeRefreshResult.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task RevokeAllRefreshTokens_ShouldEndEveryUserSession()
    {
        RegisteredUser user = await RegisterUserAsync();
        await LoginAsync(user);
        await LoginAsync(user);

        Result<IReadOnlyCollection<ActiveSessionResponse>> beforeRevoke =
            await Sender.Send(new GetActiveSessionsQuery(user.Id));
        Result revokeResult = await Sender.Send(
            new RevokeAllRefreshTokensCommand(user.Id));
        Result<IReadOnlyCollection<ActiveSessionResponse>> afterRevoke =
            await Sender.Send(new GetActiveSessionsQuery(user.Id));

        beforeRevoke.Value.Should().HaveCount(2);
        revokeResult.IsSuccess.Should().BeTrue();
        afterRevoke.Value.Should().BeEmpty();
    }

    private Task<Result<AuthenticationResponse>> LoginAsync(RegisteredUser user) =>
        Sender.Send(new LoginCommand(user.Email, user.Password));
}
