using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using DatePlanner.Infrastructure.Database;
using DatePlanner.Application.Common.Notifications;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Testcontainers.PostgreSql;

namespace DatePlanner.IntegrationTests.Abstractions;

public sealed class IntegrationTestWebAppFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private const string AuthenticationKey =
        "2215ad7e96ad93e1df2cc197295bac4dd4ce627e3fa60e8334229952fb7422b1";

    private readonly PostgreSqlContainer _dbContainer = new PostgreSqlBuilder("postgres:18")
        .WithDatabase("dateplanner_db")
        .WithUsername("postgres")
        .WithPassword("postgres")
        .Build();
    
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("IntegrationTests");
        builder.ConfigureServices(services =>
        {
            services.RemoveAll<ITurnGameNotifier>();
            services.AddSingleton<RecordingTurnGameNotifier>();
            services.AddSingleton<ITurnGameNotifier>(provider =>
                provider.GetRequiredService<RecordingTurnGameNotifier>());
        });
    }
    
    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();
        SetEnvironmentVariables();

        using IServiceScope scope = Services.CreateScope();
        DatePlannerDbContext dbContext =
            scope.ServiceProvider.GetRequiredService<DatePlannerDbContext>();

        await dbContext.Database.MigrateAsync();
    }

    async Task IAsyncLifetime.DisposeAsync()
    {
        await _dbContainer.DisposeAsync();
        await base.DisposeAsync();
        ClearEnvironmentVariables();
    }

    private void SetEnvironmentVariables()
    {
        Environment.SetEnvironmentVariable(
            "ConnectionStrings__Database",
            _dbContainer.GetConnectionString());
        Environment.SetEnvironmentVariable("Authentication__Key", AuthenticationKey);
        Environment.SetEnvironmentVariable(
            "Authentication__Issuer",
            "DatePlanner.IntegrationTests");
        Environment.SetEnvironmentVariable(
            "Authentication__Audience",
            "DatePlanner.IntegrationTests");
        Environment.SetEnvironmentVariable("Authentication__JwtExpireMinutes", "60");
        Environment.SetEnvironmentVariable("Authentication__RefreshTokenExpireDays", "7");
    }

    private static void ClearEnvironmentVariables()
    {
        Environment.SetEnvironmentVariable("ConnectionStrings__Database", null);
        Environment.SetEnvironmentVariable("Authentication__Key", null);
        Environment.SetEnvironmentVariable("Authentication__Issuer", null);
        Environment.SetEnvironmentVariable("Authentication__Audience", null);
        Environment.SetEnvironmentVariable("Authentication__JwtExpireMinutes", null);
        Environment.SetEnvironmentVariable("Authentication__RefreshTokenExpireDays", null);
    }
}
