using Dapper;
using DatePlanner.Application.Common.Authentication;
using DatePlanner.Application.Common.Data;
using DatePlanner.Application.Common.Events;
using DatePlanner.Domain.Authentication;
using DatePlanner.Domain.Couples;
using DatePlanner.Domain.Dates;
using DatePlanner.Domain.TurnGames;
using DatePlanner.Domain.Users;
using DatePlanner.Infrastructure.Common.Authentication;
using DatePlanner.Infrastructure.Common.Data;
using DatePlanner.Infrastructure.Common.Events;
using DatePlanner.Infrastructure.Database;
using DatePlanner.Infrastructure.Authentication;
using DatePlanner.Infrastructure.DatePlans;
using DatePlanner.Infrastructure.TurnGames;
using DatePlanner.Infrastructure.Users;
using DatePlanner.Infrastructure.Couples;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Npgsql;
using Quartz;

namespace DatePlanner.Infrastructure;

public static class InfrastructureConfiguration
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration,
        string databaseConnectionString)
    {
        services.AddAuthenticationInternal();

        NpgsqlDataSource npgsqlDataSource = new NpgsqlDataSourceBuilder(databaseConnectionString).Build();
        services.TryAddSingleton(npgsqlDataSource);

        services.TryAddScoped<IDbConnectionFactory, DbConnectionFactory>();
        services.TryAddScoped<IPasswordHasher, PasswordHasher>();
        services.TryAddScoped<IDomainEventsDispatcher, DomainEventsDispatcher>();
        services.TryAddSingleton(TimeProvider.System);

        SqlMapper.AddTypeHandler(new GenericArrayHandler<string>());
        
        services.AddDbContext<DatePlannerDbContext>(options =>
            options
                .UseNpgsql(
                    configuration.GetConnectionString("Database"),
                    npgsqlOptions => npgsqlOptions
                        .MigrationsHistoryTable(HistoryRepository.DefaultTableName, Schemas.DefaultSchema))
                .UseSnakeCaseNamingConvention());

        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<DatePlannerDbContext>());
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<ICoupleRepository, CoupleRepository>();
        services.AddScoped<IDatePlanRepository, DatePlanRepository>();
        services.AddScoped<ITurnGameRepository, TurnGameRepository>();
        services.AddScoped<DatePlanAttendanceProcessor>();

        var attendanceJobKey = new JobKey(nameof(DatePlanAttendanceJob));
        services.AddQuartz(quartz =>
        {
            quartz.AddJob<DatePlanAttendanceJob>(options => options.WithIdentity(attendanceJobKey));
            quartz.AddTrigger(options => options
                .ForJob(attendanceJobKey)
                .WithIdentity($"{nameof(DatePlanAttendanceJob)}-daily")
                .WithCronSchedule("0 5 0 * * ?"));
        });
        services.AddQuartzHostedService(options => options.WaitForJobsToComplete = true);

        return services;
    }
}
