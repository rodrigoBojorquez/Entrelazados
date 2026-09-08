using DatePlanner.Application.Common.Authentication;
using DatePlanner.Domain.Users;
using DatePlanner.Infrastructure;
using FluentAssertions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace DatePlanner.UnitTests.Application;

public sealed class InfrastructureAuthenticationTests
{
    [Fact]
    public async Task TokenProvider_ShouldIssueJwtAcceptedByConfiguredJwtBearerValidation()
    {
        DateTime utcNow = new(2026, 9, 8, 20, 0, 0, DateTimeKind.Utc);
        IConfiguration configuration = CreateConfiguration();
        var services = new ServiceCollection();
        services.AddSingleton(configuration);
        services.AddSingleton<TimeProvider>(new FakeTimeProvider(new DateTimeOffset(utcNow)));
        services.AddInfrastructure(
            configuration,
            "Host=localhost;Database=test;Username=test;Password=test");
        await using ServiceProvider serviceProvider = services.BuildServiceProvider();
        ITokenProvider tokenProvider = serviceProvider.GetRequiredService<ITokenProvider>();
        JwtBearerOptions bearerOptions = serviceProvider
            .GetRequiredService<IOptionsMonitor<JwtBearerOptions>>()
            .Get(JwtBearerDefaults.AuthenticationScheme);
        User user = User.Create("ana@example.com", "Ana", "Lopez", "hash");

        AccessToken accessToken = tokenProvider.GenerateAccessToken(user);
        TokenValidationResult validationResult = await new JsonWebTokenHandler()
            .ValidateTokenAsync(accessToken.Value, bearerOptions.TokenValidationParameters);

        validationResult.IsValid.Should().BeTrue();
        validationResult.ClaimsIdentity.FindFirst("sub")!.Value.Should().Be(user.Id.ToString());
        accessToken.ExpiresAtUtc.Should().Be(utcNow.AddMinutes(60));
    }

    [Fact]
    public void TokenProvider_ShouldIssueDistinctOpaqueRefreshTokensAndStableHashes()
    {
        DateTime utcNow = new(2026, 9, 8, 20, 0, 0, DateTimeKind.Utc);
        IConfiguration configuration = CreateConfiguration();
        var services = new ServiceCollection();
        services.AddSingleton(configuration);
        services.AddSingleton<TimeProvider>(new FakeTimeProvider(new DateTimeOffset(utcNow)));
        services.AddInfrastructure(
            configuration,
            "Host=localhost;Database=test;Username=test;Password=test");
        using ServiceProvider serviceProvider = services.BuildServiceProvider();
        ITokenProvider tokenProvider = serviceProvider.GetRequiredService<ITokenProvider>();

        GeneratedRefreshToken first = tokenProvider.GenerateRefreshToken();
        GeneratedRefreshToken second = tokenProvider.GenerateRefreshToken();

        first.Value.Should().NotBe(second.Value);
        first.Value.Should().NotBe(first.Hash);
        first.Hash.Should().Be(tokenProvider.HashRefreshToken(first.Value));
        first.ExpiresAtUtc.Should().Be(utcNow.AddDays(7));
    }

    private static IConfiguration CreateConfiguration()
    {
        var values = new Dictionary<string, string?>
        {
            ["Authentication:Key"] =
                "2215ad7e96ad93e1df2cc197295bac4dd4ce627e3fa60e8334229952fb7422b1",
            ["Authentication:Issuer"] = "Entrelazados",
            ["Authentication:Audience"] = "Entrelazados",
            ["Authentication:JwtExpireMinutes"] = "60",
            ["Authentication:RefreshTokenExpireDays"] = "7"
        };

        return new ConfigurationBuilder()
            .AddInMemoryCollection(values)
            .Build();
    }
}
