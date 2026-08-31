using Dapper;
using DatePlanner.Application.Common.Data;
using DatePlanner.Infrastructure.Common.Authentication;
using DatePlanner.Infrastructure.Common.Authorization;
using DatePlanner.Infrastructure.Common.Data;
using DatePlanner.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Npgsql;

namespace DatePlanner.Infrastructure;

public static class InfrastructureConfiguration
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration,
        string databaseConnectionString)
    {
        services.AddAuthenticationInternal();

        services.AddAuthorizationInternal();
        
        NpgsqlDataSource npgsqlDataSource = new NpgsqlDataSourceBuilder(databaseConnectionString).Build();
        services.TryAddSingleton(npgsqlDataSource);

        services.TryAddScoped<IDbConnectionFactory, DbConnectionFactory>();

        SqlMapper.AddTypeHandler(new GenericArrayHandler<string>());
        
        services.AddDbContext<DatePlannerDbContext>(options =>
            options
                .UseNpgsql(
                    configuration.GetConnectionString("Database"),
                    npgsqlOptions => npgsqlOptions
                        .MigrationsHistoryTable(HistoryRepository.DefaultTableName, Schemas.DefaultSchema))
                .UseSnakeCaseNamingConvention());

        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<DatePlannerDbContext>());

        // services.AddScoped<IEventRepository, EventRepository>();
        // services.AddScoped<ITicketTypeRepository, TicketTypeRepository>();
        // services.AddScoped<ICategoryRepository, CategoryRepository>();

        return services;
    }
}
